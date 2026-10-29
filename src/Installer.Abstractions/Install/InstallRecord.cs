using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Installer.Abstractions.Install
{
    /// <summary>
    /// 安装记录 —— 安装成功后写在 <c>&lt;安装目录&gt;\.install-record.json</c>。
    ///
    /// 卸载**按它删**：先精确删掉清单里的文件，再把产品目录整个清空
    /// （安装目录已经是「根\厂商\产品」的独占目录，见 <see cref="Model.InstallPaths"/>）。
    /// 现状的卸载端会递归删光 <c>Application.StartupPath</c> 下的一切
    /// （<c>FrmUnInstall.cs:232-257</c>）—— 那是**没有分层**的年代，
    /// 安装目录可能是个公共目录，所以不敢删；分层之后它是我们独占的。
    /// </summary>
    public sealed class InstallRecord
    {
        /// <summary>记录格式版本。</summary>
        [JsonProperty("schemaVersion", Order = 1)]
        public int SchemaVersion { get; set; } = 1;

        /// <summary>产品稳定标识。</summary>
        [JsonProperty("productCode", Order = 2)]
        public string ProductCode { get; set; }

        /// <summary>产品显示名（安装时的语言）。</summary>
        [JsonProperty("productName", Order = 3)]
        public string ProductName { get; set; }

        /// <summary>
        /// 厂商名（安装时的语言）。安装目录形如 <c>{根}\{厂商}\{产品}</c>；
        /// 卸载清空产品目录后，若厂商目录空了就一并回收 —— 靠它认这一层。
        /// 没填厂商时为 null。
        /// </summary>
        [JsonProperty("vendorName", NullValueHandling = NullValueHandling.Ignore, Order = 17)]
        public string VendorName { get; set; }

        /// <summary>产品版本。</summary>
        [JsonProperty("productVersion", Order = 4)]
        public string ProductVersion { get; set; }

        /// <summary>安装范围（perUser / perMachine）。</summary>
        [JsonProperty("scope", Order = 5)]
        public string Scope { get; set; }

        /// <summary>安装目录。</summary>
        [JsonProperty("installDir", Order = 6)]
        public string InstallDir { get; set; }

        /// <summary>
        /// 沙箱根目录（仅沙箱安装时非 null）。
        /// 卸载端据此还原到同一个沙箱 —— 否则沙箱安装会被当成真实安装去动真实注册表。
        /// </summary>
        [JsonProperty("sandboxRoot", NullValueHandling = NullValueHandling.Ignore, Order = 7)]
        public string SandboxRoot { get; set; }

        /// <summary>安装时间（ISO-8601 UTC）。</summary>
        [JsonProperty("installedAtUtc", Order = 7)]
        public string InstalledAtUtc { get; set; }

        /// <summary>入口程序（相对安装目录）。</summary>
        [JsonProperty("entryPoint", Order = 8)]
        public string EntryPoint { get; set; }

        /// <summary>实际安装的文件（相对安装目录）。</summary>
        [JsonProperty("files", Order = 9)]
        public List<string> Files { get; set; } = new List<string>();

        /// <summary>
        /// 每个文件的 SHA-256（相对路径 → 十六进制）。
        /// 卸载时用来区分"还是我们装的那个文件"与"用户改过/替换过的文件"。
        /// </summary>
        [JsonProperty("fileHashes", Order = 10)]
        public Dictionary<string, string> FileHashes { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>实际创建的目录（相对安装目录）。</summary>
        [JsonProperty("directories", Order = 10)]
        public List<string> Directories { get; set; } = new List<string>();

        /// <summary>创建的快捷方式绝对路径。</summary>
        [JsonProperty("shortcuts", Order = 11)]
        public List<string> Shortcuts { get; set; } = new List<string>();

        /// <summary>写入的注册表值。</summary>
        [JsonProperty("registryValues", Order = 12)]
        public List<RegistryValueRecord> RegistryValues { get; set; } = new List<RegistryValueRecord>();

        /// <summary>
        /// 为兼容老安装器的私有约定而写的键，完整路径形如
        /// <c>HKCU\SOFTWARE\WayMark\&lt;产品名&gt;</c>。卸载时整棵删掉。
        /// </summary>
        [JsonProperty("legacyKeys", NullValueHandling = NullValueHandling.Ignore, Order = 16)]
        public List<string> LegacyKeys { get; set; } = new List<string>();

        /// <summary>开机启动项的注册表路径（形如 <c>HKCU\Software\...\Run</c>）；没有则为 null。</summary>
        [JsonProperty("autostartKeyPath", NullValueHandling = NullValueHandling.Ignore, Order = 13)]
        public string AutostartKeyPath { get; set; }

        /// <summary>开机启动项的值名。</summary>
        [JsonProperty("autostartValueName", NullValueHandling = NullValueHandling.Ignore, Order = 14)]
        public string AutostartValueName { get; set; }

        /// <summary>
        /// 注册过的 COM 组件（绝对路径 + 方式）。
        /// 卸载时要先反注册再删文件 —— DLL 没了就反注册不了了。
        /// </summary>
        [JsonProperty("com", NullValueHandling = NullValueHandling.Ignore, Order = 15)]
        public List<Model.ComSpec> Com { get; set; } = new List<Model.ComSpec>();

        /// <summary>标准卸载键的完整路径。</summary>
        [JsonProperty("uninstallKeyPath", NullValueHandling = NullValueHandling.Ignore, Order = 15)]
        public string UninstallKeyPath { get; set; }

        /// <summary>卸载器路径。</summary>
        [JsonProperty("uninstallerPath", NullValueHandling = NullValueHandling.Ignore, Order = 16)]
        public string UninstallerPath { get; set; }

        /// <summary>安装记录的文件名（放在安装目录根部）。</summary>
        public const string FileName = ".install-record.json";

        /// <summary>
        /// 卸载器的文件名 —— 控制面板的 <c>UninstallString</c>、安装记录都指它。
        ///
        /// **它不能和开始菜单入口用同一个文件**（见 <see cref="MenuUninstallerFileName"/>）：
        /// Windows 会把"被卸载键登记为卸载程序的那个 exe"从开始菜单里隐藏
        /// （实测：卸载键指 A、开始菜单指 B 时 B 正常显示；把卸载键也指 B，B 立刻消失）。
        /// </summary>
        public const string UninstallerFileName = "Uninstall.exe";

        /// <summary>
        /// 开始菜单「卸载」快捷方式指向的那份卸载器（和 <see cref="UninstallerFileName"/> 同内容、不同文件）。
        ///
        /// 为什么不复用同一个文件：Windows 会把"卸载键里登记的那个 exe"当成卸载程序，
        /// **不让它出现在开始菜单/「最近添加」里**。老安装器一直能用，是因为它登记的
        /// <c>UnInstallationString</c> 指向的是一个并不存在的 <c>UnInstallation.exe</c>，
        /// 于是 <c>卸载.exe</c> 从未被识别成卸载程序。这里用同样的思路：
        /// 卸载键指 <c>Uninstall.exe</c>，开始菜单指 <c>卸载.exe</c>。
        /// </summary>
        public const string MenuUninstallerFileName = "卸载.exe";

        /// <summary>取安装目录下的记录文件路径。</summary>
        public static string GetPath(string installDir)
        {
            return System.IO.Path.Combine(installDir, FileName);
        }
    }
}
