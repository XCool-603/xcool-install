using System;
using System.Collections.Generic;
using System.Linq;
using Installer.Abstractions.Model;
using Installer.Abstractions.Platform;
using Installer.UI.Common;
using Installer.UI.Models;
using Installer.UI.Presenters;
using Xunit;

namespace Installer.Core.Tests
{
    /// <summary>
    /// 向导状态机。
    ///
    /// **这个测试类存在的主要目的是钉死"不能用按钮文字当状态"这条约束。**
    /// 现状的安装器用 <c>BtnInstall.Text.Equals("立即安装")</c> 判断状态
    /// （<c>FrmInstallation.cs:173</c>、<c>FrmUpdate.cs:43</c>、<c>FrmUnInstall.cs:117</c>），
    /// 界面一翻译成英文，安装/卸载立刻失效。这里断言状态是枚举、文案只是输出。
    /// </summary>
    public class WizardPresenterTests
    {
        private static WizardPresenter NewPresenter(
            bool licenseRequired = false,
            bool withShortcuts = false,
            bool autostart = false,
            bool runAfter = false,
            string culture = "zh-Hans")
        {
            var m = TestManifest.Create();
            m.License = new LicenseInfo
            {
                Required = licenseRequired,
                Text = new LocalizedText { { "zh-Hans", "协议正文" }, { "en", "License body" } },
            };
            m.Shortcuts = withShortcuts
                ? new List<ShortcutSpec> { new ShortcutSpec { Location = "Desktop", Name = m.Product.Name } }
                : new List<ShortcutSpec>();
            m.Autostart = autostart;
            m.RunAfterInstall = runAfter;

            var table = new StringTable(WizardStrings.Merge(m.Strings), culture, m.DefaultCulture);
            return new WizardPresenter(m, table, @"C:\Test\App");
        }

        [Fact]
        public void 需要许可时_欢迎页之后是许可页()
        {
            var p = NewPresenter(licenseRequired: true);
            Assert.Equal(WizardPage.Welcome, p.Current);

            p.Primary();
            Assert.Equal(WizardPage.License, p.Current);
        }

        [Fact]
        public void 不需要许可时_欢迎页直接到路径页()
        {
            var p = NewPresenter(licenseRequired: false);
            p.Primary();
            Assert.Equal(WizardPage.Path, p.Current);
        }

        [Fact]
        public void 未接受许可时主按钮不可用且无法前进()
        {
            var p = NewPresenter(licenseRequired: true);
            p.Primary();                       // → License
            p.Options.AcceptLicense = false;

            Assert.False(p.PrimaryEnabled);
            Assert.False(p.Primary());          // 迁移被拒绝
            Assert.Equal(WizardPage.License, p.Current);
        }

        [Fact]
        public void 接受许可后可以前进()
        {
            var p = NewPresenter(licenseRequired: true);
            p.Primary();                        // → License
            p.Options.AcceptLicense = true;

            Assert.True(p.PrimaryEnabled);
            Assert.True(p.Primary());
            Assert.Equal(WizardPage.Path, p.Current);
        }

        [Fact]
        public void 路径为空时无法前进()
        {
            var p = NewPresenter();
            p.Primary();                        // → Path
            p.Options.InstallDir = "   ";

            Assert.False(p.PrimaryEnabled);
            Assert.False(p.Primary());
            Assert.Equal(WizardPage.Path, p.Current);
        }

        [Fact]
        public void 有选项时_路径页之后直接进安装页()
        {
            // 设计稿把「安装位置」与「附加任务」合成一页，所以不再经过 Options
            var p = NewPresenter(withShortcuts: true);
            p.Primary();                        // → Path
            p.Primary();                        // → Installing
            Assert.Equal(WizardPage.Installing, p.Current);
        }

        [Fact]
        public void 没有选项时_路径页直接进入安装()
        {
            var p = NewPresenter();
            p.Primary();                        // → Path
            p.Primary();                        // → Installing
            Assert.Equal(WizardPage.Installing, p.Current);
        }

        [Fact]
        public void 上一步应逐页回退()
        {
            var p = NewPresenter(licenseRequired: true, withShortcuts: true);
            p.Primary();                        // → License
            p.Options.AcceptLicense = true;     // 不接受就出不去许可页
            p.Primary();                        // → Path
            Assert.Equal(WizardPage.Path, p.Current);

            p.Back();
            Assert.Equal(WizardPage.License, p.Current);
            p.Back();
            Assert.Equal(WizardPage.Welcome, p.Current);

            Assert.False(p.CanGoBack);          // 欢迎页没有上一步
        }

