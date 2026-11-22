using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Babel.Core.Models;
using Babel.Core.Services;

namespace Babel.Core.Utilities;

/// <summary>
/// Fast-path recognition and post-translation heuristics to prevent false foreign language detections
/// on plain English in-game chat, universal gamer callouts, acronyms, and emoticons.
/// </summary>
public static class IncomingTranslationHeuristics
{
    private static readonly HashSet<string> UniversalAcronyms = new(StringComparer.OrdinalIgnoreCase)
    {
        // Common gamer greetings & farewells
        "hi", "hey", "yo", "bye", "cya", "bb", "gn", "gm", "wb",

        // Acknowledgments & confirmations
        "ok", "okay", "k", "kk", "yes", "no", "yep", "yeah", "nope", "nah",

        // Politeness & courtesy
        "sorry", "sry", "pls", "plz", "ty", "thx", "thanks", "tyvm",
        "np", "yw",

        // Game callouts & performance shorthand
        "gg", "ggs", "gj", "wp", "ez", "rdy", "dif", "diff", "o7", "gl", "hf", "glhf",
        "rip", "ff", "n1", "gr8", "m8", "sec",

        // Acronyms & common chat shorthand
        "lol", "lmao", "rofl", "omg", "omfg", "wtf", "wth", "afk", "brb", "idk", "idc",
        "tbh", "imo", "imho", "btw"
    };

    private static readonly HashSet<string> GameplayCallouts = new(StringComparer.OrdinalIgnoreCase)
    {
        // Helldivers gameplay callouts (English words)
        "run", "help", "ammo", "revive", "res", "respawn", "go", "wait", "hold",
        "ready", "stop", "back", "careful", "watch out", "cover", "stay",
        "clear", "done", "safe", "danger", "mine", "mines",
        "good job", "well played", "nice", "good", "great", "bad", "fun", "cool",
        "dude", "man", "bro", "mate", "team", "thank you", "welcome", "please", "hello"
    };

    private static readonly HashSet<string> UniversalEmoticons = new(StringComparer.OrdinalIgnoreCase)
    {
        ":)", "(:", ":-)", "(-:",
        ":(", "):", ":-(", ")-:",
        ":d", "d:", ":-d",
        ":p", ":-p",
        ";)", "(;", ";-)", "(-;",
        ":o", ":-o",
        "xd", "x-d",
        "-_-", "-.-", "^^", "^_^",
        "o_o", "o.o", "o_0", "0_o",
        "<3", "</3",
        @"¯\_(ツ)_/¯", @"\o/",
        "!", "!!", "!!!", "?", "??", "???", "?!", "!?",
        ".", "..", "...", "+", "++", "+++"
    };

