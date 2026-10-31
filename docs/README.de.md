# ![Helldivers 1 Babel](screenshots/logo.png)

[![Latest Version](https://img.shields.io/github/v/release/difinim/helldivers1-babel?logo=github&label=release)](https://github.com/difinim/helldivers1-babel/releases/latest)
[![Build Status](https://github.com/difinim/helldivers1-babel/actions/workflows/release.yml/badge.svg)](https://github.com/difinim/helldivers1-babel/actions/workflows/release.yml)
[![Tests](https://github.com/difinim/helldivers1-babel/actions/workflows/tests.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/tests.yml?query=branch%3Amain)
[![CodeQL](https://github.com/difinim/helldivers1-babel/actions/workflows/codeql.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/codeql.yml?query=branch%3Amain)
[![OSV-Scanner](https://github.com/difinim/helldivers1-babel/actions/workflows/osv-scanner.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/osv-scanner.yml?query=branch%3Amain)
[![Dependabot](https://img.shields.io/badge/dependabot-enabled-025E8C?logo=dependabot&logoColor=white)](https://github.com/difinim/helldivers1-babel/network/updates)

*Gemeinsam springen. Mit einer Stimme sprechen.*

**Helldivers 1 Babel** ist ein quelloffener, nicht-invasiver Chat-Übersetzungs- und taktischer Kommunikationsassistent für *Helldivers (2015)* – denn *süße Freiheit klingt besser, wenn alle den Plan verstehen*.

## Translations

[English](../README.md) | [العربية](README.ar.md) | [বাংলা](README.bn.md) | [简体中文](README.zh.md) | [Čeština](README.cs.md) | [Nederlands](README.nl.md) | [Français](README.fr.md) | [Deutsch](README.de.md) | [Ελληνικά](README.el.md) | [हिन्दी](README.hi.md) | [Magyar](README.hu.md) | [Bahasa Indonesia](README.id.md) | [Italiano](README.it.md) | [日本語](README.ja.md) | [한국어](README.ko.md) | [Bahasa Melayu](README.ms.md) | [فارسی](README.fa.md) | [Polski](README.pl.md) | [Português (PT-BR)](README.pt-BR.md) | [Română](README.ro.md) | [Русский](README.ru.md) | [Español](README.es.md) | [Svenska](README.sv.md) | [Tagalog (Filipino)](README.tl.md) | [ไทย](README.th.md) | [Türkçe](README.tr.md) | [Українська](README.uk.md) | [Tiếng Việt](README.vi.md)

> [!IMPORTANT]
> **Kompatibilitätshinweis:**
> **Helldivers 1 Babel wurde ausschließlich für Helldivers (2015) auf dem PC (Steam) entwickelt.**
> **Es ist mit Mods kompatibel.**
> **Es ist NICHT kompatibel mit Helldivers 2 (2024)**, da dieses völlig unterschiedlich aufgebaut ist und ein Kernel-Level-Anti-Cheat verwendet.

## Inhaltsverzeichnis

- [1. Übersicht & Hauptfunktionen](#1-übersicht--hauptfunktionen)
- [2. Einrichtung](#2-einrichtung)
  - [Voraussetzungen](#voraussetzungen)
  - [Kostenlosen KI-API-Schlüssel einrichten](#kostenlosen-ki-api-schlüssel-einrichten)
  - [Download & Installation (Fertiges Release)](#download--installation-fertiges-release)
- [3. Benutzerhandbuch](#3-benutzerhandbuch)
  - [Erste Schritte](#erste-schritte)
  - [Kommunikation im Spiel & Anwendungsfälle](#kommunikation-im-spiel--anwendungsfälle)
- [4. Entwicklung (Aus dem Quellcode bauen)](#4-entwicklung-aus-dem-quellcode-bauen)
  - [Entwickler-Voraussetzungen](#entwickler-voraussetzungen)
  - [Automatische Umgebungseinrichtung](#automatische-umgebungseinrichtung)
  - [Master-Build & Release-Paketierung](#master-build--release-paketierung)
  - [Manuelle Schritt-für-Schritt-Erstellung](#manuelle-schritt-für-schritt-erstellung)
- [5. Lizenz](#5-lizenz)

## 1. Übersicht & Hauptfunktionen

- **Echtzeit- und bidirektionale Chat-Übersetzung:**
  - **Eingehende Nachrichten:** Erkennt automatisch die Sprache der Teammitglieder, übersetzt Nachrichten in Ihre gewählte Zielsprache (unterstützt bis zu 28 Zielsprachen, darunter Portugiesisch PT-BR, Englisch, Spanisch, Russisch, Chinesisch, Japanisch, Französisch, Deutsch und mehr) und stellt sie **lokal** im Spielchat dar, wobei der **ursprüngliche Spielername und Farbcode der Teammitglieder originalgetreu beibehalten werden** (P1 Gold, P2 Blau, P3 Lila, P4 Cyan).
  - **Ausgehende Nachrichten:** Geben Sie `[zielsprache]>>nachricht` oder einfach `>>nachricht` ein, um automatisch in der Sprache der letzten eingehenden Teammitglied-Nachricht zu antworten. Der übersetzte Text wird über Multiplayer-RPC übertragen, sodass internationale Mitspieler ihn nativ lesen können.
- **Persistente Chat-Protokolle & Dateiexport:** Vollständige Nachverfolgung des Sitzungsverlaufs mit Zeitstempeln, Spielernamen, Farben, erkannten Quellsprachen und Übersetzungslatenz. Protokolle bleiben über Anwendungsneustarts hinweg durch leichtgewichtige asynchrone Festplattenspeicherungen in Batches erhalten (konfigurierbar), mit 1-Klick-Export in eine `.txt`-Datei.
- **Durchsuchbare Chat-Protokolle:** Vollständig durchsuchbarer Chatverlauf mit Filterung über beliebige Spalten.

![Helldivers 1 Babel in Aktion](screenshots/screenshot.png)

- **Taktischer Schnell-Chat (10 benutzerdefinierte Slots):** 10 anpassbare, vordefinierte taktische Nachrichten-Slots, die jeweils an benutzerdefinierbare Hotkeys gebunden sind (z. B. Nummernblock 0–9 oder eigene Kombinationen), um taktische Funksprüche sofort im Multiplayer-Spielchat zu senden.
- **Automatische Host-Begrüßung:** Sendet automatisch eine anpassbare Willkommensnachricht im Spielchat, wenn Sie eine Lobby hosten und ein Gastspieler beitritt.

![Schnell-Chat](screenshots/quick-chat.png)

- **Taktisches Schnell-Chat-Overlay auf dem Bildschirm:** Schalten Sie mit `Nummernblock +` (oder Ihrem aufgenommenen Tastenkürzel) ein schwebendes, nicht-fokussierendes Referenz-Overlay am rechten Bildschirmrand ein/aus, um alle aktiven taktischen Funksprüche und Tastenkombinationen im Spiel schnell einzusehen. *(Erfordert, dass Helldivers im randlosen Fenstermodus oder Fenstermodus läuft).*

![Schnell-Chat-Overlay](screenshots/overlay.png)

- **Mehrsprachige Benutzeroberfläche:** Unterstützt 28 Sprachen.
- **Provider-Adapter-Architektur:** Entwickelt mit einer modularen KI-Provider-Schnittstelle. Ausgestattet mit einem optimierten Zero-Thinking-Adapter für **Google Gemini** (`gemini-flash-lite-latest` / `gemini-2.0-flash-lite`), mit erweiterbaren Verträgen für OpenAI, Claude und DeepSeek (noch nicht implementiert).
- **Einstellung des Übersetzungstons:** Lässig: informeller Gaming-Slang und taktischer Gamer-Jargon / Normal: natürlicher Konversationston ohne Gaming-Slang.
- **Globaler Pause / Fortsetzen-Hotkey:** Drücken Sie jederzeit `Strg + F10` (oder Ihre konfigurierte Tastenkombination), um den Assistenten zwischen **Aktiv** und **Pausiert** umzuschalten. Das Pausieren setzt sowohl die Chat-Übersetzung im Spiel als auch das Senden von Schnell-Chat-Nachrichten sauber aus.
- **Dedizierte Funktions-Hotkeys:**
  - **Nur Übersetzung umschalten:** Drücken Sie `Strg + F11` (oder Ihre aufgenommene Tastenkombination), um ausschließlich die Chat-Übersetzung sofort zu pausieren oder fortzusetzen, ohne den Schnell-Chat zu beeinflussen (mit Toast-Benachrichtigung auf dem Bildschirm).
  - **Nur Schnell-Chat umschalten:** Drücken Sie `Strg + F12` (oder Ihre aufgenommene Tastenkombination), um ausschließlich Schnell-Chat-Funksprüche sofort zu pausieren oder fortzusetzen, ohne die Übersetzung zu beeinflussen (mit Toast-Benachrichtigung auf dem Bildschirm).

![Einstellungen](screenshots/settings.png)

- **Sichere Speicherung von Anmeldedaten (DPAPI):** Sensible Einstellungen wie API-Tokens des KI-Providers werden im Ruhezustand mithilfe der Windows Data Protection API (DPAPI, `CurrentUser`-Bereich) verschlüsselt. Anmeldedaten werden niemals im Klartext in Konfigurationsdateien gespeichert und können nur auf Ihrem lokalen Computer von Ihrem Windows-Konto entschlüsselt werden.
- **Keine Telemetrie & vollständige Privatsphäre:** Dieses Tool erfasst, protokolliert oder sendet keinerlei Benutzerdaten oder Telemetriedaten. Alle Übersetzungsanfragen kommunizieren direkt und ausschließlich mit dem Endpunkt Ihres konfigurierten KI-Providers unter Verwendung Ihrer persönlichen API-Anmeldedaten.
- **100 % nicht-invasiv (Keine Änderung von Spieldateien):** Es werden keine Spielarchive entpackt oder modifiziert. Ihre Spieldateien bleiben zu 100 % im Originalzustand (Vanilla) und unberührt.




## 2. Einrichtung

### Voraussetzungen
Wenn Sie die vorkompilierte Standalone-Version ausführen (`HD1-Babel.exe`):
- **Betriebssystem:** Windows 10 / 11 64-Bit (x64).
- **Desktop-Laufzeitumgebung:** [Microsoft .NET 10 Desktop-Runtime (x64)](https://aka.ms/dotnet/10.0/windowsdesktop-runtime-win-x64.exe) (Direkt-Installer).
- **Zielspiel:** *Helldivers* (2015) auf Steam (64-Bit `helldivers.exe`).

### Kostenlosen KI-API-Schlüssel einrichten
- **KI-Provider-Anmeldedaten:** Google Gemini API-Schlüssel (erhalten Sie einen kostenlosen persönlichen Schlüssel in [Google AI Studio](https://aistudio.google.com/) ([siehe dieses Video-Tutorial](https://www.youtube.com/watch?v=YHFMrZD-_3g))).
- **Internetzugang:** Erforderlich für API-Aufrufe zur Übersetzung.

### Download & Installation (Fertiges Release)

Sie müssen **keine** Entwicklertools installieren oder den Quellcode kompilieren, um Babel zu verwenden ([es sei denn, Sie möchten das](#4-entwicklung-aus-dem-quellcode-bauen)):
1. Navigieren Sie zum Bereich **[Releases](https://github.com/difinim/helldivers1-babel/releases)** auf der rechten Seite der GitHub-Projektseite.
2. Klappen Sie unter dem neuesten Release die Liste **Assets** auf.
3. Laden Sie das eigenständige Distributionsarchiv herunter: **`Helldivers1Babel-v<version>-win-x64.zip`**.
4. Entpacken Sie die ZIP-Datei an einem beliebigen Ort auf Ihrem Computer (z. B. `C:\GamesTools\Helldivers1Babel`).
5. Öffnen Sie den Ordner und starten Sie **`HD1-Babel.exe`**.

> [!NOTE]
> **Voraussetzung:** Endbenutzer benötigen lediglich die kostenlose [Microsoft .NET 10 Desktop-Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/10.0). Es ist kein C++-Compiler, SDK oder eine Toolchain erforderlich, um das Release auszuführen.

> [!NOTE]
> **Bitte beachten:** Da `InjectorService.cs` standardmäßige Windows-Prozessinjektions-APIs verwendet (`VirtualAllocEx` + `WriteProcessMemory` + `CreateRemoteThread`), schlägt Windows Defender oder ein Drittanbieter-Antivirenprogramm _**möglicherweise**_ mit einer heuristischen Erkennung an (z. B. Generic HackTool). Dies ist ein normales Verhalten für Injektoren von Spiel-Modifikationen und kann bedenkenlos zur Ausnahmeliste hinzugefügt werden.



## 3. Benutzerhandbuch

### Erste Schritte
1. Starten Sie **Helldivers 1 Babel** (`HD1-Babel.exe`).
2. Öffnen Sie beim ersten Start den Reiter **Einstellungen**:
   - Geben Sie Ihren **Google Gemini API-Schlüssel** ein (wird sicher per Windows DPAPI-Verschlüsselung gespeichert).
   - Wählen Sie Ihre Zielsprache aus (z. B. `Portuguese (PT-BR)` oder `Deutsch`).
   - Klicken Sie auf **API-Verbindung testen**, um Ihren Schlüssel und die Latenz zu überprüfen.
   - Passen Sie bei Bedarf die Tastenkombination für **Pause / Fortsetzen** an (Standard ist `Strg + F10`).
3. Konfigurieren Sie Ihre taktischen Funksprüche im Reiter **Schnell-Chat** (z. B. Nummernblock 0–9 mit Ihren bevorzugten Nachrichten belegen).
4. Starten Sie **Helldivers** auf Steam. Babel erkennt `helldivers.exe` automatisch.

### Kommunikation im Spiel & Anwendungsfälle

#### 1. Automatische eingehende Übersetzung
Eingehende Nachrichten von Teammitgliedern werden in Echtzeit erkannt und in Ihre Zielsprache übersetzt, wobei der ursprüngliche Spielername und die Spielerfarbe im Chat beibehalten werden:
- Teammitglied (Englisch): `so quiet`
- Chat im Spiel: `Diveheller: (English): mó silêncio`

#### 2. Automatische Antwort auf die letzte eingehende Sprache (`>>nachricht`)
Wenn Sie Ihrer Chat-Nachricht ein `>>` voranstellen, wird Ihr Text automatisch in die Sprache der letzten eingehenden Nachricht eines Teammitglieds übersetzt:
- Ihre Eingabe im Chat: `>>sim, está calmo demais`
- Teammitglied sieht: `yes, it is too quiet`

#### 3. Explizite / Beliebige Sprachauswahl (`zielsprache>>nachricht`)
Richten Sie Ihre Nachricht jederzeit gezielt an eine bestimmte Sprache, indem Sie den Namen der Sprache gefolgt von `>>` voranstellen:

| Chat-Eingabebefehl | Zielsprache | Beispielergebnis für Teammitglieder |
| :--- | :--- | :--- |
| `russian>>bom dia!` | Russisch | `доброе утро!` |
| `english>>minas ao norte` | Englisch | `mines to the north` |
| `japanese>>cuidado com os tiros!` | Japanisch | `射線に注意せよ！` |
| `spanish>>preciso de munição` | Spanisch | `necesito munición` |
| `german>>recuem para o ponto de extração` | Deutsch | `Rückzug zum Extraktionspunkt` |
| `french>>vamos defender aqui` | Französisch | `défendons cette position` |

#### 4. Standardmäßige ausgehende Nachrichten & Unterdrückung der Selbstübersetzung
Wenn Sie eine normale Spielchat-Nachricht ohne `>>` eingeben (z. B. `need ammo`, `vamos lá`), wird Ihre Nachricht unverändert über den normalen Mehrspieler-Chat gesendet. Wenn die Option **„Standardmäßige ausgehende Nachrichten nicht übersetzen“** aktiviert ist (standardmäßig AKTIV in der Schnellsteuerungsleiste und in den Einstellungen), unterdrückt Babel die Rückübersetzung Ihrer eigenen Nachricht an Sie selbst. Wenn sie deaktiviert ist, verarbeitet Babel auch normale lokale Nachrichten über die Pipeline für eingehende Übersetzungen.

#### 5. Taktische Schnell-Chat-Tastenkombinationen
Drücken Sie während des Spielens eine beliebige konfigurierte Schnell-Chat-Taste (z. B. `Nummernblock 1`–`Nummernblock 9`), um vordefinierte Funksprüche sofort im Multiplayer-Chat zu senden, ohne das Chat-Eingabefeld zu öffnen. Alle Tastenanschläge werden nahtlos an das Spiel weitergeleitet, sodass Ihre Steuerung stets reaktionsschnell bleibt.

#### 6. Taktisches Schnell-Chat-Referenz-Overlay (`Nummernblock +`)
Drücken Sie jederzeit `Nummernblock +` (oder Ihre im Reiter **Schnell-Chat** konfigurierte Tastenkombination), um ein On-Screen-Referenz-Overlay mit allen aktiven Schnell-Chat-Vorlagen ein- oder auszublenden. Das Overlay wird rechtsbündig und vertikal zentriert auf dem Bildschirm angezeigt und listet jede aktive Vorlage im Muster `[<tastenkombination>] <nachricht>` auf (Nachrichten werden auf 30 Zeichen gekürzt) – im selben unaufdringlichen Design wie die Benachrichtigungen.
> [!NOTE]
> **Anzeigemodus-Voraussetzung:** Helldivers muss in den Videoeinstellungen des Spiels auf **Randloses Vollbild** (oder Fenstermodus) eingestellt sein, damit Windows-Overlays über dem Spiel gerendert werden können.

#### 7. Pause, Fortsetzen & Funktionsschalter
- **Globales Pause / Fortsetzen (`Strg + F10`):** Hauptschalter, der sowohl das Abfangen von Übersetzungen als auch das Senden von Schnell-Chat-Nachrichten sauber pausiert bzw. fortsetzt.
- **Nur Übersetzung umschalten (`Strg + F11`):** Schaltet nur die Übersetzungsfunktion mit einer Toast-Benachrichtigung auf dem Bildschirm ein oder aus. Der Schnell-Chat funktioniert normal weiter.
- **Nur Schnell-Chat umschalten (`Strg + F12`):** Schaltet nur das Senden von Schnell-Chat-Nachrichten mit einer Toast-Benachrichtigung auf dem Bildschirm ein oder aus. Die Chat-Übersetzung im Spiel funktioniert normal weiter.
*(Alle Tastenkombinationen können in den Reitern **Einstellungen** und **Schnell-Chat** über die Schaltflächen zur Direktaufnahme angepasst werden).*

#### 8. Exportieren & persistente Chat-Protokolle
Klicken Sie im Reiter „Übersetzung & Protokolle“ jederzeit auf **Protokolle exportieren**, um einen Speicherort auszuwählen und Ihren Chatverlauf (mit Zeitstempeln, Spielernamen, Originalnachrichten, Übersetzungen und Latenzen) in ein `.txt`-Dokument zu exportieren. Chat-Nachrichten bleiben über Anwendungsstarts hinweg durch asynchrone Batch-Schreibvorgänge auf der Festplatte erhalten (Speicherlimit ist in den Einstellungen von 0 bis 100 MB konfigurierbar, mit einer Schaltfläche **Protokollspeicher leeren**).

#### 9. Interaktive Nachrichten-Links
Squad-Chat-Nachrichten, die `http://`- oder `https://`-URLs enthalten, werden in der Spalte **Originalnachricht** als anklickbare Links dargestellt. Beim Anklicken werden Sie in einem Bestätigungsdialog gefragt, ob Sie dem Link vertrauen, bevor er in Ihrem Standard-Webbrowser geöffnet wird.



## 4. Entwicklung (Aus dem Quellcode bauen)

### Entwickler-Voraussetzungen
Wenn Sie das Projekt aus dem Quellcode kompilieren und bauen möchten:
- **Betriebssystem:** Windows 10 / 11 64-Bit (x64).
- **.NET 10 SDK:** [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (`net10.0-windows`).
- **MSYS2 MinGW64-Toolchain (Clang++, GCC, Make):** [MSYS2](https://www.msys2.org/) mit den Paketen `mingw-w64-x86_64-clang`, `mingw-w64-x86_64-gcc` und `mingw-w64-x86_64-make` – erforderlich zum Kompilieren der nativen Hook-Bibliothek (`BabelHook.dll`) und zum Erstellen des offiziellen LuaJIT 2.0-Submoduls (`vendor/luajit`), um das spielinterne Bootstrap-Skript (`babel_bootstrap.lua`) in Bytecode zu kompilieren.
- **Git:** Git für Windows (mit rekursiven Submodulen für MinHook-Amalgamation und LuaJIT 2.0).


### Automatische Umgebungseinrichtung
Doppelklicken Sie im Stammverzeichnis des Repositories auf `setup.bat` (oder führen Sie `setup.ps1` in PowerShell aus). Das Skript prüft und installiert fehlende Voraussetzungen automatisch:
1. Prüft die 64-Bit-Windows-Umgebung.
2. Prüft das .NET 10 SDK (automatische Installation über `winget`, falls fehlend).
3. Prüft die MSYS2 MinGW64-Toolchain (Clang++, GCC und Make). Falls fehlend, wird MSYS2 automatisch über `winget` installiert und die erforderlichen Pakete (`mingw-w64-x86_64-clang`, `mingw-w64-x86_64-gcc` und `mingw-w64-x86_64-make`) über `pacman` eingerichtet.
4. Initialisiert und kompiliert den LuaJIT 2.0-Compiler aus dem Submodul (`vendor/luajit`).
5. Stellt automatisch sicher, dass `C:\msys64\mingw64\bin` und `C:\msys64\usr\bin` zur PATH-Umgebungsvariable hinzugefügt werden.
6. Überprüft die MinHook-Amalgamation-Abhängigkeiten und führt einen ersten Build-Plausibilitätstest durch.

### Master-Build & Release-Paketierung
Um vorherige Build-Artefakte zu bereinigen, die native Hook-DLL zu kompilieren, alle Unit-Tests auszuführen und das Release-Paket zusammenzustellen:
```powershell
pwsh -File build.ps1
```
Das Ausgabeverzeichnis wird unter `dist/Helldivers1Babel-vX.Y.Z/` paketiert.

### Manuelle Schritt-für-Schritt-Erstellung
```powershell
# 1. Native Hook-DLL bauen (kompiliert babel_bootstrap.lua automatisch mit luajit)
pwsh -File src/Babel.Hook/build.ps1

# 2. .NET 10-Projektmappe bauen
dotnet build Helldivers1Babel.slnx

# 3. Automatisierte Test-Suites ausführen
dotnet test
```



## 5. Lizenz

Vertrieben unter der [MIT-Lizenz](../LICENSE).
