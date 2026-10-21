using System;
using System.IO;
using Babel.Core.Services;
using Xunit;

namespace Babel.Core.Tests;

public class DevModeGuardTests : IDisposable
{
    private readonly string _testRoot;

    public DevModeGuardTests()
    {
        _testRoot = Path.Combine(Path.GetTempPath(), "HD1_Babel_DevModeTests_" + Guid.NewGuid().ToString("N"));
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
    public void IsDevModeActive_WhenNoDevModeFile_ReturnsFalse()
    {
        // Directory hierarchy: _testRoot/L1/L2/L3/App
        var appDir = Path.Combine(_testRoot, "L1", "L2", "L3", "App");
        Directory.CreateDirectory(appDir);

        var result = DevModeGuard.IsDevModeActive(appDir);

        Assert.False(result);
    }

    [Fact]
    public void IsDevModeActive_WhenDevModeAtLevel1_ReturnsTrue()
    {
        // Level 1 parent: _testRoot/AppParent/App
        var parentDir = Path.Combine(_testRoot, "AppParent");
        var appDir = Path.Combine(parentDir, "App");
        Directory.CreateDirectory(appDir);

        File.WriteAllText(Path.Combine(parentDir, DevModeGuard.DevModeFileName), "");

        var result = DevModeGuard.IsDevModeActive(appDir);

        Assert.True(result);
    }

    [Fact]
    public void IsDevModeActive_WhenDevModeAtLevel2_ReturnsTrue()
    {
        // Level 2 parent: _testRoot/GrandParent/Parent/App
        var grandParent = Path.Combine(_testRoot, "GrandParent");
        var parentDir = Path.Combine(grandParent, "Parent");
        var appDir = Path.Combine(parentDir, "App");
        Directory.CreateDirectory(appDir);

        File.WriteAllText(Path.Combine(grandParent, DevModeGuard.DevModeFileName), "");

        var result = DevModeGuard.IsDevModeActive(appDir);

        Assert.True(result);
    }

    [Fact]
    public void IsDevModeActive_WhenDevModeAtLevel3_ReturnsTrue()
    {
        // Level 3 parent: _testRoot/GreatGrandParent/GrandParent/Parent/App
        var greatGrandParent = Path.Combine(_testRoot, "GreatGrandParent");
        var grandParent = Path.Combine(greatGrandParent, "GrandParent");
        var parentDir = Path.Combine(grandParent, "Parent");
        var appDir = Path.Combine(parentDir, "App");
        Directory.CreateDirectory(appDir);

        File.WriteAllText(Path.Combine(greatGrandParent, DevModeGuard.DevModeFileName), "");

        var result = DevModeGuard.IsDevModeActive(appDir);

        Assert.True(result);
    }

    [Fact]
    public void IsDevModeActive_WhenDevModeAtLevel4_ReturnsFalse()
    {
        // Level 4 parent is outside 3rd parent scope
        var l4 = Path.Combine(_testRoot, "L4");
        var l3 = Path.Combine(l4, "L3");
        var l2 = Path.Combine(l3, "L2");
        var l1 = Path.Combine(l2, "L1");
        var appDir = Path.Combine(l1, "App");
        Directory.CreateDirectory(appDir);

        File.WriteAllText(Path.Combine(l4, DevModeGuard.DevModeFileName), "");

        var result = DevModeGuard.IsDevModeActive(appDir);

        Assert.False(result);
    }

    [Fact]
    public void IsDevModeActive_WhenFileHasTxtExtension_ReturnsFalse()
    {
        // Strictly BABEL_DEV_MODE, not BABEL_DEV_MODE.txt
        var parentDir = Path.Combine(_testRoot, "AppParent");
        var appDir = Path.Combine(parentDir, "App");
        Directory.CreateDirectory(appDir);

        File.WriteAllText(Path.Combine(parentDir, DevModeGuard.DevModeFileName + ".txt"), "");

        var result = DevModeGuard.IsDevModeActive(appDir);

        Assert.False(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData(@"C:\NonExistentDirectory_For_Test_123456")]
    public void IsDevModeActive_InvalidDirectory_ReturnsFalse(string? path)
    {
        var result = DevModeGuard.IsDevModeActive(path!);
        Assert.False(result);
    }
}
