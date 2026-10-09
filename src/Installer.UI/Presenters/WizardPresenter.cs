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
                    case WizardPage.Options:
                        return "Wizard.Install";
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
                        return -1;
                    case WizardPage.License:
                        return 0;
                    case WizardPage.Path:
                        return 1;
                    case WizardPage.Options:
                        return 2;
                    case WizardPage.Installing:
                        return 3;
                    case WizardPage.Finish:
                        return 4;
                    default:
                        return -1;
                }
            }
        }

        /// <summary>步骤指示器的条目数。</summary>
        public int StepCount
        {
            get { return 5; }
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
                    Current = _licenseRequired ? WizardPage.License : WizardPage.Path;
                    return true;

                case WizardPage.License:
                    if (!Options.AcceptLicense)
                    {
                        return false;
                    }

                    Current = WizardPage.Path;
                    return true;

                case WizardPage.Path:
                    if (string.IsNullOrWhiteSpace(Options.InstallDir))
                    {
                        return false;
                    }

                    Current = _hasOptions ? WizardPage.Options : WizardPage.Installing;
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

            // 安装位置也必须写回去！
            // 漏了这一句的后果：用户在"安装位置"页改成 D:\，装完还是在 C:\ —— 因为引擎读的是清单里的旧值。
            if (!string.IsNullOrWhiteSpace(Options.InstallDir))
            {
                _manifest.DefaultInstallDir = Options.InstallDir;
            }
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
