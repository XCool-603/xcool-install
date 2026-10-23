using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Installer.Abstractions.Install;
using Installer.Abstractions.Platform;
using Installer.Core.Common;
using Installer.Core.Install;
using Installer.Core.Security;
using Newtonsoft.Json;

namespace Installer.Core.Uninstall
{
    /// <summary>卸载结果。</summary>
    public sealed class UninstallRunResult
    {
        /// <summary>是否成功。</summary>
        public bool Success { get; set; }

        /// <summary>删除了多少文件。</summary>
        public int DeletedFiles { get; set; }

        /// <summary>删除了多少快捷方式。</summary>
        public int DeletedShortcuts { get; set; }

        /// <summary>删除注册表项时遇到的问题。</summary>
        public List<string> Issues { get; set; } = new List<string>();

        /// <summary>内容被用户改动过、但仍按清单删除的文件。</summary>
        public List<string> ModifiedFiles { get; set; } = new List<string>();

        /// <summary>日志。</summary>
        public List<string> Messages { get; set; } = new List<string>();

        /// <summary>是否需要重启才能完成（自删除被延迟时）。</summary>
        public bool RequiresRestart { get; set; }

        /// <summary>被占用、连"重启后删除"都没登记成功的文件 —— 真正意义上的残留。</summary>
        public List<string> UndeletedPaths { get; set; } = new List<string>();

        /// <summary>被占用、已登记为"重启后删除"的文件（不算残留，但需要告知用户）。</summary>
        public List<string> PendingRebootPaths { get; set; } = new List<string>();

        /// <summary>
        /// 当前进程删不掉、需要交给**清理副本**在卸载器退出后重试的**绝对路径**。
        ///
        /// 最典型的就是安装目录里的 <c>Newtonsoft.Json.dll</c>：卸载器自己运行在安装目录下，
        /// .NET 会优先探测"应用程序基目录"里的同名 DLL，于是把用户 App 的那份加载进了
        /// 自己的进程 —— 卸载时去删它，锁的持有者正是卸载器本人，必然失败。
        /// 副本从 %TEMP% 启动，不碰安装目录的同名 DLL，而且等本进程退出后才动手，
        /// 那时锁已经释放，一次就能删掉。
        /// </summary>
        public List<string> BlockedPaths { get; set; } = new List<string>();
    }

