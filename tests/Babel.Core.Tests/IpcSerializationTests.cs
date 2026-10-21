using Babel.Core.Ipc;

namespace Babel.Core.Tests;

public class IpcSerializationTests
{
    [Fact]
    public void IncomingChatMessage_SerializesAndDeserializes_PreservingUnicodeAndColors()
    {
        var incoming = new IncomingChatMessagePayload
        {
            PeerId = "peer_1234",
            PlayerIndex = 2,
            PlayerName = "Helldiver_日本語",
            R = 80,
            G = 160,
            B = 255,
            Message = "Look out behind you! ⚠️ 小心坦克!"
        };

        var packet = IpcPacket.Create(IpcPacketType.ChatMessageIncoming, incoming);
        var jsonLine = packet.ToJsonLine();

        Assert.EndsWith("\n", jsonLine);

        var restoredPacket = IpcPacket.FromJsonLine(jsonLine);
        Assert.NotNull(restoredPacket);
        Assert.Equal(IpcPacketType.ChatMessageIncoming, restoredPacket.Type);

        var restoredPayload = restoredPacket.DeserializePayload<IncomingChatMessagePayload>();
        Assert.NotNull(restoredPayload);
        Assert.Equal(incoming.PeerId, restoredPayload.PeerId);
        Assert.Equal(incoming.PlayerIndex, restoredPayload.PlayerIndex);
        Assert.Equal(incoming.PlayerName, restoredPayload.PlayerName);
        Assert.Equal(incoming.R, restoredPayload.R);
        Assert.Equal(incoming.G, restoredPayload.G);
        Assert.Equal(incoming.B, restoredPayload.B);
        Assert.Equal(incoming.Message, restoredPayload.Message);
    }

    [Fact]
    public void InjectChatMessage_SerializesAndDeserializes()
    {
        var inject = new InjectChatMessagePayload
        {
            PlayerIndex = 1,
            PlayerName = "Alpha",
            R = 255,
            G = 200,
            B = 0,
            Message = "(Inglês): Cuidado atrás de você!",
            PlaySound = true
        };

        var packet = IpcPacket.Create(IpcPacketType.InjectChatMessage, inject);
        var restored = IpcPacket.FromJsonLine(packet.ToJsonLine());
        Assert.NotNull(restored);

        var payload = restored.DeserializePayload<InjectChatMessagePayload>();
        Assert.NotNull(payload);
        Assert.Equal(inject.Message, payload.Message);
        Assert.True(payload.PlaySound);
    }

    [Fact]
    public void StateUpdate_SerializesAndDeserializes()
    {
        var state = new StateUpdatePayload { IsPaused = true };
        var packet = IpcPacket.Create(IpcPacketType.StateUpdate, state);
        var restored = IpcPacket.FromJsonLine(packet.ToJsonLine());

        Assert.NotNull(restored);
        var payload = restored.DeserializePayload<StateUpdatePayload>();
        Assert.NotNull(payload);
        Assert.True(payload.IsPaused);
    }
}
