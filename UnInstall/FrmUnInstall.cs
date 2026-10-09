using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Resources;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using DSkin.DirectUI;
using Microsoft.Win32;
using UnInstall.Properties;

namespace UnInstall
{
    public partial class FrmUnInstall : FrmBase
    { 
        #region 安装程序变量
        //安装程序的公司名称
        public string UnInstallCompanyName = "";
        //安装程序的产品名称
        public string UnInstallProductName = "";
        //安装程序的执行文件名称
        public string UnInstallProductExeName = "";
        //安装程序的公司网址
        public string UnInstallCompanyUrl = "";
        //安装程序的版本号
        public string UnInstallProductVersion = "";
        #endregion

        public FrmUnInstall()
        {
            InitializeComponent();
        }

        private void FrmUnInstall_Load(object sender, EventArgs e)
        {
            GetReousrcesInfo();
            LblMainTitle.Text = string.Format("{0}", UnInstallProductName);
            Text = string.Format("{0}卸载向导", UnInstallProductName);
            PnlInstalling.Top = -PnlInstalling.Height;
            this.CloseBox.MouseClick += (s, ex) =>
            {
                ex.Handled = true;
                if (BtnUnInstall.Text != "卸载完成")
                {
                    if (new FrmMessage("系统提示", "卸载将中断,是否继续退出卸载？", true).ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }
                    CloseForm();
                }
                else
                {
                    try
                    {
                        StreamWriter sw = new StreamWriter(Application.StartupPath + "\\~del.bat", false, Encoding.Default);
                        sw.WriteLine("del /F /S /Q \"" + new DirectoryInfo(Application.StartupPath).FullName + "\"");
                        sw.Close();

                        ProcessStartInfo psi = new ProcessStartInfo(Application.StartupPath + "\\~del.bat");
                        psi.WindowStyle = ProcessWindowStyle.Hidden;
                        Process.Start(psi);
                        Process.GetCurrentProcess().Kill();
                    }
                    catch
                    {
                    }
                }
            };
        }

        public void GetReousrcesInfo()
        {
            if (!File.Exists(Application.StartupPath + "\\install.resources"))
            {
                FileStream fs = new FileStream(Application.StartupPath + "\\install.resources", FileMode.OpenOrCreate,
                    FileAccess.Write);
                IResourceWriter writer = new ResourceWriter(fs);
                writer.AddResource("InstallCompanyName", UnInstallCompanyName);
                writer.AddResource("InstallProductName", UnInstallProductName);
                writer.AddResource("InstallProductExeName", UnInstallProductExeName);
                writer.AddResource("InstallProductVersion", UnInstallProductVersion);
                writer.AddResource("InstallCompanyUrl", UnInstallCompanyUrl);
                writer.AddResource("BackColor", this.BackColor);
                writer.AddResource("BackImage", this.BackgroundImage);
                writer.AddResource("bin", new byte[0]);
                writer.Close();
                fs.Close();
            }
            IResourceReader read = new ResourceReader(Application.StartupPath + "\\install.resources");
            var iter = read.GetEnumerator();
            while (iter.MoveNext())
            {
                switch (iter.Key.ToString())
                {
                    case "InstallCompanyName":
                        UnInstallCompanyName = iter.Value.ToString();
                        break;
                    case "InstallProductName":
                        UnInstallProductName = iter.Value.ToString();
                        break;
                    case "InstallProductExeName":
                        UnInstallProductExeName = iter.Value.ToString();
                        break;
                    case "InstallCompanyUrl":
                        UnInstallCompanyUrl = iter.Value.ToString();
                        break;
                }
            }
            read.Close();
        }
        private void BtnUnInstall_Click(object sender, EventArgs e)
        {
            if (BtnUnInstall.Text == "狠心卸载")
            {
                //this.ShowSystemButtons = false;
                BtnUnInstall.Text = string.Format("正在卸载... ...");
                BtnUnInstall.BaseColor = Color.Silver;
                BtnUnInstall.HoverColor = Color.Silver;
                BtnUnInstall.PressColor = Color.Silver;
                BtnUnInstall.Enabled = false;
                //PnlInstalling.DoEffect(PnlInstalling.Top, 0, 200, "Top", (a) => { });
                PnlAgree.Enabled = false;
                PiStart.Start(); 
                try
                {
                    new Thread(UnInstallThread).Start();
                }
                catch
                {
                    new FrmMessage("系统提示", "安装程序无法写入组件，安装将被中断，请重新运行安装程序。", false).ShowDialog();
                    Process.GetCurrentProcess().Kill();
                }
            }
            else if (BtnUnInstall.Text == "卸载完成")
            {
                try
                {
                    StreamWriter sw = new StreamWriter(Application.StartupPath + "\\~del.bat", false, Encoding.Default);
                    sw.WriteLine("del /F /S /Q \"" + new DirectoryInfo(Application.StartupPath).FullName + "\"");
                    sw.Close();

                    ProcessStartInfo psi = new ProcessStartInfo(Application.StartupPath + "\\~del.bat");
                    psi.WindowStyle = ProcessWindowStyle.Hidden;
                    Process.Start(psi);
                    Process.GetCurrentProcess().Kill();
                }
                catch{}
            }
        }
        private void PnlMoveForm(object sender, MouseEventArgs e)
        {
            DSkin.NativeMethods.MouseToMoveControl(this.Handle);
        } 

