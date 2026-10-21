using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Interop;
using Babel.Core.Models;
using Babel.Core.Services;

namespace Babel.App.Services;

public class GlobalHotkeyService : IDisposable
{
    private const int WmHotkey = 0x0312;
    private const int PauseHotkeyId = 9001;
    private const int QuickChatOverlayHotkeyId = 9002;
    private const int ToggleTranslationHotkeyId = 9003;
    private const int ToggleQuickChatHotkeyId = 9004;
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_SYSKEYDOWN = 0x0104;

    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll")]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string? lpModuleName);

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    [DllImport("user32.dll")]
    private static extern short GetKeyState(int nVirtKey);

    [StructLayout(LayoutKind.Sequential)]
    private struct KBDLLHOOKSTRUCT
    {
        public uint vkCode;
        public uint scanCode;
        public uint flags;
        public uint time;
        public UIntPtr dwExtraInfo;
    }

    [Flags]
    public enum ModifierKeys : uint
    {
        None = 0x0000,
        Alt = 0x0001,
        Control = 0x0002,
        Shift = 0x0004,
        Win = 0x0008,
        NoRepeat = 0x4000
    }

    private record MacroBinding(int SlotId, uint Modifiers, uint VkCode, string Keybinding);

    private HwndSource? _hwndSource;
    private IntPtr _windowHandle;
    private bool _isPauseRegistered;
    private string _currentPauseHotkey = "Ctrl+Shift+P";

    private readonly List<MacroBinding> _macroBindings = new();
    private readonly object _macroLock = new();
    private LowLevelKeyboardProc? _llHookDelegate;
    private IntPtr _llHookHandle = IntPtr.Zero;

    public event Action? PauseHotkeyPressed;
    public event Action? HotkeyPressed; // Backwards-compatible alias for PauseHotkeyPressed
    public event Action? ToggleTranslationHotkeyPressed;
    public event Action? ToggleQuickChatHotkeyPressed;
    public event Action? QuickChatOverlayHotkeyPressed;
    public event Action<int>? MacroHotkeyPressed;

    private bool _isToggleTranslationRegistered;
    private string _currentToggleTranslationHotkey = "Ctrl+F11";
    private uint _toggleTranslationModifiers;
    private uint _toggleTranslationVk;

    private bool _isToggleQuickChatRegistered;
    private string _currentToggleQuickChatHotkey = "Ctrl+F12";
    private uint _toggleQuickChatModifiers;
    private uint _toggleQuickChatVk;

    private bool _isQuickChatOverlayRegistered;
    private string _currentQuickChatOverlayHotkey = "Numpad +";
    private uint _quickChatOverlayModifiers;
    private uint _quickChatOverlayVk;

    public bool IsRegistered => _isPauseRegistered;
    public string CurrentHotkey => _currentPauseHotkey;
    public bool IsToggleTranslationRegistered => _isToggleTranslationRegistered;
    public string CurrentToggleTranslationHotkey => _currentToggleTranslationHotkey;
    public bool IsToggleQuickChatRegistered => _isToggleQuickChatRegistered;
    public string CurrentToggleQuickChatHotkey => _currentToggleQuickChatHotkey;
    public bool IsQuickChatOverlayRegistered => _isQuickChatOverlayRegistered;
    public string CurrentQuickChatOverlayHotkey => _currentQuickChatOverlayHotkey;

    public void Initialize(IntPtr windowHandle)
    {
        _windowHandle = windowHandle;
        _hwndSource = HwndSource.FromHwnd(_windowHandle);
        _hwndSource?.AddHook(WndProc);
    }

    public bool Register(string hotkeyString) => RegisterPauseHotkey(hotkeyString);

