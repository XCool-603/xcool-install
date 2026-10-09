using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Installer.Core.Security
{
    /// <summary>
    /// 路径安全工具 —— 所有"把包内条目名变成磁盘路径"的地方**必须**走这里。
    ///
    /// 现状的 <c>FrmInstallation.cs:725</c> 直接 <c>File.Create(unZipDir + theEntry.Name)</c>，
    /// 条目名里的 <c>..\..\</c> 可以写到安装目录之外（Zip Slip）。本类就是为了让那个写法不可能再出现。
    /// </summary>
    public static class SafePath
    {
        private static readonly char[] SegmentInvalidChars = BuildSegmentInvalidChars();

        private static readonly HashSet<string> ReservedDeviceNames =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "CON", "PRN", "AUX", "NUL",
                "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
                "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9",
            };

        private static char[] BuildSegmentInvalidChars()
        {
            // Path.GetInvalidFileNameChars() 已含 " < > | : * ? \ / 与控制字符；
            // 再补上通配符与 ':'，并去重。
            var set = new HashSet<char>(Path.GetInvalidFileNameChars());
            foreach (var c in new[] { ':', '"', '<', '>', '|', '*', '?' })
            {
                set.Add(c);
            }
            var arr = new char[set.Count];
            set.CopyTo(arr);
            return arr;
        }

        /// <summary>
        /// 校验一个包内条目名是否安全。不安全就抛 <see cref="InvalidDataException"/>。
        /// </summary>
        /// <param name="entryName">zip 条目名或清单里的相对路径。</param>
        public static void ValidateEntryName(string entryName)
        {
            if (string.IsNullOrWhiteSpace(entryName))
            {
                throw new InvalidDataException("包内条目名为空。");
            }

            // 控制字符直接拒绝（含 NUL —— .NET 的路径 API 会截断，是经典的绕过手法）
            for (var i = 0; i < entryName.Length; i++)
            {
                var c = entryName[i];
                if (c == '\0' || char.IsControl(c))
                {
                    throw new InvalidDataException(
                        "包内条目名含控制字符（U+" + ((int)c).ToString("X4") + "）：" + entryName);
                }
            }

            var normalized = entryName.Replace('/', '\\');

            // 绝对路径 / 根路径 / 驱动器相对路径
            if (normalized.StartsWith("\\", StringComparison.Ordinal))
            {
                throw new InvalidDataException("包内条目名是根路径，拒绝：" + entryName);
            }

            if (normalized.Length >= 2 && normalized[1] == ':')
            {
                throw new InvalidDataException("包内条目名带驱动器前缀，拒绝：" + entryName);
            }

            var segments = normalized.Split('\\');
            for (var i = 0; i < segments.Length; i++)
            {
                var seg = segments[i];

                if (seg.Length == 0)
                {
                    throw new InvalidDataException("包内条目名含空路径段，拒绝：" + entryName);
                }

                if (seg == "." || seg == "..")
                {
                    throw new InvalidDataException("包内条目名含相对路径段 \"" + seg + "\"，拒绝：" + entryName);
                }

                // Windows 会静默去掉结尾的点与空格 —— 会造成名字碰撞与绕过
                var last = seg[seg.Length - 1];
                if (last == '.' || last == ' ')
                {
                    throw new InvalidDataException("包内条目名的路径段以点或空格结尾，拒绝：" + entryName);
                }

                if (seg.IndexOfAny(SegmentInvalidChars) >= 0)
                {
                    throw new InvalidDataException("包内条目名的路径段含非法字符，拒绝：" + entryName);
                }

                // 保留设备名（CON / NUL / COM1 …），带不带扩展名都算
                var stem = seg;
                var dot = seg.IndexOf('.');
                if (dot >= 0)
                {
                    stem = seg.Substring(0, dot);
                }

                if (ReservedDeviceNames.Contains(stem))
                {
                    throw new InvalidDataException("包内条目名使用了 Windows 保留设备名 \"" + stem + "\"，拒绝：" + entryName);
                }
            }
        }

        /// <summary>
        /// 把包内条目名安全地解析成 <paramref name="rootDir"/> 下的绝对路径。
        /// 解析结果逃出 rootDir 就抛异常。
        /// </summary>
        /// <param name="rootDir">目标根目录（安装目录）。</param>
        /// <param name="entryName">包内条目名。</param>
        /// <returns>绝对路径。</returns>
        public static string ResolveWithin(string rootDir, string entryName)
        {
            if (string.IsNullOrWhiteSpace(rootDir))
            {
                throw new ArgumentException("rootDir 不能为空。", "rootDir");
            }

            ValidateEntryName(entryName);

            var rootFull = Path.GetFullPath(rootDir);
            if (!rootFull.EndsWith("\\", StringComparison.Ordinal))
            {
                rootFull += "\\";
            }

            var combined = Path.GetFullPath(Path.Combine(rootFull, entryName.Replace('/', '\\')));

            // 再确认一次：归一化之后必须仍在 root 之下
            if (!combined.StartsWith(rootFull, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidDataException(
                    "包内条目名解析后逃出了目标目录，拒绝：" + entryName + " → " + combined);
            }

            return combined;
        }

        /// <summary>判断 <paramref name="candidatePath"/> 是否位于 <paramref name="rootDir"/> 之内。</summary>
        public static bool IsWithin(string rootDir, string candidatePath)
        {
            if (string.IsNullOrWhiteSpace(rootDir) || string.IsNullOrWhiteSpace(candidatePath))
            {
                return false;
            }

            var rootFull = Path.GetFullPath(rootDir);
            if (!rootFull.EndsWith("\\", StringComparison.Ordinal))
            {
                rootFull += "\\";
            }

            var candidateFull = Path.GetFullPath(candidatePath);
            return candidateFull.StartsWith(rootFull, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>把条目名归一化成"反斜杠分隔、无前导 .\"的形式，用于写入清单。</summary>
        public static string NormalizeRelative(string path)
        {
            if (path == null)
            {
                return null;
            }

            var p = path.Replace('/', '\\');
            while (p.StartsWith(".\\", StringComparison.Ordinal))
            {
                p = p.Substring(2);
            }

            return p;
        }

        /// <summary>把条目名转成 zip 内部使用的正斜杠形式。</summary>
        public static string ToZipEntryName(string relativePath)
        {
            return NormalizeRelative(relativePath).Replace('\\', '/');
        }

        /// <summary>去掉路径中的非法文件名字符，用于生成安全的目标文件名。</summary>
        public static string SanitizeFileName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return name;
            }

            var sb = new StringBuilder(name.Length);
            foreach (var c in name)
            {
                sb.Append(Array.IndexOf(SegmentInvalidChars, c) >= 0 ? '_' : c);
            }

            return sb.ToString();
        }
    }
}
