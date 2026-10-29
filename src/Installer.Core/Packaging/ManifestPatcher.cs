using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace Installer.Core.Packaging
{
    /// <summary>
    /// 把安装包 exe 里的**应用程序清单**（RT_MANIFEST）换成
    /// <c>requestedExecutionLevel level="requireAdministrator"</c> 的版本。
    ///
    /// 为什么需要：stub 默认是 <c>asInvoker</c>，靠运行期"发现要写 Program Files 就
    /// 用 runas 重启自己"来提权。用户要求**做出来的包本身就以管理员身份启动**
    /// （双击即弹 UAC），所以打包时直接改清单。
    ///
    /// 注意：改的是产物 exe 的清单资源，**不影响**运行期逻辑 —— 提权重启那条路
    /// 仍然保留（清单是 asInvoker 的老包照样能用）。
    /// 卸载器是从这个 exe 的前 N 字节复制出来的，所以它也会继承 requireAdministrator，
    /// 卸载时同样要求管理员 —— 这与"装到 Program Files / 写 HKLM"是对齐的。
    /// </summary>
    public static class ManifestPatcher
    {
        private const int RTManifest = 24;

        /// <summary>要写入的清单内容。</summary>
        public const string RequireAdministratorManifest =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
            "<assembly manifestVersion=\"1.0\" xmlns=\"urn:schemas-microsoft-com:asm.v1\">\r\n" +
            "  <assemblyIdentity version=\"1.0.0.0\" name=\"Installer.Runtime\" type=\"win32\"/>\r\n" +
            "  <trustInfo xmlns=\"urn:schemas-microsoft-com:asm.v3\">\r\n" +
            "    <security>\r\n" +
            "      <requestedPrivileges>\r\n" +
            "        <requestedExecutionLevel level=\"requireAdministrator\" uiAccess=\"false\"/>\r\n" +
            "      </requestedPrivileges>\r\n" +
            "    </security>\r\n" +
            "  </trustInfo>\r\n" +
            "  <compatibility xmlns=\"urn:schemas-microsoft-com:compatibility.v1\">\r\n" +
            "    <application>\r\n" +
            "      <supportedOS Id=\"{8e0f7a12-bfb3-4fe8-b9a5-48fd50a15a9a}\"/>\r\n" +
            "      <supportedOS Id=\"{1f676c76-80e1-4239-95bb-83d0f6d0da78}\"/>\r\n" +
            "      <supportedOS Id=\"{4a2f28e3-53b9-4441-ba9c-d69d4a4a6e38}\"/>\r\n" +
            "      <supportedOS Id=\"{35138b9a-5d96-4fbd-8e2d-a2440225f93a}\"/>\r\n" +
            "    </application>\r\n" +
            "  </compatibility>\r\n" +
            "  <application xmlns=\"urn:schemas-microsoft-com:asm.v3\">\r\n" +
            "    <windowsSettings>\r\n" +
            "      <dpiAware xmlns=\"http://schemas.microsoft.com/SMI/2005/WindowsSettings\">true</dpiAware>\r\n" +
            "    </windowsSettings>\r\n" +
            "  </application>\r\n" +
            "</assembly>\r\n";

        /// <summary>
        /// 把 <paramref name="exePath"/> 的清单换成 requireAdministrator。
        /// 返回 true 表示写成功。
        /// </summary>
        public static bool ApplyRequireAdministrator(string exePath)
        {
            return ReplaceManifest(exePath, RequireAdministratorManifest);
        }

        /// <summary>替换 exe 的 RT_MANIFEST（id=1）。</summary>
        public static bool ReplaceManifest(string exePath, string manifestXml)
        {
            if (string.IsNullOrEmpty(exePath))
            {
                throw new ArgumentNullException("exePath");
            }

            var bytes = Encoding.UTF8.GetBytes(manifestXml);

            // 先查清单**当前挂在哪个语言下**，只覆盖那一条。
            //
            // 踩过的坑（都实测过）：
            //   · 只按 0x0409 写：stub 的清单是语言中立(0) 的，结果两条并存，
            //     加载器仍用旧那条 —— 产物照样 asInvoker；
            //   · 先删再写：UpdateResource 直接失败；
            //   · 删完再写两次：EndUpdateResource 失败。
            // 所以：探测语言 → 只覆盖这一条。
            var language = DetectManifestLanguage(exePath);

            var handle = BeginUpdateResource(exePath, false);
            if (handle == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(),
                    "无法打开 exe 资源（BeginUpdateResource）：" + exePath);
            }

            var ok = false;
            try
            {
                ok = UpdateResource(handle, RTManifest, 1, language, bytes, bytes.Length);
                if (!ok)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "写入清单资源失败（UpdateResource）");
                }
            }
            finally
            {
                // 第二个参数 discard=false 才会真正落盘
                if (!EndUpdateResource(handle, false))
                {
                    if (ok)
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error(), "保存资源失败（EndUpdateResource）");
                    }
                }
            }

            return ok;
        }

        /// <summary>读 exe 当前的清单文本（验收 / 自检用）；没有清单返回 null。</summary>
        public static string ReadManifest(string exePath)
        {
            var module = LoadLibraryEx(exePath, IntPtr.Zero, LoadLibraryAsDatafile);
            if (module == IntPtr.Zero)
            {
                return null;
            }

            try
            {
                IntPtr res;
                uint size;
                if (!FindResourceEx(module, RTManifest, 1, 0x0409, out res, out size) &&
                    !FindResourceEx(module, RTManifest, 1, 0, out res, out size))
                {
                    return null;
                }

                var ptr = LoadResource(module, res);
                if (ptr == IntPtr.Zero)
                {
                    return null;
                }

                var data = LockResource(ptr);
                if (data == IntPtr.Zero)
                {
                    return null;
                }

                var buffer = new byte[size];
                Marshal.Copy(data, buffer, 0, (int)size);

                // 清单可能是 UTF-8（带 BOM）或 ANSI；两种都试
                var text = Encoding.UTF8.GetString(buffer);
                return text.TrimStart('\uFEFF');
            }
            finally
            {
                FreeLibrary(module);
            }
        }

        /// <summary>
        /// 查这个 exe 的清单资源挂在哪个语言下（查不到返回 0 = 语言中立）。
        ///
        /// 必须知道它，否则写错语言就变成"两条清单并存"，加载器仍用旧的那条。
        /// </summary>
        private static ushort DetectManifestLanguage(string exePath)
        {
            var module = LoadLibraryEx(exePath, IntPtr.Zero, LoadLibraryAsDatafile);
            if (module == IntPtr.Zero)
            {
                return 0;
            }

            var found = (ushort)0;
            try
            {
                EnumResourceLanguages(module, RTManifest, 1, (hModule, type, name, language, lParam) =>
                {
                    found = unchecked((ushort)language);
                    return false;   // 拿到第一个就停
                }, IntPtr.Zero);
            }
            finally
            {
                FreeLibrary(module);
            }

            return found;
        }

        private delegate bool EnumResLangProc(IntPtr hModule, IntPtr lpType, IntPtr lpName, ushort wIDLanguage,
            IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool EnumResourceLanguages(IntPtr hModule, int lpType, int lpName,
            EnumResLangProc lpEnumFunc, IntPtr lParam);

        private const uint LoadLibraryAsDatafile = 0x00000002;

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr BeginUpdateResource(string pFileName, bool bDeleteExistingResources);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool UpdateResource(IntPtr hUpdate, int lpType, IntPtr lpName, ushort wLanguage,
            byte[] lpData, int cbData);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool UpdateResource(IntPtr hUpdate, int lpType, int lpName, ushort wLanguage,
            byte[] lpData, int cbData);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool EndUpdateResource(IntPtr hUpdate, bool fDiscard);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FindResourceEx(IntPtr hModule, int lpType, int lpName, ushort wLanguage,
            out IntPtr lpResource, out uint lpnSize);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LockResource(IntPtr hResData);
    }
}
