using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GhostDist.Models;
using IniParser;
using IniParser.Model;

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
        /// 一般設定を読み込み
        /// </summary>
        public void LoadGeneralSettings(out bool isLog, out bool noLogWindow)
        {
            isLog = false;
            noLogWindow = false;

            if (!File.Exists(_iniPath))
                return;

            try
            {
                var parser = new FileIniDataParser();
                var data = parser.ReadFile(_iniPath, Encoding.GetEncoding("Shift_JIS"));

                if (data.Sections.ContainsSection("General"))
                {
                    isLog = ParseBool(data["General"]["IsLog"]);
                    noLogWindow = ParseBool(data["General"]["NoLog"]);
                }
            }
            catch (Exception)
            {
                // エラー時はデフォルト値を返す
            }
        }

        /// <summary>
        /// 共通FTP設定を読み込み
        /// </summary>
        public FtpConfiguration LoadCommonFtp()
        {
            var config = new FtpConfiguration();

            if (!File.Exists(_iniPath))
                return config;

            try
            {
                var parser = new FileIniDataParser();
                var data = parser.ReadFile(_iniPath, Encoding.GetEncoding("Shift_JIS"));

                if (data.Sections.ContainsSection("FTP"))
                {
                    config.Server = data["FTP"]["Server"] ?? "";
                    config.UserId = data["FTP"]["ID"] ?? "";
                    config.Password = data["FTP"]["Password"] ?? "";
                    config.Passive = ParseBool(data["FTP"]["Passive"]);
                    config.UseSSL = ParseBool(data["FTP"]["SSL"]);
                }
            }
            catch (Exception)
            {
                // エラー時はデフォルト値を返す
            }

            return config;
        }

        /// <summary>
        /// プロジェクト設定リストを読み込み
        /// </summary>
        public List<ProjectSettings> LoadProjects()
        {
            var projects = new List<ProjectSettings>();

            if (!File.Exists(_iniPath))
                return projects;

            try
            {
                var parser = new FileIniDataParser();
                var data = parser.ReadFile(_iniPath, Encoding.GetEncoding("Shift_JIS"));

                if (!data.Sections.ContainsSection("General"))
                    return projects;

                var countStr = data["General"]["SettingsCount"];
                if (string.IsNullOrEmpty(countStr) || !int.TryParse(countStr, out int count))
                    return projects;

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
                            UseSSL = ParseBool(data[section]["SSL"])
                        }
                    };

                    projects.Add(project);
                }
            }
            catch (Exception)
            {
                // エラー時は空のリストを返す
            }

            return projects;
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
                }

                var parser = new FileIniDataParser();
                parser.WriteFile(_iniPath, data, Encoding.GetEncoding("Shift_JIS"));
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
    }
}
