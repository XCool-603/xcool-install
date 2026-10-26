using System.Globalization;
using System.IO;

namespace Installer.Abstractions.Model
{
    /// <summary>
    /// 默认安装目录的取值规则 —— **制作端与安装端共用同一份**。
    ///
    /// 以前两边各写一套（制作端写死 <c>%LOCALAPPDATA%\Programs\我的产品</c>，
    /// 安装端在缺省时才自己拼），于是"把安装范围改成所有用户"以后，
    /// 默认目录还停在 <c>%LOCALAPPDATA%</c> —— 装出来的东西是"所有用户"的名义、
    /// 用户的目录，权限和位置都对不上。
    /// </summary>
    public static class InstallPaths
    {
        /// <summary>仅当前用户：<c>%LOCALAPPDATA%\Programs\&lt;产品名&gt;</c>，不需要管理员。</summary>
        public const string PerUserTemplate = @"%LOCALAPPDATA%\Programs\{0}";

        /// <summary>所有用户：<c>%ProgramFiles%\&lt;产品名&gt;</c>，需要管理员。</summary>
        public const string PerMachineTemplate = @"%ProgramFiles%\{0}";

        /// <summary>
        /// 按安装范围给一个默认安装目录（**保留环境变量占位符，不展开** ——
        /// 展开必须发生在目标机器上，而不是打包机上）。
        /// </summary>
        /// <param name="productName">产品名，为空时用「我的产品」。</param>
        /// <param name="allUsers">true = 所有用户（Program Files），false = 仅当前用户。</param>
        public static string Suggest(string productName, bool allUsers)
        {
            var name = string.IsNullOrWhiteSpace(productName) ? "我的产品" : productName.Trim();
            return string.Format(CultureInfo.InvariantCulture,
                allUsers ? PerMachineTemplate : PerUserTemplate, Sanitize(name));
        }

        /// <summary>把产品名里不能做文件名的字符换掉。</summary>
        public static string Sanitize(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return "我的产品";
            }

            foreach (var c in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c, '_');
            }

            name = name.Trim().TrimEnd('.');
            return name.Length == 0 ? "我的产品" : name;
        }
    }
}
