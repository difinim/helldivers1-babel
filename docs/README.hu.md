# ![Helldivers 1 Babel](screenshots/logo.png)

[![Latest Version](https://img.shields.io/github/v/release/difinim/helldivers1-babel?logo=github&label=release)](https://github.com/difinim/helldivers1-babel/releases/latest)
[![Build Status](https://github.com/difinim/helldivers1-babel/actions/workflows/release.yml/badge.svg)](https://github.com/difinim/helldivers1-babel/actions/workflows/release.yml)
[![Tests](https://github.com/difinim/helldivers1-babel/actions/workflows/tests.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/tests.yml?query=branch%3Amain)
[![CodeQL](https://github.com/difinim/helldivers1-babel/actions/workflows/codeql.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/codeql.yml?query=branch%3Amain)
[![OSV-Scanner](https://github.com/difinim/helldivers1-babel/actions/workflows/osv-scanner.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/osv-scanner.yml?query=branch%3Amain)
[![Dependabot](https://img.shields.io/badge/dependabot-enabled-025E8C?logo=dependabot&logoColor=white)](https://github.com/difinim/helldivers1-babel/network/updates)

*Merüljetek együtt. Beszéljetek egyként.*

A **Helldivers 1 Babel** egy nyílt forráskódú, a játékfájlokat nem módosító csevegésfordító és taktikai kommunikációs asszisztens a *Helldivers (2015)* játékhoz, mert *az édes szabadság még szebben szól, ha mindenki érti a tervet*.

## Translations

[English](../README.md) | [العربية](README.ar.md) | [বাংলা](README.bn.md) | [简体中文](README.zh.md) | [Čeština](README.cs.md) | [Nederlands](README.nl.md) | [Français](README.fr.md) | [Deutsch](README.de.md) | [Ελληνικά](README.el.md) | [हिन्दी](README.hi.md) | [Magyar](README.hu.md) | [Bahasa Indonesia](README.id.md) | [Italiano](README.it.md) | [日本語](README.ja.md) | [한국어](README.ko.md) | [Bahasa Melayu](README.ms.md) | [فارسی](README.fa.md) | [Polski](README.pl.md) | [Português (PT-BR)](README.pt-BR.md) | [Română](README.ro.md) | [Русский](README.ru.md) | [Español](README.es.md) | [Svenska](README.sv.md) | [Tagalog (Filipino)](README.tl.md) | [ไทย](README.th.md) | [Türkçe](README.tr.md) | [Українська](README.uk.md) | [Tiếng Việt](README.vi.md)

> [!IMPORTANT]
> **Kompatibilitási nyilatkozat:**
> **A Helldivers 1 Babel kizárólag a Helldivers (2015) PC-s (Steam) verziójához készült.**
> **Kompatibilis a modokkal.**
> **NEM kompatibilis a Helldivers 2-vel (2024)**, amely teljesen más architektúrára épül és rendszermag-szintű (kernel-level) csalásvédelmet használ.

## Tartalomjegyzék

- [1. Áttekintés és főbb funkciók](#1-áttekintés-és-főbb-funkciók)
- [2. Telepítés és beállítás](#2-telepítés-és-beállítás)
  - [Rendszerkövetelmények](#rendszerkövetelmények)
  - [Ingyenes MI API-kulcs beszerzése](#ingyenes-mi-api-kulcs-beszerzése)
  - [Letöltés és telepítés (Előre lefordított kiadás)](#letöltés-és-telepítés-előre-lefordított-kiadás)
- [3. Használati útmutató](#3-használati-útmutató)
  - [Első lépések](#első-lépések)
  - [Játékbeli kommunikáció és használati esetek](#játékbeli-kommunikáció-és-használati-esetek)
- [4. Fejlesztés (Fordítás forráskódból)](#4-fejlesztés-fordítás-forráskódból)
  - [Fejlesztői követelmények](#fejlesztői-követelmények)
  - [Automatizált környezetbeállítás](#automatizált-környezetbeállítás)
  - [Mester build és kiadáscsomagolás](#mester-build-és-kiadáscsomagolás)
  - [Manuális, lépésről lépésre történő fordítás](#manuális-lépésről-lépésre-történő-fordítás)
- [5. Licenc](#5-licenc)

## 1. Áttekintés és főbb funkciók

- **Valós idejű, kétirányú csevegésfordítás:**
  - **Bejövő üzenetek:** Automatikusan észleli a csapattársak nyelvét, lefordítja az üzeneteket a választott célnyelvre (akár 28 nyelvet támogat, köztük a brazíliai portugált, angolt, spanyolt, oroszt, kínait, japánt, franciát, németet és sok mást), majd **helyileg** jeleníti meg a játékbeli csevegésben úgy, hogy **hűen megőrzi a csapattárs eredeti nevét és színkódját** (P1 arany, P2 kék, P3 lila, P4 ciánkék).
  - **Kimenő üzenetek:** Írja be, hogy `[célnyelv]>>üzenet` vagy egyszerűen `>>üzenet`, ha automatikusan az utolsó bejövő üzenet nyelvén szeretne válaszolni. A lefordított szöveg hálózati többjátékos RPC-n keresztül kerül kiküldésre, így a külföldi csapattársak anyanyelvükön olvashatják.
- **Tartós csevegési naplók és fájlba exportálás:** Teljes játékmenet-csevegési előzmények rögzítése időbélyegekkel, játékosnevekkel, színekkel, észlelt forrásnyelvekkel és fordítási késleltetéssel. A naplók az alkalmazás újraindításakor is megmaradnak a táblázatban a pehelysúlyú, aszinkron lemezre írásoknak köszönhetően (konfigurálható), és egyetlen kattintással `.txt` fájlba exportálhatók.
- **Kereshető csevegési naplók:** Teljesen kereshető csevegési előzmények bármely oszlop szűrésének támogatásával.

![A Helldivers 1 Babel működés közben](screenshots/screenshot.png)

- **Taktikai gyorscsevegés (10 egyéni hely):** 10 testreszabható, előre definiált taktikai üzenethely, amelyek mindegyike felhasználó által beállítható gyorsbillentyűkhöz van rendelve (pl. Numpad 0-9 vagy egyéni kombinációk), hogy azonnal taktikai parancsokat közvetíthessen a többjátékos csevegésbe.
- **Automatikus üdvözlő üzenet:** Automatikusan kiküld egy testreszabható üdvözlő üzenetet a játékbeli csevegésbe, amikor szobát hosztol, és egy vendégjátékos csatlakozik.

![Gyorscsevegés](screenshots/quick-chat.png)

- **Képernyőn megjelenő taktikai gyorscsevegési fedvény:** Kapcsolja be vagy ki az ablakfókuszt el nem vevő lebegő fedvényt a képernyő jobb oldalán a `Numpad +` billentyűvel (vagy az Ön által rögzített billentyűkombinációval), hogy játék közben gyorsan áttekinthesse az összes aktív taktikai hívást és billentyűparancsot. *(Ehhez a Helldiversnek keret nélküli teljes képernyős vagy ablakos módban kell futnia).*

![Gyorscsevegés fedvény](screenshots/overlay.png)

- **Többnyelvű felhasználói felület:** 28 nyelvet támogat.
- **Szolgáltatói adapter architektúra:** Moduláris MI-szolgáltatói felülettel készült. Tartalmaz egy optimalizált, gondolkodási késleltetés nélküli adaptert a **Google Gemini**-hez (`gemini-flash-lite-latest` / `gemini-2.0-flash-lite`), valamint bővíthető szerződéseket az OpenAI, Claude és DeepSeek integrációkhoz (hamarosan).
- **Fordítási stílus beállítása:** Laza: kötetlen játékszleng és taktikai gamer kifejezések / Normál: természetes társalgási stílus játékszleng nélkül.
- **Globális Szünet / Folytatás gyorsbillentyű:** Nyomja meg a `Ctrl + F10` billentyűkombinációt (vagy az Ön által beállított parancsikont) bármikor az asszisztens **Aktív** és **Szüneteltetve** állapotok közötti váltásához. A szüneteltetés tisztán felfüggeszti a játékbeli fordítást és a Gyorscsevegés üzenetküldését is.
- **Dedikált funkcióbillentyűk:**
  - **Csak a fordítás ki/be kapcsolása:** Nyomja meg a `Ctrl + F11` billentyűkombinációt (vagy a rögzített parancsikont) a csevegésfordítás azonnali szüneteltetéséhez vagy folytatásához a Gyorscsevegés befolyásolása nélkül (képernyőértesítési visszajelzéssel).
  - **Csak a Gyorscsevegés ki/be kapcsolása:** Nyomja meg a `Ctrl + F12` billentyűkombinációt (vagy a rögzített parancsikont) a Gyorscsevegési üzenetek azonnali szüneteltetéséhez vagy folytatásához a fordítás befolyásolása nélkül (képernyőértesítési visszajelzéssel).

![Beállítások](screenshots/settings.png)

- **Biztonságos hitelesítőadat-tárolás (DPAPI):** Az érzékeny beállítások, mint például az MI-szolgáltató API-kulcsa, a Windows Data Protection API (DPAPI, `CurrentUser` hatókör) használatával titkosítva tárolódnak a lemezen. A hitelesítő adatok soha nem jelennek meg egyszerű szövegként a konfigurációs fájlokban, és kizárólag a helyi gépen, az Ön Windows-fiókjával oldhatók fel.
- **Zéró telemetria és teljes adatvédelem:** Ez az eszköz semmilyen felhasználói adatot vagy telemetriát nem gyűjt, nem naplóz és nem továbbít. Minden fordítási kérelem közvetlenül és kizárólag a konfigurált MI-szolgáltató végpontjával kommunikál a saját személyes API-kulcsával.
- **100%-ban nem invazív (Nem módosítja a játékfájlokat):** Semmilyen játékarchívum nem kerül kibontásra vagy módosításra. A játékfájlok 100%-ban érintetlenek és gyári állapotúak maradnak.




## 2. Telepítés és beállítás

### Rendszerkövetelmények
Ha az előre lefordított önálló kiadást (`HD1-Babel.exe`) használja:
- **Operációs rendszer:** Windows 10 / 11 64 bites (x64).
- **Asztali futtatókörnyezet:** [Microsoft .NET 10 Desktop Runtime (x64)](https://aka.ms/dotnet/10.0/windowsdesktop-runtime-win-x64.exe) (közvetlen telepítő).
- **Céljáték:** *Helldivers* (2015) a Steamen (64 bites `helldivers.exe`).

### Ingyenes MI API-kulcs beszerzése
- **MI-szolgáltató hitelesítő adatai:** Google Gemini API-kulcs (ingyenes személyes kulcs igényelhető a [Google AI Studio](https://aistudio.google.com/) felületén ([Lásd ezt az oktatóvideót](https://www.youtube.com/watch?v=YHFMrZD-_3g))).
- **Internetkapcsolat:** Szükséges a fordítási API-hívásokhoz.

### Letöltés és telepítés (Előre lefordított kiadás)

A Babel használatához **nem** szükséges fejlesztői eszközöket telepítenie vagy forrásból fordítania ([de ha szeretné, megteheti](#4-fejlesztés-fordítás-forráskódból)):
1. Navigáljon a GitHub projektoldal jobb oldalán található **[Releases](https://github.com/difinim/helldivers1-babel/releases)** menüpontra.
2. A legfrissebb kiadás alatt nyissa le az **Assets** listát.
3. Töltse le az önálló disztribúciós archívumot: **`Helldivers1Babel-v<version>-win-x64.zip`**.
4. Csomagolja ki a zip fájlt a számítógépén bárhová (pl. `C:\GamesTools\Helldivers1Babel`).
5. Nyissa meg a mappát, és indítsa el a **`HD1-Babel.exe`** fájlt.

> [!NOTE]
> **Előfeltétel:** A végfelhasználóknak kizárólag az ingyenes [Microsoft .NET 10 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/10.0) futtatókörnyezetre van szükségük. A kiadás futtatásához nincs szükség C++ fordítóra, SDK-ra vagy egyéb eszközkészletre.

> [!NOTE]
> **Figyelem**: Mivel az InjectorService.cs szabványos Windows folyamatbefecskendezési API-kat használ (`VirtualAllocEx` + `WriteProcessMemory` + `CreateRemoteThread`), a Windows Defender vagy harmadik féltől származó víruskeresők heurisztikus riasztást (pl. Generic HackTool) válthatnak ki. Ez teljesen természetes viselkedés a játékmod-injektorok esetében, és biztonságosan felvehető a kivételek listájára (allowlist).



## 3. Használati útmutató

### Első lépések
1. Indítsa el a **Helldivers 1 Babel** alkalmazást (`HD1-Babel.exe`).
2. Az első indításkor nyissa meg a **Beállítások** fület:
   - Adja meg a **Google Gemini API kulcsát** (amelyet a rendszer a Windows DPAPI segítségével biztonságosan titkosít).
   - Válassza ki a célnyelvet (pl. `Magyar` vagy `Portuguese (PT-BR)`).
   - Kattintson az **API kapcsolat tesztelése** gombra a kulcs és a késleltetés ellenőrzéséhez.
   - Igény szerint módosítsa a **Szünet / Folytatás** billentyűparancsát (alapértelmezés szerint `Ctrl + F10`).
3. Konfigurálja a taktikai üzeneteket a **Gyorscsevegés** fülön (pl. rendelje a Numpad 0-9 gombokat a kívánt üzenetekhez).
4. Indítsa el a **Helldivers**-t a Steamen. A Babel automatikusan észleli a `helldivers.exe` folyamatot, befecskendezi a `BabelHook.dll` fájlt, és létrehozza a nagy sebességű elnevezett csatornás (named pipe) IPC kapcsolatot.

### Játékbeli kommunikáció és használati esetek

#### 1. Automatikus bejövő fordítás
A csapattársak bejövő üzeneteit a rendszer azonnal észleli és lefordítja a célnyelvre, majd megjeleníti a játékbeli csevegésben az eredeti játékos nevével és színével:
- Csapattárs (angolul): `so quiet`
- Játékbeli csevegés: `Diveheller: (angol): olyan nagy a csend`

#### 2. Automatikus válasz a legutóbb beérkező nyelven (`>>üzenet`)
Ha az üzenetét `>>` előtaggal kezdi, a szöveg automatikusan a legutóbbi bejövő csapattárs-üzenet nyelvére fordítódik:
- Az Ön bevitele a csevegésben: `>>igen, túlságosan csendes`
- Amit a csapattárs lát: `yes, it is too quiet`

#### 3. Kifejezett / Tetszőleges nyelvválasztás (`célnyelv>>üzenet`)
Bármikor elküldheti üzenetét egy adott nyelvre, ha az üzenet elé a kívánt nyelv nevét és a `>>` jelet írja:

| Csevegési beviteli parancs | Célnyelv | Csapattársaknak közvetített példaeredmény |
| :--- | :--- | :--- |
| `russian>>szép jó reggelt!` | Orosz | `доброе утро!` |
| `english>>aknák északra` | Angol | `mines to the north` |
| `japanese>>vigyázat a tűzvonalra!` | Japán | `射線に注意せよ！` |
| `spanish>>lőszerre van szükségem` | Spanyol | `necesito munición` |
| `german>>vonuljatok vissza a kivonási ponthoz` | Német | `Rückzug zum Extraktionspunkt` |
| `french>>védjük meg ezt a pozíciót` | Francia | `défendons cette position` |

#### 4. Standard kimenő üzenetek és az önfordítás elnyomása
Amikor standard játékbeli csevegőüzenetet ír be `>>` nélkül (pl. `need ammo`, `gyerünk`), az üzenet közvetlenül az eredeti többjátékos csevegésen keresztül kerül elküldésre. A **"Standard kimenő üzenetek kihagyása"** opció bekapcsolásával (alapértelmezetten BE van kapcsolva mind a gyorsvezérlő sávban, mind a Beállításokban) a Babel megakadályozza, hogy a saját üzenete visszakerüljön önhöz fordításként. Ha KIKAPCSOLJA, a Babel a standard helyi üzeneteket is átfuttatja a bejövő fordítási folyamaton.

#### 5. Taktikai gyorscsevegési gyorsbillentyűk
Nyomja meg a beállított gyorscsevegési gyorsbillentyűk egyikét (pl. `NumPad 1`-`NumPad 9`) játék közben, hogy a csevegőablak megnyitása nélkül azonnal közvetítse az előre beállított hívásokat a többjátékos csevegésbe. Minden billentyűleütés zökkenőmentesen továbbjut a játékba, így az irányítás teljesen gördülékeny marad.

#### 6. Taktikai gyorscsevegési fedvény (`Numpad +`)
Nyomja meg a `Numpad +` billentyűt (vagy a **Gyorscsevegés** fülön beállított egyéni gyorsbillentyűt) a képernyőn megjelenő fedvény be- és kikapcsolásához, amely felsorolja az összes aktív gyorscsevegési előbeállítást. A fedvény a képernyő jobb oldalára igazítva és függőlegesen középre zárva jelenik meg, feltüntetve minden aktív elemet a `[<billentyűparancs>] <üzenet>` formátumban (az üzeneteket 30 karakterre rövidítve), a felugró értesítésekkel megegyező, zavartalan vizuális stílusban.
> [!NOTE]
> **Megjelenítési mód követelménye:** A Helldivers-t a játékon belüli Videobeállításokban **Keret nélküli teljes képernyős** (vagy Ablakos) módra kell állítani ahhoz, hogy a Windows fedvényablakai meg tudjanak jelenni a játék felett.

#### 7. Szüneteltetés, folytatás és funkciókapcsolók
- **Globális Szünet / Folytatás (`Ctrl + F10`):** Főkapcsoló, amely tisztán felfüggeszti/folytatja mind a fordítás elfogását, mind a Gyorscsevegés üzeneteinek közvetítését.
- **Csak a fordítás ki/be kapcsolása (`Ctrl + F11`):** Csak a fordítási funkciót kapcsolja be vagy ki egy képernyőn megjelenő értesítéssel. A Gyorscsevegés továbbra is a megszokott módon működik.
- **Csak a Gyorscsevegés ki/be kapcsolása (`Ctrl + F12`):** Csak a Gyorscsevegési üzenetküldést kapcsolja be vagy ki egy képernyőn megjelenő értesítéssel. A játékbeli csevegés fordítása a megszokott módon folytatódik.
*(Minden parancsikon testreszabható a **Beállítások** és a **Gyorscsevegés** lapokon a kattintásra rögzítő gombokkal).*

#### 8. Naplók exportálása és mentése
Kattintson a Fordítás és Naplók lapon a **Naplók exportálása** gombra a csevegési előzmények (időbélyegekkel, játékosnevekkel, eredeti üzenetekkel, fordításokkal és késleltetésekkel) `.txt` dokumentumba történő mentéséhez. A csevegési üzenetek az aszinkron lemezírásnak köszönhetően megmaradnak az alkalmazás újraindításakor is (a tárhelykorlát 0 és 100 MB között állítható be a Beállításokban, egy **Naplómemória törlése** gomb kíséretében).

#### 9. Interaktív hivatkozások az üzenetekben
A csapatcsevegésben megjelenő `http://` vagy `https://` URL-címek kattintható linkként jelennek meg az **Eredeti üzenet** oszlopban. Ha rákattint egy linkre, egy megerősítő párbeszédpanel kéri, hogy ellenőrizze a hivatkozás megbízhatóságát, mielőtt megnyitná azt az alapértelmezett böngészőjében.



## 4. Fejlesztés (Fordítás forráskódból)

### Fejlesztői követelmények
Amennyiben a projektet forrásból szeretné lefordítani és felépíteni:
- **Operációs rendszer:** Windows 10 / 11 64 bites (x64).
- **.NET 10 SDK:** [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (`net10.0-windows`).
- **MSYS2 MinGW64 eszközkészlet (Clang++, GCC, Make):** [MSYS2](https://www.msys2.org/) `mingw-w64-x86_64-clang`, `mingw-w64-x86_64-gcc` és `mingw-w64-x86_64-make` csomagokkal – szükséges a natív hook könyvtár (`BabelHook.dll`) lefordításához és a hivatalos LuaJIT 2.0 almodul (`vendor/luajit`) felépítéséhez a játékon belüli indító parancsfájl (`babel_bootstrap.lua`) bájtkóddá alakításához.
- **Git:** Git for Windows (rekurzív almodulokkal a MinHook és a LuaJIT 2.0 számára).


### Automatizált környezetbeállítás
Kattintson duplán a `setup.bat` fájlra (vagy futtassa a `setup.ps1` fájlt PowerShellben) a projekt gyökerében. Ez automatikusan ellenőrzi és telepíti a hiányzó előfeltételeket:
1. Ellenőrzi a 64 bites Windows környezetet.
2. Ellenőrzi a .NET 10 SDK meglétét (ha hiányzik, automatikusan telepíti a `winget` segítségével).
3. Ellenőrzi az MSYS2 MinGW64 eszközkészletet (Clang++, GCC és Make). Ha hiányzik, automatikusan telepíti az MSYS2-t a `winget`-tel, és feltelepíti a szükséges csomagokat (`mingw-w64-x86_64-clang`, `mingw-w64-x86_64-gcc` és `mingw-w64-x86_64-make`) a `pacman` segítségével.
4. Inicializálja és lefordítja a LuaJIT 2.0 fordítót az almodulból (`vendor/luajit`).
5. Automatikusan gondoskodik arról, hogy a `C:\msys64\mingw64\bin` és a `C:\msys64\usr\bin` bekerüljön a PATH útvonalba.
6. Ellenőrzi a MinHook függőségeket, és futtat egy kezdeti ellenőrző fordítást.

### Mester build és kiadáscsomagolás
A korábbi build-maradványok törléséhez, a natív hook DLL lefordításához, az összes egységteszt futtatásához és a kiadási csomag összeállításához:
```powershell
pwsh -File build.ps1
```
A kimeneti könyvtár a `dist/Helldivers1Babel-vX.Y.Z/` mappába kerül becsomagolásra.

### Manuális, lépésről lépésre történő fordítás
```powershell
# 1. Építse fel a natív hook DLL-t (automatikusan lefordítja a babel_bootstrap.lua-t a luajit segítségével)
pwsh -File src/Babel.Hook/build.ps1

# 2. Építse fel a .NET 10 megoldást
dotnet build Helldivers1Babel.slnx

# 3. Futtassa az automatizált tesztcsomagokat
dotnet test
```



## 5. Licenc

Kiadva az [MIT Licenc](../LICENSE) alatt.