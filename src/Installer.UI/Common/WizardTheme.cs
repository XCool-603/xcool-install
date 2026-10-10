using System.Drawing;

namespace Installer.UI.Common
{
    /// <summary>
    /// 向导的视觉常量 —— 唯一事实来源。
    ///
    /// 数值全部来自 Calicat 设计稿（第 3 版）的 get_design_data 原始输出，
    /// 见 docs/安装向导设计规格.md。改配色只改这里。
    ///
    /// 单位约定（最容易搞错的地方）：
    ///   尺寸类（Size / Location / Padding / Margin / 圆角）在 WinForms 里就是像素，
    ///   直接照抄设计稿的 px。
    ///   只有 Font 用 pt，换算公式 pt = px × 0.75（96 DPI）。
    /// </summary>
    internal static class WizardTheme
    {
        // ── 底色 ──────────────────────────────────────────────
        /// <summary>窗口背景（卡片外）。设计稿：页面底 #F5F7FA。</summary>
        public static readonly Color WindowBack = Color.FromArgb(245, 247, 250);

        /// <summary>卡片 / 内容底。</summary>
        public static readonly Color CardBack = Color.White;

        /// <summary>信息卡 / 任务卡 / chip 底。</summary>
        public static readonly Color SoftBack = Color.FromArgb(245, 247, 250);

        // ── 主色 ──────────────────────────────────────────────
        /// <summary>品牌蓝（安装向导主色）。</summary>
        public static readonly Color Primary = Color.FromArgb(22, 119, 255);

        /// <summary>汇总条文字用的深蓝。</summary>
        public static readonly Color PrimaryDeep = Color.FromArgb(9, 88, 217);

        /// <summary>浅蓝底（信息卡图标、汇总条）。</summary>
        public static readonly Color PrimarySoft = Color.FromArgb(230, 244, 255);

        /// <summary>危险红（卸载向导主色）。</summary>
        public static readonly Color Danger = Color.FromArgb(213, 48, 61);

        /// <summary>成功绿（已完成 / 完成页）。</summary>
        public static readonly Color Success = Color.FromArgb(82, 196, 26);

        // ── 文字 ──────────────────────────────────────────────
        /// <summary>标题 #1F2937。</summary>
        public static readonly Color TextTitle = Color.FromArgb(31, 41, 55);

        /// <summary>正文 #4B5563。</summary>
        public static readonly Color TextBody = Color.FromArgb(75, 85, 99);

        /// <summary>次要 / 占位 #9CA3AF。</summary>
        public static readonly Color TextMuted = Color.FromArgb(156, 163, 175);

        /// <summary>品牌区上的文字（白）。</summary>
        public static readonly Color TextOnPrimary = Color.White;

        // ── 边框 ──────────────────────────────────────────────
        /// <summary>通用边框 #E5E7EB。</summary>
        public static readonly Color Border = Color.FromArgb(229, 231, 235);

        /// <summary>步骤条未到态的描边 #D0D5DD。</summary>
        public static readonly Color BorderStrong = Color.FromArgb(208, 213, 221);

        // ── 提示条 ────────────────────────────────────────────
        /// <summary>黄色提示条底 #FFFBEB。</summary>
        public static readonly Color WarnBack = Color.FromArgb(255, 251, 235);

        /// <summary>黄色提示条文字 #7C6016。</summary>
        public static readonly Color WarnText = Color.FromArgb(124, 96, 22);

        /// <summary>黄色提示条图标 #FAAD14。</summary>
        public static readonly Color WarnIcon = Color.FromArgb(250, 173, 20);

        /// <summary>浅蓝提示条底 #E6F4FF。</summary>
        public static readonly Color InfoBack = Color.FromArgb(230, 244, 255);

        /// <summary>浅蓝提示条文字 #0958D9。</summary>
        public static readonly Color InfoText = Color.FromArgb(9, 88, 217);

