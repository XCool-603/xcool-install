using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Installer.Abstractions.Model;
using Installer.Abstractions.Packaging;
using Installer.Abstractions.Platform;
using Installer.Builder.Common;
using Installer.Builder.Models;
using Installer.Builder.Services;
using Installer.Core.Packaging;

namespace Installer.Builder.Presenters
{
    /// <summary>
    /// 制作端的状态与业务动作。
    ///
    /// **设计目标：界面上一共只有 3 项必填** —— 文件夹、产品名称、版本号。
    /// 模板 stub 自动找（<see cref="StubLocator"/>），输出路径自动推（<see cref="BuilderDocument.SuggestOutputPath"/>），
    /// 构建时间戳默认取当前时间。其余全部收进"高级选项"。
    ///
    /// 不引用 WinForms，所以可以单测。
    /// </summary>
    public sealed class BuilderPresenter
    {
        private string _stubPath;
        private readonly BuildService _buildService;

        /// <summary>构造。</summary>
        /// <param name="culture">界面语言；null 表示跟随系统。</param>
        /// <param name="stubPath">显式指定 stub；null 表示自动找。</param>
        /// <param name="buildService">打包服务；null 表示用默认实现（测试可注入假的）。</param>
        public BuilderPresenter(string culture = null, string stubPath = null, BuildService buildService = null)
        {
            _buildService = buildService ?? new BuildService();

            Text = new StringTable(BuilderStrings.CreateDefault(),
                culture ?? StringTable.DetectRequestedCulture(), "zh-Hans");

            Document = new BuilderDocument
            {
                Project = BuilderDocument.CreateDefault(),
                Path = null,
                Dirty = false,
            };

            _stubPath = stubPath;
        }

        /// <summary>当前文档。</summary>
        public BuilderDocument Document { get; private set; }

        /// <summary>界面文案。</summary>
        public IStringTable Text { get; private set; }

        /// <summary>取本地化文案。</summary>
        public string S(string key)
        {
            var v = Text.Get(key);
            return string.IsNullOrEmpty(v) ? key : v;
        }

        /// <summary>切换界面语言。</summary>
        public void SetCulture(string culture)
        {
            Text = new StringTable(BuilderStrings.CreateDefault(), culture, "zh-Hans");
        }

        /// <summary>当前语言。</summary>
        public string Culture
        {
            get { return Text.Culture; }
        }

        // ─────────────────────────────────────────────────────────────
        // 模板 stub：自动找，用户不需要知道它的存在
        // ─────────────────────────────────────────────────────────────

        /// <summary>解析 stub 路径（显式指定优先，否则自动找）。找不到返回 null。</summary>
        public string ResolveStubPath()
        {
            if (!string.IsNullOrWhiteSpace(_stubPath) && File.Exists(_stubPath))
            {
                return _stubPath;
            }

            return StubLocator.Find();
        }

        /// <summary>显式指定 stub（只有命令行/测试会用）。</summary>
        public void UseStub(string path)
        {
            _stubPath = path;
        }

        // ─────────────────────────────────────────────────────────────
        // 文档
        // ─────────────────────────────────────────────────────────────

        /// <summary>新建工程。</summary>
        public void NewProject()
        {
            Document = new BuilderDocument
            {
                Project = BuilderDocument.CreateDefault(),
                Path = null,
                Dirty = false,
            };
        }

        // ─────────────────────────────────────────────────────────────
        // 自动记忆：productCode 必须在多次打开之间保持不变
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// 自动保存位置。
        ///
        /// **为什么必须有这个**：<c>productCode</c> 是标准卸载键的键名。
        /// 如果每次打开制作端都生成一个新 GUID，同一个产品装三次就会在
        /// 「程序和功能」里留下三条记录；卸掉其中一条后安装记录被删，
        /// 剩下的**再也卸不掉** —— 这是实际发生过的事故。
        ///
        /// 所以把上一次的工程自动记下来，下次打开直接接着用。
        /// </summary>
        public static string AutoSavePath
        {
            get
            {
                var dir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "安装包制作助手");

                return Path.Combine(dir, "last.wmpkg.json");
            }
        }

