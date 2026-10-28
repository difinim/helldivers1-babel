using Babel.Core.Models;
using Babel.Core.Services;

namespace Babel.Core.Tests;

public class ConfigTests
{
    [Fact]
    public void DefaultConfig_HasSensibleDefaults()
    {
        var config = new BabelConfig();

        Assert.Equal("Gemini", config.ActiveProvider);
        Assert.Equal("English", config.TargetLanguage);
        Assert.True(config.IncomingTranslationEnabled);
        Assert.True(config.OutgoingTranslationEnabled);
        Assert.True(config.DontTranslateStandardOutgoing);
        Assert.True(config.DontTranslateIncomingInTargetLanguage);
        Assert.Equal(">>", config.OutgoingPrefix);
        Assert.Equal("Ctrl+F10", config.GlobalPauseHotkey);
        Assert.Equal("Ctrl+F11", config.ToggleTranslationHotkey);
        Assert.Equal("Ctrl+F12", config.ToggleQuickChatHotkey);
        Assert.Equal("Numpad +", config.QuickChatOverlayHotkey);
        Assert.True(config.ShowToastOverlay);
        Assert.False(config.DebugLoggingEnabled);
        Assert.Equal(5.0, config.TimeoutSeconds);
        Assert.Equal(10, config.QuickChatMacros.Count);
        Assert.True(config.ApiKeys.ContainsKey("Gemini"));
        Assert.True(config.ProviderModels.ContainsKey("Gemini"));
        Assert.Equal("gemini-flash-lite-latest", config.ProviderModels["Gemini"]);
        Assert.Equal(1128, config.WindowWidth);
        Assert.Equal(851, config.WindowHeight);
        Assert.Null(config.WindowLeft);
        Assert.Null(config.WindowTop);
        Assert.False(config.WindowMaximized);
        Assert.NotNull(config.LogColumnWidths);
        Assert.Empty(config.LogColumnWidths);
        Assert.Equal(1, config.ChatHistoryLimitMb);
        Assert.Equal(100, config.OverlayScalePercent);
        Assert.Equal(1.0, config.OverlayScaleFactor);
        Assert.Equal("Auto", config.TargetDisplay);
        Assert.Equal(OverlayPosition.TopRight, config.ToastPosition);
        Assert.Equal(OverlayPosition.MiddleRight, config.QuickChatOverlayPosition);
        Assert.True(config.WelcomeMessageEnabled);
        Assert.Equal("Welcome to the squad! Glad to have you with us.", config.WelcomeMessage);
        Assert.Equal(TranslationTone.Casual, config.TranslationTone);
    }

