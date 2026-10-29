using System;
using System.Collections.Generic;
using System.IO;
using Installer.Abstractions.Install;
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
