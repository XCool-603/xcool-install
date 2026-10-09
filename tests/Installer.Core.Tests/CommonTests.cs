using System;
using System.IO;
using System.Text;
using Installer.Abstractions.Packaging;
using Installer.Core.Common;
using Installer.Core.Packaging;
using Xunit;

namespace Installer.Core.Tests
{
    /// <summary>底层工具：CRC32 / SHA-256 / 偏移流。</summary>
    public class CommonTests
    {
        [Fact]
        public void Crc32_应符合标准检验值()
        {
            // CRC-32/ISO-HDLC 的标准检验向量："123456789" → 0xCBF43926
            var data = Encoding.ASCII.GetBytes("123456789");
            var crc = Crc32.Compute(data, 0, data.Length);
            Assert.Equal(0xCBF43926u, crc);
        }

        [Fact]
        public void Crc32_空输入应为零()
        {
            Assert.Equal(0u, Crc32.Compute(new byte[0], 0, 0));
        }

        [Fact]
        public void Crc32_区间计算应与整段一致()
        {
            var all = new byte[256];
            for (var i = 0; i < 256; i++)
            {
                all[i] = (byte)i;
            }

            var whole = Crc32.Compute(all, 0, 256);
            using (var ms = new MemoryStream(all))
            {
                Assert.Equal(whole, Crc32.Compute(ms, 0, 256));
            }
        }

        [Fact]
        public void Hashing_Sha256_应匹配已知值()
        {
            // SHA-256("abc")
            var hex = Hashing.ComputeBytes(Encoding.ASCII.GetBytes("abc"));
            Assert.Equal("ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad", hex);
        }

        [Fact]
        public void OffsetWriteStream_逻辑位置从零开始_物理写入落在偏移处()
        {
            using (var ms = new MemoryStream())
            {
                // 前面先放 100 字节的"stub"
                var prefix = new byte[100];
                for (var i = 0; i < 100; i++)
                {
                    prefix[i] = 0xAA;
                }

                ms.Write(prefix, 0, prefix.Length);

                using (var region = new OffsetWriteStream(ms, 100))
                {
                    Assert.Equal(0, region.Position);

                    var payload = new byte[] { 1, 2, 3, 4, 5 };
                    region.Write(payload, 0, payload.Length);

                    Assert.Equal(5, region.Position);
                    Assert.Equal(5, region.WrittenLength);

                    // 回退改写不应扩大长度
                    region.Seek(0, SeekOrigin.Begin);
                    region.Write(new byte[] { 9 }, 0, 1);
                    Assert.Equal(5, region.WrittenLength);
                }

                var all = ms.ToArray();
                Assert.Equal(105, all.Length);

                // 前缀没被碰
                for (var i = 0; i < 100; i++)
                {
                    Assert.Equal(0xAA, all[i]);
                }

                // 区间内容正确，且第一次写进去的字节确实被回退改写覆盖了
                var expected = new byte[] { 9, 2, 3, 4, 5 };
                for (var i = 0; i < expected.Length; i++)
                {
                    Assert.Equal(expected[i], all[100 + i]);
                }
            }
        }

        [Fact]
        public void OffsetWriteStream_负位置应抛异常()
        {
            using (var ms = new MemoryStream())
            using (var region = new OffsetWriteStream(ms, 0))
            {
                Assert.Throws<IOException>(() => region.Seek(-1, SeekOrigin.Begin));
            }
        }

        [Fact]
        public void SubStream_应限定在区间内()
        {
            var data = new byte[300];
            for (var i = 0; i < 300; i++)
            {
                data[i] = (byte)(i & 0xFF);
            }

            using (var ms = new MemoryStream(data))
            using (var sub = new SubStream(ms, 100, 50, true))
            {
                Assert.Equal(50, sub.Length);
                Assert.Equal(100, sub.ReadByte());

                sub.Seek(0, SeekOrigin.End);
                Assert.Equal(50, sub.Position);

                Assert.Throws<IOException>(() => sub.Seek(1, SeekOrigin.End));
            }
        }
    }

