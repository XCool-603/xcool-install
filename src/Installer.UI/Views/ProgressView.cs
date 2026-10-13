using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Installer.UI.Common;

namespace Installer.UI.Views
{
    /// <summary>
    /// 正在安装 / 正在卸载页（设计稿「正在安装3」/「正在卸载」）。
    ///
    /// 结构：标题 + 副标题 + 进度行（圆弧图标 + 阶段名 + 百分比）+ 进度条
    ///       + 统计卡（当前文件 + 4 列数字）+ 4 行步骤状态（图标 + 文案）。
    /// 卸载向导复用同一页，只把主色换成红、数字列改成「已删除 / 已释放」。
    ///
    /// 逐元素坐标见 ProgressView.Designer.cs 顶部的换算表。
    /// </summary>
    public partial class ProgressView : UserControl
    {
        /// <summary>已完成行的配色。</summary>
        private static readonly Color DoneColor = WizardTheme.Success;

        /// <summary>未开始行的配色。</summary>
        private static readonly Color IdleColor = WizardTheme.TextMuted;

        private Color _accent = WizardTheme.Primary;

        /// <summary>构造。</summary>
        public ProgressView()
        {
            InitializeComponent();
            ApplyAccent(WizardTheme.Primary);
        }

        /// <summary>标题。</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Title
        {
            get { return lblTitle.Text; }
            set { lblTitle.Text = value; }
        }

        /// <summary>副标题。</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Subtitle
        {
            get { return lblSubtitle.Text; }
            set { lblSubtitle.Text = value ?? string.Empty; }
        }

        /// <summary>当前阶段名（兼容旧名 StepText）。</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string StepText
        {
            get { return lblPhase.Text; }
            set { lblPhase.Text = value ?? string.Empty; }
        }

        /// <summary>统计卡的「正在写入：xxx」那行。</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string CurrentFileText
        {
            get { return lblCurrentFile.Text; }
            set { lblCurrentFile.Text = value ?? string.Empty; }
        }

        /// <summary>设置 4 个统计值。</summary>
        public void SetStats(string v1, string v2, string v3, string v4)
        {
            val1.Text = v1 ?? "—";
            val2.Text = v2 ?? "—";
            val3.Text = v3 ?? "—";
            val4.Text = v4 ?? "—";
        }

        /// <summary>设置 4 个统计列的标签文案。</summary>
        public void SetStatCaptions(string c1, string c2, string c3, string c4)
        {
            cap1.Text = c1;
            cap2.Text = c2;
            cap3.Text = c3;
            cap4.Text = c4;
        }

        /// <summary>设置 4 行步骤状态。</summary>
        /// <param name="titles">4 行文案。</param>
        /// <param name="currentIndex">进行中的行号（0 起）；-1 表示没有进行中的行。</param>
        public void SetSteps(string[] titles, int currentIndex)
        {
            var icons = new[] { stepIcon1, stepIcon2, stepIcon3, stepIcon4 };
            var labels = new[] { step1, step2, step3, step4 };

            for (var i = 0; i < labels.Length; i++)
            {
                labels[i].Text = titles != null && i < titles.Length ? titles[i] : string.Empty;

                var done = i < currentIndex;
                var active = i == currentIndex;

                labels[i].ForeColor = done ? DoneColor : (active ? _accent : IdleColor);
                icons[i].PrefixSvg = active ? Spinner(_accent) : (done ? Check() : Circle());
            }
        }

        /// <summary>设置百分比（0~100）。</summary>
        public void SetPercent(int percent)
        {
            var v = Math.Max(0, Math.Min(100, percent));
            progressBar.Value = v / 100F;

            // 进度条自身不画文字：AntdUI.Progress 在「有文字」时会预留文字宽度并把条截短。
            // 只有 Text=null 且 UseSystemText=true 才走「无文字」分支（空串不算 null）。
            // 百分比已经画在进度行右侧的 lblPercent 上了。
            progressBar.Text = null;

            lblPercent.Text = v + "%";
        }

        /// <summary>是否处于不确定进度（转圈）。</summary>
        public void SetIndeterminate(bool on)
        {
            progressBar.Loading = on;
        }

        /// <summary>是否出错（进度条转红）。</summary>
        public void SetError(bool on)
        {
            progressBar.State = on ? AntdUI.TType.Error : AntdUI.TType.None;
        }

        /// <summary>兼容旧调用：设计稿里已没有日志框，这里忽略。</summary>
        public void AppendLog(string line)
        {
        }

        /// <summary>设置主色（安装 = 蓝，卸载 = 红）。</summary>
        public void ApplyAccent(Color accent)
        {
            _accent = accent;

            statsCard.Back = WizardTheme.SoftBack;

            lblTitle.ForeColor = WizardTheme.TextTitle;
            lblSubtitle.ForeColor = WizardTheme.TextBody;
            lblPhase.ForeColor = accent;
            lblPercent.ForeColor = accent;
            progressBar.Fill = accent;
            iconPhase.PrefixSvg = Spinner(accent);
        }

        private static string Check()
        {
            return "<svg viewBox=\"0 0 16 16\"><circle cx=\"8\" cy=\"8\" r=\"7\" fill=\"#52C41A\"/>"
                 + "<path d=\"M4.9 8.2l2.1 2.1 4.2-4.3\" fill=\"none\" stroke=\"#FFFFFF\" stroke-width=\"1.6\" stroke-linecap=\"round\" stroke-linejoin=\"round\"/></svg>";
        }

        private static string Circle()
        {
            return "<svg viewBox=\"0 0 16 16\"><circle cx=\"8\" cy=\"8\" r=\"6.6\" fill=\"none\" stroke=\"#D0D5DD\" stroke-width=\"1.5\"/></svg>";
        }

        private static string Spinner(Color accent)
        {
            var hex = string.Format("#{0:X2}{1:X2}{2:X2}", accent.R, accent.G, accent.B);
            return "<svg viewBox=\"0 0 24 24\"><path d=\"M12 3a9 9 0 1 0 9 9\" fill=\"none\" stroke=\""
                 + hex + "\" stroke-width=\"2.2\" stroke-linecap=\"round\"/></svg>";
        }
    }
}
