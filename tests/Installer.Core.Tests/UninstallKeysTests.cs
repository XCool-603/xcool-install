using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Installer.Abstractions.Install;
using Installer.Abstractions.Platform;
using Installer.Core.Install;
using Installer.Core.Uninstall;
using Xunit;

namespace Installer.Core.Tests
{
    /// <summary>
    /// 卸载键的定位与清理。
    ///
    /// 这一组测试对应一次真实事故：制作端每次打开都生成新的 <c>productCode</c>，
    /// 同一个产品装三次就在「程序和功能」里留下三条记录；卸掉其中一条后安装记录被删，
    /// 剩下的**再也卸不掉**（用户点了没反应）。
    /// </summary>
    public class UninstallKeysTests : IDisposable
    {
        private readonly string _root;

        public UninstallKeysTests()
        {
            _root = Path.Combine(Path.GetTempPath(), "uninstallkeys-" + Guid.NewGuid().ToString("N"));
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

        private static string Code(string n)
        {
            return "{00000000-0000-0000-0000-00000000000" + n + "}";
        }

        /// <summary>造一条卸载键。</summary>
        private static void AddKey(FakeRegistryStore reg, RegistryRoot hive, string code,
                                   string displayName, string installLocation)
        {
            var sub = UninstallKeys.SubKeyRoot + "\\" + code;
            reg.SetString(hive, sub, "DisplayName", displayName);
            reg.SetString(hive, sub, "InstallLocation", installLocation);
            reg.SetString(hive, sub, "UninstallString", "\"x\" --uninstall");
        }

        // ─────────────────────────────────────────────────────────────
        // 路径比较
        // ─────────────────────────────────────────────────────────────

        [Fact]
        public void 同一目录的不同写法应被判为相同()
        {
            Assert.True(UninstallKeys.SameDirectory(@"C:\A\B", @"C:\A\B\"));
            Assert.True(UninstallKeys.SameDirectory(@"C:\A\B", @"c:\a\b"));
            Assert.True(UninstallKeys.SameDirectory(@"C:\A\B\", @"C:\A\B\\"));
        }

        [Fact]
        public void 不同目录不应被判为相同()
        {
            Assert.False(UninstallKeys.SameDirectory(@"C:\A\B", @"C:\A\C"));
            Assert.False(UninstallKeys.SameDirectory(@"C:\A\B", null));
            Assert.False(UninstallKeys.SameDirectory(null, null));
        }

        // ─────────────────────────────────────────────────────────────
        // 找孤儿
        // ─────────────────────────────────────────────────────────────

        [Fact]
        public void 应找出同目录但不同_productCode_的残留键()
        {
            var reg = new FakeRegistryStore();
            var dir = @"C:\Programs\空中小火车";

            // 用户实际遇到的情况：同一个目录，三个不同的 productCode
            AddKey(reg, RegistryRoot.CurrentUser, Code("1"), "空中小火车", dir);
            AddKey(reg, RegistryRoot.CurrentUser, Code("2"), "空中小火车", dir);
            AddKey(reg, RegistryRoot.CurrentUser, Code("3"), "空中小火车", dir);

            var orphans = UninstallKeys.FindOrphans(reg, dir, Code("3"));

            Assert.Equal(2, orphans.Count);
            Assert.DoesNotContain(orphans, k => k.KeyName == Code("3"));
            Assert.All(orphans, k => Assert.Equal("空中小火车", k.DisplayName));
        }

        [Fact]
        public void 不同安装目录的键不应被当成孤儿()
        {
            var reg = new FakeRegistryStore();

            AddKey(reg, RegistryRoot.CurrentUser, Code("1"), "别的程序", @"C:\Programs\别的程序");
            AddKey(reg, RegistryRoot.CurrentUser, Code("2"), "空中小火车", @"C:\Programs\空中小火车");

            var orphans = UninstallKeys.FindOrphans(reg, @"C:\Programs\空中小火车", Code("2"));

            Assert.Empty(orphans);
        }

        [Fact]
        public void 没有_InstallLocation_的键不应被误删()
        {
            var reg = new FakeRegistryStore();
            // 有些程序不写 InstallLocation —— 不能因为"读不到"就当成孤儿
            reg.SetString(RegistryRoot.CurrentUser, UninstallKeys.SubKeyRoot + "\\" + Code("9"),
                "DisplayName", "某个别的程序");

            Assert.Empty(UninstallKeys.FindOrphans(reg, @"C:\Programs\空中小火车", Code("1")));
        }

        [Fact]
        public void 卸载键可以在两个配置单元里被找到()
        {
            var reg = new FakeRegistryStore();
            var dir = @"C:\Programs\空中小火车";

            AddKey(reg, RegistryRoot.CurrentUser, Code("1"), "空中小火车", dir);
            AddKey(reg, RegistryRoot.LocalMachine, Code("2"), "空中小火车", dir);

            var all = UninstallKeys.FindByInstallLocation(reg, dir);

            Assert.Equal(2, all.Count);
            Assert.Contains(all, k => k.Hive == RegistryRoot.CurrentUser);
            Assert.Contains(all, k => k.Hive == RegistryRoot.LocalMachine);
        }

        [Fact]
        public void 按键名应能精确定位()
        {
            var reg = new FakeRegistryStore();
            AddKey(reg, RegistryRoot.CurrentUser, Code("7"), "空中小火车", @"C:\X");

            var found = UninstallKeys.FindByProductCode(reg, Code("7"));

            Assert.NotNull(found);
            Assert.Equal("空中小火车", found.DisplayName);
            Assert.Null(UninstallKeys.FindByProductCode(reg, Code("8")));
        }
    }

    /// <summary>降级卸载：安装记录丢失时也要能清干净。</summary>
    public class DegradedUninstallTests : IDisposable
    {
        private readonly string _root;

        public DegradedUninstallTests()
        {
            _root = Path.Combine(Path.GetTempPath(), "degraded-" + Guid.NewGuid().ToString("N"));
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

        private static void AddKey(FakeRegistryStore reg, RegistryRoot hive, string code,
                                   string displayName, string installLocation)
        {
            var sub = UninstallKeys.SubKeyRoot + "\\" + code;
            reg.SetString(hive, sub, "DisplayName", displayName);
            reg.SetString(hive, sub, "InstallLocation", installLocation);
        }

        [Fact]
        public void 没有记录时也应删掉指向本目录的全部卸载键()
        {
            var installDir = Path.Combine(_root, "Programs", "空中小火车");
            Directory.CreateDirectory(installDir);

            var platform = new FakePlatform(_root);
            var dir = installDir;

            AddKey((FakeRegistryStore)platform.Registry, RegistryRoot.CurrentUser,
                "{A}", "空中小火车", dir);
            AddKey((FakeRegistryStore)platform.Registry, RegistryRoot.CurrentUser,
                "{B}", "空中小火车", dir);

            var result = new UninstallEngine().RunWithoutRecord(installDir, platform, "{B}");

            Assert.True(result.Success);

            // 两条都该没了 —— 控制面板里不该再显示
            Assert.Empty(UninstallKeys.FindByInstallLocation(platform.Registry, dir));
        }

        [Fact]
        public void 降级卸载应删掉残留的记录与日志目录()
        {
            var installDir = Path.Combine(_root, "Programs", "空中小火车");
            Directory.CreateDirectory(Path.Combine(installDir, "Log"));
            File.WriteAllText(Path.Combine(installDir, "Log", "install.log"), "x");
            File.WriteAllText(Path.Combine(installDir, "Uninstall.exe"), "stub");
            File.WriteAllText(Path.Combine(installDir, ".install-journal.jsonl"), "{}");

            var platform = new FakePlatform(_root);
            var result = new UninstallEngine().RunWithoutRecord(installDir, platform);

            Assert.True(result.Success);
            Assert.False(Directory.Exists(Path.Combine(installDir, "Log")));
            Assert.False(File.Exists(Path.Combine(installDir, ".install-journal.jsonl")));
        }

        [Fact]
        public void 降级卸载必须保留用户自己放的文件()
        {
            var installDir = Path.Combine(_root, "Programs", "空中小火车");
            Directory.CreateDirectory(installDir);
            var userFile = Path.Combine(installDir, "我的存档.dat");
            File.WriteAllText(userFile, "重要数据");

            var platform = new FakePlatform(_root);
            new UninstallEngine().RunWithoutRecord(installDir, platform);

            Assert.True(File.Exists(userFile));
            Assert.Equal("重要数据", File.ReadAllText(userFile));
        }

        [Fact]
        public void 降级卸载不应碰别的产品的卸载键()
        {
            var installDir = Path.Combine(_root, "Programs", "空中小火车");
            Directory.CreateDirectory(installDir);

            var platform = new FakePlatform(_root);
            AddKey((FakeRegistryStore)platform.Registry, RegistryRoot.CurrentUser,
                "{MINE}", "空中小火车", installDir);
            AddKey((FakeRegistryStore)platform.Registry, RegistryRoot.CurrentUser,
                "{OTHER}", "别的程序", Path.Combine(_root, "Programs", "别的程序"));

            new UninstallEngine().RunWithoutRecord(installDir, platform);

            Assert.NotNull(UninstallKeys.FindByProductCode(platform.Registry, "{OTHER}"));
        }

        [Fact]
        public void 目录不存在时降级卸载不应抛异常()
        {
            var platform = new FakePlatform(_root);
            var result = new UninstallEngine().RunWithoutRecord(
                Path.Combine(_root, "根本不存在"), platform);

            Assert.True(result.Success);
        }
    }
}
