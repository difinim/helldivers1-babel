using Babel.Core.Interfaces;
using Babel.Providers.Claude;
using Babel.Providers.DeepSeek;
using Babel.Providers.Gemini;
using Babel.Providers.OpenAI;

namespace Babel.Providers;

/// <summary>
/// Factory for resolving translation provider instances by ID.
/// </summary>
public static class ProviderFactory
{
    public static readonly IReadOnlyList<string> SupportedProviderIds =
    [
        GeminiProvider.Id,
        OpenAiProvider.Id,
        ClaudeProvider.Id,
        DeepSeekProvider.Id
    ];

    /// <summary>
    /// Resolves an <see cref="ITranslationProvider"/> instance by its unique provider ID.
    /// Defaults to GeminiProvider if providerId is null or whitespace.
    /// </summary>
    /// <param name="providerId">The identifier of the provider (case-insensitive).</param>
    /// <param name="httpClient">Optional HttpClient to inject (useful for unit tests).</param>
    /// <returns>An instance of <see cref="ITranslationProvider"/>.</returns>
    /// <exception cref="NotSupportedException">Thrown if providerId is not recognized.</exception>
    public static ITranslationProvider GetProvider(string? providerId, HttpClient? httpClient = null)
    {
        if (string.IsNullOrWhiteSpace(providerId))
        {
            return new GeminiProvider(httpClient);
        }

        return providerId.Trim().ToLowerInvariant() switch
        {
            "gemini" => new GeminiProvider(httpClient),
            "openai" => new OpenAiProvider(httpClient),
            "claude" => new ClaudeProvider(httpClient),
            "deepseek" => new DeepSeekProvider(httpClient),
            _ => throw new NotSupportedException(
                $"Unsupported AI translation provider: '{providerId}'. Supported providers: {string.Join(", ", SupportedProviderIds)}")
        };
    }

    /// <summary>
    /// Creates instances of all registered providers.
    /// </summary>
    public static IEnumerable<ITranslationProvider> GetAllProviders(HttpClient? httpClient = null)
    {
        yield return new GeminiProvider(httpClient);
        yield return new OpenAiProvider(httpClient);
        yield return new ClaudeProvider(httpClient);
        yield return new DeepSeekProvider(httpClient);
    }
}
