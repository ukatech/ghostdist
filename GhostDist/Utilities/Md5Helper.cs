using System;
using System.IO;
using System.Security.Cryptography;

namespace GhostDist.Utilities
{
    /// <summary>
    /// MD5ハッシュ計算のヘルパークラス
    /// </summary>
    public static class Md5Helper
    {
        /// <summary>
        /// ファイルのMD5ハッシュを計算
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>MD5ハッシュ(小文字16進数)</returns>
        public static string CalculateFileMD5(string filePath)
        {
            if (!File.Exists(filePath))
                return "";

            using (var md5 = MD5.Create())
            using (var stream = File.OpenRead(filePath))
            {
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
