using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using GhostDist.Models;

namespace GhostDist.Services
{
    /// <summary>
    /// ネットワーク更新サービス
    /// 元のDelphiコードのTSettings.RunAsNetwork/RunAsNarUpと同様の動作を行う
    /// </summary>
    public class NetworkUpdateService
    {
        public event EventHandler<string> LogMessage;
        public event EventHandler<FtpProgressEventArgs> ProgressChanged;

        /// <summary>
        /// ネットワーク更新を実行
        /// 元のDelphiコードのRunAsNetwork関数に相当
        /// </summary>
        public bool Execute(ProjectSettings settings, FtpConfiguration ftpConfig)
        {
            OnLogMessage("ネットワーク更新を開始します。");

            // ローカルファイルリスト生成
            var localUpdates = new Updates2DauManager
            {
                TargetFolder = settings.TargetFolder
            };

            OnLogMessage("ファイルリストを生成します。");
            if (!localUpdates.Make(settings.ProcessName, settings.ExcludeName))
            {
                OnLogMessage("delete.txtに問題となる可能性のある記述を検出しました。");
                OnLogMessage(localUpdates.DeleteTxtError);

                var result = MessageBox.Show(
                    "delete.txtに問題となる可能性のある記述を検出しました。\n続行しますか?",
                    "警告",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.No)
                {
                    return false;
                }
            }

            OnLogMessage(localUpdates.GetFileList());
            OnLogMessage("ファイルリストを生成しました。");

            // 一時ファイルパス
            var targetFolder = Path.GetFullPath(settings.TargetFolder).TrimEnd('\\') + "\\";
            var tempDauPath = Path.Combine(targetFolder, "updates2.dau");
            var diffFiles = new List<FileElement>();

            // FTPサービス作成・接続
            using (var ftpService = new FtpService())
            {
                ftpService.LogMessage += (s, msg) => OnLogMessage(msg);
                ftpService.ProgressChanged += (s, e) => OnProgressChanged(e);
                ftpService.SetConfiguration(ftpConfig);

                try
                {
                    // FTP接続
                    OnLogMessage($"{ftpConfig.Server}へ接続します。");
                    ftpService.Connect();

                    // リモートのupdates2.dauを取得
                    var remoteUpdates = new Updates2DauManager();
                    var remoteDir = NormalizePath(settings.Directory);

                    try
                    {
                        // ターゲットディレクトリが存在するか確認・移動
                        try
                        {
                            ftpService.ChangeDirectory(remoteDir);
                        }
                        catch
                        {
                            // ターゲットディレクトリを作成
                            OnLogMessage("ターゲットディレクトリが見つかりません。作成します。");
                            ftpService.EnsureDirectoryExists(remoteDir);
                            ftpService.ChangeDirectory(remoteDir);
                        }

                        // リモートのupdates2.dauを取得
                        if (File.Exists(tempDauPath))
                        {
                            File.Delete(tempDauPath);
                        }

                        OnLogMessage("updates2.dauを取得します。");
                        try
                        {
                            var remoteDauPath = CombineRemotePath(remoteDir, "updates2.dau");
                            ftpService.DownloadFile(remoteDauPath, tempDauPath);
                        }
                        catch
                        {
                            OnLogMessage("リモートのupdates2.dauの取得に失敗しました");
                        }

                        try
                        {
                            remoteUpdates.LoadFromFile(tempDauPath);
                        }
                        catch
                        {
                            OnLogMessage("リモートのupdates2.dauのロードに失敗しました");
                        }

                        // MD5比較
                        OnLogMessage("MD5を照合します。");
                        foreach (var localFile in localUpdates.Files)
                        {
                            var remoteFile = remoteUpdates.Find(localFile.Name);

                            if (remoteFile == null)
                            {
                                OnLogMessage($"{localFile.Name}はリモートサーバ上に存在しません。アップロード対象とします。");
                                diffFiles.Add(localFile);
                            }
                            else if (!remoteFile.MD5.Equals(localFile.MD5, StringComparison.OrdinalIgnoreCase))
                            {
                                OnLogMessage($"{localFile.Name}は更新されています。アップロード対象とします。");
                                diffFiles.Add(localFile);
                            }
                        }
                    }
                    catch
                    {
                        // ディレクトリがない場合は全ファイルをアップロード
                        OnLogMessage("ターゲットディレクトリが見つかりません。作成します。");
                        ftpService.EnsureDirectoryExists(remoteDir);
                        ftpService.ChangeDirectory(remoteDir);

                        foreach (var file in localUpdates.Files)
                        {
                            diffFiles.Add(file);
                        }
                    }

                    // 更新ファイルをアップロード
                    if (diffFiles.Count > 0)
                    {
                        foreach (var file in diffFiles)
                        {
                            // ディレクトリチェック
                            var fileDir = Path.GetDirectoryName(file.Name.Replace('/', '\\'));
                            if (!string.IsNullOrEmpty(fileDir))
                            {
                                ftpService.EnsureDirectoryAndChange(fileDir);
                            }

                            // ファイルアップロード
                            var remoteFilePath = CombineRemotePath(remoteDir, file.Name);
                            OnLogMessage($"{file.Name}をアップロードします。");
                            ftpService.UploadFile(file.LocalPath, remoteFilePath);

                            Application.DoEvents();
                        }

                        // 新しいupdates2.dauをアップロード
                        if (File.Exists(tempDauPath))
                        {
                            File.Delete(tempDauPath);
                        }

                        localUpdates.SaveToFile(tempDauPath);
                        OnLogMessage("updates2.dauをアップロードします。");
                        var remoteDauPathFinal = CombineRemotePath(remoteDir, "updates2.dau");
                        ftpService.UploadFile(tempDauPath, remoteDauPathFinal);
                    }
                    else
                    {
                        OnLogMessage("更新されたファイルはありません。");
                    }

                    // 一時ファイル削除
                    if (File.Exists(tempDauPath))
                    {
                        File.Delete(tempDauPath);
                    }

                    OnLogMessage("ネットワーク更新が完了しました。");
                    return true;
                }
                catch (Exception ex)
                {
                    OnLogMessage($"FTP接続が出来ませんでした。転送は行われません: {ex.Message}");
                    return false;
                }
            }
        }

