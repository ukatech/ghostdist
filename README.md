# GhostDist - ゴースト配布系自動化システム改

Delphi 6版からC#/.NET Framework 4.8に移行したバージョンです。

[ダウンロードはこちらから](https://github.com/ukatech/ghostdist/releases)

## 概要

伺かゴーストのネットワーク更新ファイル生成・FTPアップロード・NAR(ZIP)圧縮を自動化するツールです。

## 主要機能

1. **ネットワーク更新**: updates2.dauファイルを生成し、変更されたファイルのみをFTPアップロード
2. **NAR作成+アップロード**: NAR(ZIP)ファイルを作成してFTPアップロード、配布ページHTML更新
3. **NAR作成のみ**: NAR(ZIP)ファイルを作成

## 動作環境

- Windows 7 以降
- .NET Framework 4.8

## 技術スタック

- **.NET Framework**: 4.8
- **UI**: Windows Forms
- **NuGetパッケージ**: 下記「使用ライブラリ」を参照

## ビルド方法

### 前提条件

- Visual Studio 2019以降 (または MSBuild)
- .NET Framework 4.8 SDK
- NuGet CLI

### 手順

#### コマンドラインを使用する場合

```bash
cd D:\ssp_src_set\ghostdist\src\dotnet

# NuGetパッケージ復元
nuget restore GhostDist.sln

# ビルド
msbuild GhostDist.sln /p:Configuration=Release
```

#### Visual Studioを使用する場合

1. `GhostDist.sln`を開く
2. ソリューションエクスプローラーでソリューションを右クリック → `NuGetパッケージの復元`
3. `ビルド` > `ソリューションのビルド` (Ctrl+Shift+B)

### ビルド出力

- Debug版: `GhostDist\bin\Debug\GhostDist.exe`
- Release版: `GhostDist\bin\Release\GhostDist.exe`

Costura.Fodyにより依存DLLはEXEに埋め込まれるため、**単一ファイルで配布可能**です。

## 設定ファイル

- `ghostdist.ini` - プロジェクト設定（EXEと同じフォルダ、Delphi版と完全互換）
- `user.config` - ウィンドウ位置・サイズ（%LocalAppData%以下に自動保存）

### 文字エンコーディング

- INI設定ファイル: Shift_JIS
- updates2.dau: Shift_JIS (ファイル名がSJISで対応できない場合は自動でUTF-8に切り替え)
- NAR(ZIP)内のファイル名: Shift_JIS (Info-Zip Unicode File Name拡張を必要な場合のみ追加)

すべてDelphi版と同じエンコーディングで処理されます。

## プロジェクト構造

```
dotnet/
├── GhostDist.sln               # ソリューションファイル
├── README.md                   # このファイル
├── LICENSE                     # MITライセンス
└── GhostDist/
    ├── GhostDist.csproj        # プロジェクトファイル
    ├── packages.config         # NuGetパッケージ設定
    ├── FodyWeavers.xml         # Costura.Fody設定
    ├── Program.cs              # エントリポイント
    ├── Models/                 # データモデル
    ├── Services/               # ビジネスロジック
    ├── Utilities/              # ユーティリティ
    ├── Forms/                  # UIフォーム
    └── Properties/             # プロジェクトプロパティ
```

## 使用ライブラリ

本ソフトウェアは以下のオープンソースライブラリを使用しています。
すべてMITライセンスです。

| ライブラリ | バージョン | ライセンス | 用途 |
|-----------|-----------|-----------|------|
| [FluentFTP](https://github.com/robinrodricks/FluentFTP) | 53.0.2 | MIT | FTP/FTPS通信 |
| [SharpZipLib](https://github.com/icsharpcode/SharpZipLib) | 1.3.3 | MIT | ZIP圧縮（NAR作成） |
| [Fody](https://github.com/Fody/Fody) | 6.9.3 | MIT | ビルド時アセンブリ処理 |
| [Costura.Fody](https://github.com/Fody/Costura) | 6.0.0 | MIT | DLL埋め込み（単一EXE化） |

## ライセンス

本ソフトウェアはMITライセンスの下で公開されています。
詳細は[LICENSE](LICENSE)ファイルを参照してください。

### サードパーティライセンス

#### FluentFTP
```
MIT License
Copyright (c) 2016 Robin Rodricks, J.P. Trosclair
https://github.com/robinrodricks/FluentFTP/blob/master/LICENSE.TXT
```

#### SharpZipLib
```
MIT License
Copyright (c) 2000-2018 SharpZipLib Contributors
https://github.com/icsharpcode/SharpZipLib/blob/master/LICENSE.txt
```

#### Fody / Costura.Fody
```
MIT License
Copyright (c) Simon Cropp
https://github.com/Fody/Fody/blob/master/License.txt
https://github.com/Fody/Costura/blob/master/LICENSE
```

