namespace Babel.Core.Models;

public record ColorDto(byte R, byte G, byte B, byte A = 255)
{
    public static readonly ColorDto Gold = new(255, 200, 0);       // Player 1
    public static readonly ColorDto Blue = new(80, 160, 255);      // Player 2
    public static readonly ColorDto Purple = new(180, 80, 255);    // Player 3
    public static readonly ColorDto Cyan = new(80, 255, 220);      // Player 4
    public static readonly ColorDto White = new(255, 255, 255);
    public static readonly ColorDto ErrorRed = new(255, 80, 80);

    public static ColorDto FromPlayerIndex(int playerIndex) => playerIndex switch
    {
        1 => Gold,
        2 => Blue,
        3 => Purple,
        4 => Cyan,
        _ => White
    };
}

public class ChatMessage
{
    public string PeerId { get; set; } = string.Empty;
    public int PlayerIndex { get; set; } = 1;
    public string PlayerName { get; set; } = string.Empty;
    public ColorDto PlayerColor { get; set; } = ColorDto.White;
    public string MessageText { get; set; } = string.Empty;
    public string ShortText { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public bool IsOutgoing { get; set; }
}
