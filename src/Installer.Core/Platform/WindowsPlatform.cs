using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using Installer.Abstractions.Platform;
using Microsoft.Win32;

namespace Installer.Core.Platform
{
    /// <summary>真实的 Windows 注册表实现。</summary>
    public sealed class WindowsRegistryStore : IRegistryStore
    {
        private static RegistryKey OpenBase(RegistryRoot hive)
        {
            return hive == RegistryRoot.LocalMachine ? Registry.LocalMachine : Registry.CurrentUser;
        }

        private static RegistryKey OpenWrite(RegistryRoot hive, string subKey, bool create)
        {
            return create
                ? OpenBase(hive).CreateSubKey(subKey)
                : OpenBase(hive).OpenSubKey(subKey, true);
        }

        /// <inheritdoc />
        public bool SubKeyExists(RegistryRoot hive, string subKey)
        {
            using (var k = OpenBase(hive).OpenSubKey(subKey))
            {
                return k != null;
            }
        }

        /// <inheritdoc />
        public IReadOnlyList<string> GetSubKeyNames(RegistryRoot hive, string subKey)
        {
            using (var k = OpenBase(hive).OpenSubKey(subKey))
            {
                return k == null ? new List<string>() : k.GetSubKeyNames().ToList();
            }
        }

        /// <inheritdoc />
        public bool ValueExists(RegistryRoot hive, string subKey, string name)
        {
            using (var k = OpenBase(hive).OpenSubKey(subKey))
            {
                return k != null && k.GetValue(name) != null;
            }
        }

        /// <inheritdoc />
        public string GetString(RegistryRoot hive, string subKey, string name)
        {
            using (var k = OpenBase(hive).OpenSubKey(subKey))
            {
                var v = k == null ? null : k.GetValue(name);
                return v == null ? null : Convert.ToString(v, CultureInfo.InvariantCulture);
            }
        }

        /// <inheritdoc />
        public int? GetDword(RegistryRoot hive, string subKey, string name)
        {
            using (var k = OpenBase(hive).OpenSubKey(subKey))
            {
                var v = k == null ? null : k.GetValue(name);
                return v == null ? (int?)null : Convert.ToInt32(v, CultureInfo.InvariantCulture);
            }
        }

        /// <inheritdoc />
        public void SetString(RegistryRoot hive, string subKey, string name, string value)
        {
            using (var k = OpenWrite(hive, subKey, true))
            {
                k.SetValue(name, value ?? string.Empty, RegistryValueKind.String);
            }
        }

        /// <inheritdoc />
        public void SetExpandString(RegistryRoot hive, string subKey, string name, string value)
        {
            using (var k = OpenWrite(hive, subKey, true))
            {
                k.SetValue(name, value ?? string.Empty, RegistryValueKind.ExpandString);
            }
        }

        /// <inheritdoc />
        public void SetDword(RegistryRoot hive, string subKey, string name, int value)
        {
            using (var k = OpenWrite(hive, subKey, true))
            {
                k.SetValue(name, value, RegistryValueKind.DWord);
            }
        }

        /// <inheritdoc />
        public void DeleteValue(RegistryRoot hive, string subKey, string name)
        {
            using (var k = OpenWrite(hive, subKey, false))
            {
                if (k != null && k.GetValue(name) != null)
                {
                    k.DeleteValue(name, false);
                }
            }
        }

        /// <inheritdoc />
        public void DeleteSubKeyTree(RegistryRoot hive, string subKey)
        {
            using (var k = OpenWrite(hive, subKey, false))
            {
                if (k == null)
                {
                    return;
                }
            }

            try
            {
                OpenBase(hive).DeleteSubKeyTree(subKey, false);
            }
            catch (ArgumentException)
            {
                // 已经不存在
            }
        }
    }

    /// <summary>真实的快捷方式实现（走 IShellLink）。</summary>
    public sealed class WindowsShortcutService : IShortcutService
    {
        // 外壳通知：建完快捷方式必须告诉 Explorer，否则开始菜单「所有应用」不会收录它。
        private const int SHCNE_CREATE = 0x00000002;
        private const int SHCNE_DELETE = 0x00000004;
        private const int SHCNE_UPDATEDIR = 0x00002000;
        private const int SHCNE_ASSOCCHANGED = unchecked((int)0x08000000);
        private const uint SHCNF_PATH = 0x0001;
        private const uint SHCNF_IDLIST = 0x0000;
        private const uint SHCNF_FLUSH = 0x1000;

