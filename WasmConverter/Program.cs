﻿#if UNITY_EDITOR
using dnlib.DotNet;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Converter
{
    internal class Program
    {
        private const string DnSpyConsole = "C:\\Users\\Eric\\Downloads\\dnSpy-v6.2.0-64bit\\dnSpy.Console.exe";
        private const string Clang = "C:\\llvm\\build\\Release\\bin\\clang.exe";

        static void Main(string[] args)
        {
            ILPreparer.PrepareClass(args[0], args[1], "E:\\Temp\\test.dll");
            string methods = GenerateCode(args[1], "E:\\Temp\\test.dll");
            var cleanMethods = CleanupCode(methods);
            string signatures = ExtractSignatures(cleanMethods);
            var cFileContent = CreateCString(cleanMethods, signatures);

            File.WriteAllText("E:\\Temp\\test.c", cFileContent);
            CompileToWasm();
            return;

        }

        private static void CompileToWasm()
        {
            // Use ProcessStartInfo class
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            //startInfo.UseShellExecute = false;
            startInfo.FileName = Clang;
            startInfo.WindowStyle = ProcessWindowStyle.Maximized;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            //startInfo.RedirectStandardError = true;

            startInfo.WorkingDirectory = "E:\\Temp";
            startInfo.Arguments = $"--target=wasm32 -nostdlib  \"-Wl,--no-entry\" \"-Wl,--export-all\" \"-Wl,--allow-undefined\" -o test.wasm test.c";

            try
            {
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                throw;
            }
        }

        private static string CreateCString(string cleanMethods, string signatures)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("// Header");
            builder.AppendLine("#include \"header.h\"");
            builder.AppendLine("");
            builder.AppendLine("// Signatures");
            builder.AppendLine(signatures);
            builder.AppendLine("// Methods");
            builder.AppendLine(cleanMethods);
            return builder.ToString();
        }
#if UnityTest
        public static void ConvertSingle(WasmLoaderBehavior behavior)
        {

            if (behavior.behavior == null)
            {
                return;
            }

            UnityEngine.Debug.Log($"Converting  {behavior.gameObject.name} with type {behavior.behavior.GetClass().FullName}");

            ModuleContext modCtx = ModuleDef.CreateModuleContext();
            ModuleDef module = ModuleDefMD.Load("Library/ScriptAssemblies/Assembly-CSharp.dll", modCtx);

            ILPreparer.PrepareClass("Library/ScriptAssemblies/Assembly-CSharp.dll", behavior.behavior.GetClass().FullName, "E:\\Temp\\test.dll");
            string methods = GenerateCode(behavior.behavior.GetClass().FullName, "E:\\Temp\\test.dll");
            var cleanMethods = CleanupCode(methods);
            string signatures = ExtractSignatures(cleanMethods);
            var cFileContent = CreateCString(cleanMethods, signatures);

            File.WriteAllText("E:\\Temp\\test.c", cFileContent);
            CompileToWasm();
            return;
        }
#endif

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