using System;
using Installer.UI.Models;
using Installer.UI.Views;
using Xunit;

namespace Installer.Core.Tests
{
    /// <summary>
    /// 「安装位置」页 → 选项对象 的回写方向。
    ///
    /// 为什么专门测这一个方向：它以前整条是断的 ——
    /// <c>PathView.OptionsChanged</c> 声明了、勾选时也触发了，但**全项目没有订阅者**，
    /// 于是用户在界面上取消勾选完全没效果：照样建桌面快捷方式、照样开机自启，
    /// 「为所有用户安装」更是连 <c>Scope</c> 都没写回清单。
    ///
    /// 这类 bug 不会抛异常、不会打日志，只是"用户的选择悄悄丢失"，
    /// 所以必须有一个断言钉住"界面上的勾选确实能变成引擎读到的值"。
    /// </summary>
    public class PathViewTests
    {
        [Fact]
        public void 勾选状态能读进选项()
        {
            var view = new PathView();
            try
            {
                view.InstallPath = @"D:\Apps\X";
                view.DesktopShortcut = false;
                view.StartMenuShortcut = true;
                view.Autostart = true;
                view.RunAfterInstall = false;
                view.AllUsers = true;

                var options = new WizardOptions();
                view.ReadInto(options);

                Assert.Equal(@"D:\Apps\X", options.InstallDir);
                Assert.False(options.DesktopShortcut);
                Assert.True(options.StartMenuShortcut);
                Assert.True(options.Autostart);
                Assert.False(options.RunAfterInstall);
                Assert.True(options.AllUsers);
            }
            finally
            {
                ((IDisposable)view).Dispose();
            }
        }

        [Fact]
        public void 全部取消勾选后读回来全为假()
        {
            var view = new PathView();
            try
            {
                view.InstallPath = @"C:\Apps\Y";
                view.DesktopShortcut = false;
                view.StartMenuShortcut = false;
                view.Autostart = false;
                view.RunAfterInstall = false;
                view.AllUsers = false;

                var options = new WizardOptions
                {
                    DesktopShortcut = true,
                    StartMenuShortcut = true,
                    Autostart = true,
                    RunAfterInstall = true,
                    AllUsers = true,
                };
                view.ReadInto(options);

                Assert.False(options.DesktopShortcut);
                Assert.False(options.StartMenuShortcut);
                Assert.False(options.Autostart);
                Assert.False(options.RunAfterInstall);
                Assert.False(options.AllUsers);
            }
            finally
            {
                ((IDisposable)view).Dispose();
            }
        }

        [Fact]
        public void 路径为空时不覆盖已有安装目录()
        {
            var view = new PathView();
            try
            {
                view.InstallPath = string.Empty;

                var options = new WizardOptions { InstallDir = @"C:\Keep" };
                view.ReadInto(options);

                Assert.Equal(@"C:\Keep", options.InstallDir);
            }
            finally
            {
                ((IDisposable)view).Dispose();
            }
        }

        [Fact]
        public void 传空对象不抛异常()
        {
            var view = new PathView();
            try
            {
                view.ReadInto(null);
            }
            finally
            {
                ((IDisposable)view).Dispose();
            }
        }

        [Fact]
        public void 勾选变化会触发OptionsChanged事件()
        {
            var view = new PathView();
            try
            {
                var fired = 0;
                view.OptionsChanged += () => fired++;

                view.DesktopShortcut = true;
                view.DesktopShortcut = false;

                Assert.True(fired > 0, "勾选变化没有触发 OptionsChanged —— 回写链路会再次断掉");
            }
            finally
            {
                ((IDisposable)view).Dispose();
            }
        }

        [Fact]
        public void 原子灌值后四项勾选都要在_不能被前一个抹掉()
        {
            // 用户实测反馈："制作端勾了快捷方式，安装包执行的时候没按勾选。"
            //
            // 根因就在这条链路上：往界面灌值时一个个设属性 →
            // 设第一个勾选框触发 OptionsChanged → 窗体回调 ReadInto →
            // 把**另外三个还没灌进去的**勾选框（此刻还是未勾）读回模型 →
            // 清单里四项全勾，界面只剩第一项，装出来也就只建一个快捷方式。
            var view = new PathView();
            try
            {
                var options = new WizardOptions
                {
                    InstallDir = @"D:\Apps\Demo",
                    AllUsers = true,
                    DesktopShortcut = true,
                    StartMenuShortcut = true,
                    Autostart = true,
                    RunAfterInstall = true,
                };

                view.ApplyOptions(options);

                var back = new WizardOptions();
                view.ReadInto(back);

                Assert.Equal(@"D:\Apps\Demo", back.InstallDir);
                Assert.True(back.AllUsers);
                Assert.True(back.DesktopShortcut, "桌面快捷方式被抹掉了");
                Assert.True(back.StartMenuShortcut, "开始菜单快捷方式被抹掉了");
                Assert.True(back.Autostart, "开机自启被抹掉了");
                Assert.True(back.RunAfterInstall, "安装后运行被抹掉了");
            }
            finally
            {
                ((IDisposable)view).Dispose();
            }
        }

