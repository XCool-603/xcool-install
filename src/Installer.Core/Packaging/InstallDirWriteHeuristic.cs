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

            var tail = allUsers
                ? "当前安装范围是「所有用户」（装到 %ProgramFiles%），普通用户对安装目录没有写权限，" +
                  "这类程序运行时很可能读写失败。建议把默认安装目录改成 %LOCALAPPDATA%\\Programs\\<产品名>， " +
                  "或者确认程序确实是以管理员身份运行的。"
                : "现在装到 %LOCALAPPDATA%\\Programs 是安全的；如果以后改成「所有用户」，" +
                  "普通用户就会写不进去。";

            return head + tail;
        }
    }
}
