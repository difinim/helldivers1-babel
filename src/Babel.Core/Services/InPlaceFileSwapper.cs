using System;
using System.IO;

namespace Babel.Core.Services;

/// <summary>
/// Provides atomic in-place file replacement for executing applications on Windows.
/// Renames locked binaries (*.exe, *.dll) to *.old so new files can be placed immediately,
/// and purges leftover *.old files on subsequent startups.
/// </summary>
public static class InPlaceFileSwapper
{
    public const string OldFileExtension = ".old";

    /// <summary>
    /// Purges all leftover *.old files from previous updates within the specified directory.
    /// Failures to delete individual files are trapped and logged without throwing exceptions.
    /// </summary>
    /// <param name="targetDirectory">The root directory of the application.</param>
    /// <returns>The number of old files successfully removed.</returns>
    public static int PurgeOldFiles(string targetDirectory)
    {
        if (string.IsNullOrWhiteSpace(targetDirectory) || !Directory.Exists(targetDirectory))
        {
            return 0;
        }

        int purgedCount = 0;
        try
        {
            var oldFiles = Directory.EnumerateFiles(targetDirectory, $"*{OldFileExtension}", SearchOption.AllDirectories);
            foreach (var file in oldFiles)
            {
                try
                {
                    File.Delete(file);
                    purgedCount++;
                }
                catch (Exception ex)
                {
                    BabelLog.Warn("FileSwapper", $"Could not delete leftover update file '{file}': {ex.Message}");
                }
            }

            if (purgedCount > 0)
            {
                BabelLog.Info("FileSwapper", $"Successfully purged {purgedCount} leftover update file(s).");
            }
        }
        catch (Exception ex)
        {
            BabelLog.Warn("FileSwapper", $"Failed to enumerate old update files: {ex.Message}");
        }

        return purgedCount;
    }

    /// <summary>
    /// Recursively moves and replaces files from staging directory into target directory.
    /// Existing files in the target directory are renamed with a '.old' suffix before replacement.
    /// </summary>
    /// <param name="stagingDirectory">The staging folder containing unpacked new release files.</param>
    /// <param name="targetDirectory">The application root destination directory.</param>
    /// <returns>The count of files successfully updated.</returns>
    public static int SwapFiles(string stagingDirectory, string targetDirectory)
    {
        if (string.IsNullOrWhiteSpace(stagingDirectory) || !Directory.Exists(stagingDirectory))
        {
            throw new DirectoryNotFoundException($"Staging directory not found: {stagingDirectory}");
        }

        if (string.IsNullOrWhiteSpace(targetDirectory) || !Directory.Exists(targetDirectory))
        {
            throw new DirectoryNotFoundException($"Target directory not found: {targetDirectory}");
        }

        var stagingDirInfo = new DirectoryInfo(stagingDirectory);
        int replacedCount = 0;

        foreach (var fileInfo in stagingDirInfo.EnumerateFiles("*", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(stagingDirectory, fileInfo.FullName);
            var destinationFilePath = Path.Combine(targetDirectory, relativePath);
            var destDir = Path.GetDirectoryName(destinationFilePath);

            if (!string.IsNullOrEmpty(destDir) && !Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            if (File.Exists(destinationFilePath))
            {
                var oldFilePath = destinationFilePath + OldFileExtension;
                try
                {
                    if (File.Exists(oldFilePath))
                    {
                        File.Delete(oldFilePath);
                    }
                }
                catch
                {
                    // If target.old is somehow locked, create a uniquely named .old file
                    oldFilePath = destinationFilePath + $".old.{Guid.NewGuid():N}";
                }

                try
                {
                    File.Move(destinationFilePath, oldFilePath, overwrite: true);
                }
                catch (Exception ex)
                {
                    BabelLog.Error("FileSwapper", $"Failed to rename existing file '{destinationFilePath}' to '{oldFilePath}'", ex);
                    throw;
                }
            }

            File.Copy(fileInfo.FullName, destinationFilePath, overwrite: true);
            replacedCount++;
        }

        BabelLog.Info("FileSwapper", $"In-place file swap completed. {replacedCount} file(s) updated in '{targetDirectory}'.");
        return replacedCount;
    }
}
