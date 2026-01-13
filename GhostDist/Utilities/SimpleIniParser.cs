using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace GhostDist.Utilities
{
    /// <summary>
    /// 独自のINIファイルパーサ
    /// 元のDelphiプログラムのファイル末尾ゴミ問題に対処するため、
    /// SettingsCountで指定された数だけ読み込み、それ以降は無視する
    /// </summary>
    public class SimpleIniParser
    {
        private static readonly Encoding ShiftJis = Encoding.GetEncoding("Shift_JIS");

        /// <summary>
        /// INIファイルを解析してSimpleIniDataを返す
        /// </summary>
        public SimpleIniData Parse(string content)
        {
            var data = new SimpleIniData();
            var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.None);

            string currentSection = null;
            int? settingsCount = null;
            var processedProjectSections = new HashSet<int>();

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();

                // 空行またはコメント行をスキップ
                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith(";") || trimmedLine.StartsWith("#"))
                {
                    continue;
                }

                // セクションヘッダー [SectionName]
                var sectionMatch = Regex.Match(trimmedLine, @"^\[(.+)\]$");
                if (sectionMatch.Success)
                {
                    currentSection = sectionMatch.Groups[1].Value;
                    data.AddSection(currentSection);

                    // SettingsCountが既に読み込まれている場合、プロジェクトセクションの制限を適用
                    if (settingsCount.HasValue)
                    {
                        // プロジェクトセクション（[0], [1], [2]...）かチェック
                        if (int.TryParse(currentSection, out int sectionIndex))
                        {
                            // SettingsCountを超えるセクションは無視
                            if (sectionIndex >= settingsCount.Value)
                            {
                                currentSection = null; // このセクションを無視
                                continue;
                            }
                            processedProjectSections.Add(sectionIndex);
                        }
                    }
                    continue;
                }

                // キー=値のペア
                var keyValueMatch = Regex.Match(trimmedLine, @"^([^=]+)=(.*)$");
                if (keyValueMatch.Success && currentSection != null)
                {
                    var key = keyValueMatch.Groups[1].Value.Trim();
                    var value = keyValueMatch.Groups[2].Value; // 値は前後の空白を保持

                    data.AddKeyValue(currentSection, key, value);

                    // SettingsCountを読み込んだら記録
                    if (currentSection.Equals("General", StringComparison.OrdinalIgnoreCase) &&
                        key.Equals("SettingsCount", StringComparison.OrdinalIgnoreCase))
                    {
                        if (int.TryParse(value, out int count))
                        {
                            settingsCount = count;
                        }
                    }
                }
            }

            return data;
        }

        /// <summary>
        /// ファイルからINIデータを読み込む
        /// </summary>
        public SimpleIniData ParseFile(string filePath)
        {
            string content;
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(fileStream, ShiftJis))
            {
                content = reader.ReadToEnd();
            }

            return Parse(content);
        }

        /// <summary>
        /// INIデータをファイルに書き込む
        /// </summary>
        public void WriteFile(string filePath, SimpleIniData data)
        {
            using (var writer = new StreamWriter(filePath, false, ShiftJis))
            {
                var sections = data.GetAllSections();

                // セクションの順序を制御（FTP、General、次に数値セクション）
                var orderedSections = new List<string>();

                // FTPセクションを最初に
                if (sections.ContainsKey("FTP"))
                {
                    orderedSections.Add("FTP");
                }

                // Generalセクションを次に
                if (sections.ContainsKey("General"))
                {
                    orderedSections.Add("General");
                }

                // 数値セクション（プロジェクト設定）を昇順で
                var numericSections = new List<int>();
                foreach (var sectionName in sections.Keys)
                {
                    if (int.TryParse(sectionName, out int index))
                    {
                        numericSections.Add(index);
                    }
                }
                numericSections.Sort();
                foreach (var index in numericSections)
                {
                    orderedSections.Add(index.ToString());
                }

                // その他のセクション
                foreach (var sectionName in sections.Keys)
                {
                    if (!orderedSections.Contains(sectionName))
                    {
                        orderedSections.Add(sectionName);
                    }
                }

                // セクションを書き込み
                foreach (var sectionName in orderedSections)
                {
                    var sectionData = sections[sectionName];

                    // セクションヘッダー
                    writer.WriteLine($"[{sectionName}]");

                    // キー=値のペア
                    foreach (var kvp in sectionData)
                    {
                        writer.WriteLine($"{kvp.Key}={kvp.Value}");
                    }

                    // セクション間の空行
                    writer.WriteLine();
                }
            }
        }
    }
}
