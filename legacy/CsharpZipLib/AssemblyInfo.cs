
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if (NET_1_0)
[assembly: AssemblyTitle("C#压缩解压类库 for .NET Framework 1.0")]
#elif (NET_1_1)
[assembly: AssemblyTitle("C#压缩解压类库 for .NET Framework 1.1")]
#elif (NET_2_0)
[assembly: AssemblyTitle("C#压缩解压类库")]
#elif (NET_3_0)
[assembly: AssemblyTitle("C#压缩解压类库 for .NET Framework 3.0")]
#elif (NET_3_5)
[assembly: AssemblyTitle("C#压缩解压类库 for .NET Framework 3.5")]
#elif (NETCF_1_0)
[assembly: AssemblyTitle("C#压缩解压类库 for .NET Compact Framework 1.0")]
#elif (NETCF_2_0)
[assembly: AssemblyTitle("C#压缩解压类库 for .NET Compact Framework 2.0")]
#elif (MONO_1_0)
[assembly: AssemblyTitle("C#压缩解压类库 for Mono 1.0")]
#elif (MONO_2_0)
[assembly: AssemblyTitle("C#压缩解压类库 for Mono 2.0")]
#else
[assembly: AssemblyTitle("C#压缩解压类库 unlabelled version")]
#endif

[assembly: AssemblyDescription("一款用于压缩和解压的C#类库")]
[assembly: AssemblyProduct("C#压缩解压类库")]
[assembly: AssemblyDefaultAlias("C#压缩解压类库")]
[assembly: AssemblyCulture("")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif


[assembly: AssemblyCompany("BinGoo")]
[assembly: AssemblyCopyright("Copyright 2009-2016 BinGoo")]
[assembly: AssemblyTrademark("BinGoo")]

[assembly: AssemblyVersion("1.16.05.30")]
[assembly: AssemblyInformationalVersionAttribute("1.16.05.30")]


[assembly: CLSCompliant(true)]

#if (!NETCF)
//
// If #Zip is strongly named it still allows partially trusted callers
//
[assembly: System.Security.AllowPartiallyTrustedCallers]
#endif

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

#if (CLI_1_0 || NET_1_0 || NET_1_1 || NETCF_1_0 || SSCLI)
[assembly: AssemblyDelaySign(false)]
#if VSTUDIO
[assembly: AssemblyKeyFile("../../CSharpZip.key")]
#elif AUTOBUILD
[assembly: AssemblyKeyFile("CSharpZip.key")]
#else
[assembly: AssemblyKeyFile("../CSharpZip.key")]
#endif
#endif