        /// <summary>
        /// NAR作成+アップロードを実行
        /// 元のDelphiコードのRunAsNarUp関数に相当
        /// </summary>
        public bool ExecuteNarUpload(ProjectSettings settings, FtpConfiguration ftpConfig)
        {
            OnLogMessage("NAR作成+アップロードを開始します。");

            // NAR作成
            var narService = new NarCreationService();
            narService.LogMessage += (s, msg) => OnLogMessage(msg);

            string narPath;
            try
            {
                narPath = narService.CreateNar(settings);
            }
            catch (Exception ex)
            {
                OnLogMessage($"NAR作成に失敗しました: {ex.Message}");
                return false;
            }

            var targetFolder = Path.GetFullPath(settings.TargetFolder).TrimEnd('\\') + "\\";

            // FTPサービス作成・接続
            using (var ftpService = new FtpService())
            {
                ftpService.LogMessage += (s, msg) => OnLogMessage(msg);
                ftpService.ProgressChanged += (s, e) => OnProgressChanged(e);
                ftpService.SetConfiguration(ftpConfig);

                try
                {
                    // FTP接続
                    OnLogMessage($"{ftpConfig.Server}へ接続します。");
                    ftpService.Connect();

                    var remoteDir = NormalizePath(settings.Directory);

                    // ターゲットディレクトリ確認・移動
                    try
                    {
                        ftpService.ChangeDirectory(remoteDir);
                    }
                    catch
                    {
                        OnLogMessage("ターゲットディレクトリが見つかりません。作成します。");
                        ftpService.EnsureDirectoryExists(remoteDir);
                        ftpService.ChangeDirectory(remoteDir);
                    }

                    // NARアップロード
                    var narFileName = Path.GetFileName(narPath);
                    var remoteNarPath = CombineRemotePath(remoteDir, narFileName);
                    OnLogMessage($"{narFileName}をアップロードします。");
                    ftpService.UploadFile(narPath, remoteNarPath);
                    OnLogMessage($"{narFileName}をアップロードしました。");

                    // ファイルサイズ取得
                    var fileInfo = new FileInfo(narPath);
                    var arcSize = fileInfo.Length;

                    // NARファイル削除
                    File.Delete(narPath);

                    // HTML書き換えとアップロード
                    if (!string.IsNullOrEmpty(settings.HtmlFile) && File.Exists(settings.HtmlFile))
                    {
                        var htmlContent = File.ReadAllText(settings.HtmlFile, Encoding.GetEncoding("Shift_JIS"));

                        // 変数置換（元のDelphiコードと同様の形式）
                        htmlContent = htmlContent.Replace("%uploaddate", DateTime.Now.ToString("yyyy/MM/dd"));
                        htmlContent = htmlContent.Replace("%uploadtime", DateTime.Now.ToString("HH:mm:ss"));
                        htmlContent = htmlContent.Replace("%uploadsize", (arcSize / 1024) + " KB");

                        // 一時ファイルに保存
                        var tempHtmlPath = Path.Combine(targetFolder, Path.GetFileName(settings.HtmlFile));
                        File.WriteAllText(tempHtmlPath, htmlContent, Encoding.GetEncoding("Shift_JIS"));

                        // HTMLアップロード
                        var htmlFileName = Path.GetFileName(settings.HtmlFile);
                        var remoteHtmlPath = CombineRemotePath(remoteDir, htmlFileName);
                        OnLogMessage($"{htmlFileName}をアップロードします。");
                        ftpService.UploadFile(tempHtmlPath, remoteHtmlPath);
                        OnLogMessage($"{htmlFileName}をアップロードしました。");

                        // 一時ファイル削除
                        File.Delete(tempHtmlPath);
                    }

                    OnLogMessage("NAR作成+アップロードが完了しました。");
                    return true;
                }
                catch (Exception ex)
                {
                    OnLogMessage($"FTP接続が出来ませんでした。転送は行われません: {ex.Message}");
                    return false;
                }
            }
        }

        /// <summary>
        /// リモートパスを正規化
        /// </summary>
        private string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return "/";

            path = path.Replace('\\', '/');
            if (!path.StartsWith("/"))
                path = "/" + path;

            return path.TrimEnd('/');
        }

        /// <summary>
        /// リモートパスを結合
        /// </summary>
        private string CombineRemotePath(string basePath, string relativePath)
        {
            basePath = NormalizePath(basePath);
            relativePath = relativePath.Replace('\\', '/').TrimStart('/');
            return basePath + "/" + relativePath;
        }

        protected virtual void OnLogMessage(string message)
        {
            LogMessage?.Invoke(this, message);
        }

        protected virtual void OnProgressChanged(FtpProgressEventArgs e)
        {
            ProgressChanged?.Invoke(this, e);
        }
    }
}
