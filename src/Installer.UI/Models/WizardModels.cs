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

        /// <summary>
        /// 更新信息（已经装过同一产品时**替代**「安装设置」页）。
        ///
        /// 用户诉求：已经装了 1.0.0 再装新包时，不该再让用户选安装目录/附加任务，
        /// 而是直接告诉他"当前已安装 X、要安装 Y"，主按钮变成「更新」。
        /// </summary>
        Update = 10,
    }

    /// <summary>本次安装与"已安装版本"的关系。</summary>
    public enum UpdateKind
    {
        /// <summary>没装过 —— 全新安装。</summary>
        None = 0,

        /// <summary>要装的是**更高**版本 —— 正常更新。</summary>
        Newer = 1,

        /// <summary>版本相同 —— 相当于重新安装。</summary>
        Same = 2,

        /// <summary>要装的是**更低**版本 —— 降级安装。</summary>
        Older = 3,
    }

    /// <summary>本机已经装着的那个产品（从安装记录里读出来）。</summary>
    public sealed class InstalledProduct
    {
        /// <summary>已安装版本。</summary>
        public string Version { get; set; }

        /// <summary>已安装目录 —— 更新时**直接写这里**，不再让用户选。</summary>
        public string InstallDir { get; set; }

        /// <summary>
        /// 已安装产品的名字（从安装记录/卸载键里读到的）。
        ///
        /// 用来判断"这是不是同一个产品"：不同产品如果碰巧共用了 productCode，
        /// 不能把对方当成"已安装的本产品" —— 否则全新安装会被误判成"更新"
        /// （用户实测：3 个不同产品，装第 2 个时提示"更新"）。
        /// </summary>
        public string ProductName { get; set; }
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

        /// <summary>是否为所有用户安装（对应 perMachine）。</summary>
        public bool AllUsers { get; set; }
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

        /// <summary>
        /// 按用户当前的选择，安装**开始之前**是否必须先提权。
        ///
        /// 三种情况都要提：
        /// ① 清单勾了「始终请求管理员权限」；
        /// ② 用户勾了「为所有用户安装」（要写 HKLM）；
        /// ③ 安装目录本身写不进去（典型：普通用户把目录改到了 Program Files）。
        /// </summary>
        bool NeedsElevationRestart(WizardOptions options);

        /// <summary>
        /// 以管理员身份重启安装程序，并把当前选择带过去（新实例从「安装位置」页接着走）。
        /// </summary>
        /// <returns>true = 已启动，本实例应关闭；false = 用户在 UAC 上点了「否」。</returns>
        bool RestartElevated(WizardOptions options);

        /// <summary>安装完成后启动入口程序。</summary>
        void LaunchEntryPoint();
    }
}
