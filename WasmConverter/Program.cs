#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public static TypeRef dontUseThisClass;
        static void Main(string[] args)
        {
            ModuleContext modCtx = ModuleDef.CreateModuleContext();
            ModuleDef module = ModuleDefMD.Load(args[0], modCtx);
            dontUseThisClass = module.CorLibTypes.GetTypeRef("", "DontUseThisClass");

            var type = module.Types.SingleOrDefault(x => x.FullName == args[1]);

            for (int i = 0; i < module.Types.Count; i++)
            {
                if(type != module.Types[i])
                {
                    module.Types.Remove(module.Types[i]);
                    i--;
                }
            }
            
            List<ITypeDefOrRef> toRename = new List<ITypeDefOrRef>();

            foreach (var method in type.Methods.Where(x => !x.IsConstructor))
            {
                for (int i = 0; i < method.Body.Instructions.Count; i++)
                {
                    Instruction item = method.Body.Instructions[i];
                    if (item.OpCode == OpCodes.Callvirt || item.OpCode == OpCodes.Call)
                    {
                        HandleMethodCall(toRename, item);
                    }
                    else if (item.OpCode == OpCodes.Ldlen)
                    {
                        var before = method.Body.Instructions[i - 1];
                        HandleArrayLength(module, item, before);
                    }
                    else if (item.OpCode == OpCodes.Newarr)
                    {
                        HandleNewArray(module, item);
                    }
                    else if (item.OpCode == OpCodes.Ldelem_Ref || item.OpCode == OpCodes.Ldelem)
                    {
                        HandleGetArray(module, item);
                    }
                    else if (item.OpCode == OpCodes.Stelem)
                    {
                        HandleSetArray(module, item);
                    }
                }
            }

            foreach (var item in toRename)
            {

                item.Name = "DontUseThisClass";
            }

            module.Write("E:\\Temp\\test.dll");

            return;

        }

        private static void HandleMethodCall(List<ITypeDefOrRef> toRename, Instruction item)
        {
            var refs = (item.Operand as MemberRef);
            var refs2 = (item.Operand as MethodSpec);
            var refs3 = (item.Operand as MethodDef);
            if (refs != null && !refs.Name.Contains("__"))
            {
                
                //item.OpCode = OpCodes.Call;
                refs.Name = ConvertMethod(refs.DeclaringType, refs.Name, refs.MethodSig.HasThis, refs.MethodSig.Params, refs.MethodSig.RetType);
                
                if (refs.MethodSig.HasThis)
                {
                    refs.MethodSig.HasThis = false;
                    refs.MethodSig.Params.Insert(0, refs.DeclaringType.ToTypeSig());
                }

                toRename.Add(refs.DeclaringType);

            }
            if (refs2 != null && !refs2.Name.Contains("__"))
            {
                refs2.Name = ConvertMethod(refs2.Method.DeclaringType, refs2.Name, refs2.Method.MethodSig.HasThis, refs2.Method.MethodSig.Params, refs2.Method.MethodSig.RetType);

                //item.OpCode = OpCodes.Call;
                if (refs2.Method.MethodSig.HasThis)
                {
                    refs2.Method.MethodSig.HasThis = false;
                    refs2.Method.MethodSig.Params.Insert(0, refs2.DeclaringType.ToTypeSig());
                }

                toRename.Add(refs2.DeclaringType);

            }
            if (refs3 != null && !refs3.Name.Contains("__"))
            {
                refs3.Name = ConvertMethod(refs3.DeclaringType, refs3.Name, refs3.MethodSig.HasThis, refs3.MethodSig.Params, refs3.MethodSig.RetType);
                if (refs3.MethodSig.HasThis)
                {
                    refs3.MethodSig.HasThis = false;
                    refs3.MethodSig.Params.Insert(0, refs3.DeclaringType.ToTypeSig());
                }
                //item.OpCode = OpCodes.Call;
                toRename.Add(refs3.DeclaringType);

            }
        }

        private static void HandleArrayLength(ModuleDef module, Instruction current, Instruction before)
        {
            if (before.Operand is FieldDef field)
            {
                current.OpCode = OpCodes.Call;
                switch (field.FieldType.TypeName)
                {
                    case "":

                        break;
                    default:
                        current.Operand = CreateFunction(module, "Arr_Count_Object", module.CorLibTypes.Int32, module.CorLibTypes.Int32);
                        break;
                }
            }
            else
            {
                throw new Exception("Could not get array type");
            }
        }

        private static void HandleNewArray(ModuleDef module, Instruction current)
        {
            current.OpCode = OpCodes.Call;
            switch ((current.Operand as TypeRef).Name)
            {
                case "":

                    break;
                default:
                    current.Operand = CreateFunction(module, "Newarr_Obj", module.CorLibTypes.Int32, module.CorLibTypes.Int32);
                    break;
            }
        }
        private static void HandleGetArray(ModuleDef module, Instruction current)
        {
            if(current.OpCode == OpCodes.Ldelem_Ref || current.OpCode == OpCodes.Ldelem)
            {
                current.Operand = CreateFunction(module, "Arr_Get_Object", module.CorLibTypes.Int32, module.CorLibTypes.Int32, module.CorLibTypes.Int32);
            }
            current.OpCode = OpCodes.Call;
        }
        private static void HandleSetArray(ModuleDef module, Instruction current)
        {
            if (current.OpCode == OpCodes.Stelem)
            {
                current.Operand = CreateFunction(module, "Arr_Set_Object",module.CorLibTypes.Void, module.CorLibTypes.Int32, module.CorLibTypes.Int32, module.CorLibTypes.Object);
            }
            current.OpCode = OpCodes.Call;
        }

        static Dictionary<(string, TypeSig, TypeSig[]), MemberRefUser> cache = new Dictionary<(string, TypeSig, TypeSig[]), MemberRefUser>();

        public static MemberRefUser CreateFunction(ModuleDef module, string name, TypeSig returnType, params TypeSig[] parameters)
        {
            if (!cache.ContainsKey((name, returnType, parameters)))
                cache[(name, returnType, parameters)] = new MemberRefUser(module, name, MethodSig.CreateStatic(returnType, parameters), dontUseThisClass);

            return cache[(name, returnType, parameters)];
        }

        public static string ConvertMethod(ITypeDefOrRef type, string name, bool hasThis, IList<TypeSig> parameters, TypeSig returnType)
        {
            return $"{type.FullName.Replace(".", "_").Replace("+", "_")}__{name.Replace(".", "").Replace("+", "_")}{GetParamStr(hasThis, parameters, returnType)}";
        }
        public static string GetParamStr(bool hasThis, IList<TypeSig> parameters, TypeSig returnType)
        {
            StringBuilder builder = new StringBuilder();

            if (hasThis)
                builder.Append("_this");

            foreach (var item in parameters)
            {
                if (item.FullName.StartsWith("System.Nullable`1<"))
                    builder.Append("_" + item.FullName.Replace("System.Nullable`1<", "").Replace(">", "").Replace(".", ""));
                else
                    builder.Append($"_{item.FullName.Replace(".", "")}");

            }

            if (returnType != null)
            {
                if (returnType.FullName.StartsWith("System.Nullable`1<"))
                    builder.Append("__" + returnType.FullName.Replace("System.Nullable`1<", "").Replace(">", "").Replace(".", ""));
                else
                    builder.Append($"__{returnType.FullName.Replace(".", "")}");
            }

            return builder.ToString().Replace("[]", "__").Replace("+", "_");
        }
    }
}
#endif