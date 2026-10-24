# VRCDeskDive

[English](README.md) | **日本語**

VRChat を落とさずに、VR 操作とデスクトップ操作を切り替えるツールです。

VRChat は VR モードのまま、HMD を外したらキーボードとマウスで操作できるようにします。
移動やジャンプなどは VRChat の OSC 入力で、視点（上下左右）は SteamVR ドライバーで HMD の姿勢を書き換えて動かします。

このリポジトリは [gurenchang/VRCDeskDive](https://github.com/gurenchang/VRCDeskDive) の英語・日本語対応 [フォーク](https://github.com/defectly/VRCDeskDive)です。元のプロジェクトと MIT ライセンスの著作権表示は維持しています。

## 動作環境

- Windows 10 / 11（元のプロジェクトでは Windows 10 + Quest 3 + Steam Link で動作確認）
- 実行には [.NET 10 デスクトップランタイム](https://dotnet.microsoft.com/download/dotnet/10.0)
- SteamVR
- VRChat の OSC を有効にしておくこと（アクションメニュー → オプション → OSC）

HMD を外すとスリープしてしまう場合は、近接センサーをテープなどで塞いでください。

## ビルドと起動

ソースからビルドするには、ランタイムだけでなく [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) が必要です。
SteamVR ドライバー（C）を先にビルドしてから、アプリをビルドします。
ドライバーのビルドには C++ x64 ツールを含む Visual Studio / Visual Studio Build Tools、または [zig](https://ziglang.org/) が必要です。ビルドスクリプトは Visual Studio、`PATH` 上の `zig`、`tools\zig\zig.exe` の順に探します。

リポジトリのルートで、PowerShell から実行してください。

```powershell
powershell -ExecutionPolicy Bypass -File driver\build.ps1
dotnet build -c Release
.\bin\Release\net10.0-windows\VRCDeskDive.exe
```

初回は画面の「SteamVRに登録」を押してから SteamVR を再起動してください。

- 登録されるのはビルド先のフォルダ（`bin\Release\net10.0-windows\driver\vrcdeskdive`）です。フォルダを移動したら登録し直してください。
- アプリと `driver` サブフォルダは一緒に配置してください。`.exe` だけをコピーしてもドライバーは含まれません。
- SteamVR の起動中はドライバーの DLL が使用中になるので、ドライバーを作り直すときは SteamVR を終了してください。

## 表示言語

アプリは **English** と **日本語** に対応しています。設定の言語選択で切り替えられます。選択はすぐに保存され、再起動なしでメイン画面、接続状態、視点パッド、すでに開いているチャット入力画面にも反映されます。

設定は `%APPDATA%\VRCDeskDive\settings.json` の `Language` に `"en"` または `"ja"` として保存されます。未設定または非対応の値の場合、システム言語が日本語なら日本語、それ以外なら英語になります。既存の設定ファイルもそのまま使えます。

英語版・日本語版 README は同じ設定手順と操作を説明しています。このフォークでは日本語を維持し、保存される言語選択とアプリ全体の英語表示を追加しています。ビルドスクリプトのメッセージと診断ログは英語です。

## 使い方

メインウィンドウのボタンで「VR 操作」と「デスクトップ操作」を切り替えます。
HMD を着けたままデスクトップ操作に切り替えてから外すと、その時の視点の位置と向きのまま操作を続けられます。

- タイトルバーのボタンで、最前面に固定・縮小表示を切り替えられます（次回起動時も引き継がれます）。
- ウィンドウを閉じるとアプリが終了します。

### デスクトップ操作中の操作

キー操作とマウスルック開始は、VRChat かこのツールのウィンドウが前面にあるときだけ効きます。ツールの視点パッドからも直接視点を操作できます。

| 操作 | 動作 |
| --- | --- |
| W A S D | 移動 |
| 左クリック（VRChat の画面内） | マウスルック開始（マウスの動きがそのまま視点になる） |
| Alt（設定で Tab / Ctrl に変更可） | マウスルック解除（VRChat から離れても解除） |
| 右クリック＋ドラッグ（ツールのパッド上） | スティックのように視点を回す |
| ホイールクリック | 視点を水平に戻す |
| Shift | 走る |
| Space | ジャンプ |
| V | マイク |
| Enter | チャットボックスに入力（Enter で送信 / Esc でキャンセル） |

チャット入力中以外の Esc はそのまま VRChat に届くので、VRChat のメニューを開けます。
視点の感度は設定の「マウス感度」で調整できます。

## 仕組みと制限

```text
VRCDeskDive ── OSC (127.0.0.1:9000) ──────────────► VRChat        移動・ジャンプ・走る・マイク・チャット
     │
     └── 共有メモリ ──► SteamVR ドライバー（vrserver 内）  HMD の姿勢を書き換えて視点を動かす
```

- VRChat を別のデスクトップセッションとして起動するものではありません。VRChat は VR モードのままで、視点の書き換えには SteamVR が必要です。
- ドライバーは HMD の姿勢だけを書き換えます。プレイスペースには触れません。
- デスクトップ操作中は、切り替えた時の頭の位置と左右の向きを保ったまま視点を水平にします。HMD を机に置いても視点の高さは変わりません。
- VR 操作に戻すと書き換えをやめるので、本来の HMD の姿勢に戻ります。
- アプリが落ちた場合も、ドライバーは 1.5 秒で姿勢の書き換えをやめます。
- VRChat 本体の改造は行っていません。

## トラブルシューティング

- **アプリが起動しない場合:** .NET 10 **デスクトップ**ランタイムをインストールしてください。ビルドには .NET 10 SDK も必要です。
- **C コンパイラーが見つからない場合:** C++ x64 ツールを含む Visual Studio Build Tools をインストールするか、`zig.exe` を `PATH` 上または `tools\zig\zig.exe` に配置してください。アプリより先にドライバーをビルドします。
- **ドライバーがない・接続できない場合:** ビルドしたアプリに `driver\vrcdeskdive\bin\win64\driver_vrcdeskdive.dll` があることを確認し、アプリから登録した後に SteamVR を再起動してください。アプリのフォルダを移動した場合は再登録してください。HMD も接続したままスリープさせないでください。
- **ドライバーの DLL を上書きできない場合:** SteamVR を終了してからドライバーを再ビルドし、アプリも再ビルドして新しいドライバーを出力先へコピーしてください。
- **移動やチャットが反応しない場合:** VRChat の OSC を有効にし、VRChat またはツールを前面にしてください。既定の OSC 送信先は `127.0.0.1:9000` で、設定ファイルの `OscHost` と `OscPort` に保存されています。
- **マウスルックの開始・解除がうまくいかない場合:** クリックによるマウスルックの設定と解除キーを確認してください。VRChat 内を左クリックすると開始し、別のウィンドウへ移ると解除されます。
- **詳しく調べる場合:** `%APPDATA%\VRCDeskDive\debug.log` を確認してください。起動ごとに作り直されるので、再起動前にコピーしてください。

## ファイルの場所

| ファイル | 内容 |
| --- | --- |
| `%APPDATA%\VRCDeskDive\settings.json` | 言語の選択を含む設定 |
| `%APPDATA%\VRCDeskDive\debug.log` | 不具合調査用のログ（起動ごとに作り直す） |

## ドライバーの登録を外す

SteamVR を終了してから、PowerShell で次のコマンドを実行します。SteamVR のパスは環境に合わせ、`<build-output>` はアプリの出力先フォルダに置き換えてください。

```powershell
& "C:\Program Files (x86)\Steam\steamapps\common\SteamVR\bin\win64\vrpathreg.exe" removedriver "<build-output>\driver\vrcdeskdive"
```

## ライセンス

[MIT License](LICENSE)。元の著作権表示: © 2026 gurenchang。

## サードパーティ

- [OpenVR SDK](https://github.com/ValveSoftware/openvr)（BSD-3-Clause）: `ThirdParty/OpenVR/`
- [Rajdhani](https://fonts.google.com/specimen/Rajdhani)（SIL Open Font License 1.1）: `Fonts/`
