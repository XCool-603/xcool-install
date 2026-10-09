using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Installer.Abstractions.Install
{
    /// <summary>journal 记录的操作类型。</summary>
    public static class JournalOps
    {
        /// <summary>创建了目录。</summary>
        public const string CreateDirectory = "CreateDirectory";

        /// <summary>写入了文件（可能是覆盖）。</summary>
        public const string WriteFile = "WriteFile";

        /// <summary>写了注册表值。</summary>
        public const string SetRegistryValue = "SetRegistryValue";

        /// <summary>创建了快捷方式。</summary>
        public const string CreateShortcut = "CreateShortcut";

        /// <summary>创建了注册表子键树。</summary>
        public const string CreateSubKey = "CreateSubKey";

        /// <summary>
        /// 注册了 COM 组件。
        /// 注意：DLL 自己往注册表里写的内容**无法逐条记录**，
        /// 所以撤销这一条的方式是反过来调用 DllUnregisterServer。
        /// </summary>
        public const string RegisterCom = "RegisterCom";
    }

    /// <summary>
    /// 一条可逆记录。
    ///
    /// 回滚规则：<see cref="ExistedBefore"/> 为 false 就删除，为 true 就还原
    /// （文件从 <see cref="BackupPath"/> 还原；注册表值还原成 <see cref="OldValue"/>）。
    /// </summary>
    public sealed class JournalEntry
    {
        /// <summary>产生这条记录的步骤 id。</summary>
        [JsonProperty("step")]
        public string StepId { get; set; }

        /// <summary>操作类型，见 <see cref="JournalOps"/>。</summary>
        [JsonProperty("op")]
        public string Op { get; set; }

        /// <summary>目标路径（文件 / 目录 / 快捷方式）。</summary>
        [JsonProperty("target", NullValueHandling = NullValueHandling.Ignore)]
        public string Target { get; set; }

        /// <summary>注册表配置单元（HKCU / HKLM）。</summary>
        [JsonProperty("hive", NullValueHandling = NullValueHandling.Ignore)]
        public string Hive { get; set; }

        /// <summary>注册表子键路径。</summary>
        [JsonProperty("subKey", NullValueHandling = NullValueHandling.Ignore)]
        public string SubKey { get; set; }

        /// <summary>注册表值名。</summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        /// <summary>操作之前目标是否已存在。</summary>
        [JsonProperty("existedBefore")]
        public bool ExistedBefore { get; set; }

        /// <summary>被覆盖内容的备份路径（仅文件）。</summary>
        [JsonProperty("backup", NullValueHandling = NullValueHandling.Ignore)]
        public string BackupPath { get; set; }

        /// <summary>注册表旧值。</summary>
        [JsonProperty("oldValue", NullValueHandling = NullValueHandling.Ignore)]
        public string OldValue { get; set; }

        /// <summary>附加信息（例如 COM 注册方式）。</summary>
        [JsonProperty("extra", NullValueHandling = NullValueHandling.Ignore)]
        public string Extra { get; set; }
    }

    /// <summary>
    /// 回滚日志。
    ///
    /// 存在的意义：安装中途失败（或用户取消）时能**逆序撤销**已经做的写操作，
    /// 不留半成品。现状完全没有这个能力 —— 失败就留下一个坏掉的安装。
    ///
    /// 每写一条就落盘一次（jsonl），所以即使进程被杀，下次启动也能看到未提交的 journal。
    /// </summary>
    public sealed class InstallJournal
    {
        private readonly List<JournalEntry> _entries = new List<JournalEntry>();
        private readonly string _journalPath;
        private readonly string _backupDir;
        private int _backupCounter;

        /// <summary>构造一个内存 journal（不落盘）。</summary>
        public InstallJournal()
        {
        }

        /// <summary>构造一个落盘 journal。</summary>
        /// <param name="journalPath">journal 文件路径（jsonl）。</param>
        /// <param name="backupDir">被覆盖文件的备份目录。</param>
        public InstallJournal(string journalPath, string backupDir)
        {
            _journalPath = journalPath;
            _backupDir = backupDir;
        }

        /// <summary>已记录的可逆操作（按发生顺序）。</summary>
        public IList<JournalEntry> Entries
        {
            get { return _entries; }
        }

        /// <summary>journal 文件路径（内存模式为 null）。</summary>
        public string Path
        {
            get { return _journalPath; }
        }

        /// <summary>备份目录（内存模式为 null）。</summary>
        public string BackupDir
        {
            get { return _backupDir; }
        }

        /// <summary>记录一条，并立即落盘。</summary>
        public void Record(JournalEntry entry)
        {
            if (entry == null)
            {
                throw new ArgumentNullException("entry");
            }

            _entries.Add(entry);
            AppendToDisk(entry);
        }

        /// <summary>把被覆盖的文件备份起来，返回备份路径；文件不存在时返回 null。</summary>
        public string BackupFile(string filePath)
        {
            if (string.IsNullOrEmpty(_backupDir) || !File.Exists(filePath))
            {
                return null;
            }

            Directory.CreateDirectory(_backupDir);
            var backup = System.IO.Path.Combine(_backupDir, (++_backupCounter).ToString("D6") + ".bak");
            File.Copy(filePath, backup, true);
            return backup;
        }

        /// <summary>提交：删除 journal 与备份，表示安装已成功。</summary>
        public void Commit()
        {
            if (!string.IsNullOrEmpty(_journalPath) && File.Exists(_journalPath))
            {
                try
                {
                    File.Delete(_journalPath);
                }
                catch (IOException)
                {
                }
            }

            if (!string.IsNullOrEmpty(_backupDir) && Directory.Exists(_backupDir))
            {
                try
                {
                    Directory.Delete(_backupDir, true);
                }
                catch (IOException)
                {
                }
            }
        }

        /// <summary>从磁盘读回一个未提交的 journal（崩溃恢复用）。</summary>
        public static List<JournalEntry> LoadPending(string journalPath)
        {
            var list = new List<JournalEntry>();
            if (string.IsNullOrEmpty(journalPath) || !File.Exists(journalPath))
            {
                return list;
            }

            foreach (var line in File.ReadAllLines(journalPath, Encoding.UTF8))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                try
                {
                    var e = JsonConvert.DeserializeObject<JournalEntry>(line);
                    if (e != null)
                    {
                        list.Add(e);
                    }
                }
                catch (JsonException)
                {
                    // 半截行（进程被杀）忽略
                }
            }

            return list;
        }

        private void AppendToDisk(JournalEntry entry)
        {
            if (string.IsNullOrEmpty(_journalPath))
            {
                return;
            }

            var dir = System.IO.Path.GetDirectoryName(_journalPath);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var line = JsonConvert.SerializeObject(entry, Formatting.None);
            File.AppendAllText(_journalPath, line + Environment.NewLine, new UTF8Encoding(false));
        }
    }
}
