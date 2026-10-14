using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Installer.Abstractions.Install;
using Installer.Abstractions.Model;
using Installer.Abstractions.Packaging;
using Installer.Abstractions.Platform;
using Installer.Core.Packaging;
using Installer.Runtime;
using Installer.UI.Models;
using Xunit;

namespace Installer.Core.Tests
{
    /// <summary>
    /// 向导宿主（<see cref="WizardHostImpl"/>）。
    ///
    /// 这块以前是**测不到的**：<c>CreatePlatform</c> 直接 new 具体平台类型，
    /// 注入不了假的，于是整条向导安装路径只能靠 e2e 或手工点。
    /// 「没点完成就自动运行」那个 bug 正是从这条缝里漏过去的 ——
    /// 单测全绿、e2e 全绿，但用户体验是错的。
    ///
    /// 现在 WizardHostImpl 接受一个平台工厂，这里就能把它钉住。
    /// </summary>
    public class WizardHostTests : IDisposable
    {
        private readonly string _root;

        public WizardHostTests()
        {
            _root = Path.Combine(Path.GetTempPath(), "wizardhost-" + Guid.NewGuid().ToString("N"));
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

        /// <summary>造一份最小源目录 + 工程，并打出 Setup.exe。返回 (包路径, 清单)。</summary>
        private string PackMinimalSetup(InstallerManifest manifest, out string entryRelative)
        {
            var source = Path.Combine(_root, "source");
            Directory.CreateDirectory(Path.Combine(source, "bin"));
            File.WriteAllText(Path.Combine(source, "bin", "App.exe"), "not-a-real-exe");
            File.WriteAllText(Path.Combine(source, "readme.txt"), "hello");

            entryRelative = @"bin\App.exe";

            // 用**真实构建出的 stub**：PackageBuilder.Validate 会校验 MZ/PE 头，
            // 合成字节过不了（这一点跟 ContainerFormat 单测不同）。
            // 测试项目引用了 Installer.Runtime，它的 exe 会被复制到输出目录，
            // 所以不需要依赖仓库里的相对路径。
            var stub = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Installer.Runtime.exe");
            if (!File.Exists(stub))
            {
                throw new InvalidOperationException(
                    "找不到 stub —— Installer.Runtime.exe 应随项目引用复制到测试输出目录：" + stub);
            }

            var setup = Path.Combine(_root, "Setup.exe");
            var builder = new PackageBuilder();
            builder.Build(new PackageBuildOptions
            {
                Manifest = manifest,
                SourceDir = source,
                StubPath = stub,
                OutputPath = setup,
                SkipIconPatch = true,
                PackerVersion = "test",
                BuildTimestamp = new DateTimeOffset(2026, 10, 14, 0, 0, 0, TimeSpan.Zero),
            }, null);

            return setup;
        }

        private static InstallerManifest NewManifest()
        {
            return new InstallerManifest
            {
                SchemaVersion = 2,
                ProductCode = "{11111111-2222-3333-4444-555555555555}",
                DefaultCulture = "zh-Hans",
                Cultures = new List<string> { "zh-Hans", "en" },
                Product = new ProductInfo
                {
                    Name = new LocalizedText { { "zh-Hans", "向导测试产品" }, { "en", "Wizard Test" } },
                    Version = "1.0.0",
                },
                Scope = InstallScopes.PerUser,
                EntryPoint = @"bin\App.exe",
                Files = new List<FileEntry>(),
                Shortcuts = new List<ShortcutSpec>(),
                Strings = new Dictionary<string, LocalizedText>(StringComparer.Ordinal),
                RunAfterInstall = true,      // ← 关键：清单要求装完就运行
            };
        }

        [Fact]
        public void 向导安装_清单要求装完运行_但管线不得抢先启动()
        {
            var manifest = NewManifest();
            string entryRelative;
            var setup = PackMinimalSetup(manifest, out entryRelative);

            var installDir = Path.Combine(_root, "installed");
            var platform = new FakePlatform(_root);
            var logger = new NullLogger();

            var host = new WizardHostImpl(
                setup, null, Path.Combine(_root, "wizard.log"), "zh-Hans",
                manifest, installDir, uninstallMode: false,
                productCode: null,
                platformFactory: _ => platform);

            using (var done = new ManualResetEventSlim(false))
            {
                var ok = false;
                var detail = string.Empty;

                host.StartInstall(
                    new WizardOptions { InstallDir = installDir, RunAfterInstall = true },
                    null,
                    (success, message) => { ok = success; detail = message; done.Set(); });

                Assert.True(done.Wait(TimeSpan.FromSeconds(30)), "安装没有在 30 秒内结束：" + detail);
                Assert.True(ok, "安装失败：" + detail);
            }

            // 核心断言：文件装上了，但**入口程序没有被启动**。
            // 要不要启动由「安装完成」页的勾选框 + 完成按钮决定。
            Assert.True(File.Exists(Path.Combine(installDir, entryRelative)),
                "入口程序应当已被解密释放到安装目录");

            var started = ((FakeProcessRunner)platform.Processes).Started;
            Assert.True(started.Count == 0,
                "向导安装时管线不该启动入口程序，实际启动了：" + string.Join(", ", started));
        }

        [Fact]
        public void 向导的_LaunchEntryPoint_应由完成按钮调用()
        {
            var manifest = NewManifest();
            string entryRelative;
            var setup = PackMinimalSetup(manifest, out entryRelative);

            var installDir = Path.Combine(_root, "installed2");
            var platform = new FakePlatform(_root);

            var host = new WizardHostImpl(
                setup, null, Path.Combine(_root, "wizard2.log"), "zh-Hans",
                manifest, installDir, uninstallMode: false,
                productCode: null,
                platformFactory: _ => platform);

            // LaunchEntryPoint 用的是 Process.Start（真实启动），不是平台服务，
            // 所以这里只断言「入口程序不存在时它安静地什么都不做」——
            // 避免在单测里真的拉起一个进程。
            var ex = Record.Exception(() => host.LaunchEntryPoint());
            Assert.Null(ex);
        }
    }
}
