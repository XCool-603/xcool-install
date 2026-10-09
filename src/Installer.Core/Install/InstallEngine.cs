using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Installer.Abstractions.Install;
using Installer.Abstractions.Platform;
using Installer.Core.Security;

namespace Installer.Core.Install
{
    /// <summary>安装结果。</summary>
    public sealed class InstallRunResult
    {
        /// <summary>是否成功。</summary>
        public bool Success { get; set; }

        /// <summary>是否发生了回滚。</summary>
        public bool RolledBack { get; set; }

        /// <summary>失败的步骤 id。</summary>
        public string FailedStepId { get; set; }

        /// <summary>失败原因。</summary>
        public string ErrorMessage { get; set; }

        /// <summary>回滚过程中的问题（回滚是尽力而为）。</summary>
        public List<string> RollbackIssues { get; set; } = new List<string>();

        /// <summary>实际执行的步骤数。</summary>
        public int StepsExecuted { get; set; }

        /// <summary>总步骤数。</summary>
        public int StepCount { get; set; }

        /// <summary>日志文案。</summary>
        public List<string> Messages { get; set; } = new List<string>();
    }

    /// <summary>
    /// 安装引擎。
    ///
    /// 流程：逐步执行 → 每步的写操作都进 journal → 全部成功则提交（删 journal）
    /// → 任一步失败或取消则**逆序 undo journal**。
    ///
    /// 现状对比：<c>FrmInstallation.Install()</c> 是 777 行 Form 里的 13 个 region，
    /// 没有取消、没有回滚、异常被空 catch 吞掉、失败后留下一个坏掉的安装。
    /// </summary>
    public sealed class InstallEngine
    {
        private readonly Func<IList<IInstallStep>> _stepFactory;

        /// <summary>用默认步骤管线构造。</summary>
        public InstallEngine()
            : this(StandardSteps.CreateDefault)
        {
        }

        /// <summary>用自定义步骤构造（测试会传假的步骤）。</summary>
        public InstallEngine(Func<IList<IInstallStep>> stepFactory)
        {
            if (stepFactory == null)
            {
                throw new ArgumentNullException("stepFactory");
            }

            _stepFactory = stepFactory;
        }

        /// <summary>执行安装。</summary>
        public InstallRunResult Run(InstallContext context, IProgress<InstallProgress> progress = null)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var result = new InstallRunResult();
            var steps = _stepFactory();
            result.StepCount = steps.Count;

            var ct = context.CancellationToken;
            var index = 0;

            try
            {
                foreach (var step in steps)
                {
                    ct.ThrowIfCancellationRequested();
                    index++;

                    var messageKey = step.MessageKey;
                    var text = context.Resolve(messageKey, step.Id);

                    Report(progress, new InstallProgress
                    {
                        StepIndex = index,
                        StepCount = steps.Count,
                        StepId = step.Id,
                        MessageKey = messageKey,
                        Message = text,
                        Percent = (int)(100.0 * (index - 1) / Math.Max(1, steps.Count)),
                    });

                    context.Log.Info("步骤 " + index + "/" + steps.Count + "：" + step.Id + " —— " + text);

                    step.Execute(context);
                    result.StepsExecuted = index;
                }

                // 全部成功 → 提交
                context.Journal.Commit();
                result.Success = true;

                Report(progress, new InstallProgress
                {
                    StepIndex = steps.Count,
                    StepCount = steps.Count,
                    StepId = "done",
                    Message = context.Resolve("Install.Done", "安装完成"),
                    Percent = 100,
                });

                return result;
            }
            catch (OperationCanceledException)
            {
                result.ErrorMessage = "安装已取消。";
                result.FailedStepId = index > 0 && index <= steps.Count ? steps[index - 1].Id : null;
                context.Log.Warn(result.ErrorMessage);
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
                result.FailedStepId = ex is InstallException ? ((InstallException)ex).StepId
                    : (index > 0 && index <= steps.Count ? steps[index - 1].Id : null);
                context.Log.Error("步骤失败：" + result.FailedStepId, ex);
            }

