using Babel.Core.Interfaces;
using Babel.Core.Ipc;
using Babel.Core.Models;
using Babel.Core.Services;

namespace Babel.Core.Tests;

public class MockIpcServer : IIpcServer
{
    public bool IsClientConnected { get; set; } = true;
    public List<IpcPacket> SentPackets { get; } = new();

    public event Action<IpcPacket>? PacketReceived;
    public event Action? ClientConnected;
    public event Action? ClientDisconnected;

    public Task StartAsync(CancellationToken ct = default) => Task.CompletedTask;
    public Task StopAsync() => Task.CompletedTask;

    public Task SendPacketAsync(IpcPacket packet, CancellationToken ct = default)
    {
        SentPackets.Add(packet);
        return Task.CompletedTask;
    }

    public void TriggerPacketReceived(IpcPacket packet) => PacketReceived?.Invoke(packet);
    public void TriggerClientConnected() => ClientConnected?.Invoke();
    public void TriggerClientDisconnected() => ClientDisconnected?.Invoke();
}

public class MockTranslationProvider : ITranslationProvider
{
    public string ProviderId => "Mock";
    public string DisplayName => "Mock Provider";

    public List<TranslationRequest> ReceivedRequests { get; } = new();
    public Func<TranslationRequest, BabelConfig, TranslationResult>? TranslateFunc { get; set; }

    public Task<TranslationResult> TranslateAsync(TranslationRequest request, BabelConfig config, CancellationToken ct = default)
    {
        ReceivedRequests.Add(request);
        if (TranslateFunc != null)
        {
            return Task.FromResult(TranslateFunc(request, config));
        }

        return Task.FromResult(TranslationResult.Ok($"Translated: {request.Text}", "Russian", 50));
    }

    public Task<bool> ValidateCredentialsAsync(BabelConfig config, CancellationToken ct = default) => Task.FromResult(true);
}

public class TranslationEngineTests
{
    [Fact]
    public async Task IncomingMessage_WhenEnabledAndNotPaused_ShouldTranslateAndInject()
    {
        var mockIpc = new MockIpcServer();
        var mockProvider = new MockTranslationProvider
        {
            TranslateFunc = (req, cfg) => TranslationResult.Ok("(Russian): Hello team", "Russian", 120)
        };
        var config = new BabelConfig
        {
            IncomingTranslationEnabled = true,
            TargetLanguage = "Portuguese (PT-BR)",
            SoundAlertEnabled = true
        };

        using var engine = new TranslationEngine(mockIpc, config, mockProvider);

        TranslationRequest? startedRequest = null;
        ChatMessage? translatedChatMessage = null;
        TranslationResult? translatedResult = null;

        engine.TranslationStarted += req => startedRequest = req;
        engine.MessageTranslated += (msg, res) =>
        {
            translatedChatMessage = msg;
            translatedResult = res;
        };

        var incomingPayload = new IncomingChatMessagePayload
        {
            PeerId = "peer123",
            PlayerIndex = 2,
            PlayerName = "Boris",
            R = 80,
            G = 160,
            B = 255,
            Message = "Привет команда"
        };
        var packet = IpcPacket.Create(IpcPacketType.ChatMessageIncoming, incomingPayload);

        await engine.HandlePacketAsync(packet);

        Assert.NotNull(startedRequest);
        Assert.Equal("Привет команда", startedRequest.Text);
        Assert.Equal("Boris", startedRequest.PlayerName);

        Assert.NotNull(translatedChatMessage);
        Assert.Equal("Boris", translatedChatMessage.PlayerName);
        Assert.Equal("Привет команда", translatedChatMessage.MessageText);
        Assert.Equal("(Russian): Hello team", translatedChatMessage.ShortText);
        Assert.False(translatedChatMessage.IsOutgoing);

        Assert.NotNull(translatedResult);
        Assert.True(translatedResult.Success);
        Assert.Equal("Russian", engine.LastIncomingLanguage);

        // Verify InjectChatMessage packet was sent to pipe
        var injectPacket = mockIpc.SentPackets.FirstOrDefault(p => p.Type == IpcPacketType.InjectChatMessage);
        Assert.NotNull(injectPacket);
        var injectPayload = injectPacket.DeserializePayload<InjectChatMessagePayload>();
        Assert.NotNull(injectPayload);
        Assert.Equal("Boris", injectPayload.PlayerName);
        Assert.Equal("(Russian): Hello team", injectPayload.Message);
        Assert.Equal(80, injectPayload.R);
        Assert.Equal(160, injectPayload.G);
        Assert.Equal(255, injectPayload.B);
        Assert.True(injectPayload.PlaySound);
    }