        [Fact]
        public void 失败后可以重试()
        {
            var p = NewPresenter();
            p.EnterInstalling();
            p.EnterFailed("磁盘空间不足");

            Assert.Equal(WizardPage.Failed, p.Current);
            Assert.True(p.CanRetry);
            Assert.Equal("磁盘空间不足", p.ErrorMessage);

            p.Primary();                        // 重试
            Assert.Equal(WizardPage.Installing, p.Current);
            Assert.Null(p.ErrorMessage);
        }

        // ─────────────────────────────────────────────────────────────
        // 关键：文案与状态分离
        // ─────────────────────────────────────────────────────────────

        [Fact]
        public void 主按钮返回的是文案_key_而不是已翻译的文字()
        {
            var p = NewPresenter(licenseRequired: true, withShortcuts: true);

            Assert.Equal("Wizard.Next", p.PrimaryKey);

            p.Primary();                        // → License
            Assert.Equal("Wizard.Next", p.PrimaryKey);

            p.Options.AcceptLicense = true;
            p.Primary();                        // → Path
            Assert.Equal("Wizard.Install", p.PrimaryKey);

            p.Primary();                        // → Installing
            Assert.Equal("Wizard.Cancel", p.PrimaryKey);
            Assert.True(p.PrimaryIsCancel);
        }

        [Fact]
        public void 切换语言不会改变状态机行为()
        {
            // 用同一串操作分别跑中英两版，状态序列必须完全一致
            var zh = RunSequence("zh-Hans");
            var en = RunSequence("en");

            Assert.Equal(zh, en);
            Assert.Equal(
                new[] { "Welcome", "License", "Path", "Installing", "Installing" },
                zh.ToArray());
        }

        private static List<string> RunSequence(string culture)
        {
            var p = NewPresenter(licenseRequired: true, withShortcuts: true, culture: culture);
            var trace = new List<string> { p.Current.ToString() };

            p.Primary();
            trace.Add(p.Current.ToString());

            p.Options.AcceptLicense = true;
            p.Primary();
            trace.Add(p.Current.ToString());

            p.Primary();
            trace.Add(p.Current.ToString());

            p.Primary();
            trace.Add(p.Current.ToString());

            return trace;
        }

        [Fact]
        public void 同一状态下中英两版的主按钮文案不同但_key_相同()
        {
            var zh = NewPresenter(licenseRequired: false, culture: "zh-Hans");
            var en = NewPresenter(licenseRequired: false, culture: "en");

            zh.Primary();                       // → Path
            en.Primary();

            Assert.Equal(zh.PrimaryKey, en.PrimaryKey);
            Assert.Equal("立即安装", zh.Text(zh.PrimaryKey));
            Assert.Equal("Install", en.Text(en.PrimaryKey));
        }

        [Fact]
        public void 清单里的_strings_可以覆盖内置文案()
        {
            var m = TestManifest.Create();
            m.Strings["Wizard.Next"] = new LocalizedText { { "zh-Hans", "走起" }, { "en", "Go" } };

            var table = new StringTable(WizardStrings.Merge(m.Strings), "zh-Hans", m.DefaultCulture);
            var p = new WizardPresenter(m, table, @"C:\Test");

            Assert.Equal("走起", p.Text("Wizard.Next"));
        }

        [Fact]
        public void 清单只覆盖一种语言时另一种仍用内置文案()
        {
            var m = TestManifest.Create();
            m.Strings["Wizard.Next"] = new LocalizedText { { "en", "Proceed" } };

            var merged = WizardStrings.Merge(m.Strings);

            var en = new StringTable(merged, "en", m.DefaultCulture);
            var zh = new StringTable(merged, "zh-Hans", m.DefaultCulture);

            Assert.Equal("Proceed", en.Get("Wizard.Next"));
            Assert.Equal("下一步", zh.Get("Wizard.Next"));   // 内置的中文没被抹掉
        }

        [Fact]
        public void 内置文案表默认就有中英两版()
        {
            var table = WizardStrings.CreateDefault();

            Assert.True(table.Count > 20);

            foreach (var kv in table)
            {
                Assert.True(kv.Value.ContainsKey("zh-Hans"), kv.Key + " 缺少中文");
                Assert.True(kv.Value.ContainsKey("en"), kv.Key + " 缺少英文");
            }
        }

        [Fact]
        public void 用户的选择应写回清单()
        {
            var p = NewPresenter(withShortcuts: true, autostart: true, runAfter: true);
            p.Options.DesktopShortcut = false;
            p.Options.StartMenuShortcut = true;
            p.Options.Autostart = false;
            p.Options.RunAfterInstall = false;

            p.ApplyToManifest();

            var m = GetManifest(p);
            Assert.Single(m.Shortcuts);
            Assert.Equal("StartMenu", m.Shortcuts[0].Location);
            Assert.False(m.Autostart);
            Assert.False(m.RunAfterInstall);
        }

