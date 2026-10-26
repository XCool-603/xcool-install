using System;
using System.Collections.Generic;
using System.IO;
using Installer.Abstractions.Model;
using Installer.Abstractions.Platform;
using Installer.Core.Packaging;
using Installer.Core.Platform;
using Installer.UI.Common;
using Installer.UI.Models;
using Installer.UI.Presenters;
using Xunit;

namespace Installer.Core.Tests
{
    /// <summary>
    /// 权限模型：什么时候提权、安装范围怎么写回、老程序会不会写安装目录。
    ///
    /// 这一组测试对应三个真实踩过的坑：
    /// ① 用户把目录改到 <c>C:\Program Files</c> 却不提权 —— 装到一半才失败；
    /// ② 向导里勾「为所有用户安装」等于没勾 —— <c>Scope</c> 从来没被写回；
    /// ③ 老程序把 <c>App.db</c>/<c>Log\</c> 放在安装目录 —— 装到 Program Files 就写不进去。
    /// </summary>
    public class ElevationTests
    {
        // ─────────────────────────────────────────────────────────────
        // 默认安装目录
        // ─────────────────────────────────────────────────────────────

        [Fact]
        public void 仅当前用户的默认目录在LocalAppData下()
        {
            var dir = InstallPaths.Suggest("我的产品", false);
            Assert.Equal(@"%LOCALAPPDATA%\Programs\我的产品", dir);
        }

        [Fact]
        public void 所有用户的默认目录在ProgramFiles下()
        {
            var dir = InstallPaths.Suggest("我的产品", true);
            Assert.Equal(@"%ProgramFiles%\我的产品", dir);
        }

        [Fact]
        public void 产品名里的非法字符会被替换()
        {
            var dir = InstallPaths.Suggest("A/B:C*D", false);
            Assert.DoesNotContain("/", dir);
            Assert.DoesNotContain(":", dir);
            Assert.DoesNotContain("*", dir);
        }

        [Fact]
        public void 产品名为空时用占位名()
        {
            Assert.Equal(@"%LOCALAPPDATA%\Programs\我的产品", InstallPaths.Suggest(null, false));
            Assert.Equal(@"%LOCALAPPDATA%\Programs\我的产品", InstallPaths.Suggest("   ", false));
        }

        // ─────────────────────────────────────────────────────────────
        // 可写性探测
        // ─────────────────────────────────────────────────────────────

        [Fact]
        public void 临时目录判定为可写()
        {
            Assert.True(ElevationProbe.CanWrite(Path.GetTempPath()));
        }

        [Fact]
        public void 探测不会留下临时文件()
        {
            var before = Directory.GetFiles(Path.GetTempPath(), ".wm-write-probe-*").Length;
            ElevationProbe.CanWrite(Path.GetTempPath());
            var after = Directory.GetFiles(Path.GetTempPath(), ".wm-write-probe-*").Length;
            Assert.Equal(before, after);
        }

        [Fact]
        public void 目录不存在时往上找最近的已存在祖先()
        {
            var leaf = Path.Combine(Path.GetTempPath(), "wm-none-" + Guid.NewGuid().ToString("N"),
                "a", "b", "c");

            var ancestor = ElevationProbe.NearestExistingAncestor(leaf);
            Assert.Equal(Path.GetTempPath().TrimEnd('\\'), ancestor.TrimEnd('\\'));
        }

        [Fact]
        public void 已存在的目录返回它自己()
        {
            var dir = Path.GetTempPath();
            Assert.Equal(Path.GetFullPath(dir).TrimEnd('\\'),
                ElevationProbe.NearestExistingAncestor(dir).TrimEnd('\\'));
        }

        [Fact]
        public void 空路径不做判断()
        {
            Assert.True(ElevationProbe.CanWrite(null));
            Assert.True(ElevationProbe.CanWrite(string.Empty));
        }

