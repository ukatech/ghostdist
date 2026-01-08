using System;

namespace GhostDist.Models
{
    /// <summary>
    /// プロジェクトの処理タイプ
    /// </summary>
    public enum SettingType
    {
        /// <summary>
        /// ネットワーク更新ファイル生成+FTPアップロード
        /// </summary>
        Network,

        /// <summary>
        /// NAR(ZIP)作成+FTPアップロード+HTML更新
        /// </summary>
        Upload,

        /// <summary>
        /// NAR(ZIP)作成のみ
        /// </summary>
        NarCreate
    }
}
