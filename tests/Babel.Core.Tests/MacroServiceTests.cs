using Babel.Core.Ipc;
using Babel.Core.Models;
using Babel.Core.Services;

namespace Babel.Core.Tests;

public class MacroServiceTests
{
    [Fact]
    public void Constructor_ShouldInitializeFromConfigMacros()
    {
        var mockIpc = new MockIpcServer();
        var config = new BabelConfig();
        var service = new MacroService(mockIpc, config);

        var macros = service.GetAllMacros();
        Assert.Equal(10, macros.Count);

        var slot1 = service.GetMacro(1);
        Assert.NotNull(slot1);
        Assert.Equal("Affirmative / Roger that!", slot1.MessageText);
    }

    [Fact]
    public async Task ExecuteMacroAsync_WhenEnabled_ShouldSendBroadcastAndFireEvent()
    {
        var mockIpc = new MockIpcServer();
        var config = new BabelConfig { SoundAlertEnabled = true };
        var service = new MacroService(mockIpc, config);

        int executedSlot = 0;
        string executedText = string.Empty;
        service.MacroExecuted += (slot, text) =>
        {
            executedSlot = slot;
            executedText = text;
        };

        await service.ExecuteMacroAsync(1);

        Assert.Equal(1, executedSlot);
        Assert.Equal("Affirmative / Roger that!", executedText);

        var broadcastPacket = mockIpc.SentPackets.FirstOrDefault(p => p.Type == IpcPacketType.ExecuteChatBroadcast);
        Assert.NotNull(broadcastPacket);
        var broadcastPayload = broadcastPacket.DeserializePayload<ExecuteBroadcastPayload>();
        Assert.NotNull(broadcastPayload);
        Assert.Equal("Affirmative / Roger that!", broadcastPayload.Message);
        Assert.Equal(1, broadcastPayload.PlayerIndex);

        var soundPacket = mockIpc.SentPackets.FirstOrDefault(p => p.Type == IpcPacketType.PlaySound);
        Assert.NotNull(soundPacket);
        Assert.Equal("ui_radio_chat", soundPacket.Payload);
    }

    [Fact]
    public async Task ExecuteMacroAsync_WhenPaused_ShouldNotBroadcastOrFireEvent()
    {
        var mockIpc = new MockIpcServer();
        var config = new BabelConfig();
        bool isPaused = true;
        var service = new MacroService(mockIpc, config, () => isPaused);

        bool eventFired = false;
        service.MacroExecuted += (_, _) => eventFired = true;

        await service.ExecuteMacroAsync(1);

        Assert.False(eventFired);
        Assert.Empty(mockIpc.SentPackets);
    }

    [Fact]
    public async Task ExecuteMacroAsync_WhenMacroDisabled_ShouldNotBroadcastOrFireEvent()
    {
        var mockIpc = new MockIpcServer();
        var config = new BabelConfig();
        var service = new MacroService(mockIpc, config);

        service.UpdateMacros(new List<QuickChatMacro>
        {
            new(1, "NumPad1", "Test Message", isEnabled: false)
        });

        bool eventFired = false;
        service.MacroExecuted += (_, _) => eventFired = true;

        await service.ExecuteMacroAsync(1);

        Assert.False(eventFired);
        Assert.Empty(mockIpc.SentPackets);
    }

    [Fact]
    public async Task ExecuteMacroAsync_WhenSlotNotFound_ShouldNotThrowOrBroadcast()
    {
        var mockIpc = new MockIpcServer();
        var config = new BabelConfig();
        var service = new MacroService(mockIpc, config);

        bool eventFired = false;
        service.MacroExecuted += (_, _) => eventFired = true;

        await service.ExecuteMacroAsync(999);

        Assert.False(eventFired);
        Assert.Empty(mockIpc.SentPackets);
    }

    [Fact]
    public async Task ExecuteMacroAsync_CalledTwiceRapidly_ExecutesFirstAndSuppressesSecond()
    {
        var mockIpc = new MockIpcServer();
        var config = new BabelConfig();
        var service = new MacroService(mockIpc, config);

        int executionCount = 0;
        service.MacroExecuted += (_, _) => executionCount++;

        // Rapid successive calls (< 1.0s)
        await service.ExecuteMacroAsync(1);
        await service.ExecuteMacroAsync(1);

        Assert.Equal(1, executionCount);
        Assert.Single(mockIpc.SentPackets, p => p.Type == IpcPacketType.ExecuteChatBroadcast);
    }

