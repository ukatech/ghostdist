using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GhostDist.Models;
using IniParser;
using IniParser.Model;
using IniParser.Parser;

namespace GhostDist.Services
{
    /// <summary>
    /// INI設定ファイルの読み書きサービス
    /// </summary>
    public class ConfigurationService
    {
        private string _iniPath;

        public ConfigurationService(string iniPath)
        {
            _iniPath = iniPath;
        }

        /// <summary>
        /// 設定ファイルのパスを取得
        /// </summary>
        public string IniPath => _iniPath;

        /// <summary>
        /// すべての設定を一括読み込み
        /// </summary>
        public void LoadAll(out bool isLog, out bool noLogWindow, out FtpConfiguration commonFtp, out List<ProjectSettings> projects)
        {
            isLog = false;
            noLogWindow = false;
            commonFtp = new FtpConfiguration();
            projects = new List<ProjectSettings>();

            // パスの正規化
            string fullPath;
            try
            {
                fullPath = Path.GetFullPath(_iniPath);
            }
            catch (Exception ex)
            {
                throw new Exception($"設定ファイルのパスが無効です: {_iniPath}\n詳細: {ex.Message}", ex);
            }

            // ファイルの存在確認（より詳細なチェック）
            if (!File.Exists(fullPath))
            {
                // ファイルが存在しない場合は正常（初回起動）
                return;
            }

            // ファイル情報を取得（エラー時の診断用）
            FileInfo fileInfo;
            try
            {
                fileInfo = new FileInfo(fullPath);
            }
            catch (Exception ex)
            {
                throw new Exception($"設定ファイルの情報を取得できません: {fullPath}\n詳細: {ex.Message}", ex);
            }

            try
            {
                // ファイルを共有読み取りモードで読み込み
                string iniContent;
                var encoding = Encoding.GetEncoding("Shift_JIS");

                using (var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var reader = new StreamReader(fileStream, encoding))
                {
                    iniContent = reader.ReadToEnd();
                }

                // 文字列からパース
                var parser = new IniDataParser();
                var data = parser.Parse(iniContent);

                // 一般設定
                if (data.Sections.ContainsSection("General"))
                {
                    isLog = ParseBool(data["General"]["IsLog"]);
                    noLogWindow = ParseBool(data["General"]["NoLog"]);
                }

                // 共通FTP設定
                if (data.Sections.ContainsSection("FTP"))
                {
                    commonFtp.Server = data["FTP"]["Server"] ?? "";
                    commonFtp.UserId = data["FTP"]["ID"] ?? "";
                    commonFtp.Password = data["FTP"]["Password"] ?? "";
                    commonFtp.Passive = ParseBool(data["FTP"]["Passive"]);
                    commonFtp.UseSSL = ParseBool(data["FTP"]["SSL"]);
                    commonFtp.UploadDestinationType = ParseUploadType(data["FTP"]["UploadType"]);
                    commonFtp.NarNaLoaderUploadUrl = data["FTP"]["UploadURL"] ?? "";
                }

                // プロジェクト設定
                if (data.Sections.ContainsSection("General"))
                {
                    var countStr = data["General"]["SettingsCount"];
                    if (!string.IsNullOrEmpty(countStr) && int.TryParse(countStr, out int count))
                    {
                        for (int i = 0; i < count; i++)
                        {
                            var section = i.ToString();
                            if (!data.Sections.ContainsSection(section))
                                continue;

                            var project = new ProjectSettings
                            {
                                Name = data[section]["Name"] ?? "",
                                Type = ParseSettingType(data[section]["Setting"]),
                                Directory = data[section]["Directory"] ?? "/",
                                HtmlFile = data[section]["HTML"] ?? "",
                                NarName = data[section]["NarName"] ?? "",
                                ProcessName = ConvertCommaTextToLines(data[section]["ProcessName"]),
                                ExcludeName = ConvertCommaTextToLines(data[section]["ExcludeName"]),
                                TargetFolder = data[section]["TargetFolder"] ?? "",
                                DefaultCheck = ParseBool(data[section]["DefaultCheck"], true),
                                UseCommonFtp = ParseBool(data[section]["UseCommon"]),
                                PrivateFtp = new FtpConfiguration
                                {
                                    Server = data[section]["Server"] ?? "",
                                    UserId = data[section]["ID"] ?? "",
                                    Password = data[section]["Password"] ?? "",
                                    Passive = ParseBool(data[section]["Passive"]),
                                    UseSSL = ParseBool(data[section]["SSL"]),
                                    UploadDestinationType = ParseUploadType(data[section]["UploadType"]),
                                    NarNaLoaderUploadUrl = data[section]["UploadURL"] ?? ""
                                },
                                NarNaLoaderGhostId = data[section]["UploadGhostID"] ?? ""
                            };

                            projects.Add(project);
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new Exception(
                    $"設定ファイルへのアクセスが拒否されました。\n" +
                    $"パス: {fullPath}\n" +
                    $"サイズ: {fileInfo.Length:N0} bytes\n" +
                    $"詳細: {ex.Message}", ex);
            }
            catch (IOException ex)
            {
                throw new Exception(
                    $"設定ファイルの読み込み中にI/Oエラーが発生しました。\n" +
                    $"パス: {fullPath}\n" +
                    $"サイズ: {fileInfo.Length:N0} bytes\n" +
                    $"ネットワークドライブの場合は接続を確認してください。\n" +
                    $"詳細: {ex.Message}", ex);
            }
            catch (IniParser.Exceptions.ParsingException ex)
            {
                throw new Exception(
                    $"設定ファイルの形式が不正です。\n" +
                    $"パス: {fullPath}\n" +
                    $"サイズ: {fileInfo.Length:N0} bytes\n" +
                    $"詳細: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"設定ファイルの読み込みに失敗しました。\n" +
                    $"パス: {fullPath}\n" +
                    $"サイズ: {fileInfo.Length:N0} bytes\n" +
                    $"例外の種類: {ex.GetType().Name}\n" +
                    $"詳細: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// プロジェクト設定リストを保存
        /// </summary>
        public void SaveProjects(List<ProjectSettings> projects, FtpConfiguration commonFtp, bool isLog, bool noLogWindow)
        {
            try
            {
                var data = new IniData();

                // FTP共通設定
                data["FTP"]["Server"] = commonFtp.Server;
                data["FTP"]["ID"] = commonFtp.UserId;
                data["FTP"]["Password"] = commonFtp.Password;
                data["FTP"]["Passive"] = commonFtp.Passive ? "1" : "0";
                data["FTP"]["SSL"] = commonFtp.UseSSL ? "1" : "0";
                data["FTP"]["UploadType"] = commonFtp.UploadDestinationType == UploadType.NarNaLoader ? "NNRD" : "FTP";
                data["FTP"]["UploadURL"] = commonFtp.NarNaLoaderUploadUrl;

                // 一般設定
                data["General"]["IsLog"] = isLog ? "1" : "0";
                data["General"]["NoLog"] = noLogWindow ? "1" : "0";
                data["General"]["SettingsCount"] = projects.Count.ToString();

                // 各プロジェクト設定
                for (int i = 0; i < projects.Count; i++)
                {
                    var section = i.ToString();
                    var project = projects[i];

                    data[section]["Name"] = project.Name;
                    data[section]["Setting"] = project.Type.ToString();
                    data[section]["Server"] = project.PrivateFtp.Server;
                    data[section]["ID"] = project.PrivateFtp.UserId;
                    data[section]["Password"] = project.PrivateFtp.Password;
                    data[section]["Passive"] = project.PrivateFtp.Passive ? "1" : "0";
                    data[section]["SSL"] = project.PrivateFtp.UseSSL ? "1" : "0";
                    data[section]["Directory"] = project.Directory;
                    data[section]["HTML"] = project.HtmlFile;
                    data[section]["NarName"] = project.NarName;
                    data[section]["UseCommon"] = project.UseCommonFtp ? "1" : "0";
                    data[section]["TargetFolder"] = project.TargetFolder;
                    data[section]["DefaultCheck"] = project.DefaultCheck ? "1" : "0";
                    data[section]["ProcessName"] = ConvertLinesToCommaText(project.ProcessName);
                    data[section]["ExcludeName"] = ConvertLinesToCommaText(project.ExcludeName);

                    // ななろだ設定
                    data[section]["UploadType"] = project.PrivateFtp.UploadDestinationType == UploadType.NarNaLoader ? "NNRD" : "FTP";
                    data[section]["UploadURL"] = project.PrivateFtp.NarNaLoaderUploadUrl;
                    // GhostIDはプロジェクト個別設定
                    data[section]["UploadGhostID"] = project.NarNaLoaderGhostId;
                }

                // カスタムINI書き込み（"="前後の空白を入れない）
                WriteIniFile(_iniPath, data);
            }
            catch (Exception ex)
            {
                throw new Exception($"設定ファイルの保存に失敗しました: {ex.Message}", ex);
            }
        }

        private SettingType ParseSettingType(string value)
        {
            if (string.IsNullOrEmpty(value))
                return SettingType.Network;

            switch (value.ToLower())
            {
                case "network":
                    return SettingType.Network;
                case "upload":
                    return SettingType.Upload;
                case "narcreate":
                    return SettingType.NarCreate;
                default:
                    return SettingType.Network;
            }
        }

        /// <summary>
        /// UploadType文字列をenumに変換
        /// </summary>
        private UploadType ParseUploadType(string value)
        {
            if (string.IsNullOrEmpty(value))
                return UploadType.FTP; // デフォルト

            switch (value.ToLower())
            {
                case "nnrd":
                case "nanaloader":
                    return UploadType.NarNaLoader;
                case "ftp":
                default:
                    return UploadType.FTP;
            }
        }

        private bool ParseBool(string value, bool defaultValue = false)
        {
            if (string.IsNullOrEmpty(value))
                return defaultValue;

            return value == "1" || value.ToLower() == "true";
        }

        /// <summary>
        /// カンマ区切りテキストを改行区切りに変換
        /// </summary>
        private string ConvertCommaTextToLines(string commaText)
        {
            if (string.IsNullOrEmpty(commaText))
                return "";

            var parts = commaText.Split(',');
            return string.Join(Environment.NewLine, parts.Select(p => p.Trim()).Where(p => !string.IsNullOrEmpty(p)));
        }

        /// <summary>
        /// 改行区切りテキストをカンマ区切りに変換
        /// </summary>
        private string ConvertLinesToCommaText(string lines)
        {
            if (string.IsNullOrEmpty(lines))
                return "";

            var parts = lines.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(",", parts.Select(p => p.Trim()).Where(p => !string.IsNullOrEmpty(p)));
        }

        /// <summary>
        /// INIファイルをカスタム形式で書き込み（"="前後の空白なし）
        /// </summary>
        private void WriteIniFile(string filePath, IniData data)
        {
            var encoding = Encoding.GetEncoding("Shift_JIS");
            using (var writer = new StreamWriter(filePath, false, encoding))
            {
                foreach (var section in data.Sections)
                {
                    // セクションヘッダー
                    writer.WriteLine($"[{section.SectionName}]");

                    // キー=値のペア（空白なし）
                    foreach (var key in section.Keys)
                    {
                        // 安全性のため、keyから改行と"="を削除、valueから改行を削除
                        var safeKey = SanitizeIniKey(key.KeyName);
                        var safeValue = SanitizeIniValue(key.Value);
                        writer.WriteLine($"{safeKey}={safeValue}");
                    }

                    // セクション間の空行
                    writer.WriteLine();
                }
            }
        }

        /// <summary>
        /// INIキー名をサニタイズ（改行と"="を削除）
        /// </summary>
        private string SanitizeIniKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                return "";

            return key.Replace("\r", "").Replace("\n", "").Replace("=", "");
        }

        /// <summary>
        /// INI値をサニタイズ（改行を削除）
        /// </summary>
        private string SanitizeIniValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            return value.Replace("\r", "").Replace("\n", "");
        }
    }
}
