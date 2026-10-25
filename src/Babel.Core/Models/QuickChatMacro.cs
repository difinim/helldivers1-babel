namespace Babel.Core.Models;

public class QuickChatMacro
{
    public const int MaxMessageLength = 300;

    private string _messageText = string.Empty;

    public int SlotId { get; set; } = 1;
    public string Keybinding { get; set; } = string.Empty;

    public string MessageText
    {
        get => _messageText;
        set => _messageText = value != null && value.Length > MaxMessageLength
            ? value[..MaxMessageLength]
            : (value ?? string.Empty);
    }

    public bool IsEnabled { get; set; } = true;

    public QuickChatMacro() { }

    public QuickChatMacro(int slotId, string keybinding, string messageText, bool isEnabled = true)
    {
        SlotId = slotId;
        Keybinding = keybinding;
        MessageText = messageText;
        IsEnabled = isEnabled;
    }
}
