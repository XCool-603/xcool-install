using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Resources;
using System.Text;
using Installer.Abstractions.Model;
using Installer.Core.Packaging;
using Installer.Core.Platform;
using Xunit;

namespace Installer.Core.Tests
{
    /// <summary>v1 旧包读取与迁移。</summary>
    public class LegacyV1Tests : IDisposable
    {
        private readonly string _root;

        public LegacyV1Tests()
        {
            _root = Path.Combine(Path.GetTempPath(), "legacy-tests-" + Guid.NewGuid().ToString("N"));
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

        /// <summary>按 v1 的写法造一个 install.resources。</summary>
        private string WriteV1(string productName = "古海幻行", string version = "6.0.0.0",
                               byte[] payload = null, string regditFiles = null)
        {
            var path = Path.Combine(_root, "install.resources");

            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            using (var w = new ResourceWriter(fs))
            {
                w.AddResource("InstallProductName", productName);
                w.AddResource("InstallCompanyName", "chuangqi");
                w.AddResource("InstallProductExeName", productName + ".exe");
                w.AddResource("InstallProductVersion", version);
                w.AddResource("InstallCompanyUrl", "https://example.com");
                w.AddResource("InstallSetupPath", @"C:\制作机\输出");
                w.AddResource("InstallationPath", @"%LOCALAPPDATA%\Programs\X");
                w.AddResource("InstallationExePath", @"D:\制作机\源目录");
                w.AddResource("CkDesktopShortcuts", true);
                w.AddResource("CkQuickLaunchBar", true);
                w.AddResource("CkStartUp", false);
                w.AddResource("CkStartClient", false);
                w.AddResource("BackColor", Color.FromArgb(255, 6, 157, 231));
                w.AddResource("BackImage", new byte[0]);      // v1 模板里就是这个类型
                w.AddResource("Logo", new byte[0]);
                w.AddResource("InstallFilesCount", 3);
                w.AddResource("LicenseAgreement", "许可协议正文");
                w.AddResource("RegditFiles", regditFiles ?? string.Empty);
                w.AddResource("bin", payload ?? MakeZip());
            }

            return path;
        }

        private static byte[] MakeZip()
        {
            using (var ms = new MemoryStream())
            {
                using (var zip = new System.IO.Compression.ZipArchive(ms,
                    System.IO.Compression.ZipArchiveMode.Create, true))
                {
                    var e = zip.CreateEntry("bin/App.exe");
                    using (var s = e.Open())
                    {
                        var b = Encoding.UTF8.GetBytes("fake exe");
                        s.Write(b, 0, b.Length);
                    }

                    zip.CreateEntry("readme.txt");
                }

                return ms.ToArray();
            }
        }

        [Fact]
        public void 目录里有_install_resources_就应被识别为_v1()
        {
            WriteV1();
            Assert.True(LegacyV1Reader.LooksLikeV1(_root));
            Assert.False(LegacyV1Reader.LooksLikeV1(Path.Combine(_root, "nope")));
        }

        [Fact]
        public void 应读出全部元数据()
        {
            var pkg = LegacyV1Reader.Read(WriteV1());

            Assert.Equal("古海幻行", pkg.ProductName);
            Assert.Equal("chuangqi", pkg.CompanyName);
            Assert.Equal("6.0.0.0", pkg.ProductVersion);
            Assert.Equal("古海幻行.exe", pkg.ProductExeName);
            Assert.Equal("https://example.com", pkg.CompanyUrl);
            Assert.Equal("许可协议正文", pkg.LicenseAgreement);
            Assert.NotNull(pkg.PayloadZip);
            Assert.True(pkg.PayloadZip.Length > 0);
        }

        [Fact]
        public void 应能列出负载里的条目()
        {
            var pkg = LegacyV1Reader.Read(WriteV1());
            var entries = LegacyV1Reader.ListPayloadEntries(pkg);

            Assert.Contains("bin/App.exe", entries);
            Assert.Contains("readme.txt", entries);
        }

        [Fact]
        public void 空负载应给出警告而不是静默通过()
        {
            // v1 模板里 bin 就是 byte[0]
            var pkg = LegacyV1Reader.Read(WriteV1(payload: new byte[0]));

            Assert.Null(pkg.PayloadZip);
            Assert.NotEmpty(pkg.Warnings);
        }

        [Fact]
        public void RegditFiles_应被拆成分号列表()
        {
            var pkg = LegacyV1Reader.Read(WriteV1(regditFiles: "a.dll;b.ocx;"));

            Assert.Equal(2, pkg.RegditFiles.Count);
            Assert.Contains("a.dll", pkg.RegditFiles);
            Assert.Contains("b.ocx", pkg.RegditFiles);
        }

        [Fact]
        public void 迁移成新工程应保留产品信息()
        {
            var pkg = LegacyV1Reader.Read(WriteV1());
            var project = LegacyV1Reader.ToProject(pkg);

            Assert.Equal("古海幻行", project.Manifest.Product.Name["zh-Hans"]);
            Assert.Equal("6.0.0.0", project.Manifest.Product.Version);
            Assert.Equal("chuangqi", project.Manifest.Product.Publisher["zh-Hans"]);
            Assert.Equal(@"bin\古海幻行.exe", project.Manifest.EntryPoint);
            Assert.Equal("许可协议正文", project.Manifest.License.Text["zh-Hans"]);
            Assert.True(project.Manifest.License.Required);
            Assert.Single(project.Manifest.Shortcuts);
        }

        [Fact]
        public void 迁移应生成全新的_productCode_并说明这一点()
        {
            var pkg = LegacyV1Reader.Read(WriteV1());
            var a = LegacyV1Reader.ToProject(pkg);
            var b = LegacyV1Reader.ToProject(pkg);

            // v1 里没有 productCode 概念，所以必须新生成；两次生成不应相同
            Assert.NotEqual(a.Manifest.ProductCode, b.Manifest.ProductCode);
            Assert.StartsWith("{", a.Manifest.ProductCode);
        }

        [Fact]
        public void 迁移应把_v1_的注册文件转成_COM_条目()
        {
            var pkg = LegacyV1Reader.Read(WriteV1(regditFiles: "x.dll;y.ocx;"));
            var project = LegacyV1Reader.ToProject(pkg);

            Assert.Equal(2, project.Manifest.Com.Count);
            Assert.Equal(@"bin\x.dll", project.Manifest.Com[0].Path);
            // v1 只会 regsvr32，所以迁移后也用 regsvr32（而不是那个尚未实现的 registrationFree）
            Assert.Equal(ComRegistrationModes.RegSvr32, project.Manifest.Com[0].Mode);
        }

        [Fact]
        public void 迁移不应把制作机路径带进新工程()
        {
            var pkg = LegacyV1Reader.Read(WriteV1());
            var project = LegacyV1Reader.ToProject(pkg);
            var json = ProjectSerializer.Serialize(project);

            // v1 的 InstallSetupPath / InstallationExePath 是制作机路径，新格式里不该有它们
            Assert.DoesNotContain(@"C:\制作机\输出", json);
            Assert.DoesNotContain(@"D:\制作机\源目录", json);
        }

        [Fact]
        public void 迁移出的工程应能被新格式读回()
        {
            var pkg = LegacyV1Reader.Read(WriteV1());
            var project = LegacyV1Reader.ToProject(pkg);

            var path = Path.Combine(_root, "migrated.wmpkg.json");
            ProjectSerializer.Save(path, project);

            var back = ProjectSerializer.Load(path);
            Assert.Equal("古海幻行", back.Manifest.Product.Name["zh-Hans"]);
            Assert.NotNull(back.Manifest);
        }

        [Fact]
        public void 文件不存在应抛异常()
        {
            Assert.Throws<FileNotFoundException>(() =>
                LegacyV1Reader.Read(Path.Combine(_root, "nope", "install.resources")));
        }
    }

