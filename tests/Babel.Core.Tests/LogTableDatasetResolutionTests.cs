using Babel.Core.Models;

namespace Babel.Core.Tests;

public class LogTableDatasetResolutionTests
{
    public static string ResolveAuthor(string? playerName, int playerIndex)
    {
        return !string.IsNullOrWhiteSpace(playerName) ? playerName.Trim() : $"P{playerIndex}";
    }

    public static string ResolveColorHex(int playerIndex, ColorDto? specificColor = null)
    {
        var color = specificColor ?? ColorDto.FromPlayerIndex(playerIndex);
        return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
    }

    [Theory]
    [InlineData("Helldiver", 1, "Helldiver")]
    [InlineData("  Diego  ", 2, "Diego")]
    [InlineData(null, 1, "P1")]
    [InlineData("", 2, "P2")]
    [InlineData("   ", 3, "P3")]
    [InlineData(null, 4, "P4")]
    public void ResolveAuthor_ShouldAlwaysReturnPlayerNameOrPlayerNumber(string? inputName, int playerIndex, string expectedAuthor)
    {
        var author = ResolveAuthor(inputName, playerIndex);

        Assert.Equal(expectedAuthor, author);
        Assert.DoesNotContain("You", author);
    }

    [Fact]
    public void ResolveColorHex_MapsSlotsToPlayerColors()
    {
        Assert.Equal("#FFC800", ResolveColorHex(1)); // Gold
        Assert.Equal("#50A0FF", ResolveColorHex(2)); // Blue
        Assert.Equal("#B450FF", ResolveColorHex(3)); // Purple
        Assert.Equal("#50FFDC", ResolveColorHex(4)); // Cyan
    }

    [Fact]
    public void ResolveColorHex_UsesSpecificColorWhenProvided()
    {
        var custom = new ColorDto(120, 200, 50);
        Assert.Equal("#78C832", ResolveColorHex(1, custom));
    }

    [Fact]
    public void TranslationError_EntryMapping_CreatesCorrectSchema()
    {
        var req = new TranslationRequest
        {
            PlayerIndex = 2,
            PlayerName = "Teammate",
            PlayerColor = ColorDto.Blue,
            Text = "Retreat!"
        };
        string errorMessage = "API Rate Limit (429)";

        var entry = new TranslationLogEntry
        {
            Author = ResolveAuthor(req.PlayerName, req.PlayerIndex),
            SourceLang = "-",
            OriginalMessage = req.Text,
            TranslatedMessage = $"[Error: {errorMessage}]",
            Latency = "-",
            ColorHex = ResolveColorHex(req.PlayerIndex, req.PlayerColor)
        };

        Assert.Equal("Teammate", entry.Author);
        Assert.Equal("-", entry.SourceLang);
        Assert.Equal("Retreat!", entry.OriginalMessage);
        Assert.Equal("[Error: API Rate Limit (429)]", entry.TranslatedMessage);
        Assert.Equal("-", entry.Latency);
        Assert.Equal("#50A0FF", entry.ColorHex);
    }

    [Fact]
    public void TranslationError_WithoutPlayerName_UsesDynamicPlayerNumber()
    {
        var req = new TranslationRequest
        {
            PlayerIndex = 3,
            PlayerName = "",
            PlayerColor = ColorDto.Purple,
            Text = "Mayday!"
        };
        string errorMessage = "Request timed out";

        var entry = new TranslationLogEntry
        {
            Author = ResolveAuthor(req.PlayerName, req.PlayerIndex),
            SourceLang = "-",
            OriginalMessage = req.Text,
            TranslatedMessage = $"[Error: {errorMessage}]",
            Latency = "-",
            ColorHex = ResolveColorHex(req.PlayerIndex, req.PlayerColor)
        };

        Assert.Equal("P3", entry.Author);
        Assert.Equal("[Error: Request timed out]", entry.TranslatedMessage);
        Assert.Equal("#B450FF", entry.ColorHex);
    }

    [Fact]
    public void WelcomeMessage_EntryMapping_CreatesCorrectSchemaWithDynamicPlayer()
    {
        int playerIndex = 1;
        string playerName = "HostCommander";
        string welcomeMsg = "Welcome to Super Earth!";

        var entry = new TranslationLogEntry
        {
            Author = ResolveAuthor(playerName, playerIndex),
            SourceLang = "-",
            OriginalMessage = welcomeMsg,
            TranslatedMessage = "-",
            Latency = "-",
            ColorHex = ResolveColorHex(playerIndex)
        };

        Assert.Equal("HostCommander", entry.Author);
        Assert.Equal("-", entry.SourceLang);
        Assert.Equal("Welcome to Super Earth!", entry.OriginalMessage);
        Assert.Equal("-", entry.TranslatedMessage);
        Assert.Equal("-", entry.Latency);
        Assert.Equal("#FFC800", entry.ColorHex);
    }

    [Fact]
    public void QuickChat_EntryMapping_CreatesCorrectSchemaWithDynamicPlayer()
    {
        int playerIndex = 4;
        string playerName = "";
        string macroText = "Need ammo at beacon!";

        var entry = new TranslationLogEntry
        {
            Author = ResolveAuthor(playerName, playerIndex),
            SourceLang = "-",
            OriginalMessage = macroText,
            TranslatedMessage = "-",
            Latency = "-",
            ColorHex = ResolveColorHex(playerIndex)
        };

        Assert.Equal("P4", entry.Author);
        Assert.Equal("-", entry.SourceLang);
        Assert.Equal("Need ammo at beacon!", entry.OriginalMessage);
        Assert.Equal("-", entry.TranslatedMessage);
        Assert.Equal("-", entry.Latency);
        Assert.Equal("#50FFDC", entry.ColorHex);
    }
}
