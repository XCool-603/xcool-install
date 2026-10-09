using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using Installer.Abstractions.Install;
using Installer.Abstractions.Model;
using Installer.Abstractions.Platform;
using Installer.Core.Install;
using Xunit;

namespace Installer.Core.Tests
{
    /// <summary>
    /// 安装引擎与回滚。这是整个重构里最要紧的一块 —— 现状完全没有回滚能力，
    /// 失败就留下一个坏掉的安装。
    /// </summary>
    public class InstallEngineTests : IDisposable
    {
        private readonly string _root;

        public InstallEngineTests()
        {
            _root = Path.Combine(Path.GetTempPath(), "engine-tests-" + Guid.NewGuid().ToString("N"));
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

        private InstallContext NewContext(InstallerManifest manifest = null, string installDir = null)
        {
            var platform = new FakePlatform(_root);
            manifest = manifest ?? TestManifest.Create();
            var dir = installDir ?? Path.Combine(_root, "app");

            return new InstallContext
            {
                Manifest = manifest,
                Mode = InstallMode.Install,
                InstallDir = dir,
                Platform = platform,
                Journal = new InstallJournal(),          // 内存 journal
                Log = new NullLogger(),
                Strings = manifest.Strings,
                Text = new StringTable(manifest.Strings, "zh-Hans", manifest.DefaultCulture),
                CancellationToken = CancellationToken.None,
            };
        }

        /// <summary>一个会写文件、写注册表、建快捷方式的"正常"步骤。</summary>
        private sealed class WriterStep : IInstallStep
        {
            public string Id
            {
                get { return "writer"; }
            }

            public string MessageKey
            {
                get { return "writer"; }
            }

            public bool NeedsPayload
            {
                get { return false; }
            }

            public void Execute(InstallContext ctx)
            {
                var dir = Path.Combine(ctx.InstallDir, "bin");
                JournalWriter.EnsureDirectory(ctx, Id, dir, null);

                JournalWriter.WriteFile(ctx, Id, Path.Combine(dir, "a.txt"),
                    p => File.WriteAllText(p, "hello"));

                JournalWriter.SetRegistryValue(ctx, Id, RegistryRoot.CurrentUser,
                    @"SOFTWARE\TestVendor\TestProduct", "InstallPath", ctx.InstallDir, false);

                JournalWriter.CreateShortcut(ctx, Id, Path.Combine(ctx.InstallDir, "test.lnk"),
                    Path.Combine(dir, "a.txt"), string.Empty, ctx.InstallDir, "test", null);

                ctx.InstalledFiles.Add(@"bin\a.txt");
            }
        }

        /// <summary>一个必定失败的步骤。</summary>
        private sealed class BoomStep : IInstallStep
        {
            public string Id
            {
                get { return "boom"; }
            }

            public string MessageKey
            {
                get { return "boom"; }
            }

            public bool NeedsPayload
            {
                get { return false; }
            }

            public void Execute(InstallContext ctx)
            {
                throw new InstallException(Id, "故意失败");
            }
        }

        // ─────────────────────────────────────────────────────────────

        [Fact]
        public void 全部成功时应提交并保留所有写入()
        {
            var ctx = NewContext();
            var engine = new InstallEngine(() => new List<IInstallStep> { new WriterStep() });

            var result = engine.Run(ctx);

            Assert.True(result.Success);
            Assert.False(result.RolledBack);
            Assert.Equal(1, result.StepsExecuted);

            Assert.True(File.Exists(Path.Combine(ctx.InstallDir, "bin", "a.txt")));
            Assert.True(ctx.Platform.Registry.ValueExists(RegistryRoot.CurrentUser,
                @"SOFTWARE\TestVendor\TestProduct", "InstallPath"));
            Assert.True(ctx.Platform.Shortcuts.Exists(Path.Combine(ctx.InstallDir, "test.lnk")));
        }

        [Fact]
        public void 中间失败时应逆序回滚全部写入()
        {
            var ctx = NewContext();
            var engine = new InstallEngine(() => new List<IInstallStep> { new WriterStep(), new BoomStep() });

            var result = engine.Run(ctx);

            Assert.False(result.Success);
            Assert.True(result.RolledBack);
            Assert.Equal("boom", result.FailedStepId);

            // 文件被撤销
            Assert.False(File.Exists(Path.Combine(ctx.InstallDir, "bin", "a.txt")));

            // 注册表值被撤销
            Assert.False(ctx.Platform.Registry.ValueExists(RegistryRoot.CurrentUser,
                @"SOFTWARE\TestVendor\TestProduct", "InstallPath"));

            // 快捷方式被撤销
            Assert.False(ctx.Platform.Shortcuts.Exists(Path.Combine(ctx.InstallDir, "test.lnk")));

            Assert.Empty(result.RollbackIssues);
        }

        [Fact]
        public void 回滚应把被覆盖的文件还原()
        {
            var ctx = NewContext();

            // 还原被覆盖的文件依赖备份目录 —— 内存 journal（无备份目录）做不到，
            // 所以生产路径（InstallHost）一定会传 journalPath + backupDir。
            ctx.Journal = new InstallJournal(
                Path.Combine(_root, "cover.jsonl"), Path.Combine(_root, "cover-backup"));

            // 预置一个会被覆盖的文件
            var dir = Path.Combine(ctx.InstallDir, "bin");
            Directory.CreateDirectory(dir);
            var path = Path.Combine(dir, "a.txt");
            File.WriteAllText(path, "ORIGINAL");

            var engine = new InstallEngine(() => new List<IInstallStep> { new WriterStep(), new BoomStep() });
            var result = engine.Run(ctx);

            Assert.True(result.RolledBack);
            Assert.True(File.Exists(path));
            Assert.Equal("ORIGINAL", File.ReadAllText(path));
        }

        [Fact]
        public void 没有备份目录时被覆盖的文件无法还原应被显式暴露()
        {
            // 记录这条限制：内存 journal 的 BackupFile 返回 null。
            // 生产路径必须传 backupDir，否则"覆盖后回滚"会退化成"删除"。
            var journal = new InstallJournal();
            var file = Path.Combine(_root, "nobackup.txt");
            File.WriteAllText(file, "x");

            Assert.Null(journal.BackupFile(file));
        }

        [Fact]
        public void 回滚应还原注册表的旧值而不是删除()
        {
            var ctx = NewContext();
            const string subKey = @"SOFTWARE\TestVendor\TestProduct";
            ctx.Platform.Registry.SetString(RegistryRoot.CurrentUser, subKey, "InstallPath", @"C:\OLD");

            var engine = new InstallEngine(() => new List<IInstallStep> { new WriterStep(), new BoomStep() });
            engine.Run(ctx);

            Assert.Equal(@"C:\OLD", ctx.Platform.Registry.GetString(RegistryRoot.CurrentUser, subKey, "InstallPath"));
        }

        [Fact]
        public void 取消时应回滚()
        {
            var ctx = NewContext();
            var cts = new CancellationTokenSource();
            cts.Cancel();
            ctx.CancellationToken = cts.Token;

            var engine = new InstallEngine(() => new List<IInstallStep> { new WriterStep() });
            var result = engine.Run(ctx);

            Assert.False(result.Success);
            Assert.True(result.RolledBack);
            Assert.False(File.Exists(Path.Combine(ctx.InstallDir, "bin", "a.txt")));
        }

        [Fact]
        public void 步骤执行到一半抛异常时已落下的写入也要被撤销()
        {
            // 这是"让每个步骤自己实现 Rollback"做不到的场景
            var ctx = NewContext();
            var engine = new InstallEngine(() => new List<IInstallStep> { new HalfWayStep() });

            var result = engine.Run(ctx);

            Assert.False(result.Success);
            Assert.True(result.RolledBack);
            Assert.False(File.Exists(Path.Combine(ctx.InstallDir, "written.txt")));
            Assert.False(ctx.Platform.Registry.ValueExists(RegistryRoot.CurrentUser,
                @"SOFTWARE\HalfWay", "Value"));
        }

        private sealed class HalfWayStep : IInstallStep
        {
            public string Id
            {
                get { return "halfway"; }
            }

            public string MessageKey
            {
                get { return "halfway"; }
            }

            public bool NeedsPayload
            {
                get { return false; }
            }

            public void Execute(InstallContext ctx)
            {
                JournalWriter.EnsureDirectory(ctx, Id, ctx.InstallDir, null);
                JournalWriter.WriteFile(ctx, Id, Path.Combine(ctx.InstallDir, "written.txt"),
                    p => File.WriteAllText(p, "x"));
                JournalWriter.SetRegistryValue(ctx, Id, RegistryRoot.CurrentUser,
                    @"SOFTWARE\HalfWay", "Value", "1", false);

                // 三个写操作之后才失败
                throw new InstallException(Id, "写到一半炸了");
            }
        }

        [Fact]
        public void 进度应报告每一步()
        {
            var ctx = NewContext();
            var reports = new List<InstallProgress>();
            var engine = new InstallEngine(() => new List<IInstallStep> { new WriterStep(), new WriterStep() });

            engine.Run(ctx, new Progress<InstallProgress>(reports.Add));
            Thread.Sleep(50);   // Progress<T> 是异步投递的

            Assert.Contains(reports, r => r.StepIndex == 1);
            Assert.Contains(reports, r => r.Percent == 100);
        }

        [Fact]
        public void 提交后应删除_journal_文件()
        {
            var ctx = NewContext();
            var journalPath = Path.Combine(_root, "journal.jsonl");
            ctx.Journal = new InstallJournal(journalPath, Path.Combine(_root, "backup"));

            var engine = new InstallEngine(() => new List<IInstallStep> { new WriterStep() });
            engine.Run(ctx);

            Assert.False(File.Exists(journalPath));
        }

        [Fact]
        public void 失败后_journal_文件应保留以便崩溃恢复()
        {
            var ctx = NewContext();
            var journalPath = Path.Combine(_root, "journal2.jsonl");
            ctx.Journal = new InstallJournal(journalPath, Path.Combine(_root, "backup2"));

            var engine = new InstallEngine(() => new List<IInstallStep> { new WriterStep(), new BoomStep() });
            engine.Run(ctx);

            // 回滚之后文件还在（内容可被 LoadPending 读回做诊断）
            Assert.True(File.Exists(journalPath));
            Assert.NotEmpty(InstallJournal.LoadPending(journalPath));
        }
    }

