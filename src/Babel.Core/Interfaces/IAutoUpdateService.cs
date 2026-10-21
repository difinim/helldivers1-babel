using System;
using System.Threading;
using System.Threading.Tasks;

namespace Babel.Core.Interfaces;

/// <summary>
/// Details of an update check result.
/// </summary>
public record UpdateCheckResult(
    bool UpdateAvailable,
    Version? CurrentVersion,
    Version? LatestVersion,
    string? DownloadUrl);

/// <summary>
/// Service contract for mandatory background auto-updating from GitHub releases.
/// </summary>
public interface IAutoUpdateService
{
    /// <summary>
    /// Checks whether BABEL_DEV_MODE is present in any of the 3 ancestor folders.
    /// </summary>
    bool IsDevModeActive();

    /// <summary>
    /// Checks GitHub repository latest_version.txt to determine if a newer release exists.
    /// </summary>
    Task<UpdateCheckResult> CheckForUpdatesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads the release zip package and performs the atomic in-place file replacement.
    /// </summary>
    Task<bool> DownloadAndApplyUpdateAsync(string downloadUrl, Version targetVersion, CancellationToken cancellationToken = default);

    /// <summary>
    /// Orchestrates the complete update lifecycle (guard check, version query, download, and in-place file swap).
    /// Returns true if an update was successfully staged/swapped into place and is ready for restart.
    /// </summary>
    Task<bool> RunUpdateWorkflowAsync(CancellationToken cancellationToken = default);
}
