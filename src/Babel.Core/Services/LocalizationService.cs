using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace Babel.Core.Services;

public class LanguageChangedEventArgs : EventArgs
{
    public string Language { get; }
    public bool IsRtl { get; }

    public LanguageChangedEventArgs(string language, bool isRtl)
    {
        Language = language;
        IsRtl = isRtl;
    }
}

public sealed class LocalizationService
{
    private static readonly Lazy<LocalizationService> _lazyInstance = new(() => new LocalizationService());
    public static LocalizationService Instance => _lazyInstance.Value;

    private readonly object _lock = new();
    private readonly Dictionary<string, string> _englishFallback = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> _currentTranslations = new(StringComparer.Ordinal);
    private string _currentLanguage = "English";

    public static readonly string[] SupportedLanguagesList = new[]
    {
        "Arabic",
        "Bengali",
        "Chinese",
        "Czech",
        "Dutch",
        "English",
        "French",
        "German",
        "Greek",
        "Hindi",
        "Hungarian",
        "Indonesian",
        "Italian",
        "Japanese",
        "Korean",
        "Malay",
        "Persian (Farsi)",
        "Polish",
        "Portuguese (PT-BR)",
        "Romanian",
        "Russian",
        "Spanish",
        "Swedish",
        "Tagalog (Filipino)",
        "Thai",
        "Turkish",
        "Ukrainian",
        "Vietnamese"
    };

    public string CurrentLanguage
    {
        get
        {
            lock (_lock)
            {
                return _currentLanguage;
            }
        }
    }

    public bool IsRtl => IsRtlLanguage(CurrentLanguage);

    public event EventHandler<LanguageChangedEventArgs>? LanguageChanged;

    public LocalizationService()
    {
        LoadEnglishFallback();
        lock (_lock)
        {
            _currentLanguage = "English";
            foreach (var kvp in _englishFallback)
            {
                _currentTranslations[kvp.Key] = kvp.Value;
            }
        }
    }

    private void LoadEnglishFallback()
    {
        var dict = LoadResourceDictionary("English");
        if (dict != null)
        {
            foreach (var kvp in dict)
            {
                _englishFallback[kvp.Key] = kvp.Value;
            }
        }
    }

    public static bool IsRtlLanguage(string? language)
    {
        if (string.IsNullOrWhiteSpace(language)) return false;
        return string.Equals(language, "Arabic", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(language, "Persian (Farsi)", StringComparison.OrdinalIgnoreCase);
    }

    public IReadOnlyList<string> GetSupportedLanguages() => SupportedLanguagesList;

    public void SetLanguage(string? language)
    {
        var targetLang = string.IsNullOrWhiteSpace(language) ? "English" : language.Trim();

        var loadedDict = LoadResourceDictionary(targetLang);
        if (loadedDict == null && !string.Equals(targetLang, "English", StringComparison.OrdinalIgnoreCase))
        {
            targetLang = "English";
            loadedDict = LoadResourceDictionary("English");
        }

        bool changed = false;
        lock (_lock)
        {
            _currentLanguage = targetLang;
            _currentTranslations.Clear();

            // Always populate fallback first
            foreach (var kvp in _englishFallback)
            {
                _currentTranslations[kvp.Key] = kvp.Value;
            }

            // Overwrite with localized definitions if available
            if (loadedDict != null)
            {
                foreach (var kvp in loadedDict)
                {
                    if (!string.IsNullOrEmpty(kvp.Value))
                    {
                        _currentTranslations[kvp.Key] = kvp.Value;
                    }
                }
            }

            changed = true;
        }

        if (changed)
        {
            LanguageChanged?.Invoke(this, new LanguageChangedEventArgs(targetLang, IsRtlLanguage(targetLang)));
        }
    }

    public string GetString(string key)
    {
        if (string.IsNullOrEmpty(key)) return string.Empty;

        lock (_lock)
        {
            if (_currentTranslations.TryGetValue(key, out var val) && !string.IsNullOrEmpty(val))
            {
                return val;
            }

            if (_englishFallback.TryGetValue(key, out var fbVal) && !string.IsNullOrEmpty(fbVal))
            {
                return fbVal;
            }
        }

        return key;
    }

    public string Format(string key, params object[] args)
    {
        var template = GetString(key);
        if (args == null || args.Length == 0) return template;

        try
        {
            return string.Format(template, args);
        }
        catch
        {
            return template;
        }
    }

    public IReadOnlyDictionary<string, string> GetAllStrings()
    {
        lock (_lock)
        {
            return new Dictionary<string, string>(_currentTranslations, StringComparer.Ordinal);
        }
    }

    private static Dictionary<string, string>? LoadResourceDictionary(string language)
    {
        var assembly = typeof(LocalizationService).Assembly;
        var resourceName = $"Babel.Core.i18n.{language}.json";

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            return null;
        }

        using var reader = new StreamReader(stream, System.Text.Encoding.UTF8);
        var json = reader.ReadToEnd();
        return JsonSerializer.Deserialize<Dictionary<string, string>>(json);
    }
}