    [Fact]
    public async Task IncomingMessage_WhenPaused_ShouldNotTranslateOrInject()
    {
        var mockIpc = new MockIpcServer();
        var mockProvider = new MockTranslationProvider();
        var config = new BabelConfig { IncomingTranslationEnabled = true };

        using var engine = new TranslationEngine(mockIpc, config, mockProvider);
        engine.SetPaused(true);

        var incomingPayload = new IncomingChatMessagePayload
        {
            PlayerName = "Boris",
            Message = "Привет"
        };
        var packet = IpcPacket.Create(IpcPacketType.ChatMessageIncoming, incomingPayload);

        await engine.HandlePacketAsync(packet);

        Assert.Empty(mockProvider.ReceivedRequests);
        Assert.DoesNotContain(mockIpc.SentPackets, p => p.Type == IpcPacketType.InjectChatMessage);
    }

    [Fact]
    public async Task IncomingMessage_WhenDisabled_ShouldNotTranslateOrInject()
    {
        var mockIpc = new MockIpcServer();
        var mockProvider = new MockTranslationProvider();
        var config = new BabelConfig { IncomingTranslationEnabled = false };

        using var engine = new TranslationEngine(mockIpc, config, mockProvider);

        var incomingPayload = new IncomingChatMessagePayload
        {
            PlayerName = "Boris",
            Message = "Привет"
        };
        var packet = IpcPacket.Create(IpcPacketType.ChatMessageIncoming, incomingPayload);

        await engine.HandlePacketAsync(packet);

        Assert.Empty(mockProvider.ReceivedRequests);
        Assert.DoesNotContain(mockIpc.SentPackets, p => p.Type == IpcPacketType.InjectChatMessage);
    }

    [Fact]
    public async Task OutgoingMessage_WithAutoReply_ShouldUseLastIncomingLanguage()
    {
        var mockIpc = new MockIpcServer();
        var mockProvider = new MockTranslationProvider
        {
            TranslateFunc = (req, cfg) => TranslationResult.Ok($"[RU]: {req.Text}", req.TargetLanguage, 90)
        };
        var config = new BabelConfig { OutgoingTranslationEnabled = true, OutgoingPrefix = ">>" };

        using var engine = new TranslationEngine(mockIpc, config, mockProvider)
        {
            LastIncomingLanguage = "Russian"
        };

        ChatMessage? translatedChatMessage = null;
        engine.MessageTranslated += (msg, res) => translatedChatMessage = msg;

        var outgoingPayload = new OutgoingChatMessagePayload
        {
            PlayerIndex = 1,
            PlayerName = "Helldiver",
            Message = ">>Need help at extraction"
        };
        var packet = IpcPacket.Create(IpcPacketType.ChatMessageOutgoing, outgoingPayload);

        await engine.HandlePacketAsync(packet);

        Assert.Single(mockProvider.ReceivedRequests);
        var req = mockProvider.ReceivedRequests[0];
        Assert.Equal("Need help at extraction", req.Text);
        Assert.Equal("Russian", req.TargetLanguage);
        Assert.True(req.IsOutgoing);

        Assert.NotNull(translatedChatMessage);
        Assert.True(translatedChatMessage.IsOutgoing);

        var broadcastPacket = mockIpc.SentPackets.FirstOrDefault(p => p.Type == IpcPacketType.ExecuteChatBroadcast);
        Assert.NotNull(broadcastPacket);
        var broadcastPayload = broadcastPacket.DeserializePayload<ExecuteBroadcastPayload>();
        Assert.NotNull(broadcastPayload);
        Assert.Equal("[RU]: Need help at extraction", broadcastPayload.Message);
        Assert.Equal(1, broadcastPayload.PlayerIndex);
    }

