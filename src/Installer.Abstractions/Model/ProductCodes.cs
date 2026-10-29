using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Installer.Abstractions.Model
{
    /// <summary>
    /// 产品标识（<c>productCode</c>）的生成规则：**由启动程序名决定**。
    ///
    /// 规则很简单：
    /// <list type="bullet">
    /// <item>同一个 exe 名（不分大小写）→ 同一个 productCode；</item>
    /// <item>不同的 exe 名 → 不同的 productCode。</item>
    /// </list>
    ///
    /// 为什么这样做：productCode 是标准卸载键的键名，也是"这算不算同一个产品"的判据。
    /// 以前每个工程随机生成一个 GUID —— 用户把同一个工程另存成几个项目时，
    /// GUID 会被一起复制，于是三个不同的产品共用一个 productCode：
    /// 装第二个时被误判成"更新"，三个产品的卸载项还会互相覆盖。
    /// 改成由 exe 名推导之后，同一个产品反复打包得到同一个码（能正常升级），
    /// 不同产品天然拿到不同的码，不需要用户维护任何东西。
    /// </summary>
    public static class ProductCodes
    {
        /// <summary>从启动程序路径/文件名推导 productCode（返回 <c>{XXXXXXXX-…}</c> 形式）。</summary>
        /// <param name="entryPoint">启动程序，可带相对目录，如 <c>bin\App.exe</c>。</param>
        public static string FromEntryPoint(string entryPoint)
        {
            var stem = EntryPointStem(entryPoint);
            if (string.IsNullOrEmpty(stem))
            {
                return null;
            }

            // 用 SHA-1 取前 16 字节做 GUID：稳定、跨机器一致、不需要额外存储
            using (var sha = SHA1.Create())
            {
                var hash = sha.ComputeHash(Encoding.UTF8.GetBytes("xcool-install|" + stem.ToLowerInvariant()));
                var bytes = new byte[16];
                Array.Copy(hash, bytes, 16);

                // 按 RFC 4122 打上版本位（v5 风格），避免出现全零等怪值
                bytes[7] = (byte)((bytes[7] & 0x0F) | 0x50);
                bytes[8] = (byte)((bytes[8] & 0x3F) | 0x80);

                return "{" + new Guid(bytes).ToString("D").ToUpperInvariant() + "}";
            }
        }

        /// <summary>启动程序文件名（去扩展名）。<c>bin\App.exe</c> → <c>App</c>。</summary>
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
