using System.Collections.Concurrent;
using Babel.Core.Interfaces;
using Babel.Core.Ipc;
using Babel.Core.Models;
using Babel.Core.Utilities;

namespace Babel.Core.Services;

public class TranslationEngine : ITranslationEngine, IDisposable
{
    private readonly IIpcServer _ipcServer;
    private BabelConfig _config;
    private ITranslationProvider? _provider;
    private Func<string, ITranslationProvider>? _providerFactory;
    private readonly ConcurrentDictionary<string, DateTime> _recentBroadcasts = new(StringComparer.OrdinalIgnoreCase);
    private static readonly TimeSpan BroadcastTtl = TimeSpan.FromSeconds(10.0);

    public bool IsPaused { get; private set; }
    public string LastIncomingLanguage { get; set; } = string.Empty;
    public int LocalPlayerIndex { get; set; } = 1;
    public string LocalPlayerName { get; set; } = string.Empty;

    public BabelConfig Config
    {
        get => _config;
        set => _config = value ?? throw new ArgumentNullException(nameof(value));
    }

    public event Action<TranslationRequest>? TranslationStarted;
    public event Action<ChatMessage, TranslationResult>? MessageTranslated;
    public event Action<string>? TranslationFailed;
    public event Action<TranslationRequest, string>? TranslationRequestFailed;
    public event Action<bool>? StateChanged;

    public TranslationEngine(
        IIpcServer ipcServer,
        BabelConfig config,
        ITranslationProvider? provider = null,
        Func<string, ITranslationProvider>? providerFactory = null)
    {
        _ipcServer = ipcServer ?? throw new ArgumentNullException(nameof(ipcServer));
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _provider = provider;
        _providerFactory = providerFactory;

        _ipcServer.PacketReceived += OnPacketReceived;
        _ipcServer.ClientConnected += OnClientConnected;
    }

    public TranslationEngine(IIpcServer ipcServer, BabelConfig config, ITranslationProvider provider)
        : this(ipcServer, config, provider, null)
    {
    }

    public TranslationEngine(IIpcServer ipcServer, BabelConfig config, Func<string, ITranslationProvider> providerFactory)
        : this(ipcServer, config, null, providerFactory)
    {
    }

    public void SetProvider(ITranslationProvider provider)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    public void SetProviderFactory(Func<string, ITranslationProvider> providerFactory)
    {
        _providerFactory = providerFactory ?? throw new ArgumentNullException(nameof(providerFactory));
    }

    public void TogglePause()
    {
        SetPaused(!IsPaused);
    }

    public void SetPaused(bool paused)
    {
        if (IsPaused != paused)
        {
            IsPaused = paused;
            StateChanged?.Invoke(IsPaused);

            _ = _ipcServer.SendPacketAsync(IpcPacket.Create(IpcPacketType.StateUpdate, new StateUpdatePayload { IsPaused = IsPaused }));
        }
    }

    public void RegisterRecentBroadcast(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        _recentBroadcasts[message.Trim()] = DateTime.UtcNow;
        PruneExpiredBroadcasts();
    }

    private void PruneExpiredBroadcasts()
    {
        var cutoff = DateTime.UtcNow - BroadcastTtl;
        foreach (var kvp in _recentBroadcasts)
        {
            if (kvp.Value < cutoff)
            {
                _recentBroadcasts.TryRemove(kvp.Key, out _);
            }
        }
    }

    private void OnClientConnected()
    {
        // Broadcast current pause state to newly connected client
        _ = _ipcServer.SendPacketAsync(IpcPacket.Create(IpcPacketType.StateUpdate, new StateUpdatePayload { IsPaused = IsPaused }));
    }

    private void OnPacketReceived(IpcPacket packet)
    {
        _ = HandlePacketAsync(packet);
    }

    public async Task HandlePacketAsync(IpcPacket packet, CancellationToken ct = default)
    {
        if (packet == null) return;

        switch (packet.Type)
        {
            case IpcPacketType.ChatMessageIncoming:
                await HandleIncomingMessageAsync(packet, ct).ConfigureAwait(false);
                break;

            case IpcPacketType.ChatMessageOutgoing:
                await HandleOutgoingMessageAsync(packet, ct).ConfigureAwait(false);
                break;
        }
    }