        [Fact]
        public void 清单里勾了四项时_向导默认四项都要勾着()
        {
            // 用户实测反馈："制作端选了快捷方式，安装包执行的时候没按选择。"
            // 事故点在"往界面灌值"那一步（PathView.ApplyOptions），这里同时把
            // Presenter 这一侧钉住：清单四项 → Options 四项 → 写回清单仍是四项。
            var m = TestManifest.Create();
            m.Shortcuts = new List<ShortcutSpec>
            {
                new ShortcutSpec { Location = "Desktop", Name = m.Product.Name },
                new ShortcutSpec { Location = "StartMenu", Name = m.Product.Name },
            };
            m.Autostart = true;
            m.RunAfterInstall = true;

            var table = new StringTable(WizardStrings.Merge(m.Strings), "zh-Hans", m.DefaultCulture);
            var p = new WizardPresenter(m, table, @"C:\Test\App");

            Assert.True(p.Options.DesktopShortcut);
            Assert.True(p.Options.StartMenuShortcut);
            Assert.True(p.Options.Autostart);
            Assert.True(p.Options.RunAfterInstall);

            p.ApplyToManifest();

            Assert.Equal(2, GetManifest(p).Shortcuts.Count);
            Assert.True(GetManifest(p).Autostart);
            Assert.True(GetManifest(p).RunAfterInstall);
        }

        // ─────────────────────────────────────────────────────────────
        // 回归：用户在"安装位置"页改成 D 盘，就**必须**装到 D 盘
        // ─────────────────────────────────────────────────────────────

        [Fact]
        public void 用户在向导里改的安装位置必须写回清单()
        {
            var p = NewPresenter();
            p.Options.InstallDir = @"D:\我的程序";

            p.ApplyToManifest();

            Assert.Equal(@"D:\我的程序", GetManifest(p).DefaultInstallDir);
        }

        [Fact]
        public void 安装位置为空时不应把清单里的默认值抹掉()
        {
            var p = NewPresenter();
            var before = GetManifest(p).DefaultInstallDir;

            p.Options.InstallDir = null;
            p.ApplyToManifest();

            Assert.Equal(before, GetManifest(p).DefaultInstallDir);
        }

        [Fact]
        public void 换一个盘符也要生效()
        {
            // 这条对应真实反馈："安装到 D 盘，但文件还是装到 C 盘了"
            var p = NewPresenter();
            var original = GetManifest(p).DefaultInstallDir;

            Assert.NotEqual(@"E:\Target", original);

            p.Options.InstallDir = @"E:\Target";
            p.ApplyToManifest();

            Assert.Equal(@"E:\Target", GetManifest(p).DefaultInstallDir);
            Assert.NotEqual(original, GetManifest(p).DefaultInstallDir);
        }

        [Fact]
        public void 卸载模式的主按钮与取消按钮语义正确()
        {
            var p = NewPresenter();
            p.EnterUninstallConfirm();

            Assert.Equal(WizardPage.UninstallConfirm, p.Current);
            Assert.Equal("Uninstall.Title", p.PrimaryKey);
            // 设计稿第 3 版里卸载向导也有步骤条（3 步：确认卸载 / 正在卸载 / 完成）
            Assert.True(p.ShowSteps);
            Assert.Equal(3, p.StepCount);
            Assert.Equal(0, p.StepIndex);
            Assert.True(p.CanClose);

            p.Primary();
            Assert.Equal(WizardPage.Uninstalling, p.Current);
            Assert.True(p.PrimaryIsCancel);
            Assert.Equal(1, p.StepIndex);
            Assert.False(p.CanClose);           // 卸载中不允许直接关

            p.EnterUninstalled();
            Assert.Equal(2, p.StepIndex);
            Assert.Equal("Wizard.Close", p.SecondaryKey);
        }

        [Fact]
        public void 点快捷路径chip应按_根目录_厂商_产品名_拼出安装目录()
        {
            // 「安装设置」页那排 Program Files / 本地 AppData chips 以前点了没反应。
            // 拼装规则放在 Presenter 里，所以能在这里钉住：少一层厂商、丢产品名都会静默装错地方。
            var p = NewPresenter();

            Assert.Equal(@"D:\Program Files\唯非工作室\测试产品",
                p.ComposeInstallDir(@"D:\Program Files"));
        }

        [Fact]
        public void 没填厂商时chip拼出来的目录不带厂商层级()
        {
            var m = TestManifest.Create();
            m.Product.Publisher = new LocalizedText();
            var table = new StringTable(WizardStrings.Merge(m.Strings), "zh-Hans", m.DefaultCulture);
            var p = new WizardPresenter(m, table, @"C:\Test\App");

            Assert.Equal(@"D:\Program Files\测试产品", p.ComposeInstallDir(@"D:\Program Files"));
        }

