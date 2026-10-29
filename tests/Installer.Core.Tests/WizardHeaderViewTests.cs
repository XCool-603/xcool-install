using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using Installer.UI.Views;
using Xunit;

namespace Installer.Core.Tests
{
    /// <summary>
    /// 向导顶部品牌区的配色。
    ///
    /// 用户反馈三件事：
    /// ① 安装向导的标题是**黑字**（蓝底黑字，看不清）→ 必须白字；
    /// ② 图标块有一圈**白边**（AntdUI.Panel 默认 1px 描边）→ 不能有描边；
    /// ③ 卸载向导的标题区还是蓝色 → 要换成删除红，文字换成红底上合适的颜色。
    /// </summary>
    public class WizardHeaderViewTests
    {
        private static readonly Color Primary = Color.FromArgb(22, 119, 255);
        private static readonly Color Danger = Color.FromArgb(213, 48, 61);

        private static T Field<T>(object view, string name)
        {
            var f = view.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(f);
            return (T)f.GetValue(view);
        }

        [Fact]
        public void 安装向导头部应是蓝底白字且图标无描边()
        {
            var view = new WizardHeaderView();
            view.ApplyAccent(Primary);
            view.ApplyText(null, "测试产品", "1.0.0", false);

            var brand = Field<AntdUI.Panel>(view, "brandPanel");
            var icon = Field<AntdUI.Panel>(view, "iconPanel");
            var product = Field<AntdUI.Label>(view, "lblProduct");
            var subtitle = Field<AntdUI.Label>(view, "lblSubtitle");

            Assert.Equal(Primary, brand.Back.Value);
            Assert.Equal(Color.White, product.ForeColor);
            Assert.Equal(0f, icon.BorderWidth);

            // 副标题比产品名淡，但不能淡到看不见
            Assert.NotEqual(product.ForeColor, subtitle.ForeColor);
            var sub = subtitle.ForeColor.Value;
            Assert.True(sub.R > 180 && sub.G > 180 && sub.B > 180,
                "副标题太暗：" + sub);

            Assert.Contains("安装向导", subtitle.Text);
        }

        [Fact]
        public void 卸载向导头部应是删除红底白字()
        {
            var view = new WizardHeaderView();
            view.ApplyAccent(Danger);
            view.ApplyText(null, "测试产品", "1.0.0", true);

            var brand = Field<AntdUI.Panel>(view, "brandPanel");
            var icon = Field<AntdUI.Panel>(view, "iconPanel");
            var product = Field<AntdUI.Label>(view, "lblProduct");
            var subtitle = Field<AntdUI.Label>(view, "lblSubtitle");

            Assert.Equal(Danger, brand.Back.Value);
            Assert.Equal(Color.White, product.ForeColor);
            Assert.Equal(0f, icon.BorderWidth);

            // 副标题在红底上要偏亮（白 78% 叠红）
            var sub = subtitle.ForeColor.Value;
            Assert.True(sub.R > 200 && sub.G > 150,
                "副标题在红底上不够亮：" + sub);

            Assert.Contains("卸载向导", subtitle.Text);
        }

        [Fact]
        public void 安装模式画安装包图形且隐藏占位方块()
        {
            var view = new WizardHeaderView();

            view.ApplyGlyph(false);

            var pic = Field<System.Windows.Forms.PictureBox>(view, "picIcon");
            var placeholder = Field<AntdUI.Label>(view, "lblIcon");

            Assert.True(pic.Visible, "安装图标没有显示出来");
            Assert.NotNull(pic.Image);
            Assert.False(placeholder.Visible, "占位方块应该被隐藏");
        }

        [Fact]
        public void 卸载模式的图标应与安装不同()
        {
            var view = new WizardHeaderView();

            view.ApplyAccent(Danger);
            view.ApplyGlyph(false);
            var pic = Field<System.Windows.Forms.PictureBox>(view, "picIcon");
            var installBytes = ToBytes(pic.Image);

            view.ApplyGlyph(true);
            var uninstallBytes = ToBytes(pic.Image);

            Assert.NotNull(installBytes);
            Assert.NotNull(uninstallBytes);
            Assert.NotEmpty(installBytes);
            Assert.False(installBytes.SequenceEqual(uninstallBytes), "卸载图标和安装图标画出来一样");
        }

        private static byte[] ToBytes(System.Drawing.Image image)
        {
            if (image == null)
            {
                return null;
            }

            using (var ms = new System.IO.MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }

        [Fact]
        public void 图标不应再垫白底方块()
        {
            var view = new WizardHeaderView();
            view.ApplyAccent(Danger);
            view.ApplyGlyph(true);

            var block = Field<AntdUI.Panel>(view, "iconPanel");
            var pic = Field<System.Windows.Forms.PictureBox>(view, "picIcon");

            Assert.False(block.Visible, "图标下面不该再有白底方块");
            Assert.True(pic.Visible);
            Assert.NotNull(pic.Image);
        }    }
}
