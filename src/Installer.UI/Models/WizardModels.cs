using System;
using Installer.Abstractions.Install;

namespace Installer.UI.Models
{
    /// <summary>向导页面。</summary>
    public enum WizardPage
    {
        /// <summary>欢迎。</summary>
        Welcome = 0,

        /// <summary>许可协议。</summary>
        License = 1,

        /// <summary>安装位置。</summary>
        Path = 2,

        /// <summary>选项。</summary>
        Options = 3,

        /// <summary>正在安装（进度）。</summary>
        Installing = 4,

        /// <summary>安装完成。</summary>
        Finish = 5,

        /// <summary>安装失败。</summary>
        Failed = 6,

        /// <summary>卸载确认。</summary>
        UninstallConfirm = 7,

        /// <summary>正在卸载。</summary>
        Uninstalling = 8,

        /// <summary>卸载完成。</summary>
        Uninstalled = 9,
    }

    /// <summary>用户在向导里做的选择。</summary>
    public sealed class WizardOptions
    {
        /// <summary>安装目录。</summary>
        public string InstallDir { get; set; }

        /// <summary>是否接受了许可协议。</summary>
        public bool AcceptLicense { get; set; }

        /// <summary>是否创建桌面快捷方式。</summary>
        public bool DesktopShortcut { get; set; }

        /// <summary>是否创建开始菜单快捷方式。</summary>
        public bool StartMenuShortcut { get; set; }

        /// <summary>是否开机启动。</summary>
        public bool Autostart { get; set; }

        /// <summary>安装完成后是否立即运行。</summary>
        public bool RunAfterInstall { get; set; }
    }

    /// <summary>
    /// UI 与安装引擎之间的桥。
    ///
    /// 有了它，<see cref="Presenters.WizardPresenter"/> 与所有 View 都不需要引用
    /// <c>InstallEngine</c>，也就不需要引用 WinForms 之外的东西 —— 于是它们可以单测。
    /// </summary>
    public interface IWizardHost
    {
        /// <summary>开始安装。进度通过回调送回（可能在后台线程）。</summary>
        /// <param name="options">用户选择。</param>
        /// <param name="onProgress">进度回调。</param>
        /// <param name="onDone">完成回调：(成功, 失败原因 / 完成后的安装目录)。</param>
        void StartInstall(WizardOptions options, Action<InstallProgress> onProgress, Action<bool, string> onDone);

        /// <summary>请求取消安装（会触发回滚）。</summary>
        void RequestCancel();

        /// <summary>开始卸载。</summary>
        void StartUninstall(Action<InstallProgress> onProgress, Action<bool, string> onDone);

        /// <summary>安装完成后启动入口程序。</summary>
        void LaunchEntryPoint();
    }
}