    /// <summary>COM 注册方式的派发。</summary>
    public class ComRegistrarTests
    {
        private static string WriteDummyDll(string dir, string name = "x.dll")
        {
            Directory.CreateDirectory(dir);
            var path = Path.Combine(dir, name);
            File.WriteAllBytes(path, new byte[] { 0x4D, 0x5A, 0, 0 });
            return path;
        }

        [Fact]
        public void 文件不存在应返回失败而不是抛异常()
        {
            var r = ComRegistrar.Apply(Path.Combine(Path.GetTempPath(), "definitely-missing.dll"),
                ComRegistrationModes.RegSvr32, true);

            Assert.False(r.Success);
            Assert.Contains("不存在", r.Message);
        }

        [Fact]
        public void 免注册_COM_应明确报未实现而不是假装成功()
        {
            var dir = Path.Combine(Path.GetTempPath(), "com-tests-" + Guid.NewGuid().ToString("N"));
            try
            {
                var dll = WriteDummyDll(dir);

                var r = ComRegistrar.Apply(dll, ComRegistrationModes.RegistrationFree, true);

                Assert.False(r.Success);
                Assert.Contains("尚未实现", r.Message);
                Assert.Contains(ComRegistrationModes.RegSvr32, r.Message);   // 告诉用户该改成什么
            }
            finally
            {
                try
                {
                    Directory.Delete(dir, true);
                }
                catch (IOException)
                {
                }
            }
        }

