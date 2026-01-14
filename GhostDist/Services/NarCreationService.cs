using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GhostDist.Models;
using GhostDist.Utilities;
using ICSharpCode.SharpZipLib.Zip;

namespace GhostDist.Services
{
    /// <summary>
    /// NAR(ZIP)ファイル作成サービス
    /// </summary>
    public class NarCreationService
    {
        public event EventHandler<string> LogMessage;

        /// <summary>
        /// NARファイルを作成
        /// </summary>
        /// <param name="settings">プロジェクト設定</param>
        /// <returns>作成されたNARファイルのパス</returns>
        public string CreateNar(ProjectSettings settings)
        {
            OnLogMessage("NARを生成します。");

            var updates = new Updates2DauManager
            {
                TargetFolder = settings.TargetFolder
            };

            // ファイルリスト生成
            OnLogMessage("ファイルリストを生成します。");

            // NAR作成に必須のファイルパターンを追加
            var processPatterns = AddRequiredPatternsForNar(settings.ProcessName);

            if (!updates.Make(processPatterns, settings.ExcludeName))
            {
                OnLogMessage($"delete.txtに問題となる可能性のある記述を検出しました。");
                OnLogMessage(updates.DeleteTxtError);
                throw new Exception("delete.txtに問題があります。");
            }

            OnLogMessage(updates.GetFileList());
            OnLogMessage("ファイルリストを生成しました。");

            // updates2.dau生成
            var targetFolder = Path.GetFullPath(settings.TargetFolder).TrimEnd('\\') + "\\";
            var dauPath = Path.Combine(targetFolder, "updates2.dau");
            updates.SaveToFile(dauPath, OnLogMessage);

            // ghost/master/updates2.dauにもコピー
            var masterDauPath = Path.Combine(targetFolder, "ghost", "master", "updates2.dau");
            var masterDir = Path.GetDirectoryName(masterDauPath);
            if (!Directory.Exists(masterDir))
            {
                Directory.CreateDirectory(masterDir);
            }
            File.Copy(dauPath, masterDauPath, true);

            // ZIP圧縮
            OnLogMessage("圧縮処理を開始します。");
            var tempZipPath = Path.Combine(targetFolder, "temp.zip");

            try
            {
                using (var zipStream = new FileStream(tempZipPath, FileMode.Create))
                using (var zipOutput = new ZipOutputStream(zipStream))
                {
                    // Shift_JIS対応
                    zipOutput.SetLevel(9); // 最高圧縮レベル
                    zipOutput.IsStreamOwner = true;

                    // Shift_JISエンコーディング設定（後方互換性のため維持）
                    var shiftJisEncoding = EncodingHelper.GetShiftJISEncoding();
                    ZipStrings.CodePage = shiftJisEncoding.CodePage;

                    // ディレクトリを収集（重複を排除）
                    var directories = new HashSet<string>();
                    foreach (var file in updates.Files)
                    {
                        var entryName = file.RelativePath.Replace('\\', '/');
                        var parts = entryName.Split('/');

                        // 親ディレクトリを全て収集
                        for (int i = 0; i < parts.Length - 1; i++)
                        {
                            var dirPath = string.Join("/", parts.Take(i + 1)) + "/";
                            directories.Add(dirPath);
                        }
                    }

                    // ディレクトリエントリを先に追加（アルファベット順）
                    foreach (var dirPath in directories.OrderBy(d => d))
                    {
                        AddDirectoryEntry(zipOutput, dirPath, targetFolder, shiftJisEncoding);
                    }

                    // ファイルエントリを追加
                    foreach (var file in updates.Files)
                    {
                        // アーカイブ内のパス（相対パス、スラッシュ区切り）
                        var entryName = file.RelativePath.Replace('\\', '/');

                        // エントリ作成
                        var entry = new ZipEntry(entryName);
                        var fileInfo = new FileInfo(file.LocalPath);
                        entry.DateTime = fileInfo.LastWriteTime;
                        entry.Size = fileInfo.Length;

                        // 256バイト以下のファイルはStore（無圧縮）に設定
                        if (fileInfo.Length <= 256)
                        {
                            entry.CompressionMethod = CompressionMethod.Stored;
                        }

                        // Extra Fieldsを追加
                        var extraFields = new List<byte[]>();

                        // NTFS Timestamp Extra Field追加
                        var ntfsTimestamp = NtfsTimestampExtraField.Create(
                            fileInfo.LastWriteTime,
                            fileInfo.LastAccessTime,
                            fileInfo.CreationTime
                        );
                        extraFields.Add(ntfsTimestamp);

                        // Unicode Path Extra Field追加（Shift_JISで表現できない文字がある場合）
                        if (EncodingHelper.RequiresUnicodeSupport(entryName))
                        {
                            var shiftJisBytes = shiftJisEncoding.GetBytes(entryName);
                            var unicodeExtraField = UnicodePathExtraField.Create(entryName, shiftJisBytes);
                            extraFields.Add(unicodeExtraField);

                            OnLogMessage($"Unicode対応: {entryName}");
                        }

                        // すべてのExtra Fieldsを結合
                        entry.ExtraData = CombineExtraFields(extraFields);

                        zipOutput.PutNextEntry(entry);

                        // ファイル内容をコピー
                        using (var fileStream = File.OpenRead(file.LocalPath))
                        {
                            fileStream.CopyTo(zipOutput);
                        }

                        zipOutput.CloseEntry();
                    }

                    zipOutput.Finish();
                }

                OnLogMessage("圧縮処理を終了しました。");
            }
            catch (Exception ex)
            {
                OnLogMessage($"圧縮処理に失敗しました: {ex.Message}");
                throw;
            }

            // ファイル名の変数置換
            var finalNarName = ReplaceVariables(settings.NarName);
            var finalNarPath = Path.GetFullPath(finalNarName);

            // 既存ファイルを削除
            if (File.Exists(finalNarPath))
            {
                File.Delete(finalNarPath);
            }

            // リネーム
            File.Move(tempZipPath, finalNarPath);
            OnLogMessage($"{finalNarPath}を生成しました。");

            return finalNarPath;
        }

