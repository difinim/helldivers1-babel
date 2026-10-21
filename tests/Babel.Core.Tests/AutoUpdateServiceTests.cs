using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Babel.Core.Services;
using Xunit;

namespace Babel.Core.Tests;

public class AutoUpdateServiceTests : IDisposable
{
    private readonly string _testRoot;

    public AutoUpdateServiceTests()
    {
        _testRoot = Path.Combine(Path.GetTempPath(), "HD1_Babel_UpdateServiceTests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_testRoot);
    }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(_testRoot))
            {
                Directory.Delete(_testRoot, recursive: true);
            }
        }
        catch { }
    }

    private class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;

        public MockHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responder)
        {
            _responder = responder;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_responder(request));
        }
    }

    [Fact]
    public async Task CheckForUpdatesAsync_WhenRemoteVersionNewer_ReturnsUpdateAvailable()
    {
        var appDir = Path.Combine(_testRoot, "App");
        Directory.CreateDirectory(appDir);

        var mockHandler = new MockHttpMessageHandler(req =>
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("1.1.0\n", Encoding.UTF8, "text/plain")
            };
        });

        using var client = new HttpClient(mockHandler);
        using var service = new AutoUpdateService(
            baseDirectory: appDir,
            currentVersion: new Version(1, 0, 0),
            httpClient: client);

        var result = await service.CheckForUpdatesAsync();

        Assert.True(result.UpdateAvailable);
        Assert.Equal(new Version(1, 1, 0), result.LatestVersion);
        Assert.NotNull(result.DownloadUrl);
        Assert.Contains("v1.1.0", result.DownloadUrl);
    }

    [Fact]
    public async Task CheckForUpdatesAsync_WhenRemoteVersionSame_ReturnsNoUpdate()
    {
        var appDir = Path.Combine(_testRoot, "App");
        Directory.CreateDirectory(appDir);

        var mockHandler = new MockHttpMessageHandler(req =>
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("1.0.0", Encoding.UTF8, "text/plain")
            };
        });

        using var client = new HttpClient(mockHandler);
        using var service = new AutoUpdateService(
            baseDirectory: appDir,
            currentVersion: new Version(1, 0, 0),
            httpClient: client);

        var result = await service.CheckForUpdatesAsync();

        Assert.False(result.UpdateAvailable);
    }

    [Fact]
    public async Task CheckForUpdatesAsync_WhenDevModeActive_SkipsCheck()
    {
        // Place BABEL_DEV_MODE in parent of App
        var parentDir = Path.Combine(_testRoot, "Parent");
        var appDir = Path.Combine(parentDir, "App");
        Directory.CreateDirectory(appDir);
        File.WriteAllText(Path.Combine(parentDir, DevModeGuard.DevModeFileName), "");

        bool httpCalled = false;
        var mockHandler = new MockHttpMessageHandler(req =>
        {
            httpCalled = true;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("2.0.0", Encoding.UTF8, "text/plain")
            };
        });

        using var client = new HttpClient(mockHandler);
        using var service = new AutoUpdateService(
            baseDirectory: appDir,
            currentVersion: new Version(1, 0, 0),
            httpClient: client);

        var result = await service.CheckForUpdatesAsync();

        Assert.False(result.UpdateAvailable);
        Assert.False(httpCalled);
        Assert.True(service.IsDevModeActive());
    }

    [Fact]
    public async Task CheckForUpdatesAsync_WhenHttpFails_FailsSafelyWithoutThrowing()
    {
        var appDir = Path.Combine(_testRoot, "App");
        Directory.CreateDirectory(appDir);

        var mockHandler = new MockHttpMessageHandler(req =>
        {
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        });

        using var client = new HttpClient(mockHandler);
        using var service = new AutoUpdateService(
            baseDirectory: appDir,
            currentVersion: new Version(1, 0, 0),
            httpClient: client);

        var result = await service.CheckForUpdatesAsync();

        Assert.False(result.UpdateAvailable);
    }

    [Fact]
    public async Task DownloadAndApplyUpdateAsync_ValidZip_SwapsBinariesCleanly()
    {
        var appDir = Path.Combine(_testRoot, "App");
        Directory.CreateDirectory(appDir);

        // Pre-populate app directory with 1.0 binaries
        var exePath = Path.Combine(appDir, "HD1-Babel.exe");
        var hookPath = Path.Combine(appDir, "BabelHook.dll");
        var corePath = Path.Combine(appDir, "Babel.Core.dll");
        File.WriteAllText(exePath, "1.0 exe");
        File.WriteAllText(hookPath, "1.0 hook");
        File.WriteAllText(corePath, "1.0 core");

        // Create a mock release zip in memory
        byte[] zipBytes;
        using (var mem = new MemoryStream())
        {
            using (var archive = new ZipArchive(mem, ZipArchiveMode.Create, true))
            {
                var entryExe = archive.CreateEntry("HD1-Babel.exe");
                using (var writer = new StreamWriter(entryExe.Open())) writer.Write("1.1 exe");

                var entryHook = archive.CreateEntry("BabelHook.dll");
                using (var writer = new StreamWriter(entryHook.Open())) writer.Write("1.1 hook");

                var entryCore = archive.CreateEntry("Babel.Core.dll");
                using (var writer = new StreamWriter(entryCore.Open())) writer.Write("1.1 core");
            }
            zipBytes = mem.ToArray();
        }

        var mockHandler = new MockHttpMessageHandler(req =>
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(zipBytes)
            };
        });

        using var client = new HttpClient(mockHandler);
        using var service = new AutoUpdateService(
            baseDirectory: appDir,
            currentVersion: new Version(1, 0, 0),
            httpClient: client);

        var success = await service.DownloadAndApplyUpdateAsync(
            "https://github.com/difinim/helldivers1-babel/releases/download/v1.1.0/test.zip",
            new Version(1, 1, 0));

        Assert.True(success);

        // Binaries in app directory are updated
        Assert.Equal("1.1 exe", File.ReadAllText(exePath));
        Assert.Equal("1.1 hook", File.ReadAllText(hookPath));
        Assert.Equal("1.1 core", File.ReadAllText(corePath));

        // Backups exist
        Assert.True(File.Exists(exePath + ".old"));
        Assert.Equal("1.0 exe", File.ReadAllText(exePath + ".old"));
    }
}
