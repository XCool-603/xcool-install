using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Installer.UI.Common;

namespace Installer.UI.Views
{
    /// <summary>
    /// 向导步骤条：圆点 + 文字 + 连线。
    ///
    /// 三种状态（设计稿取值）：
    /// <list type="bullet">
    /// <item>已完成 —— 实心绿圆 + 白色勾，文字深灰</item>
    /// <item>当前   —— 实心主色圆 + 白色数字，文字主色加粗</item>
    /// <item>未到   —— 白底 + 1px 灰描边 + 灰数字，文字浅灰</item>
    /// </list>
    ///
    /// 为什么自绘：AntdUI 2.4.3 的 <c>Steps</c> 控件做不出「圆点 + 长连线 + 文字右置」
    /// 这套设计，而自绘只有 60 行，可控性高得多。
    /// </summary>
    public class StepBarView : Control
    {
        private string[] _steps = new string[0];
        private int _current;

        private Color _accent = WizardTheme.Primary;

        /// <summary>构造。</summary>
        public StepBarView()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                     | ControlStyles.OptimizedDoubleBuffer
                     | ControlStyles.UserPaint
                     | ControlStyles.ResizeRedraw, true);

            BackColor = WizardTheme.CardBack;
            Font = WizardTheme.BodyFont;   // 13px → 9.75pt
            Height = WizardTheme.StepBarHeight;
        }

        /// <summary>设置步骤标题。</summary>
        public string[] Steps
        {
            get { return _steps; }
            set
            {
                _steps = value ?? new string[0];
                Invalidate();
            }
        }

        /// <summary>当前步骤（0 起）。</summary>
        public int Current
        {
            get { return _current; }
            set
            {
                _current = value;
                Invalidate();
            }
        }

        /// <summary>主色（安装 = 蓝，卸载 = 红）。</summary>
        public Color Accent
        {
            get { return _accent; }
            set
            {
                _accent = value;
                Invalidate();
            }
        }

        /// <summary>自绘。</summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;

            // UserPaint + AllPaintingInWmPaint 下必须自己填背景，否则透出父容器的灰
            using (var bg = new SolidBrush(BackColor))
            {
                g.FillRectangle(bg, ClientRectangle);
            }

            if (_steps.Length == 0)
            {
                return;
            }
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            const int pad = 32;   // 设计稿：步骤条 padding [20,32,20,32]
            const int dot = 24;   // 设计稿：步骤标记 24x24
            const int dotGap = 8;
            const int lineGap = 12;   // 连接线 wrapper padding-left 12
            const int minLine = 12;

            // ① 量出每段「圆 + 文字」的宽度
            var textWidths = new int[_steps.Length];
            var total = 0;
            for (var i = 0; i < _steps.Length; i++)
            {
                var bold = i == _current ? FontStyle.Bold : FontStyle.Regular;
                using (var f = new Font(Font, bold))
                {
                    textWidths[i] = TextRenderer.MeasureText(g, _steps[i], f).Width;
                }

                total += dot + dotGap + textWidths[i];
            }

            // ② 剩余空间平均分给 n-1 条连线
            var slots = Math.Max(1, _steps.Length - 1);
            var avail = Math.Max(0, Width - pad * 2 - total - lineGap * 2 * slots);
            var lineLen = Math.Max(minLine, avail / slots);

            var cy = Height / 2;
            var x = pad;

            for (var i = 0; i < _steps.Length; i++)
            {
                var done = i < _current;
                var active = i == _current;

                // 圆
                var dotRect = new Rectangle(x, cy - dot / 2, dot, dot);

                if (done)
                {
                    using (var b = new SolidBrush(WizardTheme.Success))
                    {
                        g.FillEllipse(b, dotRect);
                    }

                    DrawCheck(g, dotRect);
                }
                else if (active)
                {
                    using (var b = new SolidBrush(_accent))
                    {
                        g.FillEllipse(b, dotRect);
                    }

                    DrawNumber(g, dotRect, i + 1, Color.White);
                }
                else
                {
                    using (var b = new SolidBrush(Color.White))
                    {
                        g.FillEllipse(b, dotRect);
                    }

                    using (var p = new Pen(WizardTheme.BorderStrong, 1F))
                    {
                        g.DrawEllipse(p, dotRect);
                    }

                    DrawNumber(g, dotRect, i + 1, WizardTheme.TextMuted);
                }

                x += dot + dotGap;

                // 文字
                var fore = done || active
                    ? (active ? _accent : WizardTheme.TextBody)
                    : WizardTheme.TextMuted;

                var style = active ? FontStyle.Bold : FontStyle.Regular;
                using (var f = new Font(Font, style))
                using (var b = new SolidBrush(fore))
                {
                    var size = g.MeasureString(_steps[i], f);
                    g.DrawString(_steps[i], f, b, x, cy - size.Height / 2);
                }

                x += textWidths[i];

                // 连线
                if (i < _steps.Length - 1)
                {
                    x += lineGap;
                    var color = i < _current ? WizardTheme.Success : WizardTheme.Border;
                    using (var p = new Pen(color, 2F))   // 设计稿：连接线高 2
                    {
                        g.DrawLine(p, x, cy, x + lineLen, cy);
                    }

                    x += lineLen + lineGap;
                }
            }
        }

        private static void DrawCheck(Graphics g, Rectangle dot)
        {
            using (var p = new Pen(Color.White, 2F))
            {
                p.StartCap = LineCap.Round;
                p.EndCap = LineCap.Round;

                var cx = dot.X + dot.Width / 2F;
                var cy = dot.Y + dot.Height / 2F;

                g.DrawLines(p, new[]
                {
                    new PointF(cx - 5F, cy),
                    new PointF(cx - 1.5F, cy + 3.5F),
                    new PointF(cx + 5F, cy - 4F)
                });
            }
        }

        private static void DrawNumber(Graphics g, Rectangle dot, int number, Color color)
        {
            using (var f = new Font("微软雅黑", 9F, FontStyle.Bold))
            using (var b = new SolidBrush(color))
            using (var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
            {
                g.DrawString(number.ToString(), f, b, dot, sf);
            }
        }
    }
}
