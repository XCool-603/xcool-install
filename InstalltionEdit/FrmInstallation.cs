using System;
using System.Collections;
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
using CSharpZip.Zip;
using DSkin.DirectUI;
using InstalltionEdit;
using InstalltionEdit.Properties;
using Microsoft.Win32;

namespace InstalltionEdit
{
    public partial class FrmInstallation : FrmBase
    {
        #region 变量
        /// <summary>
        /// 安装路径选择对话框
        /// </summary>
        private FolderBrowserDialog _fbd = new FolderBrowserDialog();

        private int filesCount = 0;
        #endregion

        public byte[] binZip =new byte[0];
        #region 安装程序变量
        //安装程序的公司名称
        public string InstallCompanyName = "";
        //安装程序的产品名称
        public string InstallProductName = "";
        //安装程序的执行文件名称
        public string InstallProductExeName = "";
        //安装程序的公司网址
        public string InstallCompanyUrl = "";
        //安装程序的版本号
        public string InstallProductVersion = "";
        /// <summary>
        /// 安装程序的路径
        /// </summary>
        public string InstallPath {
            get { return TxtInstalltionEditPath.Text; }
            set { TxtInstalltionEditPath.Text = value; }
        }

        #endregion

        #region 构造函数
        public FrmInstallation()
        {
            InitializeComponent();
            this.Height = 390;
            TxtInstalltionEditPath.Text = "";
            GetReousrcesInfo();
            CkCustomOptions.Checked = true;
            _fbd.RootFolder = System.Environment.SpecialFolder.Desktop;
            _fbd.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            _fbd.ShowNewFolderButton = true;
            if (TxtInstalltionEditPath.Text.Trim().Equals(""))
            {
                TxtInstalltionEditPath.Text = string.Format("{0}\\{1}\\{2}", _fbd.SelectedPath, InstallCompanyName, InstallProductName);
            }
            
            LblMainTitle.Text = string.Format("{0}", InstallProductName);

            Text = string.Format("{0}安装向导", InstallProductName);
            PnlInstalling.Top = -PnlInstalling.Height;
            //lblCompanyName.Text = Application.CompanyName;
            this.CloseBox.MouseClick += (s, e) =>
            {
                e.Handled = true;
                if (!BtnInstall.Text.Equals("安装完成"))
                {
                    if (new FrmMessage("系统提示", "你确定要取消安装并退出安装向导？", true) { BackColor = this.BackColor }.ShowDialog() == DialogResult.OK)
                    {
                        IsAccordClose = true;
                        CloseForm();
                    }
                }
                else
                {
                    CloseForm();
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
                writer.AddResource("InstallCompanyName", InstallCompanyName);
                writer.AddResource("InstallProductName", InstallProductName);
                writer.AddResource("InstallProductExeName", InstallProductExeName);
                writer.AddResource("InstallProductVersion", InstallProductVersion);
                writer.AddResource("InstallCompanyUrl", InstallCompanyUrl);
                writer.AddResource("InstallationPath", TxtInstalltionEditPath.Text);

                writer.AddResource("CkDesktopShortcuts", true);
                writer.AddResource("CkQuickLaunchBar", true);
                writer.AddResource("CkStartUp", true);
                writer.AddResource("CkStartClient", true);
                writer.AddResource("LicenseAgreement", "");
                writer.AddResource("BackColor", this.BackColor);
                writer.AddResource("BackImage", this.BackgroundImage);
                writer.AddResource("Logo",PicLogo.BackgroundImage);
                writer.AddResource("bin", new object());
                writer.Close();
                fs.Close();
            }
            IResourceReader read = new ResourceReader(Application.StartupPath + "\\install.resources");
            var iter = read.GetEnumerator();
            while (iter.MoveNext())
            {
                try
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
                            filesCount = int.Parse(iter.Value.ToString());
                            break;
                        case "InstallationPath":
                            TxtInstalltionEditPath.Text = iter.Value.ToString();
                            break;
                        case "CkDesktopShortcuts":
                            CkDesktopShortcuts.Checked = (bool)iter.Value;
                            break;
                        case "CkQuickLaunchBar":
                            CkQuickLaunchBar.Checked = (bool)iter.Value;
                            break;
                        case "LicenseAgreement":
                            Code.LicenseAgreement = iter.Value.ToString();
                            break;
                        case "CkStartUp":
                            CkStartUp.Checked = (bool)iter.Value;
                            break;
                        case "CkStartClient":
                            CkStartClient.Checked = (bool)iter.Value;
                            break;
                        case "BackColor":
                            this.BackColor = (Color)iter.Value;
                            BtnInstall.BaseColor = Color.FromArgb(200, BackColor);
                            BtnInstall.HoverColor = Color.FromArgb(255, BackColor);
                            BtnInstall.PressColor = Color.FromArgb(215, BackColor);
                            break;
                        case "BackImage":
                            Bitmap backImage = (Bitmap)iter.Value;
                            if (backImage != null)
                            {
                                BackgroundImage = backImage;
                            }
                            break;
                        case "Logo":
                            Bitmap logo = (Bitmap)iter.Value;
                            PicLogo.BackgroundImage = logo;
                            break;
                        case "bin":
                            binZip = (byte[])iter.Value;
                            break;
                    }
                }
                catch
                {
                }
            }
            read.Close();
        }

