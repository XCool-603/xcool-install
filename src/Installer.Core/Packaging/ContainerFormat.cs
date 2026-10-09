using System;
using System.IO;
using System.Text;
using Installer.Abstractions.Model;
using Installer.Abstractions.Packaging;
using Installer.Core.Common;

namespace Installer.Core.Packaging
{
    /// <summary>
    /// 容器格式的读写实现。
    ///
    /// 布局（顺序即文件中的顺序）：
    /// <code>
    /// [0 .. stubLength)              模板 exe（PE 结构，加载器忽略其后的数据）
    /// [payloadOffset .. +payloadLen) 压缩后的 payload（zip）
    /// [manifestOffset .. +manifestLen) manifest.json（UTF-8）
    /// [fileLength-128 .. fileLength) footer（索引 + 哈希 + 自校验）
    /// </code>
    ///
    /// footer 放在**末尾**是关键：PE 加载器忽略尾部数据，所以打包端只需要"追加"，
    /// 不需要重写 PE 头、不需要 UpdateResource，也不需要 Seek 回填。
    /// </summary>
    public static class ContainerFormat
    {
        private static readonly UTF8Encoding Utf8NoBom = new UTF8Encoding(false);

        // ─────────────────────────────────────────────────────────
        // 写入
        // ─────────────────────────────────────────────────────────

        /// <summary>
        /// 在当前流位置写入 footer。调用方必须保证当前位置就是 footer 的起始位置（即 payload 与 manifest 之后）。
        /// </summary>
        public static void WriteFooter(Stream output, ContainerInfo info)
        {
            if (output == null)
            {
                throw new ArgumentNullException("output");
            }

            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            var footer = new byte[ContainerFormatSpec.FooterSize];

            Buffer.BlockCopy(ContainerFormatSpec.Magic, 0, footer, ContainerFormatSpec.OffsetMagic, 8);
            WriteInt32(footer, ContainerFormatSpec.OffsetFormatVersion, info.FormatVersion);
            WriteInt32(footer, ContainerFormatSpec.OffsetFlags, (int)info.Flags);
            WriteInt64(footer, ContainerFormatSpec.OffsetPayloadOffset, info.PayloadOffset);
            WriteInt64(footer, ContainerFormatSpec.OffsetPayloadLength, info.PayloadLength);
            WriteInt64(footer, ContainerFormatSpec.OffsetManifestOffset, info.ManifestOffset);
            WriteInt32(footer, ContainerFormatSpec.OffsetManifestLength, info.ManifestLength);

            var hash = info.PayloadSha256 ?? new byte[32];
            if (hash.Length != 32)
            {
                throw new ArgumentException("PayloadSha256 必须是 32 字节。", "info");
            }

            Buffer.BlockCopy(hash, 0, footer, ContainerFormatSpec.OffsetPayloadSha256, 32);

            var stubHash = info.StubSha256 ?? new byte[32];
            if (stubHash.Length != 32)
            {
                throw new ArgumentException("StubSha256 必须是 32 字节。", "info");
            }

            Buffer.BlockCopy(stubHash, 0, footer, ContainerFormatSpec.OffsetStubSha256, 32);

            var crc = Crc32.Compute(footer, 0, ContainerFormatSpec.CrcCoveredLength);
            WriteUInt32(footer, ContainerFormatSpec.OffsetFooterCrc32, crc);

            output.Write(footer, 0, footer.Length);
        }

        // ─────────────────────────────────────────────────────────
        // 读取
        // ─────────────────────────────────────────────────────────

        /// <summary>流的长度是否足以容纳 footer + 一个最小 stub。</summary>
        public static bool LooksLikeContainer(Stream input)
        {
            return input != null
                   && input.CanSeek
                   && input.Length >= ContainerFormatSpec.FooterSize + ContainerFormatSpec.MinimumStubSize;
        }

