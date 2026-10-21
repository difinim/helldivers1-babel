using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Babel.App.Services;

public class TrayIconService : IDisposable
{
    private const int WmTrayNotify = 0x8001;
    private const int WmRButtonUp = 0x0205;
    private const int WmLButtonUp = 0x0202;
    private const int WmLButtonDblClk = 0x0203;

    private const uint NimAdd = 0x00000000;
    private const uint NimModify = 0x00000001;
    private const uint NimDelete = 0x00000002;

    private const uint NifMessage = 0x00000001;
    private const uint NifIcon = 0x00000002;
    private const uint NifTip = 0x00000004;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct NotifyIconData
    {
        public int cbSize;
        public IntPtr hWnd;
        public int uID;
        public uint uFlags;
        public int uCallbackMessage;
        public IntPtr hIcon;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szTip;
    }

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    private static extern bool Shell_NotifyIcon(uint dwMessage, ref NotifyIconData lpData);

    [DllImport("user32.dll")]
    private static extern IntPtr LoadIcon(IntPtr hInstance, IntPtr lpIconName);

    private readonly Window _window;
    private IntPtr _hwnd;
    private HwndSource? _hwndSource;
    private bool _isAdded;
    private NotifyIconData _nid;
    private System.Drawing.Icon? _appIcon;

    public event Action? OpenRequested;
    public event Action? TogglePauseRequested;
    public event Action? ExitRequested;

    public TrayIconService(Window window)
    {
        _window = window;
    }

    public void Initialize()
    {
        _hwnd = new WindowInteropHelper(_window).EnsureHandle();
        _hwndSource = HwndSource.FromHwnd(_hwnd);
        _hwndSource?.AddHook(WndProc);

        try
        {
            var exePath = Environment.ProcessPath;
            if (!string.IsNullOrEmpty(exePath) && File.Exists(exePath))
            {
                _appIcon = System.Drawing.Icon.ExtractAssociatedIcon(exePath!);
            }
        }
        catch
        {
            // Fallback to IDI_APPLICATION
        }

        IntPtr hIcon = _appIcon?.Handle ?? LoadIcon(IntPtr.Zero, (IntPtr)32512);

        _nid = new NotifyIconData
        {
            cbSize = Marshal.SizeOf<NotifyIconData>(),
            hWnd = _hwnd,
            uID = 1001,
            uFlags = NifMessage | NifIcon | NifTip,
            uCallbackMessage = WmTrayNotify,
            hIcon = hIcon,
            szTip = "Helldivers 1 Babel (Active)"
        };

        _isAdded = Shell_NotifyIcon(NimAdd, ref _nid);
    }

    public void UpdateStatus(string tipText)
    {
        if (!_isAdded) return;
        _nid.szTip = tipText.Length > 120 ? tipText[..120] : tipText;
        Shell_NotifyIcon(NimModify, ref _nid);
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == WmTrayNotify)
        {
            int eventId = (int)(lParam.ToInt64() & 0xFFFF);
            if (eventId is WmLButtonUp or WmLButtonDblClk)
            {
                OpenRequested?.Invoke();
                handled = true;
            }
            else if (eventId == WmRButtonUp)
            {
                ShowContextMenu();
                handled = true;
            }
        }

        return IntPtr.Zero;
    }

    private void ShowContextMenu()
    {
        NativeWindowMethods.SetForegroundWindow(_hwnd);

        var menu = new System.Windows.Controls.ContextMenu();

        var openItem = new System.Windows.Controls.MenuItem { Header = "Open Helldivers 1 Babel" };
        openItem.Click += (_, _) =>
        {
            menu.IsOpen = false;
            _window.Dispatcher.BeginInvoke(new Action(() => OpenRequested?.Invoke()));
        };
        menu.Items.Add(openItem);

        var pauseItem = new System.Windows.Controls.MenuItem { Header = "Toggle Pause (Ctrl+F10)" };
        pauseItem.Click += (_, _) =>
        {
            menu.IsOpen = false;
            _window.Dispatcher.BeginInvoke(new Action(() => TogglePauseRequested?.Invoke()));
        };
        menu.Items.Add(pauseItem);

        menu.Items.Add(new System.Windows.Controls.Separator());

        var exitItem = new System.Windows.Controls.MenuItem { Header = "Exit" };
        exitItem.Click += (_, _) =>
        {
            menu.IsOpen = false;
            _window.Dispatcher.BeginInvoke(new Action(() => ExitRequested?.Invoke()));
        };
        menu.Items.Add(exitItem);

        menu.IsOpen = true;
    }

    public void Dispose()
    {
        if (_isAdded)
        {
            Shell_NotifyIcon(NimDelete, ref _nid);
            _isAdded = false;
        }

        if (_hwndSource != null)
        {
            _hwndSource.RemoveHook(WndProc);
            _hwndSource = null;
        }

        _appIcon?.Dispose();
        _appIcon = null;
    }
}
