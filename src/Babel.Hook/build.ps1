param(
    [string]$Configuration = "Release",
    [string]$OutDir = "$PSScriptRoot\bin\$Configuration"
)

$ErrorActionPreference = "Stop"

# Detect clang++ compiler
$clangPath = $null
if (Test-Path "C:\msys64\mingw64\bin\clang++.exe") {
    $clangPath = "C:\msys64\mingw64\bin\clang++.exe"
} elseif (Get-Command clang++ -ErrorAction SilentlyContinue) {
    $clangPath = (Get-Command clang++).Source
} elseif (Test-Path "C:\Program Files\LLVM\bin\clang++.exe") {
    $clangPath = "C:\Program Files\LLVM\bin\clang++.exe"
}

if (-not $clangPath) {
    Write-Error "Clang++ compiler not found. Please install MSYS2 or run setup.bat."
    exit 1
}

Write-Host "[Babel.Hook] Using compiler: $clangPath" -ForegroundColor Cyan

if (-not (Test-Path $OutDir)) {
    New-Item -ItemType Directory -Path $OutDir -Force | Out-Null
}

# Recompile Lua bootstrap bytecode dynamically from source using LuaJIT 2.0
$bootstrapLua = Join-Path $PSScriptRoot "src\babel_bootstrap.lua"
$bootstrapHeader = Join-Path $PSScriptRoot "src\babel_bootstrap_bytecode.h"
$bootstrapLuajit = Join-Path $PSScriptRoot "src\babel_bootstrap.luajit"
$repoRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$vendorLuajitDir = Join-Path $repoRoot "vendor\luajit"
$vendorLuajitSrc = Join-Path $vendorLuajitDir "src"
$vendorLuajitExe = Join-Path $vendorLuajitSrc "luajit.exe"

# 1. Resolve or Build LuaJIT 2.0 from submodule
$luaCompiler = $null

if (-not (Test-Path $vendorLuajitExe)) {
    # If submodule directory is empty, initialize it
    if (-not (Test-Path (Join-Path $vendorLuajitSrc "Makefile"))) {
        Write-Host "[Babel.Hook] Initializing vendor/luajit submodule..." -ForegroundColor Cyan
        git -C $repoRoot submodule update --init --recursive vendor/luajit
    }

    if (Test-Path (Join-Path $vendorLuajitSrc "Makefile")) {
        # Locate GNU make / mingw32-make
        $makeCmd = $null
        if (Test-Path "C:\msys64\mingw64\bin\mingw32-make.exe") {
            $makeCmd = "C:\msys64\mingw64\bin\mingw32-make.exe"
        } elseif (Get-Command mingw32-make -ErrorAction SilentlyContinue) {
            $makeCmd = (Get-Command mingw32-make).Source
        } elseif (Get-Command make -ErrorAction SilentlyContinue) {
            $makeCmd = (Get-Command make).Source
        }

        if ($makeCmd) {
            Write-Host "[Babel.Hook] Compiling LuaJIT 2.0 from vendor/luajit source ($makeCmd)..." -ForegroundColor Cyan
            & $makeCmd -C $vendorLuajitSrc -j4
            if ($LASTEXITCODE -ne 0) {
                Write-Error "[Babel.Hook] Failed to build LuaJIT 2.0 in $vendorLuajitSrc"
            }
        }
    }
}

if (Test-Path $vendorLuajitExe) {
    $luaCompiler = $vendorLuajitExe
    Write-Host "[Babel.Hook] Using LuaJIT 2.0 from submodule: $luaCompiler" -ForegroundColor Cyan
} elseif (Test-Path "C:\cygwin64\bin\luajit.exe") {
    $ver = & "C:\cygwin64\bin\luajit.exe" -v 2>&1
    if ($ver -match "LuaJIT 2\.0") {
        $luaCompiler = "C:\cygwin64\bin\luajit.exe"
        Write-Host "[Babel.Hook] Using fallback LuaJIT 2.0 compiler: $luaCompiler" -ForegroundColor Cyan
    }
}

if (-not $luaCompiler) {
    Write-Error "Cannot build BabelHook.dll: LuaJIT 2.0 compiler could not be built or found. Please ensure MinGW make (mingw-w64-x86_64-make) is installed."
    exit 1
}

