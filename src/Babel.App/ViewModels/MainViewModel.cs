using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using Babel.Core.Ipc;
using Babel.Core.Models;
using Babel.Core.Services;
using Babel.Core.Utilities;
using Babel.Providers;
using Babel.App.Services;
using Babel.App.Views;

namespace Babel.App.ViewModels;

public class MacroSlotViewModel : INotifyPropertyChanged
{
    private string _keybinding = string.Empty;
    private string _messageText = string.Empty;
    private bool _isEnabled = true;
    private bool _showBottomBorder = true;

    public int SlotId { get; set; }

    public string Keybinding
    {
        get => _keybinding;
        set { _keybinding = value; OnPropertyChanged(); }
    }

    public string MessageText
    {
        get => _messageText;
        set
        {
            var clamped = value != null && value.Length > QuickChatMacro.MaxMessageLength
                ? value[..QuickChatMacro.MaxMessageLength]
                : (value ?? string.Empty);

            if (_messageText != clamped)
            {
                _messageText = clamped;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CharacterCount));
                OnPropertyChanged(nameof(CharacterCountDisplay));
                OnPropertyChanged(nameof(CharacterCountColor));
            }
        }
    }

    public int CharacterCount => _messageText?.Length ?? 0;

    public string CharacterCountDisplay => $"{CharacterCount}/{QuickChatMacro.MaxMessageLength}";

    public string CharacterCountColor => CharacterCount switch
    {
        >= QuickChatMacro.MaxMessageLength => "#E06C75",
        >= 250 => "#FFE600",
        _ => "#737988"
    };

    public bool IsEnabled
    {
        get => _isEnabled;
        set { _isEnabled = value; OnPropertyChanged(); }
    }

    public bool ShowBottomBorder
    {
        get => _showBottomBorder;
        set { _showBottomBorder = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public class MainViewModel : INotifyPropertyChanged
{
    private readonly ConfigurationService _configService;
    private BabelConfig _config;

    private string _gameStatusKey = "Status_GameWaiting";
    private object[] _gameStatusArgs = Array.Empty<object>();
    private string _gameStatusText = string.Empty;
    private bool _isGameRunning;
    private bool _isPipeConnected;
    private bool _isPaused;
    private string _testApiStatus = string.Empty;
    private bool _isTestingApi;
    private System.Windows.Threading.DispatcherTimer? _previewIdleTimer;
    private bool _isViewModelInitialized;
    private System.Diagnostics.Process? _currentGameProcess;

    public ConfigurationService ConfigService => _configService;
    public BabelConfig Config => _config;

    public string AppVersion => $"v{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "1.0.0"}";

    public NamedPipeServer PipeServer { get; }
    public TranslationEngine TranslationEngine { get; }
    public MacroService MacroService { get; }
    public WelcomeMessageService WelcomeMessageService { get; }
    public ProcessMonitorService ProcessMonitor { get; }
    public InjectorService InjectorService { get; }
    public PersistentChatLogService PersistentLogService => _persistentLogService;

    private readonly PersistentChatLogService _persistentLogService;

    public ObservableCollection<TranslationLogEntry> History { get; } = new();
    public ICollectionView HistoryView { get; }
    public ObservableCollection<MacroSlotViewModel> Macros { get; } = new();
    public ObservableCollection<TranslationTone> AvailableTranslationTones { get; } = new([
        TranslationTone.Casual,
        TranslationTone.Normal
    ]);
    public ObservableCollection<string> AvailableDateFormats { get; } = new([
        "yyyy-MM-dd",
        "dd/MM/yyyy",
        "MM/dd/yyyy",
        "dd.MM.yyyy",
        "yyyy/MM/dd",
        "dd-MM-yyyy"
    ]);
    public ObservableCollection<string> AvailableTimeFormats { get; } = new([
        "HH:mm:ss",
        "hh:mm:ss tt",
        "HH:mm",
        "hh:mm tt"
    ]);

    public string[] AvailableLanguages { get; } = new[]
    {
        "Arabic",
        "Bengali",
        "Chinese",
        "Czech",
        "Dutch",
        "English",
        "French",
        "German",
        "Greek",
        "Hindi",
        "Hungarian",
        "Indonesian",
        "Italian",
        "Japanese",
        "Korean",
        "Malay",
        "Persian (Farsi)",
        "Polish",
        "Portuguese (PT-BR)",
        "Romanian",
        "Russian",
        "Spanish",
        "Swedish",
        "Tagalog (Filipino)",
        "Thai",
        "Turkish",
        "Ukrainian",
        "Vietnamese"
    };

    public string[] AvailableProviders { get; } = new[]
    {
        "Gemini",
        "OpenAI",
        "Claude",
        "DeepSeek"
    };

    public string GameStatusText
    {
        get => _gameStatusText;
        set { _gameStatusText = value; OnPropertyChanged(); }
    }

    public bool IsGameRunning
    {
        get => _isGameRunning;
        set { _isGameRunning = value; OnPropertyChanged(); }
    }

    public bool IsPipeConnected
    {
        get => _isPipeConnected;
        set { _isPipeConnected = value; OnPropertyChanged(); }
    }

    private bool _isTranslationPaused;
    private bool _isQuickChatPaused;

    public bool IsPaused
    {
        get => _isPaused;
        set
        {
            if (_isPaused != value)
            {
                _isPaused = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(StatusBadgeText));
                OnPropertyChanged(nameof(StatusBadgeColor));
                OnPropertyChanged(nameof(PauseResumeButtonText));
                UpdateEngineStates();
            }
        }
    }

    public bool IsTranslationPaused
    {
        get => _isTranslationPaused;
        set
        {
            if (_isTranslationPaused != value)
            {
                _isTranslationPaused = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(StatusBadgeText));
                OnPropertyChanged(nameof(StatusBadgeColor));
                UpdateEngineStates();
            }
        }
    }

    public bool IsQuickChatPaused
    {
        get => _isQuickChatPaused;
        set
        {
            if (_isQuickChatPaused != value)
            {
                _isQuickChatPaused = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(StatusBadgeText));
                OnPropertyChanged(nameof(StatusBadgeColor));
                UpdateEngineStates();
            }
        }
    }

    public string StatusBadgeText
    {
        get
        {
            if (IsPaused) return LocalizationService.Instance.GetString("Badge_Paused");
            if (IsTranslationPaused && IsQuickChatPaused) return LocalizationService.Instance.GetString("Badge_AllMuted");
            if (IsTranslationPaused) return LocalizationService.Instance.GetString("Badge_TransPaused");
            if (IsQuickChatPaused) return LocalizationService.Instance.GetString("Badge_QcPaused");
            return LocalizationService.Instance.GetString("Badge_Active");
        }
    }

    public string StatusBadgeColor
    {
        get
        {
            if (IsPaused) return "#FFCC00";
            if (IsTranslationPaused || IsQuickChatPaused) return "#FFA500";
            return "#00E676";
        }
    }

    public string PauseResumeButtonText => IsPaused
        ? LocalizationService.Instance.Format("Btn_Resume", PauseHotkey)
        : LocalizationService.Instance.Format("Btn_Pause", PauseHotkey);

    public string TestApiStatus
    {
        get => _testApiStatus;
        set { _testApiStatus = value; OnPropertyChanged(); }
    }

    public bool IsTestingApi
    {
        get => _isTestingApi;
        set { _isTestingApi = value; OnPropertyChanged(); }
    }

    public string TargetLanguage
    {
        get => _config.TargetLanguage;
        set
        {
            if (_config.TargetLanguage != value)
            {
                _config.TargetLanguage = value;
                OnPropertyChanged();
                SaveConfig();
                AppLocalizationManager.SetLanguage(value);
                RefreshLocalizedProperties();
            }
        }
    }

    public void RefreshLocalizedProperties()
    {
        OnPropertyChanged(nameof(StatusBadgeText));
        OnPropertyChanged(nameof(PauseResumeButtonText));
        OnPropertyChanged(nameof(ToastPositionDisplay));
        OnPropertyChanged(nameof(QuickChatOverlayPositionDisplay));
        OnPropertyChanged(nameof(LogSearchFilterStatus));
        UpdateGameStatusText();
    }

    public void SetGameStatus(string key, params object[] args)
    {
        _gameStatusKey = key;
        _gameStatusArgs = args;
        UpdateGameStatusText();
    }

    public void UpdateGameStatusText()
    {
        GameStatusText = LocalizationService.Instance.Format(_gameStatusKey, _gameStatusArgs);
    }

    public TranslationTone TranslationTone
    {
        get => _config.TranslationTone;
        set
        {
            if (_config.TranslationTone != value)
            {
                _config.TranslationTone = value;
                OnPropertyChanged();
                SaveConfig();
            }
        }
    }

    public string DateFormat
    {
        get => _config.DateFormat;
        set
        {
            if (_config.DateFormat != value)
            {
                _config.DateFormat = value;
                OnPropertyChanged();
                SaveConfig();
                RefreshLogDatesAndTimes();
            }
        }
    }

    public string TimeFormat
    {
        get => _config.TimeFormat;
        set
        {
            if (_config.TimeFormat != value)
            {
                _config.TimeFormat = value;
                OnPropertyChanged();
                SaveConfig();
                RefreshLogDatesAndTimes();
            }
        }
    }

    public string FormatDate(DateTime dt)
    {
        try
        {
            return dt.ToString(string.IsNullOrWhiteSpace(_config.DateFormat) ? "yyyy-MM-dd" : _config.DateFormat, CultureInfo.InvariantCulture);
        }
        catch
        {
            return dt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
    }

    public string FormatTime(DateTime dt)
    {
        try
        {
            return dt.ToString(string.IsNullOrWhiteSpace(_config.TimeFormat) ? "HH:mm:ss" : _config.TimeFormat, CultureInfo.InvariantCulture);
        }
        catch
        {
            return dt.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
        }
    }

    public void RefreshLogDatesAndTimes()
    {
        foreach (var entry in History)
        {
            entry.Date = FormatDate(entry.Timestamp);
            entry.Time = FormatTime(entry.Timestamp);
        }
        if (HasLogSearchQuery)
        {
            ApplyLogSearchAsync(_logSearchQuery);
        }
        else
        {
            HistoryView?.Refresh();
            OnPropertyChanged(nameof(LogSearchFilterStatus));
        }
    }

    private string _logSearchQuery = string.Empty;
    private HashSet<TranslationLogEntry>? _matchedEntries;
    private int _filteredLogCount;
    private CancellationTokenSource? _searchCts;
    private string[]? _activeSearchTokens;

    public string LogSearchQuery
    {
        get => _logSearchQuery;
        set
        {
            var clamped = value ?? string.Empty;
            if (_logSearchQuery != clamped)
            {
                _logSearchQuery = clamped;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasLogSearchQuery));
                ApplyLogSearchAsync(clamped);
            }
        }
    }

    public bool HasLogSearchQuery => !string.IsNullOrWhiteSpace(_logSearchQuery);

    public string LogSearchFilterStatus
    {
        get
        {
            if (HistoryView == null) return string.Empty;
            if (HasLogSearchQuery)
            {
                return LocalizationService.Instance.Format("Trans_LogSearch_StatusCount", _filteredLogCount, History.Count);
            }
            return LocalizationService.Instance.Format("Trans_LogSearch_StatusTotal", History.Count);
        }
    }

    public void ClearLogSearch()
    {
        LogSearchQuery = string.Empty;
    }

    private void ApplyLogSearchAsync(string query)
    {
        _searchCts?.Cancel();
        _searchCts?.Dispose();
        _searchCts = null;

        if (string.IsNullOrWhiteSpace(query))
        {
            _matchedEntries = null;
            _activeSearchTokens = null;
            _filteredLogCount = History.Count;
            HistoryView?.Refresh();
            OnPropertyChanged(nameof(LogSearchFilterStatus));
            return;
        }

        var tokens = LogSearchFilter.Tokenize(query);
        _activeSearchTokens = tokens;
        var snapshot = History.ToList();
        var cts = new CancellationTokenSource();
        _searchCts = cts;
        var token = cts.Token;

        Task.Run(() =>
        {
            var matches = new HashSet<TranslationLogEntry>(ReferenceEqualityComparer.Instance);
            for (int i = 0; i < snapshot.Count; i++)
            {
                if (token.IsCancellationRequested) return;
                var entry = snapshot[i];
                if (LogSearchFilter.Matches(entry, tokens))
                {
                    matches.Add(entry);
                }
            }

            if (token.IsCancellationRequested) return;

            void ApplyResults()
            {
                if (token.IsCancellationRequested) return;

                // Include any items added to History that arrived while searching
                if (_activeSearchTokens != null)
                {
                    for (int i = 0; i < History.Count; i++)
                    {
                        var entry = History[i];
                        if (snapshot.Contains(entry)) break;
                        if (LogSearchFilter.Matches(entry, _activeSearchTokens))
                        {
                            matches.Add(entry);
                        }
                    }
                }

                _matchedEntries = matches;
                _filteredLogCount = matches.Count;
                HistoryView?.Refresh();
                OnPropertyChanged(nameof(LogSearchFilterStatus));
            }

            if (Application.Current?.Dispatcher != null && !Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.BeginInvoke(ApplyResults);
            }
            else
            {
                ApplyResults();
            }
        }, token);
    }

    private bool FilterLogEntry(object obj)
    {
        if (_matchedEntries == null) return true;
        return obj is TranslationLogEntry entry && _matchedEntries.Contains(entry);
    }

    public bool IncomingEnabled
    {
        get => _config.IncomingTranslationEnabled;
        set
        {
            _config.IncomingTranslationEnabled = value;
            OnPropertyChanged();
            SaveConfig();
        }
    }

    public bool OutgoingEnabled
    {
        get => _config.OutgoingTranslationEnabled;
        set
        {
            _config.OutgoingTranslationEnabled = value;
            OnPropertyChanged();
            SaveConfig();
        }
    }

    public bool DontTranslateStandardOutgoing
    {
        get => _config.DontTranslateStandardOutgoing;
        set
        {
            _config.DontTranslateStandardOutgoing = value;
            OnPropertyChanged();
            SaveConfig();
        }
    }

    public bool DontTranslateIncomingInTargetLanguage
    {
        get => _config.DontTranslateIncomingInTargetLanguage;
        set
        {
            _config.DontTranslateIncomingInTargetLanguage = value;
            OnPropertyChanged();
            SaveConfig();
        }
    }

    public string GeminiApiKey
    {
        get => _config.ApiKeys.GetValueOrDefault("Gemini", "");
        set
        {
            _config.ApiKeys["Gemini"] = value;
            OnPropertyChanged();
            SaveConfig();
        }
    }

    public string GeminiModel
    {
        get => _config.ProviderModels.GetValueOrDefault("Gemini", "gemini-flash-lite-latest");
        set
        {
            _config.ProviderModels["Gemini"] = value;
            OnPropertyChanged();
            SaveConfig();
        }
    }

    public double TimeoutSeconds
    {
        get => _config.TimeoutSeconds;
        set
        {
            _config.TimeoutSeconds = value;
            OnPropertyChanged();
            SaveConfig();
        }
    }

    public string PauseHotkey
    {
        get => _config.GlobalPauseHotkey;
        set
        {
            if (_config.GlobalPauseHotkey != value)
            {
                _config.GlobalPauseHotkey = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PauseResumeButtonText));
                SaveConfig();
            }
        }
    }

    public string ToggleTranslationHotkey
    {
        get => _config.ToggleTranslationHotkey;
        set
        {
            if (_config.ToggleTranslationHotkey != value)
            {
                _config.ToggleTranslationHotkey = value;
                OnPropertyChanged();
                SaveConfig();
            }
        }
    }

    public string ToggleQuickChatHotkey
    {
        get => _config.ToggleQuickChatHotkey;
        set
        {
            if (_config.ToggleQuickChatHotkey != value)
            {
                _config.ToggleQuickChatHotkey = value;
                OnPropertyChanged();
                SaveConfig();
            }
        }
    }

    public string QuickChatOverlayHotkey
    {
        get => _config.QuickChatOverlayHotkey;
        set
        {
            if (_config.QuickChatOverlayHotkey != value)
            {
                _config.QuickChatOverlayHotkey = value;
                OnPropertyChanged();
                SaveConfig();
            }
        }
    }

    public bool DebugLoggingEnabled
    {
        get => _config.DebugLoggingEnabled;
        set
        {
            _config.DebugLoggingEnabled = value;
            BabelLog.IsEnabled = value;
            OnPropertyChanged();
            SaveConfig();
        }
    }

    public int ChatHistoryLimitMb
    {
        get => _config.ChatHistoryLimitMb;
        set
        {
            int clamped = Math.Clamp(value, 0, 100);
            if (_config.ChatHistoryLimitMb != clamped)
            {
                _config.ChatHistoryLimitMb = clamped;
                _persistentLogService.LimitMb = clamped;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ChatHistoryLimitDisplay));
                SaveConfig();
            }
        }
    }

    public string ChatHistoryLimitDisplay => ChatHistoryLimitMb switch
    {
        0 => "0 MB (Off)",
        1 => "1 MB (Default)",
        _ => $"{ChatHistoryLimitMb} MB"
    };

    public int OverlayScalePercent
    {
        get => _config.OverlayScalePercent;
        set
        {
            int clamped = Math.Clamp(value, 50, 200);
            int snapped = (int)Math.Round(clamped / 25.0) * 25;
            if (_config.OverlayScalePercent != snapped)
            {
                _config.OverlayScalePercent = snapped;
                double scale = _config.OverlayScaleFactor;
                ToastNotificationService.UpdateScale(scale);
                QuickChatOverlayService.UpdateScale(scale);
                OnPropertyChanged();
                OnPropertyChanged(nameof(OverlayScaleDisplay));
                SaveConfig();

                if (_isViewModelInitialized)
                {
                    TriggerOverlayPreview();
                }
            }
        }
    }

    [Obsolete("Use OverlayScalePercent instead")]
    public int OverlayScaleLevel
    {
        get => _config.OverlayScaleLevel;
        set => OverlayScalePercent = 100 + (value - 1) * 25;
    }

    public string OverlayScaleDisplay => OverlayScalePercent switch
    {
        100 => "100% (Default)",
        _ => $"{OverlayScalePercent}%"
    };

    public void TriggerOverlayPreview()
    {
        _previewIdleTimer?.Stop();

        double scale = _config.OverlayScaleFactor;
        ToastNotificationService.UpdateScale(scale);
        ToastNotificationService.ShowPreview(
            "Overlay & Toast Size Preview",
            $"Size set to {OverlayScaleDisplay} (auto-closes in 2s idle)",
            Babel.App.Services.ToastType.Info,
            durationMs: 8000);

        QuickChatOverlayService.UpdateScale(scale);
        SyncMacrosToConfig();
        QuickChatOverlayService.Show(_config.QuickChatMacros, QuickChatOverlayHotkey);

        _previewIdleTimer ??= new System.Windows.Threading.DispatcherTimer();
        _previewIdleTimer.Interval = TimeSpan.FromSeconds(2.0);
        _previewIdleTimer.Tick -= OnPreviewIdleTimerTick;
        _previewIdleTimer.Tick += OnPreviewIdleTimerTick;
        _previewIdleTimer.Start();
    }

    private void OnPreviewIdleTimerTick(object? sender, EventArgs e)
    {
        _previewIdleTimer?.Stop();
        ToastNotificationService.CloseCurrent();
        QuickChatOverlayService.Hide();
    }

    public string TargetDisplay
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_config.TargetDisplay) ||
                string.Equals(_config.TargetDisplay, "Auto", StringComparison.OrdinalIgnoreCase) ||
                _config.TargetDisplay.Contains("Auto", StringComparison.OrdinalIgnoreCase))
            {
                return AvailableDisplays.FirstOrDefault(d => d.Contains("Auto", StringComparison.OrdinalIgnoreCase))
                       ?? "Auto (Follow Helldivers Window) - Default";
            }

            if (string.Equals(_config.TargetDisplay, "Primary", StringComparison.OrdinalIgnoreCase) ||
                _config.TargetDisplay.StartsWith("Primary", StringComparison.OrdinalIgnoreCase))
            {
                return AvailableDisplays.FirstOrDefault(d => d.StartsWith("Primary", StringComparison.OrdinalIgnoreCase))
                       ?? "Primary Display";
            }

            return AvailableDisplays.FirstOrDefault(d => d.Equals(_config.TargetDisplay, StringComparison.OrdinalIgnoreCase))
                   ?? _config.TargetDisplay;
        }
        set
        {
            if (_config.TargetDisplay != value && !string.IsNullOrWhiteSpace(value))
            {
                _config.TargetDisplay = value;
                ToastNotificationService.TargetDisplay = value;
                QuickChatOverlayService.TargetDisplay = value;
                OnPropertyChanged();
                SaveConfig();
            }
        }
    }

    public List<string> AvailableDisplays
    {
        get
        {
            var list = new List<string>
            {
                "Auto (Follow Helldivers Window) - Default",
                "Primary Display"
            };
            var monitors = DisplayMonitorService.GetMonitors();
            foreach (var m in monitors)
            {
                list.Add(m.DisplayName);
            }
            return list;
        }
    }

    public OverlayPosition ToastPosition
    {
        get => _config.ToastPosition;
        set
        {
            if (_config.ToastPosition != value)
            {
                _config.ToastPosition = value;
                ToastNotificationService.CurrentPosition = value;
                ToastNotificationService.UpdatePosition(value, TargetDisplay);
                OnPropertyChanged();
                OnPropertyChanged(nameof(ToastPositionDisplay));
                SaveConfig();
            }
        }
    }

    public string ToastPositionDisplay => PositionSelectorDialog.GetPositionDisplayName(ToastPosition);

    public OverlayPosition QuickChatOverlayPosition
    {
        get => _config.QuickChatOverlayPosition;
        set
        {
            if (_config.QuickChatOverlayPosition != value)
            {
                _config.QuickChatOverlayPosition = value;
                QuickChatOverlayService.CurrentPosition = value;
                QuickChatOverlayService.UpdatePosition(value, TargetDisplay);
                OnPropertyChanged();
                OnPropertyChanged(nameof(QuickChatOverlayPositionDisplay));
                SaveConfig();
            }
        }
    }

    public string QuickChatOverlayPositionDisplay => PositionSelectorDialog.GetPositionDisplayName(QuickChatOverlayPosition);

    public void TriggerToastPositionPreview(OverlayPosition position, string display)
    {
        _previewIdleTimer?.Stop();

        ToastNotificationService.TargetDisplay = display;
        ToastNotificationService.CurrentPosition = position;
        ToastNotificationService.ShowPreview(
            "Toast Position Preview",
            $"Anchor: {PositionSelectorDialog.GetPositionDisplayName(position)}",
            Babel.App.Services.ToastType.Info,
            durationMs: 8000);

        _previewIdleTimer ??= new System.Windows.Threading.DispatcherTimer();
        _previewIdleTimer.Interval = TimeSpan.FromSeconds(2.0);
        _previewIdleTimer.Tick -= OnPreviewIdleTimerTick;
        _previewIdleTimer.Tick += OnPreviewIdleTimerTick;
        _previewIdleTimer.Start();
    }

    public void TriggerQuickChatPositionPreview(OverlayPosition position, string display)
    {
        _previewIdleTimer?.Stop();

        QuickChatOverlayService.TargetDisplay = display;
        QuickChatOverlayService.CurrentPosition = position;
        SyncMacrosToConfig();
        QuickChatOverlayService.Show(_config.QuickChatMacros, QuickChatOverlayHotkey);

        _previewIdleTimer ??= new System.Windows.Threading.DispatcherTimer();
        _previewIdleTimer.Interval = TimeSpan.FromSeconds(2.0);
        _previewIdleTimer.Tick -= OnPreviewIdleTimerTick;
        _previewIdleTimer.Tick += OnPreviewIdleTimerTick;
        _previewIdleTimer.Start();
    }

    public bool ShowToastOverlay
    {
        get => _config.ShowToastOverlay;
        set
        {
            if (_config.ShowToastOverlay != value)
            {
                _config.ShowToastOverlay = value;
                ToastNotificationService.IsEnabled = value;
                if (!value)
                {
                    ToastNotificationService.CloseCurrent();
                }
                OnPropertyChanged();
                SaveConfig();
            }
        }
    }

    public void ShowTestToast()
    {
        if (!ShowToastOverlay)
        {
            System.Windows.MessageBox.Show(
                "Toast notifications are currently deactivated.\n\nCheck 'Enable On-Screen Toast Overlay' to activate toasts.",
                "Toast Notifications Deactivated",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information);
            return;
        }

        ToastNotificationService.Show(
            "Helldivers 1 Babel",
            "Toast overlay notification test successful!",
            Babel.App.Services.ToastType.Success,
            durationMs: 3000);
    }

    public void OpenLogsFolder()
    {
        try
        {
            var dir = BabelLog.LogDirectory;
            Directory.CreateDirectory(dir);
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = dir,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            BabelLog.Error("MainViewModel", "Failed to open logs folder", ex);
        }
    }

    public MainViewModel()
    {
        _configService = new ConfigurationService();
        _config = _configService.Load();
        AppLocalizationManager.Initialize(_config.TargetLanguage);
        LocalizationService.Instance.LanguageChanged += (_, _) => RefreshLocalizedProperties();
        SetGameStatus("Status_GameWaiting");

        BabelLog.IsEnabled = _config.DebugLoggingEnabled;
        ToastNotificationService.IsEnabled = _config.ShowToastOverlay;
        ToastNotificationService.ScaleFactor = _config.OverlayScaleFactor;
        ToastNotificationService.CurrentPosition = _config.ToastPosition;
        ToastNotificationService.TargetDisplay = _config.TargetDisplay;
        ToastNotificationService.GetGameWindowHandle = () => _currentGameProcess?.MainWindowHandle;

        QuickChatOverlayService.ScaleFactor = _config.OverlayScaleFactor;
        QuickChatOverlayService.CurrentPosition = _config.QuickChatOverlayPosition;
        QuickChatOverlayService.TargetDisplay = _config.TargetDisplay;
        QuickChatOverlayService.GetGameWindowHandle = () => _currentGameProcess?.MainWindowHandle;
        BabelLog.Info("MainViewModel", "Helldivers 1 Babel initializing...");

        Action<string> appLogger = msg =>
        {
            BabelLog.Info("NativeService", msg);
        };

        PipeServer = new NamedPipeServer();
        _persistentLogService = new PersistentChatLogService(limitMb: _config.ChatHistoryLimitMb);

        HistoryView = CollectionViewSource.GetDefaultView(History);
        HistoryView.Filter = FilterLogEntry;

        // Load persisted chat history
        var previousLogs = _persistentLogService.LoadRecent(maxEntries: 500);
        foreach (var entry in previousLogs)
        {
            if (entry.Timestamp == default && DateTime.TryParse(entry.Time, out var parsed))
            {
                entry.Timestamp = parsed;
            }
            entry.Date = FormatDate(entry.Timestamp);
            entry.Time = FormatTime(entry.Timestamp);
            History.Add(entry);
        }

        TranslationEngine = new TranslationEngine(PipeServer, _config, null, id => ProviderFactory.GetProvider(id));
        MacroService = new MacroService(
            PipeServer,
            _config,
            () => IsPaused,
            msg => TranslationEngine?.RegisterRecentBroadcast(msg),
            () => TranslationEngine?.LocalPlayerIndex ?? 1);
        WelcomeMessageService = new WelcomeMessageService(
            PipeServer,
            _config,
            () => IsPaused || IsQuickChatPaused,
            msg => TranslationEngine?.RegisterRecentBroadcast(msg),
            null,
            () => TranslationEngine?.LocalPlayerIndex ?? 1);

        ProcessMonitor = new ProcessMonitorService("helldivers", TimeSpan.FromMilliseconds(1500), appLogger);
        InjectorService = new InjectorService(appLogger);

        // Populate macros
        foreach (var m in _config.QuickChatMacros)
        {
            Macros.Add(new MacroSlotViewModel
            {
                SlotId = m.SlotId,
                Keybinding = m.Keybinding,
                MessageText = m.MessageText,
                IsEnabled = m.IsEnabled
            });
        }
        UpdateMacroSeparators();

        // Hook engine events
        TranslationEngine.MessageTranslated += OnMessageTranslated;
        TranslationEngine.TranslationRequestFailed += OnTranslationRequestFailed;
        TranslationEngine.TranslationFailed += error =>
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                ToastNotificationService.Show("Translation Error", error, ToastType.Error, durationMs: 3500);
            });
        };
        WelcomeMessageService.WelcomeMessageSent += OnWelcomeMessageSent;
        MacroService.MacroExecuted += OnMacroExecuted;

        PipeServer.ClientConnected += () =>
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                IsPipeConnected = true;
                if (IsGameRunning)
                {
                    SetGameStatus("Status_GameRunningConnected");
                }
            });
        };
        PipeServer.ClientDisconnected += () =>
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                IsPipeConnected = false;
                if (IsGameRunning)
                {
                    SetGameStatus("Status_GameRunningHookDisconnected");
                }
            });
        };

        ProcessMonitor.ProcessDetected += OnGameDetected;
        ProcessMonitor.ProcessTerminated += OnGameTerminated;

        _isViewModelInitialized = true;
    }

    public void StartServices()
    {
        _ = PipeServer.StartAsync();
        ProcessMonitor.Start();
    }

    public void StopServices()
    {
        try
        {
            ProcessMonitor.Dispose();
        }
        catch { }

        try
        {
            PipeServer.Dispose();
        }
        catch { }

        try
        {
            WelcomeMessageService?.Dispose();
        }
        catch { }

        try
        {
            _persistentLogService.Dispose();
        }
        catch { }
    }

    private async void OnGameDetected(System.Diagnostics.Process proc)
    {
        _currentGameProcess = proc;
        var dispatcher = Application.Current?.Dispatcher;
        if (dispatcher == null) return;

        await dispatcher.InvokeAsync(async () =>
        {
            IsGameRunning = true;
            SetGameStatus("Status_GameDetectedPreparing", proc.Id);

            // Delay injection by 1.5s to let the game complete PE loading, CRT startup, and unpacking
            await Task.Delay(1500);

            try
            {
                if (!proc.HasExited)
                {
                    bool injected = TryInjectDll(proc.Id);
                    SetGameStatus(injected ? "Status_GameRunningInjected" : "Status_GameRunningInjectionFailed", proc.Id);
                }
            }
            catch (Exception ex)
            {
                SetGameStatus("Status_GameRunningError", proc.Id, ex.Message);
            }
        });
    }

    private void OnGameTerminated()
    {
        _currentGameProcess = null;
        Application.Current?.Dispatcher.Invoke(() =>
        {
            IsGameRunning = false;
            SetGameStatus("Status_GameWaiting");
        });
    }

    private bool TryInjectDll(int pid)
    {
        try
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var dllPath = Path.Combine(baseDir, "BabelHook.dll");

            if (!File.Exists(dllPath))
            {
                // Fallback to project Release build path if running in dev
                var devPath = Path.GetFullPath(Path.Combine(baseDir, @"..\..\..\..\src\Babel.Hook\bin\Release\BabelHook.dll"));
                if (File.Exists(devPath)) dllPath = devPath;
            }

            if (File.Exists(dllPath))
            {
                if (InjectorService.IsInjected(pid, "BabelHook.dll"))
                {
                    return true;
                }
                return InjectorService.Inject(pid, dllPath);
            }
            return false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Injection failed: {ex.Message}");
            return false;
        }
    }

    public static string ResolveAuthor(string? playerName, int playerIndex)
    {
        return !string.IsNullOrWhiteSpace(playerName) ? playerName.Trim() : $"P{playerIndex}";
    }

    public static string ResolveColorHex(int playerIndex, ColorDto? specificColor = null)
    {
        var color = specificColor ?? ColorDto.FromPlayerIndex(playerIndex);
        return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
    }

    private void AddLogEntry(TranslationLogEntry entry)
    {
        void Insert()
        {
            History.Insert(0, entry);
            _persistentLogService.Enqueue(entry);

            // Limit in-memory history to 500 entries
            while (History.Count > 500)
            {
                var removed = History[History.Count - 1];
                History.RemoveAt(History.Count - 1);
                if (_matchedEntries != null && _matchedEntries.Remove(removed))
                {
                    _filteredLogCount--;
                }
            }

            if (_activeSearchTokens != null && _matchedEntries != null)
            {
                if (LogSearchFilter.Matches(entry, _activeSearchTokens))
                {
                    _matchedEntries.Add(entry);
                    _filteredLogCount++;
                }
            }
            else if (!HasLogSearchQuery)
            {
                _filteredLogCount = History.Count;
            }

            OnPropertyChanged(nameof(LogSearchFilterStatus));
        }

        if (Application.Current?.Dispatcher != null && !Application.Current.Dispatcher.CheckAccess())
        {
            Application.Current.Dispatcher.Invoke(Insert);
        }
        else
        {
            Insert();
        }
    }

    private void OnMessageTranslated(ChatMessage msg, TranslationResult res)
    {
        var now = DateTime.Now;
        var entry = new TranslationLogEntry
        {
            Timestamp = now,
            Date = FormatDate(now),
            Time = FormatTime(now),
            Author = ResolveAuthor(msg.PlayerName, msg.PlayerIndex),
            SourceLang = res.DetectedSourceLanguage,
            OriginalMessage = msg.MessageText,
            TranslatedMessage = res.Success ? res.TranslatedText : $"[Error: {res.ErrorMessage}]",
            Latency = $"{res.LatencyMs}ms",
            ColorHex = ResolveColorHex(msg.PlayerIndex, msg.PlayerColor)
        };

        AddLogEntry(entry);
    }

    private void OnTranslationRequestFailed(TranslationRequest req, string error)
    {
        var now = DateTime.Now;
        var entry = new TranslationLogEntry
        {
            Timestamp = now,
            Date = FormatDate(now),
            Time = FormatTime(now),
            Author = ResolveAuthor(req.PlayerName, req.PlayerIndex),
            SourceLang = "-",
            OriginalMessage = req.Text,
            TranslatedMessage = $"[Error: {error}]",
            Latency = "-",
            ColorHex = ResolveColorHex(req.PlayerIndex, req.PlayerColor)
        };

        AddLogEntry(entry);
    }

    private void OnWelcomeMessageSent(string message)
    {
        int pIdx = TranslationEngine?.LocalPlayerIndex ?? 1;
        string pName = TranslationEngine?.LocalPlayerName ?? string.Empty;

        var now = DateTime.Now;
        var entry = new TranslationLogEntry
        {
            Timestamp = now,
            Date = FormatDate(now),
            Time = FormatTime(now),
            Author = ResolveAuthor(pName, pIdx),
            SourceLang = "-",
            OriginalMessage = message,
            TranslatedMessage = "-",
            Latency = "-",
            ColorHex = ResolveColorHex(pIdx)
        };

        AddLogEntry(entry);
    }

    private void OnMacroExecuted(int slotId, string message)
    {
        int pIdx = TranslationEngine?.LocalPlayerIndex ?? 1;
        string pName = TranslationEngine?.LocalPlayerName ?? string.Empty;

        var now = DateTime.Now;
        var entry = new TranslationLogEntry
        {
            Timestamp = now,
            Date = FormatDate(now),
            Time = FormatTime(now),
            Author = ResolveAuthor(pName, pIdx),
            SourceLang = "-",
            OriginalMessage = message,
            TranslatedMessage = "-",
            Latency = "-",
            ColorHex = ResolveColorHex(pIdx)
        };

        AddLogEntry(entry);
    }

    public void TogglePause()
    {
        IsPaused = !IsPaused;

        if (IsPaused)
        {
            ToastNotificationService.Show("Helldivers 1 Babel", LocalizationService.Instance.Format("Toast_AssistantPaused", PauseHotkey), ToastType.Warning);
        }
        else
        {
            ToastNotificationService.Show("Helldivers 1 Babel", LocalizationService.Instance.GetString("Toast_AssistantResumed"), ToastType.Success);
        }
    }

    public void ToggleTranslation()
    {
        IsTranslationPaused = !IsTranslationPaused;

        if (IsTranslationPaused)
        {
            ToastNotificationService.Show(LocalizationService.Instance.GetString("Tab_TranslationAndLogs"), LocalizationService.Instance.Format("Toast_TranslationPaused", ToggleTranslationHotkey), ToastType.Warning);
        }
        else
        {
            ToastNotificationService.Show(LocalizationService.Instance.GetString("Tab_TranslationAndLogs"), LocalizationService.Instance.GetString("Toast_TranslationActive"), ToastType.Success);
        }
    }

    public void ToggleQuickChat()
    {
        IsQuickChatPaused = !IsQuickChatPaused;

        if (IsQuickChatPaused)
        {
            ToastNotificationService.Show(LocalizationService.Instance.GetString("Tab_QuickChat"), LocalizationService.Instance.Format("Toast_QuickChatPaused", ToggleQuickChatHotkey), ToastType.Warning);
        }
        else
        {
            ToastNotificationService.Show(LocalizationService.Instance.GetString("Tab_QuickChat"), LocalizationService.Instance.GetString("Toast_QuickChatActive"), ToastType.Success);
        }
    }

    public void UpdateEngineStates()
    {
        TranslationEngine?.SetPaused(IsPaused || IsTranslationPaused);
        MacroService?.SetPaused(IsPaused || IsQuickChatPaused);
    }

    public void ClearHistory()
    {
        _matchedEntries?.Clear();
        _filteredLogCount = 0;
        History.Clear();
        OnPropertyChanged(nameof(LogSearchFilterStatus));
    }

    public async Task ClearPersistentLogsAsync()
    {
        _matchedEntries?.Clear();
        _filteredLogCount = 0;
        History.Clear();
        OnPropertyChanged(nameof(LogSearchFilterStatus));
        await _persistentLogService.ClearAsync();
    }

    public async Task FlushPersistentLogsAsync()
    {
        await _persistentLogService.FlushAndCompleteAsync().ConfigureAwait(false);
    }

    public string GenerateLogsExportText()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("================================================================================");
        sb.AppendLine("  HELLDIVERS 1 BABEL - CHAT LOGS EXPORT");
        sb.AppendLine($"  Exported Date : {FormatDate(DateTime.Now)} {FormatTime(DateTime.Now)}");

        if (HasLogSearchQuery)
        {
            var tokens = LogSearchFilter.Tokenize(_logSearchQuery);
            var filteredList = History.Where(e => LogSearchFilter.Matches(e, tokens)).ToList();
            sb.AppendLine($"  Search Filter : \"{_logSearchQuery.Trim()}\"");
            sb.AppendLine($"  Total Messages: {filteredList.Count} (Filtered from {History.Count})");
            sb.AppendLine("================================================================================");
            sb.AppendLine();

            if (filteredList.Count == 0)
            {
                sb.AppendLine("No chat messages matched the active search filter.");
                return sb.ToString();
            }

            foreach (var entry in filteredList)
            {
                string langInfo = string.IsNullOrWhiteSpace(entry.SourceLang) ? "" : $" [{entry.SourceLang}]";
                string datePrefix = string.IsNullOrWhiteSpace(entry.Date) ? entry.Time : $"{entry.Date} {entry.Time}";
                sb.AppendLine($"[{datePrefix}] {entry.Author}{langInfo} (Latency: {entry.Latency})");
                sb.AppendLine($"  Original  : {entry.OriginalMessage}");
                sb.AppendLine($"  Translated: {entry.TranslatedMessage}");
                sb.AppendLine();
            }

            return sb.ToString();
        }
        else
        {
            sb.AppendLine($"  Total Messages: {History.Count}");
            sb.AppendLine("================================================================================");
            sb.AppendLine();

            if (History.Count == 0)
            {
                sb.AppendLine("No chat messages recorded in this session.");
                return sb.ToString();
            }

            foreach (var entry in History)
            {
                string langInfo = string.IsNullOrWhiteSpace(entry.SourceLang) ? "" : $" [{entry.SourceLang}]";
                string datePrefix = string.IsNullOrWhiteSpace(entry.Date) ? entry.Time : $"{entry.Date} {entry.Time}";
                sb.AppendLine($"[{datePrefix}] {entry.Author}{langInfo} (Latency: {entry.Latency})");
                sb.AppendLine($"  Original  : {entry.OriginalMessage}");
                sb.AppendLine($"  Translated: {entry.TranslatedMessage}");
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }

    public void UpdateMacroSeparators()
    {
        for (int i = 0; i < Macros.Count; i++)
        {
            Macros[i].ShowBottomBorder = (i < Macros.Count - 1);
        }
    }

    public void SyncMacrosToConfig()
    {
        for (int i = 0; i < Macros.Count && i < _config.QuickChatMacros.Count; i++)
        {
            _config.QuickChatMacros[i].Keybinding = Macros[i].Keybinding;
            _config.QuickChatMacros[i].MessageText = Macros[i].MessageText;
            _config.QuickChatMacros[i].IsEnabled = Macros[i].IsEnabled;
        }
    }

    public void SaveConfig()
    {
        SyncMacrosToConfig();
        _configService.Save(_config);
        MacroService.UpdateMacros(_config.QuickChatMacros);
    }

    public string ExportQuickChatMessagesJson()
    {
        SyncMacrosToConfig();
        return QuickChatJsonHelper.ExportMessages(Macros.Select(m => m.MessageText));
    }

    public int ImportQuickChatMessagesJson(string json)
    {
        var importedMessages = QuickChatJsonHelper.ParseMessages(json);
        if (importedMessages.Count == 0)
        {
            throw new InvalidOperationException("The JSON file does not contain any valid quick chat messages.");
        }

        int count = Math.Min(Macros.Count, importedMessages.Count);
        for (int i = 0; i < count; i++)
        {
            Macros[i].MessageText = importedMessages[i];
        }

        SaveConfig();
        return count;
    }

    public void ResetSettingsToDefaults(bool preserveApiKeys = true)
    {
        var defaults = new BabelConfig();

        // Translation & Logs Tab
        TargetLanguage = defaults.TargetLanguage; // "English"
        IncomingEnabled = defaults.IncomingTranslationEnabled;
        OutgoingEnabled = defaults.OutgoingTranslationEnabled;
        DontTranslateStandardOutgoing = defaults.DontTranslateStandardOutgoing;
        DontTranslateIncomingInTargetLanguage = defaults.DontTranslateIncomingInTargetLanguage;
        _config.OutgoingPrefix = defaults.OutgoingPrefix;
        _config.SoundAlertEnabled = defaults.SoundAlertEnabled;

        // Settings > General
        ChatHistoryLimitMb = defaults.ChatHistoryLimitMb;
        TimeoutSeconds = defaults.TimeoutSeconds;
        TranslationTone = defaults.TranslationTone;
        DateFormat = defaults.DateFormat;
        TimeFormat = defaults.TimeFormat;
        DebugLoggingEnabled = defaults.DebugLoggingEnabled;

        // Settings > Global Shortcuts
        PauseHotkey = defaults.GlobalPauseHotkey;
        ToggleTranslationHotkey = defaults.ToggleTranslationHotkey;
        ToggleQuickChatHotkey = defaults.ToggleQuickChatHotkey;
        QuickChatOverlayHotkey = defaults.QuickChatOverlayHotkey;

        // Settings > Display
        ShowToastOverlay = defaults.ShowToastOverlay;
        ToastPosition = defaults.ToastPosition;
        QuickChatOverlayPosition = defaults.QuickChatOverlayPosition;
        OverlayScalePercent = defaults.OverlayScalePercent;
        TargetDisplay = defaults.TargetDisplay;

        // Settings > AI Provider
        GeminiModel = defaults.ProviderModels.GetValueOrDefault("Gemini", "gemini-flash-lite-latest");

        if (!preserveApiKeys)
        {
            GeminiApiKey = "";
        }

        // Synchronize active background services
        BabelLog.IsEnabled = DebugLoggingEnabled;
        ToastNotificationService.IsEnabled = ShowToastOverlay;
        ToastNotificationService.ScaleFactor = _config.OverlayScaleFactor;
        ToastNotificationService.CurrentPosition = ToastPosition;
        ToastNotificationService.TargetDisplay = TargetDisplay;
        ToastNotificationService.UpdatePosition(ToastPosition, TargetDisplay);
        ToastNotificationService.UpdateScale(_config.OverlayScaleFactor);

        QuickChatOverlayService.ScaleFactor = _config.OverlayScaleFactor;
        QuickChatOverlayService.CurrentPosition = QuickChatOverlayPosition;
        QuickChatOverlayService.TargetDisplay = TargetDisplay;
        QuickChatOverlayService.UpdatePosition(QuickChatOverlayPosition, TargetDisplay);
        QuickChatOverlayService.UpdateScale(_config.OverlayScaleFactor);

        _persistentLogService.LimitMb = ChatHistoryLimitMb;

        SaveConfig();
    }

    public void ToggleQuickChatOverlay()
    {
        SyncMacrosToConfig();
        QuickChatOverlayService.Toggle(_config.QuickChatMacros, QuickChatOverlayHotkey);
    }

    public async Task TestApiConnectionAsync()
    {
        IsTestingApi = true;
        TestApiStatus = LocalizationService.Instance.GetString("Api_TestingConnection");

        try
        {
            var provider = ProviderFactory.GetProvider("Gemini");
            var req = new TranslationRequest
            {
                Text = "minas ao norte",
                TargetLanguage = "English",
                SourceLanguage = "auto",
                IsOutgoing = false
            };

            var res = await provider.TranslateAsync(req, _config);
            if (res.Success)
            {
                string resultText = res.TranslatedText.Trim();
                if (!resultText.StartsWith('('))
                {
                    string src = string.IsNullOrWhiteSpace(res.DetectedSourceLanguage) ? "Portuguese" : res.DetectedSourceLanguage;
                    resultText = $"({src}): {resultText}";
                }
                TestApiStatus = LocalizationService.Instance.Format("Api_ConnectedResult", res.LatencyMs, resultText);
            }
            else
            {
                TestApiStatus = LocalizationService.Instance.Format("Api_ConnectionFailed", res.ErrorMessage ?? string.Empty);
            }
        }
        catch (Exception ex)
        {
            TestApiStatus = LocalizationService.Instance.Format("Api_ConnectionFailed", ex.Message);
        }
        finally
        {
            IsTestingApi = false;
        }
    }

    public async Task TestSendMacroAsync(int slotId)
    {
        await MacroService.ExecuteMacroAsync(slotId);
    }

    public bool WelcomeMessageEnabled
    {
        get => _config.WelcomeMessageEnabled;
        set
        {
            if (_config.WelcomeMessageEnabled != value)
            {
                _config.WelcomeMessageEnabled = value;
                OnPropertyChanged();
            }
        }
    }

    public string WelcomeMessageText
    {
        get => _config.WelcomeMessage;
        set
        {
            var clamped = value != null && value.Length > QuickChatMacro.MaxMessageLength
                ? value[..QuickChatMacro.MaxMessageLength]
                : (value ?? string.Empty);

            if (_config.WelcomeMessage != clamped)
            {
                _config.WelcomeMessage = clamped;
                OnPropertyChanged();
                OnPropertyChanged(nameof(WelcomeMessageCharCount));
                OnPropertyChanged(nameof(WelcomeMessageCharCountDisplay));
                OnPropertyChanged(nameof(WelcomeMessageCharCountColor));
            }
        }
    }

    public int WelcomeMessageCharCount => _config.WelcomeMessage?.Length ?? 0;

    public string WelcomeMessageCharCountDisplay => $"{WelcomeMessageCharCount}/{QuickChatMacro.MaxMessageLength}";

    public string WelcomeMessageCharCountColor => WelcomeMessageCharCount switch
    {
        >= QuickChatMacro.MaxMessageLength => "#E06C75",
        >= 250 => "#FFE600",
        _ => "#737988"
    };

    public async Task TestSendWelcomeMessageAsync()
    {
        if (WelcomeMessageService != null)
        {
            await WelcomeMessageService.SendWelcomeMessageAsync();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