    [Fact]
    public void ConfigurationService_SaveAndLoad_RoundTripsCorrectly()
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"babel_test_cfg_{Guid.NewGuid():N}.json");
        try
        {
            var service = new ConfigurationService(tempFile);
            var original = new BabelConfig
            {
                TargetLanguage = "Spanish",
                TimeoutSeconds = 8.5,
                GlobalPauseHotkey = "Ctrl+Shift+F9",
                ToggleTranslationHotkey = "Ctrl+Shift+T",
                ToggleQuickChatHotkey = "Ctrl+Shift+Q",
                QuickChatOverlayHotkey = "Ctrl+F11",
                ShowToastOverlay = false,
                WindowWidth = 1200,
                WindowHeight = 900,
                WindowLeft = 150,
                WindowTop = 80,
                WindowMaximized = true,
                DontTranslateIncomingInTargetLanguage = false
            };
            original.ApiKeys["Gemini"] = "AIzaSyTestKey123";
            original.LogColumnWidths["Time"] = 80;
            original.LogColumnWidths["Player"] = 180;
            original.LogColumnWidths["Original Message"] = 350;
            original.ChatHistoryLimitMb = 25;
            original.OverlayScalePercent = 150;
            original.TargetDisplay = "Display 2";
            original.ToastPosition = OverlayPosition.BottomLeft;
            original.QuickChatOverlayPosition = OverlayPosition.TopCenter;
            original.WelcomeMessageEnabled = false;
            original.WelcomeMessage = "Custom Welcome Message!";

            service.Save(original);

            Assert.True(File.Exists(tempFile));

            var loaded = service.Load();
            Assert.Equal("Spanish", loaded.TargetLanguage);
            Assert.Equal(8.5, loaded.TimeoutSeconds);
            Assert.Equal("Ctrl+Shift+F9", loaded.GlobalPauseHotkey);
            Assert.Equal("Ctrl+Shift+T", loaded.ToggleTranslationHotkey);
            Assert.Equal("Ctrl+Shift+Q", loaded.ToggleQuickChatHotkey);
            Assert.Equal("Ctrl+F11", loaded.QuickChatOverlayHotkey);
            Assert.False(loaded.ShowToastOverlay);
            Assert.Equal("AIzaSyTestKey123", loaded.ApiKeys["Gemini"]);
            Assert.Equal(1200, loaded.WindowWidth);
            Assert.Equal(900, loaded.WindowHeight);
            Assert.Equal(150, loaded.WindowLeft);
            Assert.Equal(80, loaded.WindowTop);
            Assert.True(loaded.WindowMaximized);
            Assert.Equal(80, loaded.LogColumnWidths["Time"]);
            Assert.Equal(180, loaded.LogColumnWidths["Player"]);
            Assert.Equal(350, loaded.LogColumnWidths["Original Message"]);
            Assert.Equal(25, loaded.ChatHistoryLimitMb);
            Assert.Equal(150, loaded.OverlayScalePercent);
            Assert.Equal(1.50, loaded.OverlayScaleFactor);
            Assert.Equal("Display 2", loaded.TargetDisplay);
            Assert.Equal(OverlayPosition.BottomLeft, loaded.ToastPosition);
            Assert.Equal(OverlayPosition.TopCenter, loaded.QuickChatOverlayPosition);
            Assert.False(loaded.DontTranslateIncomingInTargetLanguage);
            Assert.False(loaded.WelcomeMessageEnabled);
            Assert.Equal("Custom Welcome Message!", loaded.WelcomeMessage);
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }

    [Fact]
    public void WelcomeMessage_ClampsTo300Characters()
    {
        var config = new BabelConfig
        {
            WelcomeMessage = new string('A', 350)
        };
        Assert.Equal(300, config.WelcomeMessage.Length);

        config.WelcomeMessage = null!;
        Assert.Equal(string.Empty, config.WelcomeMessage);
    }

    [Fact]
    public void ConfigurationService_LoadInvalidJson_FallsBackToDefaults()
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"babel_test_corrupt_{Guid.NewGuid():N}.json");
        try
        {
            File.WriteAllText(tempFile, "{ this is invalid json !!! }");
            var service = new ConfigurationService(tempFile);

            var config = service.Load();
            Assert.NotNull(config);
            Assert.Equal("Gemini", config.ActiveProvider);
            Assert.Equal("English", config.TargetLanguage);
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }

    [Fact]
    public void ConfigurationService_ValidateAndNormalize_PopulatesMissingHotkeys()
    {
        var config = new BabelConfig
        {
            GlobalPauseHotkey = "",
            ToggleTranslationHotkey = "   ",
            ToggleQuickChatHotkey = null!,
            QuickChatOverlayHotkey = ""
        };

        ConfigurationService.ValidateAndNormalize(config);

        Assert.Equal("Ctrl+F10", config.GlobalPauseHotkey);
        Assert.Equal("Ctrl+F11", config.ToggleTranslationHotkey);
        Assert.Equal("Ctrl+F12", config.ToggleQuickChatHotkey);
        Assert.Equal("Numpad +", config.QuickChatOverlayHotkey);
    }

    [Fact]
    public void ConfigurationService_ValidateAndNormalize_PopulatesWindowAndColumnDefaults()
    {
        var config = new BabelConfig
        {
            WindowWidth = null,
            WindowHeight = 100, // Invalid small height
            LogColumnWidths = null!
        };

        ConfigurationService.ValidateAndNormalize(config);

        Assert.Equal(1128, config.WindowWidth);
        Assert.Equal(851, config.WindowHeight);
        Assert.NotNull(config.LogColumnWidths);
    }

    [Fact]
    public void ConfigurationService_ValidateAndNormalize_ClampsChatHistoryLimitMb()
    {
        var configNegative = new BabelConfig { ChatHistoryLimitMb = -5 };
        ConfigurationService.ValidateAndNormalize(configNegative);
        Assert.Equal(0, configNegative.ChatHistoryLimitMb);

        var configOverflow = new BabelConfig { ChatHistoryLimitMb = 999 };
        ConfigurationService.ValidateAndNormalize(configOverflow);
        Assert.Equal(100, configOverflow.ChatHistoryLimitMb);
    }

    [Theory]
    [InlineData(50, 0.50)]
    [InlineData(75, 0.75)]
    [InlineData(100, 1.00)]
    [InlineData(125, 1.25)]
    [InlineData(150, 1.50)]
    [InlineData(175, 1.75)]
    [InlineData(200, 2.00)]
    public void OverlayScale_CalculatesCorrectFactorForPercentages(int percent, double expectedFactor)
    {
        var config = new BabelConfig { OverlayScalePercent = percent };
        Assert.Equal(expectedFactor, config.OverlayScaleFactor);
    }

    [Fact]
    public void OverlayScale_ClampsAndSnapsPercentages()
    {
        var configUnderflow = new BabelConfig { OverlayScalePercent = 10 };
        Assert.Equal(50, configUnderflow.OverlayScalePercent);
        Assert.Equal(0.50, configUnderflow.OverlayScaleFactor);

        var configOverflow = new BabelConfig { OverlayScalePercent = 999 };
        Assert.Equal(200, configOverflow.OverlayScalePercent);
        Assert.Equal(2.00, configOverflow.OverlayScaleFactor);

        var configSnapping = new BabelConfig { OverlayScalePercent = 80 };
        Assert.Equal(75, configSnapping.OverlayScalePercent);
        Assert.Equal(0.75, configSnapping.OverlayScaleFactor);
    }

    [Fact]
    public void OverlayScale_LegacyOverlayScaleLevel_DeserializesCorrectly()
    {
        var json = "{\"OverlayScaleLevel\": 2}";
        var loaded = System.Text.Json.JsonSerializer.Deserialize<BabelConfig>(json);
        Assert.NotNull(loaded);
        Assert.Equal(125, loaded.OverlayScalePercent);
        Assert.Equal(1.25, loaded.OverlayScaleFactor);

        // When both are present, OverlayScalePercent takes priority
        var jsonBoth = "{\"OverlayScalePercent\": 75, \"OverlayScaleLevel\": 4}";
        var loadedBoth = System.Text.Json.JsonSerializer.Deserialize<BabelConfig>(jsonBoth);
        Assert.NotNull(loadedBoth);
        Assert.Equal(75, loadedBoth.OverlayScalePercent);
        Assert.Equal(0.75, loadedBoth.OverlayScaleFactor);
    }

    [Theory]
    [InlineData(OverlayPosition.TopLeft)]
    [InlineData(OverlayPosition.TopCenter)]
    [InlineData(OverlayPosition.TopRight)]
    [InlineData(OverlayPosition.MiddleLeft)]
    [InlineData(OverlayPosition.Center)]
    [InlineData(OverlayPosition.MiddleRight)]
    [InlineData(OverlayPosition.BottomLeft)]
    [InlineData(OverlayPosition.BottomCenter)]
    [InlineData(OverlayPosition.BottomRight)]
    public void OverlayPosition_SerializesAndDeserializesAllValues(OverlayPosition pos)
    {
        var config = new BabelConfig
        {
            ToastPosition = pos,
            QuickChatOverlayPosition = pos
        };

        var json = System.Text.Json.JsonSerializer.Serialize(config);
        var loaded = System.Text.Json.JsonSerializer.Deserialize<BabelConfig>(json);

        Assert.NotNull(loaded);
        Assert.Equal(pos, loaded.ToastPosition);
        Assert.Equal(pos, loaded.QuickChatOverlayPosition);
    }

    [Fact]
    public void ConfigurationService_ValidateAndNormalize_FallbacksForInvalidOverlayPositionAndDisplay()
    {
        var config = new BabelConfig
        {
            TargetDisplay = "   ",
            ToastPosition = (OverlayPosition)999,
            QuickChatOverlayPosition = (OverlayPosition)(-1)
        };

        ConfigurationService.ValidateAndNormalize(config);

        Assert.Equal("Auto", config.TargetDisplay);
        Assert.Equal(OverlayPosition.TopRight, config.ToastPosition);
        Assert.Equal(OverlayPosition.MiddleRight, config.QuickChatOverlayPosition);
        Assert.True(config.WelcomeMessageEnabled);
        Assert.Equal("Welcome to the squad! Glad to have you with us.", config.WelcomeMessage);
        Assert.Equal(TranslationTone.Casual, config.TranslationTone);
    }

    [Fact]
    public void BabelConfig_Defaults_MatchExpectedFactoryValues()
    {
        var config = new BabelConfig();

        Assert.Equal("English", config.TargetLanguage);
        Assert.False(config.DebugLoggingEnabled);
        Assert.Equal("gemini-flash-lite-latest", config.ProviderModels["Gemini"]);
        Assert.Equal("Auto", config.TargetDisplay);
        Assert.Equal(100, config.OverlayScalePercent);
        Assert.Equal(OverlayPosition.TopRight, config.ToastPosition);
        Assert.Equal(OverlayPosition.MiddleRight, config.QuickChatOverlayPosition);
        Assert.Equal(1, config.ChatHistoryLimitMb);
        Assert.Equal(5.0, config.TimeoutSeconds);
        Assert.True(config.IncomingTranslationEnabled);
        Assert.True(config.OutgoingTranslationEnabled);
        Assert.True(config.DontTranslateStandardOutgoing);
        Assert.True(config.DontTranslateIncomingInTargetLanguage);
    }

    [Fact]
    public void ResetToDefaults_RestoresDontTranslateIncomingInTargetLanguage()
    {
        var config = new BabelConfig
        {
            DontTranslateIncomingInTargetLanguage = false
        };

        Assert.False(config.DontTranslateIncomingInTargetLanguage);

        var defaults = new BabelConfig();
        config.DontTranslateIncomingInTargetLanguage = defaults.DontTranslateIncomingInTargetLanguage;

        Assert.True(config.DontTranslateIncomingInTargetLanguage);
    }

    [Theory]
    [InlineData(TranslationTone.Casual, "Casual")]
    [InlineData(TranslationTone.Normal, "Normal")]
    public void TranslationTone_SerializesAndDeserializesCorrectly(TranslationTone tone, string expectedString)
    {
        var original = new BabelConfig { TranslationTone = tone };
        var json = System.Text.Json.JsonSerializer.Serialize(original);
        Assert.Contains($"\"TranslationTone\":\"{expectedString}\"", json);

        var deserialized = System.Text.Json.JsonSerializer.Deserialize<BabelConfig>(json);
        Assert.NotNull(deserialized);
        Assert.Equal(tone, deserialized.TranslationTone);
    }

    [Fact]
    public void ResetToDefaults_RestoresTranslationToneToCasual()
    {
        var config = new BabelConfig
        {
            TranslationTone = TranslationTone.Normal
        };
        Assert.Equal(TranslationTone.Normal, config.TranslationTone);

        var defaults = new BabelConfig();
        config.TranslationTone = defaults.TranslationTone;
        Assert.Equal(TranslationTone.Casual, config.TranslationTone);
    }
}
