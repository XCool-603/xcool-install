using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using Installer.UI.Common;

namespace Installer.UI.Views
{
    /// <summary>
    /// 许可协议页（设计稿第 3 版第 2 步）。
    ///
    /// 结构照抄设计稿：页面标题 + 副标题 + 协议正文框（浅灰底、描边、圆角 12，内含
    /// 蓝色文档图标 + 协议抬头 + 6 段条款）+ 浅灰同意条款行（勾选框 + 文案 + 灰色提示）。
    /// 未勾选时由 Presenter 让「下一步」保持禁用。
    ///
    /// 坐标全部在 LicenseView.Designer.cs 里（设计稿绝对坐标 − (56,192)）。
    /// 本文件只做两件 Designer 里不能做的事：
    ///   1. 把清单里的协议正文切段填进设计稿的 6 个条款位（Designer 里是设计稿原文的静态预览）；
    ///   2. 主色 / 事件。
    /// </summary>
    public partial class LicenseView : UserControl
    {
        /// <summary>条款正文的宽度（设计稿 536，扣掉竖向滚动条 17 保证不出横向滚动条）。</summary>
        private const int ClauseWidth = 519;

        /// <summary>条款标题行高（设计稿 18）。</summary>
        private const int ClauseTitleHeight = 18;

        /// <summary>条款标题与正文之间的间距（设计稿 4）。</summary>
        private const int ClauseTitleGap = 4;

        /// <summary>条款正文的最小高度（设计稿 2 行 = 40）。</summary>
        private const int ClauseBodyHeight = 40;

        /// <summary>条款之间的间距（设计稿 12）。</summary>
        private const int ClauseGap = 12;

        private readonly AntdUI.Label[] _clauseTitles;
        private readonly AntdUI.Label[] _clauseBodies;
        private string _licenseText = string.Empty;

        /// <summary>构造。</summary>
        public LicenseView()
        {
            InitializeComponent();

            _clauseTitles = new[]
            {
                clause1Title, clause2Title, clause3Title, clause4Title, clause5Title, clause6Title,
            };
            _clauseBodies = new[]
            {
                clause1Body, clause2Body, clause3Body, clause4Body, clause5Body, clause6Body,
            };

            HookEvents();
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
            set { lblSubtitle.Text = value; }
        }

        /// <summary>协议正文框里的抬头（设计稿：「空中小火车 最终用户许可协议」）。</summary>
        public string LicenseTitle
        {
            get { return lblCardTitle.Text; }
            set { lblCardTitle.Text = value; }
        }

        /// <summary>协议正文。按段落/编号切成条款，填进设计稿的 6 个条款位。</summary>
        public string LicenseText
        {
            get { return _licenseText; }
            set
            {
                _licenseText = value ?? string.Empty;
                RenderLicense(_licenseText);
            }
        }

        /// <summary>接受文案（勾选框右侧那一行）。</summary>
        public string AcceptText
        {
            get { return lblAccept.Text; }
            set { lblAccept.Text = value; }
        }

        /// <summary>勾选行下方的灰色提示。</summary>
        public string AcceptHint
        {
            get { return lblAcceptHint.Text; }
            set { lblAcceptHint.Text = value ?? string.Empty; }
        }

        /// <summary>是否已接受。</summary>
        public bool Accepted
        {
            get { return chkAccept.Checked; }
            set { chkAccept.Checked = value; }
        }

        /// <summary>接受状态变化。参数为新的接受状态。</summary>
        public event Action<bool> AcceptedChanged;

        /// <summary>设置主色（安装 = 蓝，卸载 = 红）。</summary>
        public void ApplyAccent(Color accent)
        {
            // 同意条款行：浅灰底（只设 AntdUI 的 Back，设 BackColor 会把圆角盖成直角）
            acceptRow.Back = WizardTheme.SoftBack;

            lblTitle.ForeColor = WizardTheme.TextTitle;
            lblSubtitle.ForeColor = WizardTheme.TextBody;
            lblCardTitle.ForeColor = WizardTheme.TextTitle;
            lblAccept.ForeColor = WizardTheme.TextTitle;
            lblAcceptHint.ForeColor = WizardTheme.TextMuted;

            for (var i = 0; i < _clauseTitles.Length; i++)
            {
                _clauseTitles[i].ForeColor = WizardTheme.TextTitle;
                _clauseBodies[i].ForeColor = WizardTheme.TextBody;
            }

            // 勾选框选中色跟随主色
            chkAccept.Fill = accent;
        }

        /// <summary>
        /// 刷新协议抬头的文案模板。key = License.CardTitle（默认「{0} 最终用户许可协议」）。
        /// 由 WizardForm 在进入许可页时调用；不调用则用 Designer 里的占位文案。
        /// </summary>
        public void ApplyCaptions(Func<string, string> lookup, string productName)
        {
            var template = Lookup(lookup, "License.CardTitle", "{0} 最终用户许可协议");
            lblCardTitle.Text = string.Format(CultureInfo.CurrentCulture, template, productName ?? string.Empty);
        }

