using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using CSharpZip.Zip;
using DSkin.DirectUI;
using Installation.Properties;
using Microsoft.Win32;

namespace Installation
{
    public partial class FrmInstallation : FrmBase
    {
        #region 函数申明
        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWow64Process([In] IntPtr hProcess, [Out] out bool lpSystemInfo);
        [DllImport("DllTest.dll")]

        public static extern int DllRegisterServer();//注册时用

        [DllImport("DllTest.dll")]

        public static extern int DllUnregisterServer();//取消注册时用
        #endregion

        #region 变量
        /// <summary>
        /// 安装路径选择对话框
        /// </summary>
        private FolderBrowserDialog _fbd = new FolderBrowserDialog();

        private int filesCount = 0;
        #endregion

        #region 安装程序变量
        /// <summary>
        /// 安装程序的路径
        /// </summary>
        public string InstallPath
        {
            get { return TxtInstallationPath.Text; }
            set { TxtInstallationPath.Text = value; }
        }
        #endregion

        #region 构造函数
        public FrmInstallation()
        {
            InitializeComponent();
            this.Height = 390;

            TxtInstallationPath.Text = "";

            CkCustomOptions.Checked = true;
            _fbd.RootFolder = System.Environment.SpecialFolder.Desktop;
            _fbd.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            _fbd.ShowNewFolderButton = true;
            if (TxtInstallationPath.Text.Trim().Equals(""))
            {
                
            }

            PnlInstalling.Top = -PnlInstalling.Height;
            //lblCompanyName.Text = Application.CompanyName;
            this.CloseBox.MouseClick += (s, e) =>
            {
                e.Handled = true;
                if (!BtnInstall.Text.Equals("安装完成"))
                {
                    if (new FrmMessage("系统提示", "你确定要取消安装并退出安装向导？", true, this.BackColor).ShowDialog() == DialogResult.OK)
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

        private void FrmInstallation_Load(object sender, EventArgs e)
        {
            FormBackColor = ResourceManage.Instance.BackColor;
            this.BackColor = FormBackColor;
            this.BackgroundImage = ResourceManage.Instance.BackgroundImage;
            this.PicLogo.BackgroundImage = ResourceManage.Instance.Logo;
            this.BtnInstall.BaseColor = Color.FromArgb(200, FormBackColor);
            this.BtnInstall.HoverColor = Color.FromArgb(255, FormBackColor);
            this.BtnInstall.PressColor = Color.FromArgb(215, FormBackColor);

            this.LblMainTitle.Text = string.Format("{0}", ResourceManage.Instance.InstallProductName);
            this.Text = string.Format("{0}安装向导", ResourceManage.Instance.InstallProductName);

            this.TxtInstallationPath.Text = string.Format("{0}\\{1}\\{2}", _fbd.SelectedPath, ResourceManage.Instance.InstallCompanyName, ResourceManage.Instance.InstallProductName);
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
                TxtInstallationPath.Text = string.Format("{0}\\{1}\\{2}", _fbd.SelectedPath, ResourceManage.Instance.InstallCompanyName, ResourceManage.Instance.InstallProductName);
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
                try
                {
                    DirectoryInfo di = new DirectoryInfo(TxtInstallationPath.Text);
                    TxtInstallationPath.Text = di.FullName.Substring(0, di.FullName.LastIndexOf(di.Name) + di.Name.Length);
                    Directory.CreateDirectory(TxtInstallationPath.Text);
                }
                catch
                {
                    new FrmMessage("系统提示", "无法将文件安装到你选定的目录，请检查路径或者是否拥有写权限。", false, BackColor).ShowDialog();
                    return;
                }
                //if (Directory.Exists(TxtInstallationPath.Text))
                //{
                //    if (GetFileNum(TxtInstallationPath.Text) > 0)
                //    {
                //        new FrmMessage("系统提示", "无法将文件安装到你选定的目录！\r\n请检查路径或者检查路径目录下是否存在其他文件。", false, BackColor).ShowDialog();
                //        return;
                //    }
                //}
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

                InstallProgressBar.Maximum = 11 + filesCount + ResourceManage.Instance.RegeditList.Length;
                InstallProgressBar.Value = 0;
                try
                {
                    new Thread(InstallThread).Start();
                }
                catch
                {
                    new FrmMessage("系统提示", "安装程序无法写入组件，安装将被中断，请重新运行安装程序。", false, BackColor).ShowDialog();
                    Process.GetCurrentProcess().Kill();
                }
            }
            else if (BtnInstall.Text.Equals("安装完成"))
            {
                if (CkStartClient.Checked)
                {
                    Process.Start(TxtInstallationPath.Text + "\\bin\\" + ResourceManage.Instance.InstallProductExeName);
                }
                this.Close();
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

                RegistryKey regFather = Registry.CurrentUser.OpenSubKey("SOFTWARE\\WayMark\\" + ResourceManage.Instance.InstallProductName, false);
                if (regFather != null)
                {
                    // 安装路径
                    string installpath = regFather.GetValue("InstallPath", "").ToString();
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
                    else if (Int32.Parse(str.Replace(".", "")) == Int32.Parse(ResourceManage.Instance.InstallProductVersion.Replace(".", "")))
                    {
                        if (new FrmMessage("系统提示", "当前计算机中已安装了本产品的相同版本，你确定以修复方式重新安装？", true, BackColor).ShowDialog() ==
                            DialogResult.Cancel)
                        {
                            CloseForm();
                        }
                        InstallPath = regFather.GetValue("InstallPath", "").ToString();
                        _fbd.SelectedPath = InstallPath;
                    }
                    else if (Int32.Parse(str.Replace(".", "")) > Int32.Parse(ResourceManage.Instance.InstallProductVersion.Replace(".", "")))
                    {
                        if (new FrmMessage("系统提示", "当前计算机中安装的产品版本高于当前安装程序的版本，你确定要使用旧版本替换新版本吗？", true, BackColor).ShowDialog() ==
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

                if (new FrmMessage("系统提示", "软件文件被破坏，接下来将以修复方式重新安装！", true, BackColor).ShowDialog() == DialogResult.Cancel)
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
                if (!Directory.Exists(srcPath))
                {
                    return fileNum;
                }
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
            bool bIsWow64 = false;
            IsWow64Process(Process.GetCurrentProcess().Handle, out bIsWow64);


            #region 正在检测相关程序是否在运
            type = type + 1;
            SetProgressMsg("正在检测相关程序是否在运... ...", type);
            KillPro();
            Thread.Sleep(100);
            #endregion

            #region 正在检测是否安装有旧版本
            type = type + 1;
            SetProgressMsg("正在检测是否安装有旧版本... ...", type);
            try
            {
                RegistryKey regFather =
                    Registry.CurrentUser.OpenSubKey(
                        "SOFTWARE\\WayMark", true);
                if (regFather == null)
                {
                    regFather =
                        Registry.CurrentUser.OpenSubKey(
                            "SOFTWARE", true);
                    //不存在WayMark节点则创建注册表节点
                    regFather.CreateSubKey("WayMark");
                    regFather =
                    Registry.CurrentUser.OpenSubKey(
                        "SOFTWARE\\WayMark", true);
                }
                regFather.DeleteSubKey(ResourceManage.Instance.InstallProductName, true);
                regFather.Close();
            }
            catch
            {
            }
            Thread.Sleep(100);
            #endregion

            #region 创建临时文件
            type = type + 1;
            SetProgressMsg("创建临时文件... ...", type);
            File.WriteAllBytes(TxtInstallationPath.Text + "\\bin.zip", ResourceManage.Instance.BinZip);
            Thread.Sleep(100);
            #endregion

            #region 正在解压文件
            SetProgressMsg("正在解压文件... ...", type);
            type = GetFiles(TxtInstallationPath.Text + "\\bin.zip", TxtInstallationPath.Text + "\\bin", type);
            //type = type + 1;
            Thread.Sleep(100);
            #endregion

            #region 正在注册文件
            SetProgressMsg("正在注册文件... ...", type);
            var RegeditList = ResourceManage.Instance.RegeditList;
            for (int i = 0; i < RegeditList.Length; i++)
            {
                type = type + 1;
                if (!string.IsNullOrEmpty(RegeditList[i]))
                {
                    try
                    {
                        SetProgressMsg(string.Format("正在注册文件:{0}... ...", RegeditList[i]), type);
                        string systemfoldr = Environment.GetFolderPath(Environment.SpecialFolder.System);
                        string pathfolder = InstallPath;
                        File.Copy(pathfolder + "\\bin\\" + RegeditList[i], systemfoldr + "\\" + RegeditList[i], true);

                        Process p = new Process();
                        p.StartInfo.FileName = "Regsvr32.exe";
                        p.StartInfo.Arguments = string.Format("/s {0}", systemfoldr + "\\" + RegeditList[i]);//路径中不能有空格
                        //不使用系统外壳程序启动
                        p.StartInfo.UseShellExecute = false;
                        //不重定向输入
                        p.StartInfo.RedirectStandardInput = true;
                        //重定向输出
                        p.StartInfo.RedirectStandardOutput = true;
                        //是否开启错误输出写入
                        p.StartInfo.RedirectStandardError = true;
                        //不显示窗体
                        p.StartInfo.CreateNoWindow = true;
                        ////设置以管理员身份运行启动
                        p.StartInfo.Verb = "runas";
                        p.Start();
                        p.BeginOutputReadLine();
                        p.WaitForExit();
                        p.Close();
                        SetProgressMsg(string.Format("注册文件:{0}:注册成功... ...", ResourceManage.Instance.RegeditList[i]), type);
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show("执行命令失败，请检查输入的命令是否正确！");
                    }
                    //开始注册
                    Thread.Sleep(100);
                }
            }
            //type = type + 1;
            Thread.Sleep(100);
            #endregion

            #region 正在创建卸载文件
            type = type + 1;
            SetProgressMsg("正在创建卸载文件... ...", type);
            File.WriteAllBytes(TxtInstallationPath.Text + "\\卸载.exe", Installation.Properties.Resources.UnInstall);
            if (!File.Exists(TxtInstallationPath.Text + "\\DSkin.dll"))
            {
                File.WriteAllBytes(TxtInstallationPath.Text + "\\DSkin.dll", Installation.Properties.Resources.DSkin);
            }

            Thread.Sleep(100);

            #endregion

            #region 正在创建卸载服务文件
            type = type + 1;
            SetProgressMsg("正在创建卸载服务文件... ...", type);
            if (File.Exists(InstallPath + "\\install.resources"))
            {
                File.Delete(InstallPath + "\\install.resources");
            }
            if (!File.Exists(InstallPath + "\\install.resources"))
            {
                FileStream fs = new FileStream(InstallPath + "\\install.resources", FileMode.OpenOrCreate,
                    FileAccess.Write);
                IResourceWriter writer = new ResourceWriter(fs);
                writer.AddResource("InstallCompanyName", ResourceManage.Instance.InstallCompanyName);
                writer.AddResource("InstallProductName", ResourceManage.Instance.InstallProductName);
                writer.AddResource("InstallProductExeName", ResourceManage.Instance.InstallProductExeName);
                writer.AddResource("InstallCompanyUrl", ResourceManage.Instance.InstallCompanyUrl);
                writer.AddResource("InstallationPath", TxtInstallationPath.Text);
                writer.AddResource("BackColor", this.BackColor);
                writer.AddResource("BackImage", this.BackgroundImage);
                writer.AddResource("bin", new byte[0]);
                writer.Close();
                fs.Close();
            }
            Thread.Sleep(100);
            #endregion

            #region 正在删除临时文件
            type = type + 1;
            SetProgressMsg("正在删除临时文件... ...", type);
            File.Delete(TxtInstallationPath.Text + "\\bin.zip");

            Thread.Sleep(100);
            #endregion

            #region 注册程序
            type = type + 1;
            SetProgressMsg("正在向系统注册程序... ...", type);
            try
            {
                RegistryKey regFather =
                    Registry.CurrentUser.OpenSubKey("SOFTWARE\\WayMark",
                        true);
                regFather.CreateSubKey(ResourceManage.Instance.InstallProductName);
                regFather =
                    Registry.CurrentUser.OpenSubKey(
                        "SOFTWARE\\WayMark\\" + ResourceManage.Instance.InstallProductName, true);
                regFather.SetValue("InstallPath", TxtInstallationPath.Text);
                regFather.SetValue("DisplayIcon", TxtInstallationPath.Text + "\\" + ResourceManage.Instance.InstallProductExeName);
                regFather.SetValue("DisplayName", ResourceManage.Instance.InstallProductName);
                regFather.SetValue("DisplayVersion", ResourceManage.Instance.InstallProductVersion);
                regFather.SetValue("Publisher", ResourceManage.Instance.InstallCompanyName);
                regFather.SetValue("UnInstallationString", TxtInstallationPath.Text + "\\UnInstallation.exe");
                regFather.SetValue("URLInfoAbout", ResourceManage.Instance.InstallCompanyUrl);
            }
            catch
            {

            }

            Thread.Sleep(100);
            #endregion

            #region 正在创建桌面快捷方式
            type = type + 1;
            Shortcut sc = new Shortcut();
            sc.Path = TxtInstallationPath.Text + "\\bin\\" + ResourceManage.Instance.InstallProductExeName;
            sc.Arguments = "";
            sc.WorkingDirectory = TxtInstallationPath.Text;
            sc.Description = "";
            if (CkDesktopShortcuts.Checked)
            {
                SetProgressMsg("正在创建桌面快捷方式... ...", type);
                sc.Save(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + ResourceManage.Instance.InstallProductName +
                        ".lnk");
            }
            Thread.Sleep(100);
            #endregion

            #region 正在创建开始菜单快捷方式
            type = type + 1;
            if (CkQuickLaunchBar.Checked)
            {
                SetProgressMsg("正在创建开始菜单快捷方式... ...", type);
                if (
                    !Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + "\\" +
                                      ResourceManage.Instance.InstallProductName + "\\bin"))
                {
                    Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + "\\" +
                                              ResourceManage.Instance.InstallProductName + "\\bin");
                }
                sc.Save(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + "\\" + ResourceManage.Instance.InstallProductName +
                        "\\" + ResourceManage.Instance.InstallProductName + ".lnk");
                sc = new Shortcut();
                sc.Path = TxtInstallationPath.Text + "\\卸载.exe";
                sc.Arguments = "";
                sc.WorkingDirectory = TxtInstallationPath.Text;
                sc.Description = "";
                sc.Save(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + "\\" + ResourceManage.Instance.InstallProductName +
                        "\\卸载.lnk");
            }
            Thread.Sleep(100);
            #endregion

            #region 正在注册开机启动
            type = type + 1;
            if (CkStartUp.Checked)
            {
                SetProgressMsg("正在注册开机启动... ...", type);
                try
                {
                    RegistryKey runKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                    runKey.SetValue(ResourceManage.Instance.InstallProductName, InstallPath + "\\bin\\" + ResourceManage.Instance.InstallProductExeName);
                    runKey.Close();
                }
                catch
                {
                }
            }
            Thread.Sleep(100);
            #endregion

            #region 正在释放的临时文件
            type = type + 1;
            SetProgressMsg("正在清理安装时释放的临时文件... ...", type);
            try
            {
                if (File.Exists(TxtInstallationPath + "\\InstallationUtil.exe"))
                {

                    File.Delete(TxtInstallationPath + "\\InstallationUtil.exe");
                }
                File.Delete("InstallationUtil.InstallationLog");
            }
            catch
            {
            }
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
            Process[] proArr = Process.GetProcessesByName(ResourceManage.Instance.InstallProductName);
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
                            using (FileStream streamWriter = File.Create(unZipDir + theEntry.Name))
                            {

                                int size = 2048;
                                byte[] data = new byte[2048];
                                while (true)
                                {
                                    size = s.Read(data, 0, data.Length);
                                    if (size > 0)
                                    {
                                        streamWriter.Write(data, 0, size);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
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
            FrmLicenseAgreement frmLicenseAgreement = new FrmLicenseAgreement();
            frmLicenseAgreement.ShowDialog();
        }
    }
}
