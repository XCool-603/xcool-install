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
