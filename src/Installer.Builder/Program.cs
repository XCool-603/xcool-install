using System;
using System.IO;
using System.Windows.Forms;

namespace Installer.Builder
{
    /// <summary>制作端入口。</summary>
    internal static class Program
    {
        /// <summary>应用程序的主入口点。</summary>
        [STAThread]
        private static int Main(string[] args)
        {
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
                    form.RenderToPng(renderDir);
                    form.Close();
                }

                Console.WriteLine("已渲染到 " + renderDir);
                return 0;
            }

            Application.Run(new Forms.BuilderForm());
            return 0;
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