        /// <summary>尝试载入上次自动保存的工程。成功返回 true。</summary>
        public bool TryLoadAutoSaved()
        {
            try
            {
                var path = AutoSavePath;
                if (!File.Exists(path))
                {
                    return false;
                }

                Load(path);

                // 自动恢复的不算"用户打开的工程"，标题上不显示路径
                Document.Path = null;
                Document.Dirty = false;
                return true;
            }
            catch (Exception)
            {
                // 自动保存的文件坏了就当没有，不打扰用户
                return false;
            }
        }

        /// <summary>把当前工程自动记下来（失败不抛异常）。</summary>
        public void AutoSave()
        {
            try
            {
                ProjectSerializer.Save(AutoSavePath, Document.Project);
            }
            catch (Exception)
            {
                // 自动保存失败不该影响主流程
            }
        }

        /// <summary>打开工程。</summary>
        public void Load(string path)
        {
            Document = new BuilderDocument
            {
                Project = ProjectSerializer.Load(path),
                Path = path,
                Dirty = false,
            };
        }

        /// <summary>保存工程。</summary>
        public void Save(string path)
        {
            ProjectSerializer.Save(path, Document.Project);
            Document.Path = path;
            Document.Dirty = false;
        }

        /// <summary>把一个已存在的安装包反解成工程。</summary>
        public void ImportFromPackage(string packagePath)
        {
            using (var fs = File.OpenRead(packagePath))
            {
                var info = ContainerFormat.Read(fs);
                var manifest = ContainerFormat.ReadManifest(fs, info);

                manifest.Files = new List<FileEntry>();
                manifest.Directories = new List<string>();
                manifest.Build = null;

                Document = new BuilderDocument
                {
                    Project = new InstallerProject
                    {
                        Manifest = manifest,
                        Build = new ProjectBuildSettings(),
                    },
                    Path = null,
                    Dirty = true,
                };
            }
        }

        // ─────────────────────────────────────────────────────────────
        // 输出路径：自动推导
        // ─────────────────────────────────────────────────────────────

        /// <summary>当前应当使用的输出路径（用户没固定就用自动推导的）。</summary>
        public string ResolveOutputPath()
        {
            var build = Document.Project.Build ?? new ProjectBuildSettings();

            if (!string.IsNullOrWhiteSpace(build.OutputPath))
            {
                return build.OutputPath;
            }

            var m = Document.Project.Manifest;
            return BuilderDocument.SuggestOutputPath(
                PickProductName(m),
                m.Product.Version,
                build.SourceDir);
        }

