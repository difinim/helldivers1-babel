using Babel.Core.Models;
using Babel.Providers.Claude;
using Babel.Providers.DeepSeek;
using Babel.Providers.Gemini;
using Babel.Providers.OpenAI;

namespace Babel.Providers.Tests;

public class ProviderFactoryTests
{
    [Fact]
    public void GetProvider_Gemini_ReturnsGeminiProvider()
    {
        var provider = ProviderFactory.GetProvider("Gemini");
        Assert.IsType<GeminiProvider>(provider);
        Assert.Equal("Gemini", provider.ProviderId);
        Assert.Equal("Google Gemini", provider.DisplayName);
    }

    [Fact]
    public void GetProvider_NullOrEmpty_DefaultsToGemini()
    {
        var providerNull = ProviderFactory.GetProvider(null);
        var providerEmpty = ProviderFactory.GetProvider("");
        var providerWhitespace = ProviderFactory.GetProvider("   ");

        Assert.IsType<GeminiProvider>(providerNull);
        Assert.IsType<GeminiProvider>(providerEmpty);
        Assert.IsType<GeminiProvider>(providerWhitespace);
    }

    [Fact]
    public async Task GetProvider_OpenAi_ReturnsOpenAiProvider_ThrowsNotImplemented()
    {
        var provider = ProviderFactory.GetProvider("OpenAI");
        Assert.IsType<OpenAiProvider>(provider);
        Assert.Equal("OpenAI", provider.ProviderId);
        Assert.Equal("OpenAI (ChatGPT)", provider.DisplayName);

        var config = new BabelConfig();
        var request = new TranslationRequest { Text = "test" };

        var exTranslate = await Assert.ThrowsAsync<NotImplementedException>(() => provider.TranslateAsync(request, config));
        Assert.Equal("Coming in next release", exTranslate.Message);

        var exValidate = await Assert.ThrowsAsync<NotImplementedException>(() => provider.ValidateCredentialsAsync(config));
        Assert.Equal("Coming in next release", exValidate.Message);
    }

    [Fact]
    public async Task GetProvider_Claude_ReturnsClaudeProvider_ThrowsNotImplemented()
    {
        var provider = ProviderFactory.GetProvider("Claude");
        Assert.IsType<ClaudeProvider>(provider);
        Assert.Equal("Claude", provider.ProviderId);
        Assert.Equal("Anthropic Claude", provider.DisplayName);

        var config = new BabelConfig();
        var request = new TranslationRequest { Text = "test" };

        var exTranslate = await Assert.ThrowsAsync<NotImplementedException>(() => provider.TranslateAsync(request, config));
        Assert.Equal("Coming in next release", exTranslate.Message);

        var exValidate = await Assert.ThrowsAsync<NotImplementedException>(() => provider.ValidateCredentialsAsync(config));
        Assert.Equal("Coming in next release", exValidate.Message);
    }

    [Fact]
    public async Task GetProvider_DeepSeek_ReturnsDeepSeekProvider_ThrowsNotImplemented()
    {
        var provider = ProviderFactory.GetProvider("DeepSeek");
        Assert.IsType<DeepSeekProvider>(provider);
        Assert.Equal("DeepSeek", provider.ProviderId);
        Assert.Equal("DeepSeek", provider.DisplayName);

        var config = new BabelConfig();
        var request = new TranslationRequest { Text = "test" };

        var exTranslate = await Assert.ThrowsAsync<NotImplementedException>(() => provider.TranslateAsync(request, config));
        Assert.Equal("Coming in next release", exTranslate.Message);

        var exValidate = await Assert.ThrowsAsync<NotImplementedException>(() => provider.ValidateCredentialsAsync(config));
        Assert.Equal("Coming in next release", exValidate.Message);
    }

    [Fact]
    public void GetProvider_Unknown_ThrowsNotSupportedException()
    {
        Assert.Throws<NotSupportedException>(() => ProviderFactory.GetProvider("NonExistentProvider"));
    }

    [Fact]
    public void GetAllProviders_ReturnsAllRegisteredProviders()
    {
        var providers = ProviderFactory.GetAllProviders().ToList();
        Assert.Equal(4, providers.Count);
        Assert.Contains(providers, p => p is GeminiProvider);
        Assert.Contains(providers, p => p is OpenAiProvider);
        Assert.Contains(providers, p => p is ClaudeProvider);
        Assert.Contains(providers, p => p is DeepSeekProvider);
    }
}
