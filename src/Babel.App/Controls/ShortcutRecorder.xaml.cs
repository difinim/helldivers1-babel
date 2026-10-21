using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Babel.App.Controls;

public partial class ShortcutRecorder : UserControl
{
    public static readonly DependencyProperty HotkeyProperty =
        DependencyProperty.Register(
            nameof(Hotkey),
            typeof(string),
            typeof(ShortcutRecorder),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnHotkeyChanged));

    public static readonly DependencyProperty IsRecordingProperty =
        DependencyProperty.Register(
            nameof(IsRecording),
            typeof(bool),
            typeof(ShortcutRecorder),
            new PropertyMetadata(false, OnIsRecordingChanged));

    public event EventHandler<string>? HotkeyChanged;

    public string Hotkey
    {
        get => (string)GetValue(HotkeyProperty);
        set => SetValue(HotkeyProperty, value);
    }

    public bool IsRecording
    {
        get => (bool)GetValue(IsRecordingProperty);
        private set => SetValue(IsRecordingProperty, value);
    }

    public ShortcutRecorder()
    {
        InitializeComponent();
        UpdateNormalText();
    }

    private static void OnHotkeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ShortcutRecorder control && !control.IsRecording)
        {
            control.UpdateNormalText();
        }
    }

    private static void OnIsRecordingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ShortcutRecorder control)
        {
            if ((bool)e.NewValue)
            {
                control.UpdateRecordingText();
            }
            else
            {
                control.UpdateNormalText();
            }
        }
    }

    private void UpdateNormalText()
    {
        if (string.IsNullOrWhiteSpace(Hotkey))
        {
            RecordButton.Content = "[ Press Key... ]";
        }
        else
        {
            var trimmed = Hotkey.Trim();
            if (trimmed.StartsWith('[') && trimmed.EndsWith(']'))
            {
                RecordButton.Content = trimmed;
            }
            else
            {
                RecordButton.Content = $"[ {trimmed} ]";
            }
        }
    }

    private void UpdateRecordingText()
    {
        var mods = GetActiveModifiers();
        if (mods.Count == 0)
        {
            RecordButton.Content = "[ Press Keys... ]";
        }
        else
        {
            RecordButton.Content = $"[ {string.Join("+", mods)} + ... ]";
        }
    }

    private void OnRecordButtonClicked(object sender, RoutedEventArgs e)
    {
        if (!IsRecording)
        {
            StartRecording();
        }
        else
        {
            CancelRecording();
        }
    }

    private Window? _hookedWindow;

    public void StartRecording()
    {
        IsRecording = true;
        _hookedWindow = Window.GetWindow(this);
        if (_hookedWindow != null)
        {
            _hookedWindow.PreviewKeyDown += OnWindowPreviewKeyDown;
            _hookedWindow.PreviewKeyUp += OnWindowPreviewKeyUp;
        }

        RecordButton.Focus();
        Keyboard.Focus(RecordButton);
        UpdateRecordingText();
    }

    public void CancelRecording()
    {
        DetachWindowHooks();
        IsRecording = false;
        UpdateNormalText();
    }

    private void StopRecording()
    {
        DetachWindowHooks();
        IsRecording = false;
        UpdateNormalText();
    }

    private void DetachWindowHooks()
    {
        if (_hookedWindow != null)
        {
            _hookedWindow.PreviewKeyDown -= OnWindowPreviewKeyDown;
            _hookedWindow.PreviewKeyUp -= OnWindowPreviewKeyUp;
            _hookedWindow = null;
        }
    }

    private void OnWindowPreviewKeyDown(object sender, KeyEventArgs e)
    {
        HandleKeyEvent(e);
    }

    private void OnWindowPreviewKeyUp(object sender, KeyEventArgs e)
    {
        if (!IsRecording) return;
        e.Handled = true;
        UpdateRecordingText();
    }

    private void OnRecordButtonPreviewKeyDown(object sender, KeyEventArgs e)
    {
        HandleKeyEvent(e);
    }

    private void OnRecordButtonPreviewKeyUp(object sender, KeyEventArgs e)
    {
        if (!IsRecording) return;
        e.Handled = true;
        UpdateRecordingText();
    }

    private void HandleKeyEvent(KeyEventArgs e)
    {
        if (!IsRecording) return;

        e.Handled = true;

        Key key = e.Key == Key.System ? e.SystemKey : e.Key;
        if (key == Key.ImeProcessed)
        {
            key = e.ImeProcessedKey;
        }

        // If user presses Escape, cancel recording without changing existing shortcut
        if (key == Key.Escape)
        {
            CancelRecording();
            return;
        }

        // If user presses a modifier key alone, keep listening and show "[ Ctrl + ... ]"
        if (IsModifierKey(key))
        {
            UpdateRecordingText();
            return;
        }

        // Non-modifier key was pressed! Formats canonical key string
        string canonicalKey = FormatKey(key);
        if (string.IsNullOrEmpty(canonicalKey))
        {
            return;
        }

        var mods = GetActiveModifiers();
        string canonicalShortcut = mods.Count > 0
            ? $"{string.Join("+", mods)}+{canonicalKey}"
            : canonicalKey;

        // Commits the shortcut to the target property
        Hotkey = canonicalShortcut;
        HotkeyChanged?.Invoke(this, canonicalShortcut);

        // Exits listening mode, restores normal button appearance
        StopRecording();
    }

    private static bool IsModifierKey(Key key)
    {
        return key switch
        {
            Key.LeftCtrl or Key.RightCtrl or
            Key.LeftShift or Key.RightShift or
            Key.LeftAlt or Key.RightAlt or
            Key.LWin or Key.RWin => true,
            _ => false
        };
    }

    private static List<string> GetActiveModifiers()
    {
        var list = new List<string>(4);

        bool ctrl = Keyboard.IsKeyDown(Key.LeftCtrl) ||
                    Keyboard.IsKeyDown(Key.RightCtrl) ||
                    (Keyboard.Modifiers & ModifierKeys.Control) != 0;

        bool shift = Keyboard.IsKeyDown(Key.LeftShift) ||
                     Keyboard.IsKeyDown(Key.RightShift) ||
                     (Keyboard.Modifiers & ModifierKeys.Shift) != 0;

        bool alt = Keyboard.IsKeyDown(Key.LeftAlt) ||
                   Keyboard.IsKeyDown(Key.RightAlt) ||
                   (Keyboard.Modifiers & ModifierKeys.Alt) != 0;

        bool win = Keyboard.IsKeyDown(Key.LWin) ||
                   Keyboard.IsKeyDown(Key.RWin) ||
                   (Keyboard.Modifiers & ModifierKeys.Windows) != 0;

        if (ctrl) list.Add("Ctrl");
        if (shift) list.Add("Shift");
        if (alt) list.Add("Alt");
        if (win) list.Add("Win");

        return list;
    }

    private static string FormatKey(Key key)
    {
        // 1. Direct NumPad keys (standard NumLock ON key codes)
        if (key >= Key.NumPad0 && key <= Key.NumPad9)
        {
            return $"NumPad{(int)key - (int)Key.NumPad0}";
        }

        // 2. NumPad keys when NumLock is OFF (Windows sends navigation virtual keys)
        if (!Keyboard.IsKeyToggled(Key.NumLock))
        {
            switch (key)
            {
                case Key.Insert: return "NumPad0";
                case Key.End: return "NumPad1";
                case Key.Down: return "NumPad2";
                case Key.PageDown: return "NumPad3";
                case Key.Left: return "NumPad4";
                case Key.Clear: return "NumPad5";
                case Key.Right: return "NumPad6";
                case Key.Home: return "NumPad7";
                case Key.Up: return "NumPad8";
                case Key.PageUp: return "NumPad9";
            }
        }

        return key switch
        {
            Key.Return or Key.Enter => "Enter",
            Key.Space => "Space",
            Key.Tab => "Tab",
            Key.Back => "Back",
            Key.Delete => "Delete",
            Key.Insert => "Insert",
            Key.Home => "Home",
            Key.End => "End",
            Key.PageUp => "PageUp",
            Key.PageDown => "PageDown",
            Key.Up => "Up",
            Key.Down => "Down",
            Key.Left => "Left",
            Key.Right => "Right",

            Key.Add => "Numpad +",
            Key.Subtract => "Numpad -",
            Key.Multiply => "Numpad *",
            Key.Divide => "Numpad /",
            Key.Decimal => "Numpad .",

            Key.D0 => "0",
            Key.D1 => "1",
            Key.D2 => "2",
            Key.D3 => "3",
            Key.D4 => "4",
            Key.D5 => "5",
            Key.D6 => "6",
            Key.D7 => "7",
            Key.D8 => "8",
            Key.D9 => "9",

            >= Key.A and <= Key.Z => key.ToString(),
            >= Key.F1 and <= Key.F24 => key.ToString(),

            _ => key.ToString()
        };
    }
}
