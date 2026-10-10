using System;
using System.Drawing;
using System.Windows.Forms;
using Installer.UI.Common;

namespace Installer.UI.Views
{
    /// <summary>
    /// 正在安装 / 正在卸载页（设计稿第 4 步 / 卸载第 2 步）。
    ///
    /// 结构：标题 + 副标题 + 进度行（阶段名 + 百分比）+ 进度条
    ///       + 统计卡（当前文件 + 4 列数字）+ 4 行步骤状态。
    /// 卸载向导复用同一页，只把主色换成红、数字列改成「已删除 / 已释放」。
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
        public string Title
        {
            get { return lblTitle.Text; }
            set { lblTitle.Text = value; }
        }

        /// <summary>副标题。</summary>
        public string Subtitle
        {
            get { return lblSubtitle.Text; }
            set { lblSubtitle.Text = value ?? string.Empty; }
        }

        /// <summary>当前阶段名（兼容旧名 StepText）。</summary>
        public string StepText
        {
            get { return lblPhase.Text; }
            set { lblPhase.Text = value ?? string.Empty; }
        }

        /// <summary>统计卡的「正在写入：xxx」那行。</summary>
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
            var labels = new[] { step1, step2, step3, step4 };

            for (var i = 0; i < labels.Length; i++)
            {
                labels[i].Text = titles != null && i < titles.Length ? titles[i] : string.Empty;

                var done = i < currentIndex;
                var active = i == currentIndex;

                labels[i].ForeColor = done ? DoneColor : (active ? _accent : IdleColor);
                labels[i].PrefixSvg = active ? Spinner() : (done ? Check() : Circle());
            }
        }

        /// <summary>设置百分比（0~100）。</summary>
        public void SetPercent(int percent)
        {
            var v = Math.Max(0, Math.Min(100, percent));
            progressBar.Value = v / 100F;
            progressBar.Text = string.Empty;   // 进度行已经有百分比了，别重复
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
        }

        private static string Check()
        {
            return "<svg viewBox=\"0 0 16 16\"><circle cx=\"8\" cy=\"8\" r=\"7\" fill=\"#52C41A\"/>"
                 + "<path d=\"M4.8 8.2l2.1 2.1 4.3-4.4\" fill=\"none\" stroke=\"#FFFFFF\" stroke-width=\"1.7\" stroke-linecap=\"round\" stroke-linejoin=\"round\"/></svg>";
        }

        private static string Circle()
        {
            return "<svg viewBox=\"0 0 16 16\"><circle cx=\"8\" cy=\"8\" r=\"6.4\" fill=\"none\" stroke=\"#9CA3AF\" stroke-width=\"1.4\"/></svg>";
        }

        private string Spinner()
        {
            var hex = string.Format("#{0:X2}{1:X2}{2:X2}", _accent.R, _accent.G, _accent.B);
            return "<svg viewBox=\"0 0 16 16\"><path d=\"M8 2.6a5.4 5.4 0 1 0 5.4 5.4\" fill=\"none\" stroke=\""
                 + hex + "\" stroke-width=\"1.7\" stroke-linecap=\"round\"/></svg>";
        }
    }
}
