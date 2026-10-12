using System.Reflection;
using System.Runtime.InteropServices;

// 经典（legacy）csproj 的程序集属性写在这里。
//
// 为什么不用 SDK 的 <AssemblyAttribute> + WriteCodeFragment：
// 那是 CPS/SDK 项目的写法。VS 的经典项目系统不认这套，
// 会在「错误列表」里报分析器异常，也会让设计器加载时缺少程序集属性。
// 经典项目的惯用做法就是这个 Properties\AssemblyInfo.cs 文件。
[assembly: AssemblyTitle("Installer.UI")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Installer.UI")]
[assembly: AssemblyProduct("Installer.UI")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]
[assembly: Guid("6053b80a-a193-4c7c-b0e7-8662386a3d68")]

[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0")]