        /// <summary>
        /// 安装线程
        /// </summary>
        public void UnInstallThread()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback((o) =>
            {
                int index = 0;
                UnInstall(index);
                this.Invoke(new MethodInvoker(() =>
                {
                    try
                    {
                        BtnUnInstall.Text = "卸载完成";
                        BtnUnInstall.Enabled = true;
                        BtnUnInstall.BaseColor = Color.FromArgb(9, 163, 220);
                        BtnUnInstall.HoverColor = Color.FromArgb(60, 195, 245);
                        BtnUnInstall.PressColor = Color.FromArgb(9, 140, 188);

                        PiStart.Stop();
                        PiStart.DoEffect(PiStart.Top, -PiStart.Height, 200, "Top", (a) => { });
                        LblUnInstatllInfo.DoEffect(LblUnInstatllInfo.Left, PiStart.Left, 200, "Left", (a) => { });
                    }
                    catch{}
                    try
                    {
                        RegistryKey regFather = Registry.CurrentUser.OpenSubKey(
                            "SOFTWARE\\WayMark\\" + UnInstallProductName, true);
                        if (regFather != null)
                        {
                            new FrmMessage("系统提示", "注册表还有残余的信息未能删除。", false).ShowDialog();
                            regFather = Registry.CurrentUser.OpenSubKey(
                            "SOFTWARE\\WayMark" , true);
                            if (regFather != null)
                            {
                                regFather.DeleteSubKey(UnInstallProductName, false);
                                regFather.Close();
                            }
                        }
                        
                    }
                    catch
                    {
                    }
                }));
            }));
        }

        public int UnInstall(int type)
        {
            

            #region 正在卸载注册表信息

            type = type + 1;
            SetProgressMsg(string.Format("正在卸载注册表信息... ..."), type);
            try
            {
                RegistryKey regFather = Registry.CurrentUser.OpenSubKey(
                    "SOFTWARE\\WayMark", true);
                regFather.DeleteSubKey(UnInstallProductName, true);
                regFather.Close();
            }
            catch
            {
            }
            Thread.Sleep(100);

            #endregion

            #region 正在删除应用文件
            type = type + 1;
            SetProgressMsg(string.Format("正在检测应用文件... ..."), type);
            DirectoryInfo diriinfo = new DirectoryInfo(Application.StartupPath+"\\");
            DirectoryInfo[] folders = diriinfo.GetDirectories();
            FileInfo[] files = diriinfo.GetFiles();
            foreach (DirectoryInfo folderinfo in folders)
            {
                try
                {
                    type = type + 1;
                    SetProgressMsg(string.Format("正在删除{0}配置文件夹... ...", folderinfo.Name), type);
                    Directory.Delete(folderinfo.FullName,true);
                    Thread.Sleep(100);
                }
                catch {}
            }

            foreach (FileInfo file in files)
            {
                try
                {
                    type = type + 1;
                    SetProgressMsg(string.Format("正在删除{0}文件... ...", file.Name), type);
                    File.Delete(file.FullName); 
                    Thread.Sleep(100);
                }
                catch{}
            }
            Thread.Sleep(100);
            #endregion

            try
            {
                #region 正在删除桌面快捷方式

                type = type + 1;
                SetProgressMsg(string.Format("正在删除桌面快捷方式... ..."), type);
                File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + UnInstallProductName +
                            ".lnk");
                Thread.Sleep(100);

                #endregion

                #region 正在删除快速启动栏快捷方式

                type = type + 1;
                SetProgressMsg(string.Format("正在删除快速启动栏快捷方式... ..."), type);
                if (
                    Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + "\\" +
                                     Application.ProductName))
                {
                    Directory.Delete(
                        Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + "\\" + UnInstallProductName,
                        true);
                }
                Thread.Sleep(100);

                #endregion
            }
            catch{}

            #region 卸载成功
            type = type + 1;
            SetProgressMsg(string.Format("已成功卸载！",UnInstallProductName), type); 
            Thread.Sleep(100);
            #endregion
            return type;
        }

        /// <summary>
        /// 线程控制UI显示进度信息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="progressvalue"></param>
        public void SetProgressMsg(string msg, int progressvalue)
        {
            this.Invoke(new MethodInvoker(delegate
            {
                LblUnInstatllInfo.Text = msg;
            }));
        }

        private void BtnContactUs_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(UnInstallCompanyUrl); 
        }
    }
}
