using System;
using System.Collections.Generic;
using System.IO;
using Installer.Abstractions.Platform;

namespace Installer.Core.Install
{
    /// <summary>找到的一个卸载键。</summary>
    public sealed class UninstallKeyRef
    {
        /// <summary>配置单元。</summary>
        public RegistryRoot Hive { get; set; }

        /// <summary>键名（通常就是 productCode）。</summary>
        public string KeyName { get; set; }

        /// <summary>显示名。</summary>
        public string DisplayName { get; set; }

        /// <summary>安装目录。</summary>
        public string InstallLocation { get; set; }

        /// <summary>卸载命令。</summary>
        public string UninstallString { get; set; }

        /// <summary>完整子键路径。</summary>
        public string SubKey
        {
            get { return UninstallKeys.SubKeyRoot + "\\" + KeyName; }
        }
    }

    /// <summary>
    /// 标准卸载键的查找与清理。
    ///
    /// **为什么需要这个**：制作端以前每次打开都生成一个新的 productCode，
    /// 同一个产品装三次就在控制面板里留下三条记录 —— 而它们指向同一个安装目录、同一个卸载器。
    /// 卸载掉其中一条之后，其余的因为安装记录已被删除而**再也卸不掉**。
    /// </summary>
    public static class UninstallKeys
    {
        /// <summary>标准卸载键的父路径。</summary>
        public const string SubKeyRoot = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

        /// <summary>要扫的配置单元（perUser 装 HKCU，perMachine 装 HKLM，卸载时要两个都看）。</summary>
        public static readonly RegistryRoot[] AllHives =
        {
            RegistryRoot.CurrentUser,
            RegistryRoot.LocalMachine,
        };

        /// <summary>列出某个配置单元下的全部卸载键。</summary>
        public static IList<UninstallKeyRef> List(IRegistryStore registry, RegistryRoot hive)
        {
            var result = new List<UninstallKeyRef>();

            IReadOnlyList<string> names;
            try
            {
                names = registry.GetSubKeyNames(hive, SubKeyRoot);
            }
            catch (Exception)
            {
                return result;
            }

            if (names == null)
            {
                return result;
            }

            foreach (var name in names)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }

                try
                {
                    result.Add(new UninstallKeyRef
                    {
                        Hive = hive,
                        KeyName = name,
                        DisplayName = registry.GetString(hive, SubKeyRoot + "\\" + name, "DisplayName"),
                        InstallLocation = registry.GetString(hive, SubKeyRoot + "\\" + name, "InstallLocation"),
                        UninstallString = registry.GetString(hive, SubKeyRoot + "\\" + name, "UninstallString"),
                    });
                }
                catch (Exception)
                {
                    // 个别键读不了不影响整体
                }
            }

            return result;
        }

        /// <summary>列出所有配置单元下的卸载键。</summary>
        public static IList<UninstallKeyRef> ListAll(IRegistryStore registry)
        {
            var all = new List<UninstallKeyRef>();
            foreach (var hive in AllHives)
            {
                all.AddRange(List(registry, hive));
            }

            return all;
        }

        /// <summary>两个路径是否指向同一个目录（忽略大小写、结尾反斜杠、环境变量）。</summary>
        public static bool SameDirectory(string a, string b)
        {
            if (string.IsNullOrWhiteSpace(a) || string.IsNullOrWhiteSpace(b))
            {
                return false;
            }

            var na = Normalize(a);
            var nb = Normalize(b);
            return string.Equals(na, nb, StringComparison.OrdinalIgnoreCase);
        }

        private static string Normalize(string path)
        {
            var expanded = Environment.ExpandEnvironmentVariables(path).Trim();
            try
            {
                expanded = Path.GetFullPath(expanded);
            }
            catch (Exception)
            {
                // 路径非法就按原样比
            }

            return expanded.TrimEnd('\\', '/');
        }

        /// <summary>
        /// 找出"同一个安装目录、但不是当前 productCode"的残留卸载键。
        ///
        /// 这就是控制面板里出现多条"同一个程序"的根因：productCode 变了，
        /// 但安装目录没变 —— 老键就成了孤儿。
        /// </summary>
        public static IList<UninstallKeyRef> FindOrphans(IRegistryStore registry, string installDir,
                                                         string currentProductCode)
        {
            var result = new List<UninstallKeyRef>();

            foreach (var key in ListAll(registry))
            {
                if (string.IsNullOrWhiteSpace(key.InstallLocation))
                {
                    continue;
                }

                if (!SameDirectory(key.InstallLocation, installDir))
                {
                    continue;
                }

                if (string.Equals(key.KeyName, currentProductCode, StringComparison.OrdinalIgnoreCase))
                {
                    continue;    // 自己那条，留着
                }

                result.Add(key);
            }

            return result;
        }

        /// <summary>
        /// 找出"同一个安装目录"的全部卸载键（不排除自己）。
        /// 卸载器在没有安装记录时用它来定位自己该删哪些键。
        /// </summary>
        public static IList<UninstallKeyRef> FindByInstallLocation(IRegistryStore registry, string installDir)
        {
            var result = new List<UninstallKeyRef>();

            foreach (var key in ListAll(registry))
            {
                if (!string.IsNullOrWhiteSpace(key.InstallLocation)
                    && SameDirectory(key.InstallLocation, installDir))
                {
                    result.Add(key);
                }
            }

            return result;
        }

        /// <summary>按键名（productCode）精确查找。</summary>
        public static UninstallKeyRef FindByProductCode(IRegistryStore registry, string productCode)
        {
            if (string.IsNullOrWhiteSpace(productCode))
            {
                return null;
            }

            foreach (var key in ListAll(registry))
            {
                if (string.Equals(key.KeyName, productCode, StringComparison.OrdinalIgnoreCase))
                {
                    return key;
                }
            }

            return null;
        }
    }
}
