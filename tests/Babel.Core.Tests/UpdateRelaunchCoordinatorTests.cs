using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Babel.Core.Interfaces;
using Babel.Core.Services;
using Xunit;

namespace Babel.Core.Tests;

public class UpdateRelaunchCoordinatorTests
{
    private class MockProcessMonitor : IProcessMonitorService
    {
#pragma warning disable CS0067
        public event Action<Process>? ProcessDetected;
#pragma warning restore CS0067
        public event Action? ProcessTerminated;

        public bool IsGameRunning { get; set; }
        public int? TargetProcessId => IsGameRunning ? 1234 : null;
        public Process? CurrentProcess => null;

        public void Start() { }
        public void Stop() { }
        public void Dispose() { }

        public void TriggerProcessTerminated()
        {
            IsGameRunning = false;
            ProcessTerminated?.Invoke();
        }
    }

    private class MockAutoUpdateService : IAutoUpdateService
    {
        public bool DevModeActive { get; set; }
        public bool UpdateWorkflowResult { get; set; }
        public bool WorkflowWasRun { get; private set; }

        public bool IsDevModeActive() => DevModeActive;

        public Task<UpdateCheckResult> CheckForUpdatesAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new UpdateCheckResult(UpdateWorkflowResult, new Version(1, 0, 0), new Version(1, 1, 0), "http://test.zip"));
        }

        public Task<bool> DownloadAndApplyUpdateAsync(string downloadUrl, Version targetVersion, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(UpdateWorkflowResult);
        }

        public Task<bool> RunUpdateWorkflowAsync(CancellationToken cancellationToken = default)
        {
            WorkflowWasRun = true;
            return Task.FromResult(UpdateWorkflowResult);
        }
    }

    [Fact]
    public async Task Coordinator_WhenGameIdle_TriggersRestartImmediately()
    {
        var monitor = new MockProcessMonitor { IsGameRunning = false };
        var updater = new MockAutoUpdateService { DevModeActive = false, UpdateWorkflowResult = true };

        var restartSignaled = new ManualResetEventSlim(false);

        using var coordinator = new UpdateRelaunchCoordinator(
            updater,
            monitor,
            () => restartSignaled.Set(),
            startupDelayMs: 0);

        coordinator.Start();

        var triggered = restartSignaled.Wait(TimeSpan.FromSeconds(2));
        Assert.True(triggered, "Restart action should have been invoked immediately when game is idle.");
        Assert.True(updater.WorkflowWasRun);
    }

    [Fact]
    public async Task Coordinator_WhenGameRunning_DefersRestartUntilGameExits()
    {
        var monitor = new MockProcessMonitor { IsGameRunning = true };
        var updater = new MockAutoUpdateService { DevModeActive = false, UpdateWorkflowResult = true };

        var restartSignaled = new ManualResetEventSlim(false);

        using var coordinator = new UpdateRelaunchCoordinator(
            updater,
            monitor,
            () => restartSignaled.Set(),
            startupDelayMs: 0);

        coordinator.Start();

        // Give worker thread time to process and find game is running
        await Task.Delay(100);

        Assert.False(restartSignaled.IsSet, "Restart action must not be called while game is running.");
        Assert.True(updater.WorkflowWasRun);

        // Now game exits
        monitor.TriggerProcessTerminated();

        var triggered = restartSignaled.Wait(TimeSpan.FromSeconds(2));
        Assert.True(triggered, "Restart action should be triggered after game termination.");
    }

    [Fact]
    public async Task Coordinator_WhenDevModeActive_DoesNotRunUpdateOrRestart()
    {
        var monitor = new MockProcessMonitor { IsGameRunning = false };
        var updater = new MockAutoUpdateService { DevModeActive = true, UpdateWorkflowResult = true };

        var restartSignaled = new ManualResetEventSlim(false);

        using var coordinator = new UpdateRelaunchCoordinator(
            updater,
            monitor,
            () => restartSignaled.Set(),
            startupDelayMs: 0);

        coordinator.Start();

        await Task.Delay(100);

        Assert.False(restartSignaled.IsSet);
        Assert.False(updater.WorkflowWasRun, "Workflow should not be executed when dev mode is active.");
    }

    [Fact]
    public async Task Coordinator_WhenNoUpdateAvailable_DoesNotRestart()
    {
        var monitor = new MockProcessMonitor { IsGameRunning = false };
        var updater = new MockAutoUpdateService { DevModeActive = false, UpdateWorkflowResult = false };

        var restartSignaled = new ManualResetEventSlim(false);

        using var coordinator = new UpdateRelaunchCoordinator(
            updater,
            monitor,
            () => restartSignaled.Set(),
            startupDelayMs: 0);

        coordinator.Start();

        await Task.Delay(100);

        Assert.False(restartSignaled.IsSet);
        Assert.True(updater.WorkflowWasRun);
    }
}
