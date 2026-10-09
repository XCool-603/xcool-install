using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Installer.Abstractions.Model;
using Installer.Builder.Models;
using Installer.Builder.Presenters;
using Installer.Core.Packaging;
using Xunit;

namespace Installer.Core.Tests
{
    /// <summary>
    /// 制作端。
    ///
    /// **这个测试类同时钉住了"界面简化"的约定**：
    /// 模板 stub 自动找、输出路径自动推、校验只要求 3 项 —— 谁把它们改回"让用户填"，
    /// 这些测试就会红。
    /// </summary>
    public class BuilderPresenterTests : IDisposable
    {
        private readonly string _root;

        public BuilderPresenterTests()
        {
            _root = Path.Combine(Path.GetTempPath(), "builder-tests-" + Guid.NewGuid().ToString("N"));
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

        /// <summary>造一个够大的假 stub（MZ 头 + 2 KB）。</summary>
        private static byte[] MakeStub()
        {
            var b = new byte[2048];
            b[0] = (byte)'M';
            b[1] = (byte)'Z';
            return b;
        }

        private string NewStub()
        {
            var p = Path.Combine(_root, "stub-" + Guid.NewGuid().ToString("N") + ".exe");
            File.WriteAllBytes(p, MakeStub());
            return p;
        }

        /// <summary>造一个有内容的源目录。</summary>
        private string NewSource(int files = 1)
        {
            var dir = Path.Combine(_root, "src-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            for (var i = 0; i < files; i++)
            {
                File.WriteAllText(Path.Combine(dir, "App" + i + ".exe"), "x");
            }

            return dir;
        }

        private BuilderPresenter NewPresenter(string stub = null)
        {
            return new BuilderPresenter("zh-Hans", stub ?? NewStub());
        }

        // ─────────────────────────────────────────────────────────────
        // 模板 stub：用户不该看见它
        // ─────────────────────────────────────────────────────────────

        [Fact]
        public void 模板_stub_应当自动解析出来_不需要用户指定()
        {
            var p = NewPresenter();

            var stub = p.ResolveStubPath();

            Assert.False(string.IsNullOrWhiteSpace(stub));
            Assert.True(File.Exists(stub));
        }

        [Fact]
        public void 显式指定的_stub_优先于自动查找()
        {
            var mine = NewStub();
            var p = new BuilderPresenter("zh-Hans", mine);

            Assert.Equal(mine, p.ResolveStubPath());
        }

        [Fact]
        public void 找不到_stub_时校验要给出可操作的提示()
        {
            var p = new BuilderPresenter("zh-Hans", Path.Combine(_root, "nonexistent.exe"));
            p.Document.Project.Build.SourceDir = NewSource();
            p.Document.Project.Manifest.Product.Name = new LocalizedText { { "zh-Hans", "X" } };

            var problems = p.Validate();

            Assert.Contains(problems, s => s.Contains("Installer.Runtime.exe"));
            Assert.Contains(problems, s => s.Contains("同一个文件夹"));
        }

        // ─────────────────────────────────────────────────────────────
        // 输出路径：自动推导
        // ─────────────────────────────────────────────────────────────

        [Fact]
        public void 输出路径应当自动推导到源目录的同级()
        {
            var src = NewSource();
            var p = NewPresenter();
            p.Document.Project.Build.SourceDir = src;
            p.Document.Project.Manifest.Product.Name = new LocalizedText { { "zh-Hans", "汽车小镇" } };
            p.Document.Project.Manifest.Product.Version = "2.4.1";

            var output = p.ResolveOutputPath();

            Assert.Equal(Path.GetDirectoryName(src), Path.GetDirectoryName(output));
            Assert.Equal("汽车小镇_2.4.1.exe", Path.GetFileName(output));
        }

        [Fact]
        public void 用户固定了输出路径就用用户的()
        {
            var p = NewPresenter();
            var mine = Path.Combine(_root, "我自己定的.exe");
            p.Document.Project.Build.OutputPath = mine;

            Assert.Equal(mine, p.ResolveOutputPath());
        }

        [Fact]
        public void 产品名里的非法文件名字符要被替换掉()
        {
            var path = BuilderDocument.SuggestOutputPath("a/b:c*d", "1.0", @"C:\x\y");

            Assert.DoesNotContain("/", Path.GetFileName(path));
            Assert.DoesNotContain(":", Path.GetFileName(path));
            Assert.DoesNotContain("*", Path.GetFileName(path));
        }

        // ─────────────────────────────────────────────────────────────
        // 校验：只要求 3 项
        // ─────────────────────────────────────────────────────────────

        [Fact]
        public void 刚打开时只因为没选文件夹而报错()
        {
            var p = NewPresenter();
            p.Document.Project.Manifest.Product.Name = new LocalizedText { { "zh-Hans", "X" } };

            var problems = p.Validate();

            Assert.Single(problems);
            Assert.Contains("还没选", problems[0]);
        }

        [Fact]
        public void 选好文件夹填好名称版本就能通过校验()
        {
            var p = NewPresenter();
            p.Document.Project.Build.SourceDir = NewSource();
            p.Document.Project.Manifest.Product.Name = new LocalizedText { { "zh-Hans", "汽车小镇" } };
            p.Document.Project.Manifest.Product.Version = "1.0.0";

            Assert.Empty(p.Validate());
        }

        [Fact]
        public void 空文件夹要被指出来()
        {
            var p = NewPresenter();
            var empty = Path.Combine(_root, "empty");
            Directory.CreateDirectory(empty);

            p.Document.Project.Build.SourceDir = empty;
            p.Document.Project.Manifest.Product.Name = new LocalizedText { { "zh-Hans", "X" } };

            Assert.Contains(p.Validate(), s => s.Contains("空的"));
        }

        [Fact]
        public void 产品名称为空要被指出来()
        {
            var p = NewPresenter();
            p.Document.Project.Build.SourceDir = NewSource();
            p.Document.Project.Manifest.Product.Name = new LocalizedText { { "zh-Hans", "  " } };

            Assert.Contains(p.Validate(), s => s.Contains("产品名称"));
        }

        [Fact]
        public void 版本号为空要被指出来()
        {
            var p = NewPresenter();
            p.Document.Project.Build.SourceDir = NewSource();
            p.Document.Project.Manifest.Product.Name = new LocalizedText { { "zh-Hans", "X" } };
            p.Document.Project.Manifest.Product.Version = "";

            Assert.Contains(p.Validate(), s => s.Contains("版本号"));
        }

        [Fact]
        public void 校验失败时打包应抛异常而不是静默继续()
        {
            var p = NewPresenter();
            p.Document.Project.Manifest.Product.Name = new LocalizedText { { "zh-Hans", "X" } };

            var ex = Assert.Throws<Installer.Abstractions.Packaging.PackageBuildException>(() => p.Build());
            Assert.Contains("还没选", ex.Message);
        }

        // ─────────────────────────────────────────────────────────────
        // 自动推断
        // ─────────────────────────────────────────────────────────────

        [Fact]
        public void 入口程序为空时应自动猜一个()
        {
            var src = NewSource();
            File.WriteAllText(Path.Combine(src, "readme.txt"), "x");

            var guess = BuilderPresenter.GuessEntryPoint(src);

            Assert.NotNull(guess);
            Assert.EndsWith(".exe", guess);
        }

        [Fact]
        public void 猜入口程序时应优先根目录下的_exe()
        {
            var src = Path.Combine(_root, "guess");
            Directory.CreateDirectory(Path.Combine(src, "bin"));
            File.WriteAllText(Path.Combine(src, "bin", "inner.exe"), "x");
            File.WriteAllText(Path.Combine(src, "Main.exe"), "x");

            Assert.Equal("Main.exe", BuilderPresenter.GuessEntryPoint(src));
        }

        [Fact]
        public void 猜入口程序时不应挑到卸载器()
        {
            var src = Path.Combine(_root, "guess2");
            Directory.CreateDirectory(src);
            File.WriteAllText(Path.Combine(src, "Uninstall.exe"), "x");
            File.WriteAllText(Path.Combine(src, "App.exe"), "x");

            Assert.Equal("App.exe", BuilderPresenter.GuessEntryPoint(src));
        }

        [Fact]
        public void 统计源目录应给出文件数与总大小()
        {
            var src = NewSource(3);

            int count;
            long bytes;
            BuilderPresenter.MeasureSource(src, out count, out bytes);

            Assert.Equal(3, count);
            Assert.Equal(3, bytes);
        }

        [Fact]
        public void 统计不存在的目录应是零而不是抛异常()
        {
            int count;
            long bytes;
            BuilderPresenter.MeasureSource(Path.Combine(_root, "nope"), out count, out bytes);

            Assert.Equal(0, count);
            Assert.Equal(0, bytes);
        }

        // ─────────────────────────────────────────────────────────────
        // 启动程序：主界面上直接选（不用进高级选项）
        // ─────────────────────────────────────────────────────────────

        [Fact]
        public void 应能列出文件夹里所有可执行文件()
        {
            var src = Path.Combine(_root, "exes");
            Directory.CreateDirectory(Path.Combine(src, "bin"));
            File.WriteAllText(Path.Combine(src, "Launcher.exe"), "x");
            File.WriteAllText(Path.Combine(src, "bin", "Main.exe"), "x");
            File.WriteAllText(Path.Combine(src, "readme.txt"), "x");

            var exes = BuilderPresenter.FindExecutables(src);

            Assert.Equal(2, exes.Count);
            Assert.Contains("Launcher.exe", exes);
            Assert.Contains(@"bin\Main.exe", exes);
        }

        [Fact]
        public void 列出可执行文件时根目录的排前面()
        {
            var src = Path.Combine(_root, "exes2");
            Directory.CreateDirectory(Path.Combine(src, "bin"));
            File.WriteAllText(Path.Combine(src, "bin", "A.exe"), "x");
            File.WriteAllText(Path.Combine(src, "Z.exe"), "x");

            var exes = BuilderPresenter.FindExecutables(src);

            Assert.Equal("Z.exe", exes[0]);
        }

        [Fact]
        public void 列出可执行文件时应排除卸载器()
        {
            var src = Path.Combine(_root, "exes3");
            Directory.CreateDirectory(src);
            File.WriteAllText(Path.Combine(src, "Uninstall.exe"), "x");
            File.WriteAllText(Path.Combine(src, "App.exe"), "x");

            var exes = BuilderPresenter.FindExecutables(src);

            Assert.Single(exes);
            Assert.Equal("App.exe", exes[0]);
        }

        [Fact]
        public void 文件夹里没有_exe_时应返回空列表()
        {
            var src = Path.Combine(_root, "noexe");
            Directory.CreateDirectory(src);
            File.WriteAllText(Path.Combine(src, "a.txt"), "x");

            Assert.Empty(BuilderPresenter.FindExecutables(src));
        }

        [Fact]
        public void 用户选的启动程序不应被自动猜测覆盖()
        {
            var p = NewPresenter();
            var src = Path.Combine(_root, "pick");
            Directory.CreateDirectory(src);
            File.WriteAllText(Path.Combine(src, "A.exe"), "x");
            File.WriteAllText(Path.Combine(src, "B.exe"), "x");

            p.Document.Project.Build.SourceDir = src;
            p.Document.Project.Manifest.Product.Name = new LocalizedText { { "zh-Hans", "挑一个" } };
            p.Document.Project.Manifest.Product.Version = "1.0";
            p.Document.Project.Manifest.EntryPoint = "B.exe";

            p.Build();

            Assert.Equal("B.exe", p.Document.Project.Manifest.EntryPoint);
        }

        // ─────────────────────────────────────────────────────────────
        // 中英文只填一个
        // ─────────────────────────────────────────────────────────────

        [Fact]
        public void 只填中文不填英文应能通过校验()
        {
            var p = NewPresenter();
            p.Document.Project.Build.SourceDir = NewSource();
            p.Document.Project.Manifest.Product.Name = new LocalizedText
            {
                { "zh-Hans", "汽车小镇" },
                { "en", "" },
            };
            p.Document.Project.Manifest.Product.Version = "1.0";

            Assert.Empty(p.Validate());
        }

        [Fact]
        public void 只填英文不填中文也应能通过校验()
        {
            var p = NewPresenter();
            p.Document.Project.Build.SourceDir = NewSource();
            p.Document.Project.Manifest.Product.Name = new LocalizedText
            {
                { "zh-Hans", "" },
                { "en", "Auto Town" },
            };
            p.Document.Project.Manifest.Product.Version = "1.0";

            Assert.Empty(p.Validate());
        }

        [Fact]
        public void 两个都不填才报错()
        {
            var p = NewPresenter();
            p.Document.Project.Build.SourceDir = NewSource();
            p.Document.Project.Manifest.Product.Name = new LocalizedText
            {
                { "zh-Hans", "  " },
                { "en", "" },
            };

            var problems = p.Validate();

            Assert.Contains(problems, s => s.Contains("产品名称") && s.Contains("至少填一个"));
        }

        [Fact]
        public void 取产品名时应跳过空的那个()
        {
            var p = NewPresenter();
            var m = p.Document.Project.Manifest;
            m.DefaultCulture = "zh-Hans";
            m.Product.Name = new LocalizedText
            {
                { "zh-Hans", "汽车小镇" },
                { "en", "" },
            };

            Assert.Equal("汽车小镇", p.PickProductName(m));
        }

        [Fact]
        public void 输出文件名不应取到空的产品名()
        {
            var src = NewSource();
            var p = NewPresenter();
            p.Document.Project.Build.SourceDir = src;
            p.Document.Project.Manifest.DefaultCulture = "zh-Hans";
            p.Document.Project.Manifest.Product.Name = new LocalizedText
            {
                { "zh-Hans", "汽车小镇" },
                { "en", "" },
            };
            p.Document.Project.Manifest.Product.Version = "3.0";

            Assert.Equal("汽车小镇_3.0.exe", Path.GetFileName(p.ResolveOutputPath()));
        }

        [Fact]
        public void 打包时应把缺的语言补上()
        {
            var p = NewPresenter();
            p.Document.Project.Build.SourceDir = NewSource();
            p.Document.Project.Manifest.Product.Name = new LocalizedText
            {
                { "zh-Hans", "汽车小镇" },
                { "en", "" },
            };
            p.Document.Project.Manifest.Product.Version = "1.0";
            p.Document.Project.Manifest.EntryPoint = "App0.exe";

            p.Build();

            // 英文系统上跑向导不能显示空产品名
            Assert.Equal("汽车小镇", p.Document.Project.Manifest.Product.Name["en"]);
        }

        [Fact]
        public void 补语言时不应覆盖用户填好的那个()
        {
            var p = NewPresenter();
            var m = p.Document.Project.Manifest;
            m.Product.Name = new LocalizedText
            {
                { "zh-Hans", "汽车小镇" },
                { "en", "Auto Town" },
            };

            BuilderPresenter.FillMissingLanguages(m);

            Assert.Equal("汽车小镇", m.Product.Name["zh-Hans"]);
            Assert.Equal("Auto Town", m.Product.Name["en"]);
        }

        // ─────────────────────────────────────────────────────────────
        // 工程文件
        // ─────────────────────────────────────────────────────────────

        [Fact]
        public void 裸清单形状的工程文件应能被识别()
        {
            var json = @"{
              ""schemaVersion"": 2,
              ""productCode"": ""{11111111-2222-3333-4444-555555555555}"",
              ""product"": { ""name"": { ""zh-Hans"": ""裸清单"" }, ""version"": ""1.0"" },
              ""scope"": ""perUser""
            }";

            var project = ProjectSerializer.Deserialize(json);

            Assert.Equal("裸清单", project.Manifest.Product.Name["zh-Hans"]);
            Assert.NotNull(project.Build);
            Assert.Null(project.Build.SourceDir);
        }

        [Fact]
        public void 完整工程形状应能读出_build_段()
        {
            var json = @"{
              ""manifest"": {
                ""schemaVersion"": 2,
                ""productCode"": ""{11111111-2222-3333-4444-555555555555}"",
                ""product"": { ""name"": { ""zh-Hans"": ""完整工程"" }, ""version"": ""2.0"" },
                ""scope"": ""perMachine""
              },
              ""build"": {
                ""sourceDir"": ""C:\\src"",
                ""stubPath"": ""C:\\stub.exe"",
                ""outputPath"": ""C:\\out\\Setup.exe"",
                ""timestamp"": ""2026-10-08T12:00:00Z"",
                ""comFiles"": [ ""bin\\x.ocx"" ]
              }
            }";

            var project = ProjectSerializer.Deserialize(json);

            Assert.Equal("完整工程", project.Manifest.Product.Name["zh-Hans"]);
            Assert.Equal(InstallScopes.PerMachine, project.Manifest.Scope);
            Assert.Equal(@"C:\src", project.Build.SourceDir);
            Assert.Equal("2026-10-08T12:00:00Z", project.Build.Timestamp);
            Assert.Single(project.Build.ComFiles);
        }

