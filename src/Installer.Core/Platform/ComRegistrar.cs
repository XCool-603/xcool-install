using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Installer.Abstractions.Model;

namespace Installer.Core.Platform
{
    /// <summary>COM 注册的结果。</summary>
    public sealed class ComRegistrationResult
    {
        /// <summary>是否成功。</summary>
        public bool Success { get; set; }

        /// <summary>退出码 / HRESULT。</summary>
        public int Code { get; set; }

        /// <summary>诊断信息。</summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// COM 组件注册。
    ///
    /// **现状（<c>FrmInstallation.cs:486-492</c>）的做法是错的**：
    /// 先把 DLL 复制到 <c>%SystemRoot%\System32</c>，再 <c>Regsvr32.exe /s &lt;路径&gt;</c>，
    /// 而且参数不加引号（代码注释自己写着"路径中不能有空格"），
    /// 还同时设了 <c>UseShellExecute=false</c> 和 <c>Verb="runas"</c>（后者被忽略），
    /// 异常被空 catch 吞掉后仍然打印"注册成功"。
    ///
    /// 这里提供两种正确的方式：
    /// ① <see cref="ComRegistrationModes.RegSvr32"/>（默认）：调用 regsvr32，**不复制到 System32**，
    ///    参数加引号，读掉 stdout/stderr 避免管道死锁，退出码非 0 就报错；
    /// ② <see cref="ComRegistrationModes.Direct"/>：在本进程内 LoadLibrary + DllRegisterServer，
    ///    适合 regsvr32 被安全软件拦截的环境（代价：坏 DLL 会拖垮安装器进程）。
    /// </summary>
    public static class ComRegistrar
    {
        /// <summary>regsvr32 的退出码含义。</summary>
        public static string DescribeRegSvr32Code(int code)
        {
            switch (code)
            {
                case 0:
                    return "成功";
                case 3:
                    return "找不到 DllRegisterServer / DllUnregisterServer 导出";
                case 4:
                    return "LoadLibrary 失败（不是有效 DLL，或位数不匹配）";
                case 5:
                    return "DllInstall 失败";
                default:
                    return "未知退出码 " + code;
            }
        }

        /// <summary>按指定方式注册或反注册。</summary>
        /// <param name="dllPath">DLL / OCX 的完整路径。</param>
        /// <param name="mode">注册方式。</param>
        /// <param name="register">true = 注册，false = 反注册。</param>
        public static ComRegistrationResult Apply(string dllPath, string mode, bool register)
        {
            if (string.IsNullOrWhiteSpace(dllPath))
            {
                throw new ArgumentException("dllPath 不能为空。", "dllPath");
            }

            if (!File.Exists(dllPath))
            {
                return new ComRegistrationResult
                {
                    Success = false,
                    Code = -1,
                    Message = "文件不存在：" + dllPath,
                };
            }

            if (string.Equals(mode, ComRegistrationModes.RegistrationFree, StringComparison.OrdinalIgnoreCase))
            {
                // 免注册 COM 需要在打包时从类型库导出 CLSID 并写进 <App>.exe.manifest，
                // 那是另一条实现路径；这里明确拒绝，不假装成功。
                return new ComRegistrationResult
                {
                    Success = false,
                    Code = -2,
                    Message = "免注册 COM（registrationFree）尚未实现 —— 请把清单里的 com[].mode 改成 \"" +
                              ComRegistrationModes.RegSvr32 + "\" 或 \"" + ComRegistrationModes.Direct + "\"。",
                };
            }

            if (string.Equals(mode, ComRegistrationModes.Direct, StringComparison.OrdinalIgnoreCase))
            {
                return Direct(dllPath, register);
            }

            return RegSvr32(dllPath, register);
        }