    /// <summary>
    /// 卸载引擎。
    ///
    /// **按安装记录精确删除**，而不是"把安装目录递归删光"。
    /// 现状（<c>FrmUnInstall.cs:232-257</c>）会删掉 <c>Application.StartupPath</c> 下的
    /// 一切 —— 用户放在安装目录里的数据、或者误放到共享目录时的无关文件，全部陪葬。
    /// </summary>
    public sealed class UninstallEngine
    {
        /// <summary>从安装记录执行卸载。</summary>
        /// <param name="record">安装记录。</param>
        /// <param name="platform">平台服务。</param>
        /// <param name="progress">进度回调。</param>
        /// <param name="deleteInstallDir">是否尝试删除安装目录本身。</param>
        public UninstallRunResult Run(InstallRecord record, IPlatformServices platform,
                                      IProgress<InstallProgress> progress = null,
                                      bool deleteInstallDir = true)
        {
            if (record == null)
            {
                throw new ArgumentNullException("record");
            }

            var result = new UninstallRunResult();
            var installDir = record.InstallDir;

            if (string.IsNullOrWhiteSpace(installDir) || !Directory.Exists(installDir))
            {
                // 目录没了也要清注册表与快捷方式
                platform.Log.Warn("安装目录不存在，只清理注册表与快捷方式：" + installDir);
            }

            var total = 4;
            var index = 0;

            // ① 结束正在运行的程序
            Step(progress, ++index, total, "uninstall-kill", "正在结束正在运行的程序…");
            TryKill(record, platform, result);

            // ② 删快捷方式
            Step(progress, ++index, total, "uninstall-shortcuts", "正在删除快捷方式…");
            foreach (var link in record.Shortcuts ?? new List<string>())
            {
                try
                {
                    if (platform.Shortcuts.Exists(link))
                    {
                        platform.Shortcuts.Delete(link);
                        result.DeletedShortcuts++;
                    }
                }
                catch (Exception ex)
                {
                    result.Issues.Add("删除快捷方式失败 " + link + "：" + ex.Message);
                }
            }

            // ③ 删注册表（自启动项 + 标准卸载键 + 记录里写入的每一个值）
            Step(progress, ++index, total, "uninstall-registry", "正在清理注册表…");
            TryDeleteAutostart(record, platform, result);
            TryDeleteUninstallKey(record, platform, result);
            TryDeleteLegacyKeys(record, platform, result);

            // ③.5 反注册 COM —— 必须在删文件**之前**，DLL 没了就反注册不了了
            Step(progress, ++index, total, "uninstall-com", "正在反注册组件…");
            TryUnregisterCom(record, platform, result);

            // ④ 按清单删文件（这是与现状最大的区别）
            Step(progress, ++index, total, "uninstall-files", "正在删除程序文件…");
            DeleteRecordedFiles(record, platform, result);

            // ⑤ 安装目录里的已知残留：Log 目录、安装记录、journal、.rollback
            //
            // 这一步以前只在降级卸载（RunWithoutRecord）里做，正常卸载路径整个漏掉了。
            // 删 "Log" 目录的唯一代码就在 DeleteLeftovers 里 —— 所以走正常卸载时
            // Log 目录**永远不会**被删。用户反馈「卸载成功后 Log 目录还在」就是这里漏的。
            DeleteLeftovers(record.InstallDir, result);

            if (deleteInstallDir)
            {
                DeleteEmptyDirectories(record, platform, result);
            }

            // 只有"程序文件确实都清掉了"才算成功：
            // 直接删掉、或者已登记为重启后删除，两者都算清掉。
            //
            // 以前这里是无条件 true —— 文件被占用删不掉时界面照样显示"卸载成功"，
            // 用户看到的是"卸载成功但文件还在"。这是最糟的失败方式：既不干净，也不告知。
            result.Success = result.UndeletedPaths.Count == 0;
            return result;
        }

        // ─────────────────────────────────────────────────────────────

        private static void TryKill(InstallRecord record, IPlatformServices platform, UninstallRunResult result)
        {
            foreach (var name in RunnableNames(record))
            {
                try
                {
                    if (platform.Processes.GetRunningProcessNames()
                            .Any(n => string.Equals(n, name, StringComparison.OrdinalIgnoreCase)))
                    {
                        var killed = platform.Processes.Kill(name);
                        result.Messages.Add("已结束 " + killed + " 个进程：" + name);
                    }
                }
                catch (Exception ex)
                {
                    result.Issues.Add("结束进程失败 " + name + "：" + ex.Message);
                }
            }
        }

        /// <summary>
        /// 需要结束的进程名 = 入口点 + 安装记录里**所有**可执行文件。
        ///
        /// 以前只杀入口点那一个名字，这是"卸载成功但 DLL 还在"的根因：
        /// 主程序经常拉起子进程（日志、通讯/TCP、更新器、托盘守护），名字各不相同，
        /// 但都会把 payload 里的 DLL 映射进自己的地址空间。主程序被杀掉、子进程还活着，
        /// 那个 DLL 就一直被占用 → File.Delete 抛异常 → 文件留在原地。
        /// 用户机器上残留的 Newtonsoft.Json.dll 正是这种情况。
        /// </summary>
        private static IEnumerable<string> RunnableNames(InstallRecord record)
        {
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var candidates = new List<string>();

            if (!string.IsNullOrEmpty(record.EntryPoint))
            {
                candidates.Add(record.EntryPoint);
            }

            if (record.Files != null)
            {
                candidates.AddRange(record.Files);
            }

            foreach (var rel in candidates)
            {
                if (string.IsNullOrEmpty(rel))
                {
                    continue;
                }

                var ext = Path.GetExtension(rel);
                var runnable = string.Equals(ext, ".exe", StringComparison.OrdinalIgnoreCase)
                            || string.Equals(ext, ".com", StringComparison.OrdinalIgnoreCase)
                            || string.Equals(ext, ".bat", StringComparison.OrdinalIgnoreCase)
                            || string.Equals(ext, ".cmd", StringComparison.OrdinalIgnoreCase);

                if (!runnable)
                {
                    continue;
                }

                var name = Path.GetFileNameWithoutExtension(rel);
                if (!string.IsNullOrEmpty(name) && seen.Add(name))
                {
                    yield return name;
                }
            }
        }

