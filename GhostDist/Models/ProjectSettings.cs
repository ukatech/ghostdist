using System;
using System.IO;

namespace GhostDist.Models
{
    /// <summary>
    /// プロジェクト設定
    /// </summary>
    public class ProjectSettings
    {
        /// <summary>
        /// プロジェクト名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 処理タイプ
        /// </summary>
        public SettingType Type { get; set; }

        /// <summary>
        /// FTPアップロード先ディレクトリ
        /// </summary>
        public string Directory { get; set; }

        /// <summary>
        /// 配布ページHTMLファイルのパス
        /// </summary>
        public string HtmlFile { get; set; }

        /// <summary>
        /// NAR/ZIPファイル名(%year等の変数を含む可能性あり)
        /// </summary>
        public string NarName { get; set; }

        /// <summary>
        /// 処理対象ファイル名条件(改行区切り)
        /// </summary>
        public string ProcessName { get; set; }

        /// <summary>
        /// 除外ファイル名条件(改行区切り)
        /// </summary>
        public string ExcludeName { get; set; }

        /// <summary>
        /// 基準フォルダ
        /// </summary>
        public string TargetFolder { get; set; }

        /// <summary>
        /// 起動時にチェックする
        /// </summary>
        public bool DefaultCheck { get; set; }

        /// <summary>
        /// プライベートFTP設定
        /// </summary>
        public FtpConfiguration PrivateFtp { get; set; }

        /// <summary>
        /// 共通FTP設定を使用するか
        /// </summary>
        public bool UseCommonFtp { get; set; }

        public ProjectSettings()
        {
            Name = "";
            Type = SettingType.Network;
            Directory = "/";
            HtmlFile = "";
            NarName = "";
            ProcessName = GetDefaultProcessPatterns();
            ExcludeName = GetDefaultExcludePatterns();
            TargetFolder = "";
            DefaultCheck = true;
            PrivateFtp = new FtpConfiguration();
            UseCommonFtp = false;
        }

        /// <summary>
        /// デフォルトの処理対象パターン
        /// </summary>
        private string GetDefaultProcessPatterns()
        {
            return string.Join(Environment.NewLine, new[]
            {
                "delete.txt",
                "install.txt",
                "readme.*",
                "thumbnail*.*",
                "developer_options.txt",
                "ghost/master/*.txt",
                "ghost/master/*.dll",
                "ghost/master/*.exe",
                "ghost/master/*.ico",
                "ghost/master/*.dic",
                "ghost/master/*.ayc",
                "ghost/master/*.kis",
                "ghost/master/*.kaw",
                "ghost/master/*.sat",
                "ghost/master/*.png",
                "ghost/master/*.pnr",
                "ghost/master/*.jpg",
                "ghost/master/*.jpe",
                "ghost/master/*.jpeg",
                "ghost/master/saori/*.dll",
                "ghost/master/saori/*.exe",
                "ghost/master/openkeeps/*.kis",
                "ghost/master/openkeeps/plugin/*.kis",
                "ghost/master/template/*.kis",
                "ghost/master/template/another/*.kis",
                "ghost/master/system/*.dic",
                "ghost/master/system/*.txt",
                "shell/master/*.txt",
                "shell/master/*.dll",
                "shell/master/*.ico",
                "shell/master/*.png",
                "shell/master/*.pna",
                "shell/master/*.pnr"
            });
        }

        /// <summary>
        /// デフォルトの除外パターン
        /// </summary>
        private string GetDefaultExcludePatterns()
        {
            return string.Join(Environment.NewLine, new[]
            {
                "ngm.dat",
                "profile.txt",
                "updates.txt",
                "ghost/master/satori_savedata.txt",
                "ghost/master/satori_savebackup.txt",
                "ghost/master/satorite.txt",
                "ghost/master/satorite.exe",
                "ghost/master/tama.exe",
                "ghost/master/nsconf.txt",
                "ghost/master/Dict-KEEPSs.txt",
                "ghost/master/Dict-Learned.txt",
                "ghost/master/misaka_vars.txt",
                "ghost/master/aya_variable.*",
                "ghost/master/aya5_variable.*",
                "ghost/master/yaya_variable.*",
                "ghost/master/ssp_shiori_log.txt",
                "ghost/master/updates.txt",
                "ghost/master/れしば.txt",
                "ghost/master/satorite.txt",
                "ghost/master/*.log"
            });
        }

        /// <summary>
        /// TargetFolderをフルパス化して末尾にバックスラッシュを付与
        /// </summary>
        public string GetExpandedTargetFolder()
        {
            if (string.IsNullOrEmpty(TargetFolder))
                return "";

            var expanded = Path.GetFullPath(TargetFolder);
            return Path.GetDirectoryName(expanded) + Path.DirectorySeparatorChar + Path.GetFileName(expanded) + Path.DirectorySeparatorChar;
        }

        /// <summary>
        /// NarNameをフルパス化(%変数はそのまま)
        /// </summary>
        public string GetExpandedNarName()
        {
            if (string.IsNullOrEmpty(NarName))
                return "";

            return Path.GetFullPath(NarName);
        }
    }
}