        /// <summary>
        /// ファイル名テンプレートの変数を置換
        /// </summary>
        private string ReplaceVariables(string template)
        {
            var now = DateTime.Now;

            return template
                .Replace("%year", now.Year.ToString("0000"))
                .Replace("%shortyear", (now.Year % 100).ToString("00"))
                .Replace("%month", now.Month.ToString("00"))
                .Replace("%day", now.Day.ToString("00"))
                .Replace("%hour", now.Hour.ToString("00"))
                .Replace("%minute", now.Minute.ToString("00"))
                .Replace("%second", now.Second.ToString("00"));
        }

        /// <summary>
        /// ディレクトリエントリをZIPに追加
        /// </summary>
        /// <param name="zipOutput">ZIPストリーム</param>
        /// <param name="dirPath">ディレクトリパス（末尾に/を含む）</param>
        /// <param name="targetFolder">対象フォルダ</param>
        /// <param name="shiftJisEncoding">Shift_JISエンコーディング</param>
        private void AddDirectoryEntry(ZipOutputStream zipOutput, string dirPath, string targetFolder, Encoding shiftJisEncoding)
        {
            // ディレクトリエントリ作成（末尾に/を付ける）
            var entry = new ZipEntry(dirPath);
            entry.DateTime = DateTime.Now;
            entry.Size = 0;

            // ディレクトリの実際のパスを取得（タイムスタンプ取得用）
            var localDirPath = Path.Combine(targetFolder, dirPath.Replace('/', '\\').TrimEnd('\\'));
            DirectoryInfo dirInfo = null;
            if (Directory.Exists(localDirPath))
            {
                dirInfo = new DirectoryInfo(localDirPath);
                entry.DateTime = dirInfo.LastWriteTime;
            }

            // Extra Fieldsを追加
            var extraFields = new List<byte[]>();

            // NTFS Timestamp Extra Field追加（ディレクトリが存在する場合）
            if (dirInfo != null)
            {
                var ntfsTimestamp = NtfsTimestampExtraField.Create(
                    dirInfo.LastWriteTime,
                    dirInfo.LastAccessTime,
                    dirInfo.CreationTime
                );
                extraFields.Add(ntfsTimestamp);
            }

            // Unicode Path Extra Field追加（Shift_JISで表現できない文字がある場合）
            if (EncodingHelper.RequiresUnicodeSupport(dirPath))
            {
                var shiftJisBytes = shiftJisEncoding.GetBytes(dirPath);
                var unicodeExtraField = UnicodePathExtraField.Create(dirPath, shiftJisBytes);
                extraFields.Add(unicodeExtraField);

                OnLogMessage($"Unicode対応 (ディレクトリ): {dirPath}");
            }

            // すべてのExtra Fieldsを結合
            entry.ExtraData = CombineExtraFields(extraFields);

            zipOutput.PutNextEntry(entry);
            zipOutput.CloseEntry();
        }

        /// <summary>
        /// 複数のExtra Fieldバイト配列を結合
        /// </summary>
        /// <param name="extraFields">Extra Fieldのリスト</param>
        /// <returns>結合されたバイト配列</returns>
        private byte[] CombineExtraFields(List<byte[]> extraFields)
        {
            if (extraFields == null || extraFields.Count == 0)
            {
                return null;
            }

            var totalLength = extraFields.Sum(ef => ef.Length);
            var combined = new byte[totalLength];
            var offset = 0;

            foreach (var extraField in extraFields)
            {
                Array.Copy(extraField, 0, combined, offset, extraField.Length);
                offset += extraField.Length;
            }

            return combined;
        }

        /// <summary>
        /// NAR作成に必須のファイルパターンを追加
        /// </summary>
        /// <param name="processPatterns">元の処理対象パターン</param>
        /// <returns>必須パターンを含む処理対象パターン</returns>
        private string AddRequiredPatternsForNar(string processPatterns)
        {
            var requiredPatterns = new[] { "install.txt", "updates2.dau", "ghost/master/updates2.dau" };
            var patterns = new List<string>();

            // 既存のパターンを追加
            if (!string.IsNullOrEmpty(processPatterns))
            {
                patterns.AddRange(processPatterns.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => p.Trim())
                    .Where(p => !string.IsNullOrEmpty(p)));
            }

            // 必須パターンを追加（重複チェック）
            foreach (var required in requiredPatterns)
            {
                if (!patterns.Any(p => p.Equals(required, StringComparison.OrdinalIgnoreCase)))
                {
                    patterns.Add(required);
                }
            }

            return string.Join(Environment.NewLine, patterns);
        }

        protected virtual void OnLogMessage(string message)
        {
            LogMessage?.Invoke(this, message);
        }
    }
}
