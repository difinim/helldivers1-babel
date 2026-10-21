namespace Babel.Core.Models;

public class QuickChatMacro
{
    public int SlotId { get; set; } = 1;
    public string Keybinding { get; set; } = string.Empty;
    public string MessageText { get; set; } = string.Empty;
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
