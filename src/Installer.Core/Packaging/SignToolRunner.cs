using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Installer.Abstractions.Packaging;

namespace Installer.Core.Packaging
{
    /// <summary>
    /// signtool 封装。
    ///
    /// 顺序提醒：**图标补丁 → 签名**。反过来的话，补丁会改写 PE 资源，让 Authenticode 签名失效。
    /// </summary>
    public static class SignToolRunner
    {
        /// <summary>在常见的 Windows SDK 安装位置里找 signtool.exe。</summary>
        public static string FindSignTool()
        {
            var roots = new[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                    @"Windows Kits\10\bin"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                    @"Windows Kits\10\bin"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                    @"Windows Kits\8.1\bin"),
            };

            foreach (var root in roots)
            {
                if (!Directory.Exists(root))
                {
                    continue;
                }

                // 取版本号最大的 x64 目录
                var candidates = Directory.EnumerateDirectories(root)
                    .SelectMany(d => Directory.Exists(Path.Combine(d, "x64"))
                        ? new[] { Path.Combine(d, "x64", "signtool.exe") }
                        : new string[0])
                    .Where(File.Exists)
                    .OrderByDescending(p => p, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (candidates.Count > 0)
                {
                    return candidates[0];
                }
            }

            return null;
        }

        /// <summary>对文件签名。</summary>
        /// <param name="filePath">要签名的文件。</param>
        /// <param name="options">打包选项（提供 signtool 路径、证书指纹、时间戳 URL）。</param>
        public static void Sign(string filePath, PackageBuildOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            var signTool = options.SignToolPath;
            if (string.IsNullOrEmpty(signTool))
            {
                signTool = FindSignTool();
            }

            if (string.IsNullOrEmpty(signTool) || !File.Exists(signTool))
            {
                throw new PackageBuildException(
                    "找不到 signtool.exe。请显式指定 options.SignToolPath，或从 Windows SDK 安装签名工具。");
            }

            var args = new List<string> { "sign", "/fd", "sha256", "/v" };

            if (!string.IsNullOrEmpty(options.SignCertificateThumbprint))
            {
                args.Add("/sha1");
                args.Add(options.SignCertificateThumbprint);
            }
            else
            {
                // 没有指纹就走"自动选择证书"，需要交互或已安装默认证书
                args.Add("/a");
            }

            if (!string.IsNullOrEmpty(options.SignTimestampUrl))
            {
                args.Add("/tr");
                args.Add(options.SignTimestampUrl);
                args.Add("/td");
                args.Add("sha256");
            }

            args.Add(filePath);

            var psi = new ProcessStartInfo
            {
                FileName = signTool,
                Arguments = string.Join(" ", args.Select(QuoteIfNeeded).ToArray()),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };

            var stdout = new StringBuilder();
            var stderr = new StringBuilder();

            using (var p = new Process())
            {
                p.StartInfo = psi;
                p.OutputDataReceived += (s, e) =>
                {
                    if (e.Data != null)
                    {
                        stdout.AppendLine(e.Data);
                    }
                };
                p.ErrorDataReceived += (s, e) =>
                {
                    if (e.Data != null)
                    {
                        stderr.AppendLine(e.Data);
                    }
                };

                p.Start();
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();

                // 两个流都要读，否则管道写满会死锁
                p.WaitForExit();

                if (p.ExitCode != 0)
                {
                    throw new PackageBuildException(
                        "签名失败（signtool 退出码 " + p.ExitCode + "）：\r\n" +
                        stdout + "\r\n" + stderr);
                }
            }
        }

        private static string QuoteIfNeeded(string arg)
        {
            if (arg == null)
            {
                return "\"\"";
            }

            return arg.IndexOf(' ') >= 0 ? "\"" + arg + "\"" : arg;
        }
    }
}
