# ![Helldivers 1 Babel](screenshots/logo.png)

[![Latest Version](https://img.shields.io/github/v/release/difinim/helldivers1-babel?logo=github&label=release)](https://github.com/difinim/helldivers1-babel/releases/latest)
[![Build Status](https://github.com/difinim/helldivers1-babel/actions/workflows/release.yml/badge.svg)](https://github.com/difinim/helldivers1-babel/actions/workflows/release.yml)
[![Tests](https://github.com/difinim/helldivers1-babel/actions/workflows/tests.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/tests.yml?query=branch%3Amain)
[![CodeQL](https://github.com/difinim/helldivers1-babel/actions/workflows/codeql.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/codeql.yml?query=branch%3Amain)
[![OSV-Scanner](https://github.com/difinim/helldivers1-babel/actions/workflows/osv-scanner.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/osv-scanner.yml?query=branch%3Amain)
[![Dependabot](https://img.shields.io/badge/dependabot-enabled-025E8C?logo=dependabot&logoColor=white)](https://github.com/difinim/helldivers1-babel/network/updates)

*Terjun bersama. Bertutur senada.*

**Helldivers 1 Babel** ialah pembantu terjemahan sembang dan komunikasi taktikal sumber terbuka yang tidak invasif untuk *Helldivers (2015)*, kerana *kebebasan yang manis kedengaran lebih bermakna apabila semua orang memahami perancangan*.

## Translations

[English](../README.md) | [العربية](README.ar.md) | [বাংলা](README.bn.md) | [简体中文](README.zh.md) | [Čeština](README.cs.md) | [Nederlands](README.nl.md) | [Français](README.fr.md) | [Deutsch](README.de.md) | [Ελληνικά](README.el.md) | [हिन्दी](README.hi.md) | [Magyar](README.hu.md) | [Bahasa Indonesia](README.id.md) | [Italiano](README.it.md) | [日本語](README.ja.md) | [한국어](README.ko.md) | [Bahasa Melayu](README.ms.md) | [فارسی](README.fa.md) | [Polski](README.pl.md) | [Português (PT-BR)](README.pt-BR.md) | [Română](README.ro.md) | [Русский](README.ru.md) | [Español](README.es.md) | [Svenska](README.sv.md) | [Tagalog (Filipino)](README.tl.md) | [ไทย](README.th.md) | [Türkçe](README.tr.md) | [Українська](README.uk.md) | [Tiếng Việt](README.vi.md)

> [!IMPORTANT]
> **Penafian Keserasian:**
> **Helldivers 1 Babel dibangunkan khas untuk Helldivers (2015) pada PC (Steam).**
> **Ia serasi dengan mod.**
> **Ia TIDAK serasi dengan Helldivers 2 (2024)**, yang berbeza sama sekali dan menggunakan sistem anti-tipu (anti-cheat) tahap kernel.

## Jadual Kandungan

- [1. Gambaran Keseluruhan & Ciri-Ciri Utama](#1-gambaran-keseluruhan--ciri-ciri-utama)
- [2. Persediaan](#2-persediaan)
  - [Keperluan Sistem](#keperluan-sistem)
  - [Persediaan Kunci API AI Percuma](#persediaan-kunci-api-ai-percuma)
  - [Muat Turun & Pemasangan (Keluaran Siap Bina)](#muat-turun--pemasangan-keluaran-siap-bina)
- [3. Panduan Penggunaan](#3-panduan-penggunaan)
  - [Permulaan](#permulaan)
  - [Komunikasi Dalam Permainan & Kes Penggunaan](#komunikasi-dalam-permainan--kes-penggunaan)
- [4. Pembangunan (Membina daripada Sumber)](#4-pembangunan-membina-daripada-sumber)
  - [Keperluan Pembangun](#keperluan-pembangun)
  - [Persediaan Persekitaran Automatik](#persediaan-persekitaran-automatik)
  - [Binaan Utama & Pembungkusan Keluaran](#binaan-utama--pembungkusan-keluaran)
  - [Binaan Manual Langkah Demi Langkah](#binaan-manual-langkah-demi-langkah)
- [5. Lesen](#5-lesen)

## 1. Gambaran Keseluruhan & Ciri-Ciri Utama

- **Terjemahan Sembang Dwiarah Masa Nyata:**
  - **Mesej Masuk:** Mengesan bahasa rakan sepasukan secara automatik, menterjemah mesej ke bahasa sasaran pilihan anda (menyokong sehingga 28 bahasa sasaran, termasuk Portugis PT-BR, Bahasa Inggeris, Sepanyol, Rusia, Cina, Jepun, Perancis, Jerman, dan lain-lain), serta memaparkannya secara **setempat (locally)** ke dalam sembang permainan sambil **mengekalkan nama pemain dan kod warna asal rakan sepasukan dengan tepat** (P1 Emas, P2 Biru, P3 Ungu, P4 Sian).
  - **Mesej Keluar:** Taip `[bahasa_sasaran]>>mesej` atau taip `>>mesej` sahaja untuk membalas secara automatik dalam bahasa mesej terakhir yang diterima daripada rakan sepasukan. Teks yang diterjemahkan disiarkan melalui RPC berbilang pemain supaya rakan sepasukan antarabangsa dapat membacanya secara langsung.
- **Log Sembang Berterusan & Eksport Fail:** Penjejakan sejarah sembang sesi penuh dengan cap masa, nama pemain, warna, bahasa sumber yang dikesan, dan kependaman terjemahan. Log kekal dalam jadual merentasi permulaan semula aplikasi melalui penulisan cakera kelompok tak segerak yang ringan (boleh dikonfigurasi), dengan eksport satu klik ke fail `.txt`.
- **Log Sembang Boleh Dicari:** Sejarah sembang yang boleh dicari sepenuhnya dengan menapis mana-mana lajur.

![Helldivers 1 Babel beraksi](screenshots/screenshot.png)

- **Sembang Pantas Taktikal (10 Slot Tersuai):** 10 slot mesej taktikal pratakrif yang boleh disesuaikan, setiap satunya diikat pada kekunci pintas boleh dikonfigurasikan pengguna (cth. Numpad 0-9 atau kombinasi tersuai) untuk menghantar panggilan taktikal serta-merta ke dalam sembang berbilang pemain dalam permainan.
- **Mesej Sembang Pantas Aluan Automatik:** Menyiarkan mesej aluan yang boleh disesuaikan secara automatik ke dalam sembang permainan apabila menghoskan lobi dan pemain tetamu menyertai.

![Sembang Pantas](screenshots/quick-chat.png)

- **Tindanan Rujukan Sembang Pantas Atas Skrin:** Togol tindanan rujukan terapung yang tidak mengganggu di sebelah kanan skrin dengan `Numpad +` (atau pintasan yang anda rakam) untuk menyemak semua panggilan taktikal dan pintasan aktif dalam permainan dengan pantas. *(Memerlukan Helldivers dijalankan dalam mod Borderless Fullscreen atau Windowed).*

![Tindanan Sembang Pantas](screenshots/overlay.png)

- **Sokongan UI Pelbagai Bahasa:** Menyokong 28 Bahasa.
- **Seni Bina Penyesuai Penyedia:** Dibina dengan antara muka penyedia AI modular. Dilengkapi penyesuai sifar-pemikiran (zero-thinking) yang dioptimumkan untuk **Google Gemini** (`gemini-flash-lite-latest` / `gemini-2.0-flash-lite`), dengan kontrak penyedia yang boleh diperluas untuk OpenAI, Claude, dan DeepSeek (akan datang).
- **Tetapan Nada Terjemahan:** Kasual: slanga permainan tidak formal dan lingo taktikal pemain / Normal: nada perbualan semula jadi tanpa slanga permainan.
- **Pintasan Global Jeda / Sambung:** Tekan `Ctrl + F10` (atau pintasan yang dikonfigurasikan) pada bila-bila masa untuk menogol pembantu antara status **Aktif** dan **Dijeda**. Menjeda akan menggantung kedua-dua pemintasan terjemahan sembang dalam permainan dan penyiaran mesej Sembang Pantas secara kemas.
- **Kekunci Pintas Ciri Khusus:**
  - **Togol Terjemahan Sahaja:** Tekan `Ctrl + F11` (atau pintasan yang direkodkan) untuk menjeda atau menyambung terjemahan sembang serta-merta tanpa menjejaskan Sembang Pantas (dengan maklum balas pemberitahuan toast pada skrin).
  - **Togol Sembang Pantas Sahaja:** Tekan `Ctrl + F12` (atau pintasan yang direkodkan) untuk menjeda atau menyambung panggilan Sembang Pantas serta-merta tanpa menjejaskan terjemahan (dengan maklum balas pemberitahuan toast pada skrin).

![Tetapan](screenshots/settings.png)

- **Penyimpanan Kredensial Selamat (DPAPI):** Keutamaan sensitif seperti token API penyedia AI disulitkan semasa disimpan menggunakan Windows Data Protection API (DPAPI, skop `CurrentUser`). Kredensial tidak pernah disimpan dalam teks biasa dalam fail konfigurasi dan hanya boleh dinyahsulit pada mesin tempatan anda oleh akaun Windows anda.
- **Sifar Telemetri & Privasi Lengkap:** Alat ini tidak mengumpul, melog, atau menghantar sebarang data pengguna atau data telemetri. Semua permintaan terjemahan berkomunikasi secara langsung dan eksklusif dengan titik akhir penyedia AI yang dikonfigurasikan menggunakan kredensial API peribadi anda.
- **100% Tidak Invasif (Sifar Pengubahsuaian Fail Permainan):** Tiada arkib permainan yang diekstrak atau diubah suai. Fail permainan anda kekal 100% tulen (vanilla) tanpa sebarang usikan.

## 2. Persediaan

### Keperluan Sistem
Jika anda menjalankan keluaran mandiri yang telah dikompilasikan (`HD1-Babel.exe`):
- **Sistem Pengendalian:** Windows 10 / 11 64-bit (x64).
- **Desktop Runtime:** [Microsoft .NET 10 Desktop Runtime (x64)](https://aka.ms/dotnet/10.0/windowsdesktop-runtime-win-x64.exe) (pemasang terus).
- **Permainan Sasaran:** *Helldivers* (2015) di Steam (`helldivers.exe` 64-bit).

### Persediaan Kunci API AI Percuma
- **Kredensial Penyedia AI:** Kunci API Google Gemini (dapatkan kunci peribadi percuma daripada [Google AI Studio](https://aistudio.google.com/) ([Tonton video tutorial ini](https://www.youtube.com/watch?v=YHFMrZD-_3g))).
- **Akses Internet:** Diperlukan untuk panggilan API terjemahan.

### Muat Turun & Pemasangan (Keluaran Siap Bina)

Anda **tidak** perlu memasang alatan pembangun atau menyusun daripada sumber untuk menggunakan Babel ([tetapi jika anda mahu](#4-pembangunan-membina-daripada-sumber)):
1. Navigasi ke bahagian **[Releases](https://github.com/difinim/helldivers1-babel/releases)** di sebelah kanan halaman projek GitHub.
2. Di bawah keluaran terkini, kembangkan senarai **Assets**.
3. Muat turun arkib pengedaran kendiri: **`Helldivers1Babel-v<versi>-win-x64.zip`**.
4. Ekstrak fail zip di mana-mana sahaja pada komputer anda (cth. `C:\GamesTools\Helldivers1Babel`).
5. Buka folder dan jalankan **`HD1-Babel.exe`**.

> [!NOTE]
> **Prasyarat:** Pengguna akhir hanya memerlukan [Microsoft .NET 10 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/10.0) percuma. Tiada pengkompil C++, SDK, atau toolchain diperlukan untuk menjalankan keluaran ini.

> [!NOTE]
> **Perhatian:** Kerana InjectorService.cs menggunakan API suntikan proses Windows standard (`VirtualAllocEx` + `WriteProcessMemory` + `CreateRemoteThread`), Windows Defender atau Antivirus pihak ketiga _**mungkin**_ mencetuskan pengesanan heuristik (cth. Generic HackTool). Ini adalah tingkah laku biasa untuk penyuntik mod permainan dan boleh dimasukkan ke dalam senarai dibenarkan (allowlist) dengan selamat.

## 3. Panduan Penggunaan

### Permulaan
1. Mulakan **Helldivers 1 Babel** (`HD1-Babel.exe`).
2. Semasa pelancaran pertama, buka tab **Tetapan**:
   - Masukkan **Kunci API Google Gemini** anda (disimpan dengan selamat melalui penyulitan Windows DPAPI).
   - Pilih bahasa sasaran anda (cth. `Bahasa Melayu`).
   - Klik **Uji Sambungan API** untuk mengesahkan kunci dan kependaman anda.
   - Sesuaikan pintasan **Jeda / Sambung** jika mahu (lalai kepada `Ctrl + F10`).
3. Konfigurasikan panggilan taktikal anda dalam tab **Sembang Pantas** (cth. ikat Numpad 0-9 pada mesej pilihan anda).
4. Lancarkan **Helldivers** di Steam. Babel mengesan `helldivers.exe` secara automatik, menyuntik `BabelHook.dll`, dan mewujudkan sambungan IPC Named Pipe berkelajuan tinggi.

### Komunikasi Dalam Permainan & Kes Penggunaan

#### 1. Terjemahan Masuk Automatik
Mesej masuk daripada rakan sepasukan dikesan dan diterjemahkan ke bahasa sasaran anda secara langsung, dipaparkan dalam sembang permainan dengan nama dan warna asal pemain:
- Rakan sepasukan (English): `so quiet`
- Sembang Dalam Permainan: `Diveheller: (English): senyap sangat`

#### 2. Balas Automatik kepada Bahasa Masuk Terakhir (`>>mesej`)
Meletakkan awalan `>>` pada mesej sembang anda secara automatik menterjemahkan teks anda ke bahasa mesej rakan sepasukan yang terakhir masuk:
- Input anda dalam sembang: `>>ya, terlalu tenang di sini`
- Rakan sepasukan melihat: `yes, it is too quiet`

#### 3. Pemilihan Bahasa Tertentu / Eksplisit (`bahasa sasaran>>mesej`)
Tujukan mesej anda kepada bahasa tertentu pada bila-bila masa dengan meletakkan awalan nama bahasa diikuti dengan `>>`:

| Perintah Input Sembang | Bahasa Sasaran | Contoh Hasil Disiarkan kepada Rakan Sepasukan |
| :--- | :--- | :--- |
| `russian>>selamat pagi!` | Russian | `доброе утро!` |
| `english>>periuk api di utara` | English | `mines to the north` |
| `japanese>>berhati-hati dengan tembakan!` | Japanese | `射線に注意せよ！` |
| `spanish>>perlukan peluru` | Spanish | `necesito munición` |
| `german>>undur ke titik pengekstrakan` | German | `Rückzug zum Extraktionspunkt` |
| `french>>mari bertahan di sini` | French | `défendons cette position` |

#### 4. Mesej Keluar Biasa & Penindasan Terjemahan Sendiri
Apabila menaip mesej sembang standard tanpa `>>` (cth. `need ammo`, `jom pergi`), mesej anda dihantar terus melalui sembang biasa berbilang pemain. Dengan pilihan **"Jangan terjemah mesej keluar biasa"** diaktifkan (HIDUP secara lalai dalam bar kawalan pantas dan Tetapan), Babel menindas terjemahan mesej anda kembali kepada anda. Jika dimatikan, Babel akan memproses mesej tempatan biasa melalui saluran terjemahan masuk juga.

#### 5. Pintasan Sembang Pantas Taktikal
Tekan sebarang kekunci pintas sembang pantas yang dikonfigurasikan (cth. `NumPad 1`-`NumPad 9`) semasa bermain untuk menyiarkan panggilan pratetap serta-merta ke dalam sembang berbilang pemain tanpa membuka ruang input sembang. Semua penekanan kekunci diteruskan secara lancar ke permainan, memastikan kawalan anda kekal responsif.

#### 6. Tindanan Rujukan Sembang Pantas Taktikal (`Numpad +`)
Tekan `Numpad +` (atau pintasan yang anda rakam dalam tab **Sembang Pantas**) pada bila-bila masa untuk menogol tindanan rujukan atas skrin yang menyenaraikan semua pratetap Sembang Pantas yang aktif. Tindanan dipaparkan dijajarkan ke kanan dan di tengah secara menegak pada skrin, menunjukkan setiap pratetap aktif dalam corak `[<pintasan>] <mesej>` (dengan mesej dipendekkan sehingga 30 aksara) menggunakan gaya visual tidak mengganggu yang sama seperti notifikasi toast.
> [!NOTE]
> **Keperluan Mod Paparan:** Helldivers mesti dikonfigurasikan untuk berjalan dalam mod **Borderless Fullscreen** (atau Windowed) dalam tetapan Video dalam permainan supaya tetingkap tindanan Windows boleh dipaparkan di atas permainan.

#### 7. Jeda, Sambung & Togol Ciri
- **Jeda / Sambung Global (`Ctrl + F10`):** Suis utama yang menggantung/menyambung kedua-dua pemintasan terjemahan dan penyiaran mesej Sembang Pantas secara kemas.
- **Togol Terjemahan Sahaja (`Ctrl + F11`):** Menogol ciri terjemahan sahaja hidup atau mati dengan pemberitahuan toast pada skrin. Sembang Pantas terus berfungsi seperti biasa.
- **Togol Sembang Pantas Sahaja (`Ctrl + F12`):** Menogol penyiaran mesej Sembang Pantas sahaja hidup atau mati dengan pemberitahuan toast pada skrin. Terjemahan sembang dalam permainan terus berfungsi seperti biasa.
*(Semua pintasan boleh disesuaikan dalam tab **Tetapan** dan **Sembang Pantas** menggunakan butang rekod pintasan).*

#### 8. Mengeksport & Log Sembang Berterusan
Klik **Eksport Log** dalam tab Terjemahan & Log pada bila-bila masa untuk memilih lokasi dan mengeksport sejarah sembang anda (dengan cap masa, nama pemain, mesej asal, terjemahan, dan kependaman) ke dokumen `.txt`. Mesej sembang kekal secara automatik dalam jadual merentasi pelancaran aplikasi melalui penulisan cakera kelompok tak segerak (had penyimpanan boleh dikonfigurasikan dari 0 hingga 100 MB dalam Tetapan, dengan butang **Kosongkan memori log**).

#### 9. Pautan Mesej Interaktif
Mesej sembang skuad yang mengandungi URL `http://` atau `https://` dipaparkan sebagai pautan boleh diklik dalam lajur **Mesej Asal**. Apabila diklik, dialog pengesahan akan meminta anda mengesahkan jika anda mempercayai pautan tersebut sebelum membukanya dalam pelayar web lalai anda.

## 4. Pembangunan (Membina daripada Sumber)

### Keperluan Pembangun
Jika anda menyusun dan membina projek ini daripada sumber:
- **Sistem Pengendalian:** Windows 10 / 11 64-bit (x64).
- **.NET 10 SDK:** [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (`net10.0-windows`).
- **Toolchain MSYS2 MinGW64 (Clang++, GCC, Make):** [MSYS2](https://www.msys2.org/) dengan pakej `mingw-w64-x86_64-clang`, `mingw-w64-x86_64-gcc`, dan `mingw-w64-x86_64-make` - diperlukan untuk mengompilasi perpustakaan cangkuk asli (`BabelHook.dll`) dan membina submodul rasmi LuaJIT 2.0 (`vendor/luajit`) untuk mengompilasi skrip bootstrap dalam permainan (`babel_bootstrap.lua`) kepada baitkod.
- **Git:** Git for Windows (dengan submodul rekursif untuk MinHook amalgamation dan LuaJIT 2.0).

### Persediaan Persekitaran Automatik
Dwiklik `setup.bat` (atau jalankan `setup.ps1` dalam PowerShell) di punca repositori. Ia secara automatik mengesahkan dan memasang prasyarat yang tiada:
1. Mengesahkan persekitaran Windows 64-bit.
2. Memeriksa .NET 10 SDK (pemasangan automatik melalui `winget` jika tiada).
3. Memeriksa toolchain MSYS2 MinGW64 (Clang++, GCC, dan Make). Jika tiada, MSYS2 dipasang secara automatik melalui `winget` dan memasang pakej yang diperlukan (`mingw-w64-x86_64-clang`, `mingw-w64-x86_64-gcc`, dan `mingw-w64-x86_64-make`) melalui `pacman`.
4. Memulakan dan membina pengkompil LuaJIT 2.0 daripada submodul (`vendor/luajit`).
5. Memastikan `C:\msys64\mingw64\bin` dan `C:\msys64\usr\bin` ditambah ke PATH secara automatik.
6. Mengesahkan kebergantungan MinHook amalgamation dan menjalankan pemeriksaan asas binaan awal.

### Binaan Utama & Pembungkusan Keluaran
Untuk membersihkan artifak terdahulu, menyusun DLL cangkuk asli, menjalankan semua ujian unit, dan memasang pakej keluaran:
```powershell
pwsh -File build.ps1
```
Direktori output akan dibungkuskan ke dalam `dist/Helldivers1Babel-vX.Y.Z/`.

### Binaan Manual Langkah Demi Langkah
```powershell
# 1. Bina DLL cangkuk asli (secara automatik mengompilasi babel_bootstrap.lua dengan luajit)
pwsh -File src/Babel.Hook/build.ps1

# 2. Bina solusi .NET 10
dotnet build Helldivers1Babel.slnx

# 3. Jalankan suite ujian automatik
dotnet test
```

## 5. Lesen

Diedarkan di bawah [Lesen MIT](../LICENSE).
