using Babel.Core.Models;
using Babel.Core.Utilities;

namespace Babel.Core.Tests;

public class QuickChatMacroTests
{
    [Fact]
    public void MaxMessageLength_IsSetTo300()
    {
        Assert.Equal(300, QuickChatMacro.MaxMessageLength);
    }

    [Fact]
    public void MessageText_Under300Chars_PreservedExactly()
    {
        var macro = new QuickChatMacro(1, "NumPad1", "Short message");
        Assert.Equal("Short message", macro.MessageText);

        macro.MessageText = "Updated message";
        Assert.Equal("Updated message", macro.MessageText);
    }

    [Fact]
    public void MessageText_Exactly300Chars_PreservedExactly()
    {
        var text300 = new string('A', 300);
        var macro = new QuickChatMacro(1, "NumPad1", text300);

        Assert.Equal(300, macro.MessageText.Length);
        Assert.Equal(text300, macro.MessageText);
    }

    [Fact]
    public void MessageText_Exceeding300Chars_TruncatedTo300Chars()
    {
        var text350 = new string('B', 350);
        var macro = new QuickChatMacro(1, "NumPad1", text350);

        Assert.Equal(300, macro.MessageText.Length);
        Assert.Equal(text350[..300], macro.MessageText);

        macro.MessageText = new string('C', 400);
        Assert.Equal(300, macro.MessageText.Length);
        Assert.Equal(new string('C', 300), macro.MessageText);
    }

    [Fact]
    public void MessageText_NullAssignment_SetsEmptyString()
    {
        var macro = new QuickChatMacro(1, "NumPad1", "Initial");
        macro.MessageText = null!;

        Assert.Equal(string.Empty, macro.MessageText);
    }

    [Fact]
    public void QuickChatJsonHelper_ImportLongMessage_TruncatesWhenAssignedToMacro()
    {
        var longMessage = new string('X', 450);
        var json = QuickChatJsonHelper.ExportMessages(new[] { longMessage });

        var parsed = QuickChatJsonHelper.ParseMessages(json);
        Assert.Single(parsed);
        Assert.Equal(450, parsed[0].Length);

        var macro = new QuickChatMacro { MessageText = parsed[0] };
        Assert.Equal(300, macro.MessageText.Length);
        Assert.Equal(new string('X', 300), macro.MessageText);
    }
}
