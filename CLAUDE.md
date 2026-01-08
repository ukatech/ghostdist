# GhostDist - C#/.NET版 開発ドキュメント

## 概要

伺か(SSP)ゴーストのネットワーク更新ファイル生成・FTPアップロード・NAR(ZIP)圧縮を行うWindows Forms アプリケーションです。

**プロジェクト構造**: Forms（UI）/ Models（データ）/ Services（ビジネスロジック）/ Utilities（ユーティリティ）

## 技術スタック

- **.NET Framework 4.8** / C# 7.3 / Windows Forms
- **NuGetパッケージ**: FluentFTP (37.0.4), ini-parser-netstandard (2.5.3), SharpZipLib (1.3.3)
- **重要**: すべてのファイルで**Shift_JIS**を使用（ghostdist.ini, updates2.dau, NARファイル名）

## ビルド方法

```bash
# NuGet復元
nuget restore GhostDist.sln

# ビルド
msbuild GhostDist.sln /p:Configuration=Release
```

出力: `GhostDist/bin/Release/GhostDist.exe`

## 伺か/SSP固有の仕様

### 1. updates2.dau形式

SOH文字（`\x01`）区切りのテキストファイル：
```
filename\x01MD5hash\x01size=12345\x01
filename2\x01MD5hash2\x01size=67890\x01charset=Shift_JIS\x01
```

**仕様**:
- 区切り文字: `\x01`（SOH, ASCII 1）
- 1行目のみ `charset=Shift_JIS` を追加
- ファイル名: スラッシュ区切り（例: `ghost/master/descript.txt`）
- MD5: 32桁の16進数小文字
- サイズ: `size=` プレフィックス付き

### 2. NAR (Nanika Archive) 形式

ZIPファイルの伺か専用フォーマット：
- **ファイル名エンコーディング**: Shift_JIS（重要！）
- **必須ファイル**: `install.txt`, `updates2.dau`, `ghost/master/updates2.dau`
- **拡張子**: `.nar`

### 3. 動作モード

- **Network**: updates2.dauのMD5比較→差分のみFTPアップロード
- **Upload**: NAR作成→FTPアップロード→HTML変数置換（`%uploaddate`, `%uploadtime`, `%uploadsize`など）
- **NarCreate**: NAR作成のみ（FTPアップロードなし）

### 4. 変数置換

NARファイル名とHTML内の変数を実行時に置換：
- 日時: `%year`, `%month`, `%day`, `%hour`, `%minute`, `%second`
- アップロード情報: `%uploaddate`, `%uploadtime`, `%uploadsize`

### 5. FTP/FTPS対応

- FluentFTP使用（Passive/Active, Explicit TLS対応）
- 自己署名証明書自動許可（`ValidateAnyCertificate = true`）
- ディレクトリ自動作成、接続再利用

## 設定ファイル仕様 (ghostdist.ini)

**文字エンコーディング**: Shift_JIS / **形式**: INI形式

### セクション構成

**[General]**
- `IsLog` (0/1): ログファイル保存
- `NoLog` (0/1): ログウィンドウ非表示
- `SettingsCount`: プロジェクト数

**[FTP]** （共通FTP設定）
- `Server`, `ID`, `Password`
- `Passive` (0/1): パッシブモード
- `SSL` (0/1): FTPS使用（Explicit TLS）

**[0], [1], ...** （各プロジェクト設定）
- `Name`: プロジェクト名
- `Setting`: 動作モード（`Network`, `Upload`, `NarCreate`）
- `TargetFolder`: ゴースト基準フォルダ
- `ProcessName`: 処理ファイルパターン（カンマ区切り、例: `*.txt,*.dic`）
- `ExcludeName`: 除外ファイルパターン（カンマ区切り、例: `*.bak,*.tmp`）
- `UseCommon` (0/1): 共通FTP設定使用
- FTP設定（個別）: `Server`, `ID`, `Password`, `Passive`, `SSL`, `Directory`
- `HTML`: 配布ページHTMLパス（Upload時のみ）
- `NarName`: NARファイル名テンプレート（変数使用可）
- `DefaultCheck` (0/1): 起動時チェック

## 既知の問題と今後の改善

### 制限事項
1. **Shift_JIS制限**: Shift_JISで表現できない文字を含むファイル名は使用不可
2. **パフォーマンス**: 大量ファイル（1000+）のMD5計算に時間がかかる、FTPアップロード中UIフリーズ
3. **FTPS**: 自己署名証明書を自動許可（`ValidateAnyCertificate = true`）、証明書検証カスタマイズ不可
4. **エラーハンドリング**: FTP接続エラー時のリトライ機能なし

### 改善予定
- **async/await非同期処理化**（UIフリーズ防止）
- **FTPリトライ機能**（ネットワーク不安定時対応）
- **プログレスバー詳細化**（現在のファイル名、進捗率表示）

## トラブルシューティング

| 問題 | 対処法 |
|-----|--------|
| NuGetパッケージ復元失敗 | `nuget locals all -clear` → `nuget restore` |
| ビルドエラー（CS0246） | `Update-Package -reinstall` |
| FTP接続エラー（530） | FTP設定（Server, ID, Password）確認、Passive/Activeモード切替 |
| FTPS接続エラー（TLS） | FTPサーバーのExplicit TLS対応確認、`SSL=0`で通常FTP試行 |
| 日本語ファイル名文字化け | Shift_JIS使用確認、Windows言語設定確認 |

## 参考資料

- **伺か/SSP**: https://ssp.shillest.net/
- **ukagaka.info**: http://ukagaka.info/
- **FluentFTP**: https://github.com/robinrodricks/FluentFTP
- **SharpZipLib**: https://github.com/icsharpcode/SharpZipLib

---
元のDelphiコードのライセンスに準拠 | Last Updated: 2026-01-09
