using Babel.Core.Interfaces;
using Babel.Core.Ipc;
using Babel.Core.Models;

namespace Babel.Core.Services;

public class MacroService : IMacroService
{
    private readonly IIpcServer _ipcServer;
    private readonly BabelConfig _config;
    private readonly Func<bool>? _isPausedFunc;
    private readonly ITranslationEngine? _translationEngine;
    private readonly Action<string>? _onBroadcastSent;
    private readonly Dictionary<int, QuickChatMacro> _macros = new();
    private readonly object _lock = new();
    private readonly object _floodLock = new();
    private DateTime _lastExecutionTime = DateTime.MinValue;

    public event Action<int, string>? MacroExecuted;
    public event Action<bool>? StateChanged;
    private bool _isExplicitlyPaused;

    public bool IsPaused
    {
        get
        {
            if (_isExplicitlyPaused) return true;
            if (_isPausedFunc != null && _isPausedFunc()) return true;
            return false;
        }
    }

    public void SetPaused(bool paused)
    {
        if (_isExplicitlyPaused != paused)
        {
            _isExplicitlyPaused = paused;
            StateChanged?.Invoke(IsPaused);
        }
    }

    public void TogglePause()
    {
        SetPaused(!_isExplicitlyPaused);
    }

    public MacroService(IIpcServer ipcServer, BabelConfig config, Func<bool>? isPausedFunc = null, Action<string>? onBroadcastSent = null)
    {
        _ipcServer = ipcServer ?? throw new ArgumentNullException(nameof(ipcServer));
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _isPausedFunc = isPausedFunc;
        _onBroadcastSent = onBroadcastSent;

        UpdateMacros(_config.QuickChatMacros ?? BabelConfig.CreateDefaultMacros());
    }

    public MacroService(IIpcServer ipcServer, BabelConfig config, ITranslationEngine translationEngine)
        : this(ipcServer, config, null, msg => translationEngine?.RegisterRecentBroadcast(msg))
    {
        _translationEngine = translationEngine;
    }

    public void UpdateMacros(IEnumerable<QuickChatMacro> macros)
    {
        ArgumentNullException.ThrowIfNull(macros);

        lock (_lock)
        {
            _macros.Clear();
            var list = new List<QuickChatMacro>();
            foreach (var m in macros)
            {
                _macros[m.SlotId] = m;
                list.Add(m);
            }
            _config.QuickChatMacros = list;
        }
    }

    public QuickChatMacro? GetMacro(int slotId)
    {
        lock (_lock)
        {
            return _macros.TryGetValue(slotId, out var macro) ? macro : null;
        }
    }

    public IReadOnlyList<QuickChatMacro> GetAllMacros()
    {
        lock (_lock)
        {
            return _macros.Values.OrderBy(m => m.SlotId).ToList();
        }
    }

    public async Task ExecuteMacroAsync(int slotId)
    {
        if (IsPaused)
        {
            BabelLog.Info("MacroService", $"ExecuteMacroAsync(Slot {slotId}) ignored because Quick Chat or system is paused.");
            return;
        }

        lock (_floodLock)
        {
            var now = DateTime.UtcNow;
            if (now - _lastExecutionTime < TimeSpan.FromSeconds(1.0))
            {
                BabelLog.Info("MacroService", $"ExecuteMacroAsync: Slot {slotId} suppressed by anti-flood gate (< 1.0s limit).");
                return;
            }
            _lastExecutionTime = now;
        }

        QuickChatMacro? macro;
        lock (_lock)
        {
            _macros.TryGetValue(slotId, out macro);
        }

        if (macro == null)
        {
            BabelLog.Warn("MacroService", $"ExecuteMacroAsync: No macro defined for Slot {slotId}.");
            return;
        }

        if (!macro.IsEnabled)
        {
            BabelLog.Info("MacroService", $"ExecuteMacroAsync: Slot {slotId} ('{macro.MessageText}') is disabled.");
            return;
        }

        if (string.IsNullOrWhiteSpace(macro.MessageText))
        {
            BabelLog.Warn("MacroService", $"ExecuteMacroAsync: Slot {slotId} message text is empty.");
            return;
        }

        BabelLog.Info("MacroService", $"Executing Macro Slot {slotId} [{macro.Keybinding}]: \"{macro.MessageText}\" (PipeConnected={_ipcServer.IsClientConnected})");

        var broadcastPayload = new ExecuteBroadcastPayload
        {
            PlayerIndex = 1,
            Message = macro.MessageText
        };

        var packet = IpcPacket.Create(IpcPacketType.ExecuteChatBroadcast, broadcastPayload);
        await _ipcServer.SendPacketAsync(packet).ConfigureAwait(false);

        if (_config.SoundAlertEnabled)
        {
            var soundPacket = new IpcPacket(IpcPacketType.PlaySound, "ui_radio_chat");
            await _ipcServer.SendPacketAsync(soundPacket).ConfigureAwait(false);
        }

        if (_translationEngine != null)
        {
            _translationEngine.RegisterRecentBroadcast(macro.MessageText);
        }
        else
        {
            _onBroadcastSent?.Invoke(macro.MessageText);
        }

        MacroExecuted?.Invoke(slotId, macro.MessageText);
    }
}
