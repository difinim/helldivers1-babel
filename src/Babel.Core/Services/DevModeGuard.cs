using System;
using System.IO;

namespace Babel.Core.Services;

/// <summary>
/// Detects if development mode is enabled by strictly checking for the existence
/// of a 'BABEL_DEV_MODE' file in up to 3 ancestor parent directories (.., ..\.., ..\..\..).
/// When present, auto-update checks and binary replacements are completely bypassed.
/// </summary>
public static class DevModeGuard
{
    public const string DevModeFileName = "BABEL_DEV_MODE";

    /// <summary>
    /// Checks whether the exact file 'BABEL_DEV_MODE' exists in the 1st, 2nd, or 3rd parent directory.
    /// </summary>
    /// <param name="baseDirectory">The root directory of the application binaries.</param>
    /// <returns>True if BABEL_DEV_MODE was found within 3 ancestor directories; false otherwise.</returns>
    public static bool IsDevModeActive(string baseDirectory)
    {
        if (string.IsNullOrWhiteSpace(baseDirectory) || !Directory.Exists(baseDirectory))
        {
            return false;
        }

        var current = new DirectoryInfo(baseDirectory);
        for (int depth = 1; depth <= 3; depth++)
        {
            current = current.Parent;
            if (current == null)
            {
                break;
            }

            var devFilePath = Path.Combine(current.FullName, DevModeFileName);
            if (File.Exists(devFilePath))
            {
                BabelLog.Info("DevModeGuard", $"BABEL_DEV_MODE detected at ancestor level {depth} ('{devFilePath}'). Auto-update disabled.");
                return true;
            }
        }

        return false;
    }
}
