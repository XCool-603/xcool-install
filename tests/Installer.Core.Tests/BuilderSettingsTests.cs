using System;
using System.IO;
using Installer.Builder.Models;
using Xunit;

namespace Installer.Core.Tests
{
    /// <summary>
    /// 制作端要**记住上次选择的生成目录**，默认桌面。
    ///
    /// 用户反馈：选了生成目录，重启后又变回桌面 —— 因为以前这个选择只存在
    /// 当前工程里，而且一改产品名/版本就被"自动推导"覆盖掉了。
    /// </summary>
    public class BuilderSettingsTests : IDisposable
    {
        private readonly string _root;

        public BuilderSettingsTests()
        {
            _root = Path.Combine(Path.GetTempPath(), "bsettings-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_root);
        }

        public void Dispose()
        {
            try
            {
                Directory.Delete(_root, true);
            }
            catch (IOException)
            {
            }
        }

        [Fact]
        public void 记住的是目录而不是文件名()
        {
            var settings = new BuilderSettings();
            var file = Path.Combine(_root, "我的安装包_1.0.exe");

            settings.RememberOutputDir(file);

            Assert.Equal(_root, settings.OutputDir);
        }

        [Fact]
        public void 目录不存在时不记()
        {
            var settings = new BuilderSettings();
            settings.RememberOutputDir(Path.Combine(_root, "nope", "x.exe"));

            Assert.Null(settings.OutputDir);
        }

        [Fact]
        public void 选过的目录要优先于源目录同级()
        {
            var source = Path.Combine(_root, "src");
            Directory.CreateDirectory(source);

            var preferred = Path.Combine(_root, "dist");
            Directory.CreateDirectory(preferred);

            var path = BuilderDocument.SuggestOutputPath("我的产品", "1.0.0", source, preferred);

            Assert.Equal(preferred, Path.GetDirectoryName(path));
            Assert.Equal("我的产品_1.0.0.exe", Path.GetFileName(path));
        }

        [Fact]
        public void 没选过目录时退回源目录同级()
        {
            var source = Path.Combine(_root, "src");
            Directory.CreateDirectory(source);

            var path = BuilderDocument.SuggestOutputPath("我的产品", "1.0.0", source, null);

            Assert.Equal(_root, Path.GetDirectoryName(path));
        }

        [Fact]
        public void 连源目录都没有时默认桌面()
        {
            var path = BuilderDocument.SuggestOutputPath("我的产品", "1.0.0", null, null);

            Assert.Equal(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                Path.GetDirectoryName(path));
        }

        [Fact]
        public void 设置文件放在_appdata_下的固定位置()
        {
            var path = BuilderSettings.GetPath();

            Assert.Contains("安装包制作助手", path);
            Assert.EndsWith("settings.json", path);
        }
    }
}
