using System;
using System.Collections.Generic;
using System.IO;
using Installer.Abstractions.Install;
using Installer.Abstractions.Model;
using Installer.Core.Uninstall;
using Xunit;

namespace Installer.Core.Tests
{
    /// <summary>
    /// 卸载要**把产品目录清空**。
    ///
    /// 用户反馈的原话是「卸载逻辑现在有问题，不是清空整理产品目录」：
    /// 以前只按安装记录精确删文件、再删"已经空了的"目录，于是
    /// 程序运行时写进安装目录的任何东西（Log、缓存、改过的配置）
    /// 都留在那里，产品目录永远删不掉。
    ///
    /// 现在的边界（也是这几条测试钉住的东西）：
    ///   · 产品目录 = <c>{根}\{厂商}\{产品}</c>，是我们独占的 → 整棵清空；
    ///   · 共享容器（Program Files / Programs / 只有一层的目录）→ 只删自己的东西，绝不整目录删；
    ///   · 降级卸载（安装记录丢了，分不清哪些是用户的）→ 保守，只删已知残留。
    /// </summary>
    public class UninstallWipeTests : IDisposable
    {
        private readonly string _root;

        public UninstallWipeTests()
        {
            _root = Path.Combine(Path.GetTempPath(), "wipe-tests-" + Guid.NewGuid().ToString("N"));
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

        private InstallRecord NewRecord(string installDir, string vendor)
        {
            return new InstallRecord
            {
                ProductCode = "{AAAAAAAA-0000-0000-0000-000000000000}",
                ProductName = "空中小火车",
                VendorName = vendor,
                ProductVersion = "1.0.0",
                Scope = InstallScopes.PerMachine,
                InstallDir = installDir,
                EntryPoint = "App.exe",
                Files = new List<string> { "App.exe" },
                FileHashes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
            };
        }

        [Fact]
        public void 有记录时应把产品目录整个清空_并回收空的厂商目录()
        {
            var vendorDir = Path.Combine(_root, "Program Files (x86)", "唯非工作室");
            var productDir = Path.Combine(vendorDir, "空中小火车");
            Directory.CreateDirectory(Path.Combine(productDir, "sub"));
            Directory.CreateDirectory(Path.Combine(productDir, "Log"));

            File.WriteAllText(Path.Combine(productDir, "App.exe"), "x");
            File.WriteAllText(Path.Combine(productDir, "Uninstall.exe"), "stub");
            File.WriteAllText(Path.Combine(productDir, "sub", "b.dat"), "x");
            File.WriteAllText(Path.Combine(productDir, "Log", "run.log"), "x");
            // 运行时写进去、安装记录里根本没有的文件
            File.WriteAllText(Path.Combine(productDir, "user-setting.ini"), "x");

            var platform = new FakePlatform(_root);
            var record = NewRecord(productDir, "唯非工作室");

            var result = new UninstallEngine().Run(record, platform);

            Assert.True(result.Success, string.Join("；", result.Issues.ToArray()));
            Assert.False(Directory.Exists(productDir), "产品目录没有被清空");

            // 厂商目录空了，应该一并回收，别在 Program Files 下留空壳
            Assert.False(Directory.Exists(vendorDir), "空的厂商目录应被回收");
        }

        [Fact]
        public void 厂商目录里还有别的产品时应保留()
        {
            var vendorDir = Path.Combine(_root, "Program Files (x86)", "唯非工作室");
            var productDir = Path.Combine(vendorDir, "空中小火车");
            var sibling = Path.Combine(vendorDir, "另一个产品");
            Directory.CreateDirectory(productDir);
            Directory.CreateDirectory(sibling);

            File.WriteAllText(Path.Combine(productDir, "App.exe"), "x");
            File.WriteAllText(Path.Combine(sibling, "Other.exe"), "x");

            var platform = new FakePlatform(_root);
            var result = new UninstallEngine().Run(NewRecord(productDir, "唯非工作室"), platform);

            Assert.True(result.Success);
            Assert.False(Directory.Exists(productDir));
            Assert.True(Directory.Exists(sibling), "不该碰同厂商的其它产品");
            Assert.True(Directory.Exists(vendorDir), "厂商目录非空时必须保留");
        }

        [Fact]
        public void 安装目录是共享容器时不应整目录删除()
        {
            // 老包（没分层）会装到 D:\Program Files (x86) 这种公共根里
            var shared = Path.Combine(_root, "Program Files (x86)");
            Directory.CreateDirectory(Path.Combine(shared, "别家软件"));
            File.WriteAllText(Path.Combine(shared, "别家软件", "other.exe"), "x");
            File.WriteAllText(Path.Combine(shared, "App.exe"), "x");

            var platform = new FakePlatform(_root);
            var record = NewRecord(shared, null);
            record.Scope = InstallScopes.PerMachine;

            var result = new UninstallEngine().Run(record, platform);

            Assert.True(result.Success, string.Join("；", result.Issues.ToArray()));
            Assert.False(File.Exists(Path.Combine(shared, "App.exe")), "自己的文件还是要删掉");
            Assert.True(File.Exists(Path.Combine(shared, "别家软件", "other.exe")), "别人的程序不能陪葬");
            Assert.True(Directory.Exists(shared), "共享容器本身不能删");
        }

        [Fact]
        public void 降级卸载分不清归属时应保留用户文件()
        {
            var productDir = Path.Combine(_root, "Programs", "空中小火车");
            Directory.CreateDirectory(productDir);
            var userFile = Path.Combine(productDir, "我的存档.dat");
            File.WriteAllText(userFile, "重要数据");

            var platform = new FakePlatform(_root);
            new UninstallEngine().RunWithoutRecord(productDir, platform);

            Assert.True(File.Exists(userFile), "没有安装记录时不该赌哪份是用户数据");
        }
    }
}