        #endregion

        #region 事件
        #region 安装路径文本框边框显示效果
        private void PnlPathEnter(object sender, EventArgs e)
        {
            PnlPath.Borders.AllColor = Color.FromArgb(100, Color.Black);
        }

        private void PnlPathLeave(object sender, EventArgs e)
        {
            PnlPath.Borders.AllColor = Color.Transparent;
        }

        private void PnlMoveForm(object sender, MouseEventArgs e)
        {
            DSkin.NativeMethods.MouseToMoveControl(this.Handle);
        } 
        #endregion

        /// <summary>
        /// 窗体显示后检测之前的安装信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmInstallation_Shown(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                Thread.Sleep(1000);
                this.Invoke(new MethodInvoker(delegate
                {
                    CheckInstall();
                }));
            }).Start();
        }
        /// <summary>
        /// 浏览安装目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            if (_fbd.ShowDialog() == DialogResult.OK)
            {
                TxtInstalltionEditPath.Text = string.Format("{0}\\{1}\\{2}", _fbd.SelectedPath, InstallCompanyName, InstallProductName);
            }
        }
        /// <summary>
        /// 同意协议勾选事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CkAgreeCheckedChanged(object sender, EventArgs e)
        {
            BtnInstall.Enabled = CkAgree.Checked;
            BtnInstall.BaseColor = CkAgree.Checked ? Color.FromArgb(9, 163, 220) : Color.Silver;
        }
        /// <summary>
        /// 开始安装和安装完成按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnInstallClick(object sender, EventArgs e)
        {
            if (BtnInstall.Text.Equals("立即安装"))
            {
                BtnInstall.Text = string.Format("正在安装... ...");
                BtnInstall.BaseColor = Color.Silver;
                BtnInstall.HoverColor = Color.Silver;
                BtnInstall.PressColor = Color.Silver;
                BtnInstall.Enabled = false;

                PnlInstalling.DoEffect(PnlInstalling.Top, 0, 200, "Top", (a) => { });
                CkCustomOptions.Checked = true;
                PnlAgree.DoEffect(PnlAgree.Left, -PnlAgree.Width, 400, "Left", (a) => { });
                PnlCustom.DoEffect(PnlCustom.Left, this.Width + PnlCustom.Width, 400, "Left", (a) => { });
                PiStart.Start();

                InstallProgressBar.Maximum = 11+filesCount;
                InstallProgressBar.Value = 0;
                try
                {
                    new Thread(InstallThread).Start();
                }
                catch
                {
                    new FrmMessage("系统提示", "安装程序无法写入组件，安装将被中断，请重新运行安装程序。", false){BackColor = this.BackColor}.ShowDialog();
                    Process.GetCurrentProcess().Kill();
                }
            }
            else if (BtnInstall.Text.Equals("安装完成"))
            {
                if (CkStartClient.Checked)
                {
                    Process.Start(TxtInstalltionEditPath.Text + "\\bin\\" + InstallProductExeName);
                }
                this.Close();
                //BtnInstall.Text = string.Format("立即安装");
                //PiStart.Stop();
                //BtnInstall.BaseColor = Color.FromArgb(9, 163, 220);
                //BtnInstall.HoverColor = Color.FromArgb(60, 195, 245);
                //BtnInstall.PressColor = Color.FromArgb(9, 140, 188);
                //PnlInstalling.DoEffect(PnlInstalling.Top, -PnlInstalling.Height, 200, "Top", (a) => { });
                //CkCustomOptions.Checked = true;
                //PnlAgree.DoEffect(PnlAgree.Left, 0, 400, "Left", (a) => { });
                //PnlCustom.DoEffect(PnlCustom.Left, this.Width - PnlCustom.Width, 400, "Left", (a) => { });
            }
            else
            {
            }

        } 
        #endregion
     
        #region 方法
        #region 自定义选项收起展开动画
        private void dSkinCheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (CkCustomOptions.Checked)
            {
                HideCustom();
            }
            else
            {
                ShowCustom();
            }
        }
        /// <summary>
        /// 收起动画
        /// </summary>
        public void HideCustom()
        {
            float op = 1.0f;
            PnlCustomOptions.DoEffect(() =>
            {
                PnlCustomOptions.Enabled = false;
                if (op > 0)
                {
                    op -= 0.05f;
                    PnlCustomOptions.ImageAttribute = DSkin.ImageEffects.ChangeOpacity(op);
                    return true;
                }
                PnlCustomOptions.ImageAttribute = DSkin.ImageEffects.ChangeOpacity(0f);

                return false;
            });
            this.DoEffect(() =>
            {
                if (this.Height > 390)
                {
                    this.Height -= (this.Height - 390 + 10) / 10;
                    return true;
                }
                this.Height = 390;
                return false;
            });
        }
        /// <summary>
        /// 展开动画
        /// </summary>
        public void ShowCustom()
        {
            this.DoEffect(() =>
            {
                if (this.Height < 500)
                {
                    this.Height += (500 - this.Height + 10) / 10;
                    return true;
                }
                this.Height = 500;

                return false;
            });
            float op = 0f;
            PnlCustomOptions.DoEffect(() =>
            {
                if (op < 1f)
                {
                    op += 0.05f;
                    PnlCustomOptions.ImageAttribute = DSkin.ImageEffects.ChangeOpacity(op);
                    return true;
                }
                PnlCustomOptions.ImageAttribute = DSkin.ImageEffects.ChangeOpacity(1f);

                PnlCustomOptions.Enabled = true;
                return false;
            });
        } 
        #endregion

        #region 检查安装

        public void CheckInstall()
        {
            try
            {
                
                RegistryKey regFather = Registry.CurrentUser.OpenSubKey("SOFTWARE\\WayMark\\" + InstallProductName,
                    false);
                if (regFather != null)
                {
                    
                    string installpath=regFather.GetValue("InstallPath", "").ToString();
                    //如果安装文件夹已经为空，则卸载完成，只是注册表信息未删除而已
                    if (GetFileNum(installpath) == 0)
                    {
                        return;
                    }
                    string str = regFather.GetValue("DisplayVersion", "").ToString();
                    if (str.Length == 0)
                    {
                        //注册表信息不完整，无法验证版本，强制安装
                    }
                    else if (Int32.Parse(str.Replace(".", "")) == Int32.Parse(InstallProductVersion.Replace(".", "")))
                    {
                        if (new FrmMessage("系统提示", "当前计算机中已安装了本产品的相同版本，你确定以修复方式重新安装？", true) { BackColor = this.BackColor }.ShowDialog() ==
                            DialogResult.Cancel)
                        {
                            CloseForm();
                        }
                        _fbd.SelectedPath = InstallPath;
                        InstallPath = regFather.GetValue("InstallPath", "").ToString();
                    }
                    else if (Int32.Parse(str.Replace(".", "")) > Int32.Parse(InstallProductVersion.Replace(".", "")))
                    {
                        if (new FrmMessage("系统提示", "当前计算机中安装的产品版本高于当前安装程序的版本，你确定要使用旧版本替换新版本吗？", true) { BackColor = this.BackColor }.ShowDialog() ==
                            DialogResult.Cancel)
                        {
                            CloseForm();
                        }
                        InstallPath = regFather.GetValue("InstallPath", "").ToString();
                        _fbd.SelectedPath = InstallPath;
                    }
                }
            }
            catch
            {

                if (new FrmMessage("系统提示", "软件文件被破坏，接下来将以修复方式重新安装！", true) { BackColor = this.BackColor }.ShowDialog() == DialogResult.Cancel)
                {
                    CloseForm();
                }
                _fbd.SelectedPath = InstallPath;
            }
        }

        #endregion

        /// <summary>
        /// 获取某目录下的所有文件(包括子目录下文件)的数量
        /// </summary>
        /// <param name="srcPath"></param>
        /// <returns></returns>
        public int GetFileNum(string srcPath)
        {
            int fileNum = 0;
            try
            {
                string[] fileList = System.IO.Directory.GetFileSystemEntries(srcPath);
                foreach (string file in fileList)
                {
                    if (System.IO.Directory.Exists(file))
                        GetFileNum(file);
                    else
                        fileNum++;
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            return fileNum;
        }
        /// <summary>
        /// 安装线程
        /// </summary>
        public void InstallThread()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback((o) =>
            {
                int index = 0;
                Install(index);
                this.Invoke(new MethodInvoker(() =>
                {
                    BtnInstall.Text = "安装完成";
                    BtnInstall.Enabled = true;
                    BtnInstall.BaseColor = Color.FromArgb(9, 163, 220);
                    BtnInstall.HoverColor = Color.FromArgb(60, 195, 245);
                    BtnInstall.PressColor = Color.FromArgb(9, 140, 188);
                    PiStart.Stop();
                    PnlInstalling.DoEffect(PnlInstalling.Top, -PnlInstalling.Height, 200, "Top", (a) => { });
                }));
            }));
        }

        /// <summary>
        /// 安装方法
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public int Install(int type)
        {

            #region 正在检测相关程序是否在运
            type = type + 1;
            SetProgressMsg("正在检测相关程序是否在运... ...", type);
            Thread.Sleep(100);
            #endregion

            #region 正在检测是否安装有旧版本
            type = type + 1;
            SetProgressMsg("正在检测是否安装有旧版本... ...", type);
            Thread.Sleep(100);
            #endregion

            #region 创建临时文件
            type = type + 1;
            SetProgressMsg("创建临时文件... ...", type);
            Thread.Sleep(100);
            #endregion

            #region 正在解压文件
            SetProgressMsg("正在解压文件... ...", type);
            for (int i = 0; i < filesCount; i++)
            {
                SetProgressMsg(string.Format("正在提取文件：{0}", i), type);
                type += 1;
                Thread.Sleep(100);
            }
            Thread.Sleep(100);
            #endregion

            #region 正在创建卸载文件
            type = type + 1;
            SetProgressMsg("正在创建卸载文件... ...", type);
            Thread.Sleep(100);
            
            #endregion

            #region 正在创建卸载服务文件
            type = type + 1;
            SetProgressMsg("正在创建卸载服务文件... ...", type);
            Thread.Sleep(100);
            #endregion

            #region 正在删除临时文件
            type = type + 1;
            SetProgressMsg("正在删除临时文件... ...", type);
            Thread.Sleep(100);
            #endregion

            #region 注册程序
            type = type + 1;
            SetProgressMsg("正在向系统注册程序... ...", type);
            Thread.Sleep(100);
            #endregion

            #region 正在创建桌面快捷方式
            type = type + 1;
            if (CkDesktopShortcuts.Checked)
            {
                SetProgressMsg("正在创建桌面快捷方式... ...", type);
            }
            Thread.Sleep(100);
            #endregion

            #region 正在创建开始菜单快捷方式
            type = type + 1;
            if (CkQuickLaunchBar.Checked)
            {
                SetProgressMsg("正在创建开始菜单快捷方式... ...", type);
            }
            Thread.Sleep(100);
            #endregion

            #region 正在注册开机启动
            type = type + 1;
            if (CkStartUp.Checked)
            {
                SetProgressMsg("正在注册开机启动... ...", type);
            }
            Thread.Sleep(100);
            #endregion
            #region 正在释放的临时文件
            type = type + 1;
            SetProgressMsg("正在清理安装时释放的临时文件... ...", type);
            Thread.Sleep(100);
            #endregion

            type = type + 1;
            return type;
        }
        /// <summary>
        /// 关闭正在执行的程序
        /// </summary>
        public void KillPro()
        {
            Process[] proArr = Process.GetProcessesByName(InstallProductName);
            for (int i = 0; i < proArr.Length; i++)
            {
                proArr[i].Kill();
            }
        }
        /// <summary>
        /// 解压安装包
        /// </summary>
        /// <param name="zipFilePath"></param>
        /// <param name="unZipDir"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public int GetFiles(string zipFilePath, string unZipDir, int type)
        {
            if (unZipDir == string.Empty)
                unZipDir = zipFilePath.Replace(Path.GetFileName(zipFilePath),
                    Path.GetFileNameWithoutExtension(zipFilePath));
            if (!unZipDir.EndsWith("\\"))
                unZipDir += "\\";
            if (!Directory.Exists(unZipDir))
                Directory.CreateDirectory(unZipDir);
            try
            {
                using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFilePath)))
                {

                    ZipEntry theEntry;
                    while ((theEntry = s.GetNextEntry()) != null)
                    {
                        string directoryName = Path.GetDirectoryName(theEntry.Name);
                        string fileName = Path.GetFileName(theEntry.Name);
                        if (directoryName.Length > 0)
                        {
                            Directory.CreateDirectory(unZipDir + directoryName);
                        }
                        if (!directoryName.EndsWith("\\"))
                            directoryName += "\\";
                        if (fileName != String.Empty)
                        {
                            SetProgressMsg(string.Format("正在提取文件：{0}", theEntry.Name), type);
                            type += 1;
                            Thread.Sleep(100);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            type += 1;
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
                LblInstatllInfo.Text = msg;
                InstallProgressBar.Value = progressvalue;
            }));
        } 
        #endregion

        private void LblAgreement_Click(object sender, EventArgs e)
        {
            //FrmLicenseAgreement frmLicenseAgreement=new FrmLicenseAgreement();
            //frmLicenseAgreement.ShowDialog();
        }

        
    }
}
