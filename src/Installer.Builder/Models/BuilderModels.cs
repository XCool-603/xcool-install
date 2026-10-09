using System;
using System.Collections.Generic;
using System.IO;
using Installer.Abstractions.Model;

namespace Installer.Builder.Models
{
    /// <summary>制作端的当前文档状态。</summary>
    public sealed class BuilderDocument
    {
        /// <summary>当前工程。</summary>
        public InstallerProject Project { get; set; }

        /// <summary>工程文件路径（未保存过为 null）。</summary>
        public string Path { get; set; }

        /// <summary>是否有未保存的修改。</summary>
        public bool Dirty { get; set; }

        /// <summary>造一个默认工程。</summary>
        public static InstallerProject CreateDefault()
        {
            return new InstallerProject
            {
                Manifest = new InstallerManifest
                {
                    SchemaVersion = 2,
                    ProductCode = NewProductCode(),
                    DefaultCulture = "zh-Hans",
                    Cultures = new List<string> { "zh-Hans", "en" },
                    Product = new ProductInfo
                    {
                        Name = new LocalizedText { { "zh-Hans", "" }, { "en", "" } },
                        Version = "1.0.0",
                        Publisher = new LocalizedText { { "zh-Hans", "" }, { "en", "" } },
                        Url = "",
                    },
                    Scope = InstallScopes.PerUser,
                    DefaultInstallDir = @"%LOCALAPPDATA%\Programs\我的产品",
                    EntryPoint = "",
                    License = new LicenseInfo
                    {
                        Required = false,
                        Text = new LocalizedText { { "zh-Hans", "" }, { "en", "" } },
                    },
                    Ui = new UiInfo { Theme = "light", AccentColor = "#069DE7" },
                    Shortcuts = new List<ShortcutSpec>
                    {
                        new ShortcutSpec { Location = "Desktop", Name = new LocalizedText() },
                        new ShortcutSpec { Location = "StartMenu", Name = new LocalizedText() },
                    },
                    Files = new List<FileEntry>(),
                    Strings = new Dictionary<string, LocalizedText>(StringComparer.Ordinal),
                },
                Build = new ProjectBuildSettings(),
            };
        }

        /// <summary>生成一个新的 productCode。</summary>
        public static string NewProductCode()
        {
            return "{" + Guid.NewGuid().ToString().ToUpperInvariant() + "}";
        }

        /// <summary>
        /// 根据源目录和产品信息推导输出路径。
        /// 用户不需要选 —— 默认放在源目录的**同级**目录里。
        /// </summary>
        public static string SuggestOutputPath(string productName, string version, string sourceDir)
        {
            string dir;
            if (string.IsNullOrWhiteSpace(sourceDir))
            {
                dir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            }
            else
            {
                var trimmed = sourceDir.TrimEnd('\\', '/');
                dir = System.IO.Path.GetDirectoryName(trimmed);
                if (string.IsNullOrEmpty(dir))
                {
                    dir = trimmed;
                }
            }

            var name = Sanitize(string.IsNullOrWhiteSpace(productName) ? "安装包" : productName.Trim());
            var ver = string.IsNullOrWhiteSpace(version) ? string.Empty : "_" + Sanitize(version.Trim());

            // 不要再拼一个 "_安装包" 后缀：产品名叫"安装包"时会变成"安装包_1.0_安装包.exe"
            return System.IO.Path.Combine(dir, name + ver + ".exe");
        }

        /// <summary>把不能出现在文件名里的字符换掉。</summary>
        public static string Sanitize(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return "安装包";
            }

            var invalid = System.IO.Path.GetInvalidFileNameChars();
            var sb = new System.Text.StringBuilder(name.Length);
            foreach (var c in name)
            {
                sb.Append(Array.IndexOf(invalid, c) >= 0 ? '_' : c);
            }

            var s = sb.ToString().Trim().Trim('.');
            return s.Length == 0 ? "安装包" : s;
        }
    }
}
