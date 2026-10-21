using Babel.Core.Services;

namespace Babel.Core.Tests;

public class BabelLogTests
{
    [Fact]
    public void BabelLog_Properties_ShouldReturnValidPaths()
    {
        Assert.False(string.IsNullOrWhiteSpace(BabelLog.LogDirectory));
        Assert.False(string.IsNullOrWhiteSpace(BabelLog.LogFilePath));
        Assert.EndsWith("app.log", BabelLog.LogFilePath);
    }

    [Fact]
    public void BabelLog_IsEnabled_ShouldToggle()
    {
        var original = BabelLog.IsEnabled;
        try
        {
            BabelLog.IsEnabled = false;
            Assert.False(BabelLog.IsEnabled);

            BabelLog.IsEnabled = true;
            Assert.True(BabelLog.IsEnabled);
        }
        finally
        {
            BabelLog.IsEnabled = original;
        }
    }

    [Fact]
    public void BabelLog_LogMethods_ShouldNotThrow()
    {
        var ex = Record.Exception(() =>
        {
            BabelLog.Info("TestCategory", "Test info message");
            BabelLog.Warn("TestCategory", "Test warning message");
            BabelLog.Error("TestCategory", "Test error message", new InvalidOperationException("Test inner"));
        });

        Assert.Null(ex);
    }
}
