using System;
using System.Collections.Generic;
using Installer.Abstractions.Model;

namespace Installer.Abstractions.Platform
{
    /// <summary>特殊文件夹种类（与 <see cref="Environment.SpecialFolder"/> 对齐，但可被沙箱重定向）。</summary>
    public enum SpecialFolderKind
    {
        /// <summary>桌面。</summary>
        Desktop = 0,

        /// <summary>开始菜单。</summary>
        StartMenu = 1,

        /// <summary>「启动」文件夹（开机自启）。</summary>
        Startup = 2,

        /// <summary>%LOCALAPPDATA%。</summary>
        LocalApplicationData = 3,

        /// <summary>%APPDATA%。</summary>
        RoamingApplicationData = 4,

        /// <summary>%ProgramFiles%。</summary>
        ProgramFiles = 5,

        /// <summary>%SystemRoot%\System32。</summary>
        System = 6,

        /// <summary>%TEMP%。</summary>
        Temp = 7,
    }

    /// <summary>注册表配置单元。</summary>
    public enum RegistryRoot
    {
        /// <summary>HKEY_CURRENT_USER。</summary>
        CurrentUser = 0,

        /// <summary>HKEY_LOCAL_MACHINE。</summary>
        LocalMachine = 1,
    }

    /// <summary>极简日志接口。安装过程必须可追溯 —— 现状全靠空 catch。</summary>
    public interface ILogger
    {
        /// <summary>普通信息。</summary>
        void Info(string message);

        /// <summary>警告。</summary>
        void Warn(string message);

        /// <summary>错误。</summary>
        void Error(string message, Exception exception = null);
    }

    /// <summary>多语言文案解析。</summary>
    public interface IStringTable
    {
        /// <summary>当前语言。</summary>
        string Culture { get; }

        /// <summary>按 key 取文案；找不到返回 null。</summary>
        string Get(string key);

        /// <summary>
        /// 解析一段多语言文本（用于 <c>product.name</c> / <c>publisher</c> / 许可协议这类
        /// 不在 strings 表里、但同样多语言的字段）。
        /// </summary>
        /// <param name="text">多语言文本。</param>
        /// <param name="fallback">全都匹配不到时的兜底值。</param>
        string Resolve(Installer.Abstractions.Model.LocalizedText text, string fallback);

        /// <summary>可用语言。</summary>
        IReadOnlyList<string> AvailableCultures { get; }
    }

    /// <summary>注册表访问抽象（可被沙箱替换成文件实现，便于测试）。</summary>
    public interface IRegistryStore
    {
        /// <summary>子键是否存在。</summary>
        bool SubKeyExists(RegistryRoot hive, string subKey);

        /// <summary>枚举子键名。</summary>
        IReadOnlyList<string> GetSubKeyNames(RegistryRoot hive, string subKey);

        /// <summary>值是否存在。</summary>
        bool ValueExists(RegistryRoot hive, string subKey, string name);

        /// <summary>读字符串值。</summary>
        string GetString(RegistryRoot hive, string subKey, string name);

        /// <summary>读 DWORD 值。</summary>
        int? GetDword(RegistryRoot hive, string subKey, string name);

        /// <summary>写字符串值。</summary>
        void SetString(RegistryRoot hive, string subKey, string name, string value);

        /// <summary>写可展开字符串值（REG_EXPAND_SZ）。</summary>
        void SetExpandString(RegistryRoot hive, string subKey, string name, string value);

        /// <summary>写 DWORD 值。</summary>
        void SetDword(RegistryRoot hive, string subKey, string name, int value);

        /// <summary>删除值。</summary>
        void DeleteValue(RegistryRoot hive, string subKey, string name);

        /// <summary>递归删除子键。</summary>
        void DeleteSubKeyTree(RegistryRoot hive, string subKey);
    }

    /// <summary>快捷方式创建/删除抽象。</summary>
    public interface IShortcutService
    {
        /// <summary>创建 .lnk。</summary>
        void Create(string linkPath, string targetPath, string arguments, string workingDirectory,
                    string description, string iconPath);

        /// <summary>删除 .lnk。</summary>
        void Delete(string linkPath);

        /// <summary>是否存在。</summary>
        bool Exists(string linkPath);
    }

    /// <summary>进程操作抽象。</summary>
    public interface IProcessRunner
    {
        /// <summary>按进程名（不含 .exe）枚举正在运行的进程名。</summary>
        IReadOnlyList<string> GetRunningProcessNames();

        /// <summary>结束指定进程名的全部进程；返回结束的数量。</summary>
        int Kill(string processName);

        /// <summary>启动进程并等待；返回退出码。stdout/stderr 会被读掉，避免管道死锁。</summary>
        int Run(string fileName, string arguments, bool waitForExit, out string output);
    }

    /// <summary>环境信息抽象。</summary>
    public interface IEnvironmentInfo
    {
        /// <summary>取特殊文件夹路径（沙箱模式下会被重定向）。</summary>
        string GetFolder(SpecialFolderKind kind);

        /// <summary>当前进程是否以管理员运行。</summary>
        bool IsAdministrator { get; }

        /// <summary>展开 %ENV% 变量。</summary>
        string ExpandEnvironmentVariables(string value);

        /// <summary>目标路径所在卷的可用空间（字节）；取不到返回 -1。</summary>
        long GetAvailableFreeSpace(string path);

        /// <summary>操作系统版本描述。</summary>
        string OsDescription { get; }
    }

    /// <summary>平台服务的集合。步骤只允许通过这些接口触碰系统。</summary>
    public interface IPlatformServices
    {
        /// <summary>注册表。</summary>
        IRegistryStore Registry { get; }

        /// <summary>快捷方式。</summary>
        IShortcutService Shortcuts { get; }

        /// <summary>进程。</summary>
        IProcessRunner Processes { get; }

        /// <summary>环境。</summary>
        IEnvironmentInfo Environment { get; }

        /// <summary>日志。</summary>
        ILogger Log { get; }
    }

    /// <summary>什么都不做的日志实现（测试与默认值用）。</summary>
    public sealed class NullLogger : ILogger
    {
        /// <summary>单例。</summary>
        public static readonly NullLogger Instance = new NullLogger();

        /// <inheritdoc />
        public void Info(string message)
        {
        }

        /// <inheritdoc />
        public void Warn(string message)
        {
        }

        /// <inheritdoc />
        public void Error(string message, Exception exception = null)
        {
        }
    }
}
