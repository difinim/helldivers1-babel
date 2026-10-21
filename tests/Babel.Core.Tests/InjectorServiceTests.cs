namespace Babel.Core.Tests;

using System;
using System.Diagnostics;
using System.IO;
using Babel.Core.Services;
using Xunit;

public class InjectorServiceTests
{
    [Fact]
    public void IsInjected_InvalidParameters_ReturnsFalse()
    {
        var injector = new InjectorService();

        Assert.False(injector.IsInjected(-1, "BabelHook.dll"));
        Assert.False(injector.IsInjected(0, "BabelHook.dll"));
        Assert.False(injector.IsInjected(Process.GetCurrentProcess().Id, ""));
        Assert.False(injector.IsInjected(Process.GetCurrentProcess().Id, "   "));
    }

    [Fact]
    public void IsInjected_KnownLoadedModuleInCurrentProcess_ReturnsTrue()
    {
        var logs = new System.Collections.Generic.List<string>();
        var injector = new InjectorService(logs.Add);

        int currentPid = Process.GetCurrentProcess().Id;

        // kernel32.dll and ntdll.dll are loaded into every Windows process
        bool isLoaded = injector.IsInjected(currentPid, "kernel32.dll");
        Assert.True(isLoaded);

        bool isNtdllLoaded = injector.IsInjected(currentPid, "ntdll.dll");
        Assert.True(isNtdllLoaded);
    }

    [Fact]
    public void IsInjected_NonExistentModule_ReturnsFalse()
    {
        var injector = new InjectorService();
        int currentPid = Process.GetCurrentProcess().Id;

        bool isLoaded = injector.IsInjected(currentPid, "TotallyFakeNonExistentModule_999.dll");
        Assert.False(isLoaded);
    }

    [Fact]
    public void Inject_NonExistentDll_ReturnsFalse()
    {
        var logs = new System.Collections.Generic.List<string>();
        var injector = new InjectorService(logs.Add);

        int currentPid = Process.GetCurrentProcess().Id;
        bool result = injector.Inject(currentPid, "C:\\NonExistentPath\\NonExistentDll.dll");

        Assert.False(result);
        Assert.Contains(logs, l => l.Contains("not found"));
    }

    [Fact]
    public void Inject_InvalidPid_ReturnsFalse()
    {
        var logs = new System.Collections.Generic.List<string>();
        var injector = new InjectorService(logs.Add);

        bool result = injector.Inject(-1, "dummy.dll");
        Assert.False(result);
    }

    [Fact]
    public void Inject_AlreadyLoadedModule_ReturnsTrue()
    {
        var logs = new System.Collections.Generic.List<string>();
        var injector = new InjectorService(logs.Add);

        int currentPid = Process.GetCurrentProcess().Id;
        string system32 = Environment.GetFolderPath(Environment.SpecialFolder.System);
        string kernel32Path = Path.Combine(system32, "kernel32.dll");

        if (File.Exists(kernel32Path))
        {
            // kernel32 is already loaded into current process
            bool result = injector.Inject(currentPid, kernel32Path);
            Assert.True(result);
            Assert.Contains(logs, l => l.Contains("already loaded"));
        }
    }
}
