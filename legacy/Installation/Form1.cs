using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.CSharp;

namespace Installation
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ColorDialog cd=new ColorDialog();
            label1.BackColor = cd.Color;
        }
        //源代码模板
        string codeBase = @"
                            using System;
                            using System.Windows.Forms;
                            namespace Installation
                            {
                                static class Program
                                {
                                    /// <summary>
                                    /// 应用程序的主入口点。
                                    /// </summary>
                                    [STAThread]
                                    static void Main()
                                    {
                                        Application.EnableVisualStyles();
                                        Application.SetCompatibleTextRenderingDefault(false);
                                        Application.Run(new FrmInstallation());
                                    }
                                }
                            }";


        private void button2_Click(object sender, EventArgs e)
        {
            string code = @"
                            using System;
                            using System.Windows.Forms;
                            namespace Installation
                            {
                                static class Program
                                {
                                    /// <summary>
                                    /// 应用程序的主入口点。
                                    /// </summary>
                                    [STAThread]
                                    static void Main()
                                    {
                                        Application.EnableVisualStyles();
                                        Application.SetCompatibleTextRenderingDefault(false);
                                        Application.Run(new FrmInstallation());
                                    }
                                }
                            }";
//            string code = @"using System;
//using System.Windows.Forms;
//namespace Application{
//     class App{
//       public static void Main(string[] args){
//         Console.WriteLine(" + "\"Hello,haha\"" + @");
//
//            Console.ReadLine();
//       }
//    }
//}";
             bool noInput = false;
             FileInfo sourceCode = null;
             sourceCode = new FileInfo(Application.StartupPath + "\\setup.exe");
             if (!sourceCode.Exists)
             {
                 noInput = true;
             }

             string objectExecutive = "setup.exe";
              CompilerParameters compilerParameters = new CompilerParameters();
              compilerParameters.ReferencedAssemblies.Add("System.dll");
              compilerParameters.ReferencedAssemblies.Add("System.Windows.Forms.dll");
              compilerParameters.GenerateExecutable = true;
              //compilerParameters.CompilerOptions = "/target:winexe";
              compilerParameters.OutputAssembly = objectExecutive;
              compilerParameters.IncludeDebugInformation = true;
              compilerParameters.GenerateInMemory = false;
              compilerParameters.TreatWarningsAsErrors = false;
   
              CompilerResults compilerResults = null;
              if (noInput)
              {
                  compilerResults = CodeDomProvider.CreateProvider("CSharp").CompileAssemblyFromSource(compilerParameters,code);
              }
              else
              {
                  compilerResults = CodeDomProvider.CreateProvider("CSharp").CompileAssemblyFromFile(compilerParameters, sourceCode.FullName);
              }
              if (compilerResults.Errors.Count > 0)
              {
                  Console.WriteLine("Errors:");
                  foreach (CompilerError ce in compilerResults.Errors)
                  {
                      Console.WriteLine("    {0}", ce.ToString());
                  }
              }
              else
              {
                  Console.WriteLine("Compiler Completed.");
              }
        }
        private CompilerResults CompileCode(string SourceCode, string ExeuteFileName)
        {
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters cp = new CompilerParameters(new string[] { "System.dll", "System.Windows.Forms.dll" }, ExeuteFileName, true);
            cp.CompilerOptions = "/target:winexe";
            cp.GenerateExecutable = true;
            cp.GenerateInMemory = false;
            CompilerResults cr = provider.CompileAssemblyFromSource(cp, SourceCode);
            return cr;
        }
    }
}
