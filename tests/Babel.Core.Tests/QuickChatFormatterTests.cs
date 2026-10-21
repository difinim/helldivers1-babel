using System.Collections.Generic;
using System.Linq;
using Babel.Core.Models;
using Babel.Core.Utilities;
using Xunit;

namespace Babel.Core.Tests;

public class QuickChatFormatterTests
{
    [Fact]
    public void TruncateMessage_Under30Characters_ReturnsOriginalText()
    {
        string message = "Affirmative / Roger that!";
        string result = QuickChatFormatter.TruncateMessage(message);

        Assert.Equal("Affirmative / Roger that!", result);
        Assert.True(result.Length <= 30);
    }

    [Fact]
    public void TruncateMessage_Exact30Characters_ReturnsOriginalText()
    {
        string message = "123456789012345678901234567890";
        Assert.Equal(30, message.Length);

        string result = QuickChatFormatter.TruncateMessage(message);

        Assert.Equal(message, result);
        Assert.Equal(30, result.Length);
    }

    [Theory]
    [InlineData("Watch out! Danger / Heavy enemy!", "Watch out! Danger / Heavy enem")] // 32 chars -> 30
    [InlineData("Clear the area / Incoming stratagem!", "Clear the area / Incoming stra")] // 35 chars -> 30
    [InlineData("Super Earth needs you right now Helldivers!", "Super Earth needs you right no")] // 43 chars -> 30
    public void TruncateMessage_Over30Characters_TruncatesToExact30Characters(string input, string expected)
    {
        string result = QuickChatFormatter.TruncateMessage(input);

        Assert.Equal(expected, result);
        Assert.Equal(30, result.Length);
    }

