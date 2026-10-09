using System;
using Newtonsoft.Json;

namespace Installer.Abstractions.Model
{
    /// <summary>
    /// 制作工程文件（<c>.wmpkg.json</c>）。
    ///
    /// 两种写法都支持：
    /// ① 只有清单（手写最省事）：<c>{ "schemaVersion": 2, "productCode": ... }</c>
    /// ② 完整工程（制作端 GUI 存出来的）：<c>{ "manifest": {...}, "build": {...} }</c>
    ///
    /// Installer.Core 里的 ProjectSerializer 会自动识别是哪一种。
    /// </summary>
    public sealed class InstallerProject
    {
        /// <summary>清单。</summary>
        [JsonProperty("manifest", Order = 1)]
        public InstallerManifest Manifest { get; set; }

        /// <summary>打包参数（源目录 / stub / 输出 / 图标）。</summary>
        [JsonProperty("build", Order = 2)]
        public ProjectBuildSettings Build { get; set; } = new ProjectBuildSettings();
    }

    /// <summary>工程里的打包参数。</summary>
    public sealed class ProjectBuildSettings
    {
        /// <summary>待打包目录。</summary>
        [JsonProperty("sourceDir", NullValueHandling = NullValueHandling.Ignore, Order = 1)]
        public string SourceDir { get; set; }

        /// <summary>模板 stub 路径。</summary>
        [JsonProperty("stubPath", NullValueHandling = NullValueHandling.Ignore, Order = 2)]
        public string StubPath { get; set; }

        /// <summary>产物路径。</summary>
        [JsonProperty("outputPath", NullValueHandling = NullValueHandling.Ignore, Order = 3)]
        public string OutputPath { get; set; }

        /// <summary>图标路径。</summary>
        [JsonProperty("iconPath", NullValueHandling = NullValueHandling.Ignore, Order = 4)]
        public string IconPath { get; set; }

        /// <summary>固定构建时间戳（ISO-8601），用于可重现构建。</summary>
        [JsonProperty("timestamp", NullValueHandling = NullValueHandling.Ignore, Order = 5)]
        public string Timestamp { get; set; }

        /// <summary>要注册的 COM 组件（相对源目录的路径）。</summary>
        [JsonProperty("comFiles", NullValueHandling = NullValueHandling.Ignore, Order = 6)]
        public System.Collections.Generic.List<string> ComFiles { get; set; }
    }
}
