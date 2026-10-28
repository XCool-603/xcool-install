using System;
using System.IO;
using System.Resources;
using System.Windows.Forms;
using Installation.Properties;
using Microsoft.Win32;

namespace Installation
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 加载资源文件
            ResourceManage.Instance.GetReousrcesInfo();
            // 安装名
            var productName = ResourceManage.Instance.InstallProductName;
            // 注册表
            RegistryKey regFather = Registry.CurrentUser.OpenSubKey("SOFTWARE\\WayMark\\" + productName, false);

            if (regFather != null)
            {
                var installPath = regFather.GetValue("InstallPath", string.Empty).ToString();

                if (!string.IsNullOrEmpty(installPath) && Directory.Exists(installPath))
                {
                    var files = Directory.GetFiles(installPath);
                    if (files.Length > 0)
                    {
                        Application.Run(new FrmUpdate());
                        return;
                    }
                }
                //var proExeName = regFather.GetValue("InstallProductExeName", string.Empty).ToString();

                //if (!string.IsNullOrEmpty(installPath) && !string.IsNullOrEmpty(proExeName))
                //{
                //    var path = Path.Combine(installPath, proExeName);

                //    if (File.Exists(path))
                //    {
                //        Application.Run(new FrmUpdate());
                //        return;
                //    }
                //}
            }

            Application.Run(new FrmInstallation());
        }
    }
}
