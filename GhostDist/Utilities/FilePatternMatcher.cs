using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GhostDist.Utilities
{
    /// <summary>
    /// ワイルドカードパターンマッチングでファイルを検索
    /// </summary>
    public static class FilePatternMatcher
    {
        /// <summary>
        /// パターンに一致するファイルのリストを取得
        /// </summary>
        /// <param name="basePath">基準フォルダ</param>
        /// <param name="pattern">パターン(例: ghost/master/*.txt)</param>
        /// <returns>一致するファイルのリスト</returns>
        public static List<string> GetMatchingFiles(string basePath, string pattern)
        {
            try
            {
                // パターンをスラッシュで分割(Delphi版互換のため)
                pattern = pattern.Replace('/', '\\');

                // 絶対パスに変換
                var fullPattern = Path.Combine(basePath, pattern);
                var directory = Path.GetDirectoryName(fullPattern);
                var searchPattern = Path.GetFileName(fullPattern);

                if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
                    return new List<string>();

                // ワイルドカード検索
                var files = Directory.GetFiles(directory, searchPattern, SearchOption.TopDirectoryOnly);
                return files.ToList();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }
    }
}