    [Fact]
    public async Task OutgoingMessage_WithExplicitLanguagePrefix_ShouldTranslateToSpecifiedLanguage()
    {
        var mockIpc = new MockIpcServer();
        var mockProvider = new MockTranslationProvider
        {
            TranslateFunc = (req, cfg) => TranslationResult.Ok($"[FR]: {req.Text}", "French", 70)
        };
        var config = new BabelConfig { OutgoingTranslationEnabled = true, OutgoingPrefix = ">>" };

        using var engine = new TranslationEngine(mockIpc, config, mockProvider)
        {
            LastIncomingLanguage = "Russian" // Should be ignored in favor of explicit french>>
        };

        var outgoingPayload = new OutgoingChatMessagePayload
        {
            PlayerIndex = 1,
            PlayerName = "Helldiver",
            Message = "french>>Good game everyone!"
        };
        var packet = IpcPacket.Create(IpcPacketType.ChatMessageOutgoing, outgoingPayload);

        await engine.HandlePacketAsync(packet);

        Assert.Single(mockProvider.ReceivedRequests);
        var req = mockProvider.ReceivedRequests[0];
        Assert.Equal("Good game everyone!", req.Text);
        Assert.Equal("french", req.TargetLanguage);

        var broadcastPacket = mockIpc.SentPackets.FirstOrDefault(p => p.Type == IpcPacketType.ExecuteChatBroadcast);
        Assert.NotNull(broadcastPacket);
        var broadcastPayload = broadcastPacket.DeserializePayload<ExecuteBroadcastPayload>();
        Assert.NotNull(broadcastPayload);
        Assert.Equal("[FR]: Good game everyone!", broadcastPayload.Message);
    }

    [Fact]
    public async Task OutgoingMessage_WhenPaused_ShouldNotTranslateOrBroadcast()
    {
        var mockIpc = new MockIpcServer();
        var mockProvider = new MockTranslationProvider();
        var config = new BabelConfig { OutgoingTranslationEnabled = true };

        using var engine = new TranslationEngine(mockIpc, config, mockProvider);
        engine.TogglePause(); // Pauses

        var outgoingPayload = new OutgoingChatMessagePayload
        {
            PlayerName = "Helldiver",
            Message = ">>Need help"
        };
        var packet = IpcPacket.Create(IpcPacketType.ChatMessageOutgoing, outgoingPayload);

        await engine.HandlePacketAsync(packet);

        Assert.Empty(mockProvider.ReceivedRequests);
        Assert.DoesNotContain(mockIpc.SentPackets, p => p.Type == IpcPacketType.ExecuteChatBroadcast);
    }

    [Fact]
    public async Task TogglePause_ShouldNotifySubscribersAndSendIpcPacket()
    {
        var mockIpc = new MockIpcServer();
        var config = new BabelConfig();
        using var engine = new TranslationEngine(mockIpc, config, new MockTranslationProvider());

        bool? stateReceived = null;
        engine.StateChanged += isPaused => stateReceived = isPaused;

        Assert.False(engine.IsPaused);

        engine.TogglePause();
        Assert.True(engine.IsPaused);
        Assert.True(stateReceived);

        var packet = mockIpc.SentPackets.LastOrDefault(p => p.Type == IpcPacketType.StateUpdate);
        Assert.NotNull(packet);
        var payload = packet.DeserializePayload<StateUpdatePayload>();
        Assert.NotNull(payload);
        Assert.True(payload.IsPaused);

        engine.TogglePause();
        Assert.False(engine.IsPaused);
        Assert.False(stateReceived);
    }