    [Fact]
    public async Task ExecuteMacroAsync_AfterOneSecondCooldown_AllowsSubsequentExecution()
    {
        var mockIpc = new MockIpcServer();
        var config = new BabelConfig();
        var service = new MacroService(mockIpc, config);

        int executionCount = 0;
        service.MacroExecuted += (_, _) => executionCount++;

        await service.ExecuteMacroAsync(1);
        await Task.Delay(1050);
        await service.ExecuteMacroAsync(2);

        Assert.Equal(2, executionCount);
        Assert.Equal(2, mockIpc.SentPackets.Count(p => p.Type == IpcPacketType.ExecuteChatBroadcast));
    }

    [Fact]
    public async Task ExecuteMacroAsync_WithCallback_NotifiesBroadcastRegistration()
    {
        var mockIpc = new MockIpcServer();
        var config = new BabelConfig();
        string registeredBroadcast = string.Empty;

        var service = new MacroService(mockIpc, config, null, msg => registeredBroadcast = msg);

        await service.ExecuteMacroAsync(1);

        Assert.Equal("Affirmative / Roger that!", registeredBroadcast);
    }

    [Fact]
    public void SetPaused_ShouldUpdateIsPausedAndFireStateChanged()
    {
        var mockIpc = new MockIpcServer();
        var config = new BabelConfig();
        var service = new MacroService(mockIpc, config);

        bool? stateFired = null;
        service.StateChanged += paused => stateFired = paused;

        Assert.False(service.IsPaused);

        service.SetPaused(true);
        Assert.True(service.IsPaused);
        Assert.True(stateFired);

        service.SetPaused(false);
        Assert.False(service.IsPaused);
        Assert.False(stateFired);
    }

    [Fact]
    public void TogglePause_ShouldInvertPauseState()
    {
        var mockIpc = new MockIpcServer();
        var config = new BabelConfig();
        var service = new MacroService(mockIpc, config);

        Assert.False(service.IsPaused);

        service.TogglePause();
        Assert.True(service.IsPaused);

        service.TogglePause();
        Assert.False(service.IsPaused);
    }

    [Fact]
    public async Task ExecuteMacroAsync_WhenExplicitlyPaused_ShouldNotBroadcastOrFireEvent()
    {
        var mockIpc = new MockIpcServer();
        var config = new BabelConfig();
        var service = new MacroService(mockIpc, config);

        service.SetPaused(true);

        bool eventFired = false;
        service.MacroExecuted += (_, _) => eventFired = true;

        await service.ExecuteMacroAsync(1);

        Assert.False(eventFired);
        Assert.Empty(mockIpc.SentPackets);
    }

    [Fact]
    public async Task ExecuteMacroAsync_WhenUnpausedAfterExplicitPause_ShouldBroadcast()
    {
        var mockIpc = new MockIpcServer();
        var config = new BabelConfig();
        var service = new MacroService(mockIpc, config);

        service.SetPaused(true);
        await service.ExecuteMacroAsync(1);
        Assert.Empty(mockIpc.SentPackets);

        service.SetPaused(false);
        await service.ExecuteMacroAsync(1);
        Assert.Single(mockIpc.SentPackets, p => p.Type == IpcPacketType.ExecuteChatBroadcast);
    }

    [Fact]
    public async Task ExecuteMacroAsync_WithDynamicPlayerIndex_UsesResolvedPlayerIndex()
    {
        var mockIpc = new MockIpcServer();
        var config = new BabelConfig();
        int currentPlayerIndex = 3;
        var service = new MacroService(
            mockIpc,
            config,
            isPausedFunc: null,
            onBroadcastSent: null,
            getPlayerIndexFunc: () => currentPlayerIndex);

        await service.ExecuteMacroAsync(1);

        var broadcastPacket = mockIpc.SentPackets.FirstOrDefault(p => p.Type == IpcPacketType.ExecuteChatBroadcast);
        Assert.NotNull(broadcastPacket);
        var broadcastPayload = broadcastPacket.DeserializePayload<ExecuteBroadcastPayload>();
        Assert.NotNull(broadcastPayload);
        Assert.Equal(3, broadcastPayload.PlayerIndex);
    }
}
