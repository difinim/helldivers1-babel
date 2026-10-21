namespace Babel.Core.Models;

public class TranslationResult
{
    public bool Success { get; set; }
    public string TranslatedText { get; set; } = string.Empty;
    public string DetectedSourceLanguage { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
    public long LatencyMs { get; set; }

    public static TranslationResult Failed(string errorMessage, long latencyMs = 0) => new()
    {
        Success = false,
        ErrorMessage = errorMessage,
        LatencyMs = latencyMs
    };

    public static TranslationResult Ok(string translatedText, string detectedSourceLang = "", long latencyMs = 0) => new()
    {
        Success = true,
        TranslatedText = translatedText,
        DetectedSourceLanguage = detectedSourceLang,
        LatencyMs = latencyMs
    };
}