        private static void TryDeleteAutostart(InstallRecord record, IPlatformServices platform, UninstallRunResult result)
        {
            if (string.IsNullOrEmpty(record.AutostartKeyPath) || string.IsNullOrEmpty(record.AutostartValueName))
            {
                return;
            }

            try
            {
                var hive = InstallEngine.ParseHive(record.AutostartKeyPath);
                var subKey = StripHive(record.AutostartKeyPath);
                platform.Registry.DeleteValue(hive, subKey, record.AutostartValueName);
                result.Messages.Add("已删除开机启动项：" + record.AutostartValueName);
            }
            catch (Exception ex)
            {
                result.Issues.Add("删除开机启动项失败：" + ex.Message);
            }
        }

        private static void TryDeleteUninstallKey(InstallRecord record, IPlatformServices platform, UninstallRunResult result)
        {            if (string.IsNullOrEmpty(record.UninstallKeyPath))
            {
                return;
            }

            try
            {
                var hive = InstallEngine.ParseHive(record.UninstallKeyPath);
                var subKey = StripHive(record.UninstallKeyPath);
                platform.Registry.DeleteSubKeyTree(hive, subKey);
                result.Messages.Add("已删除标准卸载键：" + record.UninstallKeyPath);
            }
            catch (Exception ex)
            {
                result.Issues.Add("删除标准卸载键失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 删掉为兼容老约定而写的私有键（<c>HKCU\SOFTWARE\WayMark\&lt;名字&gt;</c>）。
        ///
        /// 整棵删：老安装器只删自己的子键、把 <c>SOFTWARE\WayMark</c> 父键留在注册表里
        /// （项目分析报告.md L273 就是这么写的）。这里删到只剩必要的父键 ——
        /// 父键里还有别的产品时不能动，所以删完子键后再尝试删空父键。
        /// </summary>
        private static void TryDeleteLegacyKeys(InstallRecord record, IPlatformServices platform,
                                               UninstallRunResult result)
        {
            if (record.LegacyKeys == null || record.LegacyKeys.Count == 0)
            {
                return;
            }

            foreach (var fullPath in record.LegacyKeys)
            {
                if (string.IsNullOrWhiteSpace(fullPath))
                {
                    continue;
                }

                try
                {
                    var hive = InstallEngine.ParseHive(fullPath);
                    var subKey = StripHive(fullPath);
                    platform.Registry.DeleteSubKeyTree(hive, subKey);
                    result.Messages.Add("已删除老约定兼容键：" + fullPath);
                }
                catch (Exception ex)
                {
                    result.Issues.Add("删除老约定兼容键失败 " + fullPath + "：" + ex.Message);
                }
            }
        }

        /// <summary>
        /// 反注册 COM 组件。
        /// 必须在删文件之前做 —— DLL 被删掉之后就再也反注册不了了，注册表里会留下死项。
        /// </summary>
        private static void TryUnregisterCom(InstallRecord record, IPlatformServices platform, UninstallRunResult result)
        {
            if (record.Com == null || record.Com.Count == 0)
            {
                return;
            }

            foreach (var spec in record.Com)
            {
                if (spec == null || string.IsNullOrWhiteSpace(spec.Path))
                {
                    continue;
                }

                if (!File.Exists(spec.Path))
                {
                    result.Issues.Add("COM 组件已不存在，无法反注册：" + spec.Path);
                    continue;
                }

                try
                {
                    var mode = string.IsNullOrWhiteSpace(spec.Mode)
                        ? Installer.Abstractions.Model.ComRegistrationModes.RegSvr32
                        : spec.Mode;

                    var undo = Installer.Core.Platform.ComRegistrar.Apply(spec.Path, mode, false);
                    if (undo.Success)
                    {
                        result.Messages.Add("已反注册 COM：" + Path.GetFileName(spec.Path));
                    }
                    else
                    {
                        result.Issues.Add("反注册 COM 失败 " + Path.GetFileName(spec.Path) + "：" + undo.Message);
                    }
                }
                catch (Exception ex)
                {
                    result.Issues.Add("反注册 COM 抛异常 " + spec.Path + "：" + ex.Message);
                }
            }
        }

        /// <summary>按安装记录删除文件。内容被改过的也删，但会记下来。</summary>
        private static void DeleteRecordedFiles(InstallRecord record, IPlatformServices platform, UninstallRunResult result)
        {
            if (string.IsNullOrEmpty(record.InstallDir) || !Directory.Exists(record.InstallDir))
            {
                return;
            }

            foreach (var rel in record.Files ?? new List<string>())
            {
                string full;
                try
                {
                    full = SafePath.ResolveWithin(record.InstallDir, rel);
                }
                catch (Exception ex)
                {
                    result.Issues.Add("跳过不安全的记录路径 " + rel + "：" + ex.Message);
                    continue;
                }

                if (!File.Exists(full))
                {
                    continue;
                }

                // 内容变了说明用户改过或替换过 —— 仍然删（是我们的文件），但记一笔
                string expected;
                if (record.FileHashes != null && record.FileHashes.TryGetValue(rel, out expected)
                    && !string.IsNullOrEmpty(expected))
                {
                    try
                    {
                        var actual = Hashing.ComputeFile(full);
                        if (!string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase))
                        {
                            result.ModifiedFiles.Add(rel);
                        }
                    }
                    catch (IOException)
                    {
                    }
                }

                if (DeleteWithRetry(full))
                {
                    result.DeletedFiles++;
                    continue;
                }

                // 删不掉：文件还被别的进程占着（子进程没杀干净，或用户手上开着句柄）。
                // 不再只是记一笔 Issue 了事 —— 先重试，再登记"重启后删除"，
                // 两者都失败才算真正的残留，并由 result.Success 如实反映出来。
                result.Issues.Add("删除文件失败（被占用）：" + rel);
                HandleUndeletable(full, rel, result);
            }

            // 安装记录自己也要删
            var recordPath = InstallRecord.GetPath(record.InstallDir);
            try
            {
                if (File.Exists(recordPath))
                {
                    File.Delete(recordPath);
                }
            }
            catch (Exception ex)
            {
                result.Issues.Add("删除安装记录失败：" + ex.Message);
            }
        }

        // ─────────────────────────────────────────────────────────────
        // 删不掉的文件：重试 → 登记重启后删除 → 如实上报
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// 删文件，被占用时重试几次再放弃。
        ///
        /// 为什么要重试：占用往往来自刚刚被结束的进程（日志 / 通讯子进程）。
        /// 进程退出到内核真正释放文件句柄之间有几十到几百毫秒的窗口，
        /// 一上来就 File.Delete 必然失败 —— 而失败会被当成"残留"报给用户。
        /// </summary>
        private static bool DeleteWithRetry(string path)
        {
            for (var attempt = 0; attempt < 6; attempt++)
            {
                try
                {
                    if (!File.Exists(path))
                    {
                        return true;
                    }

                    var attrs = File.GetAttributes(path);
                    if ((attrs & FileAttributes.ReadOnly) != 0)
                    {
                        File.SetAttributes(path, attrs & ~FileAttributes.ReadOnly);
                    }

                    File.Delete(path);
                    return true;
                }
                catch (Exception)
                {
                    if (attempt == 5)
                    {
                        break;
                    }

                    System.Threading.Thread.Sleep(250 * (attempt + 1));
                }
            }

            return !File.Exists(path);
        }

        /// <summary>登记"下次重启时删除"。</summary>
        private static bool ScheduleDeleteOnReboot(string path)
        {
            try
            {
                return MoveFileEx(path, null, MOVEFILE_DELAY_UNTIL_REBOOT);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 处理删不掉的文件 / 目录：能登记"重启后删除"就记入 PendingRebootPaths（不算残留），
        /// 登记不上才记入 UndeletedPaths（真残留，会让 Success = false）。
        /// </summary>
        private static void HandleUndeletable(string path, string display, UninstallRunResult result)
        {
            // 无论后面能不能登记"重启后删除"，都记下绝对路径 —— 清理副本会在本进程退出后
            // 重试删除。绝大多数情况（尤其是"卸载器自己锁着自己从安装目录加载的 DLL"）
            // 副本一次就能删掉，根本用不着等到重启。
            result.BlockedPaths.Add(path);

            if (ScheduleDeleteOnReboot(path))
            {
                result.PendingRebootPaths.Add(display);
                result.RequiresRestart = true;
                result.Messages.Add("已登记重启后删除：" + display);
                return;
            }

            result.UndeletedPaths.Add(display);
        }

        private const int MOVEFILE_DELAY_UNTIL_REBOOT = 0x4;

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool MoveFileEx(string existingFileName, string newFileName, int flags);

        /// <summary>自底向上删空目录（只删空的，用户放了东西就保留）。</summary>
        private static void DeleteEmptyDirectories(InstallRecord record, IPlatformServices platform, UninstallRunResult result)
        {
            var dir = record.InstallDir;
            if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir))
            {
                return;
            }

            var all = Directory.GetDirectories(dir, "*", SearchOption.AllDirectories)
                .OrderByDescending(d => d.Length)
                .ToList();

            foreach (var d in all)
            {
                try
                {
                    if (Directory.GetFileSystemEntries(d).Length == 0)
                    {
                        Directory.Delete(d, false);
                    }
                }
                catch (IOException)
                {
                }
            }

            // 子目录清完之后，**安装目录本身也要删**。
            // 之前只遍历子目录（GetDirectories 的结果不含 dir 自己），
            // 所以卸载后总会留下一个空的产品文件夹 —— 用户反馈「卸载后安装目录本身还在」。
            try
            {
                if (!Directory.Exists(dir))
                {
                    return;
                }

                if (Directory.GetFileSystemEntries(dir).Length == 0)
                {
                    Directory.Delete(dir, false);
                    result.Messages.Add("已删除安装目录：" + dir);
                }
                else
                {
                    // 目录里还剩东西（通常是用户自己的文件），保留并说明原因
                    result.Messages.Add("安装目录非空，已保留：" + dir);
                }
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        private static string StripHive(string fullPath)
        {
            var idx = fullPath.IndexOf('\\');
            return idx < 0 ? fullPath : fullPath.Substring(idx + 1);
        }

        private static void Step(IProgress<InstallProgress> progress, int index, int total, string id, string message)
        {
            if (progress != null)
            {
                progress.Report(new InstallProgress
                {
                    StepIndex = index,
                    StepCount = total,
                    StepId = id,
                    Message = message,
                    Percent = (int)(100.0 * index / total),
                });
            }
        }

        // ─────────────────────────────────────────────────────────────
        // 读取安装记录
        // ─────────────────────────────────────────────────────────────

        /// <summary>从安装目录读安装记录；不存在返回 null。</summary>
        public static InstallRecord LoadRecord(string installDir)
        {
            var path = InstallRecord.GetPath(installDir);
            if (!File.Exists(path))
            {
                return null;
            }

            return JsonConvert.DeserializeObject<InstallRecord>(File.ReadAllText(path, Encoding.UTF8));
        }

        // ─────────────────────────────────────────────────────────────
        // 没有安装记录时的降级卸载
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// 安装记录丢失时的**降级卸载**。
        ///
        /// **为什么需要它**：制作端以前每次打开都生成新的 productCode，
        /// 同一个产品装三次就在「程序和功能」里留下三条记录。卸掉其中一条时，
        /// 安装记录被删掉，剩下两条点"卸载"就**永远没反应**了 —— 这正是用户遇到的问题。
        ///
        /// 这里不依赖记录，改为按"卸载键指向的安装目录"定位：
        /// ① 删掉指向本目录的全部卸载键（可选再按 productCode 精确定位一条）
        /// ② 按显示名删掉桌面/开始菜单里的快捷方式
        /// ③ 删掉安装目录里的残留（卸载器自己、记录、日志、journal、.rollback）
        /// ④ 删空目录（用户自己放的东西保留）
        /// </summary>
        public UninstallRunResult RunWithoutRecord(string installDir, IPlatformServices platform,
                                                   string productCode = null,
                                                   IProgress<InstallProgress> progress = null)
        {
            var result = new UninstallRunResult();
            const int total = 4;
            var index = 0;

            // ① 卸载键
            Step(progress, ++index, total, "uninstall-keys", "正在清理残留的卸载项…");

            var keys = new List<UninstallKeyRef>();

            var byCode = UninstallKeys.FindByProductCode(platform.Registry, productCode);
            if (byCode != null)
            {
                keys.Add(byCode);
            }

            // 不管有没有 productCode，把"指向这个目录"的键全清掉 —— 那才是控制面板里多出来的条目
            foreach (var k in UninstallKeys.FindByInstallLocation(platform.Registry, installDir))
            {
                var dup = keys.Any(x => x.Hive == k.Hive
                    && string.Equals(x.KeyName, k.KeyName, StringComparison.OrdinalIgnoreCase));
                if (!dup)
                {
                    keys.Add(k);
                }
            }

            var displayNames = new List<string>();

            foreach (var key in keys)
            {
                if (!string.IsNullOrWhiteSpace(key.DisplayName)
                    && !displayNames.Contains(key.DisplayName))
                {
                    displayNames.Add(key.DisplayName);
                }

                try
                {
                    platform.Registry.DeleteSubKeyTree(key.Hive, key.SubKey);
                    result.Messages.Add("已删除卸载项：" + InstallEngine.HiveName(key.Hive) + "\\" + key.SubKey);
                }
                catch (Exception ex)
                {
                    result.Issues.Add("删除卸载项失败 " + key.SubKey + "：" + ex.Message);
                }
            }

            // ② 快捷方式（按显示名找）
            Step(progress, ++index, total, "uninstall-shortcuts", "正在清理快捷方式…");
            DeleteShortcutsByName(platform, displayNames, result);

            // ③ 安装目录里的残留
            Step(progress, ++index, total, "uninstall-files", "正在删除残留文件…");
            DeleteLeftovers(installDir, result);

            // ④ 空目录
            Step(progress, ++index, total, "uninstall-dirs", "正在清理空目录…");
            DeleteEmptyDirectories(installDir);

            // 与 Run 保持一致：有真正删不掉的残留就不算成功
            result.Success = result.UndeletedPaths.Count == 0;
            return result;
        }

        /// <summary>按显示名删掉桌面 / 开始菜单 / 启动 里的快捷方式。</summary>
        private static void DeleteShortcutsByName(IPlatformServices platform,
                                                  IList<string> displayNames,
                                                  UninstallRunResult result)
        {
            if (displayNames == null || displayNames.Count == 0)
            {
                return;
            }

            var folders = new[]
            {
                SpecialFolderKind.Desktop,
                SpecialFolderKind.StartMenu,
                SpecialFolderKind.Startup,
            };

            foreach (var kind in folders)
            {
                string dir;
                try
                {
                    dir = platform.Environment.GetFolder(kind);
                }
                catch (Exception)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir))
                {
                    continue;
                }

                foreach (var name in displayNames)
                {
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        continue;
                    }

                    var link = Path.Combine(dir, name + ".lnk");
                    try
                    {
                        if (File.Exists(link))
                        {
                            File.Delete(link);
                            result.DeletedShortcuts++;
                            result.Messages.Add("已删除快捷方式：" + link);
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Issues.Add("删除快捷方式失败 " + link + "：" + ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 删掉安装目录里的**已知残留**：卸载器自己、安装记录、日志、journal、.rollback。
        /// **不递归删目录** —— 用户放进去的东西必须留下（这是与现状最大的区别）。
        /// </summary>
        private static void DeleteLeftovers(string installDir, UninstallRunResult result)
        {
            if (string.IsNullOrEmpty(installDir) || !Directory.Exists(installDir))
            {
                return;
            }

            var targets = new List<string>
            {
                Path.Combine(installDir, "Uninstall.exe"),
                InstallRecord.GetPath(installDir),
                Path.Combine(installDir, ".install-journal.jsonl"),
                Path.Combine(installDir, ".install.log"),
            };

            // 日志目录和回滚目录整棵删掉 —— 这两个是**运行时产物**，根本不在安装清单里，
            // 只按清单删文件永远清不掉。用户反馈残留的 "Log" 目录就是它。
            foreach (var dir in new[] { "Log", ".rollback" })
            {
                var full = Path.Combine(installDir, dir);
                if (!Directory.Exists(full))
                {
                    continue;
                }

                if (TryDeleteDirectory(full))
                {
                    result.Messages.Add("已删除目录：" + full);
                    continue;
                }

                result.Issues.Add("删除目录失败（被占用）：" + dir);
                HandleUndeletable(full, dir + Path.DirectorySeparatorChar, result);
            }

            foreach (var path in targets)
            {
                if (!File.Exists(path))
                {
                    continue;
                }

                // 卸载器自己正被占用，删不掉很正常 —— 交给自删除流程
                // （把副本复制到 %TEMP% 由它删掉安装目录里的 Uninstall.exe）。
                // 注意：这里跳过它，也**绝不能**把它算进 UndeletedPaths ——
                // 否则每次正常卸载都会被判成"失败"。
                if (string.Equals(Path.GetFileName(path), "Uninstall.exe", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (DeleteWithRetry(path))
                {
                    result.DeletedFiles++;
                    result.Messages.Add("已删除：" + path);
                    continue;
                }

                result.Issues.Add("删除失败（被占用）：" + Path.GetFileName(path));
                HandleUndeletable(path, Path.GetFileName(path), result);
            }
        }

        /// <summary>整棵删目录；被占用时重试几次。</summary>
        private static bool TryDeleteDirectory(string path)
        {
            for (var attempt = 0; attempt < 6; attempt++)
            {
                try
                {
                    if (!Directory.Exists(path))
                    {
                        return true;
                    }

                    Directory.Delete(path, true);
                    return true;
                }
                catch (Exception)
                {
                    if (attempt == 5)
                    {
                        break;
                    }

                    System.Threading.Thread.Sleep(250 * (attempt + 1));
                }
            }

            return !Directory.Exists(path);
        }

        /// <summary>自底向上删空目录（只删空的）。</summary>
        private static void DeleteEmptyDirectories(string dir)
        {
            if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir))
            {
                return;
            }

            var all = Directory.GetDirectories(dir, "*", SearchOption.AllDirectories)
                .OrderByDescending(d => d.Length)
                .ToList();

            foreach (var d in all)
            {
                try
                {
                    if (Directory.GetFileSystemEntries(d).Length == 0)
                    {
                        Directory.Delete(d, false);
                    }
                }
                catch (IOException)
                {
                }
            }
        }
    }
}
