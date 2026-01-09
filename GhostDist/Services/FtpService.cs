using System;
using System.IO;
using System.Net.Sockets;
using System.Windows.Forms;
using FluentFTP;
using FluentFTP.Exceptions;
using GhostDist.Models;

namespace GhostDist.Services
{
    /// <summary>
    /// FTP操作のイベント引数
    /// </summary>
    public class FtpProgressEventArgs : EventArgs
    {
        public long BytesProcessed { get; set; }
        public long TotalBytes { get; set; }
        public string FileName { get; set; }
        public int Percent { get; set; }
    }

    /// <summary>
    /// FTP操作サービス (FluentFTP使用)
    /// 元のDelphiコードのTIdFTPと同様に接続を維持して操作を行う
    /// </summary>
    public class FtpService : IDisposable
    {
        private FtpClient _client;
        private FtpConfiguration _config;
        private string _currentDirectory;
        private bool _disposed;

        public event EventHandler<FtpProgressEventArgs> ProgressChanged;
        public event EventHandler<string> LogMessage;

        /// <summary>
        /// FTP接続設定を設定
        /// </summary>
        public void SetConfiguration(FtpConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// FTPサーバーに接続
        /// </summary>
        public void Connect()
        {
            if (_config == null)
                throw new InvalidOperationException("FTP設定が設定されていません。");

            // 既存の接続があれば切断
            Disconnect();

            // クライアント作成
            _client = new FtpClient(_config.Server);
            _client.Credentials = new System.Net.NetworkCredential(_config.UserId, _config.Password);

            // 転送モード設定
            _client.Config.DataConnectionType = _config.Passive
                ? FtpDataConnectionType.AutoPassive
                : FtpDataConnectionType.AutoActive;

            // SSL/TLS設定
            if (_config.UseSSL)
            {
                _client.Config.EncryptionMode = FtpEncryptionMode.Explicit;
                // 自己署名証明書を許可（元のDelphiコードと同様）
                _client.ValidateCertificate += (control, e) => { e.Accept = true; };
            }
            else
            {
                _client.Config.EncryptionMode = FtpEncryptionMode.None;
            }

            // バイナリ転送モード
            _client.Config.DownloadDataType = FtpDataType.Binary;
            _client.Config.UploadDataType = FtpDataType.Binary;

            // タイムアウト設定
            _client.Config.ConnectTimeout = 30000;
            _client.Config.ReadTimeout = 60000;
            _client.Config.DataConnectionConnectTimeout = 30000;
            _client.Config.DataConnectionReadTimeout = 60000;

            // 切断時のQUITコマンドを無効化（ハング防止）
            _client.Config.DisconnectWithQuit = false;

            // 接続
            _client.Connect();
            OnLogMessage($"{_config.Server}へ接続しました。");
        }

        /// <summary>
        /// FTPサーバーから切断
        /// </summary>
        public void Disconnect()
        {
            if (_client != null)
            {
                try
                {
                    if (_client.IsConnected)
                    {
                        // タイムアウトを設定して切断を試みる
                        var disconnectTask = System.Threading.Tasks.Task.Run(() => _client.Disconnect());
                        if (!disconnectTask.Wait(TimeSpan.FromSeconds(5)))
                        {
                            OnLogMessage("FTP切断がタイムアウトしました。強制的に接続を閉じます。");
                        }
                        else
                        {
                            OnLogMessage("FTP接続を切断しました。");
                        }
                    }

                    // Disposeにもタイムアウトを設定
                    var disposeTask = System.Threading.Tasks.Task.Run(() => _client.Dispose());
                    if (!disposeTask.Wait(TimeSpan.FromSeconds(3)))
                    {
                        OnLogMessage("FTPクライアントのDispose処理がタイムアウトしました。");
                    }
                }
                catch (Exception ex)
                {
                    OnLogMessage($"FTP切断中にエラーが発生しました: {ex.Message}");
                }

                _client = null;
                _currentDirectory = null;
            }
        }

        /// <summary>
        /// 接続状態を確認
        /// </summary>
        public bool IsConnected => _client?.IsConnected ?? false;

        /// <summary>
        /// 作業ディレクトリを変更
        /// </summary>
        public void ChangeDirectory(string remotePath)
        {
            EnsureConnected();

            _client.SetWorkingDirectory(remotePath);
            _currentDirectory = remotePath;
        }

        /// <summary>
        /// 一つ上のディレクトリに移動
        /// </summary>
        public void ChangeDirectoryUp()
        {
            EnsureConnected();

            _client.SetWorkingDirectory("..");
            _currentDirectory = _client.GetWorkingDirectory();
        }

        /// <summary>
        /// ディレクトリを作成
        /// </summary>
        public void CreateDirectory(string remotePath)
        {
            EnsureConnected();

            try
            {
                _client.CreateDirectory(remotePath);
                OnLogMessage($"ディレクトリ作成: {remotePath}");
            }
            catch (FtpCommandException ex) when (ex.CompletionCode == "550")
            {
                // ディレクトリが既に存在する場合は無視
                OnLogMessage($"ディレクトリは既に存在します: {remotePath}");
            }
        }

        /// <summary>
        /// ディレクトリが存在するかチェック
        /// </summary>
        public bool DirectoryExists(string remotePath)
        {
            EnsureConnected();

            return _client.DirectoryExists(remotePath);
        }

        /// <summary>
        /// ディレクトリパスを階層的に作成して移動
        /// 元のDelphiコードのDirCheck関数と同様の動作
        /// </summary>
        public void EnsureDirectoryAndChange(string relativePath)
        {
            EnsureConnected();

            if (string.IsNullOrEmpty(relativePath))
                return;

            // パスを分割してディレクトリを順に確認・作成・移動
            var parts = relativePath.Replace('/', '\\').Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in parts)
            {
                try
                {
                    _client.SetWorkingDirectory(part);
                }
                catch
                {
                    try
                    {
                        OnLogMessage($"ディレクトリ{part}を作成します。");
                        _client.CreateDirectory(part);
                    }
                    catch
                    {
                        // 作成失敗は無視
                    }
                    _client.SetWorkingDirectory(part);
                }
            }

            // 元のディレクトリに戻る
            for (int i = 0; i < parts.Length; i++)
            {
                _client.SetWorkingDirectory("..");
            }
        }