    private async Task HandleIncomingMessageAsync(IpcPacket packet, CancellationToken ct)
    {
        if (IsPaused || !_config.IncomingTranslationEnabled)
        {
            return;
        }

        var payload = packet.DeserializePayload<IncomingChatMessagePayload>();
        if (payload == null || string.IsNullOrWhiteSpace(payload.Message))
        {
            return;
        }

        if (payload.IsLocal)
        {
            LocalPlayerIndex = payload.PlayerIndex;
            if (!string.IsNullOrWhiteSpace(payload.PlayerName))
            {
                LocalPlayerName = payload.PlayerName.Trim();
            }
        }

        PruneExpiredBroadcasts();
        string trimmedMsg = payload.Message.Trim();

        bool isRecentOutbound = _recentBroadcasts.TryGetValue(trimmedMsg, out var timestamp) &&
                                (DateTime.UtcNow - timestamp) <= BroadcastTtl;

        bool isQuickChatMacro = (payload.PlayerIndex == LocalPlayerIndex) &&
                                (_config.QuickChatMacros?.Any(m => m.IsEnabled &&
                                    string.Equals(m.MessageText?.Trim(), trimmedMsg, StringComparison.OrdinalIgnoreCase)) ?? false);

        bool isLocalSender = payload.IsLocal || isRecentOutbound || isQuickChatMacro;

        if (isLocalSender)
        {
            if (_config.DontTranslateStandardOutgoing || isRecentOutbound || isQuickChatMacro)
            {
                BabelLog.Info("TranslationEngine", $"Incoming message '{payload.Message}' matched local sender / outbound broadcast. Suppressing self-translation.");
                return;
            }
        }

        BabelLog.Info("TranslationEngine", $"Incoming chat from P{payload.PlayerIndex} ({payload.PlayerName}): \"{payload.Message}\"");

        var request = new TranslationRequest
        {
            Text = payload.Message,
            SourceLanguage = "auto",
            TargetLanguage = _config.TargetLanguage,
            PlayerIndex = payload.PlayerIndex,
            PlayerName = payload.PlayerName,
            PlayerColor = new ColorDto(payload.R, payload.G, payload.B),
            IsOutgoing = false
        };

        TranslationStarted?.Invoke(request);

        try
        {
            TranslationResult result;
            if (IncomingTranslationHeuristics.TryFastPath(payload.Message, _config.TargetLanguage, out var fastResult) && fastResult != null)
            {
                BabelLog.Info("TranslationEngine", $"Incoming message fast-path matched: \"{payload.Message}\" (0ms, detected: '{fastResult.DetectedSourceLanguage}')");
                result = fastResult;
            }
            else
            {
                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(_config.TimeoutSeconds > 0 ? _config.TimeoutSeconds : 5.0));
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, timeoutCts.Token);

                var provider = GetCurrentProvider();
                result = await provider.TranslateAsync(request, _config, linkedCts.Token).ConfigureAwait(false);

                if (result.Success)
                {
                    result = IncomingTranslationHeuristics.NormalizeIdenticalTranslation(result, payload.Message, _config.TargetLanguage);
                }
            }

