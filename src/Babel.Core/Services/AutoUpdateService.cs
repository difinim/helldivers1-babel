using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Babel.Core.Interfaces;

namespace Babel.Core.Services;

/// <summary>
/// Implements mandatory, silent, background auto-updates by querying latest_version.txt
/// in GitHub and downloading release distribution zip packages.
/// </summary>
public class AutoUpdateService : IAutoUpdateService, IDisposable
{
    private readonly string _baseDirectory;
    private readonly Version _currentVersion;
    private readonly HttpClient _httpClient;
    private readonly bool _ownsHttpClient;
    private readonly string _repoOwner;
    private readonly string _repoName;
    private readonly string _branch;

    public AutoUpdateService(
        string? baseDirectory = null,
        Version? currentVersion = null,
        HttpClient? httpClient = null,
        string repoOwner = "difinim",
        string repoName = "helldivers1-babel",
        string branch = "main")
    {
        _baseDirectory = !string.IsNullOrWhiteSpace(baseDirectory)
            ? baseDirectory
            : AppContext.BaseDirectory;

        _currentVersion = currentVersion
            ?? Assembly.GetEntryAssembly()?.GetName().Version
            ?? Assembly.GetExecutingAssembly().GetName().Version
            ?? new Version(1, 0, 0);

        _repoOwner = repoOwner;
        _repoName = repoName;
        _branch = branch;

        if (httpClient != null)
        {
            _httpClient = httpClient;
            _ownsHttpClient = false;
        }
        else
        {
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(15)
            };
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Helldivers1Babel-Updater/1.0");
            _ownsHttpClient = true;
        }
    }

    /// <inheritdoc />
    public bool IsDevModeActive()
    {
        return DevModeGuard.IsDevModeActive(_baseDirectory);
    }

    /// <inheritdoc />
    public async Task<UpdateCheckResult> CheckForUpdatesAsync(CancellationToken cancellationToken = default)
    {
        if (IsDevModeActive())
        {
            BabelLog.Info("AutoUpdate", "Skipping update check because BABEL_DEV_MODE is active.");
            return new UpdateCheckResult(false, _currentVersion, null, null);
        }

        var url = $"https://raw.githubusercontent.com/{_repoOwner}/{_repoName}/{_branch}/latest_version.txt?t={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";

        try
        {
            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCts.CancelAfter(TimeSpan.FromSeconds(8));

            var response = await _httpClient.GetStringAsync(url, timeoutCts.Token).ConfigureAwait(false);
            var trimmedVersion = response.Trim().TrimStart('v', 'V');

            if (!Version.TryParse(trimmedVersion, out var remoteVersion))
            {
                // Fallback for single-dot versions like 1.0 -> 1.0.0
                if (Version.TryParse(trimmedVersion + ".0", out var fallbackVer))
                {
                    remoteVersion = fallbackVer;
                }
                else
                {
                    BabelLog.Warn("AutoUpdate", $"Unable to parse remote version string: '{trimmedVersion}'");
                    return new UpdateCheckResult(false, _currentVersion, null, null);
                }
            }

            // Normalize both to major.minor.build comparison
            var normalizedCurrent = new Version(
                _currentVersion.Major,
                _currentVersion.Minor,
                Math.Max(0, _currentVersion.Build));

            var normalizedRemote = new Version(
                remoteVersion.Major,
                remoteVersion.Minor,
                Math.Max(0, remoteVersion.Build));

            if (normalizedRemote > normalizedCurrent)
            {
                var downloadUrl = $"https://github.com/{_repoOwner}/{_repoName}/releases/download/v{trimmedVersion}/Helldivers1Babel-v{trimmedVersion}-win-x64.zip";
                BabelLog.Info("AutoUpdate", $"Newer version found: v{trimmedVersion} (local: v{_currentVersion}). Download URL: {downloadUrl}");
                return new UpdateCheckResult(true, _currentVersion, remoteVersion, downloadUrl);
            }

            BabelLog.Info("AutoUpdate", $"Application is up to date (Local: v{_currentVersion}, Remote: v{remoteVersion}).");
            return new UpdateCheckResult(false, _currentVersion, remoteVersion, null);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            BabelLog.Info("AutoUpdate", "Version check timed out. Continuing normally.");
            return new UpdateCheckResult(false, _currentVersion, null, null);
        }
        catch (Exception ex)
        {
            BabelLog.Info("AutoUpdate", $"Version check unreachable ({ex.Message}). Continuing normally.");
            return new UpdateCheckResult(false, _currentVersion, null, null);
        }
    }

    /// <inheritdoc />
    public async Task<bool> DownloadAndApplyUpdateAsync(string downloadUrl, Version targetVersion, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(downloadUrl))
        {
            throw new ArgumentException("Download URL cannot be null or empty.", nameof(downloadUrl));
        }

        var tempDir = Path.Combine(Path.GetTempPath(), "Helldivers1Babel", $"update_{targetVersion}_{Guid.NewGuid():N}");
        var zipFilePath = Path.Combine(tempDir, "release.zip");
        var stagingDir = Path.Combine(tempDir, "staging");

        try
        {
            Directory.CreateDirectory(tempDir);
            Directory.CreateDirectory(stagingDir);

            BabelLog.Info("AutoUpdate", $"Starting background download from: {downloadUrl}");

            using (var responseStream = await _httpClient.GetStreamAsync(downloadUrl, cancellationToken).ConfigureAwait(false))
            using (var fileStream = new FileStream(zipFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 81920, useAsync: true))
            {
                await responseStream.CopyToAsync(fileStream, cancellationToken).ConfigureAwait(false);
            }

            BabelLog.Info("AutoUpdate", "Download complete. Extracting update package...");
            ZipFile.ExtractToDirectory(zipFilePath, stagingDir, overwriteFiles: true);

            // Determine if archive files are directly in staging or nested in a single top-level folder
            var sourceDir = stagingDir;
            var subDirs = Directory.GetDirectories(stagingDir);
            var rootFiles = Directory.GetFiles(stagingDir);
            if (rootFiles.Length == 0 && subDirs.Length == 1)
            {
                sourceDir = subDirs[0];
            }

            // Verify essential application files
            var mainExe = Path.Combine(sourceDir, "HD1-Babel.exe");
            var hookDll = Path.Combine(sourceDir, "BabelHook.dll");
            var coreDll = Path.Combine(sourceDir, "Babel.Core.dll");

            if (!File.Exists(mainExe) || !File.Exists(hookDll) || !File.Exists(coreDll))
            {
                throw new InvalidDataException("Downloaded package is missing essential application binaries (HD1-Babel.exe, BabelHook.dll, or Babel.Core.dll).");
            }

            BabelLog.Info("AutoUpdate", "Applying in-place file replacement...");
            InPlaceFileSwapper.SwapFiles(sourceDir, _baseDirectory);

            BabelLog.Info("AutoUpdate", $"Successfully updated binaries to version {targetVersion} in-place.");
            return true;
        }
        catch (Exception ex)
        {
            BabelLog.Error("AutoUpdate", $"Failed to download or apply update v{targetVersion}", ex);
            return false;
        }
        finally
        {
            try
            {
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, recursive: true);
                }
            }
            catch (Exception ex)
            {
                BabelLog.Warn("AutoUpdate", $"Could not clean temporary update directory '{tempDir}': {ex.Message}");
            }
        }
    }

    /// <inheritdoc />
    public async Task<bool> RunUpdateWorkflowAsync(CancellationToken cancellationToken = default)
    {
        if (IsDevModeActive())
        {
            return false;
        }

        var checkResult = await CheckForUpdatesAsync(cancellationToken).ConfigureAwait(false);
        if (!checkResult.UpdateAvailable || string.IsNullOrEmpty(checkResult.DownloadUrl) || checkResult.LatestVersion == null)
        {
            return false;
        }

        return await DownloadAndApplyUpdateAsync(checkResult.DownloadUrl, checkResult.LatestVersion, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_ownsHttpClient)
        {
            _httpClient.Dispose();
        }
    }
}
