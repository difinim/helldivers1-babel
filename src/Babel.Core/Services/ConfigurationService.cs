using System.Text.Json;
using Babel.Core.Interfaces;
using Babel.Core.Models;

namespace Babel.Core.Services;

public class ConfigurationService
{
    private readonly string _configFilePath;
    private readonly IEncryptionProvider _encryptionProvider;

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public ConfigurationService(string? customPath = null, IEncryptionProvider? encryptionProvider = null)
    {
        _encryptionProvider = encryptionProvider ?? EncryptionService.Instance;

        if (!string.IsNullOrWhiteSpace(customPath))
        {
            _configFilePath = customPath;
        }
        else
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var folder = Path.Combine(appData, "Helldivers1Babel");
            _configFilePath = Path.Combine(folder, "config.json");
        }
    }

    public string ConfigFilePath => _configFilePath;

    public BabelConfig Load()
    {
        try
        {
            if (!File.Exists(_configFilePath))
            {
                var defaultConfig = new BabelConfig();
                Save(defaultConfig);
                return defaultConfig;
            }

            var json = File.ReadAllText(_configFilePath);
            var config = JsonSerializer.Deserialize<BabelConfig>(json, SerializerOptions);
            if (config == null)
            {
                var fallback = new BabelConfig();
                Save(fallback);
                return fallback;
            }

            // Decrypt GeminiApiKeyEncrypted if present
            if (!string.IsNullOrWhiteSpace(config.GeminiApiKeyEncrypted))
            {
                try
                {
                    config.GeminiApiKey = _encryptionProvider.Decrypt(config.GeminiApiKeyEncrypted);
                }
                catch (Exception ex)
                {
                    BabelLog.Warn("ConfigurationService", $"Failed to decrypt GeminiApiKeyEncrypted: {ex.Message}");
                    config.GeminiApiKey = string.Empty;
                }
            }
            else
            {
                // Migration: check legacy plain text fields
                if (!string.IsNullOrWhiteSpace(config.LegacyGeminiApiKey))
                {
                    config.GeminiApiKey = config.LegacyGeminiApiKey;
                }
                else if (config.LegacyApiKeys != null &&
                         config.LegacyApiKeys.TryGetValue("Gemini", out var legacyGemini) &&
                         !string.IsNullOrWhiteSpace(legacyGemini))
                {
                    config.GeminiApiKey = legacyGemini;
                }
            }

            // Decrypt any other provider keys from EncryptedApiKeys
            if (config.EncryptedApiKeys != null)
            {
                foreach (var (provider, encVal) in config.EncryptedApiKeys)
                {
                    if (string.Equals(provider, "Gemini", StringComparison.OrdinalIgnoreCase)) continue;
                    if (!string.IsNullOrWhiteSpace(encVal))
                    {
                        try
                        {
                            config.ApiKeys[provider] = _encryptionProvider.Decrypt(encVal);
                        }
                        catch (Exception ex)
                        {
                            BabelLog.Warn("ConfigurationService", $"Failed to decrypt key for {provider}: {ex.Message}");
                        }
                    }
                }
            }

            // Migrate other legacy plain text API keys if present
            if (config.LegacyApiKeys != null)
            {
                foreach (var (k, v) in config.LegacyApiKeys)
                {
                    if (!string.Equals(k, "Gemini", StringComparison.OrdinalIgnoreCase) &&
                        !string.IsNullOrWhiteSpace(v) &&
                        string.IsNullOrWhiteSpace(config.ApiKeys.GetValueOrDefault(k)))
                    {
                        config.ApiKeys[k] = v;
                    }
                }
            }

            ValidateAndNormalize(config);
            return config;
        }
        catch
        {
            // Fallback safely to defaults on malformed/corrupted files
            var fallback = new BabelConfig();
            return fallback;
        }
    }

    public void Save(BabelConfig config)
    {
        ValidateAndNormalize(config);

        // Encrypt Gemini API key
        if (!string.IsNullOrEmpty(config.GeminiApiKey))
        {
            config.GeminiApiKeyEncrypted = _encryptionProvider.Encrypt(config.GeminiApiKey);
        }
        else
        {
            config.GeminiApiKeyEncrypted = string.Empty;
        }

        // Encrypt other provider keys
        config.EncryptedApiKeys ??= new(StringComparer.OrdinalIgnoreCase);
        foreach (var (provider, key) in config.ApiKeys)
        {
            if (string.Equals(provider, "Gemini", StringComparison.OrdinalIgnoreCase)) continue;
            if (!string.IsNullOrEmpty(key))
            {
                config.EncryptedApiKeys[provider] = _encryptionProvider.Encrypt(key);
            }
            else
            {
                config.EncryptedApiKeys.Remove(provider);
            }
        }

        // Clear legacy plain text fields so they are never persisted on disk
        config.LegacyGeminiApiKey = null;
        config.LegacyApiKeys = null;

        var dir = Path.GetDirectoryName(_configFilePath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        var json = JsonSerializer.Serialize(config, SerializerOptions);
        File.WriteAllText(_configFilePath, json);
    }

    public static void ValidateAndNormalize(BabelConfig config)
    {
        if (config.TimeoutSeconds <= 0.5) config.TimeoutSeconds = 5.0;
        config.OverlayScalePercent = Math.Clamp(config.OverlayScalePercent, 50, 200);
        config.OverlayScalePercent = (int)Math.Round(config.OverlayScalePercent / 25.0) * 25;
        if (string.IsNullOrWhiteSpace(config.TargetDisplay)) config.TargetDisplay = "Auto";
        if (!Enum.IsDefined(typeof(OverlayPosition), config.ToastPosition)) config.ToastPosition = OverlayPosition.TopRight;
        if (!Enum.IsDefined(typeof(OverlayPosition), config.QuickChatOverlayPosition)) config.QuickChatOverlayPosition = OverlayPosition.MiddleRight;
        if (config.ChatHistoryLimitMb < 0) config.ChatHistoryLimitMb = 0;
        if (config.ChatHistoryLimitMb > 100) config.ChatHistoryLimitMb = 100;
        if (string.IsNullOrWhiteSpace(config.ActiveProvider)) config.ActiveProvider = "Gemini";
        if (string.IsNullOrWhiteSpace(config.TargetLanguage)) config.TargetLanguage = "English";
        if (string.IsNullOrWhiteSpace(config.OutgoingPrefix)) config.OutgoingPrefix = ">>";
        if (string.IsNullOrWhiteSpace(config.GlobalPauseHotkey)) config.GlobalPauseHotkey = "Ctrl+F10";
        if (string.IsNullOrWhiteSpace(config.ToggleTranslationHotkey)) config.ToggleTranslationHotkey = "Ctrl+F11";
        if (string.IsNullOrWhiteSpace(config.ToggleQuickChatHotkey)) config.ToggleQuickChatHotkey = "Ctrl+F12";
        if (string.IsNullOrWhiteSpace(config.QuickChatOverlayHotkey)) config.QuickChatOverlayHotkey = "Numpad +";

        if (!config.WindowWidth.HasValue || config.WindowWidth < 400) config.WindowWidth = 1128;
        if (!config.WindowHeight.HasValue || config.WindowHeight < 300) config.WindowHeight = 851;
        config.LogColumnWidths ??= new(StringComparer.OrdinalIgnoreCase);

        config.ApiKeys ??= new(StringComparer.OrdinalIgnoreCase);
        config.ProviderModels ??= new(StringComparer.OrdinalIgnoreCase);

        var requiredProviders = new[] { "Gemini", "OpenAI", "Claude", "DeepSeek" };
        foreach (var p in requiredProviders)
        {
            if (!config.ApiKeys.ContainsKey(p)) config.ApiKeys[p] = "";
            if (!config.ProviderModels.ContainsKey(p))
            {
                config.ProviderModels[p] = p switch
                {
                    "Gemini" => "gemini-flash-lite-latest",
                    "OpenAI" => "gpt-4o-mini",
                    "Claude" => "claude-3-5-haiku-latest",
                    "DeepSeek" => "deepseek-chat",
                    _ => ""
                };
            }
        }

        if (config.QuickChatMacros == null || config.QuickChatMacros.Count == 0)
        {
            config.QuickChatMacros = BabelConfig.CreateDefaultMacros();
        }
        else
        {
            // Ensure exactly 10 slots
            while (config.QuickChatMacros.Count < 10)
            {
                int nextId = config.QuickChatMacros.Count + 1;
                config.QuickChatMacros.Add(new QuickChatMacro(nextId, $"NumPad{nextId % 10}", $"Message #{nextId}"));
            }
        }
    }
}
