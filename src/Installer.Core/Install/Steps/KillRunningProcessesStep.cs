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

            if (!ctx.Platform.Processes.GetRunningProcessNames()
                    .Any(n => string.Equals(n, name, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            ctx.Log.Info("检测到正在运行的进程：" + name + "，尝试结束。");
            var killed = ctx.Platform.Processes.Kill(name);
            ctx.Log.Info("已结束 " + killed + " 个进程。");

            if (killed == 0)
            {
                throw new InstallException(Id, "无法结束正在运行的程序 \"" + name + "\"，请手动关闭后重试。");
            }
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // 3. 解压 payload
}
