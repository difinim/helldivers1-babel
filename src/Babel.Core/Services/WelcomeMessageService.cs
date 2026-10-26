using System;
using System.Threading;
using System.Threading.Tasks;
using Babel.Core.Interfaces;
using Babel.Core.Ipc;
using Babel.Core.Models;

namespace Babel.Core.Services;

/// <summary>
/// Service managing the automated tactical Welcome Quick Chat message broadcast.
/// Automatically greets guest players when the user is the host, with a 5-second debounce window
/// that coalesces multiple rapid joins (e.g. 2nd, 3rd, and 4th players) into a single broadcast.
/// </summary>
public class WelcomeMessageService : IDisposable
{
    private readonly IIpcServer _ipcServer;
    private readonly BabelConfig _config;
    private readonly Func<bool>? _isPausedFunc;
    private readonly Action<string>? _onBroadcastSent;
    private readonly object _lock = new();

    private bool _isDebounceActive;
    private CancellationTokenSource? _debounceCts;
    private bool _isDisposed;

    /// <summary>
    /// Duration of the debounce window. Defaults to 5.0 seconds.
    /// Can be customized in unit tests for deterministic, instantaneous evaluation.
    /// </summary>
    public TimeSpan DebounceDelay { get; set; } = TimeSpan.FromSeconds(5.0);

    /// <summary>
    /// Indicates whether a debounce window is currently active and coalescing incoming player joins.
    /// </summary>
    public bool IsDebounceActive
    {
        get
        {
            lock (_lock)
            {
                return _isDebounceActive;
            }
        }
    }

    /// <summary>
    /// Raised whenever a welcome message is broadcast to in-game chat.
    /// </summary>
    public event Action<string>? WelcomeMessageSent;

    /// <summary>
    /// Raised whenever a player join event was evaluated but ignored/suppressed with reason.
    /// </summary>
    public event Action<string>? WelcomeMessageSuppressed;

    public WelcomeMessageService(
        IIpcServer ipcServer,
        BabelConfig config,
        Func<bool>? isPausedFunc = null,
        Action<string>? onBroadcastSent = null)
    {
        _ipcServer = ipcServer ?? throw new ArgumentNullException(nameof(ipcServer));
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _isPausedFunc = isPausedFunc;
        _onBroadcastSent = onBroadcastSent;

        _ipcServer.PacketReceived += OnPacketReceived;
    }

    public WelcomeMessageService(
        IIpcServer ipcServer,
        BabelConfig config,
        ITranslationEngine translationEngine,
        Func<bool>? isPausedFunc = null)
        : this(ipcServer, config, isPausedFunc, msg => translationEngine?.RegisterRecentBroadcast(msg))
    {
    }

    private void OnPacketReceived(IpcPacket packet)
    {
        if (packet == null || packet.Type != IpcPacketType.PlayerJoined) return;

        var payload = packet.DeserializePayload<PlayerJoinedPayload>();
        if (payload != null)
        {
            _ = HandlePlayerJoinedAsync(payload);
        }
    }

