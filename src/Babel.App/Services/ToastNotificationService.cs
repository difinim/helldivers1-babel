using System;
using System.Windows;
using Babel.App.Views;
using Babel.Core.Models;

namespace Babel.App.Services;

public enum ToastType
{
    Success,
    Warning,
    Error,
    Info
}

public static class ToastNotificationService
{
    private static ToastOverlayWindow? _currentToast;
    private static readonly object _lock = new();

    public static bool IsEnabled { get; set; } = true;
    public static double ScaleFactor { get; set; } = 1.0;
    public static OverlayPosition CurrentPosition { get; set; } = OverlayPosition.TopRight;
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
                if (_currentToast != null && _currentToast.IsLoaded)
                {
                    var workArea = DisplayMonitorService.GetTargetWorkArea(TargetDisplay, GetGameWindowHandle?.Invoke());
                    _currentToast.SetPosition(position, workArea);
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
                if (_currentToast != null && _currentToast.IsLoaded)
                {
                    _currentToast.SetScale(scaleFactor);
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

    public static bool IsShowingToast
    {
        get
        {
            lock (_lock)
            {
                return _currentToast != null && _currentToast.IsLoaded && _currentToast.IsVisible;
            }
        }
    }

    public static double CurrentToastBottom
    {
        get
        {
            lock (_lock)
            {
                if (_currentToast != null && _currentToast.IsLoaded && _currentToast.IsVisible)
                {
                    return _currentToast.Top + _currentToast.ActualHeight;
                }
                return 0;
            }
        }
    }

    public static void Show(string title, string message, ToastType type = ToastType.Info, int durationMs = 3000)
    {
        if (!IsEnabled) return;

        var app = Application.Current;
        if (app == null) return;

        if (app.Dispatcher.CheckAccess())
        {
            ShowInternal(title, message, type, durationMs);
        }
        else
        {
            app.Dispatcher.BeginInvoke(() => ShowInternal(title, message, type, durationMs));
        }
    }

    public static void ShowPreview(string title, string message, ToastType type = ToastType.Info, int durationMs = 3000)
    {
        var app = Application.Current;
        if (app == null) return;

        if (app.Dispatcher.CheckAccess())
        {
            ShowInternal(title, message, type, durationMs);
        }
        else
        {
            app.Dispatcher.BeginInvoke(() => ShowInternal(title, message, type, durationMs));
        }
    }

    public static void CloseCurrent()
    {
        lock (_lock)
        {
            if (_currentToast == null) return;

            var app = Application.Current;
            if (app == null) return;

            if (app.Dispatcher.CheckAccess())
            {
                try
                {
                    _currentToast.Dismiss();
                }
                catch { }
                _currentToast = null;
            }
            else
            {
                app.Dispatcher.BeginInvoke(() =>
                {
                    lock (_lock)
                    {
                        try
                        {
                            _currentToast?.Dismiss();
                        }
                        catch { }
                        _currentToast = null;
                    }
                });
            }
        }
    }

    private static void ShowInternal(string title, string message, ToastType type, int durationMs)
    {
        lock (_lock)
        {
            try
            {
                var workArea = DisplayMonitorService.GetTargetWorkArea(TargetDisplay, GetGameWindowHandle?.Invoke());

                if (_currentToast != null && _currentToast.IsLoaded)
                {
                    _currentToast.SetScale(ScaleFactor);
                    _currentToast.SetPosition(CurrentPosition, workArea);
                    _currentToast.UpdateContent(title, message, type, durationMs);
                    return;
                }

                _currentToast = new ToastOverlayWindow(ScaleFactor, CurrentPosition, workArea);
                _currentToast.Closed += (_, _) =>
                {
                    lock (_lock)
                    {
                        _currentToast = null;
                    }
                };

                _currentToast.Show();
                _currentToast.UpdateContent(title, message, type, durationMs);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to show toast overlay: {ex.Message}");
            }
        }
    }
}
