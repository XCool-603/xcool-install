using System;
using System.Collections.Generic;
using System.IO;
using Installer.Abstractions.Install;
using Installer.Abstractions.Model;
using Installer.Runtime;
using Xunit;

namespace Installer.Core.Tests
{
    /// <summary>
    /// 「已经装过这个产品吗？」的探测 —— 更新流程的入口。
    ///
    /// 用户诉求：已经装了 1.0.0 再装新包时，向导要跳过「安装设置」页，
    /// 改成显示"当前已安装 1.0.0 / 将要安装 2.0.0"，按钮变「更新」。
    /// 这一切的前提是**能准确读到已安装版本**，这里钉住这条读取。
    /// </summary>
    public class InstalledDetectionTests : IDisposable
    {
        private readonly string _root;

        public InstalledDetectionTests()
        {
            _root = Path.Combine(Path.GetTempPath(), "detect-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_root);
        }

        public void Dispose()
        {
            try
            {
                Directory.Delete(_root, true);
            }
            catch (IOException)
            {
            }
        }

        private string WriteRecord(string version)
        {
            var dir = Path.Combine(_root, "prod-" + version);
            Directory.CreateDirectory(dir);

            var record = new InstallRecord
            {
                ProductCode = "{AAAA1111-2222-3333-4444-555566667777}",
                ProductName = "探测测试",
                ProductVersion = version,
                InstallDir = dir,
                Files = new List<string>(),
                FileHashes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                Directories = new List<string>(),
                Shortcuts = new List<string>(),
                RegistryValues = new List<RegistryValueRecord>(),
                LegacyKeys = new List<string>(),
            };

            File.WriteAllText(InstallRecord.GetPath(dir), Newtonsoft.Json.JsonConvert.SerializeObject(record));
            return dir;
        }

        [Fact]
        public void 有安装记录时应读出已安装版本与目录()
        {
            var dir = WriteRecord("1.0.0");

            var installed = InstallHost.TryReadInstalled(dir);

            Assert.NotNull(installed);
            Assert.Equal("1.0.0", installed.Version);
            Assert.Equal(dir, installed.InstallDir);
        }

        [Fact]
        public void productCode_由启动程序名决定_同名同码异名异码()
        {
            // 用户要求：相同 exe 名 → 同一个 productCode；不同 exe 名 → 不同 productCode。
            var a1 = ProductCodes.FromEntryPoint("ProductA.exe");
            var a2 = ProductCodes.FromEntryPoint(@"bin\ProductA.exe");
            var a3 = ProductCodes.FromEntryPoint("producta.exe");     // 大小写不敏感
            var b = ProductCodes.FromEntryPoint("ProductB.exe");

            Assert.Equal(a1, a2);
            Assert.Equal(a1, a3);
            Assert.NotEqual(a1, b);

            Assert.Matches("^\\{[0-9A-F]{8}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{12}\\}$", a1);

            Assert.Null(ProductCodes.FromEntryPoint(null));
            Assert.Null(ProductCodes.FromEntryPoint("   "));
        }

        [Fact]
        public void 同一个_productCode_下的别的产品不算已安装()
        {
            // 用户实测：把同一个工程另存成 3 个项目，productCode 被一起复制，
            // 装第 2 个产品时提示"更新"。修法是**必须确认是同一个产品**。
            var manifest = new Installer.Abstractions.Model.InstallerManifest
            {
                Product = new Installer.Abstractions.Model.ProductInfo
                {
                    Name = new Installer.Abstractions.Model.LocalizedText
                    {
                        { "zh-Hans", "产品乙" }, { "en", "Product B" },
                    },
                },
            };

            Assert.True(InstallHost.MatchesProductName(manifest, "产品乙"));
            Assert.True(InstallHost.MatchesProductName(manifest, " Product B "));
            Assert.True(InstallHost.MatchesProductName(manifest, "product b"));

            // 另一个产品 → 不算本产品
            Assert.False(InstallHost.MatchesProductName(manifest, "产品甲"));
            Assert.False(InstallHost.MatchesProductName(manifest, "Product A"));
            Assert.False(InstallHost.MatchesProductName(manifest, null));
        }
        [Fact]
        public void 没有安装记录时返回空()
        {
            var empty = Path.Combine(_root, "empty");
            Directory.CreateDirectory(empty);

            Assert.Null(InstallHost.TryReadInstalled(empty));
            Assert.Null(InstallHost.TryReadInstalled(null));
            Assert.Null(InstallHost.TryReadInstalled(Path.Combine(_root, "does-not-exist")));
        }
    }
}
