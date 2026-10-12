using System.Reflection;
using System.Runtime.InteropServices;

// 经典（legacy）csproj 的程序集属性写在这里。
//
// 为什么不用 SDK 的 <AssemblyAttribute> + WriteCodeFragment：
// 那是 CPS/SDK 项目的写法。VS 的经典项目系统不认这套，
// 会在「错误列表」里报分析器异常，也会让设计器加载时缺少程序集属性。
// 经典项目的惯用做法就是这个 Properties\AssemblyInfo.cs 文件。
[assembly: AssemblyTitle("安装包制作助手")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("安装包制作助手")]
[assembly: AssemblyProduct("安装包制作助手")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]
[assembly: Guid("43ba874f-0499-4e5d-90d6-c98b1ded22cf")]

[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0")]
