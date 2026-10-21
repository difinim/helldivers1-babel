namespace Babel.Core.Services;

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Babel.Core.Interfaces;
using Babel.Core.Native;

/// <summary>
/// Implements remote thread DLL injection into 64-bit Windows processes.
/// </summary>
public sealed class InjectorService : IInjectorService
{
    private readonly Action<string>? _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="InjectorService"/>.
    /// </summary>
    /// <param name="logger">Optional logger delegate.</param>
    public InjectorService(Action<string>? logger = null)
    {
        _logger = logger;
    }

    /// <summary>
    /// Checks whether a specific DLL is loaded in the target process.
    /// </summary>
    /// <param name="processId">Target process ID.</param>
    /// <param name="dllName">File name or path of the DLL (e.g. "BabelHook.dll").</param>
    /// <returns>True if the DLL is currently loaded in the process; otherwise false.</returns>
    public bool IsInjected(int processId, string dllName = "BabelHook.dll")
    {
        if (processId <= 0 || string.IsNullOrWhiteSpace(dllName))
        {
            return false;
        }

        string targetFileName = Path.GetFileName(dllName);

        // Open target process with rights to query information and read virtual memory
        IntPtr hProcess = NativeMethods.OpenProcess(
            NativeMethods.PROCESS_QUERY_INFORMATION | NativeMethods.PROCESS_VM_READ,
            false,
            processId);

        if (hProcess == IntPtr.Zero)
        {
            // Fallback to query limited info if standard query info fails
            hProcess = NativeMethods.OpenProcess(
                NativeMethods.PROCESS_QUERY_LIMITED_INFORMATION | NativeMethods.PROCESS_VM_READ,
                false,
                processId);
        }

        if (hProcess == IntPtr.Zero)
        {
            Log($"IsInjected: Could not open process {processId} (Win32Error: {Marshal.GetLastWin32Error()}).");
            return false;
        }

        try
        {
            // Initial buffer for 1024 modules (8KB on x64)
            IntPtr[] hModules = new IntPtr[1024];
            uint cb = (uint)(hModules.Length * IntPtr.Size);

            if (!NativeMethods.EnumProcessModulesEx(hProcess, hModules, cb, out uint bytesNeeded, NativeMethods.LIST_MODULES_ALL))
            {
                Log($"IsInjected: EnumProcessModulesEx failed for PID {processId} (Win32Error: {Marshal.GetLastWin32Error()}).");
                return false;
            }

            int moduleCount = (int)(bytesNeeded / (uint)IntPtr.Size);
            if (bytesNeeded > cb)
            {
                hModules = new IntPtr[moduleCount];
                cb = bytesNeeded;
                if (!NativeMethods.EnumProcessModulesEx(hProcess, hModules, cb, out _, NativeMethods.LIST_MODULES_ALL))
                {
                    return false;
                }
            }

            var sb = new StringBuilder(1024);
            for (int i = 0; i < moduleCount; i++)
            {
                if (hModules[i] == IntPtr.Zero) continue;

                sb.Clear();
                if (NativeMethods.GetModuleFileNameEx(hProcess, hModules[i], sb, (uint)sb.Capacity) > 0)
                {
                    string modulePath = sb.ToString();
                    string moduleName = Path.GetFileName(modulePath);

                    if (string.Equals(moduleName, targetFileName, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(modulePath, dllName, StringComparison.OrdinalIgnoreCase))
                    {
                        Log($"IsInjected: Verified module '{targetFileName}' is loaded in PID {processId} ({modulePath}).");
                        return true;
                    }
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            Log($"IsInjected: Exception querying modules for PID {processId}: {ex.Message}");
            return false;
        }
        finally
        {
            NativeMethods.CloseHandle(hProcess);
        }
    }

    /// <summary>
    /// Injects a 64-bit DLL into the target process using CreateRemoteThread and LoadLibraryW.
    /// </summary>
    /// <param name="processId">Target process ID.</param>
    /// <param name="dllPath">Absolute or relative path to the DLL.</param>
    /// <returns>True if injection succeeded and module was loaded; otherwise false.</returns>
    public bool Inject(int processId, string dllPath)
    {
        if (processId <= 0)
        {
            Log($"Inject: Invalid process ID: {processId}");
            return false;
        }

        if (string.IsNullOrWhiteSpace(dllPath))
        {
            Log("Inject: DLL path is null or empty.");
            return false;
        }

        // 1. Verify injector process architecture
        if (!Environment.Is64BitProcess)
        {
            Log("Inject: Injector is not running as a 64-bit process. Cannot inject into 64-bit Helldivers.");
            return false;
        }

        // 2. Verify DLL file exists
        string fullDllPath;
        try
        {
            fullDllPath = Path.GetFullPath(dllPath);
        }
        catch (Exception ex)
        {
            Log($"Inject: Invalid DLL path '{dllPath}': {ex.Message}");
            return false;
        }

        if (!File.Exists(fullDllPath))
        {
            Log($"Inject: DLL file not found at path: {fullDllPath}");
            return false;
        }

        // 3. Verify target process exists and is 64-bit
        Process targetProcess;
        try
        {
            targetProcess = Process.GetProcessById(processId);
        }
        catch (Exception ex)
        {
            Log($"Inject: Target process {processId} not found: {ex.Message}");
            return false;
        }

        if (!ProcessMonitorService.Is64BitProcess(targetProcess))
        {
            Log($"Inject: Target process {processId} ({targetProcess.ProcessName}) is not a 64-bit process. Injection aborted.");
            return false;
        }

        // 4. Check if already injected
        string dllFileName = Path.GetFileName(fullDllPath);
        if (IsInjected(processId, dllFileName))
        {
            Log($"Inject: DLL '{dllFileName}' is already loaded in PID {processId}.");
            return true;
        }

        // 5. Open process with thread creation, memory operation, write, read, and query information rights
        uint desiredAccess = NativeMethods.PROCESS_CREATE_THREAD |
                             NativeMethods.PROCESS_VM_OPERATION |
                             NativeMethods.PROCESS_VM_WRITE |
                             NativeMethods.PROCESS_VM_READ |
                             NativeMethods.PROCESS_QUERY_INFORMATION;

        IntPtr hProcess = NativeMethods.OpenProcess(desiredAccess, false, processId);
        if (hProcess == IntPtr.Zero)
        {
            int err = Marshal.GetLastWin32Error();
            Log($"Inject: OpenProcess failed for PID {processId} (Win32Error: {err}).");
            return false;
        }

        IntPtr pRemoteMem = IntPtr.Zero;
        IntPtr hThread = IntPtr.Zero;

        try
        {
            // 6. Allocate memory in target process for the Unicode DLL path
            byte[] dllBytes = Encoding.Unicode.GetBytes(fullDllPath + "\0");
            UIntPtr allocSize = (UIntPtr)dllBytes.Length;

            pRemoteMem = NativeMethods.VirtualAllocEx(
                hProcess,
                IntPtr.Zero,
                allocSize,
                NativeMethods.MEM_COMMIT | NativeMethods.MEM_RESERVE,
                NativeMethods.PAGE_READWRITE);

            if (pRemoteMem == IntPtr.Zero)
            {
                int err = Marshal.GetLastWin32Error();
                Log($"Inject: VirtualAllocEx failed in PID {processId} (Win32Error: {err}).");
                return false;
            }

            // 7. Write Unicode DLL path to target memory
            if (!NativeMethods.WriteProcessMemory(hProcess, pRemoteMem, dllBytes, allocSize, out UIntPtr written) ||
                written != allocSize)
            {
                int err = Marshal.GetLastWin32Error();
                Log($"Inject: WriteProcessMemory failed in PID {processId}. Wrote {written}/{allocSize} bytes (Win32Error: {err}).");
                return false;
            }

            // 8. Resolve LoadLibraryW address from kernel32.dll
            IntPtr hKernel32 = NativeMethods.GetModuleHandle("kernel32.dll");
            if (hKernel32 == IntPtr.Zero)
            {
                Log("Inject: Failed to obtain kernel32.dll handle.");
                return false;
            }

            IntPtr pLoadLibraryW = NativeMethods.GetProcAddress(hKernel32, "LoadLibraryW");
            if (pLoadLibraryW == IntPtr.Zero)
            {
                Log("Inject: Failed to resolve address of LoadLibraryW in kernel32.dll.");
                return false;
            }

            // 9. Create remote thread pointing to LoadLibraryW with the DLL path pointer
            hThread = NativeMethods.CreateRemoteThread(
                hProcess,
                IntPtr.Zero,
                UIntPtr.Zero,
                pLoadLibraryW,
                pRemoteMem,
                0,
                out uint threadId);

            if (hThread == IntPtr.Zero)
            {
                int err = Marshal.GetLastWin32Error();
                Log($"Inject: CreateRemoteThread failed in PID {processId} (Win32Error: {err}).");
                return false;
            }

            Log($"Inject: Remote thread created (ThreadID: {threadId}) in PID {processId}. Waiting for execution...");

            // 10. Wait for remote thread completion with 5000ms timeout
            uint waitResult = NativeMethods.WaitForSingleObject(hThread, 5000);
            if (waitResult == NativeMethods.WAIT_TIMEOUT)
            {
                Log($"Inject: Remote thread timed out after 5000ms in PID {processId}.");
                return false;
            }
            else if (waitResult != NativeMethods.WAIT_OBJECT_0)
            {
                int err = Marshal.GetLastWin32Error();
                Log($"Inject: WaitForSingleObject returned unexpected result 0x{waitResult:X8} (Win32Error: {err}).");
                return false;
            }

            // 11. Inspect remote thread exit code (LoadLibraryW return value)
            if (NativeMethods.GetExitCodeThread(hThread, out uint exitCode))
            {
                if (exitCode == 0)
                {
                    Log($"Inject: Remote LoadLibraryW returned 0 in PID {processId}. Injection failed (module did not initialize).");
                    return false;
                }
                Log($"Inject: Remote LoadLibraryW succeeded in PID {processId} (HMODULE: 0x{exitCode:X8}).");
            }
            else
            {
                Log($"Inject: Could not retrieve exit code for remote thread in PID {processId}.");
            }

            // 12. Verify presence in process module table
            bool isLoaded = IsInjected(processId, dllFileName);
            if (isLoaded)
            {
                Log($"Inject: Successfully verified '{dllFileName}' is loaded in PID {processId}.");
                return true;
            }

            // If module table verification has not yet updated but thread exit code was valid HMODULE
            if (exitCode != 0)
            {
                Log($"Inject: Remote thread succeeded with exit code 0x{exitCode:X8}. Injection considered successful.");
                return true;
            }

            Log($"Inject: Module '{dllFileName}' not found in PID {processId} after thread execution.");
            return false;
        }
        catch (Exception ex)
        {
            Log($"Inject: Unexpected exception during injection into PID {processId}: {ex}");
            return false;
        }
        finally
        {
            // 13. Clean up remote memory and handles
            if (pRemoteMem != IntPtr.Zero)
            {
                NativeMethods.VirtualFreeEx(hProcess, pRemoteMem, UIntPtr.Zero, NativeMethods.MEM_RELEASE);
            }

            if (hThread != IntPtr.Zero)
            {
                NativeMethods.CloseHandle(hThread);
            }

            if (hProcess != IntPtr.Zero)
            {
                NativeMethods.CloseHandle(hProcess);
            }
        }
    }

    private void Log(string message)
    {
        _logger?.Invoke($"[InjectorService] {message}");
    }
}
