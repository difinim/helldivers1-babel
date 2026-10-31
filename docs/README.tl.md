# ![Helldivers 1 Babel](screenshots/logo.png)

[![Latest Version](https://img.shields.io/github/v/release/difinim/helldivers1-babel?logo=github&label=release)](https://github.com/difinim/helldivers1-babel/releases/latest)
[![Build Status](https://github.com/difinim/helldivers1-babel/actions/workflows/release.yml/badge.svg)](https://github.com/difinim/helldivers1-babel/actions/workflows/release.yml)
[![Tests](https://github.com/difinim/helldivers1-babel/actions/workflows/tests.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/tests.yml?query=branch%3Amain)
[![CodeQL](https://github.com/difinim/helldivers1-babel/actions/workflows/codeql.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/codeql.yml?query=branch%3Amain)
[![OSV-Scanner](https://github.com/difinim/helldivers1-babel/actions/workflows/osv-scanner.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/osv-scanner.yml?query=branch%3Amain)
[![Dependabot](https://img.shields.io/badge/dependabot-enabled-025E8C?logo=dependabot&logoColor=white)](https://github.com/difinim/helldivers1-babel/network/updates)

*Sumisid nang magkasama. Magsalita bilang iisa.*

Ang **Helldivers 1 Babel** ay isang open-source, non-invasive na katulong sa pagsasalin ng chat at taktikal na komunikasyon para sa *Helldivers (2015)*, dahil *mas maganda ang matamis na kalayaan kapag nauunawaan ng lahat ang plano*.

## Translations

[English](../README.md) | [العربية](README.ar.md) | [বাংলা](README.bn.md) | [简体中文](README.zh.md) | [Čeština](README.cs.md) | [Nederlands](README.nl.md) | [Français](README.fr.md) | [Deutsch](README.de.md) | [Ελληνικά](README.el.md) | [हिन्दी](README.hi.md) | [Magyar](README.hu.md) | [Bahasa Indonesia](README.id.md) | [Italiano](README.it.md) | [日本語](README.ja.md) | [한국어](README.ko.md) | [Bahasa Melayu](README.ms.md) | [فارسی](README.fa.md) | [Polski](README.pl.md) | [Português (PT-BR)](README.pt-BR.md) | [Română](README.ro.md) | [Русский](README.ru.md) | [Español](README.es.md) | [Svenska](README.sv.md) | [Tagalog (Filipino)](README.tl.md) | [ไทย](README.th.md) | [Türkçe](README.tr.md) | [Українська](README.uk.md) | [Tiếng Việt](README.vi.md)

> [!IMPORTANT]
> **Pagtanggi sa Pagkakatugma (Compatibility Disclaimer):**
> **Eksklusibong binuo ang Helldivers 1 Babel para sa Helldivers (2015) sa PC (Steam).**
> **Tugma ito sa mga mod.**
> **HINDI ito tugma sa Helldivers 2 (2024)**, na ganap na naiiba at gumagamit ng kernel-level anti-cheat.

## Talaan ng Nilalaman

- [1. Pangkalahatang-ideya at Pangunahing Tampok](#1-pangkalahatang-ideya-at-pangunahing-tampok)
- [2. Pag-set up](#2-pag-set-up)
  - [Mga Kinakailangan](#mga-kinakailangan)
  - [Mag-set up ng Libreng AI API Key](#mag-set-up-ng-libreng-ai-api-key)
  - [Pag-download at Pag-install (Pre-Built Release)](#pag-download-at-pag-install-pre-built-release)
- [3. Gabay sa Paggamit](#3-gabay-sa-paggamit)
  - [Pagsisimula](#pagsisimula)
  - [Komunikasyon sa Laro at Mga Halimbawa ng Paggamit](#komunikasyon-sa-laro-at-mga-halimbawa-ng-paggamit)
- [4. Pagbuo (Pag-compile mula sa Source)](#4-pagbuo-pag-compile-mula-sa-source)
  - [Mga Kinakailangan ng Developer](#mga-kinakailangan-ng-developer)
  - [Awtomatikong Pag-set up ng Kapaligiran](#awtomatikong-pag-set-up-ng-kapaligiran)
  - [Pangunahing Pag-build at Pag-package ng Release](#pangunahing-pag-build-at-pag-package-ng-release)
  - [Mano-manong Hakbang-hakbang na Pag-build](#mano-manong-hakbang-hakbang-na-pag-build)
- [5. Lisensya](#5-lisensya)

## 1. Pangkalahatang-ideya at Pangunahing Tampok

- **Real-Time na Dalawahang Direksyon na Pagsasalin ng Chat:**
  - **Papasok na Salin:** Awtomatikong tinutukoy ang wika ng mga kakampi, isinasalin ang mga mensahe sa iyong piniling target na wika (sumusuporta sa hanggang 28 target na wika, kabilang ang Portuguese PT-BR, Ingles, Espanyol, Ruso, Tsino, Hapon, Pranses, Aleman, at marami pa), at ipinapakita ang mga ito nang **lokal** sa chat ng laro habang **tapat na pinapanatili ang orihinal na pangalan ng manlalaro at color code** (P1 Ginto, P2 Asul, P3 Lila, P4 Cyan).
  - **Papalabas na Salin:** I-type ang `[target_na_wika]>>mensahe` o i-type lamang ang `>>mensahe` upang awtomatikong sumagot sa wika ng huling papasok na mensahe mula sa kakampi. Ang isinaling teksto ay ibinobrodkast sa pamamagitan ng multiplayer RPC upang mabasa ito ng mga internasyonal na kakampi sa kanilang sariling wika.
- **Patuloy na Chat Log at Pag-export ng File:** Buong pagsubaybay sa kasaysayan ng chat sa session na may mga timestamp, pangalan ng manlalaro, kulay, natukoy na pinagmulang wika, at latency ng pagsasalin. Nananatili ang mga log sa talahanayan kahit na i-restart ang app sa pamamagitan ng magaan at asynchronous na batch disk writes (maaaring i-configure), na may one-click na pag-export sa isang `.txt` file.
- **Naha-search na Mga Chat Log:** Ganap na naha-search na kasaysayan ng chat na nagfi-filter sa anumang column.

![Helldivers 1 Babel habang ginagamit](screenshots/screenshot.png)

- **Taktikal na Mabilisang Chat (10 Custom Slots):** 10 napapasadyang mga paunang tinukoy na slot ng mensaheng taktikal, bawat isa ay nakatali sa mga hotkey na maaaring i-configure ng user (hal. Numpad 0-9 o mga pasadyang kumbinasyon) upang agad na magpadala ng mga taktikal na callout sa chat ng multiplayer sa laro.
- **Awtomatikong Pagbati na Mensahe sa Mabilisang Chat:** Awtomatikong nagbobrodkast ng napapasadyang mensahe ng pagbati sa chat ng laro kapag ikaw ang host ng lobby at may sumaling bisitang manlalaro.

![Mabilisang Chat](screenshots/quick-chat.png)

- **On-Screen na Overlay ng Sanggunian ng Mabilisang Chat:** I-toggle ang isang lumulutang na overlay ng sanggunian sa kanang bahagi ng screen gamit ang `Numpad +` (o ang iyong naitalang shortcut) upang mabilis na masuri ang lahat ng aktibong tawag taktikal at shortcut sa laro. *(Kailangan tumakbo ang Helldivers sa Borderless Fullscreen o Windowed mode).*

![Overlay ng Mabilisang Chat](screenshots/overlay.png)

- **Suporta sa UI ng Maraming Wika:** Sumusuporta sa 28 Wika.
- **Arkitektura ng Provider Adapter:** Binuo gamit ang isang modular na interface ng AI provider. May kasamang na-optimize at walang pag-iisip (zero-thinking) na adapter para sa **Google Gemini** (`gemini-flash-lite-latest` / `gemini-2.0-flash-lite`), na may napapalawak na mga kontrata ng provider para sa OpenAI, Claude, at DeepSeek (malapit na).
- **Setting ng Tono ng Pagsasalin:** Kaswal: impormal na gaming slang at taktikal na salitang gamer / Normal: natural na tono ng pag-uusap nang walang gaming slang.
- **Pangkalahatang Shortcut sa Pag-pause / Pagpapatuloy:** Pindutin ang `Ctrl + F10` (o ang iyong na-configure na shortcut) anumang oras upang i-toggle ang katulong sa pagitan ng **Aktibo** at **Naka-pause**. Ang pag-pause ay maayos na nagsususpinde sa parehong pagsasalin ng chat sa laro at mga pagbrodkast ng mensahe ng Mabilisang Chat.
- **Mga Nakalaang Shortcut sa Tampok:**
  - **I-toggle ang Pagsasalin Lamang:** Pindutin ang `Ctrl + F11` (o ang iyong naitalang shortcut) upang agad na i-pause o ipagpatuloy ang pagsasalin ng chat nang hindi naaapektuhan ang Mabilisang Chat (may kasamang toast notification sa screen).
  - **I-toggle ang Mabilisang Chat Lamang:** Pindutin ang `Ctrl + F12` (o ang iyong naitalang shortcut) upang agad na i-pause o ipagpatuloy ang mga callout ng Mabilisang Chat nang hindi naaapektuhan ang pagsasalin (may kasamang toast notification sa screen).

![Mga Setting](screenshots/settings.png)

- **Ligtas na Imbakan ng Kredensyal (DPAPI):** Ang mga sensitibong kagustuhan tulad ng mga token ng AI provider API ay naka-encrypt habang nakaimbak gamit ang Windows Data Protection API (DPAPI, saklaw ng `CurrentUser`). Ang mga kredensyal ay hindi kailanman iniimbak sa plain text sa mga configuration file at maaari lamang i-decrypt sa iyong lokal na computer ng iyong Windows account.
- **Walang Telemetry at Ganap na Privacy:** Ang tool na ito ay hindi nangongolekta, nagla-log, o nagpapadala ng anumang data ng user o data ng telemetry. Ang lahat ng mga kahilingan sa pagsasalin ay direktang nakikipag-ugnayan lamang sa iyong na-configure na endpoint ng provider ng AI gamit ang iyong personal na mga kredensyal sa API.
- **100% Non-Invasive (Walang Pagbabago sa File ng Laro):** Walang archive ng laro ang binubuksan o binabago. Ang iyong mga file ng laro ay nananatiling 100% orihinal (vanilla) at hindi ginagalaw.

## 2. Pag-set up

### Mga Kinakailangan
Kung pinapatakbo mo ang pre-compiled standalone release (`HD1-Babel.exe`):
- **Operating System:** Windows 10 / 11 64-bit (x64).
- **Desktop Runtime:** [Microsoft .NET 10 Desktop Runtime (x64)](https://aka.ms/dotnet/10.0/windowsdesktop-runtime-win-x64.exe) (direktang installer).
- **Target na Laro:** *Helldivers* (2015) sa Steam (64-bit `helldivers.exe`).

### Mag-set up ng Libreng AI API Key
- **Mga Kredensyal ng AI Provider:** Google Gemini API Key (kumuha ng libreng personal na key mula sa [Google AI Studio](https://aistudio.google.com/) ([Panoorin ang tutorial video na ito](https://www.youtube.com/watch?v=YHFMrZD-_3g))).
- **Internet Access:** Kinakailangan para sa mga tawag sa translation API.

### Pag-download at Pag-install (Pre-Built Release)

**Hindi** mo kailangang mag-install ng mga tool ng developer o mag-compile mula sa source code upang magamit ang Babel ([ngunit kung nais mo](#4-pagbuo-pag-compile-mula-sa-source)):
1. Mag-navigate sa seksyon ng **[Releases](https://github.com/difinim/helldivers1-babel/releases)** sa kanang bahagi ng pahina ng proyekto sa GitHub.
2. Sa ilalim ng pinakabagong release, palawakin ang listahan ng **Assets**.
3. I-download ang standalone distribution archive: **`Helldivers1Babel-v<version>-win-x64.zip`**.
4. I-extract ang zip file kahit saan sa iyong computer (hal. `C:\GamesTools\Helldivers1Babel`).
5. Buksan ang folder at patakbuhin ang **`HD1-Babel.exe`**.

> [!NOTE]
> **Paunang Kinakailangan:** Ang mga end-user ay nangangailangan lamang ng libreng [Microsoft .NET 10 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/10.0). Walang C++ compiler, SDK, o toolchain ang kailangan upang patakbuhin ang release na ito.

> [!NOTE]
> **Tandaan:** Dahil gumagamit ang InjectorService.cs ng mga karaniwang API ng pag-inject ng proseso sa Windows (`VirtualAllocEx` + `WriteProcessMemory` + `CreateRemoteThread`), _**maaaring**_ mag-trigger ang Windows Defender o third-party na Antivirus ng heuristic detection (hal. Generic HackTool). Ito ay normal na pag-uugali para sa mga game mod injector at ligtas na maipapasa sa whitelist (allowlist).

## 3. Gabay sa Paggamit

### Pagsisimula
1. Simulan ang **Helldivers 1 Babel** (`HD1-Babel.exe`).
2. Sa unang paglulunsad, buksan ang tab na **Mga Setting**:
   - Ilagay ang iyong **Google Gemini API Key** (ligtas na nakaimbak sa pamamagitan ng Windows DPAPI encryption).
   - Piliin ang iyong target na wika (hal. `Tagalog (Filipino)`).
   - I-click ang **Subukan ang Koneksyon ng API** upang i-verify ang iyong key at latency.
   - I-customize ang shortcut sa **Pag-pause / Pagpapatuloy** kung nais (default ay `Ctrl + F10`).
3. I-configure ang iyong mga taktikal na callout sa tab na **Mabilisang Chat** (hal. i-bind ang Numpad 0-9 sa iyong mga gustong mensahe).
4. Ilunsad ang **Helldivers** sa Steam. Awtomatikong matutukoy ng Babel ang `helldivers.exe`.

### Komunikasyon sa Laro at Mga Halimbawa ng Paggamit

#### 1. Awtomatikong Papasok na Salin
Ang mga papasok na mensahe mula sa mga kakampi ay natutukoy at isinasalin sa iyong target na wika nang real-time, na ipinapakita sa in-game chat kasama ang orihinal na pangalan at kulay ng manlalaro:
- Kakampi (English): `so quiet`
- Chat sa Laro: `Diveheller: (English): napakatahimik naman`

#### 2. Awtomatikong Pagsagot sa Huling Wika na Pumasok (`>>mensahe`)
Ang paglalagay ng `>>` sa unahan ng iyong mensahe sa chat ay awtomatikong magsasalin ng iyong teksto sa wika ng huling papasok na mensahe mula sa kakampi:
- Iyong inilagay sa chat: `>>oo, masyadong tahimik dito`
- Makikita ng kakampi: `yes, it is too quiet`

#### 3. Tahasang Pagpili ng Wika (`target na wika>>mensahe`)
Direktang ipadala ang iyong mensahe sa isang partikular na wika anumang oras sa pamamagitan ng paglalagay ng pangalan ng wika na sinusundan ng `>>`:

| Command sa Pag-input sa Chat | Target na Wika | Halimbawang Resulta na Naibrodkast sa mga Kakampi |
| :--- | :--- | :--- |
| `russian>>magandang umaga!` | Russian | `доброе утро!` |
| `english>>mga mina sa hilaga` | English | `mines to the north` |
| `japanese>>mag-ingat sa mga putok!` | Japanese | `射線に注意せよ！` |
| `spanish>>kailangan ko ng bala` | Spanish | `necesito munición` |
| `german>>umatras sa extraction point` | German | `Rückzug zum Extraktionspunkt` |
| `french>>ipagtanggol natin dito` | French | `défendons cette position` |

#### 4. Karaniwang Papalabas na Mensahe at Pagpigil sa Sariling Pagsasalin
Kapag nagta-type ng karaniwang mensahe sa chat nang walang `>>` (hal. `need ammo`, `tara na`), ang iyong mensahe ay direktang ipinapadala sa karaniwang chat ng multiplayer. Kapag naka-on ang opsyon na **"Huwag isalin ang karaniwang papalabas na mensahe"** (ON bilang default sa quick controls bar at Settings), pinipigilan ng Babel na isalin ang sarili mong mensahe pabalik sa iyo. Kung naka-OFF, ipoproseso rin ng Babel ang mga karaniwang lokal na mensahe sa pipeline ng papasok na pagsasalin.

#### 5. Mga Shortcut ng Taktikal na Mabilisang Chat
Pindutin ang anumang na-configure na hotkey ng mabilisang chat (hal. `NumPad 1`-`NumPad 9`) habang naglalaro upang agad na mai-broadcast ang mga preset na callout sa multiplayer chat nang hindi binubuksan ang prompt ng chat. Ang lahat ng pagpindot sa key ay maayos na ipinapasa sa laro, tinitiyak na ang iyong mga kontrol ay nananatiling mabilis tumugon.

#### 6. Overlay ng Sanggunian ng Taktikal na Mabilisang Chat (`Numpad +`)
Pindutin ang `Numpad +` (o ang iyong naitalang shortcut na na-configure sa tab na **Mabilisang Chat**) anumang oras upang i-toggle ang isang on-screen na reference overlay na naglilista ng lahat ng aktibong preset ng Mabilisang Chat. Lumilitaw ang overlay na nakapantay sa kanan at nakasentro nang patayo sa screen, ipinapakita ang bawat aktibong preset sa pattern na `[<shortcut>] <mensahe>` (na ang mga mensahe ay pinaikli sa 30 character) gamit ang parehong hindi nakakaabalang visual style tulad ng mga toast.
> [!NOTE]
> **Kinakailangan sa Mode ng Display:** Dapat na naka-configure ang Helldivers na tumakbo sa **Borderless Fullscreen** (o Windowed) mode sa in-game Video settings upang maipakita ang overlay window ng Windows sa ibabaw ng laro.

#### 7. I-pause, Ipagpatuloy at Pag-toggle ng Tampok
- **Pangkalahatang Pag-pause / Pagpapatuloy (`Ctrl + F10`):** Pangunahing switch na maayos na nagsususpinde/nagpapatuloy sa parehong pagharang sa pagsasalin at pagbrodkast ng mensahe ng Mabilisang Chat.
- **I-toggle ang Pagsasalin Lamang (`Ctrl + F11`):** Tina-toggle lamang ang tampok na pagsasalin on o off na may toast notification sa screen. Ang Mabilisang Chat ay patuloy na gumagana nang normal.
- **I-toggle ang Mabilisang Chat Lamang (`Ctrl + F12`):** Tina-toggle lamang ang pagbrodkast ng mensahe ng Mabilisang Chat on o off na may toast notification sa screen. Ang pagsasalin ng chat sa laro ay patuloy na gumagana nang normal.
*(Maaaring i-customize ang lahat ng shortcut sa mga tab na **Mga Setting** at **Mabilisang Chat** gamit ang click-to-record shortcut buttons).*

#### 8. Pag-export at Patuloy na Mga Log ng Chat
I-click ang **I-export ang Mga Log** sa tab na Pagsasalin at Mga Log anumang oras upang pumili ng lokasyon at i-export ang iyong kasaysayan ng chat (may mga timestamp, pangalan ng manlalaro, orihinal na mensahe, pagsasalin, at latency) sa isang `.txt` na dokumento. Awtomatikong nananatili ang mga mensahe ng chat sa talahanayan sa mga paglulunsad ng app sa pamamagitan ng asynchronous batch disk writes (ang limitasyon sa imbakan ay maaaring i-configure mula 0 hanggang 100 MB sa Mga Setting, na may button na **Linisin ang memorya ng log**).

#### 9. Mga Interactive na Link ng Mensahe
Ang mga mensahe sa chat ng squad na naglalaman ng mga URL na `http://` o `https://` ay ipinapakita bilang mga naki-click na link sa column na **Orihinal na Mensahe**. Kapag na-click, hihilingin sa iyo ng confirmation dialog na i-verify kung pinagkakatiwalaan mo ang link bago ito buksan sa iyong default na web browser.

## 4. Pagbuo (Pag-compile mula sa Source)

### Mga Kinakailangan ng Developer
Kung ikaw ay nagko-compile at nagbi-build ng proyekto mula sa source:
- **Operating System:** Windows 10 / 11 64-bit (x64).
- **.NET 10 SDK:** [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (`net10.0-windows`).
- **MSYS2 MinGW64 Toolchain (Clang++, GCC, Make):** [MSYS2](https://www.msys2.org/) kasama ang mga package na `mingw-w64-x86_64-clang`, `mingw-w64-x86_64-gcc`, at `mingw-w64-x86_64-make` - kinakailangan para sa pag-compile ng native hook library (`BabelHook.dll`) at pag-build ng opisyal na LuaJIT 2.0 submodule (`vendor/luajit`) upang i-compile ang in-game bootstrap script (`babel_bootstrap.lua`) sa bytecode.
- **Git:** Git for Windows (na may mga recursive submodule para sa MinHook amalgamation at LuaJIT 2.0).

### Awtomatikong Pag-set up ng Kapaligiran
I-double click ang `setup.bat` (o patakbuhin ang `setup.ps1` sa PowerShell) sa root ng repository. Awtomatiko nitong bini-verify at ini-install ang mga nawawalang paunang kinakailangan:
1. Bini-verify ang 64-bit Windows environment.
2. Sinusuri kung may .NET 10 SDK (awtomatikong nag-i-install sa pamamagitan ng `winget` kung wala).
3. Sinusuri kung may MSYS2 MinGW64 toolchain (Clang++, GCC, at Make). Kung wala, awtomatikong ini-install ang MSYS2 gamit ang `winget` at ini-install ang mga kinakailangang package (`mingw-w64-x86_64-clang`, `mingw-w64-x86_64-gcc`, at `mingw-w64-x86_64-make`) sa pamamagitan ng `pacman`.
4. Inisyalisa at binuo ang LuaJIT 2.0 compiler mula sa submodule (`vendor/luajit`).
5. Awtomatikong tinitiyak na ang `C:\msys64\mingw64\bin` at `C:\msys64\usr\bin` ay naidagdag sa PATH.
6. Bini-verify ang mga dependency ng MinHook amalgamation at nagpapatakbo ng paunang build sanity check.

### Pangunahing Pag-build at Pag-package ng Release
Upang linisin ang mga nakaraang artifact, i-compile ang native hook DLL, patakbuhin ang lahat ng unit test, at buuin ang package ng release:
```powershell
pwsh -File build.ps1
```
Ang output directory ay ipapakete sa `dist/Helldivers1Babel-vX.Y.Z/`.

### Mano-manong Hakbang-hakbang na Pag-build
```powershell
# 1. I-build ang native hook DLL (awtomatikong kino-compile ang babel_bootstrap.lua gamit ang luajit)
pwsh -File src/Babel.Hook/build.ps1

# 2. I-build ang solusyon ng .NET 10
dotnet build Helldivers1Babel.slnx

# 3. Patakbuhin ang automated test suites
dotnet test
```

## 5. Lisensya

Ipinamamahagi sa ilalim ng [Lisensyang MIT](../LICENSE).
