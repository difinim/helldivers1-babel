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
    [InlineData("fun")]
    [InlineData("FUN")]
    [InlineData("Fun")]
    [InlineData("pls")]
    [InlineData("PLS")]
    [InlineData("thx")]
    [InlineData("THX")]
    public void TryFastPath_UniversalCalloutsAcrossCasings_MatchesLocally(string callout)
    {
        bool matched = IncomingTranslationHeuristics.TryFastPath(callout, "English", out var result);

        Assert.True(matched);
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal("English", result.DetectedSourceLanguage);
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
    public void TryFastPath_EmoticonsAcrossCasings_MatchesLocally(string emoticon)
    {
        bool matched = IncomingTranslationHeuristics.TryFastPath(emoticon, "English", out var result);

        Assert.True(matched);
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal("English", result.DetectedSourceLanguage);
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
    [InlineData(" fun ")]
    public void TryFastPath_SurroundingBoundaryPunctuation_MatchesCleanly(string input)
    {
        bool matched = IncomingTranslationHeuristics.TryFastPath(input, "English", out var result);

        Assert.True(matched);
        Assert.NotNull(result);
        Assert.Equal("English", result.DetectedSourceLanguage);
        Assert.Equal(0, result.LatencyMs);
    }

    [Theory]
    [InlineData("nooo")]
    [InlineData("yesss")]
    [InlineData("okkk")]
    [InlineData("sooorry")]
    public void TryFastPath_RepeatedCharacterCallouts_MatchesCleanly(string input)
    {
        bool matched = IncomingTranslationHeuristics.TryFastPath(input, "English", out var result);

        Assert.True(matched);
        Assert.NotNull(result);
        Assert.Equal("English", result.DetectedSourceLanguage);
        Assert.Equal(0, result.LatencyMs);
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
    public void NormalizeIdenticalTranslation_NormalizesSpanishHallucinationWithComplexSentence()
    {
        var original = "time to play like an old man dif :)";
        var aiResult = TranslationResult.Ok("(Spanish): time to play like an old man dif :)", "Spanish", latencyMs: 1772);

        var normalized = IncomingTranslationHeuristics.NormalizeIdenticalTranslation(aiResult, original, "English");

        Assert.Equal("English", normalized.DetectedSourceLanguage);
        Assert.Equal(original, normalized.TranslatedText);
        Assert.Equal(1772, normalized.LatencyMs);
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
