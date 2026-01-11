using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GhostDist.Services
{
    /// <summary>
    /// ななろだアップロード結果
    /// </summary>
    public class NarNaLoaderUploadResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    /// <summary>
    /// ななろだ (NarNaLoader) アップロードサービス
    /// Satolist2のNarNaLoaderClient.csを参考に実装
    /// </summary>
    public class NarNaLoaderUploadService : IDisposable
    {
        private HttpClient _httpClient;
        private bool _disposed;

        public event EventHandler<string> LogMessage;
        public event EventHandler<FtpProgressEventArgs> ProgressChanged;

        public NarNaLoaderUploadService()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromMinutes(10); // 大きいNARファイル対応
        }

        /// <summary>
        /// NARファイルをななろだにアップロード
        /// </summary>
        /// <param name="narFilePath">NARファイルのフルパス</param>
        /// <param name="ghostId">ななろだのアイテムID</param>
        /// <param name="uploadUrl">アップロード先URL</param>
        /// <param name="userId">ユーザーID (FtpConfiguration.UserId)</param>
        /// <param name="password">パスワード (FtpConfiguration.Password)</param>
        public async Task<NarNaLoaderUploadResult> UploadNarAsync(
            string narFilePath,
            string ghostId,
            string uploadUrl,
            string userId,
            string password)
        {
            OnLogMessage($"ななろだへのアップロードを開始します。");
            OnLogMessage($"アップロード先: {uploadUrl}");
            OnLogMessage($"Ghost ID: {ghostId}");

            if (!File.Exists(narFilePath))
            {
                return new NarNaLoaderUploadResult
                {
                    Success = false,
                    Message = $"NARファイルが見つかりません: {narFilePath}"
                };
            }

            try
            {
                using (var content = new MultipartFormDataContent($"----WebKitFormBoundary{Guid.NewGuid():N}"))
                {
                    // フォームパラメータ追加
                    content.Add(new StringContent("upload"), "formtype");
                    content.Add(new StringContent(""), "admin");
                    content.Add(new StringContent(ghostId), "ghost_id");
                    content.Add(new StringContent(password), "password");
                    content.Add(new StringContent(userId), "id");

                    // NARファイル追加
                    var fileInfo = new FileInfo(narFilePath);
                    var totalBytes = fileInfo.Length;
                    OnLogMessage($"ファイルサイズ: {totalBytes / 1024} KB");

                    var fileStream = File.OpenRead(narFilePath);
                    var fileContent = new StreamContent(fileStream);
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                    content.Add(fileContent, "upfile", "upfile.nar");

                    // 進捗通知 (ファイル読み込み開始)
                    OnProgressChanged(new FtpProgressEventArgs
                    {
                        BytesProcessed = 0,
                        TotalBytes = totalBytes,
                        FileName = Path.GetFileName(narFilePath),
                        Percent = 0
                    });

                    // HTTPリクエスト送信
                    OnLogMessage($"{Path.GetFileName(narFilePath)} をアップロードしています...");
                    var response = await _httpClient.PostAsync(uploadUrl, content);

                    // 進捗通知 (完了)
                    OnProgressChanged(new FtpProgressEventArgs
                    {
                        BytesProcessed = totalBytes,
                        TotalBytes = totalBytes,
                        FileName = Path.GetFileName(narFilePath),
                        Percent = 100
                    });

                    // レスポンス処理
                    var responseBytes = await response.Content.ReadAsByteArrayAsync();
                    var responseText = Encoding.GetEncoding("Shift_JIS").GetString(responseBytes);

                    OnLogMessage($"サーバーからのレスポンス: {responseText}");

                    // レスポンス解析: <!--result: メッセージ--><!--resultcode: コード-->
                    var match = Regex.Match(responseText, @"<!--result:\s*(.*)-->\s*<!--resultcode:\s*(.*)-->");
                    if (match.Success)
                    {
                        var resultMessage = match.Groups[1].Value.Trim();
                        var resultCode = match.Groups[2].Value.Trim();

                        bool success = resultCode.Equals("success", StringComparison.OrdinalIgnoreCase);

                        if (success)
                        {
                            OnLogMessage($"アップロード成功: {resultMessage}");
                        }
                        else
                        {
                            OnLogMessage($"アップロード失敗: {resultMessage}");
                        }

                        return new NarNaLoaderUploadResult
                        {
                            Success = success,
                            Message = resultMessage
                        };
                    }
                    else
                    {
                        OnLogMessage("サーバーからの応答を解析できませんでした。");
                        return new NarNaLoaderUploadResult
                        {
                            Success = false,
                            Message = "レスポンス解析失敗: " + responseText
                        };
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                OnLogMessage($"HTTP接続エラー: {ex.Message}");
                return new NarNaLoaderUploadResult
                {
                    Success = false,
                    Message = $"HTTP接続エラー: {ex.Message}"
                };
            }
            catch (TaskCanceledException ex)
            {
                OnLogMessage($"タイムアウトエラー: {ex.Message}");
                return new NarNaLoaderUploadResult
                {
                    Success = false,
                    Message = "アップロードがタイムアウトしました"
                };
            }
            catch (Exception ex)
            {
                OnLogMessage($"予期しないエラー: {ex.Message}");
                return new NarNaLoaderUploadResult
                {
                    Success = false,
                    Message = $"エラー: {ex.Message}"
                };
            }
        }

        protected virtual void OnLogMessage(string message)
        {
            LogMessage?.Invoke(this, message);
        }

        protected virtual void OnProgressChanged(FtpProgressEventArgs e)
        {
            ProgressChanged?.Invoke(this, e);
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
                    _httpClient?.Dispose();
                }
                _disposed = true;
            }
        }
    }
}
