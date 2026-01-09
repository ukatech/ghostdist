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
- **ファイル名エンコーディング**: Shift_JIS + Unicode Path Extra Field（0x7075）の併用
  - ZIPヘッダー: Shift_JIS（旧版SSP互換性）
  - Extra Field 0x7075: UTF-8（新版SSP対応、SSP 2.5.55以降）
  - Shift_JISで表現できない文字がある場合のみ自動的に0x7075を追加
- **必須ファイル**: `install.txt`, `updates2.dau`, `ghost/master/updates2.dau`
- **拡張子**: `.nar`

### 3. Unicode対応（Info-ZIP Unicode Path Extra Field）

**実装方式**: Shift_JIS + Unicode Path Extra Field（0x7075）の併用

**動作原理**:
- Shift_JISで表現可能なファイル名: Shift_JISのみ（後方互換性維持）
- Shift_JISで表現不可能な文字を含む場合: Shift_JIS + 0x7075（UTF-8）を両方含める
- 旧版SSP（2.5.54以前）: 0x7075を無視してShift_JISファイル名を使用
- 新版SSP（2.5.55以降）: 0x7075のUTF-8ファイル名を優先使用
- 一般的なZIPツール（7-zip等）: 0x7075をサポートし、正しく表示

**技術詳細**:
- **0x7075構造**: Header ID（0x7075）+ Data Size + Version（1）+ Name CRC32 + UTF-8 Name
- **CRC32**: Shift_JISファイル名バイト配列のCRC32チェックサム（検証用）
- **実装**: `UnicodePathExtraField.cs`でバイナリ生成、`NarCreationService.cs`で統合
- **判定**: `RequiresUnicodeSupport()`でShift_JISエンコード→デコードの可逆性を確認
- **ログ**: Unicode対応したファイルは「Unicode対応: ファイル名」としてログに記録

### 4. 動作モード

- **Network**: updates2.dauのMD5比較→差分のみFTPアップロード
- **Upload**: NAR作成→FTPアップロード→HTML変数置換（`%uploaddate`, `%uploadtime`, `%uploadsize`など）
- **NarCreate**: NAR作成のみ（FTPアップロードなし）

### 5. 変数置換

NARファイル名とHTML内の変数を実行時に置換：
- 日時: `%year`, `%month`, `%day`, `%hour`, `%minute`, `%second`
- アップロード情報: `%uploaddate`, `%uploadtime`, `%uploadsize`

### 6. FTP/FTPS対応

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
1. **Unicode対応**: Unicode Path Extra Field（0x7075）により、Shift_JISで表現できない文字（絵文字等）も使用可能。ただし、旧版SSP（2.5.54以前）では文字化けの可能性あり（`?`等に置換）
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
| Unicode文字が正しく表示されない | SSP 2.5.55以降を使用、7-zipで開いてファイル名を確認 |

## Unicode対応の検証方法

### テストケース

1. **Shift_JIS範囲内のファイル名**（例: `さくら.txt`）
   - 期待動作: Shift_JISのみ（0x7075なし）
   - 検証: 旧版SSPで正常動作

2. **Shift_JIS範囲外のファイル名**（例: `テスト😀.txt`）
   - 期待動作: Shift_JIS（`?`に置換） + 0x7075（正確なUTF-8）
   - 検証: 新版SSP（2.5.55以降）で絵文字が正しく表示、ログに「Unicode対応」表示

3. **混在パターン**
   - 期待動作: 必要なファイルのみ0x7075追加
   - 検証: NARファイルをHexエディタで確認

### 検証ツール

- **7-zip**: 0x7075をサポート、UTF-8ファイル名が正しく表示されることを確認
- **Hexエディタ**: NARファイル内のExtra Fieldに`75 70`（0x7075のLittle-Endian）が存在するか確認
- **SSP実機テスト**: 旧版（2.5.54）と新版（2.5.55以降）で動作確認

## 参考資料

- **伺か/SSP**: https://ssp.shillest.net/
- **SSP 2.5.55 Changelog（Unicode対応）**: https://changelog.de10.moe/entry/2021/12/29/220754
- **ukagaka.info**: http://ukagaka.info/
- **FluentFTP**: https://github.com/robinrodricks/FluentFTP
- **SharpZipLib**: https://github.com/icsharpcode/SharpZipLib
- **PKWARE APPNOTE.TXT（ZIP仕様）**: https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT
- **Info-ZIP Extra Fields**: https://libzip.org/specifications/extrafld.txt

---
元のDelphiコードのライセンスに準拠 | Last Updated: 2026-01-09 | Unicode対応実装完了
