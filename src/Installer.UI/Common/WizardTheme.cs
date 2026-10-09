using System.Drawing;

namespace Installer.UI.Common
{
    /// <summary>
    /// 向导的视觉常量 —— 唯一事实来源。
    ///
    /// 取值依据 <c>docs/安装向导设计规格.md</c>（Calicat 设计稿第 3 版）。
    /// 改配色只改这里，不要在 Designer 里散落硬编码色值。
    /// </summary>
    internal static class WizardTheme
    {
        // ── 底色 ──────────────────────────────────────────────
        /// <summary>窗口背景（卡片外）。</summary>
        public static readonly Color WindowBack = Color.FromArgb(237, 239, 242);

        /// <summary>卡片 / 内容底。</summary>
        public static readonly Color CardBack = Color.White;

        /// <summary>信息卡 / 任务卡 / chip 底。</summary>
        public static readonly Color SoftBack = Color.FromArgb(245, 247, 250);

        // ── 主色 ──────────────────────────────────────────────
        /// <summary>品牌蓝（安装向导主色）。</summary>
        public static readonly Color Primary = Color.FromArgb(22, 119, 255);

        /// <summary>危险红（卸载向导主色）。</summary>
        public static readonly Color Danger = Color.FromArgb(213, 48, 61);

        /// <summary>成功绿（已完成 / 完成页）。</summary>
        public static readonly Color Success = Color.FromArgb(82, 196, 26);

        // ── 文字 ──────────────────────────────────────────────
        /// <summary>标题。</summary>
        public static readonly Color TextTitle = Color.FromArgb(31, 41, 55);

        /// <summary>正文。</summary>
        public static readonly Color TextBody = Color.FromArgb(75, 85, 99);

        /// <summary>次要 / 占位。</summary>
        public static readonly Color TextMuted = Color.FromArgb(156, 163, 175);

        /// <summary>品牌区上的文字（白）。</summary>
        public static readonly Color TextOnPrimary = Color.White;

        // ── 边框 ──────────────────────────────────────────────
        /// <summary>通用边框。</summary>
        public static readonly Color Border = Color.FromArgb(229, 232, 239);

        /// <summary>步骤条未到态的描边。</summary>
        public static readonly Color BorderStrong = Color.FromArgb(208, 213, 221);

        // ── 提示条 ────────────────────────────────────────────
        /// <summary>黄色提示条底。</summary>
        public static readonly Color WarnBack = Color.FromArgb(255, 251, 230);

        /// <summary>黄色提示条文字。</summary>
        public static readonly Color WarnText = Color.FromArgb(145, 120, 54);

        /// <summary>浅蓝提示条底。</summary>
        public static readonly Color InfoBack = Color.FromArgb(230, 244, 255);

        /// <summary>浅蓝提示条文字。</summary>
        public static readonly Color InfoText = Color.FromArgb(22, 119, 255);

        /// <summary>红色警告条底。</summary>
        public static readonly Color DangerBack = Color.FromArgb(255, 241, 240);

        /// <summary>绿色标签 chip 底。</summary>
        public static readonly Color SuccessBack = Color.FromArgb(240, 251, 232);

        /// <summary>按钮禁用底。</summary>
        public static readonly Color DisabledBack = Color.FromArgb(240, 242, 245);

        // ── 尺寸 ──────────────────────────────────────────────
        /// <summary>窗口宽（固定）。</summary>
        public const int WindowWidth = 688;

        /// <summary>卡片四周留白。</summary>
        public const int CardMargin = 20;

        /// <summary>卡片圆角。</summary>
        public const int CardRadius = 16;

        /// <summary>品牌区高度。</summary>
        public const int HeaderHeight = 104;

        /// <summary>步骤条高度。</summary>
        public const int StepBarHeight = 40;

        /// <summary>底栏高度。</summary>
        public const int FooterHeight = 72;

        /// <summary>按钮高。</summary>
        public const int ButtonHeight = 40;

        /// <summary>按钮圆角。</summary>
        public const int ButtonRadius = 8;

        /// <summary>按钮间距。</summary>
        public const int ButtonGap = 12;

        // ── 字体 ──────────────────────────────────────────────
        private const string Family = "微软雅黑";

        /// <summary>页面大标题 17F Bold。</summary>
        public static Font TitleFont { get { return new Font(Family, 17F, FontStyle.Bold); } }

        /// <summary>正文 12F。</summary>
        public static Font BodyFont { get { return new Font(Family, 12F); } }

        /// <summary>卡片小标题 12F Bold。</summary>
        public static Font SubTitleFont { get { return new Font(Family, 12F, FontStyle.Bold); } }

        /// <summary>信息卡标签 11F。</summary>
        public static Font LabelFont { get { return new Font(Family, 11F); } }

        /// <summary>信息卡值 / 步骤条 13F。</summary>
        public static Font ValueFont { get { return new Font(Family, 13F); } }

        /// <summary>品牌区产品名 18F Bold。</summary>
        public static Font BrandFont { get { return new Font(Family, 18F, FontStyle.Bold); } }

        /// <summary>品牌区副标题 12F。</summary>
        public static Font BrandSubFont { get { return new Font(Family, 12F); } }
    }
}
