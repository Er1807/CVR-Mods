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
        static void Main(string[] args)
        {
            ILPreparer.PrepareClass(args[0], args[1], "E:\\Temp\\test.dll");


            // Use ProcessStartInfo class
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            //startInfo.UseShellExecute = false;
            startInfo.FileName = "C:\\Users\\Eric\\Downloads\\dnSpy-v6.2.0-64bit\\dnSpy.Console.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Maximized;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            //startInfo.RedirectStandardError = true;
            startInfo.Arguments= $"-t {args[1]} --member-order fpmet --no-color E:\\Temp\\test.dll";
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
                // Log error.
            }
            var str = builder.ToString();

            str = str.Replace("this.", "").Replace("DontUseThisClass.", "");
            var replaced = Regex.Replace(str, " \\d+f", m => m.Groups[0].Value.Replace("f", ""));
            Console.WriteLine(replaced);
            return;

        }
    }
}
#endif