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
                WasmDataType? wasmType = Converter.GetWasmType(field.FieldType);
                wasmModule.Fields.Add(field.Name, wasmType.Value);
            }

            foreach (var method in type.Methods.Where(x => !x.IsConstructor))
            {
                var mem = new Converter().Convert(wasmModule, method);
            }
            Console.WriteLine(wasmModule.CreateWat());
            
            Console.ReadLine();
        }
    }
}
