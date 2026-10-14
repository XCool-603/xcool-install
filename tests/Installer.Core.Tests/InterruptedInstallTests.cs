using System;
using System.Collections.Generic;
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
    /// 被中断安装的恢复。
    ///
    /// 写前日志 + 逆序 undo 原本只在「本次安装中途失败」时生效。
    /// 如果进程被杀、断电、或用户在任务管理器里结束进程，
    /// 日志与 .rollback 备份目录就留在磁盘上没人管 ——
    /// 已经写进去的文件 / 注册表值成了孤儿。
    ///
    /// PreflightStep 现在会在安装开始前把上一次的账结掉。
    /// </summary>
    public class InterruptedInstallTests : IDisposable
    {
        private readonly string _root;

        public InterruptedInstallTests()
        {
            _root = Path.Combine(Path.GetTempPath(), "interrupted-" + Guid.NewGuid().ToString("N"));
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

        private InstallContext NewContext(string installDir, string journalPath, string backupDir,
                                          IPlatformServices platform = null)
        {
            var manifest = new InstallerManifest
            {
                SchemaVersion = 2,
                ProductCode = "{99999999-8888-7777-6666-555555555555}",
                DefaultCulture = "zh-Hans",
                Cultures = new List<string> { "zh-Hans" },
                Product = new ProductInfo
                {
                    Name = new LocalizedText { { "zh-Hans", "中断测试产品" } },
                    Version = "1.0.0",
                },
                Scope = InstallScopes.PerUser,
                EntryPoint = "app.exe",
                Files = new List<FileEntry> { new FileEntry { Path = "app.exe", Size = 1024 } },
                Shortcuts = new List<ShortcutSpec>(),
                Strings = new Dictionary<string, LocalizedText>(StringComparer.Ordinal),
            };

            return new InstallContext
            {
                Manifest = manifest,
                Mode = InstallMode.Install,
                InstallDir = installDir,
                Platform = platform ?? new FakePlatform(_root),
                Journal = new InstallJournal(journalPath, backupDir),
                Log = new NullLogger(),
                Strings = manifest.Strings,
                Text = new StringTable(manifest.Strings, "zh-Hans", manifest.DefaultCulture),
                CancellationToken = CancellationToken.None,
            };
        }

        /// <summary>
        /// 造一次「被杀掉的安装」：用真实 journal 记下动作，但**不 Commit**
        /// —— 这正是进程被强杀后磁盘上的样子。
        /// </summary>
        private void SimulateInterruptedInstall(string installDir, string journalPath, string backupDir,
                                                out string orphanFile, out string orphanDir)
        {
            Directory.CreateDirectory(installDir);
            var journal = new InstallJournal(journalPath, backupDir);

            orphanFile = Path.Combine(installDir, "leftover.txt");
            File.WriteAllText(orphanFile, "上次没装完留下的");
            journal.Record(new JournalEntry
            {
                StepId = "extract",
                Op = JournalOps.WriteFile,
                Target = orphanFile,
                ExistedBefore = false,
            });

            orphanDir = Path.Combine(installDir, "subdir");
            Directory.CreateDirectory(orphanDir);
            journal.Record(new JournalEntry
            {
                StepId = "extract",
                Op = JournalOps.CreateDirectory,
                Target = orphanDir,
                ExistedBefore = false,
            });
        }

        [Fact]
        public void 上次安装被中断_再装时应当先回滚残留()
        {
            var installDir = Path.Combine(_root, "app");
            var journalPath = Path.Combine(installDir, ".install-journal.jsonl");
            var backupDir = Path.Combine(installDir, ".rollback");

            string orphanFile, orphanDir;
            SimulateInterruptedInstall(installDir, journalPath, backupDir, out orphanFile, out orphanDir);
            Assert.True(File.Exists(orphanFile), "前置条件：应当存在上次留下的文件");

            var ctx = NewContext(installDir, journalPath, backupDir);
            new PreflightStep().Execute(ctx);

            Assert.False(File.Exists(orphanFile), "被中断的安装写下的文件应当被回滚删掉");
            Assert.False(Directory.Exists(orphanDir), "被中断的安装建的空目录应当被回滚删掉");
            Assert.False(File.Exists(journalPath), "回滚完成后残留日志应当被清掉");
        }

        [Fact]
        public void 没有残留时_预检不应受影响()
        {
            var installDir = Path.Combine(_root, "clean");
            var journalPath = Path.Combine(installDir, ".install-journal.jsonl");
            var backupDir = Path.Combine(installDir, ".rollback");

            // 预先放一个用户文件，确认它不会被误删
            Directory.CreateDirectory(installDir);
            var userFile = Path.Combine(installDir, "用户数据.txt");
            File.WriteAllText(userFile, "别动我");

            var ctx = NewContext(installDir, journalPath, backupDir);
            new PreflightStep().Execute(ctx);

            Assert.True(File.Exists(userFile), "预检不该动用户自己的文件");
            Assert.True(Directory.Exists(installDir));
            Assert.False(File.Exists(journalPath), "没有残留时不该凭空造出日志");
        }

        [Fact]
        public void 残留日志覆盖过的文件_回滚后应恢复原内容()
        {
            var installDir = Path.Combine(_root, "restore");
            var journalPath = Path.Combine(installDir, ".install-journal.jsonl");
            var backupDir = Path.Combine(installDir, ".rollback");
            Directory.CreateDirectory(installDir);

            // 用户原本有一个同名文件，被上次（未完成的）安装覆盖了
            var target = Path.Combine(installDir, "config.ini");
            File.WriteAllText(target, "用户原来的内容");

            var journal = new InstallJournal(journalPath, backupDir);
            var backup = journal.BackupFile(target);          // 备份原件
            File.WriteAllText(target, "安装包覆盖后的内容");     // 覆盖
            journal.Record(new JournalEntry
            {
                StepId = "extract",
                Op = JournalOps.WriteFile,
                Target = target,
                ExistedBefore = true,
                BackupPath = backup,
            });

            var ctx = NewContext(installDir, journalPath, backupDir);
            new PreflightStep().Execute(ctx);

            Assert.Equal("用户原来的内容", File.ReadAllText(target));
            Assert.False(File.Exists(journalPath));
        }

        [Fact]
        public void 残留日志是坏的_不应让安装失败()
        {
            var installDir = Path.Combine(_root, "broken");
            var journalPath = Path.Combine(installDir, ".install-journal.jsonl");
            var backupDir = Path.Combine(installDir, ".rollback");
            Directory.CreateDirectory(installDir);

            // 半截的 JSON 行 —— 进程正好在写这一行的时候被杀
            File.WriteAllText(journalPath, "{ \"Op\": \"WriteFile\", \"Target\": \"C:\\\\x\", \"St")

            ;
            var ctx = NewContext(installDir, journalPath, backupDir);

            // 关键：不抛异常。用户现在是想装东西，不是想看回滚报错。
            var ex = Record.Exception(() => new PreflightStep().Execute(ctx));
            Assert.Null(ex);
        }
    }
}
