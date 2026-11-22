using System;
using System.Collections.Generic;
using Babel.Core.Models;
using Babel.Core.Utilities;
using Xunit;

namespace Babel.Core.Tests;

public class LogExportHelperTests
{
    [Fact]
    public void BuildExportHeader_WithoutSearchQuery_ProducesStandardHeader()
    {
        string header = LogExportHelper.BuildExportHeader("2026-11-22 02:40:00", 5);

        Assert.Contains("HELLDIVERS 1 BABEL - CHAT LOGS EXPORT", header);
        Assert.Contains("Exported Date : 2026-11-22 02:40:00", header);
        Assert.Contains("Total Messages: 5", header);
        Assert.DoesNotContain("Search Filter", header);
    }

    [Fact]
    public void BuildExportHeader_WithSearchQuery_ProducesFilteredHeader()
    {
        string header = LogExportHelper.BuildExportHeader("2026-11-22 02:40:00", 3, searchQuery: "diver", totalUnfilteredCount: 10);

        Assert.Contains("Exported Date : 2026-11-22 02:40:00", header);
        Assert.Contains("Search Filter : \"diver\"", header);
        Assert.Contains("Total Messages: 3 (Filtered from 10)", header);
    }

    [Fact]
    public void GenerateExportText_WithSelectedEntries_OutputsExpectedStructure()
    {
        var entries = new List<TranslationLogEntry>
        {
            new TranslationLogEntry
            {
                Date = "2026-11-22",
                Time = "14:20:00",
                Author = "Cadet_1",
                SourceLang = "Russian",
                OriginalMessage = "Привет",
                TranslatedMessage = "Hello",
                Latency = "120ms"
            },
            new TranslationLogEntry
            {
                Date = "2026-11-22",
                Time = "14:21:00",
                Author = "Admiral_2",
                SourceLang = "French",
                OriginalMessage = "Bonjour",
                TranslatedMessage = "Hello",
                Latency = "95ms"
            }
        };

        string export = LogExportHelper.GenerateExportText(entries, "2026-11-22 14:25:00", emptyMessage: "No chat messages selected.");

        Assert.Contains("Exported Date : 2026-11-22 14:25:00", export);
        Assert.Contains("Total Messages: 2", export);
        Assert.Contains("[2026-11-22 14:20:00] Cadet_1 [Russian] (Latency: 120ms)", export);
        Assert.Contains("  Original  : Привет", export);
        Assert.Contains("  Translated: Hello", export);
        Assert.Contains("[2026-11-22 14:21:00] Admiral_2 [French] (Latency: 95ms)", export);
        Assert.Contains("  Original  : Bonjour", export);
    }

    [Fact]
    public void GenerateExportText_WithEmptyList_OutputsSpecifiedEmptyMessage()
    {
        var entries = new List<TranslationLogEntry>();

        string export = LogExportHelper.GenerateExportText(entries, "2026-11-22 14:25:00", emptyMessage: "No chat messages selected.");

        Assert.Contains("Total Messages: 0", export);
        Assert.Contains("No chat messages selected.", export);
    }

    [Fact]
    public void GenerateExportText_WithNullList_OutputsSpecifiedEmptyMessage()
    {
        string export = LogExportHelper.GenerateExportText(null, "2026-11-22 14:25:00", emptyMessage: "No chat messages selected.");

        Assert.Contains("Total Messages: 0", export);
        Assert.Contains("No chat messages selected.", export);
    }

    [Fact]
    public void GenerateExportText_WhenSourceLangIsEmpty_OmitsLanguageBrackets()
    {
        var entries = new List<TranslationLogEntry>
        {
            new TranslationLogEntry
            {
                Date = "2026-11-22",
                Time = "15:00:00",
                Author = "Host",
                SourceLang = "",
                OriginalMessage = "Move out",
                TranslatedMessage = "Move out",
                Latency = "10ms"
            }
        };

        string export = LogExportHelper.GenerateExportText(entries, "2026-11-22 15:05:00");

        Assert.Contains("[2026-11-22 15:00:00] Host (Latency: 10ms)", export);
        Assert.DoesNotContain("[]", export);
    }

    [Fact]
    public void GenerateExportText_WhenDateIsEmpty_FallsBackToTimeOnly()
    {
        var entries = new List<TranslationLogEntry>
        {
            new TranslationLogEntry
            {
                Date = "",
                Time = "15:00:00",
                Author = "Host",
                SourceLang = "English",
                OriginalMessage = "Need ammo",
                TranslatedMessage = "Need ammo",
                Latency = "15ms"
            }
        };

        string export = LogExportHelper.GenerateExportText(entries, "2026-11-22 15:05:00");

        Assert.Contains("[15:00:00] Host [English] (Latency: 15ms)", export);
    }

    [Fact]
    public void GenerateExportText_SkipsNullElementsInList()
    {
        var entries = new List<TranslationLogEntry?>
        {
            null,
            new TranslationLogEntry
            {
                Date = "2026-11-22",
                Time = "15:00:00",
                Author = "Host",
                SourceLang = "English",
                OriginalMessage = "Test",
                TranslatedMessage = "Test",
                Latency = "5ms"
            },
            null
        };

        string export = LogExportHelper.GenerateExportText(entries!, "2026-11-22 15:05:00");

        Assert.Contains("Total Messages: 1", export);
        Assert.Contains("[2026-11-22 15:00:00] Host [English] (Latency: 5ms)", export);
    }
}
