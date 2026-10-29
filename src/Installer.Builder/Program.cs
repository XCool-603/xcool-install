using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Installer.Abstractions.Model;
using Installer.Abstractions.Packaging;
using Installer.Builder.Common;
using Installer.Builder.Models;
using Installer.Builder.Presenters;
using Installer.Builder.Services;
using Installer.Core.Packaging;

namespace Installer.Builder
{
    /// <summary>制作端入口。</summary>
    internal static class Program
    {
        /// <summary>应用程序的主入口点。</summary>
        [STAThread]
        private static int Main(string[] args)
        {
            // 无头构建：给 CI 用。必须在 Application.EnableVisualStyles 之前分流，
            // 因为这条路径完全不碰界面。
            if (HasFlag(args, "--build"))
            {
                return RunHeadlessBuild(args);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AntdUI.Config.Mode = AntdUI.TMode.Light;

            // 开发/验收用：把界面渲染成 PNG，不写任何东西
            var renderDir = GetOption(args, "--render-ui");
            if (!string.IsNullOrEmpty(renderDir))
            {
                using (var form = new Forms.BuilderForm())
                {
                    form.PrepareForRendering();

                    // 可选：预置一个文件夹，用来验证"启动程序"下拉框等依赖文件夹的界面
                    var demoFolder = GetOption(args, "--with-folder");
                    if (!string.IsNullOrEmpty(demoFolder))
                    {
                        form.PresetSourceFolder(demoFolder);
                    }

                    form.Show();
                    Application.DoEvents();
                    form.DumpBounds(renderDir);
                    form.RenderToPng(renderDir);
                    form.Close();
                }

                Console.WriteLine("已渲染到 " + renderDir);
                return 0;
            }

            // 开发/验收用：跑一遍"产品名跟随"链路并打印可断言的 key=value 行。
            //
            // 为什么要它：这段逻辑住在 BuilderForm 里，靠 --render-ui 只能看到"某个瞬间
            // 的画面"，验证不了"换启动程序 → 产品名跟着变"这条交互链路 ——
            // 而用户反馈的恰恰就是这条链路没生效。有它就能在 CI 里断言。
            // 开发/验收用：打印"生成目录"的解析结果（设置文件 → 默认桌面）。
            // 用户反馈：选了生成目录，重启又回桌面 —— 这条链路得能断言。
            if (HasFlag(args, "--selftest-output"))
            {
                try
                {
                    Console.OutputEncoding = System.Text.Encoding.UTF8;
                }
                catch (System.IO.IOException)
                {
                }

                using (var form = new Forms.BuilderForm())
                {
                    Console.WriteLine(form.SelfTestOutputPath());
                    form.Close();
                }

                return 0;
            }

            var selfTestFolder = GetOption(args, "--selftest-autoname");
            if (!string.IsNullOrEmpty(selfTestFolder))
            {
                try
                {
                    Console.OutputEncoding = System.Text.Encoding.UTF8;
                }
                catch (System.IO.IOException)
                {
                }

                using (var form = new Forms.BuilderForm())
                {
                    form.PrepareForRendering();
                    Console.WriteLine(form.SelfTestAutoName(selfTestFolder));
                    form.Close();
                }

                return 0;
            }

            Application.Run(new Forms.BuilderForm());
            return 0;
        }

        // ─────────────────────────────────────────────────────────────
        // 无头构建
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// 无头构建：把「选文件夹 → 生成安装包」这条主流程跑通，但不弹任何界面。
        ///
        /// 为什么要它：制作端原先只有 --render-ui，只能验证界面渲染，
        /// **验证不了"打包"这条真正的业务链路** —— CI 里没法覆盖。
        /// 有了这条入口，CI 就能断言打包成功并核对产物哈希。
        ///
        /// 用法：
        ///   安装包制作助手.exe --build --project app.wmpkg.json [--source 目录] [--out Setup.exe]
        ///                      [--stub Installer.Runtime.exe] [--icon app.ico]
        ///
        /// 只给 --source 不给 --project 时，按文件夹名自动推导产品名，走默认工程。
        ///
        /// 输出约定（CI 靠它判断，不靠退出码）：
        ///   成功 → BUILD-OK &lt;产物路径&gt; &lt;字节数&gt; &lt;sha256&gt;
        ///   失败 → BUILD-FAIL &lt;原因&gt;
        /// </summary>
        private static int RunHeadlessBuild(string[] args)
        {
            // 无头模式的输出要靠 CI / 脚本解析，中文必须是 UTF-8。
            // WinExe 在没有控制台时设置它会抛 IOException，所以包一层。
            try
            {
                Console.OutputEncoding = System.Text.Encoding.UTF8;
            }
            catch (System.IO.IOException)
            {
            }

            try
            {
                var projectPath = GetOption(args, "--project");
                var sourceDir = GetOption(args, "--source");
                var outPath = GetOption(args, "--out");
                var stubPath = GetOption(args, "--stub");
                var iconPath = GetOption(args, "--icon");

                InstallerProject project;
                if (!string.IsNullOrEmpty(projectPath))
                {
                    if (!File.Exists(projectPath))
                    {
                        return Fail("工程文件不存在：" + projectPath);
                    }

                    project = ProjectSerializer.Load(projectPath);
                }
                else
                {
                    if (string.IsNullOrEmpty(sourceDir))
                    {
                        return Fail("必须给 --project，或者给 --source（按文件夹名推导产品名）");
                    }

                    project = BuilderDocument.CreateDefault();
                    var name = BuilderPresenter.SuggestProductName(sourceDir);
                    project.Manifest.Product.Name = new LocalizedText { { "zh-Hans", name }, { "en", name } };
                    project.Manifest.DefaultInstallDir = @"%LOCALAPPDATA%\Programs\" + BuilderDocument.Sanitize(name);
                }

                if (project.Build == null)
                {
                    project.Build = new ProjectBuildSettings();
                }

                // 命令行覆盖工程里的设置
                if (!string.IsNullOrEmpty(sourceDir)) { project.Build.SourceDir = sourceDir; }
                if (!string.IsNullOrEmpty(outPath)) { project.Build.OutputPath = outPath; }
                if (!string.IsNullOrEmpty(stubPath)) { project.Build.StubPath = stubPath; }
                if (!string.IsNullOrEmpty(iconPath)) { project.Build.IconPath = iconPath; }

                if (string.IsNullOrEmpty(project.Build.SourceDir) || !Directory.Exists(project.Build.SourceDir))
                {
                    return Fail("源目录不存在：" + project.Build.SourceDir);
                }

                var resolvedStub = project.Build.StubPath;
                if (string.IsNullOrEmpty(resolvedStub) || !File.Exists(resolvedStub))
                {
                    resolvedStub = StubLocator.Find();
                }

                if (string.IsNullOrEmpty(resolvedStub) || !File.Exists(resolvedStub))
                {
                    return Fail("找不到 stub —— 用 --stub 指定，或把它放在本程序同目录下");
                }

                var manifest = project.Manifest;
                if (string.IsNullOrWhiteSpace(manifest.EntryPoint))
                {
                    return Fail("工程里没有入口程序（entryPoint），无法生成安装包");
                }

                // 与界面上的「生成安装包」走**同一套**安装目录规则：根\厂商\启动程序名。
                // 不这么做的话，CI 打出来的包和用户点出来的包目录层级不一样。
                BuilderPresenter.ApplyInstallDirLayout(manifest);

                var resolvedOut = project.Build.OutputPath;
                if (string.IsNullOrEmpty(resolvedOut))
                {
                    resolvedOut = BuilderDocument.SuggestOutputPath(
                        PickProductName(manifest), manifest.Product.Version, project.Build.SourceDir);
                }

                var icon = project.Build.IconPath;
                var service = new BuildService();
                var result = service.Build(new PackageBuildOptions
                {
                    Manifest = manifest,
                    SourceDir = project.Build.SourceDir,
                    StubPath = resolvedStub,
                    OutputPath = resolvedOut,
                    IconPath = string.IsNullOrWhiteSpace(icon) ? null : icon,
                    SkipIconPatch = string.IsNullOrWhiteSpace(icon),
                    // 无头构建默认也要求管理员；自动化里可用 --no-require-admin 关掉
                    RequireAdministrator = !HasFlag(args, "--no-require-admin"),
                    PackerVersion = service.PackerVersion,
                }, null);

                Console.WriteLine(string.Format(CultureInfo.InvariantCulture,
                    "BUILD-OK {0} {1} {2}", result.OutputPath, result.OutputBytes, result.OutputSha256));
                return 0;
            }
            catch (Exception ex)
            {
                return Fail(ex.GetType().Name + ": " + ex.Message);
            }
        }

        private static int Fail(string reason)
        {
            Console.WriteLine("BUILD-FAIL " + reason);
            return 1;
        }

        private static string PickProductName(InstallerManifest manifest)
        {
            if (manifest.Product != null && manifest.Product.Name != null)
            {
                // LocalizedText 是 Dictionary<string,string>，没有 Get —— 用 TryGetValue
                string zh;
                if (manifest.Product.Name.TryGetValue("zh-Hans", out zh) && !string.IsNullOrWhiteSpace(zh))
                {
                    return zh;
                }

                string en;
                if (manifest.Product.Name.TryGetValue("en", out en) && !string.IsNullOrWhiteSpace(en))
                {
                    return en;
                }
            }

            return "我的产品";
        }

        private static bool HasFlag(string[] args, string name)
        {
            foreach (var a in args)
            {
                if (string.Equals(a, name, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static string GetOption(string[] args, string name)
        {
            for (var i = 0; i < args.Length - 1; i++)
            {
                if (string.Equals(args[i], name, StringComparison.OrdinalIgnoreCase))
                {
                    return args[i + 1];
                }
            }

            return null;
        }
    }
}
