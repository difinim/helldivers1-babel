using Babel.Core.Models;
using Babel.Core.Utilities;

namespace Babel.Core.Tests;

public class IncomingTranslationHeuristicsTests
{
    [Theory]
    [InlineData("xd")]
    [InlineData("XD")]
    [InlineData("xD")]
    [InlineData("Xd")]
    [InlineData("gg")]
    [InlineData("GG")]
    [InlineData("Gg")]
    [InlineData("ty")]
    [InlineData("TY")]
    [InlineData("Ty")]
    [InlineData("ok")]
    [InlineData("OK")]
    [InlineData("Ok")]
    [InlineData("sorry")]
    [InlineData("SORRY")]
    [InlineData("sry")]
    [InlineData("SRY")]
    [InlineData("pls")]
    [InlineData("PLS")]
    [InlineData("thx")]
    [InlineData("THX")]
    public void TryFastPath_UniversalAcronymsAcrossCasings_MatchesLocallyAsNeutral(string callout)
    {
        bool matched = IncomingTranslationHeuristics.TryFastPath(callout, "English", out var result);

        Assert.True(matched);
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal("-", result.DetectedSourceLanguage);
        Assert.Equal(callout, result.TranslatedText);
        Assert.Equal(0, result.LatencyMs);
    }

    [Theory]
    [InlineData(":D")]
    [InlineData(":d")]
    [InlineData(":P")]
    [InlineData(":p")]
    [InlineData(":)")]
    [InlineData("(:")]
    [InlineData(":-)")]
    [InlineData(":(")]
    [InlineData(";-)")]
    [InlineData("-_-")]
    [InlineData("^^")]
    [InlineData("o_o")]
    [InlineData("O_O")]
    public void TryFastPath_EmoticonsAcrossCasings_MatchesLocallyAsNeutral(string emoticon)
    {
        bool matched = IncomingTranslationHeuristics.TryFastPath(emoticon, "English", out var result);

        Assert.True(matched);
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal("-", result.DetectedSourceLanguage);
        Assert.Equal(emoticon, result.TranslatedText);
        Assert.Equal(0, result.LatencyMs);
    }

    [Theory]
    [InlineData("GG!")]
    [InlineData("gg.")]
    [InlineData("ty...")]
    [InlineData("sorry!")]
    [InlineData("ok.")]
    [InlineData("XD!")]
    [InlineData(":D!")]
    public void TryFastPath_SurroundingBoundaryPunctuation_MatchesCleanlyAsNeutral(string input)
    {
        bool matched = IncomingTranslationHeuristics.TryFastPath(input, "English", out var result);

        Assert.True(matched);
        Assert.NotNull(result);
        Assert.Equal("-", result.DetectedSourceLanguage);
        Assert.Equal(0, result.LatencyMs);
    }

    [Theory]
    [InlineData("nooo")]
    [InlineData("yesss")]
    [InlineData("okkk")]
    [InlineData("sooorry")]
    public void TryFastPath_RepeatedCharacterCallouts_MatchesCleanlyAsNeutral(string input)
    {
        bool matched = IncomingTranslationHeuristics.TryFastPath(input, "English", out var result);

        Assert.True(matched);
        Assert.NotNull(result);
        Assert.Equal("-", result.DetectedSourceLanguage);
        Assert.Equal(0, result.LatencyMs);
    }

    [Theory]
    [InlineData("xd")]
    [InlineData("XD")]
    [InlineData("gg")]
    [InlineData("ty")]
    [InlineData("lol")]
    [InlineData("afk")]
    [InlineData("brb")]
    [InlineData(":)")]
    [InlineData(":D")]
    [InlineData("👍")]
    [InlineData("😂😂")]
    [InlineData("❤️")]
    [InlineData("???")]
    public void TryFastPath_NonEnglishTargetLanguage_ResolvesAsNeutralAndNotTargetLanguage(string input)
    {
        // When user's target language is Portuguese, acronyms and emojis must NOT be detected as Portuguese
        bool matched = IncomingTranslationHeuristics.TryFastPath(input, "Portuguese (PT-BR)", out var result);

        Assert.True(matched);
        Assert.NotNull(result);
        Assert.Equal("-", result.DetectedSourceLanguage);
        Assert.NotEqual("Portuguese (PT-BR)", result.DetectedSourceLanguage);
        Assert.Equal(0, result.LatencyMs);
    }

    [Theory]
    [InlineData("run")]
    [InlineData("help")]
    [InlineData("ammo")]
    [InlineData("watch out")]
    public void TryFastPath_EnglishGameplayCallouts_MatchesWhenTargetIsEnglish(string callout)
    {
        bool matched = IncomingTranslationHeuristics.TryFastPath(callout, "English", out var result);

        Assert.True(matched);
        Assert.NotNull(result);
        Assert.Equal("English", result.DetectedSourceLanguage);
    }

    [Theory]
    [InlineData("run")]
    [InlineData("help")]
    [InlineData("ammo")]
    [InlineData("watch out")]
    public void TryFastPath_EnglishGameplayCallouts_BypassesFastPathWhenTargetIsNotEnglish(string callout)
    {
        // When target is Portuguese, English gameplay callouts like "run" should be translated by AI, not fast-pathed as identical
        bool matched = IncomingTranslationHeuristics.TryFastPath(callout, "Portuguese (PT-BR)", out var result);

        Assert.False(matched);
        Assert.Null(result);
    }

