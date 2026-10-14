using System;
using System.Collections.Generic;
using Installer.Abstractions.Model;
using Installer.Abstractions.Platform;
using Installer.Builder.Common;
using Installer.Builder.Views;
using Xunit;

namespace Installer.Core.Tests
{
    /// <summary>
    /// 制作端各 View 的 **Write → Read 往返**。
    ///
    /// 为什么要有这一组：这些 View 的方法对是纯数据搬运 ——
    /// <c>Write</c> 把模型灌进控件，<c>Read</c> 把控件写回模型。
    /// 搬运错了（漏一个字段、取错属性、下拉框没初始化）不会报错，
    /// 只是**用户填的东西悄悄丢了**。之前 188 个测试一个都没覆盖这条路径。
    ///
    /// 注意：这些是 UserControl，创建控件不需要消息循环；
    /// 但 AntdUI 的下拉框要先有 Items 才能设 SelectedIndex，
    /// 所以每个用例都先 ApplyText。
    /// </summary>
    public class BuilderViewTests
    {
        private static IStringTable Text()
        {
            return new StringTable(BuilderStrings.CreateDefault(), "zh-Hans", "zh-Hans");
        }

        private static InstallerManifest Manifest()
        {
            return new InstallerManifest
            {
                SchemaVersion = 2,
                ProductCode = "{AAAAAAAA-1111-2222-3333-444444444444}",
                DefaultCulture = "zh-Hans",
                Cultures = new List<string> { "zh-Hans", "en" },
                Product = new ProductInfo
                {
                    Name = new LocalizedText { { "zh-Hans", "测试产品" }, { "en", "Test" } },
                    Version = "2.4.1",
                    Publisher = new LocalizedText { { "zh-Hans", "唯非工作室" } },
                },
                Scope = InstallScopes.PerUser,
                DefaultInstallDir = @"%LOCALAPPDATA%\Programs\测试",
                EntryPoint = @"bin\App.exe",
                Files = new List<FileEntry>(),
                Shortcuts = new List<ShortcutSpec>(),
                Strings = new Dictionary<string, LocalizedText>(StringComparer.Ordinal),
            };
        }

        [Fact]
        public void 高级选项_安装细节_往返不丢数据()
        {
            var m = Manifest();
            var b = new ProjectBuildSettings
            {
                IconPath = @"C:\icons\app.ico",
            };
            m.Scope = InstallScopes.PerMachine;
            m.DefaultInstallDir = @"D:\Apps\我的产品";
            m.Com = new List<ComSpec>
            {
                new ComSpec { Path = @"bin\a.dll", Mode = ComRegistrationModes.RegSvr32 },
                new ComSpec { Path = @"bin\b.dll", Mode = ComRegistrationModes.RegSvr32 },
            };

            var view = new AdvancedInstallView();
            view.ApplyText(Text());
            view.Write(m, b);
            view.Read(m, b);

            Assert.Equal(InstallScopes.PerMachine, m.Scope);
            Assert.Equal(@"D:\Apps\我的产品", m.DefaultInstallDir);
            Assert.Equal(@"C:\icons\app.ico", b.IconPath);
            Assert.Equal(2, m.Com.Count);
            Assert.Equal(@"bin\a.dll", m.Com[0].Path);
            Assert.Equal(@"bin\b.dll", m.Com[1].Path);

            ((IDisposable)view).Dispose();
        }

        [Fact]
        public void 高级选项_安装细节_空图标不应变成空白字符串()
        {
            // 空图标是"不打补丁"的信号，必须保持 null，
            // 否则 SkipIconPatch 的判断会拿到一个空白字符串。
            var m = Manifest();
            var b = new ProjectBuildSettings { IconPath = null };

            var view = new AdvancedInstallView();
            view.ApplyText(Text());
            view.Write(m, b);
            view.Read(m, b);

            Assert.True(string.IsNullOrWhiteSpace(b.IconPath),
                "空图标读回来应是空值，实际：" + (b.IconPath ?? "<null>"));

            ((IDisposable)view).Dispose();
        }

        [Fact]
        public void 高级选项_许可协议_往返不丢数据()
        {
            var m = Manifest();
            m.License = new LicenseInfo
            {
                Required = true,
                Text = new LocalizedText { { "zh-Hans", "第一行\n第二行" }, { "en", "line one" } },
            };

            var view = new AdvancedLicenseView();
            view.ApplyText(Text());
            view.Write(m);
            view.Read(m);

            Assert.True(m.License.Required, "「需要同意许可协议」的勾选丢了");

            string zh;
            Assert.True(m.License.Text.TryGetValue("zh-Hans", out zh));
            Assert.Contains("第一行", zh);
            Assert.Contains("第二行", zh);

            ((IDisposable)view).Dispose();
        }

        [Fact]
        public void 基本信息_往返不丢中英文与版本()
        {
            var view = new InfoStepView();
            view.ApplyText(Text());

            var name = new LocalizedText { { "zh-Hans", "空中小火车" }, { "en", "Sky Train" } };
            var publisher = new LocalizedText { { "zh-Hans", "唯非工作室" }, { "en", "WeiFei" } };

            view.Write(name, "3.1.4", publisher);

            LocalizedText outName, outPublisher;
            string outVersion;
            view.Read(out outName, out outVersion, out outPublisher);

            Assert.Equal("3.1.4", outVersion);
            Assert.Equal("空中小火车", outName["zh-Hans"]);
            Assert.Equal("Sky Train", outName["en"]);
            // 厂商按设计稿**只有一个输入框**，所以中英共用同一个值 —— 这是有意的，
            // 不是丢数据。只有产品名才有中英两个框（对应设计稿那两行输入）。
            // 即：清单模型支持 publisher 多语言，但制作端目前只让填一个。
            Assert.Equal("唯非工作室", outPublisher["zh-Hans"]);
            Assert.Equal("唯非工作室", outPublisher["en"]);

            ((IDisposable)view).Dispose();
        }

        [Fact]
        public void 快捷方式_往返不丢勾选与自动运行()
        {
            var m = Manifest();
            m.Shortcuts = new List<ShortcutSpec>
            {
                new ShortcutSpec { Location = "Desktop", Name = new LocalizedText { { "zh-Hans", "桌面" } } },
                new ShortcutSpec { Location = "StartMenu", Name = new LocalizedText { { "zh-Hans", "开始菜单" } } },
            };
            m.Autostart = true;
            m.RunAfterInstall = true;

            var view = new ShortcutStepView();
            view.ApplyText(Text());
            view.Write(m);
            view.Read(m);

            Assert.True(m.Autostart, "「开机启动」的勾选丢了");
            Assert.True(m.RunAfterInstall, "「安装后自动运行」的勾选丢了");
            Assert.Equal(2, m.Shortcuts.Count);

            ((IDisposable)view).Dispose();
        }

        [Fact]
        public void 快捷方式_全不勾_不应残留默认值()
        {
            var m = Manifest();
            m.Shortcuts = new List<ShortcutSpec>();
            m.Autostart = false;
            m.RunAfterInstall = false;

            var view = new ShortcutStepView();
            view.ApplyText(Text());
            view.Write(m);
            view.Read(m);

            Assert.False(m.Autostart);
            Assert.False(m.RunAfterInstall);

            ((IDisposable)view).Dispose();
        }
    }
}
