using System;
using System.IO;
using System.Threading.Tasks;
using Babel.Core.Models;
using Babel.Core.Services;
using Xunit;

namespace Babel.Core.Tests;

public class PersistentChatLogServiceTests : IDisposable
{
    private readonly string _testFilePath;

    public PersistentChatLogServiceTests()
    {
        _testFilePath = Path.Combine(Path.GetTempPath(), $"babel_test_history_{Guid.NewGuid():N}.jsonl");
    }

    public void Dispose()
    {
        try
        {
            if (File.Exists(_testFilePath)) File.Delete(_testFilePath);
            if (File.Exists(_testFilePath + ".tmp")) File.Delete(_testFilePath + ".tmp");
        }
        catch { }
    }

    [Fact]
    public async Task EnqueueAndFlush_WritesBatchCorrectly()
    {
        await using var service = new PersistentChatLogService(_testFilePath, limitMb: 1);

        service.Enqueue(new TranslationLogEntry
        {
            Time = "12:00:00",
            Author = "Helldiver1",
            SourceLang = "English",
            OriginalMessage = "Need ammo",
            TranslatedMessage = "Preciso de munição",
            Latency = "120ms",
            ColorHex = "#FFD700"
        });

        service.Enqueue(new TranslationLogEntry
        {
            Time = "12:00:05",
            Author = "Helldiver2",
            SourceLang = "Russian",
            OriginalMessage = "Враг рядом",
            TranslatedMessage = "Inimigo próximo",
            Latency = "150ms",
            ColorHex = "#00E676"
        });

        await service.FlushAndCompleteAsync();

        Assert.True(File.Exists(_testFilePath));
        var lines = await File.ReadAllLinesAsync(_testFilePath);
        Assert.Equal(2, lines.Length);
        Assert.Contains("Helldiver1", lines[0]);
        Assert.Contains("Helldiver2", lines[1]);
    }

    [Fact]
    public async Task LoadRecent_LoadsInReverseChronologicalOrder()
    {
        await using (var service = new PersistentChatLogService(_testFilePath, limitMb: 1))
        {
            service.Enqueue(new TranslationLogEntry { Time = "10:00:00", OriginalMessage = "First message" });
            service.Enqueue(new TranslationLogEntry { Time = "10:01:00", OriginalMessage = "Second message" });
            service.Enqueue(new TranslationLogEntry { Time = "10:02:00", OriginalMessage = "Third message" });
            await service.FlushAndCompleteAsync();
        }

        // New service instance simulates app restart
        await using (var newService = new PersistentChatLogService(_testFilePath, limitMb: 1))
        {
            var loaded = newService.LoadRecent(maxEntries: 10);
            Assert.Equal(3, loaded.Count);
            // Index 0 must be the newest entry (Third message)
            Assert.Equal("Third message", loaded[0].OriginalMessage);
            Assert.Equal("Second message", loaded[1].OriginalMessage);
            Assert.Equal("First message", loaded[2].OriginalMessage);
        }
    }

    [Fact]
    public async Task LimitZero_DisablesWriting()
    {
        await using var service = new PersistentChatLogService(_testFilePath, limitMb: 0);

        service.Enqueue(new TranslationLogEntry { OriginalMessage = "Should not be saved" });
        await service.FlushAndCompleteAsync();

        Assert.False(File.Exists(_testFilePath));
        var loaded = service.LoadRecent();
        Assert.Empty(loaded);
    }

    [Fact]
    public async Task ClearAsync_DeletesPersistentFileAndEmptiesHistory()
    {
        await using var service = new PersistentChatLogService(_testFilePath, limitMb: 1);

        service.Enqueue(new TranslationLogEntry { OriginalMessage = "Saved before clear" });
        await service.FlushAndCompleteAsync();

        Assert.True(File.Exists(_testFilePath));

        await service.ClearAsync();

        Assert.False(File.Exists(_testFilePath));
        var loaded = service.LoadRecent();
        Assert.Empty(loaded);
    }