    [Fact]
    public async Task ProviderFailure_ShouldFireTranslationFailedEvent()
    {
        var mockIpc = new MockIpcServer();
        var mockProvider = new MockTranslationProvider
        {
            TranslateFunc = (req, cfg) => TranslationResult.Failed("API Rate limit reached (429)", 20)
        };
        var config = new BabelConfig { IncomingTranslationEnabled = true };
        using var engine = new TranslationEngine(mockIpc, config, mockProvider);

        string? failedErrorMessage = null;
        engine.TranslationFailed += err => failedErrorMessage = err;

        var incomingPayload = new IncomingChatMessagePayload
        {
            PlayerName = "Boris",
            Message = "Привет"
        };
        var packet = IpcPacket.Create(IpcPacketType.ChatMessageIncoming, incomingPayload);

        await engine.HandlePacketAsync(packet);

        Assert.NotNull(failedErrorMessage);
        Assert.Contains("429", failedErrorMessage);
        Assert.DoesNotContain(mockIpc.SentPackets, p => p.Type == IpcPacketType.InjectChatMessage);
    }

    [Theory]
    [InlineData(">>hello", "", "English", "English", "hello")]
    [InlineData(">>hello", "Russian", "English", "Russian", "hello")]
    [InlineData("russian>>hello", "Spanish", "English", "russian", "hello")]
    [InlineData("es>> hola mundo ", "", "English", "es", "hola mundo")]
    [InlineData(">>", "Russian", "English", "Russian", "")]
    [InlineData("", "", "English", "English", "")]
    public void ParseOutgoing_ShouldParseCorrectly(
        string input,
        string lastLang,
        string defaultLang,
        string expectedTargetLang,
        string expectedMessage)
    {
        var (targetLang, msg) = TranslationEngine.ParseOutgoing(input, lastLang, defaultLang);
        Assert.Equal(expectedTargetLang, targetLang);
        Assert.Equal(expectedMessage, msg);
    }

    [Fact]
    public async Task IncomingMessage_MatchingRecentBroadcast_SuppressesSelfTranslation()
    {
        var mockIpc = new MockIpcServer();
        var mockProvider = new MockTranslationProvider();
        var config = new BabelConfig { IncomingTranslationEnabled = true };
        using var engine = new TranslationEngine(mockIpc, config, mockProvider);

        engine.RegisterRecentBroadcast("Custom Broadcast Message");

        var incomingPayload = new IncomingChatMessagePayload
        {
            PlayerIndex = 3,
            PlayerName = "Someone",
            Message = "Custom Broadcast Message"
        };
        var packet = IpcPacket.Create(IpcPacketType.ChatMessageIncoming, incomingPayload);

        await engine.HandlePacketAsync(packet);

        Assert.Empty(mockProvider.ReceivedRequests);
        Assert.DoesNotContain(mockIpc.SentPackets, p => p.Type == IpcPacketType.InjectChatMessage);
    }

    [Fact]
    public async Task IncomingMessage_Player1MatchingQuickChatMacro_SuppressesSelfTranslation()
    {
        var mockIpc = new MockIpcServer();
        var mockProvider = new MockTranslationProvider();
        var config = new BabelConfig { IncomingTranslationEnabled = true };
        using var engine = new TranslationEngine(mockIpc, config, mockProvider);

        var incomingPayload = new IncomingChatMessagePayload
        {
            PlayerIndex = 1,
            PlayerName = "LocalPlayer",
            Message = "Affirmative / Roger that!"
        };
        var packet = IpcPacket.Create(IpcPacketType.ChatMessageIncoming, incomingPayload);

        await engine.HandlePacketAsync(packet);

        Assert.Empty(mockProvider.ReceivedRequests);
        Assert.DoesNotContain(mockIpc.SentPackets, p => p.Type == IpcPacketType.InjectChatMessage);
    }

    [Fact]
    public async Task IncomingMessage_Player2SendingMacroText_TranslatesNormally()
    {
        var mockIpc = new MockIpcServer();
        var mockProvider = new MockTranslationProvider();
        var config = new BabelConfig { IncomingTranslationEnabled = true };
        using var engine = new TranslationEngine(mockIpc, config, mockProvider);

        var incomingPayload = new IncomingChatMessagePayload
        {
            PlayerIndex = 2,
            PlayerName = "RemoteTeammate",
            Message = "Affirmative / Roger that!"
        };
        var packet = IpcPacket.Create(IpcPacketType.ChatMessageIncoming, incomingPayload);

        await engine.HandlePacketAsync(packet);

        Assert.Single(mockProvider.ReceivedRequests);
        Assert.Contains(mockIpc.SentPackets, p => p.Type == IpcPacketType.InjectChatMessage);
    }

