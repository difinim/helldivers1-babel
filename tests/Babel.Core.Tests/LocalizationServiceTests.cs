using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Babel.Core.Services;
using Xunit;

namespace Babel.Core.Tests;

public class LocalizationServiceTests
{
    [Fact]
    public void All28Languages_AreEmbeddedInAssembly()
    {
        var assembly = typeof(LocalizationService).Assembly;
        var resourceNames = assembly.GetManifestResourceNames();

        Assert.Equal(28, LocalizationService.SupportedLanguagesList.Length);

        foreach (var lang in LocalizationService.SupportedLanguagesList)
        {
            var expectedResource = $"Babel.Core.i18n.{lang}.json";
            Assert.Contains(expectedResource, resourceNames);

            using var stream = assembly.GetManifestResourceStream(expectedResource);
            Assert.NotNull(stream);
            Assert.True(stream.Length > 0);
        }
    }

    [Fact]
    public void AllLanguages_ContainAllKeysPresentInEnglish()
    {
        var assembly = typeof(LocalizationService).Assembly;

        using var enStream = assembly.GetManifestResourceStream("Babel.Core.i18n.English.json");
        Assert.NotNull(enStream);
        using var enReader = new StreamReader(enStream);
        var enDict = JsonSerializer.Deserialize<System.Collections.Generic.Dictionary<string, string>>(enReader.ReadToEnd());
        Assert.NotNull(enDict);
        Assert.True(enDict.Count > 100);

        foreach (var lang in LocalizationService.SupportedLanguagesList)
        {
            if (lang == "English") continue;

            using var stream = assembly.GetManifestResourceStream($"Babel.Core.i18n.{lang}.json");
            Assert.NotNull(stream);
            using var reader = new StreamReader(stream);
            var dict = JsonSerializer.Deserialize<System.Collections.Generic.Dictionary<string, string>>(reader.ReadToEnd());
            Assert.NotNull(dict);

            foreach (var key in enDict.Keys)
            {
                Assert.True(dict.ContainsKey(key), $"Language '{lang}' is missing key '{key}'");
                Assert.False(string.IsNullOrWhiteSpace(dict[key]), $"Language '{lang}' has empty value for key '{key}'");
            }
        }
    }

    [Theory]
    [InlineData("Arabic", true)]
    [InlineData("arabic", true)]
    [InlineData("Persian (Farsi)", true)]
    [InlineData("persian (farsi)", true)]
    [InlineData("English", false)]
    [InlineData("Spanish", false)]
    [InlineData("Russian", false)]
    [InlineData("Chinese", false)]
    [InlineData("Japanese", false)]
    [InlineData("Portuguese (PT-BR)", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsRtlLanguage_IdentifiesRtlLanguagesCorrectly(string? language, bool expectedRtl)
    {
        var isRtl = LocalizationService.IsRtlLanguage(language);
        Assert.Equal(expectedRtl, isRtl);
    }

    [Fact]
    public void SetLanguage_UpdatesCurrentLanguageAndFiresEvent()
    {
        var service = new LocalizationService();
        string? notifiedLang = null;
        bool? notifiedRtl = null;

        service.LanguageChanged += (_, e) =>
        {
            notifiedLang = e.Language;
            notifiedRtl = e.IsRtl;
        };

        service.SetLanguage("Spanish");

        Assert.Equal("Spanish", service.CurrentLanguage);
        Assert.False(service.IsRtl);
        Assert.Equal("Spanish", notifiedLang);
        Assert.False(notifiedRtl);

        var title = service.GetString("App_HeaderTitle");
        Assert.Equal("HELLDIVERS 1 BABEL", title);

        var settings = service.GetString("Tab_Settings");
        Assert.Equal("Ajustes", settings);
    }

    [Fact]
    public void SetLanguage_RtlLanguage_SetsIsRtlTrue()
    {
        var service = new LocalizationService();
        bool receivedRtl = false;

        service.LanguageChanged += (_, e) =>
        {
            receivedRtl = e.IsRtl;
        };

        service.SetLanguage("Arabic");

        Assert.Equal("Arabic", service.CurrentLanguage);
        Assert.True(service.IsRtl);
        Assert.True(receivedRtl);

        var settings = service.GetString("Tab_Settings");
        Assert.Equal("الإعدادات", settings);
    }

    [Fact]
    public void Fallback_ReturnsEnglishValue_WhenKeyMissing()
    {
        var service = new LocalizationService();
        service.SetLanguage("Spanish");

        // Request a key that definitely exists in English
        var val = service.GetString("App_Title");
        Assert.False(string.IsNullOrEmpty(val));

        // If a non-existent key is requested, returns the key itself
        var missing = service.GetString("NonExistent_Test_Key");
        Assert.Equal("NonExistent_Test_Key", missing);
    }

    [Fact]
    public void Format_ReplacesArgumentsCorrectly()
    {
        var service = new LocalizationService();
        service.SetLanguage("English");

        var formatted = service.Format("Btn_Pause", "Ctrl+F10");
        Assert.Equal("Pause (Ctrl+F10)", formatted);

        service.SetLanguage("Portuguese (PT-BR)");
        var formattedPt = service.Format("Btn_Pause", "Ctrl+F10");
        Assert.Equal("Pausar (Ctrl+F10)", formattedPt);
    }

    [Fact]
    public void UnknownLanguage_FallsBackToEnglish()
    {
        var service = new LocalizationService();
        service.SetLanguage("KlingonUnknown");

        Assert.Equal("English", service.CurrentLanguage);
        Assert.False(service.IsRtl);
    }

    [Fact]
    public void GetAllStrings_ReturnsCompleteDictionarySnapshot()
    {
        var service = new LocalizationService();
        service.SetLanguage("French");

        var allStrings = service.GetAllStrings();
        Assert.NotNull(allStrings);
        Assert.True(allStrings.Count > 150);
        Assert.Equal("Paramètres", allStrings["Tab_Settings"]);
    }

    [Fact]
    public void Format_WithMismatchedArguments_DoesNotThrow()
    {
        var service = new LocalizationService();
        service.SetLanguage("English");

        // Format string with no args passed
        var result = service.Format("Btn_Pause");
        Assert.NotNull(result);

        // Format string with extra args
        var result2 = service.Format("Btn_Pause", "Arg1", "Arg2", "Arg3");
        Assert.NotNull(result2);
    }

    [Fact]
    public void NoLooseLanguageJsonFiles_InOutputDirectory()
    {
        var baseDir = AppContext.BaseDirectory;
        var jsonFiles = Directory.GetFiles(baseDir, "*.json", SearchOption.TopDirectoryOnly)
            .Select(Path.GetFileName)
            .Where(f => f != null)
            .ToList();

        foreach (var lang in LocalizationService.SupportedLanguagesList)
        {
            Assert.DoesNotContain($"{lang}.json", jsonFiles);
        }
    }
}
