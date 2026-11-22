using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Babel.Core.Models;

namespace Babel.Core.Utilities;

/// <summary>
/// Utility for formatting chat log export text documents.
/// </summary>
public static class LogExportHelper
{
    /// <summary>
    /// Builds the standard header for chat logs export.
    /// </summary>
    public static string BuildExportHeader(string exportedDateTime, int totalMessages, string? searchQuery = null, int? totalUnfilteredCount = null)
    {
        var sb = new StringBuilder();
        sb.AppendLine("================================================================================");
        sb.AppendLine("  HELLDIVERS 1 BABEL - CHAT LOGS EXPORT");
        sb.AppendLine($"  Exported Date : {exportedDateTime}");

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            sb.AppendLine($"  Search Filter : \"{searchQuery.Trim()}\"");
            if (totalUnfilteredCount.HasValue)
            {
                sb.AppendLine($"  Total Messages: {totalMessages} (Filtered from {totalUnfilteredCount.Value})");
            }
            else
            {
                sb.AppendLine($"  Total Messages: {totalMessages}");
            }
        }
        else
        {
            sb.AppendLine($"  Total Messages: {totalMessages}");
        }

        sb.AppendLine("================================================================================");
        sb.AppendLine();
        return sb.ToString();
    }

    /// <summary>
    /// Generates full export text including header and formatted entries.
    /// </summary>
    public static string GenerateExportText(
        IEnumerable<TranslationLogEntry>? entries,
        string exportedDateTime,
        string? searchQuery = null,
        int? totalUnfilteredCount = null,
        string emptyMessage = "No chat messages recorded in this session.")
    {
        var sb = new StringBuilder();
        var list = entries?.Where(e => e != null).ToList() ?? new List<TranslationLogEntry>();

        sb.Append(BuildExportHeader(exportedDateTime, list.Count, searchQuery, totalUnfilteredCount));

        if (list.Count == 0)
        {
            sb.AppendLine(emptyMessage);
            return sb.ToString();
        }

        AppendLogEntries(sb, list);
        return sb.ToString();
    }

    /// <summary>
    /// Appends formatted log entry blocks to the specified string builder.
    /// </summary>
    public static void AppendLogEntries(StringBuilder sb, IEnumerable<TranslationLogEntry> entries)
    {
        foreach (var entry in entries)
        {
            if (entry == null) continue;
            string langInfo = string.IsNullOrWhiteSpace(entry.SourceLang) ? "" : $" [{entry.SourceLang}]";
            string datePrefix = string.IsNullOrWhiteSpace(entry.Date) ? entry.Time : $"{entry.Date} {entry.Time}";
            sb.AppendLine($"[{datePrefix}] {entry.Author}{langInfo} (Latency: {entry.Latency})");
            sb.AppendLine($"  Original  : {entry.OriginalMessage}");
            sb.AppendLine($"  Translated: {entry.TranslatedMessage}");
            sb.AppendLine();
        }
    }
}
