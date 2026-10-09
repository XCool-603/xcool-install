using System;
using System.Collections.Generic;
using System.IO;
using Installer.Abstractions.Model;
using Installer.Abstractions.Packaging;
using Installer.Core.Packaging;
using Xunit;

namespace Installer.Core.Tests
{
    /// <summary>清单的序列化契约。</summary>
    public class ManifestTests
    {
        private static InstallerManifest BuildSample()
        {
            return new InstallerManifest
            {
                SchemaVersion = 2,
                ProductCode = "{8F3A1C2E-5B4D-4E7A-9C1F-2D3E4F5A6B7C}",
                DefaultCulture = "zh-Hans",
                Cultures = new List<string> { "zh-Hans", "en" },
                Product = new ProductInfo
                {
                    Name = new LocalizedText { { "zh-Hans", "汽车小镇" }, { "en", "Auto Town" } },
                    Version = "2.4.1",
                    Publisher = new LocalizedText { { "zh-Hans", "唯非工作室" } },
                    Url = "https://example.com",
                },
                Scope = InstallScopes.PerUser,
                DefaultInstallDir = @"%LOCALAPPDATA%\Programs\AutoTown",
                EntryPoint = @"bin\AutoTown.exe",
                Autostart = false,
                RunAfterInstall = true,
                Shortcuts = new List<ShortcutSpec>
                {
                    new ShortcutSpec
                    {
                        Location = "Desktop",
                        Name = new LocalizedText { { "zh-Hans", "汽车小镇" }, { "en", "Auto Town" } },
                    },
                },
                Com = new List<ComSpec>
                {
                    new ComSpec { Path = @"bin\x.ocx", Mode = ComRegistrationModes.RegistrationFree },
                },
                Files = new List<FileEntry>
                {
                    new FileEntry { Path = @"bin\AutoTown.exe", Size = 1234, Sha256 = new string('a', 64) },
                    new FileEntry { Path = @"bin\中文模块.dll", Size = 56, Sha256 = new string('b', 64) },
                },
                Directories = new List<string> { @"data\空目录" },
                Strings = new Dictionary<string, LocalizedText>(StringComparer.Ordinal)
                {
                    { "Button.Install", new LocalizedText { { "zh-Hans", "立即安装" }, { "en", "Install" } } },
                },
                Build = new BuildInfo
                {
                    BuiltAtUtc = "2026-10-08T12:00:00Z",
                    PackerVersion = "2.0.0",
                    EntryCount = 2,
                    PayloadBytes = 4096,
                },
            };
        }

        [Fact]
        public void 序列化往返应保持全部字段()
        {
            var original = BuildSample();
            var bytes = ManifestSerializer.SerializeToUtf8Bytes(original);
            var back = ManifestSerializer.Deserialize(bytes);

            Assert.Equal(original.SchemaVersion, back.SchemaVersion);
            Assert.Equal(original.ProductCode, back.ProductCode);
            Assert.Equal(original.Scope, back.Scope);
            Assert.Equal("汽车小镇", back.Product.Name["zh-Hans"]);
            Assert.Equal("Auto Town", back.Product.Name["en"]);
            Assert.Equal("2.4.1", back.Product.Version);
            Assert.Equal(@"bin\AutoTown.exe", back.EntryPoint);
            Assert.Equal(2, back.Files.Count);
            Assert.Equal(@"bin\中文模块.dll", back.Files[1].Path);
            Assert.Single(back.Directories);
            Assert.Equal("Install", back.Strings["Button.Install"]["en"]);
            Assert.Equal(ComRegistrationModes.RegistrationFree, back.Com[0].Mode);
            Assert.Equal("2026-10-08T12:00:00Z", back.Build.BuiltAtUtc);
            Assert.True(back.RunAfterInstall);
        }

