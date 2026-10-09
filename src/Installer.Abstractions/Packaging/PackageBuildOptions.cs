using System;
using System.Collections.Generic;
using Installer.Abstractions.Model;

namespace Installer.Abstractions.Packaging
{
    /// <summary>打包流水线的阶段。</summary>
    public enum BuildStage
    {
        /// <summary>校验工程与路径。</summary>
        Validate = 0,
        /// <summary>扫描源目录，得到文件清单骨架。</summary>
        Collect = 1,
        /// <summary>逐文件计算 SHA-256。</summary>
        Hash = 2,
        /// <summary>给 stub 补丁图标。</summary>
        PatchIcon = 3,
        /// <summary>组装容器（stub + payload + manifest + footer）。</summary>
        Compose = 4,
        /// <summary>数字签名。</summary>
        Sign = 5,
        /// <summary>回读自校验。</summary>
        Verify = 6,
        /// <summary>完成。</summary>
        Done = 7,
    }

    /// <summary>打包进度。</summary>
    public sealed class BuildProgress
    {
        /// <summary>当前阶段。</summary>
        public BuildStage Stage { get; set; }

        /// <summary>人类可读的进度描述。</summary>
        public string Message { get; set; }

        /// <summary>0–100。未知时为 -1。</summary>
        public int Percent { get; set; }

        /// <summary>构造。</summary>
        public BuildProgress(BuildStage stage, string message, int percent)
        {
            Stage = stage;
            Message = message;
            Percent = percent;
        }
    }

    /// <summary>打包选项。</summary>
    public sealed class PackageBuildOptions
    {
        /// <summary>
        /// 直接提供清单（GUI 场景）。打包时以本字段为准；从工程文件加载的场景由调用方自行反序列化后填入。
        /// </summary>
        public InstallerManifest Manifest { get; set; }

        /// <summary>待打包目录（其内容将成为安装后的文件树）。</summary>
        public string SourceDir { get; set; }

        /// <summary>模板 stub（Installer.Runtime.exe）路径。</summary>
        public string StubPath { get; set; }

        /// <summary>产物路径（.exe）。</summary>
        public string OutputPath { get; set; }

        /// <summary>可选：要写入 exe 的图标（.ico）。为 null 时保留 stub 自带图标。</summary>
        public string IconPath { get; set; }

        /// <summary>跳过图标补丁（stub 没有图标资源、或补丁失败时使用）。</summary>
        public bool SkipIconPatch { get; set; }

        /// <summary>
        /// 构建时间戳。**可重现构建的关键**：同一个值 + 同一份输入 = 同样的输出字节。
        /// 为 null 时取 <see cref="DateTimeOffset.UtcNow"/>。
        /// </summary>
        public DateTimeOffset? BuildTimestamp { get; set; }

        /// <summary>打包器版本，写入清单。</summary>
        public string PackerVersion { get; set; }

        /// <summary>把 payload 外置为同名的 <c>.pkg</c>（默认 false = 单 exe）。</summary>
        public bool ExternalPayload { get; set; }

        /// <summary>压缩级别：Optimal（默认）或 NoCompression（调试用）。</summary>
        public System.IO.Compression.CompressionLevel CompressionLevel { get; set; } =
            System.IO.Compression.CompressionLevel.Optimal;

        /// <summary>
        /// signtool.exe 的路径。为 null 时跳过签名。
        /// **注意**：签名必须在图标补丁之后，否则补丁会让签名失效。
        /// </summary>
        public string SignToolPath { get; set; }

        /// <summary>签名证书的 SHA-1 指纹（signtool /sha1）。</summary>
        public string SignCertificateThumbprint { get; set; }

        /// <summary>时间戳服务器 URL（signtool /tr）。为 null 则不带时间戳。</summary>
        public string SignTimestampUrl { get; set; }
    }

    /// <summary>打包结果。</summary>
    public sealed class PackageBuildResult
    {
        /// <summary>产物路径。</summary>
        public string OutputPath { get; set; }

        /// <summary>产物总字节数。</summary>
        public long OutputBytes { get; set; }

        /// <summary>stub 部分字节数。</summary>
        public long StubBytes { get; set; }

        /// <summary>payload（压缩后）字节数。</summary>
        public long PayloadBytes { get; set; }

        /// <summary>manifest 字节数。</summary>
        public int ManifestBytes { get; set; }

        /// <summary>payload 的 SHA-256（小写十六进制）。</summary>
        public string PayloadSha256 { get; set; }

        /// <summary>产物文件的 SHA-256（小写十六进制）。</summary>
        public string OutputSha256 { get; set; }

        /// <summary>清单中的文件条目数。</summary>
        public int EntryCount { get; set; }

        /// <summary>实际使用的构建时间戳。</summary>
        public DateTimeOffset BuildTimestamp { get; set; }

        /// <summary>是否执行了图标补丁。</summary>
        public bool IconPatched { get; set; }

        /// <summary>各阶段耗时（毫秒），用于诊断。</summary>
        public Dictionary<BuildStage, long> StageMilliseconds { get; set; } =
            new Dictionary<BuildStage, long>();
    }

    /// <summary>打包过程中的失败。</summary>
    public class PackageBuildException : Exception
    {
        /// <summary>构造。</summary>
        public PackageBuildException(string message)
            : base(message)
        {
        }

        /// <summary>构造。</summary>
        public PackageBuildException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
