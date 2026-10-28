using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Resources;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using CSharpZLib;
using DSkin.Controls;
using DSkin.DirectUI;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace InstalltionEdit
{
    public partial class FrmMain : FrmBase
    {
        #region 构造函数
        public FrmMain()
        {
            InitializeComponent();
            this.Text = string.Format("安装包制作工具  V{0}       作者：BinGoo", Application.ProductVersion);
            dSkinGridList1.Columns.Add(new DSkinGridListColumn() { Name = "文件名", Width = 159 });
            dSkinGridList1.Columns.Add(new DSkinGridListColumn() { Name = "文件类型", Width = 80 });
            dSkinGridList1.Columns.Add(new DSkinGridListColumn() { Name = "创建日期", Width = 140 });
            dSkinGridList1.Columns.Add(new DSkinGridListColumn() { Name = "是否注册", Width = 80 });
            if (!Directory.Exists(Application.StartupPath + "\\SteupProject"))
            {
                Directory.CreateDirectory(Application.StartupPath + "\\SteupProject");
            }
            if (!File.Exists(Application.StartupPath + "\\SteupProject\\steup.resources"))
            {
                CreatResources();
            }
            ReadResources(Application.StartupPath + "\\SteupProject\\steup.resources");
            if (Directory.Exists(TxtExePath.Text))
            {
                CreatFileList(TxtExePath.Text);
            }
            else
            {
                TxtExePath.Text = "";
            }
            TxtInstallPath.WaterText = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        } 
        #endregion

        #region 按钮事件
        /// <summary>
        /// 退出按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCloseClick(object sender, EventArgs e)
        {
            CloseForm();
        }

        
        /// <summary>
        /// 获取需要打包的文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnGetExeFolder(object sender, EventArgs e)
        {
            //FolderBrowserDialog dir = new FolderBrowserDialog();
            //if (dir.ShowDialog() == DialogResult.OK)
            //{
            //    TxtExePath.Text = dir.SelectedPath;
            //    CreatFileList(TxtExePath.Text);
            //}
            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = true;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    string selectedPath = dialog.FileName;
                    TxtExePath.Text = selectedPath;
                    CreatFileList(TxtExePath.Text);
                }
            }
        }
        /// <summary>
        /// 选择安装程序输出文件夹位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnOutPutPathClick(object sender, EventArgs e)
        {
            //FolderBrowserDialog dir = new FolderBrowserDialog();
            //if (dir.ShowDialog() == DialogResult.OK)
            //{
            //    TxtStuepPath.Text = dir.SelectedPath;
            //}

            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = true;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    string selectedPath = dialog.FileName;
                    TxtStuepPath.Text = selectedPath;
                }
            }
        }

        /// <summary>
        /// 选择安装的路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnInstallPathClick(object sender, EventArgs e)
        {
            //FolderBrowserDialog dir = new FolderBrowserDialog();
            //if (dir.ShowDialog() == DialogResult.OK)
            //{
            //    TxtInstallPath.Text = dir.SelectedPath;
            //}
            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = true;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    string selectedPath = dialog.FileName;
                    TxtInstallPath.Text = selectedPath;
                }
            }
        }
        /// <summary>
        /// 选择软件许可证文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLicenseFileClick(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "文本|*.txt;";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                TxtTxtLicense.Text = ReadFile(fileDialog.FileName);
            }
        }

        /// <summary>
        /// 安装文件背景色选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LblInstallBackColorClick(object sender, EventArgs e)
        {
            ColorDialog c = new ColorDialog();
            c.ShowDialog();
            LblInstallBackColor.BackColor = c.Color;
        }
        /// <summary>
        /// 安装包背景图片选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnInstallBackImageClick(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "图片|*.jpg;*.png;";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                PicInstallBackImage.BackgroundImage = new Bitmap(fileDialog.FileName);
            }
        }
        /// <summary>
        /// 清除安装包背景图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnInstallBackImageClearClick(object sender, EventArgs e)
        {
            PicInstallBackImage.BackgroundImage = null;
        }
        /// <summary>
        /// 安装包LOGO图片选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnInstallLogoClick(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "图片|*.jpg;*.png;";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                PicInstallLogo.BackgroundImage = new Bitmap(fileDialog.FileName);
            }
        }
        /// <summary>
        /// 清除安装包LOGO图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnInstallLogoClearClick(object sender, EventArgs e)
        {
            PicInstallLogo.BackgroundImage = null;
        }

        /// <summary>
        /// EXE选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbExeNameSelectedIndexChanged(object sender, EventArgs e)
        {
           
        }

        /// <summary>
        /// 开始制作安装包
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnStartClick(object sender, EventArgs e)
        {
            dSkinProgressBar1.Maximum = 9;
            dSkinProgressBar1.Value = 0;
            bool isok = IsProductInfoFull();
            if (!isok)
            {
                return;
            }
            new Thread(() =>
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback((o) =>
                {
                    ThreadBuild();
                }));
            }).Start();
        }
        /// <summary>
        /// 新建工程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnNewClick(object sender, EventArgs e)
        {
            CreatResources();
            ReadResources(Application.StartupPath + "\\SteupProject\\steup.resources");
            dSkinGridList1.Rows.Clear();
            PicInstallBackImage.BackgroundImage = null;
            PicInstallLogo.BackgroundImage = null;
            CbExeName.Items.Clear();
            CbExeName.Text = "";
            dSkinProgressBar1.Value = 0;
            Lblinfo.Text = "";
        }
        private void BtnOpenProjectClick(object sender, EventArgs e)
        {
            CreatResources();
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = string.Format("安装包制作工程文件(.dpro)|*.dpro;");
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                ReadResources(fileDialog.FileName);
                if (Directory.Exists(TxtExePath.Text))
                {
                    CreatFileList(TxtExePath.Text);
                }
                else
                {
                    TxtExePath.Text = "";
                }
                dSkinProgressBar1.Value = 0;
                Lblinfo.Text = "";
            }
        }

        

        #endregion

        #region 方法
        /// <summary>
        /// 开始制作安装包线程进度
        /// </summary>
        public void ThreadBuild()
        {
            if (!Directory.Exists(TxtStuepPath.Text))
            {
                Directory.CreateDirectory(TxtStuepPath.Text);
            }
            SetProgressMsg("正在压缩执行文件夹... ...", 1);
            ZipHelper.ZipFileDirectory(TxtExePath.Text, TxtStuepPath.Text + "\\bin.zip");

            SetProgressMsg("正在创建资源文件... ...", 2);
            CreatreSourcesInfoThread();

            SetProgressMsg("正在删除临时压缩文件... ...", 4);
            File.Delete(TxtStuepPath.Text + "\\bin.zip");
            SetProgressMsg("正在创建文件：DSkin.dll... ...", 5);
            File.WriteAllBytes(TxtStuepPath.Text + "\\DSkin.dll", Properties.Resources.DSkin);

            SetProgressMsg("正在创建文件：CSharpZip.dll... ...", 6);
            File.WriteAllBytes(TxtStuepPath.Text + "\\CSharpZip.dll", Properties.Resources.CSharpZip);

            SetProgressMsg("正在copy资源文件文件：install.resources... ...", 7);
            File.WriteAllBytes(string.Format("{0}\\{1}", TxtStuepPath.Text, TxtStuepName.Text), Properties.Resources.Installation);
            File.Copy(Application.StartupPath + "\\SteupProject\\steup.resources", TxtStuepPath.Text + "\\install.resources", true);
            SetProgressMsg("创建完成，正在打开生成目录... ...", 8);
            string path = TxtStuepPath.Text;
            System.Diagnostics.Process.Start("explorer.exe", path);
            SetProgressMsg("完成", 9);


        }

        /// <summary>
        /// 创建资源文件
        /// </summary>
        public void CreatreSourcesInfoThread()
        {
            this.Invoke(new MethodInvoker(() =>
            {
                string tempPath = Application.StartupPath + "\\SteupProject\\steup.resources";
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
                FileStream fs = new FileStream(tempPath, FileMode.CreateNew,
                    FileAccess.Write);
                IResourceWriter writer = new ResourceWriter(fs);
                writer.AddResource("InstallProductName", Path.GetFileNameWithoutExtension(CbExeName.SelectedDuiControl.Text));
                writer.AddResource("InstallCompanyName", TxtCompanyName.Text);
                writer.AddResource("InstallProductExeName", CbExeName.SelectedDuiControl.Text);
                writer.AddResource("InstallProductVersion", TxtProductVersion.Text);
                writer.AddResource("InstallCompanyUrl", TxtCompanyUrl.Text);
                writer.AddResource("InstallSetupPath", TxtStuepPath.Text);
                writer.AddResource("InstallationPath", TxtInstallPath.Text);
                writer.AddResource("InstallationExePath", TxtExePath.Text);
                writer.AddResource("CkDesktopShortcuts", CkDesktopShortcuts.Checked);
                writer.AddResource("CkQuickLaunchBar", CkQuickLaunchBar.Checked);
                writer.AddResource("CkStartUp", CkStartUp.Checked);
                writer.AddResource("CkStartClient", CkStartClient.Checked);
                writer.AddResource("BackColor", LblInstallBackColor.BackColor);
                writer.AddResource("BackImage", PicInstallBackImage.BackgroundImage);
                writer.AddResource("Logo", PicInstallLogo.BackgroundImage);

                _fileNum = 0;
                writer.AddResource("InstallFilesCount", GetFileNum(TxtExePath.Text));
                writer.AddResource("LicenseAgreement", TxtTxtLicense.Text);
                string files = "";
                for (int i = 0; i < dSkinGridList1.Rows.Count; i++)
                {
                    if (dSkinGridList1.Rows[i].Cells.Count >= 4)
                    {
                        if ((bool) dSkinGridList1.Rows[i].Cells[3].Value)
                        {
                            files += dSkinGridList1.Rows[i].Cells[0].Value.ToString()+";";
                        }
                    }
                }
                writer.AddResource("RegditFiles", files);
                FileStream stream = new FileInfo(TxtStuepPath.Text + "\\bin.zip").OpenRead();
                Byte[] buffer = new Byte[stream.Length];
                //从流中读取字节块并将该数据写入给定缓冲区buffer中
                stream.Read(buffer, 0, Convert.ToInt32(stream.Length));

                SetProgressMsg("正在将压缩包写入资源文件... ...", 3);
                writer.AddResource("bin", buffer);
                writer.Close();
                stream.Close();
                fs.Close();
            }));
        }
        /// <summary>
        /// 生成预览简略版资源
        /// </summary>
        public void ViewSourcesInfo()
        {
            FileStream fs = new FileStream(Application.StartupPath + "\\SteupProject\\steup.resources", FileMode.OpenOrCreate,
                    FileAccess.Write);
                IResourceWriter writer = new ResourceWriter(fs);
                writer.AddResource("InstallProductName", Path.GetFileNameWithoutExtension(CbExeName.Text));
                writer.AddResource("InstallCompanyName", TxtCompanyName.Text);
                writer.AddResource("InstallProductExeName", CbExeName.Text);
                writer.AddResource("InstallProductVersion", TxtProductVersion.Text);
                writer.AddResource("InstallCompanyUrl", TxtCompanyUrl.Text);
                writer.AddResource("InstallSetupPath", TxtStuepPath.Text);
                writer.AddResource("InstallationPath", TxtInstallPath.Text);
                writer.AddResource("InstallationExePath", TxtExePath.Text);
                writer.AddResource("CkDesktopShortcuts", CkDesktopShortcuts.Checked);
                writer.AddResource("CkQuickLaunchBar", CkQuickLaunchBar.Checked);
                writer.AddResource("CkStartUp", CkStartUp.Checked);
                writer.AddResource("CkStartClient", CkStartClient.Checked);
                writer.AddResource("BackColor", LblInstallBackColor.BackColor);
                writer.AddResource("BackImage", PicInstallBackImage.BackgroundImage);
                writer.AddResource("Logo", PicInstallLogo.BackgroundImage);
                writer.AddResource("InstallFilesCount", GetFileNum(TxtExePath.Text));
                writer.AddResource("LicenseAgreement", TxtTxtLicense.Text);
                writer.AddResource("bin", new byte[0]);
                writer.Close();
                fs.Close();
        }
        /// <summary>
        /// 文件数量
        /// </summary>
        int _fileNum = 0;
        /// <summary>
        /// 获取某目录下的所有文件(包括子目录下文件)的数量
        /// </summary>
        /// <param name="srcPath"></param>
        /// <returns></returns>
        public int GetFileNum(string srcPath)
        {
            try
            {
                string[] fileList = System.IO.Directory.GetFileSystemEntries(srcPath);
                foreach (string file in fileList)
                {
                    if (System.IO.Directory.Exists(file))
                        GetFileNum(file);
                    else
                        _fileNum++;
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            return _fileNum;
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
                Lblinfo.Text = msg;
                dSkinProgressBar1.Value = progressvalue;
            }));
        }
        /// <summary>
        /// 读取TXT文件信息
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string ReadFile(string fileName)
        {
            //异常检测开始
            try
            {
                string fileContent = "";
                using (var reader = new StreamReader(fileName))
                {
                    fileContent = reader.ReadToEnd();
                }
                return fileContent;
            }
            catch
            {
                //抛出异常
                return "";
            }
            //异常检测结束
        }

        public void CreatResources()
        {
            FileStream fs = new FileStream(Application.StartupPath + "\\SteupProject\\steup.resources", FileMode.OpenOrCreate,
                       FileAccess.Write);
            IResourceWriter writer = new ResourceWriter(fs);
            writer.AddResource("InstallProductName", "");
            writer.AddResource("InstallCompanyName", "");
            writer.AddResource("InstallProductExeName", "");
            writer.AddResource("InstallProductVersion", "");
            writer.AddResource("InstallCompanyUrl", "");
            writer.AddResource("InstallSetupPath", "");
            writer.AddResource("InstallationPath", "");
            writer.AddResource("InstallationExePath", "");
            writer.AddResource("CkDesktopShortcuts", false);
            writer.AddResource("CkQuickLaunchBar", false);
            writer.AddResource("CkStartUp", false);
            writer.AddResource("CkStartClient", false);
            writer.AddResource("BackColor", Color.FromArgb(255, 6, 157, 231));
            writer.AddResource("BackImage", new byte[0]);
            writer.AddResource("Logo", new byte[0]);
            writer.AddResource("InstallFilesCount", 0);
            writer.AddResource("LicenseAgreement", "");
            writer.AddResource("bin", new byte[0]);
            writer.Close();
            fs.Close();
        }

        public void ReadResources(string filepath)
        {
            //FileStream fs = new FileStream(Application.StartupPath + "\\install.resources", FileMode.OpenOrCreate, FileAccess.Write);
            IResourceReader read = new ResourceReader(filepath);
            var iter = read.GetEnumerator();
            while (iter.MoveNext())
            {
                try
                {
                    switch (iter.Key.ToString())
                    {
                        case "InstallCompanyName":
                            TxtCompanyName.Text = iter.Value.ToString();
                            break;
                        case "InstallProductExeName":
                            CbExeName.Text = iter.Value.ToString();
                            break;
                        case "InstallProductVersion":
                            TxtProductVersion.Text = iter.Value.ToString();
                            break;
                        case "InstallCompanyUrl":
                            TxtCompanyUrl.Text = iter.Value.ToString();
                            break;
                        case "InstallSetupPath":
                            TxtStuepPath.Text = iter.Value.ToString();
                            break;
                        case "InstallationPath":
                            TxtInstallPath.Text = iter.Value.ToString();
                            break;
                        case "InstallationExePath":
                            TxtExePath.Text = iter.Value.ToString();
                            break;
                        case "LicenseAgreement":
                            TxtTxtLicense.Text = iter.Value.ToString();
                            break;
                        case "BackColor":
                            LblInstallBackColor.BackColor = (Color) iter.Value;
                            break;
                        case "BackImage":
                            PicInstallBackImage.BackgroundImage = (Bitmap) iter.Value;
                            break;
                        case "Logo":
                            PicInstallLogo.BackgroundImage = (Bitmap) iter.Value;
                            break;
                        case "CkDesktopShortcuts":
                            CkDesktopShortcuts.Checked = (bool) iter.Value;
                            break;
                        case "CkQuickLaunchBar":
                            CkQuickLaunchBar.Checked = (bool) iter.Value;
                            break;
                        case "CkStartUp":
                            CkStartUp.Checked = (bool) iter.Value;
                            break;
                        case "CkStartClient":
                            CkStartClient.Checked = (bool) iter.Value;
                            break;
                    }
                }
                catch
                {
                }
            }
            read.Close();
        }

        public void CreatFileList(string path)
        {
            dSkinGridList1.Rows.Clear();
            CbExeName.Items.Clear();
            CbExeName.Text = "";
            DirectoryInfo diriinfo = new DirectoryInfo(TxtExePath.Text);
            DirectoryInfo[] folders = diriinfo.GetDirectories();
            FileInfo[] files = diriinfo.GetFiles();
            foreach (DirectoryInfo folderinfo in folders)
            {
                try
                {
                    DSkinGridListRow row = new DSkinGridListRow();
                    row.Cells.Add(new DSkinGridListCell() { Value = folderinfo.Name });
                    row.Cells.Add(new DSkinGridListCell() { Value = "文件夹" });
                    row.Cells.Add(new DSkinGridListCell() { Value = folderinfo.CreationTime.ToString("yyyy-MM-dd HH:mm:ss") });
                    dSkinGridList1.Rows.Add(row);
                }
                catch { }
            }
            foreach (FileInfo file in files)
            {
                try
                {
                    DSkinGridListRow row = new DSkinGridListRow();
                    row.Cells.Add(new DSkinGridListCell() { Value = file.Name });
                    row.Cells.Add(new DSkinGridListCell() { Value = file.Extension });
                    row.Cells.Add(new DSkinGridListCell() { Value = file.CreationTime.ToString("yyyy-MM-dd HH:mm:ss") });
                    if (file.Extension.ToUpper().Equals(".DLL") || file.Extension.ToUpper().Equals(".OCX"))
                    {
                        row.Cells.Add(new DSkinGridListCell() { Value = false,Text = "", ItemType = ControlType.DuiCheckBox });
                    }
                    dSkinGridList1.Rows.Add(row);
                    if (file.Extension.Trim().ToLower().Equals(".exe"))
                    {
                        DuiLabel lblname = new DuiLabel()
                        {
                            Font = new Font("微软雅黑", 9),
                            Text = file.Name,
                            TextRenderMode = TextRenderingHint.ClearTypeGridFit,
                            TextAlign = ContentAlignment.MiddleLeft,
                            Size = new Size(CbExeName.Width, CbExeName.Height)
                        };
                        lblname.MouseClick += (s, e) =>
                        {
                           DuiLabel lbl= (DuiLabel) s;
                            FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(Path.Combine(TxtExePath.Text, lbl.Text)); ;
                            TxtProductVersion.Text = myFileVersionInfo.FileVersion;
                        };
                        CbExeName.Items.Add(lblname);
                    }
                }
                catch { }
            }
        }
        public bool IsProductInfoFull()
        {
            if (CbExeName.SelectedDuiControl == null)
            {
                new FrmMessage("系统提示", "清先选择exe执行文件！", false).ShowDialog();
                return false;
            }
            string[] exeNames = CbExeName.SelectedDuiControl.Text.Split('.');
            if (exeNames.Length != 2 || exeNames[1].ToUpper() != "EXE")
            {
                new FrmMessage("系统提示", "选择的exe执行文件不是有效的执行文件！", false).ShowDialog();
                return false;
            }
            if (string.IsNullOrEmpty(TxtStuepPath.Text.Trim()))
            {
                new FrmMessage("系统提示", "请输入要输出的安装包路径！", false).ShowDialog();
                return false;
            }
            if (string.IsNullOrEmpty(TxtStuepName.Text.Trim()))
            {
                new FrmMessage("系统提示", "请输入要输出的安装包名称！", false).ShowDialog();
                return false;
            }
            return true;
        }
        #endregion

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnEditLicenseClick(object sender, EventArgs e)
        {
            FrmLicenseAgreement frmLicenseAgreement=new FrmLicenseAgreement(TxtTxtLicense.Text);
            frmLicenseAgreement.ShowDialog();
            TxtTxtLicense.Text = frmLicenseAgreement.License;
        }

        private void BtnSaveLicenseClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TxtTxtLicense.Text))
            {
                MessageBox.Show("要写入的文件内容不能为空");
            }
            else
            {
                SaveFileDialog sfd = new SaveFileDialog();
                //设置保存文件的格式
                sfd.Filter = "文本文件(*.txt)|*.txt";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    //使用“另存为”对话框中输入的文件名实例化StreamWriter对象
                    StreamWriter sw = new StreamWriter(sfd.FileName, true);
                    //向创建的文件中写入内容
                    sw.WriteLine(TxtTxtLicense.Text);
                    //关闭当前文件写入流
                    sw.Close();
                }
            }
        }

        private void BtnSaveProjectClick(object sender, EventArgs e)
        {

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.AddExtension = true;
            sfd.RestoreDirectory = true;
            //设置保存文件的格式工程资源文件|*.resources;
            sfd.Filter = "安装包制作工程文件(.dpro)|*.dpro;";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = new FileStream(Application.StartupPath + "\\install.resources", FileMode.OpenOrCreate,
                       FileAccess.Write);
                IResourceWriter writer = new ResourceWriter(fs);
                writer.AddResource("InstallProductName", Path.GetFileNameWithoutExtension(CbExeName.Text));
                writer.AddResource("InstallCompanyName", TxtCompanyName.Text);
                writer.AddResource("InstallProductExeName", CbExeName.Text);
                writer.AddResource("InstallProductVersion", TxtProductVersion.Text);
                writer.AddResource("InstallCompanyUrl", TxtCompanyUrl.Text);
                writer.AddResource("InstallSetupPath", TxtStuepPath.Text);
                writer.AddResource("InstallationPath", TxtInstallPath.Text);
                writer.AddResource("InstallationExePath", TxtExePath.Text);
                writer.AddResource("CkDesktopShortcuts", CkDesktopShortcuts.Checked);
                writer.AddResource("CkQuickLaunchBar", CkQuickLaunchBar.Checked);
                writer.AddResource("CkStartUp", CkStartUp.Checked);
                writer.AddResource("CkStartClient", CkStartClient.Checked);
                writer.AddResource("BackColor", LblInstallBackColor.BackColor);
                writer.AddResource("BackImage", PicInstallBackImage.BackgroundImage);
                writer.AddResource("Logo", PicInstallLogo.BackgroundImage);
                writer.AddResource("InstallFilesCount", GetFileNum(TxtExePath.Text));
                writer.AddResource("LicenseAgreement", TxtTxtLicense.Text);
                writer.AddResource("bin", new byte[0]);
                writer.Close();
                fs.Close();
                //使用“另存为”对话框中输入的文件名实例化StreamWriter对象
                StreamWriter sw = new StreamWriter(sfd.FileName, true);
                //向创建的文件中写入内容
                sw.WriteLine(TxtTxtLicense.Text);
                //关闭当前文件写入流
                sw.Close();
                string filename = sfd.FileName;
                if (sfd.FileName.Split('.').Length == 1)
                {
                    filename = filename + ".dpro";
                }
                File.Copy(Application.StartupPath + "\\install.resources", filename, true);
            }

        }

        private void BtnViewClick(object sender, EventArgs e)
        {
            bool isok = IsProductInfoFull();
            if (isok)
            {
                ViewSourcesInfo();
                FrmInstallation frmInstallation = new FrmInstallation();
                frmInstallation.Show();
            }
        }

        private void dSkinPanel2_MouseDown(object sender, MouseEventArgs e)
        {
            DSkin.NativeMethods.MouseToMoveControl(this.Handle);
        }
    }
}
