using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Installer.Core.Common
{
    /// <summary>SHA-256 工具。</summary>
    public static class Hashing
    {
        private static readonly string[] HexLookup = BuildHexLookup();

        private static string[] BuildHexLookup()
        {
            var t = new string[256];
            for (var i = 0; i < 256; i++)
            {
                t[i] = i.ToString("x2");
            }

            return t;
        }

        /// <summary>把字节数组转成小写十六进制字符串。</summary>
        public static string ToHex(byte[] bytes)
        {
            if (bytes == null)
            {
                return null;
            }

            var sb = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes)
            {
                sb.Append(HexLookup[b]);
            }

            return sb.ToString();
        }

        /// <summary>计算字节数组的 SHA-256（小写十六进制）。</summary>
        public static string ComputeBytes(byte[] data)
        {
            return ToHex(Sha256Raw(data));
        }

        /// <summary>计算字节数组的 SHA-256 原始 32 字节。</summary>
        public static byte[] Sha256Raw(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            using (var sha = SHA256.Create())
            {
                return sha.ComputeHash(data);
            }
        }

        /// <summary>流式计算一个文件的 SHA-256（小写十六进制）。内存占用恒定。</summary>
        public static string ComputeFile(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 20))
            {
                return ComputeStream(fs);
            }
        }

        /// <summary>流式计算流的 SHA-256（从当前位置读到末尾）。</summary>
        public static string ComputeStream(Stream stream)
        {
            using (var sha = SHA256.Create())
            {
                var buffer = new byte[1 << 20];
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    sha.TransformBlock(buffer, 0, read, null, 0);
                }

                sha.TransformFinalBlock(new byte[0], 0, 0);
                return ToHex(sha.Hash);
            }
        }

        /// <summary>流式计算可定位流中指定区间的 SHA-256（小写十六进制）。</summary>
        public static string ComputeRegion(Stream seekable, long offset, long length, Action<long, long> progress = null)
        {
            if (seekable == null)
            {
                throw new ArgumentNullException("seekable");
            }

            seekable.Seek(offset, SeekOrigin.Begin);
            using (var sha = SHA256.Create())
            {
                var buffer = new byte[1 << 20];
                long done = 0;
                while (done < length)
                {
                    var want = (int)Math.Min(buffer.Length, length - done);
                    var read = seekable.Read(buffer, 0, want);
                    if (read <= 0)
                    {
                        throw new EndOfStreamException("计算哈希时读到文件末尾。");
                    }

                    sha.TransformBlock(buffer, 0, read, null, 0);
                    done += read;
                    if (progress != null)
                    {
                        progress(done, length);
                    }
                }

                sha.TransformFinalBlock(new byte[0], 0, 0);
                return ToHex(sha.Hash);
            }
        }

        /// <summary>流式计算流中指定长度的 SHA-256 原始字节。</summary>
        public static byte[] ComputeRegionBytes(Stream seekable, long offset, long length)
        {
            seekable.Seek(offset, SeekOrigin.Begin);
            using (var sha = SHA256.Create())
            {
                var buffer = new byte[1 << 20];
                long done = 0;
                while (done < length)
                {
                    var want = (int)Math.Min(buffer.Length, length - done);
                    var read = seekable.Read(buffer, 0, want);
                    if (read <= 0)
                    {
                        throw new EndOfStreamException("计算哈希时读到文件末尾。");
                    }

                    sha.TransformBlock(buffer, 0, read, null, 0);
                    done += read;
                }

                sha.TransformFinalBlock(new byte[0], 0, 0);
                return sha.Hash;
            }
        }
    }
}
