using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using Installer.Abstractions.Model;

namespace Installer.Core.Packaging
{
    /// <summary>v1 旧包里读出来的内容。</summary>
    public sealed class LegacyPackage
    {
        /// <summary>install.resources 的路径。</summary>
        public string ResourcesPath { get; set; }

        /// <summary>产品名。</summary>
        public string ProductName { get; set; }

        /// <summary>公司名。</summary>
        public string CompanyName { get; set; }

        /// <summary>可执行文件名。</summary>
        public string ProductExeName { get; set; }

        /// <summary>版本。</summary>
        public string ProductVersion { get; set; }

        /// <summary>公司网址。</summary>
        public string CompanyUrl { get; set; }

        /// <summary>默认安装目录。</summary>
        public string InstallationPath { get; set; }

        /// <summary>许可协议正文。</summary>
        public string LicenseAgreement { get; set; }

        /// <summary>需要 regsvr32 注册的文件名。</summary>
        public List<string> RegditFiles { get; set; } = new List<string>();

        /// <summary>背景色（ARGB）；取不到为 null。</summary>
        public int? BackColorArgb { get; set; }

        /// <summary>打包负载（bin.zip 的字节）。</summary>
        public byte[] PayloadZip { get; set; }

        /// <summary>原始键值对（供诊断）。</summary>
        public Dictionary<string, string> RawKeys { get; set; } = new Dictionary<string, string>(StringComparer.Ordinal);

        /// <summary>读取过程中遇到的问题。</summary>
        public List<string> Warnings { get; set; } = new List<string>();
    }

    /// <summary>
    /// v1 旧包读取器。
    ///
    /// v1 的载体是 exe 同目录的 <c>install.resources</c>（.NET ResourceWriter 二进制），
    /// 里面是一堆**无 schema 的散键**。现状的读法在
    /// <c>Installation\ResourceManage.cs:44-126</c>。
    ///
    /// 这个类的用途是**迁移**：把老工程读出来转成新的 <c>.wmpkg.json</c>，
    /// 而不是让新安装器去装老包（老包自带老安装器，本来就能装）。
    /// </summary>
    public static class LegacyV1Reader
    {
        /// <summary>v1 的资源文件名。</summary>
        public const string ResourcesFileName = "install.resources";

        /// <summary>目录里是否像是一个 v1 安装包。</summary>
        public static bool LooksLikeV1(string directory)
        {
            return !string.IsNullOrWhiteSpace(directory)
                   && File.Exists(Path.Combine(directory, ResourcesFileName));
        }

        /// <summary>读取 v1 包。</summary>
        /// <param name="resourcesPath">install.resources 的路径，或其所在目录。</param>
        public static LegacyPackage Read(string resourcesPath)
        {
            if (Directory.Exists(resourcesPath))
            {
                resourcesPath = Path.Combine(resourcesPath, ResourcesFileName);
            }

            if (!File.Exists(resourcesPath))
            {
                throw new FileNotFoundException("找不到 v1 资源文件：" + resourcesPath, resourcesPath);
            }

            var pkg = new LegacyPackage { ResourcesPath = resourcesPath };

            using (var reader = new ResourceReader(resourcesPath))
            {
                var iter = reader.GetEnumerator();
                while (iter.MoveNext())
                {
                    var key = Convert.ToString(iter.Key, CultureInfo.InvariantCulture);
                    var value = iter.Value;
                    if (key == null)
                    {
                        continue;
                    }

                    // 记录一份可读的键值，便于诊断
                    if (value is string)
                    {
                        pkg.RawKeys[key] = (string)value;
                    }
                    else if (value is byte[])
                    {
                        pkg.RawKeys[key] = "<byte[" + ((byte[])value).Length + "]>";
                    }
                    else if (value != null)
                    {
                        pkg.RawKeys[key] = "<" + value.GetType().Name + ">";
                    }

                    switch (key)
                    {
                        case "InstallProductName":
                            pkg.ProductName = AsString(value);
                            break;
                        case "InstallCompanyName":
                            pkg.CompanyName = AsString(value);
                            break;
                        case "InstallProductExeName":
                            pkg.ProductExeName = AsString(value);
                            break;
                        case "InstallProductVersion":
                            pkg.ProductVersion = AsString(value);
                            break;
                        case "InstallCompanyUrl":
                            pkg.CompanyUrl = AsString(value);
                            break;
                        case "InstallationPath":
                            pkg.InstallationPath = AsString(value);
                            break;
                        case "LicenseAgreement":
                            pkg.LicenseAgreement = AsString(value);
                            break;
                        case "RegditFiles":
                            var files = AsString(value);
                            if (!string.IsNullOrEmpty(files))
                            {
                                pkg.RegditFiles = files
                                    .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(s => s.Trim())
                                    .Where(s => s.Length > 0)
                                    .ToList();
                            }

                            break;
                        case "BackColor":
                            if (value is System.Drawing.Color)
                            {
                                pkg.BackColorArgb = ((System.Drawing.Color)value).ToArgb();
                            }

                            break;
                        case "bin":
                            // 模板里 bin 可能是 byte[0]，甚至是个 object —— 都不能当成负载
                            var bytes = value as byte[];
                            if (bytes != null && bytes.Length > 0)
                            {
                                pkg.PayloadZip = bytes;
                            }
                            else
                            {
                                pkg.Warnings.Add("bin 不是有效的字节数组（" +
                                                 (value == null ? "null" : value.GetType().Name) +
                                                 "），这个包可能只是模板。");
                            }

                            break;
                    }
                }
            }

            if (pkg.PayloadZip == null)
            {
                pkg.Warnings.Add("包里没有负载（bin 为空），无法还原文件清单。");
            }

            return pkg;
        }