# 2. Compile babel_bootstrap.lua to bytecode header
if (Test-Path $bootstrapLua) {
    Write-Host "[Babel.Hook] Compiling babel_bootstrap.lua with LuaJIT 2.0 ($luaCompiler)..." -ForegroundColor Cyan
    $tempLuajit = [System.IO.Path]::GetTempFileName()
    $prevLuaPath = $env:LUA_PATH
    try {
        # Configure LUA_PATH to include jit module libraries if using vendor compiler
        if (Test-Path $vendorLuajitSrc) {
            $jitPath = (Join-Path $vendorLuajitSrc "?.lua") + ";" + (Join-Path $vendorLuajitSrc "?\init.lua") + ";;"
            $env:LUA_PATH = if ($prevLuaPath) { "$jitPath;$prevLuaPath" } else { $jitPath }
        }

        & $luaCompiler -b $bootstrapLua $tempLuajit
        if ($LASTEXITCODE -eq 0 -and (Test-Path $tempLuajit)) {
            $bytes = [System.IO.File]::ReadAllBytes($tempLuajit)

            # Enforce LuaJIT 2.0 magic bytes (0x1b 0x4c 0x4a 0x01)
            if ($bytes.Length -lt 4 -or $bytes[0] -ne 0x1b -or $bytes[1] -ne 0x4c -or $bytes[2] -ne 0x4a -or $bytes[3] -ne 0x01) {
                Write-Error ("[Babel.Hook] Incompatible bytecode! Version is 0x{0:x2}, expected 0x01 (LuaJIT 2.0). Helldivers 1 will reject this bytecode!" -f $bytes[3])
                exit 1
            }

            $sb = [System.Text.StringBuilder]::new()
            [void]$sb.AppendLine("// Auto-generated from babel_bootstrap.lua via LuaJIT 2.0")
            [void]$sb.AppendLine("// DO NOT EDIT DIRECTLY - Modify babel_bootstrap.lua instead.")
            [void]$sb.AppendLine("#pragma once")
            [void]$sb.AppendLine("#include <cstddef>")
            [void]$sb.AppendLine("")
            [void]$sb.AppendLine("static const unsigned char g_babel_bootstrap_bytecode[$($bytes.Length)] = {")
            for ($i = 0; $i -lt $bytes.Length; $i += 16) {
                $count = [Math]::Min(16, $bytes.Length - $i)
                $chunk = $bytes[$i..($i + $count - 1)] | ForEach-Object { "0x{0:x2}" -f $_ }
                $line = "    " + ($chunk -join ", ")
                if (($i + $count) -lt $bytes.Length) { $line += "," }
                [void]$sb.AppendLine($line)
            }
            [void]$sb.AppendLine("};")
            [void]$sb.AppendLine("static const size_t g_babel_bootstrap_bytecode_size = $($bytes.Length);")
            [System.IO.File]::WriteAllText($bootstrapHeader, $sb.ToString())
            Write-Host "[Babel.Hook] Generated $bootstrapHeader ($($bytes.Length) bytes, LuaJIT 2.0 verified)" -ForegroundColor Green
        } else {
            Write-Error "Failed to compile $bootstrapLua into bytecode!"
            exit 1
        }
    } finally {
        $env:LUA_PATH = $prevLuaPath
        if (Test-Path $tempLuajit) {
            Remove-Item $tempLuajit -Force -ErrorAction SilentlyContinue
        }
        if (Test-Path $bootstrapLuajit) {
            Remove-Item $bootstrapLuajit -Force -ErrorAction SilentlyContinue
        }
    }
}

$outputDll = Join-Path $OutDir "BabelHook.dll"

# Compile all .cpp sources in src/
$cppSources = Get-ChildItem -Path "$PSScriptRoot\src\*.cpp" | ForEach-Object { $_.FullName }

$sources = @()
$sources += $cppSources
$sources += @(
    "$PSScriptRoot\minhook\src\buffer.c",
    "$PSScriptRoot\minhook\src\hook.c",
    "$PSScriptRoot\minhook\src\trampoline.c",
    "$PSScriptRoot\minhook\src\hde\hde64.c"
)

$includeDirs = @(
    "-I$PSScriptRoot\include",
    "-I$PSScriptRoot\src",
    "-I$PSScriptRoot\minhook\include",
    "-I$PSScriptRoot\minhook\src"
)

$cflags = @(
    "-target", "x86_64-w64-windows-gnu",
    "-shared",
    "-static",
    "-std=c++20",
    "-Wall",
    "-Wno-deprecated",
    "-lkernel32",
    "-luser32"
)

if ($Configuration -eq "Release") {
    $cflags += @("-O2", "-s")
} else {
    $cflags += @("-O0", "-g")
}

$cmdArgs = @()
$cmdArgs += $cflags
$cmdArgs += $includeDirs
$cmdArgs += $sources
$cmdArgs += @("-o", $outputDll)

Write-Host "[Babel.Hook] Compiling $outputDll..." -ForegroundColor Green
& $clangPath @cmdArgs

if ($LASTEXITCODE -eq 0) {
    Write-Host "[Babel.Hook] Build successful -> $outputDll" -ForegroundColor Green
} else {
    Write-Error "[Babel.Hook] Build failed with exit code $LASTEXITCODE"
}