        [Fact]
        public void 工程文件往返应保持一致()
        {
            var p = NewPresenter();
            p.Document.Project.Build.SourceDir = _root;
            p.Document.Project.Manifest.Product.Name["zh-Hans"] = "往返测试";

            var path = Path.Combine(_root, "app.wmpkg.json");
            p.Save(path);

            var loaded = ProjectSerializer.Load(path);

            Assert.Equal("往返测试", loaded.Manifest.Product.Name["zh-Hans"]);
            Assert.Equal(_root, loaded.Build.SourceDir);
        }

        [Fact]
        public void 保存的工程文件应是可读缩进_JSON_且中文不转义()
        {
            var p = NewPresenter();
            p.Document.Project.Manifest.Product.Name["zh-Hans"] = "汽车小镇";

            var json = ProjectSerializer.Serialize(p.Document.Project);

            Assert.Contains("\n", json);
            Assert.Contains("汽车小镇", json);
            Assert.Contains("\"manifest\"", json);
            Assert.Contains("\"build\"", json);
        }

        [Fact]
        public void 保存后应清除脏标记并记住路径()
        {
            var p = NewPresenter();
            p.Document.Dirty = true;

            var path = Path.Combine(_root, "saved.wmpkg.json");
            p.Save(path);

            Assert.False(p.Document.Dirty);
            Assert.Equal(path, p.Document.Path);
        }

