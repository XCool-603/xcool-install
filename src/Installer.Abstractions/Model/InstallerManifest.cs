using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Installer.Abstractions.Model
{
    /// <summary>
    /// 安装范围。决定权限模型与注册表根：
    /// perUser  → 安装到 %LOCALAPPDATA%\Programs，写 HKCU，asInvoker（**推荐默认**，无需 UAC）
    /// perMachine → 安装到 %ProgramFiles%，写 HKLM，requireAdministrator
    /// </summary>
    public static class InstallScopes
    {
        /// <summary>每用户安装（HKCU，无需管理员）。</summary>
        public const string PerUser = "perUser";

        /// <summary>每机器安装（HKLM，需要管理员）。</summary>
        public const string PerMachine = "perMachine";
    }

    /// <summary>COM 注册方式。</summary>
    public static class ComRegistrationModes
    {
        /// <summary>
        /// 调用 regsvr32 注册（**默认**）。不复制到 System32、参数加引号、读掉输出流、退出码非 0 报错。
        /// </summary>
        public const string RegSvr32 = "regsvr32";

        /// <summary>
        /// 在本进程内 LoadLibrary + DllRegisterServer。
        /// 适合 regsvr32 被安全软件拦截的环境；代价是坏 DLL 会拖垮安装器进程。
        /// </summary>
        public const string Direct = "direct";

        /// <summary>
        /// 免注册 COM（写 <c>&lt;App&gt;.exe.manifest</c>）。
        /// **尚未实现** —— 需要从类型库导出 CLSID，当前会明确报错而不是假装成功。
        /// </summary>
        public const string RegistrationFree = "registrationFree";
    }

    /// <summary>
    /// 安装包清单 —— 容器内 <c>manifest.json</c> 的强类型映射。
    /// 这是**打包端与安装端唯一的共享契约**：两边都引用本类型，字段不匹配是编译期错误。
    /// </summary>
    public sealed class InstallerManifest
    {
        /// <summary>清单格式版本。当前为 2。</summary>
        [JsonProperty("schemaVersion", Order = 1)]
        public int SchemaVersion { get; set; } = 2;

        /// <summary>产品稳定标识（GUID 字符串），用于标准卸载键。**必须 ASCII 且不随语言变化**。</summary>
        [JsonProperty("productCode", Order = 2)]
        public string ProductCode { get; set; }

        /// <summary>默认语言（BCP-47），匹配不到时回退到它。</summary>
        [JsonProperty("defaultCulture", Order = 3)]
        public string DefaultCulture { get; set; } = "zh-Hans";

        /// <summary>本包包含的全部语言。</summary>
        [JsonProperty("cultures", Order = 4)]
        public List<string> Cultures { get; set; } = new List<string>();

        /// <summary>产品信息（名称/版本/厂商，均为多语言）。</summary>
        [JsonProperty("product", Order = 5)]
        public ProductInfo Product { get; set; } = new ProductInfo();

        /// <summary>安装范围，取 <see cref="InstallScopes"/> 中的值。</summary>
        [JsonProperty("scope", Order = 6)]
        public string Scope { get; set; } = InstallScopes.PerUser;

        /// <summary>默认安装目录，支持 %ENV% 环境变量展开。</summary>
        [JsonProperty("defaultInstallDir", Order = 7)]
        public string DefaultInstallDir { get; set; }

        /// <summary>安装完成后要启动的可执行文件（相对安装目录）。</summary>
        [JsonProperty("entryPoint", Order = 8)]
        public string EntryPoint { get; set; }

        /// <summary>许可协议。</summary>
        [JsonProperty("license", Order = 9)]
        public LicenseInfo License { get; set; }

        /// <summary>向导外观。</summary>
        [JsonProperty("ui", Order = 10)]
        public UiInfo Ui { get; set; }

        /// <summary>快捷方式清单。制作端勾选什么，这里就有什么 —— 不再有"写了没人读的键"。</summary>
        [JsonProperty("shortcuts", Order = 11)]
        public List<ShortcutSpec> Shortcuts { get; set; } = new List<ShortcutSpec>();

        /// <summary>是否注册开机启动（写 HKCU 或 HKLM 的 Run 键，与 <see cref="Scope"/> 一致）。</summary>
        [JsonProperty("autostart", Order = 12)]
        public bool Autostart { get; set; }

        /// <summary>安装完成后是否立即启动 <see cref="EntryPoint"/>。</summary>
        [JsonProperty("runAfterInstall", Order = 13)]
        public bool RunAfterInstall { get; set; }

        /// <summary>需要注册的 COM 组件。</summary>
        [JsonProperty("com", Order = 14)]
        public List<ComSpec> Com { get; set; } = new List<ComSpec>();

        /// <summary>
        /// 文件清单（相对路径 + 大小 + sha256）。
        /// 卸载按它删、更新按它比对、修复安装按它校验 —— 这是整套方案里收益最大的一个字段。
        /// </summary>
        [JsonProperty("files", Order = 15)]
        public List<FileEntry> Files { get; set; } = new List<FileEntry>();

        /// <summary>
        /// 空目录清单（相对路径）。
        /// 现状的打包器不为目录生成 zip 条目，所以源目录里的空目录会被静默丢掉；这里显式记录。
        /// </summary>
        [JsonProperty("directories", Order = 16)]
        public List<string> Directories { get; set; } = new List<string>();

        /// <summary>界面文案表：key → { 语言 → 文本 }。</summary>
        [JsonProperty("strings", Order = 17)]
        public Dictionary<string, LocalizedText> Strings { get; set; } =
            new Dictionary<string, LocalizedText>(StringComparer.Ordinal);

        /// <summary>构建信息。</summary>
        [JsonProperty("build", Order = 18)]
        public BuildInfo Build { get; set; }
    }

    /// <summary>产品信息，字段均为多语言文本。</summary>
    public sealed class ProductInfo
    {
        /// <summary>产品显示名。</summary>
        [JsonProperty("name", Order = 1)]
        public LocalizedText Name { get; set; } = new LocalizedText();

        /// <summary>产品版本（如 2.4.1）。</summary>
        [JsonProperty("version", Order = 2)]
        public string Version { get; set; }

        /// <summary>厂商名。</summary>
        [JsonProperty("publisher", Order = 3)]
        public LocalizedText Publisher { get; set; } = new LocalizedText();

        /// <summary>厂商网址。</summary>
        [JsonProperty("url", Order = 4)]
        public string Url { get; set; }
    }

    /// <summary>许可协议。</summary>
    public sealed class LicenseInfo
    {
        /// <summary>是否必须同意才能继续安装。</summary>
        [JsonProperty("required", Order = 1)]
        public bool Required { get; set; } = true;

        /// <summary>协议正文（多语言）。</summary>
        [JsonProperty("text", Order = 2)]
        public LocalizedText Text { get; set; } = new LocalizedText();
    }

    /// <summary>向导外观配置。</summary>
    public sealed class UiInfo
    {
        /// <summary>主题：light / dark。</summary>
        [JsonProperty("theme", Order = 1)]
        public string Theme { get; set; } = "light";

        /// <summary>主色调（#RRGGBB）。</summary>
        [JsonProperty("accentColor", Order = 2)]
        public string AccentColor { get; set; } = "#069DE7";

        /// <summary>背景图在 payload 内的相对路径。</summary>
        [JsonProperty("backgroundImage", Order = 3)]
        public string BackgroundImage { get; set; }

        /// <summary>Logo 在 payload 内的相对路径。</summary>
        [JsonProperty("logo", Order = 4)]
        public string Logo { get; set; }
    }

    /// <summary>快捷方式。</summary>
    public sealed class ShortcutSpec
    {
        /// <summary>位置：Desktop / StartMenu / Startup。</summary>
        [JsonProperty("location", Order = 1)]
        public string Location { get; set; }

        /// <summary>快捷方式显示名（多语言）。</summary>
        [JsonProperty("name", Order = 2)]
        public LocalizedText Name { get; set; } = new LocalizedText();

        /// <summary>目标，相对安装目录。为空则用 <see cref="InstallerManifest.EntryPoint"/>。</summary>
        [JsonProperty("target", Order = 3)]
        public string Target { get; set; }

        /// <summary>启动参数。</summary>
        [JsonProperty("arguments", Order = 4)]
        public string Arguments { get; set; }

        /// <summary>起始位置，相对安装目录。为空则用安装目录本身。</summary>
        [JsonProperty("workingDirectory", Order = 5)]
        public string WorkingDirectory { get; set; }
    }

    /// <summary>COM 组件注册项。</summary>
    public sealed class ComSpec
    {
        /// <summary>DLL/OCX 在 payload 内的相对路径。</summary>
        [JsonProperty("path", Order = 1)]
        public string Path { get; set; }

        /// <summary>注册方式，取 <see cref="ComRegistrationModes"/> 中的值。</summary>
        [JsonProperty("mode", Order = 2)]
        public string Mode { get; set; } = ComRegistrationModes.RegSvr32;
    }

    /// <summary>文件清单条目。</summary>
    public sealed class FileEntry
    {
        /// <summary>相对安装目录的路径，用反斜杠分隔。</summary>
        [JsonProperty("path", Order = 1)]
        public string Path { get; set; }

        /// <summary>字节数。</summary>
        [JsonProperty("size", Order = 2)]
        public long Size { get; set; }

        /// <summary>小写十六进制 SHA-256。</summary>
        [JsonProperty("sha256", Order = 3)]
        public string Sha256 { get; set; }
    }

    /// <summary>多语言文本：语言代码 → 文本。</summary>
    public sealed class LocalizedText : Dictionary<string, string>
    {
        /// <summary>构造一个空的多语言文本。</summary>
        public LocalizedText()
            : base(StringComparer.Ordinal)
        {
        }

        /// <summary>用单一语言构造。</summary>
        public LocalizedText(string culture, string text)
            : base(StringComparer.Ordinal)
        {
            this[culture] = text;
        }
    }

    /// <summary>构建信息。</summary>
    public sealed class BuildInfo
    {
        /// <summary>
        /// 构建时间（ISO-8601 UTC）。它同时决定 zip 条目的 mtime ——
        /// 想得到可重现的字节，就必须固定这个值。
        /// </summary>
        [JsonProperty("builtAtUtc", Order = 1)]
        public string BuiltAtUtc { get; set; }

        /// <summary>打包器版本。</summary>
        [JsonProperty("packerVersion", Order = 2)]
        public string PackerVersion { get; set; }

        /// <summary>清单里的文件条目数。</summary>
        [JsonProperty("entryCount", Order = 3)]
        public int EntryCount { get; set; }

        /// <summary>payload（压缩后）的字节数。</summary>
        [JsonProperty("payloadBytes", Order = 4)]
        public long PayloadBytes { get; set; }
    }
}
