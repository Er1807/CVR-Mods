﻿#if UNITY_EDITOR
using System;
using System.Linq;
using System.Text;
using System.Windows;
using dnlib.DotNet;

namespace Converter
{
    internal class Program
    {
        public static TypeRef TypeType;
        public static TypeRef TypeInt;
        public static TypeRef TypeFloat;
        public static TypeRef TypeDouble;
        public static TypeRef TypeLong;
        public static TypeRef TypeObject;
        public static TypeRef TypeVoid;
        static void Main(string[] args)
        {
            ModuleContext modCtx = ModuleDef.CreateModuleContext();
            ModuleDef module = ModuleDefMD.Load(args[0], modCtx);
            var type = module.Types.SingleOrDefault(x => x.FullName == args[1]);
            TypeType = new CorLibTypes(module).GetTypeRef("System", "Type");
            TypeInt = new CorLibTypes(module).GetTypeRef("System", "Int32");
            TypeLong = new CorLibTypes(module).GetTypeRef("System", "Int64");
            TypeFloat = new CorLibTypes(module).GetTypeRef("System", "Single");
            TypeDouble = new CorLibTypes(module).GetTypeRef("System", "Double");
            TypeObject = new CorLibTypes(module).GetTypeRef("System", "Object");
            TypeVoid = new CorLibTypes(module).GetTypeRef("System", "Void");
            WasmModule wasmModule = new WasmModule();
            wasmModule.declaringType = type.ToTypeSig();
            foreach (var field in type.Fields)
            {
                wasmModule.Fields.Add(field.Name, new WasmField(field));
            }

            foreach (var method in type.Methods.Where(x => !x.IsConstructor))
            {
                var mem = new Converter().Convert(wasmModule, method);
            }

            foreach (var stringOperand in wasmModule.Functions.SelectMany(x => x.Instructions).Where(x => x.Operand is WasmStringOperand).Select(x => x.Operand as WasmStringOperand))
            {
                stringOperand.Value = wasmModule.Allocate(stringOperand.StrValue);
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

                Console.Write($"{item.Value.Type}:{item.Key}|");
            }

            Console.ReadLine();

        }
    }
}
#endif