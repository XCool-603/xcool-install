using System;
using System.IO;
using Installer.Core.Packaging;
using Xunit;

namespace Installer.Core.Tests
{
    /// <summary>
    /// 安装包要"以管理员身份运行"（清单 <c>requireAdministrator</c>）。
    ///
    /// 用户要求：做出来的安装包双击就要弹 UAC，而不是先跑起来再自己 runas 重启。
    /// 做法是打包时替换产物 exe 的 RT_MANIFEST 资源，这里钉住这个替换真的生效。
    /// </summary>
    public class ManifestPatcherTests : IDisposable
    {
        private readonly string _dir;

        public ManifestPatcherTests()
        {
            _dir = Path.Combine(Path.GetTempPath(), "manifest-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_dir);
        }

        public void Dispose()
        {
            try
            {
                Directory.Delete(_dir, true);
            }
            catch (IOException)
            {
            }
        }

        /// <summary>拿一个真实的 exe 做样本（notepad 自带 asInvoker 清单）。</summary>
        private static string SampleExe()
        {
            var path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.System), "notepad.exe");

            return File.Exists(path) ? path : null;
        }

        [Fact]
        public void 替换清单后应能读出_requireAdministrator()
        {
            var source = SampleExe();
            if (source == null)
            {
                return;   // 极简环境里没有 notepad，跳过
            }

            var copy = Path.Combine(_dir, "sample.exe");
            File.Copy(source, copy, true);

            Assert.True(ManifestPatcher.ApplyRequireAdministrator(copy));

            // 直接查文件字节：清单是作为 RT_MANIFEST 资源原样写进去的
            var text = System.Text.Encoding.UTF8.GetString(File.ReadAllBytes(copy));
            Assert.Contains("requireAdministrator", text);
            Assert.DoesNotContain("asInvoker", text);
        }

        [Fact]
        public void 默认清单内容本身就是_requireAdministrator()
        {
            var text = ManifestPatcher.RequireAdministratorManifest;

            Assert.Contains("level=\"requireAdministrator\"", text);
            Assert.Contains("requestedExecutionLevel", text);
            Assert.DoesNotContain("asInvoker", text);
        }

        [Fact]
        public void 目标文件不存在时应抛异常而不是静默成功()
        {
            Assert.ThrowsAny<Exception>(() =>
                ManifestPatcher.ApplyRequireAdministrator(Path.Combine(_dir, "no-such-file.exe")));
        }
    }
}
