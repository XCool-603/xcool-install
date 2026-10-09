using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Installer.Abstractions.Model;
using Installer.Abstractions.Platform;

namespace Installer.Core.Tests
{
    /// <summary>内存注册表 —— 让安装步骤可以在没有真实系统的前提下被断言。</summary>
    public sealed class FakeRegistryStore : IRegistryStore
    {
        private readonly Dictionary<string, Dictionary<string, string>> _data =
            new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

        private static string Key(RegistryRoot hive, string subKey)
        {
            return (hive == RegistryRoot.LocalMachine ? "HKLM" : "HKCU") + "\\" + (subKey ?? string.Empty).TrimEnd('\\');
        }

        /// <summary>测试断言用：全部键。</summary>
        public IReadOnlyList<string> AllKeys
        {
            get { return _data.Keys.ToList(); }
        }

        /// <inheritdoc />
        public bool SubKeyExists(RegistryRoot hive, string subKey)
        {
            return _data.ContainsKey(Key(hive, subKey));
        }

        /// <inheritdoc />
        public IReadOnlyList<string> GetSubKeyNames(RegistryRoot hive, string subKey)
        {
            var prefix = Key(hive, subKey) + "\\";
            return _data.Keys.Where(k => k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .Select(k => k.Substring(prefix.Length).Split('\\')[0])
                .Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        }

        /// <inheritdoc />
        public bool ValueExists(RegistryRoot hive, string subKey, string name)
        {
            Dictionary<string, string> v;
            return _data.TryGetValue(Key(hive, subKey), out v) && v.ContainsKey(name);
        }

        /// <inheritdoc />
        public string GetString(RegistryRoot hive, string subKey, string name)
        {
            Dictionary<string, string> v;
            string s;
            return _data.TryGetValue(Key(hive, subKey), out v) && v.TryGetValue(name, out s) ? s : null;
        }

        /// <inheritdoc />
        public int? GetDword(RegistryRoot hive, string subKey, string name)
        {
            int v;
            return int.TryParse(GetString(hive, subKey, name), out v) ? v : (int?)null;
        }

        /// <inheritdoc />
        public void SetString(RegistryRoot hive, string subKey, string name, string value)
        {
            var k = Key(hive, subKey);
            Dictionary<string, string> v;
            if (!_data.TryGetValue(k, out v))
            {
                v = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                _data[k] = v;
            }

            v[name] = value ?? string.Empty;
        }

        /// <inheritdoc />
        public void SetExpandString(RegistryRoot hive, string subKey, string name, string value)
        {
            SetString(hive, subKey, name, value);
        }

        /// <inheritdoc />
        public void SetDword(RegistryRoot hive, string subKey, string name, int value)
        {
            SetString(hive, subKey, name, value.ToString());
        }

        /// <inheritdoc />
        public void DeleteValue(RegistryRoot hive, string subKey, string name)
        {
            Dictionary<string, string> v;
            if (_data.TryGetValue(Key(hive, subKey), out v))
            {
                v.Remove(name);
            }
        }

        /// <inheritdoc />
        public void DeleteSubKeyTree(RegistryRoot hive, string subKey)
        {
            var k = Key(hive, subKey);
            foreach (var d in _data.Keys.Where(x => string.Equals(x, k, StringComparison.OrdinalIgnoreCase)
                                                    || x.StartsWith(k + "\\", StringComparison.OrdinalIgnoreCase)).ToList())
            {
                _data.Remove(d);
            }
        }
    }

