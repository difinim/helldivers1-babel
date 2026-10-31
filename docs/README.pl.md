# ![Helldivers 1 Babel](screenshots/logo.png)

[![Latest Version](https://img.shields.io/github/v/release/difinim/helldivers1-babel?logo=github&label=release)](https://github.com/difinim/helldivers1-babel/releases/latest)
[![Build Status](https://github.com/difinim/helldivers1-babel/actions/workflows/release.yml/badge.svg)](https://github.com/difinim/helldivers1-babel/actions/workflows/release.yml)
[![Tests](https://github.com/difinim/helldivers1-babel/actions/workflows/tests.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/tests.yml?query=branch%3Amain)
[![CodeQL](https://github.com/difinim/helldivers1-babel/actions/workflows/codeql.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/codeql.yml?query=branch%3Amain)
[![OSV-Scanner](https://github.com/difinim/helldivers1-babel/actions/workflows/osv-scanner.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/osv-scanner.yml?query=branch%3Amain)
[![Dependabot](https://img.shields.io/badge/dependabot-enabled-025E8C?logo=dependabot&logoColor=white)](https://github.com/difinim/helldivers1-babel/network/updates)

*Skaczcie razem. Mówcie jednym głosem.*

**Helldivers 1 Babel** to otwartoźródłowy, bezinwazyjny asystent tłumaczenia czatu i komunikacji taktycznej dla *Helldivers (2015)*, ponieważ *słodka wolność brzmi lepiej, gdy każdy rozumie plan*.

## Translations

[English](../README.md) | [العربية](README.ar.md) | [বাংলা](README.bn.md) | [简体中文](README.zh.md) | [Čeština](README.cs.md) | [Nederlands](README.nl.md) | [Français](README.fr.md) | [Deutsch](README.de.md) | [Ελληνικά](README.el.md) | [हिन्दी](README.hi.md) | [Magyar](README.hu.md) | [Bahasa Indonesia](README.id.md) | [Italiano](README.it.md) | [日本語](README.ja.md) | [한국어](README.ko.md) | [Bahasa Melayu](README.ms.md) | [فارسی](README.fa.md) | [Polski](README.pl.md) | [Português (PT-BR)](README.pt-BR.md) | [Română](README.ro.md) | [Русский](README.ru.md) | [Español](README.es.md) | [Svenska](README.sv.md) | [Tagalog (Filipino)](README.tl.md) | [ไทย](README.th.md) | [Türkçe](README.tr.md) | [Українська](README.uk.md) | [Tiếng Việt](README.vi.md)

> [!IMPORTANT]
> **Zastrzeżenie dotyczące kompatybilności:**
> **Helldivers 1 Babel został stworzony wyłącznie dla gry Helldivers (2015) na PC (Steam).**
> **Jest kompatybilny z modami.**
> **NIE jest kompatybilny z Helldivers 2 (2024)**, który jest zupełnie inną grą i korzysta z systemu anti-cheat na poziomie jądra systemu (kernel-level).

## Spis treści

- [1. Przegląd i główne funkcje](#1-przegląd-i-główne-funkcje)
- [2. Konfiguracja](#2-konfiguracja)
  - [Wymagania](#wymagania)
  - [Uzyskanie bezpłatnego klucza API AI](#uzyskanie-bezpłatnego-klucza-api-ai)
  - [Pobieranie i instalacja (Gotowe wydanie)](#pobieranie-i-instalacja-gotowe-wydanie)
- [3. Instrukcja obsługi](#3-instrukcja-obsługi)
  - [Pierwsze kroki](#pierwsze-kroki)
  - [Komunikacja w grze i przykłady użycia](#komunikacja-w-grze-i-przykłady-użycia)
- [4. Programowanie (Kompilacja ze źródeł)](#4-programowanie-kompilacja-ze-źródeł)
  - [Wymagania deweloperskie](#wymagania-deweloperskie)
  - [Automatyczna konfiguracja środowiska](#automatyczna-konfiguracja-środowiska)
  - [Główny proces budowania i tworzenie paczki wydania](#główny-proces-budowania-i-tworzenie-paczki-wydania)
  - [Ręczna kompilacja krok po kroku](#ręczna-kompilacja-krok-po-kroku)
- [5. Licencja](#5-licencja)

## 1. Przegląd i główne funkcje

- **Dwukierunkowe tłumaczenie czatu w czasie rzeczywistym:**
  - **Wiadomości przychodzące:** Automatycznie wykrywa język członków drużyny, tłumaczy wiadomości na wybrany przez Ciebie język docelowy (obsługuje do 28 języków, w tym portugalski PT-BR, angielski, hiszpański, rosyjski, chiński, japoński, francuski, niemiecki i inne) i wyświetla je **lokalnie** na czacie w grze, **wiernie zachowując oryginalną nazwę gracza oraz kod koloru członka drużyny** (P1 Złoty, P2 Niebieski, P3 Fioletowy, P4 Cyjan).
  - **Wiadomości wychodzące:** Wpisz `[język_docelowy]>>wiadomość` lub po prostu `>>wiadomość`, aby automatycznie odpowiedzieć w języku ostatniej odebranej wiadomości sojusznika. Przetłumaczony tekst jest transmitowany przez protokół sieciowy RPC, dzięki czemu zagraniczni sojusznicy odczytują go w swoim ojczystym języku.
- **Trwałe dzienniki czatu i eksport do pliku:** Pełne śledzenie historii czatu z sesji ze znacznikami czasu, nazwami graczy, kolorami, wykrytymi językami źródłowymi oraz opóźnieniem tłumaczenia. Dzienniki są zachowywane w tabeli pomiędzy uruchomieniami aplikacji dzięki lekkim, asynchronicznym zapisom wsadowym na dysk (konfigurowalne), z możliwością eksportu do pliku `.txt` jednym kliknięciem.
- **Przeszukiwalne dzienniki czatu:** W pełni przeszukiwalna historia czatu z filtrowaniem według dowolnych kolumn.

![Helldivers 1 Babel w akcji](screenshots/screenshot.png)

- **Taktyczny Szybki Czat (10 własnych slotów):** 10 konfigurowalnych, predefiniowanych slotów komunikatów taktycznych, przypisanych do skrótów klawiszowych (np. Numpad 0-9 lub własne kombinacje), pozwalających na natychmiastowe nadawanie meldunków taktycznych na czacie wieloosobowym w grze.

![Szybki czat](screenshots/quick-chat.png)

- **Automatyczne powitanie gospodarza:** Automatycznie wysyła konfigurowalną wiadomość powitalną na czacie gry, gdy prowadzisz poczekalnię (jesteś hostem) i dołącza do niej nowy gracz.
- **Ekranowa nakładka referencyjna Szybkiego Czatu:** Włącz/wyłącz niefokusującą, pływającą nakładkę referencyjną po prawej stronie ekranu za pomocą `Numpad +` (lub przypisanego skrótu), aby szybko podejrzeć w grze wszystkie aktywne meldunki taktyczne i skróty. *(Wymaga uruchomienia Helldivers w trybie okna bez ramek lub w oknie).*

![Nakładka Szybkiego Czatu](screenshots/overlay.png)

- **Obsługa wielojęzycznego interfejsu użytkownika:** Obsługuje 28 języków.
- **Modułowa architektura dostawców AI:** Zbudowana w oparciu o modułowy interfejs adapterów AI. Zawiera zoptymalizowany adapter dla **Google Gemini** (`gemini-flash-lite-latest` / `gemini-2.0-flash-lite`) o zerowym opóźnieniu myślenia, z możliwością łatwego rozszerzenia o OpenAI, Claude oraz DeepSeek (jeszcze niezaimplementowane).
- **Ustawienie stylu tłumaczenia:** Swobodny: nieformalny slang i taktyczny żargon graczy / Zwykły: naturalny ton konwersacyjny bez slangu graczy.
- **Globalny skrót Wstrzymaj / Wznów:** Naciśnij w dowolnym momencie `Ctrl + F10` (lub skonfigurowany skrót), aby przełączyć asystenta pomiędzy stanem **Aktywny** a **Wstrzymano**. Wstrzymanie prawidłowo zawiesza zarówno tłumaczenie czatu w grze, jak i wysyłanie wiadomości Szybkiego Czatu.
- **Dedykowane skróty funkcji:**
  - **Przełącz tylko tłumaczenie:** Naciśnij `Ctrl + F11` (lub nagrany skrót), aby natychmiast wstrzymać lub wznowić samo tłumaczenie czatu bez wpływu na Szybki Czat (z powiadomieniem na ekranie).
  - **Przełącz tylko Szybki Czat:** Naciśnij `Ctrl + F12` (lub nagrany skrót), aby natychmiast wstrzymać lub wznowić wysyłanie wiadomości Szybkiego Czatu bez wpływu na tłumaczenie (z powiadomieniem na ekranie).

![Ustawienia](screenshots/settings.png)

- **Bezpieczne przechowywanie danych uwierzytelniających (DPAPI):** Wrażliwe ustawienia, takie jak tokeny API dostawców AI, są szyfrowane na dysku za pomocą interfejsu Windows Data Protection API (DPAPI, zakres `CurrentUser`). Dane logowania nigdy nie są zapisywane w postaci jawnego tekstu w plikach konfiguracyjnych i mogą zostać odszyfrowane wyłącznie na Twoim komputerze przez Twoje konto Windows.
- **Zero telemetrii i pełna prywatność:** To narzędzie nie zbiera, nie rejestruje ani nie wysyła żadnych danych użytkownika ani danych telemetrycznych. Wszystkie żądania tłumaczenia trafiają bezpośrednio i wyłącznie do skonfigurowanego punktu końcowego dostawcy AI przy użyciu Twoich prywatnych danych API.
- **W 100% bezinwazyjne (Zero modyfikacji plików gry):** Żadne archiwa gry nie są rozpakowywane ani modyfikowane. Pliki gry pozostają w 100% oryginalne (vanilla) i nienaruszone.




## 2. Konfiguracja

### Wymagania
Jeśli korzystasz ze skompilowanego, samodzielnego wydania (`HD1-Babel.exe`):
- **System operacyjny:** Windows 10 / 11 64-bit (x64).
- **Środowisko uruchomieniowe:** [Microsoft .NET 10 Desktop Runtime (x64)](https://aka.ms/dotnet/10.0/windowsdesktop-runtime-win-x64.exe) (bezpośredni instalator).
- **Obsługiwana gra:** *Helldivers* (2015) na Steam (wersja 64-bitowa `helldivers.exe`).

### Uzyskanie bezpłatnego klucza API AI
- **Klucz API dostawcy AI:** Klucz Google Gemini API (darmowy klucz osobisty można uzyskać w [Google AI Studio](https://aistudio.google.com/) ([zobacz poradnik wideo](https://www.youtube.com/watch?v=YHFMrZD-_3g))).
- **Dostęp do Internetu:** Wymagany do wykonywania połączeń z interfejsem API tłumaczeń.

### Pobieranie i instalacja (Gotowe wydanie)

**Nie** musisz instalować narzędzi programistycznych ani kompilować ze źródeł, aby korzystać z Babel ([chyba że chcesz](#4-programowanie-kompilacja-ze-źródeł)):
1. Przejdź do sekcji **[Releases](https://github.com/difinim/helldivers1-babel/releases)** po prawej stronie strony projektu na GitHubie.
2. W sekcji najnowszego wydania rozwiń listę **Assets**.
3. Pobierz samodzielne archiwum dystrybucyjne: **`Helldivers1Babel-v<version>-win-x64.zip`**.
4. Wypakuj plik zip w dowolnym miejscu na komputerze (np. `C:\GamesTools\Helldivers1Babel`).
5. Otwórz folder i uruchom **`HD1-Babel.exe`**.

> [!NOTE]
> **Wymagania wstępne:** Użytkownicy końcowi potrzebują jedynie bezpłatnego środowiska [Microsoft .NET 10 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/10.0). Do uruchomienia wydania nie jest wymagany żaden kompilator C++, SDK ani zestaw narzędzi programistycznych.

> [!NOTE]
> **Ważna informacja:** Ponieważ `InjectorService.cs` korzysta ze standardowych funkcji API iniekcji procesów systemu Windows (`VirtualAllocEx` + `WriteProcessMemory` + `CreateRemoteThread`), Windows Defender lub antywirusy innych firm _**mogą**_ wywołać alert heurystyczny (np. Generic HackTool). Jest to normalne zachowanie w przypadku injectorów modów do gier i plik można bezpiecznie dodać do wyjątków.



## 3. Instrukcja obsługi

### Pierwsze kroki
1. Uruchom program **Helldivers 1 Babel** (`HD1-Babel.exe`).
2. Przy pierwszym uruchomieniu przejdź do zakładki **Ustawienia**:
   - Wprowadź swój **klucz API Google Gemini** (bezpiecznie szyfrowany przez Windows DPAPI).
   - Wybierz język docelowy (np. `Portuguese (PT-BR)` lub `Polski`).
   - Kliknij **Testuj połączenie API**, aby zweryfikować poprawność klucza oraz opóźnienie.
   - W razie potrzeby dostosuj skrót **Wstrzymaj / Wznów** (domyślnie `Ctrl + F10`).
3. Skonfiguruj swoje meldunki taktyczne w zakładce **Szybki czat** (np. przypisz klawisze Numpad 0-9 do wybranych wiadomości).
4. Uruchom grę **Helldivers** na platformie Steam. Babel automatycznie wykryje proces `helldivers.exe`, wstrzyknie bibliotekę `BabelHook.dll` i ustanowi szybką komunikację IPC przez nazwane potoki (named pipes).

### Komunikacja w grze i przykłady użycia

#### 1. Automatyczne tłumaczenie wiadomości przychodzących
Wiadomości przychodzące od członków drużyny są wykrywane i tłumaczone na bieżąco na Twój język docelowy, a następnie wyświetlane na czacie w grze z zachowaniem oryginalnej nazwy gracza i koloru:
- Sojusznik (angielski): `so quiet`
- Czat w grze: `Diveheller: (English): mó silêncio`

#### 2. Automatyczna odpowiedź w języku ostatniej wiadomości (`>>wiadomość`)
Poprzedzenie wiadomości prefiksem `>>` powoduje automatyczne przetłumaczenie Twojego tekstu na język ostatniej wiadomości odebranej od członka drużyny:
- Wpisujesz na czacie: `>>sim, está calmo demais`
- Sojusznik widzi: `yes, it is too quiet`

#### 3. Jawny / Dowolny wybór języka (`język docelowy>>wiadomość`)
Skieruj swoją wiadomość do określonego języka w dowolnym momencie, poprzedzając ją nazwą języka i znakami `>>`:

| Polecenie na czacie | Język docelowy | Przykładowy wynik wysłany do drużyny |
| :--- | :--- | :--- |
| `russian>>bom dia!` | Rosyjski | `доброе утро!` |
| `english>>minas ao norte` | Angielski | `mines to the north` |
| `japanese>>cuidado com os tiros!` | Japoński | `射線に注意せよ！` |
| `spanish>>preciso de munição` | Hiszpański | `necesito munición` |
| `german>>recuem para o ponto de extração` | Niemiecki | `Rückzug zum Extraktionspunkt` |
| `french>>vamos defender aqui` | Francuski | `défendons cette position` |

#### 4. Standardowe wiadomości wychodzące i brak samoczynnego tłumaczenia
Gdy wpisujesz zwykłą wiadomość na czacie w grze bez prefiksu `>>` (np. `need ammo`, `vamos lá`), Twoja wiadomość jest wysyłana bezpośrednio przez standardowy czat wieloosobowy. Przy włączonej opcji **„Nie tłumacz standardowych wychodzących”** (włączona domyślnie zarówno na pasku szybkiej kontroli, jak i w Ustawieniach), Babel zapobiega tłumaczeniu Twojej własnej wiadomości z powrotem do Ciebie. Po wyłączeniu tej opcji Babel przetwarza również standardowe wiadomości lokalne przez kanał tłumaczenia przychodzącego.

#### 5. Skróty klawiszowe taktycznego Szybkiego Czatu
Naciśnij dowolny skonfigurowany klawisz szybkiego czatu (np. `NumPad 1`–`NumPad 9`) podczas rozgrywki, aby natychmiast nadać zdefiniowany komunikat na czacie drużynowym bez otwierania okna wprowadzania tekstu. Wszystkie naciśnięcia klawiszy są bez zakłóceń przekazywane do gry, zapewniając pełną responsywność sterowania.

#### 6. Ekranowa nakładka referencyjna Szybkiego Czatu (`Numpad +`)
W dowolnym momencie naciśnij `Numpad +` (lub przypisany skrót skonfigurowany w zakładce **Szybki czat**), aby włączyć lub wyłączyć ekranową nakładkę referencyjną ze wszystkimi aktywnymi wpisami Szybkiego Czatu. Nakładka pojawia się wyrównana do prawej krawędzi i wyśrodkowana w pionie na ekranie, prezentując każdy aktywny szablon we wzorcu `[<skrót>] <wiadomość>` (wiadomości są skracane do 30 znaków) w takim samym subtelnym stylu wizualnym jak powiadomienia.
> [!NOTE]
> **Wymóg trybu wyświetlania:** Helldivers musi być uruchomiony w trybie **Okno bez ramek** (lub W oknie) w ustawieniach wideo gry, aby nakładki systemowe Windows mogły być renderowane na tle gry.

#### 7. Wstrzymywanie, wznawianie i przełączniki funkcji
- **Globalne Wstrzymaj / Wznów (`Ctrl + F10`):** Główny przełącznik, który całkowicie wstrzymuje lub wznawia zarówno przechwytywanie tłumaczeń, jak i wysyłanie wiadomości Szybkiego Czatu.
- **Przełącz tylko tłumaczenie (`Ctrl + F11`):** Włącza lub wyłącza wyłącznie funkcję tłumaczenia z powiadomieniem na ekranie. Szybki czat działa dalej normalnie.
- **Przełącz tylko Szybki Czat (`Ctrl + F12`):** Włącza lub wyłącza wyłącznie nadawanie komunikatów Szybkiego Czatu z powiadomieniem na ekranie. Tłumaczenie czatu w grze działa dalej normalnie.
*(Wszystkie skróty można dostosować w zakładkach **Ustawienia** oraz **Szybki czat** za pomocą przycisków rejestrowania skrótów).*

#### 8. Eksport i trwałe dzienniki czatu
Kliknij **Eksportuj dzienniki** w zakładce Tłumaczenie i Dzienniki w dowolnym momencie, aby wybrać folder i wyeksportować historię czatu (ze znacznikami czasu, nazwami graczy, oryginalnymi wiadomościami, tłumaczeniami i opóźnieniami) do pliku `.txt`. Wiadomości z czatu są automatycznie zachowywane w tabeli pomiędzy uruchomieniami aplikacji dzięki asynchronicznym zapisom wsadowym na dysk (limit miejsca można konfigurować w Ustawieniach od 0 do 100 MB, z przyciskiem **Wyczyść pamięć dzienników**).

#### 9. Interaktywne łącza w wiadomościach
Wiadomości na czacie drużyny zawierające linki `http://` lub `https://` są renderowane jako klikalne łącza w kolumnie **Wiadomość oryginalna**. Po kliknięciu pojawi się okno dialogowe z pytaniem o potwierdzenie, czy ufasz witrynie, przed otwarciem jej w domyślnej przeglądarce internetowej.



## 4. Programowanie (Kompilacja ze źródeł)

### Wymagania deweloperskie
Jeśli kompilujesz i budujesz projekt ze źródeł:
- **System operacyjny:** Windows 10 / 11 64-bit (x64).
- **.NET 10 SDK:** [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (`net10.0-windows`).
- **Zestaw narzędzi MSYS2 MinGW64 (Clang++, GCC, Make):** [MSYS2](https://www.msys2.org/) z pakietami `mingw-w64-x86_64-clang`, `mingw-w64-x86_64-gcc` oraz `mingw-w64-x86_64-make` – wymagane do kompilacji natywnej biblioteki hooka (`BabelHook.dll`) oraz zbudowania oficjalnego submodułu LuaJIT 2.0 (`vendor/luajit`) w celu skompilowania skryptu startowego w grze (`babel_bootstrap.lua`) do kodu bajtowego.
- **Git:** Git dla Windows (z rekursywnymi submodułami dla MinHook amalgamation i LuaJIT 2.0).


### Automatyczna konfiguracja środowiska
Kliknij dwukrotnie plik `setup.bat` (lub uruchom `setup.ps1` w programie PowerShell) w katalogu głównym repozytorium. Automatycznie zweryfikuje i zainstaluje brakujące komponenty:
1. Weryfikuje 64-bitowe środowisko Windows.
2. Sprawdza obecność .NET 10 SDK (automatyczna instalacja przez `winget` w przypadku braku).
3. Sprawdza zestaw narzędzi MSYS2 MinGW64 (Clang++, GCC i Make). W przypadku braku automatycznie instaluje MSYS2 za pomocą `winget` oraz instaluje wymagane pakiety (`mingw-w64-x86_64-clang`, `mingw-w64-x86_64-gcc` i `mingw-w64-x86_64-make`) przez `pacman`.
4. Inicjalizuje i kompiluje kompilator LuaJIT 2.0 z submodułu (`vendor/luajit`).
5. Automatycznie dba o dodanie `C:\msys64\mingw64\bin` oraz `C:\msys64\usr\bin` do zmiennej środowiskowej PATH.
6. Weryfikuje zależności MinHook amalgamation i przeprowadza wstępną kontrolę poprawności budowania.

### Główny proces budowania i tworzenie paczki wydania
Aby wyczyścić poprzednie pliki tymczasowe, skompilować natywną bibliotekę hooka DLL, uruchomić wszystkie testy jednostkowe i przygotować paczkę wydania:
```powershell
pwsh -File build.ps1
```
Katalog wyjściowy zostanie spakowany do folderu `dist/Helldivers1Babel-vX.Y.Z/`.

### Ręczna kompilacja krok po kroku
```powershell
# 1. Zbuduj natywną bibliotekę DLL hooka (automatycznie kompiluje babel_bootstrap.lua za pomocą luajit)
pwsh -File src/Babel.Hook/build.ps1

# 2. Zbuduj rozwiązanie .NET 10
dotnet build Helldivers1Babel.slnx

# 3. Uruchom automatyczne zestawy testów
dotnet test
```



## 5. Licencja

Rozpowszechniane na warunkach [licencji MIT](../LICENSE).
