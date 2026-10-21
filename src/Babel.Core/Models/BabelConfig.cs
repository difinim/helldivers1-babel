using System.Text.Json.Serialization;

namespace Babel.Core.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OverlayPosition
{
    TopLeft,
    TopCenter,
    TopRight,
    MiddleLeft,
    Center,
    MiddleRight,
    BottomLeft,
    BottomCenter,
    BottomRight
}

public class BabelConfig
{
    public string ActiveProvider { get; set; } = "Gemini";
    public string TargetLanguage { get; set; } = "English";
    public bool IncomingTranslationEnabled { get; set; } = true;
    public bool OutgoingTranslationEnabled { get; set; } = true;
    public string OutgoingPrefix { get; set; } = ">>";
    public bool DontTranslateStandardOutgoing { get; set; } = true;
    public bool SoundAlertEnabled { get; set; } = true;
    public double TimeoutSeconds { get; set; } = 5.0;
    public string GlobalPauseHotkey { get; set; } = "Ctrl+F10";
    public string ToggleTranslationHotkey { get; set; } = "Ctrl+F11";
    public string ToggleQuickChatHotkey { get; set; } = "Ctrl+F12";
    public string QuickChatOverlayHotkey { get; set; } = "Numpad +";
    public bool ShowToastOverlay { get; set; } = true;
    public bool DebugLoggingEnabled { get; set; } = false;

    private int _overlayScalePercent = 100;
    private bool _percentExplicitlySet;

    /// <summary>
    /// Visual scale percentage for on-screen Toast notifications and Quick Chat Overlay (50% to 200%, step 25%). Default is 100%.
    /// </summary>
    public int OverlayScalePercent
    {
        get => _overlayScalePercent;
        set
        {
            int clamped = Math.Clamp(value, 50, 200);
            _overlayScalePercent = (int)Math.Round(clamped / 25.0) * 25;
            _percentExplicitlySet = true;
        }
    }

    /// <summary>
    /// Calculated scale multiplier based on OverlayScalePercent (50% -> 0.50x, 75% -> 0.75x, 100% -> 1.00x, ..., 200% -> 2.00x).
    /// </summary>
    [JsonIgnore]
    public double OverlayScaleFactor => _overlayScalePercent / 100.0;

    /// <summary>
    /// Legacy scale level (1 to 5) for backward compatibility in C# code.
    /// </summary>
    [JsonIgnore]
    public int OverlayScaleLevel
    {
        get => (_overlayScalePercent - 100) / 25 + 1;
        set => OverlayScalePercent = 100 + (value - 1) * 25;
    }

    /// <summary>
    /// Deserializes legacy OverlayScaleLevel from JSON if OverlayScalePercent was not explicitly provided.
    /// </summary>
    [JsonPropertyName("OverlayScaleLevel")]
    [JsonInclude]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    internal int? LegacyOverlayScaleLevel
    {
        get => null;
        set
        {
            if (value.HasValue && !_percentExplicitlySet)
            {
                _overlayScalePercent = Math.Clamp(100 + (value.Value - 1) * 25, 50, 200);
            }
        }
    }

    /// <summary>
    /// Target display monitor to show toasts and Quick Chat Overlay on ("Auto", "Primary", or specific display name/id).
    /// </summary>
    public string TargetDisplay { get; set; } = "Auto";

    /// <summary>
    /// Anchor position on screen for Toast notifications. Default is TopRight.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OverlayPosition ToastPosition { get; set; } = OverlayPosition.TopRight;

    /// <summary>
    /// Anchor position on screen for Quick Chat Overlay. Default is MiddleRight.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OverlayPosition QuickChatOverlayPosition { get; set; } = OverlayPosition.MiddleRight;

    /// <summary>
    /// Storage limit for persistent chat history table logs in megabytes (0 to 100 MB). Default is 1 MB.
    /// Setting to 0 disables persistent chat logging to disk.
    /// </summary>
    public int ChatHistoryLimitMb { get; set; } = 1;

    // Window geometry & state persistence
    public double? WindowWidth { get; set; } = 1128;
    public double? WindowHeight { get; set; } = 851;
    public double? WindowLeft { get; set; }
    public double? WindowTop { get; set; }
    public bool WindowMaximized { get; set; } = false;

    // Logs column widths persistence (keyed by column header name)
    public Dictionary<string, double> LogColumnWidths { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// DPAPI encrypted Base64 string for Gemini API Key stored securely on disk.
    /// </summary>
    public string GeminiApiKeyEncrypted { get; set; } = string.Empty;

    /// <summary>
    /// DPAPI encrypted Base64 strings for other AI providers stored securely on disk.
    /// </summary>
    public Dictionary<string, string> EncryptedApiKeys { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Plain text runtime property for Gemini API key.
    /// Excluded from JSON serialization to prevent persisting secrets in plain text.
    /// </summary>
    [JsonIgnore]
    public string GeminiApiKey
    {
        get => ApiKeys.GetValueOrDefault("Gemini", string.Empty);
        set => ApiKeys["Gemini"] = value ?? string.Empty;
    }

    /// <summary>
    /// Plain text runtime API keys dictionary for providers.
    /// Excluded from JSON serialization to prevent persisting secrets in plain text.
    /// </summary>
    [JsonIgnore]
    public Dictionary<string, string> ApiKeys { get; set; } = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Gemini"] = "",
        ["OpenAI"] = "",
        ["Claude"] = "",
        ["DeepSeek"] = ""
    };

    /// <summary>
    /// Legacy plain text property used exclusively for backward-compatible deserialization and migration.
    /// Ignored during serialization when null.
    /// </summary>
    [JsonPropertyName("GeminiApiKey")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? LegacyGeminiApiKey { get; set; }

    /// <summary>
    /// Legacy plain text dictionary used exclusively for backward-compatible deserialization and migration.
    /// Ignored during serialization when null.
    /// </summary>
    [JsonPropertyName("ApiKeys")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, string>? LegacyApiKeys { get; set; }

    public Dictionary<string, string> ProviderModels { get; set; } = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Gemini"] = "gemini-flash-lite-latest",
        ["OpenAI"] = "gpt-4o-mini",
        ["Claude"] = "claude-3-5-haiku-latest",
        ["DeepSeek"] = "deepseek-chat"
    };

    public List<QuickChatMacro> QuickChatMacros { get; set; } = CreateDefaultMacros();

    public static List<QuickChatMacro> CreateDefaultMacros()
    {
        return new List<QuickChatMacro>
        {
            new(1, "NumPad1", "Affirmative / Roger that!", true),
            new(2, "NumPad2", "Negative / Cancel that!", true),
            new(3, "NumPad3", "Hold position / Stay here!", true),
            new(4, "NumPad4", "Fall back / Retreat now!", true),
            new(5, "NumPad5", "Need ammo / Resupply here!", true),
            new(6, "NumPad6", "Calling in Reinforcements!", true),
            new(7, "NumPad7", "Watch out! Danger / Heavy enemy!", true),
            new(8, "NumPad8", "Moving to objective!", true),
            new(9, "NumPad9", "Clear the area / Incoming stratagem!", true),
            new(10, "NumPad0", "Thank you, Helldiver!", true)
        };
    }
}
