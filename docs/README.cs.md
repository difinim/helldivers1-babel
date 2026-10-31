# ![Helldivers 1 Babel](screenshots/logo.png)

[![Latest Version](https://img.shields.io/github/v/release/difinim/helldivers1-babel?logo=github&label=release)](https://github.com/difinim/helldivers1-babel/releases/latest)
[![Build Status](https://github.com/difinim/helldivers1-babel/actions/workflows/release.yml/badge.svg)](https://github.com/difinim/helldivers1-babel/actions/workflows/release.yml)
[![Tests](https://github.com/difinim/helldivers1-babel/actions/workflows/tests.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/tests.yml?query=branch%3Amain)
[![CodeQL](https://github.com/difinim/helldivers1-babel/actions/workflows/codeql.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/codeql.yml?query=branch%3Amain)
[![OSV-Scanner](https://github.com/difinim/helldivers1-babel/actions/workflows/osv-scanner.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/osv-scanner.yml?query=branch%3Amain)
[![Dependabot](https://img.shields.io/badge/dependabot-enabled-025E8C?logo=dependabot&logoColor=white)](https://github.com/difinim/helldivers1-babel/network/updates)

*Bojujte společně. Mluvte jedním hlasem.*

**Helldivers 1 Babel** je otevřený, neinvazivní asistent pro překlad herního chatu a taktickou komunikaci pro *Helldivers (2015)*, protože *sladká svoboda zní nejlépe, když všichni rozumí plánu*.

## Translations

[English](../README.md) | [العربية](README.ar.md) | [বাংলা](README.bn.md) | [简体中文](README.zh.md) | [Čeština](README.cs.md) | [Nederlands](README.nl.md) | [Français](README.fr.md) | [Deutsch](README.de.md) | [Ελληνικά](README.el.md) | [हिन्दी](README.hi.md) | [Magyar](README.hu.md) | [Bahasa Indonesia](README.id.md) | [Italiano](README.it.md) | [日本語](README.ja.md) | [한국어](README.ko.md) | [Bahasa Melayu](README.ms.md) | [فارسی](README.fa.md) | [Polski](README.pl.md) | [Português (PT-BR)](README.pt-BR.md) | [Română](README.ro.md) | [Русский](README.ru.md) | [Español](README.es.md) | [Svenska](README.sv.md) | [Tagalog (Filipino)](README.tl.md) | [ไทย](README.th.md) | [Türkçe](README.tr.md) | [Українська](README.uk.md) | [Tiếng Việt](README.vi.md)

> [!IMPORTANT]
> **Upozornění ke kompatibilitě:**
> **Helldivers 1 Babel je vyvíjen výhradně pro Helldivers (2015) na PC (Steam).**
> **Je kompatibilní s modifikacemi.**
> **NENÍ kompatibilní s Helldivers 2 (2024)**, což je zcela odlišná hra využívající anti-cheat na úrovni jádra (kernel-level).

## Obsah

- [1. Přehled a klíčové funkce](#1-přehled-a-klíčové-funkce)
- [2. Instalace a nastavení](#2-instalace-a-nastavení)
  - [Požadavky](#požadavky)
  - [Nastavení bezplatného API klíče AI](#nastavení-bezplatného-api-klíče-ai)
  - [Stažení a instalace (Předpřipravené vydání)](#stažení-a-instalace-předpřipravené-vydání)
- [3. Uživatelská příručka](#3-uživatelská-příručka)
  - [Začínáme](#začínáme)
  - [Komunikace ve hře a případy použití](#komunikace-ve-hře-a-případy-použití)
- [4. Vývoj (Sestavení ze zdrojových kódů)](#4-vývoj-sestavení-ze-zdrojových-kódů)
  - [Požadavky pro vývojáře](#požadavky-pro-vývojáře)
  - [Automatizovaná příprava prostředí](#automatizovaná-příprava-prostředí)
  - [Hlavní sestavení a vytvoření balíčku vydání](#hlavní-sestavení-a-vytvoření-balíčku-vydání)
  - [Manuální sestavení krok za krokem](#manuální-sestavení-krok-za-krokem)
- [5. Licence](#5-licence)

## 1. Přehled a klíčové funkce

- **Obousměrný překlad chatu v reálném čase:**
  - **Příchozí zprávy:** Automaticky detekuje jazyk spoluhráčů, překládá zprávy do zvoleného cílového jazyka (podporuje až 28 cílových jazyků, včetně češtiny, portugalštiny PT-BR, angličtiny, španělštiny, ruštiny, čínštiny, japonštiny, francouzštiny, němčiny a dalších) a vykresluje je **lokálně** přímo do herního chatu při **věrném zachování původního jména hráče a barevného kódu** (P1 zlatá, P2 modrá, P3 fialová, P4 azurová).
  - **Odchozí zprávy:** Napište `[cílový_jazyk]>>zpráva` nebo jednoduše `>>zpráva` pro automatickou odpověď v jazyce poslední příchozí zprávy spoluhráče. Přeložený text je vysílán přes multiplayerový RPC, takže jej mezinárodní spoluhráči čtou ve svém rodném jazyce.
- **Trvalé protokoly chatu a export do souboru:** Kompletní sledování historie chatu relace s časovými razítky, jmény hráčů, barvami, rozpoznaným zdrojovým jazykem a odezvou překladu. Protokoly se v tabulce uchovávají i po restartu aplikace díky lehkému asynchronnímu dávkovému zápisu na disk (nastavitelné), s možností exportu do souboru `.txt` jedním kliknutím.
- **Prohledávatelné protokoly chatu:** Plně prohledávatelná historie chatu s filtrováním ve všech sloupcích.

![Helldivers 1 Babel v akci](screenshots/screenshot.png)

- **Taktický Rychlý chat (10 vlastních slotů):** 10 přizpůsobitelných slotů pro předem definované taktické zprávy, přiřazených k uživatelsky konfigurovatelným klávesovým zkratkám (např. Numpad 0–9 nebo vlastní kombinace) pro okamžité vysílání taktických hlášek do multiplayerového herního chatu.

![Rychlý chat](screenshots/quick-chat.png)

- **Automatické uvítání v Rychlém chatu:** Automaticky vysílá přizpůsobitelnou uvítací zprávu do herního chatu, když hostujete lobby a připojí se k vám cizí hráč.
- **Informační překrytí Rychlého chatu na obrazovce:** Zobrazení neaktivujícího plovoucího překrytí na pravé straně obrazovky stisknutím klávesy `Numpad +` (nebo vaší zaznamenané zkratky) pro rychlou kontrolu všech aktivních taktických hlášek a zkratek přímo během hraní. *(Vyžaduje běh hry Helldivers v režimu bezokrajového okna nebo v okně).*

![Překrytí Rychlého chatu](screenshots/overlay.png)

- **Podpora 28 jazyků rozhraní:** Kompletní lokalizace uživatelského rozhraní do 28 jazyků.
- **Modulární architektura adaptérů poskytovatelů:** Postaveno na modulárním rozhraní pro poskytovatele AI. Zahrnuje optimalizovaný adaptér bez zbytečného přemýšlení pro **Google Gemini** (`gemini-flash-lite-latest` / `gemini-2.0-flash-lite`) a rozšiřitelná rozhraní pro OpenAI, Claude a DeepSeek (připravuje se).
- **Nastavení tónu překladu:** Neformální (neformální herní slang a taktické výrazy) / Normální (přirozený konverzační tón bez herního slangu).
- **Globální zkratka Pozastavit / Pokračovat:** Stisknutím `Ctrl + F10` (nebo vaší nastavené zkratky) můžete asistenta kdykoli přepnout mezi stavy **Aktivní** a **Pozastaveno**. Pozastavení čistě pozastaví překlad herního chatu i odesílání zpráv Rychlého chatu.
- **Vyhrazené zkratky pro jednotlivé funkce:**
  - **Přepnutí pouze překladu:** Stiskněte `Ctrl + F11` (nebo vaši zkratku) pro okamžité pozastavení nebo obnovení překladu chatu bez ovlivnění Rychlého chatu (s upozorněním na obrazovce).
  - **Přepnutí pouze Rychlého chatu:** Stiskněte `Ctrl + F12` (nebo vaši zkratku) pro okamžité pozastavení nebo obnovení taktických hlášek bez ovlivnění překladu (s upozorněním na obrazovce).

![Nastavení](screenshots/settings.png)

- **Bezpečné ukládání přihlašovacích údajů (DPAPI):** Citlivá nastavení, jako jsou API klíče AI poskytovatelů, jsou šifrována v klidovém stavu pomocí rozhraní Windows Data Protection API (DPAPI, rozsah `CurrentUser`). Přihlašovací údaje se nikdy neukládají v čistém textu v konfiguračních souborech a lze je dešifrovat pouze na vašem místním počítači pod vaším uživatelským účtem Windows.
- **Žádná telemetrie a naprosté soukromí:** Tento nástroj neshromažďuje, nezaznamenává ani neodesílá žádná uživatelská data ani telemetrii. Všechny požadavky na překlad komunikují přímo a výhradně s koncovým bodem vašeho nakonfigurovaného AI poskytovatele pomocí vašeho osobního API klíče.
- **100% neinvazivní (žádná úprava herních souborů):** Žádné archivy hry se nerozbalují ani neupravují. Vaše herní soubory zůstávají 100% původní a nedotčené.

## 2. Instalace a nastavení

### Požadavky
Pokud používáte předem zkompilované samostatné vydání (`HD1-Babel.exe`):
- **Operační systém:** Windows 10 / 11 64-bit (x64).
- **Běhové prostředí pro desktop:** [Microsoft .NET 10 Desktop Runtime (x64)](https://aka.ms/dotnet/10.0/windowsdesktop-runtime-win-x64.exe) (přímý instalátor).
- **Cílová hra:** *Helldivers* (2015) na platformě Steam (64bitový `helldivers.exe`).

### Nastavení bezplatného API klíče AI
- **Přihlašovací údaje k AI poskytovateli:** API klíč Google Gemini (získejte bezplatný osobní klíč v [Google AI Studio](https://aistudio.google.com/) ([Podívejte se na toto instruktážní video](https://www.youtube.com/watch?v=YHFMrZD-_3g))).
- **Přístup k internetu:** Vyžadován pro volání překladového rozhraní API.

### Stažení a instalace (Předpřipravené vydání)

K používání Babelu **nemusíte** instalovat vývojářské nástroje ani kompilovat ze zdrojových kódů ([pokud ovšem nechcete](#4-vývoj-sestavení-ze-zdrojových-kódů)):
1. Přejděte do sekce **[Releases](https://github.com/difinim/helldivers1-babel/releases)** na pravé straně stránky projektu na GitHubu.
2. U nejnovějšího vydání rozbalte seznam **Assets**.
3. Stáhněte samostatný archiv distribuce: **`Helldivers1Babel-v<version>-win-x64.zip`**.
4. Rozbalte soubor zip kamkoli na svém počítači (např. `C:\GamesTools\Helldivers1Babel`).
5. Otevřete složku a spusťte **`HD1-Babel.exe`**.

> [!NOTE]
> **Předpoklad:** Koncovým uživatelům stačí pouze bezplatný balíček [Microsoft .NET 10 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/10.0). Ke spuštění vydání není vyžadován kompilátor C++, SDK ani vývojářský toolchain.

> [!NOTE]
> **Mějte na paměti**: Vzhledem k tomu, že `InjectorService.cs` používá standardní rozhraní API systému Windows pro injekci procesů (`VirtualAllocEx` + `WriteProcessMemory` + `CreateRemoteThread`), Windows Defender nebo antivirový software třetích stran _**mohou**_ spustit heuristickou detekci (např. Generic HackTool). Jedná se o běžné chování injektorů herních modifikací a soubor lze bezpečně zařadit na seznam výjimek.

## 3. Uživatelská příručka

### Začínáme
1. Spusťte **Helldivers 1 Babel** (`HD1-Babel.exe`).
2. Při prvním spuštění otevřete záložku **Nastavení**:
   - Zadejte svůj **Google Gemini API klíč** (bezpečně uložen pomocí šifrování Windows DPAPI).
   - Vyberte svůj cílový jazyk (např. `Čeština`).
   - Klikněte na **Otestovat připojení API** pro ověření klíče a odezvy.
   - V případě potřeby upravte zkratku **Pozastavit/Pokračovat** (výchozí je `Ctrl + F10`).
3. Nakonfigurujte své taktické hlášky na záložce **Rychlý chat** (např. přiřaďte Numpad 0–9 k vybraným zprávám).
4. Spusťte **Helldivers** na Steamu. Babel automaticky detekuje proces `helldivers.exe`, provede injekci `BabelHook.dll` a naváže vysokorychlostní IPC spojení přes pojmenovanou rouru (named pipe).

### Komunikace ve hře a případy použití

#### 1. Automatický příchozí překlad
Příchozí zprávy od spoluhráčů jsou za běhu detekovány a překládány do vašeho cílového jazyka, přičemž se v herním chatu zobrazují s původním jménem hráče a jeho barvou:
- Spoluhráč (English): `so quiet`
- Herní chat: `Diveheller: (English): hrobové ticho`

#### 2. Automatická odpověď v jazyce poslední příchozí zprávy (`>>zpráva`)
Pokud před zprávu v chatu přidáte předponu `>>`, text se automaticky přeloží do jazyka poslední příchozí zprávy spoluhráče:
- Vaše zadání v chatu: `>>ano, je tu až moc velký klid`
- Spoluhráč uvidí: `yes, it is too quiet`

#### 3. Explicitní volba libovolného jazyka (`cílový jazyk>>zpráva`)
Zprávu můžete kdykoli nasměrovat do konkrétního jazyka tak, že před ni napíšete název jazyka následovaný `>>`:

| Příkaz v chatu | Cílový jazyk | Příklad výsledku vysílaného spoluhráčům |
| :--- | :--- | :--- |
| `russian>>bom dia!` | Ruština | `доброе утро!` |
| `english>>miny na severu` | Angličtina | `mines to the north` |
| `japanese>>pozor na palbu!` | Japonština | `射線に注意せよ！` |
| `spanish>>potřebuji munici` | Španělština | `necesito munición` |
| `german>>ústup k bodu evakuace` | Němčina | `Rückzug zum Extraktionspunkt` |
| `french>>budeme bránit tuto pozici` | Francouzština | `défendons cette position` |

#### 4. Standardní odchozí zprávy a potlačení překladu vlastních zpráv
Při psaní standardní zprávy do chatu bez `>>` (např. `need ammo`, `jdeme na to`) je zpráva odeslána přímo přes běžný multiplayerový chat. Je-li povolena možnost **„Nepřekládat standardní odchozí zprávy“** (ve výchozím stavu ZAPNUTO na liště rychlých voleb i v Nastavení), Babel potlačuje zpětný překlad vaší vlastní zprávy pro vás. Pokud volbu vypnete, Babel zpracuje i lokální odchozí zprávy přes kanál příchozího překladu.

#### 5. Zkratky taktického Rychlého chatu
Stiskněte libovolnou nakonfigurovanou zkratku rychlého chatu (např. `NumPad 1`–`NumPad 9`) během hraní pro okamžité odeslání předvolených hlášek do týmového chatu bez otevírání textového pole. Všechna stisknutí kláves hladce procházejí do hry, což zajišťuje okamžitou odezvu ovládání.

#### 6. Informační překrytí Rychlého chatu na obrazovce (`Numpad +`)
Stiskněte kdykoli `Numpad +` (nebo vaši zaznamenanou zkratku nakonfigurovanou na záložce **Rychlý chat**) pro přepnutí zobrazení překrytí se seznamem všech aktivních předvoleb Rychlého chatu. Překrytí se zobrazuje zarovnané doprava a svisle vycentrované na obrazovce, přičemž zobrazuje každou aktivní předvolbu ve formátu `[<zkratka>] <zpráva>` (zprávy jsou zkráceny na 30 znaků) ve stejném nerušivém vizuálním stylu jako vyskakovací upozornění.
> [!NOTE]
> **Požadavek na režim zobrazení:** Helldivers musí být v grafickém nastavení hry spuštěn v režimu **Bezokrajové okno** (Borderless Fullscreen) nebo **V okně** (Windowed), aby se překryvná okna systému Windows mohla vykreslovat přes hru.

#### 7. Pozastavení, pokračování a přepínače funkcí
- **Globální Pozastavit / Pokračovat (`Ctrl + F10`):** Hlavní přepínač, který čistě pozastaví/obnoví zachytávání překladu i odesílání zpráv Rychlého chatu.
- **Přepnutí pouze překladu (`Ctrl + F11`):** Zapne nebo vypne pouze funkci překladu s oznámením na obrazovce. Rychlý chat pokračuje v běžném provozu.
- **Přepnutí pouze Rychlého chatu (`Ctrl + F12`):** Zapne nebo vypne pouze odesílání hlášek Rychlého chatu s oznámením na obrazovce. Překlad herního chatu funguje i nadále.
*(Všechny zkratky lze přizpůsobit na záložkách **Nastavení** a **Rychlý chat** pomocí tlačítek pro záznam stisku).*

#### 8. Export a trvalé protokoly chatu
Kdykoli klikněte na **Exportovat protokoly** na záložce Překlad a Protokoly, zvolte umístění a uložte historii chatu (s časovými razítky, jmény hráčů, původními zprávami, překlady a odezvami) do textového souboru `.txt`. Zprávy chatu se v tabulce automaticky uchovávají mezi spuštěními aplikace prostřednictvím asynchronního dávkového zápisu na disk (limit úložiště lze nastavit od 0 do 100 MB v Nastavení, k dispozici je tlačítko **Vymazat paměť protokolů**).

#### 9. Interaktivní odkazy ve zprávách
Zprávy v chatu týmu obsahující adresy URL začínající na `http://` nebo `https://` se ve sloupci **Původní zpráva** zobrazují jako klikatelné odkazy. Po kliknutí se zobrazí potvrzovací dialog pro ověření důvěryhodnosti odkazu před jeho otevřením ve výchozím webovém prohlížeči.

## 4. Vývoj (Sestavení ze zdrojových kódů)

### Požadavky pro vývojáře
Pokud projekt kompilujete a sestavujete ze zdrojových kódů:
- **Operační systém:** Windows 10 / 11 64-bit (x64).
- **.NET 10 SDK:** [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (`net10.0-windows`).
- **Sada nástrojů MSYS2 MinGW64 (Clang++, GCC, Make):** [MSYS2](https://www.msys2.org/) s balíčky `mingw-w64-x86_64-clang`, `mingw-w64-x86_64-gcc` a `mingw-w64-x86_64-make` — vyžadováno pro kompilaci nativní knihovny hooku (`BabelHook.dll`) a sestavení oficiálního submodulu LuaJIT 2.0 (`vendor/luajit`) ke zkompilování zaváděcího skriptu (`babel_bootstrap.lua`) do bajtkódu.
- **Git:** Git pro Windows (s rekurzivními submoduly pro MinHook amalgamation a LuaJIT 2.0).

### Automatizovaná příprava prostředí
Dvakrát klikněte na `setup.bat` (nebo spusťte `setup.ps1` v PowerShellu) v kořenovém adresáři repozitáře. Skript automaticky ověří a nainstaluje chybějící součásti:
1. Ověří 64bitové prostředí Windows.
2. Zkontroluje přítomnost .NET 10 SDK (automaticky nainstaluje přes `winget`, pokud chybí).
3. Zkontroluje sadu nástrojů MSYS2 MinGW64 (Clang++, GCC a Make). Pokud chybí, automaticky nainstaluje MSYS2 přes `winget` a potřebné balíčky (`mingw-w64-x86_64-clang`, `mingw-w64-x86_64-gcc` a `mingw-w64-x86_64-make`) přes `pacman`.
4. Inicializuje a sestaví kompilátor LuaJIT 2.0 ze submodulu (`vendor/luajit`).
5. Automaticky zajistí přidání cest `C:\msys64\mingw64\bin` a `C:\msys64\usr\bin` do systémové proměnné PATH.
6. Ověří závislosti MinHook amalgamation a provede počáteční kontrolu sestavení.

### Hlavní sestavení a vytvoření balíčku vydání
Chcete-li vyčistit předchozí artefakty, zkompilovat nativní knihovnu DLL, spustit všechny jednotkové testy a sestavit distribuční balíček:
```powershell
pwsh -File build.ps1
```
Výstupní adresář bude zabalen do `dist/Helldivers1Babel-vX.Y.Z/`.

### Manuální sestavení krok za krokem
```powershell
# 1. Sestavení nativní knihovny hooku (automaticky kompiluje babel_bootstrap.lua pomocí luajit)
pwsh -File src/Babel.Hook/build.ps1

# 2. Sestavení řešení .NET 10
dotnet build Helldivers1Babel.slnx

# 3. Spuštění automatických sad testů
dotnet test
```

## 5. Licence

Distribuováno pod [Licencí MIT](../LICENSE).
