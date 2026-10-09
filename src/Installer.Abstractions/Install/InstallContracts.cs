using System;
using System.Collections.Generic;
using System.IO;
using Installer.Abstractions.Model;
using Installer.Abstractions.Platform;

namespace Installer.Abstractions.Install
{
    /// <summary>安装模式。</summary>
    public enum InstallMode
    {
        /// <summary>全新安装。</summary>
        Install = 0,

        /// <summary>覆盖更新已有安装。</summary>
        Update = 1,

        /// <summary>修复安装（校验并补齐缺失/损坏的文件）。</summary>
        Repair = 2,
    }

    /// <summary>安装过程中的失败。带上失败的步骤 id，便于定位。</summary>
    public class InstallException : Exception
    {
        /// <summary>失败的步骤 id。</summary>
        public string StepId { get; private set; }

        /// <summary>构造。</summary>
        public InstallException(string stepId, string message)
            : base(message)
        {
            StepId = stepId;
        }

        /// <summary>构造。</summary>
        public InstallException(string stepId, string message, Exception inner)
            : base(message, inner)
        {
            StepId = stepId;
        }
    }

    /// <summary>步骤的执行结果。</summary>
    public sealed class StepResult
    {
        /// <summary>成功。</summary>
        public static readonly StepResult Ok = new StepResult(true, null);

        /// <summary>是否成功。</summary>
        public bool Success { get; private set; }

        /// <summary>失败原因（成功时为 null）。</summary>
        public string Message { get; private set; }

        private StepResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        /// <summary>构造一个失败结果。</summary>
        public static StepResult Fail(string message)
        {
            return new StepResult(false, message);
        }
    }

    /// <summary>安装进度。</summary>
    public sealed class InstallProgress
    {
        /// <summary>当前步骤的序号（从 1 开始）。</summary>
        public int StepIndex { get; set; }

        /// <summary>总步骤数。</summary>
        public int StepCount { get; set; }

        /// <summary>当前步骤 id。</summary>
        public string StepId { get; set; }

        /// <summary>给用户看的文案 key（走 <see cref="InstallContext.Strings"/> 本地化）。</summary>
        public string MessageKey { get; set; }

        /// <summary>已解析好的文案（没有 key 时用它）。</summary>
        public string Message { get; set; }

        /// <summary>0–100；未知为 -1。</summary>
        public int Percent { get; set; }

        /// <summary>是否处于回滚中。</summary>
        public bool IsRollingBack { get; set; }
    }

    /// <summary>
    /// 一个安装步骤。
    ///
    /// **唯一的硬性约定**：<see cref="Execute"/> 里每做一个写操作（建目录 / 写文件 /
    /// 写注册表 / 建快捷方式），就必须先往 <see cref="InstallContext.Journal"/> 记一条
    /// 可逆记录。
    ///
    /// 撤销**不**由步骤自己实现 —— 引擎会逆序遍历 journal 逐条 undo。
    /// 这样做的好处是：即使某个步骤执行到一半抛异常，已经落下的写操作照样能被精确撤销。
    /// 如果让每个步骤自己写 Rollback，就必然会出现"撤不干净"的步骤。
    /// </summary>
    public interface IInstallStep
    {
        /// <summary>稳定 id，用于日志、journal 与测试断言。</summary>
        string Id { get; }

        /// <summary>进度文案 key（对应清单里的 strings）。</summary>
        string MessageKey { get; }

        /// <summary>是否需要 payload。只做注册表/快捷方式的步骤可以返回 false。</summary>
        bool NeedsPayload { get; }

        /// <summary>执行。失败时抛 <see cref="InstallException"/>。</summary>
        void Execute(InstallContext context);
    }

    /// <summary>
    /// 安装上下文 —— 步骤需要的一切都从这里拿。
    /// 步骤**不允许**自己去读真实注册表或桌面路径，必须走 <see cref="Platform"/>，
    /// 否则沙箱模式下会污染真实系统、也无法测试。
    /// </summary>
    public sealed class InstallContext
    {
        /// <summary>清单。</summary>
        public InstallerManifest Manifest { get; set; }

        /// <summary>安装模式。</summary>
        public InstallMode Mode { get; set; }

        /// <summary>安装目录（绝对路径，已展开环境变量）。</summary>
        public string InstallDir { get; set; }

        /// <summary>payload（zip）流。由调用方负责生命周期。</summary>
        public Stream Payload { get; set; }

        /// <summary>
        /// stub 区长度（= 容器里 payloadOffset）。
        /// 部署卸载器时只拷贝自己文件的前这么多字节 —— 否则会把整个 payload 也拷一份。
        /// </summary>
        public long StubLength { get; set; }

        /// <summary>当前正在运行的可执行文件路径（安装器本体）。</summary>
        public string CurrentExecutablePath { get; set; }

        /// <summary>平台服务（注册表 / 快捷方式 / 进程 / 环境）。</summary>
        public IPlatformServices Platform { get; set; }

        /// <summary>回滚日志。</summary>
        public InstallJournal Journal { get; set; }

        /// <summary>日志。</summary>
        public ILogger Log { get; set; }

        /// <summary>多语言文案表（来自清单）。</summary>
        public Dictionary<string, LocalizedText> Strings { get; set; }

        /// <summary>当前语言解析器。</summary>
        public IStringTable Text { get; set; }

        /// <summary>安装时实际创建/覆盖的文件清单（用于写安装记录）。</summary>
        public List<string> InstalledFiles { get; set; } = new List<string>();

        /// <summary>安装时创建的快捷方式路径。</summary>
        public List<string> CreatedShortcuts { get; set; } = new List<string>();

        /// <summary>安装时写入的注册表值（供安装记录与卸载使用）。</summary>
        public List<RegistryValueRecord> WrittenRegistryValues { get; set; } = new List<RegistryValueRecord>();

        /// <summary>是否只校验不落盘（试运行）。</summary>
        public bool DryRun { get; set; }

        /// <summary>沙箱根目录（非沙箱安装为 null）。会被写进安装记录。</summary>
        public string SandboxRoot { get; set; }

        /// <summary>取消令牌。</summary>
        public System.Threading.CancellationToken CancellationToken { get; set; }

        /// <summary>取本地化文案。</summary>
        public string Resolve(string key, string fallback)
        {
            if (Text != null)
            {
                var v = Text.Get(key);
                if (!string.IsNullOrEmpty(v))
                {
                    return v;
                }
            }

            return fallback ?? key;
        }
    }

    /// <summary>一条注册表值记录（用于安装记录与卸载）。</summary>
    public sealed class RegistryValueRecord
    {
        /// <summary>配置单元。</summary>
        public string Hive { get; set; }

        /// <summary>子键路径。</summary>
        public string SubKey { get; set; }

        /// <summary>值名。</summary>
        public string Name { get; set; }

        /// <summary>值。</summary>
        public string Value { get; set; }
    }
}
