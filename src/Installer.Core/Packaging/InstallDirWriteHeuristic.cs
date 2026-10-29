using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Installer.Abstractions.Model;

namespace Installer.Core.Packaging
{
    /// <summary>
    /// 判断"这个程序会不会往自己的安装目录里写东西"。
    ///
    /// 为什么需要它：一大堆老 WinForm 程序把 <c>App.db</c>、<c>Log\</c>、<c>Data\</c>
    /// 直接放在 <c>Application.StartupPath</c> 下面。这类程序装到
    /// <c>%LOCALAPPDATA%\Programs</c> 完全没问题，一旦装到
    /// <c>%ProgramFiles%</c>，普通用户对安装目录没有写权限 ——
    /// 程序**启动正常**、一操作就抛异常，用户会认为"安装包有问题"。
    ///
    /// 所以在**打包时**就给出提示，比装完之后再排查便宜得多。
    /// </summary>
    public static class InstallDirWriteHeuristic
    {
        /// <summary>一看到就基本可以断定"运行时会写"的扩展名。</summary>
        private static readonly HashSet<string> RiskyExtensions = new HashSet<string>(
            new[]
            {
                ".db", ".sqlite", ".sqlite3", ".db3", ".mdb", ".accdb", ".sdf",
                ".ini", ".log", ".bak", ".tmp", ".dat",
            },
            StringComparer.OrdinalIgnoreCase);