    /// <summary>
    /// Evaluates a player join event. If the user is host, the feature is enabled, and not paused,
    /// schedules a welcome message at the end of the debounce window. If already in a window, coalesces the event.
    /// </summary>
    public async Task HandlePlayerJoinedAsync(PlayerJoinedPayload payload, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(payload);

        if (!payload.IsHost)
        {
            string reason = $"User is not host (PeerId={payload.PeerId}, PlayerName='{payload.PlayerName}').";
            BabelLog.Info("WelcomeMessageService", $"Welcome message suppressed: {reason}");
            WelcomeMessageSuppressed?.Invoke(reason);
            return;
        }

        if (!_config.WelcomeMessageEnabled)
        {
            string reason = "Welcome Quick Chat feature is disabled in settings.";
            BabelLog.Info("WelcomeMessageService", $"Welcome message suppressed: {reason}");
            WelcomeMessageSuppressed?.Invoke(reason);
            return;
        }

        if (string.IsNullOrWhiteSpace(_config.WelcomeMessage))
        {
            string reason = "Configured welcome message is empty or whitespace.";
            BabelLog.Info("WelcomeMessageService", $"Welcome message suppressed: {reason}");
            WelcomeMessageSuppressed?.Invoke(reason);
            return;
        }

        if (_isPausedFunc != null && _isPausedFunc())
        {
            string reason = "System or Quick Chat is paused.";
            BabelLog.Info("WelcomeMessageService", $"Welcome message suppressed: {reason}");
            WelcomeMessageSuppressed?.Invoke(reason);
            return;
        }

        CancellationToken linkedToken;
        lock (_lock)
        {
            if (_isDisposed) return;

            if (_isDebounceActive)
            {
                BabelLog.Info("WelcomeMessageService",
                    $"Guest player '{payload.PlayerName}' (PeerId={payload.PeerId}) joined during active debounce window. Coalescing into existing broadcast timer.");
                return;
            }

            _isDebounceActive = true;
            _debounceCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            linkedToken = _debounceCts.Token;

            BabelLog.Info("WelcomeMessageService",
                $"Guest player '{payload.PlayerName}' (PeerId={payload.PeerId}) joined as host. Starting {DebounceDelay.TotalSeconds:F1}s debounce window.");
        }

        _ = RunDebounceAsync(linkedToken);
    }

    private async Task RunDebounceAsync(CancellationToken ct)
    {
        try
        {
            await Task.Delay(DebounceDelay, ct).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            lock (_lock)
            {
                _isDebounceActive = false;
            }
            return;
        }

        lock (_lock)
        {
            if (_isDisposed)
            {
                _isDebounceActive = false;
                return;
            }
            _isDebounceActive = false;
        }

        // Re-check pause and enable states before dispatching
        if (_isPausedFunc != null && _isPausedFunc())
        {
            BabelLog.Info("WelcomeMessageService", "Debounce expired, but system/Quick Chat is now paused. Suppressing broadcast.");
            WelcomeMessageSuppressed?.Invoke("System or Quick Chat is paused at dispatch time.");
            return;
        }

        if (!_config.WelcomeMessageEnabled || string.IsNullOrWhiteSpace(_config.WelcomeMessage))
        {
            return;
        }

        await SendWelcomeMessageInternalAsync(ct).ConfigureAwait(false);
    }

    /// <summary>
    /// Explicitly sends the configured welcome message immediately (e.g. for GUI 'Test Send').
    /// </summary>
    public async Task SendWelcomeMessageAsync(CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_config.WelcomeMessage))
        {
            BabelLog.Warn("WelcomeMessageService", "SendWelcomeMessageAsync: Configured welcome message is empty.");
            return;
        }

        await SendWelcomeMessageInternalAsync(ct).ConfigureAwait(false);
    }

    private async Task SendWelcomeMessageInternalAsync(CancellationToken ct)
    {
        string message = _config.WelcomeMessage.Trim();
        if (message.Length > QuickChatMacro.MaxMessageLength)
        {
            message = message[..QuickChatMacro.MaxMessageLength];
        }

        BabelLog.Info("WelcomeMessageService", $"Dispatching Welcome Quick Chat message: \"{message}\" (PipeConnected={_ipcServer.IsClientConnected})");

        var broadcastPayload = new ExecuteBroadcastPayload
        {
            PlayerIndex = 1,
            Message = message
        };

        var packet = IpcPacket.Create(IpcPacketType.ExecuteChatBroadcast, broadcastPayload);
        await _ipcServer.SendPacketAsync(packet, ct).ConfigureAwait(false);

        if (_config.SoundAlertEnabled)
        {
            var soundPacket = new IpcPacket(IpcPacketType.PlaySound, "ui_radio_chat");
            await _ipcServer.SendPacketAsync(soundPacket, ct).ConfigureAwait(false);
        }

        _onBroadcastSent?.Invoke(message);
        WelcomeMessageSent?.Invoke(message);
    }

    public void Dispose()
    {
        lock (_lock)
        {
            if (_isDisposed) return;
            _isDisposed = true;
            _isDebounceActive = false;
            _debounceCts?.Cancel();
            _debounceCts?.Dispose();
            _debounceCts = null;
        }

        _ipcServer.PacketReceived -= OnPacketReceived;
    }
}
