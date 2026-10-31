# ![Helldivers 1 Babel](screenshots/logo.png)

[![Latest Version](https://img.shields.io/github/v/release/difinim/helldivers1-babel?logo=github&label=release)](https://github.com/difinim/helldivers1-babel/releases/latest)
[![Build Status](https://github.com/difinim/helldivers1-babel/actions/workflows/release.yml/badge.svg)](https://github.com/difinim/helldivers1-babel/actions/workflows/release.yml)
[![Tests](https://github.com/difinim/helldivers1-babel/actions/workflows/tests.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/tests.yml?query=branch%3Amain)
[![CodeQL](https://github.com/difinim/helldivers1-babel/actions/workflows/codeql.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/codeql.yml?query=branch%3Amain)
[![OSV-Scanner](https://github.com/difinim/helldivers1-babel/actions/workflows/osv-scanner.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/osv-scanner.yml?query=branch%3Amain)
[![Dependabot](https://img.shields.io/badge/dependabot-enabled-025E8C?logo=dependabot&logoColor=white)](https://github.com/difinim/helldivers1-babel/network/updates)

*함께 강하하라. 하나 되어 소통하라.*

**Helldivers 1 Babel**은 *Helldivers (2015)*를 위한 오픈 소스 비침습적 채팅 번역 및 전술 통신 어시스턴트입니다. 모두가 작전을 이해할 때 *달콤한 자유*의 외침은 더욱 완벽해집니다.

## Translations

[English](../README.md) | [العربية](README.ar.md) | [বাংলা](README.bn.md) | [简体中文](README.zh.md) | [Čeština](README.cs.md) | [Nederlands](README.nl.md) | [Français](README.fr.md) | [Deutsch](README.de.md) | [Ελληνικά](README.el.md) | [हिन्दी](README.hi.md) | [Magyar](README.hu.md) | [Bahasa Indonesia](README.id.md) | [Italiano](README.it.md) | [日本語](README.ja.md) | [한국어](README.ko.md) | [Bahasa Melayu](README.ms.md) | [فارسی](README.fa.md) | [Polski](README.pl.md) | [Português (PT-BR)](README.pt-BR.md) | [Română](README.ro.md) | [Русский](README.ru.md) | [Español](README.es.md) | [Svenska](README.sv.md) | [Tagalog (Filipino)](README.tl.md) | [ไทย](README.th.md) | [Türkçe](README.tr.md) | [Українська](README.uk.md) | [Tiếng Việt](README.vi.md)

> [!IMPORTANT]
> **호환성 안내:**
> **Helldivers 1 Babel은 PC(Steam) 버전의 Helldivers (2015) 전용으로 개발되었습니다.**
> **모드(Mod)와 호환됩니다.**
> **Helldivers 2 (2024)와는 호환되지 않습니다.** 엔진 구조가 완전히 다르며 커널 레벨 안티치트가 적용되어 있습니다.

## 목차

- [1. 개요 및 주요 기능](#1-개요-및-주요-기능)
- [2. 설치 및 설정](#2-설치-및-설정)
  - [시스템 요구 사양](#시스템-요구-사양)
  - [무료 AI API 키 발급](#무료-ai-api-키-발급)
  - [다운로드 및 설치 (빌드된 릴리스)](#다운로드-및-설치-빌드된-릴리스)
- [3. 사용 가이드](#3-사용-가이드)
  - [시작하기](#시작하기)
  - [게임 내 통신 및 활용 사례](#게임-내-통신-및-활용-사례)
- [4. 개발 (소스 코드 빌드)](#4-개발-소스-코드-빌드)
  - [개발자 요구 사항](#개발자-요구-사항)
  - [자동 환경 설정](#자동-환경-설정)
  - [마스터 빌드 및 릴리스 패키징](#마스터-빌드-및-릴리스-패키징)
  - [단계별 수동 빌드](#단계별-수동-빌드)
- [5. 라이선스](#5-라이선스)

## 1. 개요 및 주요 기능

- **실시간 양방향 채팅 번역:**
  - **수신 메시지:** 팀원의 언어를 자동 감지하여 선택한 대상 언어(한국어, 영어, 포르투갈어(PT-BR), 스페인어, 러시아어, 중국어, 일본어, 프랑스어, 독일어 등 최대 28개 언어 지원)로 실시간 번역하며, 원래 팀원의 플레이어 이름과 색상 코드(P1 골드, P2 블루, P3 퍼플, P4 시안)를 **그대로 유지**한 채 인게임 채팅에 **로컬로 렌더링**합니다.
  - **발신 메시지:** `[target_language]>>메시지` 또는 단순히 `>>메시지`를 입력하면 마지막으로 수신된 팀원 메시지의 언어로 자동 번역하여 전송합니다. 번역된 텍스트는 멀티플레이어 RPC를 통해 브로드캐스트되므로 외국인 팀원도 모국어로 읽을 수 있습니다.
- **영구 채팅 로그 및 파일 내보내기:** 타임스탬프, 플레이어 이름, 색상, 감지된 언어, 번역 지연 시간을 포함한 세션 전체 채팅 기록을 추적합니다. 경량 비동기 배치 디스크 쓰기(용량 설정 가능)를 통해 앱을 재시작해도 테이블 로그가 유지되며, 클릭 한 번으로 `.txt` 파일로 내보낼 수 있습니다.
- **검색 가능한 채팅 로그:** 모든 열을 필터링할 수 있는 완벽한 검색 기능 지원.

![Helldivers 1 Babel 실행 화면](screenshots/screenshot.png)

- **전술 퀵챗 (10개 커스텀 슬롯):** 사용자가 설정 가능한 단축키(예: 숫자 패드 0~9 또는 커스텀 조합)에 10개의 사용자 정의 전술 메시지 슬롯을 지정하여, 인게임 멀티플레이어 채팅창에 즉시 전술 신호를 브로드캐스트할 수 있습니다.

![퀵챗](screenshots/quick-chat.png)

- **자동 환영 퀵챗 메시지:** 로비 호스트 시 게스트 플레이어가 입장하면 사용자 지정 환영 메시지를 인게임 채팅창에 자동으로 전송합니다.
- **화면 전술 퀵챗 오버레이:** `Numpad +`(또는 지정한 단축키)를 눌러 화면 우측에 게임 포커스를 빼앗지 않는 플로팅 오버레이를 켜고 끌 수 있어 게임 내에서 활성화된 모든 전술 단축키와 메시지를 즉시 확인할 수 있습니다. *(Helldivers가 테두리 없는 창 모드 또는 창 모드로 실행 중이어야 합니다)*.

![퀵챗 오버레이](screenshots/overlay.png)

- **다국어 UI 지원:** 28개 언어 지원.
- **제공자 어댑터 아키텍처:** 모듈식 AI 제공자 인터페이스 설계. **Google Gemini**(`gemini-flash-lite-latest` / `gemini-2.0-flash-lite`)에 최적화된 씽킹 지연 없는 어댑터가 기본 내장되어 있으며, OpenAI, Claude, DeepSeek 확장이 가능하도록 설계되었습니다(추후 지원 예정).
- **번역 스타일(톤) 설정:** 캐주얼(비공식 게임 은어 및 전술 게이머 용어 사용) / 노멀(은어 없는 자연스러운 일상 대화체).
- **글로벌 일시중지 / 다시 시작 단축키:** 언제든지 `Ctrl + F10`(또는 설정한 단축키)을 눌러 어시스턴트를 **활성**과 **일시중지** 상태로 전환할 수 있습니다. 일시중지 시 인게임 채팅 번역과 퀵챗 메시지 전송이 모두 안전하게 중단됩니다.
- **개별 기능 전용 단축키:**
  - **번역만 전환:** `Ctrl + F11`(또는 설정한 단축키)을 눌러 퀵챗 기능에 영향 없이 채팅 번역만 즉시 일시중지/재개(화면 토스트 알림 제공).
  - **퀵챗만 전환:** `Ctrl + F12`(또는 설정한 단축키)을 눌러 번역 기능에 영향 없이 퀵챗 전송만 즉시 일시중지/재개(화면 토스트 알림 제공).

![설정](screenshots/settings.png)

- **안전한 자격 증명 저장소 (DPAPI):** AI API 키와 같은 민감한 설정은 Windows 데이터 보호 API(DPAPI, `CurrentUser` 범위)를 통해 암호화되어 저장됩니다. 설정 파일에 일반 텍스트로 저장되지 않으며, 사용자 본인의 Windows 계정으로만 해당 PC에서 복호화할 수 있습니다.
- **원격 측정 제로 & 완전한 개인정보 보호:** 본 도구는 어떠한 사용자 데이터나 원격 측정 데이터도 수집, 기록, 전송하지 않습니다. 모든 번역 요청은 본인의 개인 API 키를 사용하여 구성된 AI 제공자 엔드포인트와 직접 통신합니다.
- **100% 비침습적 (게임 파일 변조 제로):** 게임 아카이브를 추출하거나 수정하지 않습니다. 게임 파일은 100% 바닐라 상태로 온전하게 유지됩니다.

## 2. 설치 및 설정

### 시스템 요구 사양
빌드된 독립형 릴리스(`HD1-Babel.exe`)를 실행하는 경우:
- **운영체제:** Windows 10 / 11 64비트 (x64).
- **데스크톱 런타임:** [Microsoft .NET 10 Desktop Runtime (x64)](https://aka.ms/dotnet/10.0/windowsdesktop-runtime-win-x64.exe) (직접 설치 프로그램).
- **대상 게임:** Steam 버전 *Helldivers* (2015) (64비트 `helldivers.exe`).

### 무료 AI API 키 발급
- **AI 제공자 자격 증명:** Google Gemini API 키 ([Google AI Studio](https://aistudio.google.com/)에서 개인 무료 키 발급 가능, [튜토리얼 영상 참고](https://www.youtube.com/watch?v=YHFMrZD-_3g)).
- **인터넷 연결:** 번역 API 호출에 필수.

### 다운로드 및 설치 (빌드된 릴리스)

Babel을 사용하기 위해 개발 도구를 설치하거나 소스에서 컴파일할 필요는 없습니다([직접 빌드하려는 경우 제외](#4-개발-소스-코드-빌드)):
1. GitHub 프로젝트 페이지 우측의 **[Releases](https://github.com/difinim/helldivers1-babel/releases)** 탭으로 이동합니다.
2. 최신 릴리스 아래의 **Assets** 목록을 펼칩니다.
3. 독립형 배포 압축 파일 다운로드: **`Helldivers1Babel-v<version>-win-x64.zip`**.
4. PC의 원하는 경로(예: `C:\GamesTools\Helldivers1Babel`)에 압축을 풉니다.
5. 폴더를 열고 **`HD1-Babel.exe`**를 실행합니다.

> [!NOTE]
> **필수 구성 요소:** 일반 사용자는 무료 [Microsoft .NET 10 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/10.0)만 설치하면 됩니다. 릴리스 버전을 실행하기 위해 C++ 컴파일러, SDK 또는 빌드 툴체인이 필요하지 않습니다.

> [!NOTE]
> **보안 프로그램 주의 사항:** InjectorService.cs는 표준 Windows 프로세스 인젝션 API(`VirtualAllocEx` + `WriteProcessMemory` + `CreateRemoteThread`)를 사용하므로, Windows Defender 또는 타사 백신 프로그램이 휴리스틱 감지(예: Generic HackTool 등)를 작동시킬 수 있습니다. 이는 게임 모드 인젝터에서 흔히 나타나는 정상적인 동작이며, 안심하고 예외(화이트리스트)로 등록하여 사용하시면 됩니다.

## 3. 사용 가이드

### 시작하기
1. **Helldivers 1 Babel**(`HD1-Babel.exe`)을 실행합니다.
2. 처음 실행 시 **설정** 탭을 엽니다:
   - **Google Gemini API 키**를 입력합니다 (Windows DPAPI 암호화로 안전하게 저장됨).
   - 대상 언어를 선택합니다 (예: `한국어`).
   - **API 연결 테스트**를 클릭하여 키와 지연 시간을 확인합니다.
   - 필요한 경우 **일시중지/재개 단축키**를 변경합니다 (기본값: `Ctrl + F10`).
3. **퀵챗** 탭에서 전술 메시지를 설정합니다 (예: 숫자 패드 0~9에 원하는 문구 지정).
4. Steam에서 **Helldivers**를 실행합니다. Babel이 `helldivers.exe`를 자동 감지하고 `BabelHook.dll`을 인젝션하여 초고속 네임드 파이프 IPC를 구성합니다.

### 게임 내 통신 및 활용 사례

#### 1. 자동 수신 메시지 번역
팀원의 수신 메시지를 실시간으로 감지하여 설정한 대상 언어로 번역하고, 원래 플레이어 이름과 색상으로 인게임 채팅에 출력합니다:
- 팀원 (영어): `so quiet`
- 인게임 채팅: `Diveheller: (English): 너무 조용하네요`

#### 2. 마지막 수신 언어로 자동 답장 (`>>메시지`)
채팅 메시지 앞에 `>>`를 붙이면 마지막으로 메시지를 보낸 팀원의 언어로 텍스트가 자동 번역됩니다:
- 내가 입력한 채팅: `>>네, 폭풍전야 같네요`
- 팀원에게 표시되는 내용: `yes, it is too quiet`

#### 3. 특정 언어 직접 지정 (`지정언어>>메시지`)
언어 이름 뒤에 `>>`를 붙여 언제든지 특정 언어로 번역하여 전송할 수 있습니다:

| 채팅 입력 명령어 | 대상 언어 | 팀원에게 전송되는 번역 결과 예시 |
| :--- | :--- | :--- |
| `russian>>bom dia!` | 러시아어 | `доброе утро!` |
| `english>>minas ao norte` | 영어 | `mines to the north` |
| `japanese>>cuidado com os tiros!` | 일본어 | `射線に注意せよ！` |
| `spanish>>preciso de munição` | 스페인어 | `necesito munición` |
| `german>>recuem para o ponto de extração` | 독일어 | `Rückzug zum Extraktionspunkt` |
| `french>>vamos defender aqui` | 프랑스어 | `défendons cette position` |

#### 4. 일반 발신 메시지 및 자체 번역 억제
`>>` 없이 일반 인게임 채팅 메시지(예: `need ammo`, `vamos lá`)를 입력하면 바닐라 멀티플레이어 채팅을 통해 그대로 전송됩니다. **"일반 발신 메시지 번역 안 함"** 토글이 켜져 있으면(빠른 컨트롤 바 및 설정 탭 모두에서 기본값 활성화), Babel은 자신이 보낸 일반 메시지를 본인에게 다시 번역하지 않습니다. 끄면 로컬 일반 메시지도 수신 번역 파이프라인을 거치게 됩니다.

#### 5. 전술 퀵챗 단축키
플레이 도중 설정된 퀵챗 단축키(예: `NumPad 1`~`NumPad 9`)를 누르면 채팅창을 열지 않고도 프리셋 신호를 멀티플레이어 채팅으로 즉시 전송합니다. 모든 키 입력은 게임으로도 정상 전달되어 조작 흐름이 끊기지 않습니다.

#### 6. 전술 퀵챗 화면 오버레이 (`Numpad +`)
언제든지 `Numpad +`(또는 **퀵챗** 탭에서 지정한 단축키)를 눌러 활성화된 모든 퀵챗 목록을 보여주는 화면 오버레이를 켜고 끌 수 있습니다. 화면 우측 중앙에 정렬되어 `[<단축키>] <메시지>` 형식(메시지는 최대 30자까지 축약)으로 표시되며, 토스트 알림과 동일하게 깔끔하고 방해되지 않는 스타일로 렌더링됩니다.
> [!NOTE]
> **화면 모드 요구 사항:** 오버레이 창이 게임 화면 위에 정상적으로 표시되려면 게임 내 그래픽 설정에서 Helldivers를 **테두리 없는 창 모드(Borderless Fullscreen)** 또는 창 모드로 설정해야 합니다.

#### 7. 일시중지, 다시 시작 및 기능 전환
- **글로벌 일시중지 / 다시 시작 (`Ctrl + F10`):** 번역 가로채기와 퀵챗 전송을 모두 깔끔하게 중단/재개하는 마스터 스위치입니다.
- **번역만 전환 (`Ctrl + F11`):** 퀵챗은 유지한 채 화면 토스트 알림과 함께 채팅 번역 기능만 켜고 끕니다.
- **퀵챗만 전환 (`Ctrl + F12`):** 번역은 유지한 채 화면 토스트 알림과 함께 퀵챗 메시지 전송 기능만 켜고 끕니다.
*(모든 단축키는 **설정** 및 **퀵챗** 탭의 단축키 녹화 버튼을 통해 원하는 키로 변경할 수 있습니다).*

#### 8. 채팅 로그 내보내기 및 영구 보관
"번역 및 로그" 탭에서 언제든지 **로그 내보내기**를 클릭하여 원하는 위치에 채팅 기록(타임스탬프, 플레이어 이름, 원문, 번역문, 지연 시간 포함)을 `.txt` 파일로 저장할 수 있습니다. 채팅 메시지는 비동기 배치 디스크 저장을 통해 앱을 껐다 켜도 테이블에 자동으로 유지됩니다(저장 용량 한도는 설정에서 0~100MB 범위로 조정 가능하며 **로그 메모리 지우기** 버튼도 제공됩니다).

#### 9. 상호작용 가능한 링크 지원
`http://` 또는 `https://` URL이 포함된 스쿼드 채팅 메시지는 **원문** 열에 클릭 가능한 하이퍼링크로 표시됩니다. 클릭 시 신뢰할 수 있는 링크인지 확인하는 대화상자가 표시된 후 기본 웹 브라우저로 안전하게 열립니다.

## 4. 개발 (소스 코드 빌드)

### 개발자 요구 사항
소스에서 프로젝트를 컴파일하고 빌드하려는 경우:
- **운영체제:** Windows 10 / 11 64비트 (x64).
- **.NET 10 SDK:** [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (`net10.0-windows`).
- **MSYS2 MinGW64 툴체인 (Clang++, GCC, Make):** [MSYS2](https://www.msys2.org/) - `mingw-w64-x86_64-clang`, `mingw-w64-x86_64-gcc`, `mingw-w64-x86_64-make` 패키지 필요. 네이티브 후킹 라이브러리(`BabelHook.dll`) 컴파일 및 인게임 부트스트랩 스크립트(`babel_bootstrap.lua`)를 바이트코드로 컴파일하기 위한 공식 LuaJIT 2.0 서브모듈(`vendor/luajit`) 빌드에 사용됩니다.
- **Git:** Windows용 Git (MinHook amalgamation 및 LuaJIT 2.0 재귀 서브모듈 포함).

### 자동 환경 설정
리포지토리 루트에서 `setup.bat`을 더블 클릭(또는 PowerShell에서 `setup.ps1` 실행)하면 누락된 필수 구성 요소를 자동으로 확인하고 설치합니다:
1. 64비트 Windows 환경 확인.
2. .NET 10 SDK 확인 (누락 시 `winget`으로 자동 설치).
3. MSYS2 MinGW64 툴체인(Clang++, GCC, Make) 확인. 누락 시 `winget`을 통해 MSYS2를 자동 설치하고 `pacman`으로 필수 패키지(`mingw-w64-x86_64-clang`, `mingw-w64-x86_64-gcc`, `mingw-w64-x86_64-make`) 설치.
4. 서브모듈(`vendor/luajit`)에서 LuaJIT 2.0 컴파일러 초기화 및 빌드.
5. `C:\msys64\mingw64\bin` 및 `C:\msys64\usr\bin` 경로가 환경 변수 PATH에 등록되었는지 자동 확인.
6. MinHook amalgamation 종속성 검증 및 초기 빌드 테스트 실행.

### 마스터 빌드 및 릴리스 패키징
이전 빌드 결과물을 정리하고 네이티브 훅 DLL을 컴파일하며 모든 단위 테스트를 통과한 후 릴리스 패키지를 생성하려면:
```powershell
pwsh -File build.ps1
```
결과물은 `dist/Helldivers1Babel-vX.Y.Z/` 디렉터리에 패키징됩니다.

### 단계별 수동 빌드
```powershell
# 1. 네이티브 훅 DLL 빌드 (luajit으로 babel_bootstrap.lua 자동 컴파일)
pwsh -File src/Babel.Hook/build.ps1

# 2. .NET 10 솔루션 빌드
dotnet build Helldivers1Babel.slnx

# 3. 자동화 테스트 스위트 실행
dotnet test
```

## 5. 라이선스

[MIT 라이선스](../LICENSE)에 따라 배포됩니다.
