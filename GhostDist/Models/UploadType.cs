namespace GhostDist.Models
{
    /// <summary>
    /// アップロード先の種類 (Upload モード時のみ使用)
    /// </summary>
    public enum UploadType
    {
        /// <summary>
        /// FTPアップロード (デフォルト)
        /// </summary>
        FTP,

        /// <summary>
        /// ななろだ (NarNaLoader) アップロード
        /// </summary>
        NarNaLoader
    }
}
