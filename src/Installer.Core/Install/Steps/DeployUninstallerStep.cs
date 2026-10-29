using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using Installer.Abstractions.Install;
using Installer.Abstractions.Model;
using Installer.Abstractions.Platform;
using Installer.Core.Common;
using Installer.Core.Packaging;
using Installer.Core.Platform;
using Installer.Core.Security;
using Newtonsoft.Json;

namespace Installer.Core.Install
{
    // ─────────────────────────────────────────────────────────────────

    /// <summary>
    /// 释放卸载器到安装目录。
    ///
    /// 只拷贝**自己文件的前 <see cref="InstallContext.StubLength"/> 字节**（即 stub 部分），
    /// 而不是整个安装包 —— 否则 Uninstall.exe 会白白背上几十 MB 的 payload。
    /// </summary>
    public sealed class DeployUninstallerStep : IInstallStep
    {
        /// <inheritdoc />
        public string Id
        {
            get { return "deploy-uninstaller"; }
        }

        /// <inheritdoc />
        public string MessageKey
        {
            get { return "Install.DeployUninstaller"; }
        }

        /// <inheritdoc />
        public bool NeedsPayload
        {
            get { return false; }
        }

        /// <inheritdoc />
        public void Execute(InstallContext ctx)
        {
            var source = ctx.CurrentExecutablePath;
            if (string.IsNullOrEmpty(source) || !File.Exists(source))
            {
                source = Assembly.GetEntryAssembly() != null ? Assembly.GetEntryAssembly().Location : null;
            }

            if (string.IsNullOrEmpty(source) || !File.Exists(source))
            {
                try
                {
                    source = Process.GetCurrentProcess().MainModule.FileName;
                }
                catch (InvalidOperationException)
                {
                }
            }

            if (string.IsNullOrEmpty(source) || !File.Exists(source))
            {
                // 不能只写日志就跳过 —— 那样用户装完之后安装目录里根本没有 Uninstall.exe，
                // 而且界面上看不到任何异常。这里升级成 Issue，让它显式暴露出来。
                ctx.Log.Warn("找不到安装器自身路径，跳过卸载器部署。");
                ctx.Log.Error("未能部署卸载器（" + InstallRecord.UninstallerFileName + "）：找不到安装器自身路径。");
                return;
            }

            var dest = Path.Combine(ctx.InstallDir, InstallRecord.UninstallerFileName);
            var stubLength = ctx.StubLength > 0 ? ctx.StubLength : new FileInfo(source).Length;

            if (ctx.DryRun)
            {
                return;
            }

            JournalWriter.WriteFile(ctx, Id, dest, p =>
            {
                using (var input = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var output = File.Create(p))
                {
                    var buffer = new byte[1 << 20];
                    long remaining = stubLength;
                    while (remaining > 0)
                    {
                        var want = (int)Math.Min(buffer.Length, remaining);
                        var read = input.Read(buffer, 0, want);
                        if (read <= 0)
                        {
                            break;
                        }

                        output.Write(buffer, 0, read);
                        remaining -= read;
                    }
                }
            });

            ctx.Log.Info("已部署卸载器：" + dest + "（" + stubLength + " 字节）");
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // 8. 写安装记录
}
