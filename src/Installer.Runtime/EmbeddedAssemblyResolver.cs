using System;
using System.IO;
using System.Reflection;

namespace Installer.Runtime
{
    /// <summary>
    /// 从自身嵌入资源里加载依赖程序集，让 stub 成为真正的单文件。
    ///
    /// 思路与旧项目里那份从未被调用的 <c>InstalltionEdit\LoadResourcesDll.cs</c> 相同，
    /// 但这次是真的被调用了。
    /// </summary>
    internal static class EmbeddedAssemblyResolver
    {
        private const string ResourcePrefix = "Installer.Runtime.Deps.";
        private static bool _registered;

        /// <summary>注册 AssemblyResolve 处理器。必须在触碰任何依赖类型**之前**调用。</summary>
        public static void Register()
        {
            if (_registered)
            {
                return;
            }

            _registered = true;
            AppDomain.CurrentDomain.AssemblyResolve += Resolve;
        }

        private static Assembly Resolve(object sender, ResolveEventArgs args)
        {
            var simpleName = new AssemblyName(args.Name).Name;
            if (string.IsNullOrEmpty(simpleName))
            {
                return null;
            }

            var resourceName = ResourcePrefix + simpleName + ".dll";
            var self = Assembly.GetExecutingAssembly();

            using (var stream = self.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    return null;
                }

                var bytes = new byte[stream.Length];
                var offset = 0;
                while (offset < bytes.Length)
                {
                    var read = stream.Read(bytes, offset, bytes.Length - offset);
                    if (read <= 0)
                    {
                        break;
                    }

                    offset += read;
                }

                return Assembly.Load(bytes);
            }
        }
    }
}
