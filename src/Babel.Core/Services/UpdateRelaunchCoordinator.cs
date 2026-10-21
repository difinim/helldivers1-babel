using System;
using System.Threading;
using System.Threading.Tasks;
using Babel.Core.Interfaces;

namespace Babel.Core.Services;

/// <summary>
/// Coordinates background update execution and triggers Option B restart:
/// - If Helldivers is idle, restarts immediately.
/// - If Helldivers is active, defers restart until the game process terminates.
/// </summary>
public sealed class UpdateRelaunchCoordinator : IDisposable
{
    private readonly IAutoUpdateService _updateService;
    private readonly IProcessMonitorService _processMonitorService;
    private readonly Action _requestRestartAction;
    private readonly int _startupDelayMs;
    private readonly CancellationTokenSource _cts = new();

    private bool _isUpdatePendingRestart;
    private bool _isSubscribedToGameExit;
    private bool _isDisposed;
    private readonly object _lock = new();

    public UpdateRelaunchCoordinator(
        IAutoUpdateService updateService,
        IProcessMonitorService processMonitorService,
        Action requestRestartAction,
        int startupDelayMs = 2000)
    {
        _updateService = updateService ?? throw new ArgumentNullException(nameof(updateService));
        _processMonitorService = processMonitorService ?? throw new ArgumentNullException(nameof(processMonitorService));
        _requestRestartAction = requestRestartAction ?? throw new ArgumentNullException(nameof(requestRestartAction));
        _startupDelayMs = startupDelayMs;
    }

    public bool IsUpdatePendingRestart => _isUpdatePendingRestart;

    /// <summary>
    /// Spawns the background check and staging task.
    /// </summary>
    public void Start()
    {
        _ = Task.Run(async () =>
        {
            try
            {
                if (_updateService.IsDevModeActive())
                {
                    return;
                }

                if (_startupDelayMs > 0)
                {
                    await Task.Delay(_startupDelayMs, _cts.Token).ConfigureAwait(false);
                }

                var applied = await _updateService.RunUpdateWorkflowAsync(_cts.Token).ConfigureAwait(false);
                if (!applied || _isDisposed)
                {
                    return;
                }

                lock (_lock)
                {
                    _isUpdatePendingRestart = true;
                    EvaluateRestartConditions();
                }
            }
            catch (OperationCanceledException)
            {
                // Normal cancellation on shutdown
            }
            catch (Exception ex)
            {
                BabelLog.Warn("UpdateCoordinator", $"Auto-update workflow ended with message: {ex.Message}");
            }
        });
    }

    private void EvaluateRestartConditions()
    {
        if (!_isUpdatePendingRestart || _isDisposed) return;

        if (!_processMonitorService.IsGameRunning)
        {
            BabelLog.Info("UpdateCoordinator", "Game is idle. Requesting silent restart for update application.");
            _requestRestartAction();
        }
        else
        {
            BabelLog.Info("UpdateCoordinator", "Game is running. Deferring silent restart until Helldivers exits.");
            if (!_isSubscribedToGameExit)
            {
                _isSubscribedToGameExit = true;
                _processMonitorService.ProcessTerminated += OnGameTerminated;
            }
        }
    }

    private void OnGameTerminated()
    {
        lock (_lock)
        {
            if (_isSubscribedToGameExit)
            {
                _processMonitorService.ProcessTerminated -= OnGameTerminated;
                _isSubscribedToGameExit = false;
            }

            if (!_isUpdatePendingRestart || _isDisposed) return;

            BabelLog.Info("UpdateCoordinator", "Helldivers process terminated. Initiating deferred silent restart.");
            _requestRestartAction();
        }
    }

    public void Dispose()
    {
        lock (_lock)
        {
            if (_isDisposed) return;
            _isDisposed = true;

            if (_isSubscribedToGameExit)
            {
                _processMonitorService.ProcessTerminated -= OnGameTerminated;
                _isSubscribedToGameExit = false;
            }
        }

        _cts.Cancel();
        _cts.Dispose();

        if (_updateService is IDisposable disposableService)
        {
            disposableService.Dispose();
        }
    }
}
