# ![Helldivers 1 Babel](screenshots/logo.png)

[![Latest Version](https://img.shields.io/github/v/release/difinim/helldivers1-babel?logo=github&label=release)](https://github.com/difinim/helldivers1-babel/releases/latest)
[![Build Status](https://github.com/difinim/helldivers1-babel/actions/workflows/release.yml/badge.svg)](https://github.com/difinim/helldivers1-babel/actions/workflows/release.yml)
[![Tests](https://github.com/difinim/helldivers1-babel/actions/workflows/tests.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/tests.yml?query=branch%3Amain)
[![CodeQL](https://github.com/difinim/helldivers1-babel/actions/workflows/codeql.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/codeql.yml?query=branch%3Amain)
[![OSV-Scanner](https://github.com/difinim/helldivers1-babel/actions/workflows/osv-scanner.yml/badge.svg?branch=main)](https://github.com/difinim/helldivers1-babel/actions/workflows/osv-scanner.yml?query=branch%3Amain)
[![Dependabot](https://img.shields.io/badge/dependabot-enabled-025E8C?logo=dependabot&logoColor=white)](https://github.com/difinim/helldivers1-babel/network/updates)

*共に降下せよ。言葉を一つに。*

**Helldivers 1 Babel** は、『*Helldivers (2015)*』向けのオープンソースかつ非侵襲（ゲームファイルを改変しない）チャット翻訳＆戦術通信アシスタントです。全員が作戦を理解していれば、甘美なる自由の響きはさらに素晴らしいものになります。

## Translations

[English](../README.md) | [العربية](README.ar.md) | [বাংলা](README.bn.md) | [简体中文](README.zh.md) | [Čeština](README.cs.md) | [Nederlands](README.nl.md) | [Français](README.fr.md) | [Deutsch](README.de.md) | [Ελληνικά](README.el.md) | [हिन्दी](README.hi.md) | [Magyar](README.hu.md) | [Bahasa Indonesia](README.id.md) | [Italiano](README.it.md) | [日本語](README.ja.md) | [한국어](README.ko.md) | [Bahasa Melayu](README.ms.md) | [فارسی](README.fa.md) | [Polski](README.pl.md) | [Português (PT-BR)](README.pt-BR.md) | [Română](README.ro.md) | [Русский](README.ru.md) | [Español](README.es.md) | [Svenska](README.sv.md) | [Tagalog (Filipino)](README.tl.md) | [ไทย](README.th.md) | [Türkçe](README.tr.md) | [Українська](README.uk.md) | [Tiếng Việt](README.vi.md)

> [!IMPORTANT]
> **互換性に関する免責事項:**
> **Helldivers 1 Babel は、PC（Steam版）の『Helldivers (2015)』専用に開発されています。**
> **Mod との併用が可能です。**
> **『Helldivers 2 (2024)』には対応していません。** 仕組みが完全に異なり、カーネルレベルのアンチチートが導入されているためです。

## 目次

- [1. 概要と主な機能](#1-概要と主な機能)
- [2. セットアップ](#2-セットアップ)
  - [動作要件](#動作要件)
  - [無料の AI API キーの取得](#無料の-ai-api-キーの取得)
  - [ダウンロードとインストール（ビルド済みリリース）](#ダウンロードとインストールビルド済みリリース)
- [3. 使い方ガイド](#3-使い方ガイド)
  - [はじめに](#はじめに)
  - [ゲーム内コミュニケーションと活用例](#ゲーム内コミュニケーションと活用例)
- [4. 開発（ソースからのビルド）](#4-開発ソースからのビルド)
  - [開発要件](#開発要件)
  - [自動環境セットアップ](#自動環境セットアップ)
  - [マスタービルドとリリースパッケージング](#マスタービルドとリリースパッケージング)
  - [手動ステップバイステップビルド](#手動ステップバイステップビルド)
- [5. ライセンス](#5-ライセンス)

## 1. 概要と主な機能

- **リアルタイム双方向チャット翻訳:**
  - **受信メッセージ:** チームメイトの言語を自動検出し、選択した翻訳先言語（日本語、英語、ポルトガル語（PT-BR）、スペイン語、ロシア語、中国語、フランス語、ドイツ語など最大28言語に対応）へ翻訳して、元のチームメイトのプレイヤー名とカラーコード（P1 ゴールド、P2 ブルー、P3 パープル、P4 シアン）を**忠実に保持**したまま、ゲーム内チャットに**ローカル描画**します。
  - **送信メッセージ:** `[target_language]>>メッセージ` または単に `>>メッセージ` と入力すると、直前に受信したチームメイトのメッセージの言語で自動返信します。翻訳されたテキストはマルチプレイヤーRPCを介してブロードキャストされ、他国のチームメイトは母国語で読むことができます。
- **永続チャットログとファイル出力:** タイムスタンプ、プレイヤー名、カラー、検出された言語、翻訳遅延を含むセッション全チャット履歴を記録します。軽量な非同期バッチディスク書き込み（容量設定可能）により、アプリを再起動してもログテーブルの内容が保持され、ワンクリックで `.txt` ファイルにエクスポートできます。
- **検索可能なチャットログ:** すべての列を対象にフィルタリングできる完全検索対応のチャット履歴。

![Helldivers 1 Babel の動作画面](screenshots/screenshot.png)

- **戦術クイックチャット (10個のカスタムスロット):** 10個のカスタマイズ可能な事前定義戦術メッセージスロット。ユーザー設定可能なショートカットキー（テンキー 0〜9 や自由なキーの組み合わせなど）に割り当て、ゲーム内マルチプレイヤーチャットへ瞬時に戦術コールアウトを送信できます。
- **自動ウェルカムクイックチャットメッセージ:** ロビーをホストしている際、ゲストプレイヤーが参加すると、カスタマイズ可能な歓迎メッセージをゲーム内チャットへ自動的にブロードキャストします。

![クイックチャット](screenshots/quick-chat.png)

- **画面上のクイックチャット参照オーバーレイ:** `Numpad +`（または設定したショートカット）を押すことで、画面右側にフォーカスを奪わないフローティング参照オーバーレイを表示・非表示し、有効な戦術コールアウトとショートカットをゲーム内で素早く確認できます。*（Helldivers をボーダーレスウィンドウまたはウィンドウモードで実行する必要があります）*。

![クイックチャットオーバーレイ](screenshots/overlay.png)

- **多言語 UI 対応:** 28言語に対応。
- **プロバイダーアダプターアーキテクチャ:** モジュール式の AI プロバイダーインターフェースで設計されています。**Google Gemini**（`gemini-flash-lite-latest` / `gemini-2.0-flash-lite`）に最適化された思考遅延ゼロのアダプターが付属し、OpenAI、Claude、DeepSeek 向けの拡張可能なプロバイダー契約も備えています（順次実装予定）。
- **翻訳のトーン設定:** カジュアル（カジュアルなゲームスラングや戦術ゲーマー用語を使用） / ノーマル（スラングを含まない自然な会話調）。
- **グローバル一時停止 / 再開ショートカット:** いつでも `Ctrl + F10`（または設定したショートカット）を押すことで、アシスタントの**稼働中**と**一時停止**を切り替えます。一時停止すると、ゲーム内チャットの翻訳とクイックチャットの送信の両方がきれいに保留されます。
- **個別機能ショートカット:**
  - **翻訳機能のみ切替:** `Ctrl + F11`（または設定したショートカット）を押して、クイックチャットに影響を与えることなくチャット翻訳のみを即座に一時停止/再開（画面上にトースト通知を表示）。
  - **クイックチャットのみ切替:** `Ctrl + F12`（または設定したショートカット）を押して、チャット翻訳に影響を与えることなくクイックチャットコールアウトのみを即座に一時停止/再開（画面上にトースト通知を表示）。

![設定](screenshots/settings.png)

- **安全な認証情報ストレージ (DPAPI):** AI プロバイダーの API トークンなどの機密情報は、Windows データ保護 API（DPAPI、`CurrentUser` スコープ）により暗号化されて保存されます。認証情報が設定ファイルに平文で保存されることは一切なく、お使いの Windows アカウントでのみローカルマシン上で復号できます。
- **テレメトリゼロ＆完全なプライバシー保護:** 本ツールは、ユーザーデータやテレメトリデータの収集、ログ記録、送信を一切行いません。すべての翻訳リクエストは、個人の API 認証情報を使用して、設定した AI プロバイダーのエンドポイントと直接通信します。
- **100% 非侵襲（ゲームファイルの改変ゼロ）:** ゲームアーカイブの展開や変更は一切行いません。ゲームファイルは 100% バニラのまま保たれます。

## 2. セットアップ

### 動作要件
ビルド済みのスタンドアロンリリース（`HD1-Babel.exe`）を実行する場合:
- **オペレーティングシステム:** Windows 10 / 11 64ビット (x64)。
- **デスクトップランタイム:** [Microsoft .NET 10 Desktop Runtime (x64)](https://aka.ms/dotnet/10.0/windowsdesktop-runtime-win-x64.exe) (直接インストーラー)。
- **対象ゲーム:** Steam版『*Helldivers*』(2015) (64ビット版 `helldivers.exe`)。

### 無料の AI API キーの取得
- **AI プロバイダー認証情報:** Google Gemini API キー ([Google AI Studio](https://aistudio.google.com/) から個人の無料キーを取得可能。[解説動画はこちら](https://www.youtube.com/watch?v=YHFMrZD-_3g))。
- **インターネット接続:** 翻訳 API 通信に必要。

### ダウンロードとインストール（ビルド済みリリース）

Babel を使用するために開発ツールのインストールやソースからのコンパイルを行う必要は**ありません**（[ソースからビルドしたい場合を除く](#4-開発ソースからのビルド)）:
1. GitHub プロジェクトページの右側にある **[Releases](https://github.com/difinim/helldivers1-babel/releases)** セクションにアクセスします。
2. 最新リリースの下にある **Assets** 一覧を展開します。
3. スタンドアロン配布アーカイブ **`Helldivers1Babel-v<version>-win-x64.zip`** をダウンロードします。
4. zip ファイルをコンピュータ内の任意の場所（例: `C:\GamesTools\Helldivers1Babel`）に解凍します。
5. フォルダを開き、**`HD1-Babel.exe`** を実行します。

> [!NOTE]
> **事前準備:** 一般ユーザーに必要なのは、無料の [Microsoft .NET 10 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/10.0) のみです。リリース版を実行するために C++ コンパイラ、SDK、ビルドツールチェーンは一切不要です。

> [!NOTE]
> **セキュリティソフトに関する注意:** InjectorService.cs は標準的な Windows プロセスインジェクション API（`VirtualAllocEx` + `WriteProcessMemory` + `CreateRemoteThread`）を使用しているため、Windows Defender やサードパーティ製ウイルス対策ソフトがヒューリスティック検知（例: Generic HackTool など）を行う**場合があります**。これはゲームの Mod インジェクターにおいて一般的な挙動であり、安全にホワイトリスト（除外設定）へ追加していただけます。

## 3. 使い方ガイド

### はじめに
1. **Helldivers 1 Babel** (`HD1-Babel.exe`) を起動します。
2. 初回起動時、**設定**タブを開きます:
   - **Google Gemini APIキー**を入力します（Windows DPAPI 暗号化によって安全に保存されます）。
   - 翻訳先言語を選択します（例: `日本語`）。
   - **API接続テスト**をクリックして、キーと遅延を確認します。
   - 必要に応じて**一時停止/再開キー**をカスタマイズします（デフォルトは `Ctrl + F10`）。
3. **クイックチャット**タブで戦術コールアウトを設定します（例: Numpad 0〜9 にお好みのメッセージを割り当て）。
4. Steam で **Helldivers** を起動します。Babel は `helldivers.exe` を自動検出し、`BabelHook.dll` をインジェクトして、高速な名前付きパイプ IPC を確立します。

### ゲーム内コミュニケーションと活用例

#### 1. 自動受信メッセージ翻訳
チームメイトからの受信メッセージは自動検出され、即座に翻訳先言語に翻訳されてゲーム内チャットに元のプレイヤー名とカラーで表示されます:
- チームメイト (英語): `so quiet`
- ゲーム内チャット: `Diveheller: (English): 静かすぎるな`

#### 2. 直前の受信言語への自動返信 (`>>メッセージ`)
チャットメッセージの先頭に `>>` を付けると、チームメイトから最後に受信したメッセージの言語にテキストが自動翻訳されます:
- チャットでの入力: `>>ああ、静かすぎる`
- チームメイトに見える表示: `yes, it is too quiet`

#### 3. 明示的・任意の言語指定 (`指定言語>>メッセージ`)
メッセージの先頭に言語名と `>>` を付けることで、いつでも特定の言語を指定して送信できます:

| チャット入力コマンド | 翻訳先言語 | チームメイトに送信される結果の例 |
| :--- | :--- | :--- |
| `russian>>bom dia!` | ロシア語 | `доброе утро!` |
| `english>>minas ao norte` | 英語 | `mines to the north` |
| `japanese>>cuidado com os tiros!` | 日本語 | `射線に注意せよ！` |
| `spanish>>preciso de munição` | スペイン語 | `necesito munición` |
| `german>>recuem para o ponto de extração` | ドイツ語 | `Rückzug zum Extraktionspunkt` |
| `french>>vamos defender aqui` | フランス語 | `défendons cette position` |

#### 4. 通常の送信メッセージと自己翻訳の抑止
`>>` を付けずに通常のゲーム内チャットメッセージ（例: `need ammo`, `vamos lá` など）を入力した場合、メッセージは通常のマルチプレイヤーチャット経由でそのまま送信されます。**「通常の送信メッセージを翻訳しない」**トグルが有効になっている場合（クイックコントロールバーおよび設定の両方でデフォルト有効）、Babel は自身のメッセージを自分宛てに翻訳して再表示するのを抑止します。オフにした場合、Babel はローカルの通常メッセージも受信翻訳パイプラインで処理します。

#### 5. 戦術クイックチャットショートカット
プレイ中に設定済みのクイックチャットキー（例: `NumPad 1`〜`NumPad 9`）を押すと、チャット入力画面を開くことなくプリセットのコールアウトをマルチプレイヤーチャットに即座に送信できます。すべてのキー入力はゲーム側にもシームレスに伝達されるため、操作性が損なわれることはありません。

#### 6. 戦術クイックチャット参照オーバーレイ (`Numpad +`)
いつでも `Numpad +`（または**クイックチャット**タブで登録したショートカット）を押すと、有効なすべてのクイックチャットプリセットを一覧表示する画面上の参照オーバーレイを切り替えられます。オーバーレイは画面右側の中央に表示され、各有効プリセットが `[<ショートカット>] <メッセージ>` の形式（メッセージは最大30文字に省略）で、トースト通知と同様の邪魔にならないスタイルで描画されます。
> [!NOTE]
> **表示モード要件:** Windows オーバーレイウィンドウをゲーム上に描画するには、ゲーム内のビデオ設定で Helldivers を**ボーダーレスウィンドウ**（またはウィンドウモード）で実行するように設定する必要があります。

#### 7. 一時停止・再開と個別機能の切替
- **グローバル一時停止 / 再開 (`Ctrl + F10`):** 翻訳のインターセプトとクイックチャットの送信の両方をきれいに保留/再開するマスタースイッチ。
- **翻訳機能のみ切替 (`Ctrl + F11`):** クイックチャットを正常に維持したまま、画面上のトースト通知を伴ってチャット翻訳機能のみをオン/オフします。
- **クイックチャットのみ切替 (`Ctrl + F12`):** チャット翻訳を正常に維持したまま、画面上のトースト通知を伴ってクイックチャットの送信のみをオン/オフします。
*(すべてのショートカットは、**設定**および**クイックチャット**タブの記録ボタンをクリックしてカスタマイズできます)。*

#### 8. チャットログの出力と永続保持
「翻訳とログ」タブの**ログ出力**をいつでもクリックして、保存先を選択し、チャット履歴（タイムスタンプ、プレイヤー名、元のメッセージ、翻訳、遅延を含む）を `.txt` ドキュメントにエクスポートできます。チャットメッセージは非同期バッチディスク書き込みにより、アプリを再起動しても自動的にテーブルに保持されます（保存容量の上限は設定で 0〜100 MB の範囲で設定可能。**ログメモリ消去**ボタンも備えています）。

#### 9. インタラクティブなメッセージ内リンク
`http://` または `https://` の URL を含む分隊チャットメッセージは、**元のメッセージ**列にクリック可能なリンクとして表示されます。クリックすると、信頼できるリンクであるかを確認するダイアログが表示され、確認後に既定のウェブブラウザで開かれます。

## 4. 開発（ソースからのビルド）

### 開発要件
ソースからプロジェクトをコンパイルおよびビルドする場合:
- **オペレーティングシステム:** Windows 10 / 11 64ビット (x64)。
- **.NET 10 SDK:** [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (`net10.0-windows`)。
- **MSYS2 MinGW64 ツールチェーン (Clang++, GCC, Make):** [MSYS2](https://www.msys2.org/)。`mingw-w64-x86_64-clang`、`mingw-w64-x86_64-gcc`、`mingw-w64-x86_64-make` パッケージが必要 — ネイティブフックライブラリ (`BabelHook.dll`) のコンパイル、およびゲーム内ブートストラップスクリプト (`babel_bootstrap.lua`) をバイトコードにコンパイルするための公式 LuaJIT 2.0 サブモジュール (`vendor/luajit`) のビルドに使用します。
- **Git:** Git for Windows (MinHook アマルガメーションおよび LuaJIT 2.0 用の再帰的サブモジュールを含む)。

### 自動環境セットアップ
リポジトリのルートにある `setup.bat` をダブルクリック（または PowerShell で `setup.ps1` を実行）します。不足している前提条件を自動的に検証してインストールします:
1. 64ビット Windows 環境を確認。
2. .NET 10 SDK を確認（不足している場合は `winget` 経由で自動インストール）。
3. MSYS2 MinGW64 ツールチェーン（Clang++、GCC、Make）を確認。不足している場合は `winget` 経由で MSYS2 を自動インストールし、`pacman` で必要なパッケージ（`mingw-w64-x86_64-clang`、`mingw-w64-x86_64-gcc`、`mingw-w64-x86_64-make`）をインストール。
4. サブモジュール (`vendor/luajit`) から LuaJIT 2.0 コンパイラを初期化してビルド。
5. `C:\msys64\mingw64\bin` および `C:\msys64\usr\bin` が PATH に追加されていることを自動的に確認。
6. MinHook アマルガメーションの依存関係を検証し、初期ビルドの正常性チェックを実行。

### マスタービルドとリリースパッケージング
以前の成果物をクリーンアップし、ネイティブフック DLL をコンパイルし、すべての単体テストを実行して、リリースパッケージを組み立てる場合:
```powershell
pwsh -File build.ps1
```
出力ディレクトリは `dist/Helldivers1Babel-vX.Y.Z/` にパッケージ化されます。

### 手動ステップバイステップビルド
```powershell
# 1. ネイティブフック DLL をビルド (luajit を使って babel_bootstrap.lua を自動コンパイル)
pwsh -File src/Babel.Hook/build.ps1

# 2. .NET 10 ソリューションをビルド
dotnet build Helldivers1Babel.slnx

# 3. 自動テストスイートを実行
dotnet test
```

## 5. ライセンス

[MIT ライセンス](../LICENSE)のもとで配布されています。