    [Fact]
    public void TruncateMessage_WithSurroundingWhitespace_TrimsBeforeTruncating()
    {
        string message = "   Hold position / Stay here!   ";
        string result = QuickChatFormatter.TruncateMessage(message);

        Assert.Equal("Hold position / Stay here!", result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void TruncateMessage_NullOrEmpty_ReturnsEmptyString(string? message)
    {
        string result = QuickChatFormatter.TruncateMessage(message);
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void TruncateMessage_CustomMaxLength_TruncatesCorrectly()
    {
        string message = "Helldivers to hellpods";
        string result = QuickChatFormatter.TruncateMessage(message, 10);

        Assert.Equal("Helldivers", result);
        Assert.Equal(10, result.Length);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void TruncateMessage_ZeroOrNegativeLength_ReturnsEmptyString(int maxLength)
    {
        string message = "Helldivers";
        string result = QuickChatFormatter.TruncateMessage(message, maxLength);

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void FormatOverlayItem_StandardInput_FormatsAccordingToPattern()
    {
        string key = "NumPad1";
        string msg = "Affirmative / Roger that!";

        string formatted = QuickChatFormatter.FormatOverlayItem(key, msg);

        Assert.Equal("[NumPad1] Affirmative / Roger that!", formatted);
    }

    [Theory]
    [InlineData("[NumPad2]", "Negative / Cancel that!", "[NumPad2] Negative / Cancel that!")]
    [InlineData("[[NumPad3]]", "Hold position", "[NumPad3] Hold position")]
    [InlineData("  NumPad4  ", "Fall back!", "[NumPad4] Fall back!")]
    [InlineData("Numpad +", "Need ammo", "[Numpad +] Need ammo")]
    [InlineData("[Numpad +]", "Need ammo", "[Numpad +] Need ammo")]
    public void FormatOverlayItem_WithBracketsOrWhitespaceInKeybinding_CleansBracketsConsistently(string key, string msg, string expected)
    {
        string formatted = QuickChatFormatter.FormatOverlayItem(key, msg);
        Assert.Equal(expected, formatted);
    }

    [Fact]
    public void FormatOverlayItem_LongMessage_TruncatesMessageInItemPattern()
    {
        string key = "NumPad7";
        string msg = "Watch out! Danger / Heavy enemy!"; // 32 chars

        string formatted = QuickChatFormatter.FormatOverlayItem(key, msg);

        Assert.Equal("[NumPad7] Watch out! Danger / Heavy enem", formatted);
    }

    [Fact]
    public void FormatOverlayItem_FromMacro_FormatsCorrectly()
    {
        var macro = new QuickChatMacro(5, "NumPad5", "Need ammo / Resupply here!", true);

        string formatted = QuickChatFormatter.FormatOverlayItem(macro);

        Assert.Equal("[NumPad5] Need ammo / Resupply here!", formatted);
    }

    [Fact]
    public void FormatOverlayItem_FromNullMacro_ReturnsEmptyString()
    {
        string formatted = QuickChatFormatter.FormatOverlayItem((QuickChatMacro)null!);
        Assert.Equal(string.Empty, formatted);
    }

    [Fact]
    public void GetActiveOverlayItems_DefaultMacros_Returns10FormattedItemsWithMax30CharMessages()
    {
        var defaults = BabelConfig.CreateDefaultMacros();

        var items = QuickChatFormatter.GetActiveOverlayItems(defaults);

        Assert.Equal(10, items.Count);
        Assert.Equal("[NumPad1] Affirmative / Roger that!", items[0]);
        Assert.Equal("[NumPad2] Negative / Cancel that!", items[1]);
        Assert.Equal("[NumPad3] Hold position / Stay here!", items[2]);
        Assert.Equal("[NumPad4] Fall back / Retreat now!", items[3]);
        Assert.Equal("[NumPad5] Need ammo / Resupply here!", items[4]);
        Assert.Equal("[NumPad6] Calling in Reinforcements!", items[5]);
        Assert.Equal("[NumPad7] Watch out! Danger / Heavy enem", items[6]);
        Assert.Equal("[NumPad8] Moving to objective!", items[7]);
        Assert.Equal("[NumPad9] Clear the area / Incoming stra", items[8]);
        Assert.Equal("[NumPad0] Thank you, Helldiver!", items[9]);

        // Verify every item matches the pattern [<shortcut>] <message> with <= 30 char message
        foreach (var item in items)
        {
            Assert.StartsWith("[", item);
            int closingBracket = item.IndexOf(']');
            Assert.True(closingBracket > 1);
            Assert.Equal(' ', item[closingBracket + 1]);

            string messagePart = item[(closingBracket + 2)..];
            Assert.True(messagePart.Length <= 30);
        }
    }

    [Fact]
    public void GetActiveOverlayItems_FiltersDisabledMacros()
    {
        var macros = new List<QuickChatMacro>
        {
            new(1, "NumPad1", "Active Message", isEnabled: true),
            new(2, "NumPad2", "Disabled Message", isEnabled: false),
            new(3, "NumPad3", "Another Active", isEnabled: true)
        };

        var items = QuickChatFormatter.GetActiveOverlayItems(macros);

        Assert.Equal(2, items.Count);
        Assert.Equal("[NumPad1] Active Message", items[0]);
        Assert.Equal("[NumPad3] Another Active", items[1]);
    }

    [Fact]
    public void GetActiveOverlayItems_FiltersEmptyKeybindingOrEmptyMessage()
    {
        var macros = new List<QuickChatMacro>
        {
            new(1, "NumPad1", "Valid Callout", true),
            new(2, "", "Missing Key", true),
            new(3, "   ", "Whitespace Key", true),
            new(4, "NumPad4", "", true),
            new(5, "NumPad5", "   ", true),
            new(6, "NumPad6", "Valid Callout 2", true)
        };

        var items = QuickChatFormatter.GetActiveOverlayItems(macros);

        Assert.Equal(2, items.Count);
        Assert.Equal("[NumPad1] Valid Callout", items[0]);
        Assert.Equal("[NumPad6] Valid Callout 2", items[1]);
    }

    [Fact]
    public void GetActiveOverlayItems_OrdersBySlotId()
    {
        var macros = new List<QuickChatMacro>
        {
            new(3, "NumPad3", "Slot Three", true),
            new(1, "NumPad1", "Slot One", true),
            new(2, "NumPad2", "Slot Two", true)
        };

        var items = QuickChatFormatter.GetActiveOverlayItems(macros);

        Assert.Equal(3, items.Count);
        Assert.Equal("[NumPad1] Slot One", items[0]);
        Assert.Equal("[NumPad2] Slot Two", items[1]);
        Assert.Equal("[NumPad3] Slot Three", items[2]);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetActiveOverlayItems_NullOrEmptyCollection_ReturnsEmptyList(bool isNull)
    {
        List<QuickChatMacro>? macros = isNull ? null : new List<QuickChatMacro>();
        var items = QuickChatFormatter.GetActiveOverlayItems(macros);

        Assert.NotNull(items);
        Assert.Empty(items);
    }
}
