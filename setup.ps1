# setup.ps1 - Automated One-Click Windows Setup & Dependency Installer
# Requires 64-bit Windows 10/11

$ErrorActionPreference = "Continue"

Write-Host "=======================================================" -ForegroundColor Cyan
Write-Host "  Helldivers 1 Babel - Dependency & Environment Setup  " -ForegroundColor Cyan
Write-Host "=======================================================" -ForegroundColor Cyan
Write-Host ""

# 1. Check OS Architecture
$is64Bit = [Environment]::Is64BitOperatingSystem
if (-not $is64Bit) {
    Write-Host "[ERROR] Helldivers 1 Babel requires a 64-bit Windows operating system." -ForegroundColor Red
    exit 1
}
Write-Host "[OK] 64-bit Windows detected ($([System.Environment]::OSVersion.VersionString))." -ForegroundColor Green

# 2. Check .NET 10 SDK
Write-Host "Checking .NET 10 SDK..." -ForegroundColor Yellow
$hasDotNet10 = $false
try {
    $sdks = dotnet --list-sdks 2>$null
    if ($sdks -match "10\.\d+\.\d+") {
        $hasDotNet10 = $true
        Write-Host "[OK] .NET 10 SDK is installed: $($Matches[0])" -ForegroundColor Green
    }
} catch {}

if (-not $hasDotNet10) {
    Write-Host "[WARN] .NET 10 SDK not found. Attempting install via winget..." -ForegroundColor Yellow
    if (Get-Command winget -ErrorAction SilentlyContinue) {
        & winget install Microsoft.DotNet.SDK.10 --silent --accept-source-agreements --accept-package-agreements
        # Refresh PATH
        $env:Path = [System.Environment]::GetEnvironmentVariable("Path","Machine") + ";" + [System.Environment]::GetEnvironmentVariable("Path","User")
        $sdks = dotnet --list-sdks 2>$null
        if ($sdks -match "10\.\d+\.\d+") {
            Write-Host "[OK] .NET 10 SDK installed successfully!" -ForegroundColor Green
        } else {
            Write-Host "[ERROR] .NET 10 SDK installation could not be verified automatically." -ForegroundColor Red
            Write-Host "Please download and install it manually from: https://dotnet.microsoft.com/download/dotnet/10.0" -ForegroundColor Cyan
        }
    } else {
        Write-Host "[ERROR] winget is not available on this system." -ForegroundColor Red
        Write-Host "Please download and install .NET 10 SDK manually: https://dotnet.microsoft.com/download/dotnet/10.0" -ForegroundColor Cyan
    }
}

# 3. Check C++ Compiler and LuaJIT Toolchain (MSYS2 MinGW64)
Write-Host "Checking MSYS2 MinGW64 Toolchain (Clang++, GCC, LuaJIT)..." -ForegroundColor Yellow

$msysClang = "C:\msys64\mingw64\bin\clang++.exe"
$msysLua = "C:\msys64\mingw64\bin\luajit.exe"
$msysPacman = "C:\msys64\usr\bin\pacman.exe"

$hasToolchain = (Test-Path $msysClang) -and (Test-Path $msysLua)

if (-not $hasToolchain) {
    if (-not (Test-Path $msysPacman)) {
        Write-Host "[WARN] MSYS2 not found. Installing MSYS2 via winget..." -ForegroundColor Yellow
        if (Get-Command winget -ErrorAction SilentlyContinue) {
            & winget install MSYS2.MSYS2 --silent --accept-source-agreements --accept-package-agreements
        } else {
            Write-Host "[ERROR] winget is not available on this system." -ForegroundColor Red
            Write-Host "Please download and install MSYS2 manually: https://www.msys2.org/" -ForegroundColor Cyan
            exit 1
        }
    }

    if (Test-Path $msysPacman) {
        Write-Host "Installing GitHub Actions toolchain packages via pacman..." -ForegroundColor Cyan
        Write-Host "  Packages: mingw-w64-x86_64-clang mingw-w64-x86_64-gcc mingw-w64-x86_64-luajit" -ForegroundColor DarkGray
        & $msysPacman -S --noconfirm --needed mingw-w64-x86_64-clang mingw-w64-x86_64-gcc mingw-w64-x86_64-luajit
    }
}

# Verify and ensure PATH includes MSYS2 MinGW64
if (Test-Path $msysClang) {
    Write-Host "[OK] MSYS2 Clang++ detected: $msysClang" -ForegroundColor Green
    
    $mingwBin = "C:\msys64\mingw64\bin"
    $msysUsrBin = "C:\msys64\usr\bin"
    if ($env:Path -notlike "*$mingwBin*") {
        $env:Path = "$mingwBin;$msysUsrBin;" + $env:Path
    }
    
    $userPath = [System.Environment]::GetEnvironmentVariable("Path", "User")
    if ($userPath -notlike "*$mingwBin*") {
        [System.Environment]::SetEnvironmentVariable("Path", "$userPath;$mingwBin;$msysUsrBin", "User")
        Write-Host "[OK] Added $mingwBin to User PATH." -ForegroundColor Green
    }
} else {
    Write-Host "[ERROR] MSYS2 Clang++ could not be found at $msysClang." -ForegroundColor Red
}

if (Test-Path $msysLua) {
    Write-Host "[OK] MSYS2 LuaJIT detected: $msysLua" -ForegroundColor Green
} elseif (Get-Command luajit -ErrorAction SilentlyContinue) {
    Write-Host "[OK] LuaJIT compiler detected: $( (Get-Command luajit).Source )" -ForegroundColor Green
} else {
    Write-Host "[WARN] LuaJIT compiler not found." -ForegroundColor Yellow
}

# 5. Verify MinHook Amalgamation
Write-Host "Verifying MinHook headers and source..." -ForegroundColor Yellow
$minhookHeader = Join-Path $PSScriptRoot "src\Babel.Hook\minhook\include\MinHook.h"
if (Test-Path $minhookHeader) {
    Write-Host "[OK] MinHook library is present in src/Babel.Hook/minhook/" -ForegroundColor Green
} else {
    Write-Host "[WARN] MinHook header not found. Run setup again or ensure git submodules are checked out." -ForegroundColor Yellow
}

# 6. Sanity Check Build
Write-Host ""
Write-Host "Running initial environment sanity build..." -ForegroundColor Yellow
try {
    dotnet build "$PSScriptRoot\Helldivers1Babel.slnx" -c Debug --nologo -v q
    Write-Host "[OK] .NET solution build sanity check succeeded!" -ForegroundColor Green
} catch {
    Write-Host "[WARN] Initial build check had warnings or errors. Check your .NET SDK setup." -ForegroundColor Red
}

Write-Host ""
Write-Host "=======================================================" -ForegroundColor Cyan
Write-Host "  Setup completed! You can now build or launch Babel.  " -ForegroundColor Cyan
Write-Host "=======================================================" -ForegroundColor Cyan
