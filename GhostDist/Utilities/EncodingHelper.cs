using System.Text;

namespace GhostDist.Utilities
{
    /// <summary>
    /// エンコーディング関連のヘルパークラス
    /// </summary>
    public static class EncodingHelper
    {
        private static readonly Encoding ShiftJIS = Encoding.GetEncoding("Shift_JIS");

        /// <summary>
        /// 文字列がShift_JISで可逆的にエンコード可能かチェック
        /// </summary>
        /// <param name="text">チェック対象の文字列</param>
        /// <returns>Shift_JISで可逆的にエンコード可能な場合true、不可能な場合false</returns>
        public static bool CanEncodeInShiftJIS(string text)
        {
            try
            {
                // Shift_JISでエンコード→デコードして元に戻るか確認
                var bytes = ShiftJIS.GetBytes(text);
                var decoded = ShiftJIS.GetString(bytes);

                // 完全一致しない場合はShift_JISで表現不可能
                return text == decoded;
            }
            catch
            {
                // エンコード失敗 = Shift_JISで表現不可能
                return false;
            }
        }

        /// <summary>
        /// 文字列がShift_JISで表現できない文字を含むかチェック（Unicode対応が必要かの判定）
        /// </summary>
        /// <param name="text">チェック対象の文字列</param>
        /// <returns>Unicode対応が必要な場合true、不要な場合false</returns>
        public static bool RequiresUnicodeSupport(string text)
        {
            return !CanEncodeInShiftJIS(text);
        }

        /// <summary>
        /// Shift_JISエンコーディングを取得
        /// </summary>
        /// <returns>Shift_JISエンコーディング</returns>
        public static Encoding GetShiftJISEncoding()
        {
            return ShiftJIS;
        }
    }
}
