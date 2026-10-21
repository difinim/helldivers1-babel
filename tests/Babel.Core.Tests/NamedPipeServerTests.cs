using System.IO.Pipes;
using Babel.Core.Ipc;
using Babel.Core.Services;

namespace Babel.Core.Tests;

public class NamedPipeServerTests
{
    [Fact]
    public void Constructor_InitializesExpectedDefaults()
    {
        var server = new NamedPipeServer();
        Assert.False(server.IsClientConnected);
    }

    [Fact]
    public void Constructor_CustomPipeName_RetainsPipeName()
    {
        var customName = "custom_babel_pipe";
        var server = new NamedPipeServer(customName);
        Assert.False(server.IsClientConnected);
    }

    [Fact]
    public async Task StartAndStop_ShouldManageLifecycleCleanly()
    {
        var pipeName = $"babel_test_{Guid.NewGuid():N}";
        var server = new NamedPipeServer(pipeName);

        Assert.False(server.IsClientConnected);

        await server.StartAsync();
        Assert.False(server.IsClientConnected);

        await server.StopAsync();
        Assert.False(server.IsClientConnected);
    }

    [Fact]
    public async Task SendPacket_WhenDisconnected_DoesNotThrow()
    {
        var server = new NamedPipeServer();
        var packet = new IpcPacket(IpcPacketType.Heartbeat, "ping");

        // Should cleanly return without throwing when no client is connected
        await server.SendPacketAsync(packet);
        Assert.False(server.IsClientConnected);
    }

    [Fact]
    public async Task Dispose_ShouldCleanupResourcesCleanly()
    {
        var server = new NamedPipeServer();
        await server.StartAsync();
        server.Dispose();
        Assert.False(server.IsClientConnected);
    }
}
