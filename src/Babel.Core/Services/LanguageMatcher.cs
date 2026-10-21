namespace Babel.Core.Services;

/// <summary>
/// Utility for comparing and matching language names, codes, aliases, and dialect variants.
/// </summary>
public static class LanguageMatcher
{
    private static readonly Dictionary<string, string> CanonicalLanguageMap = new(StringComparer.OrdinalIgnoreCase)
    {
        // English
        ["english"] = "en",
        ["en"] = "en",

        // Portuguese
        ["portuguese"] = "pt",
        ["português"] = "pt",
        ["portugues"] = "pt",
        ["pt-br"] = "pt",
        ["pt"] = "pt",

        // Spanish
        ["spanish"] = "es",
        ["español"] = "es",
        ["espanol"] = "es",
        ["castilian"] = "es",
        ["castellano"] = "es",
        ["es"] = "es",

        // French
        ["french"] = "fr",
        ["français"] = "fr",
        ["francais"] = "fr",
        ["fr"] = "fr",

        // German
        ["german"] = "de",
        ["deutsch"] = "de",
        ["de"] = "de",

        // Russian
        ["russian"] = "ru",
        ["русский"] = "ru",
        ["ru"] = "ru",

        // Chinese
        ["chinese"] = "zh",
        ["mandarin"] = "zh",
        ["simplified chinese"] = "zh",
        ["traditional chinese"] = "zh",
        ["zh"] = "zh",
        ["zh-cn"] = "zh",
        ["zh-tw"] = "zh",

        // Japanese
        ["japanese"] = "ja",
        ["nihongo"] = "ja",
        ["日本語"] = "ja",
        ["ja"] = "ja",
        ["jp"] = "ja",

        // Korean
        ["korean"] = "ko",
        ["한국어"] = "ko",
        ["ko"] = "ko",

        // Italian
        ["italian"] = "it",
        ["italiano"] = "it",
        ["it"] = "it",

        // Persian / Farsi
        ["persian"] = "fa",
        ["farsi"] = "fa",
        ["fa"] = "fa",

        // Tagalog / Filipino
        ["tagalog"] = "tl",
        ["filipino"] = "tl",
        ["tl"] = "tl",
        ["fil"] = "tl",

        // Arabic
        ["arabic"] = "ar",
        ["ar"] = "ar",

        // Polish
        ["polish"] = "pl",
        ["polski"] = "pl",
        ["pl"] = "pl",

        // Dutch
        ["dutch"] = "nl",
        ["nederlands"] = "nl",
        ["nl"] = "nl",

        // Turkish
        ["turkish"] = "tr",
        ["türkçe"] = "tr",
        ["turkce"] = "tr",
        ["tr"] = "tr",

        // Ukrainian
        ["ukrainian"] = "uk",
        ["українська"] = "uk",
        ["uk"] = "uk",

        // Vietnamese
        ["vietnamese"] = "vi",
        ["tiếng việt"] = "vi",
        ["tieng viet"] = "vi",
        ["vi"] = "vi",

        // Swedish
        ["swedish"] = "sv",
        ["svenska"] = "sv",
        ["sv"] = "sv",

        // Greek
        ["greek"] = "el",
        ["ελληνικά"] = "el",
        ["el"] = "el",

        // Hindi
        ["hindi"] = "hi",
        ["hi"] = "hi",

        // Indonesian
        ["indonesian"] = "id",
        ["bahasa indonesia"] = "id",
        ["id"] = "id",

        // Malay
        ["malay"] = "ms",
        ["bahasa melayu"] = "ms",
        ["ms"] = "ms",

        // Czech
        ["czech"] = "cs",
        ["čeština"] = "cs",
        ["cestina"] = "cs",
        ["cs"] = "cs",

        // Hungarian
        ["hungarian"] = "hu",
        ["magyar"] = "hu",
        ["hu"] = "hu",

        // Romanian
        ["romanian"] = "ro",
        ["română"] = "ro",
        ["romana"] = "ro",
        ["ro"] = "ro",

        // Thai
        ["thai"] = "th",
        ["th"] = "th",

        // Bengali
        ["bengali"] = "bn",
        ["bangla"] = "bn",
        ["bn"] = "bn"
    };

    /// <summary>
    /// Returns true if two language descriptions represent the same language family.
    /// </summary>
    public static bool IsSameLanguage(string? lang1, string? lang2)
    {
        if (string.IsNullOrWhiteSpace(lang1) || string.IsNullOrWhiteSpace(lang2))
        {
            return false;
        }

        lang1 = lang1.Trim();
        lang2 = lang2.Trim();

        if (string.Equals(lang1, "auto", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(lang2, "auto", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (string.Equals(lang1, lang2, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var tokens1 = ExtractTokens(lang1);
        var tokens2 = ExtractTokens(lang2);

        // Check direct token match
        foreach (var t1 in tokens1)
        {
            if (tokens2.Contains(t1))
            {
                return true;
            }
        }

        // Check canonical language code mapping
        foreach (var t1 in tokens1)
        {
            if (CanonicalLanguageMap.TryGetValue(t1, out var code1))
            {
                foreach (var t2 in tokens2)
                {
                    if (CanonicalLanguageMap.TryGetValue(t2, out var code2) &&
                        string.Equals(code1, code2, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private static HashSet<string> ExtractTokens(string lang)
    {
        var tokens = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { lang };

        // Handle parentheses: e.g. "Portuguese (PT-BR)" -> "Portuguese", "PT-BR"
        int openParen = lang.IndexOf('(');
        if (openParen >= 0)
        {
            string basePart = lang[..openParen].Trim();
            if (!string.IsNullOrEmpty(basePart))
            {
                tokens.Add(basePart);
            }

            int closeParen = lang.IndexOf(')', openParen);
            if (closeParen > openParen)
            {
                string insideParen = lang.Substring(openParen + 1, closeParen - openParen - 1).Trim();
                if (!string.IsNullOrEmpty(insideParen))
                {
                    tokens.Add(insideParen);
                }
            }
        }

        // Handle hyphen / underscore: e.g. "pt-br" -> "pt"
        int sepIdx = lang.IndexOfAny(['-', '_']);
        if (sepIdx > 0)
        {
            string prefix = lang[..sepIdx].Trim();
            if (!string.IsNullOrEmpty(prefix))
            {
                tokens.Add(prefix);
            }
        }

        return tokens;
    }
}
