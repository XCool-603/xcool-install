using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Installer.Abstractions.Model;
using Installer.Abstractions.Platform;
using Installer.UI.Common;
using Installer.UI.Models;

namespace Installer.UI.Presenters
{
    /// <summary>
    /// 向导的状态机。
    ///
    /// **这个类存在的首要理由是消灭"用按钮文字当状态"这个地雷** ——
    /// 现状的安装器用 <c>BtnInstall.Text.Equals("立即安装")</c> 判断状态
    /// （<c>FrmInstallation.cs:173</c>），界面一翻译成英文，安装器立刻失效。
    ///
    /// 这里状态是枚举，文案只是状态的**输出**。
    /// 不引用 WinForms，所以可以完整单测。
    /// </summary>
    public sealed class WizardPresenter
    {
        private readonly IStringTable _text;
        private readonly InstallerManifest _manifest;
        private readonly bool _licenseRequired;
        private readonly bool _hasOptions;

        /// <summary>构造。</summary>
        /// <param name="manifest">清单。</param>
        /// <param name="text">文案表（应已合并内置文案）。</param>
        /// <param name="defaultInstallDir">已解析好的默认安装目录。</param>
        public WizardPresenter(InstallerManifest manifest, IStringTable text, string defaultInstallDir)
        {
            if (manifest == null)
            {
                throw new ArgumentNullException("manifest");
            }

            _manifest = manifest;
            _text = text ?? new StringTable(WizardStrings.CreateDefault(), null, "zh-Hans");

            _licenseRequired = manifest.License != null && manifest.License.Required;

            var hasShortcuts = manifest.Shortcuts != null && manifest.Shortcuts.Count > 0;
            _hasOptions = hasShortcuts || manifest.Autostart || manifest.RunAfterInstall;

            Options = new WizardOptions
            {
                InstallDir = defaultInstallDir,
                AcceptLicense = !_licenseRequired,
                DesktopShortcut = HasShortcutAt("Desktop"),
                StartMenuShortcut = HasShortcutAt("StartMenu"),
                Autostart = manifest.Autostart,
                RunAfterInstall = manifest.RunAfterInstall,
                // 「为所有用户安装」必须从清单的 scope 初始化，否则界面显示的状态
                // 和实际安装范围不一致（清单是 perMachine 时复选框却是空的）。
                AllUsers = string.Equals(manifest.Scope, InstallScopes.PerMachine,
                                         StringComparison.OrdinalIgnoreCase),
            };

            Current = WizardPage.Welcome;
            ErrorMessage = null;
        }

        /// <summary>当前页面。</summary>
        public WizardPage Current { get; private set; }

        /// <summary>用户选择。</summary>
        public WizardOptions Options { get; private set; }

        /// <summary>失败原因（仅在 <see cref="WizardPage.Failed"/> 有意义）。</summary>
        public string ErrorMessage { get; private set; }

        /// <summary>是否显示"上一步"。</summary>
        public bool CanGoBack
        {
            get
            {
                switch (Current)
                {
                    case WizardPage.License:
                        return true;
                    case WizardPage.Path:
                        return true;
                    case WizardPage.Options:
                        return true;
                    case WizardPage.Update:
                        // 更新页可以退回上一步（欢迎 / 许可）
                        return true;
                    default:
                        return false;
                }
            }
        }

        /// <summary>主按钮的文案 key。</summary>
        public string PrimaryKey
        {
            get
            {
                switch (Current)
                {
                    case WizardPage.Path:
                    case WizardPage.Options:
                        // 设计稿把「安装位置」与「附加任务」合成一页，所以这一页的主按钮直接是「安装」
                        return "Wizard.Install";
                    case WizardPage.Update:
                        // 已装过 → 按版本关系换按钮：更新 / 重新安装 / 仍要安装
                        return Kind == UpdateKind.Same
                            ? "Wizard.Reinstall"
                            : Kind == UpdateKind.Older ? "Wizard.InstallAnyway" : "Wizard.Update";
                    case WizardPage.Installing:
                    case WizardPage.Uninstalling:
                        return "Wizard.Cancel";
                    case WizardPage.Finish:
                    case WizardPage.Uninstalled:
                        return "Wizard.Finish";
                    case WizardPage.Failed:
                        return "Wizard.Retry";
                    case WizardPage.UninstallConfirm:
                        return "Uninstall.Title";
                    default:
                        return "Wizard.Next";
                }
            }
        }

