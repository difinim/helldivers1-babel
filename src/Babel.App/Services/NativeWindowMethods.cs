namespace Babel.App.Services;

using System;
using System.Runtime.InteropServices;

/// <summary>
/// Win32 P/Invoke declarations for window activation, foreground control, and cross-instance synchronization.
/// </summary>
internal static class NativeWindowMethods
{
    public const int ASFW_ANY = -1;
    public const int SW_HIDE = 0;
    public const int SW_SHOWNORMAL = 1;
    public const int SW_SHOW = 5;
    public const int SW_RESTORE = 9;

    public const int GWL_EXSTYLE = -20;
    public const int WS_EX_TOOLWINDOW = 0x00000080;
    public const int WS_EX_TOPMOST = 0x00000008;
    public const int WS_EX_NOACTIVATE = 0x08000000;

    [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
    public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
    public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool AllowSetForegroundWindow(int dwProcessId);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsIconic(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    public static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr lpdwProcessId);

    [DllImport("kernel32.dll")]
    public static extern uint GetCurrentThreadId();

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, [MarshalAs(UnmanagedType.Bool)] bool fAttach);

    /// <summary>
    /// Forces a window to the foreground, restoring it if minimized and attaching input threads if necessary.
    /// </summary>
    public static void BringWindowToForeground(IntPtr hWnd)
    {
        if (hWnd == IntPtr.Zero) return;

        if (IsIconic(hWnd))
        {
            ShowWindow(hWnd, SW_RESTORE);
        }
        else
        {
            ShowWindow(hWnd, SW_SHOW);
        }

        IntPtr foregroundHwnd = GetForegroundWindow();
        uint foregroundThreadId = GetWindowThreadProcessId(foregroundHwnd, IntPtr.Zero);
        uint currentThreadId = GetCurrentThreadId();

        if (foregroundThreadId != 0 && foregroundThreadId != currentThreadId)
        {
            AttachThreadInput(currentThreadId, foregroundThreadId, true);
            SetForegroundWindow(hWnd);
            AttachThreadInput(currentThreadId, foregroundThreadId, false);
        }
        else
        {
            SetForegroundWindow(hWnd);
        }
    }
}
