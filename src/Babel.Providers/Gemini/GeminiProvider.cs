using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using Babel.Core.Models;
using Babel.Providers.Base;

namespace Babel.Providers.Gemini;

/// <summary>
/// Translation adapter for Google Gemini models using the generateContent REST API.
/// </summary>
public class GeminiProvider : BaseTranslationProvider
{
    public const string Id = "Gemini";
    public const string DefaultModel = "gemini-1.5-flash";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public override string ProviderId => Id;
    public override string DisplayName => "Google Gemini";

    public GeminiProvider(HttpClient? httpClient = null) : base(httpClient)
    {
    }

    protected override async Task<TranslationResult> TranslateCoreAsync(
        TranslationRequest request,
        BabelConfig config,
        CancellationToken ct)
    {
        if (!config.ApiKeys.TryGetValue(ProviderId, out var apiKey) || string.IsNullOrWhiteSpace(apiKey))
        {
            return TranslationResult.Failed("Gemini API key is not configured. Please set your API key in Settings.");
        }

        var model = config.ProviderModels.TryGetValue(ProviderId, out var configuredModel) && !string.IsNullOrWhiteSpace(configuredModel)
            ? configuredModel.Trim()
            : DefaultModel;

        var isCasual = config.TranslationTone == TranslationTone.Casual;
        var systemInstruction = request.IsOutgoing
            ? (isCasual
                ? "Translate the provided message into the requested language for in-game chat in Helldivers. Casual, lowercase, keep acronyms uppercase. Only return translation."
                : "Translate the provided message into the requested language for in-game chat in Helldivers. Standard, natural tone, keep acronyms uppercase. Only return translation.")
            : (isCasual
                ? $"Translate in-game chat for the Helldivers game to {request.TargetLanguage}. Prefix with source language name: (<language name in English>): <translation>. Casual. Only return translation."
                : $"Translate in-game chat for the Helldivers game to {request.TargetLanguage}. Prefix with source language name: (<language name in English>): <translation>. Standard, natural tone. Only return translation.");

        var userPrompt = request.IsOutgoing
            ? $"Target language: {request.TargetLanguage}. Message: {request.Text}"
            : request.Text;

        // Construct request payload explicitly omitting thinkingConfig to guarantee sub-second responses (~0.7s)
        var payload = new GeminiGenerateContentRequest
        {
            SystemInstruction = new GeminiContent
            {
                Parts = [new GeminiPart { Text = systemInstruction }]
            },
            Contents =
            [
                new GeminiContent
                {
                    Parts = [new GeminiPart { Text = userPrompt }]
                }
            ]
        };

        var endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/{Uri.EscapeDataString(model)}:generateContent?key={Uri.EscapeDataString(apiKey)}";
        var jsonContent = JsonSerializer.Serialize(payload, JsonOptions);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
        };

        using var response = await HttpClient.SendAsync(httpRequest, ct).ConfigureAwait(false);
        var responseBody = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var normalizedError = NormalizeHttpError(response.StatusCode, responseBody);
            return TranslationResult.Failed(normalizedError);
        }

        var geminiResponse = JsonSerializer.Deserialize<GeminiGenerateContentResponse>(responseBody, JsonOptions);
        if (geminiResponse?.Candidates == null || geminiResponse.Candidates.Count == 0)
        {
            return TranslationResult.Failed("Gemini returned no response candidates.");
        }

        var firstCandidate = geminiResponse.Candidates[0];
        var partText = firstCandidate.Content?.Parts?.ElementAtOrDefault(0)?.Text;

        if (string.IsNullOrWhiteSpace(partText))
        {
            return TranslationResult.Failed("Gemini returned empty translation content.");
        }

        var cleanedText = CleanTranslationText(partText);

        string detectedSourceLanguage = string.Empty;
        if (!request.IsOutgoing)
        {
            var match = Regex.Match(cleanedText, @"^\(([^)]+)\):");
            if (match.Success)
            {
                detectedSourceLanguage = match.Groups[1].Value.Trim();
            }
        }
        else if (!string.IsNullOrWhiteSpace(request.SourceLanguage) && request.SourceLanguage != "auto")
        {
            detectedSourceLanguage = request.SourceLanguage;
        }

        return TranslationResult.Ok(cleanedText, detectedSourceLanguage);
    }

    /// <inheritdoc />
    public override async Task<bool> ValidateCredentialsAsync(BabelConfig config, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(config);

        if (!config.ApiKeys.TryGetValue(ProviderId, out var key) || string.IsNullOrWhiteSpace(key))
        {
            return false;
        }

        try
        {
            var testRequest = new TranslationRequest
            {
                Text = "Hello",
                TargetLanguage = "English",
                IsOutgoing = true
            };

            var result = await TranslateAsync(testRequest, config, ct).ConfigureAwait(false);
            return result.Success;
        }
        catch
        {
            return false;
        }
    }

    private static string CleanTranslationText(string raw)
    {
        var text = raw.Trim();
        // Remove enclosing quotes if Gemini wrapped response in quotes
        if (text.Length >= 2 && text.StartsWith('"') && text.EndsWith('"'))
        {
            text = text[1..^1].Trim();
        }
        return text;
    }
}
