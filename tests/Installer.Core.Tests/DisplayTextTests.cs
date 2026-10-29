using System;
using Installer.UI.Forms;
using Xunit;

namespace Installer.Core.Tests
{
    /// <summary>
    /// 界面上的路径压缩。
    ///
    /// 「安装设置」页那条浅蓝提示是**单行**、宽 520px。真实路径常有 80 多个字符，
    /// 一带中文就会换行，第二行正好被条子高度裁掉 —— 用户看到的是
    /// 「将安装到」后面什么都没有（实测反馈过"界面像是坏了"）。
    /// 所以显示时留盘符 + 尾巴、砍中间。
    /// </summary>
    public class DisplayTextTests
    {
        [Fact]
        public void 短路径原样显示()
        {
            Assert.Equal(@"C:\Apps\Demo", WizardForm.ShortenPathForDisplay(@"C:\Apps\Demo"));
        }

        [Fact]
        public void 长路径应保留盘符与尾部()
        {
            var path = @"C:\Users\Administrator\AppData\Local\Temp\installer-e2e\sandbox\Programs\唯非工作室\空中飞翔CSVS";

            var shortPath = WizardForm.ShortenPathForDisplay(path, 52);

            Assert.True(shortPath.Length <= 52, "压缩后仍然太长：" + shortPath.Length);
            Assert.StartsWith(@"C:\…\", shortPath);
            Assert.EndsWith(@"唯非工作室\空中飞翔CSVS", shortPath);
        }

        [Fact]
        public void 压缩结果不应从半个目录名开始()
        {
            var path = @"C:\Users\Administrator\AppData\Local\Temp\abc\Programs\厂商\产品";

            var shortPath = WizardForm.ShortenPathForDisplay(path, 40);

            // "…\" 后面必须是一个完整目录名的开头（首字符不是被切断的单词残留）
            var rest = shortPath.Substring(@"C:\…\".Length);
            Assert.False(rest.StartsWith("m\\") || rest.StartsWith("p\\"), shortPath);
        }

        [Fact]
        public void 空路径不应抛异常()
        {
            Assert.Equal(string.Empty, WizardForm.ShortenPathForDisplay(string.Empty));
            Assert.Null(WizardForm.ShortenPathForDisplay(null));
        }
    }
}
