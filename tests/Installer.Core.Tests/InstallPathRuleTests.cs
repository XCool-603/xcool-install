using System;
using Installer.Abstractions.Model;
using Xunit;

namespace Installer.Core.Tests
{
    /// <summary>
    /// 安装目录的层级规则：<c>{根}\{厂商}\{产品}</c>，没填厂商就不建厂商那一层。
    ///
    /// 为什么值得单独钉一遍：这条规则决定了"程序装到哪、卸载能删什么"。
    /// 拼错一层不会报错，只会把几百个文件平铺进 <c>D:\Program Files (x86)</c>，
    /// 跟别人的软件混在一起 —— 而且这种情况下卸载**不敢**整目录删，
    /// 于是又变成"卸载成功但目录还在"。
    /// </summary>
    public class InstallPathRuleTests
    {
        [Fact]
        public void 默认目录应带厂商层级()
        {
            var dir = InstallPaths.Suggest("空中小火车", "唯非工作室", false);

            Assert.Equal(@"%LOCALAPPDATA%\Programs\唯非工作室\空中小火车", dir);
        }

        [Fact]
        public void 没填厂商时不应凭空多一层()
        {
            Assert.Equal(@"%LOCALAPPDATA%\Programs\空中小火车",
                InstallPaths.Suggest("空中小火车", null, false));

            Assert.Equal(@"%LOCALAPPDATA%\Programs\空中小火车",
                InstallPaths.Suggest("空中小火车", "   ", false));
        }

        [Fact]
        public void 所有用户应落到_Program_Files()
        {
            Assert.Equal(@"%ProgramFiles%\唯非工作室\空中小火车",
                InstallPaths.Suggest("空中小火车", "唯非工作室", true));
        }

        [Fact]
        public void 给根目录应拼出厂商与产品两层()
        {
            Assert.Equal(@"D:\Program Files (x86)\唯非工作室\空中小火车",
                InstallPaths.Compose(@"D:\Program Files (x86)", "唯非工作室", "空中小火车"));
        }

        [Fact]
        public void 根目录末尾带斜杠也不应拼出双斜杠()
        {
            Assert.Equal(@"D:\Apps\唯非工作室\空中小火车",
                InstallPaths.Compose(@"D:\Apps\", "唯非工作室", "空中小火车"));
        }

        [Fact]
        public void 重复拼接不应套娃()
        {
            var once = InstallPaths.Compose(@"D:\Apps", "厂商", "产品");
            var twice = InstallPaths.Compose(once, "厂商", "产品");

            Assert.Equal(once, twice);
            Assert.Equal(@"D:\Apps\厂商\产品", once);
        }

        [Fact]
        public void 已经是完整目录时不应再加一层产品名()
        {
            Assert.Equal(@"C:\MyApp", InstallPaths.Compose(@"C:\MyApp", null, "MyApp"));
        }

        [Fact]
        public void 取根目录应剥掉厂商与产品两层()
        {
            Assert.Equal(@"%LOCALAPPDATA%\Programs",
                InstallPaths.SplitBase(@"%LOCALAPPDATA%\Programs\唯非工作室\空中小火车", "唯非工作室", "空中小火车"));

            Assert.Equal(@"D:\Apps",
                InstallPaths.SplitBase(@"D:\Apps\空中小火车", null, "空中小火车"));
        }

        [Fact]
        public void 取根目录时遇到不相干的路径应原样返回()
        {
            Assert.Equal(@"D:\Tools\别的东西",
                InstallPaths.SplitBase(@"D:\Tools\别的东西", "厂商", "产品"));
        }

        [Fact]
        public void 产品目录与共享根目录应能区分开()
        {
            // 共享容器：里面的东西不全是我们的，不能整目录删
            Assert.True(InstallPaths.IsSharedRoot(@"D:\Program Files (x86)"));
            Assert.True(InstallPaths.IsSharedRoot(@"D:\"));
            Assert.True(InstallPaths.IsSharedRoot(@"C:\Program Files"));
            Assert.True(InstallPaths.IsSharedRoot(@"C:\Users\Administrator\AppData\Local\Programs"));
            Assert.True(InstallPaths.IsSharedRoot(@"D:\Apps"));

            // 产品目录：我们独占
            Assert.False(InstallPaths.IsSharedRoot(@"D:\Program Files (x86)\空中小火车"));
            Assert.False(InstallPaths.IsSharedRoot(@"D:\Program Files (x86)\唯非工作室\空中小火车"));
        }

        [Fact]
        public void 目录名里的非法字符应被换掉()
        {
            var dir = InstallPaths.Compose(@"D:\Apps", "厂商", "A/B:C*D");

            Assert.DoesNotContain("/", dir.Substring(@"D:\Apps\厂商\".Length));
            Assert.StartsWith(@"D:\Apps\厂商\", dir);
        }

        [Fact]
        public void 产品名为空时应回落到占位名()
        {
            Assert.Equal(InstallPaths.PlaceholderProductName, InstallPaths.Sanitize(null));
        }
    }
}
