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

    /// <summary>安装完成后启动入口程序（带存在性检查与异常处理）。</summary>
    public sealed class RunAfterInstallStep : IInstallStep
    {
        /// <inheritdoc />
        public string Id
        {
            get { return "run-after-install"; }
        }

        /// <inheritdoc />
        public string MessageKey
        {
            get { return "Install.Launching"; }
        }

        /// <inheritdoc />
        public bool NeedsPayload
        {
            get { return false; }
        }

        /// <inheritdoc />
        public void Execute(InstallContext ctx)
        {
            if (!ctx.Manifest.RunAfterInstall)
            {
                return;
            }

            // 图形向导把「是否启动」交给「安装完成」页的勾选框 + 完成按钮，
            // 这里必须让路 —— 否则程序会在用户还没看到完成页时就自己弹出来。
            if (ctx.SuppressAutoLaunch)
            {
                ctx.Log.Info("向导接管启动，安装管线不自动运行入口程序。");
                return;
            }

            var entry = StandardSteps.EntryPointFullPath(ctx);
            if (string.IsNullOrEmpty(entry) || !File.Exists(entry))
            {
                ctx.Log.Warn("清单要求安装后启动，但入口程序不存在：" + entry);
                return;
            }

            if (ctx.DryRun)
            {
                return;
            }

            try
            {
                ctx.Platform.Processes.Run(entry, string.Empty, false, out _);
                ctx.Log.Info("已启动：" + entry);
            }
            catch (Exception ex)
            {
                // 启动失败不该让整个安装失败 —— 文件已经装好了
                ctx.Log.Warn("启动入口程序失败（不影响安装）：" + ex.Message);
            }
        }
    }
}
