using System;
using Babel.Core.Models;

namespace Babel.Core.Utilities;

/// <summary>
/// Provides fast, case-insensitive multi-column search filtering for chat log entries.
/// </summary>
public static class LogSearchFilter
{
    /// <summary>
    /// Determines whether the specified translation log entry matches the search query.
    /// Evaluates across all columns: Date, Time, Author, SourceLang, OriginalMessage, TranslatedMessage, Latency.
    /// Multi-token queries (separated by whitespace) require all tokens to match.
    /// </summary>
    public static bool Matches(TranslationLogEntry? entry, string? searchQuery)
    {
        if (entry == null) return false;
        if (string.IsNullOrWhiteSpace(searchQuery)) return true;

        var tokens = searchQuery.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (tokens.Length == 0) return true;

        // Concatenate all column values (including formatted Date and Time respecting user configuration)
        string rowContent = $"{entry.Date} {entry.Time} {entry.Author} {entry.SourceLang} {entry.OriginalMessage} {entry.TranslatedMessage} {entry.Latency}";

        foreach (var token in tokens)
        {
            if (!rowContent.Contains(token, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        return true;
    }
}