    /// <summary>内存快捷方式。</summary>
    public sealed class FakeShortcutService : IShortcutService
    {
        /// <summary>已创建的 .lnk。</summary>
        public HashSet<string> Links { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>记录每次创建的目标。</summary>
        public Dictionary<string, string> Targets { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <inheritdoc />
        public void Create(string linkPath, string targetPath, string arguments, string workingDirectory,
                           string description, string iconPath)
        {
            Links.Add(linkPath);
            Targets[linkPath] = targetPath;
        }

        /// <inheritdoc />
        public void Delete(string linkPath)
        {
            Links.Remove(linkPath);
        }

        /// <inheritdoc />
        public bool Exists(string linkPath)
        {
            return Links.Contains(linkPath);
        }
    }

    /// <summary>假进程操作。</summary>
    public sealed class FakeProcessRunner : IProcessRunner
    {
        /// <summary>模拟正在运行的进程名。</summary>
        public HashSet<string> Running { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>启动过的程序。</summary>
        public List<string> Started { get; } = new List<string>();

        /// <inheritdoc />
        public IReadOnlyList<string> GetRunningProcessNames()
        {
            return Running.ToList();
        }

        /// <inheritdoc />
        public int Kill(string processName)
        {
            return Running.Remove(processName) ? 1 : 0;
        }

        /// <inheritdoc />
        public int Run(string fileName, string arguments, bool waitForExit, out string output)
        {
            Started.Add(fileName);
            output = null;
            return 0;
        }
    }

    /// <summary>假环境：所有特殊文件夹都指向一个临时根。</summary>
    public sealed class FakeEnvironmentInfo : IEnvironmentInfo
    {
        /// <summary>构造。</summary>
        public FakeEnvironmentInfo(string root)
        {
            Root = root;
            Directory.CreateDirectory(root);
        }

        /// <summary>根目录。</summary>
        public string Root { get; private set; }

        /// <summary>模拟可用空间；-1 表示未知。</summary>
        public long FreeSpace { get; set; } = -1;

        /// <inheritdoc />
        public string GetFolder(SpecialFolderKind kind)
        {
            var dir = Path.Combine(Root, kind.ToString());
            Directory.CreateDirectory(dir);
            return dir;
        }

        /// <inheritdoc />
        public bool IsAdministrator
        {
            get { return false; }
        }

        /// <inheritdoc />
        public string ExpandEnvironmentVariables(string value)
        {
            return value;
        }

        /// <inheritdoc />
        public long GetAvailableFreeSpace(string path)
        {
            return FreeSpace;
        }

        /// <inheritdoc />
        public string OsDescription
        {
            get { return "fake"; }
        }
    }

    /// <summary>把假实现打包成一套平台服务。</summary>
    public sealed class FakePlatform : IPlatformServices
    {
        /// <summary>构造。</summary>
        public FakePlatform(string root)
        {
            Registry = new FakeRegistryStore();
            Shortcuts = new FakeShortcutService();
            Processes = new FakeProcessRunner();
            Environment = new FakeEnvironmentInfo(root);
            Log = new NullLogger();
        }

        /// <inheritdoc />
        public IRegistryStore Registry { get; private set; }

        /// <inheritdoc />
        public IShortcutService Shortcuts { get; private set; }

        /// <inheritdoc />
        public IProcessRunner Processes { get; private set; }

        /// <inheritdoc />
        public IEnvironmentInfo Environment { get; private set; }

        /// <inheritdoc />
        public ILogger Log { get; private set; }
    }

    /// <summary>测试用的清单构造器。</summary>
    public static class TestManifest
    {
        /// <summary>造一个最小可用清单。</summary>
        public static InstallerManifest Create(string version = "1.0.0", string scope = InstallScopes.PerUser)
        {
            return new InstallerManifest
            {
                SchemaVersion = 2,
                ProductCode = "{11111111-2222-3333-4444-555555555555}",
                DefaultCulture = "zh-Hans",
                Cultures = new List<string> { "zh-Hans", "en" },
                Product = new ProductInfo
                {
                    Name = new LocalizedText { { "zh-Hans", "测试产品" }, { "en", "Test Product" } },
                    Version = version,
                    Publisher = new LocalizedText { { "zh-Hans", "唯非工作室" } },
                },
                Scope = scope,
                DefaultInstallDir = @"%LOCALAPPDATA%\Programs\TestProduct",
                EntryPoint = @"bin\App.exe",
                Files = new List<FileEntry>
                {
                    new FileEntry { Path = @"bin\App.exe", Size = 10, Sha256 = new string('a', 64) },
                },
                Strings = new Dictionary<string, LocalizedText>(StringComparer.Ordinal)
                {
                    { "Install.Preflight", new LocalizedText { { "zh-Hans", "正在检查" }, { "en", "Checking" } } },
                },
            };
        }
    }
}
