namespace Babel.Core.Tests;

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Babel.Core.Services;
using Xunit;

public class ProcessMonitorTests
{
    [Fact]
    public void Constructor_DefaultParameters_InitializesExpectedDefaults()
    {
        using var monitor = new ProcessMonitorService();

        Assert.Equal("helldivers", monitor.ProcessName);
        Assert.Equal(TimeSpan.FromMilliseconds(1500), monitor.PollingInterval);
        Assert.False(monitor.IsGameRunning);
        Assert.Null(monitor.TargetProcessId);
        Assert.Null(monitor.CurrentProcess);
    }

    [Fact]
    public void Constructor_StripsExeExtension_Properly()
    {
        using var monitor = new ProcessMonitorService("helldivers.exe");
        Assert.Equal("helldivers", monitor.ProcessName);
    }

    [Fact]
    public void Is64BitProcess_CurrentProcess_ReturnsTrueOn64BitOS()
    {
        if (Environment.Is64BitOperatingSystem && Environment.Is64BitProcess)
        {
            var currentProc = Process.GetCurrentProcess();
            bool is64 = ProcessMonitorService.Is64BitProcess(currentProc);
            Assert.True(is64);

            bool is64ById = ProcessMonitorService.Is64BitProcess(currentProc.Id);
            Assert.True(is64ById);
        }
    }

    [Fact]
    public void Is64BitProcess_InvalidPid_ReturnsFalse()
    {
        Assert.False(ProcessMonitorService.Is64BitProcess(-1));
        Assert.False(ProcessMonitorService.Is64BitProcess(0));
        Assert.False(ProcessMonitorService.Is64BitProcess(int.MaxValue));
    }

    [Fact]
    public void StartAndStop_CyclesCleanlyWithoutExceptions()
    {
        var logs = new System.Collections.Generic.List<string>();
        using var monitor = new ProcessMonitorService(
            processName: "nonexistent_proc_xyz_123",
            pollingInterval: TimeSpan.FromMilliseconds(100),
            logger: logs.Add);

        monitor.Start();
        // Multiple starts should be idempotent
        monitor.Start();

        Thread.Sleep(250);

        Assert.False(monitor.IsGameRunning);
        Assert.Null(monitor.TargetProcessId);

        monitor.Stop();
        // Multiple stops should be idempotent
        monitor.Stop();
    }

    [Fact]
    public void EventAliases_WireUpCorrectly()
    {
        using var monitor = new ProcessMonitorService();

        Action<Process> startedHandler = _ => { };
        Action exitedHandler = () => { };

        monitor.ProcessStarted += startedHandler;
        monitor.ProcessExited += exitedHandler;

        monitor.ProcessStarted -= startedHandler;
        monitor.ProcessExited -= exitedHandler;
    }
}