        /// <summary>
        /// ディレクトリパスを階層的に作成（移動なし）
        /// </summary>
        public void EnsureDirectoryExists(string remotePath)
        {
            EnsureConnected();

            if (string.IsNullOrEmpty(remotePath) || remotePath == "/")
                return;

            // FluentFTPのCreateDirectoryはrecursive=trueで再帰的に作成可能
            if (!_client.DirectoryExists(remotePath))
            {
                _client.CreateDirectory(remotePath, true);
                OnLogMessage($"ディレクトリを作成しました: {remotePath}");
            }
        }

        /// <summary>
        /// ファイルをアップロード
        /// </summary>
        public void UploadFile(string localPath, string remotePath)
        {
            EnsureConnected();

            if (!File.Exists(localPath))
                throw new FileNotFoundException($"ローカルファイルが見つかりません: {localPath}");

            var fileInfo = new FileInfo(localPath);
            var totalBytes = fileInfo.Length;
            var fileName = Path.GetFileName(localPath);

            // 進捗コールバック
            Action<FtpProgress> progress = (p) =>
            {
                OnProgressChanged(new FtpProgressEventArgs
                {
                    BytesProcessed = p.TransferredBytes,
                    TotalBytes = totalBytes,
                    FileName = fileName,
                    Percent = (int)p.Progress
                });
                Application.DoEvents();
            };

            // リモートパスの正規化
            remotePath = remotePath.Replace('\\', '/');
            if (!remotePath.StartsWith("/"))
            {
                remotePath = "/" + remotePath;
            }

            // アップロード実行
            var status = _client.UploadFile(localPath, remotePath, FtpRemoteExists.Overwrite, true, FtpVerify.None, progress);

            if (status == FtpStatus.Success)
            {
                OnLogMessage($"アップロード完了: {fileName}");
            }
            else
            {
                throw new Exception($"アップロードに失敗しました: {fileName} (Status: {status})");
            }
        }

        /// <summary>
        /// ファイルをダウンロード
        /// </summary>
        public void DownloadFile(string remotePath, string localPath)
        {
            EnsureConnected();

            // リモートパスの正規化
            remotePath = remotePath.Replace('\\', '/');
            if (!remotePath.StartsWith("/"))
            {
                remotePath = "/" + remotePath;
            }

            var fileName = Path.GetFileName(remotePath);

            // ローカルディレクトリが存在しない場合は作成
            var localDir = Path.GetDirectoryName(localPath);
            if (!string.IsNullOrEmpty(localDir) && !Directory.Exists(localDir))
            {
                Directory.CreateDirectory(localDir);
            }

            // 進捗コールバック（FluentFTP 53.x対応）
            Action<FtpProgress> progress = (p) =>
            {
                OnProgressChanged(new FtpProgressEventArgs
                {
                    BytesProcessed = p.TransferredBytes,
                    TotalBytes = p.TransferredBytes > 0 ? (long)(p.TransferredBytes / (p.Progress / 100.0)) : 0,
                    FileName = fileName,
                    Percent = (int)p.Progress
                });
                Application.DoEvents();
            };

            // ダウンロード実行
            var status = _client.DownloadFile(localPath, remotePath, FtpLocalExists.Overwrite, FtpVerify.None, progress);

            if (status == FtpStatus.Success)
            {
                OnLogMessage($"ダウンロード完了: {fileName}");
            }
            else if (status == FtpStatus.Failed)
            {
                throw new Exception($"ダウンロードに失敗しました: {fileName}");
            }
        }

        /// <summary>
        /// ファイルが存在するかチェック
        /// </summary>
        public bool FileExists(string remotePath)
        {
            EnsureConnected();

            remotePath = remotePath.Replace('\\', '/');
            if (!remotePath.StartsWith("/"))
            {
                remotePath = "/" + remotePath;
            }

            return _client.FileExists(remotePath);
        }

        /// <summary>
        /// 接続状態を確認
        /// </summary>
        private void EnsureConnected()
        {
            if (_client == null || !_client.IsConnected)
            {
                throw new InvalidOperationException("FTPサーバーに接続されていません。");
            }
        }

        protected virtual void OnProgressChanged(FtpProgressEventArgs e)
        {
            ProgressChanged?.Invoke(this, e);
        }

        protected virtual void OnLogMessage(string message)
        {
            LogMessage?.Invoke(this, message);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Disconnect();
                }
                _disposed = true;
            }
        }

        ~FtpService()
        {
            Dispose(false);
        }
    }
}
