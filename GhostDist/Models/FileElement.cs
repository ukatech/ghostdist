using System;
using System.IO;
using System.Linq;
using GhostDist.Utilities;

namespace GhostDist.Models
{
    /// <summary>
    /// updates2.dauに記録される個別ファイル情報
    /// </summary>
    public class FileElement
    {
        /// <summary>
        /// リモート名(スラッシュ区切り) 例: ghost/master/shiori.dll
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ローカルの絶対パス 例: C:\ghost\ghost\master\shiori.dll
        /// </summary>
        public string LocalPath { get; set; }

        /// <summary>
        /// 相対パス(バックスラッシュ区切り) 例: ghost\master\shiori.dll
        /// </summary>
        public string RelativePath { get; set; }

        /// <summary>
        /// MD5ハッシュ値(小文字16進数)
        /// </summary>
        public string MD5 { get; set; }

        /// <summary>
        /// ファイルサイズ(バイト)
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// ターゲットフォルダのパスを設定してローカルパスから各種プロパティを計算
        /// </summary>
        public void SetLocalPath(string targetFolder, string localPath)
        {
            LocalPath = localPath;

            // MD5計算
            MD5 = Md5Helper.CalculateFileMD5(localPath);

            // ファイルサイズ取得
            var fileInfo = new FileInfo(localPath);
            Size = fileInfo.Length;

            // 相対パス計算(バックスラッシュ)
            targetFolder = Path.GetFullPath(targetFolder).TrimEnd('\\') + "\\";
            var fullPath = Path.GetFullPath(localPath);

            if (fullPath.StartsWith(targetFolder, StringComparison.OrdinalIgnoreCase))
            {
                RelativePath = fullPath.Substring(targetFolder.Length);
            }
            else
            {
                RelativePath = Path.GetFileName(localPath);
            }

            // リモート名(スラッシュ区切り)
            Name = RelativePath.Replace('\\', '/');
        }

        /// <summary>
        /// updates2.dauの1行フォーマットに変換
        /// </summary>
        public string ToMD5Line()
        {
            return $"{Name}\x01{MD5}\x01size={Size}\x01";
        }

        /// <summary>
        /// updates2.dauの1行から FileElement を復元
        /// </summary>
        public static FileElement FromMD5Line(string line)
        {
            var parts = line.Split('\x01');
            if (parts.Length < 2)
                return null;

            var element = new FileElement
            {
                Name = parts[0],
                MD5 = parts[1],
                Size = 0
            };

            // size=XXX の解析
            if (parts.Length >= 3)
            {
                var sizeStr = parts[2].Replace("size=", "").Replace("charset=Shift_JIS", "").Trim();
                if (!string.IsNullOrEmpty(sizeStr) && long.TryParse(sizeStr, out long size))
                {
                    element.Size = size;
                }
            }

            return element;
        }
    }
}