    public bool RegisterPauseHotkey(string hotkeyString)
    {
        UnregisterPauseHotkey();

        if (string.IsNullOrWhiteSpace(hotkeyString) || _windowHandle == IntPtr.Zero)
        {
            return false;
        }

        if (!ParseHotkey(hotkeyString, out var modifiers, out var vk))
        {
            BabelLog.Warn("GlobalHotkeyService", $"Failed to parse pause hotkey '{hotkeyString}'");
            return false;
        }

        modifiers |= (uint)ModifierKeys.NoRepeat;
        _isPauseRegistered = RegisterHotKey(_windowHandle, PauseHotkeyId, modifiers, vk);
        if (_isPauseRegistered)
        {
            _currentPauseHotkey = hotkeyString;
            BabelLog.Info("GlobalHotkeyService", $"Pause hotkey registered: '{hotkeyString}' (VK: 0x{vk:X2})");
        }
        else
        {
            int err = Marshal.GetLastWin32Error();
            BabelLog.Warn("GlobalHotkeyService", $"Failed to register pause hotkey '{hotkeyString}' (Win32Error: {err})");
        }

        return _isPauseRegistered;
    }

    public void Unregister() => UnregisterPauseHotkey();

    public void UnregisterPauseHotkey()
    {
        if (_isPauseRegistered && _windowHandle != IntPtr.Zero)
        {
            UnregisterHotKey(_windowHandle, PauseHotkeyId);
            _isPauseRegistered = false;
        }
    }

    public bool RegisterQuickChatOverlayHotkey(string hotkeyString)
    {
        UnregisterQuickChatOverlayHotkey();

        if (string.IsNullOrWhiteSpace(hotkeyString) || _windowHandle == IntPtr.Zero)
        {
            return false;
        }

        if (!ParseHotkey(hotkeyString, out var modifiers, out var vk))
        {
            BabelLog.Warn("GlobalHotkeyService", $"Failed to parse quick chat overlay hotkey '{hotkeyString}'");
            return false;
        }

        _quickChatOverlayModifiers = modifiers;
        _quickChatOverlayVk = vk;

        modifiers |= (uint)ModifierKeys.NoRepeat;
        _isQuickChatOverlayRegistered = RegisterHotKey(_windowHandle, QuickChatOverlayHotkeyId, modifiers, vk);
        if (_isQuickChatOverlayRegistered)
        {
            _currentQuickChatOverlayHotkey = hotkeyString;
            BabelLog.Info("GlobalHotkeyService", $"Quick chat overlay hotkey registered: '{hotkeyString}' (VK: 0x{vk:X2})");
        }
        else
        {
            int err = Marshal.GetLastWin32Error();
            BabelLog.Warn("GlobalHotkeyService", $"Failed to register quick chat overlay hotkey '{hotkeyString}' via RegisterHotKey (Win32Error: {err}). Falling back to low-level keyboard hook.");
            EnsureLowLevelHookInstalled();
        }

        return true;
    }

    public void UnregisterQuickChatOverlayHotkey()
    {
        if (_isQuickChatOverlayRegistered && _windowHandle != IntPtr.Zero)
        {
            UnregisterHotKey(_windowHandle, QuickChatOverlayHotkeyId);
            _isQuickChatOverlayRegistered = false;
        }
        _quickChatOverlayModifiers = 0;
        _quickChatOverlayVk = 0;
    }

    public bool RegisterToggleTranslationHotkey(string hotkeyString)
    {
        UnregisterToggleTranslationHotkey();

        if (string.IsNullOrWhiteSpace(hotkeyString) || _windowHandle == IntPtr.Zero)
        {
            return false;
        }

        if (!ParseHotkey(hotkeyString, out var modifiers, out var vk))
        {
            BabelLog.Warn("GlobalHotkeyService", $"Failed to parse toggle translation hotkey '{hotkeyString}'");
            return false;
        }

        _toggleTranslationModifiers = modifiers;
        _toggleTranslationVk = vk;

        modifiers |= (uint)ModifierKeys.NoRepeat;
        _isToggleTranslationRegistered = RegisterHotKey(_windowHandle, ToggleTranslationHotkeyId, modifiers, vk);
        if (_isToggleTranslationRegistered)
        {
            _currentToggleTranslationHotkey = hotkeyString;
            BabelLog.Info("GlobalHotkeyService", $"Toggle translation hotkey registered: '{hotkeyString}' (VK: 0x{vk:X2})");
        }
        else
        {
            int err = Marshal.GetLastWin32Error();
            BabelLog.Warn("GlobalHotkeyService", $"Failed to register toggle translation hotkey '{hotkeyString}' via RegisterHotKey (Win32Error: {err}). Falling back to low-level keyboard hook.");
            EnsureLowLevelHookInstalled();
        }

        return true;
    }

