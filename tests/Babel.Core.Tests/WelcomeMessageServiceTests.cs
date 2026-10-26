using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Babel.Core.Ipc;
using Babel.Core.Models;
using Babel.Core.Services;
using Xunit;

namespace Babel.Core.Tests;

public class WelcomeMessageServiceTests
{
    [Fact]
    public async Task HandlePlayerJoinedAsync_WhenNotHost_ShouldSuppressAndNotBroadcast()
    {
        var mockIpc = new MockIpcServer();
        var config = new BabelConfig { WelcomeMessageEnabled = true, WelcomeMessage = "Welcome, Helldiver!" };
        using var service = new WelcomeMessageService(mockIpc, config)
        {
            DebounceDelay = TimeSpan.FromMilliseconds(40)
        };

        string? suppressedReason = null;
        service.WelcomeMessageSuppressed += reason => suppressedReason = reason;

        var payload = new PlayerJoinedPayload
        {
            PeerId = "guest_123",
            PlayerName = "Cadet",
            IsHost = false
        };

        await service.HandlePlayerJoinedAsync(payload);
        await Task.Delay(80);

        Assert.NotNull(suppressedReason);
        Assert.Contains("not host", suppressedReason, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(mockIpc.SentPackets, p => p.Type == IpcPacketType.ExecuteChatBroadcast);
    }

    [Fact]
    public async Task HandlePlayerJoinedAsync_WhenDisabled_ShouldSuppressAndNotBroadcast()
    {
        var mockIpc = new MockIpcServer();
        var config = new BabelConfig { WelcomeMessageEnabled = false, WelcomeMessage = "Welcome, Helldiver!" };
        using var service = new WelcomeMessageService(mockIpc, config)
        {
            DebounceDelay = TimeSpan.FromMilliseconds(40)
        };

        string? suppressedReason = null;
        service.WelcomeMessageSuppressed += reason => suppressedReason = reason;

        var payload = new PlayerJoinedPayload
        {
            PeerId = "guest_123",
            PlayerName = "Cadet",
            IsHost = true
        };

        await service.HandlePlayerJoinedAsync(payload);
        await Task.Delay(80);

        Assert.NotNull(suppressedReason);
        Assert.Contains("disabled", suppressedReason, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(mockIpc.SentPackets, p => p.Type == IpcPacketType.ExecuteChatBroadcast);
    }

    [Fact]
    public async Task HandlePlayerJoinedAsync_WhenMessageEmptyOrWhitespace_ShouldSuppress()
    {
        var mockIpc = new MockIpcServer();
        var config = new BabelConfig { WelcomeMessageEnabled = true, WelcomeMessage = "   " };
        using var service = new WelcomeMessageService(mockIpc, config)
        {
            DebounceDelay = TimeSpan.FromMilliseconds(40)
        };

        string? suppressedReason = null;
        service.WelcomeMessageSuppressed += reason => suppressedReason = reason;

        var payload = new PlayerJoinedPayload
        {
            PeerId = "guest_123",
            PlayerName = "Cadet",
            IsHost = true
        };

        await service.HandlePlayerJoinedAsync(payload);
        await Task.Delay(80);

        Assert.NotNull(suppressedReason);
        Assert.Contains("empty", suppressedReason, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(mockIpc.SentPackets, p => p.Type == IpcPacketType.ExecuteChatBroadcast);
    }

    [Fact]
    public async Task HandlePlayerJoinedAsync_WhenPaused_ShouldSuppress()
    {
        var mockIpc = new MockIpcServer();
        var config = new BabelConfig { WelcomeMessageEnabled = true, WelcomeMessage = "Welcome, Helldiver!" };
        using var service = new WelcomeMessageService(mockIpc, config, isPausedFunc: () => true)
        {
            DebounceDelay = TimeSpan.FromMilliseconds(40)
        };

        string? suppressedReason = null;
        service.WelcomeMessageSuppressed += reason => suppressedReason = reason;

        var payload = new PlayerJoinedPayload
        {
            PeerId = "guest_123",
            PlayerName = "Cadet",
            IsHost = true
        };

        await service.HandlePlayerJoinedAsync(payload);
        await Task.Delay(80);

        Assert.NotNull(suppressedReason);
        Assert.Contains("paused", suppressedReason, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(mockIpc.SentPackets, p => p.Type == IpcPacketType.ExecuteChatBroadcast);
    }

    [Fact]
    public async Task HandlePlayerJoinedAsync_WhenValidHost_ShouldBroadcastAfterDebounceDelay()
    {
        var mockIpc = new MockIpcServer();
        var config = new BabelConfig
        {
            WelcomeMessageEnabled = true,
            WelcomeMessage = "Welcome to Super Earth!",
            SoundAlertEnabled = true
        };

        string? broadcastCallbackMsg = null;
        using var service = new WelcomeMessageService(mockIpc, config, onBroadcastSent: msg => broadcastCallbackMsg = msg)
        {
            DebounceDelay = TimeSpan.FromMilliseconds(50)
        };

        string? sentEventMsg = null;
        service.WelcomeMessageSent += msg => sentEventMsg = msg;

        var payload = new PlayerJoinedPayload
        {
            PeerId = "guest_1",
            PlayerName = "Helldiver_01",
            IsHost = true
        };

        await service.HandlePlayerJoinedAsync(payload);

        // Before debounce expiration, no packet sent yet
        Assert.DoesNotContain(mockIpc.SentPackets, p => p.Type == IpcPacketType.ExecuteChatBroadcast);
        Assert.True(service.IsDebounceActive);

        // Wait for debounce window to elapse
        await Task.Delay(100);

        Assert.False(service.IsDebounceActive);
        Assert.Equal("Welcome to Super Earth!", sentEventMsg);
        Assert.Equal("Welcome to Super Earth!", broadcastCallbackMsg);

        var broadcastPacket = mockIpc.SentPackets.FirstOrDefault(p => p.Type == IpcPacketType.ExecuteChatBroadcast);
        Assert.NotNull(broadcastPacket);
        var broadcastPayload = broadcastPacket.DeserializePayload<ExecuteBroadcastPayload>();
        Assert.NotNull(broadcastPayload);
        Assert.Equal("Welcome to Super Earth!", broadcastPayload.Message);
        Assert.Equal(1, broadcastPayload.PlayerIndex);

        var soundPacket = mockIpc.SentPackets.FirstOrDefault(p => p.Type == IpcPacketType.PlaySound);
        Assert.NotNull(soundPacket);
        Assert.Equal("ui_radio_chat", soundPacket.Payload);
    }

    [Fact]
    public async Task HandlePlayerJoinedAsync_WhenMultiplePlayersJoinInDebounceWindow_ShouldCoalesceToOneBroadcast()
    {
        var mockIpc = new MockIpcServer();
        var config = new BabelConfig
        {
            WelcomeMessageEnabled = true,
            WelcomeMessage = "Welcome to the squad!",
            SoundAlertEnabled = false
        };

        int sentCount = 0;
        using var service = new WelcomeMessageService(mockIpc, config)
        {
            DebounceDelay = TimeSpan.FromMilliseconds(100)
        };
        service.WelcomeMessageSent += _ => sentCount++;

        // Player 2 joins at t=0
        await service.HandlePlayerJoinedAsync(new PlayerJoinedPayload { PeerId = "p2", PlayerName = "Player2", IsHost = true });
        Assert.True(service.IsDebounceActive);

        // Player 3 joins at t=20ms (inside debounce window)
        await Task.Delay(20);
        await service.HandlePlayerJoinedAsync(new PlayerJoinedPayload { PeerId = "p3", PlayerName = "Player3", IsHost = true });

        // Player 4 joins at t=40ms (inside debounce window)
        await Task.Delay(20);
        await service.HandlePlayerJoinedAsync(new PlayerJoinedPayload { PeerId = "p4", PlayerName = "Player4", IsHost = true });

        // Wait for 100ms debounce to expire
        await Task.Delay(120);

        Assert.False(service.IsDebounceActive);
        Assert.Equal(1, sentCount);

        var broadcastPackets = mockIpc.SentPackets.Where(p => p.Type == IpcPacketType.ExecuteChatBroadcast).ToList();
        Assert.Single(broadcastPackets);
    }

    [Fact]
    public async Task HandlePlayerJoinedAsync_WhenNewPlayerJoinsAfterWindowExpires_ShouldSendSecondBroadcast()
    {
        var mockIpc = new MockIpcServer();
        var config = new BabelConfig
        {
            WelcomeMessageEnabled = true,
            WelcomeMessage = "Welcome Helldivers!",
            SoundAlertEnabled = false
        };

        int sentCount = 0;
        using var service = new WelcomeMessageService(mockIpc, config)
        {
            DebounceDelay = TimeSpan.FromMilliseconds(50)
        };
        service.WelcomeMessageSent += _ => sentCount++;

        // First window: Player 2 joins
        await service.HandlePlayerJoinedAsync(new PlayerJoinedPayload { PeerId = "p2", PlayerName = "Player2", IsHost = true });
        await Task.Delay(90);

        Assert.Equal(1, sentCount);
        Assert.False(service.IsDebounceActive);

        // Second window: Player 3 joins after first window ended
        await service.HandlePlayerJoinedAsync(new PlayerJoinedPayload { PeerId = "p3", PlayerName = "Player3", IsHost = true });
        await Task.Delay(90);

        Assert.Equal(2, sentCount);
        var broadcastPackets = mockIpc.SentPackets.Where(p => p.Type == IpcPacketType.ExecuteChatBroadcast).ToList();
        Assert.Equal(2, broadcastPackets.Count);
    }

    [Fact]
    public async Task HandlePlayerJoinedAsync_ClampsMessageTo300Characters()
    {
        string longMessage = new string('W', 350);
        var mockIpc = new MockIpcServer();
        var config = new BabelConfig
        {
            WelcomeMessageEnabled = true,
            WelcomeMessage = longMessage,
            SoundAlertEnabled = false
        };

        string? broadcasted = null;
        using var service = new WelcomeMessageService(mockIpc, config)
        {
            DebounceDelay = TimeSpan.FromMilliseconds(40)
        };
        service.WelcomeMessageSent += msg => broadcasted = msg;

        await service.HandlePlayerJoinedAsync(new PlayerJoinedPayload { PeerId = "p2", PlayerName = "Player2", IsHost = true });
        await Task.Delay(80);

        Assert.NotNull(broadcasted);
        Assert.Equal(300, broadcasted.Length);
        Assert.Equal(300, config.WelcomeMessage.Length);

        var packet = mockIpc.SentPackets.First(p => p.Type == IpcPacketType.ExecuteChatBroadcast);
        var payload = packet.DeserializePayload<ExecuteBroadcastPayload>();
        Assert.NotNull(payload);
        Assert.Equal(300, payload.Message.Length);
    }

    [Fact]
    public async Task TriggerPacketReceived_FromNamedPipe_TriggersDebounceAndBroadcast()
    {
        var mockIpc = new MockIpcServer();
        var config = new BabelConfig
        {
            WelcomeMessageEnabled = true,
            WelcomeMessage = "Greetings!",
            SoundAlertEnabled = false
        };

        using var service = new WelcomeMessageService(mockIpc, config)
        {
            DebounceDelay = TimeSpan.FromMilliseconds(50)
        };

        var joinPayload = new PlayerJoinedPayload
        {
            PeerId = "steam_peer_099",
            PlayerName = "Helldiver_99",
            IsHost = true
        };

        var packet = IpcPacket.Create(IpcPacketType.PlayerJoined, joinPayload);
        mockIpc.TriggerPacketReceived(packet);

        Assert.True(service.IsDebounceActive);

        await Task.Delay(100);

        Assert.False(service.IsDebounceActive);
        var broadcastPacket = mockIpc.SentPackets.FirstOrDefault(p => p.Type == IpcPacketType.ExecuteChatBroadcast);
        Assert.NotNull(broadcastPacket);
    }

    [Fact]
    public async Task SendWelcomeMessageAsync_SendsImmediatelyWithoutDebounceDelay()
    {
        var mockIpc = new MockIpcServer();
        var config = new BabelConfig
        {
            WelcomeMessageEnabled = true,
            WelcomeMessage = "Manual Test Welcome!",
            SoundAlertEnabled = false
        };

        using var service = new WelcomeMessageService(mockIpc, config)
        {
            DebounceDelay = TimeSpan.FromSeconds(60) // Long delay
        };

        await service.SendWelcomeMessageAsync();

        // Should be sent immediately without waiting for the 60s debounce
        var broadcastPacket = mockIpc.SentPackets.FirstOrDefault(p => p.Type == IpcPacketType.ExecuteChatBroadcast);
        Assert.NotNull(broadcastPacket);
        var payload = broadcastPacket.DeserializePayload<ExecuteBroadcastPayload>();
        Assert.NotNull(payload);
        Assert.Equal("Manual Test Welcome!", payload.Message);
    }

    [Fact]
    public async Task Dispose_CancelsPendingDebounceTimer()
    {
        var mockIpc = new MockIpcServer();
        var config = new BabelConfig
        {
            WelcomeMessageEnabled = true,
            WelcomeMessage = "Will not be sent",
            SoundAlertEnabled = false
        };

        var service = new WelcomeMessageService(mockIpc, config)
        {
            DebounceDelay = TimeSpan.FromMilliseconds(80)
        };

        await service.HandlePlayerJoinedAsync(new PlayerJoinedPayload { PeerId = "p2", PlayerName = "Player2", IsHost = true });
        Assert.True(service.IsDebounceActive);

        // Dispose service midway through debounce
        service.Dispose();

        await Task.Delay(120);

        Assert.False(service.IsDebounceActive);
        Assert.DoesNotContain(mockIpc.SentPackets, p => p.Type == IpcPacketType.ExecuteChatBroadcast);
    }
}