    /// <summary>容器格式的往返与损坏检测。</summary>
    public class ContainerFormatTests
    {
        private static byte[] BuildSampleContainer(out ContainerInfo info)
        {
            var stub = new byte[4096];
            for (var i = 0; i < stub.Length; i++)
            {
                stub[i] = (byte)(i & 0xFF);
            }

            var payload = Encoding.UTF8.GetBytes("PAYLOAD-BYTES-0123456789");
            var manifest = Encoding.UTF8.GetBytes("{\"schemaVersion\":2}");

            using (var ms = new MemoryStream())
            {
                ms.Write(stub, 0, stub.Length);

                var payloadOffset = ms.Position;
                ms.Write(payload, 0, payload.Length);
                var payloadLength = payload.Length;

                var manifestOffset = ms.Position;
                ms.Write(manifest, 0, manifest.Length);

                var endPosition = ms.Position;

                var payloadHash = Hashing.Sha256Raw(payload);
                ms.Seek(endPosition, SeekOrigin.Begin);

                info = new ContainerInfo
                {
                    FormatVersion = ContainerFormatSpec.FormatVersion,
                    Flags = ContainerFlags.None,
                    PayloadOffset = payloadOffset,
                    PayloadLength = payloadLength,
                    ManifestOffset = manifestOffset,
                    ManifestLength = manifest.Length,
                    PayloadSha256 = payloadHash,
                    StubSha256 = Hashing.Sha256Raw(stub),
                    FileLength = endPosition + ContainerFormatSpec.FooterSize,
                };

                ContainerFormat.WriteFooter(ms, info);
                return ms.ToArray();
            }
        }

        [Fact]
        public void 往返_应读出与写入一致的索引()
        {
            ContainerInfo written;
            var bytes = BuildSampleContainer(out written);

            using (var ms = new MemoryStream(bytes))
            {
                var read = ContainerFormat.Read(ms);

                Assert.Equal(written.PayloadOffset, read.PayloadOffset);
                Assert.Equal(written.PayloadLength, read.PayloadLength);
                Assert.Equal(written.ManifestOffset, read.ManifestOffset);
                Assert.Equal(written.ManifestLength, read.ManifestLength);
                Assert.Equal(bytes.Length, read.FileLength);
                Assert.True(read.IsPayloadEmbedded);
                Assert.Equal(Hashing.ToHex(written.PayloadSha256), Hashing.ToHex(read.PayloadSha256));
                Assert.Equal(Hashing.ToHex(written.StubSha256), Hashing.ToHex(read.StubSha256));
            }
        }

        [Fact]
        public void 哈希校验_应同时覆盖_payload_与_stub()
        {
            ContainerInfo info;
            var bytes = BuildSampleContainer(out info);

            using (var ms = new MemoryStream(bytes))
            {
                Assert.True(ContainerFormat.VerifyPayloadHash(ms, info));
                Assert.True(ContainerFormat.VerifyStubHash(ms, info));
            }

            // 改 stub 里的一个字节 → stub 哈希必须失败，payload 哈希仍应通过
            var tamperedStub = (byte[])bytes.Clone();
            tamperedStub[10] ^= 0xFF;
            using (var ms = new MemoryStream(tamperedStub))
            {
                var t = ContainerFormat.Read(ms);
                Assert.False(ContainerFormat.VerifyStubHash(ms, t));
                Assert.True(ContainerFormat.VerifyPayloadHash(ms, t));
            }

            // 改 payload 里的一个字节 → payload 哈希必须失败，stub 哈希仍应通过
            var tamperedPayload = (byte[])bytes.Clone();
            tamperedPayload[info.PayloadOffset + 1] ^= 0xFF;
            using (var ms = new MemoryStream(tamperedPayload))
            {
                var t = ContainerFormat.Read(ms);
                Assert.True(ContainerFormat.VerifyStubHash(ms, t));
                Assert.False(ContainerFormat.VerifyPayloadHash(ms, t));
            }
        }

