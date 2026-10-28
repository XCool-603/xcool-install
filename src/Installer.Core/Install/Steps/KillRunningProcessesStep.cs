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

    /// <summary>结束同名进程，避免文件占用。按 **exe 名**匹配（现状用产品显示名，基本匹配不到）。</summary>
    public sealed class KillRunningProcessesStep : IInstallStep
    {
        /// <inheritdoc />
        public string Id
        {
            get { return "kill"; }
        }

        /// <inheritdoc />
        public string MessageKey
        {
            get { return "Install.KillRunning"; }
        }

        /// <inheritdoc />
        public bool NeedsPayload
        {
            get { return false; }
        }

        /// <inheritdoc />
        public void Execute(InstallContext ctx)
        {
            var exe = ctx.Manifest.EntryPoint;
            if (string.IsNullOrEmpty(exe))
            {
                return;
            }

            var name = Path.GetFileNameWithoutExtension(exe);
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            // 若安装目录尚不存在（首次全新安装），不需要杀任何进程，更避免误杀同名系统/其他目录程序
            if (!Directory.Exists(ctx.InstallDir))
            {
                return;
            }

            if (!ctx.Platform.Processes.GetRunningProcessNames()
                    .Any(n => string.Equals(n, name, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            ctx.Log.Info("检测到正在运行的进程：" + name + "，尝试结束安装目录内的实例。");
            var killed = ctx.Platform.Processes.KillInDirectory(name, ctx.InstallDir);
            ctx.Log.Info("已结束 " + killed + " 个安装目录内的进程。");
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // 3. 解压 payload
}
