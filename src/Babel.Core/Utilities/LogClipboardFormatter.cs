using System;
using System.Collections.Generic;
using System.Linq;
using Babel.Core.Models;

namespace Babel.Core.Utilities;

/// <summary>
/// Utility for formatting log entries for clipboard copy operations.
/// </summary>
public static class LogClipboardFormatter
{
    /// <summary>
    /// Formats a single translation log entry as tab-separated values matching table columns:
    /// Date (if present), Time, Player, Detected, Original Message, Translated Message, Latency.
    /// </summary>
    public static string FormatTabSeparatedRow(TranslationLogEntry? entry)
    {
        if (entry == null) return string.Empty;
        if (!string.IsNullOrEmpty(entry.Date))
        {
            return $"{entry.Date}\t{entry.Time}\t{entry.Author}\t{entry.SourceLang}\t{entry.OriginalMessage}\t{entry.TranslatedMessage}\t{entry.Latency}";
        }
        return $"{entry.Time}\t{entry.Author}\t{entry.SourceLang}\t{entry.OriginalMessage}\t{entry.TranslatedMessage}\t{entry.Latency}";
    }

    /// <summary>
    /// Formats multiple translation log entries as tab-separated rows.
    /// </summary>
    public static string FormatTabSeparatedRows(IEnumerable<TranslationLogEntry>? entries)
    {
        if (entries == null) return string.Empty;
        return string.Join(Environment.NewLine, entries.Where(e => e != null).Select(FormatTabSeparatedRow));
    }

    /// <summary>
    /// Formats a single translation log entry as a clean, readable single-line string:
    /// e.g. "[12:34:56] PlayerName: Translated (Original: OriginalText)" or "[12:34:56] PlayerName: Message".
    /// </summary>
    public static string FormatSingleLine(TranslationLogEntry? entry)
    {
        if (entry == null) return string.Empty;

        var author = string.IsNullOrWhiteSpace(entry.Author) ? "Unknown" : entry.Author.Trim();
        var original = entry.OriginalMessage?.Trim() ?? string.Empty;
        var translated = entry.TranslatedMessage?.Trim() ?? string.Empty;

        bool hasTranslation = !string.IsNullOrEmpty(translated) && !string.Equals(translated, "-", StringComparison.Ordinal);

        string messageText;
        if (hasTranslation && !string.IsNullOrEmpty(original) &&
            !string.Equals(translated, original, StringComparison.OrdinalIgnoreCase))
        {
            messageText = $"{translated} (Original: {original})";
        }
        else if (hasTranslation)
        {
            messageText = translated;
        }
        else
        {
            messageText = original;
        }

        string timestampPrefix;
        if (!string.IsNullOrEmpty(entry.Date) && !string.IsNullOrEmpty(entry.Time))
        {
            timestampPrefix = $"[{entry.Date} {entry.Time}] ";
        }
        else if (!string.IsNullOrEmpty(entry.Time))
        {
            timestampPrefix = $"[{entry.Time}] ";
        }
        else
        {
            timestampPrefix = string.Empty;
        }

        return $"{timestampPrefix}{author}: {messageText}";
    }

    /// <summary>
    /// Formats multiple translation log entries into single lines joined by newlines.
    /// </summary>
    public static string FormatSingleLines(IEnumerable<TranslationLogEntry>? entries)
    {
        if (entries == null) return string.Empty;
        return string.Join(Environment.NewLine, entries.Where(e => e != null).Select(FormatSingleLine));
    }

    /// <summary>
    /// Extracts player names from entries, joined by newlines.
    /// </summary>
    public static string FormatPlayerNames(IEnumerable<TranslationLogEntry>? entries)
    {
        if (entries == null) return string.Empty;
        return string.Join(Environment.NewLine, entries.Where(e => e != null && !string.IsNullOrWhiteSpace(e.Author)).Select(e => e.Author.Trim()));
    }

    /// <summary>
    /// Extracts original messages from entries, joined by newlines.
    /// </summary>
    public static string FormatOriginalMessages(IEnumerable<TranslationLogEntry>? entries)
    {
        if (entries == null) return string.Empty;
        return string.Join(Environment.NewLine, entries.Where(e => e != null && !string.IsNullOrWhiteSpace(e.OriginalMessage)).Select(e => e.OriginalMessage));
    }

    /// <summary>
    /// Extracts translated messages from entries, joined by newlines.
    /// </summary>
    public static string FormatTranslatedMessages(IEnumerable<TranslationLogEntry>? entries)
    {
        if (entries == null) return string.Empty;
        return string.Join(Environment.NewLine, entries.Where(e => e != null && !string.IsNullOrWhiteSpace(e.TranslatedMessage)).Select(e => e.TranslatedMessage));
    }
}
