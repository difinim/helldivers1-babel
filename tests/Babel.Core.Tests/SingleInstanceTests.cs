namespace Babel.Core.Tests;

using System;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

[SupportedOSPlatform("windows")]
public class SingleInstanceTests
{
    [Fact]
    public void SecondInstance_DetectsExistingMutex()
    {
        var testMutexName = $@"Local\Babel_Test_Mutex_{Guid.NewGuid():N}";

        using var mutex1 = new Mutex(true, testMutexName, out bool isFirstCreated);
        Assert.True(isFirstCreated);

        using var mutex2 = new Mutex(true, testMutexName, out bool isSecondCreated);
        Assert.False(isSecondCreated);
    }

    [Fact]
    public async Task WakeupEvent_SignalsExistingInstanceSuccessfully()
    {
        var testEventName = $@"Local\Babel_Test_Wakeup_{Guid.NewGuid():N}";

        using var wakeupEvent = new EventWaitHandle(false, EventResetMode.AutoReset, testEventName);
        var signalReceived = new TaskCompletionSource<bool>();

        var registeredWait = ThreadPool.RegisterWaitForSingleObject(
            wakeupEvent,
            (state, timedOut) =>
            {
                if (!timedOut)
                {
                    signalReceived.TrySetResult(true);
                }
            },
            null,
            5000,
            true);

        try
        {
            // Simulate second instance signaling the wakeup event
            using (var secondaryEvent = EventWaitHandle.OpenExisting(testEventName))
            {
                secondaryEvent.Set();
            }

            var result = await Task.WhenAny(signalReceived.Task, Task.Delay(2000));
            Assert.Same(signalReceived.Task, result);
            Assert.True(await signalReceived.Task);
        }
        finally
        {
            registeredWait.Unregister(null);
        }
    }
}
