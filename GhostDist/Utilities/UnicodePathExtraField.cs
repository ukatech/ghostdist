using System;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Checksum;

namespace GhostDist.Utilities
{
    /// <summary>
    /// Info-ZIP Unicode Path Extra Field (0x7075) を生成するユーティリティ
    /// </summary>
    public static class UnicodePathExtraField
    {
        private const ushort HEADER_ID = 0x7075;
        private const byte VERSION = 1;

        /// <summary>
        /// Unicode Path Extra Field のバイナリデータを生成
        /// </summary>
        /// <param name="unicodeName">UTF-8エンコードするファイル名</param>
        /// <param name="shiftJisNameBytes">Shift_JISでエンコードされたファイル名バイト配列</param>
        /// <returns>0x7075 Extra Field のバイナリデータ</returns>
        public static byte[] Create(string unicodeName, byte[] shiftJisNameBytes)
        {
            // UTF-8エンコード（BOMなし）
            var utf8Bytes = Encoding.UTF8.GetBytes(unicodeName);

            // Shift_JISファイル名のCRC32計算
            var crc32 = CalculateCrc32(shiftJisNameBytes);

            // データサイズ = Version(1) + CRC32(4) + UTF-8名前(可変)
            ushort dataSize = (ushort)(1 + 4 + utf8Bytes.Length);

            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                writer.Write(HEADER_ID);        // 2 bytes (Little-Endian)
                writer.Write(dataSize);         // 2 bytes (Little-Endian)
                writer.Write(VERSION);          // 1 byte
                writer.Write(crc32);            // 4 bytes (Little-Endian)
                writer.Write(utf8Bytes);        // Variable

                return ms.ToArray();
            }
        }

        /// <summary>
        /// CRC32チェックサムを計算（ZIP標準のCRC32アルゴリズム）
        /// </summary>
        private static uint CalculateCrc32(byte[] data)
        {
            var crc = new Crc32();
            crc.Update(data);
            return (uint)crc.Value;
        }
    }
}