        /// <summary>
        /// 尝试读取容器索引。不是本格式（magic 不匹配 / 文件太短）时返回 null。
        /// 是本格式但已损坏或版本过高时抛异常 —— 那两种情况的处理方式不同，不该混为一谈。
        /// </summary>
        public static ContainerInfo TryRead(Stream input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            if (!input.CanSeek)
            {
                throw new ArgumentException("读取容器需要可定位的流。", "input");
            }

            if (!LooksLikeContainer(input))
            {
                return null;
            }

            var fileLength = input.Length;
            var footer = new byte[ContainerFormatSpec.FooterSize];
            input.Seek(fileLength - ContainerFormatSpec.FooterSize, SeekOrigin.Begin);
            ReadExactly(input, footer, 0, footer.Length);

            for (var i = 0; i < 8; i++)
            {
                if (footer[ContainerFormatSpec.OffsetMagic + i] != ContainerFormatSpec.Magic[i])
                {
                    return null;
                }
            }

            var storedCrc = ReadUInt32(footer, ContainerFormatSpec.OffsetFooterCrc32);
            var actualCrc = Crc32.Compute(footer, 0, ContainerFormatSpec.CrcCoveredLength);
            if (storedCrc != actualCrc)
            {
                throw new InvalidDataException(
                    "容器 footer 校验失败（CRC32 不匹配）—— 文件可能已损坏或被截断。");
            }

            var formatVersion = ReadInt32(footer, ContainerFormatSpec.OffsetFormatVersion);
            if (formatVersion > ContainerFormatSpec.FormatVersion)
            {
                throw new NotSupportedException(
                    "容器格式版本 " + formatVersion + " 高于本程序支持的 " + ContainerFormatSpec.FormatVersion +
                    "，请使用更新的安装器。");
            }

            var info = new ContainerInfo
            {
                FormatVersion = formatVersion,
                Flags = (ContainerFlags)ReadInt32(footer, ContainerFormatSpec.OffsetFlags),
                PayloadOffset = ReadInt64(footer, ContainerFormatSpec.OffsetPayloadOffset),
                PayloadLength = ReadInt64(footer, ContainerFormatSpec.OffsetPayloadLength),
                ManifestOffset = ReadInt64(footer, ContainerFormatSpec.OffsetManifestOffset),
                ManifestLength = ReadInt32(footer, ContainerFormatSpec.OffsetManifestLength),
                PayloadSha256 = new byte[32],
                StubSha256 = new byte[32],
                FileLength = fileLength,
            };

            Buffer.BlockCopy(footer, ContainerFormatSpec.OffsetPayloadSha256, info.PayloadSha256, 0, 32);
            Buffer.BlockCopy(footer, ContainerFormatSpec.OffsetStubSha256, info.StubSha256, 0, 32);

            ValidateLayout(info);
            return info;
        }

        /// <summary>读取容器索引，不是本格式就抛异常。</summary>
        public static ContainerInfo Read(Stream input)
        {
            var info = TryRead(input);
            if (info == null)
            {
                throw new InvalidDataException("目标文件不是本程序的安装包容器（magic 不匹配）。");
            }

            return info;
        }

        private static void ValidateLayout(ContainerInfo info)
        {
            var contentEnd = info.FileLength - ContainerFormatSpec.FooterSize;

            if (info.PayloadOffset < ContainerFormatSpec.MinimumStubSize)
            {
                throw new InvalidDataException(
                    "容器损坏：payloadOffset=" + info.PayloadOffset + " 小于最小 stub 长度。");
            }

            if (info.PayloadLength <= 0)
            {
                throw new InvalidDataException("容器损坏：payloadLength=" + info.PayloadLength + "。");
            }

            if (info.ManifestLength <= 0)
            {
                throw new InvalidDataException("容器损坏：manifestLength=" + info.ManifestLength + "。");
            }

            var payloadEnd = info.PayloadOffset + info.PayloadLength;
            var manifestEnd = info.ManifestOffset + info.ManifestLength;

            if (payloadEnd > info.ManifestOffset)
            {
                throw new InvalidDataException("容器损坏：payload 与 manifest 区间重叠。");
            }

            if (manifestEnd > contentEnd)
            {
                throw new InvalidDataException("容器损坏：manifest 区间超出了 footer 之前的内容区。");
            }
        }

        // ─────────────────────────────────────────────────────────
        // 内容访问
        // ─────────────────────────────────────────────────────────

        /// <summary>读取 manifest 的 JSON 文本。</summary>
        public static string ReadManifestJson(Stream input, ContainerInfo info)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            input.Seek(info.ManifestOffset, SeekOrigin.Begin);
            var buffer = new byte[info.ManifestLength];
            ReadExactly(input, buffer, 0, buffer.Length);
            return Utf8NoBom.GetString(buffer);
        }