        [Fact]
        public void 显式拒绝写的目录判定为不可写()
        {
            // 用 Deny ACE 造一个"确定性写不进去"的目录 ——
            // 这样这条断言在管理员和非管理员环境下都成立。
            var dir = Path.Combine(Path.GetTempPath(), "wm-deny-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);

            // 用 Everyone（WellKnownSidType.WorldSid，与系统语言无关）显式 Deny 写权限
            var everyone = new System.Security.Principal.SecurityIdentifier(
                System.Security.Principal.WellKnownSidType.WorldSid, null);

            var rule = new System.Security.AccessControl.FileSystemAccessRule(
                everyone,
                System.Security.AccessControl.FileSystemRights.CreateFiles
                | System.Security.AccessControl.FileSystemRights.WriteData,
                System.Security.AccessControl.InheritanceFlags.ContainerInherit
                | System.Security.AccessControl.InheritanceFlags.ObjectInherit,
                System.Security.AccessControl.PropagationFlags.None,
                System.Security.AccessControl.AccessControlType.Deny);

            var acl = Directory.GetAccessControl(dir);
            acl.AddAccessRule(rule);
            Directory.SetAccessControl(dir, acl);

            try
            {
                Assert.False(ElevationProbe.CanWrite(dir), "被显式拒绝写的目录必须判定为不可写");

                // 不存在的子目录同理 —— 探测要往上找到这个不可写的祖先
                Assert.False(ElevationProbe.CanWrite(Path.Combine(dir, "sub", "deeper")));
            }
            finally
            {
                // 必须先撤掉 Deny，否则连自己都删不掉
                var back = Directory.GetAccessControl(dir);
                back.RemoveAccessRule(rule);
                Directory.SetAccessControl(dir, back);

                try
                {
                    Directory.Delete(dir, true);
                }
                catch (IOException)
                {
                }
            }
        }

        // ─────────────────────────────────────────────────────────────
        // 「程序会不会往安装目录里写」的体检
        // ─────────────────────────────────────────────────────────────

        [Fact]
        public void 检出数据目录与数据文件()
        {
            WithSourceDir(dir =>
            {
                Directory.CreateDirectory(Path.Combine(dir, "Log"));
                Directory.CreateDirectory(Path.Combine(dir, "SQLiteDB"));
                Directory.CreateDirectory(Path.Combine(dir, "x64"));       // 运行库，不该报
                File.WriteAllText(Path.Combine(dir, "App.exe"), "x");
                File.WriteAllText(Path.Combine(dir, "Config.ini"), "x");
                File.WriteAllText(Path.Combine(dir, "data.db"), "x");

                var hits = InstallDirWriteHeuristic.Detect(dir);

                Assert.Contains("Log" + Path.DirectorySeparatorChar, hits);
                Assert.Contains("SQLiteDB" + Path.DirectorySeparatorChar, hits);
                Assert.Contains("Config.ini", hits);
                Assert.Contains("data.db", hits);
                Assert.DoesNotContain("x64" + Path.DirectorySeparatorChar, hits);
            });
        }

        [Fact]
        public void 干净的目录不产生提示()
        {
            WithSourceDir(dir =>
            {
                File.WriteAllText(Path.Combine(dir, "App.exe"), "x");
                File.WriteAllText(Path.Combine(dir, "App.dll"), "x");

                Assert.Empty(InstallDirWriteHeuristic.Detect(dir));
                Assert.Null(InstallDirWriteHeuristic.Describe(dir, false));
            });
        }

        [Fact]
        public void 所有用户时提示里点明ProgramFiles写不进去()
        {
            WithSourceDir(dir =>
            {
                Directory.CreateDirectory(Path.Combine(dir, "Log"));
                File.WriteAllText(Path.Combine(dir, "App.exe"), "x");

                var text = InstallDirWriteHeuristic.Describe(dir, true);

                Assert.NotNull(text);
                Assert.Contains("%ProgramFiles%", text);
                Assert.Contains("%LOCALAPPDATA%", text);
            });
        }

        [Fact]
        public void 目录不存在时体检不抛异常()
        {
            var missing = Path.Combine(Path.GetTempPath(), "wm-none-" + Guid.NewGuid().ToString("N"));
            Assert.Empty(InstallDirWriteHeuristic.Detect(missing));
        }

        // ─────────────────────────────────────────────────────────────
        // 安装范围与默认目录的联动
        // ─────────────────────────────────────────────────────────────

        [Fact]
        public void 清单是所有用户时向导的复选框应为勾选()
        {
            var m = TestManifest.Create(scope: InstallScopes.PerMachine);
            var p = NewPresenter(m);

            Assert.True(p.Options.AllUsers);
        }

        [Fact]
        public void 清单是仅当前用户时向导的复选框应不勾()
        {
            var m = TestManifest.Create(scope: InstallScopes.PerUser);
            var p = NewPresenter(m);

            Assert.False(p.Options.AllUsers);
        }

        [Fact]
        public void 勾选所有用户后安装范围写回清单()
        {
            var m = TestManifest.Create(scope: InstallScopes.PerUser);
            var p = NewPresenter(m);

            p.Options.AllUsers = true;
            p.ApplyToManifest();

            Assert.Equal(InstallScopes.PerMachine, m.Scope);
        }

        [Fact]
        public void 取消勾选所有用户后安装范围退回仅当前用户()
        {
            var m = TestManifest.Create(scope: InstallScopes.PerMachine);
            var p = NewPresenter(m);

            p.Options.AllUsers = false;
            p.ApplyToManifest();

            Assert.Equal(InstallScopes.PerUser, m.Scope);
        }

        [Fact]
        public void 切换范围时默认目录跟着换到对应档()
        {
            var m = TestManifest.Create(scope: InstallScopes.PerUser);
            var p = NewPresenter(m);

            p.Options.AllUsers = true;
            p.ApplyScopeDefaultDir();

            Assert.Equal(Environment.ExpandEnvironmentVariables(@"%ProgramFiles%\测试产品"),
                p.Options.InstallDir);

            p.Options.AllUsers = false;
            p.ApplyScopeDefaultDir();

            Assert.Equal(Environment.ExpandEnvironmentVariables(@"%LOCALAPPDATA%\Programs\测试产品"),
                p.Options.InstallDir);
        }

        [Fact]
        public void 安装目录也会写回清单()
        {
            var m = TestManifest.Create();
            var p = NewPresenter(m);

            p.Options.InstallDir = @"D:\Apps\Demo";
            p.ApplyToManifest();

            Assert.Equal(@"D:\Apps\Demo", m.DefaultInstallDir);
        }

        [Fact]
        public void 可以把当前页直接设成安装位置页()
        {
            var m = TestManifest.Create();
            var p = NewPresenter(m);

            Assert.Equal(WizardPage.Welcome, p.Current);

            p.StartAt(WizardPage.Path);
            Assert.Equal(WizardPage.Path, p.Current);
        }

        // ─────────────────────────────────────────────────────────────
        // 清单字段往返
        // ─────────────────────────────────────────────────────────────

        [Fact]
        public void 始终提权的字段能序列化并读回()
        {
            var m = TestManifest.Create();
            m.AlwaysElevate = true;

            var json = ManifestSerializer.Serialize(m);
            var back = ManifestSerializer.Deserialize(json);

            Assert.True(back.AlwaysElevate);
        }

        [Fact]
        public void 始终提权默认为假()
        {
            var back = ManifestSerializer.Deserialize(ManifestSerializer.Serialize(TestManifest.Create()));
            Assert.False(back.AlwaysElevate);
        }

        // ─────────────────────────────────────────────────────────────
        // 提权决策（纯函数，穷举）
        //
        // 验证机是内置 Administrator 且 EnableLUA=0 —— 根本不存在非提权进程，
        // 弹不出 UAC。所以这一组把"要不要提权"的每一种组合都钉死，
        // 而不是靠"真的弹一次"。
        // ─────────────────────────────────────────────────────────────

        [Fact]
        public void 已是管理员时安装一律不再提权()
        {
            var m = TestManifest.Create(scope: InstallScopes.PerMachine);
            Assert.False(ElevationRules.InstallNeedsAdmin(m, @"C:\Program Files\X", true));
        }

        [Fact]
        public void 所有用户安装需要提权()
        {
            var m = TestManifest.Create(scope: InstallScopes.PerMachine);
            Assert.True(ElevationRules.InstallNeedsAdmin(m, @"C:\Anywhere\X", false));
        }

        [Fact]
        public void 始终提权标记需要提权()
        {
            var m = TestManifest.Create(scope: InstallScopes.PerUser);
            m.AlwaysElevate = true;
            Assert.True(ElevationRules.InstallNeedsAdmin(m, @"C:\Anywhere\X", false));
        }

        [Fact]
        public void 仅当前用户且目录可写时不需要提权()
        {
            var m = TestManifest.Create(scope: InstallScopes.PerUser);
            Assert.False(ElevationRules.InstallNeedsAdmin(m, Path.GetTempPath(), false));
        }

        [Fact]
        public void 目录写不进去时即使仅当前用户也要提权()
        {
            WithDeniedDir(dir =>
            {
                var m = TestManifest.Create(scope: InstallScopes.PerUser);
                Assert.True(ElevationRules.InstallNeedsAdmin(m, dir, false),
                    "目录不可写却判定为不需要提权 —— 这正是「装到一半才失败」的成因");
            });
        }

        [Fact]
        public void 清单为空时只看目录()
        {
            Assert.False(ElevationRules.InstallNeedsAdmin(null, Path.GetTempPath(), false));
        }

        [Fact]
        public void 向导里已是管理员时不再提权()
        {
            Assert.False(ElevationRules.WizardNeedsAdmin(true, false, true, true, @"C:\Program Files\X"));
        }

        [Fact]
        public void 沙箱里一律不提权()
        {
            Assert.False(ElevationRules.WizardNeedsAdmin(false, true, true, true, @"C:\Program Files\X"));
        }

        [Fact]
        public void 向导里勾所有用户需要提权()
        {
            Assert.True(ElevationRules.WizardNeedsAdmin(false, false, false, true, Path.GetTempPath()));
        }

        [Fact]
        public void 向导里始终提权标记需要提权()
        {
            Assert.True(ElevationRules.WizardNeedsAdmin(false, false, true, false, Path.GetTempPath()));
        }

        [Fact]
        public void 向导里目录不可写需要提权()
        {
            WithDeniedDir(dir =>
            {
                Assert.True(ElevationRules.WizardNeedsAdmin(false, false, false, false, dir));
            });
        }

        [Fact]
        public void 向导里普通选择不需要提权()
        {
            Assert.False(ElevationRules.WizardNeedsAdmin(false, false, false, false, Path.GetTempPath()));
            Assert.False(ElevationRules.WizardNeedsAdmin(false, false, false, false, null));
            Assert.False(ElevationRules.WizardNeedsAdmin(false, false, false, false, "  "));
        }

        // ─────────────────────────────────────────────────────────────
        // 提权重启后的续跑
        // ─────────────────────────────────────────────────────────────

        [Fact]
        public void 续跑会把上次的选择原样恢复()
        {
            var m = TestManifest.Create(scope: InstallScopes.PerUser);
            var p = NewPresenter(m);

            p.ApplyResume(new WizardOptions
            {
                InstallDir = @"D:\Apps\续跑",
                AcceptLicense = true,
                DesktopShortcut = false,
                StartMenuShortcut = true,
                Autostart = true,
                RunAfterInstall = false,
                AllUsers = true,
            });

            Assert.Equal(@"D:\Apps\续跑", p.Options.InstallDir);
            Assert.True(p.Options.AcceptLicense);
            Assert.False(p.Options.DesktopShortcut);
            Assert.True(p.Options.StartMenuShortcut);
            Assert.True(p.Options.Autostart);
            Assert.False(p.Options.RunAfterInstall);
            Assert.True(p.Options.AllUsers);

            // 提权后应该直接落在「安装位置」页，而不是让用户从欢迎页重走
            Assert.Equal(WizardPage.Path, p.Current);
        }

        [Fact]
        public void 续跑时目录为空则保留默认值()
        {
            var m = TestManifest.Create();
            var p = NewPresenter(m);
            var original = p.Options.InstallDir;

            p.ApplyResume(new WizardOptions { InstallDir = null });

            Assert.Equal(original, p.Options.InstallDir);
        }

        [Fact]
        public void 续跑传空对象不改变任何状态()
        {
            var m = TestManifest.Create();
            var p = NewPresenter(m);
            var before = p.Current;

            p.ApplyResume(null);

            Assert.Equal(before, p.Current);
        }

        [Fact]
        public void 续跑文件的内容能序列化并读回()
        {
            // 提权重启时走的是「序列化 → 写 %TEMP% → 新实例反序列化」，
            // 这里钉死这个契约：少一个字段，用户提权后就得重勾一遍。
            var saved = new WizardOptions
            {
                InstallDir = @"D:\Apps\X",
                AcceptLicense = true,
                DesktopShortcut = true,
                StartMenuShortcut = false,
                Autostart = true,
                RunAfterInstall = true,
                AllUsers = false,
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(saved);
            var back = Newtonsoft.Json.JsonConvert.DeserializeObject<WizardOptions>(json);

            Assert.Equal(saved.InstallDir, back.InstallDir);
            Assert.Equal(saved.AcceptLicense, back.AcceptLicense);
            Assert.Equal(saved.DesktopShortcut, back.DesktopShortcut);
            Assert.Equal(saved.StartMenuShortcut, back.StartMenuShortcut);
            Assert.Equal(saved.Autostart, back.Autostart);
            Assert.Equal(saved.RunAfterInstall, back.RunAfterInstall);
            Assert.Equal(saved.AllUsers, back.AllUsers);
        }

        // ─────────────────────────────────────────────────────────────

        /// <summary>造一个"确定性写不进去"的目录（Everyone 显式 Deny 写权限）。</summary>
        private static void WithDeniedDir(Action<string> body)
        {
            var dir = Path.Combine(Path.GetTempPath(), "wm-deny-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);

            var everyone = new System.Security.Principal.SecurityIdentifier(
                System.Security.Principal.WellKnownSidType.WorldSid, null);

            var rule = new System.Security.AccessControl.FileSystemAccessRule(
                everyone,
                System.Security.AccessControl.FileSystemRights.CreateFiles
                | System.Security.AccessControl.FileSystemRights.WriteData,
                System.Security.AccessControl.InheritanceFlags.ContainerInherit
                | System.Security.AccessControl.InheritanceFlags.ObjectInherit,
                System.Security.AccessControl.PropagationFlags.None,
                System.Security.AccessControl.AccessControlType.Deny);

            var acl = Directory.GetAccessControl(dir);
            acl.AddAccessRule(rule);
            Directory.SetAccessControl(dir, acl);

            try
            {
                body(dir);
            }
            finally
            {
                var back = Directory.GetAccessControl(dir);
                back.RemoveAccessRule(rule);
                Directory.SetAccessControl(dir, back);

                try
                {
                    Directory.Delete(dir, true);
                }
                catch (IOException)
                {
                }
            }
        }

        private static WizardPresenter NewPresenter(InstallerManifest m)
        {
            var table = new StringTable(WizardStrings.Merge(m.Strings), "zh-Hans", m.DefaultCulture);
            return new WizardPresenter(m, table, @"C:\Test\App");
        }

        private static void WithSourceDir(Action<string> body)
        {
            var dir = Path.Combine(Path.GetTempPath(), "wm-heur-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);

            try
            {
                body(dir);
            }
            finally
            {
                try
                {
                    Directory.Delete(dir, true);
                }
                catch (IOException)
                {
                }
            }
        }
    }
}
