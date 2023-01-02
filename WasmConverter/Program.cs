#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;

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

            for (int i = 0; i < module.Types.Count; i++)
            {
                if(type != module.Types[i])
                {
                    module.Types.Remove(module.Types[i]);
                    i--;
                }
            }
            var systemConsole = module.CorLibTypes.GetTypeRef("", "DontUseThisClass");
            var writeLine2 =  new MemberRefUser(module, "Arr_Length",
                            MethodSig.CreateStatic(module.CorLibTypes.Int32, module.CorLibTypes.Object),
                            systemConsole);

            List<ITypeDefOrRef> toRename = new List<ITypeDefOrRef>();

            foreach (var method in type.Methods.Where(x => !x.IsConstructor))
            {
                foreach (var item in method.Body.Instructions)
                {
                    if (item.OpCode == OpCodes.Callvirt || item.OpCode == OpCodes.Call)
                    {
                        var refs = (item.Operand as MemberRef);
                        var refs2 = (item.Operand as MethodSpec);
                        var refs3 = (item.Operand as MethodDef);
                        if (refs != null && !refs.Name.Contains("__"))
                        {
                            if (refs.MethodSig.HasThis)
                            {
                                refs.MethodSig.HasThis = false;
                                refs.MethodSig.Params.Insert(0, refs.DeclaringType.ToTypeSig());
                            }
                            //item.OpCode = OpCodes.Call;
                            refs.Name = refs.DeclaringType.Name + "__" + refs.Name;
                            toRename.Add(refs.DeclaringType);

                        }
                        if (refs2 != null && !refs2.Name.Contains("__"))
                        {
                            //item.OpCode = OpCodes.Call;
                            refs2.Name = refs2.DeclaringType.Name + "__" + refs2.Name;
                            toRename.Add(refs2.DeclaringType);

                        }
                        if (refs3 != null && !refs3.Name.Contains("__"))
                        {
                            if (refs3.MethodSig.HasThis)
                            {
                                refs3.MethodSig.HasThis = false;
                                refs3.MethodSig.Params.Insert(0, refs3.DeclaringType.ToTypeSig());
                            }
                            //item.OpCode = OpCodes.Call;
                            refs3.Name = refs3.DeclaringType.Name + "__" + refs3.Name;
                            toRename.Add(refs3.DeclaringType);

                        }
                    }
                    if (item.OpCode == OpCodes.Ldlen)
                    {
                        item.OpCode = OpCodes.Call;
                        item.Operand = writeLine2;
                    }
                }
            }

            foreach (var item in toRename)
            {

                item.Name = "DontUseThisClass";
            }

            module.Write("E:\\Temp\\test.dll");

            return;
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