    public void UnregisterToggleTranslationHotkey()
    {
        if (_isToggleTranslationRegistered && _windowHandle != IntPtr.Zero)
        {
            UnregisterHotKey(_windowHandle, ToggleTranslationHotkeyId);
            _isToggleTranslationRegistered = false;
        }
        _toggleTranslationModifiers = 0;
        _toggleTranslationVk = 0;
    }

    public bool RegisterToggleQuickChatHotkey(string hotkeyString)
    {
        UnregisterToggleQuickChatHotkey();

        if (string.IsNullOrWhiteSpace(hotkeyString) || _windowHandle == IntPtr.Zero)
        {
            return false;
        }

        if (!ParseHotkey(hotkeyString, out var modifiers, out var vk))
        {
            BabelLog.Warn("GlobalHotkeyService", $"Failed to parse toggle quick chat hotkey '{hotkeyString}'");
            return false;
        }

        _toggleQuickChatModifiers = modifiers;
        _toggleQuickChatVk = vk;

        modifiers |= (uint)ModifierKeys.NoRepeat;
        _isToggleQuickChatRegistered = RegisterHotKey(_windowHandle, ToggleQuickChatHotkeyId, modifiers, vk);
        if (_isToggleQuickChatRegistered)
        {
            _currentToggleQuickChatHotkey = hotkeyString;
            BabelLog.Info("GlobalHotkeyService", $"Toggle quick chat hotkey registered: '{hotkeyString}' (VK: 0x{vk:X2})");
        }
        else
        {
            int err = Marshal.GetLastWin32Error();
            BabelLog.Warn("GlobalHotkeyService", $"Failed to register toggle quick chat hotkey '{hotkeyString}' via RegisterHotKey (Win32Error: {err}). Falling back to low-level keyboard hook.");
            EnsureLowLevelHookInstalled();
        }

        return true;
    }

    public void UnregisterToggleQuickChatHotkey()
    {
        if (_isToggleQuickChatRegistered && _windowHandle != IntPtr.Zero)
        {
            UnregisterHotKey(_windowHandle, ToggleQuickChatHotkeyId);
            _isToggleQuickChatRegistered = false;
        }
        _toggleQuickChatModifiers = 0;
        _toggleQuickChatVk = 0;
    }

    public void RegisterMacroHotkeys(IEnumerable<QuickChatMacro> macros)
    {
        lock (_macroLock)
        {
            _macroBindings.Clear();

            if (macros != null)
            {
                foreach (var m in macros)
                {
                    if (!m.IsEnabled || string.IsNullOrWhiteSpace(m.Keybinding)) continue;

                    if (ParseHotkey(m.Keybinding, out var modifiers, out var vk))
                    {
                        _macroBindings.Add(new MacroBinding(m.SlotId, modifiers, vk, m.Keybinding));
                        BabelLog.Info("GlobalHotkeyService", $"Registered pass-through Macro Slot {m.SlotId}: '{m.Keybinding}' (VK: 0x{vk:X2})");
                    }
                }
            }
        }

        EnsureLowLevelHookInstalled();
    }

    public void UnregisterMacroHotkeys()
    {
        lock (_macroLock)
        {
            _macroBindings.Clear();
        }
        UninstallLowLevelHook();
    }