        [Fact]
        public void 原子灌值期间不应触发任何回写事件()
        {
            // 灌值是"模型 → 视图"的单向操作，期间绝不能反过来写模型（设计文档 §9 不变量 #9）。
            var view = new PathView();
            try
            {
                var fired = 0;
                view.OptionsChanged += () => fired++;
                view.PathChanged += () => fired++;
                view.AllUsersChanged += () => fired++;

                view.ApplyOptions(new WizardOptions
                {
                    InstallDir = @"D:\Apps\X",
                    AllUsers = true,
                    DesktopShortcut = true,
                    StartMenuShortcut = true,
                    Autostart = true,
                    RunAfterInstall = true,
                });

                Assert.Equal(0, fired);
            }
            finally
            {
                ((IDisposable)view).Dispose();
            }
        }

        [Fact]
        public void 点快捷路径chip应触发事件并带上根目录()
        {
            var view = new PathView();
            try
            {
                string root = null;
                view.PathChipClicked += r => root = r;

                view.ActivateChip(PathView.ChipLocalAppData);
                Assert.Equal(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), root);

                view.ActivateChip(PathView.ChipProgramFilesX86);
                Assert.Equal(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), root);
            }
            finally
            {
                ((IDisposable)view).Dispose();
            }
        }

        [Fact]
        public void Program_Files_那个选项已经删掉了_只保留_x86()
        {
            // 用户实测："选 Program Files 和 Program Files (x86) 没有区别。"
            // 原因是安装器是 x86 进程，SpecialFolder.ProgramFiles 被 WOW64 重定向到
            // C:\Program Files (x86) —— 两个选项指向同一个目录。所以那个 chip 被删了。
            var view = new PathView();
            try
            {
                string root = null;

                Assert.True(view.TryGetChipTarget(PathView.ChipProgramFilesX86, out root));

                var x86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
                Assert.Equal(x86, root);

                // 64 位系统上两者不同，这里顺带证明"用的是 (x86) 那个"，不是被重定向的 ProgramFiles
                var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                if (!string.Equals(x86, programFiles, StringComparison.OrdinalIgnoreCase))
                {
                    Assert.NotEqual(programFiles, root);
                }
            }
            finally
            {
                ((IDisposable)view).Dispose();
            }
        }

        [Fact]
        public void 没有D盘时不应提供D盘那个选项()
        {
            var view = new PathView();
            try
            {
                string root;

                var available = view.TryGetChipTarget(PathView.ChipDriveD, out root);

                // 结论必须和本机实际情况一致：有 D 盘才给这个选项
                Assert.Equal(System.IO.Directory.Exists(@"D:\"), available);

                if (available)
                {
                    Assert.Equal(@"D:\Program Files (x86)", root);
                }
            }
            finally
            {
                ((IDisposable)view).Dispose();
            }
        }

        [Fact]
        public void 未知的chip序号不应触发事件()
        {
            var view = new PathView();
            try
            {
                var fired = 0;
                view.PathChipClicked += r => fired++;

                view.ActivateChip(0);
                view.ActivateChip(99);

                Assert.Equal(0, fired);
            }
            finally
            {
                ((IDisposable)view).Dispose();
            }
        }

        [Fact]
        public void 勾选所有用户会触发AllUsersChanged事件()
        {
            var view = new PathView();
            try
            {
                var fired = 0;
                view.AllUsersChanged += () => fired++;

                view.AllUsers = true;

                Assert.True(fired > 0, "「为所有用户安装」没有触发 AllUsersChanged —— 切换范围不会换默认目录");
            }
            finally
            {
                ((IDisposable)view).Dispose();
            }
        }
    }
}
