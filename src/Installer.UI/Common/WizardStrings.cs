using System;
using System.Collections.Generic;
using Installer.Abstractions.Model;

namespace Installer.UI.Common
{
    /// <summary>
    /// 向导的内置文案表（中英双语）。
    ///
    /// 为什么要内置一份：向导必须在**任何**清单下都能显示文字。
    /// 清单里的 <c>strings</c> 会覆盖这里的同名 key，所以制作端可以逐条替换。
    ///
    /// 这也直接兑现了"支持英文"——不需要制作端做任何事，向导默认就有英文。
    /// </summary>
    public static class WizardStrings
    {
        /// <summary>内置的中英文案。</summary>
        public static Dictionary<string, LocalizedText> CreateDefault()
        {
            var t = new Dictionary<string, LocalizedText>(StringComparer.Ordinal);

            Add(t, "Wizard.Subtitle", "安装向导", "Setup Wizard");
            Add(t, "Wizard.Next", "下一步", "Next");
            Add(t, "Wizard.Back", "上一步", "Back");
            Add(t, "Wizard.Cancel", "取消", "Cancel");
            Add(t, "Wizard.Install", "立即安装", "Install");
            Add(t, "Wizard.Update", "立即更新", "Update Now");
            Add(t, "Wizard.Repair", "立即修复", "Repair Now");
            Add(t, "Step.UpdateConfirm", "确认更新", "Confirm Update");
            Add(t, "Step.RepairConfirm", "确认修复", "Confirm Repair");
            Add(t, "Update.Title", "「{0}」版本更新", "{0} Update");
            Add(t, "Update.Body", "检测到本机已安装版本 {0}，即将更新升级至最新版本 {1}。", "Version {0} is currently installed. Ready to update to {1}.");
            Add(t, "Update.Body2", "更新将保留您的所有个人配置与数据，建议在更新前关闭正在运行的相关程序。", "All user data and settings will be preserved. Close any running instances before continuing.");
            Add(t, "Update.Hint", "点击「立即更新」将在原目录完成一键升级。", "Click Update Now to upgrade in-place.");
            Add(t, "Repair.Title", "「{0}」维护与修复", "{0} Maintenance & Repair");
            Add(t, "Repair.Body", "检测到本机已安装相同版本（{0}）。您可以重新安装以修复可能损坏或丢失的文件。", "Version {0} is already installed. Reinstall to repair missing or corrupted files.");
            Add(t, "Repair.Body2", "修复安装将按哈希只补充缺失或损坏的文件，不影响正常数据。", "Repair will restore any missing or corrupted files without affecting your data.");
            Add(t, "Repair.Hint", "点击「立即修复」开始文件校验与补齐。", "Click Repair Now to verify and restore all files.");
            Add(t, "Downgrade.Title", "已安装更高版本", "Newer Version Installed");
            Add(t, "Downgrade.Body", "检测到本机已安装更高版本（{0} > {1}）。如需降级安装，请先卸载现有版本。", "A newer version ({0}) is installed. Please uninstall it first to downgrade.");
            Add(t, "Downgrade.Hint", "为避免数据损坏，安装程序无法直接覆盖安装旧版本。", "To avoid data corruption, older versions cannot overwrite newer ones directly.");
            Add(t, "Wizard.Finish", "完成", "Finish");
            Add(t, "Wizard.InstallKind", "安装向导", "Setup wizard");
            Add(t, "Wizard.UninstallKind", "卸载向导", "Uninstall wizard");
            Add(t, "Wizard.Version", "版本", "version");
            Add(t, "Wizard.Close", "关闭", "Close");
            Add(t, "Wizard.Retry", "重试", "Retry");

            Add(t, "Wizard.StepInfo", "第 {0} 步，共 {1} 步", "Step {0} of {1}");
            Add(t, "Welcome.HintNoLicense", "确认安装位置与附加任务后，点击「下一步」开始安装。",
                "Review the install location and tasks, then click Next to begin.");
            Add(t, "Finish.RunHint", "点击「完成」后将自动启动程序", "The app will start after you click Finish");
            // 完成页的勾选标签与「运行」那一行 —— 设计稿第 5 页的措辞和安装设置页不同，
            // 不能和 Options.* 共用（共用必然有一边写错，这正是之前"两边不一致"的根因）。
            Add(t, "Finish.ChipDesktop", "桌面快捷方式", "Desktop shortcut");
            Add(t, "Finish.ChipStartMenu", "开始菜单", "Start Menu");
            Add(t, "Finish.ChipAutostart", "安装后自动运行", "Run after install");
            Add(t, "Finish.RunNow", "立即运行「{0}」", "Run {0} now");
            Add(t, "Finish.UninstallHint", "如需卸载，可在「设置 → 应用 → 已安装的应用」中找到「{0}」。",
                "To uninstall, find {0} in Settings > Apps.");
            Add(t, "Finish.OpenDir", "打开安装目录", "Open install folder");
            Add(t, "Progress.Subtitle", "安装程序正在解压并写入文件，请不要关闭窗口或关机。",
                "Setup is extracting files. Do not close the window or power off.");
            Add(t, "Progress.CapFiles", "已解压文件", "Files extracted");
            Add(t, "Progress.CapSize", "已写入大小", "Data written");
            Add(t, "Progress.CapElapsed", "已用时间", "Elapsed");
            Add(t, "Progress.CapRemaining", "预计剩余", "Remaining");
            Add(t, "Progress.Step1", "准备安装目录", "Prepare install folder");
            Add(t, "Progress.Step2", "解压程序文件", "Extract program files");
            Add(t, "Progress.Step3", "创建桌面与开始菜单快捷方式", "Create desktop & Start Menu shortcuts");
            Add(t, "Progress.Step4", "写入注册表与卸载信息", "Write registry & uninstall info");
            Add(t, "Path.Subtitle", "确认安装目录，并勾选安装时需要一并完成的操作。",
                "Confirm the install folder and pick the tasks to perform.");
            Add(t, "Options.Desktop.Desc", "在桌面创建启动图标", "Create a launch icon on the desktop");
            Add(t, "Options.StartMenu.Desc", "在开始菜单添加启动项", "Add a launch entry to the Start menu");
            Add(t, "Options.Autostart.Desc", "登录系统后自动运行", "Run automatically after you sign in");
            Add(t, "Options.RunAfter.Desc", "点「完成」后立即启动", "Launch right after you click Finish");
            Add(t, "Options.AllUsers", "为所有用户安装", "Install for all users");
            Add(t, "Options.AllUsers.Hint", "需要对当前电脑的管理员权限，取消勾选则只为你自己安装。",
                "Requires administrator rights; uncheck to install for yourself only.");
            Add(t, "Options.InfoFormat", "将安装到 {0} · 已选择 {1} 项附加任务",
                "Will install to {0} · {1} additional tasks selected");
            Add(t, "Wizard.StepWelcome", "欢迎", "Welcome");
            Add(t, "License.Subtitle", "请仔细阅读下面的许可协议。安装「{0}」前，你需要接受本协议的全部条款。",
                "Please read the license agreement below. You must accept it before installing {0}.");
            Add(t, "License.AcceptHint", "勾选后才能继续安装；若不同意，可点击「取消」退出安装。",
                "You must accept to continue. Click Cancel to exit without installing.");
            Add(t, "Welcome.CaptionDir", "默认安装位置", "Default location");
            Add(t, "Welcome.CaptionSpace", "需要磁盘空间", "Disk space required");
            Add(t, "Welcome.CaptionVersion", "版本与发布者", "Version & publisher");
            Add(t, "Welcome.Hint", "下一步需要先阅读并同意「许可协议」，点击「下一步」继续。",
                "You will be asked to accept the license agreement on the next step.");
            Add(t, "Step.License", "许可协议", "License");
            Add(t, "Step.Path", "安装位置", "Location");
            Add(t, "Step.Options", "选项", "Options");
            Add(t, "Wizard.StepSettings", "安装设置", "Setup");
            Add(t, "Step.Install", "正在安装", "Installing");

            // 文案照抄设计稿第 3 版（欢迎页），不要自己改写
            Add(t, "Welcome.Title", "欢迎使用「{0}」安装向导", "Welcome to the {0} Setup Wizard");
            Add(t, "Welcome.Body",
                "这个向导将引导你完成「{0} {1}」的安装，整个过程大约需要 1 分钟。",
                "This wizard will install {0} {1}. It takes about a minute.");
            Add(t, "Welcome.Body2",
                "继续之前，建议先关闭其他正在运行的程序，避免安装过程中出现文件占用。",
                "Before continuing, close any running instances to avoid file locks.");
            Add(t, "Welcome.SpaceValue", "{0}（本机可用 {1}）", "{0} ({1} free on this PC)");

            // 文案照抄设计稿第 3 版（许可协议页），不要自己改写
            Add(t, "License.Title", "许可协议", "License Agreement");
            Add(t, "License.CardTitle", "{0} 最终用户许可协议", "{0} End User License Agreement");
            Add(t, "License.Accept", "我已阅读并同意许可协议", "I have read and accept the license agreement");
            Add(t, "License.MustAccept", "请先接受许可协议。", "You must accept the license agreement first.");

            Add(t, "Path.Title", "选择安装位置与附加任务", "Choose location & tasks");
            Add(t, "Path.Label", "安装位置", "Install location");
            Add(t, "Path.Browse", "浏览…", "Browse...");
            Add(t, "Path.Space", "可用空间：{0}，需要约：{1}",
                "Free space: {0}, required: {1}");
            Add(t, "Path.Empty", "安装路径不能为空。", "Install path cannot be empty.");
            Add(t, "Path.NeedsAdmin",
                "这个安装位置需要管理员权限，但 UAC 授权被取消了。\r\n" +
                "可以换一个自己能写的目录（例如「文档」下面），或者再点一次「下一步」重新授权。",
                "This location needs administrator rights, but the UAC prompt was declined.\r\n" +
                "Pick a folder you can write to, or click Next again to retry the prompt.");

            // 「安装设置」页的四张任务卡 —— 逐字对齐设计稿第 3 页，
            // 并且必须和制作端「快捷方式」那一步的用词完全一致。
            //
            // 这里踩过一个坑：Options.Autostart 被写成「安装后自动运行」，但它其实是
            // **开机自启**那张卡的标题 —— 标题和它自己的描述（「登录系统后自动运行」）
            // 自相矛盾，也和制作端的「开机自动启动」对不上。用户反馈的
            // "制作流程和安装过程选项不一致"就是这里。
            //
            // 根因是同一个 key 被两个页面复用，而设计稿两页的措辞本来就不同：
            //   安装设置页：创建桌面快捷方式 / 创建开始菜单快捷方式 / 开机时自动启动 / 安装完成后运行
            //   完成页    ：桌面快捷方式     / 开始菜单             / 安装后自动运行   / 立即运行「{0}」
            // 所以完成页那套单独用 Finish.Chip* / Finish.RunNow，不再和这里共用。
            Add(t, "Options.Title", "附加任务", "Additional tasks");
            Add(t, "Options.Desktop", "创建桌面快捷方式", "Create a desktop shortcut");
            Add(t, "Options.StartMenu", "创建开始菜单快捷方式", "Create a Start Menu shortcut");
            Add(t, "Options.Autostart", "开机时自动启动", "Start with Windows");
            Add(t, "Options.RunAfter", "安装完成后运行", "Run after installation");

            Add(t, "Progress.Title", "正在安装「{0}」", "Installing {0}");
            Add(t, "Progress.Preparing", "正在准备…", "Preparing...");
            Add(t, "Progress.RollingBack", "安装失败，正在撤销更改…", "Installation failed, rolling back...");
            Add(t, "Install.Com", "正在注册组件…", "Registering components...");
            Add(t, "Install.Repair", "正在校验并修复文件…", "Verifying and repairing files...");
            Add(t, "Install.CleanupOrphans", "正在清理残留的卸载项…", "Cleaning up stale uninstall entries...");
            Add(t, "Install.CleanupOldFiles", "正在清理上一版残留文件…", "Removing files from the previous version...");

            Add(t, "Finish.Title", "「{0}」安装完成", "{0} is installed");
            Add(t, "Finish.Body", "已成功安装到 {0}，共写入 {1} 个文件、{2}。", "Installed to {0} ({1} files, {2}).");

            Add(t, "Failed.Title", "安装失败", "Installation failed");
            Add(t, "Failed.Body", "安装未能完成：\r\n{0}\r\n\r\n已回滚到安装前的状态。",
                "Installation could not complete:\r\n{0}\r\n\r\nThe changes have been rolled back.");

            Add(t, "Uninstall.Title", "卸载", "Uninstall");
            Add(t, "Uninstall.Confirm", "将从本机移除 {0}。\r\n\r\n是否继续？",
                "This will remove {0} from your computer.\r\n\r\nContinue?");
            Add(t, "Uninstall.Done", "卸载完成。", "Uninstall complete.");
            Add(t, "Uninstall.Running", "正在卸载…", "Uninstalling...");
            Add(t, "Uninstall.Progress", "正在删除文件与注册表项…", "Removing files and registry entries...");

            // ── 卸载向导「确认卸载」页（设计稿第 3 版，文案照抄设计稿）─────
            // 注意：Uninstall.Title（=「卸载」）同时也是这一页主按钮的文案 key，
            // 不要把它改成页面标题 —— 页面标题用下面的 Uninstall.ConfirmTitle。
            Add(t, "Uninstall.ConfirmTitle", "卸载「{0}」", "Uninstall {0}");
            Add(t, "Uninstall.ConfirmSubtitle",
                "卸载程序将移除「{0} {1}」的程序文件、快捷方式与注册表项。",
                "Uninstall will remove the program files, shortcuts and registry entries of {0} {1}.");
            Add(t, "Uninstall.CaptionDir", "当前安装位置", "Current location");
            Add(t, "Uninstall.CaptionSpace", "占用磁盘空间", "Disk space used");
            Add(t, "Uninstall.CaptionVersion", "安装时间与版本", "Installed on & version");
            Add(t, "Uninstall.SpaceValue", "{0}（卸载后将释放）", "{0} (freed after uninstall)");
            Add(t, "Uninstall.DeleteData", "同时删除用户配置与缓存数据", "Also delete user settings and cache");
            Add(t, "Uninstall.DeleteData.Hint",
                "包括个人设置、登录状态与本地缓存；取消勾选则保留，方便下次安装时继续使用。",
                "Includes personal settings, sign-in state and local cache. Uncheck to keep them for a future install.");
            Add(t, "Uninstall.Warn1",
                "卸载后程序将无法启动，桌面与开始菜单的快捷方式也会一并移除。",
                "The app will no longer start, and its desktop and Start menu shortcuts are removed as well.");
            Add(t, "Uninstall.Warn2", "如需重新使用，请再次运行安装包进行安装。",
                "Run the installer again if you want to use it later.");

            // 卸载向导的 3 步步骤条（确认卸载 / 正在卸载 / 完成）
            Add(t, "Step.UninstallConfirm", "确认卸载", "Confirm");
            Add(t, "Step.Uninstalling", "正在卸载", "Uninstalling");

            return t;
        }

        /// <summary>
        /// 把清单里的 strings 合并到内置表之上（清单优先）。
        /// 返回一个**新**字典，不修改入参。
        /// </summary>
        public static Dictionary<string, LocalizedText> Merge(Dictionary<string, LocalizedText> manifestStrings)
        {
            var merged = CreateDefault();

            if (manifestStrings == null)
            {
                return merged;
            }

            foreach (var kv in manifestStrings)
            {
                if (kv.Value == null || kv.Value.Count == 0)
                {
                    continue;
                }

                LocalizedText existing;
                if (!merged.TryGetValue(kv.Key, out existing))
                {
                    merged[kv.Key] = kv.Value;
                    continue;
                }

                // 逐语言覆盖：清单只给 en 时，zh-Hans 仍用内置的
                foreach (var lang in kv.Value)
                {
                    existing[lang.Key] = lang.Value;
                }
            }

            return merged;
        }

        private static void Add(Dictionary<string, LocalizedText> table, string key, string zh, string en)
        {
            table[key] = new LocalizedText { { "zh-Hans", zh }, { "en", en } };
        }
    }
}
