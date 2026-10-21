using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;

namespace Babel.App.Services;

public class DisplayMonitorInfo
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public Rect WorkArea { get; set; }
    public Rect Bounds { get; set; }
    public bool IsPrimary { get; set; }

    public override string ToString() => DisplayName;
}

public static class DisplayMonitorService
{
    private const uint MONITOR_DEFAULTTOPRIMARY = 0x00000001;
    private const uint MONITORINFOF_PRIMARY = 0x00000001;

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct MONITORINFOEX
    {
        public int cbSize;
        public RECT rcMonitor;
        public RECT rcWork;
        public uint dwFlags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string szDevice;
    }

    private delegate bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumProc lpfnEnum, IntPtr dwData);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFOEX lpmi);

    [DllImport("user32.dll")]
    private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool IsWindow(IntPtr hWnd);

    public static List<DisplayMonitorInfo> GetMonitors()
    {
        var monitors = new List<DisplayMonitorInfo>();
        double dpiScaleX = 1.0;
        double dpiScaleY = 1.0;

        try
        {
            var mainWin = Application.Current?.MainWindow;
            if (mainWin != null)
            {
                var source = PresentationSource.FromVisual(mainWin);
                if (source?.CompositionTarget != null)
                {
                    dpiScaleX = source.CompositionTarget.TransformToDevice.M11;
                    dpiScaleY = source.CompositionTarget.TransformToDevice.M22;
                }
            }
        }
        catch { }

        if (dpiScaleX <= 0) dpiScaleX = 1.0;
        if (dpiScaleY <= 0) dpiScaleY = 1.0;

        int index = 1;
        EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, (IntPtr hMon, IntPtr hdc, ref RECT rect, IntPtr data) =>
        {
            var mi = new MONITORINFOEX();
            mi.cbSize = Marshal.SizeOf<MONITORINFOEX>();

            if (GetMonitorInfo(hMon, ref mi))
            {
                bool isPrimary = (mi.dwFlags & MONITORINFOF_PRIMARY) != 0;
                
                Rect workArea;
                if (isPrimary)
                {
                    // Primary work area is directly available via WPF SystemParameters
                    workArea = SystemParameters.WorkArea;
                }
                else
                {
                    workArea = new Rect(
                        mi.rcWork.Left / dpiScaleX,
                        mi.rcWork.Top / dpiScaleY,
                        (mi.rcWork.Right - mi.rcWork.Left) / dpiScaleX,
                        (mi.rcWork.Bottom - mi.rcWork.Top) / dpiScaleY);
                }

                int pixelWidth = mi.rcMonitor.Right - mi.rcMonitor.Left;
                int pixelHeight = mi.rcMonitor.Bottom - mi.rcMonitor.Top;
                string device = mi.szDevice ?? $"Display{index}";
                string primaryBadge = isPrimary ? " - Primary" : "";

                monitors.Add(new DisplayMonitorInfo
                {
                    Id = device,
                    DisplayName = $"Display {index} ({pixelWidth}x{pixelHeight}{primaryBadge})",
                    WorkArea = workArea,
                    Bounds = new Rect(
                        mi.rcMonitor.Left / dpiScaleX,
                        mi.rcMonitor.Top / dpiScaleY,
                        pixelWidth / dpiScaleX,
                        pixelHeight / dpiScaleY),
                    IsPrimary = isPrimary
                });

                index++;
            }
            return true;
        }, IntPtr.Zero);

        if (monitors.Count == 0)
        {
            monitors.Add(new DisplayMonitorInfo
            {
                Id = "Primary",
                DisplayName = "Primary Display",
                WorkArea = SystemParameters.WorkArea,
                IsPrimary = true
            });
        }

        return monitors;
    }

    public static Rect GetTargetWorkArea(string? targetDisplay, IntPtr? gameHwnd = null)
    {
        var monitors = GetMonitors();
        var primaryWorkArea = SystemParameters.WorkArea;

        if (string.IsNullOrWhiteSpace(targetDisplay) || 
            string.Equals(targetDisplay, "Auto", StringComparison.OrdinalIgnoreCase) || 
            targetDisplay.Contains("Auto", StringComparison.OrdinalIgnoreCase))
        {
            if (gameHwnd.HasValue && gameHwnd.Value != IntPtr.Zero && IsWindow(gameHwnd.Value))
            {
                var hMon = MonitorFromWindow(gameHwnd.Value, MONITOR_DEFAULTTOPRIMARY);
                if (hMon != IntPtr.Zero)
                {
                    var mi = new MONITORINFOEX();
                    mi.cbSize = Marshal.SizeOf<MONITORINFOEX>();
                    if (GetMonitorInfo(hMon, ref mi))
                    {
                        var match = monitors.Find(m => string.Equals(m.Id, mi.szDevice, StringComparison.OrdinalIgnoreCase));
                        if (match != null)
                        {
                            return match.WorkArea;
                        }
                    }
                }
            }

            var primary = monitors.Find(m => m.IsPrimary);
            return primary?.WorkArea ?? primaryWorkArea;
        }

        if (string.Equals(targetDisplay, "Primary", StringComparison.OrdinalIgnoreCase) ||
            targetDisplay.StartsWith("Primary", StringComparison.OrdinalIgnoreCase))
        {
            var primary = monitors.Find(m => m.IsPrimary);
            return primary?.WorkArea ?? primaryWorkArea;
        }

        // Match by device ID or DisplayName prefix
        var matched = monitors.Find(m =>
            string.Equals(m.Id, targetDisplay, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(m.DisplayName, targetDisplay, StringComparison.OrdinalIgnoreCase) ||
            m.DisplayName.StartsWith(targetDisplay, StringComparison.OrdinalIgnoreCase));

        return matched?.WorkArea ?? primaryWorkArea;
    }
}
