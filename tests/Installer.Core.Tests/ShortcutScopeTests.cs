using System;
using System.IO;
using System.Threading;
using Installer.Abstractions.Install;
using Installer.Abstractions.Model;
using Installer.Abstractions.Platform;
using Installer.Core.Install;
using Xunit;

namespace Installer.Core.Tests
{
    /// <summary>
    /// 快捷方式要按**安装范围**落到对应的目录。
    ///
    /// 用户反馈「勾了创建桌面快捷方式，装完桌面上没有」的根因就在这里：
    /// 「所有用户」安装仍然往**当前用户**桌面写。提权时如果输入的是另一个管理员账号，
    /// 快捷方式会进那个账号的配置文件 —— 装它的人自己都看不到，
    /// 而控制面板里却确实有这个程序。
    /// </summary>
    public class ShortcutScopeTests : IDisposable
    {
        private readonly string _root;

        public ShortcutScopeTests()
        {
            _root = Path.Combine(Path.GetTempPath(), "shortcut-scope-" + Guid.NewGuid().ToString("N"));
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

        private InstallContext NewContext(string scope, params string[] locations)
        {
            var manifest = TestManifest.Create(scope: scope);
            manifest.Shortcuts = new System.Collections.Generic.List<ShortcutSpec>();
            foreach (var location in locations)
            {
                manifest.Shortcuts.Add(new ShortcutSpec
                {
                    Location = location,
                    Name = new LocalizedText { { "zh-Hans", "测试产品" } },
                });
            }

            var platform = new FakePlatform(_root);

            return new InstallContext
            {
                Manifest = manifest,
                Mode = InstallMode.Install,
                InstallDir = Path.Combine(_root, "app"),
                Platform = platform,
                Journal = new InstallJournal(),
                Log = new NullLogger(),
                Strings = manifest.Strings,
                Text = new StringTable(manifest.Strings, "zh-Hans", manifest.DefaultCulture),
                CancellationToken = CancellationToken.None,
            };
        }

        [Fact]
        public void 所有用户安装应落到公共桌面与公共开始菜单()
        {
            var ctx = NewContext(InstallScopes.PerMachine, "Desktop", "StartMenu");
            Directory.CreateDirectory(ctx.InstallDir);

            new ShortcutsStep().Execute(ctx);

            var commonDesktop = ctx.Platform.Environment.GetFolder(SpecialFolderKind.CommonDesktop);
            var commonStartMenu = Path.Combine(ctx.Platform.Environment.GetFolder(SpecialFolderKind.CommonStartMenu), "测试产品");

            Assert.Contains(ctx.CreatedShortcuts, p => p.StartsWith(commonDesktop, StringComparison.OrdinalIgnoreCase));
            Assert.Contains(ctx.CreatedShortcuts, p => p.StartsWith(commonStartMenu, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void 仅当前用户安装应落到用户自己的桌面()
        {
            var ctx = NewContext(InstallScopes.PerUser, "Desktop");
            Directory.CreateDirectory(ctx.InstallDir);

            new ShortcutsStep().Execute(ctx);

            var desktop = ctx.Platform.Environment.GetFolder(SpecialFolderKind.Desktop);
            var commonDesktop = ctx.Platform.Environment.GetFolder(SpecialFolderKind.CommonDesktop);

            Assert.Contains(ctx.CreatedShortcuts, p => p.StartsWith(desktop, StringComparison.OrdinalIgnoreCase));
            Assert.DoesNotContain(ctx.CreatedShortcuts, p => p.StartsWith(commonDesktop, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void 清单里没有快捷方式时一个都不建()
        {
            var ctx = NewContext(InstallScopes.PerMachine);
            Directory.CreateDirectory(ctx.InstallDir);

            new ShortcutsStep().Execute(ctx);

            Assert.Empty(ctx.CreatedShortcuts);
        }
    }
}