        /// <summary>
        /// 取一个**非空**的产品名。
        /// 中英文都可以留空其中一个，所以不能直接取字典里的第一个 —— 那可能正好是空的那个。
        /// </summary>
        public string PickProductName(InstallerManifest m)
        {
            var name = m == null || m.Product == null ? null : m.Product.Name;
            if (name == null || name.Count == 0)
            {
                return null;
            }

            // 优先默认语言
            string preferred;
            if (!string.IsNullOrWhiteSpace(m.DefaultCulture)
                && name.TryGetValue(m.DefaultCulture, out preferred)
                && !string.IsNullOrWhiteSpace(preferred))
            {
                return preferred.Trim();
            }

            var first = name.Values.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));
            return first == null ? null : first.Trim();
        }

        // ─────────────────────────────────────────────────────────────
        // 校验：只有 3 项是必须的
        // ─────────────────────────────────────────────────────────────

        /// <summary>校验工程，返回问题列表（空表示可以打包）。</summary>
        public IList<string> Validate()
        {
            var problems = new List<string>();
            var m = Document.Project.Manifest;
            var b = Document.Project.Build ?? new ProjectBuildSettings();

            if (string.IsNullOrWhiteSpace(b.SourceDir))
            {
                problems.Add("还没选要打包的文件夹。");
            }
            else if (!Directory.Exists(b.SourceDir))
            {
                problems.Add("文件夹不存在：" + b.SourceDir);
            }
            else if (Directory.GetFileSystemEntries(b.SourceDir).Length == 0)
            {
                problems.Add("这个文件夹是空的：" + b.SourceDir);
            }

            var names = m.Product.Name == null ? new List<string>() : m.Product.Name.Values.ToList();
            if (names.All(string.IsNullOrWhiteSpace))
            {
                problems.Add("产品名称不能为空（中英文至少填一个）。");
            }

            if (string.IsNullOrWhiteSpace(m.Product.Version))
            {
                problems.Add("版本号不能为空。");
            }

            if (string.IsNullOrWhiteSpace(ResolveStubPath()))
            {
                problems.Add(StubLocator.MissingHint());
            }

            var output = ResolveOutputPath();
            if (string.IsNullOrWhiteSpace(output))
            {
                problems.Add("输出路径为空。");
            }

            if (!string.IsNullOrWhiteSpace(b.IconPath) && !File.Exists(b.IconPath))
            {
                problems.Add("图标文件不存在：" + b.IconPath);
            }

            if (m.License != null && m.License.Required
                && (m.License.Text == null || m.License.Text.Values.All(string.IsNullOrWhiteSpace)))
            {
                problems.Add("勾了「必须接受许可协议」，但协议正文是空的。");
            }

            return problems;
        }

        // ─────────────────────────────────────────────────────────────
        // 打包
        // ─────────────────────────────────────────────────────────────

        /// <summary>执行打包。</summary>
        public PackageBuildResult Build(IProgress<BuildProgress> progress = null)
        {
            var problems = Validate();
            if (problems.Count > 0)
            {
                throw new PackageBuildException(string.Join(Environment.NewLine, problems.ToArray()));
            }

            var m = Document.Project.Manifest;
            var b = Document.Project.Build;

            // 中英文只填了一个时，把缺的那个补上：
            // 否则英文系统上跑向导会显示一个空产品名
            FillMissingLanguages(m);

            // 快捷方式名跟着产品名走
            SyncShortcutNames(m);

            // 入口程序没填就自动猜一个（打包文件夹里最像主程序的那个 exe）
            if (string.IsNullOrWhiteSpace(m.EntryPoint))
            {
                m.EntryPoint = GuessEntryPoint(b.SourceDir);
            }

            // 安装目录没改过就跟着产品名走
            if (string.IsNullOrWhiteSpace(m.DefaultInstallDir)
                || m.DefaultInstallDir == @"%LOCALAPPDATA%\Programs\我的产品")
            {
                m.DefaultInstallDir = @"%LOCALAPPDATA%\Programs\" + BuilderDocument.Sanitize(PickProductName(m));
            }

            DateTimeOffset? timestamp = null;
            if (!string.IsNullOrWhiteSpace(b.Timestamp))
            {
                timestamp = DateTimeOffset.Parse(b.Timestamp, CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
            }

            var options = new PackageBuildOptions
            {
                Manifest = m,
                SourceDir = b.SourceDir,
                StubPath = ResolveStubPath(),
                OutputPath = ResolveOutputPath(),
                IconPath = string.IsNullOrWhiteSpace(b.IconPath) ? null : b.IconPath,
                SkipIconPatch = string.IsNullOrWhiteSpace(b.IconPath),
                BuildTimestamp = timestamp,
                PackerVersion = _buildService.PackerVersion,
            };

            return _buildService.Build(options, progress);
        }

        /// <summary>
        /// 从文件夹名猜一个产品名。
        ///
        /// 用户选了 `D:\空中小火车`，产品名就该是"空中小火车" —— 省掉一次输入。
        /// 如果文件夹是构建输出目录（bin / Debug / publish / dist…），就往上找一级。
        /// </summary>
        public static string SuggestProductName(string sourceDir)
        {
            if (string.IsNullOrWhiteSpace(sourceDir))
            {
                return null;
            }

            var dir = sourceDir.TrimEnd('\\', '/');

            // 构建输出目录的名字没有信息量，往上找
            for (var i = 0; i < 4 && !string.IsNullOrEmpty(dir); i++)
            {
                var name = Path.GetFileName(dir);
                if (string.IsNullOrEmpty(name))
                {
                    break;
                }

                if (!IsBuildOutputName(name))
                {
                    return name;
                }

                var parent = Path.GetDirectoryName(dir);
                if (string.IsNullOrEmpty(parent))
                {
                    break;
                }

                dir = parent;
            }

            return null;
        }

        private static bool IsBuildOutputName(string name)
        {
            var names = new[]
            {
                "bin", "obj", "debug", "release", "publish", "output", "out",
                "dist", "build", "x86", "x64", "anycpu", "net48", "net6.0", "net8.0",
            };

            return names.Any(n => string.Equals(n, name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>在打包文件夹里猜一个入口程序。</summary>
        public static string GuessEntryPoint(string sourceDir)
        {
            var exes = FindExecutables(sourceDir);

            if (exes.Count == 0)
            {
                return null;
            }

            // 优先根目录下的（"bin\App.exe" 这种排在 "App.exe" 后面）
            var root = exes.FirstOrDefault(e => e.IndexOf('\\') < 0);
            return root ?? exes[0];
        }

        /// <summary>
        /// 列出打包文件夹里所有可执行文件（相对路径）。
        /// 顺序：根目录的排前面，其余按路径排序 —— 界面上给用户挑"启动程序"用。
        /// 卸载器（Uninstall*.exe）会被排除。
        /// </summary>
        public static List<string> FindExecutables(string sourceDir)
        {
            var result = new List<string>();

            if (string.IsNullOrWhiteSpace(sourceDir) || !Directory.Exists(sourceDir))
            {
                return result;
            }

            var rootPath = sourceDir.TrimEnd('\\', '/');

            try
            {
                foreach (var full in Directory.GetFiles(rootPath, "*.exe", SearchOption.AllDirectories))
                {
                    var name = Path.GetFileName(full);
                    if (name.StartsWith("Uninstall", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var rel = full.Substring(rootPath.Length).TrimStart('\\', '/').Replace('/', '\\');
                    result.Add(rel);
                }
            }
            catch (IOException)
            {
                return result;
            }
            catch (UnauthorizedAccessException)
            {
                return result;
            }

            return result
                .OrderBy(e => e.IndexOf('\\') < 0 ? 0 : 1)
                .ThenBy(e => e, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        /// <summary>
        /// 中英文只填了一个时，把缺的那个补上。
        /// 否则用户在英文系统上跑向导会看到一个空的产品名。
        /// </summary>
        public static void FillMissingLanguages(InstallerManifest m)
        {
            if (m == null || m.Product == null || m.Product.Name == null)
            {
                return;
            }

            var fallback = m.Product.Name.Values.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));
            if (string.IsNullOrWhiteSpace(fallback))
            {
                return;
            }

            var cultures = m.Cultures != null && m.Cultures.Count > 0
                ? m.Cultures
                : new List<string>(m.Product.Name.Keys);

            foreach (var culture in cultures)
            {
                string existing;
                if (!m.Product.Name.TryGetValue(culture, out existing) || string.IsNullOrWhiteSpace(existing))
                {
                    m.Product.Name[culture] = fallback.Trim();
                }
            }

            // 厂商也照同样处理
            if (m.Product.Publisher != null)
            {
                var pubFallback = m.Product.Publisher.Values.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));
                if (!string.IsNullOrWhiteSpace(pubFallback))
                {
                    foreach (var culture in cultures)
                    {
                        string existing;
                        if (!m.Product.Publisher.TryGetValue(culture, out existing) || string.IsNullOrWhiteSpace(existing))
                        {
                            m.Product.Publisher[culture] = pubFallback.Trim();
                        }
                    }
                }
            }
        }

        /// <summary>统计源目录里的文件数与总大小。</summary>
        public static void MeasureSource(string sourceDir, out int fileCount, out long totalBytes)
        {
            fileCount = 0;
            totalBytes = 0;

            if (string.IsNullOrWhiteSpace(sourceDir) || !Directory.Exists(sourceDir))
            {
                return;
            }

            try
            {
                foreach (var f in Directory.EnumerateFiles(sourceDir, "*", SearchOption.AllDirectories))
                {
                    fileCount++;
                    try
                    {
                        totalBytes += new FileInfo(f).Length;
                    }
                    catch (IOException)
                    {
                    }
                }
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        private static void SyncShortcutNames(InstallerManifest m)
        {
            if (m.Shortcuts == null || m.Product == null || m.Product.Name == null)
            {
                return;
            }

            foreach (var s in m.Shortcuts)
            {
                s.Name = new LocalizedText();
                foreach (var kv in m.Product.Name)
                {
                    s.Name[kv.Key] = kv.Value;
                }
            }
        }
    }
}
