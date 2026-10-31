# ![Helldivers 1 Babel](screenshots/logo.png)

[![Latest Version](https://img.shields.io/github/v/release/difinim/helldivers1-babel?logo=github&label=release)](https://github.com/difinim/helldivers1-babel/releases/latest)
[![Build Status](https://github.com/difinim/helldivers1-babel/actions/workflows/release.yml/badge.svg)](https://github.com/difinim/helldivers1-babel/actions/workflows/release.yml)
[![Tests](https://github.com/difinim/helldivers1-babel/actions/workflows/tests.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/tests.yml?query=branch%3Amain)
[![CodeQL](https://github.com/difinim/helldivers1-babel/actions/workflows/codeql.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/codeql.yml?query=branch%3Amain)
[![OSV-Scanner](https://github.com/difinim/helldivers1-babel/actions/workflows/osv-scanner.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/osv-scanner.yml?query=branch%3Amain)
[![Dependabot](https://img.shields.io/badge/dependabot-enabled-025E8C?logo=dependabot&logoColor=white)](https://github.com/difinim/helldivers1-babel/network/updates)

*Dyk tillsammans. Tala som en.*

**Helldivers 1 Babel** är en öppen källkod och icke-invasiv chattöversättnings- och taktisk kommunikationsassistent för *Helldivers (2015)*, eftersom *ljuv frihet låter bättre när alla förstår planen*.

## Translations

[English](../README.md) | [العربية](README.ar.md) | [বাংলা](README.bn.md) | [简体中文](README.zh.md) | [Čeština](README.cs.md) | [Nederlands](README.nl.md) | [Français](README.fr.md) | [Deutsch](README.de.md) | [Ελληνικά](README.el.md) | [हिन्दी](README.hi.md) | [Magyar](README.hu.md) | [Bahasa Indonesia](README.id.md) | [Italiano](README.it.md) | [日本語](README.ja.md) | [한국어](README.ko.md) | [Bahasa Melayu](README.ms.md) | [فارسی](README.fa.md) | [Polski](README.pl.md) | [Português (PT-BR)](README.pt-BR.md) | [Română](README.ro.md) | [Русский](README.ru.md) | [Español](README.es.md) | [Svenska](README.sv.md) | [Tagalog (Filipino)](README.tl.md) | [ไทย](README.th.md) | [Türkçe](README.tr.md) | [Українська](README.uk.md) | [Tiếng Việt](README.vi.md)

> [!IMPORTANT]
> **Kompatibilitetsfriskrivning:**
> **Helldivers 1 Babel är uteslutande utvecklat för Helldivers (2015) på PC (Steam).**
> **Det är kompatibelt med moddar.**
> **Det är INTE kompatibelt med Helldivers 2 (2024)**, som är helt annorlunda och använder anti-fusk på kärnnivå (kernel-level anti-cheat).

## Innehållsförteckning

- [1. Översikt & Huvudfunktioner](#1-översikt--huvudfunktioner)
- [2. Installation](#2-installation)
  - [Systemkrav](#systemkrav)
  - [Skaffa en gratis AI API-nyckel](#skaffa-en-gratis-ai-api-nyckel)
  - [Nedladdning & Installation (Färdig Release)](#nedladdning--installation-färdig-release)
- [3. Användarguide](#3-användarguide)
  - [Kom igång](#kom-igång)
  - [Kommunikation i spelet & Användningsområden](#kommunikation-i-spelet--användningsområden)
- [4. Utveckling (Bygg från källkod)](#4-utveckling-bygg-från-källkod)
  - [Utvecklarkrav](#utvecklarkrav)
  - [Automatisk miljökonfiguration](#automatisk-miljökonfiguration)
  - [Huvudbygge & Release-paketering](#huvudbygge--release-paketering)
  - [Manuellt steg-för-steg-bygge](#manuellt-steg-för-steg-bygge)
- [5. Licens](#5-licens)

## 1. Översikt & Huvudfunktioner

- **Realtids dubbelriktad chattöversättning:**
  - **Inkommande meddelanden:** Upptäcker automatiskt lagkamraternas språk, översätter meddelanden till ditt valda målspråk (stöder upp till 28 målspråk, inklusive portugisiska PT-BR, engelska, spanska, ryska, kinesiska, japanska, franska, tyska med flera) och visar dem **lokalt** i spelchatten med **fullt bevarande av lagkamratens ursprungliga namn och färgkod** (P1 Guld, P2 Blå, P3 Lila, P4 Cyan).
  - **Utgående meddelanden:** Skriv `[målspråk]>>meddelande` eller bara `>>meddelande` för att automatiskt svara på språket från det senast inkomna meddelandet från en lagkamrat. Översatt text sänds via multiplayer-RPC så att internationella medspelare kan läsa det direkt på sitt eget språk.
- **Persistenta chattloggar & filexport:** Fullständig historikspårning över spelsessionen med tidsstämplar, spelarnamn, färger, upptäckta källspråk och översättningslatens. Loggarna sparas i tabellen mellan appstarter via lättviktig asynkron batchskrivning till disk (konfigurerbar), med ett-klicks-export till en `.txt`-fil.
- **Sökbara chattloggar:** Fullt sökbar chatthistorik som kan filtreras över alla kolumner.

![Helldivers 1 Babel i aktion](screenshots/screenshot.png)

- **Taktisk Snabbchatt (10 anpassade platser):** 10 anpassningsbara fördefinierade taktiska anrop, var och en kopplad till användarkonfigurerbara snabbtangenter (t.ex. Numpad 0-9 eller egna kombinationer) för att omedelbart sända taktiska anrop i flerspelarchatten i spelet.
- **Automatiskt välkomstmeddelande:** Skickar automatiskt ett anpassningsbart välkomstmeddelande i spelchatten när du är värd för en lobby och en gästspelare ansluter.

![Snabbchatt](screenshots/quick-chat.png)

- **Taktiskt Snabbchatt-overlay på skärmen:** Slå på/av ett flytande referensoverlay till höger på skärmen med `Numpad +` (eller ditt inspelade snabbkommando) för att snabbt se alla aktiva anrop och snabbtangenter i spelet utan att störa spelfokus. *(Kräver att Helldivers körs i ramlöst fönsterläge eller fönsterläge).*

![Snabbchatt-overlay](screenshots/overlay.png)

- **Stöd för flerspråkigt gränssnitt:** Stöder 28 språk.
- **Adapterarkitektur för AI-leverantörer:** Byggd med ett modulärt gränssnitt för AI-leverantörer. Levereras med en optimerad, zero-thinking adapter för **Google Gemini** (`gemini-flash-lite-latest` / `gemini-2.0-flash-lite`), med utbyggbara kontrakt för OpenAI, Claude och DeepSeek (ännu inte implementerade).
- **Inställning för översättningston:** Avslappnad: informell spelslang och taktiskt spelarjargong / Normal: naturlig samtalston utan spelslang.
- **Global Pausa / Återuppta-snabbtangent:** Tryck på `Ctrl + F10` (eller ditt konfigurerade snabbkommando) när som helst för att växla assistenten mellan **Aktiv** och **Pausad**. En paus avbryter både chattöversättning i spelet och sändning av snabbchattmeddelanden rent.
- **Dedikerade snabbtangenter för funktioner:**
  - **Växla enbart översättning:** Tryck på `Ctrl + F11` (eller ditt inspelade snabbkommando) för att omedelbart pausa eller återuppta chattöversättningen utan att påverka Snabbchatt (med skärmavisering/toast).
  - **Växla enbart Snabbchatt:** Tryck på `Ctrl + F12` (eller ditt inspelade snabbkommando) för att omedelbart pausa eller återuppta Snabbchatt-anrop utan att påverka översättningen (med skärmavisering/toast).

![Inställningar](screenshots/settings.png)

- **Säker lagring av inloggningsuppgifter (DPAPI):** Känsliga inställningar som API-nycklar för AI-leverantörer krypteras vid lagring med Windows Data Protection API (DPAPI, `CurrentUser`-omfång). Inloggningsuppgifter sparas aldrig i klartext i konfigurationsfiler och kan endast dekrypteras på din lokala dator av ditt Windows-konto.
- **Noll telemetri & fullständig integritet:** Detta verktyg samlar inte in, loggar inte och skickar inte några användardata eller telemetridata. Alla översättningsförfrågningar kommunicerar direkt och uteslutande med din konfigurerade AI-leverantörs slutpunkt med dina personliga API-uppgifter.
- **100 % icke-invasivt (Ingen modifiering av spelfiler):** Inga spelarkiv packas upp eller modifieras. Dina spelfiler förblir 100 % original (vanilla) och orörda.




## 2. Installation

### Systemkrav
Om du kör den förkompilerade fristående versionen (`HD1-Babel.exe`):
- **Operativsystem:** Windows 10 / 11 64-bitars (x64).
- **Desktop Runtime:** [Microsoft .NET 10 Desktop Runtime (x64)](https://aka.ms/dotnet/10.0/windowsdesktop-runtime-win-x64.exe) (direkt installationsprogram).
- **Målspel:** *Helldivers* (2015) på Steam (64-bitars `helldivers.exe`).

### Skaffa en gratis AI API-nyckel
- **Inloggningsuppgifter för AI-leverantör:** Google Gemini API-nyckel (skaffa en gratis personlig nyckel via [Google AI Studio](https://aistudio.google.com/) ([se denna videoguide](https://www.youtube.com/watch?v=YHFMrZD-_3g))).
- **Internetanslutning:** Krävs för API-anrop vid översättning.

### Nedladdning & Installation (Färdig Release)

Du behöver **inte** installera utvecklarverktyg eller kompilera från källkod för att använda Babel ([men om du vill göra det](#4-utveckling-bygg-från-källkod)):
1. Gå till avsnittet **[Releases](https://github.com/difinim/helldivers1-babel/releases)** på höger sida av GitHub-projektsidan.
2. Under den senaste versionen, expandera listan **Assets**.
3. Ladda ner det fristående distributionsarkivet: **`Helldivers1Babel-v<version>-win-x64.zip`**.
4. Packa upp zip-filen var som helst på datorn (t.ex. `C:\GamesTools\Helldivers1Babel`).
5. Öppna mappen och kör **`HD1-Babel.exe`**.

> [!NOTE]
> **Förutsättning:** Slutanvändare behöver endast den kostnadsfria [Microsoft .NET 10 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/10.0). Ingen C++-kompilator, SDK eller verktygskedja krävs för att köra releasen.

> [!NOTE]
> **Observera:** Eftersom `InjectorService.cs` använder vanliga API:er för processinjektion i Windows (`VirtualAllocEx` + `WriteProcessMemory` + `CreateRemoteThread`), kan Windows Defender eller tredjeparts antivirusprogram _**eventuellt**_ ge ett heuristiskt larm (t.ex. Generic HackTool). Detta är ett normalt beteende för injektorer till spelmoddar och kan säkert godkännas (vitlistas).



## 3. Användarguide

### Kom igång
1. Starta **Helldivers 1 Babel** (`HD1-Babel.exe`).
2. Vid första start, öppna fliken **Inställningar**:
   - Ange din **Google Gemini API-nyckel** (lagras säkert via Windows DPAPI-kryptering).
   - Välj ditt målspråk (t.ex. `Portuguese (PT-BR)` eller `Svenska`).
   - Klicka på **Testa API-anslutning** för att verifiera din nyckel och latens.
   - Anpassa snabbtangenten för **Pausa / Återuppta** om så önskas (standard är `Ctrl + F10`).
3. Konfigurera dina taktiska anrop i fliken **Snabbchatt** (t.ex. bind Numpad 0-9 till dina önskade meddelanden).
4. Starta **Helldivers** på Steam. Babel upptäcker automatiskt `helldivers.exe`, injicerar `BabelHook.dll` och upprättar en snabb named pipe IPC-anslutning.

### Kommunikation i spelet & Användningsområden

#### 1. Automatisk inkommande översättning
Inkommande meddelanden från lagkamrater upptäcks och översätts i farten till ditt målspråk, och visas i spelchatten med ursprungligt spelarnamn och färg:
- Lagkamrat (engelska): `so quiet`
- Spelchatt: `Diveheller: (English): mó silêncio`

#### 2. Autosvara på senast inkomna språk (`>>meddelande`)
Om du inleder ditt chattmeddelande med `>>` översätts din text automatiskt till språket för det senast inkomna meddelandet från en lagkamrat:
- Din inmatning i chatten: `>>sim, está calmo demais`
- Lagkamrat ser: `yes, it is too quiet`

#### 3. Uttryckligt / Valfritt språkval (`målspråk>>meddelande`)
Rikta ditt meddelande till ett specifikt språk när som helst genom att ange språkets namn följt av `>>`:

| Chattkommando | Målspråk | Exempelresultat som sänds till lagkamrater |
| :--- | :--- | :--- |
| `russian>>bom dia!` | Ryska | `доброе утро!` |
| `english>>minas ao norte` | Engelska | `mines to the north` |
| `japanese>>cuidado com os tiros!` | Japanska | `射線に注意せよ！` |
| `spanish>>preciso de munição` | Spanska | `necesito munición` |
| `german>>recuem para o ponto de extração` | Tyska | `Rückzug zum Extraktionspunkt` |
| `french>>vamos defender aqui` | Franska | `défendons cette position` |

#### 4. Vanliga utgående meddelanden & ignorering av egen översättning
När du skriver ett vanligt chattmeddelande i spelet utan `>>` (t.ex. `need ammo`, `vamos lá`), skickas ditt meddelande direkt via vanlig flerspelarchatt. Med alternativet **"Översätt inte vanliga utgående meddelanden"** aktiverat (PÅ som standard i både snabbkontrollfältet och Inställningar) förhindrar Babel att ditt eget meddelande översätts tillbaka till dig. Om det är avstängt behandlar Babel även vanliga lokala meddelanden genom den inkommande översättningspipelinen.

#### 5. Snabbtangenter för taktisk Snabbchatt
Tryck på valfri konfigurerad snabbtangent för Snabbchatt (t.ex. `Numpad 1`–`Numpad 9`) medan du spelar för att omedelbart sända förinställda anrop till flerspelarchatten utan att öppna chattrutan. Alla tangenttryckningar skickas sömlöst vidare till spelet, vilket säkerställer att din spelkontroll förblir responsiv.

#### 6. Taktiskt Snabbchatt-referensoverlay (`Numpad +`)
Tryck på `Numpad +` (eller din inspelade snabbtangent konfigurerad i fliken **Snabbchatt**) när som helst för att växla ett referensoverlay på skärmen som listar alla aktiva Snabbchatt-förval. Overlayet visas högerjusterat och vertikalt centrerat på skärmen och visar varje aktivt förval i mönstret `[<snabbtangent>] <meddelande>` (med meddelanden trunkerade till 30 tecken) med samma diskreta visuella stil som aviseringar.
> [!NOTE]
> **Krav på visningsläge:** Helldivers måste vara konfigurerat att köras i **Ramlöst fönsterläge** (eller fönsterläge) i spelets videoinställningar för att Windows-overlays ska kunna visas ovanpå spelet.

#### 7. Pausa, Återuppta & Funktionsväxling
- **Global Pausa / Återuppta (`Ctrl + F10`):** Huvudströmbrytare som rent pausar/återupptar både översättningsfångst och sändning av Snabbchatt.
- **Växla enbart översättning (`Ctrl + F11`):** Slår av eller på enbart översättningsfunktionen med en skärmavisering. Snabbchatt fortsätter att fungera normalt.
- **Växla enbart Snabbchatt (`Ctrl + F12`):** Slår av eller på enbart sändning av Snabbchatt-meddelanden med en skärmavisering. Chattöversättning i spelet fortsätter att fungera normalt.
*(Alla snabbkommandon kan anpassas i flikarna **Inställningar** och **Snabbchatt** med knapparna för direktinspelning).*

#### 8. Exportera & persistenta chattloggar
Klicka på **Exportera loggar** i fliken Översättning & Loggar när som helst för att välja en plats och exportera din chatthistorik (med tidsstämplar, spelarnamn, originalmeddelanden, översättningar och latens) till ett `.txt`-dokument. Chattmeddelanden sparas automatiskt i tabellen mellan programstarter via asynkrona batchskrivningar till disk (lagringsgränsen kan konfigureras från 0 till 100 MB i Inställningar, med en knapp för att **Rensa loggminne**).

#### 9. Interaktiva meddelandelänkar
Gruppchattmeddelanden som innehåller `http://`- eller `https://`-länkar visas som klickbara länkar i kolumnen **Ursprungligt meddelande**. När du klickar på dem visas en bekräftelsedialog där du får verifiera om du litar på länken innan den öppnas i din standardwebbläsare.



## 4. Utveckling (Bygg från källkod)

### Utvecklarkrav
Om du kompilerar och bygger projektet från källkod:
- **Operativsystem:** Windows 10 / 11 64-bitars (x64).
- **.NET 10 SDK:** [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (`net10.0-windows`).
- **MSYS2 MinGW64 Toolchain (Clang++, GCC, Make):** [MSYS2](https://www.msys2.org/) med paketen `mingw-w64-x86_64-clang`, `mingw-w64-x86_64-gcc` och `mingw-w64-x86_64-make` – krävs för att kompilera det interna hook-biblioteket (`BabelHook.dll`) och bygga den officiella undermodulen LuaJIT 2.0 (`vendor/luajit`) för att kompilera spelets bootstrap-skript (`babel_bootstrap.lua`) till bytekod.
- **Git:** Git för Windows (med rekursiva undermoduler för MinHook amalgamation och LuaJIT 2.0).


### Automatisk miljökonfiguration
Dubbelklicka på `setup.bat` (eller kör `setup.ps1` i PowerShell) i arkivets rotkatalog. Skriptet verifierar och installerar automatiskt saknade förutsättningar:
1. Verifierar 64-bitars Windows-miljö.
2. Kontrollerar .NET 10 SDK (installeras automatiskt via `winget` om det saknas).
3. Kontrollerar MSYS2 MinGW64-verktygskedjan (Clang++, GCC och Make). Om det saknas installeras MSYS2 automatiskt via `winget` och de nödvändiga paketen (`mingw-w64-x86_64-clang`, `mingw-w64-x86_64-gcc` och `mingw-w64-x86_64-make`) installeras via `pacman`.
4. Initierar och bygger LuaJIT 2.0-kompilatorn från undermodulen (`vendor/luajit`).
5. Säkerställer automatiskt att `C:\msys64\mingw64\bin` och `C:\msys64\usr\bin` läggs till i PATH.
6. Verifierar MinHook-beroenden och utför en första kontrollbyggnation.

### Huvudbygge & Release-paketering
För att rensa tidigare byggartefakter, kompilera native hook DLL, köra alla enhetstester och sätta ihop releasepaketet:
```powershell
pwsh -File build.ps1
```
Utdatakatalogen paketeras i `dist/Helldivers1Babel-vX.Y.Z/`.

### Manuellt steg-för-steg-bygge
```powershell
# 1. Bygg det interna hook-DLL-biblioteket (kompilerar automatiskt babel_bootstrap.lua med luajit)
pwsh -File src/Babel.Hook/build.ps1

# 2. Bygg .NET 10-lösningen
dotnet build Helldivers1Babel.slnx

# 3. Kör automatiserade testsviter
dotnet test
```



## 5. Licens

Distribueras under [MIT-licensen](../LICENSE).
