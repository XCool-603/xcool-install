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
using CSharpZip.Zip;
using CSharpZLib;

namespace InstalltionEdit
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        #region 安装程序变量
        //安装程序的公司名称
        public string InstallCompanyName = "广州维脉";
        //安装程序的产品名称
        public string InstallProductName = "智能交通服务系统";
        //安装程序的执行文件名称
        public string InstallProductExeName = "智能交通服务系统.exe";
        //安装程序的公司网址
        public string InstallCompanyUrl = "http://www.gzwaymark.com";

        /// <summary>
        /// 安装程序的路径
        /// </summary>
        public string InstallationPath = "";

        #endregion

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
        private void Form1_Load(object sender, EventArgs e)
        {
            if (!File.Exists(Application.StartupPath + "\\insatll.resources"))
            {
                FileStream fs = new FileStream(Application.StartupPath + "\\insatll.resources", FileMode.OpenOrCreate, FileAccess.Write);
                IResourceWriter writer = new ResourceWriter(fs);
                writer.AddResource("InstallProductName", TxtProductName.Text);
                writer.AddResource("InstallCompanyName", TxtCompanyName.Text);
                writer.AddResource("InstallProductExeName", CbExeName.Text);
                writer.AddResource("InstallCompanyUrl", TxtCompanyUrl.Text);
                writer.AddResource("InstallSetupPath", TxtStuepPath.Text);
                writer.AddResource("InstallationPath", TxtInstallPath.Text);
                writer.AddResource("InstallFilesCount", "0");
                writer.AddResource("BackColor", Color.FromArgb(6, 157, 231));
                writer.AddResource("bin", new byte[0]);
                writer.Close();
                fs.Close();
            }
            //FileStream fs = new FileStream(Application.StartupPath + "\\insatll.resources", FileMode.OpenOrCreate, FileAccess.Write);
            IResourceReader read = new ResourceReader(Application.StartupPath + "\\insatll.resources");
            var iter = read.GetEnumerator();
            while (iter.MoveNext())
            {
                switch (iter.Key.ToString())
                {
                    //case "InstallProductName": TxtProductName.Text = iter.Value.ToString();
                    //    break;
                    case "InstallCompanyName": TxtCompanyName.Text = iter.Value.ToString();
                        break;
                    //case "InstallProductExeName": CbExeName.Text = iter.Value.ToString();
                    //    break;
                    case "InstallCompanyUrl": TxtCompanyUrl.Text = iter.Value.ToString();
                        break;
                    case "InstallSetupPath": TxtStuepPath.Text = iter.Value.ToString();
                        break;
                    case "InstallationPath": TxtInstallPath.Text = iter.Value.ToString();
                        break;
                    case "BackColor": label7.BackColor = (Color)iter.Value;
                        break;
                    case "BackImage": pictureBox1.BackgroundImage = (Bitmap)iter.Value;
                        break;
                }
            }
            read.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (TxtProductName.Text == "")
            {
                MessageBox.Show("请选择执行文件！");
                return;
            }
            string[] exeNames = CbExeName.Text.Split('.');
            if (exeNames.Length != 2 || exeNames[1].ToUpper() != "EXE")
            {
                MessageBox.Show("选择的exe执行文件不是有效的执行文件！");
                return;
            } 
            progressBar1.Maximum = 9;
            progressBar1.Value = 0;
            if (TxtStuepName.Text.Trim().Equals(""))
            {
                MessageBox.Show("请输入要输出的安装包名称");
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

        public void ThreadBuild()
        {
            
                SetProgressMsg("正在压缩执行文件夹... ...", 1);
                ZipHelper.ZipFileDirectory(TxtExePath.Text, TxtStuepPath.Text + "\\bin.zip");

                SetProgressMsg("正在创建资源文件... ...", 2);
                CreatreSourcesIndf();

                SetProgressMsg("正在删除临时压缩文件... ...", 4);
                File.Delete(TxtStuepPath.Text + "\\bin.zip");
                SetProgressMsg("正在创建文件：DSkin.dll... ...", 5);
                File.WriteAllBytes(TxtStuepPath.Text + "\\DSkin.dll", Properties.Resources.DSkin);

                SetProgressMsg("正在创建文件：CSharpZip.dll... ...", 6);
                File.WriteAllBytes(TxtStuepPath.Text + "\\CSharpZip.dll", Properties.Resources.CSharpZip);

                SetProgressMsg("正在copy资源文件文件：insatll.resources... ...", 7);
                File.WriteAllBytes(string.Format("{0}\\{1}", TxtStuepPath.Text, TxtStuepName.Text), Properties.Resources.Installation);
                File.Copy(Application.StartupPath + "\\insatll.resources", TxtStuepPath.Text + "\\insatll.resources", true);
                SetProgressMsg("创建完成，正在打开生成目录... ...", 8);
                string path = TxtStuepPath.Text;
                System.Diagnostics.Process.Start("explorer.exe", path);
                SetProgressMsg("完成", 9);
                
            
        }

        public void CreatreSourcesIndf()
        {
            this.Invoke(new MethodInvoker(() =>
            {
                FileStream fs = new FileStream(Application.StartupPath + "\\insatll.resources", FileMode.OpenOrCreate,
                    FileAccess.Write);
                IResourceWriter writer = new ResourceWriter(fs);
                writer.AddResource("InstallProductName", TxtProductName.Text);
                writer.AddResource("InstallCompanyName", TxtCompanyName.Text);
                writer.AddResource("InstallProductExeName", CbExeName.Text);
                writer.AddResource("InstallProductVersion", TxtProductVersion.Text);
                writer.AddResource("InstallCompanyUrl", TxtCompanyUrl.Text);
                writer.AddResource("InstallSetupPath", TxtStuepPath.Text);
                writer.AddResource("InstallationPath", TxtInstallPath.Text);
                writer.AddResource("CkDesktopShortcuts", CkDesktopShortcuts.Checked);
                writer.AddResource("CkQuickLaunchBar", CkQuickLaunchBar.Checked);
                writer.AddResource("CkStartUp", CkStartUp.Checked);
                writer.AddResource("CkStartClient", CkStartClient.Checked);
                writer.AddResource("BackColor", label7.BackColor);
                writer.AddResource("BackImage", pictureBox1.BackgroundImage);
                writer.AddResource("Logo", pictureBox2.BackgroundImage);
                writer.AddResource("InstallFilesCount", GetFileNum(TxtExePath.Text));
                writer.AddResource("LicenseAgreement", ReadFile(TxtTxtLicense.Text));
                
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
        int fileNum = 0;
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
        /// 线程控制UI显示进度信息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="progressvalue"></param>
        public void SetProgressMsg(string msg, int progressvalue)
        {
            this.Invoke(new MethodInvoker(delegate
            {
                Lblinfo.Text = msg;
                progressBar1.Value = progressvalue;
            }));
        } 
        private void label7_Click(object sender, EventArgs e)
        {
            ColorDialog c=new ColorDialog();
            c.ShowDialog();
            label7.BackColor = c.Color;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            IResourceReader read = new ResourceReader(Application.StartupPath + "\\insatll.resources");
            
            var iter = read.GetEnumerator();
            while (iter.MoveNext())
            {
                switch (iter.Key.ToString())
                {
                    case "bin": byte[] zipfile=(byte[])iter.Value;
                        File.WriteAllBytes(TxtStuepPath.Text + "\\bin.zip", zipfile);
                        break;
                }
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            FolderBrowserDialog dir=new FolderBrowserDialog();
            if(dir.ShowDialog()==DialogResult.OK)
            {
                listView1.Items.Clear();
                CbExeName.Items.Clear();
                CbExeName.Text = "";
                TxtExePath.Text = dir.SelectedPath;
                DirectoryInfo diriinfo = new DirectoryInfo(TxtExePath.Text);
                DirectoryInfo[] folders = diriinfo.GetDirectories();
                FileInfo[] files = diriinfo.GetFiles();
                foreach (DirectoryInfo folderinfo in folders)
                {
                    try
                    {
                        listView1.Items.Add(new ListViewItem(new string[] { folderinfo.Name, "文件夹", folderinfo .CreationTime.ToString("yyyy-MM-dd HH:mm:ss")}));
                    }
                    catch { }
                }
                foreach (FileInfo file in files)
                {
                    try
                    {
                        listView1.Items.Add(new ListViewItem(new string[] { file.Name, file.Extension, file.CreationTime.ToString("yyyy-MM-dd HH:mm:ss") }));
                        if (file.Extension.Trim().ToLower().Equals(".exe"))
                        {
                            CbExeName.Items.Add(file.Name);
                        }
                    }
                    catch { }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dir=new FolderBrowserDialog();
            if (dir.ShowDialog() == DialogResult.OK)
            {
                TxtInstallPath.Text = dir.SelectedPath;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            TxtProductName.Text = CbExeName.Text.Substring(0, CbExeName.Text.LastIndexOf('.'));

            FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(Path.Combine(TxtExePath.Text, CbExeName.Text)); ;
            TxtProductVersion.Text = myFileVersionInfo.FileVersion;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "图片|*.jpg;*.png;";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
               pictureBox1.BackgroundImage=new Bitmap(fileDialog.FileName);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            pictureBox1.BackgroundImage = null;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dir = new FolderBrowserDialog();
            if (dir.ShowDialog() == DialogResult.OK)
            {
                TxtStuepPath.Text = dir.SelectedPath;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "图片|*.jpg;*.png;";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox2.BackgroundImage = new Bitmap(fileDialog.FileName);
            }
        }
        private void button9_Click(object sender, EventArgs e)
        {
            pictureBox2.BackgroundImage = null;
        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "文本|*.txt;";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                TxtTxtLicense.Text = fileDialog.FileName;
            }
        }
    }
}
