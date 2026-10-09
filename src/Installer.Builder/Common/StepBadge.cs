namespace Installer.Builder.Common
{
    /// <summary>
    /// 步骤徽章：主色圆角方块 + 白色数字，用 SVG 直接画。
    ///
    /// 为什么要手写 SVG：AntdUI 2.4.3 的 <c>SvgDb</c> 只有 35 个 <c>Ico*</c> 常量，
    /// 没有官方图标集（<c>HomeOutlined</c> 那套在 2.4.3 里不存在）。
    ///
    /// 用 <c>Label.PrefixSvg</c> 承载的好处是**一个控件就够**，
    /// 不必再套一层 Panel + Label。
    /// </summary>
    internal static class StepBadge
    {
        /// <summary>激活态徽章底色（主色）。</summary>
        public const string ActiveFill = "#1677FF";

        /// <summary>淡化态徽章底色（设计稿取值）。</summary>
        public const string DimmedFill = "#9DC5FC";

        /// <summary>生成徽章 SVG。</summary>
        /// <param name="number">步骤号（1/2/3）。</param>
        /// <param name="dimmed">是否淡化。</param>
        public static string Create(int number, bool dimmed)
        {
            var fill = dimmed ? DimmedFill : ActiveFill;

            return "<svg viewBox=\"0 0 20 20\">"
                 + "<rect width=\"20\" height=\"20\" rx=\"6\" fill=\"" + fill + "\"/>"
                 + "<text x=\"10\" y=\"15\" font-size=\"13\" fill=\"#FFFFFF\" text-anchor=\"middle\">"
                 + number
                 + "</text></svg>";
        }
    }
}
