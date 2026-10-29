using System;
using Installer.Abstractions.Model;

namespace Installer.Core.Packaging
{
    /// <summary>
    /// 把清单里的安装目录收敛成 <c>&lt;根&gt;\&lt;厂商&gt;\&lt;产品&gt;</c>。
    ///
    /// 为什么需要单独一个类：打包有三条入口（制作端界面、无头 <c>--build</c>、CLI <c>pack</c>），
    /// 只要有一条漏了规范化，做出来的包就会带着"上一版的旧目录"——
    /// 用户实测：产品名从「空中飞翔CSVS」改成「接蛋大比拼CSVS程序」之后，
    /// 向导里显示的目录变成
    /// <c>D:\Program Files (x86)\chuangqi\空中飞翔CSVS\chuangqi\接蛋大比拼CSVS程序</c>
    /// （旧产品目录没被替换，厂商层还出现了两次）。
    ///
    /// 规则：
    /// <list type="bullet">
    /// <item>厂商/产品名取清单的**默认语言** —— 界面语言切成英文时安装目录不该跟着变；</item>
    /// <item>目录为空 → 按安装范围取根目录；</item>
    /// <item>已经是 <c>…\厂商\产品</c> 的路径原样保留（<see cref="InstallPaths.Compose"/> 幂等）。</item>
    /// </list>
    /// </summary>
    public static class InstallDirNormalizer
    {
        /// <summary>就地规范化清单的 <c>DefaultInstallDir</c> 与 <c>InstallRoot</c>。</summary>
        public static void Normalize(InstallerManifest manifest)
        {
            if (manifest == null || manifest.Product == null)
            {
                return;
            }

            var vendor = InstallPaths.SanitizeOrEmpty(
                InstallPaths.PickText(manifest.Product.Publisher, manifest.DefaultCulture, manifest.DefaultCulture));

            // 目录里的"产品名"用**启动程序名**（去扩展名）：唯一、可预期、和语言无关。
            var name = EntryPointStem(manifest.EntryPoint);
            if (string.IsNullOrWhiteSpace(name))
            {
                name = InstallPaths.PickText(manifest.Product.Name, manifest.DefaultCulture, "zh-Hans");
            }

            var root = InstallPaths.RootOf(manifest.DefaultInstallDir, vendor);
            if (string.IsNullOrWhiteSpace(root))
            {
                root = InstallPaths.Root(string.Equals(manifest.Scope, InstallScopes.PerMachine,
                    StringComparison.OrdinalIgnoreCase));
            }

            manifest.InstallRoot = root;
            manifest.DefaultInstallDir = InstallPaths.Compose(root, vendor, name);

            // 产品标识由启动程序名推导：同名同码（可正常升级），异名异码（不同产品互不干扰）
            var code = ProductCodes.FromEntryPoint(manifest.EntryPoint);
            if (!string.IsNullOrEmpty(code))
            {
                manifest.ProductCode = code;
            }
        }

        /// <summary>启动程序文件名（去扩展名）；没有入口程序返回 null。</summary>
        public static string EntryPointStem(string entryPoint)
        {
            if (string.IsNullOrWhiteSpace(entryPoint))
            {
                return null;
            }

            var file = entryPoint.Replace('/', '\\');
            var slash = file.LastIndexOf('\\');
            if (slash >= 0)
            {
                file = file.Substring(slash + 1);
            }

            var dot = file.LastIndexOf('.');
            if (dot > 0)
            {
                file = file.Substring(0, dot);
            }

            return string.IsNullOrWhiteSpace(file) ? null : file.Trim();
        }
    }
}
