#include <windows.h>
#include "MinHook.h"
#include "ipc_client.h"
#include "hook_engine.h"
#include "hook_log.h"

static DWORD WINAPI BabelHookInitThread(LPVOID lpParam) {
    hook_log("BabelHookInitThread: Thread started in PID %lu. Waiting for game stabilization...", GetCurrentProcessId());
    Sleep(200);

    MH_STATUS s = MH_Initialize();
    hook_log("BabelHookInitThread: MH_Initialize returned %d", (int)s);

    ipc_client_start();
    hook_log("BabelHookInitThread: ipc_client_start called");

    hook_engine_install();
    hook_log("BabelHookInitThread: hook_engine_install completed");

    return 0;
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved) {
    switch (ul_reason_for_call) {
    case DLL_PROCESS_ATTACH:
        DisableThreadLibraryCalls(hModule);
        hook_log("DllMain: DLL_PROCESS_ATTACH received. Spawning initialization thread...");
        CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)BabelHookInitThread, NULL, 0, NULL);
        break;
    case DLL_PROCESS_DETACH:
        hook_log("DllMain: DLL_PROCESS_DETACH received. Cleaning up...");
        hook_engine_uninstall();
        ipc_client_stop();
        MH_Uninitialize();
        hook_log("DllMain: Cleanup complete.");
        break;
    }
    return TRUE;
}
