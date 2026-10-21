using Babel.Core.Ipc;

namespace Babel.Core.Interfaces;

public interface IIpcServer
{
    bool IsClientConnected { get; }
    event Action<IpcPacket>? PacketReceived;
    event Action? ClientConnected;
    event Action? ClientDisconnected;

    Task StartAsync(CancellationToken ct = default);
    Task StopAsync();
    Task SendPacketAsync(IpcPacket packet, CancellationToken ct = default);
}
