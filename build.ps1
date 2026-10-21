# build.ps1 - Unified Build & Packaging Script for Helldivers 1 Babel
param(
    [string]$Configuration = "Release",
    [string]$Version = "1.0.0"
)

$ErrorActionPreference = "Stop"

$RootDir = $PSScriptRoot
$DistDir = Join-Path $RootDir "dist\Helldivers1Babel-v$Version"

Write-Host "=======================================================" -ForegroundColor Cyan
Write-Host "  Helldivers 1 Babel - Master Build & Release Packager " -ForegroundColor Cyan
Write-Host "=======================================================" -ForegroundColor Cyan
Write-Host "Configuration : $Configuration" -ForegroundColor Yellow
Write-Host "Target Version: $Version" -ForegroundColor Yellow
Write-Host "Output Folder : $DistDir" -ForegroundColor Yellow
Write-Host ""

# 1. Clean Previous Build Artifacts & Intermediate Outputs
Write-Host "[1/5] Cleaning previous build artifacts and outputs..." -ForegroundColor Cyan

$runningProcs = Get-Process -Name "HD1-Babel" -ErrorAction SilentlyContinue
if ($runningProcs) {
    Write-Host "[Info] HD1-Babel is currently running. Stopping instance to allow clean file packaging..." -ForegroundColor Yellow
    $runningProcs | Stop-Process -Force
    Start-Sleep -Milliseconds 1000
}

# Clean dist directory
if (Test-Path "$RootDir\dist") {
    Write-Host "  Removing $RootDir\dist..." -ForegroundColor DarkGray
    Remove-Item -Recurse -Force "$RootDir\dist" -ErrorAction SilentlyContinue
}

# Clean bin and obj folders across src and tests
Get-ChildItem -Path "$RootDir\src", "$RootDir\tests" -Include "bin", "obj" -Recurse -Directory -ErrorAction SilentlyContinue | ForEach-Object {
    Write-Host "  Removing $($_.FullName)..." -ForegroundColor DarkGray
    Remove-Item -Recurse -Force $_.FullName -ErrorAction SilentlyContinue
}

# Run dotnet clean
& dotnet clean "$RootDir\Helldivers1Babel.slnx" -c $Configuration --nologo -v q
Write-Host "[OK] Clean completed successfully." -ForegroundColor Green
Write-Host ""

# 2. Build Native Hook DLL (BabelHook.dll)
Write-Host "[2/5] Building BabelHook.dll (x64 Native)..." -ForegroundColor Cyan
& pwsh -File "$RootDir\src\Babel.Hook\build.ps1" -Configuration $Configuration
if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to build BabelHook.dll"
    exit 1
}

$hookDll = Join-Path $RootDir "src\Babel.Hook\bin\$Configuration\BabelHook.dll"
if (-not (Test-Path $hookDll)) {
    Write-Error "Output DLL not found at $hookDll"
    exit 1
}
Write-Host "[OK] BabelHook.dll built successfully." -ForegroundColor Green

# 3. Run All Automated Test Suites
Write-Host ""
Write-Host "[3/5] Running automated test suites..." -ForegroundColor Cyan
& dotnet test "$RootDir\Helldivers1Babel.slnx" -c $Configuration --nologo
if ($LASTEXITCODE -ne 0) {
    Write-Error "Test suite execution failed!"
    exit 1
}
Write-Host "[OK] All tests passed." -ForegroundColor Green

# 4. Publish Desktop App (WPF x64)
Write-Host ""
Write-Host "[4/5] Publishing HD1-Babel (WPF x64)..." -ForegroundColor Cyan

if (Test-Path $DistDir) {
    Remove-Item -Recurse -Force $DistDir
}
New-Item -ItemType Directory -Path $DistDir -Force | Out-Null

& dotnet publish "$RootDir\src\Babel.App\Babel.App.csproj" `
    -c $Configuration `
    -r win-x64 `
    --self-contained false `
    -o $DistDir `
    -p:PublishSingleFile=false `
    -p:Version=$Version `
    -p:AssemblyVersion=$Version `
    -p:FileVersion=$Version `
    -p:InformationalVersion="v$Version" `
    --nologo

if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to publish HD1-Babel"
    exit 1
}

# Purge debug symbols (*.pdb) from release output
Get-ChildItem -Path $DistDir -Filter "*.pdb" -Recurse | Remove-Item -Force
Write-Host "[OK] HD1-Babel published and debug symbols (*.pdb) purged." -ForegroundColor Green

# 5. Assemble Release Bundle
Write-Host ""
Write-Host "[5/5] Assembling release distribution package..." -ForegroundColor Cyan

# Copy BabelHook.dll into release root and verify
Copy-Item -Path $hookDll -Destination $DistDir -Force
$distHookDll = Join-Path $DistDir "BabelHook.dll"
if (-not (Test-Path $distHookDll)) {
    Write-Error "Failed to copy BabelHook.dll to $DistDir"
    exit 1
}
Write-Host "[OK] BabelHook.dll copied to distribution package." -ForegroundColor Green

# Copy End-User Documentation & License
$readmeTxt = Join-Path $RootDir "README.txt"
if (-not (Test-Path $readmeTxt)) {
    Write-Error "README.txt not found at $readmeTxt"
    exit 1
}
Copy-Item -Path $readmeTxt -Destination $DistDir -Force
Copy-Item -Path "$RootDir\LICENSE" -Destination (Join-Path $DistDir "LICENSE.txt") -Force

# Verify essential distribution assets
$distExe = Join-Path $DistDir "HD1-Babel.exe"
if (-not (Test-Path $distExe)) {
    Write-Error "HD1-Babel.exe not found in distribution package!"
    exit 1
}

$coreDll = Join-Path $DistDir "Babel.Core.dll"
if (-not (Test-Path $coreDll)) {
    Write-Error "Babel.Core.dll not found in distribution package!"
    exit 1
}

$providersDll = Join-Path $DistDir "Babel.Providers.dll"
if (-not (Test-Path $providersDll)) {
    Write-Error "Babel.Providers.dll not found in distribution package!"
    exit 1
}

Write-Host ""
Write-Host "=======================================================" -ForegroundColor Cyan
Write-Host "  Release Package Ready: $DistDir" -ForegroundColor Green
Write-Host "=======================================================" -ForegroundColor Cyan

Get-ChildItem -Path $DistDir | Select-Object Name, Length
