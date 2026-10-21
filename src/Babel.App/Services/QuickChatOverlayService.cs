using System;
using System.Collections.Generic;
using System.Windows;
using Babel.App.Views;
using Babel.Core.Models;
using Babel.Core.Services;

namespace Babel.App.Services;

/// <summary>
/// Service managing the display lifecycle and toggle actions of the on-screen Quick Chat reference overlay.
/// </summary>
public static class QuickChatOverlayService
{
    private static QuickChatOverlayWindow? _currentOverlay;
    private static readonly object _lock = new();

    public static double ScaleFactor { get; set; } = 1.0;
    public static OverlayPosition CurrentPosition { get; set; } = OverlayPosition.MiddleRight;
    public static string TargetDisplay { get; set; } = "Primary";
    public static Func<IntPtr?>? GetGameWindowHandle { get; set; }

    public static void UpdatePosition(OverlayPosition position, string? targetDisplay = null)
    {
        CurrentPosition = position;
        if (!string.IsNullOrWhiteSpace(targetDisplay)) TargetDisplay = targetDisplay;

        var app = Application.Current;
        if (app == null) return;

        Action action = () =>
        {
            lock (_lock)
            {
                if (_currentOverlay != null && _currentOverlay.IsShowing)
                {
                    var workArea = DisplayMonitorService.GetTargetWorkArea(TargetDisplay, GetGameWindowHandle?.Invoke());
                    _currentOverlay.SetPosition(position, workArea);
                }
            }
        };

        if (app.Dispatcher.CheckAccess())
        {
            action();
        }
        else
        {
            app.Dispatcher.BeginInvoke(action);
        }
    }

    public static void UpdateScale(double scaleFactor)
    {
        ScaleFactor = scaleFactor;
        var app = Application.Current;
        if (app == null) return;

        Action action = () =>
        {
            lock (_lock)
            {
                if (_currentOverlay != null && _currentOverlay.IsShowing)
                {
                    _currentOverlay.SetScale(scaleFactor);
                }
            }
        };

        if (app.Dispatcher.CheckAccess())
        {
            action();
        }
        else
        {
            app.Dispatcher.BeginInvoke(action);
        }
    }

    public static bool IsVisible
    {
        get
        {
            lock (_lock)
            {
                return _currentOverlay != null && _currentOverlay.IsShowing;
            }
        }
    }

    public static void Toggle(IEnumerable<QuickChatMacro> macros, string? toggleShortcut = null)
    {
        var app = Application.Current;
        if (app == null) return;

        if (app.Dispatcher.CheckAccess())
        {
            ToggleInternal(macros, toggleShortcut);
        }
        else
        {
            app.Dispatcher.BeginInvoke(() => ToggleInternal(macros, toggleShortcut));
        }
    }

    public static void Show(IEnumerable<QuickChatMacro> macros, string? toggleShortcut = null)
    {
        var app = Application.Current;
        if (app == null) return;

        if (app.Dispatcher.CheckAccess())
        {
            ShowInternal(macros, toggleShortcut);
        }
        else
        {
            app.Dispatcher.BeginInvoke(() => ShowInternal(macros, toggleShortcut));
        }
    }

    public static void Hide()
    {
        var app = Application.Current;
        if (app == null) return;

        if (app.Dispatcher.CheckAccess())
        {
            HideInternal();
        }
        else
        {
            app.Dispatcher.BeginInvoke(HideInternal);
        }
    }

    private static void ToggleInternal(IEnumerable<QuickChatMacro> macros, string? toggleShortcut)
    {
        lock (_lock)
        {
            if (_currentOverlay != null && _currentOverlay.IsShowing)
            {
                HideInternal();
            }
            else
            {
                ShowInternal(macros, toggleShortcut);
            }
        }
    }

    private static void ShowInternal(IEnumerable<QuickChatMacro> macros, string? toggleShortcut)
    {
        lock (_lock)
        {
            try
            {
                var workArea = DisplayMonitorService.GetTargetWorkArea(TargetDisplay, GetGameWindowHandle?.Invoke());

                if (_currentOverlay != null && _currentOverlay.IsShowing)
                {
                    _currentOverlay.SetScale(ScaleFactor);
                    _currentOverlay.SetPosition(CurrentPosition, workArea);
                    _currentOverlay.UpdateMacros(macros, toggleShortcut);
                    return;
                }

                if (_currentOverlay != null)
                {
                    try { _currentOverlay.Close(); } catch { }
                    _currentOverlay = null;
                }

                _currentOverlay = new QuickChatOverlayWindow(ScaleFactor, CurrentPosition, workArea);
                _currentOverlay.Closed += (_, _) =>
                {
                    lock (_lock)
                    {
                        _currentOverlay = null;
                    }
                };

                _currentOverlay.Show();
                _currentOverlay.UpdateMacros(macros, toggleShortcut);
            }
            catch (Exception ex)
            {
                BabelLog.Error("QuickChatOverlayService", "Failed to show quick chat overlay", ex);
            }
        }
    }

    private static void HideInternal()
    {
        lock (_lock)
        {
            try
            {
                _currentOverlay?.Dismiss();
            }
            catch (Exception ex)
            {
                BabelLog.Error("QuickChatOverlayService", "Failed to dismiss quick chat overlay", ex);
            }
        }
    }
}