        /// <summary>红色警告条底。</summary>
        public static readonly Color DangerBack = Color.FromArgb(255, 241, 240);

        /// <summary>绿色标签 chip 底。</summary>
        public static readonly Color SuccessBack = Color.FromArgb(240, 251, 232);

        /// <summary>按钮禁用底。</summary>
        public static readonly Color DisabledBack = Color.FromArgb(240, 242, 245);

        // ── 尺寸（px，直接照抄设计稿）────────────────────────
        /// <summary>窗口宽。</summary>
        public const int WindowWidth = 688;

        /// <summary>窗口四周留白 → 卡片宽 688-24*2 = 640。</summary>
        public const int WindowPadding = 24;

        /// <summary>卡片宽。</summary>
        public const int CardWidth = 640;

        /// <summary>卡片圆角。</summary>
        public const int CardRadius = 16;

        /// <summary>品牌区高度。</summary>
        public const int HeaderHeight = 104;

        /// <summary>品牌区左右留白。</summary>
        public const int HeaderPadding = 32;

        /// <summary>应用图标容器尺寸（56×56，圆角 16）。</summary>
        public const int BrandIconSize = 56;

        /// <summary>步骤条高度。</summary>
        public const int StepBarHeight = 64;

        /// <summary>步骤标记尺寸（24×24，圆角 12）。</summary>
        public const int StepDotSize = 24;

        /// <summary>底栏高度。</summary>
        public const int FooterHeight = 72;

        /// <summary>内容区左右留白。</summary>
        public const int ContentPadding = 32;

        /// <summary>按钮高。</summary>
        public const int ButtonHeight = 40;

        /// <summary>按钮圆角。</summary>
        public const int ButtonRadius = 8;

        /// <summary>按钮之间的间距。</summary>
        public const int ButtonGap = 12;

        // ── 内层圆角 ──────────────────────────────────────────
        /// <summary>信息卡圆角。</summary>
        public const int InfoCardRadius = 14;

        /// <summary>提示条圆角。</summary>
        public const int HintRadius = 10;

        /// <summary>输入框 / 任务卡 / chip 圆角。</summary>
        public const int FieldRadius = 8;

        // ── 字体（pt = 设计 px × 0.75）────────────────────────
        private const string Family = "微软雅黑";

        /// <summary>品牌产品名 20px → 15pt SemiBold。</summary>
        public static Font BrandFont { get { return new Font(Family, 15F, FontStyle.Bold); } }

        /// <summary>品牌副标题 13px → 9.75pt。</summary>
        public static Font BrandSubFont { get { return new Font(Family, 9.75F); } }

        /// <summary>页面大标题 18px → 13.5pt SemiBold。</summary>
        public static Font TitleFont { get { return new Font(Family, 13.5F, FontStyle.Bold); } }

        /// <summary>区块标题 14px → 10.5pt SemiBold。</summary>
        public static Font SectionFont { get { return new Font(Family, 10.5F, FontStyle.Bold); } }

        /// <summary>正文 13px → 9.75pt。</summary>
        public static Font BodyFont { get { return new Font(Family, 9.75F); } }

        /// <summary>正文加粗（步骤条当前项）13px → 9.75pt。</summary>
        public static Font BodyBoldFont { get { return new Font(Family, 9.75F, FontStyle.Bold); } }

        /// <summary>辅助文字 12px → 9pt。</summary>
        public static Font CaptionFont { get { return new Font(Family, 9F); } }

        /// <summary>按钮文字 14px → 10.5pt。</summary>
        public static Font ButtonFont { get { return new Font(Family, 10.5F); } }

        /// <summary>按钮文字加粗（主按钮）14px → 10.5pt SemiBold。</summary>
        public static Font ButtonBoldFont { get { return new Font(Family, 10.5F, FontStyle.Bold); } }

        /// <summary>步骤标记里的数字 12px → 9pt。</summary>
        public static Font StepNumberFont { get { return new Font(Family, 9F, FontStyle.Bold); } }
    }
}
