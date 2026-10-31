# ![Helldivers 1 Babel](screenshots/logo.png)

[![Latest Version](https://img.shields.io/github/v/release/difinim/helldivers1-babel?logo=github&label=release)](https://github.com/difinim/helldivers1-babel/releases/latest)
[![Build Status](https://github.com/difinim/helldivers1-babel/actions/workflows/release.yml/badge.svg)](https://github.com/difinim/helldivers1-babel/actions/workflows/release.yml)
[![Tests](https://github.com/difinim/helldivers1-babel/actions/workflows/tests.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/tests.yml?query=branch%3Amain)
[![CodeQL](https://github.com/difinim/helldivers1-babel/actions/workflows/codeql.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/codeql.yml?query=branch%3Amain)
[![OSV-Scanner](https://github.com/difinim/helldivers1-babel/actions/workflows/osv-scanner.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/osv-scanner.yml?query=branch%3Amain)
[![Dependabot](https://img.shields.io/badge/dependabot-enabled-025E8C?logo=dependabot&logoColor=white)](https://github.com/difinim/helldivers1-babel/network/updates)

*并肩空降，众语归一。*

**Helldivers 1 Babel** 是专为《*Helldivers (2015)*》（地狱潜者1）打造的开源、非侵入式游戏内聊天翻译与战术通信助手——*当大家都心领神会，甜蜜的自由听起来才更加悦耳*。

## Translations

[English](../README.md) | [العربية](README.ar.md) | [বাংলা](README.bn.md) | [简体中文](README.zh.md) | [Čeština](README.cs.md) | [Nederlands](README.nl.md) | [Français](README.fr.md) | [Deutsch](README.de.md) | [Ελληνικά](README.el.md) | [हिन्दी](README.hi.md) | [Magyar](README.hu.md) | [Bahasa Indonesia](README.id.md) | [Italiano](README.it.md) | [日本語](README.ja.md) | [한국어](README.ko.md) | [Bahasa Melayu](README.ms.md) | [فارسی](README.fa.md) | [Polski](README.pl.md) | [Português (PT-BR)](README.pt-BR.md) | [Română](README.ro.md) | [Русский](README.ru.md) | [Español](README.es.md) | [Svenska](README.sv.md) | [Tagalog (Filipino)](README.tl.md) | [ไทย](README.th.md) | [Türkçe](README.tr.md) | [Українська](README.uk.md) | [Tiếng Việt](README.vi.md)

> [!IMPORTANT]
> **兼容性声明:**
> **Helldivers 1 Babel 仅针对 PC (Steam) 平台的《Helldivers (2015)》开发。**
> **完全兼容游戏 Mod。**
> **不兼容《Helldivers 2 (2024)》**（二者底层架构完全不同且引入了内核级反作弊）。

## 目录

- [1. 概述与核心特性](#1-概述与核心特性)
- [2. 安装与配置](#2-安装与配置)
  - [运行环境要求](#运行环境要求)
  - [获取免费的 AI API 密钥](#获取免费的-ai-api-密钥)
  - [下载与安装（预编译发布版）](#下载与安装预编译发布版)
- [3. 使用指南](#3-使用指南)
  - [新手入门](#新手入门)
  - [游戏内通信与使用场景](#游戏内通信与使用场景)
- [4. 开发指南（源码编译）](#4-开发指南源码编译)
  - [开发者环境要求](#开发者环境要求)
  - [自动化环境配置](#自动化环境配置)
  - [主构建与发布打包](#主构建与发布打包)
  - [手动分步构建](#手动分步构建)
- [5. 开源许可](#5-开源许可)

## 1. 概述与核心特性

- **实时双向聊天翻译:**
  - **接收翻译:** 自动检测队友所用语言，即时翻译为您选择的目标语言（支持多达 28 种语言，包括简体中文、英语、葡萄牙语 (PT-BR)、西班牙语、俄语、日语、法语、德语等），并在游戏内聊天频道**本地渲染**，同时**忠实保留队友原名和玩家颜色代码**（P1 金色、P2 蓝色、P3 紫色、P4 青色）。
  - **发送翻译:** 输入 `[target_language]>>消息内容` 或直接输入 `>>消息内容`，即可自动翻译为上一条收到的队友消息所使用的语言。翻译后的文本将通过多人联机 RPC 广播，以便外国队友以母语阅读。
- **持久化聊天日志与文件导出:** 完整跟踪会话聊天历史，记录时间戳、玩家名称、颜色、识别语言以及翻译延迟。通过轻量级异步批量磁盘写入（大小限制可配置），在软件重启后依然完整保留表格日志，并支持一键导出为 `.txt` 文件。
- **可搜索的聊天日志:** 支持按任意列条件过滤的完整聊天记录搜索功能。

![Helldivers 1 Babel 运行效果](screenshots/screenshot.png)

- **战术快捷聊天 (10 个自定义槽位):** 包含 10 个可自由配置的预设战术指令槽位，每个槽位均可绑定自定义快捷键（如小键盘 0-9 或组合键），无需打开输入框即可一键向多人联机聊天频道广播战术呼叫。
- **房主自动欢迎消息:** 当您担任主机且有客队玩家加入房间时，自动向聊天频道广播一条自定义欢迎消息。

![快捷聊天](screenshots/quick-chat.png)

- **屏幕战术快捷指令浮层:** 按下 `Numpad +`（或您设置的快捷键）可随时在屏幕右侧切换非抢占焦点的悬浮参考浮层，在游戏战斗中快速查看当前所有可用的快捷战术指令。*（需要《Helldivers》运行在无边框窗口或窗口化模式下）*。

![快捷聊天浮层](screenshots/overlay.png)

- **多语言 UI 支持:** 支持 28 种界面语言。
- **提供商适配器架构:** 采用模块化 AI 架构设计。内置针对 **Google Gemini**（`gemini-flash-lite-latest` / `gemini-2.0-flash-lite`）深度优化的零思考延迟适配器，并具备支持 OpenAI、Claude、DeepSeek 的可扩展接口契约（即将推出）。
- **翻译风格设置:** 休闲模式（采用游戏行话与玩家战术黑话）/ 标准模式（自然对话语气，无游戏行话）。
- **全局暂停 / 恢复快捷键:** 随时按下 `Ctrl + F10`（或自定义快捷键）即可在**活动中**与**已暂停**状态间切换。暂停时将干净利落地挂起聊天翻译拦截与快捷聊天广播。
- **独立功能快捷键:**
  - **开关翻译快捷键:** 按下 `Ctrl + F11`（或自定义快捷键）可仅暂停或恢复聊天翻译（不影响快捷聊天，并伴随屏幕弹窗提示）。
  - **开关快捷聊天快捷键:** 按下 `Ctrl + F12`（或自定义快捷键）可仅暂停或恢复快捷聊天广播（不影响翻译，并伴随屏幕弹窗提示）。

![设置](screenshots/settings.png)

- **安全凭据存储 (DPAPI):** AI 提供商 API 密钥等敏感信息均使用 Windows 数据保护 API（DPAPI，`CurrentUser` 范围）加密存储于本地磁盘。凭据绝不会以明文形式出现在配置文件中，且只能在您本地电脑上由您的 Windows 账户解密。
- **零遥测与完全隐私保护:** 本工具绝不收集、记录或发送任何用户数据或遥测数据。所有翻译请求仅使用您的个人 API 凭据直接且专属地与您所配置的 AI 提供商终结点通信。
- **100% 非侵入式（绝不修改游戏文件）:** 不解包、不修改任何游戏资源或二进制文件，您的游戏客户端保持 100% 官方原版完整性。

## 2. 安装与配置

### 运行环境要求
预编译独立版本 (`HD1-Babel.exe`):
- **操作系统:** Windows 10 / 11 64 位 (x64)。
- **桌面运行时:** [Microsoft .NET 10 Desktop Runtime (x64)](https://aka.ms/dotnet/10.0/windowsdesktop-runtime-win-x64.exe) (官方直接安装包)。
- **目标游戏:** Steam 版《*Helldivers*》(2015)（64 位 `helldivers.exe`）。

### 获取免费的 AI API 密钥
- **AI 提供商凭据:** Google Gemini API 密钥（可从 [Google AI Studio](https://aistudio.google.com/) 免费申请个人密钥，[参考教程视频](https://www.youtube.com/watch?v=YHFMrZD-_3g)）。
- **网络连接:** 翻译 API 请求所必需。

### 下载与安装（预编译发布版）

使用 Babel **无需**安装开发工具或从源码编译（[除非您希望自行构建](#4-开发指南源码编译)）：
1. 访问 GitHub 项目页面右侧的 **[Releases](https://github.com/difinim/helldivers1-babel/releases)** 发布区。
2. 展开最新版本的 **Assets** 附件列表。
3. 下载独立分发压缩包：**`Helldivers1Babel-v<version>-win-x64.zip`**。
4. 解压到电脑任意文件夹（例如 `C:\GamesTools\Helldivers1Babel`）。
5. 打开文件夹并运行 **`HD1-Babel.exe`**。

> [!NOTE]
> **先决条件:** 终端玩家仅需安装免费的 [Microsoft .NET 10 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/10.0)。运行发布版本无需安装 C++ 编译器、SDK 或任何构建工具链。

> [!NOTE]
> **杀毒软件提示:** 由于 InjectorService.cs 使用了标准的 Windows 进程注入 API（`VirtualAllocEx` + `WriteProcessMemory` + `CreateRemoteThread`），Windows Defender 或第三方杀毒软件**可能会**触发启发式安全误报（如 Generic HackTool 等）。这属于游戏注入类辅助工具的正常现象，添加白名单（信任项）即可安全运行。

## 3. 使用指南

### 新手入门
1. 启动 **Helldivers 1 Babel** (`HD1-Babel.exe`)。
2. 首次启动时，切换至**设置**选项卡：
   - 输入您的 **Google Gemini API 密钥**（通过 Windows DPAPI 加密安全保存）。
   - 选择您的目标语言（例如 `简体中文`）。
   - 点击**测试 API 连接**以验证密钥有效性与网络延迟。
   - 根据喜好自定义**暂停/恢复快捷键**（默认快捷键为 `Ctrl + F10`）。
3. 在**快捷聊天**选项卡中配置您的战术指令（例如将小键盘 0-9 绑定至常用战术呼叫）。
4. 从 Steam 启动 **Helldivers**。Babel 会自动捕获 `helldivers.exe`。

### 游戏内通信与使用场景

#### 1. 自动接收翻译
队友发送的消息将被自动检测并即时翻译为您设定的目标语言，在游戏聊天框中以原玩家名称和对应颜色显示：
- 队友 (英语): `so quiet`
- 游戏内聊天框: `Diveheller: (English): 太安静了`

#### 2. 自动回复最近一条接收语言 (`>>消息内容`)
在聊天消息前加上 `>>` 前缀，会自动将您的输入翻译为上一条收到的队友消息所使用的语言：
- 您的输入: `>>是的，有些反常`
- 队友看到的内容: `yes, it is too quiet`

#### 3. 显式 / 任意指定语言发送 (`目标语言>>消息内容`)
输入特定语言名称加 `>>` 前缀，可随时按需指定目标翻译语言：

| 聊天输入指令 | 目标语言 | 广播给队友的翻译效果示例 |
| :--- | :--- | :--- |
| `russian>>bom dia!` | 俄语 | `доброе утро!` |
| `english>>minas ao norte` | 英语 | `mines to the north` |
| `japanese>>cuidado com os tiros!` | 日语 | `射線に注意せよ！` |
| `spanish>>preciso de munição` | 西班牙语 | `necesito munición` |
| `german>>recuem para o ponto de extração` | 德语 | `Rückzug zum Extraktionspunkt` |
| `french>>vamos defender aqui` | 法语 | `défendons cette position` |

#### 4. 常规发送消息与自我翻译抑制
不带 `>>` 输入普通聊天消息时（例如 `need ammo`, `vamos lá`），消息将通过原生多人联机聊天通道直接广播。当勾选**“不翻译常规发送消息”**（在快捷控制栏与“设置”中均默认开启），Babel 将自动抑制对您自己发送的普通消息的二次翻译回显。若取消勾选，Babel 也会将您本地发送的普通消息纳入接收翻译流水线中。

#### 5. 战术快捷聊天按键
游戏中按下任意配置的快捷键（例如 `NumPad 1`-`NumPad 9`），即可无需唤起聊天输入框直接广播预设指令。所有按键事件均无缝穿透至游戏，完全不影响您的战斗操作。

#### 6. 屏幕快捷指令参考浮层 (`Numpad +`)
随时按下 `Numpad +`（或在**快捷聊天**中自定义的快捷键）即可开关屏幕右侧的战术指令速查浮层。浮层靠右垂直居中显示，按照 `[<快捷键>] <消息内容>` 格式展示所有已激活的指令（文本超出 30 字符自动省略），并采用与弹窗通知一致的轻量无侵入视觉风格。
> [!NOTE]
> **显示模式要求:** 必须在游戏内“视频”设置中将《Helldivers》设置为**无边框窗口模式**（或窗口化），Windows 浮层才能置顶显示在游戏画面上方。

#### 7. 暂停、恢复与功能独立开关
- **全局暂停 / 恢复 (`Ctrl + F10`):** 总开关，彻底暂停或恢复翻译拦截与快捷聊天广播。
- **开关翻译快捷键 (`Ctrl + F11`):** 仅暂停或恢复聊天翻译功能，快捷聊天依然正常工作，并伴有屏幕通知。
- **开关快捷聊天快捷键 (`Ctrl + F12`):** 仅暂停或恢复快捷聊天功能，聊天翻译依然正常工作，并伴有屏幕通知。
*(所有按键均可在**设置**与**快捷聊天**选项卡中点击按键录制按钮自由修改)。*

#### 8. 日志导出与持久化记录
在“翻译与日志”选项卡中随时点击**导出日志**，即可选择路径将完整聊天记录（包含时间戳、玩家名称、原始消息、翻译内容与延迟）导出为 `.txt` 文本文件。聊天消息利用轻量异步批处理磁盘写入持久化保存（磁盘上限可在设置中设为 0 至 100 MB，并配有**清空日志缓存**按钮）。

#### 9. 交互式消息链接
队伍聊天中包含 `http://` 或 `https://` 的网址链接将在**原始消息**列渲染为可点击的超链接。点击后会弹出确认对话框提示您确认安全性，随后在系统默认浏览器中打开。

## 4. 开发指南（源码编译）

### 开发者环境要求
从源码编译构建本项目：
- **操作系统:** Windows 10 / 11 64 位 (x64)。
- **.NET 10 SDK:** [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (`net10.0-windows`)。
- **MSYS2 MinGW64 工具链 (Clang++, GCC, Make):** [MSYS2](https://www.msys2.org/) 并安装 `mingw-w64-x86_64-clang`、`mingw-w64-x86_64-gcc` 和 `mingw-w64-x86_64-make` 软件包——用于编译原生 Hook 动态链接库 (`BabelHook.dll`) 以及构建官方 LuaJIT 2.0 子模块 (`vendor/luajit`)，以便将游戏内引导脚本 (`babel_bootstrap.lua`) 编译为字节码。
- **Git:** Windows 版 Git（包含 MinHook 聚合代码库与 LuaJIT 2.0 的递归子模块）。

### 自动化环境配置
双击仓库根目录下的 `setup.bat`（或在 PowerShell 中执行 `setup.ps1`）。脚本将自动检测并补齐缺失的依赖项：
1. 验证 64 位 Windows 环境。
2. 检查 .NET 10 SDK（缺失时通过 `winget` 自动安装）。
3. 检查 MSYS2 MinGW64 工具链（Clang++、GCC、Make）。如缺失，通过 `winget` 安装 MSYS2 并通过 `pacman` 安装所需软件包（`mingw-w64-x86_64-clang`、`mingw-w64-x86_64-gcc`、`mingw-w64-x86_64-make`）。
4. 初始化并构建子模块 (`vendor/luajit`) 中的 LuaJIT 2.0 编译器。
5. 自动确保 `C:\msys64\mingw64\bin` 和 `C:\msys64\usr\bin` 已加入系统 PATH。
6. 验证 MinHook 依赖项完整性并执行初始构建健全性检查。

### 主构建与发布打包
清理旧构建产物、编译原生 Hook DLL、运行全部单元测试并打包生成发布目录：
```powershell
pwsh -File build.ps1
```
输出目录将被打包至 `dist/Helldivers1Babel-vX.Y.Z/`。

### 手动分步构建
```powershell
# 1. 构建原生 Hook DLL（自动调用 luajit 编译 babel_bootstrap.lua）
pwsh -File src/Babel.Hook/build.ps1

# 2. 构建 .NET 10 解决方案
dotnet build Helldivers1Babel.slnx

# 3. 运行自动化测试套件
dotnet test
```

## 5. 开源许可

在 [MIT 许可协议](../LICENSE) 下开源发布。
