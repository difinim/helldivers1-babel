@echo off
setlocal
pushd "%~dp0"
echo =======================================================
echo   Helldivers 1 Babel - Environment Setup Launcher
echo =======================================================
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0setup.ps1"
popd
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERROR] Setup encountered an issue. See details above.
)
pause