    [Fact]
    public async Task LoadRecent_CorruptedLines_AreSafelyIgnored()
    {
        // Write file with valid line, corrupt line, and valid line
        await File.WriteAllLinesAsync(_testFilePath, new[]
        {
            "{\"Time\":\"01:00\",\"Author\":\"P1\",\"SourceLang\":\"EN\",\"OriginalMessage\":\"Valid 1\",\"TranslatedMessage\":\"T1\",\"Latency\":\"10ms\",\"ColorHex\":\"#FFF\"}",
            "{ this is corrupt invalid json line !@#$ }",
            "{\"Time\":\"01:02\",\"Author\":\"P2\",\"SourceLang\":\"EN\",\"OriginalMessage\":\"Valid 2\",\"TranslatedMessage\":\"T2\",\"Latency\":\"10ms\",\"ColorHex\":\"#FFF\"}"
        });

        await using var service = new PersistentChatLogService(_testFilePath, limitMb: 1);
        var loaded = service.LoadRecent(maxEntries: 10);

        Assert.Equal(2, loaded.Count);
        Assert.Equal("Valid 2", loaded[0].OriginalMessage);
        Assert.Equal("Valid 1", loaded[1].OriginalMessage);
    }

    [Fact]
    public async Task EnforceSizeLimit_PrunesOldestEntriesWhenExceeded()
    {
        // Create 1 MB limit service
        await using var service = new PersistentChatLogService(_testFilePath, limitMb: 1);

        // Pre-fill file with large text lines to simulate crossing 1 MB (1,048,576 bytes)
        var largeLine = "{\"Time\":\"00:00\",\"Author\":\"P1\",\"SourceLang\":\"EN\",\"OriginalMessage\":\"" + new string('A', 50000) + "\",\"TranslatedMessage\":\"T\",\"Latency\":\"10ms\",\"ColorHex\":\"#FFF\"}";

        // Write 25 large lines (~1.25 MB) directly to test file
        using (var sw = new StreamWriter(_testFilePath, false))
        {
            for (int i = 0; i < 25; i++)
            {
                sw.WriteLine(largeLine);
            }
            sw.WriteLine("{\"Time\":\"99:99\",\"Author\":\"P1\",\"SourceLang\":\"EN\",\"OriginalMessage\":\"LATEST_RECORD\",\"TranslatedMessage\":\"T\",\"Latency\":\"10ms\",\"ColorHex\":\"#FFF\"}");
        }

        long sizeBefore = new FileInfo(_testFilePath).Length;
        Assert.True(sizeBefore > 1024 * 1024);

        // Enqueuing a small item triggers background write and size limit enforcement
        service.Enqueue(new TranslationLogEntry { OriginalMessage = "TRIGGER_PRUNE" });
        await service.FlushAndCompleteAsync();

        var fileInfo = new FileInfo(_testFilePath);
        Assert.True(fileInfo.Length <= 1024 * 1024, $"Expected file size <= 1MB, got {fileInfo.Length} bytes");

        // Verify recent records are preserved
        var loaded = service.LoadRecent(maxEntries: 10);
        Assert.NotEmpty(loaded);
        Assert.Equal("TRIGGER_PRUNE", loaded[0].OriginalMessage);
    }

    [Fact]
    public async Task MultipleFlushAndCompleteCalls_AreIdempotentAndSafe()
    {
        await using var service = new PersistentChatLogService(_testFilePath, limitMb: 1);
        service.Enqueue(new TranslationLogEntry { OriginalMessage = "Test 1" });

        await service.FlushAndCompleteAsync();
        // Subsequent calls should complete immediately and not throw
        await service.FlushAndCompleteAsync();
        await service.FlushAndCompleteAsync();

        var loaded = service.LoadRecent();
        Assert.Single(loaded);
        Assert.Equal("Test 1", loaded[0].OriginalMessage);
    }

    [Fact]
    public void Dispose_CanBeCalledSynchronouslyWithoutDeadlock()
    {
        var service = new PersistentChatLogService(_testFilePath, limitMb: 1);
        service.Enqueue(new TranslationLogEntry { OriginalMessage = "Sync Dispose Message" });

        // Synchronous dispose must not hang or throw
        service.Dispose();

        // Calling Dispose again should also be safe (idempotent)
        service.Dispose();

        Assert.True(File.Exists(_testFilePath));
    }
}
