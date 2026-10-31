using System;
using System.Collections.Generic;
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
    private static readonly HashSet<string> UniversalCallouts = new(StringComparer.OrdinalIgnoreCase)
    {
        // Common gamer greetings & farewells
        "hi", "hello", "hey", "yo", "bye", "cya", "bb",

        // Acknowledgments & confirmations
        "ok", "okay", "k", "kk", "yes", "no", "yep", "yeah", "nope", "nah",

        // Politeness & courtesy
        "sorry", "sry", "pls", "plz", "please", "ty", "thx", "thanks", "thank you",
        "np", "yw", "welcome",

        // Game callouts & performance
        "gg", "ggs", "gj", "good job", "wp", "well played", "nice", "good", "great",
        "bad", "fun", "cool", "ez",

        // Helldivers gameplay callouts
        "run", "help", "ammo", "revive", "res", "respawn", "go", "wait", "hold",
        "ready", "rdy", "stop", "back", "careful", "watch out", "cover", "stay",
        "clear", "done", "safe", "danger", "mine", "mines",

        // Acronyms & common chat shorthand
        "lol", "lmao", "rofl", "omg", "wtf", "wth", "afk", "brb", "idk", "idc",
        "dif", "diff", "dude", "man", "bro", "mate", "team"
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
    /// Checks if an incoming message is a recognized universal callout, gamer shorthand, or emoticon
    /// that can be resolved locally at 0ms latency without consuming API quota.
    /// Comparisons are case-insensitive and tolerate surrounding boundary punctuation.
    /// </summary>
    public static bool TryFastPath(string? message, string targetLanguage, out TranslationResult? result)
    {
        result = null;
        if (string.IsNullOrWhiteSpace(message)) return false;

        bool isTargetEnglish = LanguageMatcher.IsSameLanguage(targetLanguage, "English");

        string rawTrimmed = message.Trim();
        string lower = rawTrimmed.ToLowerInvariant();

        // 1. Direct emoticon match across casing (e.g. ":)", ":D", "XD", "-_-", "^^")
        if (UniversalEmoticons.Contains(lower))
        {
            result = TranslationResult.Ok(rawTrimmed, targetLanguage, 0);
            return true;
        }

        // 2. Direct callout match across casing (e.g. "GG", "ty", "ok", "sorry", "fun")
        if (isTargetEnglish && UniversalCallouts.Contains(lower))
        {
            result = TranslationResult.Ok(rawTrimmed, targetLanguage, 0);
            return true;
        }

        // 3. Strip surrounding boundary punctuation (e.g. "GG!", "ty...", "ok.", "sorry!", "XD!")
        string stripped = StripBoundaryPunctuation(lower);
        if (!string.IsNullOrEmpty(stripped) && stripped != lower)
        {
            if (UniversalEmoticons.Contains(stripped))
            {
                result = TranslationResult.Ok(rawTrimmed, targetLanguage, 0);
                return true;
            }

            if (isTargetEnglish && UniversalCallouts.Contains(stripped))
            {
                result = TranslationResult.Ok(rawTrimmed, targetLanguage, 0);
                return true;
            }
        }

        // 4. Repeated character callouts (e.g. "ggs", "okkk", "yesss", "nooo", "sooorry")
        if (isTargetEnglish && IsRepeatedCallout(stripped.Length > 0 ? stripped : lower))
        {
            result = TranslationResult.Ok(rawTrimmed, targetLanguage, 0);
            return true;
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

    private static string StripBoundaryPunctuation(string s)
    {
        return s.Trim('!', '?', '.', ',', '~', '*', ' ', '"', '\'');
    }

    private static bool IsRepeatedCallout(string lower)
    {
        if (lower.Length < 3) return false;

        // 1. Collapse characters repeated 3 or more times (e.g. "sooorry" -> "sorry", "yeees" -> "yes", "nooo" -> "no")
        var collapsed3 = Regex.Replace(lower, @"(.)\1{2,}", "$1");
        if (UniversalCallouts.Contains(collapsed3)) return true;

        // 2. Collapse all consecutive repeated characters (e.g. "okkk" -> "ok")
        var collapsedAll = Regex.Replace(lower, @"(.)\1+", "$1");
        return UniversalCallouts.Contains(collapsedAll);
    }
}