        // ─────────────────────────────────────────────────────────────
        // 打包
        // ─────────────────────────────────────────────────────────────

        [Fact]
        public void 快捷方式名应跟随产品名()
        {
            var p = NewPresenter();
            p.Document.Project.Build.SourceDir = NewSource();
            p.Document.Project.Manifest.Product.Name = new LocalizedText { { "zh-Hans", "新名字" } };
            p.Document.Project.Manifest.Product.Version = "1.0";
            p.Document.Project.Manifest.EntryPoint = "App0.exe";
            p.Document.Project.Manifest.Shortcuts = new List<ShortcutSpec>
            {
                new ShortcutSpec { Location = "Desktop", Name = new LocalizedText { { "zh-Hans", "旧名字" } } },
            };

            p.Build();

            Assert.Equal("新名字", p.Document.Project.Manifest.Shortcuts[0].Name["zh-Hans"]);
        }

        [Fact]
        public void 打包应自动补上入口程序()
        {
            var p = NewPresenter();
            p.Document.Project.Build.SourceDir = NewSource();
            p.Document.Project.Manifest.Product.Name = new LocalizedText { { "zh-Hans", "自动入口" } };
            p.Document.Project.Manifest.Product.Version = "1.0";
            p.Document.Project.Manifest.EntryPoint = null;

            p.Build();

            Assert.False(string.IsNullOrWhiteSpace(p.Document.Project.Manifest.EntryPoint));
        }