        /// <summary>
        /// 判断一个 PE 文件是 32 位还是 64 位。
        /// 返回 true = 32 位，false = 64 位，null = 读不出来。
        /// </summary>
        public static bool? Detect32Bit(string pePath)
        {
            try
            {
                using (var fs = new FileStream(pePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var head = new byte[0x40];
                    if (fs.Read(head, 0, head.Length) != head.Length)
                    {
                        return null;
                    }

                    if (head[0] != (byte)'M' || head[1] != (byte)'Z')
                    {
                        return null;
                    }

                    // e_lfanew 在 0x3C，指向 PE 签名
                    var lfanew = BitConverter.ToInt32(head, 0x3C);
                    if (lfanew <= 0 || lfanew > 1024 * 1024)
                    {
                        return null;
                    }

                    fs.Seek(lfanew, SeekOrigin.Begin);
                    var pe = new byte[6];
                    if (fs.Read(pe, 0, pe.Length) != pe.Length)
                    {
                        return null;
                    }

                    if (pe[0] != (byte)'P' || pe[1] != (byte)'E' || pe[2] != 0 || pe[3] != 0)
                    {
                        return null;
                    }

                    var machine = BitConverter.ToUInt16(pe, 4);
                    switch (machine)
                    {
                        case 0x014C:    // I386
                            return true;
                        case 0x8664:    // AMD64
                        case 0xAA64:    // ARM64
                            return false;
                        default:
                            return null;
                    }
                }
            }
            catch (IOException)
            {
                return null;
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
        }

        /// <summary>
        /// 找到与被注册组件**位数匹配**的 regsvr32。
        ///
        /// 这一步不能省：64 位 Windows 上 <c>System32\regsvr32.exe</c> 是 64 位的、
        /// <c>SysWOW64\regsvr32.exe</c> 才是 32 位的。用错了会以退出码 4 失败，
        /// 而错误信息只会说"LoadLibrary 失败"，非常难查。
        /// </summary>
        public static string FindRegSvr32(string dllPath)
        {
            var windows = Environment.GetEnvironmentVariable("SystemRoot");
            if (string.IsNullOrEmpty(windows))
            {
                windows = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            }

            var is32 = Detect32Bit(dllPath);
            var candidates = new List<string>();

            if (is32 == true)
            {
                // 目标组件是 32 位 → 必须用 SysWOW64 里那个
                candidates.Add(Path.Combine(windows, "SysWOW64", "regsvr32.exe"));
                candidates.Add(Path.Combine(windows, "System32", "regsvr32.exe"));
            }
            else if (is32 == false)
            {
                candidates.Add(Path.Combine(windows, "System32", "regsvr32.exe"));
            }
            else
            {
                // 位数读不出来：优先用本进程所在位数对应的那个
                if (IntPtr.Size == 8)
                {
                    candidates.Add(Path.Combine(windows, "System32", "regsvr32.exe"));
                }
                else
                {
                    candidates.Add(Path.Combine(windows, "SysWOW64", "regsvr32.exe"));
                    candidates.Add(Path.Combine(windows, "System32", "regsvr32.exe"));
                }
            }

            foreach (var c in candidates)
            {
                if (File.Exists(c))
                {
                    return c;
                }
            }

            return "regsvr32.exe";
        }

        /// <summary>用 regsvr32 注册（不复制到 System32）。</summary>
        public static ComRegistrationResult RegSvr32(string dllPath, bool register)
        {
            var exe = FindRegSvr32(dllPath);

            // /s 静默；路径必须加引号（现状就是漏了这一步）
            var arguments = (register ? "/s " : "/u /s ") + "\"" + dllPath + "\"";

            var psi = new ProcessStartInfo
            {
                FileName = exe,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
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

                // 两个流都要读，否则管道写满会死锁
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
                p.WaitForExit(120000);

                var code = p.ExitCode;
                var detail = (stdout.ToString() + stderr.ToString()).Trim();

                if (code == 0)
                {
                    return new ComRegistrationResult
                    {
                        Success = true,
                        Code = 0,
                        Message = "regsvr32 成功（" + Path.GetFileName(exe) + "）",
                    };
                }

                var bits = Detect32Bit(dllPath);
                var bitsText = bits == true ? "32 位" : bits == false ? "64 位" : "位数未知";

                return new ComRegistrationResult
                {
                    Success = false,
                    Code = code,
                    Message = string.Format(CultureInfo.InvariantCulture,
                        "regsvr32 退出码 {0}（{1}）；组件是 {2}，用的是 {3}{4}",
                        code, DescribeRegSvr32Code(code), bitsText, exe,
                        string.IsNullOrEmpty(detail) ? string.Empty : "；输出：" + detail),
                };
            }
        }

        // ─────────────────────────────────────────────────────────────
        // 直接调用导出（不经过 regsvr32）
        // ─────────────────────────────────────────────────────────────

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr LoadLibraryW(string fileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true, BestFitMapping = false)]
        private static extern IntPtr GetProcAddress(IntPtr module, string procName);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FreeLibrary(IntPtr module);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DllServerDelegate();

        /// <summary>在本进程内调用 DllRegisterServer / DllUnregisterServer。</summary>
        public static ComRegistrationResult Direct(string dllPath, bool register)
        {
            var export = register ? "DllRegisterServer" : "DllUnregisterServer";
            var module = LoadLibraryW(dllPath);

            if (module == IntPtr.Zero)
            {
                var err = Marshal.GetLastWin32Error();
                return new ComRegistrationResult
                {
                    Success = false,
                    Code = err,
                    Message = string.Format(CultureInfo.InvariantCulture,
                        "LoadLibrary 失败（Win32 错误 {0}）—— 常见原因是位数不匹配（32 位安装器加载 64 位组件）或缺少依赖：{1}",
                        err, dllPath),
                };
            }

            try
            {
                var proc = GetProcAddress(module, export);
                if (proc == IntPtr.Zero)
                {
                    return new ComRegistrationResult
                    {
                        Success = false,
                        Code = -3,
                        Message = "找不到导出 " + export + "：" + dllPath,
                    };
                }

                var fn = (DllServerDelegate)Marshal.GetDelegateForFunctionPointer(proc, typeof(DllServerDelegate));
                var hr = fn();

                return new ComRegistrationResult
                {
                    Success = hr == 0,
                    Code = hr,
                    Message = hr == 0
                        ? export + " 成功"
                        : string.Format(CultureInfo.InvariantCulture, "{0} 返回 HRESULT 0x{1:X8}", export, hr),
                };
            }
            finally
            {
                FreeLibrary(module);
            }
        }

        /// <summary>按清单里的路径把相对路径补成绝对路径。</summary>
        public static string Resolve(string installDir, string relativePath)
        {
            return Installer.Core.Security.SafePath.ResolveWithin(installDir, relativePath);
        }

        /// <summary>枚举清单里的 COM 组件（路径已解析为绝对路径）。</summary>
        public static List<ComSpec> ResolveAll(string installDir, InstallerManifest manifest)
        {
            var list = new List<ComSpec>();
            if (manifest == null || manifest.Com == null)
            {
                return list;
            }

            foreach (var spec in manifest.Com)
            {
                if (spec == null || string.IsNullOrWhiteSpace(spec.Path))
                {
                    continue;
                }

                list.Add(new ComSpec
                {
                    Path = Resolve(installDir, spec.Path),
                    Mode = string.IsNullOrWhiteSpace(spec.Mode) ? ComRegistrationModes.RegSvr32 : spec.Mode,
                });
            }

            return list;
        }
    }
}
