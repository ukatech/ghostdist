using System;
using System.IO;
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
            if (!updates.Make(settings.ProcessName, settings.ExcludeName))
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
            updates.SaveToFile(dauPath);

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
                    var shiftJisEncoding = Encoding.GetEncoding("Shift_JIS");
                    ZipStrings.CodePage = shiftJisEncoding.CodePage;

                    foreach (var file in updates.Files)
                    {
                        // アーカイブ内のパス（相対パス、スラッシュ区切り）
                        var entryName = file.RelativePath.Replace('\\', '/');

                        // エントリ作成
                        var entry = new ZipEntry(entryName);
                        entry.DateTime = File.GetLastWriteTime(file.LocalPath);
                        entry.Size = new FileInfo(file.LocalPath).Length;

                        // Unicode Path Extra Field追加（Shift_JISで表現できない文字がある場合）
                        if (RequiresUnicodeSupport(entryName, shiftJisEncoding))
                        {
                            var shiftJisBytes = shiftJisEncoding.GetBytes(entryName);
                            var unicodeExtraField = UnicodePathExtraField.Create(entryName, shiftJisBytes);

                            // 既存のExtraDataと結合（存在する場合）
                            if (entry.ExtraData != null && entry.ExtraData.Length > 0)
                            {
                                var combined = new byte[entry.ExtraData.Length + unicodeExtraField.Length];
                                Array.Copy(entry.ExtraData, 0, combined, 0, entry.ExtraData.Length);
                                Array.Copy(unicodeExtraField, 0, combined, entry.ExtraData.Length, unicodeExtraField.Length);
                                entry.ExtraData = combined;
                            }
                            else
                            {
                                entry.ExtraData = unicodeExtraField;
                            }

                            OnLogMessage($"Unicode対応: {entryName}");
                        }

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
        /// ファイル名がShift_JISで正確に表現できるか判定
        /// </summary>
        /// <param name="filename">判定対象のファイル名</param>
        /// <param name="shiftJisEncoding">Shift_JISエンコーディング</param>
        /// <returns>Unicode対応が必要な場合true</returns>
        private bool RequiresUnicodeSupport(string filename, Encoding shiftJisEncoding)
        {
            try
            {
                // Shift_JISでエンコード→デコードして元に戻るか確認
                var bytes = shiftJisEncoding.GetBytes(filename);
                var decoded = shiftJisEncoding.GetString(bytes);

                // 完全一致しない場合はUnicode必須
                return decoded != filename;
            }
            catch
            {
                // エンコード失敗 = Unicode必須
                return true;
            }
        }

        protected virtual void OnLogMessage(string message)
        {
            LogMessage?.Invoke(this, message);
        }
    }
}