        /// <summary>
        /// 把 v1 包转换成新工程。
        ///
        /// 注意：v1 的 zip 条目名是按**当时的区域 ANSI 代码页**编码的
        /// （<c>CsharpZipLib\Zip\ZipConstants.cs:434</c>），跨语言会乱码。
        /// 所以这里只还原元数据；文件清单要靠重新打包时扫描源目录。
        /// </summary>
        public static InstallerProject ToProject(LegacyPackage package, string sourceDir = null)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            var name = string.IsNullOrWhiteSpace(package.ProductName)
                ? "LegacyProduct"
                : package.ProductName;

            var manifest = new InstallerManifest
            {
                SchemaVersion = 2,
                ProductCode = "{" + Guid.NewGuid().ToString().ToUpperInvariant() + "}",
                DefaultCulture = "zh-Hans",
                Cultures = new List<string> { "zh-Hans" },
                Product = new ProductInfo
                {
                    Name = new LocalizedText { { "zh-Hans", name }, { "en", name } },
                    Version = string.IsNullOrWhiteSpace(package.ProductVersion) ? "1.0.0" : package.ProductVersion,
                    Publisher = new LocalizedText
                    {
                        { "zh-Hans", package.CompanyName ?? string.Empty },
                        { "en", package.CompanyName ?? string.Empty },
                    },
                    Url = package.CompanyUrl ?? string.Empty,
                },
                Scope = InstallScopes.PerUser,
                DefaultInstallDir = string.IsNullOrWhiteSpace(package.InstallationPath)
                    ? @"%LOCALAPPDATA%\Programs\" + name
                    : package.InstallationPath,
                EntryPoint = string.IsNullOrWhiteSpace(package.ProductExeName)
                    ? null
                    : @"bin\" + package.ProductExeName,
                License = new LicenseInfo
                {
                    Required = !string.IsNullOrWhiteSpace(package.LicenseAgreement),
                    Text = new LocalizedText
                    {
                        { "zh-Hans", package.LicenseAgreement ?? string.Empty },
                        { "en", string.Empty },
                    },
                },
                Ui = new UiInfo
                {
                    Theme = "light",
                    AccentColor = package.BackColorArgb.HasValue
                        ? "#" + (package.BackColorArgb.Value & 0xFFFFFF).ToString("X6", CultureInfo.InvariantCulture)
                        : "#069DE7",
                },
                Shortcuts = new List<ShortcutSpec>
                {
                    new ShortcutSpec
                    {
                        Location = "Desktop",
                        Name = new LocalizedText { { "zh-Hans", name }, { "en", name } },
                    },
                },
                Com = package.RegditFiles
                    .Select(f => new ComSpec { Path = @"bin\" + f, Mode = ComRegistrationModes.RegSvr32 })
                    .ToList(),
                Files = new List<FileEntry>(),
                Strings = new Dictionary<string, LocalizedText>(StringComparer.Ordinal),
            };

            var build = new ProjectBuildSettings
            {
                SourceDir = sourceDir,
                ComFiles = package.RegditFiles.Select(f => @"bin\" + f).ToList(),
                Timestamp = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture),
            };

            return new InstallerProject { Manifest = manifest, Build = build };
        }

        /// <summary>把 v1 包里 bin.zip 的条目名列出来（诊断用）。</summary>
        public static IList<string> ListPayloadEntries(LegacyPackage package)
        {
            var names = new List<string>();
            if (package == null || package.PayloadZip == null)
            {
                return names;
            }

            try
            {
                using (var ms = new MemoryStream(package.PayloadZip))
                using (var zip = new System.IO.Compression.ZipArchive(ms, System.IO.Compression.ZipArchiveMode.Read))
                {
                    foreach (var e in zip.Entries)
                    {
                        names.Add(e.FullName);
                    }
                }
            }
            catch (InvalidDataException ex)
            {
                names.Add("! 负载不是有效的 zip：" + ex.Message);
            }

            return names;
        }

        private static string AsString(object value)
        {
            if (value == null)
            {
                return null;
            }

            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }
    }
}
