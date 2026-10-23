using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Babel.App.Services;
using Babel.App.ViewModels;
using Babel.Core.Models;
using Babel.Core.Services;
using Babel.Core.Utilities;

namespace Babel.App;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;
    private readonly DispatcherTimer _layoutSaveDebounceTimer;
    private GlobalHotkeyService? _hotkeyService;
    private TrayIconService? _trayIconService;
    private UpdateRelaunchCoordinator? _updateCoordinator;
    private bool _isKeyVisible;
    private bool _isExplicitExit;

    public MainWindow()
    {
        InitializeComponent();

        _viewModel = new MainViewModel();
        DataContext = _viewModel;

        _layoutSaveDebounceTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1.5)
        };
        _layoutSaveDebounceTimer.Tick += (_, _) =>
        {
            _layoutSaveDebounceTimer.Stop();
            SaveWindowAndGridConfiguration();
        };

        RestoreWindowConfiguration();
        RestoreLogColumnWidths();

        Loaded += OnWindowLoaded;
        Closing += OnWindowClosing;
        StateChanged += OnWindowStateChanged;
        LocationChanged += (_, _) => ScheduleLayoutSave();
        SizeChanged += (_, _) => ScheduleLayoutSave();
    }

    private void OnWindowLoaded(object sender, RoutedEventArgs e)
    {
        var hwnd = new WindowInteropHelper(this).Handle;

        // Initialize Global Hotkeys
        _hotkeyService = new GlobalHotkeyService();
        _hotkeyService.Initialize(hwnd);
        _hotkeyService.RegisterPauseHotkey(_viewModel.PauseHotkey);
        _hotkeyService.RegisterToggleTranslationHotkey(_viewModel.ToggleTranslationHotkey);
        _hotkeyService.RegisterToggleQuickChatHotkey(_viewModel.ToggleQuickChatHotkey);
        _hotkeyService.RegisterQuickChatOverlayHotkey(_viewModel.QuickChatOverlayHotkey);
        _hotkeyService.RegisterMacroHotkeys(_viewModel.Config.QuickChatMacros);

        _viewModel.PropertyChanged += (s, args) =>
        {
            if (args.PropertyName == nameof(MainViewModel.PauseHotkey))
            {
                _hotkeyService?.RegisterPauseHotkey(_viewModel.PauseHotkey);
            }
            else if (args.PropertyName == nameof(MainViewModel.ToggleTranslationHotkey))
            {
                _hotkeyService?.RegisterToggleTranslationHotkey(_viewModel.ToggleTranslationHotkey);
            }
            else if (args.PropertyName == nameof(MainViewModel.ToggleQuickChatHotkey))
            {
                _hotkeyService?.RegisterToggleQuickChatHotkey(_viewModel.ToggleQuickChatHotkey);
            }
            else if (args.PropertyName == nameof(MainViewModel.QuickChatOverlayHotkey))
            {
                _hotkeyService?.RegisterQuickChatOverlayHotkey(_viewModel.QuickChatOverlayHotkey);
            }
        };

        _hotkeyService.PauseHotkeyPressed += () =>
        {
            Dispatcher.Invoke(() => _viewModel.TogglePause());
        };

        _hotkeyService.ToggleTranslationHotkeyPressed += () =>
        {
            Dispatcher.Invoke(() => _viewModel.ToggleTranslation());
        };

        _hotkeyService.ToggleQuickChatHotkeyPressed += () =>
        {
            Dispatcher.Invoke(() => _viewModel.ToggleQuickChat());
        };

        _hotkeyService.QuickChatOverlayHotkeyPressed += () =>
        {
            Dispatcher.Invoke(() => _viewModel.ToggleQuickChatOverlay());
        };

        _hotkeyService.MacroHotkeyPressed += slotId =>
        {
            Dispatcher.Invoke(async () =>
            {
                await _viewModel.TestSendMacroAsync(slotId);
            });
        };

        // Track column width changes dynamically
        foreach (var col in HistoryDataGrid.Columns)
        {
            var desc = DependencyPropertyDescriptor.FromProperty(DataGridColumn.WidthProperty, typeof(DataGridColumn));
            desc?.AddValueChanged(col, (_, _) => ScheduleLayoutSave());
        }

        // Initialize System Tray
        _trayIconService = new TrayIconService(this);
        _trayIconService.Initialize();
        _trayIconService.OpenRequested += RestoreWindow;
        _trayIconService.TogglePauseRequested += () => _viewModel.TogglePause();
        _trayIconService.ExitRequested += OnExitRequested;

        if (Application.Current != null)
        {
            Application.Current.SessionEnding += (_, _) => _isExplicitExit = true;
        }

        // Initialize PasswordBox from config
        GeminiPasswordBox.Password = _viewModel.GeminiApiKey;
        GeminiPlainTextBox.Text = _viewModel.GeminiApiKey;

        // Start background pipe and monitor services
        _viewModel.StartServices();

        // Option B: Start background auto-update coordinator
        _updateCoordinator = new UpdateRelaunchCoordinator(
            new AutoUpdateService(),
            _viewModel.ProcessMonitor,
            TriggerSilentRestartAndExit);
        _updateCoordinator.Start();
    }

    private void TriggerSilentRestartAndExit()
    {
        Dispatcher.BeginInvoke(() =>
        {
            try
            {
                var currentPid = Environment.ProcessId;
                var exePath = Environment.ProcessPath ?? System.IO.Path.Combine(AppContext.BaseDirectory, "HD1-Babel.exe");

                var startInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = $"--wait-pid {currentPid}",
                    UseShellExecute = true,
                    WorkingDirectory = AppContext.BaseDirectory
                };

                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                BabelLog.Error("MainWindow", "Failed to launch updated binary during silent restart", ex);
                return;
            }

            OnExitRequested();
        });
    }

    private void OnExitRequested()
    {
        if (_isExplicitExit) return;
        _isExplicitExit = true;

        SaveWindowAndGridConfiguration();
        FlushPendingLogs();

        ToastNotificationService.CloseCurrent();
        QuickChatOverlayService.Hide();

        _updateCoordinator?.Dispose();
        _viewModel.StopServices();

        _trayIconService?.Dispose();
        _hotkeyService?.Dispose();

        Close();
        Application.Current?.Shutdown();
    }

    private void OnWindowStateChanged(object? sender, EventArgs e)
    {
        if (WindowState == WindowState.Minimized)
        {
            SaveWindowAndGridConfiguration();
            Hide();
            _trayIconService?.UpdateStatus($"Helldivers 1 Babel - {_viewModel.StatusBadgeText} [{_viewModel.TargetLanguage}]");
            ToastNotificationService.Show("Helldivers 1 Babel", "Minimized to System Tray (Active in background)", ToastType.Info);
        }
        else
        {
            ScheduleLayoutSave();
        }
    }

    private void OnWindowClosing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        if (_isExplicitExit)
        {
            return;
        }

        SaveWindowAndGridConfiguration();

        // Minimize to tray instead of closing unless explicitly exiting
        e.Cancel = true;
        Hide();
        _trayIconService?.UpdateStatus($"Helldivers 1 Babel - {_viewModel.StatusBadgeText} [{_viewModel.TargetLanguage}]");
        ToastNotificationService.Show("Helldivers 1 Babel", "Minimized to System Tray (Active in background)", ToastType.Info);
    }

    public void RestoreAndBringToForeground()
    {
        if (!IsVisible)
        {
            Show();
        }

        if (WindowState == WindowState.Minimized)
        {
            WindowState = WindowState.Normal;
        }

        Activate();
        Topmost = true;
        Topmost = false;
        Focus();

        var hwnd = new WindowInteropHelper(this).EnsureHandle();
        if (hwnd != IntPtr.Zero)
        {
            NativeWindowMethods.BringWindowToForeground(hwnd);
        }
    }

    private void RestoreWindow()
    {
        RestoreAndBringToForeground();
    }

    private void OnTogglePauseClicked(object sender, RoutedEventArgs e)
    {
        _viewModel.TogglePause();
    }

    private void OnExportLogsClicked(object sender, RoutedEventArgs e)
    {
        try
        {
            var sfd = new Microsoft.Win32.SaveFileDialog
            {
                Title = "Export Chat Logs",
                Filter = "Text Documents (*.txt)|*.txt|All Files (*.*)|*.*",
                DefaultExt = ".txt",
                FileName = $"Helldivers_Chat_Logs_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
            };

            if (sfd.ShowDialog(this) == true)
            {
                string text = _viewModel.GenerateLogsExportText();
                System.IO.File.WriteAllText(sfd.FileName, text, System.Text.Encoding.UTF8);
                MessageBox.Show(this, $"Chat logs exported successfully to:\n{sfd.FileName}", "Export Chat Logs", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            BabelLog.Error("MainWindow", "Failed to export chat logs", ex);
            MessageBox.Show(this, $"Failed to export chat logs:\n{ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void OnClearLogsClicked(object sender, RoutedEventArgs e)
    {
        _viewModel.ClearHistory();
    }

    private void HistoryDataGrid_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        var dep = e.OriginalSource as DependencyObject;
        while (dep != null && dep is not DataGridRow)
        {
            dep = VisualTreeHelper.GetParent(dep);
        }

        if (dep is DataGridRow row)
        {
            if (!row.IsSelected)
            {
                HistoryDataGrid.SelectedItem = row.Item;
            }
        }
    }

    private void HistoryDataGrid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
    {
        var dep = e.OriginalSource as DependencyObject;
        while (dep != null && dep is not DataGridRow)
        {
            if (dep is System.Windows.Controls.Primitives.DataGridColumnHeader)
            {
                e.Handled = true;
                return;
            }
            dep = VisualTreeHelper.GetParent(dep);
        }

        if (HistoryDataGrid.SelectedItems.Count == 0)
        {
            e.Handled = true;
        }
    }

    private List<TranslationLogEntry> GetSelectedOrCurrentLogEntries()
    {
        var selected = HistoryDataGrid.SelectedItems.OfType<TranslationLogEntry>().ToList();
        if (selected.Count == 0 && HistoryDataGrid.SelectedItem is TranslationLogEntry current)
        {
            selected.Add(current);
        }
        return selected;
    }

    private void OnCopyRowClicked(object sender, RoutedEventArgs e)
    {
        try
        {
            if (ApplicationCommands.Copy.CanExecute(null, HistoryDataGrid))
            {
                ApplicationCommands.Copy.Execute(null, HistoryDataGrid);
                return;
            }
        }
        catch (Exception ex)
        {
            BabelLog.Warn("MainWindow", $"Native DataGrid copy failed, falling back to manual copy: {ex.Message}");
        }

        var selected = GetSelectedOrCurrentLogEntries();
        if (selected.Count > 0)
        {
            SafeSetClipboardText(LogClipboardFormatter.FormatTabSeparatedRows(selected));
        }
    }

    private void OnCopyFormattedLineClicked(object sender, RoutedEventArgs e)
    {
        var selected = GetSelectedOrCurrentLogEntries();
        if (selected.Count > 0)
        {
            SafeSetClipboardText(LogClipboardFormatter.FormatSingleLines(selected));
        }
    }

    private void OnCopyPlayerNameClicked(object sender, RoutedEventArgs e)
    {
        var selected = GetSelectedOrCurrentLogEntries();
        if (selected.Count > 0)
        {
            SafeSetClipboardText(LogClipboardFormatter.FormatPlayerNames(selected));
        }
    }

    private void OnCopyOriginalMessageClicked(object sender, RoutedEventArgs e)
    {
        var selected = GetSelectedOrCurrentLogEntries();
        if (selected.Count > 0)
        {
            SafeSetClipboardText(LogClipboardFormatter.FormatOriginalMessages(selected));
        }
    }

    private void OnCopyTranslatedMessageClicked(object sender, RoutedEventArgs e)
    {
        var selected = GetSelectedOrCurrentLogEntries();
        if (selected.Count > 0)
        {
            SafeSetClipboardText(LogClipboardFormatter.FormatTranslatedMessages(selected));
        }
    }

    private static void SafeSetClipboardText(string text)
    {
        if (string.IsNullOrEmpty(text)) return;

        for (int i = 0; i < 3; i++)
        {
            try
            {
                Clipboard.SetText(text);
                return;
            }
            catch (System.Runtime.InteropServices.ExternalException)
            {
                System.Threading.Thread.Sleep(25);
            }
            catch (Exception ex)
            {
                BabelLog.Warn("MainWindow", $"Failed to set clipboard text: {ex.Message}");
                break;
            }
        }
    }

    private async void OnClearLogsMemoryClicked(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show(
            this,
            "Are you sure you want to clear all persistent chat logs from memory and disk?\n\nThis will permanently delete the saved message history.",
            "Clear Logs Memory",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            await _viewModel.ClearPersistentLogsAsync();
            MessageBox.Show(this, "Persistent chat logs have been cleared successfully.", "Clear Logs Memory", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    public void FlushPendingLogs()
    {
        try
        {
            Task.Run(async () => await _viewModel.FlushPersistentLogsAsync().ConfigureAwait(false))
                .Wait(TimeSpan.FromSeconds(3));
        }
        catch (Exception ex)
        {
            BabelLog.Error("MainWindow", "Failed to flush pending persistent logs during shutdown", ex);
        }
    }

    private async void OnTestSendMacroClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is int slotId)
        {
            await _viewModel.TestSendMacroAsync(slotId);
        }
    }

    private void OnImportQuickChatClicked(object sender, RoutedEventArgs e)
    {
        try
        {
            var ofd = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Import Quick Chat Messages",
                Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                DefaultExt = ".json"
            };

            if (ofd.ShowDialog(this) == true)
            {
                string json = System.IO.File.ReadAllText(ofd.FileName, System.Text.Encoding.UTF8);
                int count = _viewModel.ImportQuickChatMessagesJson(json);
                _hotkeyService?.RegisterMacroHotkeys(_viewModel.Config.QuickChatMacros);

                ToastNotificationService.Show("Quick Chat", $"Imported {count} message(s) successfully", ToastType.Success);
                MessageBox.Show(
                    this,
                    $"Successfully imported {count} Quick Chat message(s)!\n\nNote: Shortcuts and active states were preserved.",
                    "Import Quick Chat Messages",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            BabelLog.Error("MainWindow", "Failed to import quick chat messages", ex);
            MessageBox.Show(
                this,
                $"Failed to import quick chat messages:\n{ex.Message}",
                "Import Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void OnExportQuickChatClicked(object sender, RoutedEventArgs e)
    {
        try
        {
            var sfd = new Microsoft.Win32.SaveFileDialog
            {
                Title = "Export Quick Chat Messages",
                Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                DefaultExt = ".json",
                FileName = $"QuickChat_Messages_{DateTime.Now:yyyyMMdd_HHmmss}.json"
            };

            if (sfd.ShowDialog(this) == true)
            {
                string json = _viewModel.ExportQuickChatMessagesJson();
                System.IO.File.WriteAllText(sfd.FileName, json, System.Text.Encoding.UTF8);

                ToastNotificationService.Show("Quick Chat", "Messages exported successfully", ToastType.Success);
                MessageBox.Show(
                    this,
                    $"Quick Chat messages exported successfully to:\n{sfd.FileName}",
                    "Export Quick Chat Messages",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            BabelLog.Error("MainWindow", "Failed to export quick chat messages", ex);
            MessageBox.Show(
                this,
                $"Failed to export quick chat messages:\n{ex.Message}",
                "Export Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void OnSaveMacrosClicked(object sender, RoutedEventArgs e)
    {
        _viewModel.SaveConfig();
        _hotkeyService?.RegisterMacroHotkeys(_viewModel.Config.QuickChatMacros);
        _hotkeyService?.RegisterQuickChatOverlayHotkey(_viewModel.QuickChatOverlayHotkey);
        _hotkeyService?.RegisterToggleQuickChatHotkey(_viewModel.ToggleQuickChatHotkey);
        MessageBox.Show(this, "Quick Chat settings saved and hotkeys updated!", "Babel", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void OnTestQuickChatOverlayClicked(object sender, RoutedEventArgs e)
    {
        _viewModel.ToggleQuickChatOverlay();
    }

    private void OnGeminiPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (!_isKeyVisible)
        {
            _viewModel.GeminiApiKey = GeminiPasswordBox.Password;
            GeminiPlainTextBox.Text = GeminiPasswordBox.Password;
        }
    }

    private void OnToggleShowApiKeyClicked(object sender, RoutedEventArgs e)
    {
        _isKeyVisible = !_isKeyVisible;
        if (_isKeyVisible)
        {
            GeminiPlainTextBox.Text = GeminiPasswordBox.Password;
            GeminiPasswordBox.Visibility = Visibility.Collapsed;
            GeminiPlainTextBox.Visibility = Visibility.Visible;
            BtnShowHideKey.Content = "Hide";
        }
        else
        {
            GeminiPasswordBox.Password = GeminiPlainTextBox.Text;
            GeminiPlainTextBox.Visibility = Visibility.Collapsed;
            GeminiPasswordBox.Visibility = Visibility.Visible;
            BtnShowHideKey.Content = "Show";
        }
    }

    private async void OnTestApiClicked(object sender, RoutedEventArgs e)
    {
        await _viewModel.TestApiConnectionAsync();
    }

    private void OnTestToastClicked(object sender, RoutedEventArgs e)
    {
        _viewModel.ShowTestToast();
    }

    private void OnSelectToastPositionClicked(object sender, RoutedEventArgs e)
    {
        var dialog = new Views.PositionSelectorDialog(
            "Configure Toast Position",
            _viewModel.ToastPosition)
        {
            Owner = this,
            PreviewCallback = pos => _viewModel.TriggerToastPositionPreview(pos, _viewModel.TargetDisplay)
        };

        if (dialog.ShowDialog() == true)
        {
            _viewModel.ToastPosition = dialog.SelectedPosition;
        }
    }

    private void OnSelectQuickChatPositionClicked(object sender, RoutedEventArgs e)
    {
        var dialog = new Views.PositionSelectorDialog(
            "Configure Quick Chat Overlay Position",
            _viewModel.QuickChatOverlayPosition)
        {
            Owner = this,
            PreviewCallback = pos => _viewModel.TriggerQuickChatPositionPreview(pos, _viewModel.TargetDisplay)
        };

        if (dialog.ShowDialog() == true)
        {
            _viewModel.QuickChatOverlayPosition = dialog.SelectedPosition;
        }
    }

    private void OnOverlaySliderMouseDown(object sender, MouseButtonEventArgs e)
    {
        _viewModel.TriggerOverlayPreview();
    }

    private void OnResetAllSettingsClicked(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show(
            this,
            "Are you sure you want to reset all settings to their default values?",
            "Reset All Settings",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            _viewModel.ResetSettingsToDefaults();

            // Re-register default hotkeys
            _hotkeyService?.RegisterPauseHotkey(_viewModel.PauseHotkey);
            _hotkeyService?.RegisterToggleTranslationHotkey(_viewModel.ToggleTranslationHotkey);
            _hotkeyService?.RegisterToggleQuickChatHotkey(_viewModel.ToggleQuickChatHotkey);
            _hotkeyService?.RegisterQuickChatOverlayHotkey(_viewModel.QuickChatOverlayHotkey);

            // Synchronize API key UI
            GeminiPasswordBox.Password = _viewModel.GeminiApiKey;
            GeminiPlainTextBox.Text = _viewModel.GeminiApiKey;

            ToastNotificationService.Show("Settings Reset", "All settings restored to default values", ToastType.Info);
        }
    }

    private void OnSaveSettingsClicked(object sender, RoutedEventArgs e)
    {
        SaveWindowAndGridConfiguration();
        _viewModel.SaveConfig();

        // Update hotkey registration
        _hotkeyService?.RegisterPauseHotkey(_viewModel.PauseHotkey);
        _hotkeyService?.RegisterToggleTranslationHotkey(_viewModel.ToggleTranslationHotkey);
        _hotkeyService?.RegisterToggleQuickChatHotkey(_viewModel.ToggleQuickChatHotkey);
        _hotkeyService?.RegisterQuickChatOverlayHotkey(_viewModel.QuickChatOverlayHotkey);
        _hotkeyService?.RegisterMacroHotkeys(_viewModel.Config.QuickChatMacros);

        MessageBox.Show(this, "Settings saved successfully!", "Babel", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void RestoreWindowConfiguration()
    {
        // Restore window dimensions if valid
        if (_viewModel.Config.WindowWidth.HasValue && _viewModel.Config.WindowWidth.Value >= MinWidth)
        {
            Width = _viewModel.Config.WindowWidth.Value;
        }
        if (_viewModel.Config.WindowHeight.HasValue && _viewModel.Config.WindowHeight.Value >= MinHeight)
        {
            Height = _viewModel.Config.WindowHeight.Value;
        }

        // Restore window position with virtual screen bounds verification
        if (_viewModel.Config.WindowLeft.HasValue && _viewModel.Config.WindowTop.HasValue)
        {
            double savedLeft = _viewModel.Config.WindowLeft.Value;
            double savedTop = _viewModel.Config.WindowTop.Value;

            var virtualRect = new Rect(
                SystemParameters.VirtualScreenLeft,
                SystemParameters.VirtualScreenTop,
                SystemParameters.VirtualScreenWidth,
                SystemParameters.VirtualScreenHeight);

            var windowRect = new Rect(savedLeft, savedTop, Width, Height);
            var intersection = Rect.Intersect(virtualRect, windowRect);

            // Verify at least a 100x50 area of the window is visible on screen
            if (!intersection.IsEmpty && intersection.Width >= 100 && intersection.Height >= 50)
            {
                WindowStartupLocation = WindowStartupLocation.Manual;
                Left = Math.Max(SystemParameters.VirtualScreenLeft, Math.Min(savedLeft, SystemParameters.VirtualScreenLeft + SystemParameters.VirtualScreenWidth - 100));
                Top = Math.Max(SystemParameters.VirtualScreenTop, Math.Min(savedTop, SystemParameters.VirtualScreenTop + SystemParameters.VirtualScreenHeight - 50));
            }
            else
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
        }
        else
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        if (_viewModel.Config.WindowMaximized)
        {
            WindowState = WindowState.Maximized;
        }
    }

    private void RestoreLogColumnWidths()
    {
        if (_viewModel.Config.LogColumnWidths == null || _viewModel.Config.LogColumnWidths.Count == 0)
        {
            return;
        }

        foreach (var col in HistoryDataGrid.Columns)
        {
            var header = col.Header?.ToString();
            if (!string.IsNullOrEmpty(header) &&
                _viewModel.Config.LogColumnWidths.TryGetValue(header, out double savedWidth))
            {
                if (savedWidth > 0)
                {
                    col.Width = new DataGridLength(savedWidth, DataGridLengthUnitType.Pixel);
                }
                else if (savedWidth < 0)
                {
                    col.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                }
            }
        }
    }

    private void ScheduleLayoutSave()
    {
        if (!IsLoaded) return;
        _layoutSaveDebounceTimer.Stop();
        _layoutSaveDebounceTimer.Start();
    }

    public void SaveWindowAndGridConfiguration()
    {
        try
        {
            _layoutSaveDebounceTimer.Stop();

            if (WindowState == WindowState.Maximized)
            {
                _viewModel.Config.WindowMaximized = true;
                if (!RestoreBounds.IsEmpty)
                {
                    _viewModel.Config.WindowWidth = RestoreBounds.Width;
                    _viewModel.Config.WindowHeight = RestoreBounds.Height;
                    _viewModel.Config.WindowLeft = RestoreBounds.Left;
                    _viewModel.Config.WindowTop = RestoreBounds.Top;
                }
            }
            else if (WindowState == WindowState.Normal)
            {
                _viewModel.Config.WindowMaximized = false;
                _viewModel.Config.WindowWidth = ActualWidth > 0 ? ActualWidth : Width;
                _viewModel.Config.WindowHeight = ActualHeight > 0 ? ActualHeight : Height;
                _viewModel.Config.WindowLeft = Left;
                _viewModel.Config.WindowTop = Top;
            }
            else if (WindowState == WindowState.Minimized && !RestoreBounds.IsEmpty)
            {
                _viewModel.Config.WindowWidth = RestoreBounds.Width;
                _viewModel.Config.WindowHeight = RestoreBounds.Height;
                _viewModel.Config.WindowLeft = RestoreBounds.Left;
                _viewModel.Config.WindowTop = RestoreBounds.Top;
            }

            if (HistoryDataGrid != null)
            {
                _viewModel.Config.LogColumnWidths ??= new(StringComparer.OrdinalIgnoreCase);
                foreach (var col in HistoryDataGrid.Columns)
                {
                    var header = col.Header?.ToString();
                    if (string.IsNullOrEmpty(header)) continue;

                    if (col.Width.IsStar)
                    {
                        _viewModel.Config.LogColumnWidths[header] = -1;
                    }
                    else if (col.Width.IsAbsolute)
                    {
                        double width = col.ActualWidth > 0 ? col.ActualWidth : col.Width.Value;
                        if (width > 20)
                        {
                            _viewModel.Config.LogColumnWidths[header] = Math.Round(width, 1);
                        }
                    }
                }
            }

            _viewModel.SaveConfig();
        }
        catch (Exception ex)
        {
            BabelLog.Warn("MainWindow", $"Failed to save window and grid configuration: {ex.Message}");
        }
    }

    private void OnOpenLogsClicked(object sender, RoutedEventArgs e)
    {
        _viewModel.OpenLogsFolder();
    }

    private void OnAboutClicked(object sender, RoutedEventArgs e)
    {
        var aboutDialog = new AboutDialog
        {
            Owner = this
        };
        aboutDialog.ShowDialog();
    }

    private void OnGitHubLinkClicked(object sender, RoutedEventArgs e)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/difinim/helldivers1-babel",
                UseShellExecute = true
            });
        }
        catch
        {
            // Ignore if default browser cannot be opened
        }
    }
}