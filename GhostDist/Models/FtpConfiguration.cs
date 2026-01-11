using System;

namespace GhostDist.Models
{
    /// <summary>
    /// FTP接続設定（アップロード設定全般を含む）
    /// </summary>
    public class FtpConfiguration
    {
        /// <summary>
        /// FTPサーバーアドレス
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// ユーザーID
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// パスワード
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// パッシブモードを使用するか
        /// </summary>
        public bool Passive { get; set; }

        /// <summary>
        /// SSL/TLSを使用するか(FTPS)
        /// </summary>
        public bool UseSSL { get; set; }

        /// <summary>
        /// アップロード先の種類 (Upload モード時のみ有効)
        /// </summary>
        public UploadType UploadDestinationType { get; set; }

        /// <summary>
        /// ななろだのアップロードURL
        /// </summary>
        public string NarNaLoaderUploadUrl { get; set; }

        public FtpConfiguration()
        {
            Server = "";
            UserId = "";
            Password = "";
            Passive = false;
            UseSSL = false;

            // ななろだ関連のデフォルト値
            UploadDestinationType = UploadType.FTP;
            NarNaLoaderUploadUrl = "";
        }
    }
}
