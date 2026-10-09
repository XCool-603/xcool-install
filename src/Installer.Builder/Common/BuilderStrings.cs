using System;
using System.Collections.Generic;
using Installer.Abstractions.Model;

namespace Installer.Builder.Common
{
    /// <summary>制作端界面的内置中英文案。</summary>
    public static class BuilderStrings
    {
        /// <summary>内置文案表。</summary>
        public static Dictionary<string, LocalizedText> CreateDefault()
        {
            var t = new Dictionary<string, LocalizedText>(StringComparer.Ordinal);

            Add(t, "App.Title", "安装包制作助手", "Installer Builder");

            // 三步
            Add(t, "Step.Folder", "第 1 步 · 选择要打包的文件夹", "Step 1 - Choose the folder to package");
            Add(t, "Step.Folder.Hint", "把文件夹拖到这里，或者点右边的按钮",
                "Drop the folder here, or use the button on the right");
            Add(t, "Step.Folder.Pick", "选择文件夹", "Choose folder");
            Add(t, "Step.Folder.Change", "更换文件夹", "Change folder");
            Add(t, "Step.Info", "第 2 步 · 填写基本信息", "Step 2 - Basic information");
            Add(t, "Step.Shortcuts", "第 3 步 · 快捷方式", "Step 3 - Shortcuts");

            Add(t, "Field.Name", "产品名称", "Product name");
            Add(t, "Field.NameEn", "英文名称", "English name");
            Add(t, "Field.Version", "版本号", "Version");
            Add(t, "Field.Publisher", "厂商", "Publisher");
            Add(t, "Field.EntryPoint", "启动程序", "Start program");
            Add(t, "Field.EntryPoint.None", "（这个文件夹里没有 .exe）", "(no .exe found in this folder)");
            Add(t, "Field.EntryPoint.Multiple", "发现 {0} 个可执行文件，请选要启动的那个",
                "{0} executables found - pick the one to launch");
            Add(t, "Field.NameEn.Hint", "可留空（英文系统会显示中文名）", "Optional");

            Add(t, "Shortcut.Desktop", "创建桌面快捷方式", "Desktop shortcut");
            Add(t, "Shortcut.StartMenu", "创建开始菜单快捷方式", "Start Menu shortcut");
            Add(t, "Shortcut.Autostart", "开机自动启动", "Start with Windows");
            Add(t, "Shortcut.RunAfter", "安装完成后立即运行", "Run after install");

            Add(t, "Files.None", "还没有选择文件夹", "No folder selected yet");
            Add(t, "Files.Count", "共 {0} 个文件，{1}", "{0} files, {1}");
            Add(t, "Files.Empty", "这个文件夹是空的", "This folder is empty");

            Add(t, "Advanced.Open", "高级选项…", "Advanced...");
            Add(t, "Advanced.Title", "高级选项", "Advanced options");
            Add(t, "Tab.Install", "安装细节", "Install details");
            Add(t, "Advanced.Com", "COM 组件", "COM components");
            Add(t, "Advanced.License", "许可协议", "License");
            Add(t, "Advanced.LicenseRequired", "必须接受才能安装", "Require acceptance");
            Add(t, "Advanced.LicenseZh", "中文", "Chinese");
            Add(t, "Advanced.LicenseEn", "英文", "English");
            Add(t, "Advanced.Accent", "主题色", "Accent color");
            Add(t, "Advanced.Theme", "主题", "Theme");
            Add(t, "Advanced.ThemeLight", "浅色", "Light");
            Add(t, "Advanced.ThemeDark", "深色", "Dark");
            Add(t, "Advanced.Icon", "安装包图标", "Installer icon");
            Add(t, "Advanced.EntryPoint", "启动程序", "Entry point");
            Add(t, "Advanced.EntryPoint.Hint", "相对打包文件夹，例如 bin\\App.exe",
                "Relative to the packaged folder, e.g. bin\\App.exe");
            Add(t, "Advanced.Scope", "安装范围", "Scope");
            Add(t, "Advanced.ScopePerUser", "仅当前用户（不需要管理员）", "Current user only (no admin)");
            Add(t, "Advanced.ScopePerMachine", "所有用户（需要管理员）", "All users (requires admin)");
            Add(t, "Advanced.InstallDir", "默认安装目录", "Default install folder");
            Add(t, "Advanced.Com", "需要注册的 COM 组件", "COM components to register");
            Add(t, "Advanced.Com.Hint", "填相对打包文件夹的路径，一行一个",
                "Paths relative to the packaged folder, one per line");
            Add(t, "Advanced.Output", "固定输出路径（留空则自动）", "Fixed output path (blank = automatic)");
            Add(t, "Advanced.Timestamp", "固定构建时间戳（留空则用当前时间）",
                "Fixed build timestamp (blank = now)");
            Add(t, "Advanced.Ok", "确定", "OK");
            Add(t, "Advanced.Cancel", "取消", "Cancel");

            Add(t, "Output.Label", "将生成：", "Will create:");
            Add(t, "Output.Change", "更换位置…", "Change...");
            Add(t, "Output.Empty", "（选好文件夹后自动确定）", "(determined once a folder is chosen)");

            Add(t, "Btn.Preview", "预览向导", "Preview");
            Add(t, "Btn.Build", "生成安装包", "Build installer");
            Add(t, "Btn.Building", "正在生成…", "Building...");

            Add(t, "Status.Ready", "选一个文件夹就可以开始了", "Pick a folder to begin");
            Add(t, "Status.Building", "正在生成…", "Building...");
            Add(t, "Status.Done", "完成：{0}（{1}）", "Done: {0} ({1})");
            Add(t, "Status.Failed", "失败：{0}", "Failed: {0}");

            Add(t, "Msg.Done.Title", "生成成功", "Build complete");
            Add(t, "Msg.Done.Body", "安装包已生成：\r\n\r\n{0}\r\n\r\n大小 {1}。\r\n\r\n要打开所在文件夹吗？",
                "Installer created:\r\n\r\n{0}\r\n\r\nSize {1}.\r\n\r\nOpen the containing folder?");
            Add(t, "Msg.Fix.Title", "还差一点", "Almost there");
            Add(t, "Msg.Error.Title", "出错了", "Something went wrong");

            Add(t, "Filter.Icon", "图标 (*.ico)|*.ico|所有文件 (*.*)|*.*",
                "Icon (*.ico)|*.ico|All files (*.*)|*.*");
            Add(t, "Filter.Exe", "可执行文件 (*.exe)|*.exe|所有文件 (*.*)|*.*",
                "Executable (*.exe)|*.exe|All files (*.*)|*.*");

            return t;
        }

        private static void Add(Dictionary<string, LocalizedText> table, string key, string zh, string en)
        {
            table[key] = new LocalizedText { { "zh-Hans", zh }, { "en", en } };
        }
    }
}
