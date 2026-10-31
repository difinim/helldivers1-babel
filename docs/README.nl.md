# ![Helldivers 1 Babel](screenshots/logo.png)

[![Latest Version](https://img.shields.io/github/v/release/difinim/helldivers1-babel?logo=github&label=release)](https://github.com/difinim/helldivers1-babel/releases/latest)
[![Build Status](https://github.com/difinim/helldivers1-babel/actions/workflows/release.yml/badge.svg)](https://github.com/difinim/helldivers1-babel/actions/workflows/release.yml)
[![Tests](https://github.com/difinim/helldivers1-babel/actions/workflows/tests.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/tests.yml?query=branch%3Amain)
[![CodeQL](https://github.com/difinim/helldivers1-babel/actions/workflows/codeql.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/codeql.yml?query=branch%3Amain)
[![OSV-Scanner](https://github.com/difinim/helldivers1-babel/actions/workflows/osv-scanner.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/osv-scanner.yml?query=branch%3Amain)
[![Dependabot](https://img.shields.io/badge/dependabot-enabled-025E8C?logo=dependabot&logoColor=white)](https://github.com/difinim/helldivers1-babel/network/updates)

*Duik samen. Spreek als één.*

**Helldivers 1 Babel** is een open-source, niet-invasieve chatvertaal- en tactische communicatieassistent voor *Helldivers (2015)*, want *zoete vrijheid klinkt beter als iedereen het plan begrijpt*.

## Translations

[English](../README.md) | [العربية](README.ar.md) | [বাংলা](README.bn.md) | [简体中文](README.zh.md) | [Čeština](README.cs.md) | [Nederlands](README.nl.md) | [Français](README.fr.md) | [Deutsch](README.de.md) | [Ελληνικά](README.el.md) | [हिन्दी](README.hi.md) | [Magyar](README.hu.md) | [Bahasa Indonesia](README.id.md) | [Italiano](README.it.md) | [日本語](README.ja.md) | [한국어](README.ko.md) | [Bahasa Melayu](README.ms.md) | [فارسی](README.fa.md) | [Polski](README.pl.md) | [Português (PT-BR)](README.pt-BR.md) | [Română](README.ro.md) | [Русский](README.ru.md) | [Español](README.es.md) | [Svenska](README.sv.md) | [Tagalog (Filipino)](README.tl.md) | [ไทย](README.th.md) | [Türkçe](README.tr.md) | [Українська](README.uk.md) | [Tiếng Việt](README.vi.md)

> [!IMPORTANT]
> **Compatibiliteitsdisclaimer:**
> **Helldivers 1 Babel is uitsluitend ontwikkeld voor Helldivers (2015) op pc (Steam).**
> **Het is compatibel met mods.**
> **Het is NIET compatibel met Helldivers 2 (2024)**, dat compleet anders is en anti-cheat op kernelniveau gebruikt.

## Inhoudsopgave

- [1. Overzicht & Belangrijkste Functies](#1-overzicht--belangrijkste-functies)
- [2. Installatie](#2-installatie)
  - [Vereisten](#vereisten)
  - [Een gratis AI-API-sleutel instellen](#een-gratis-ai-api-sleutel-instellen)
  - [Downloaden & Installatie (Kant-en-klare Release)](#downloaden--installatie-kant-en-klare-release)
- [3. Handleiding](#3-handleiding)
  - [Aan de slag](#aan-de-slag)
  - [In-game Communicatie & Toepassingen](#in-game-communicatie--toepassingen)
- [4. Ontwikkeling (Bouwen vanuit Broncode)](#4-ontwikkeling-bouwen-vanuit-broncode)
  - [Vereisten voor Ontwikkelaars](#vereisten-voor-ontwikkelaars)
  - [Geautomatiseerde Omgevingsconfiguratie](#geautomatiseerde-omgevingsconfiguratie)
  - [Master Build & Release-verpakking](#master-build--release-verpakking)
  - [Handmatige Stap-voor-stap Build](#handmatige-stap-voor-stap-build)
- [5. Licentie](#5-licentie)

## 1. Overzicht & Belangrijkste Functies

- **Realtime bidirectionele chatvertaling:**
  - **Inkomende berichten:** Detecteert automatisch de taal van teamgenoten, vertaalt berichten naar uw gekozen doeltaal (ondersteunt tot 28 doeltalen, waaronder Portugees PT-BR, Engels, Spaans, Russisch, Chinees, Japans, Frans, Duits en meer) en geeft ze **lokaal** weer in de in-game chat met **behoud van de originele spelersnaam en kleurcode van de teamgenoot** (P1 Goud, P2 Blauw, P3 Paars, P4 Cyaan).
  - **Uitgaande berichten:** Typ `[doeltaal]>>bericht` of simpelweg `>>bericht` om automatisch te antwoorden in de taal van het laatste inkomende bericht van een teamgenoot. Vertaalde tekst wordt verzonden via multiplayer-RPC zodat internationale teamgenoten het direct in hun eigen taal lezen.
- **Persistente chatlogboeken & bestandsexport:** Volledige registratie van chatsessies met tijdstempels, spelersnamen, kleuren, gedetecteerde brontalen en vertaallatentie. Logboeken blijven bewaard na het herstarten van de applicatie via lichtgewicht asynchrone batchgewijze schijfopslag (configureerbaar), met een 1-klik export naar een `.txt`-bestand.
- **Doorzoekbare chatlogboeken:** Volledig doorzoekbare chatgeschiedenis met filtering op alle kolommen.

![Helldivers 1 Babel in actie](screenshots/screenshot.png)

- **Tactische Snelle Chat (10 aangepaste slots):** 10 aanpasbare, vooraf gedefinieerde tactische berichtenslots, elk gekoppeld aan configureerbare sneltoetsen (bijv. Numpad 0-9 of aangepaste combinaties) om direct tactische berichten naar de in-game multiplayer-chat te verzenden.
- **Automatisch host-welkomstbericht:** Verzendt automatisch een aanpasbaar welkomstbericht in de in-game chat wanneer u een lobby host en een gastspeler meedoet.

![Snelle Chat](screenshots/quick-chat.png)

- **Tactische Snelle Chat-overlay op het scherm:** Schakel een niet-focusserende zwevende referentie-overlay aan de rechterkant van het scherm in/uit met `Numpad +` (of uw opgenomen sneltoets) om snel alle actieve tactische berichten en sneltoetsen in de game te bekijken. *(Vereist dat Helldivers draait in randloos venster of venstermodus).*

![Snelle Chat-overlay](screenshots/overlay.png)

- **Meertalige interface:** Ondersteunt 28 talen.
- **Provider-adapterarchitectuur:** Gebouwd met een modulaire AI-providerinterface. Voorzien van een geoptimaliseerde, zero-thinking adapter voor **Google Gemini** (`gemini-flash-lite-latest` / `gemini-2.0-flash-lite`), met uitbreidbare provider-interfaces voor OpenAI, Claude en DeepSeek (nog niet geïmplementeerd).
- **Instelling van vertaaltoon:** Informeel: informeel gaming-jargon en tactische gaming-slang / Normaal: natuurlijke conversatietoon zonder gaming-slang.
- **Globale Pauzeren / Hervatten-sneltoets:** Druk op elk gewenst moment op `Ctrl + F10` (of uw geconfigureerde sneltoets) om de assistent te schakelen tussen **Actief** en **Gepauzeerd**. Pauzeren onderbreekt zowel de in-game chatvertaling als het verzenden van Snelle Chat-berichten netjes.
- **Toegewijde sneltoetsen voor functies:**
  - **Alleen Vertaling schakelen:** Druk op `Ctrl + F11` (of uw opgenomen sneltoets) om direct alleen chatvertaling te pauzeren of te hervatten zonder Snelle Chat te beïnvloeden (met schermmelding/toast-bevestiging).
  - **Alleen Snelle Chat schakelen:** Druk op `Ctrl + F12` (of uw opgenomen sneltoets) om direct alleen Snelle Chat-berichten te pauzeren of te hervatten zonder vertaling te beïnvloeden (met schermmelding/toast-bevestiging).

![Instellingen](screenshots/settings.png)

- **Beveiligde opslag van inloggegevens (DPAPI):** Gevoelige instellingen zoals API-tokens van AI-providers worden lokaal versleuteld opgeslagen met de Windows Data Protection API (DPAPI, `CurrentUser`-bereik). Inloggegevens worden nooit in platte tekst opgeslagen in configuratiebestanden en kunnen alleen worden ontsleuteld op uw lokale computer door uw Windows-account.
- **Nul telemetrie & volledige privacy:** Deze tool verzamelt, registreert of verzendt geen enkele vorm van gebruikers- of telemetriedata. Alle vertaalverzoeken communiceren rechtstreeks en uitsluitend met het eindpunt van uw geconfigureerde AI-provider via uw persoonlijke API-inloggegevens.
- **100% niet-invasief (Geen aanpassing van spelbestanden):** Er worden geen spelarchieven uitgepakt of aangepast. Uw spelbestanden blijven 100% origineel (vanilla) en onaangetast.




## 2. Installatie

### Vereisten
Als u de vooraf gecompileerde standalone-versie gebruikt (`HD1-Babel.exe`):
- **Besturingssysteem:** Windows 10 / 11 64-bits (x64).
- **Desktop Runtime:** [Microsoft .NET 10 Desktop Runtime (x64)](https://aka.ms/dotnet/10.0/windowsdesktop-runtime-win-x64.exe) (directe installer).
- **Doelspel:** *Helldivers* (2015) op Steam (64-bits `helldivers.exe`).

### Een gratis AI-API-sleutel instellen
- **AI-provider inloggegevens:** Google Gemini API-sleutel (verkrijg een gratis persoonlijke sleutel via [Google AI Studio](https://aistudio.google.com/) ([bekijk deze videotutorial](https://www.youtube.com/watch?v=YHFMrZD-_3g))).
- **Internettoegang:** Vereist voor vertaal-API-aanroepen.

### Downloaden & Installatie (Kant-en-klare Release)

U hoeft **geen** ontwikkelaarstools te installeren of broncode te compileren om Babel te gebruiken ([tenzij u dat wilt](#4-ontwikkeling-bouwen-vanuit-broncode)):
1. Ga naar het onderdeel **[Releases](https://github.com/difinim/helldivers1-babel/releases)** aan de rechterkant van de GitHub-projectpagina.
2. Vouw onder de nieuwste release de lijst **Assets** uit.
3. Download het standalone distributiearchief: **`Helldivers1Babel-v<version>-win-x64.zip`**.
4. Pak het zip-bestand uit op een gewenste locatie op uw computer (bijv. `C:\GamesTools\Helldivers1Babel`).
5. Open de map en start **`HD1-Babel.exe`**.

> [!NOTE]
> **Vereiste:** Eindgebruikers hebben alleen de gratis [Microsoft .NET 10 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/10.0) nodig. Er is geen C++-compiler, SDK of toolchain vereist om de release uit te voeren.

> [!NOTE]
> **Let op:** Omdat `InjectorService.cs` standaard Windows-procesinjectie-API's gebruikt (`VirtualAllocEx` + `WriteProcessMemory` + `CreateRemoteThread`), kan Windows Defender of antivirussoftware van derden _**mogelijk**_ een heuristische waarschuwing geven (bijv. Generic HackTool). Dit is normaal gedrag voor game-mod-injectors en kan veilig worden toegestaan (allowlist).



## 3. Handleiding

### Aan de slag
1. Start **Helldivers 1 Babel** (`HD1-Babel.exe`).
2. Open bij de eerste start het tabblad **Instellingen**:
   - Voer uw **Google Gemini API-sleutel** in (veilig opgeslagen via Windows DPAPI-versleuteling).
   - Selecteer uw gewenste doeltaal (bijv. `Portuguese (PT-BR)` of `Nederlands`).
   - Klik op **API-verbinding testen** om uw sleutel en latentie te controleren.
   - Pas desgewenst de sneltoets voor **Pauzeren / Hervatten** aan (standaard `Ctrl + F10`).
3. Configureer uw tactische berichten in het tabblad **Snelle Chat** (koppel bijv. Numpad 0-9 aan uw favoriete berichten).
4. Start **Helldivers** op Steam. Babel detecteert automatisch `helldivers.exe`.

### In-game Communicatie & Toepassingen

#### 1. Automatische inkomende vertaling
Inkomende berichten van teamgenoten worden direct gedetecteerd en vertaald naar uw doeltaal, en weergegeven in de in-game chat met behoud van de originele spelersnaam en kleur:
- Teamgenoot (Engels): `so quiet`
- In-Game Chat: `Diveheller: (English): mó silêncio`

#### 2. Automatisch antwoorden in de laatst ontvangen taal (`>>bericht`)
Door uw chatbericht te laten beginnen met `>>` wordt uw tekst automatisch vertaald naar de taal van het laatste inkomende bericht van een teamgenoot:
- Uw invoer in de chat: `>>sim, está calmo demais`
- Teamgenoot ziet: `yes, it is too quiet`

#### 3. Expliciete / Willekeurige taalkeuze (`doeltaal>>bericht`)
Richt uw bericht op elk moment op een specifieke taal door de naam van de taal gevolgd door `>>` voor uw bericht te typen:

| Chatinvoercommando | Doeltaal | Voorbeeldresultaat verzonden naar teamgenoten |
| :--- | :--- | :--- |
| `russian>>bom dia!` | Russisch | `доброе утро!` |
| `english>>minas ao norte` | Engels | `mines to the north` |
| `japanese>>cuidado com os tiros!` | Japans | `射線に注意せよ！` |
| `spanish>>preciso de munição` | Spaans | `necesito munición` |
| `german>>recuem para o ponto de extração` | Duits | `Rückzug zum Extraktionspunkt` |
| `french>>vamos defender aqui` | Frans | `défendons cette position` |

#### 4. Standaard uitgaande berichten & onderdrukking van zelfvertaling
Wanneer u een standaard in-game chatbericht zonder `>>` typt (bijv. `need ammo`, `vamos lá`), wordt uw bericht rechtstreeks via de standaard multiplayer-chat verzonden. Met de optie **„Standaard uitgaande berichten niet vertalen”** ingeschakeld (standaard AAN in zowel de snelle bedieningsbalk als in Instellingen), voorkomt Babel dat uw eigen bericht naar uzelf wordt terugvertaald. Indien uitgeschakeld verwerkt Babel ook standaard lokale berichten via de pijplijn voor inkomende vertalingen.

#### 5. Tactische Snelle Chat-sneltoetsen
Druk tijdens het spelen op een geconfigureerde sneltoets voor Snelle Chat (bijv. `Numpad 1`–`Numpad 9`) om direct vooraf ingestelde berichten naar de multiplayer-chat te verzenden zonder het chatvenster te openen. Alle toetsaanslagen worden naadloos doorgegeven aan het spel, zodat uw besturing altijd responsief blijft.

#### 6. Tactische Snelle Chat-referentie-overlay (`Numpad +`)
Druk op elk moment op `Numpad +` (of uw opgenomen sneltoets uit het tabblad **Snelle Chat**) om een schermoverlay in of uit te schakelen met alle actieve Snelle Chat-voorinstellingen. De overlay verschijnt rechts uitgelijnd en verticaal gecentreerd op het scherm, waarbij elke actieve voorinstelling wordt getoond in het formaat `[<sneltoets>] <bericht>` (berichten worden ingekort tot 30 tekens) in dezelfde subtiele stijl als meldingen.
> [!NOTE]
> **Vereiste weergavemodus:** Helldivers moet in de video-instellingen van de game zijn ingesteld op **Randloos venster** (of Venstermodus) zodat Windows-overlays boven het spel kunnen worden weergegeven.

#### 7. Pauzeren, Hervatten & Functieschakelaars
- **Globaal Pauzeren / Hervatten (`Ctrl + F10`):** Hoofdschakelaar die zowel vertalingsonderschepping als het verzenden van Snelle Chat-berichten netjes onderbreekt en hervat.
- **Alleen Vertaling schakelen (`Ctrl + F11`):** Schakelt alleen de vertaalfunctie in of uit met een schermmelding. Snelle Chat blijft normaal functioneren.
- **Alleen Snelle Chat schakelen (`Ctrl + F12`):** Schakelt alleen het verzenden van Snelle Chat-berichten in of uit met een schermmelding. In-game chatvertaling blijft normaal functioneren.
*(Alle sneltoetsen kunnen worden aangepast in de tabbladen **Instellingen** en **Snelle Chat** via de opnameknoppen voor sneltoetsen).*

#### 8. Exporteren & persistente chatlogboeken
Klik op elk gewenst moment op **Logboeken exporteren** in het tabblad Vertaling & Logboeken om een locatie te kiezen en uw chatgeschiedenis (met tijdstempels, spelersnamen, originele berichten, vertalingen en latenties) te exporteren naar een `.txt`-bestand. Chatberichten blijven automatisch behouden na het opstarten van de app via asynchrone batchgewijze schrijfbewerkingen naar schijf (opslaglimiet is instelbaar van 0 tot 100 MB in Instellingen, met een knop **Logboekgeheugen wissen**).

#### 9. Interactieve berichtlinks
Squad-chatberichten die `http://`- of `https://`-URL's bevatten, worden in de kolom **Origineel bericht** weergegeven als klikbare links. Wanneer u hierop klikt, vraagt een bevestigingsvenster of u de link vertrouwt voordat deze in uw standaard webbrowser wordt geopend.



## 4. Ontwikkeling (Bouwen vanuit Broncode)

### Vereisten voor Ontwikkelaars
Als u het project zelf wilt compileren en bouwen vanuit de broncode:
- **Besturingssysteem:** Windows 10 / 11 64-bits (x64).
- **.NET 10 SDK:** [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (`net10.0-windows`).
- **MSYS2 MinGW64 Toolchain (Clang++, GCC, Make):** [MSYS2](https://www.msys2.org/) met de pakketten `mingw-w64-x86_64-clang`, `mingw-w64-x86_64-gcc` en `mingw-w64-x86_64-make` – vereist voor het compileren van de native hook-bibliotheek (`BabelHook.dll`) en het bouwen van de officiële LuaJIT 2.0-submodule (`vendor/luajit`) om het in-game bootstrap-script (`babel_bootstrap.lua`) te compileren naar bytecode.
- **Git:** Git voor Windows (met recursieve submodules voor MinHook-amalgamation en LuaJIT 2.0).


### Geautomatiseerde Omgevingsconfiguratie
Dubbelklik op `setup.bat` (of voer `setup.ps1` uit in PowerShell) in de hoofdmap van de repository. Dit controleert en installeert automatisch eventuele ontbrekende vereisten:
1. Controleert de 64-bits Windows-omgeving.
2. Controleert op de .NET 10 SDK (automatische installatie via `winget` indien afwezig).
3. Controleert op de MSYS2 MinGW64 toolchain (Clang++, GCC en Make). Indien afwezig wordt MSYS2 automatisch geïnstalleerd via `winget` en worden de vereiste pakketten (`mingw-w64-x86_64-clang`, `mingw-w64-x86_64-gcc` en `mingw-w64-x86_64-make`) geïnstalleerd via `pacman`.
4. Initialiseert en bouwt de LuaJIT 2.0-compiler vanuit de submodule (`vendor/luajit`).
5. Zorgt er automatisch voor dat `C:\msys64\mingw64\bin` en `C:\msys64\usr\bin` worden toegevoegd aan het PATH.
6. Verifieert de MinHook-afhankelijkheden en voert een initiële controlebuild uit.

### Master Build & Release-verpakking
Om eerdere artefacten op te schonen, de native hook-DLL te compileren, alle unit-tests uit te voeren en het releasepakket samen te stellen:
```powershell
pwsh -File build.ps1
```
De uitvoermap wordt verpakt in `dist/Helldivers1Babel-vX.Y.Z/`.

### Handmatige Stap-voor-stap Build
```powershell
# 1. Bouw de native hook DLL (compileert babel_bootstrap.lua automatisch met luajit)
pwsh -File src/Babel.Hook/build.ps1

# 2. Bouw de .NET 10-oplossing
dotnet build Helldivers1Babel.slnx

# 3. Voer geautomatiseerde testsuites uit
dotnet test
```



## 5. Licentie

Gedistribueerd onder de [MIT-licentie](../LICENSE).