        /// <summary>由构造函数挂接（Designer 里只订阅 WinForms 认得的事件）。</summary>
        private void HookEvents()
        {
            chkAccept.CheckedChanged += (sender, e) =>
            {
                var handler = AcceptedChanged;
                if (handler != null)
                {
                    handler(e.Value);
                }
            };

            // 点文案也等同于点勾选框
            lblAccept.Click += (sender, e) => { chkAccept.Checked = !chkAccept.Checked; };
        }

        /// <summary>
        /// 把协议正文切段后填进设计稿的 6 个条款位。
        ///
        /// 切段规则：以「1.」「2、」这类编号开头的行是条款标题，其后到下一个编号之间的行是正文；
        /// 空行也断开。切不出编号时整段当正文（标题隐藏），所以任意纯文本都不会丢字。
        /// 超过 6 段时多出来的并入第 6 段正文（条款位只有 6 个，设计稿就这么大）。
        /// 没超出时按设计稿的节奏排（标题 18 + 间距 4 + 正文 ≥40 + 间距 12 = 74）。
        /// </summary>
        private void RenderLicense(string text)
        {
            var blocks = SplitBlocks(text);

            if (blocks.Count > _clauseTitles.Length)
            {
                var last = _clauseTitles.Length - 1;
                var overflow = new StringBuilder(blocks[last].Body);
                for (var i = last + 1; i < blocks.Count; i++)
                {
                    if (overflow.Length > 0)
                    {
                        overflow.Append("\r\n\r\n");
                    }

                    if (blocks[i].Title.Length > 0)
                    {
                        overflow.Append(blocks[i].Title).Append("\r\n");
                    }

                    overflow.Append(blocks[i].Body);
                }

                blocks[last] = new LicenseBlock(blocks[last].Title, overflow.ToString());
                blocks.RemoveRange(_clauseTitles.Length, blocks.Count - _clauseTitles.Length);
            }

            var y = 0;
            for (var i = 0; i < _clauseTitles.Length; i++)
            {
                var title = _clauseTitles[i];
                var body = _clauseBodies[i];

                if (i >= blocks.Count)
                {
                    title.Visible = false;
                    body.Visible = false;
                    continue;
                }

                var block = blocks[i];
                var hasTitle = block.Title.Length > 0;

                title.Visible = hasTitle;
                title.Text = block.Title;
                title.Location = new Point(0, y);
                if (hasTitle)
                {
                    y += ClauseTitleHeight + ClauseTitleGap;
                }

                body.Visible = true;
                body.Text = block.Body;
                body.Location = new Point(0, y);
                body.Size = new Size(ClauseWidth, Math.Max(MeasureHeight(body, block.Body), ClauseBodyHeight));
                y += body.Height + ClauseGap;
            }

            clauseHost.Size = new Size(ClauseWidth, Math.Max(y - ClauseGap, 0));
        }

        private static int MeasureHeight(Control owner, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return 0;
            }

            var size = TextRenderer.MeasureText(
                text,
                owner.Font,
                new Size(ClauseWidth, int.MaxValue),
                TextFormatFlags.WordBreak | TextFormatFlags.NoPadding | TextFormatFlags.TextBoxControl);
            return size.Height + 2;
        }

        private static List<LicenseBlock> SplitBlocks(string text)
        {
            var blocks = new List<LicenseBlock>();
            if (string.IsNullOrEmpty(text))
            {
                return blocks;
            }

            var lines = text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            var buffer = new List<string>();
            foreach (var raw in lines)
            {
                var line = raw.Trim();
                if (line.Length == 0)
                {
                    Flush(blocks, buffer);
                    continue;
                }

                if (buffer.Count > 0 && IsHeading(line))
                {
                    Flush(blocks, buffer);
                }

                buffer.Add(line);
            }

            Flush(blocks, buffer);
            return blocks;
        }

        private static void Flush(List<LicenseBlock> blocks, List<string> buffer)
        {
            if (buffer.Count == 0)
            {
                return;
            }

            var start = IsHeading(buffer[0]) ? 1 : 0;
            var title = start == 1 ? buffer[0] : string.Empty;
            var body = new StringBuilder();
            for (var i = start; i < buffer.Count; i++)
            {
                if (body.Length > 0)
                {
                    body.Append(' ');
                }

                body.Append(buffer[i]);
            }

            blocks.Add(new LicenseBlock(title, body.ToString()));
            buffer.Clear();
        }

        /// <summary>「1. 授权范围」「2、使用限制」这类行是条款标题；「2 个工作日」不是。</summary>
        private static bool IsHeading(string line)
        {
            var i = 0;
            while (i < line.Length && char.IsDigit(line[i]))
            {
                i++;
            }

            if (i == 0 || i >= line.Length)
            {
                return false;
            }

            var c = line[i];
            return c == '.' || c == '、' || c == ')' || c == '）';
        }

        private static string Lookup(Func<string, string> lookup, string key, string fallback)
        {
            if (lookup == null)
            {
                return fallback;
            }

            var v = lookup(key);
            return string.IsNullOrEmpty(v) || string.Equals(v, key, StringComparison.Ordinal) ? fallback : v;
        }

        /// <summary>一段协议条款：标题（可为空）+ 正文。</summary>
        private struct LicenseBlock
        {
            public LicenseBlock(string title, string body)
            {
                Title = title;
                Body = body;
            }

            public string Title { get; private set; }

            public string Body { get; private set; }
        }
    }
}
