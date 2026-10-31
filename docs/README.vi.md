# ![Helldivers 1 Babel](screenshots/logo.png)

[![Latest Version](https://img.shields.io/github/v/release/difinim/helldivers1-babel?logo=github&label=release)](https://github.com/difinim/helldivers1-babel/releases/latest)
[![Build Status](https://github.com/difinim/helldivers1-babel/actions/workflows/release.yml/badge.svg)](https://github.com/difinim/helldivers1-babel/actions/workflows/release.yml)
[![Tests](https://github.com/difinim/helldivers1-babel/actions/workflows/tests.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/tests.yml?query=branch%3Amain)
[![CodeQL](https://github.com/difinim/helldivers1-babel/actions/workflows/codeql.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/codeql.yml?query=branch%3Amain)
[![OSV-Scanner](https://github.com/difinim/helldivers1-babel/actions/workflows/osv-scanner.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/osv-scanner.yml?query=branch%3Amain)
[![Dependabot](https://img.shields.io/badge/dependabot-enabled-025E8C?logo=dependabot&logoColor=white)](https://github.com/difinim/helldivers1-babel/network/updates)

*Cùng đổ bộ. Thống nhất tiếng nói.*

**Helldivers 1 Babel** là trợ lý dịch thuật trò chuyện và giao tiếp chiến thuật mã nguồn mở, không can thiệp sâu (non-invasive) dành cho *Helldivers (2015)*, bởi vì *tự do đích thực sẽ tuyệt vời hơn khi tất cả mọi người đều hiểu rõ kế hoạch tác chiến*.

## Translations

[English](../README.md) | [العربية](README.ar.md) | [বাংলা](README.bn.md) | [简体中文](README.zh.md) | [Čeština](README.cs.md) | [Nederlands](README.nl.md) | [Français](README.fr.md) | [Deutsch](README.de.md) | [Ελληνικά](README.el.md) | [हिन्दी](README.hi.md) | [Magyar](README.hu.md) | [Bahasa Indonesia](README.id.md) | [Italiano](README.it.md) | [日本語](README.ja.md) | [한국어](README.ko.md) | [Bahasa Melayu](README.ms.md) | [فارسی](README.fa.md) | [Polski](README.pl.md) | [Português (PT-BR)](README.pt-BR.md) | [Română](README.ro.md) | [Русский](README.ru.md) | [Español](README.es.md) | [Svenska](README.sv.md) | [Tagalog (Filipino)](README.tl.md) | [ไทย](README.th.md) | [Türkçe](README.tr.md) | [Українська](README.uk.md) | [Tiếng Việt](README.vi.md)

> [!IMPORTANT]
> **Tuyên bố miễn trừ về tính tương thích:**
> **Helldivers 1 Babel được phát triển nghiêm ngặt dành riêng cho Helldivers (2015) trên PC (Steam).**
> **Tương thích hoàn toàn với các bản mod.**
> **KHÔNG tương thích với Helldivers 2 (2024)** vì trò chơi này hoàn toàn khác biệt và sử dụng hệ thống chống gian lận cấp nhân (kernel-level anti-cheat).

## Mục lục

- [1. Tổng quan & Các tính năng chính](#1-tổng-quan--các-tính-năng-chính)
- [2. Cài đặt & Thiết lập](#2-cài-đặt--thiết-lập)
  - [Yêu cầu hệ thống](#yêu-cầu-hệ-thống)
  - [Thiết lập khóa API AI miễn phí](#thiết-lập-khóa-api-ai-miễn-phí)
  - [Tải về & Cài đặt (Bản phát hành dựng sẵn)](#tải-về--cài-đặt-bản-phát-hành-dựng-sẵn)
- [3. Hướng dẫn sử dụng](#3-hướng-dẫn-sử-dụng)
  - [Bắt đầu](#bắt-đầu)
  - [Giao tiếp trong game & Trường hợp sử dụng](#giao-tiếp-trong-game--trường-hợp-sử-dụng)
- [4. Phát triển (Dựng từ mã nguồn)](#4-phát-triển-dựng-từ-mã-nguồn)
  - [Yêu cầu đối với nhà phát triển](#yêu-cầu-đối-với-nhà-phát-triển)
  - [Thiết lập môi trường tự động](#thiết-lập-môi-trường-tự-động)
  - [Dựng bản Master & Đóng gói phát hành](#dựng-bản-master--đóng-gói-phát-hành)
  - [Dựng thủ công từng bước](#dựng-thủ-công-từng-bước)
- [5. Giấy phép](#5-giấy-phép)

## 1. Tổng quan & Các tính năng chính

- **Dịch thuật trò chuyện hai chiều theo thời gian thực:**
  - **Dịch tin nhắn đến:** Tự động phát hiện ngôn ngữ của đồng đội, dịch tin nhắn sang ngôn ngữ đích bạn đã chọn (hỗ trợ tới 28 ngôn ngữ, bao gồm Tiếng Việt, Tiếng Anh, Tiếng Bồ Đào Nha PT-BR, Tiếng Tây Ban Nha, Tiếng Nga, Tiếng Trung, Tiếng Nhật, Tiếng Pháp, Tiếng Đức,...), và kết xuất nội bộ vào khung chat trong trò chơi trong khi **giữ nguyên vẹn tên người chơi và mã màu gốc của đồng đội** (P1 Vàng, P2 Xanh lam, P3 Tím, P4 Xanh lơ).
  - **Dịch tin nhắn đi:** Gõ `[ngôn_ngữ_đích]>>tin_nhắn` hoặc đơn giản là `>>tin_nhắn` để tự động trả lời bằng ngôn ngữ của tin nhắn gần nhất từ đồng đội. Văn bản đã dịch sẽ được phát qua RPC nhiều người chơi để các đồng đội quốc tế đọc trực tiếp bằng ngôn ngữ của họ.
- **Nhật ký trò chuyện bền vững & Xuất tệp:** Theo dõi toàn bộ lịch sử chat trong phiên chơi với dấu thời gian, tên người chơi, màu sắc, ngôn ngữ nguồn được phát hiện và độ trễ dịch thuật. Nhật ký được duy trì trong bảng qua các lần khởi động lại ứng dụng nhờ cơ chế ghi đĩa theo lô bất đồng bộ nhẹ nhàng (có thể điều chỉnh dung lượng), cùng tính năng xuất ra tệp `.txt` chỉ với một cú nhấp chuột.
- **Nhật ký trò chuyện có thể tìm kiếm:** Lịch sử chat có thể tìm kiếm đầy đủ và lọc theo bất kỳ cột nào.

![Helldivers 1 Babel đang hoạt động](screenshots/screenshot.png)

- **Chat nhanh chiến thuật (10 ô tùy chỉnh):** 10 ô tin nhắn chiến thuật được định sẵn và có thể tùy chỉnh, mỗi ô được gắn với một phím tắt do người dùng cấu hình (ví dụ: Numpad 0-9 hoặc các tổ hợp phím tùy ý) để truyền ngay các khẩu lệnh chiến thuật vào chat nhiều người chơi.

![Chat nhanh](screenshots/quick-chat.png)

- **Tin nhắn Chat nhanh chào mừng tự động:** Tự động gửi một tin nhắn chào mừng tùy chỉnh vào chat trong game khi bạn làm chủ phòng và có người chơi mới tham gia.
- **Lớp phủ tham chiếu Chat nhanh trên màn hình:** Bật/tắt lớp phủ nổi không chiếm tiêu điểm ở bên phải màn hình bằng phím `Numpad +` (hoặc phím tắt đã lưu của bạn) để nhanh chóng xem lại tất cả các khẩu lệnh chiến thuật và phím tắt đang hoạt động khi đang chơi. *(Yêu cầu Helldivers chạy ở chế độ Không viền - Borderless Fullscreen hoặc Cửa sổ - Windowed)*.

![Lớp phủ Chat nhanh](screenshots/overlay.png)

- **Hỗ trợ giao diện đa ngôn ngữ:** Hỗ trợ 28 ngôn ngữ.
- **Kiến trúc bộ điều hợp nhà cung cấp:** Được xây dựng với giao diện nhà cung cấp AI dạng mô-đun. Đi kèm với bộ điều hợp tối ưu hóa độ trễ suy nghĩ dành cho **Google Gemini** (`gemini-flash-lite-latest` / `gemini-2.0-flash-lite`), cùng hợp đồng mở rộng sẵn sàng cho OpenAI, Claude và DeepSeek (sắp ra mắt).
- **Cài đặt giọng điệu dịch thuật:** Thoải mái (tiếng lóng game thủ và thuật ngữ chiến thuật) / Thông thường (giọng điệu trò chuyện tự nhiên, không dùng tiếng lóng).
- **Phím tắt Tạm dừng / Tiếp tục toàn cục:** Nhấn `Ctrl + F10` (hoặc phím tắt tùy chỉnh) bất cứ lúc nào để chuyển đổi trợ lý giữa trạng thái **Hoạt động** và **Tạm dừng**. Khi tạm dừng, cả việc dịch chat và gửi tin nhắn Chat nhanh đều được hoãn lại một cách an toàn.
- **Phím tắt chuyên dụng cho từng tính năng:**
  - **Bật/tắt riêng dịch thuật:** Nhấn `Ctrl + F11` (hoặc phím tắt đã lưu) để tạm dừng hoặc tiếp tục riêng tính năng dịch chat mà không ảnh hưởng đến Chat nhanh (có thông báo nổi trên màn hình).
  - **Bật/tắt riêng Chat nhanh:** Nhấn `Ctrl + F12` (hoặc phím tắt đã lưu) để tạm dừng hoặc tiếp tục riêng tính năng Chat nhanh mà không ảnh hưởng đến dịch thuật (có thông báo nổi trên màn hình).

![Cài đặt](screenshots/settings.png)

- **Lưu trữ thông tin xác thực an toàn (DPAPI):** Các tùy chọn nhạy cảm như token API AI được mã hóa an toàn trên đĩa bằng Windows Data Protection API (DPAPI, phạm vi `CurrentUser`). Thông tin xác thực không bao giờ được lưu dưới dạng văn bản thuần trong tệp cấu hình và chỉ có thể được giải mã trên máy tính của bạn bởi tài khoản Windows của bạn.
- **Không thu thập dữ liệu đo từ xa & Hoàn toàn riêng tư:** Công cụ này không thu thập, ghi lại hoặc gửi bất kỳ dữ liệu người dùng hay dữ liệu đo từ xa nào. Mọi yêu cầu dịch thuật đều giao tiếp trực tiếp và duy nhất với điểm cuối nhà cung cấp AI mà bạn đã cấu hình bằng khóa API cá nhân của bạn.
- **100% Không can thiệp sâu (Không sửa đổi tệp trò chơi):** Không có kho lưu trữ game nào bị giải nén hoặc sửa đổi. Các tệp trò chơi của bạn được giữ nguyên bản 100%.

## 2. Cài đặt & Thiết lập

### Yêu cầu hệ thống
Nếu bạn đang chạy bản phát hành độc lập dựng sẵn (`HD1-Babel.exe`):
- **Hệ điều hành:** Windows 10 / 11 64-bit (x64).
- **Thời gian chạy Desktop:** [Microsoft .NET 10 Desktop Runtime (x64)](https://aka.ms/dotnet/10.0/windowsdesktop-runtime-win-x64.exe) (trình cài đặt trực tiếp).
- **Trò chơi mục tiêu:** *Helldivers* (2015) trên Steam (bản 64-bit `helldivers.exe`).

### Thiết lập khóa API AI miễn phí
- **Thông tin xác thực nhà cung cấp AI:** Khóa API Google Gemini (lấy khóa cá nhân miễn phí từ [Google AI Studio](https://aistudio.google.com/) ([Xem video hướng dẫn](https://www.youtube.com/watch?v=YHFMrZD-_3g))).
- **Kết nối Internet:** Cần thiết cho các cuộc gọi API dịch thuật.

### Tải về & Cài đặt (Bản phát hành dựng sẵn)

Bạn **không** cần cài đặt công cụ phát triển hay biên dịch từ mã nguồn để sử dụng Babel ([trừ khi bạn muốn tự dựng](#4-phát-triển-dựng-từ-mã-nguồn)):
1. Điều hướng đến mục **[Releases](https://github.com/difinim/helldivers1-babel/releases)** ở bên phải trang dự án GitHub.
2. Dưới bản phát hành mới nhất, mở rộng danh sách **Assets**.
3. Tải về kho lưu trữ phân phối độc lập: **`Helldivers1Babel-v<version>-win-x64.zip`**.
4. Giải nén tệp zip vào bất kỳ đâu trên máy tính của bạn (ví dụ: `C:\GamesTools\Helldivers1Babel`).
5. Mở thư mục và chạy **`HD1-Babel.exe`**.

> [!NOTE]
> **Điều kiện tiên quyết:** Người dùng cuối chỉ cần cài đặt miễn phí [Microsoft .NET 10 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/10.0). Không yêu cầu trình biên dịch C++, SDK hay chuỗi công cụ nào để chạy bản phát hành.

> [!NOTE]
> **Lưu ý phần mềm diệt virus:** Vì InjectorService.cs sử dụng các API tiêm tiến trình Windows tiêu chuẩn (`VirtualAllocEx` + `WriteProcessMemory` + `CreateRemoteThread`), Windows Defender hoặc phần mềm diệt virus bên thứ ba _**có thể**_ kích hoạt cảnh báo phỏng đoán (ví dụ: Generic HackTool). Đây là hành vi bình thường đối với các công cụ tiêm mod trò chơi và có thể an toàn thêm vào danh sách loại trừ (whitelist).

## 3. Hướng dẫn sử dụng

### Bắt đầu
1. Khởi chạy **Helldivers 1 Babel** (`HD1-Babel.exe`).
2. Trong lần khởi chạy đầu tiên, mở tab **Cài đặt**:
   - Nhập **Khóa API Google Gemini** của bạn (được lưu trữ an toàn qua mã hóa Windows DPAPI).
   - Chọn ngôn ngữ đích của bạn (ví dụ: `Tiếng Việt`).
   - Nhấn **Kiểm tra kết nối API** để xác minh khóa và độ trễ.
   - Tùy chỉnh phím tắt **Tạm dừng / Tiếp tục** nếu muốn (mặc định là `Ctrl + F10`).
3. Cấu hình các khẩu lệnh chiến thuật của bạn trong tab **Chat nhanh** (ví dụ: gán Numpad 0-9 cho các tin nhắn bạn muốn).
4. Khởi chạy **Helldivers** trên Steam. Babel sẽ tự động phát hiện `helldivers.exe`, tiêm `BabelHook.dll` và thiết lập kết nối IPC qua named pipe tốc độ cao.

### Giao tiếp trong game & Trường hợp sử dụng

#### 1. Tự động dịch tin nhắn đến
Tin nhắn đến từ đồng đội sẽ được phát hiện và dịch sang ngôn ngữ đích của bạn ngay lập tức, hiển thị trong khung chat của game với tên người chơi và màu sắc ban đầu:
- Đồng đội (Tiếng Anh): `so quiet`
- Khung chat trong game: `Diveheller: (English): yên ắng quá`

#### 2. Tự động trả lời theo ngôn ngữ tin nhắn đến gần nhất (`>>tin_nhắn`)
Thêm tiền tố `>>` vào trước tin nhắn chat của bạn sẽ tự động dịch văn bản của bạn sang ngôn ngữ của tin nhắn gần nhất nhận được từ đồng đội:
- Bạn gõ trong chat: `>>đúng vậy, bình yên trước cơn bão`
- Đồng đội nhìn thấy: `yes, it is too quiet`

#### 3. Chọn ngôn ngữ đích rõ ràng / tùy ý (`ngôn ngữ đích>>tin_nhắn`)
Gửi tin nhắn của bạn đến một ngôn ngữ cụ thể bất kỳ lúc nào bằng cách thêm tiền tố tên ngôn ngữ theo sau là `>>`:

| Lệnh nhập trong Chat | Ngôn ngữ đích | Ví dụ kết quả phát đến đồng đội |
| :--- | :--- | :--- |
| `russian>>bom dia!` | Tiếng Nga | `доброе утро!` |
| `english>>minas ao norte` | Tiếng Anh | `mines to the north` |
| `japanese>>cuidado com os tiros!` | Tiếng Nhật | `射線に注意せよ！` |
| `spanish>>preciso de munição` | Tiếng Tây Ban Nha | `necesito munición` |
| `german>>recuem para o ponto de extração` | Tiếng Đức | `Rückzug zum Extraktionspunkt` |
| `french>>vamos defender aqui` | Tiếng Pháp | `défendons cette position` |

#### 4. Tin nhắn đi thông thường & Triệt tiêu tự dịch
Khi gõ tin nhắn trò chuyện thông thường trong game không có `>>` (ví dụ: `need ammo`, `vamos lá`), tin nhắn của bạn sẽ được gửi trực tiếp qua kênh chat nhiều người chơi nguyên bản. Khi tùy chọn **"Không dịch tin nhắn đi thông thường"** được bật (mặc định BẬT ở cả thanh điều khiển nhanh và Cài đặt), Babel sẽ triệt tiêu việc dịch ngược tin nhắn của chính bạn về cho bạn. Nếu tắt tùy chọn này, Babel cũng sẽ xử lý các tin nhắn thông thường cục bộ qua quy trình dịch tin nhắn đến.

#### 5. Phím tắt Chat nhanh chiến thuật
Nhấn bất kỳ phím tắt chat nhanh nào đã cấu hình (ví dụ: `NumPad 1`-`NumPad 9`) trong khi chơi để phát ngay lập tức các khẩu lệnh cài sẵn vào chat nhiều người chơi mà không cần mở hộp thoại chat. Tất cả các lần nhấn phím đều được chuyển tiếp liền mạch tới game, đảm bảo trải nghiệm điều khiển của bạn luôn phản hồi tức thì.

#### 6. Lớp phủ tham chiếu Chat nhanh trên màn hình (`Numpad +`)
Nhấn `Numpad +` (hoặc phím tắt đã ghi của bạn được cấu hình trong tab **Chat nhanh**) bất kỳ lúc nào để bật/tắt lớp phủ tham chiếu trên màn hình hiển thị danh sách tất cả các khẩu lệnh Chat nhanh đang hoạt động. Lớp phủ xuất hiện căn phải và căn giữa theo chiều dọc màn hình, hiển thị từng cài đặt đang hoạt động theo định dạng `[<phím tắt>] <tin nhắn>` (tin nhắn được rút gọn tối đa 30 ký tự) bằng phong cách hiển thị nhẹ nhàng, không gây cản trở tương tự như thông báo nổi.
> [!NOTE]
> **Yêu cầu chế độ hiển thị:** Helldivers phải được cấu hình để chạy ở chế độ **Không viền (Borderless Fullscreen)** hoặc Cửa sổ (Windowed) trong cài đặt Video của game để các cửa sổ lớp phủ Windows có thể vẽ lên trên trò chơi.

#### 7. Tạm dừng, Tiếp tục & Bật/tắt tính năng
- **Tạm dừng / Tiếp tục toàn cục (`Ctrl + F10`):** Công tắc chính giúp tạm dừng hoặc tiếp tục cả việc chặn dịch thuật và phát tin nhắn Chat nhanh một cách triệt để.
- **Bật/tắt riêng dịch thuật (`Ctrl + F11`):** Chỉ bật hoặc tắt tính năng dịch chat với thông báo nổi trên màn hình. Chat nhanh vẫn tiếp tục hoạt động bình thường.
- **Bật/tắt riêng Chat nhanh (`Ctrl + F12`):** Chỉ bật hoặc tắt tính năng phát khẩu lệnh Chat nhanh với thông báo nổi trên màn hình. Tính năng dịch chat trong game vẫn tiếp tục hoạt động bình thường.
*(Tất cả phím tắt có thể được tùy chỉnh trong tab **Cài đặt** và **Chat nhanh** bằng các nút ghi phím tắt nhấp chuột).*

#### 8. Xuất & Duy trì nhật ký trò chuyện
Nhấp vào **Xuất nhật ký** trong tab Dịch thuật & Nhật ký bất cứ lúc nào để chọn vị trí và xuất lịch sử trò chuyện (kèm dấu thời gian, tên người chơi, tin nhắn gốc, bản dịch và độ trễ) ra tệp văn bản `.txt`. Các tin nhắn trò chuyện tự động được lưu trữ bền vững trong bảng qua các lần khởi chạy ứng dụng nhờ cơ chế ghi đĩa theo lô bất đồng bộ (giới hạn lưu trữ có thể định cấu hình từ 0 đến 100 MB trong Cài đặt, đi kèm nút **Xóa bộ nhớ nhật ký**).

#### 9. Liên kết tương tác trong tin nhắn
Các tin nhắn trò chuyện trong đội chứa liên kết URL `http://` hoặc `https://` sẽ được hiển thị dưới dạng liên kết có thể nhấp chuột trong cột **Tin nhắn gốc**. Khi nhấp vào, hộp thoại xác nhận sẽ hỏi bạn có tin tưởng liên kết này hay không trước khi mở trong trình duyệt web mặc định.

## 4. Phát triển (Dựng từ mã nguồn)

### Yêu cầu đối với nhà phát triển
Nếu bạn biên dịch và dựng dự án từ mã nguồn:
- **Hệ điều hành:** Windows 10 / 11 64-bit (x64).
- **.NET 10 SDK:** [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (`net10.0-windows`).
- **Chuỗi công cụ MSYS2 MinGW64 (Clang++, GCC, Make):** [MSYS2](https://www.msys2.org/) với các gói `mingw-w64-x86_64-clang`, `mingw-w64-x86_64-gcc`, và `mingw-w64-x86_64-make` - cần thiết để biên dịch thư viện hook gốc (`BabelHook.dll`) và dựng submodule LuaJIT 2.0 chính thức (`vendor/luajit`) nhằm biên dịch tập lệnh bootstrap trong game (`babel_bootstrap.lua`) thành bytecode.
- **Git:** Git for Windows (với các submodule đệ quy cho MinHook amalgamation và LuaJIT 2.0).

### Thiết lập môi trường tự động
Nhấp đúp vào `setup.bat` (hoặc chạy `setup.ps1` trong PowerShell) ở thư mục gốc của kho lưu trữ. Tập lệnh sẽ tự động kiểm tra và cài đặt các điều kiện tiên quyết còn thiếu:
1. Kiểm tra môi trường Windows 64-bit.
2. Kiểm tra .NET 10 SDK (tự động cài đặt qua `winget` nếu thiếu).
3. Kiểm tra chuỗi công cụ MSYS2 MinGW64 (Clang++, GCC và Make). Nếu thiếu, tự động cài đặt MSYS2 qua `winget` và cài đặt các gói bắt buộc (`mingw-w64-x86_64-clang`, `mingw-w64-x86_64-gcc` và `mingw-w64-x86_64-make`) qua `pacman`.
4. Khởi tạo và dựng trình biên dịch LuaJIT 2.0 từ submodule (`vendor/luajit`).
5. Tự động đảm bảo `C:\msys64\mingw64\bin` và `C:\msys64\usr\bin` được thêm vào PATH.
6. Kiểm tra tính toàn vẹn của MinHook amalgamation và chạy kiểm tra tính hợp lệ của bản dựng ban đầu.

### Dựng bản Master & Đóng gói phát hành
Để dọn dẹp các tệp bản dựng cũ, biên dịch DLL hook gốc, chạy tất cả các bài kiểm thử đơn vị và đóng gói bản phát hành:
```powershell
pwsh -File build.ps1
```
Thư mục đầu ra sẽ được đóng gói vào `dist/Helldivers1Babel-vX.Y.Z/`.

### Dựng thủ công từng bước
```powershell
# 1. Dựng DLL hook gốc (tự động biên dịch babel_bootstrap.lua bằng luajit)
pwsh -File src/Babel.Hook/build.ps1

# 2. Dựng giải pháp .NET 10
dotnet build Helldivers1Babel.slnx

# 3. Chạy bộ kiểm thử tự động
dotnet test
```

## 5. Giấy phép

Được phân phối theo [Giấy phép MIT](../LICENSE).
