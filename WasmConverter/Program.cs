using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Converter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ModuleContext modCtx = ModuleDef.CreateModuleContext();
            ModuleDefMD module = ModuleDefMD.Load(args[0], modCtx);
            var type = module.Types.SingleOrDefault(x => x.FullName == args[1]);
            WasmModule wasmModule = new WasmModule();

            foreach (var field in type.Fields)
            {
                wasmModule.Fields.Add(field.Name, field.FieldType);
            }

            foreach (var method in type.Methods.Where(x => !x.IsConstructor))
            {
                var mem = new Converter().Convert(wasmModule, method);
            }
            var str = wasmModule.CreateWat();
            Console.WriteLine(str);


            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine(Convert.ToBase64String(Encoding.UTF8.GetBytes(str)));
            Console.WriteLine();
            Console.WriteLine();
            foreach (var item in wasmModule.Fields)
            {

                Console.Write($"{item.Value}:{item.Key}|");
            }

            Console.ReadLine();
        }
    }
}
