using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Babel.Core.Models;

namespace Babel.Core.Services;

/// <summary>
/// Provides high-performance, asynchronous batch persistence of translation table logs to disk (chat_history.jsonl)
/// with zero impact on gameplay or frame rate.
/// </summary>
public class PersistentChatLogService : IAsyncDisposable, IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false
    };

    private readonly string _filePath;
    private readonly Channel<TranslationLogEntry> _channel;
    private readonly CancellationTokenSource _cts = new();
    private readonly Task _workerTask;
    private readonly object _fileLock = new();
    private int _limitMb;
    private bool _isDisposed;

    public string FilePath => _filePath;

    public int LimitMb
    {
        get => Volatile.Read(ref _limitMb);
        set
        {
            int clamped = Math.Clamp(value, 0, 100);
            Volatile.Write(ref _limitMb, clamped);
            if (clamped == 0)
            {
                // When set to 0 MB, immediately discard pending queue
                DrainPendingQueue();
            }
        }
    }

    public PersistentChatLogService(string? customPath = null, int limitMb = 1)
    {
        _limitMb = Math.Clamp(limitMb, 0, 100);

        if (!string.IsNullOrWhiteSpace(customPath))
        {
            _filePath = customPath;
        }
        else
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var folder = Path.Combine(appData, "Helldivers1Babel");
            _filePath = Path.Combine(folder, "chat_history.jsonl");
        }

        _channel = Channel.CreateUnbounded<TranslationLogEntry>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        });

        _workerTask = Task.Run(ProcessQueueAsync);
    }

    /// <summary>
    /// Non-blocking, lightweight enqueue (<1 µs) for chat log entries.
    /// Safe to call from any thread or game hook callback.
    /// </summary>
    public void Enqueue(TranslationLogEntry entry)
    {
        if (LimitMb <= 0 || _isDisposed)
        {
            return;
        }

        _channel.Writer.TryWrite(entry);
    }

    /// <summary>
    /// Loads recent messages from disk in reverse chronological order (newest first)
    /// suitable for direct display in the Translation & Logs table.
    /// </summary>
    public List<TranslationLogEntry> LoadRecent(int maxEntries = 500)
    {
        var entries = new List<TranslationLogEntry>();

        if (LimitMb <= 0 || !File.Exists(_filePath))
        {
            return entries;
        }

        try
        {
            lock (_fileLock)
            {
                if (!File.Exists(_filePath)) return entries;

                using var stream = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var reader = new StreamReader(stream);

                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    try
                    {
                        var item = JsonSerializer.Deserialize<TranslationLogEntry>(line, JsonOptions);
                        if (item != null)
                        {
                            entries.Add(item);
                        }
                    }
                    catch
                    {
                        // Safely skip any corrupted line
                    }
                }
            }

            // File stores chronologically (oldest at top). Reverse so index 0 is newest.
            entries.Reverse();

            if (maxEntries > 0 && entries.Count > maxEntries)
            {
                entries.RemoveRange(maxEntries, entries.Count - maxEntries);
            }
        }
        catch (Exception ex)
        {
            BabelLog.Warn("PersistentChatLog", $"Failed to load chat history: {ex.Message}");
        }

        return entries;
    }

    /// <summary>
    /// Clears all persistent logs from disk and memory.
    /// </summary>
    public async Task ClearAsync()
    {
        DrainPendingQueue();

        await Task.Run(() =>
        {
            lock (_fileLock)
            {
                try
                {
                    if (File.Exists(_filePath))
                    {
                        File.Delete(_filePath);
                    }
                }
                catch (Exception ex)
                {
                    BabelLog.Warn("PersistentChatLog", $"Failed to delete persistent chat file: {ex.Message}");
                }
            }
        });
    }

    /// <summary>
    /// Background batch processing loop. Consumes queued log entries in batches and appends to disk.
    /// </summary>
    private async Task ProcessQueueAsync()
    {
        var batch = new List<TranslationLogEntry>(64);

        try
        {
            while (await _channel.Reader.WaitToReadAsync(_cts.Token))
            {
                batch.Clear();

                // Consume first available entry
                if (_channel.Reader.TryRead(out var firstEntry))
                {
                    batch.Add(firstEntry);
                }

                // Burst batching: pull up to 64 items immediately available
                while (batch.Count < 64 && _channel.Reader.TryRead(out var nextEntry))
                {
                    batch.Add(nextEntry);
                }

                if (batch.Count > 0 && LimitMb > 0)
                {
                    await WriteBatchToFileAsync(batch);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Worker is shutting down
        }
        catch (Exception ex)
        {
            BabelLog.Error("PersistentChatLog", "Background queue worker encountered error", ex);
        }
        finally
        {
            // Drain remaining items on shutdown if persistence is enabled
            batch.Clear();
            while (_channel.Reader.TryRead(out var remaining))
            {
                batch.Add(remaining);
            }

            if (batch.Count > 0 && LimitMb > 0)
            {
                try
                {
                    await WriteBatchToFileAsync(batch);
                }
                catch
                {
                    // Best-effort flush on shutdown
                }
            }
        }
    }

    private async Task WriteBatchToFileAsync(List<TranslationLogEntry> batch)
    {
        await Task.Run(() =>
        {
            lock (_fileLock)
            {
                try
                {
                    var dir = Path.GetDirectoryName(_filePath);
                    if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    using var stream = new FileStream(_filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                    using var writer = new StreamWriter(stream);

                    foreach (var entry in batch)
                    {
                        var json = JsonSerializer.Serialize(entry, JsonOptions);
                        writer.WriteLine(json);
                    }

                    writer.Flush();
                }
                catch (Exception ex)
                {
                    BabelLog.Warn("PersistentChatLog", $"Failed to append log batch: {ex.Message}");
                }

                // Check size limit and prune if exceeded
                EnforceSizeLimitLocked();
            }
        });
    }

    /// <summary>
    /// Checks if the file size exceeds LimitMb. If so, prunes oldest lines to keep size around 80% of limit.
    /// Must be called inside _fileLock.
    /// </summary>
    private void EnforceSizeLimitLocked()
    {
        int limit = LimitMb;
        if (limit <= 0)
        {
            try
            {
                if (File.Exists(_filePath)) File.Delete(_filePath);
            }
            catch { }
            return;
        }

        try
        {
            if (!File.Exists(_filePath)) return;

            var fileInfo = new FileInfo(_filePath);
            long maxBytes = (long)limit * 1024 * 1024;

            if (fileInfo.Length <= maxBytes) return;

            long targetBytes = (long)(maxBytes * 0.80); // Keep 80% of limit to avoid continuous pruning
            var lines = File.ReadAllLines(_filePath);

            long accumulatedBytes = 0;
            int startIndex = lines.Length;

            for (int i = lines.Length - 1; i >= 0; i--)
            {
                int lineByteCount = System.Text.Encoding.UTF8.GetByteCount(lines[i]) + Environment.NewLine.Length;
                if (accumulatedBytes + lineByteCount > targetBytes)
                {
                    break;
                }
                accumulatedBytes += lineByteCount;
                startIndex = i;
            }

            var tempPath = _filePath + ".tmp";
            using (var writer = new StreamWriter(tempPath, false, System.Text.Encoding.UTF8))
            {
                for (int i = startIndex; i < lines.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(lines[i]))
                    {
                        writer.WriteLine(lines[i]);
                    }
                }
            }

            File.Move(tempPath, _filePath, overwrite: true);
            BabelLog.Info("PersistentChatLog", $"Pruned chat history file from {fileInfo.Length} bytes to ~{accumulatedBytes} bytes to stay within {limit} MB limit.");
        }
        catch (Exception ex)
        {
            BabelLog.Warn("PersistentChatLog", $"Failed to prune chat history file: {ex.Message}");
        }
    }

    private void DrainPendingQueue()
    {
        while (_channel.Reader.TryRead(out _)) { }
    }

    /// <summary>
    /// Flushes any pending batch and completes worker. Called on application exit.
    /// </summary>
    public async Task FlushAndCompleteAsync()
    {
        _channel.Writer.TryComplete();
        try
        {
            await _workerTask.WaitAsync(TimeSpan.FromSeconds(3)).ConfigureAwait(false);
        }
        catch
        {
            // Ignore timeouts on shutdown
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_isDisposed) return;
        _isDisposed = true;

        await FlushAndCompleteAsync().ConfigureAwait(false);
        _cts.Cancel();
        _cts.Dispose();
        GC.SuppressFinalize(this);
    }

    public void Dispose()
    {
        if (_isDisposed) return;
        _isDisposed = true;

        try
        {
            Task.Run(async () => await FlushAndCompleteAsync().ConfigureAwait(false)).Wait(TimeSpan.FromSeconds(3));
        }
        catch { }

        _cts.Cancel();
        _cts.Dispose();
        GC.SuppressFinalize(this);
    }
}
