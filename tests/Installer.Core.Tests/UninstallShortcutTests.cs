using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Installer.Abstractions.Install;
using Installer.Abstractions.Model;
using Installer.Abstractions.Platform;
using Installer.Core.Install;
using Xunit;

namespace Installer.Core.Tests
{
    /// <summary>
    /// 开始菜单里的「卸载」快捷方式。
    ///
    /// 用户诉求：卸载只能去「设置 → 应用」，太绕；希望开始菜单里点一下就能卸。
    /// 这里钉住三件事：
    /// ① 它真的建出来（跟着 scope 落公共/用户开始菜单）；
    /// ② 命令行带 <c>--uninstall --product-code</c>（安装记录丢了也能定位卸载项）；
    /// ③ 它进了 <c>CreatedShortcuts</c> —— 也就是**卸载时会被删掉**，
    ///    否则卸完还留一个指向已删除文件的死链接。
    /// </summary>
    public class UninstallShortcutTests : IDisposable
    {
        private readonly string _root;

        public UninstallShortcutTests()
        {
            _root = Path.Combine(Path.GetTempPath(), "uninsscut-" + Guid.NewGuid().ToString("N"));
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

        private InstallContext NewContext(string scope, bool createUninstaller = true)
        {
            var manifest = TestManifest.Create(scope: scope);
            manifest.Shortcuts = new List<ShortcutSpec>();

            var platform = new FakePlatform(_root);
            var dir = Path.Combine(_root, "app");
            Directory.CreateDirectory(dir);

            if (createUninstaller)
            {
                File.WriteAllText(Path.Combine(dir, InstallRecord.UninstallerFileName), "stub");
            }

            return new InstallContext
            {
                Manifest = manifest,
                Mode = InstallMode.Install,
                InstallDir = dir,
                Platform = platform,
                Journal = new InstallJournal(),
                Log = new NullLogger(),
                Strings = manifest.Strings,
                Text = new StringTable(manifest.Strings, "zh-Hans", manifest.DefaultCulture),
                CancellationToken = CancellationToken.None,
            };
        }

        [Fact]
        public void 所有用户安装应在公共开始菜单建卸载快捷方式()
        {
            var ctx = NewContext(InstallScopes.PerMachine);

            new UninstallShortcutStep().Execute(ctx);

            // 卸载入口放在**产品文件夹**里：平铺在开始菜单根上时 Windows 的「所有应用」不收录
            var folder = Path.Combine(ctx.Platform.Environment.GetFolder(SpecialFolderKind.CommonStartMenu), "测试产品");
            var link = Path.Combine(folder, "卸载.lnk");

            Assert.Contains(link, ctx.CreatedShortcuts);
            Assert.True(ctx.Platform.Shortcuts.Exists(link), "卸载快捷方式没有真的建出来");
        }

        [Fact]
        public void 仅当前用户安装应建到用户自己的开始菜单()
        {
            var ctx = NewContext(InstallScopes.PerUser);

            new UninstallShortcutStep().Execute(ctx);

            var folder = Path.Combine(ctx.Platform.Environment.GetFolder(SpecialFolderKind.StartMenu), "测试产品");
            Assert.Contains(Path.Combine(folder, "卸载.lnk"), ctx.CreatedShortcuts);
        }

        [Fact]
        public void 快捷方式应指向非_Uninstall_入口并带上_product_code()
        {
            // 目标**不能**是 Uninstall.exe：Windows 会把这种条目判定成卸载程序，
            // 故意不放进开始菜单的「最近添加」（实测结论）。
            var ctx = NewContext(InstallScopes.PerMachine);

            new UninstallShortcutStep().Execute(ctx);

            var fake = (FakeShortcutService)ctx.Platform.Shortcuts;
            var link = ctx.CreatedShortcuts.Single();

            var expectedTarget = Path.Combine(ctx.InstallDir, InstallRecord.MenuUninstallerFileName);
            Assert.Equal(expectedTarget, fake.Targets[link]);
            Assert.Equal("卸载.exe", Path.GetFileName(fake.Targets[link]));
            Assert.True(File.Exists(expectedTarget), "开始菜单入口文件没有落盘");

            Assert.Contains("--uninstall", fake.Arguments[link]);
            Assert.Contains(ctx.Manifest.ProductCode, fake.Arguments[link]);
        }

        [Fact]
        public void 没有卸载器时不应建死链接()
        {
            var ctx = NewContext(InstallScopes.PerMachine, createUninstaller: false);

            new UninstallShortcutStep().Execute(ctx);

            Assert.Empty(ctx.CreatedShortcuts);
        }
    }
}