        /// <summary>主按钮是否可用。</summary>
        public bool PrimaryEnabled
        {
            get
            {
                if (Current == WizardPage.License)
                {
                    return Options.AcceptLicense;
                }

                if (Current == WizardPage.Path)
                {
                    return !string.IsNullOrWhiteSpace(Options.InstallDir);
                }

                return true;
            }
        }

        /// <summary>取消按钮的文案 key（完成后变成"关闭"）。</summary>
        public string SecondaryKey
        {
            get
            {
                switch (Current)
                {
                    case WizardPage.Finish:
                    case WizardPage.Uninstalled:
                    case WizardPage.Failed:
                        return "Wizard.Close";
                    default:
                        return "Wizard.Cancel";
                }
            }
        }

        /// <summary>主按钮是否应该表现为"危险/取消"（安装中时它是取消）。</summary>
        public bool PrimaryIsCancel
        {
            get { return Current == WizardPage.Installing || Current == WizardPage.Uninstalling; }
        }

        /// <summary>取本地化文案。</summary>
        public string Text(string key)
        {
            var v = _text.Get(key);
            return string.IsNullOrEmpty(v) ? key : v;
        }

        /// <summary>产品显示名。</summary>
        public string ProductName
        {
            get { return _text.Resolve(_manifest.Product.Name, _manifest.ProductCode); }
        }

        /// <summary>产品版本。</summary>
        public string ProductVersion
        {
            get { return _manifest.Product.Version; }
        }

        // ─────────────────────────────────────────────────────────────
        // 更新 / 重新安装 / 降级
        // ─────────────────────────────────────────────────────────────

        /// <summary>本机已装着的那个产品（null = 全新安装）。</summary>
        public InstalledProduct Installed { get; private set; }

        /// <summary>是否走"更新"流程（已装过同一产品 → 跳过「安装设置」页）。</summary>
        public bool IsUpdate
        {
            get { return Installed != null && !string.IsNullOrWhiteSpace(Installed.Version); }
        }

        /// <summary>本次安装与已安装版本的关系。</summary>
        public UpdateKind Kind
        {
            get
            {
                if (!IsUpdate)
                {
                    return UpdateKind.None;
                }

                var cmp = Installer.Core.Install.PreflightStep.CompareVersions(
                    Installed.Version, _manifest.Product.Version);

                if (cmp < 0)
                {
                    return UpdateKind.Newer;   // 新版本更高
                }

                return cmp == 0 ? UpdateKind.Same : UpdateKind.Older;
            }
        }

        /// <summary>
        /// 告诉向导"本机已经装了这个产品"。
        ///
        /// 一旦设置：不再进「安装设置」页，安装目录直接用已安装的那个（用户诉求：
        /// 更新时不要重新选目录），主按钮文案按版本关系变成「更新 / 重新安装 / 仍要安装」。
        /// </summary>
        public void SetInstalled(InstalledProduct installed)
        {
            Installed = installed;

            if (installed != null && !string.IsNullOrWhiteSpace(installed.InstallDir))
            {
                Options.InstallDir = installed.InstallDir;
            }
        }

        /// <summary>更新页的标题（按版本关系换措辞）。</summary>
        public string UpdateTitle
        {
            get
            {
                var key = Kind == UpdateKind.Same
                    ? "Update.Title.Same"
                    : Kind == UpdateKind.Older ? "Update.Title.Older" : "Update.Title";

                return string.Format(CultureInfo.CurrentCulture, _text.Get(key), ProductName);
            }
        }