        [Fact]
        public void 假_DLL_用_regsvr32_注册应失败且带退出码含义()
        {
            var dir = Path.Combine(Path.GetTempPath(), "com-tests-" + Guid.NewGuid().ToString("N"));
            try
            {
                var dll = WriteDummyDll(dir);

                var r = ComRegistrar.Apply(dll, ComRegistrationModes.RegSvr32, true);

                // 4 字节的假 DLL 不可能注册成功；关键是**必须报失败**而不是像现状那样吞掉后打印"成功"
                Assert.False(r.Success);
                Assert.Contains("regsvr32", r.Message);
            }
            finally
            {
                try
                {
                    Directory.Delete(dir, true);
                }
                catch (IOException)
                {
                }
            }
        }

        [Theory]
        [InlineData(0, "成功")]
        [InlineData(3, "DllRegisterServer")]
        [InlineData(4, "LoadLibrary")]
        [InlineData(5, "DllInstall")]
        public void regsvr32_退出码应有可读解释(int code, string fragment)
        {
            Assert.Contains(fragment, ComRegistrar.DescribeRegSvr32Code(code));
        }

        [Fact]
        public void 相对路径应被解析到安装目录内()
        {
            var dir = Path.Combine(Path.GetTempPath(), "com-resolve-" + Guid.NewGuid().ToString("N"));
            try
            {
                Directory.CreateDirectory(dir);
                var resolved = ComRegistrar.Resolve(dir, @"bin\x.dll");

                Assert.StartsWith(dir, resolved, StringComparison.OrdinalIgnoreCase);
                Assert.EndsWith("x.dll", resolved);
            }
            finally
            {
                try
                {
                    Directory.Delete(dir, true);
                }
                catch (IOException)
                {
                }
            }
        }

        [Fact]
        public void 逃逸路径应被拒绝()
        {
            var dir = Path.Combine(Path.GetTempPath(), "com-resolve-" + Guid.NewGuid().ToString("N"));
            try
            {
                Directory.CreateDirectory(dir);
                Assert.ThrowsAny<Exception>(() => ComRegistrar.Resolve(dir, @"..\..\evil.dll"));
            }
            finally
            {
                try
                {
                    Directory.Delete(dir, true);
                }
                catch (IOException)
                {
                }
            }
        }

        [Fact]
        public void 清单里的_COM_应被解析成绝对路径()
        {
            var dir = Path.Combine(Path.GetTempPath(), "com-all-" + Guid.NewGuid().ToString("N"));
            try
            {
                Directory.CreateDirectory(dir);

                var m = TestManifest.Create();
                m.Com = new List<ComSpec>
                {
                    new ComSpec { Path = @"bin\a.dll" },
                    new ComSpec { Path = @"bin\b.ocx", Mode = ComRegistrationModes.Direct },
                };

                var all = ComRegistrar.ResolveAll(dir, m);

                Assert.Equal(2, all.Count);
                Assert.StartsWith(dir, all[0].Path, StringComparison.OrdinalIgnoreCase);
                Assert.Equal(ComRegistrationModes.RegSvr32, all[0].Mode);   // 默认值
                Assert.Equal(ComRegistrationModes.Direct, all[1].Mode);
            }
            finally
            {
                try
                {
                    Directory.Delete(dir, true);
                }
                catch (IOException)
                {
                }
            }
        }
    }
}
