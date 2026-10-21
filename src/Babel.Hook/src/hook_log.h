#pragma once
#include <windows.h>
#include <cstdio>
#include <cstdarg>

inline void hook_log(const char* fmt, ...) {
    char appData[MAX_PATH];
    if (GetEnvironmentVariableA("APPDATA", appData, sizeof(appData)) == 0) return;
    char folder[MAX_PATH];
    snprintf(folder, sizeof(folder), "%s\\Helldivers1Babel", appData);
    CreateDirectoryA(folder, NULL);

    char path[MAX_PATH];
    snprintf(path, sizeof(path), "%s\\Helldivers1Babel\\hook.log", appData);

    FILE* f = fopen(path, "a");
    if (!f) return;

    SYSTEMTIME st;
    GetLocalTime(&st);
    fprintf(f, "[%02d:%02d:%02d.%03d] ", st.wHour, st.wMinute, st.wSecond, st.wMilliseconds);

    va_list args;
    va_start(args, fmt);
    vfprintf(f, fmt, args);
    va_end(args);
    fprintf(f, "\n");
    fclose(f);
}
