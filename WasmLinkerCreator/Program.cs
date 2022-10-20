using System;
using WrapperCodeGenerator;

namespace LinkerCreator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WrapperGenerator.GenerateClass(typeof(System.String), new System.Collections.Generic.List<string>());
            Console.WriteLine(builder.ToString());
        }
        
    }
}
