#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Xml.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;
using static System.Net.Mime.MediaTypeNames;

namespace Converter
{
    internal class Program
    {
        private const string DnSpyConsole = "C:\\Users\\Eric\\Downloads\\dnSpy-v6.2.0-64bit\\dnSpy.Console.exe";

        static void Main(string[] args)
        {
            ILPreparer.PrepareClass(args[0], args[1], "E:\\Temp\\test.dll");
            string methods = GenerateCode(args[1], "E:\\Temp\\test.dll");
            var cleanMethods = CleanupCode(methods);
            string signatures = ExtractSignatures(cleanMethods);

            Console.WriteLine("// Header");
            Console.WriteLine("#include \"header.h\"");
            Console.WriteLine();
            Console.WriteLine("// Signatures");
            Console.WriteLine(signatures);
            Console.WriteLine("// Methods");
            Console.WriteLine(cleanMethods);
            return;

        }

        private static string ExtractSignatures(string methods)
        {
            var matchers = Regex.Matches(methods, "\\w+ \\w+ \\w+\\(.*?\\)");

            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < matchers.Count; i++)
            {
                builder.AppendLine(matchers[i].Value + ";");

            }

            var signatures = builder.ToString();
            return signatures;
        }

        private static string CleanupCode(string methods)
        {
            //Cleanup
            methods = methods.Replace("this.", "");
            methods = methods.Replace("DontUseThisClass.", "");
            methods = methods.Replace("using ", "// using ");
            methods = Regex.Replace(methods, " \\d+f", m => m.Groups[0].Value.Replace("f", ""));
            methods = Regex.Replace(methods, "public class ([A-z]+) .*?{", m => $"//Class {m.Groups[1].Value}", RegexOptions.Singleline);
            methods = Regex.Replace(methods, "}[\r\n ]*$", m => $"// End Class", RegexOptions.Singleline);
            return methods;
        }

        private static string GenerateCode(string type, string dll)
        {
            // Use ProcessStartInfo class
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            //startInfo.UseShellExecute = false;
            startInfo.FileName = DnSpyConsole;
            startInfo.WindowStyle = ProcessWindowStyle.Maximized;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            //startInfo.RedirectStandardError = true;
            startInfo.Arguments = $"-t {type} --member-order fpmet --no-color {dll}";
            StringBuilder builder = new StringBuilder();
            try
            {
                using (Process exeProcess = Process.Start(startInfo))
                {
                    while (!exeProcess.StandardOutput.EndOfStream)
                    {
                        builder.AppendLine(exeProcess.StandardOutput.ReadLine());
                    }
                    exeProcess.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                throw;
            }
            var str = builder.ToString();
            return str;
        }
    }
}
#endif