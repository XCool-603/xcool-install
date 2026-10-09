using System;
using System.IO;
using System.Reflection;

namespace Installer.Builder.Common
{
    /// <summary>
    /// 找模板 stub（Installer.Runtime.exe）。
    ///
    /// **用户不该看见这个文件。** 它是工具自己的零件，不是用户要提供的东西，
    /// 所以这里按固定顺序自动找，界面上没有"选择模板"这一项。
    /// </summary>
    public static class StubLocator
    {
        /// <summary>stub 的文件名。</summary>
        public const string StubFileName = "Installer.Runtime.exe";

        /// <summary>
        /// 自动定位 stub。找不到返回 null。
        /// 顺序：本程序同目录 → 同目录下的 stub\ → 向上找两级（源码树里跑的时候）。
        /// </summary>
        public static string Find()
        {
            foreach (var dir in CandidateDirectories())
            {
                if (string.IsNullOrEmpty(dir))
                {
                    continue;
                }

                try
                {
                    var path = Path.Combine(dir, StubFileName);
                    if (File.Exists(path))
                    {
                        return Path.GetFullPath(path);
                    }

                    var nested = Path.Combine(dir, "stub", StubFileName);
                    if (File.Exists(nested))
                    {
                        return Path.GetFullPath(nested);
                    }
                }
                catch (ArgumentException)
                {
                }
                catch (IOException)
                {
                }
            }

            return null;
        }

        /// <summary>候选目录（按优先级）。</summary>
        private static string[] CandidateDirectories()
        {
            var exe = Assembly.GetExecutingAssembly().Location;
            var dir = string.IsNullOrEmpty(exe) ? null : Path.GetDirectoryName(exe);

            if (string.IsNullOrEmpty(dir))
            {
                return new string[0];
            }

            return new[]
            {
                dir,
                Path.Combine(dir, ".."),                       // 上级（bin\Release → bin）
                Path.Combine(dir, "..", ".."),                 // 再上一级
                Path.Combine(dir, "..", "..", "..", "Installer.Runtime", "bin", "Release"),
                Path.Combine(dir, "..", "..", "..", "Installer.Runtime", "bin", "Debug"),
            };
        }

        /// <summary>找不到时给用户看的提示。</summary>
        public static string MissingHint()
        {
            return "找不到 " + StubFileName + "。\r\n\r\n" +
                   "它应该和本程序放在同一个文件夹里 —— 请确认没有单独把本程序拷出来。";
        }
    }
}
