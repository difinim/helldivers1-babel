using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Babel.Core.Utilities;

namespace Babel.App.Controls;

public static class LinkAwareTextBlock
{
    public static readonly DependencyProperty FormattedTextProperty =
        DependencyProperty.RegisterAttached(
            "FormattedText",
            typeof(string),
            typeof(LinkAwareTextBlock),
            new PropertyMetadata(string.Empty, OnFormattedTextChanged));

    public static string GetFormattedText(DependencyObject obj) =>
        (string)obj.GetValue(FormattedTextProperty);

    public static void SetFormattedText(DependencyObject obj, string value) =>
        obj.SetValue(FormattedTextProperty, value);

    private static void OnFormattedTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not TextBlock textBlock)
            return;

        string text = e.NewValue as string ?? string.Empty;

        if (string.IsNullOrEmpty(text))
        {
            textBlock.Inlines.Clear();
            textBlock.Text = string.Empty;
            return;
        }

        // Fast path: if there is no URL scheme (http:// or https://), avoid parsing runs and hyperlinks completely
        if (!text.Contains("http://", StringComparison.OrdinalIgnoreCase) &&
            !text.Contains("https://", StringComparison.OrdinalIgnoreCase))
        {
            textBlock.Inlines.Clear();
            textBlock.Text = MessageLinkParser.SanitizeText(text);
            return;
        }

        textBlock.Inlines.Clear();

        try
        {
            // Sanitize control characters and BiDi overrides to protect WPF text rendering pipeline
            string sanitized = MessageLinkParser.SanitizeText(text);

            var segments = MessageLinkParser.Parse(sanitized);
            foreach (var segment in segments)
            {
                if (segment.Type == MessageSegmentType.Link && segment.Url != null)
                {
                    // Create hyperlink without setting NavigateUri to prevent WPF internal navigation exceptions
                    var hyperlink = new Hyperlink(new Run(segment.Text))
                    {
                        Foreground = new SolidColorBrush(Color.FromRgb(0x58, 0xA6, 0xFF)),
                        Cursor = Cursors.Hand,
                        ToolTip = "Click to open link in external web browser"
                    };

                    string targetUrl = segment.Url;
                    hyperlink.Click += (sender, args) =>
                    {
                        args.Handled = true;
                        PromptAndOpenUrl(targetUrl);
                    };

                    textBlock.Inlines.Add(hyperlink);
                }
                else
                {
                    textBlock.Inlines.Add(new Run(segment.Text));
                }
            }
        }
        catch (Exception ex)
        {
            // Crash-immune fallback: render sanitized plain text run if formatting fails
            System.Diagnostics.Debug.WriteLine($"LinkAwareTextBlock formatting fallback: {ex.Message}");
            textBlock.Inlines.Clear();
            textBlock.Inlines.Add(new Run(MessageLinkParser.SanitizeText(text)));
        }
    }

    public static void PromptAndOpenUrl(string url)
    {
        // Strict verification using centralized security rules
        if (!MessageLinkParser.IsSafeBrowserUrl(url, out var uri, out string reason) || uri == null)
        {
            var owner = Application.Current?.MainWindow;
            string warningMessage = $"The selected link is not safe to open:\n\n{reason}";
            if (owner != null && owner.IsVisible)
            {
                MessageBox.Show(
                    owner,
                    warningMessage,
                    "Security Warning - Invalid Link",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            else
            {
                MessageBox.Show(
                    warningMessage,
                    "Security Warning - Invalid Link",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            return;
        }

        // Format dialog display cleanly and truncate excessively long URLs to prevent message box horizontal blowouts
        string displayUrl = uri.AbsoluteUri;
        if (displayUrl.Length > 200)
        {
            displayUrl = displayUrl.Substring(0, 197) + "...";
        }

        string hostDisplay = uri.Host;
        if (!uri.IsDefaultPort)
        {
            hostDisplay += $":{uri.Port}";
        }

        string promptMessage =
            "Do you trust this website and want to open it in your default web browser?\n\n" +
            $"Destination Host:\n{hostDisplay}\n\n" +
            $"Link Address:\n{displayUrl}\n\n" +
            "SECURITY NOTICE: Only open links from trusted teammates. Malicious websites can compromise your computer.";

        var mainWin = Application.Current?.MainWindow;
        MessageBoxResult result;
        if (mainWin != null && mainWin.IsVisible)
        {
            result = MessageBox.Show(
                mainWin,
                promptMessage,
                "Trust Link Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question,
                MessageBoxResult.No); // Default to NO (Cancel) for maximum security
        }
        else
        {
            result = MessageBox.Show(
                promptMessage,
                "Trust Link Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question,
                MessageBoxResult.No); // Default to NO (Cancel) for maximum security
        }

        if (result == MessageBoxResult.Yes)
        {
            try
            {
                // Final defense-in-depth re-check on uri.AbsoluteUri before launching process
                if (!MessageLinkParser.IsSafeBrowserUrl(uri.AbsoluteUri, out var finalUri, out _) || finalUri == null)
                {
                    return;
                }

                Process.Start(new ProcessStartInfo
                {
                    FileName = finalUri.AbsoluteUri,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                var owner = Application.Current?.MainWindow;
                string errMsg = $"Failed to open link in external browser:\n{ex.Message}";
                if (owner != null && owner.IsVisible)
                {
                    MessageBox.Show(owner, errMsg, "Browser Launch Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show(errMsg, "Browser Launch Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
