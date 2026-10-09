using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;

namespace Installation
{
    internal class ResourceManage
    {
        public static ResourceManage Instance = new ResourceManage();

        #region 安装程序变量
        //安装程序的公司名称
        public string InstallCompanyName = "";
        //安装程序的产品名称
        public string InstallProductName = "";
        //安装程序的执行文件名称
        public string InstallProductExeName = "";
        //安装程序的公司网址
        public string InstallCompanyUrl = "";
        //安装路径
        public string InstallationPath = "";
        //安装程序的版本号
        public string InstallProductVersion = "";
        // 注册文件
        public string[] RegeditList = new string[0];
        // 文件数量
        public int FilesCount = 0;
        // 背景颜色
        public Color BackColor = Color.White;
        // 背景图
        public Image BackgroundImage = null;
        // logo图
        public Image Logo = null;
        // 压缩包
        public byte[] BinZip = null;

        #endregion

        public void GetReousrcesInfo()
        {
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\install.resources"))
            {
                FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\install.resources", FileMode.OpenOrCreate,
                    FileAccess.Write);
                IResourceWriter writer = new ResourceWriter(fs);
                writer.AddResource("InstallCompanyName", InstallCompanyName);
                writer.AddResource("InstallProductName", InstallProductName);
                writer.AddResource("InstallProductExeName", InstallProductExeName);
                writer.AddResource("InstallProductVersion", InstallProductVersion);
                writer.AddResource("InstallCompanyUrl", InstallCompanyUrl);
                writer.AddResource("InstallationPath", InstallationPath);

                writer.AddResource("CkDesktopShortcuts", true);
                writer.AddResource("CkQuickLaunchBar", true);
                writer.AddResource("CkStartUp", true);
                writer.AddResource("CkStartClient", true);
                writer.AddResource("LicenseAgreement", "");
                writer.AddResource("BackColor", this.BackColor);
                writer.AddResource("BackImage", this.BackgroundImage);
                writer.AddResource("Logo", this.Logo);
                writer.AddResource("bin", new byte[0]);
                writer.Close();
                fs.Close();
            }
            IResourceReader read = new ResourceReader(AppDomain.CurrentDomain.BaseDirectory + "\\install.resources");
            var iter = read.GetEnumerator();
            while (iter.MoveNext())
            {
                switch (iter.Key.ToString())
                {
                    case "InstallCompanyName":
                        InstallCompanyName = iter.Value.ToString();
                        break;
                    case "InstallProductName":
                        InstallProductName = iter.Value.ToString();
                        break;
                    case "InstallProductExeName":
                        InstallProductExeName = iter.Value.ToString();
                        break;
                    case "InstallProductVersion":
                        InstallProductVersion = iter.Value.ToString();
                        break;
                    case "InstallCompanyUrl":
                        InstallCompanyUrl = iter.Value.ToString();
                        break;
                    case "InstallFilesCount":
                        FilesCount = int.Parse(iter.Value.ToString());
                        break;
                    case "InstallationPath":
                        InstallationPath = iter.Value.ToString();
                        break;
                    case "LicenseAgreement":
                        Code.LicenseAgreement = iter.Value.ToString();
                        break;
                    case "BackColor":
                        BackColor = (Color)iter.Value;
                        break;
                    case "BackImage":
                        Image backImage = (Image)iter.Value;
                        if (backImage != null)
                        {
                            BackgroundImage = backImage;
                        }
                        break;
                    case "RegditFiles":
                        RegeditList = iter.Value.ToString().Split(';');
                        break;
                    case "Logo":
                        Image logo = (Image)iter.Value;
                        if (logo != null)
                        {
                            Logo = logo;
                        }
                        break;
                    case "bin":
                        BinZip = (byte[])iter.Value;
                        break;
                }
            }
            read.Close();
        }
    }
}
