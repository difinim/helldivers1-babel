using System.Collections.Generic;
using System.Linq;
using Babel.Core.Models;

namespace Babel.Core.Utilities;

/// <summary>
/// Utility for formatting and truncating Quick Chat macro messages for the reference overlay.
/// </summary>
public static class QuickChatFormatter
{
    public const int MaxMessageLength = 30;

    /// <summary>
    /// Truncates a message to at most maxLength (default 30) characters.
    /// Leading and trailing whitespaces are trimmed.
    /// </summary>
    public static string TruncateMessage(string? message, int maxLength = MaxMessageLength)
    {
        if (string.IsNullOrEmpty(message)) return string.Empty;

        var trimmed = message.Trim();
        if (maxLength <= 0) return string.Empty;

        return trimmed.Length <= maxLength ? trimmed : trimmed[..maxLength];
    }

    /// <summary>
    /// Formats a single quick chat item following the pattern:
    /// [<shortcut binded>] <message truncated in 30 chars>
    /// </summary>
    public static string FormatOverlayItem(string? keybinding, string? message)
    {
        var cleanKey = (keybinding ?? string.Empty).Trim().Trim('[', ']');
        var truncated = TruncateMessage(message, MaxMessageLength);
        return $"[{cleanKey}] {truncated}";
    }

    /// <summary>
    /// Formats a QuickChatMacro into the overlay item pattern.
    /// </summary>
    public static string FormatOverlayItem(QuickChatMacro macro)
    {
        if (macro == null) return string.Empty;
        return FormatOverlayItem(macro.Keybinding, macro.MessageText);
    }

    /// <summary>
    /// Returns the formatted list items for all active Quick Chat macros.
    /// A macro is considered active when IsEnabled is true, and it has a non-empty keybinding and message text.
    /// Results are ordered by SlotId.
    /// </summary>
    public static List<string> GetActiveOverlayItems(IEnumerable<QuickChatMacro>? macros)
    {
        if (macros == null) return new List<string>();

        return macros
            .Where(m => m != null && m.IsEnabled && !string.IsNullOrWhiteSpace(m.Keybinding) && !string.IsNullOrWhiteSpace(m.MessageText))
            .OrderBy(m => m.SlotId)
            .Select(FormatOverlayItem)
            .ToList();
    }
}
