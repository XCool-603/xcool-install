using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Installer.Core.Platform
{
    /// <summary>
    /// 最小化的 IShellLink / IPersistFile 互操作，用来创建 .lnk。
    ///
    /// 现状的 <c>Installation\Shortcut.cs</c> 也是走这条路（而不是 WScript.Shell），
    /// 这里做了一版更精简、并补上 COM 对象释放的实现。
    /// </summary>
    internal static class ShellLink
    {
        [ComImport]
        [Guid("00021401-0000-0000-C000-000000000046")]
        [ClassInterface(ClassInterfaceType.None)]
        private class CShellLink
        {
        }

        [ComImport]
        [Guid("000214F9-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IShellLinkW
        {
            void GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cch, IntPtr pfd, int fFlags);
            void GetIDList(out IntPtr ppidl);
            void SetIDList(IntPtr pidl);
            void GetDescription([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cch);
            void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
            void GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cch);
            void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
            void GetArguments([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cch);
            void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
            void GetHotkey(out short pwHotkey);
            void SetHotkey(short wHotkey);
            void GetShowCmd(out int piShowCmd);
            void SetShowCmd(int iShowCmd);
            void GetIconLocation([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cch, out int piIcon);
            void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
            void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);
            void Resolve(IntPtr hwnd, int fFlags);
            void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
        }

        [ComImport]
        [Guid("0000010b-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IPersistFile
        {
            void GetClassID(out Guid pClassID);
            [PreserveSig]
            int IsDirty();
            void Load([MarshalAs(UnmanagedType.LPWStr)] string pszFileName, uint dwMode);
            void Save([MarshalAs(UnmanagedType.LPWStr)] string pszFileName, [MarshalAs(UnmanagedType.Bool)] bool fRemember);
            void SaveCompleted([MarshalAs(UnmanagedType.LPWStr)] string pszFileName);
            void GetCurFile([MarshalAs(UnmanagedType.LPWStr)] out string ppszFileName);
        }

        private const int MAX_PATH = 260;

        /// <summary>创建一个 .lnk 文件。</summary>
        public static void Create(string linkPath, string targetPath, string arguments,
                                  string workingDirectory, string description, string iconPath)
        {
            object shellLinkObject = null;
            object persistFileObject = null;

            try
            {
                shellLinkObject = new CShellLink();
                var link = (IShellLinkW)shellLinkObject;

                link.SetPath(targetPath);

                if (!string.IsNullOrEmpty(arguments))
                {
                    link.SetArguments(arguments);
                }

                if (!string.IsNullOrEmpty(workingDirectory))
                {
                    link.SetWorkingDirectory(workingDirectory);
                }

                if (!string.IsNullOrEmpty(description))
                {
                    link.SetDescription(description);
                }

                if (!string.IsNullOrEmpty(iconPath) && System.IO.File.Exists(iconPath))
                {
                    link.SetIconLocation(iconPath, 0);
                }

                persistFileObject = shellLinkObject;
                var persist = (IPersistFile)persistFileObject;
                persist.Save(linkPath, true);
            }
            finally
            {
                // 必须释放 COM 对象：现状的 Shortcut 类从不 ReleaseComObject
                if (persistFileObject != null && Marshal.IsComObject(persistFileObject))
                {
                    Marshal.ReleaseComObject(persistFileObject);
                }

                if (shellLinkObject != null && Marshal.IsComObject(shellLinkObject))
                {
                    Marshal.ReleaseComObject(shellLinkObject);
                }
            }
        }
    }
}
