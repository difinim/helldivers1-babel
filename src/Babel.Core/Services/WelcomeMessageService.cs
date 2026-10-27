using System;
using System.Collections.Generic;
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
/// Remembers players in an in-memory sliding cache for 2 hours to avoid duplicate welcomes across
/// mission results, dropship transitions, and lobby re-entries.
/// </summary>
public class WelcomeMessageService : IDisposable
{
    private readonly IIpcServer _ipcServer;
    private readonly BabelConfig _config;
    private readonly Func<bool>? _isPausedFunc;
    private readonly Action<string>? _onBroadcastSent;
    private readonly Func<DateTimeOffset> _utcNowFunc;
    private readonly Dictionary<string, DateTimeOffset> _seenPlayers = new(StringComparer.OrdinalIgnoreCase);
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
    /// Duration to keep player identifiers in memory to suppress duplicate welcome messages.
    /// Defaults to 2 hours. Can be customized in unit tests for deterministic testing.
    /// </summary>
    public TimeSpan PlayerMemoryExpiration { get; set; } = TimeSpan.FromHours(2.0);

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
    /// Number of tracked player identifiers currently stored in memory.
    /// </summary>
    public int TrackedPlayersCount
    {
        get
        {
            lock (_lock)
            {
                return _seenPlayers.Count;
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
        Action<string>? onBroadcastSent = null,
        Func<DateTimeOffset>? utcNowFunc = null)
    {
        _ipcServer = ipcServer ?? throw new ArgumentNullException(nameof(ipcServer));
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _isPausedFunc = isPausedFunc;
        _onBroadcastSent = onBroadcastSent;
        _utcNowFunc = utcNowFunc ?? (() => DateTimeOffset.UtcNow);

        _ipcServer.PacketReceived += OnPacketReceived;
    }

    public WelcomeMessageService(
        IIpcServer ipcServer,
        BabelConfig config,
        ITranslationEngine translationEngine,
        Func<bool>? isPausedFunc = null,
        Func<DateTimeOffset>? utcNowFunc = null)
        : this(ipcServer, config, isPausedFunc, msg => translationEngine?.RegisterRecentBroadcast(msg), utcNowFunc)
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
    /// Checks whether a player identifier is currently remembered in active (unexpired) memory.
    /// </summary>
    public bool IsPlayerInMemory(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier)) return false;

        lock (_lock)
        {
            if (_seenPlayers.TryGetValue(identifier, out var expiresAt))
            {
                return expiresAt > _utcNowFunc();
            }
            return false;
        }
    }

    /// <summary>
    /// Clears all remembered player identifiers from memory.
    /// </summary>
    public void ClearSeenPlayers()
    {
        lock (_lock)
        {
            _seenPlayers.Clear();
        }
    }

    /// <summary>
    /// Evaluates a player join event. If the user is host, the feature is enabled, and not paused,
    /// checks the 2-hour in-memory cache:
    /// - If the player is already known within 2 hours, renews their expiration and suppresses the welcome.
    /// - If the player is a newcomer, records them and schedules a welcome message at the end of the 5-second debounce window.
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

        string identifier = !string.IsNullOrWhiteSpace(payload.PeerId)
            ? payload.PeerId.Trim()
            : payload.PlayerName?.Trim() ?? string.Empty;

        var now = _utcNowFunc();
        CancellationToken linkedToken;

        lock (_lock)
        {
            if (_isDisposed) return;

            // In-memory 2-hour sliding lookup
            if (!string.IsNullOrWhiteSpace(identifier))
            {
                if (_seenPlayers.TryGetValue(identifier, out var expiresAt) && expiresAt > now)
                {
                    // Already in memory within 2 hours: renew expiration and suppress welcome message
                    var newExpires = now + PlayerMemoryExpiration;
                    _seenPlayers[identifier] = newExpires;

                    string reason = $"Player '{payload.PlayerName}' (Id='{identifier}') already seen in memory within active window. Expiration renewed to {newExpires:u}. Suppressing welcome message.";
                    BabelLog.Info("WelcomeMessageService", reason);
                    WelcomeMessageSuppressed?.Invoke(reason);
                    return;
                }

                // Newcomer or expired: record with fresh expiration
                _seenPlayers[identifier] = now + PlayerMemoryExpiration;
            }

            PurgeExpiredPlayersLocked(now);

            if (_isDebounceActive)
            {
                BabelLog.Info("WelcomeMessageService",
                    $"Newcomer guest player '{payload.PlayerName}' (Id='{identifier}') joined during active debounce window. Coalescing into existing broadcast timer.");
                return;
            }

            _isDebounceActive = true;
            _debounceCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            linkedToken = _debounceCts.Token;

            BabelLog.Info("WelcomeMessageService",
                $"Newcomer guest player '{payload.PlayerName}' (Id='{identifier}') joined as host. Starting {DebounceDelay.TotalSeconds:F1}s debounce window.");
        }

        _ = RunDebounceAsync(linkedToken);
    }

    private void PurgeExpiredPlayersLocked(DateTimeOffset now)
    {
        if (_seenPlayers.Count > 100)
        {
            var expired = new List<string>();
            foreach (var kvp in _seenPlayers)
            {
                if (kvp.Value <= now)
                {
                    expired.Add(kvp.Key);
                }
            }
            foreach (var key in expired)
            {
                _seenPlayers.Remove(key);
            }
        }
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
