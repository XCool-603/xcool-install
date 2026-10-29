using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Installer.Abstractions.Model
{
    /// <summary>
    /// 安装目录的取值规则 —— **制作端与安装端共用同一份**。
    ///
    /// 目录层级（用户明确要求，见「安装位置要按厂商/产品分层」）：
    /// <code>
    /// %ProgramFiles%\{厂商}\{产品}
    /// %LOCALAPPDATA%\Programs\{厂商}\{产品}
    /// </code>
    /// 厂商为空时**不建**这一层，直接是 <c>{根}\{产品}</c>。
    ///
    /// 以前两边各写一套（制作端写死 <c>%LOCALAPPDATA%\Programs\我的产品</c>，
    /// 安装端在缺省时才自己拼），于是"把安装范围改成所有用户"以后，
    /// 默认目录还停在 <c>%LOCALAPPDATA%</c> —— 装出来的东西是"所有用户"的名义、
    /// 用户的目录，权限和位置都对不上。
    ///
    /// 更糟的是制作端允许把**根目录**（如 <c>D:\Program Files (x86)</c>）直接当安装目录，
    /// 于是几百个文件被平铺进用户自己的程序目录里，跟别的软件混在一起 ——
    /// 卸载时又不敢整目录删（会误伤别人的程序）。分层之后"产品目录"是**我们独占**的，
    /// 卸载才敢把它整个清掉。
    /// </summary>
    public static class InstallPaths
    {
        /// <summary>仅当前用户的安装根目录：<c>%LOCALAPPDATA%\Programs</c>，不需要管理员。</summary>
        public const string PerUserRoot = @"%LOCALAPPDATA%\Programs";

        /// <summary>所有用户的安装根目录：<c>%ProgramFiles%</c>，需要管理员。</summary>
        public const string PerMachineRoot = @"%ProgramFiles%";

        /// <summary>
        /// 仅当前用户的默认安装目录模板：<c>%LOCALAPPDATA%\Programs\{产品名}</c>。
        /// 保留给"没有厂商"的老调用方 —— 新代码请用 <see cref="Suggest(string, string, bool)"/>。
        /// </summary>
        public const string PerUserTemplate = PerUserRoot + @"\{0}";

        /// <summary>所有用户的默认安装目录模板：<c>%ProgramFiles%\{产品名}</c>。</summary>
        public const string PerMachineTemplate = PerMachineRoot + @"\{0}";

        /// <summary>占位产品名 —— 制作端新建工程时用它，打包时会被真实产品名替换。</summary>
        public const string PlaceholderProductName = "我的产品";

        /// <summary>
        /// 按安装范围给一个默认安装目录（**保留环境变量占位符，不展开** ——
        /// 展开必须发生在目标机器上，而不是打包机上）。
        /// </summary>
        /// <param name="productName">产品名，为空时用「我的产品」。</param>
        /// <param name="allUsers">true = 所有用户（Program Files），false = 仅当前用户。</param>
        public static string Suggest(string productName, bool allUsers)
        {
            return Suggest(productName, null, allUsers);
        }

        /// <summary>
        /// 按「安装范围 + 厂商 + 产品名」给出默认安装目录。
        /// 厂商为空时不建厂商层级。
        /// </summary>
        /// <param name="productName">产品名，为空时用「我的产品」。</param>
        /// <param name="vendorName">厂商名，可为空。</param>
        /// <param name="allUsers">true = 所有用户（Program Files），false = 仅当前用户。</param>
        public static string Suggest(string productName, string vendorName, bool allUsers)
        {
            return Compose(Root(allUsers), vendorName, productName);
        }

        /// <summary>安装根目录（保留 <c>%ENV%</c> 占位符）。</summary>
        /// <param name="allUsers">true = 所有用户。</param>
        public static string Root(bool allUsers)
        {
            return allUsers ? PerMachineRoot : PerUserRoot;
        }

        /// <summary>
        /// 把「根目录 + 厂商 + 产品」拼成最终安装目录。
        ///
        /// **幂等**：如果 <paramref name="rootOrFull"/> 结尾已经是 <c>[厂商\]产品</c>，
        /// 原样返回 —— 否则每次打包都会多套一层
        /// （<c>...\厂商\产品\厂商\产品</c>），而且用户看不出来。
        /// </summary>
        /// <param name="rootOrFull">根目录，或者已经是最终目录的路径。</param>
        /// <param name="vendorName">厂商名，可为空。</param>
        /// <param name="productName">产品名，为空时用「我的产品」。</param>
        public static string Compose(string rootOrFull, string vendorName, string productName)
        {
            var product = Sanitize(productName);
            var vendor = SanitizeOrEmpty(vendorName);

            var root = string.IsNullOrWhiteSpace(rootOrFull)
                ? PerUserRoot
                : rootOrFull.Trim();

            var segments = SplitSegments(root);

            // 已经是 ...\[厂商\]产品 → 不重复追加。
            // 只认"最后一层正好是产品名"这一条：多套一层（...\厂商\产品\厂商\产品）
            // 是这类拼接最典型的翻车方式，而且装完才看得出来。
            if (segments.Count > 0 &&
                string.Equals(segments[segments.Count - 1], product, StringComparison.OrdinalIgnoreCase))
            {
                return root;
            }

            var result = root.TrimEnd('\\', '/');
            if (vendor.Length > 0)
            {
                result = result + "\\" + vendor;
            }

            return result + "\\" + product;
        }

        /// <summary>
        /// 反向操作：从最终安装目录里剥掉厂商/产品两层，取出**根目录**。
        /// 制作端「安装根目录」输入框用它回显 —— 用户填的是根，看到的不该是嵌套后的全路径。
        ///
        /// 剥不动（用户手填了完全不相干的路径）就原样返回。
        /// </summary>
        /// <param name="installDir">最终安装目录（可含 %ENV%）。</param>
        /// <param name="vendorName">厂商名，可为空。</param>
        /// <param name="productName">产品名，可为空。</param>
        public static string SplitBase(string installDir, string vendorName, string productName)
        {
            if (string.IsNullOrWhiteSpace(installDir))
            {
                return installDir;
            }

            var dir = installDir.Trim();
            var product = SanitizeOrEmpty(productName);
            var vendor = SanitizeOrEmpty(vendorName);

            var segments = SplitSegments(dir);
            if (segments.Count == 0)
            {
                return dir;
            }

            var drop = 0;

            if (product.Length > 0 &&
                string.Equals(segments[segments.Count - 1], product, StringComparison.OrdinalIgnoreCase))
            {
                drop = 1;

                if (vendor.Length > 0 && segments.Count > 1 &&
                    string.Equals(segments[segments.Count - 2], vendor, StringComparison.OrdinalIgnoreCase))
                {
                    drop = 2;
                }
            }

            if (drop == 0)
            {
                return dir;
            }

            return JoinSegments(segments.Take(segments.Count - drop).ToList(), dir);
        }

        /// <summary>
        /// 判断一个目录是不是**安装根目录本身**（Program Files、%LOCALAPPDATA%\Programs…），
        /// 也就是"这里面的东西不全是我们的，不能整目录删"。
        /// </summary>
        /// <param name="dir">已展开环境变量的绝对路径。</param>
        public static bool IsSharedRoot(string dir)
        {
            if (string.IsNullOrWhiteSpace(dir))
            {
                return true;
            }

            var full = dir.Trim().TrimEnd('\\', '/');
            if (full.Length == 0)
            {
                return true;
            }

            // 盘根（C:、D:\）
            var root = Path.GetPathRoot(full);
            if (!string.IsNullOrEmpty(root) &&
                string.Equals(full.TrimEnd('\\'), root.TrimEnd('\\'), StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            var segments = SplitSegments(full);

            // 去掉"根"那一段：盘符（C:）或环境变量占位（%LOCALAPPDATA%）。
            // 之后至少还要有两层（容器\产品）才算"我们独占的产品目录"。
            var start = segments.Count > 0 && IsRootSegment(segments[0]) ? 1 : 0;
            var below = segments.Count - start;

            if (below <= 1)
            {
                return true;
            }

            var leaf = segments[segments.Count - 1];

            foreach (var name in SharedLeafNames)
            {
                if (string.Equals(leaf, name, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>是不是"根"那一段（盘符或环境变量占位）。</summary>
        private static bool IsRootSegment(string segment)
        {
            if (string.IsNullOrEmpty(segment))
            {
                return true;
            }

            return segment.EndsWith(":", StringComparison.Ordinal)
                   || segment.StartsWith("%", StringComparison.Ordinal);
        }

        /// <summary>常见的"容器目录"名字 —— 出现在这些目录里说明没分层。</summary>
        private static readonly string[] SharedLeafNames =
        {
            "Program Files",
            "Program Files (x86)",
            "Programs",
            "Windows",
            "WindowsApps",
            "Users",
            "AppData",
            "LocalAppData",
            "Roaming",
            "Local",
            "Desktop",
            "Documents",
            "Downloads",
            "Temp",
            "System32",
            "SysWOW64",
            "Common Files",
        };

        /// <summary>把产品名里不能做文件名的字符换掉。</summary>
        public static string Sanitize(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return PlaceholderProductName;
            }

            foreach (var c in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c, '_');
            }

            name = name.Trim().TrimEnd('.');
            return name.Length == 0 ? PlaceholderProductName : name;
        }

        /// <summary>厂商名消毒；为空返回空串（**不**回退成占位名，否则会凭空多一层）。</summary>
        public static string SanitizeOrEmpty(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return string.Empty;
            }

            var cleaned = name;
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                cleaned = cleaned.Replace(c, '_');
            }

            return cleaned.Trim().TrimEnd('.');
        }

        /// <summary>
        /// 从多语言文本里挑一个能用来做目录名的值：
        /// 先按首选语言，再按默认语言，最后取任意非空值。
        /// </summary>
        /// <param name="text">多语言文本（可为 null）。</param>
        /// <param name="preferredCulture">首选语言。</param>
        /// <param name="fallbackCulture">默认语言。</param>
        public static string PickText(LocalizedText text, string preferredCulture, string fallbackCulture)
        {
            if (text == null || text.Count == 0)
            {
                return string.Empty;
            }

            string value;
            if (!string.IsNullOrWhiteSpace(preferredCulture) &&
                text.TryGetValue(preferredCulture, out value) && !string.IsNullOrWhiteSpace(value))
            {
                return value.Trim();
            }

            if (!string.IsNullOrWhiteSpace(fallbackCulture) &&
                text.TryGetValue(fallbackCulture, out value) && !string.IsNullOrWhiteSpace(value))
            {
                return value.Trim();
            }

            var first = text.Values.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));
            return first == null ? string.Empty : first.Trim();
        }

        /// <summary>按 <c>\</c> / <c>/</c> 切段，丢掉空段。</summary>
        private static List<string> SplitSegments(string path)
        {
            return (path ?? string.Empty)
                .Split(new[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();
        }

        /// <summary>把段拼回去，并保留原路径的盘符风格（<c>C:\a\b</c>）。</summary>
        private static string JoinSegments(IList<string> segments, string like)
        {
            if (segments.Count == 0)
            {
                return like;
            }

            var first = segments[0];

            // "C:" 这种盘符段后面直接跟 \，不额外加分隔符
            var sb = new System.Text.StringBuilder(first);
            for (var i = 1; i < segments.Count; i++)
            {
                sb.Append('\\').Append(segments[i]);
            }

            // 环境变量开头的路径（%LOCALAPPDATA%\Programs）不带盘符，也不带前导 \
            return sb.ToString();
        }
    }
}