    /// <summary>预检里的版本比较 —— 现状用 <c>Int32.Parse(v.Replace(".",""))</c>，位数不同就误判。</summary>
    public class PreflightVersionTests
    {
        [Theory]
        [InlineData("1.0.0", "1.0.0", 0)]
        [InlineData("1.0", "1.0.0", -1)]          // 位数不同也要正确
        [InlineData("1.0.1", "1.0", 1)]
        [InlineData("4.16.5", "4.16.05", 0)]      // 旧实现会把这两个判成不同
        [InlineData("2.0.0.0", "1.9.9.9", 1)]
        public void 版本比较应正确(string a, string b, int expectedSign)
        {
            var actual = PreflightStep.CompareVersions(a, b);
            Assert.Equal(expectedSign, Math.Sign(actual));
        }

        [Fact]
        public void 非数字版本应回退到字符串比较且不抛异常()
        {
            var ex = Record.Exception(() => PreflightStep.CompareVersions("abc", "abd"));
            Assert.Null(ex);
            Assert.True(PreflightStep.CompareVersions("abc", "abd") < 0);
        }
    }

    /// <summary>多语言文案的回退链。</summary>
    public class StringTableTests
    {
        private static Dictionary<string, LocalizedText> Table()
        {
            return new Dictionary<string, LocalizedText>(StringComparer.Ordinal)
            {
                { "K1", new LocalizedText { { "zh-Hans", "中文" }, { "en", "English" } } },
                { "K2", new LocalizedText { { "zh-Hans", "只有中文" } } },
            };
        }