    private void EnsureLowLevelHookInstalled()
    {
        if (_llHookHandle != IntPtr.Zero) return;

        _llHookDelegate = LowLevelKeyboardCallback;
        using var curProcess = System.Diagnostics.Process.GetCurrentProcess();
        using var curModule = curProcess.MainModule;
        IntPtr hMod = GetModuleHandle(curModule?.ModuleName);
        _llHookHandle = SetWindowsHookEx(WH_KEYBOARD_LL, _llHookDelegate, hMod, 0);

        if (_llHookHandle != IntPtr.Zero)
        {
            BabelLog.Info("GlobalHotkeyService", "Low-level keyboard hook (WH_KEYBOARD_LL) installed with transparent pass-through.");
        }
        else
        {
            int err = Marshal.GetLastWin32Error();
            BabelLog.Warn("GlobalHotkeyService", $"Failed to install WH_KEYBOARD_LL hook (Win32Error: {err})");
        }
    }

    private void UninstallLowLevelHook()
    {
        if (_llHookHandle != IntPtr.Zero)
        {
            UnhookWindowsHookEx(_llHookHandle);
            _llHookHandle = IntPtr.Zero;
            _llHookDelegate = null;
            BabelLog.Info("GlobalHotkeyService", "Low-level keyboard hook uninstalled.");
        }
    }

    private IntPtr LowLevelKeyboardCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
        {
            var kb = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);
            uint vk = kb.vkCode;
            bool isExtended = (kb.flags & 1) != 0;

            // When NumLock is OFF, map non-extended keypad navigation keys to their NumPad VK codes
            if (!isExtended)
            {
                vk = vk switch
                {
                    0x2D => 0x60, // VK_INSERT -> VK_NUMPAD0
                    0x23 => 0x61, // VK_END    -> VK_NUMPAD1
                    0x28 => 0x62, // VK_DOWN   -> VK_NUMPAD2
                    0x22 => 0x63, // VK_NEXT   -> VK_NUMPAD3
                    0x25 => 0x64, // VK_LEFT   -> VK_NUMPAD4
                    0x0C => 0x65, // VK_CLEAR  -> VK_NUMPAD5
                    0x27 => 0x66, // VK_RIGHT  -> VK_NUMPAD6
                    0x24 => 0x67, // VK_HOME   -> VK_NUMPAD7
                    0x26 => 0x68, // VK_UP     -> VK_NUMPAD8
                    0x21 => 0x69, // VK_PRIOR  -> VK_NUMPAD9
                    0x2E => 0x6E, // VK_DELETE -> VK_DECIMAL
                    _ => vk
                };
            }

