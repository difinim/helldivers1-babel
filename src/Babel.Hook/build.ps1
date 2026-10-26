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

# Recompile Lua bootstrap bytecode if a Lua compiler is present and lua script is newer (or header is missing)
$bootstrapLua = Join-Path $PSScriptRoot "src\babel_bootstrap.lua"
$bootstrapHeader = Join-Path $PSScriptRoot "src\babel_bootstrap_bytecode.h"
$bootstrapLuajit = Join-Path $PSScriptRoot "src\babel_bootstrap.luajit"

# Detect Lua compiler (LuaJIT)
# IMPORTANT: Helldivers 1 (2015) embeds LuaJIT 2.0 (bytecode version 1). LuaJIT 2.1 produces bytecode version 2,
# which Helldivers' luaL_loadbuffer rejects with: "cannot load incompatible bytecode".
# We must find and use LuaJIT 2.0 specifically.
$luaCompiler = $null
$candidateCompilers = @(
    "C:\cygwin64\bin\luajit.exe",
    "C:\cygwin64\home\Diego\Projects\bitsquid-toolkit\tools\bin\luajit.exe"
)
if (Get-Command luajit -ErrorAction SilentlyContinue) {
    $candidateCompilers += (Get-Command luajit).Source
}
$candidateCompilers += "C:\msys64\mingw64\bin\luajit.exe"

foreach ($cand in $candidateCompilers) {
    if (Test-Path $cand) {
        $ver = & $cand -v 2>&1
        if ($ver -match "LuaJIT 2\.0") {
            $luaCompiler = $cand
            break
        }
    }
}

if (-not $luaCompiler) {
    foreach ($cand in $candidateCompilers) {
        if (Test-Path $cand) { $luaCompiler = $cand; break }
    }
}

if ($luaCompiler -and (Test-Path $bootstrapLua)) {
    if ((-not (Test-Path $bootstrapHeader)) -or ((Get-Item $bootstrapLua).LastWriteTime -gt (Get-Item $bootstrapHeader).LastWriteTime)) {
        Write-Host "[Babel.Hook] Compiling babel_bootstrap.lua with LuaJIT ($luaCompiler)..." -ForegroundColor Cyan
        $tempLuajit = [System.IO.Path]::GetTempFileName()
        try {
            & $luaCompiler -b $bootstrapLua $tempLuajit
            if ($LASTEXITCODE -eq 0 -and (Test-Path $tempLuajit)) {
                $bytes = [System.IO.File]::ReadAllBytes($tempLuajit)
                if ($bytes.Length -ge 4 -and ($bytes[0] -ne 0x1b -or $bytes[1] -ne 0x4c -or $bytes[2] -ne 0x4a -or $bytes[3] -ne 0x01)) {
                    Write-Error "[Babel.Hook] Incompatible bytecode detected! Header version is 0x{0:x2}, expected 0x01 (LuaJIT 2.0). Helldivers 1 will reject this bytecode!" -f $bytes[3]
                    exit 1
                }
                $sb = [System.Text.StringBuilder]::new()
                [void]$sb.AppendLine("// Auto-generated from babel_bootstrap.lua")
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
                Write-Host "[Babel.Hook] Generated $bootstrapHeader ($($bytes.Length) bytes)" -ForegroundColor Green
            } else {
                Write-Error "Failed to compile $bootstrapLua into bytecode!"
                exit 1
            }
        } finally {
            if (Test-Path $tempLuajit) {
                Remove-Item $tempLuajit -Force -ErrorAction SilentlyContinue
            }
            if (Test-Path $bootstrapLuajit) {
                Remove-Item $bootstrapLuajit -Force -ErrorAction SilentlyContinue
            }
        }
    }
} elseif (-not (Test-Path $bootstrapHeader)) {
    Write-Error "Cannot build BabelHook.dll: $bootstrapHeader is missing and LuaJIT compiler was not found. Please install LuaJIT or run setup.ps1."
    exit 1
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
