using System;
using System.Runtime.CompilerServices;
using Babel.Core.Models;

namespace Babel.Core.Utilities;

/// <summary>
/// Provides fast, zero-allocation, case-insensitive multi-column search filtering for chat log entries.
/// </summary>
public static class LogSearchFilter
{
    private static readonly string[] EmptyTokens = Array.Empty<string>();

    /// <summary>
    /// Pre-tokenizes a search query string once, splitting on whitespace.
    /// </summary>
    public static string[] Tokenize(string? searchQuery)
    {
        if (string.IsNullOrWhiteSpace(searchQuery))
        {
            return EmptyTokens;
        }

        return searchQuery.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    }

    /// <summary>
    /// Determines whether the specified translation log entry matches pre-tokenized query terms.
    /// Performs zero string allocations and short-circuits on the first non-matching token.
    /// </summary>
    public static bool Matches(TranslationLogEntry? entry, string[]? tokens)
    {
        if (entry == null) return false;
        if (tokens == null || tokens.Length == 0) return true;

        for (int i = 0; i < tokens.Length; i++)
        {
            string token = tokens[i];

            // Check columns in order of highest probability of match
            if (!ColumnContains(entry.OriginalMessage, token) &&
                !ColumnContains(entry.TranslatedMessage, token) &&
                !ColumnContains(entry.Author, token) &&
                !ColumnContains(entry.Date, token) &&
                !ColumnContains(entry.Time, token) &&
                !ColumnContains(entry.SourceLang, token) &&
                !ColumnContains(entry.Latency, token))
            {
                return false; // Fast exit: this token is missing from all columns
            }
        }

        return true;
    }

    /// <summary>
    /// Determines whether the specified translation log entry matches the search query string.
    /// </summary>
    public static bool Matches(TranslationLogEntry? entry, string? searchQuery)
    {
        if (entry == null) return false;
        if (string.IsNullOrWhiteSpace(searchQuery)) return true;

        var tokens = Tokenize(searchQuery);
        return Matches(entry, tokens);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool ColumnContains(string? columnValue, string token)
    {
        return columnValue != null && columnValue.Contains(token, StringComparison.OrdinalIgnoreCase);
    }
}