        [Fact]
        public void 序列化应是可读的缩进_JSON_且中文不转义()
        {
            var json = ManifestSerializer.Serialize(BuildSample());

            Assert.Contains("\n", json);              // 缩进 → 可 diff
            Assert.Contains("汽车小镇", json);         // 中文原样，不写成 \uXXXX
            Assert.Contains("\"zh-Hans\"", json);     // 字典键不被改大小写
            Assert.Contains("\"schemaVersion\"", json);
        }

        [Fact]
        public void 缺少_files_的工程文件应能被宽松解析()
        {
            // 工程文件（.wmpkg.json）本来就没有 files，由打包器填充
            var json = @"{
              ""schemaVersion"": 2,
              ""productCode"": ""{11111111-2222-3333-4444-555555555555}"",
              ""product"": { ""name"": { ""zh-Hans"": ""x"" }, ""version"": ""1.0"" },
              ""scope"": ""perUser""
            }";

            var m = ManifestSerializer.Deserialize(json);
            Assert.NotNull(m);
            Assert.Empty(m.Files);
        }

        [Fact]
        public void 严格校验应拒绝没有_files_的清单()
        {
            var m = BuildSample();
            m.Files.Clear();
            Assert.Throws<InvalidDataException>(() => ManifestSerializer.Validate(m, true));
        }

        [Theory]
        [InlineData(null, "空")]
        [InlineData("", "空")]
        [InlineData("not json", "JSON")]
        public void 非法输入应抛异常(string json, string reasonFragment)
        {
            var ex = Assert.ThrowsAny<Exception>(() => ManifestSerializer.Deserialize(json));
            Assert.Contains(reasonFragment, ex.Message);
        }

        [Fact]
        public void 未知_scope_应被拒绝()
        {
            var m = BuildSample();
            m.Scope = "whatever";
            Assert.Throws<InvalidDataException>(() => ManifestSerializer.Validate(m, true));
        }

        [Fact]
        public void 过高的_schemaVersion_应被拒绝()
        {
            var m = BuildSample();
            m.SchemaVersion = InstallerManifestSchema.Current + 1;
            Assert.Throws<NotSupportedException>(() => ManifestSerializer.Validate(m, true));
        }

        [Fact]
        public void 缺少_productCode_应被拒绝()
        {
            var m = BuildSample();
            m.ProductCode = "";
            Assert.Throws<InvalidDataException>(() => ManifestSerializer.Validate(m, true));
        }

        [Fact]
        public void 缺少_product_name_应被拒绝()
        {
            var m = BuildSample();
            m.Product.Name.Clear();
            Assert.Throws<InvalidDataException>(() => ManifestSerializer.Validate(m, true));
        }
    }

    /// <summary>容器常量的一致性。</summary>
    public class ContainerSpecTests
    {
        [Fact]
        public void footer_各字段不得越界()
        {
            var fields = new[]
            {
                ContainerFormatSpec.OffsetMagic,
                ContainerFormatSpec.OffsetFormatVersion,
                ContainerFormatSpec.OffsetFlags,
                ContainerFormatSpec.OffsetPayloadOffset,
                ContainerFormatSpec.OffsetPayloadLength,
                ContainerFormatSpec.OffsetManifestOffset,
                ContainerFormatSpec.OffsetManifestLength,
                ContainerFormatSpec.OffsetPayloadSha256,
                ContainerFormatSpec.OffsetStubSha256,
                ContainerFormatSpec.OffsetFooterCrc32,
            };

            foreach (var f in fields)
            {
                Assert.InRange(f, 0, ContainerFormatSpec.FooterSize - 1);
            }

            Assert.Equal(8, ContainerFormatSpec.Magic.Length);
            Assert.Equal(128, ContainerFormatSpec.FooterSize);
            Assert.Equal(112, ContainerFormatSpec.CrcCoveredLength);
            Assert.True(ContainerFormatSpec.OffsetFooterCrc32 + 4 <= ContainerFormatSpec.FooterSize);
            Assert.True(ContainerFormatSpec.OffsetStubSha256 + 32 <= ContainerFormatSpec.OffsetFooterCrc32);
        }
    }
}
