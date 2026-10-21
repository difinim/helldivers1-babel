using System;
using System.IO;

namespace Babel.Core.Services;

/// <summary>
/// Provides unified diagnostic logging to %APPDATA%\Helldivers1Babel\app.log.
/// </summary>
public static class BabelLog
{
    private static readonly object _fileLock = new();
    private static string? _logFilePath;
    private static bool _isEnabled = true;

    public static bool IsEnabled
    {
        get => _isEnabled;
        set => _isEnabled = value;
    }

    public static string LogDirectory
    {
        get
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(appData, "Helldivers1Babel");
        }
    }

    public static string LogFilePath
    {
        get
        {
            if (_logFilePath == null)
            {
                Directory.CreateDirectory(LogDirectory);
                _logFilePath = Path.Combine(LogDirectory, "app.log");
            }
            return _logFilePath;
        }
    }

    public static void Info(string category, string message) => Log("INFO", category, message);
    public static void Warn(string category, string message) => Log("WARN", category, message);
    public static void Error(string category, string message, Exception? ex = null)
    {
        var msg = ex != null ? $"{message} (Exception: {ex.Message})" : message;
        Log("ERROR", category, msg);
    }

    public static void Log(string level, string category, string message)
    {
        if (!_isEnabled) return;

        try
        {
            var line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [{level}] [{category}] {message}{Environment.NewLine}";
            lock (_fileLock)
            {
                File.AppendAllText(LogFilePath, line);
            }
            System.Diagnostics.Debug.Write(line);
        }
        catch
        {
            // Fail-safe: logging should never crash the application
        }
    }
}
