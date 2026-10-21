using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Babel.App.Services;
using Babel.Core.Models;

namespace Babel.App.Views;

public partial class ToastOverlayWindow : Window
{
    private DispatcherTimer? _dismissTimer;
    private bool _isClosing;

    public OverlayPosition Position { get; set; } = OverlayPosition.TopRight;
    public Rect? TargetWorkArea { get; set; }

    public ToastOverlayWindow(double scale = 1.0, OverlayPosition position = OverlayPosition.TopRight, Rect? targetWorkArea = null)
    {
        InitializeComponent();
        Position = position;
        TargetWorkArea = targetWorkArea;
        SetScale(scale);
    }

    public void SetScale(double scale)
    {
        if (ToastScaleTransform != null)
        {
            ToastScaleTransform.ScaleX = scale;
            ToastScaleTransform.ScaleY = scale;
            PositionWindow();
        }
    }

    public void SetPosition(OverlayPosition position, Rect? workArea = null)
    {
        Position = position;
        if (workArea.HasValue) TargetWorkArea = workArea.Value;
        PositionWindow();
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        var hwnd = new WindowInteropHelper(this).Handle;
        int exStyle = NativeWindowMethods.GetWindowLong(hwnd, NativeWindowMethods.GWL_EXSTYLE);
        NativeWindowMethods.SetWindowLong(
            hwnd,
            NativeWindowMethods.GWL_EXSTYLE,
            exStyle | NativeWindowMethods.WS_EX_NOACTIVATE | NativeWindowMethods.WS_EX_TOOLWINDOW | NativeWindowMethods.WS_EX_TOPMOST);
    }

    public void UpdateContent(string title, string message, ToastType type, int durationMs = 3000)
    {
        TitleText.Text = title;
        MessageText.Text = message;

        Color accentColor;
        string icon;
        string badge;

        switch (type)
        {
            case ToastType.Success:
                accentColor = Color.FromRgb(0x64, 0xFF, 0x78); // Green
                icon = "▶";
                badge = "ACTIVE";
                break;
            case ToastType.Warning:
                accentColor = Color.FromRgb(0xFF, 0xE6, 0x00); // Amber / Yellow
                icon = "⏸";
                badge = "PAUSED";
                break;
            case ToastType.Error:
                accentColor = Color.FromRgb(0xFF, 0x55, 0x55); // Crimson Red
                icon = "⚠";
                badge = "ERROR";
                break;
            case ToastType.Info:
            default:
                accentColor = Color.FromRgb(0x58, 0xA6, 0xFF); // Cyan / Blue
                icon = "🗕";
                badge = "TRAY";
                break;
        }

        var accentBrush = new SolidColorBrush(accentColor);
        var bgTintBrush = new SolidColorBrush(Color.FromArgb(0x28, accentColor.R, accentColor.G, accentColor.B));

        IconBadge.BorderBrush = accentBrush;
        IconBadge.Background = bgTintBrush;
        IconText.Text = icon;
        IconText.Foreground = accentBrush;

        BadgeBorder.Background = bgTintBrush;
        BadgeText.Text = badge;
        BadgeText.Foreground = accentBrush;

        ToastBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(0x60, accentColor.R, accentColor.G, accentColor.B));

        PositionWindow();
        PlayFadeIn();
        StartDismissTimer(durationMs);
    }

    private void PositionWindow()
    {
        UpdateLayout();
        var workArea = TargetWorkArea ?? SystemParameters.WorkArea;
        const double marginX = 20;
        const double marginY = 24;

        double x = Position switch
        {
            OverlayPosition.TopLeft or OverlayPosition.MiddleLeft or OverlayPosition.BottomLeft =>
                workArea.Left + marginX,
            OverlayPosition.TopCenter or OverlayPosition.Center or OverlayPosition.BottomCenter =>
                workArea.Left + (workArea.Width - ActualWidth) / 2.0,
            _ => // TopRight, MiddleRight, BottomRight
                workArea.Right - ActualWidth - marginX
        };

        double y = Position switch
        {
            OverlayPosition.TopLeft or OverlayPosition.TopCenter or OverlayPosition.TopRight =>
                workArea.Top + marginY,
            OverlayPosition.MiddleLeft or OverlayPosition.Center or OverlayPosition.MiddleRight =>
                workArea.Top + (workArea.Height - ActualHeight) / 2.0,
            _ => // BottomLeft, BottomCenter, BottomRight
                workArea.Bottom - ActualHeight - marginY
        };

        Left = Math.Max(workArea.Left, Math.Min(x, workArea.Right - ActualWidth));
        Top = Math.Max(workArea.Top, Math.Min(y, workArea.Bottom - ActualHeight));
    }

    private void PlayFadeIn()
    {
        if (Resources["FadeInStoryboard"] is Storyboard fadeIn)
        {
            fadeIn.Begin(this);
        }
        else
        {
            Opacity = 1.0;
        }
    }

    private void StartDismissTimer(int durationMs)
    {
        _dismissTimer?.Stop();
        _dismissTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(durationMs)
        };
        _dismissTimer.Tick += (s, args) =>
        {
            _dismissTimer.Stop();
            Dismiss();
        };
        _dismissTimer.Start();
    }

    public void Dismiss()
    {
        if (_isClosing) return;
        _isClosing = true;
        _dismissTimer?.Stop();

        if (Resources["FadeOutStoryboard"] is Storyboard fadeOut)
        {
            fadeOut.Completed += (_, _) => Close();
            fadeOut.Begin(this);
        }
        else
        {
            Close();
        }
    }

    private void OnToastClicked(object sender, MouseButtonEventArgs e)
    {
        Dismiss();
    }

    private void OnCloseClicked(object sender, RoutedEventArgs e)
    {
        Dismiss();
    }
}