        [Fact]
        public void 非容器文件应返回_null()
        {
            var junk = new byte[5000];
            using (var ms = new MemoryStream(junk))
            {
                Assert.Null(ContainerFormat.TryRead(ms));
            }
        }

        [Fact]
        public void 太短的文件应返回_null()
        {
            using (var ms = new MemoryStream(new byte[64]))
            {
                Assert.Null(ContainerFormat.TryRead(ms));
            }
        }

        [Fact]
        public void footer_被改动应抛_InvalidDataException()
        {
            ContainerInfo info;
            var bytes = BuildSampleContainer(out info);

            // 改 payload 哈希字段（在 CRC 覆盖范围内）
            bytes[bytes.Length - ContainerFormatSpec.FooterSize + ContainerFormatSpec.OffsetPayloadSha256] ^= 0xFF;

            using (var ms = new MemoryStream(bytes))
            {
                Assert.Throws<InvalidDataException>(() => ContainerFormat.TryRead(ms));
            }
        }

        [Fact]
        public void 高版本容器应抛_NotSupportedException()
        {
            ContainerInfo info;
            var bytes = BuildSampleContainer(out info);

            var versionOffset = bytes.Length - ContainerFormatSpec.FooterSize + ContainerFormatSpec.OffsetFormatVersion;
            bytes[versionOffset] = 99;

            // 版本字段在 CRC 覆盖范围内，所以要重算 CRC 才能走到版本判断
            var footerStart = bytes.Length - ContainerFormatSpec.FooterSize;
            var crc = Crc32.Compute(bytes, footerStart, ContainerFormatSpec.CrcCoveredLength);
            var crcOffset = footerStart + ContainerFormatSpec.OffsetFooterCrc32;
            var crcBytes = BitConverter.GetBytes(crc);
            Array.Copy(crcBytes, 0, bytes, crcOffset, 4);

            using (var ms = new MemoryStream(bytes))
            {
                Assert.Throws<NotSupportedException>(() => ContainerFormat.TryRead(ms));
            }
        }

        [Fact]
        public void 损坏的布局应被拒绝()
        {
            ContainerInfo info;
            var bytes = BuildSampleContainer(out info);

            // 把 payloadLength 改成与 manifest 重叠
            var footerStart = bytes.Length - ContainerFormatSpec.FooterSize;
            var lenOffset = footerStart + ContainerFormatSpec.OffsetPayloadLength;
            var badLength = BitConverter.GetBytes((long)(bytes.Length));
            Array.Copy(badLength, 0, bytes, lenOffset, 8);

            var crc = Crc32.Compute(bytes, footerStart, ContainerFormatSpec.CrcCoveredLength);
            Array.Copy(BitConverter.GetBytes(crc), 0, bytes, footerStart + ContainerFormatSpec.OffsetFooterCrc32, 4);

            using (var ms = new MemoryStream(bytes))
            {
                Assert.Throws<InvalidDataException>(() => ContainerFormat.TryRead(ms));
            }
        }

        [Fact]
        public void OpenPayload_应返回定位好的子流()
        {
            ContainerInfo info;
            var bytes = BuildSampleContainer(out info);

            using (var ms = new MemoryStream(bytes))
            {
                var read = ContainerFormat.Read(ms);
                using (var payload = ContainerFormat.OpenPayload(ms, read))
                {
                    var buffer = new byte[payload.Length];
                    var done = 0;
                    while (done < buffer.Length)
                    {
                        var n = payload.Read(buffer, done, buffer.Length - done);
                        if (n <= 0)
                        {
                            break;
                        }

                        done += n;
                    }

                    Assert.Equal("PAYLOAD-BYTES-0123456789", Encoding.UTF8.GetString(buffer));
                }
            }
        }
    }
}
