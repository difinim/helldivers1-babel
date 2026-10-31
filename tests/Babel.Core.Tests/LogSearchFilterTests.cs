using Babel.Core.Models;
using Babel.Core.Utilities;

namespace Babel.Core.Tests;

public class LogSearchFilterTests
{
    private static TranslationLogEntry CreateSampleEntry()
    {
        return new TranslationLogEntry
        {
            Timestamp = new DateTime(2026, 10, 30, 14, 35, 22),
            Date = "2026-10-30",
            Time = "14:35:22",
            Author = "General Brasch",
            ColorHex = "#FFC800",
            SourceLang = "RU",
            OriginalMessage = "Za Super Earth, Helldivers!",
            TranslatedMessage = "For Super Earth, Helldivers!",
            Latency = "120ms"
        };
    }

    [Fact]
    public void Matches_NullEntry_ReturnsFalse()
    {
        Assert.False(LogSearchFilter.Matches(null, "Super"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Matches_NullOrWhitespaceQuery_ReturnsTrue(string? query)
    {
        var entry = CreateSampleEntry();
        Assert.True(LogSearchFilter.Matches(entry, query));
    }

    [Theory]
    [InlineData("2026-10-30")]     // Date
    [InlineData("14:35:22")]       // Time
    [InlineData("Brasch")]         // Author
    [InlineData("RU")]             // SourceLang / Detected
    [InlineData("Za Super")]       // OriginalMessage
    [InlineData("For Super")]      // TranslatedMessage
    [InlineData("120ms")]          // Latency
    public void Matches_SingleToken_MatchesAcrossAnyColumn(string token)
    {
        var entry = CreateSampleEntry();
        Assert.True(LogSearchFilter.Matches(entry, token));
    }

    [Theory]
    [InlineData("general")]
    [InlineData("GENERAL")]
    [InlineData("GeNeRaL")]
    [InlineData("ru")]
    [InlineData("HELLDIVERS")]
    [InlineData("120MS")]
    public void Matches_IsCaseInsensitive(string token)
    {
        var entry = CreateSampleEntry();
        Assert.True(LogSearchFilter.Matches(entry, token));
    }

    [Theory]
    [InlineData("Brasch Earth 120ms")]
    [InlineData("2026-10-30 14:35 RU")]
    [InlineData("Helldivers Za For")]
    public void Matches_MultiTokenQuery_ReturnsTrueWhenAllTokensMatch(string query)
    {
        var entry = CreateSampleEntry();
        Assert.True(LogSearchFilter.Matches(entry, query));
    }

    [Theory]
    [InlineData("Brasch Cyborgs")]        // "Cyborgs" does not exist
    [InlineData("2026-10-30 999ms")]       // "999ms" does not exist
    [InlineData("NonExistentToken")]
    public void Matches_MultiTokenQuery_ReturnsFalseWhenAnyTokenFails(string query)
    {
        var entry = CreateSampleEntry();
        Assert.False(LogSearchFilter.Matches(entry, query));
    }

    [Fact]
    public void Matches_RespectsConfiguredCustomDateTimeFormat()
    {
        // Custom format: Date "31/10/2026", Time "02:30 PM"
        var entry = new TranslationLogEntry
        {
            Timestamp = new DateTime(2026, 10, 31, 14, 30, 0),
            Date = "31/10/2026",
            Time = "02:30 PM",
            Author = "Helldiver1",
            SourceLang = "EN",
            OriginalMessage = "Requesting extraction",
            TranslatedMessage = "Requesting extraction",
            Latency = "45ms"
        };

        // Matches formatted strings
        Assert.True(LogSearchFilter.Matches(entry, "31/10/2026"));
        Assert.True(LogSearchFilter.Matches(entry, "02:30 PM"));
        Assert.True(LogSearchFilter.Matches(entry, "PM 31/10"));

        // Does NOT match unformatted ISO string because entry is formatted according to user settings
        Assert.False(LogSearchFilter.Matches(entry, "2026-10-31"));
    }

    [Fact]
    public void Matches_HandlesMultipleWhitespaceSeparatorsGracefully()
    {
        var entry = CreateSampleEntry();
        Assert.True(LogSearchFilter.Matches(entry, "   Brasch     120ms    Earth   "));
    }

    [Fact]
    public void Matches_FiltersListOfMixedEntriesCorrectly()
    {
        var entries = new List<TranslationLogEntry>
        {
            new() { Date = "2026-10-30", Time = "10:00:00", Author = "P1", OriginalMessage = "Hello", TranslatedMessage = "Hola", Latency = "50ms" },
            new() { Date = "2026-10-30", Time = "11:00:00", Author = "P2", OriginalMessage = "Help me", TranslatedMessage = "Ayúdame", Latency = "120ms" },
            new() { Date = "2026-10-31", Time = "12:00:00", Author = "P1", OriginalMessage = "Affirmative", TranslatedMessage = "Afirmativo", Latency = "45ms" }
        };

        // Filter by Date
        Assert.Equal(2, entries.Count(e => LogSearchFilter.Matches(e, "2026-10-30")));
        Assert.Single(entries, e => LogSearchFilter.Matches(e, "2026-10-31"));

        // Filter by Author + message token
        Assert.Single(entries, e => LogSearchFilter.Matches(e, "P1 Hola"));
        Assert.DoesNotContain(entries, e => LogSearchFilter.Matches(e, "P2 Hola"));

        // Filter by Latency
        Assert.Single(entries, e => LogSearchFilter.Matches(e, "120ms"));
        Assert.Equal(3, entries.Count(e => LogSearchFilter.Matches(e, "ms")));
    }

    [Fact]
    public void Tokenize_SplitsOnWhitespace_AndHandlesNull()
    {
        Assert.Empty(LogSearchFilter.Tokenize(null));
        Assert.Empty(LogSearchFilter.Tokenize(""));
        Assert.Empty(LogSearchFilter.Tokenize("   "));

        var tokens = LogSearchFilter.Tokenize("  hello   world  123 ");
        Assert.Equal(new[] { "hello", "world", "123" }, tokens);
    }

    [Fact]
    public void Matches_WithPreTokenizedArray_MatchesCorrectly()
    {
        var entry = CreateSampleEntry();
        var tokens = new[] { "Brasch", "120ms", "Earth" };
        Assert.True(LogSearchFilter.Matches(entry, tokens));

        var failingTokens = new[] { "Brasch", "Cyborg" };
        Assert.False(LogSearchFilter.Matches(entry, failingTokens));

        Assert.True(LogSearchFilter.Matches(entry, Array.Empty<string>()));
        Assert.False(LogSearchFilter.Matches(null, tokens));
    }
}
