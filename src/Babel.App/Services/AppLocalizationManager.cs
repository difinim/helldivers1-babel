using System.Windows;
using Babel.Core.Services;

namespace Babel.App.Services;

public static class AppLocalizationManager
{
    private static bool _initialized;

    public static void Initialize(string initialLanguage)
    {
        if (!_initialized)
        {
            _initialized = true;
            LocalizationService.Instance.LanguageChanged += OnLanguageChanged;
        }

        LocalizationService.Instance.SetLanguage(initialLanguage);
        ApplyToCurrentApplication();
    }

    public static void SetLanguage(string language)
    {
        LocalizationService.Instance.SetLanguage(language);
    }

    private static void OnLanguageChanged(object? sender, LanguageChangedEventArgs e)
    {
        ApplyToCurrentApplication();
    }

    private static void ApplyToCurrentApplication()
    {
        var app = Application.Current;
        if (app == null) return;

        void Apply()
        {
            var resources = app.Resources;
            var dict = LocalizationService.Instance.GetAllStrings();
            foreach (var kvp in dict)
            {
                resources[kvp.Key] = kvp.Value;
            }

            var flowDirection = LocalizationService.Instance.IsRtl
                ? FlowDirection.RightToLeft
                : FlowDirection.LeftToRight;

            resources["App_FlowDirection"] = flowDirection;

            foreach (Window window in app.Windows)
            {
                window.FlowDirection = flowDirection;
            }
        }

        if (app.Dispatcher.CheckAccess())
        {
            Apply();
        }
        else
        {
            app.Dispatcher.Invoke(Apply);
        }
    }
}
