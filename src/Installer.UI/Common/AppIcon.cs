using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Installer.UI.Common
{
    /// <summary>
    /// 窗口图标。
    ///
    /// 为什么不靠 WinForms 的默认行为：窗体不显式设置 <see cref="Form.Icon"/> 时，
    /// 标题栏 / 任务栏 / Alt+Tab 用哪个图标取决于窗口类图标与宿主进程，
    /// 并不保证等于 exe 自身的图标 —— 表现出来就是「换了图标，但很多地方还是旧的」。
    /// 显式设置才能保证这几处一致。
    ///
    /// 图标从**当前进程的 exe** 取，于是：
    ///   - 制作端 → 取 安装包制作助手.exe 的图标
    ///   - 安装向导 → 取 Setup.exe 的图标，也就是打包时由 PeIconPatcher
    ///     打进去的那个（用户在制作端选的图标），自动跟着走。
    /// </summary>
    public static class AppIcon
    {
        private static Icon _cached;
        private static bool _resolved;

        /// <summary>
        /// 取当前 exe 的图标；取不到返回 null（窗体沿用系统默认图标）。
        /// 设计器里返回 null —— 否则会去取 devenv.exe 的图标。
        /// </summary>
        public static Icon Load()
        {
            if (_resolved)
            {
                return _cached;
            }

            _resolved = true;

            // 构造函数里 DesignMode 不可靠，要用 LicenseManager
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                return null;
            }

            try
            {
                var exe = Application.ExecutablePath;
                if (!string.IsNullOrEmpty(exe) && File.Exists(exe))
                {
                    _cached = Icon.ExtractAssociatedIcon(exe);
                }
            }
            catch (ArgumentException)
            {
            }
            catch (IOException)
            {
            }
            catch (System.Security.SecurityException)
            {
            }

            return _cached;
        }
    }
}
