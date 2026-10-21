namespace Babel.Core.Interfaces;

using System.Diagnostics;

/// <summary>
/// Service interface for monitoring the lifecycle of the target game process (helldivers.exe).
/// </summary>
public interface IProcessMonitorService : IDisposable
{
    /// <summary>
    /// Raised when the target game process is detected and confirmed to be running and 64-bit.
    /// </summary>
    event Action<Process>? ProcessDetected;

    /// <summary>
    /// Raised when the target game process terminates or exits.
    /// </summary>
    event Action? ProcessTerminated;

    /// <summary>
    /// Gets whether the target game process is currently running.
    /// </summary>
    bool IsGameRunning { get; }

    /// <summary>
    /// Gets the process ID of the running target game, or null if not running.
    /// </summary>
    int? TargetProcessId { get; }

    /// <summary>
    /// Gets the active target Process instance, or null if not running.
    /// </summary>
    Process? CurrentProcess { get; }

    /// <summary>
    /// Starts background polling for the target game process.
    /// </summary>
    void Start();

    /// <summary>
    /// Stops background polling.
    /// </summary>
    void Stop();
}
