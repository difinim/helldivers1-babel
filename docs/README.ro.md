# ![Helldivers 1 Babel](screenshots/logo.png)

[![Latest Version](https://img.shields.io/github/v/release/difinim/helldivers1-babel?logo=github&label=release)](https://github.com/difinim/helldivers1-babel/releases/latest)
[![Build Status](https://github.com/difinim/helldivers1-babel/actions/workflows/release.yml/badge.svg)](https://github.com/difinim/helldivers1-babel/actions/workflows/release.yml)
[![Tests](https://github.com/difinim/helldivers1-babel/actions/workflows/tests.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/tests.yml?query=branch%3Amain)
[![CodeQL](https://github.com/difinim/helldivers1-babel/actions/workflows/codeql.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/codeql.yml?query=branch%3Amain)
[![OSV-Scanner](https://github.com/difinim/helldivers1-babel/actions/workflows/osv-scanner.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/osv-scanner.yml?query=branch%3Amain)
[![Dependabot](https://img.shields.io/badge/dependabot-enabled-025E8C?logo=dependabot&logoColor=white)](https://github.com/difinim/helldivers1-babel/network/updates)

*Luptați împreună. Vorbiți pe o singură voce.*

**Helldivers 1 Babel** este un asistent open-source, non-invaziv, de traducere a chat-ului și comunicare tactică pentru *Helldivers (2015)*, pentru că *dulcele gust al libertății sună mult mai bine când toată lumea înțelege planul*.

## Translations

[English](../README.md) | [العربية](README.ar.md) | [বাংলা](README.bn.md) | [简体中文](README.zh.md) | [Čeština](README.cs.md) | [Nederlands](README.nl.md) | [Français](README.fr.md) | [Deutsch](README.de.md) | [Ελληνικά](README.el.md) | [हिन्दी](README.hi.md) | [Magyar](README.hu.md) | [Bahasa Indonesia](README.id.md) | [Italiano](README.it.md) | [日本語](README.ja.md) | [한국어](README.ko.md) | [Bahasa Melayu](README.ms.md) | [فارسی](README.fa.md) | [Polski](README.pl.md) | [Português (PT-BR)](README.pt-BR.md) | [Română](README.ro.md) | [Русский](README.ru.md) | [Español](README.es.md) | [Svenska](README.sv.md) | [Tagalog (Filipino)](README.tl.md) | [ไทย](README.th.md) | [Türkçe](README.tr.md) | [Українська](README.uk.md) | [Tiếng Việt](README.vi.md)

> [!IMPORTANT]
> **Notă privind compatibilitatea:**
> **Helldivers 1 Babel este dezvoltat exclusiv pentru Helldivers (2015) pe PC (Steam).**
> **Este compatibil cu modurile de joc.**
> **NU este compatibil cu Helldivers 2 (2024)**, care este complet diferit și utilizează un sistem anti-cheat la nivel de nucleu (kernel-level).

## Cuprins

- [1. Prezentare generală și caracteristici cheie](#1-prezentare-generală-și-caracteristici-cheie)
- [2. Configurare](#2-configurare)
  - [Cerințe de sistem](#cerințe-de-sistem)
  - [Obținerea unei chei API AI gratuite](#obținerea-unei-chei-api-ai-gratuite)
  - [Descărcare și instalare (Versiune precompilată)](#descărcare-și-instalare-versiune-precompilată)
- [3. Ghid de utilizare](#3-ghid-de-utilizare)
  - [Primii pași](#primii-pași)
  - [Comunicare în joc și cazuri de utilizare](#comunicare-în-joc-și-cazuri-de-utilizare)
- [4. Dezvoltare (Compilare din codul sursă)](#4-dezvoltare-compilare-din-codul-sursă)
  - [Cerințe pentru dezvoltatori](#cerințe-pentru-dezvoltatori)
  - [Configurarea automată a mediului](#configurarea-automată-a-mediului)
  - [Compilare principală și împachetare pentru lansare](#compilare-principală-și-împachetare-pentru-lansare)
  - [Compilare manuală pas cu pas](#compilare-manuală-pas-cu-pas)
- [5. Licență](#5-licență)

## 1. Prezentare generală și caracteristici cheie

- **Traducere bidirecțională a chat-ului în timp real:**
  - **Mesaje primite:** Detectează automat limba coechipierilor, traduce mesajele în limba țintă aleasă de tine (acceptă până la 28 de limbi țintă, inclusiv română, portugheză PT-BR, engleză, spaniolă, rusă, chineză, japoneză, franceză, germană și multe altele) și le afișează **local** în chat-ul din joc, **păstrând cu fidelitate numele original al jucătorului și codul de culoare** (P1 Auriu, P2 Albastru, P3 Mov, P4 Turcoaz).
  - **Mesaje trimise:** Tastează `[limba_țintă]>>mesaj` sau pur și simplu `>>mesaj` pentru a răspunde automat în limba ultimului mesaj primit de la un coechipier. Textul tradus este transmis prin RPC multiplayer, astfel încât coechipierii internaționali îl citesc direct în limba lor maternă.
- **Jurnale de chat persistente și export în fișier:** Urmărirea completă a istoricului conversațiilor din sesiune cu marcaje temporale, nume de jucători, culori, limbi sursă detectate și latența traducerii. Jurnalele persistă în tabel între repornirile aplicației prin scrieri asincrone pe disc în loturi (configurabile), cu export printr-un singur clic într-un fișier `.txt`.
- **Jurnale de chat căutabile:** Istoric complet căutabil cu filtrare pe oricare dintre coloane.

![Helldivers 1 Babel în acțiune](screenshots/screenshot.png)

- **Chat Rapid tactic (10 sloturi personalizabile):** 10 sloturi pentru mesaje tactice predefinite, asociate cu taste rapide configurabile de utilizator (de ex. Numpad 0–9 sau combinații personalizate) pentru a transmite instant comenzi tactice în chat-ul multiplayer din joc.

![Chat Rapid](screenshots/quick-chat.png)

- **Mesaj de bun venit automat în Chat Rapid:** Transmite automat un mesaj de întâmpinare personalizat în chat-ul din joc atunci când găzduiești un lobby și un jucător oaspete se alătură echipei.
- **Overlay de referință pentru Chat Rapid pe ecran:** Comută un overlay plutitor neobstructiv în partea dreaptă a ecranului apăsând `Numpad +` (sau scurtătura configurată) pentru a consulta rapid toate comenzile tactice active și tastele rapide în timpul jocului. *(Necesită ca Helldivers să ruleze în mod Fereastră fără margini sau Fereastră).*

![Overlay Chat Rapid](screenshots/overlay.png)

- **Interfață disponibilă în 28 de limbi:** Localizare completă a interfeței grafice în 28 de limbi.
- **Arhitectură modulară cu adaptoare pentru furnizori AI:** Construită pe o interfață modulară pentru furnizori AI. Vine cu un adaptor optimizat cu latență minimă pentru **Google Gemini** (`gemini-flash-lite-latest` / `gemini-2.0-flash-lite`), precum și contracte extensibile pentru OpenAI, Claude și DeepSeek (în dezvoltare).
- **Setare ton traducere:** Relaxat (jargon informal de gaming și expresii tactice) / Normal (ton conversațional natural fără jargon specific).
- **Scurtătură globală Pauză / Reluare:** Apasă `Ctrl + F10` (sau scurtătura configurată) în orice moment pentru a comuta asistentul între stările **Activ** și **În pauză**. Pauza suspendă în mod curat atât traducerea chat-ului din joc, cât și transmiterea mesajelor de Chat Rapid.
- **Scurtături dedicate pentru funcții individuale:**
  - **Comutare exclusivă traducere:** Apasă `Ctrl + F11` (sau scurtătura configurată) pentru a pune pe pauză sau a relua instantaneu traducerea chat-ului fără a afecta Chatul Rapid (cu notificare pe ecran).
  - **Comutare exclusivă Chat Rapid:** Apasă `Ctrl + F12` (sau scurtătura configurată) pentru a pune pe pauză sau a relua instantaneu mesajele de Chat Rapid fără a afecta traducerea (cu notificare pe ecran).

![Setări](screenshots/settings.png)

- **Stocare securizată a credențialelor (DPAPI):** Datele de configurare sensibile, cum ar fi cheile API pentru furnizorii AI, sunt criptate în repaus utilizând Windows Data Protection API (DPAPI, domeniu `CurrentUser`). Credențialele nu sunt niciodată stocate în text clar în fișierele de configurare și pot fi decriptate doar pe computerul local de către contul tău de utilizator Windows.
- **Fără telemetrie și confidențialitate deplină:** Această aplicație nu colectează, nu stochează și nu trimite date despre utilizatori sau telemetrie. Toate solicitările de traducere comunică direct și exclusiv cu punctul final al furnizorului AI configurat, utilizând cheia API personală.
- **100% non-invaziv (fără modificarea fișierelor jocului):** Nicio arhivă a jocului nu este extrasă sau modificată. Fișierele de instalare ale jocului rămân 100% nemodificate și curate.

## 2. Configurare

### Cerințe de sistem
Dacă rulezi versiunea autonomă precompilată (`HD1-Babel.exe`):
- **Sistem de operare:** Windows 10 / 11 pe 64 de biți (x64).
- **Mediu de execuție desktop:** [Microsoft .NET 10 Desktop Runtime (x64)](https://aka.ms/dotnet/10.0/windowsdesktop-runtime-win-x64.exe) (instalator direct).
- **Joc vizat:** *Helldivers* (2015) pe Steam (`helldivers.exe` pe 64 de biți).

### Obținerea unei chei API AI gratuite
- **Credențiale furnizor AI:** Cheie API Google Gemini (obține o cheie personală gratuită de pe [Google AI Studio](https://aistudio.google.com/) ([Vezi acest ghid video](https://www.youtube.com/watch?v=YHFMrZD-_3g))).
- **Acces la internet:** Necesar pentru apelurile către API-ul de traducere.

### Descărcare și instalare (Versiune precompilată)

**Nu** este nevoie să instalezi instrumente de dezvoltare sau să compilezi din codul sursă pentru a folosi Babel ([decât dacă dorești acest lucru](#4-dezvoltare-compilare-din-codul-sursă)):
1. Navighează la secțiunea **[Releases](https://github.com/difinim/helldivers1-babel/releases)** din partea dreaptă a paginii proiectului pe GitHub.
2. Sub cea mai recentă versiune lansată, extinde lista **Assets**.
3. Descarcă arhiva de distribuție autonomă: **`Helldivers1Babel-v<versiune>-win-x64.zip`**.
4. Extrage fișierul zip oriunde pe computer (de ex. `C:\GamesTools\Helldivers1Babel`).
5. Deschide folderul și rulează **`HD1-Babel.exe`**.

> [!NOTE]
> **Cerință prealabilă:** Utilizatorii finali au nevoie doar de pachetul gratuit [Microsoft .NET 10 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/10.0). Nu este necesar un compilator C++, un SDK sau alte instrumente de compilare.

> [!NOTE]
> **Atenție**: Deoarece `InjectorService.cs` utilizează API-uri standard de injectare de procese Windows (`VirtualAllocEx` + `WriteProcessMemory` + `CreateRemoteThread`), Windows Defender sau alte programe antivirus terțe _**ar putea**_ declanșa o avertizare euristică (de ex. Generic HackTool). Acesta este un comportament normal pentru injectoarele de moduri de joc și fișierul poate fi adăugat în siguranță la lista de excluderi.

## 3. Ghid de utilizare

### Primii pași
1. Pornește **Helldivers 1 Babel** (`HD1-Babel.exe`).
2. La prima lansare, deschide fila **Setări**:
   - Introdu **Cheia API Google Gemini** (stocată în siguranță prin criptarea Windows DPAPI).
   - Selectează limba țintă dorită (de ex. `Română`).
   - Fă clic pe **Test Conexiune API** pentru a verifica funcționarea cheii și latența.
   - Personalizează scurtătura de **Pauză/Reluare** dacă dorești (implicit `Ctrl + F10`).
3. Configurează mesajele tactice în fila **Chat Rapid** (de ex. asociază Numpad 0–9 cu mesajele preferate).
4. Lansează **Helldivers** pe Steam. Babel va detecta automat procesul `helldivers.exe`, va injecta biblioteca `BabelHook.dll` și va stabili o conexiune IPC de mare viteză prin canal de tip named pipe.

### Comunicare în joc și cazuri de utilizare

#### 1. Traducere automată a mesajelor primite
Mesajele primite de la coechipieri sunt detectate și traduse în limba ta țintă în timp real, fiind afișate în chat-ul din joc cu numele și culoarea originală a jucătorului:
- Coechipier (English): `so quiet`
- Chat în joc: `Diveheller: (English): e o liniște mormântală`

#### 2. Răspuns automat în limba ultimului mesaj primit (`>>mesaj`)
Adăugarea prefixului `>>` la începutul mesajului traduce automat textul tău în limba ultimului mesaj primit de la un coechipier:
- Introducerea ta în chat: `>>da, este prea liniște`
- Coechipierul vede: `yes, it is too quiet`

#### 3. Selectare explicită a unei limbi oarecare (`limba țintă>>mesaj`)
Poți trimite un mesaj tradus într-o anumită limbă în orice moment prefixându-l cu numele limbii urmat de `>>`:

| Comandă în chat | Limba țintă | Exemplu rezultat transmis coechipierilor |
| :--- | :--- | :--- |
| `russian>>bom dia!` | Rusă | `доброе утро!` |
| `english>>mine la nord` | Engleză | `mines to the north` |
| `japanese>>atenție la linia de tragere!` | Japoneză | `射線に注意せよ！` |
| `spanish>>am nevoie de muniție` | Spaniolă | `necesito munición` |
| `german>>retragere la punctul de extracție` | Germană | `Rückzug zum Extraktionspunkt` |
| `french>>apărăm această poziție` | Franceză | `défendons cette position` |

#### 4. Mesaje de ieșire standard și suprimarea auto-traducerii
Când tastezi un mesaj obișnuit în chat fără `>>` (de ex. `need ammo`, `haideți`), mesajul este trimis direct prin chat-ul standard din multiplayer. Când opțiunea **„Nu traduce mesajele de ieșire standard”** este activată (activă implicit atât în bara de control rapid, cât și în Setări), Babel suprimă retraducerea propriului mesaj către tine. Dacă dezactivezi această opțiune, Babel va procesa și mesajele locale trimise prin fluxul de traducere a mesajelor primite.

#### 5. Scurtături tactice pentru Chat Rapid
Apasă orice tastă rapidă configurată pentru Chat Rapid (de ex. `NumPad 1`–`NumPad 9`) în timpul jocului pentru a trimite instant mesaje predefinite în chat-ul echipei, fără a deschide caseta de introducere a textului. Toate apăsările de taste sunt transmise direct către joc, asigurând comenzi fluide și responsive.

#### 6. Overlay de referință pentru Chat Rapid pe ecran (`Numpad +`)
Apasă `Numpad +` (sau scurtătura salvată în fila **Chat Rapid**) în orice moment pentru a afișa un overlay de referință pe ecran care listează toate mesajele rapide active. Overlay-ul apare aliniat la dreapta și centrat vertical pe ecran, afișând fiecare comandă activă în formatul `[<scurtătură>] <mesaj>` (mesajele fiind trunchiate la 30 de caractere) în același stil vizual discret ca al notificărilor toast.
> [!NOTE]
> **Cerință mod afișaj:** În setările video ale jocului Helldivers, modul de afișare trebuie setat pe **Fereastră fără margini** (Borderless Fullscreen) sau **Fereastră** (Windowed) pentru ca ferestrele de tip overlay din Windows să poată fi afișate peste joc.

#### 7. Pauză, reluare și comutatoare de funcții
- **Pauză / Reluare globală (`Ctrl + F10`):** Comutatorul principal care suspendă/relucrează atât interceptarea traducerilor, cât și transmiterea mesajelor de Chat Rapid.
- **Comutare exclusivă traducere (`Ctrl + F11`):** Activează sau dezactivează exclusiv traducerea chat-ului, afișând o notificare pe ecran. Chatul Rapid continuă să funcționeze normal.
- **Comutare exclusivă Chat Rapid (`Ctrl + F12`):** Activează sau dezactivează exclusiv transmiterea mesajelor de Chat Rapid, afișând o notificare pe ecran. Traducerea chat-ului continuă să funcționeze normal.
*(Toate scurtăturile pot fi personalizate în filele **Setări** și **Chat Rapid** prin simpla apăsare a butoanelor de înregistrare).*

#### 8. Export și persistența jurnalelor de chat
Fă clic pe **Exportă Jurnale** în fila „Traducere și Jurnale” în orice moment pentru a alege o locație și a salva istoricul chat-ului (cu marcaje temporale, nume de jucători, mesaje originale, traduceri și latențe) într-un fișier `.txt`. Mesajele de chat se păstrează automat în tabel între lansările aplicației prin scrieri asincrone pe disc în loturi (limita de stocare este configurabilă de la 0 la 100 MB în Setări, fiind disponibil și un buton **Curăță memoria jurnalelor**).

#### 9. Linkuri interactive în mesaje
Mesajele din chat-ul echipei care conțin adrese URL `http://` sau `https://` sunt afișate ca linkuri interactive pe care se poate face clic în coloana **Mesaj Original**. La accesarea unui link, un dialog de confirmare solicită verificarea înainte de deschiderea adresei în browserul web implicit.

## 4. Dezvoltare (Compilare din codul sursă)

### Cerințe pentru dezvoltatori
Dacă compilezi și construiești proiectul din codul sursă:
- **Sistem de operare:** Windows 10 / 11 pe 64 de biți (x64).
- **.NET 10 SDK:** [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (`net10.0-windows`).
- **Set de instrumente MSYS2 MinGW64 (Clang++, GCC, Make):** [MSYS2](https://www.msys2.org/) cu pachetele `mingw-w64-x86_64-clang`, `mingw-w64-x86_64-gcc` și `mingw-w64-x86_64-make` — necesare pentru compilarea bibliotecii native de interceptare (`BabelHook.dll`) și compilarea submodulului oficial LuaJIT 2.0 (`vendor/luajit`) pentru a compila scriptul de inițializare (`babel_bootstrap.lua`) în bytecode.
- **Git:** Git for Windows (cu submodule recursive pentru amalgamarea MinHook și LuaJIT 2.0).

### Configurarea automată a mediului
Fă dublu clic pe `setup.bat` (sau rulează `setup.ps1` în PowerShell) în rădăcina depozitului. Acesta verifică și instalează automat componentele necesare:
1. Verifică mediul Windows pe 64 de biți.
2. Verifică prezența .NET 10 SDK (se instalează automat prin `winget` dacă lipsește).
3. Verifică setul de instrumente MSYS2 MinGW64 (Clang++, GCC și Make). Dacă lipsește, instalează automat MSYS2 prin `winget` și pachetele necesare (`mingw-w64-x86_64-clang`, `mingw-w64-x86_64-gcc` și `mingw-w64-x86_64-make`) prin `pacman`.
4. Inițializează și compilează compilatorul LuaJIT 2.0 din submodul (`vendor/luajit`).
5. Se asigură automat că directoarele `C:\msys64\mingw64\bin` și `C:\msys64\usr\bin` sunt adăugate în variabila PATH.
6. Verifică dependențele de amalgamare MinHook și rulează o verificare preliminară a compilării.

### Compilare principală și împachetare pentru lansare
Pentru a curăța artefactele anterioare, a compila biblioteca nativă DLL de hook, a rula toate testele unitare și a asambla pachetul de lansare:
```powershell
pwsh -File build.ps1
```
Directorul rezultat va fi împachetat în `dist/Helldivers1Babel-vX.Y.Z/`.

### Compilare manuală pas cu pas
```powershell
# 1. Compilarea bibliotecii native de hook (compilează automat babel_bootstrap.lua cu luajit)
pwsh -File src/Babel.Hook/build.ps1

# 2. Compilarea soluției .NET 10
dotnet build Helldivers1Babel.slnx

# 3. Rularea suitelor de teste automate
dotnet test
```

## 5. Licență

Distribuit sub [Licența MIT](../LICENSE).