            // ── 回滚：逆序 undo journal ──────────────────────────────
            result.RolledBack = true;
            RollbackJournal(context, steps.Count, progress, result);

            return result;
        }

        /// <summary>逆序撤销 journal 里的全部写操作。尽力而为，单个失败不中断其余撤销。</summary>
        private static void RollbackJournal(InstallContext context, int stepCount,
                                            IProgress<InstallProgress> progress, InstallRunResult result)
        {
            var entries = context.Journal.Entries;
            context.Log.Warn("开始回滚，共 " + entries.Count + " 条记录。");

            for (var i = entries.Count - 1; i >= 0; i--)
            {
                var e = entries[i];
                try
                {
                    Report(progress, new InstallProgress
                    {
                        StepIndex = 0,
                        StepCount = stepCount,
                        StepId = e.StepId,
                        Message = context.Resolve("Install.RollingBack", "正在撤销更改…") + " " + e.Op,
                        Percent = -1,
                        IsRollingBack = true,
                    });

                    Undo(context, e);
                }
                catch (Exception ex)
                {
                    var msg = "撤销失败（" + e.Op + " " + (e.Target ?? e.SubKey) + "）：" + ex.Message;
                    result.RollbackIssues.Add(msg);
                    context.Log.Error(msg, ex);
                }
            }

            context.Log.Warn("回滚结束，问题 " + result.RollbackIssues.Count + " 条。");
        }