        /// <summary>
        /// 读取并反序列化 manifest。
        /// 这里做**严格**校验（要求 files 非空）—— 因为读的是安装包，不是一个工程文件。
        /// </summary>
        public static InstallerManifest ReadManifest(Stream input, ContainerInfo info)
        {
            var manifest = ManifestSerializer.Deserialize(ReadManifestJson(input, info));
            ManifestSerializer.Validate(manifest, true);
            return manifest;
        }

        /// <summary>
        /// 打开 payload 区间作为一个独立的只读流，可直接交给
        /// <see cref="System.IO.Compression.ZipArchive"/> 读取，无需落地到临时文件。
        /// </summary>
        public static Stream OpenPayload(Stream input, ContainerInfo info)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            if (!info.IsPayloadEmbedded)
            {
                throw new NotSupportedException(
                    "本容器的 payload 是外置的（flags=ExternalPayload），请使用外置 payload 的读取路径。");
            }

            return new SubStream(input, info.PayloadOffset, info.PayloadLength, leaveOpen: true);
        }

        /// <summary>校验 payload 的 SHA-256 是否与 footer 中记录的一致。</summary>
        /// <param name="input">容器流。</param>
        /// <param name="info">容器索引。</param>
        /// <param name="progress">可选的进度回调（已处理字节数, 总字节数）。</param>
        /// <returns>一致返回 true。</returns>
        public static bool VerifyPayloadHash(Stream input, ContainerInfo info, Action<long, long> progress = null)
        {
            if (info.PayloadSha256 == null || info.PayloadSha256.Length != 32)
            {
                throw new InvalidDataException("容器 footer 中没有有效的 payload 哈希。");
            }

            var actual = Hashing.ComputeRegionBytes(input, info.PayloadOffset, info.PayloadLength);
            if (actual.Length != info.PayloadSha256.Length)
            {
                return false;
            }

            for (var i = 0; i < actual.Length; i++)
            {
                if (actual[i] != info.PayloadSha256[i])
                {
                    return false;
                }
            }

            if (progress != null)
            {
                progress(info.PayloadLength, info.PayloadLength);
            }

            return true;
        }

        /// <summary>
        /// 校验 stub 区（文件开头到 payload 之前）的 SHA-256。
        /// 只校验 payload 挡不住"改安装器代码"这类篡改，所以两者都要查。
        /// </summary>
        public static bool VerifyStubHash(Stream input, ContainerInfo info)
        {
            if (info.StubSha256 == null || info.StubSha256.Length != 32)
            {
                throw new InvalidDataException("容器 footer 中没有有效的 stub 哈希。");
            }

            var actual = Hashing.ComputeRegionBytes(input, 0, info.StubLength);
            return BytesEqual(actual, info.StubSha256);
        }

        private static bool BytesEqual(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }

            for (var i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }

            return true;
        }

        // ─────────────────────────────────────────────────────────
        // 小端读写
        // ─────────────────────────────────────────────────────────

        private static void WriteInt32(byte[] buffer, int offset, int value)
        {
            buffer[offset] = (byte)value;
            buffer[offset + 1] = (byte)(value >> 8);
            buffer[offset + 2] = (byte)(value >> 16);
            buffer[offset + 3] = (byte)(value >> 24);
        }

        private static void WriteUInt32(byte[] buffer, int offset, uint value)
        {
            WriteInt32(buffer, offset, unchecked((int)value));
        }

        private static void WriteInt64(byte[] buffer, int offset, long value)
        {
            for (var i = 0; i < 8; i++)
            {
                buffer[offset + i] = (byte)(value >> (8 * i));
            }
        }

        private static int ReadInt32(byte[] buffer, int offset)
        {
            return buffer[offset]
                   | (buffer[offset + 1] << 8)
                   | (buffer[offset + 2] << 16)
                   | (buffer[offset + 3] << 24);
        }

        private static uint ReadUInt32(byte[] buffer, int offset)
        {
            return unchecked((uint)ReadInt32(buffer, offset));
        }

        private static long ReadInt64(byte[] buffer, int offset)
        {
            long value = 0;
            for (var i = 0; i < 8; i++)
            {
                value |= (long)buffer[offset + i] << (8 * i);
            }

            return value;
        }

        private static void ReadExactly(Stream stream, byte[] buffer, int offset, int count)
        {
            var done = 0;
            while (done < count)
            {
                var read = stream.Read(buffer, offset + done, count - done);
                if (read <= 0)
                {
                    throw new EndOfStreamException("读取容器时意外到达文件末尾。");
                }

                done += read;
            }
        }
    }
}
