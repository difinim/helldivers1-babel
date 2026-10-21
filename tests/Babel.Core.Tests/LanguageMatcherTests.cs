using Babel.Core.Services;

namespace Babel.Core.Tests;

public class LanguageMatcherTests
{
    [Theory]
    [InlineData("English", "English")]
    [InlineData("english", "ENGLISH")]
    [InlineData("Spanish", "spanish")]
    [InlineData("Russian", "Russian")]
    [InlineData("French", "FRENCH")]
    public void IsSameLanguage_ExactOrCaseInsensitive_ReturnsTrue(string lang1, string lang2)
    {
        Assert.True(LanguageMatcher.IsSameLanguage(lang1, lang2));
    }

    [Theory]
    [InlineData("Portuguese (PT-BR)", "Portuguese")]
    [InlineData("Portuguese", "Portuguese (PT-BR)")]
    [InlineData("PT-BR", "Portuguese (PT-BR)")]
    [InlineData("pt-br", "Portuguese")]
    [InlineData("Portuguese (PT-BR)", "Português")]
    [InlineData("Tagalog (Filipino)", "Tagalog")]
    [InlineData("Tagalog (Filipino)", "Filipino")]
    [InlineData("Filipino", "Tagalog (Filipino)")]
    [InlineData("Persian (Farsi)", "Persian")]
    [InlineData("Persian (Farsi)", "Farsi")]
    [InlineData("Farsi", "Persian (Farsi)")]
    public void IsSameLanguage_DialectsAndParentheses_ReturnsTrue(string lang1, string lang2)
    {
        Assert.True(LanguageMatcher.IsSameLanguage(lang1, lang2));
    }

    [Theory]
    [InlineData("en", "English")]
    [InlineData("English", "en")]
    [InlineData("pt", "Portuguese (PT-BR)")]
    [InlineData("es", "Spanish")]
    [InlineData("ru", "Russian")]
    [InlineData("ja", "Japanese")]
    [InlineData("zh", "Chinese")]
    [InlineData("de", "German")]
    [InlineData("fr", "French")]
    public void IsSameLanguage_LanguageCodesAndAliases_ReturnsTrue(string lang1, string lang2)
    {
        Assert.True(LanguageMatcher.IsSameLanguage(lang1, lang2));
    }

    [Theory]
    [InlineData("Russian", "English")]
    [InlineData("Spanish", "Portuguese (PT-BR)")]
    [InlineData("German", "French")]
    [InlineData("Japanese", "Chinese")]
    [InlineData("Italian", "Spanish")]
    [InlineData("Korean", "Japanese")]
    public void IsSameLanguage_DifferentLanguages_ReturnsFalse(string lang1, string lang2)
    {
        Assert.False(LanguageMatcher.IsSameLanguage(lang1, lang2));
    }

    [Theory]
    [InlineData(null, "English")]
    [InlineData("English", null)]
    [InlineData("", "English")]
    [InlineData("   ", "English")]
    [InlineData("auto", "English")]
    [InlineData("English", "auto")]
    [InlineData(null, null)]
    public void IsSameLanguage_NullEmptyOrAuto_ReturnsFalse(string? lang1, string? lang2)
    {
        Assert.False(LanguageMatcher.IsSameLanguage(lang1, lang2));
    }
}
