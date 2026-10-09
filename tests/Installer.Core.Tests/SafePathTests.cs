using System.IO;
using Installer.Core.Security;
using Xunit;

namespace Installer.Core.Tests
{
    /// <summary>
    /// <see cref="SafePath"/> 是安装端唯一的安全防线（现状的 Zip Slip 就是因为没有它）。
    /// 这些用例覆盖各种逃逸手法。
    /// </summary>
    public class SafePathTests
    {
        [Theory]
        [InlineData("bin\\app.exe")]
        [InlineData("bin/sub/app.exe")]
        [InlineData("中文模块.dll")]
        [InlineData("a\\b\\c\\d.txt")]
        [InlineData("file with spaces.txt")]
        [InlineData("dot.in.middle.txt")]
        public void 合法条目名应通过(string entry)
        {
            SafePath.ValidateEntryName(entry);
        }

        [Theory]
        [InlineData("..\\..\\escaped.txt")]      // 经典 Zip Slip
        [InlineData("../escaped.txt")]           // 正斜杠变体
        [InlineData("a\\..\\..\\b.txt")]         // 中间穿越
        [InlineData("..")]
        [InlineData("a\\..")]
        [InlineData(".\\x.txt")]                 // 显式当前目录段
        [InlineData("\\absolute.txt")]           // 根路径
        [InlineData("/absolute.txt")]
        [InlineData("C:\\Windows\\evil.dll")]    // 绝对路径
        [InlineData("C:evil.dll")]               // 驱动器相对
        [InlineData("a\\\\b.txt")]               // 空路径段
        [InlineData("a\\b\\")]                   // 结尾空段
        [InlineData("x.txt:evil")]               // NTFS 备用数据流
        [InlineData("trailing.")]                // 结尾点（Windows 会静默去掉）
        [InlineData("trailing ")]                // 结尾空格
        [InlineData("a\\trailing.\\b.txt")]
        [InlineData("CON")]                      // 保留设备名
        [InlineData("nul.txt")]
        [InlineData("COM1")]
        [InlineData("a\\LPT9.log")]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("a\0b.txt")]                 // 内嵌 NUL（路径 API 会截断）
        [InlineData("a\tb.txt")]                 // 控制字符
        public void 非法条目名应被拒绝(string entry)
        {
            Assert.ThrowsAny<System.Exception>(() => SafePath.ValidateEntryName(entry));
        }

        [Fact]
        public void ResolveWithin_正常路径应落在根目录下()
        {
            var root = Path.Combine(Path.GetTempPath(), "saferoot");
            var result = SafePath.ResolveWithin(root, "bin\\app.exe");

            Assert.True(SafePath.IsWithin(root, result));
            Assert.EndsWith("app.exe", result);
        }

        [Theory]
        [InlineData("..\\escaped.txt")]
        [InlineData("..\\..\\..\\escaped.txt")]
        [InlineData("C:\\escaped.txt")]
        public void ResolveWithin_逃逸路径应被拒绝(string entry)
        {
            var root = Path.Combine(Path.GetTempPath(), "saferoot");
            Assert.ThrowsAny<System.Exception>(() => SafePath.ResolveWithin(root, entry));
        }

        [Fact]
        public void ResolveWithin_大小写不敏感的前缀不应误判()
        {
            // C:\Temp\saferoot 与 C:\Temp\saferootX 必须区分开
            var root = Path.Combine(Path.GetTempPath(), "saferoot");
            var sibling = Path.Combine(Path.GetTempPath(), "saferootX", "a.txt");

            Assert.False(SafePath.IsWithin(root, sibling));
        }

        [Theory]
        [InlineData("a/b/c.txt", "a\\b\\c.txt")]
        [InlineData(".\\a.txt", "a.txt")]
        [InlineData("a\\b.txt", "a\\b.txt")]
        public void NormalizeRelative_应统一分隔符(string input, string expected)
        {
            Assert.Equal(expected, SafePath.NormalizeRelative(input));
        }

        [Theory]
        [InlineData("a\\b.txt", "a/b.txt")]
        [InlineData("中文\\x.dll", "中文/x.dll")]
        public void ToZipEntryName_应转成正斜杠(string input, string expected)
        {
            Assert.Equal(expected, SafePath.ToZipEntryName(input));
        }

        [Fact]
        public void SanitizeFileName_应替换非法字符()
        {
            var cleaned = SafePath.SanitizeFileName("a:b*c?.txt");
            Assert.DoesNotContain(":", cleaned);
            Assert.DoesNotContain("*", cleaned);
            Assert.DoesNotContain("?", cleaned);
            Assert.EndsWith(".txt", cleaned);
        }
    }
}
