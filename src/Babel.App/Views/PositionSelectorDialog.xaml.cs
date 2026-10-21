using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Babel.Core.Models;

namespace Babel.App.Views;

public partial class PositionSelectorDialog : Window
{
    private static readonly Brush SelectedBrush = new SolidColorBrush(Color.FromRgb(0xFF, 0xE6, 0x00)); // Helldivers yellow
    private static readonly Brush UnselectedBrush = new SolidColorBrush(Color.FromRgb(0x6D, 0x6D, 0x6D));

    public OverlayPosition SelectedPosition { get; private set; }
    public Action<OverlayPosition>? PreviewCallback { get; set; }

    public PositionSelectorDialog(string title, OverlayPosition initialPosition)
    {
        InitializeComponent();
        HeaderTitle.Text = title;
        SelectedPosition = initialPosition;

        UpdateAnchorVisuals();
    }

    private void OnAnchorClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string tagStr && Enum.TryParse<OverlayPosition>(tagStr, out var newPos))
        {
            SelectedPosition = newPos;
            UpdateAnchorVisuals();
            PreviewCallback?.Invoke(SelectedPosition);
        }
    }

    private void UpdateAnchorVisuals()
    {
        SetAnchorBrush(PathTopLeft, SelectedPosition == OverlayPosition.TopLeft);
        SetAnchorBrush(PathTopCenter, SelectedPosition == OverlayPosition.TopCenter);
        SetAnchorBrush(PathTopRight, SelectedPosition == OverlayPosition.TopRight);
        SetAnchorBrush(PathMiddleLeft, SelectedPosition == OverlayPosition.MiddleLeft);
        SetAnchorBrush(PathCenter, SelectedPosition == OverlayPosition.Center);
        SetAnchorBrush(PathMiddleRight, SelectedPosition == OverlayPosition.MiddleRight);
        SetAnchorBrush(PathBottomLeft, SelectedPosition == OverlayPosition.BottomLeft);
        SetAnchorBrush(PathBottomCenter, SelectedPosition == OverlayPosition.BottomCenter);
        SetAnchorBrush(PathBottomRight, SelectedPosition == OverlayPosition.BottomRight);

        SelectedPositionLabel.Text = $"Selected: {GetPositionDisplayName(SelectedPosition)}";
    }

    private static void SetAnchorBrush(Shape shape, bool isSelected)
    {
        if (shape != null)
        {
            shape.Fill = isSelected ? SelectedBrush : UnselectedBrush;
        }
    }

    public static string GetPositionDisplayName(OverlayPosition position) => position switch
    {
        OverlayPosition.TopLeft => "Top Left (↖)",
        OverlayPosition.TopCenter => "Top Center (↑)",
        OverlayPosition.TopRight => "Top Right (↗)",
        OverlayPosition.MiddleLeft => "Middle Left (←)",
        OverlayPosition.Center => "Center (⏺)",
        OverlayPosition.MiddleRight => "Middle Right (→)",
        OverlayPosition.BottomLeft => "Bottom Left (↙)",
        OverlayPosition.BottomCenter => "Bottom Center (↓)",
        OverlayPosition.BottomRight => "Bottom Right (↘)",
        _ => position.ToString()
    };

    private void OnDoneClicked(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void OnCancelClicked(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
