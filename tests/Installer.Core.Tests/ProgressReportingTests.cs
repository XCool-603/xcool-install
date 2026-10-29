using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using Installer.Abstractions.Install;
using Installer.Abstractions.Model;
using Installer.Abstractions.Platform;
using Installer.Core.Install;
using Installer.Core.Uninstall;
using Xunit;

namespace Installer.Core.Tests
{
    /// <summary>
    /// 进度必须带**真实数据**：装到哪个文件、已装几个、共几个、多少字节。
    ///
    /// 用户反馈：进度页只显示「正在解压程序文件」这种步骤名，统计卡是设计稿里的
    /// 示例数字（123 / 199、56.4 MB / 90.6 MB），看不出到底装了什么。
    /// 这里钉住：解压与删除都会逐个文件上报 <see cref="InstallProgress.CurrentFile"/> /
    /// <c>FilesDone</c> / <c>FilesTotal</c> / <c>BytesTotal</c>。
    /// </summary>
    public class ProgressReportingTests : IDisposable
    {
        private readonly string _root;

        public ProgressReportingTests()
        {
            _root = Path.Combine(Path.GetTempPath(), "progress-" + Guid.NewGuid().ToString("N"));
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

        private sealed class Collect : IProgress<InstallProgress>
        {
            public readonly List<InstallProgress> Reports = new List<InstallProgress>();

            public void Report(InstallProgress value)
            {
                Reports.Add(value);
            }
        }

        private static byte[] MakePayload(int files)
        {
            using (var ms = new MemoryStream())
            {
                using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
                {
                    for (var i = 1; i <= files; i++)
                    {
                        var entry = zip.CreateEntry("bin\\file" + i + ".dll");
                        using (var s = entry.Open())
                        {
                            var bytes = Encoding.UTF8.GetBytes(new string('x', 100 * i));
                            s.Write(bytes, 0, bytes.Length);
                        }
                    }
                }

                return ms.ToArray();
            }
        }

        [Fact]
        public void 解压时应逐个文件上报真实进度()
        {
            const int count = 3;
            var payload = MakePayload(count);

            var manifest = TestManifest.Create();
            manifest.Files = Enumerable.Range(1, count)
                .Select(i => new FileEntry { Path = "bin\\file" + i + ".dll", Size = 100 * i })
                .ToList();

            var installDir = Path.Combine(_root, "app");
            Directory.CreateDirectory(installDir);

            var reports = new Collect();
            var ctx = new InstallContext
            {
                Manifest = manifest,
                Mode = InstallMode.Install,
                InstallDir = installDir,
                Platform = new FakePlatform(_root),
                Journal = new InstallJournal(),
                Log = new NullLogger(),
                Strings = manifest.Strings,
                Text = new StringTable(manifest.Strings, "zh-Hans", manifest.DefaultCulture),
                CancellationToken = CancellationToken.None,
                Payload = new MemoryStream(payload),
                Progress = reports,
            };

            new ExtractPayloadStep().Execute(ctx);

            var fileReports = reports.Reports.Where(r => r.FilesTotal > 0).ToList();
            Assert.NotEmpty(fileReports);

            // 总数与字节总数是真实的
            Assert.Equal(count, fileReports.First().FilesTotal);
            Assert.True(fileReports.First().BytesTotal > 0, "没有上报总字节数");

            // 逐个文件：文件数递增，且每一份报告都指明是哪个文件
            Assert.Contains(fileReports, r => r.CurrentFile != null && r.CurrentFile.Contains("file1.dll"));
            Assert.Contains(fileReports, r => r.FilesDone == count);
            Assert.True(fileReports.Last().BytesDone >= fileReports.First().BytesDone);

            // Percent 必须是 -1（逐文件上报不参与步骤百分比，否则进度条会来回跳）
            Assert.All(fileReports, r => Assert.Equal(-1, r.Percent));
        }

        [Fact]
        public void 卸载时应逐个文件上报真实进度()
        {
            var installDir = Path.Combine(_root, "prod");
            Directory.CreateDirectory(Path.Combine(installDir, "bin"));

            File.WriteAllText(Path.Combine(installDir, "bin", "a.dll"), new string('a', 200));
            File.WriteAllText(Path.Combine(installDir, "bin", "b.dll"), new string('b', 400));

            var record = new InstallRecord
            {
                ProductCode = "{AAAA1111-2222-3333-4444-555566667777}",
                ProductName = "进度测试",
                InstallDir = installDir,
                EntryPoint = "bin\\a.dll",
                Files = new List<string> { "bin\\a.dll", "bin\\b.dll" },
                FileHashes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                Directories = new List<string>(),
                Shortcuts = new List<string>(),
                RegistryValues = new List<RegistryValueRecord>(),
                LegacyKeys = new List<string>(),
            };

            var reports = new Collect();
            var result = new UninstallEngine().Run(record, new FakePlatform(_root), reports);

            Assert.True(result.Success, string.Join("; ", result.Issues.ToArray()));

            var fileReports = reports.Reports.Where(r => r.FilesTotal > 0).ToList();
            Assert.NotEmpty(fileReports);
            Assert.Equal(2, fileReports.First().FilesTotal);
            Assert.True(fileReports.First().BytesTotal >= 600, "总字节数不对：" + fileReports.First().BytesTotal);
            Assert.Contains(fileReports, r => r.CurrentFile != null && r.CurrentFile.Contains("a.dll"));
            Assert.Contains(fileReports, r => r.FilesDone == 2);
        }
    }
}