    [Fact]
    public async Task OutgoingMessage_RegistersBroadcastAndSuppressesSubsequentEcho()
    {
        var mockIpc = new MockIpcServer();
        var mockProvider = new MockTranslationProvider
        {
            TranslateFunc = (req, cfg) => TranslationResult.Ok("Olá equipe!", "Portuguese (PT-BR)", 80)
        };
        var config = new BabelConfig
        {
            OutgoingTranslationEnabled = true,
            IncomingTranslationEnabled = true
        };
        using var engine = new TranslationEngine(mockIpc, config, mockProvider);

        // 1. Send outgoing message
        var outgoingPayload = new OutgoingChatMessagePayload
        {
            PlayerIndex = 1,
            PlayerName = "LocalPlayer",
            Message = ">> Hello team!"
        };
        await engine.HandlePacketAsync(IpcPacket.Create(IpcPacketType.ChatMessageOutgoing, outgoingPayload));

        // Outgoing broadcast was dispatched
        Assert.Contains(mockIpc.SentPackets, p => p.Type == IpcPacketType.ExecuteChatBroadcast);
        mockProvider.ReceivedRequests.Clear();
        mockIpc.SentPackets.Clear();

        // 2. Game echoes back the broadcast via ChatMessageIncoming
        var echoPayload = new IncomingChatMessagePayload
        {
            PlayerIndex = 1,
            PlayerName = "LocalPlayer",
            Message = "Olá equipe!"
        };
        await engine.HandlePacketAsync(IpcPacket.Create(IpcPacketType.ChatMessageIncoming, echoPayload));

        // Suppressed! No translation request or injection
        Assert.Empty(mockProvider.ReceivedRequests);
        Assert.Empty(mockIpc.SentPackets);
    }

    [Fact]
    public async Task IncomingMessage_WhenFromLocalSenderAndOptionEnabled_ShouldSuppressTranslation()
    {
        var mockIpc = new MockIpcServer();
        var mockProvider = new MockTranslationProvider();
        var config = new BabelConfig
        {
            IncomingTranslationEnabled = true,
            DontTranslateStandardOutgoing = true
        };
        using var engine = new TranslationEngine(mockIpc, config, mockProvider);

        var incomingPayload = new IncomingChatMessagePayload
        {
            PlayerIndex = 1,
            PlayerName = "LocalPlayer",
            Message = "Need ammo at extraction",
            IsLocal = true
        };
        await engine.HandlePacketAsync(IpcPacket.Create(IpcPacketType.ChatMessageIncoming, incomingPayload));

        // Suppressed! No translation request or injection
        Assert.Empty(mockProvider.ReceivedRequests);
        Assert.Empty(mockIpc.SentPackets);
    }

    [Fact]
    public async Task IncomingMessage_WhenFromLocalSenderAndOptionDisabled_ShouldTranslateAndInject()
    {
        var mockIpc = new MockIpcServer();
        var mockProvider = new MockTranslationProvider
        {
            TranslateFunc = (req, cfg) => TranslationResult.Ok("Preciso de munição na extração", "English", 60)
        };
        var config = new BabelConfig
        {
            IncomingTranslationEnabled = true,
            DontTranslateStandardOutgoing = false
        };
        using var engine = new TranslationEngine(mockIpc, config, mockProvider);

        var incomingPayload = new IncomingChatMessagePayload
        {
            PlayerIndex = 1,
            PlayerName = "LocalPlayer",
            Message = "Need ammo at extraction",
            IsLocal = true
        };
        await engine.HandlePacketAsync(IpcPacket.Create(IpcPacketType.ChatMessageIncoming, incomingPayload));

        // Translated and injected because option is disabled
        Assert.Single(mockProvider.ReceivedRequests);
        Assert.Contains(mockIpc.SentPackets, p => p.Type == IpcPacketType.InjectChatMessage);
    }

