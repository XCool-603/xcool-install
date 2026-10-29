using System;
using System.IO;
using Installer.Abstractions.Model;
using Installer.Runtime;
using Xunit;

namespace Installer.Core.Tests
{
    /// <summary>
    /// 安装根盘符不在这台机器上时的回退。
    ///
    /// 用户实测反馈：制作机上把安装根目录填成了 <c>D:\Program Files</c>，
    /// 装到一台没有 D 盘的电脑上就装不进去 —— 期望是"落到 C 盘的 Program Files (x86)"。
    /// </summary>
    public class MissingDriveTests : IDisposable
    {
        private readonly string _root;

        public MissingDriveTests()
        {
            _root = Path.Combine(Path.GetTempPath(), "nodisk-" + Guid.NewGuid().ToString("N"));
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

        /// <summary>找一个这台机器上确实不存在的盘符（测试不能依赖"恰好没有 D 盘"）。</summary>
        private static string MissingDrivePath()
        {
            foreach (var letter in new[] { 'Y', 'X', 'W', 'V', 'U', 'Q' })
            {
                var root = letter + ":\\";
                if (!Directory.Exists(root))
                {
                    return Path.Combine(root, "Program Files", "厂商", "产品");
                }
            }

            return null;
        }

        [Fact]
        public void 安装根盘符不存在时应退回到系统默认根并保留厂商产品两层()
        {
            var missing = MissingDrivePath();
            if (missing == null)
            {
                return;   // 这台机器盘符全占满了，跳过（不静默通过一个假断言）
            }

            var platform = new FakePlatform(_root);
            var manifest = TestManifest.Create(scope: InstallScopes.PerMachine);

            var dir = InstallHost.ApplyMissingRootFallback(missing, manifest, "厂商", "产品", platform);

            // FakeEnvironmentInfo 不展开环境变量，所以这里正好能看出"用的是哪个模板"
            Assert.Equal(InstallPaths.Root(true) + @"\厂商\产品", dir);
        }

        [Fact]
        public void 盘符存在时不应改动安装目录()
        {
            var platform = new FakePlatform(_root);
            var manifest = TestManifest.Create(scope: InstallScopes.PerMachine);
            var dir = Path.Combine(_root, "apps", "厂商", "产品");

            var result = InstallHost.ApplyMissingRootFallback(dir, manifest, "厂商", "产品", platform);

            Assert.Equal(dir, result);
        }

        [Fact]
        public void 仅当前用户安装时应退回到用户目录()
        {
            var missing = MissingDrivePath();
            if (missing == null)
            {
                return;
            }

            var platform = new FakePlatform(_root);
            var manifest = TestManifest.Create(scope: InstallScopes.PerUser);

            var dir = InstallHost.ApplyMissingRootFallback(missing, manifest, null, "产品", platform);

            Assert.Equal(InstallPaths.Root(false) + @"\产品", dir);
        }
    }
}