        [Fact]
        public void 卸载进度页的文案必须和安装页分开()
        {
            // 用户实测截图：卸载进度页显示「安装程序正在解压并写入文件」「已解压文件 123/199」
            // 「准备安装目录…」—— 整块沿用安装文案。这里钉住卸载页自己的一套 key。
            var table = WizardStrings.CreateDefault();

            var uninstallKeys = new[]
            {
                "Uninstall.ProgressTitle", "Uninstall.ProgressSubtitle",
                "Uninstall.CapFiles", "Uninstall.CapSize", "Uninstall.CapElapsed", "Uninstall.CapRemaining",
                "Uninstall.Step1", "Uninstall.Step2", "Uninstall.Step3", "Uninstall.Step4",
            };

            foreach (var key in uninstallKeys)
            {
                Assert.True(table.ContainsKey(key), "缺少卸载进度页文案：" + key);
                Assert.False(string.IsNullOrWhiteSpace(table[key]["zh-Hans"]), key + " 缺少中文");
                Assert.False(string.IsNullOrWhiteSpace(table[key]["en"]), key + " 缺少英文");
            }

            // 统计卡标题不能和安装页相同
            Assert.NotEqual(table["Progress.CapFiles"]["zh-Hans"], table["Uninstall.CapFiles"]["zh-Hans"]);
            // 步骤名也不能相同
            Assert.NotEqual(table["Progress.Step1"]["zh-Hans"], table["Uninstall.Step1"]["zh-Hans"]);
        }

        // ── 更新流程（已安装同一产品）────────────────────────────────

        [Fact]
        public void 已安装旧版本时应跳过安装设置页并显示版本对比()
        {
            var manifest = TestManifest.Create();
            manifest.Product.Version = "2.0.0";

            var p = new WizardPresenter(manifest, Text(manifest), @"C:\Apps\Test");
            p.SetInstalled(new InstalledProduct { Version = "1.0.0", InstallDir = @"D:\Old\Test" });

            Assert.True(p.IsUpdate);
            Assert.Equal(UpdateKind.Newer, p.Kind);
            Assert.Equal(@"D:\Old\Test", p.Options.InstallDir);   // 直接写已安装目录
            Assert.Contains("1.0.0", p.UpdateHint);
            Assert.Contains("2.0.0", p.UpdateHint);

            // 欢迎页下一步 → 更新页（不是安装设置页）
            p.Primary();
            Assert.Equal(WizardPage.Update, p.Current);
            Assert.Equal("Wizard.Update", p.PrimaryKey);

            // 更新页主按钮 → 直接开装
            p.Primary();
            Assert.Equal(WizardPage.Installing, p.Current);
        }

        [Fact]
        public void 相同版本应提示重新安装()
        {
            var manifest = TestManifest.Create();
            manifest.Product.Version = "1.0.0";

            var p = new WizardPresenter(manifest, Text(manifest), @"C:\Apps\Test");
            p.SetInstalled(new InstalledProduct { Version = "1.0.0", InstallDir = @"C:\Apps\Test" });

            Assert.Equal(UpdateKind.Same, p.Kind);

            p.Primary();
            Assert.Equal(WizardPage.Update, p.Current);
            Assert.Equal("Wizard.Reinstall", p.PrimaryKey);
        }

        [Fact]
        public void 低版本应提示仍要安装并允许降级()
        {
            var manifest = TestManifest.Create();
            manifest.Product.Version = "0.9.0";

            var p = new WizardPresenter(manifest, Text(manifest), @"C:\Apps\Test");
            p.SetInstalled(new InstalledProduct { Version = "1.0.0", InstallDir = @"C:\Apps\Test" });

            Assert.Equal(UpdateKind.Older, p.Kind);
            Assert.True(p.AllowDowngrade);

            p.Primary();
            Assert.Equal(WizardPage.Update, p.Current);
            Assert.Equal("Wizard.InstallAnyway", p.PrimaryKey);
        }

        [Fact]
        public void 没装过时应照旧进安装设置页()
        {
            var manifest = TestManifest.Create();
            var p = new WizardPresenter(manifest, Text(manifest), @"C:\Apps\Test");

            Assert.False(p.IsUpdate);
            Assert.Equal(UpdateKind.None, p.Kind);

            p.Primary();
            Assert.Equal(WizardPage.Path, p.Current);
            Assert.Equal("Wizard.Install", p.PrimaryKey);
        }

        private static StringTable Text(InstallerManifest m)
        {
            return new StringTable(WizardStrings.Merge(m.Strings), "zh-Hans", m.DefaultCulture);
        }

        private static InstallerManifest GetManifest(WizardPresenter p)
        {
            // 反射拿私有字段：测试需要确认"选择写回了清单"
            var field = typeof(WizardPresenter).GetField("_manifest",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return (InstallerManifest)field.GetValue(p);
        }
    }
}
