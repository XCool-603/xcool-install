using System;
using Installer.Abstractions.Model;

namespace Installer.Core.Platform
{
    /// <summary>
    /// 提权决策 —— **纯判定，不做任何动作**。
    ///
    /// 抽出来的理由很实际：这套判断以前内联在 <c>InstallHost</c> / <c>WizardHostImpl</c> 里，
    /// 只能靠"真的弹一次 UAC"来验证。而验证机上 UAC 是关的（EnableLUA=0、内置 Administrator），
    /// 于是这条最关键的分支长期处于"写了但没人验过"的状态 ——
    /// 用户把目录改到 Program Files 装到一半失败，就是从这个盲区里漏出去的。
    ///
    /// 现在决策在这里，用布尔入参穷举；真正会弹 UAC 的动作只留在调用方那几行。
    /// </summary>
    public static class ElevationRules
    {
        /// <summary>
        /// 安装阶段（含静默安装）要不要提权。
        ///
        /// 两个来源：清单要求（所有用户 / 始终提权），以及安装目录本身写不进去。
        /// </summary>
        /// <param name="manifest">清单，可为 null。</param>
        /// <param name="installDir">已解析的安装目录。</param>
        /// <param name="isElevated">当前进程是否已经是管理员。</param>
        public static bool InstallNeedsAdmin(InstallerManifest manifest, string installDir, bool isElevated)
        {
            if (isElevated)
            {
                return false;
            }

            return ScopeRequiresAdmin(manifest) || !ElevationProbe.CanWrite(installDir);
        }

        /// <summary>
        /// 清单本身是否要求管理员：<c>scope = perMachine</c>（要写 HKLM），
        /// 或者制作端勾了「始终请求管理员权限」。
        /// </summary>
        public static bool ScopeRequiresAdmin(InstallerManifest manifest)
        {
            if (manifest == null)
            {
                return false;
            }

            if (string.Equals(manifest.Scope, InstallScopes.PerMachine, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return manifest.AlwaysElevate;
        }

        /// <summary>
        /// 向导里点「下一步」、进度条开始**之前**要不要提权。
        ///
        /// 与 <see cref="InstallNeedsAdmin"/> 的差别：这里用的是用户在向导里的**选择**，
        /// 而不是清单里的默认值 —— 用户完全可能在界面上勾上「为所有用户安装」，
        /// 或者把目录改到一个自己写不进去的地方。
        /// </summary>
        /// <param name="isElevated">当前进程是否已经是管理员。</param>
        /// <param name="sandboxed">是否沙箱运行（沙箱一律不提权，否则 e2e/测试会弹 UAC 卡死）。</param>
        /// <param name="alwaysElevate">清单里的「始终请求管理员权限」。</param>
        /// <param name="allUsers">用户是否勾了「为所有用户安装」。</param>
        /// <param name="installDir">用户当前选定的安装目录。</param>
        public static bool WizardNeedsAdmin(bool isElevated, bool sandboxed, bool alwaysElevate,
                                            bool allUsers, string installDir)
        {
            if (isElevated || sandboxed)
            {
                return false;
            }

            if (alwaysElevate || allUsers)
            {
                return true;
            }

            return !string.IsNullOrWhiteSpace(installDir) && !ElevationProbe.CanWrite(installDir);
        }
    }
}
