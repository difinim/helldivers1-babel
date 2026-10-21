namespace Babel.Core.Services;

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Babel.Core.Interfaces;
using Babel.Core.Native;

/// <summary>
/// Monitors for the launch and termination of the Helldivers (2015) 64-bit game process.
/// </summary>
public sealed class ProcessMonitorService : IProcessMonitorService
{
    private readonly object _lock = new();
    private readonly string _processName;
    private readonly TimeSpan _pollingInterval;
    private readonly Action<string>? _logger;

    private CancellationTokenSource? _cts;
    private Task? _pollTask;
    private Process? _currentProcess;
    private bool _isGameRunning;
    private int? _targetProcessId;
    private bool _disposed;

    /// <summary>
    /// Raised when the target 64-bit game process is detected.
    /// </summary>
    public event Action<Process>? ProcessDetected;

    /// <summary>
    /// Raised when the target game process has exited or terminated.
    /// </summary>
    public event Action? ProcessTerminated;

    /// <summary>
    /// Alias for <see cref="ProcessDetected"/>.
    /// </summary>
    public event Action<Process>? ProcessStarted
    {
        add => ProcessDetected += value;
        remove => ProcessDetected -= value;
    }

    /// <summary>
    /// Alias for <see cref="ProcessTerminated"/>.
    /// </summary>
    public event Action? ProcessExited
    {
        add => ProcessTerminated += value;
        remove => ProcessTerminated -= value;
    }

    /// <summary>
    /// Gets whether the target game process is currently running.
    /// </summary>
    public bool IsGameRunning
    {
        get
        {
            lock (_lock)
            {
                return _isGameRunning;
            }
        }
    }

    /// <summary>
    /// Gets the process ID of the running game, or null if not running.
    /// </summary>
    public int? TargetProcessId
    {
        get
        {
            lock (_lock)
            {
                return _targetProcessId;
            }
        }
    }

    /// <summary>
    /// Gets the active target Process instance, or null if not running.
    /// </summary>
    public Process? CurrentProcess
    {
        get
        {
            lock (_lock)
            {
                return _currentProcess;
            }
        }
    }

    /// <summary>
    /// Gets the monitored process name (without extension).
    /// </summary>
    public string ProcessName => _processName;

    /// <summary>
    /// Gets the polling interval used to check for the process.
    /// </summary>
    public TimeSpan PollingInterval => _pollingInterval;

    /// <summary>
    /// Initializes a new instance of <see cref="ProcessMonitorService"/>.
    /// </summary>
    /// <param name="processName">Process name to monitor (defaults to "helldivers").</param>
    /// <param name="pollingInterval">Interval between poll checks (defaults to 1.5 seconds).</param>
    /// <param name="logger">Optional logging delegate.</param>
    public ProcessMonitorService(
        string processName = "helldivers",
        TimeSpan? pollingInterval = null,
        Action<string>? logger = null)
    {
        if (string.IsNullOrWhiteSpace(processName))
        {
            processName = "helldivers";
        }

        if (processName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
        {
            processName = Path.GetFileNameWithoutExtension(processName);
        }

        _processName = processName;
        _pollingInterval = pollingInterval ?? TimeSpan.FromMilliseconds(1500);
        _logger = logger;
    }

    /// <summary>
    /// Starts background polling for the target game process.
    /// </summary>
    public void Start()
    {
        lock (_lock)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(ProcessMonitorService));
            }

            if (_cts != null)
            {
                return; // Already running
            }

