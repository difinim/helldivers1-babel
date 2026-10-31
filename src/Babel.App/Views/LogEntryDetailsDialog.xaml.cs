using System;
using System.Windows;
using System.Windows.Media;
using Babel.Core.Models;
using Babel.Core.Utilities;

namespace Babel.App.Views;

public partial class LogEntryDetailsDialog : Window
{
    private readonly TranslationLogEntry _entry;

    public LogEntryDetailsDialog(TranslationLogEntry entry)
    {
        InitializeComponent();
        _entry = entry ?? throw new ArgumentNullException(nameof(entry));

        PlayerNameText.Text = string.IsNullOrWhiteSpace(entry.Author) ? "P1" : entry.Author;

        if (!string.IsNullOrWhiteSpace(entry.ColorHex))
        {
            try
            {
                var color = (Color)ColorConverter.ConvertFromString(entry.ColorHex);
                PlayerColorIndicator.Fill = new SolidColorBrush(color);
            }
            catch
            {
                PlayerColorIndicator.Fill = new SolidColorBrush(Colors.Gold);
            }
        }

        DetectedLangText.Text = string.IsNullOrWhiteSpace(entry.SourceLang) ? "-" : entry.SourceLang;
        DateText.Text = entry.Date;
        TimeText.Text = entry.Time;
        LatencyText.Text = string.IsNullOrWhiteSpace(entry.Latency) ? string.Empty : $"({entry.Latency})";

        OriginalMessageTextBox.Text = entry.OriginalMessage ?? string.Empty;
        TranslatedMessageTextBox.Text = entry.TranslatedMessage ?? string.Empty;
    }

    private void OnCopyOriginalClicked(object sender, RoutedEventArgs e)
    {
        MainWindow.SafeSetClipboardText(_entry.OriginalMessage);
    }

    private void OnCopyTranslatedClicked(object sender, RoutedEventArgs e)
    {
        MainWindow.SafeSetClipboardText(_entry.TranslatedMessage);
    }

    private void OnCopyFormattedClicked(object sender, RoutedEventArgs e)
    {
        var line = LogClipboardFormatter.FormatSingleLine(_entry);
        MainWindow.SafeSetClipboardText(line);
    }

    private void OnCloseClicked(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