        [Fact]
        public void 新建工程应生成新的_productCode()
        {
            var a = BuilderDocument.CreateDefault();
            var b = BuilderDocument.CreateDefault();

            Assert.NotEqual(a.Manifest.ProductCode, b.Manifest.ProductCode);
            Assert.StartsWith("{", a.Manifest.ProductCode);
        }

        // ─────────────────────────────────────────────────────────────
        // 界面文案
        // ─────────────────────────────────────────────────────────────

        [Fact]
        public void 界面文案应有中英两版()
        {
            var zh = new BuilderPresenter("zh-Hans", NewStub());
            var en = new BuilderPresenter("en", NewStub());

            Assert.Equal("安装包制作助手", zh.S("App.Title"));
            Assert.Equal("Installer Builder", en.S("App.Title"));
            Assert.Equal("第 1 步 · 选择要打包的文件夹", zh.S("Step.Folder"));
            Assert.Equal("Step 1 - Choose the folder to package", en.S("Step.Folder"));
        }

        [Fact]
        public void 内置文案表默认就有中英两版()
        {
            var table = Installer.Builder.Common.BuilderStrings.CreateDefault();

            Assert.True(table.Count > 40);

            foreach (var kv in table)
            {
                Assert.True(kv.Value.ContainsKey("zh-Hans"), kv.Key + " 缺少中文");
                Assert.True(kv.Value.ContainsKey("en"), kv.Key + " 缺少英文");
            }
        }
    }
}