            _cts = new CancellationTokenSource();
            _pollTask = Task.Run(() => PollLoopAsync(_cts.Token));
            Log($"Process monitor started for '{_processName}' (interval: {_pollingInterval.TotalMilliseconds}ms).");
        }
    }

    /// <summary>
    /// Stops background polling.
    /// </summary>
    public void Stop()
    {
        CancellationTokenSource? ctsToCancel;
        lock (_lock)
        {
            ctsToCancel = _cts;
            _cts = null;
            _pollTask = null;
        }

        if (ctsToCancel != null)
        {
            try
            {
                ctsToCancel.Cancel();
                ctsToCancel.Dispose();
            }
            catch
            {
                // Ignore disposal errors
            }
            Log("Process monitor stopped.");
        }
    }

    /// <summary>
    /// Checks if a process is running as a 64-bit architecture.
    /// </summary>
    public static bool Is64BitProcess(Process process)
    {
        if (process == null) return false;
        if (!Environment.Is64BitOperatingSystem) return false;

        try
        {
            if (process.HasExited) return false;
        }
        catch
        {
            return false;
        }

        IntPtr hProcess = IntPtr.Zero;
        bool mustClose = false;

        try
        {
            try
            {
                hProcess = process.Handle;
            }
            catch
            {
                hProcess = NativeMethods.OpenProcess(
                    NativeMethods.PROCESS_QUERY_LIMITED_INFORMATION,
                    false,
                    process.Id);
                mustClose = (hProcess != IntPtr.Zero);
            }

            if (hProcess == IntPtr.Zero)
            {
                return false;
            }

            if (NativeMethods.IsWow64Process(hProcess, out bool isWow64))
            {
                // On a 64-bit OS:
                // isWow64 == true means 32-bit process under WOW64.
                // isWow64 == false means native 64-bit process.
                return !isWow64;
            }

            return false;
        }
        catch
        {
            return false;
        }
        finally
        {
            if (mustClose && hProcess != IntPtr.Zero)
            {
                NativeMethods.CloseHandle(hProcess);
            }
        }
    }

    /// <summary>
    /// Checks if a process ID corresponds to a 64-bit architecture.
    /// </summary>
    public static bool Is64BitProcess(int processId)
    {
        if (processId <= 0 || !Environment.Is64BitOperatingSystem)
        {
            return false;
        }

        IntPtr hProcess = NativeMethods.OpenProcess(
            NativeMethods.PROCESS_QUERY_LIMITED_INFORMATION,
            false,
            processId);

        if (hProcess == IntPtr.Zero)
        {
            return false;
        }

        try
        {
            if (NativeMethods.IsWow64Process(hProcess, out bool isWow64))
            {
                return !isWow64;
            }
            return false;
        }
        finally
        {
            NativeMethods.CloseHandle(hProcess);
        }
    }

    private async Task PollLoopAsync(CancellationToken ct)
    {
        // Immediate initial check
        CheckProcess();

        using var timer = new PeriodicTimer(_pollingInterval);
        try
        {
            while (await timer.WaitForNextTickAsync(ct))
            {
                CheckProcess();
            }
        }
        catch (OperationCanceledException)
        {
            // Expected on Stop() or Dispose()
        }
        catch (Exception ex)
        {
            Log($"Unhandled exception in poll loop: {ex.Message}");
        }
    }

    private void CheckProcess()
    {
        try
        {
            // 1. If currently tracked, verify it has not exited
            bool hasExited = false;
            lock (_lock)
            {
                if (_currentProcess != null)
                {
                    try
                    {
                        hasExited = _currentProcess.HasExited;
                    }
                    catch
                    {
                        hasExited = true;
                    }
                }
            }

            if (hasExited)
            {
                HandleProcessTerminated();
            }

            // 2. If not running, scan for the process
            bool shouldScan = false;
            lock (_lock)
            {
                shouldScan = !_isGameRunning;
            }

            if (shouldScan)
            {
                ScanForProcess();
            }
        }
        catch (Exception ex)
        {
            Log($"Error during process check: {ex.Message}");
        }
    }

    private void ScanForProcess()
    {
        Process[] candidates;
        try
        {
            candidates = Process.GetProcessesByName(_processName);
        }
        catch (Exception ex)
        {
            Log($"Failed to enumerate processes by name '{_processName}': {ex.Message}");
            return;
        }

        Process? matched = null;
        foreach (var proc in candidates)
        {
            if (matched == null && !proc.HasExited && Is64BitProcess(proc))
            {
                matched = proc;
            }
            else
            {
                proc.Dispose();
            }
        }

        if (matched != null)
        {
            bool fireEvent = false;
            lock (_lock)
            {
                if (!_isGameRunning)
                {
                    _currentProcess = matched;
                    _targetProcessId = matched.Id;
                    _isGameRunning = true;
                    fireEvent = true;

                    try
                    {
                        _currentProcess.EnableRaisingEvents = true;
                        _currentProcess.Exited += OnProcessExitedHandler;
                    }
                    catch
                    {
                        // Polling fallback handles termination if events cannot be raised
                    }
                }
                else
                {
                    matched.Dispose();
                }
            }

            if (fireEvent)
            {
                Log($"Detected 64-bit target process: {_processName} (PID: {matched.Id})");
                try
                {
                    ProcessDetected?.Invoke(matched);
                }
                catch (Exception ex)
                {
                    Log($"Exception in ProcessDetected handler: {ex.Message}");
                }
            }
        }
    }

    private void OnProcessExitedHandler(object? sender, EventArgs e)
    {
        HandleProcessTerminated();
    }

    private void HandleProcessTerminated()
    {
        Process? toDispose = null;
        int? terminatedPid = null;
        bool fireEvent = false;

        lock (_lock)
        {
            if (_isGameRunning)
            {
                _isGameRunning = false;
                terminatedPid = _targetProcessId;
                _targetProcessId = null;
                toDispose = _currentProcess;
                _currentProcess = null;
                fireEvent = true;
            }
        }

        if (toDispose != null)
        {
            try
            {
                toDispose.Exited -= OnProcessExitedHandler;
                toDispose.Dispose();
            }
            catch
            {
                // Ignore disposal errors
            }
        }

        if (fireEvent)
        {
            Log($"Target game process terminated (PID: {terminatedPid}).");
            try
            {
                ProcessTerminated?.Invoke();
            }
            catch (Exception ex)
            {
                Log($"Exception in ProcessTerminated handler: {ex.Message}");
            }
        }
    }

    private void Log(string message)
    {
        _logger?.Invoke($"[ProcessMonitorService] {message}");
    }

    /// <summary>
    /// Disposes resources, stops the monitor, and unhooks process events.
    /// </summary>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        Stop();

        Process? proc;
        lock (_lock)
        {
            _isGameRunning = false;
            _targetProcessId = null;
            proc = _currentProcess;
            _currentProcess = null;
        }

        if (proc != null)
        {
            try
            {
                proc.Exited -= OnProcessExitedHandler;
                proc.Dispose();
            }
            catch
            {
                // Ignore
            }
        }
    }
}