    [Theory]
    [InlineData("watch top left")]
    [InlineData("this is good")]
    [InlineData("time to play like an old man dif :)")]
    [InlineData("need assistance at extraction")]
    public void TryFastPath_RegularSentences_ReturnsFalseToAllowAiTranslation(string sentence)
    {
        bool matched = IncomingTranslationHeuristics.TryFastPath(sentence, "English", out var result);

        Assert.False(matched);
        Assert.Null(result);
    }

    [Theory]
    [InlineData("xd", true)]
    [InlineData("XD", true)]
    [InlineData(":)", true)]
    [InlineData(":D", true)]
    [InlineData("👍", true)]
    [InlineData("😂😂", true)]
    [InlineData("❤️", true)]
    [InlineData("???", true)]
    [InlineData("!", true)]
    [InlineData("gg", true)]
    [InlineData("GG!", true)]
    [InlineData("ty", true)]
    [InlineData("lol", true)]
    [InlineData("LOOOL", true)]
    [InlineData("afk", true)]
    [InlineData("brb", true)]
    [InlineData("hahaha", true)]
    [InlineData("kkkk", true)]
    [InlineData("jajaja", true)]
    [InlineData("gg :)", true)]
    [InlineData("lol ty", true)]
    [InlineData("watch top left", false)]
    [InlineData("need ammo", false)]
    [InlineData("vamos defender aqui", false)]
    [InlineData("minas ao norte", false)]
    public void IsAcronymAbbreviationOrEmoji_ClassifiesCorrectly(string input, bool expected)
    {
        bool actual = IncomingTranslationHeuristics.IsAcronymAbbreviationOrEmoji(input);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void NormalizeIdenticalTranslation_NormalizesRussianHallucinationForEnglishSentence()
    {
        var aiResult = TranslationResult.Ok("(Russian): watch top left", "Russian", latencyMs: 898);

        var normalized = IncomingTranslationHeuristics.NormalizeIdenticalTranslation(aiResult, "watch top left", "English");

        Assert.Equal("English", normalized.DetectedSourceLanguage);
        Assert.Equal("watch top left", normalized.TranslatedText);
        Assert.Equal(898, normalized.LatencyMs);
    }

    [Fact]
    public void NormalizeIdenticalTranslation_NormalizesSpanishHallucinationForEnglishSentence()
    {
        var aiResult = TranslationResult.Ok("(Spanish): this is good", "Spanish", latencyMs: 958);

        var normalized = IncomingTranslationHeuristics.NormalizeIdenticalTranslation(aiResult, "this is good", "English");

        Assert.Equal("English", normalized.DetectedSourceLanguage);
        Assert.Equal("this is good", normalized.TranslatedText);
        Assert.Equal(958, normalized.LatencyMs);
    }

    [Fact]
    public void NormalizeIdenticalTranslation_AcronymOrEmojiDoesNotAdoptTargetLanguage()
    {
        var aiResult = TranslationResult.Ok("lol", "Portuguese (PT-BR)", latencyMs: 250);

        var normalized = IncomingTranslationHeuristics.NormalizeIdenticalTranslation(aiResult, "lol", "Portuguese (PT-BR)");

        // Acronyms and emojis must be marked language-neutral "-"
        Assert.Equal("-", normalized.DetectedSourceLanguage);
        Assert.NotEqual("Portuguese (PT-BR)", normalized.DetectedSourceLanguage);
        Assert.Equal("lol", normalized.TranslatedText);
    }

    [Fact]
    public void NormalizeIdenticalTranslation_PreservesGenuineSpanishTranslation()
    {
        var original = "hola amigo";
        var aiResult = TranslationResult.Ok("(Spanish): hello friend", "Spanish", latencyMs: 800);

        var normalized = IncomingTranslationHeuristics.NormalizeIdenticalTranslation(aiResult, original, "English");

        // Should NOT be normalized to English because "hello friend" != "hola amigo"
        Assert.Equal("Spanish", normalized.DetectedSourceLanguage);
        Assert.Equal("(Spanish): hello friend", normalized.TranslatedText);
    }

    [Fact]
    public void NormalizeIdenticalTranslation_PreservesGenuineRussianTranslation()
    {
        var original = "беги";
        var aiResult = TranslationResult.Ok("(Russian): run", "Russian", latencyMs: 850);

        var normalized = IncomingTranslationHeuristics.NormalizeIdenticalTranslation(aiResult, original, "English");

        // Should NOT be normalized to English because "run" != "беги"
        Assert.Equal("Russian", normalized.DetectedSourceLanguage);
        Assert.Equal("(Russian): run", normalized.TranslatedText);
    }

    [Fact]
    public void NormalizeIdenticalTranslation_HandlesNullOrFailedResultsGracefully()
    {
        var failed = TranslationResult.Failed("Network error", 500);
        var normFailed = IncomingTranslationHeuristics.NormalizeIdenticalTranslation(failed, "test", "English");
        Assert.False(normFailed.Success);

        var nullResult = IncomingTranslationHeuristics.NormalizeIdenticalTranslation(null!, "test", "English");
        Assert.Null(nullResult);
    }
}
