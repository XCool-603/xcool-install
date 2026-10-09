using System;

namespace Installer.Abstractions.Packaging
{
    /// <summary>
    /// 容器格式的**唯一常量定义处**。写入端与读取端都从这里取偏移，
    /// 因此"打包器写的偏移"与"stub 读的偏移"不可能漂移。
    /// </summary>
    public static class ContainerFormatSpec
    {
        /// <summary>footer 魔数：<c>WMINS2\0\0</c>（WayMark Installer v2）。</summary>
        public static readonly byte[] Magic = { 0x57, 0x4D, 0x49, 0x4E, 0x53, 0x32, 0x00, 0x00 };

        /// <summary>当前容器格式版本。</summary>
        public const int FormatVersion = 2;

        /// <summary>footer 总长度（字节）。</summary>
        public const int FooterSize = 128;

        // ── footer 字段偏移（全部小端）────────────────────────────
        /// <summary>magic（8 字节）。</summary>
        public const int OffsetMagic = 0;
        /// <summary>formatVersion（int32）。</summary>
        public const int OffsetFormatVersion = 8;
        /// <summary>flags（int32）。</summary>
        public const int OffsetFlags = 12;
        /// <summary>payload 起始偏移（int64）。</summary>
        public const int OffsetPayloadOffset = 16;
        /// <summary>payload 长度（int64）。</summary>
        public const int OffsetPayloadLength = 24;
        /// <summary>manifest 起始偏移（int64）。</summary>
        public const int OffsetManifestOffset = 32;
        /// <summary>manifest 长度（int32）。</summary>
        public const int OffsetManifestLength = 40;
        /// <summary>payload 的 SHA-256（32 字节）。</summary>
        public const int OffsetPayloadSha256 = 48;
        /// <summary>stub 区（[0, PayloadOffset)）的 SHA-256（32 字节）。</summary>
        public const int OffsetStubSha256 = 80;
        /// <summary>footer 自身前 112 字节的 CRC-32（uint32）。</summary>
        public const int OffsetFooterCrc32 = 112;
        /// <summary>footer 内参与 CRC 计算的长度（即 crc 字段之前的字节数，含两个哈希）。</summary>
        public const int CrcCoveredLength = 112;

        /// <summary>stub 支持的最低 PE 结构要求：必须是一个可执行文件。</summary>
        public const int MinimumStubSize = 1024;
    }

    /// <summary>容器标志位。</summary>
    [Flags]
    public enum ContainerFlags
    {
        /// <summary>无特殊标志：payload 内嵌在同一个文件里（单 exe）。</summary>
        None = 0,

        /// <summary>payload 外置在同名的 <c>.pkg</c> 文件里（用于将来的差分更新）。</summary>
        ExternalPayload = 1,
    }

    /// <summary>读取容器后得到的索引信息（不含 manifest 正文与 payload 数据）。</summary>
    public sealed class ContainerInfo
    {
        /// <summary>容器格式版本。</summary>
        public int FormatVersion { get; set; }

        /// <summary>标志位。</summary>
        public ContainerFlags Flags { get; set; }

        /// <summary>payload 起始偏移。</summary>
        public long PayloadOffset { get; set; }

        /// <summary>payload 字节数。</summary>
        public long PayloadLength { get; set; }

        /// <summary>manifest 起始偏移。</summary>
        public long ManifestOffset { get; set; }

        /// <summary>manifest 字节数。</summary>
        public int ManifestLength { get; set; }

        /// <summary>payload 的 SHA-256。</summary>
        public byte[] PayloadSha256 { get; set; }

        /// <summary>
        /// stub 区（文件开头到 payload 之前）的 SHA-256。
        /// 只校验 payload 是不够的 —— 安装器的代码本身（stub）被改一样危险。
        /// </summary>
        public byte[] StubSha256 { get; set; }

        /// <summary>容器文件总长度。</summary>
        public long FileLength { get; set; }

        /// <summary>payload 是否内嵌在同一个文件里。</summary>
        public bool IsPayloadEmbedded
        {
            get { return (Flags & ContainerFlags.ExternalPayload) == 0; }
        }

        /// <summary>stub 区（footer 之前、payload 之前的全部字节）长度，即被替换的模板 exe 大小。</summary>
        public long StubLength
        {
            get { return PayloadOffset; }
        }
    }
}
