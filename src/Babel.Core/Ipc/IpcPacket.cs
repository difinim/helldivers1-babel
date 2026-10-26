using System.Text.Json;
using System.Text.Json.Serialization;
using Babel.Core.Models;

namespace Babel.Core.Ipc;

public enum IpcPacketType
{
    Unknown = 0,
    Heartbeat = 1,
    ChatMessageIncoming = 2,
    ChatMessageOutgoing = 3,
    InjectChatMessage = 4,
    ExecuteChatBroadcast = 5,
    PlaySound = 6,
    StateUpdate = 7,
    PlayerJoined = 8
}

public class IpcPacket
{
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public IpcPacketType Type { get; set; } = IpcPacketType.Unknown;

    [JsonPropertyName("payload")]
    public string Payload { get; set; } = string.Empty;

    public IpcPacket() { }

    public IpcPacket(IpcPacketType type, string payload = "")
    {
        Type = type;
        Payload = payload;
    }

    public static IpcPacket Create<T>(IpcPacketType type, T data)
    {
        return new IpcPacket(type, JsonSerializer.Serialize(data));
    }

    public T? DeserializePayload<T>()
    {
        if (string.IsNullOrWhiteSpace(Payload)) return default;
        return JsonSerializer.Deserialize<T>(Payload);
    }

    public string ToJsonLine()
    {
        return JsonSerializer.Serialize(this) + "\n";
    }

    public static IpcPacket? FromJsonLine(string line)
    {
        if (string.IsNullOrWhiteSpace(line)) return null;
        return JsonSerializer.Deserialize<IpcPacket>(line.Trim());
    }
}

public class IncomingChatMessagePayload
{
    [JsonPropertyName("peer_id")]
    public string PeerId { get; set; } = string.Empty;

    [JsonPropertyName("player_index")]
    public int PlayerIndex { get; set; } = 1;

    [JsonPropertyName("player_name")]
    public string PlayerName { get; set; } = string.Empty;

    [JsonPropertyName("r")]
    public byte R { get; set; } = 255;

    [JsonPropertyName("g")]
    public byte G { get; set; } = 255;

    [JsonPropertyName("b")]
    public byte B { get; set; } = 255;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("is_local")]
    public bool IsLocal { get; set; } = false;
}

public class OutgoingChatMessagePayload
{
    [JsonPropertyName("player_index")]
    public int PlayerIndex { get; set; } = 1;

    [JsonPropertyName("player_name")]
    public string PlayerName { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}

public class InjectChatMessagePayload
{
    [JsonPropertyName("player_index")]
    public int PlayerIndex { get; set; } = 1;

    [JsonPropertyName("player_name")]
    public string PlayerName { get; set; } = string.Empty;

    [JsonPropertyName("r")]
    public byte R { get; set; } = 255;

    [JsonPropertyName("g")]
    public byte G { get; set; } = 255;

    [JsonPropertyName("b")]
    public byte B { get; set; } = 255;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("play_sound")]
    public bool PlaySound { get; set; } = true;
}

public class ExecuteBroadcastPayload
{
    [JsonPropertyName("player_index")]
    public int PlayerIndex { get; set; } = 1;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}

public class StateUpdatePayload
{
    [JsonPropertyName("is_paused")]
    public bool IsPaused { get; set; }
}

public class PlayerJoinedPayload
{
    [JsonPropertyName("peer_id")]
    public string PeerId { get; set; } = string.Empty;

    [JsonPropertyName("player_name")]
    public string PlayerName { get; set; } = string.Empty;

    [JsonPropertyName("is_host")]
    public bool IsHost { get; set; } = false;
}
