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
        }

        /// <inheritdoc />
        public void Delete(string linkPath)
        {
            if (File.Exists(linkPath))
            {
                File.Delete(linkPath);
            }
        }

        /// <inheritdoc />
        public bool Exists(string linkPath)
        {
            return File.Exists(linkPath);
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
            catch (Exception)
            {
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
