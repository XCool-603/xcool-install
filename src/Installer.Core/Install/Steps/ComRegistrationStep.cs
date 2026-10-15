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
    /// 注册清单里声明的 COM 组件。
    ///
    /// 现状（<c>FrmInstallation.cs:486-492</c>）先把 DLL 复制到 <c>System32</c> 再 regsvr32，
    /// 参数不加引号，`Verb="runas"` 与 `UseShellExecute=false` 冲突，异常被吞后仍打印"注册成功"。
    /// 这里不复制到 System32，参数加引号，失败**必须**报错。
    /// </summary>
    public sealed class ComRegistrationStep : IInstallStep
    {
        /// <inheritdoc />
        public string Id
        {
            get { return "com"; }
        }

        /// <inheritdoc />
        public string MessageKey
        {
            get { return "Install.Com"; }
        }

        /// <inheritdoc />
        public bool NeedsPayload
        {
            get { return false; }
        }

        /// <inheritdoc />
        public void Execute(InstallContext ctx)
        {
            var specs = ComRegistrar.ResolveAll(ctx.InstallDir, ctx.Manifest);
            if (specs.Count == 0)
            {
                return;
            }

            if (ctx.DryRun)
            {
                ctx.Log.Info("[试运行] 将注册 " + specs.Count + " 个 COM 组件。");
                return;
            }

            foreach (var spec in specs)
            {
                ctx.CancellationToken.ThrowIfCancellationRequested();

                // 先记 journal 再注册：即使 DllRegisterServer 写了一半才失败，回滚也会反注册
                ctx.Journal.Record(new JournalEntry
                {
                    StepId = Id,
                    Op = JournalOps.RegisterCom,
                    Target = spec.Path,
                    Extra = spec.Mode,
                });

                var result = ComRegistrar.Apply(spec.Path, spec.Mode, true);
                if (!result.Success)
                {
                    throw new InstallException(Id, "注册 COM 组件失败：" + spec.Path + "\r\n" + result.Message);
                }

                ctx.Log.Info("已注册 COM：" + spec.Path + "（" + spec.Mode + "）");
            }
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // 4. 快捷方式
}
