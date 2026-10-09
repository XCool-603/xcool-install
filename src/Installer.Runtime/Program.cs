using System;
using System.Runtime.CompilerServices;

namespace Installer.Runtime
{
    /// <summary>
    /// 入口。**这里刻意不引用任何依赖类型**：先注册 AssemblyResolve，再跳到
    /// <see cref="InstallHost"/>。否则 JIT 可能在 Register 之前就去解析 Installer.Core，
    /// 而那时嵌入资源还没被登记。
    /// </summary>
    internal static class Program
    {
        [STAThread]
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static int Main(string[] args)
        {
            EmbeddedAssemblyResolver.Register();
            return InstallHost.Run(args);
        }
    }
}
