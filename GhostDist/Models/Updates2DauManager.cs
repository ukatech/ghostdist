using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GhostDist.Utilities;

namespace GhostDist.Models
{
    /// <summary>
    /// updates2.dauファイルの管理クラス
    /// </summary>
    public class Updates2DauManager
    {
        /// <summary>
        /// ファイル要素のリスト
        /// </summary>
        public List<FileElement> Files { get; set; }

        /// <summary>
        /// ターゲットフォルダ(基準フォルダ)
        /// </summary>
        public string TargetFolder { get; set; }

        /// <summary>
        /// delete.txtのエラーメッセージ
        /// </summary>
        public string DeleteTxtError { get; private set; }

        public Updates2DauManager()
        {
            Files = new List<FileElement>();
            TargetFolder = "";
            DeleteTxtError = "";
        }

        /// <summary>
        /// ファイルリストを生成
        /// </summary>
        /// <param name="processPatterns">処理対象パターン(改行区切り)</param>
        /// <param name="excludePatterns">除外パターン(改行区切り)</param>
        /// <returns>成功時true、delete.txtに問題がある場合false</returns>
        public bool Make(string processPatterns, string excludePatterns)
        {
            Files.Clear();
            DeleteTxtError = "";

            // パターンをリストに分割
            var processList = SplitPatterns(processPatterns);
            var excludeList = SplitPatterns(excludePatterns);

            // 除外パターンに一致するファイルのリストを取得
            var excludeFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var pattern in excludeList)
            {
                var files = FilePatternMatcher.GetMatchingFiles(TargetFolder, pattern);
                foreach (var file in files)
                {
                    excludeFiles.Add(Path.GetFullPath(file));
                }
            }

            // 処理対象パターンに一致するファイルを収集
            foreach (var pattern in processList)
            {
                var files = FilePatternMatcher.GetMatchingFiles(TargetFolder, pattern);
                foreach (var file in files)
                {
                    var fullPath = Path.GetFullPath(file);

                    // 除外リストに含まれていない & 重複していない
                    if (!excludeFiles.Contains(fullPath) &&
                        !Files.Any(f => f.LocalPath.Equals(fullPath, StringComparison.OrdinalIgnoreCase)))
                    {
                        var element = new FileElement();
                        element.SetLocalPath(TargetFolder, fullPath);
                        Files.Add(element);
                    }
                }
            }

            // delete.txtのバリデーション
            return ValidateDeleteTxt();
        }

        /// <summary>
        /// delete.txtが存在する場合、ファイルリストと重複がないかチェック
        /// </summary>
        private bool ValidateDeleteTxt()
        {
            var deleteTxtPath = Path.Combine(TargetFolder, "delete.txt");
            if (!File.Exists(deleteTxtPath))
                return true;

            // delete.txtに含まれるファイル名を取得
            var deleteLines = File.ReadAllLines(deleteTxtPath, Encoding.GetEncoding("Shift_JIS"));
            var errors = new List<string>();

            for (int i = 0; i < deleteLines.Length; i++)
            {
                var deleteLine = deleteLines[i].Trim();
                if (string.IsNullOrEmpty(deleteLine))
                    continue;

                // ファイルリストに同じファイルが存在するかチェック
                foreach (var file in Files)
                {
                    // 相対パスで比較(スラッシュ/バックスラッシュ両対応)
                    var normalizedDelete = deleteLine.Replace('\\', '/');
                    var normalizedFile = file.RelativePath.Replace('\\', '/');

                    if (normalizedDelete.Equals(normalizedFile, StringComparison.OrdinalIgnoreCase))
                    {
                        errors.Add($"delete.txt {i + 1}行目、{deleteLine} は存在します。");
                    }
                }
            }

            if (errors.Count > 0)
            {
                DeleteTxtError = string.Join(Environment.NewLine, errors);
                return false;
            }

            return true;
        }

        /// <summary>
        /// パターン文字列を改行で分割してリストに変換
        /// </summary>
        private List<string> SplitPatterns(string patterns)
        {
            if (string.IsNullOrEmpty(patterns))
                return new List<string>();

            return patterns.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                          .Select(p => p.Trim())
                          .Where(p => !string.IsNullOrEmpty(p))
                          .ToList();
        }

        /// <summary>
        /// updates2.dauファイルから読み込み
        /// </summary>
        public void LoadFromFile(string path)
        {
            Files.Clear();

            if (!File.Exists(path))
                return;

            var lines = File.ReadAllLines(path, Encoding.GetEncoding("Shift_JIS"));
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var element = FileElement.FromMD5Line(line);
                if (element != null)
                {
                    Files.Add(element);
                }
            }
        }

        /// <summary>
        /// updates2.dauファイルに保存
        /// </summary>
        public void SaveToFile(string path)
        {
            var lines = new List<string>();

            for (int i = 0; i < Files.Count; i++)
            {
                var line = Files[i].ToMD5Line();

                // 最初の行にcharsetを追加
                if (i == 0)
                {
                    line += "charset=Shift_JIS\x01";
                }

                lines.Add(line);
            }

            File.WriteAllLines(path, lines, Encoding.GetEncoding("Shift_JIS"));
        }

        /// <summary>
        /// ファイル名で検索
        /// </summary>
        public FileElement Find(string name)
        {
            return Files.FirstOrDefault(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// ファイルリストを取得(相対パス、改行区切り)
        /// </summary>
        public string GetFileList()
        {
            return string.Join(Environment.NewLine, Files.Select(f => f.RelativePath));
        }
    }
}