        /// <summary>撤销一条记录。</summary>
        private static void Undo(InstallContext context, JournalEntry e)
        {
            var platform = context.Platform;

            switch (e.Op)
            {
                case JournalOps.CreateDirectory:
                    if (!e.ExistedBefore && Directory.Exists(e.Target))
                    {
                        // 只删空目录：里面还有东西说明不是我们建的，或者用户放东西进去了
                        if (Directory.GetFileSystemEntries(e.Target).Length == 0)
                        {
                            Directory.Delete(e.Target, false);
                        }
                    }

                    break;

                case JournalOps.WriteFile:
                    if (!e.ExistedBefore)
                    {
                        if (File.Exists(e.Target))
                        {
                            File.Delete(e.Target);
                        }
                    }
                    else if (!string.IsNullOrEmpty(e.BackupPath) && File.Exists(e.BackupPath))
                    {
                        File.Copy(e.BackupPath, e.Target, true);
                    }

                    break;

                case JournalOps.CreateShortcut:
                    if (!e.ExistedBefore)
                    {
                        platform.Shortcuts.Delete(e.Target);
                    }

                    break;

                case JournalOps.SetRegistryValue:
                    if (!e.ExistedBefore)
                    {
                        platform.Registry.DeleteValue(ParseHive(e.Hive), e.SubKey, e.Name);
                    }
                    else
                    {
                        platform.Registry.SetString(ParseHive(e.Hive), e.SubKey, e.Name, e.OldValue ?? string.Empty);
                    }

                    break;

                case JournalOps.CreateSubKey:
                    if (!e.ExistedBefore)
                    {
                        platform.Registry.DeleteSubKeyTree(ParseHive(e.Hive), e.SubKey);
                    }

                    break;

                case JournalOps.RegisterCom:
                    // DLL 自己写的注册表内容记不下来，只能反过来调用 DllUnregisterServer
                    try
                    {
                        var mode = string.IsNullOrEmpty(e.Extra)
                            ? Installer.Abstractions.Model.ComRegistrationModes.RegSvr32
                            : e.Extra;

                        var undo = Installer.Core.Platform.ComRegistrar.Apply(e.Target, mode, false);
                        if (!undo.Success)
                        {
                            context.Log.Warn("反注册 COM 未成功：" + undo.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        context.Log.Warn("反注册 COM 抛异常：" + ex.Message);
                    }

                    break;

                default:
                    context.Log.Warn("未知的 journal 操作，跳过：" + e.Op);
                    break;
            }
        }

        /// <summary>把 hive 字符串解析回枚举。</summary>
        public static RegistryRoot ParseHive(string hive)
        {
            return string.Equals(hive, "HKLM", StringComparison.OrdinalIgnoreCase)
                   || string.Equals(hive, "LocalMachine", StringComparison.OrdinalIgnoreCase)
                ? RegistryRoot.LocalMachine
                : RegistryRoot.CurrentUser;
        }

        /// <summary>hive 枚举转字符串。</summary>
        public static string HiveName(RegistryRoot hive)
        {
            return hive == RegistryRoot.LocalMachine ? "HKLM" : "HKCU";
        }

        private static void Report(IProgress<InstallProgress> progress, InstallProgress p)
        {
            if (progress != null)
            {
                progress.Report(p);
            }
        }
    }

    /// <summary>写 journal 的便捷封装 —— 步骤统一用它，避免漏记。</summary>
    public static class JournalWriter
    {
        /// <summary>记一条"创建目录"（若之前不存在）。</summary>
        public static void EnsureDirectory(InstallContext ctx, string stepId, string dir, IList<string> createdDirs)
        {
            if (Directory.Exists(dir))
            {
                return;
            }

            Directory.CreateDirectory(dir);
            if (createdDirs != null)
            {
                createdDirs.Add(dir);
            }

            ctx.Journal.Record(new JournalEntry
            {
                StepId = stepId,
                Op = JournalOps.CreateDirectory,
                Target = dir,
                ExistedBefore = false,
            });
        }

        /// <summary>写文件并记录（必要时先备份被覆盖的内容）。</summary>
        public static void WriteFile(InstallContext ctx, string stepId, string path, Action<string> writer)
        {
            var existed = File.Exists(path);
            var backup = existed ? ctx.Journal.BackupFile(path) : null;

            ctx.Journal.Record(new JournalEntry
            {
                StepId = stepId,
                Op = JournalOps.WriteFile,
                Target = path,
                ExistedBefore = existed,
                BackupPath = backup,
            });

            writer(path);
        }

        /// <summary>写注册表值并记录旧值。</summary>
        public static void SetRegistryValue(InstallContext ctx, string stepId, RegistryRoot hive,
                                            string subKey, string name, string value, bool expand)
        {
            var existed = ctx.Platform.Registry.ValueExists(hive, subKey, name);
            var oldValue = existed ? ctx.Platform.Registry.GetString(hive, subKey, name) : null;

            ctx.Journal.Record(new JournalEntry
            {
                StepId = stepId,
                Op = JournalOps.SetRegistryValue,
                Hive = InstallEngine.HiveName(hive),
                SubKey = subKey,
                Name = name,
                ExistedBefore = existed,
                OldValue = oldValue,
            });

            if (expand)
            {
                ctx.Platform.Registry.SetExpandString(hive, subKey, name, value);
            }
            else
            {
                ctx.Platform.Registry.SetString(hive, subKey, name, value);
            }

            ctx.WrittenRegistryValues.Add(new RegistryValueRecord
            {
                Hive = InstallEngine.HiveName(hive),
                SubKey = subKey,
                Name = name,
                Value = value,
            });
        }

        /// <summary>建快捷方式并记录。</summary>
        public static void CreateShortcut(InstallContext ctx, string stepId, string linkPath,
                                          string target, string arguments, string workingDirectory,
                                          string description, string iconPath)
        {
            var existed = ctx.Platform.Shortcuts.Exists(linkPath);

            ctx.Journal.Record(new JournalEntry
            {
                StepId = stepId,
                Op = JournalOps.CreateShortcut,
                Target = linkPath,
                ExistedBefore = existed,
            });

            ctx.Platform.Shortcuts.Create(linkPath, target, arguments, workingDirectory, description, iconPath);
            ctx.CreatedShortcuts.Add(linkPath);
        }
    }
}
