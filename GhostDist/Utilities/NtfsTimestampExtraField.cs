using System;
using System.IO;

namespace GhostDist.Utilities
{
    /// <summary>
    /// NTFS Timestamp Extra Field (0x000a) を生成するユーティリティ
    /// ZIPファイルにNTFSの高精度タイムスタンプ（100ナノ秒単位）を保存
    /// </summary>
    public static class NtfsTimestampExtraField
    {
        private const ushort HEADER_ID = 0x000a;
        private const ushort RESERVED = 0x0000;
        private const ushort TAG_TIMESTAMP = 0x0001;

        /// <summary>
        /// NTFS Timestamp Extra Field のバイナリデータを生成
        /// </summary>
        /// <param name="modifyTime">最終更新日時</param>
        /// <param name="accessTime">最終アクセス日時（nullの場合はmodifyTimeと同じ）</param>
        /// <param name="createTime">作成日時（nullの場合はmodifyTimeと同じ）</param>
        /// <returns>0x000a Extra Field のバイナリデータ</returns>
        public static byte[] Create(DateTime modifyTime, DateTime? accessTime = null, DateTime? createTime = null)
        {
            // NTFSファイル時刻（1601年1月1日からの100ナノ秒単位）に変換
            var mtime = ToNtfsTime(modifyTime);
            var atime = ToNtfsTime(accessTime ?? modifyTime);
            var ctime = ToNtfsTime(createTime ?? modifyTime);

            // データサイズ = Reserved(4) + Tag(2) + TagSize(2) + Mtime(8) + Atime(8) + Ctime(8)
            ushort dataSize = 4 + 2 + 2 + 8 + 8 + 8; // 32 bytes
            ushort tagSize = 8 + 8 + 8; // 24 bytes (3つのタイムスタンプ)

            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                writer.Write(HEADER_ID);        // 2 bytes (Little-Endian)
                writer.Write(dataSize);         // 2 bytes (Little-Endian)
                writer.Write(RESERVED);         // 4 bytes (Reserved, always 0)
                writer.Write(RESERVED);
                writer.Write(TAG_TIMESTAMP);    // 2 bytes (Tag ID = 1)
                writer.Write(tagSize);          // 2 bytes (Tag Size = 24)
                writer.Write(mtime);            // 8 bytes (Modify Time)
                writer.Write(atime);            // 8 bytes (Access Time)
                writer.Write(ctime);            // 8 bytes (Create Time)

                return ms.ToArray();
            }
        }

        /// <summary>
        /// DateTimeをNTFSファイル時刻（1601年1月1日からの100ナノ秒単位）に変換
        /// </summary>
        private static long ToNtfsTime(DateTime dateTime)
        {
            // DateTimeをFileTimeに変換（UTC基準）
            var fileTime = dateTime.ToFileTimeUtc();
            return fileTime;
        }
    }
}
