# ![Helldivers 1 Babel](screenshots/logo.png)

[![Latest Version](https://img.shields.io/github/v/release/difinim/helldivers1-babel?logo=github&label=release)](https://github.com/difinim/helldivers1-babel/releases/latest)
[![Build Status](https://github.com/difinim/helldivers1-babel/actions/workflows/release.yml/badge.svg)](https://github.com/difinim/helldivers1-babel/actions/workflows/release.yml)
[![Tests](https://github.com/difinim/helldivers1-babel/actions/workflows/tests.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/tests.yml?query=branch%3Amain)
[![CodeQL](https://github.com/difinim/helldivers1-babel/actions/workflows/codeql.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/codeql.yml?query=branch%3Amain)
[![OSV-Scanner](https://github.com/difinim/helldivers1-babel/actions/workflows/osv-scanner.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/osv-scanner.yml?query=branch%3Amain)
[![Dependabot](https://img.shields.io/badge/dependabot-enabled-025E8C?logo=dependabot&logoColor=white)](https://github.com/difinim/helldivers1-babel/network/updates)

*Terjun bersama. Berbicara dalam satu suara.*

**Helldivers 1 Babel** adalah asisten terjemahan obrolan dan komunikasi taktis sumber terbuka yang non-invasif untuk *Helldivers (2015)*, karena *kebebasan terdengar lebih manis saat semua orang memahami rencananya*.

## Translations

[English](../README.md) | [العربية](README.ar.md) | [বাংলা](README.bn.md) | [简体中文](README.zh.md) | [Čeština](README.cs.md) | [Nederlands](README.nl.md) | [Français](README.fr.md) | [Deutsch](README.de.md) | [Ελληνικά](README.el.md) | [हिन्दी](README.hi.md) | [Magyar](README.hu.md) | [Bahasa Indonesia](README.id.md) | [Italiano](README.it.md) | [日本語](README.ja.md) | [한국어](README.ko.md) | [Bahasa Melayu](README.ms.md) | [فارسی](README.fa.md) | [Polski](README.pl.md) | [Português (PT-BR)](README.pt-BR.md) | [Română](README.ro.md) | [Русский](README.ru.md) | [Español](README.es.md) | [Svenska](README.sv.md) | [Tagalog (Filipino)](README.tl.md) | [ไทย](README.th.md) | [Türkçe](README.tr.md) | [Українська](README.uk.md) | [Tiếng Việt](README.vi.md)

> [!IMPORTANT]
> **Pernyataan Kompatibilitas:**
> **Helldivers 1 Babel dikembangkan secara khusus untuk Helldivers (2015) di PC (Steam).**
> **Kompatibel dengan mod.**
> **TIDAK kompatibel dengan Helldivers 2 (2024)**, yang sepenuhnya berbeda dan menggunakan anti-cheat tingkat kernel.

## Daftar Isi

- [1. Ikhtisar & Fitur Utama](#1-ikhtisar--fitur-utama)
- [2. Pengaturan & Pemasangan](#2-pengaturan--pemasangan)
  - [Persyaratan Sistem](#persyaratan-sistem)
  - [Pengaturan Kunci API AI Gratis](#pengaturan-kunci-api-ai-gratis)
  - [Unduh & Pemasangan (Rilis Siap Pakai)](#unduh--pemasangan-rilis-siap-pakai)
- [3. Panduan Penggunaan](#3-panduan-penggunaan)
  - [Memulai](#memulai)
  - [Komunikasi Dalam Game & Skenario Penggunaan](#komunikasi-dalam-game--skenario-penggunaan)
- [4. Pengembangan (Membangun dari Kode Sumber)](#4-pengembangan-membangun-dari-kode-sumber)
  - [Persyaratan Pengembang](#persyaratan-pengembang)
  - [Penyiapan Lingkungan Otomatis](#penyiapan-lingkungan-otomatis)
  - [Build Utama & Pembuatan Paket Rilis](#build-utama--pembuatan-paket-rilis)
  - [Langkah Build Manual](#langkah-build-manual)
- [5. Lisensi](#5-lisensi)

## 1. Ikhtisar & Fitur Utama

- **Terjemahan Obrolan Dua Arah Waktu-Nyata (Real-Time):**
  - **Pesan Masuk:** Mendeteksi bahasa rekan tim secara otomatis, menerjemahkan pesan ke dalam bahasa target pilihan Anda (mendukung hingga 28 bahasa target, termasuk Portugis PT-BR, Inggris, Spanyol, Rusia, Mandarin, Jepang, Prancis, Jerman, dan lainnya), dan merendernya secara **lokal** ke dalam obrolan dalam game dengan **tetap mempertahankan nama pemain asli serta kode warna aslinya** (P1 Emas, P2 Biru, P3 Ungu, P4 Sian).
  - **Pesan Keluar:** Ketik `[bahasa_target]>>pesan` atau cukup ketik `>>pesan` untuk membalas otomatis dalam bahasa pesan terakhir dari rekan tim yang diterima. Teks yang diterjemahkan disiarkan melalui RPC multipemain sehingga rekan tim internasional membacanya dalam bahasa asli mereka.
- **Log Obrolan Persisten & Ekspor Berkas:** Pelacakan riwayat obrolan sesi lengkap dengan stempel waktu, nama pemain, warna, bahasa sumber yang terdeteksi, dan latensi terjemahan. Log tersimpan di tabel melintasi restart aplikasi melalui penulisan berkas disk secara batch asinkron yang ringan (dapat dikonfigurasi), dengan ekspor sekali klik ke berkas `.txt`.
- **Log Obrolan yang Dapat Dicari:** Riwayat obrolan yang dapat dicari sepenuhnya dengan memfilter kolom apa pun.

![Helldivers 1 Babel beraksi](screenshots/screenshot.png)

- **Obrolan Cepat Taktis (10 Slot Kustom):** 10 slot pesan taktis pratetap yang dapat disesuaikan, masing-masing terikat ke tombol pintas yang dapat dikonfigurasi pengguna (misalnya Numpad 0-9 atau kombinasi kustom) untuk mengirimkan pesan taktis secara instan ke obrolan multipemain dalam game.

![Obrolan Cepat](screenshots/quick-chat.png)

- **Pesan Obrolan Cepat Sambutan Otomatis:** Secara otomatis menyiarkan pesan sambutan yang dapat disesuaikan ke obrolan dalam game saat Anda menjadi host lobi dan ada pemain tamu yang bergabung.
- **Hamparan Referensi Obrolan Cepat di Layar:** Alihkan hamparan referensi mengambang non-intrusif di sisi kanan layar dengan `Numpad +` (atau pintasan yang Anda rekam) untuk memeriksa dengan cepat semua perintah taktis dan pintasan aktif saat bermain game. *(Memerlukan Helldivers berjalan dalam mode Borderless Fullscreen atau Windowed).*

![Hamparan Obrolan Cepat](screenshots/overlay.png)

- **Dukungan UI Multi-Bahasa:** Mendukung 28 Bahasa.
- **Arsitektur Adaptor Penyedia:** Dibangun dengan antarmuka penyedia AI yang modular. Dilengkapi adaptor teroptimasi zero-thinking untuk **Google Gemini** (`gemini-flash-lite-latest` / `gemini-2.0-flash-lite`), dengan kontrak penyedia yang dapat diperluas untuk OpenAI, Claude, dan DeepSeek (segera hadir).
- **Pengaturan Nada Terjemahan:** Kasual: bahasa gaul game informal dan istilah taktis gamer / Normal: nada percakapan alami tanpa bahasa gaul game.
- **Pintasan Global Jeda / Lanjutkan:** Tekan `Ctrl + F10` (atau pintasan yang Anda konfigurasi) kapan saja untuk beralih antara status **Aktif** dan **Dijeda**. Menjeda akan menangguhkan intersepsi terjemahan obrolan dalam game dan penyiaran pesan Obrolan Cepat secara bersih.
- **Pintasan Fitur Khusus:**
  - **Alihkan Terjemahan Saja:** Tekan `Ctrl + F11` (atau pintasan terekam Anda) untuk menjeda atau melanjutkan terjemahan obrolan secara instan tanpa memengaruhi Obrolan Cepat (dengan notifikasi layar toast).
  - **Alihkan Obrolan Cepat Saja:** Tekan `Ctrl + F12` (atau pintasan terekam Anda) untuk menjeda atau melanjutkan Obrolan Cepat secara instan tanpa memengaruhi terjemahan (dengan notifikasi layar toast).

![Pengaturan](screenshots/settings.png)

- **Penyimpanan Kredensial yang Aman (DPAPI):** Preferensi sensitif seperti token API penyedia AI dienkripsi saat disimpan menggunakan Windows Data Protection API (DPAPI, cakupan `CurrentUser`). Kredensial tidak pernah disimpan dalam bentuk teks biasa di berkas konfigurasi dan hanya dapat didekripsi di komputer lokal oleh akun Windows Anda.
- **Nol Telemetri & Privasi Penuh:** Alat ini tidak mengumpulkan, mencatat, atau mengirim data pengguna atau data telemetri apa pun. Semua permintaan terjemahan berkomunikasi secara langsung dan eksklusif dengan endpoint penyedia AI yang Anda konfigurasi menggunakan kredensial API pribadi Anda.
- **100% Non-Invasif (Nol Modifikasi Berkas Game):** Tidak ada arsip game yang diekstrak atau dimodifikasi. Berkas game Anda tetap 100% orisinal (vanilla) dan tidak tersentuh.

## 2. Pengaturan & Pemasangan

### Persyaratan Sistem
Jika Anda menjalankan rilis mandiri yang telah dikompilasi (`HD1-Babel.exe`):
- **Sistem Operasi:** Windows 10 / 11 64-bit (x64).
- **Desktop Runtime:** [Microsoft .NET 10 Desktop Runtime (x64)](https://aka.ms/dotnet/10.0/windowsdesktop-runtime-win-x64.exe) (penginstal langsung).
- **Game Target:** *Helldivers* (2015) di Steam (64-bit `helldivers.exe`).

### Pengaturan Kunci API AI Gratis
- **Kredensial Penyedia AI:** Kunci API Google Gemini (dapatkan kunci pribadi gratis dari [Google AI Studio](https://aistudio.google.com/) ([Lihat video tutorial ini](https://www.youtube.com/watch?v=YHFMrZD-_3g))).
- **Akses Internet:** Diperlukan untuk panggilan API terjemahan.

### Unduh & Pemasangan (Rilis Siap Pakai)

Anda **tidak** perlu memasang alat pengembang atau mengompilasi dari kode sumber untuk menggunakan Babel ([tetapi jika Anda ingin melakukannya](#4-pengembangan-membangun-dari-kode-sumber)):
1. Buka bagian **[Releases](https://github.com/difinim/helldivers1-babel/releases)** di sisi kanan halaman proyek GitHub.
2. Di bawah rilis terbaru, buka daftar **Assets**.
3. Unduh arsip distribusi mandiri: **`Helldivers1Babel-v<versi>-win-x64.zip`**.
4. Ekstrak berkas zip di mana saja di komputer Anda (misalnya `C:\GamesTools\Helldivers1Babel`).
5. Buka folder dan jalankan **`HD1-Babel.exe`**.

> [!NOTE]
> **Prasyarat:** Pengguna akhir hanya memerlukan [Microsoft .NET 10 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/10.0) gratis. Tidak diperlukan kompiler C++, SDK, atau toolchain untuk menjalankan rilis ini.

> [!NOTE]
> **Harap diperhatikan:** Karena InjectorService.cs menggunakan API injeksi proses standar Windows (`VirtualAllocEx` + `WriteProcessMemory` + `CreateRemoteThread`), Windows Defender atau Antivirus pihak ketiga _**mungkin**_ memicu deteksi heuristik (misalnya Generic HackTool). Ini adalah perilaku normal untuk injektor mod game dan dapat dimasukkan ke daftar pengecualian (allowlist) dengan aman.

## 3. Panduan Penggunaan

### Memulai
1. Jalankan **Helldivers 1 Babel** (`HD1-Babel.exe`).
2. Saat peluncuran pertama, buka tab **Pengaturan**:
   - Masukkan **Kunci API Google Gemini** Anda (disimpan dengan aman melalui enkripsi Windows DPAPI).
   - Pilih bahasa target Anda (misalnya `Bahasa Indonesia`).
   - Klik **Uji Koneksi API** untuk memverifikasi kunci dan latensi Anda.
   - Sesuaikan pintasan **Jeda / Lanjutkan** jika diinginkan (default ke `Ctrl + F10`).
3. Konfigurasikan pesan taktis Anda di tab **Obrolan Cepat** (misalnya tetapkan Numpad 0-9 untuk pesan pilihan Anda).
4. Luncurkan **Helldivers** di Steam. Babel secara otomatis mendeteksi `helldivers.exe`, menginjeksi `BabelHook.dll`, dan membuat koneksi IPC Named Pipe berkecepatan tinggi.

### Komunikasi Dalam Game & Skenario Penggunaan

#### 1. Terjemahan Masuk Otomatis
Pesan masuk dari rekan tim terdeteksi dan diterjemahkan ke bahasa target Anda secara langsung, ditampilkan di obrolan game dengan nama dan warna pemain asli:
- Rekan tim (English): `so quiet`
- Obrolan Dalam Game: `Diveheller: (English): sunyi sekali`

#### 2. Balas Otomatis ke Bahasa Terakhir yang Diterima (`>>pesan`)
Awali pesan obrolan Anda dengan `>>` untuk menerjemahkan teks Anda secara otomatis ke bahasa pesan terakhir dari rekan tim:
- Input Anda di obrolan: `>>ya, terlalu tenang di sini`
- Rekan tim melihat: `yes, it is too quiet`

#### 3. Pemilihan Bahasa Tertentu / Eksplisit (`bahasa sasaran>>pesan`)
Tujukan pesan Anda ke bahasa tertentu kapan saja dengan mengawalinya dengan nama bahasa yang diikuti dengan `>>`:

| Perintah Input Obrolan | Bahasa Sasaran | Contoh Hasil yang Disiarkan ke Rekan Tim |
| :--- | :--- | :--- |
| `russian>>selamat pagi!` | Russian | `доброе утро!` |
| `english>>ranjau di utara` | English | `mines to the north` |
| `japanese>>hati-hati dengan tembakan!` | Japanese | `射線に注意せよ！` |
| `spanish>>butuh amunisi` | Spanish | `necesito munición` |
| `german>>mundur ke titik ekstraksi` | German | `Rückzug zum Extraktionspunkt` |
| `french>>mari bertahan di sini` | French | `défendons cette position` |

#### 4. Pesan Keluar Standar & Penekanan Terjemahan Sendiri
Saat mengetik pesan obrolan standar tanpa `>>` (misalnya `need ammo`, `ayo maju`), pesan Anda dikirim langsung melalui obrolan biasa multipemain. Dengan opsi **"Jangan terjemahkan pesan keluar standar"** diaktifkan (AKTIF secara default di bilah kontrol cepat dan Pengaturan), Babel mencegah pesan Anda diterjemahkan kembali ke diri Anda sendiri. Jika dimatikan (OFF), Babel juga akan memproses pesan lokal standar melalui alur terjemahan masuk.

#### 5. Pintasan Obrolan Cepat Taktis
Tekan tombol pintas obrolan cepat yang telah dikonfigurasi (misalnya `NumPad 1`-`NumPad 9`) saat bermain untuk langsung menyiarkan pesan taktis pratetap ke obrolan multipemain tanpa membuka kotak teks obrolan. Penekanan tombol diteruskan ke game secara mulus, menjaga kontrol Anda tetap responsif.

#### 6. Hamparan Referensi Obrolan Cepat Taktis (`Numpad +`)
Tekan `Numpad +` (atau pintasan yang Anda rekam di tab **Obrolan Cepat**) kapan saja untuk mengalihkan hamparan referensi di layar yang mencantumkan semua preset Obrolan Cepat yang aktif. Hamparan muncul rata kanan dan berada di tengah vertikal layar, menampilkan setiap preset aktif dalam format `[<pintasan>] <pesan>` (dengan pesan dipotong hingga 30 karakter) menggunakan gaya visual non-intrusif yang sama seperti notifikasi toast.
> [!NOTE]
> **Persyaratan Mode Tampilan:** Helldivers harus dikonfigurasi untuk berjalan dalam mode **Borderless Fullscreen** (atau Windowed) di pengaturan Video game agar jendela hamparan Windows dapat ditampilkan di atas game.

#### 7. Jeda, Lanjutkan & Sakelar Fitur
- **Jeda / Lanjutkan Global (`Ctrl + F10`):** Sakelar utama yang menangguhkan/melanjutkan intersepsi terjemahan dan penyiaran pesan Obrolan Cepat secara bersih.
- **Alihkan Terjemahan Saja (`Ctrl + F11`):** Mengaktifkan atau menonaktifkan fitur terjemahan saja dengan notifikasi toast di layar. Obrolan Cepat tetap berfungsi normal.
- **Alihkan Obrolan Cepat Saja (`Ctrl + F12`):** Mengaktifkan atau menonaktifkan penyiaran pesan Obrolan Cepat saja dengan notifikasi toast di layar. Terjemahan obrolan tetap berfungsi normal.
*(Semua pintasan dapat dikustomisasi di tab **Pengaturan** dan **Obrolan Cepat** menggunakan tombol perekam pintasan).*

#### 8. Ekspor & Log Obrolan Persisten
Klik **Ekspor Log** di tab Terjemahan & Log kapan saja untuk memilih lokasi dan mengekspor riwayat obrolan Anda (dengan stempel waktu, nama pemain, pesan asli, terjemahan, dan latensi) ke berkas `.txt`. Pesan obrolan disimpan secara otomatis di tabel melintasi peluncuran aplikasi melalui penulisan batch asinkron ke disk (batas penyimpanan dapat dikonfigurasi dari 0 hingga 100 MB di Pengaturan, disertai tombol **Bersihkan memori log**).

#### 9. Tautan Pesan Interaktif
Pesan obrolan skuad yang berisi URL `http://` atau `https://` dirender sebagai tautan yang dapat diklik di kolom **Pesan Asli**. Saat diklik, kotak dialog konfirmasi akan meminta Anda memverifikasi apakah Anda mempercayai tautan tersebut sebelum membukanya di peramban web default Anda.

## 4. Pengembangan (Membangun dari Kode Sumber)

### Persyaratan Pengembang
Jika Anda mengompilasi dan membangun proyek dari kode sumber:
- **Sistem Operasi:** Windows 10 / 11 64-bit (x64).
- **.NET 10 SDK:** [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (`net10.0-windows`).
- **Toolchain MSYS2 MinGW64 (Clang++, GCC, Make):** [MSYS2](https://www.msys2.org/) dengan paket `mingw-w64-x86_64-clang`, `mingw-w64-x86_64-gcc`, dan `mingw-w64-x86_64-make` - diperlukan untuk mengompilasi pustaka hook native (`BabelHook.dll`) dan membangun submodul resmi LuaJIT 2.0 (`vendor/luajit`) guna mengompilasi skrip bootstrap dalam game (`babel_bootstrap.lua`) ke bytecode.
- **Git:** Git for Windows (dengan submodul rekursif untuk MinHook amalgamation dan LuaJIT 2.0).

### Penyiapan Lingkungan Otomatis
Klik dua kali `setup.bat` (atau jalankan `setup.ps1` di PowerShell) di root repositori. Skrip ini secara otomatis memverifikasi dan memasang prasyarat yang belum ada:
1. Memverifikasi lingkungan Windows 64-bit.
2. Memeriksa .NET 10 SDK (otomatis memasang lewat `winget` jika belum terpasang).
3. Memeriksa toolchain MSYS2 MinGW64 (Clang++, GCC, dan Make). Jika belum terpasang, otomatis memasang MSYS2 melalui `winget` dan menginstal paket yang diperlukan (`mingw-w64-x86_64-clang`, `mingw-w64-x86_64-gcc`, dan `mingw-w64-x86_64-make`) via `pacman`.
4. Menginisialisasi dan membangun kompiler LuaJIT 2.0 dari submodul (`vendor/luajit`).
5. Memastikan `C:\msys64\mingw64\bin` dan `C:\msys64\usr\bin` ditambahkan ke PATH secara otomatis.
6. Memverifikasi dependensi MinHook amalgamation dan menjalankan pemeriksaan awal kewarasan build.

### Build Utama & Pembuatan Paket Rilis
Untuk membersihkan artefak sebelumnya, mengompilasi hook DLL native, menjalankan seluruh unit test, dan menyusun paket rilis:
```powershell
pwsh -File build.ps1
```
Direktori output akan dikemas ke dalam `dist/Helldivers1Babel-vX.Y.Z/`.

### Langkah Build Manual
```powershell
# 1. Bangun hook DLL native (otomatis mengompilasi babel_bootstrap.lua dengan luajit)
pwsh -File src/Babel.Hook/build.ps1

# 2. Bangun solusi .NET 10
dotnet build Helldivers1Babel.slnx

# 3. Jalankan rangkaian tes otomatis
dotnet test
```

## 5. Lisensi

Didistribusikan di bawah [Lisensi MIT](../LICENSE).
