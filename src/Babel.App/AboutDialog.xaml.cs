using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace Babel.App;

public partial class AboutDialog : Window
{
    public AboutDialog()
    {
        InitializeComponent();
        var ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        VersionTextBlock.Text = ver != null ? $"v{ver.Major}.{ver.Minor}.{ver.Build}" : "v1.0.0";
    }

    private void OnRequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = e.Uri.AbsoluteUri,
                UseShellExecute = true
            });
        }
        catch
        {
            // Ignore if default browser cannot be opened
        }
        e.Handled = true;
    }

    private void OnCloseClicked(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