            if (IsHelldiversOrAppForeground())
            {
                uint activeModifiers = GetCurrentModifiers();

                if (!_isQuickChatOverlayRegistered && _quickChatOverlayVk != 0 && _quickChatOverlayVk == vk && (_quickChatOverlayModifiers & ~(uint)ModifierKeys.NoRepeat) == activeModifiers)
                {
                    BabelLog.Info("GlobalHotkeyService", "Quick chat overlay hotkey triggered via low-level hook.");
                    Task.Run(() => QuickChatOverlayHotkeyPressed?.Invoke());
                }

                if (!_isToggleTranslationRegistered && _toggleTranslationVk != 0 && _toggleTranslationVk == vk && (_toggleTranslationModifiers & ~(uint)ModifierKeys.NoRepeat) == activeModifiers)
                {
                    BabelLog.Info("GlobalHotkeyService", "Toggle translation hotkey triggered via low-level hook.");
                    Task.Run(() => ToggleTranslationHotkeyPressed?.Invoke());
                }

                if (!_isToggleQuickChatRegistered && _toggleQuickChatVk != 0 && _toggleQuickChatVk == vk && (_toggleQuickChatModifiers & ~(uint)ModifierKeys.NoRepeat) == activeModifiers)
                {
                    BabelLog.Info("GlobalHotkeyService", "Toggle quick chat hotkey triggered via low-level hook.");
                    Task.Run(() => ToggleQuickChatHotkeyPressed?.Invoke());
                }

                int matchedSlot = -1;

                lock (_macroLock)
                {
                    foreach (var binding in _macroBindings)
                    {
                        uint requiredMods = binding.Modifiers & ~(uint)ModifierKeys.NoRepeat;
                        if (binding.VkCode == vk && requiredMods == activeModifiers)
                        {
                            matchedSlot = binding.SlotId;
                            break;
                        }
                    }
                }

                if (matchedSlot > 0)
                {
                    BabelLog.Info("GlobalHotkeyService", $"Pass-through Macro Slot {matchedSlot} triggered (VK: 0x{vk:X2}). Propagating key to game.");
                    Task.Run(() => MacroHotkeyPressed?.Invoke(matchedSlot));
                }
            }
        }

        // ALWAYS pass through the keystroke so Helldivers and other software receive it unmodified!
        return CallNextHookEx(_llHookHandle, nCode, wParam, lParam);
    }

    private bool IsHelldiversOrAppForeground()
    {
        IntPtr fgHwnd = GetForegroundWindow();
        if (fgHwnd == IntPtr.Zero) return false;
        if (_windowHandle != IntPtr.Zero && fgHwnd == _windowHandle) return true;

        GetWindowThreadProcessId(fgHwnd, out uint pid);
        if (pid == 0) return false;

        try
        {
            using var proc = System.Diagnostics.Process.GetProcessById((int)pid);
            return proc.ProcessName.Contains("helldivers", StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    private static uint GetCurrentModifiers()
    {
        uint mods = 0;
        if ((GetKeyState(0x11) & 0x8000) != 0) mods |= (uint)ModifierKeys.Control; // VK_CONTROL
        if ((GetKeyState(0x12) & 0x8000) != 0) mods |= (uint)ModifierKeys.Alt;     // VK_MENU
        if ((GetKeyState(0x10) & 0x8000) != 0) mods |= (uint)ModifierKeys.Shift;   // VK_SHIFT
        if ((GetKeyState(0x5B) & 0x8000) != 0 || (GetKeyState(0x5C) & 0x8000) != 0) mods |= (uint)ModifierKeys.Win; // VK_LWIN / VK_RWIN
        return mods;
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == WmHotkey)
        {
            int id = wParam.ToInt32();
            if (id == PauseHotkeyId)
            {
                BabelLog.Info("GlobalHotkeyService", "Pause hotkey triggered via WM_HOTKEY");
                PauseHotkeyPressed?.Invoke();
                HotkeyPressed?.Invoke();
                handled = true;
            }
            else if (id == QuickChatOverlayHotkeyId)
            {
                BabelLog.Info("GlobalHotkeyService", "Quick chat overlay hotkey triggered via WM_HOTKEY");
                QuickChatOverlayHotkeyPressed?.Invoke();
                handled = true;
            }
            else if (id == ToggleTranslationHotkeyId)
            {
                BabelLog.Info("GlobalHotkeyService", "Toggle translation hotkey triggered via WM_HOTKEY");
                ToggleTranslationHotkeyPressed?.Invoke();
                handled = true;
            }
            else if (id == ToggleQuickChatHotkeyId)
            {
                BabelLog.Info("GlobalHotkeyService", "Toggle quick chat hotkey triggered via WM_HOTKEY");
                ToggleQuickChatHotkeyPressed?.Invoke();
                handled = true;
            }
        }

        return IntPtr.Zero;
    }

    public static bool ParseHotkey(string hotkeyString, out uint modifiers, out uint vk)
    {
        modifiers = (uint)ModifierKeys.None;
        vk = 0;

        if (string.IsNullOrWhiteSpace(hotkeyString)) return false;

        string remaining = hotkeyString.Trim();

        // Extract modifiers in a loop from the beginning of the string
        bool foundModifier = true;
        while (foundModifier && remaining.Length > 0)
        {
            foundModifier = false;
            if (remaining.StartsWith("Ctrl+", StringComparison.OrdinalIgnoreCase) ||
                remaining.StartsWith("Control+", StringComparison.OrdinalIgnoreCase))
            {
                modifiers |= (uint)ModifierKeys.Control;
                int idx = remaining.IndexOf('+');
                remaining = remaining[(idx + 1)..].TrimStart();
                foundModifier = true;
            }
            else if (remaining.StartsWith("Alt+", StringComparison.OrdinalIgnoreCase))
            {
                modifiers |= (uint)ModifierKeys.Alt;
                remaining = remaining[4..].TrimStart();
                foundModifier = true;
            }
            else if (remaining.StartsWith("Shift+", StringComparison.OrdinalIgnoreCase))
            {
                modifiers |= (uint)ModifierKeys.Shift;
                remaining = remaining[6..].TrimStart();
                foundModifier = true;
            }
            else if (remaining.StartsWith("Win+", StringComparison.OrdinalIgnoreCase) ||
                     remaining.StartsWith("Windows+", StringComparison.OrdinalIgnoreCase))
            {
                modifiers |= (uint)ModifierKeys.Win;
                int idx = remaining.IndexOf('+');
                remaining = remaining[(idx + 1)..].TrimStart();
                foundModifier = true;
            }
        }

        if (string.IsNullOrWhiteSpace(remaining)) return false;

        string keyPart = remaining.Trim();
        string kpUpper = keyPart.ToUpperInvariant().Replace(" ", "");

        // 1. Numpad arithmetic and special aliases
        switch (kpUpper)
        {
            case "NUMPAD+": case "NUM+": case "NUMPADPLUS": case "NUMPADADD": case "ADD": case "PLUS":
                vk = 0x6B; return true; // VK_ADD
            case "NUMPAD-": case "NUM-": case "NUMPADMINUS": case "NUMPADSUBTRACT": case "SUBTRACT": case "MINUS":
                vk = 0x6D; return true; // VK_SUBTRACT
            case "NUMPAD*": case "NUM*": case "NUMPADMULTIPLY": case "MULTIPLY":
                vk = 0x6A; return true; // VK_MULTIPLY
            case "NUMPAD/": case "NUM/": case "NUMPADDIVIDE": case "DIVIDE":
                vk = 0x6F; return true; // VK_DIVIDE
            case "NUMPAD.": case "NUM.": case "NUMPADDECIMAL": case "DECIMAL":
                vk = 0x6E; return true; // VK_DECIMAL

            case "NUM0": case "NUMPAD0": vk = 0x60; return true;
            case "NUM1": case "NUMPAD1": vk = 0x61; return true;
            case "NUM2": case "NUMPAD2": vk = 0x62; return true;
            case "NUM3": case "NUMPAD3": vk = 0x63; return true;
            case "NUM4": case "NUMPAD4": vk = 0x64; return true;
            case "NUM5": case "NUMPAD5": vk = 0x65; return true;
            case "NUM6": case "NUMPAD6": vk = 0x66; return true;
            case "NUM7": case "NUMPAD7": vk = 0x67; return true;
            case "NUM8": case "NUMPAD8": vk = 0x68; return true;
            case "NUM9": case "NUMPAD9": vk = 0x69; return true;

            case "F9": vk = 0x78; return true;
            case "F10": vk = 0x79; return true;
            case "F11": vk = 0x7A; return true;
            case "F12": vk = 0x7B; return true;
        }

        // Single digit '0'..'9'
        if (keyPart.Length == 1 && keyPart[0] >= '0' && keyPart[0] <= '9')
        {
            vk = (uint)keyPart[0];
            return true;
        }

        // Try standard WPF Key enum
        if (Enum.TryParse<Key>(keyPart, true, out var key))
        {
            vk = (uint)KeyInterop.VirtualKeyFromKey(key);
            if (vk != 0) return true;
        }

        return false;
    }

    public void Dispose()
    {
        UnregisterPauseHotkey();
        UnregisterToggleTranslationHotkey();
        UnregisterToggleQuickChatHotkey();
        UnregisterQuickChatOverlayHotkey();
        UnregisterMacroHotkeys();
        if (_hwndSource != null)
        {
            _hwndSource.RemoveHook(WndProc);
            _hwndSource = null;
        }
    }
}
