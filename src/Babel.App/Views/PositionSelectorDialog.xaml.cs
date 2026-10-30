using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Babel.Core.Models;
using Babel.Core.Services;

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

        SelectedPositionLabel.Text = LocalizationService.Instance.Format("Pos_SelectedPrefix", GetPositionDisplayName(SelectedPosition));
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
        OverlayPosition.TopLeft => LocalizationService.Instance.GetString("Pos_TopLeft"),
        OverlayPosition.TopCenter => LocalizationService.Instance.GetString("Pos_TopCenter"),
        OverlayPosition.TopRight => LocalizationService.Instance.GetString("Pos_TopRight"),
        OverlayPosition.MiddleLeft => LocalizationService.Instance.GetString("Pos_MiddleLeft"),
        OverlayPosition.Center => LocalizationService.Instance.GetString("Pos_Center"),
        OverlayPosition.MiddleRight => LocalizationService.Instance.GetString("Pos_MiddleRight"),
        OverlayPosition.BottomLeft => LocalizationService.Instance.GetString("Pos_BottomLeft"),
        OverlayPosition.BottomCenter => LocalizationService.Instance.GetString("Pos_BottomCenter"),
        OverlayPosition.BottomRight => LocalizationService.Instance.GetString("Pos_BottomRight"),
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
