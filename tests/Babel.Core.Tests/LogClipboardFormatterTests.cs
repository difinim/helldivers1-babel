using Babel.Core.Models;
using Babel.Core.Utilities;

namespace Babel.Core.Tests;

public class LogClipboardFormatterTests
{
    [Fact]
    public void FormatTabSeparatedRow_WithValidEntry_FormatsAllColumns()
    {
        var entry = new TranslationLogEntry
        {
            Time = "14:20:00",
            Author = "Helldiver_1",
            SourceLang = "Russian",
            OriginalMessage = "Привет",
            TranslatedMessage = "Hello",
            Latency = "120ms"
        };

        var result = LogClipboardFormatter.FormatTabSeparatedRow(entry);

        Assert.Equal("14:20:00\tHelldiver_1\tRussian\tПривет\tHello\t120ms", result);
    }

    [Fact]
    public void FormatTabSeparatedRow_WithNullEntry_ReturnsEmptyString()
    {
        var result = LogClipboardFormatter.FormatTabSeparatedRow(null);
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void FormatTabSeparatedRows_WithMultipleEntries_JoinsWithNewlines()
    {
        var entries = new[]
        {
            new TranslationLogEntry { Time = "10:00:00", Author = "P1", SourceLang = "ES", OriginalMessage = "Hola", TranslatedMessage = "Hello", Latency = "50ms" },
            new TranslationLogEntry { Time = "10:00:05", Author = "P2", SourceLang = "FR", OriginalMessage = "Salut", TranslatedMessage = "Hi", Latency = "60ms" }
        };

        var result = LogClipboardFormatter.FormatTabSeparatedRows(entries);

        var lines = result.Split(Environment.NewLine);
        Assert.Equal(2, lines.Length);
        Assert.Equal("10:00:00\tP1\tES\tHola\tHello\t50ms", lines[0]);
        Assert.Equal("10:00:05\tP2\tFR\tSalut\tHi\t60ms", lines[1]);
    }

    [Fact]
    public void FormatTabSeparatedRows_WithNullOrEmpty_ReturnsEmptyString()
    {
        Assert.Equal(string.Empty, LogClipboardFormatter.FormatTabSeparatedRows(null));
        Assert.Equal(string.Empty, LogClipboardFormatter.FormatTabSeparatedRows(Array.Empty<TranslationLogEntry>()));
    }

    [Fact]
    public void FormatSingleLine_WhenTranslatedDiffersFromOriginal_IncludesBoth()
    {
        var entry = new TranslationLogEntry
        {
            Time = "12:00:00",
            Author = "Vanguard",
            OriginalMessage = "За демократию!",
            TranslatedMessage = "For democracy!"
        };

        var result = LogClipboardFormatter.FormatSingleLine(entry);

        Assert.Equal("[12:00:00] Vanguard: For democracy! (Original: За демократию!)", result);
    }

    [Fact]
    public void FormatSingleLine_WhenTranslatedEqualsOriginal_ShowsSingleMessage()
    {
        var entry = new TranslationLogEntry
        {
            Time = "12:00:00",
            Author = "Vanguard",
            OriginalMessage = "Affirmative",
            TranslatedMessage = "Affirmative"
        };

        var result = LogClipboardFormatter.FormatSingleLine(entry);

        Assert.Equal("[12:00:00] Vanguard: Affirmative", result);
    }

    [Fact]
    public void FormatSingleLine_WhenTranslatedIsEmpty_FallsBackToOriginal()
    {
        var entry = new TranslationLogEntry
        {
            Time = "12:00:00",
            Author = "Vanguard",
            OriginalMessage = "Need ammo!",
            TranslatedMessage = ""
        };

        var result = LogClipboardFormatter.FormatSingleLine(entry);

        Assert.Equal("[12:00:00] Vanguard: Need ammo!", result);
    }

    [Fact]
    public void FormatSingleLine_WhenAuthorIsEmpty_UsesUnknown()
    {
        var entry = new TranslationLogEntry
        {
            Time = "12:00:00",
            Author = "  ",
            OriginalMessage = "Help!",
            TranslatedMessage = "Help!"
        };

        var result = LogClipboardFormatter.FormatSingleLine(entry);

        Assert.Equal("[12:00:00] Unknown: Help!", result);
    }

    [Fact]
    public void FormatSingleLine_WithNullEntry_ReturnsEmptyString()
    {
        Assert.Equal(string.Empty, LogClipboardFormatter.FormatSingleLine(null));
    }

    [Fact]
    public void FormatSingleLines_WithMultipleEntries_FormatsAll()
    {
        var entries = new[]
        {
            new TranslationLogEntry { Time = "01:00:00", Author = "A", OriginalMessage = "Msg1", TranslatedMessage = "Msg1" },
            new TranslationLogEntry { Time = "01:00:02", Author = "B", OriginalMessage = "Msg2", TranslatedMessage = "Msg2" }
        };

        var result = LogClipboardFormatter.FormatSingleLines(entries);

        var lines = result.Split(Environment.NewLine);
        Assert.Equal(2, lines.Length);
        Assert.Equal("[01:00:00] A: Msg1", lines[0]);
        Assert.Equal("[01:00:02] B: Msg2", lines[1]);
    }

    [Fact]
    public void FormatPlayerNames_ExtractsNonEmptyAuthors()
    {
        var entries = new[]
        {
            new TranslationLogEntry { Author = "Leader" },
            new TranslationLogEntry { Author = "   " },
            new TranslationLogEntry { Author = "Scout" }
        };

        var result = LogClipboardFormatter.FormatPlayerNames(entries);

        var lines = result.Split(Environment.NewLine);
        Assert.Equal(2, lines.Length);
        Assert.Equal("Leader", lines[0]);
        Assert.Equal("Scout", lines[1]);
    }

    [Fact]
    public void FormatOriginalMessages_ExtractsNonEmptyOriginals()
    {
        var entries = new[]
        {
            new TranslationLogEntry { OriginalMessage = "Move to extraction" },
            new TranslationLogEntry { OriginalMessage = "" },
            new TranslationLogEntry { OriginalMessage = "Calling airstrike" }
        };

        var result = LogClipboardFormatter.FormatOriginalMessages(entries);

        var lines = result.Split(Environment.NewLine);
        Assert.Equal(2, lines.Length);
        Assert.Equal("Move to extraction", lines[0]);
        Assert.Equal("Calling airstrike", lines[1]);
    }

    [Fact]
    public void FormatTranslatedMessages_ExtractsNonEmptyTranslations()
    {
        var entries = new[]
        {
            new TranslationLogEntry { TranslatedMessage = "Translated 1" },
            new TranslationLogEntry { TranslatedMessage = "  " },
            new TranslationLogEntry { TranslatedMessage = "Translated 2" }
        };

        var result = LogClipboardFormatter.FormatTranslatedMessages(entries);

        var lines = result.Split(Environment.NewLine);
        Assert.Equal(2, lines.Length);
        Assert.Equal("Translated 1", lines[0]);
        Assert.Equal("Translated 2", lines[1]);
    }

    [Fact]
    public void FormatSingleLine_WhenTranslatedIsDash_FallsBackToOriginal()
    {
        var entry = new TranslationLogEntry
        {
            Time = "12:00:00",
            Author = "P1",
            OriginalMessage = "Welcome to the squad!",
            TranslatedMessage = "-"
        };

        var result = LogClipboardFormatter.FormatSingleLine(entry);

        Assert.Equal("[12:00:00] P1: Welcome to the squad!", result);
    }

    [Fact]
    public void FormatSingleLine_WhenTranslationError_FormatsErrorWithOriginal()
    {
        var entry = new TranslationLogEntry
        {
            Time = "12:00:00",
            Author = "Boris",
            OriginalMessage = "Привет",
            TranslatedMessage = "[Error: Request timed out]"
        };

        var result = LogClipboardFormatter.FormatSingleLine(entry);

        Assert.Equal("[12:00:00] Boris: [Error: Request timed out] (Original: Привет)", result);
    }

    [Fact]
    public void FormatSingleLine_WithDynamicPlayerNumbers_FormatsCorrectly()
    {
        var p2Entry = new TranslationLogEntry
        {
            Time = "14:15:16",
            Author = "P2",
            OriginalMessage = "Affirmative!",
            TranslatedMessage = "-"
        };

        var result = LogClipboardFormatter.FormatSingleLine(p2Entry);

        Assert.Equal("[14:15:16] P2: Affirmative!", result);
        Assert.DoesNotContain("You", result);
    }
}
