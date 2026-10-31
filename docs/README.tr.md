# ![Helldivers 1 Babel](screenshots/logo.png)

[![Latest Version](https://img.shields.io/github/v/release/difinim/helldivers1-babel?logo=github&label=release)](https://github.com/difinim/helldivers1-babel/releases/latest)
[![Build Status](https://github.com/difinim/helldivers1-babel/actions/workflows/release.yml/badge.svg)](https://github.com/difinim/helldivers1-babel/actions/workflows/release.yml)
[![Tests](https://github.com/difinim/helldivers1-babel/actions/workflows/tests.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/tests.yml?query=branch%3Amain)
[![CodeQL](https://github.com/difinim/helldivers1-babel/actions/workflows/codeql.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/codeql.yml?query=branch%3Amain)
[![OSV-Scanner](https://github.com/difinim/helldivers1-babel/actions/workflows/osv-scanner.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/osv-scanner.yml?query=branch%3Amain)
[![Dependabot](https://img.shields.io/badge/dependabot-enabled-025E8C?logo=dependabot&logoColor=white)](https://github.com/difinim/helldivers1-babel/network/updates)

*Birlikte dalış yapın. Tek ses olun.*

**Helldivers 1 Babel**, *Helldivers (2015)* için açık kaynaklı, müdahalesiz bir sohbet çevirisi ve taktiksel iletişim asistanıdır; çünkü *tatlı özgürlüğün sesi, herkes planı anladığında kulağa çok daha güzel gelir*.

## Translations

[English](../README.md) | [العربية](README.ar.md) | [বাংলা](README.bn.md) | [简体中文](README.zh.md) | [Čeština](README.cs.md) | [Nederlands](README.nl.md) | [Français](README.fr.md) | [Deutsch](README.de.md) | [Ελληνικά](README.el.md) | [हिन्दी](README.hi.md) | [Magyar](README.hu.md) | [Bahasa Indonesia](README.id.md) | [Italiano](README.it.md) | [日本語](README.ja.md) | [한국어](README.ko.md) | [Bahasa Melayu](README.ms.md) | [فارسی](README.fa.md) | [Polski](README.pl.md) | [Português (PT-BR)](README.pt-BR.md) | [Română](README.ro.md) | [Русский](README.ru.md) | [Español](README.es.md) | [Svenska](README.sv.md) | [Tagalog (Filipino)](README.tl.md) | [ไทย](README.th.md) | [Türkçe](README.tr.md) | [Українська](README.uk.md) | [Tiếng Việt](README.vi.md)

> [!IMPORTANT]
> **Uyumluluk Uyarısı:**
> **Helldivers 1 Babel, yalnızca PC (Steam) üzerindeki Helldivers (2015) için geliştirilmiştir.**
> **Modlarla uyumludur.**
> Tamamen farklı olan ve çekirdek (kernel) düzeyinde hile koruması kullanan **Helldivers 2 (2024) ile UYUMLU DEĞİLDİR**.

## İçindekiler

- [1. Genel Bakış ve Temel Özellikler](#1-genel-bakış-ve-temel-özellikler)
- [2. Kurulum](#2-kurulum)
  - [Gereksinimler](#gereksinimler)
  - [Ücretsiz Yapay Zeka API Anahtarı Alma](#ücretsiz-yapay-zeka-api-anahtarı-alma)
  - [İndirme ve Kurulum (Hazır Sürüm)](#indirme-ve-kurulum-hazır-sürüm)
- [3. Kullanım Kılavuzu](#3-kullanım-kılavuzu)
  - [Başlarken](#başlarken)
  - [Oyun İçi İletişim ve Kullanım Senaryoları](#oyun-içi-iletişim-ve-kullanım-senaryoları)
- [4. Geliştirme (Kaynaktan Derleme)](#4-geliştirme-kaynaktan-derleme)
  - [Geliştirici Gereksinimleri](#geliştirici-gereksinimleri)
  - [Otomatik Ortam Kurulumu](#otomatik-ortam-kurulumu)
  - [Ana Derleme ve Sürüm Paketleme](#ana-derleme-ve-sürüm-paketleme)
  - [Adım Adım Manuel Derleme](#adım-adım-manuel-derleme)
- [5. Lisans](#5-lisans)

## 1. Genel Bakış ve Temel Özellikler

- **Gerçek Zamanlı Çift Yönlü Sohbet Çevirisi:**
  - **Gelen Mesajlar:** Takım arkadaşlarınızın dilini otomatik olarak algılar, mesajları seçtiğiniz hedef dile çevirir (Portekizce PT-BR, İngilizce, İspanyolca, Rusça, Çince, Japonca, Fransızca, Almanca ve daha fazlası dahil 28 hedef dili destekler) ve **orijinal takım arkadaşının oyuncu adını ve renk kodunu** (O1 Altın, O2 Mavi, O3 Mor, O4 Camgöbeği) **birebir koruyarak** oyun içi sohbete **yerel** olarak aktarır.
  - **Giden Mesajlar:** Son gelen takım arkadaşı mesajının dilinde otomatik yanıt vermek için `[hedef_dil]>>mesaj` veya yalnızca `>>mesaj` yazın. Çevrilen metin çok oyunculu RPC üzerinden yayınlanır, böylece yabancı takım arkadaşları bunu kendi dillerinde okur.
- **Kalıcı Sohbet Günlükleri ve Dosyaya Aktarma:** Zaman damgaları, oyuncu adları, renkler, algılanan kaynak diller ve çeviri gecikmesiyle tam oturum sohbet geçmişi takibi. Günlükler, hafif asenkron toplu disk yazımları (yapılandırılabilir) aracılığıyla uygulama yeniden başlatılsa bile tabloda korunur; tek tıklamayla bir `.txt` dosyasına aktarılabilir.
- **Aranabilir Sohbet Günlükleri:** Tüm sütunlara göre filtreleme yapabilen, tamamen aranabilir sohbet geçmişi.

![Helldivers 1 Babel çalışırken](screenshots/screenshot.png)

- **Taktiksel Hızlı Sohbet (10 Özel Yuva):** Taktiksel çağrıları oyun içi çok oyunculu sohbete anında iletmek için her biri kullanıcı tarafından yapılandırılabilen kısayol tuşlarına (ör. Numpad 0-9 veya özel kombinasyonlar) atanmış 10 özelleştirilebilir önceden tanımlanmış taktiksel mesaj yuvası.
- **Otomatik Karşılama Hızlı Sohbet Mesajı:** Bir lobiye ev sahipliği yaptığınızda ve konuk bir oyuncu katıldığında oyun içi sohbete otomatik olarak özelleştirilebilir bir karşılama mesajı yayınlar.

![Hızlı Sohbet](screenshots/quick-chat.png)

- **Ekran Üzerinde Hızlı Sohbet Referans Arayüzü:** Oyun içindeki tüm aktif taktiksel çağrıları ve kısayolları hızlıca incelemek için `Numpad +` (veya kaydettiğiniz kısayol) ile ekranın sağ tarafında odak almayan kayan bir referans arayüzünü açıp kapatın. *(Helldivers'ın Çerçevesiz Pencereli veya Pencereli modda çalışmasını gerektirir).*

![Hızlı Sohbet arayüzü](screenshots/overlay.png)

- **Çoklu Dil Arayüz Desteği:** 28 Dili destekler.
- **Sağlayıcı Adaptör Mimarisi:** Modüler bir yapay zeka sağlayıcı arayüzü ile inşa edilmiştir. **Google Gemini** (`gemini-flash-lite-latest` / `gemini-2.0-flash-lite`) için optimize edilmiş, düşünme gecikmesi olmayan bir adaptörle gelir; OpenAI, Claude ve DeepSeek için genişletilebilir sağlayıcı sözleşmelerine sahiptir (henüz uygulanmamıştır).
- **Çeviri Tonu Ayarı:** Günlük: resmi olmayan oyun argosu ve taktiksel oyuncu jargonu / Normal: oyun argosu içermeyen doğal konuşma dili.
- **Genel Duraklat / Sürdür Kısayolu:** Asistanı **Aktif** ve **Duraklatıldı** arasında değiştirmek için istediğiniz zaman `Ctrl + F10` (veya yapılandırdığınız kısayola) basın. Duraklatma, hem oyun içi sohbet çevirisini hem de Hızlı Sohbet mesajı gönderimini temiz bir şekilde askıya alır.
- **Özel İşlev Kısayolları:**
  - **Yalnızca Çeviriyi Aç/Kapat:** Hızlı Sohbeti etkilemeden yalnızca sohbet çevirisini anında duraklatmak veya sürdürmek için `Ctrl + F11` (veya kaydettiğiniz kısayola) basın (ekran bildirimi geri bildirimi ile).
  - **Yalnızca Hızlı Sohbeti Aç/Kapat:** Çeviriyi etkilemeden yalnızca Hızlı Sohbet çağrılarını anında duraklatmak veya sürdürmek için `Ctrl + F12` (veya kaydettiğiniz kısayola) basın (ekran bildirimi geri bildirimi ile).

![Ayarlar](screenshots/settings.png)

- **Güvenli Kimlik Bilgisi Depolama (DPAPI):** Yapay zeka sağlayıcısı API belirteçleri gibi hassas tercihler, Windows Veri Koruma API'si (DPAPI, `CurrentUser` kapsamı) kullanılarak bekleme durumunda şifrelenir. Kimlik bilgileri yapılandırma dosyalarında hiçbir zaman düz metin olarak saklanmaz ve yalnızca yerel makinenizde Windows hesabınız tarafından çözülebilir.
- **Sıfır Telemetri ve Tam Gizlilik:** Bu araç hiçbir kullanıcı verisi veya telemetri verisi toplamaz, günlüğe kaydetmez veya göndermez. Tüm çeviri istekleri, kişisel API kimlik bilgileriniz kullanılarak doğrudan ve yalnızca yapılandırılmış yapay zeka sağlayıcısı uç noktanızla iletişim kurar.
- **%100 Müdahalesiz (Sıfır Oyun Dosyası Değişikliği):** Hiçbir oyun arşivi açılmaz veya değiştirilmez. Oyun dosyalarınız %100 orijinal ve ellenmemiş olarak kalır.




## 2. Kurulum

### Gereksinimler
Önceden derlenmiş tek başına çalışan sürümü (`HD1-Babel.exe`) çalıştırıyorsanız:
- **İşletim Sistemi:** Windows 10 / 11 64-bit (x64).
- **Masaüstü Çalışma Zamanı:** [Microsoft .NET 10 Desktop Runtime (x64)](https://aka.ms/dotnet/10.0/windowsdesktop-runtime-win-x64.exe) (doğrudan yükleyici).
- **Hedef Oyun:** Steam üzerindeki *Helldivers* (2015) (64-bit `helldivers.exe`).

### Ücretsiz Yapay Zeka API Anahtarı Alma
- **Yapay Zeka Sağlayıcı Kimlik Bilgileri:** Google Gemini API Anahtarı ([Google AI Studio](https://aistudio.google.com/) üzerinden ücretsiz kişisel anahtar edinin ([Bu eğitim videosunu izleyin](https://www.youtube.com/watch?v=YHFMrZD-_3g))).
- **İnternet Erişimi:** Çeviri API çağrıları için gereklidir.

### İndirme ve Kurulum (Hazır Sürüm)

Babel'ı kullanmak için geliştirici araçları yüklemenize veya kaynaktan derlemenize gerek **yoktur** ([ancak isterseniz yapabilirsiniz](#4-geliştirme-kaynaktan-derleme)):
1. GitHub proje sayfasının sağ tarafındaki **[Releases](https://github.com/difinim/helldivers1-babel/releases)** bölümüne gidin.
2. En son sürümün altındaki **Assets** listesini genişletin.
3. Bağımsız dağıtım arşivini indirin: **`Helldivers1Babel-v<version>-win-x64.zip`**.
4. Zip dosyasını bilgisayarınızda herhangi bir yere çıkartın (ör. `C:\GamesTools\Helldivers1Babel`).
5. Klasörü açın ve **`HD1-Babel.exe`** dosyasını çalıştırın.

> [!NOTE]
> **Önkoşul:** Son kullanıcıların yalnızca ücretsiz [Microsoft .NET 10 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/10.0) paketine ihtiyacı vardır. Sürümü çalıştırmak için C++ derleyicisi, SDK veya araç zinciri gerekmez.

> [!NOTE]
> **Dikkat**: InjectorService.cs standart Windows işlem enjeksiyon API'lerini (`VirtualAllocEx` + `WriteProcessMemory` + `CreateRemoteThread`) kullandığından, Windows Defender veya üçüncü taraf antivirüs programları bulgusal bir algılama (heuristic detection, ör. Generic HackTool) tetikleyebilir. Bu, oyun mod enjektörleri için normal bir durumdur ve güvenle izin verilenler listesine (allowlist) eklenebilir.



## 3. Kullanım Kılavuzu

### Başlarken
1. **Helldivers 1 Babel** uygulamasını başlatın (`HD1-Babel.exe`).
2. İlk başlatmada **Ayarlar** sekmesini açın:
   - **Google Gemini API Anahtarı**nızı girin (Windows DPAPI şifrelemesi ile güvenle saklanır).
   - Hedef dilinizi seçin (ör. `Türkçe` veya `Portuguese (PT-BR)`).
   - Anahtarınızı ve gecikme sürenizi doğrulamak için **API Bağlantısını Test Et** düğmesine tıklayın.
   - İsterseniz **Duraklat / Sürdür** kısayolunu özelleştirin (varsayılan olarak `Ctrl + F10`).
3. Taktiksel çağrılarınızı **Hızlı Sohbet** sekmesinde yapılandırın (ör. Numpad 0-9 tuşlarını istediğiniz mesajlara atayın).
4. Steam üzerinden **Helldivers**'ı başlatın. Babel otomatik olarak `helldivers.exe` sürecini tespit eder.

### Oyun İçi İletişim ve Kullanım Senaryoları

#### 1. Otomatik Gelen Çevirisi
Takım arkadaşlarınızdan gelen mesajlar anında tespit edilir ve hedef dilinize çevrilerek orijinal oyuncunun adı ve rengiyle oyun içi sohbette gösterilir:
- Takım arkadaşı (İngilizce): `so quiet`
- Oyun İçi Sohbet: `Diveheller: (İngilizce): çok sessiz`

#### 2. Son Gelen Dile Otomatik Yanıt Verme (`>>mesaj`)
Sohbet mesajınızın başına `>>` eklemek, metninizi otomatik olarak en son gelen takım arkadaşı mesajının diline çevirir:
- Sohbetteki girişiniz: `>>evet, fazla sakin`
- Takım arkadaşının gördüğü: `yes, it is too quiet`

#### 3. Belirli / İsteğe Bağlı Dil Seçimi (`hedef dil>>mesaj`)
Mesajınızın başına dil adını ve ardından `>>` ekleyerek mesajınızı istediğiniz zaman belirli bir dile yönlendirin:

| Sohbet Giriş Komutu | Hedef Dil | Takım Arkadaşlarına Yayınlanan Örnek Sonuç |
| :--- | :--- | :--- |
| `russian>>günaydın!` | Rusça | `доброе утро!` |
| `english>>kuzeyde mayınlar var` | İngilizce | `mines to the north` |
| `japanese>>ateş hattına dikkat edin!` | Japonca | `射線に注意せよ！` |
| `spanish>>cephane lazım` | İspanyolca | `necesito munición` |
| `german>>tahliye noktasına geri çekilin` | Almanca | `Rückzug zum Extraktionspunkt` |
| `french>>burayı savunalım` | Fransızca | `défendons cette position` |

#### 4. Standart Giden Mesajlar ve Kendi Kendini Çevirmeyi Engelleme
`>>` olmadan standart bir oyun içi sohbet mesajı yazarken (ör. `need ammo`, `hadi gidelim`), mesajınız doğrudan orijinal çok oyunculu sohbet üzerinden iletilir. **"Standart giden mesajları çevirme"** seçeneği etkinken (hem hızlı denetim çubuğunda hem de Ayarlar'da varsayılan olarak AÇIKTIR), Babel kendi mesajınızı tekrar size çevirmeyi engeller. KAPALI konuma getirilirse, Babel standart yerel mesajları da gelen çeviri hattı üzerinden işler.

#### 5. Taktiksel Hızlı Sohbet Kısayolları
Sohbet satırını açmadan önceden ayarlanmış çağrıları çok oyunculu sohbete anında iletmek için oyun sırasında yapılandırılmış herhangi bir hızlı sohbet kısayol tuşuna (ör. `NumPad 1`-`NumPad 9`) basın. Kontrollerinizin yanıt vermeye devam etmesini sağlamak için tüm tuş basımları oyuna sorunsuz bir şekilde iletilir.

#### 6. Taktiksel Hızlı Sohbet Referans Arayüzü (`Numpad +`)
Tüm aktif Hızlı Sohbet önayarlarını listeleyen ekran üstü referans arayüzünü açıp kapatmak için istediğiniz zaman `Numpad +` tuşuna (veya **Hızlı Sohbet** sekmesinde kaydettiğiniz kısayola) basın. Arayüz ekranın sağ tarafına hizalı ve dikey olarak ortalanmış şekilde görünür; bildirimlerle aynı rahatsız etmeyen görsel stili kullanarak her aktif önayarı `[<kısayol>] <mesaj>` düzeninde (mesajlar 30 karakterle sınırlandırılarak) gösterir.
> [!NOTE]
> **Görüntü Modu Gereksinimi:** Windows arayüz pencerelerinin oyun üzerinde gösterilebilmesi için Helldivers'ın oyun içi Video ayarlarında **Çerçevesiz Pencereli** (veya Pencereli) modda çalışacak şekilde yapılandırılması gerekir.

#### 7. Duraklatma, Sürdürme ve Özellik Anahtarları
- **Genel Duraklat / Sürdür (`Ctrl + F10`):** Hem çeviri yakalamayı hem de Hızlı Sohbet mesajı yayınlamayı temiz bir şekilde askıya alan/sürdüren ana anahtardır.
- **Yalnızca Çeviriyi Aç/Kapat (`Ctrl + F11`):** Ekran bildirimi ile Hızlı Sohbeti etkilemeden yalnızca çeviri özelliğini açar veya kapatır. Hızlı Sohbet normal şekilde çalışmaya devam eder.
- **Yalnızca Hızlı Sohbeti Aç/Kapat (`Ctrl + F12`):** Ekran bildirimi ile çeviriyi etkilemeden yalnızca Hızlı Sohbet mesajı yayınlamayı açar veya kapatır. Oyun içi sohbet çevirisi normal şekilde çalışmaya devam eder.
*(Tüm kısayollar, tıklayıp kaydetme kısayol düğmeleri kullanılarak **Ayarlar** ve **Hızlı Sohbet** sekmelerinde özelleştirilebilir).*

#### 8. Dışa Aktarma ve Kalıcı Sohbet Günlükleri
Sohbet geçmişinizi (zaman damgaları, oyuncu adları, orijinal mesajlar, çeviriler ve gecikme süreleriyle birlikte) bir `.txt` belgesine aktarmak üzere bir konum seçmek için Çeviri ve Kayıtlar sekmesindeki **Kayıtları Dışa Aktar** düğmesine istediğiniz zaman tıklayın. Sohbet mesajları, asenkron toplu disk yazımları sayesinde uygulama açılışları arasında tabloda otomatik olarak kalıcı kalır (depolama sınırı Ayarlar'da 0 ile 100 MB arasında yapılandırılabilir; ayrıca **Kayıt belleğini temizle** düğmesi mevcuttur).

#### 9. Etkileşimli Mesaj Bağlantıları
`http://` veya `https://` URL'lerini içeren manga sohbet mesajları, **Orijinal Mesaj** sütununda tıklanabilir bağlantılar olarak işlenir. Tıklandığında, bağlantıyı varsayılan web tarayıcınızda açmadan önce güvenip güvenmediğinizi doğrulamanızı isteyen bir onay iletişim kutusu görüntülenir.



## 4. Geliştirme (Kaynaktan Derleme)

### Geliştirici Gereksinimleri
Projeyi kaynaktan derliyor ve oluşturuyorsanız:
- **İşletim Sistemi:** Windows 10 / 11 64-bit (x64).
- **.NET 10 SDK:** [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (`net10.0-windows`).
- **MSYS2 MinGW64 Araç Zinciri (Clang++, GCC, Make):** [MSYS2](https://www.msys2.org/) ile `mingw-w64-x86_64-clang`, `mingw-w64-x86_64-gcc` ve `mingw-w64-x86_64-make` paketleri - yerel kanca kütüphanesini (`BabelHook.dll`) derlemek ve oyun içi önyükleme betiğini (`babel_bootstrap.lua`) bayt koduna derlemek amacıyla resmi LuaJIT 2.0 alt modülünü (`vendor/luajit`) derlemek için gereklidir.
- **Git:** Windows için Git (MinHook birleştirmesi ve LuaJIT 2.0 için özyinelemeli alt modüllerle birlikte).


### Otomatik Ortam Kurulumu
Depo kök dizinindeki `setup.bat` dosyasına çift tıklayın (veya PowerShell'de `setup.ps1` dosyasını çalıştırın). Eksik önkoşulları otomatik olarak doğrular ve yükler:
1. 64-bit Windows ortamını doğrular.
2. .NET 10 SDK'yı denetler (eksikse `winget` aracılığıyla otomatik yükler).
3. MSYS2 MinGW64 araç zincirini (Clang++, GCC ve Make) denetler. Eksikse, `winget` ile MSYS2'yi otomatik yükler ve `pacman` aracılığıyla gerekli paketleri (`mingw-w64-x86_64-clang`, `mingw-w64-x86_64-gcc` ve `mingw-w64-x86_64-make`) kurar.
4. Alt modülden (`vendor/luajit`) LuaJIT 2.0 derleyicisini başlatır ve derler.
5. `C:\msys64\mingw64\bin` ve `C:\msys64\usr\bin` dizinlerinin PATH ortam değişkenine eklendiğinden otomatik olarak emin olur.
6. MinHook birleştirme bağımlılıklarını doğrular ve ilk derleme kontrolünü çalıştırır.

### Ana Derleme ve Sürüm Paketleme
Önceki derleme çıktılarını temizlemek, yerel kanca DLL'ini derlemek, tüm birim testlerini çalıştırmak ve sürüm paketini bir araya getirmek için:
```powershell
pwsh -File build.ps1
```
Çıktı dizini `dist/Helldivers1Babel-vX.Y.Z/` içinde paketlenecektir.

### Adım Adım Manuel Derleme
```powershell
# 1. Yerel kanca DLL'ini derleyin (babel_bootstrap.lua dosyasını luajit ile otomatik derler)
pwsh -File src/Babel.Hook/build.ps1

# 2. .NET 10 çözümünü derleyin
dotnet build Helldivers1Babel.slnx

# 3. Otomatik test paketlerini çalıştırın
dotnet test
```



## 5. Lisans

[MIT Lisansı](../LICENSE) altında dağıtılmaktadır.