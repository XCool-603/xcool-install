using System.Runtime.CompilerServices;

// WizardHostImpl 是 internal —— 测试要能直接构造它，才能把「向导安装路径」钉住
// （「没点完成就自动运行」那个 bug 就是从这条缝里漏过去的）。
//
// ⚠️ 这里手写程序集特性，而不是在 csproj 里写 <InternalsVisibleTo Include="..." />。
//    那个 MSBuild 项是 .NET SDK **2.0+** 才支持的（由 GenerateAssemblyInfo 生成
//    等价的特性）。VS2017 自带 1.x SDK，它不认这个项，测试项目在 VS2017 里会报：
//        CS0122: "WizardHostImpl" 不可访问，因为它具有一定的保护级别
//    写成特性则所有 SDK 版本通吃。
//
// 注意：SDK 风格项目默认 GenerateAssemblyInfo=true，会自动生成
// AssemblyVersion / AssemblyTitle 等特性，但**不含** InternalsVisibleTo，
// 所以本文件只放这一个特性不会冲突。
[assembly: InternalsVisibleTo("Installer.Core.Tests")]
