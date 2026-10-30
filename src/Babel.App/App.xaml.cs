namespace Babel.App;

using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using Babel.App.Services;
using Babel.Core.Services;

/// <summary>
/// Interaction logic for App.xaml
/// Enforces single-instance application behavior and brings existing instance to foreground upon reactivation.
/// </summary>
public partial class App : Application
{
    private const string AppMutexName = @"Local\Helldivers1Babel_App_SingleInstance_Mutex";
    private const string AppWakeupEventName = @"Local\Helldivers1Babel_App_Wakeup_Event";

    private Mutex? _mutex;
    private EventWaitHandle? _wakeupEvent;
    private RegisteredWaitHandle? _registeredWait;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // If launched via Option B silent auto-update, wait for the previous process to release mutex and exit
        int? waitPid = null;
        for (int i = 0; i < e.Args.Length; i++)
        {
            if (string.Equals(e.Args[i], "--wait-pid", StringComparison.OrdinalIgnoreCase) && i + 1 < e.Args.Length)
            {
                if (int.TryParse(e.Args[i + 1], out var parsedPid))
                {
                    waitPid = parsedPid;
                }
            }
        }

        if (waitPid.HasValue)
        {
            try
            {
                using var targetProcess = Process.GetProcessById(waitPid.Value);
                targetProcess.WaitForExit(3000);
            }
            catch
            {
                // Process already terminated or invalid PID
            }
        }

        // Purge leftover *.old binaries from previous updates now that old process is terminated
        InPlaceFileSwapper.PurgeOldFiles(AppContext.BaseDirectory);

        bool isNewInstance;
        try
        {
            _mutex = new Mutex(true, AppMutexName, out isNewInstance);
        }
        catch (AbandonedMutexException)
        {
            // Previous instance crashed or exited abnormally without releasing mutex
            isNewInstance = true;
        }

        if (!isNewInstance)
        {
            // Clean up our unowned mutex handle
            _mutex?.Dispose();
            _mutex = null;

            // Grant foreground activation permission to the existing instance
            NativeWindowMethods.AllowSetForegroundWindow(NativeWindowMethods.ASFW_ANY);

            // Signal the existing instance to bring its window to foreground
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    using var wakeupEvent = EventWaitHandle.OpenExisting(AppWakeupEventName);
                    wakeupEvent.Set();
                    break;
                }
                catch (WaitHandleCannotBeOpenedException)
                {
                    Thread.Sleep(50);
                }
                catch
                {
                    break;
                }
            }

            Shutdown();
            return;
        }

        // We are the primary instance: create the wakeup event and register wait callback
        _wakeupEvent = new EventWaitHandle(false, EventResetMode.AutoReset, AppWakeupEventName);
        _registeredWait = ThreadPool.RegisterWaitForSingleObject(
            _wakeupEvent,
            OnWakeupSignaled,
            null,
            -1,
            false);

        var configService = new ConfigurationService();
        var initialConfig = configService.Load();
        AppLocalizationManager.Initialize(initialConfig.TargetLanguage);

        var mainWindow = new MainWindow();
        MainWindow = mainWindow;
        mainWindow.Show();
    }

    private void OnWakeupSignaled(object? state, bool timedOut)
    {
        Dispatcher.BeginInvoke(() =>
        {
            if (MainWindow is MainWindow mainWindow)
            {
                mainWindow.RestoreAndBringToForeground();
            }
        });
    }

    protected override void OnExit(ExitEventArgs e)
    {
        if (MainWindow is MainWindow mainWindow)
        {
            mainWindow.SaveWindowAndGridConfiguration();
            mainWindow.FlushPendingLogs();
        }

        _registeredWait?.Unregister(null);
        _wakeupEvent?.Dispose();

        if (_mutex != null)
        {
            try
            {
                _mutex.ReleaseMutex();
            }
            catch { }
            _mutex.Dispose();
            _mutex = null;
        }

        base.OnExit(e);
    }
}