        /// <summary>更新页的副标题。</summary>
        public string UpdateSubtitle
        {
            get
            {
                return string.Format(CultureInfo.CurrentCulture, _text.Get("Update.Subtitle"),
                    ProductName, ProductVersion);
            }
        }

        /// <summary>更新页的提示条（说明这次会发生什么）。</summary>
        public string UpdateHint
        {
            get
            {
                switch (Kind)
                {
                    case UpdateKind.Same:
                        return string.Format(CultureInfo.CurrentCulture, _text.Get("Update.Hint.Same"),
                            Installed.Version);
                    case UpdateKind.Older:
                        return string.Format(CultureInfo.CurrentCulture, _text.Get("Update.Hint.Older"),
                            ProductVersion, Installed.Version);
                    case UpdateKind.Newer:
                        return string.Format(CultureInfo.CurrentCulture, _text.Get("Update.Hint.Newer"),
                            Installed.Version, ProductVersion);
                    default:
                        return string.Empty;
                }
            }
        }

        /// <summary>降级安装时，需要把"允许降级"传下去（否则预检会拦住）。</summary>
        public bool AllowDowngrade
        {
            get { return Kind == UpdateKind.Older; }
        }

        /// <summary>发布者（按当前语言）。</summary>
        public string Publisher
        {
            get { return _text.Resolve(_manifest.Product.Publisher, string.Empty); }
        }

        /// <summary>
        /// 厂商名 —— 安装目录的**厂商层级**用它（<c>{根}\{厂商}\{产品}</c>）。
        /// 没填厂商返回空串，表示不建这一层。
        /// </summary>
        public string Vendor
        {
            get
            {
                return InstallPaths.SanitizeOrEmpty(_text.Resolve(_manifest.Product.Publisher, string.Empty));
            }
        }

        /// <summary>许可协议正文（按当前语言）。</summary>
        public string LicenseText
        {
            get
            {
                if (_manifest.License == null)
                {
                    return string.Empty;
                }

                return _text.Resolve(_manifest.License.Text, string.Empty);
            }
        }

        /// <summary>当前页面在步骤指示器里的序号（0 基）；不适用时返回 -1。</summary>
        public int StepIndex
        {
            get
            {
                switch (Current)
                {
                    case WizardPage.Welcome:
                        return 0;
                    case WizardPage.License:
                        return 1;
                    case WizardPage.UninstallConfirm:
                        return 0;
                    case WizardPage.Uninstalling:
                        return 1;
                    case WizardPage.Uninstalled:
                        return 2;
                    case WizardPage.Path:
                    case WizardPage.Options:
                        // 设计稿把「安装位置」与「附加任务」合成一步「安装设置」
                        // 没有许可协议时许可页被跳过，后面的步骤号整体前移一位
                        return _licenseRequired ? 2 : 1;
                    case WizardPage.Update:
                        // 更新流程：第 2 步就是「更新信息」（没有「安装设置」）
                        return _licenseRequired ? 2 : 1;
                    case WizardPage.Installing:
                        return _licenseRequired ? 3 : 2;
                    case WizardPage.Finish:
                        return _licenseRequired ? 4 : 3;
                    default:
                        return -1;
                }
            }
        }

        /// <summary>
        /// 当前流程实际会经过的步骤文案 key。
        /// 没勾「必须接受许可」时许可页会被跳过，步骤条就应该是 4 步而不是 5 步 ——
        /// 否则会看到「欢迎」直接跳到「安装设置」，中间那格永远高亮不了。
        /// </summary>
        /// <summary>清单是否要求必须接受许可协议。</summary>
        public bool LicenseRequired
        {
            get { return _licenseRequired; }
        }

        /// <summary>是否处于卸载流程（三页：确认卸载 / 正在卸载 / 完成）。</summary>
        public bool IsUninstallFlow
        {
            get
            {
                return Current == WizardPage.UninstallConfirm
                    || Current == WizardPage.Uninstalling
                    || Current == WizardPage.Uninstalled;
            }
        }