            if (result.Success && !string.IsNullOrWhiteSpace(result.TranslatedText))
            {
                BabelLog.Info("TranslationEngine", $"Incoming translation successful ({result.LatencyMs}ms) [{result.DetectedSourceLanguage} -> {_config.TargetLanguage}]: \"{result.TranslatedText}\"");
                UpdateLastIncomingLanguage(result);

                bool isSameLanguage = LanguageMatcher.IsSameLanguage(result.DetectedSourceLanguage, _config.TargetLanguage);
                if (_config.DontTranslateIncomingInTargetLanguage && isSameLanguage)
                {
                    BabelLog.Info("TranslationEngine", $"Incoming message '{payload.Message}' is already in target language '{_config.TargetLanguage}' (detected: '{result.DetectedSourceLanguage}'). Suppressing in-game translation injection.");
                }
                else
                {
                    string formattedText = FormatIncomingTranslation(result);

                    var injectPayload = new InjectChatMessagePayload
                    {
                        PlayerIndex = payload.PlayerIndex,
                        PlayerName = payload.PlayerName,
                        R = payload.R,
                        G = payload.G,
                        B = payload.B,
                        Message = formattedText,
                        PlaySound = _config.SoundAlertEnabled
                    };

                    var injectPacket = IpcPacket.Create(IpcPacketType.InjectChatMessage, injectPayload);
                    await _ipcServer.SendPacketAsync(injectPacket, ct).ConfigureAwait(false);
                }

                var chatMessage = new ChatMessage
                {
                    PeerId = payload.PeerId,
                    PlayerIndex = payload.PlayerIndex,
                    PlayerName = payload.PlayerName,
                    PlayerColor = new ColorDto(payload.R, payload.G, payload.B),
                    MessageText = payload.Message,
                    ShortText = result.TranslatedText,
                    Timestamp = DateTime.UtcNow,
                    IsOutgoing = false
                };

                MessageTranslated?.Invoke(chatMessage, result);
            }
            else
            {
                string error = result.ErrorMessage ?? "Incoming translation failed.";
                BabelLog.Warn("TranslationEngine", $"Incoming translation failed: {error}");
                TranslationFailed?.Invoke(error);
                TranslationRequestFailed?.Invoke(request, error);
            }
        }
        catch (OperationCanceledException)
        {
            string error = "Incoming translation request timed out.";
            BabelLog.Warn("TranslationEngine", error);
            TranslationFailed?.Invoke(error);
            TranslationRequestFailed?.Invoke(request, error);
        }
        catch (Exception ex)
        {
            string error = $"Incoming translation error: {ex.Message}";
            BabelLog.Error("TranslationEngine", error, ex);
            TranslationFailed?.Invoke(error);
            TranslationRequestFailed?.Invoke(request, error);
        }
    }

    private async Task HandleOutgoingMessageAsync(IpcPacket packet, CancellationToken ct)
    {
        if (IsPaused || !_config.OutgoingTranslationEnabled)
        {
            return;
        }

        var payload = packet.DeserializePayload<OutgoingChatMessagePayload>();
        if (payload == null || string.IsNullOrWhiteSpace(payload.Message))
        {
            return;
        }

        LocalPlayerIndex = payload.PlayerIndex;
        if (!string.IsNullOrWhiteSpace(payload.PlayerName))
        {
            LocalPlayerName = payload.PlayerName.Trim();
        }

        string prefix = string.IsNullOrWhiteSpace(_config.OutgoingPrefix) ? ">>" : _config.OutgoingPrefix;
        if (!payload.Message.Contains(prefix, StringComparison.Ordinal))
        {
            RegisterRecentBroadcast(payload.Message);
            return;
        }

        BabelLog.Info("TranslationEngine", $"Outgoing raw message intercepted: \"{payload.Message}\"");

        var (targetLang, textToTranslate) = ParseOutgoing(payload.Message, LastIncomingLanguage, "English", prefix);
        if (string.IsNullOrWhiteSpace(textToTranslate))
        {
            return;
        }

        BabelLog.Info("TranslationEngine", $"Outgoing translation requested: \"{textToTranslate}\" -> Target: {targetLang}");

        var request = new TranslationRequest
        {
            Text = textToTranslate,
            SourceLanguage = "auto",
            TargetLanguage = targetLang,
            PlayerIndex = payload.PlayerIndex,
            PlayerName = payload.PlayerName,
            PlayerColor = ColorDto.FromPlayerIndex(payload.PlayerIndex),
            IsOutgoing = true
        };

        TranslationStarted?.Invoke(request);

        try
        {
            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(_config.TimeoutSeconds > 0 ? _config.TimeoutSeconds : 5.0));
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, timeoutCts.Token);

            var provider = GetCurrentProvider();
            var result = await provider.TranslateAsync(request, _config, linkedCts.Token).ConfigureAwait(false);

            if (result.Success && !string.IsNullOrWhiteSpace(result.TranslatedText))
            {
                BabelLog.Info("TranslationEngine", $"Outgoing translation successful ({result.LatencyMs}ms): \"{result.TranslatedText}\"");

                RegisterRecentBroadcast(result.TranslatedText);

                var broadcastPayload = new ExecuteBroadcastPayload
                {
                    PlayerIndex = payload.PlayerIndex,
                    Message = result.TranslatedText
                };

                var broadcastPacket = IpcPacket.Create(IpcPacketType.ExecuteChatBroadcast, broadcastPayload);
                await _ipcServer.SendPacketAsync(broadcastPacket, ct).ConfigureAwait(false);

                var chatMessage = new ChatMessage
                {
                    PlayerIndex = payload.PlayerIndex,
                    PlayerName = payload.PlayerName,
                    PlayerColor = ColorDto.FromPlayerIndex(payload.PlayerIndex),
                    MessageText = textToTranslate,
                    ShortText = result.TranslatedText,
                    Timestamp = DateTime.UtcNow,
                    IsOutgoing = true
                };

                MessageTranslated?.Invoke(chatMessage, result);
            }
            else
            {
                string error = result.ErrorMessage ?? "Outgoing translation failed.";
                BabelLog.Warn("TranslationEngine", $"Outgoing translation failed: {error}");
                TranslationFailed?.Invoke(error);
                TranslationRequestFailed?.Invoke(request, error);
            }
        }
        catch (OperationCanceledException)
        {
            string error = "Outgoing translation request timed out.";
            BabelLog.Warn("TranslationEngine", error);
            TranslationFailed?.Invoke(error);
            TranslationRequestFailed?.Invoke(request, error);
        }
        catch (Exception ex)
        {
            string error = $"Outgoing translation error: {ex.Message}";
            BabelLog.Error("TranslationEngine", error, ex);
            TranslationFailed?.Invoke(error);
            TranslationRequestFailed?.Invoke(request, error);
        }
    }

    public static (string TargetLanguage, string Message) ParseOutgoing(
        string text,
        string lastIncomingLanguage,
        string defaultLanguage = "English",
        string prefix = ">>")
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return (defaultLanguage, string.Empty);
        }

        int idx = text.IndexOf(prefix, StringComparison.Ordinal);
        if (idx < 0)
        {
            return (defaultLanguage, text.Trim());
        }

        string langPart = text[..idx].Trim();
        string msgPart = text[(idx + prefix.Length)..].Trim();

        string targetLang;
        if (!string.IsNullOrWhiteSpace(langPart))
        {
            targetLang = langPart;
        }
        else if (!string.IsNullOrWhiteSpace(lastIncomingLanguage))
        {
            targetLang = lastIncomingLanguage;
        }
        else
        {
            targetLang = defaultLanguage;
        }

        return (targetLang, msgPart);
    }

    private void UpdateLastIncomingLanguage(TranslationResult result)
    {
        if (!string.IsNullOrWhiteSpace(result.DetectedSourceLanguage))
        {
            LastIncomingLanguage = result.DetectedSourceLanguage.Trim();
            return;
        }

        if (!string.IsNullOrWhiteSpace(result.TranslatedText) && result.TranslatedText.StartsWith('('))
        {
            int closeParen = result.TranslatedText.IndexOf(')');
            if (closeParen > 1)
            {
                string extracted = result.TranslatedText.Substring(1, closeParen - 1).Trim();
                if (!string.IsNullOrWhiteSpace(extracted))
                {
                    LastIncomingLanguage = extracted;
                }
            }
        }
    }

    private static string FormatIncomingTranslation(TranslationResult result)
    {
        if (result.TranslatedText.StartsWith('(') && result.TranslatedText.Contains("):"))
        {
            return result.TranslatedText;
        }

        if (!string.IsNullOrWhiteSpace(result.DetectedSourceLanguage))
        {
            return $"({result.DetectedSourceLanguage}): {result.TranslatedText}";
        }

        return result.TranslatedText;
    }

    private ITranslationProvider GetCurrentProvider()
    {
        if (_providerFactory != null)
        {
            var provider = _providerFactory(_config.ActiveProvider);
            if (provider != null) return provider;
        }

        if (_provider != null)
        {
            return _provider;
        }

        throw new InvalidOperationException($"No translation provider configured for '{_config.ActiveProvider}'.");
    }

    public void Dispose()
    {
        _ipcServer.PacketReceived -= OnPacketReceived;
        _ipcServer.ClientConnected -= OnClientConnected;
        GC.SuppressFinalize(this);
    }
}
