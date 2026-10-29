using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Installer.UI.Common
{
    /// <summary>
    /// 品牌区里的图标 —— **单色实心图形**，直接画在品牌区上（不再垫白底方块）。
    ///
    /// <list type="bullet">
    /// <item>安装：**包裹/箱子**（安装包的本体），不是"下载箭头"；</item>
    /// <item>卸载：**垃圾桶**。</item>
    /// </list>
    ///
    /// 两个都用同一套画法（实心剪影 + 主色细缝表现结构），风格统一。
    /// </summary>
    public static class HeaderGlyph
    {
        /// <summary>生成图标位图（透明背景）。</summary>
        /// <param name="uninstall">true = 垃圾桶，false = 安装包（箱子）。</param>
        /// <param name="size">边长（像素）。</param>
        /// <param name="color">图形颜色（单色）。</param>
        /// <param name="cutColor">结构缝的颜色 —— 通常给品牌区底色，缝才"看不见"。</param>
        public static Bitmap Create(bool uninstall, int size, Color color, Color cutColor)
        {
            if (size < 8)
            {
                size = 8;
            }

            var bmp = new Bitmap(size, size);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.Clear(Color.Transparent);

                if (uninstall)
                {
                    DrawTrash(g, size, color, cutColor);
                }
                else
                {
                    DrawPackage(g, size, color, cutColor);
                }
            }

            return bmp;
        }

        private static PointF P(float x, float y, float size)
        {
            var k = size / 24f;
            return new PointF(x * k, y * k);
        }

        private static float K(float v, float size)
        {
            return v * size / 24f;
        }

        /// <summary>安装：一个立着的包裹箱（实心六边形 + 三道主色缝）。</summary>
        private static void DrawPackage(Graphics g, float size, Color color, Color cutColor)
        {
            var top = P(12, 2.6f, size);
            var rightTop = P(20.4f, 7.2f, size);
            var rightBottom = P(20.4f, 16.8f, size);
            var bottom = P(12, 21.4f, size);
            var leftBottom = P(3.6f, 16.8f, size);
            var leftTop = P(3.6f, 7.2f, size);

            using (var brush = new SolidBrush(color))
            {
                g.FillPolygon(brush, new[] { top, rightTop, rightBottom, bottom, leftBottom, leftTop });
            }

            // 三道缝：顶面两条棱 + 中间竖棱，用底色画出来就是"箱子"而不是"六边形"
            using (var pen = new Pen(cutColor, Math.Max(1.4f, K(1.3f, size))))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                pen.LineJoin = LineJoin.Round;

                g.DrawLines(pen, new[] { leftTop, P(12, 11.6f, size), rightTop });
                g.DrawLine(pen, P(12, 11.6f, size), P(12, 21.4f, size));
            }
        }

        /// <summary>卸载：一个垃圾桶（实心桶身 + 盖 + 提手 + 两道缝）。</summary>
        private static void DrawTrash(Graphics g, float size, Color color, Color cutColor)
        {
            using (var brush = new SolidBrush(color))
            {
                // 盖
                FillRounded(g, brush, 4.2f, 5.9f, 19.8f, 8.1f, K(0.9f, size), size);

                // 提手
                FillRounded(g, brush, 9.3f, 3.3f, 14.7f, 6.1f, K(0.7f, size), size);

                // 桶身（上宽下窄）
                g.FillPolygon(brush, new[]
                {
                    P(6.6f, 8.9f, size), P(17.4f, 8.9f, size),
                    P(16.2f, 20.6f, size), P(7.8f, 20.6f, size),
                });
            }

            // 两道缝
            using (var pen = new Pen(cutColor, Math.Max(1.4f, K(1.3f, size))))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                g.DrawLine(pen, P(10.3f, 11.4f, size), P(10.6f, 18.2f, size));
                g.DrawLine(pen, P(13.7f, 11.4f, size), P(13.4f, 18.2f, size));
            }
        }

        private static void FillRounded(Graphics g, Brush brush, float x1, float y1, float x2, float y2,
                                        float radius, float size)
        {
            var rect = new RectangleF(P(x1, y1, size).X, P(x1, y1, size).Y,
                K(x2 - x1, size), K(y2 - y1, size));

            using (var path = new GraphicsPath())
            {
                var d = radius * 2;
                path.AddArc(rect.Left, rect.Top, d, d, 180, 90);
                path.AddArc(rect.Right - d, rect.Top, d, d, 270, 90);
                path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
                path.AddArc(rect.Left, rect.Bottom - d, d, d, 90, 90);
                path.CloseFigure();
                g.FillPath(brush, path);
            }
        }
    }
}