        /// <summary>一看到就基本可以断定"运行时会写"的目录名。</summary>
        private static readonly HashSet<string> RiskyDirectoryNames = new HashSet<string>(
            new[]
            {
                "data", "database", "databases", "db", "sqlitedb", "dbdata",
                "log", "logs", "logging",
                "config", "configs", "conf",
                "cache", "temp", "tmp",
                "upload", "uploads", "download", "downloads",
                "backup", "backups",
                "save", "saves", "saved",
                "photo", "photos", "picture", "pictures",
                "report", "reports", "output", "outputs",
            },
            StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 扫描打包目录的**顶层**，找出疑似"运行时会写"的项。
        ///
        /// 只扫顶层：往下递归会把 <c>x64\System.Data.SQLite.dll</c> 这种
        /// 纯运行库误报成数据目录，噪音大到没人看。
        /// </summary>
        /// <param name="sourceDir">打包源目录。</param>
        /// <returns>命中项（相对路径），按字母序。</returns>
        public static IReadOnlyList<string> Detect(string sourceDir)
        {
            var hits = new List<string>();

            if (string.IsNullOrWhiteSpace(sourceDir) || !Directory.Exists(sourceDir))
            {
                return hits;
            }

            try
            {
                foreach (var dir in Directory.GetDirectories(sourceDir))
                {
                    var name = Path.GetFileName(dir);
                    if (!string.IsNullOrEmpty(name) && RiskyDirectoryNames.Contains(name))
                    {
                        hits.Add(name + Path.DirectorySeparatorChar);
                    }
                }

                foreach (var file in Directory.GetFiles(sourceDir))
                {
                    var ext = Path.GetExtension(file);
                    if (!string.IsNullOrEmpty(ext) && RiskyExtensions.Contains(ext))
                    {
                        hits.Add(Path.GetFileName(file));
                    }
                }
            }
            catch (Exception)
            {
                // 读不动就算了 —— 这是"锦上添花"的检查，绝不能因为它让打包失败
                return hits;
            }

            hits.Sort(StringComparer.OrdinalIgnoreCase);
            return hits;
        }

        /// <summary>
        /// 生成给制作人员看的提示文案。没有命中时返回 null。
        /// </summary>
        /// <param name="sourceDir">打包源目录。</param>
        /// <param name="allUsers">
        /// 安装范围是否"所有用户"。为 true 时提示里要额外说明
        /// "这个包装到 Program Files 会有问题"，否则只提示建议。
        /// </param>
        public static string Describe(string sourceDir, bool allUsers)
        {
            return Describe(sourceDir, allUsers, false);
        }

        /// <summary>
        /// 生成给制作人员看的提示文案（区分"程序是否始终以管理员运行"）。没有命中时返回 null。
        ///
        /// 为什么要把 <paramref name="alwaysElevate"/> 分出来：勾了「始终请求管理员权限」的包，
        /// 程序运行时本来就是管理员 —— 往 Program Files 写东西通常没问题。
        /// 以前不管这个一律说"很可能读写失败"，等于给人一条用不上的建议，
        /// 用户只会觉得提示在瞎报。
        /// </summary>
        /// <param name="sourceDir">打包源目录。</param>
        /// <param name="allUsers">安装范围是否"所有用户"。</param>
        /// <param name="alwaysElevate">清单是否勾了「始终请求管理员权限」。</param>
        public static string Describe(string sourceDir, bool allUsers, bool alwaysElevate)
        {
            return Describe(sourceDir, allUsers, alwaysElevate, false);
        }

        /// <summary>
        /// 生成提示文案（再区分"安装包本身是否要求管理员"）。没有命中时返回 null。
        ///
        /// <paramref name="requireAdministrator"/> 与 <paramref name="alwaysElevate"/> 是**两件事**：
        /// <list type="bullet">
        /// <item>前者只让**安装包**以管理员运行（双击弹 UAC）；</item>
        /// <item>后者是**程序自己**声明始终提权。</item>
        /// </list>
        /// 安装包提权不会让装好的程序也提权 —— 所以"程序要往安装目录里写"这件事仍然要看后者。
        /// 用户反馈"都是管理员权限了为什么还提示"，就是这两者被混为一谈；文案必须说清楚。
        /// </summary>
        public static string Describe(string sourceDir, bool allUsers, bool alwaysElevate, bool requireAdministrator)
        {
            var hits = Detect(sourceDir);
            if (hits.Count == 0)
            {
                return null;
            }

            var sample = string.Join("、", hits.Take(5).ToArray());
            if (hits.Count > 5)
            {
                sample += string.Format(CultureInfo.InvariantCulture, " 等 {0} 项", hits.Count);
            }

            var head = string.Format(CultureInfo.InvariantCulture,
                "打包目录里发现 {0} 个疑似「程序运行时会往安装目录里写」的项：{1}。",
                hits.Count, sample);

            if (!allUsers)
            {
                // 装在 %LOCALAPPDATA%\Programs 下，普通用户本来就能写 —— 不是问题，
                // 但如果安装包要求管理员，要顺手说清楚"提权的只是安装过程"。
                if (requireAdministrator)
                {
                    return head +
                           "本包会以管理员身份运行（双击安装包会弹 UAC），但**装好之后的程序仍是当前用户的普通权限** —— " +
                           "提权的只是安装过程。现在装到 %LOCALAPPDATA%\\Programs（实际目录 " +
                           "%LOCALAPPDATA%\\Programs\\厂商\\产品名）普通用户可写，所以运行没问题；" +
                           "如果以后改成「所有用户」（装到 %ProgramFiles%），就需要程序自身也声明以管理员启动，" +
                           "否则运行时写这些文件会失败。";
                }

                return head +
                       "现在装到 %LOCALAPPDATA%\\Programs（实际目录是 %LOCALAPPDATA%\\Programs\\厂商\\产品名）" +
                       "是安全的，普通用户能写；如果以后改成「所有用户」，普通用户就写不进去了。";
            }

            // 所有用户 + 始终提权：程序本身就是管理员，写入一般没问题，重点是提醒"别的入口别踩坑"
            var elevateNote = alwaysElevate
                ? "本包勾了「始终请求管理员权限」，安装和启动都会提权，写这个目录通常没问题 —— " +
                  "但请确认程序自己的清单也声明以管理员启动，否则用户拿普通权限直接双击 exe 时仍会写失败。" +
                  "另外开机自启、其他非管理员账号启动时也会受限。"
                : "如果必须装在 Program Files：先勾上「始终请求管理员权限」，并确认程序自身是以管理员身份运行的。";

            return head +
                   "当前安装范围是「所有用户」（装到 %ProgramFiles%），普通用户对那个目录没有写权限，" +
                   "程序运行时要写这些文件就会失败。" +
                   "建议把「安装根目录」改成 %LOCALAPPDATA%\\Programs" +
                   "（最终目录是 %LOCALAPPDATA%\\Programs\\厂商\\产品名，普通用户可写）；" +
                   elevateNote;
        }
    }
}
