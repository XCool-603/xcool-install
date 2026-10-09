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
            Add(t, "Wizard.Finish", "完成", "Finish");
            Add(t, "Wizard.Close", "关闭", "Close");
            Add(t, "Wizard.Retry", "重试", "Retry");

            Add(t, "Step.License", "许可协议", "License");
            Add(t, "Step.Path", "安装位置", "Location");
            Add(t, "Step.Options", "选项", "Options");
            Add(t, "Step.Install", "安装", "Install");

            Add(t, "Welcome.Title", "欢迎使用", "Welcome");
            Add(t, "Welcome.Body",
                "这个向导将引导你完成安装。\r\n\r\n点击「下一步」继续。",
                "This wizard will guide you through the installation.\r\n\r\nClick Next to continue.");

            Add(t, "License.Title", "许可协议", "License Agreement");
            Add(t, "License.Accept", "我已阅读并接受许可协议", "I accept the terms of the license agreement");
            Add(t, "License.MustAccept", "请先接受许可协议。", "You must accept the license agreement first.");

            Add(t, "Path.Title", "选择安装位置", "Choose install location");
            Add(t, "Path.Label", "安装到：", "Install to:");
            Add(t, "Path.Browse", "浏览…", "Browse...");
            Add(t, "Path.Space", "可用空间：{0}，需要约：{1}",
                "Free space: {0}, required: {1}");
            Add(t, "Path.Empty", "安装路径不能为空。", "Install path cannot be empty.");

            Add(t, "Options.Title", "安装选项", "Options");
            Add(t, "Options.Desktop", "创建桌面快捷方式", "Create a desktop shortcut");
            Add(t, "Options.StartMenu", "创建开始菜单快捷方式", "Create a Start Menu shortcut");
            Add(t, "Options.Autostart", "开机自动启动", "Start with Windows");
            Add(t, "Options.RunAfter", "安装完成后立即运行", "Run after installation");

            Add(t, "Progress.Title", "正在安装", "Installing");
            Add(t, "Progress.Preparing", "正在准备…", "Preparing...");
            Add(t, "Progress.RollingBack", "安装失败，正在撤销更改…", "Installation failed, rolling back...");
            Add(t, "Install.Com", "正在注册组件…", "Registering components...");
            Add(t, "Install.Repair", "正在校验并修复文件…", "Verifying and repairing files...");
            Add(t, "Install.CleanupOrphans", "正在清理残留的卸载项…", "Cleaning up stale uninstall entries...");

            Add(t, "Finish.Title", "安装完成", "Installation complete");
            Add(t, "Finish.Body", "{0} 已安装到：\r\n{1}", "{0} has been installed to:\r\n{1}");

            Add(t, "Failed.Title", "安装失败", "Installation failed");
            Add(t, "Failed.Body", "安装未能完成：\r\n{0}\r\n\r\n已回滚到安装前的状态。",
                "Installation could not complete:\r\n{0}\r\n\r\nThe changes have been rolled back.");

            Add(t, "Uninstall.Title", "卸载", "Uninstall");
            Add(t, "Uninstall.Confirm", "将从本机移除 {0}。\r\n\r\n是否继续？",
                "This will remove {0} from your computer.\r\n\r\nContinue?");
            Add(t, "Uninstall.Done", "卸载完成。", "Uninstall complete.");
            Add(t, "Uninstall.Running", "正在卸载…", "Uninstalling...");
            Add(t, "Uninstall.Progress", "正在删除文件与注册表项…", "Removing files and registry entries...");

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
