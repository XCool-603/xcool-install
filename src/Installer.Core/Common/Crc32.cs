using System;
using System.IO;

namespace Installer.Core.Common
{
    /// <summary>标准 IEEE CRC-32（反射多项式 0xEDB88320）。用于 footer 自身的完整性校验。</summary>
    public static class Crc32
    {
        private static readonly uint[] Table = BuildTable();

        private static uint[] BuildTable()
        {
            var table = new uint[256];
            for (uint i = 0; i < 256; i++)
            {
                var c = i;
                for (var k = 0; k < 8; k++)
                {
                    c = (c & 1) != 0 ? 0xEDB88320u ^ (c >> 1) : c >> 1;
                }

                table[i] = c;
            }

            return table;
        }

        /// <summary>计算字节数组指定区间的 CRC-32。</summary>
        public static uint Compute(byte[] data, int offset, int count)
        {
            var crc = 0xFFFFFFFFu;
            for (var i = offset; i < offset + count; i++)
            {
                crc = Table[(crc ^ data[i]) & 0xFF] ^ (crc >> 8);
            }

            return crc ^ 0xFFFFFFFFu;
        }

        /// <summary>计算可定位流中指定区间的 CRC-32。计算结束后流位置未定义。</summary>
        public static uint Compute(Stream stream, long offset, long count)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            stream.Seek(offset, SeekOrigin.Begin);
            var buffer = new byte[64 * 1024];
            var crc = 0xFFFFFFFFu;
            var remaining = count;
            while (remaining > 0)
            {
                var want = (int)Math.Min(buffer.Length, remaining);
                var read = stream.Read(buffer, 0, want);
                if (read <= 0)
                {
                    throw new EndOfStreamException("计算 CRC 时读到文件末尾（期望 " + count + " 字节）。");
                }

                for (var i = 0; i < read; i++)
                {
                    crc = Table[(crc ^ buffer[i]) & 0xFF] ^ (crc >> 8);
                }

                remaining -= read;
            }

            return crc ^ 0xFFFFFFFFu;
        }
    }
}
