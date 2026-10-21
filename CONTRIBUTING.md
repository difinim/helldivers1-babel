# Contributing to Helldivers 1 Babel

Thank you for your interest in contributing to **Helldivers 1 Babel**!

This project is an open-source, non-invasive tactical translation and quick chat communication assistant for *Helldivers (2015)* on PC (Steam). We welcome community contributions, bug reports, and provider additions to help Helldivers communicate across languages throughout the galaxy.

---

## 1. Ground Rules & Project Philosophy

- **Compatibility Scope:** Strictly designed for *Helldivers* (Arrowhead Game Studios, 2015) on PC (Steam). It is **not** compatible with Helldivers 2.
- **100% Non-Invasive:** We do not modify, patch, or tamper with official game bundle archives or files on disk. Contributions must maintain this zero-file-modification guarantee.
- **Privacy & Zero Telemetry:** The tool does not collect or transmit user data or analytics. API credentials must always remain protected via Windows DPAPI and never be logged or transmitted to third-party endpoints.

---

## 2. Reporting Issues

If you encounter a bug, crash, or unexpected behavior:
1. Search existing [GitHub Issues](https://github.com/difinim/helldivers1-babel/issues) to avoid duplicates.
2. If opening a new issue, include:
   - Operating system version (e.g., Windows 10 / 11 64-bit).
   - Target language and AI provider being used (e.g. Gemini).
   - Clear steps to reproduce the issue.
   - Relevant excerpts from `%APPDATA%\Helldivers1Babel\app.log` (make sure **never** to include your API keys).

---

## 3. Development Workflow & Setup

### Prerequisites
- **Operating System:** Windows 10 / 11 64-bit (x64).
- **.NET 10 SDK:** [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (`net10.0-windows`).
- **C++ Compiler:** [LLVM Clang](https://github.com/llvm/llvm-project/releases) (recommended) or Microsoft Visual C++ (MSVC).
- **Lua Compiler:** [LuaJIT 2.0+](https://github.com/DevelopersCommunity/cmake-luajit/releases) (`luajit.exe` in PATH) for compiling `babel_bootstrap.lua` to in-game bytecode.

### Initial Setup
Run the automated environment verification script from the repository root:
```powershell
pwsh -File setup.ps1
```

### Building the Project
```powershell
# 1. Compile the native hook DLL
pwsh -File src/Babel.Hook/build.ps1

# 2. Build the .NET 10 solution
dotnet build Helldivers1Babel.slnx
```

### Running Test Suites
All unit tests must pass before submitting a pull request:
```powershell
dotnet test
```

### Standalone Release Packaging
To build a complete, ready-to-distribute release package:
```powershell
pwsh -File build.ps1
```
The output will be placed in `dist/Helldivers1Babel-v1.0.0/`.

---

## 4. Architectural Overview

The repository is modularly structured into four distinct components:
- [`src/Babel.Core/`](file:///C:/cygwin64/home/Diego/Projects/helldivers1-babel/src/Babel.Core): Core domain logic, named pipe IPC server, translation engine, quick chat shortcut dispatcher, and DPAPI encryption service.
- [`src/Babel.Providers/`](file:///C:/cygwin64/home/Diego/Projects/helldivers1-babel/src/Babel.Providers): Pluggable AI provider adapters implementing `ITranslationProvider` (e.g. Google Gemini adapter).
- [`src/Babel.Hook/`](file:///C:/cygwin64/home/Diego/Projects/helldivers1-babel/src/Babel.Hook): Lightweight native C++ DLL using MinHook for named-pipe IPC communication with the game process.
- [`src/Babel.App/`](file:///C:/cygwin64/home/Diego/Projects/helldivers1-babel/src/Babel.App): Desktop WPF user interface, tray management, and low-level keyboard pass-through hooks.

### Adding New AI Providers
To contribute a new translation provider (e.g., DeepSeek, OpenAI, Claude, or a local self-hosted endpoint):
1. Implement the `ITranslationProvider` interface under `src/Babel.Providers/`.
2. Register the provider in `ProviderFactory.cs`.
3. Add corresponding unit tests under `tests/Babel.Providers.Tests/`.

---

## 5. Coding Standards

- **Code Formatting:** Follow standard C# naming and formatting conventions.
- **Error Handling:** All external I/O, IPC operations, and network API requests must include robust error handling and timeout cancellation tokens.
- **No Hardcoded Keys or Secrets:** Never commit API keys, secrets, or personal credentials.
- **Pass-Through Integrity:** Quick chat shortcuts must preserve non-blocking in-game key pass-through.

---

## 6. Submitting a Pull Request

1. Fork the repository and create a feature branch (`git checkout -b feature/my-feature`).
2. Implement your changes following the coding standards.
3. Run `dotnet test` and ensure all test suites pass with zero errors.
4. Commit your changes with clear, descriptive commit messages.
5. Push to your fork and submit a Pull Request against the `main` branch.

---

## 7. License

By contributing to **Helldivers 1 Babel**, you agree that your contributions will be licensed under the project's [MIT License](LICENSE).
