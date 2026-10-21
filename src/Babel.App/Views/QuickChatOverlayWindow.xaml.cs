using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using Babel.App.Services;
using Babel.Core.Models;
using Babel.Core.Utilities;

namespace Babel.App.Views;

public partial class QuickChatOverlayWindow : Window
{
    private bool _isClosing;

    public bool IsShowing => IsVisible && !_isClosing;
    public OverlayPosition Position { get; set; } = OverlayPosition.MiddleRight;
    public Rect? TargetWorkArea { get; set; }

    public QuickChatOverlayWindow(double scale = 1.0, OverlayPosition position = OverlayPosition.MiddleRight, Rect? targetWorkArea = null)
    {
        InitializeComponent();
        Position = position;
        TargetWorkArea = targetWorkArea;
        SetScale(scale);
    }

    public void SetScale(double scale)
    {
        if (OverlayScaleTransform != null)
        {
            OverlayScaleTransform.ScaleX = scale;
            OverlayScaleTransform.ScaleY = scale;
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

    public void UpdateMacros(IEnumerable<QuickChatMacro> macros, string? toggleShortcut = null)
    {
        var items = QuickChatFormatter.GetActiveOverlayItems(macros);

        if (items.Count == 0)
        {
            EmptyNotice.Visibility = Visibility.Visible;
            MacroItemsControl.ItemsSource = null;
        }
        else
        {
            EmptyNotice.Visibility = Visibility.Collapsed;
            MacroItemsControl.ItemsSource = items;
        }

        if (!string.IsNullOrWhiteSpace(toggleShortcut))
        {
            FooterHint.Text = $"Press {toggleShortcut} to hide";
        }

        PositionWindow();
        PlayFadeIn();
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

        // Collision avoidance with active toast on the same monitor
        if (ToastNotificationService.IsShowingToast)
        {
            bool sameColumn = Position switch
            {
                OverlayPosition.TopLeft or OverlayPosition.MiddleLeft or OverlayPosition.BottomLeft =>
                    ToastNotificationService.CurrentPosition is OverlayPosition.TopLeft or OverlayPosition.MiddleLeft or OverlayPosition.BottomLeft,
                OverlayPosition.TopCenter or OverlayPosition.Center or OverlayPosition.BottomCenter =>
                    ToastNotificationService.CurrentPosition is OverlayPosition.TopCenter or OverlayPosition.Center or OverlayPosition.BottomCenter,
                _ =>
                    ToastNotificationService.CurrentPosition is OverlayPosition.TopRight or OverlayPosition.MiddleRight or OverlayPosition.BottomRight
            };

            if (sameColumn)
            {
                if (Position is OverlayPosition.TopLeft or OverlayPosition.TopCenter or OverlayPosition.TopRight or
                    OverlayPosition.MiddleLeft or OverlayPosition.Center or OverlayPosition.MiddleRight)
                {
                    double minTop = ToastNotificationService.CurrentToastBottom + 8;
                    y = Math.Max(minTop, y);
                }
            }
        }

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

    public void Dismiss()
    {
        if (_isClosing) return;
        _isClosing = true;

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

    private void OnCloseClicked(object sender, RoutedEventArgs e)
    {
        Dismiss();
    }
}