    [Fact]
    public async Task IncomingMessage_WhenFromTeammate_ShouldTranslateNormallyRegardlessOfOption()
    {
        var mockIpc = new MockIpcServer();
        var mockProvider = new MockTranslationProvider
        {
            TranslateFunc = (req, cfg) => TranslationResult.Ok("Vamos!", "Spanish", 60)
        };
        var config = new BabelConfig
        {
            IncomingTranslationEnabled = true,
            DontTranslateStandardOutgoing = true // Enabled, but message is from teammate
        };
        using var engine = new TranslationEngine(mockIpc, config, mockProvider);

        var incomingPayload = new IncomingChatMessagePayload
        {
            PlayerIndex = 2,
            PlayerName = "TeammateCarlos",
            Message = "¡Vamos!",
            IsLocal = false
        };
        await engine.HandlePacketAsync(IpcPacket.Create(IpcPacketType.ChatMessageIncoming, incomingPayload));

        Assert.Single(mockProvider.ReceivedRequests);
        Assert.Contains(mockIpc.SentPackets, p => p.Type == IpcPacketType.InjectChatMessage);
    }

    [Fact]
    public async Task OutgoingStandardMessage_RegistersRecentBroadcast_AndSuppressesEcho()
    {
        var mockIpc = new MockIpcServer();
        var mockProvider = new MockTranslationProvider();
        var config = new BabelConfig
        {
            OutgoingTranslationEnabled = true,
            IncomingTranslationEnabled = true,
            DontTranslateStandardOutgoing = true
        };
        using var engine = new TranslationEngine(mockIpc, config, mockProvider);

        // 1. Send standard outgoing message (no >>)
        var outgoingPayload = new OutgoingChatMessagePayload
        {
            PlayerIndex = 1,
            PlayerName = "LocalPlayer",
            Message = "Bora extrair pessoal"
        };
        await engine.HandlePacketAsync(IpcPacket.Create(IpcPacketType.ChatMessageOutgoing, outgoingPayload));

        // No outgoing broadcast packet created for standard message (vanilla game handled dispatch)
        Assert.DoesNotContain(mockIpc.SentPackets, p => p.Type == IpcPacketType.ExecuteChatBroadcast);

        // 2. Incoming echo arrives from game
        var echoPayload = new IncomingChatMessagePayload
        {
            PlayerIndex = 1,
            PlayerName = "LocalPlayer",
            Message = "Bora extrair pessoal",
            IsLocal = false // Even if IsLocal was false, recent outbound cache catches it
        };
        await engine.HandlePacketAsync(IpcPacket.Create(IpcPacketType.ChatMessageIncoming, echoPayload));

        // Suppressed! No translation request or injection
        Assert.Empty(mockProvider.ReceivedRequests);
        Assert.Empty(mockIpc.SentPackets);
    }

    [Fact]
    public async Task IncomingMessage_WhenTranslationFails_FiresTranslationFailedEvent()
    {
        var mockIpc = new MockIpcServer();
        var mockProvider = new MockTranslationProvider
        {
            TranslateFunc = (req, cfg) => TranslationResult.Failed("API quota exceeded", 150)
        };
        var config = new BabelConfig
        {
            IncomingTranslationEnabled = true
        };
        using var engine = new TranslationEngine(mockIpc, config, mockProvider);

        string? failedError = null;
        engine.TranslationFailed += error => failedError = error;

        var incomingPayload = new IncomingChatMessagePayload
        {
            PlayerIndex = 2,
            PlayerName = "Teammate",
            Message = "Incoming bugs!",
            IsLocal = false
        };
        await engine.HandlePacketAsync(IpcPacket.Create(IpcPacketType.ChatMessageIncoming, incomingPayload));

        Assert.NotNull(failedError);
        Assert.Equal("API quota exceeded", failedError);
    }
}
