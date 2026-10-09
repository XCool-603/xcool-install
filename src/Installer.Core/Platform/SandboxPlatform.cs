using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Installer.Abstractions.Platform;
using Newtonsoft.Json;

namespace Installer.Core.Platform
{
    /// <summary>
    /// 沙箱注册表：把注册表写到一个 JSON 文件里，不碰真实系统。
    ///
    /// 用途：
    /// ① 端到端测试 —— 否则跑一次 e2e 就会在真实的「程序和功能」里留下垃圾；
    /// ② 便携安装 —— 需要把配置完全收进一个目录的场景。
    /// </summary>
    public sealed class SandboxRegistryStore : IRegistryStore
    {
        private readonly string _file;
        private readonly object _gate = new object();
        private Dictionary<string, Dictionary<string, string>> _data;

        /// <summary>构造。</summary>
        /// <param name="file">存储文件路径（registry.json）。</param>
        public SandboxRegistryStore(string file)
        {
            _file = file;
            _data = Load(file);
        }

        private static Dictionary<string, Dictionary<string, string>> Load(string file)
        {
            if (!File.Exists(file))
            {
                return new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
            }

            try
            {
                var json = File.ReadAllText(file, Encoding.UTF8);
                var d = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json);
                return d ?? new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
            }
            catch (JsonException)
            {
                return new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
            }
        }

        private void Save()
        {
            var dir = Path.GetDirectoryName(_file);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            File.WriteAllText(_file, JsonConvert.SerializeObject(_data, Formatting.Indented), new UTF8Encoding(false));
        }

        private static string Key(RegistryRoot hive, string subKey)
        {
            var h = hive == RegistryRoot.LocalMachine ? "HKLM" : "HKCU";
            return h + "\\" + (subKey ?? string.Empty).TrimEnd('\\');
        }

        /// <inheritdoc />
        public bool SubKeyExists(RegistryRoot hive, string subKey)
        {
            lock (_gate)
            {
                var k = Key(hive, subKey);
                return _data.ContainsKey(k);
            }
        }

        /// <inheritdoc />
        public IReadOnlyList<string> GetSubKeyNames(RegistryRoot hive, string subKey)
        {
            lock (_gate)
            {
                var prefix = Key(hive, subKey) + "\\";
                return _data.Keys
                    .Where(k => k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    .Select(k => k.Substring(prefix.Length).Split('\\')[0])
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }
        }

        /// <inheritdoc />
        public bool ValueExists(RegistryRoot hive, string subKey, string name)
        {
            lock (_gate)
            {
                Dictionary<string, string> values;
                return _data.TryGetValue(Key(hive, subKey), out values) && values.ContainsKey(name);
            }
        }

        /// <inheritdoc />
        public string GetString(RegistryRoot hive, string subKey, string name)
        {
            lock (_gate)
            {
                Dictionary<string, string> values;
                string v;
                if (_data.TryGetValue(Key(hive, subKey), out values) && values.TryGetValue(name, out v))
                {
                    return v;
                }

                return null;
            }
        }

        /// <inheritdoc />
        public int? GetDword(RegistryRoot hive, string subKey, string name)
        {
            var s = GetString(hive, subKey, name);
            int v;
            return int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out v) ? v : (int?)null;
        }

        /// <inheritdoc />
        public void SetString(RegistryRoot hive, string subKey, string name, string value)
        {
            lock (_gate)
            {
                var k = Key(hive, subKey);
                Dictionary<string, string> values;
                if (!_data.TryGetValue(k, out values))
                {
                    values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    _data[k] = values;
                }

                values[name] = value ?? string.Empty;
                Save();
            }
        }

        /// <inheritdoc />
        public void SetExpandString(RegistryRoot hive, string subKey, string name, string value)
        {
            SetString(hive, subKey, name, value);
        }

        /// <inheritdoc />
        public void SetDword(RegistryRoot hive, string subKey, string name, int value)
        {
            SetString(hive, subKey, name, value.ToString(CultureInfo.InvariantCulture));
        }

        /// <inheritdoc />
        public void DeleteValue(RegistryRoot hive, string subKey, string name)
        {
            lock (_gate)
            {
                Dictionary<string, string> values;
                if (_data.TryGetValue(Key(hive, subKey), out values))
                {
                    values.Remove(name);
                    Save();
                }
            }
        }

        /// <inheritdoc />
        public void DeleteSubKeyTree(RegistryRoot hive, string subKey)
        {
            lock (_gate)
            {
                var k = Key(hive, subKey);
                var doomed = _data.Keys
                    .Where(x => string.Equals(x, k, StringComparison.OrdinalIgnoreCase)
                                || x.StartsWith(k + "\\", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                foreach (var d in doomed)
                {
                    _data.Remove(d);
                }

                Save();
            }
        }
    }

    /// <summary>沙箱环境：把特殊文件夹重定向到一个根目录下。</summary>
    public sealed class SandboxEnvironmentInfo : IEnvironmentInfo
    {
        private readonly string _root;
        private readonly WindowsEnvironmentInfo _real = new WindowsEnvironmentInfo();

        /// <summary>构造。</summary>
        public SandboxEnvironmentInfo(string root)
        {
            _root = root;
        }

        /// <summary>沙箱根目录。</summary>
        public string Root
        {
            get { return _root; }
        }

        /// <inheritdoc />
        public string GetFolder(SpecialFolderKind kind)
        {
            string sub;
            switch (kind)
            {
                case SpecialFolderKind.Desktop:
                    sub = "Desktop";
                    break;
                case SpecialFolderKind.StartMenu:
                    sub = "StartMenu";
                    break;
                case SpecialFolderKind.Startup:
                    sub = "Startup";
                    break;
                case SpecialFolderKind.LocalApplicationData:
                    sub = "LocalAppData";
                    break;
                case SpecialFolderKind.RoamingApplicationData:
                    sub = "AppData";
                    break;
                case SpecialFolderKind.ProgramFiles:
                    sub = "ProgramFiles";
                    break;
                case SpecialFolderKind.Temp:
                    sub = "Temp";
                    break;
                case SpecialFolderKind.System:
                    // 系统目录**不**重定向：预检里"不许装进系统目录"的判断需要真实路径
                    return _real.GetFolder(kind);
                default:
                    return _real.GetFolder(kind);
            }

            var dir = Path.Combine(_root, sub);
            Directory.CreateDirectory(dir);
            return dir;
        }

        /// <inheritdoc />
        public bool IsAdministrator
        {
            get { return _real.IsAdministrator; }
        }

        /// <inheritdoc />
        public string ExpandEnvironmentVariables(string value)
        {
            return _real.ExpandEnvironmentVariables(value);
        }

        /// <inheritdoc />
        public long GetAvailableFreeSpace(string path)
        {
            return _real.GetAvailableFreeSpace(path);
        }

        /// <inheritdoc />
        public string OsDescription
        {
            get { return _real.OsDescription; }
        }
    }

    /// <summary>沙箱平台服务：注册表与特殊文件夹被重定向，其余仍是真实实现。</summary>
    public sealed class SandboxPlatformServices : WindowsPlatformServices
    {
        /// <summary>构造。</summary>
        public SandboxPlatformServices(string sandboxRoot, ILogger logger)
            : base(logger,
                   new SandboxRegistryStore(Path.Combine(sandboxRoot, "registry.json")),
                   new SandboxEnvironmentInfo(sandboxRoot))
        {
        }
    }
}
