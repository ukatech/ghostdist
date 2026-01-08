using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GhostDist.Utilities
{
    /// <summary>
    /// ファイルリスト構築のヘルパークラス
    /// </summary>
    public static class FileListBuilder
    {
        /// <summary>
        /// 複数のパターンから重複なしでファイルリストを構築
        /// </summary>
        /// <param name="basePath">基準フォルダ</param>
        /// <param name="patterns">パターンのリスト</param>
        /// <param name="excludePatterns">除外パターンのリスト</param>
        /// <returns>ファイルパスのリスト</returns>
        public static List<string> BuildFileList(string basePath, IEnumerable<string> patterns, IEnumerable<string> excludePatterns)
        {
            var files = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var excludeFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // 除外ファイルのリストを作成
            if (excludePatterns != null)
            {
                foreach (var pattern in excludePatterns)
                {
                    var matchedFiles = FilePatternMatcher.GetMatchingFiles(basePath, pattern);
                    foreach (var file in matchedFiles)
                    {
                        excludeFiles.Add(Path.GetFullPath(file));
                    }
                }
            }

            // 処理対象ファイルのリストを作成
            if (patterns != null)
            {
                foreach (var pattern in patterns)
                {
                    var matchedFiles = FilePatternMatcher.GetMatchingFiles(basePath, pattern);
                    foreach (var file in matchedFiles)
                    {
                        var fullPath = Path.GetFullPath(file);
                        if (!excludeFiles.Contains(fullPath))
                        {
                            files.Add(fullPath);
                        }
                    }
                }
            }

            return files.ToList();
        }
    }
}
