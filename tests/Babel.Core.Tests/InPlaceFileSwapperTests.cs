using System;
using System.IO;
using Babel.Core.Services;
using Xunit;

namespace Babel.Core.Tests;

public class InPlaceFileSwapperTests : IDisposable
{
    private readonly string _testRoot;

    public InPlaceFileSwapperTests()
    {
        _testRoot = Path.Combine(Path.GetTempPath(), "HD1_Babel_FileSwapperTests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_testRoot);
    }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(_testRoot))
            {
                Directory.Delete(_testRoot, recursive: true);
            }
        }
        catch { }
    }

    [Fact]
    public void PurgeOldFiles_DeletesOnlyOldFilesAndLeavesOthersIntact()
    {
        var appDir = Path.Combine(_testRoot, "App");
        Directory.CreateDirectory(appDir);

        var normalFile = Path.Combine(appDir, "HD1-Babel.exe");
        var oldFile1 = Path.Combine(appDir, "HD1-Babel.exe.old");
        var oldFile2 = Path.Combine(appDir, "BabelHook.dll.old");

        File.WriteAllText(normalFile, "current binary");
        File.WriteAllText(oldFile1, "old binary 1");
        File.WriteAllText(oldFile2, "old binary 2");

        var purged = InPlaceFileSwapper.PurgeOldFiles(appDir);

        Assert.Equal(2, purged);
        Assert.True(File.Exists(normalFile));
        Assert.False(File.Exists(oldFile1));
        Assert.False(File.Exists(oldFile2));
    }

    [Fact]
    public void SwapFiles_ReplacesExistingFilesAndRenamesToOld()
    {
        var appDir = Path.Combine(_testRoot, "App");
        var stagingDir = Path.Combine(_testRoot, "Staging");
        Directory.CreateDirectory(appDir);
        Directory.CreateDirectory(stagingDir);

        var targetExe = Path.Combine(appDir, "HD1-Babel.exe");
        var targetDll = Path.Combine(appDir, "BabelHook.dll");
        File.WriteAllText(targetExe, "version 1.0.0 exe");
        File.WriteAllText(targetDll, "version 1.0.0 dll");

        var stagedExe = Path.Combine(stagingDir, "HD1-Babel.exe");
        var stagedDll = Path.Combine(stagingDir, "BabelHook.dll");
        var stagedNewFile = Path.Combine(stagingDir, "NewFeature.dll");
        File.WriteAllText(stagedExe, "version 1.1.0 exe");
        File.WriteAllText(stagedDll, "version 1.1.0 dll");
        File.WriteAllText(stagedNewFile, "new feature dll");

        var swappedCount = InPlaceFileSwapper.SwapFiles(stagingDir, appDir);

        Assert.Equal(3, swappedCount);

        // Active files now have new content
        Assert.Equal("version 1.1.0 exe", File.ReadAllText(targetExe));
        Assert.Equal("version 1.1.0 dll", File.ReadAllText(targetDll));
        Assert.Equal("new feature dll", File.ReadAllText(Path.Combine(appDir, "NewFeature.dll")));

        // Backups exist with .old extension
        var oldExe = targetExe + ".old";
        var oldDll = targetDll + ".old";
        Assert.True(File.Exists(oldExe));
        Assert.True(File.Exists(oldDll));
        Assert.Equal("version 1.0.0 exe", File.ReadAllText(oldExe));
        Assert.Equal("version 1.0.0 dll", File.ReadAllText(oldDll));
    }

    [Fact]
    public void SwapFiles_WhenOldFileAlreadyExists_OverwritesOldBackupCleanly()
    {
        var appDir = Path.Combine(_testRoot, "App");
        var stagingDir = Path.Combine(_testRoot, "Staging");
        Directory.CreateDirectory(appDir);
        Directory.CreateDirectory(stagingDir);

        var targetExe = Path.Combine(appDir, "HD1-Babel.exe");
        var oldExe = targetExe + ".old";
        File.WriteAllText(targetExe, "version 1.0.0 exe");
        File.WriteAllText(oldExe, "ancient version exe");

        var stagedExe = Path.Combine(stagingDir, "HD1-Babel.exe");
        File.WriteAllText(stagedExe, "version 1.1.0 exe");

        var swappedCount = InPlaceFileSwapper.SwapFiles(stagingDir, appDir);

        Assert.Equal(1, swappedCount);
        Assert.Equal("version 1.1.0 exe", File.ReadAllText(targetExe));
        Assert.Equal("version 1.0.0 exe", File.ReadAllText(oldExe));
    }
}