        /// <summary>当前流程实际会经过的步骤文案 key。</summary>
        public string[] StepKeys
        {
            get
            {
                // 卸载流程是 3 步（确认卸载 / 正在卸载 / 完成）
                if (IsUninstallFlow)
                {
                    return new[] { "Step.UninstallConfirm", "Step.Uninstalling", "Wizard.Finish" };
                }

                // 更新流程：没有「安装设置」页，第 2 步换成「更新信息」
                if (IsUpdate)
                {
                    return _licenseRequired
                        ? new[] { "Wizard.StepWelcome", "Step.License", "Step.Update", "Step.Install", "Wizard.Finish" }
                        : new[] { "Wizard.StepWelcome", "Step.Update", "Step.Install", "Wizard.Finish" };
                }

                // 安装流程：没勾「必须接受许可」时许可页会被跳过，步骤条就是 4 步而不是 5 步
                return _licenseRequired
                    ? new[] { "Wizard.StepWelcome", "Step.License", "Wizard.StepSettings", "Step.Install", "Wizard.Finish" }
                    : new[] { "Wizard.StepWelcome", "Wizard.StepSettings", "Step.Install", "Wizard.Finish" };
            }
        }

        /// <summary>步骤指示器的条目数。</summary>
        public int StepCount
        {
            get { return StepKeys.Length; }
        }

        // ─────────────────────────────────────────────────────────────
        // 状态迁移
        // ─────────────────────────────────────────────────────────────