        [Fact]
        public void 精确语言命中()
        {
            var t = new StringTable(Table(), "en", "zh-Hans");
            Assert.Equal("English", t.Get("K1"));
        }

        [Fact]
        public void 语言主标签回退()
        {
            var t = new StringTable(Table(), "en-US", "zh-Hans");
            Assert.Equal("English", t.Get("K1"));
        }

        [Fact]
        public void 缺语言时回退到默认语言()
        {
            var t = new StringTable(Table(), "en", "zh-Hans");
            Assert.Equal("只有中文", t.Get("K2"));
        }

        [Fact]
        public void 未知语言回退到默认语言()
        {
            var t = new StringTable(Table(), "de-DE", "zh-Hans");
            Assert.Equal("中文", t.Get("K1"));
        }

        [Fact]
        public void 不存在的_key_返回_null()
        {
            var t = new StringTable(Table(), "en", "zh-Hans");
            Assert.Null(t.Get("NOPE"));
        }

        [Fact]
        public void Resolve_按语言取多语言字段()
        {
            var name = new LocalizedText { { "zh-Hans", "汽车小镇" }, { "en", "Auto Town" } };
            Assert.Equal("Auto Town", new StringTable(Table(), "en", "zh-Hans").Resolve(name, "?"));
            Assert.Equal("汽车小镇", new StringTable(Table(), "zh-Hans", "zh-Hans").Resolve(name, "?"));
            Assert.Equal("?", new StringTable(Table(), "en", "zh-Hans").Resolve(null, "?"));
        }
    }
}