    /// <summary>
    /// Returns true if the message is an acronym, abbreviation, text emoticon, or emoji/symbol.
    /// These messages are language-neutral and must not be attributed to the user's target language,
    /// preserving the last genuine conversational language for outgoing auto-replies.
    /// </summary>
    public static bool IsAcronymAbbreviationOrEmoji(string? message)
    {
        if (string.IsNullOrWhiteSpace(message)) return false;

        string rawTrimmed = message.Trim();

        // 1. Check if the message contains NO letters at all (e.g. "👍", "😂😂", "❤️", ":)", ":D", "???", "!!", "+1")
        if (!ContainsLetters(rawTrimmed))
        {
            return true;
        }

        string lower = rawTrimmed.ToLowerInvariant();

        // 2. Direct emoticon match across casing (e.g. "xd", "x-d", "o_o")
        if (UniversalEmoticons.Contains(lower))
        {
            return true;
        }

        // 3. Direct universal acronym/abbreviation match (e.g. "gg", "lol", "ty", "afk", "brb")
        if (UniversalAcronyms.Contains(lower))
        {
            return true;
        }

        // 4. Strip surrounding boundary punctuation (e.g. "gg!", "ty...", "xd!", "lol?")
        string stripped = StripBoundaryPunctuation(lower);
        if (!string.IsNullOrEmpty(stripped) && stripped != lower)
        {
            if (UniversalEmoticons.Contains(stripped) || UniversalAcronyms.Contains(stripped))
            {
                return true;
            }
        }

        string toCheck = stripped.Length > 0 ? stripped : lower;

        // 5. Repeated character acronyms (e.g. "loool", "sooorry", "okkk", "ggs", "yesss", "nooo")
        if (IsRepeatedAcronymOrCallout(toCheck))
        {
            return true;
        }

        // 6. Laughter patterns (e.g. "hahaha", "hehe", "jajaja", "kkkk", "huehue")
        if (IsLaughter(toCheck))
        {
            return true;
        }

        // 7. Multi-word acronym/emoticon combos (e.g. "gg :)", "lol ty", "ok ty")
        var words = toCheck.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        if (words.Length > 1 && words.All(w =>
            !ContainsLetters(w) ||
            UniversalEmoticons.Contains(w) ||
            UniversalAcronyms.Contains(w) ||
            IsRepeatedAcronymOrCallout(w) ||
            IsLaughter(w)))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Checks if an incoming message is a recognized universal callout, gamer shorthand, or emoticon
    /// that can be resolved locally at 0ms latency without consuming API quota.
    /// Comparisons are case-insensitive and tolerate surrounding boundary punctuation.
    /// </summary>
    public static bool TryFastPath(string? message, string targetLanguage, out TranslationResult? result)
    {
        result = null;
        if (string.IsNullOrWhiteSpace(message)) return false;

        string rawTrimmed = message.Trim();

        // 1. Language-neutral acronyms, abbreviations, emoticons, and emojis (resolved as "-" neutral across all target languages)
        if (IsAcronymAbbreviationOrEmoji(rawTrimmed))
        {
            result = TranslationResult.Ok(rawTrimmed, "-", 0);
            return true;
        }

        bool isTargetEnglish = LanguageMatcher.IsSameLanguage(targetLanguage, "English");
        string lower = rawTrimmed.ToLowerInvariant();
        string stripped = StripBoundaryPunctuation(lower);
        string toCheck = stripped.Length > 0 ? stripped : lower;

        // 2. English gameplay callouts (e.g. "run", "help", "ammo", "watch out") when target language is English
        if (isTargetEnglish)
        {
            if (GameplayCallouts.Contains(lower) || GameplayCallouts.Contains(toCheck))
            {
                result = TranslationResult.Ok(rawTrimmed, "English", 0);
                return true;
            }

            if (IsRepeatedGameplayCallout(toCheck))
            {
                result = TranslationResult.Ok(rawTrimmed, "English", 0);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Normalizes an AI translation result when the translated message body is substantially identical
    /// to the original message. If the text was returned unaltered by the AI, it indicates that the
    /// input text was already in the target language and the foreign detection was an LLM hallucination.
    /// </summary>
    public static TranslationResult NormalizeIdenticalTranslation(
        TranslationResult result,
        string? originalMessage,
        string targetLanguage)
    {
        if (result == null || !result.Success || string.IsNullOrWhiteSpace(originalMessage))
        {
            return result!;
        }

        var (prefixLang, cleanBody) = ExtractTranslationBody(result.TranslatedText);

        if (AreMessagesSubstantiallyIdentical(cleanBody, originalMessage))
        {
            // If the message is an acronym, abbreviation, or emoji, preserve it as language-neutral
            if (IsAcronymAbbreviationOrEmoji(originalMessage))
            {
                return TranslationResult.Ok(cleanBody, "-", result.LatencyMs);
            }

            bool isTargetEnglish = LanguageMatcher.IsSameLanguage(targetLanguage, "English");

            if (isTargetEnglish || !string.IsNullOrWhiteSpace(targetLanguage))
            {
                return TranslationResult.Ok(cleanBody, targetLanguage, result.LatencyMs);
            }
        }

        return result;
    }

    /// <summary>
    /// Extracts the translation content from a prefixed string like "(Russian): watch top left".
    /// </summary>
    public static (string? ExtractedLanguage, string CleanBody) ExtractTranslationBody(string? translatedText)
    {
        if (string.IsNullOrWhiteSpace(translatedText))
        {
            return (null, string.Empty);
        }

        var trimmed = translatedText.Trim();
        var match = Regex.Match(trimmed, @"^\(([^)]+)\):\s*(.*)$", RegexOptions.Singleline);
        if (match.Success)
        {
            string lang = match.Groups[1].Value.Trim();
            string body = match.Groups[2].Value.Trim();
            return (lang, body);
        }

        return (null, trimmed);
    }

    /// <summary>
    /// Compares two strings case-insensitively and tolerates minor boundary punctuation differences.
    /// </summary>
    public static bool AreMessagesSubstantiallyIdentical(string? a, string? b)
    {
        if (string.IsNullOrWhiteSpace(a) || string.IsNullOrWhiteSpace(b)) return false;

        string normA = a.Trim();
        string normB = b.Trim();

        if (string.Equals(normA, normB, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        string strippedA = StripBoundaryPunctuation(normA);
        string strippedB = StripBoundaryPunctuation(normB);

        return string.Equals(strippedA, strippedB, StringComparison.OrdinalIgnoreCase);
    }

    private static bool ContainsLetters(string s)
    {
        foreach (var rune in s.EnumerateRunes())
        {
            if (Rune.IsLetter(rune))
            {
                return true;
            }
        }
        return false;
    }

    private static string StripBoundaryPunctuation(string s)
    {
        return s.Trim('!', '?', '.', ',', '~', '*', ' ', '"', '\'');
    }

    private static bool IsLaughter(string lower)
    {
        if (lower.Length < 2) return false;

        // Matches laughter chains like "haha", "hahaha", "hehe", "hihi", "jaja", "jajaja", "huehue", "kkkk", "wwww"
        if (Regex.IsMatch(lower, @"^(ha|he|hi|ho|ja|kk|hue|w)+h?a?$", RegexOptions.IgnoreCase))
        {
            return true;
        }

        if (Regex.IsMatch(lower, @"^[k5w]{3,}$", RegexOptions.IgnoreCase))
        {
            return true;
        }

        return false;
    }

    private static bool IsRepeatedAcronymOrCallout(string lower)
    {
        if (lower.Length < 3) return false;

        // 1. Collapse characters repeated 3 or more times (e.g. "loool" -> "lol", "sooorry" -> "sorry", "yeees" -> "yes")
        var collapsed3 = Regex.Replace(lower, @"(.)\1{2,}", "$1");
        if (UniversalAcronyms.Contains(collapsed3) || UniversalEmoticons.Contains(collapsed3)) return true;

        // 2. Collapse all consecutive repeated characters (e.g. "okkk" -> "ok")
        var collapsedAll = Regex.Replace(lower, @"(.)\1+", "$1");
        if (UniversalAcronyms.Contains(collapsedAll) || UniversalEmoticons.Contains(collapsedAll)) return true;

        if (collapsedAll == "gg" || lower == "ggs") return true;

        return false;
    }

    private static bool IsRepeatedGameplayCallout(string lower)
    {
        if (lower.Length < 3) return false;

        var collapsed3 = Regex.Replace(lower, @"(.)\1{2,}", "$1");
        if (GameplayCallouts.Contains(collapsed3)) return true;

        var collapsedAll = Regex.Replace(lower, @"(.)\1+", "$1");
        return GameplayCallouts.Contains(collapsedAll);
    }
}