        /// <summary>点主按钮。返回 true 表示界面需要刷新；返回 false 表示校验未通过。</summary>
        public bool Primary()
        {
            switch (Current)
            {
                case WizardPage.Welcome:
                    Current = _licenseRequired ? WizardPage.License : NextAfterWelcome();
                    return true;

                case WizardPage.License:
                    if (!Options.AcceptLicense)
                    {
                        return false;
                    }

                    Current = NextAfterWelcome();
                    return true;

                case WizardPage.Update:
                    // 更新页 → 直接开装（文件写进已安装目录）
                    Current = WizardPage.Installing;
                    return true;

                case WizardPage.Path:
                    if (string.IsNullOrWhiteSpace(Options.InstallDir))
                    {
                        return false;
                    }

                    // 设计稿把「安装位置」与「附加任务」合成一页，所以不再经过 Options
                    Current = WizardPage.Installing;
                    return true;

                case WizardPage.Options:
                    Current = WizardPage.Installing;
                    return true;

                case WizardPage.Installing:
                case WizardPage.Uninstalling:
                    // 安装中主按钮是"取消"
                    return true;

                case WizardPage.Finish:
                case WizardPage.Uninstalled:
                    return true;

                case WizardPage.Failed:
                    Current = WizardPage.Installing;
                    ErrorMessage = null;
                    return true;

                case WizardPage.UninstallConfirm:
                    Current = WizardPage.Uninstalling;
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// 欢迎页/许可页之后该去哪个页。
        ///
        /// 已经装过同一产品 → **跳过「安装设置」**，直接进「更新信息」页
        /// （用户诉求：更新时不要再让用户选目录/勾任务，只告诉他版本关系）。
        /// </summary>
        private WizardPage NextAfterWelcome()
        {
            return IsUpdate ? WizardPage.Update : WizardPage.Path;
        }

        /// <summary>点"上一步"。</summary>
        public void Back()
        {
            switch (Current)
            {
                case WizardPage.License:
                    Current = WizardPage.Welcome;
                    break;
                case WizardPage.Path:
                    Current = _licenseRequired ? WizardPage.License : WizardPage.Welcome;
                    break;
                case WizardPage.Options:
                    Current = WizardPage.Path;
                    break;
                case WizardPage.Update:
                    Current = _licenseRequired ? WizardPage.License : WizardPage.Welcome;
                    break;
            }
        }

        /// <summary>进入安装中。</summary>
        public void EnterInstalling()
        {
            Current = WizardPage.Installing;
            ErrorMessage = null;
        }

        /// <summary>
        /// 重置回欢迎页（供视觉验收逐页截图用）。
        /// 只重置页面与许可接受状态，不动用户的路径/选项选择。
        /// </summary>
        public void Reset()
        {
            Current = WizardPage.Welcome;
            ErrorMessage = null;
            Options.AcceptLicense = !_licenseRequired;
        }

        /// <summary>
        /// **仅供渲染/截图使用**：直接把当前页设成指定值，绕过 <see cref="Primary"/> 的流程约束。
        ///
        /// 为什么不能靠 Primary() 逐步推进：
        ///   · 不需要许可协议时（<c>_licenseRequired=false</c>），
        ///     Welcome --Primary--> **直接到 Path**，License 页**根本不可达**；
        ///   · Path --Primary--> **直接到 Installing**，
        ///     Options 页在设计稿里已经并入 Path，同样不可达。
        /// 于是"想截 License 页"这件事在流程上做不到 —— 只能直接设页。
        ///
        /// 这个方法只影响 <see cref="Current"/>，不改任何用户选项，
        /// 也不参与真实安装流程。
        /// </summary>
        internal void ForceCurrentForPreview(WizardPage page)
        {
            Current = page;
        }

        /// <summary>安装成功。</summary>
        public void EnterFinish()
        {
            Current = WizardPage.Finish;
        }

        /// <summary>安装失败。</summary>
        public void EnterFailed(string message)
        {
            ErrorMessage = message;
            Current = WizardPage.Failed;
        }

        /// <summary>进入卸载确认。</summary>
        public void EnterUninstallConfirm()
        {
            Current = WizardPage.UninstallConfirm;
        }

        /// <summary>进入卸载中。</summary>
        public void EnterUninstalling()
        {
            Current = WizardPage.Uninstalling;
        }

        /// <summary>卸载完成。</summary>
        public void EnterUninstalled()
        {
            Current = WizardPage.Uninstalled;
        }

        /// <summary>安装中的主按钮被按下 → 用户要求取消。</summary>
        public bool IsCancelling
        {
            get { return Current == WizardPage.Installing || Current == WizardPage.Uninstalling; }
        }

        /// <summary>失败后是否可以重试。</summary>
        public bool CanRetry
        {
            get { return Current == WizardPage.Failed; }
        }

        /// <summary>当前页面是否允许直接关闭窗口。</summary>
        public bool CanClose
        {
            get
            {
                return Current != WizardPage.Installing
                       && Current != WizardPage.Uninstalling
                       && Current != WizardPage.Finish
                       && Current != WizardPage.Uninstalled;
            }
        }

        /// <summary>是否显示步骤指示器。</summary>
        public bool ShowSteps
        {
            get { return StepIndex >= 0; }
        }

        private bool HasShortcutAt(string location)
        {
            if (_manifest.Shortcuts == null)
            {
                return false;
            }

            return _manifest.Shortcuts.Any(s =>
                string.Equals(s.Location, location, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>把用户在向导里的选择写回清单（引擎按清单执行）。</summary>
        public void ApplyToManifest()
        {
            var shortcuts = new List<ShortcutSpec>();

            if (Options.DesktopShortcut)
            {
                shortcuts.Add(FindOrCreate("Desktop"));
            }

            if (Options.StartMenuShortcut)
            {
                shortcuts.Add(FindOrCreate("StartMenu"));
            }

            _manifest.Shortcuts = shortcuts;
            _manifest.Autostart = Options.Autostart;
            _manifest.RunAfterInstall = Options.RunAfterInstall;

            // 安装范围也必须写回去！
            // 漏了这一句的后果：「为所有用户安装」勾了等于没勾 —— Scope 还是 perUser，
            // 于是装进用户目录、只写 HKCU、不建 HKLM 卸载项，控制面板里所有用户都看不到它。
            _manifest.Scope = Options.AllUsers ? InstallScopes.PerMachine : InstallScopes.PerUser;

            // 安装位置也必须写回去！
            // 漏了这一句的后果：用户在"安装位置"页改成 D:\，装完还是在 C:\ —— 因为引擎读的是清单里的旧值。
            if (!string.IsNullOrWhiteSpace(Options.InstallDir))
            {
                _manifest.DefaultInstallDir = Options.InstallDir;
            }
        }

        /// <summary>
        /// 切换安装范围时把安装目录换成对应的默认值。
        ///
        /// 只在这一下重置：用户之后手工改过的路径不会被覆盖。
        /// 不重置的话会出现"勾了所有用户、目录还在 %LOCALAPPDATA%"——
        /// 名义上是全机器安装，实际却装在用户目录里，权限和位置都对不上。
        /// </summary>
        public void ApplyScopeDefaultDir()
        {
            Options.InstallDir = Environment.ExpandEnvironmentVariables(
                InstallPaths.Suggest(ProductName, Vendor, Options.AllUsers));
        }

        /// <summary>
        /// 把一个**根目录**（Program Files / 本地 AppData / D:\Program Files…）变成真正的安装目录。
        ///
        /// 「安装设置」页那排快捷路径 chips 以前是死的（点了没反应）——
        /// 用户只能靠「浏览」一级级点进去。这里补上：点 chip → 按「根\厂商\产品」拼出最终目录。
        /// 逻辑放在 Presenter 里（而不是窗体里）是为了能被单测钉住：
        /// 少拼一层厂商、或者把产品名拼丢，界面不会报错，只会**静默装错地方**。
        /// </summary>
        /// <param name="rootDirectory">根目录（可含 %ENV%）。</param>
        public string ComposeInstallDir(string rootDirectory)
        {
            return Environment.ExpandEnvironmentVariables(
                InstallPaths.Compose(rootDirectory, Vendor, ProductName));
        }

        /// <summary>
        /// 直接把当前页设成指定页。
        /// 用于两处：提权重启后的续跑（回到「安装位置」页）、以及 UAC 被拒后回退。
        /// </summary>
        public void StartAt(WizardPage page)
        {
            Current = page;
        }

        /// <summary>
        /// 提权重启后的续跑：把上次会话的选择合并进来，并停在「安装位置」页。
        ///
        /// 放在 Presenter 而不是窗体里，是为了**能被单测覆盖** ——
        /// 这段代码决定"用户提权后要不要重新勾一遍选项"，
        /// 写错了不会报错，只会让用户刚才的选择悄悄丢失。
        /// </summary>
        /// <param name="saved">上次会话的选择；为 null 时什么都不做。</param>
        public void ApplyResume(WizardOptions saved)
        {
            if (saved == null)
            {
                return;
            }

            var o = Options;

            // 目录为空说明上次没选过，保留清单里的默认值
            if (!string.IsNullOrWhiteSpace(saved.InstallDir))
            {
                o.InstallDir = saved.InstallDir;
            }

            o.AcceptLicense = saved.AcceptLicense;
            o.DesktopShortcut = saved.DesktopShortcut;
            o.StartMenuShortcut = saved.StartMenuShortcut;
            o.Autostart = saved.Autostart;
            o.RunAfterInstall = saved.RunAfterInstall;
            o.AllUsers = saved.AllUsers;

            Current = WizardPage.Path;
        }

        private ShortcutSpec FindOrCreate(string location)
        {
            var existing = _manifest.Shortcuts != null
                ? _manifest.Shortcuts.FirstOrDefault(s =>
                    string.Equals(s.Location, location, StringComparison.OrdinalIgnoreCase))
                : null;

            return existing ?? new ShortcutSpec
            {
                Location = location,
                Name = _manifest.Product.Name,
            };
        }
    }
}