        [System.Runtime.InteropServices.DllImport("shell32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        private static extern void SHChangeNotify(int eventId, uint flags, string item1, string item2);

        /// <summary>
        /// 参数为 IDLIST 版本（不传具体条目，让外壳整体刷新）。
        /// </summary>
        [System.Runtime.InteropServices.DllImport("shell32.dll", EntryPoint = "SHChangeNotify",
            CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        private static extern void SHChangeNotifyIdList(int eventId, uint flags, IntPtr item1, IntPtr item2);

        /// <inheritdoc />
        public void Create(string linkPath, string targetPath, string arguments, string workingDirectory,
                           string description, string iconPath)
        {
            var dir = Path.GetDirectoryName(linkPath);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            ShellLink.Create(linkPath, targetPath, arguments, workingDirectory, description, iconPath);

            // 精确通知：这个文件建好了 + 它所在的目录变了。
            // 只发 ASSOCCHANGED 时实测**不可靠** —— 开始菜单「所有应用」可能一直不收录新条目
            // （用户反馈"目录里有、菜单里没有"就是这件事）。目录也在变，所以两条都发。
            Notify(SHCNE_CREATE, linkPath);
            Notify(SHCNE_UPDATEDIR, dir);
            NotifyAll();
        }

        /// <inheritdoc />
        public void Delete(string linkPath)
        {
            if (!File.Exists(linkPath))
            {
                return;
            }

            File.Delete(linkPath);
            Notify(SHCNE_DELETE, linkPath);
            Notify(SHCNE_UPDATEDIR, Path.GetDirectoryName(linkPath));
        }

        /// <inheritdoc />
        public bool Exists(string linkPath)
        {
            return File.Exists(linkPath);
        }

        private static void Notify(int eventId, string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            try
            {
                SHChangeNotify(eventId, SHCNF_PATH | SHCNF_FLUSH, path, null);
            }
            catch (Exception)
            {
                // 通知失败不影响安装：外壳迟早会自己发现，最多晚一点显示
            }
        }

        /// <summary>再补一条"整体刷新"，覆盖只认 ASSOCCHANGED 的场景。</summary>
        private static void NotifyAll()
        {
            try
            {
                SHChangeNotifyIdList(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);
            }
            catch (Exception)
            {
            }
        }
    }

    /// <summary>真实的进程操作。</summary>
    public sealed class WindowsProcessRunner : IProcessRunner
    {
        /// <inheritdoc />
        public IReadOnlyList<string> GetRunningProcessNames()
        {
            var names = new List<string>();
            foreach (var p in Process.GetProcesses())
            {
                try
                {
                    names.Add(p.ProcessName);
                }
                catch (InvalidOperationException)
                {
                }
                finally
                {
                    p.Dispose();
                }
            }

            return names;
        }

        /// <inheritdoc />
        public int Kill(string processName)
        {
            var killed = 0;
            foreach (var p in Process.GetProcessesByName(processName))
            {
                try
                {
                    p.Kill();
                    p.WaitForExit(5000);
                    killed++;
                }
                catch (Exception)
                {
                    // 权限不足或已退出
                }
                finally
                {
                    p.Dispose();
                }
            }

            return killed;
        }

        /// <inheritdoc />
        public int Run(string fileName, string arguments, bool waitForExit, out string output)
        {
            var psi = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments ?? string.Empty,
                UseShellExecute = true,
                WorkingDirectory = Path.GetDirectoryName(fileName) ?? Environment.CurrentDirectory,
            };

            output = null;
            using (var p = Process.Start(psi))
            {
                if (p == null)
                {
                    return -1;
                }

                if (waitForExit)
                {
                    p.WaitForExit();
                    return p.ExitCode;
                }

                return 0;
            }
        }
    }

    /// <summary>真实的环境信息。</summary>
    public class WindowsEnvironmentInfo : IEnvironmentInfo
    {
        /// <inheritdoc />
        public virtual string GetFolder(SpecialFolderKind kind)
        {
            switch (kind)
            {
                case SpecialFolderKind.Desktop:
                    return Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                case SpecialFolderKind.StartMenu:
                    return Environment.GetFolderPath(Environment.SpecialFolder.Programs);
                case SpecialFolderKind.Startup:
                    return Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                case SpecialFolderKind.LocalApplicationData:
                    return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                case SpecialFolderKind.RoamingApplicationData:
                    return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                case SpecialFolderKind.ProgramFiles:
                    return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                case SpecialFolderKind.ProgramFilesX86:
                    {
                        // 32 位安装器拿到的 ProgramFiles 可能被 WOW64 重定向，
                        // 这里优先用显式的 (x86) 目录，取不到再退回 ProgramFiles。
                        var x86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
                        return string.IsNullOrEmpty(x86)
                            ? Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
                            : x86;
                    }

                case SpecialFolderKind.CommonDesktop:
                    return Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);
                case SpecialFolderKind.CommonStartMenu:
                    return Environment.GetFolderPath(Environment.SpecialFolder.CommonPrograms);
                case SpecialFolderKind.CommonStartup:
                    return Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup);
                case SpecialFolderKind.System:
                    return Environment.GetFolderPath(Environment.SpecialFolder.System);
                case SpecialFolderKind.Temp:
                    return Path.GetTempPath();
                default:
                    return null;
            }
        }

        /// <inheritdoc />
        public bool IsAdministrator
        {
            get
            {
                try
                {
                    using (var identity = WindowsIdentity.GetCurrent())
                    {
                        return new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator);
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        /// <inheritdoc />
        public string ExpandEnvironmentVariables(string value)
        {
            return string.IsNullOrEmpty(value) ? value : Environment.ExpandEnvironmentVariables(value);
        }

        /// <inheritdoc />
        public long GetAvailableFreeSpace(string path)
        {
            try
            {
                var root = Path.GetPathRoot(Path.GetFullPath(path));
                if (string.IsNullOrEmpty(root))
                {
                    return -1;
                }

                foreach (var d in DriveInfo.GetDrives())
                {
                    if (!d.IsReady)
                    {
                        continue;
                    }

                    if (string.Equals(d.Name.TrimEnd('\\'), root.TrimEnd('\\'), StringComparison.OrdinalIgnoreCase))
                    {
                        return d.AvailableFreeSpace;
                    }
                }
            }
            catch (System.IO.IOException)
            {
                // 盘符不存在、未就绪、或读取中途被拔掉 —— 返回 -1 表示"未知"，
                // 调用方（预检）会据此跳过空间检查，而不是当成"空间不足"。
            }
            catch (UnauthorizedAccessException)
            {
                // 权限不足同样按"未知"处理
            }

            return -1;
        }

        /// <inheritdoc />
        public string OsDescription
        {
            get { return Environment.OSVersion.ToString(); }
        }
    }

    /// <summary>把上述实现组装成一套平台服务。</summary>
    public class WindowsPlatformServices : IPlatformServices
    {
        private readonly ILogger _logger;

        /// <summary>用真实实现构造。</summary>
        public WindowsPlatformServices(ILogger logger)
        {
            _logger = logger ?? NullLogger.Instance;
            Registry = new WindowsRegistryStore();
            Shortcuts = new WindowsShortcutService();
            Processes = new WindowsProcessRunner();
            Environment = new WindowsEnvironmentInfo();
        }

        /// <summary>允许子类替换个别实现（沙箱模式用）。</summary>
        protected WindowsPlatformServices(ILogger logger, IRegistryStore registry, IEnvironmentInfo environment)
        {
            _logger = logger ?? NullLogger.Instance;
            Registry = registry;
            Shortcuts = new WindowsShortcutService();
            Processes = new WindowsProcessRunner();
            Environment = environment;
        }

        /// <inheritdoc />
        public IRegistryStore Registry { get; protected set; }

        /// <inheritdoc />
        public IShortcutService Shortcuts { get; protected set; }

        /// <inheritdoc />
        public IProcessRunner Processes { get; protected set; }

        /// <inheritdoc />
        public IEnvironmentInfo Environment { get; protected set; }

        /// <inheritdoc />
        public ILogger Log
        {
            get { return _logger; }
        }
    }
}
