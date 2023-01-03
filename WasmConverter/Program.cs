#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Xml.Linq;
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
                if (type != module.Types[i])
                {
                    module.Types.Remove(module.Types[i]);
                    i--;
                }
            }

            HandleFields(module, type);

            foreach (var method in type.Methods.Where(x => !x.IsConstructor))
            {
                if (!method.ReturnType.IsValueType)
                {
                    method.ReturnType = module.CorLibTypes.Int32;
                }
                foreach (var item in method.Body.Variables)
                {
                    if (!item.Type.IsValueType)
                    {
                        item.Type = module.CorLibTypes.Int32;
                    }
                }
                for (int i = 0; i < method.Body.Instructions.Count; i++)
                {
                    Instruction item = method.Body.Instructions[i];
                    if (item.OpCode == OpCodes.Callvirt || item.OpCode == OpCodes.Call)
                    {
                        HandleMethodCall(module, item,type);
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
                    else if (item.OpCode == OpCodes.Ldelem_Ref
                        || item.OpCode == OpCodes.Ldelem
                        || item.OpCode == OpCodes.Ldelem_I4
                        || item.OpCode == OpCodes.Ldelem_I8
                        || item.OpCode == OpCodes.Ldelem_R4
                        || item.OpCode == OpCodes.Ldelem_R8)
                    {
                        HandleGetArray(module, item);
                    }
                    else if (item.OpCode == OpCodes.Stelem || item.OpCode == OpCodes.Stelem_Ref
                        || item.OpCode == OpCodes.Stelem_I4
                        || item.OpCode == OpCodes.Stelem_I8
                        || item.OpCode == OpCodes.Stelem_R4
                        || item.OpCode == OpCodes.Stelem_R8)
                    {
                        HandleSetArray(module, item);
                    }
                    else if (item.OpCode == OpCodes.Ldstr)
                    {
                        method.Body.Instructions.Insert(i + 1, OpCodes.Call.ToInstruction(CreateFunction(module, "String_To_Managed", module.CorLibTypes.Int32, module.CorLibTypes.Int32)));
                    }
                    else if (item.OpCode == OpCodes.Ldfld)
                    {
                        if (item.Operand is MemberRef fieldMemberRef)
                        {
                            item.OpCode = OpCodes.Call;
                            var name = ConvertMethod(fieldMemberRef.DeclaringType, $"get_{fieldMemberRef.Name}", true, new List<TypeSig>(), fieldMemberRef.FieldSig.Type);
                            item.Operand = CreateFunction(module, name, fieldMemberRef.FieldSig.Type, new TypeSig[1] { fieldMemberRef.DeclaringType.ToTypeSig() });
                        }
                    }
                    else if (item.OpCode == OpCodes.Stfld)
                    {
                        if (item.Operand is MemberRef fieldMemberRef)
                        {
                            item.OpCode = OpCodes.Call;
                            var name = ConvertMethod(fieldMemberRef.DeclaringType, $"set_{fieldMemberRef.Name}", true, new List<TypeSig>() { fieldMemberRef.FieldSig.Type }, module.CorLibTypes.Void);
                            item.Operand = CreateFunction(module, name, module.CorLibTypes.Void, new TypeSig[2] { fieldMemberRef.DeclaringType.ToTypeSig(), fieldMemberRef.FieldSig.Type });
                        }
                    }
                    //Make branches long to mitigate errors when generating DLL do to short branches
                    else if (item.OpCode == OpCodes.Br_S)
                    {
                        item.OpCode = OpCodes.Br;
                    }
                    else if (item.OpCode == OpCodes.Brtrue_S)
                    {
                        item.OpCode = OpCodes.Brtrue;
                    }
                    else if (item.OpCode == OpCodes.Brfalse_S)
                    {
                        item.OpCode = OpCodes.Brfalse;
                    }
                }
            }


            module.Write("E:\\Temp\\test.dll");

            return;

        }

        private static void HandleFields(ModuleDef module, TypeDef type)
        {
            foreach (var item in type.Fields)
            {
                if (!item.FieldSig.Type.IsPrimitive)
                {
                    item.FieldSig = item.FieldSig.Clone();
                    item.FieldSig.Type = module.CorLibTypes.Int32;
                }
                if (item.FieldSig.Type == module.CorLibTypes.Int32)
                {
                    var getter = new MethodDefUser($"GET_{item.Name}", MethodSig.CreateStatic(module.CorLibTypes.Int32),
                                MethodImplAttributes.IL | MethodImplAttributes.Managed, MethodAttributes.Public);
                    var setter = new MethodDefUser($"SET_{item.Name}", MethodSig.CreateStatic(module.CorLibTypes.Void, module.CorLibTypes.Int32),
                                MethodImplAttributes.IL | MethodImplAttributes.Managed, MethodAttributes.Public);

                    getter.Signature.HasThis = true;
                    setter.Signature.HasThis = true;

                    getter.Body = new CilBody();
                    getter.Body.Instructions.Add(OpCodes.Ldarg_0.ToInstruction());
                    getter.Body.Instructions.Add(OpCodes.Ldfld.ToInstruction(item));
                    getter.Body.Instructions.Add(OpCodes.Ret.ToInstruction());


                    setter.Body = new CilBody();
                    setter.Body.Instructions.Add(OpCodes.Ldarg_0.ToInstruction());
                    setter.Body.Instructions.Add(OpCodes.Ldarg_1.ToInstruction());
                    setter.Body.Instructions.Add(OpCodes.Stfld.ToInstruction(item));
                    setter.Body.Instructions.Add(OpCodes.Ret.ToInstruction());

                    type.Methods.Add(getter);
                    type.Methods.Add(setter);
                }
            }
        }

        private static void HandleMethodCall(ModuleDef module, Instruction item, TypeDef type)
        {
            var refs = (item.Operand as MemberRef);
            var refs2 = (item.Operand as MethodSpec);
            var refs3 = (item.Operand as MethodDef);
            if (refs != null && refs.DeclaringType != type && refs.DeclaringType != dontUseThisClass)
            {
                
                //item.OpCode = OpCodes.Call;
                var name = ConvertMethod(refs.DeclaringType, refs.Name, refs.MethodSig.HasThis, refs.MethodSig.Params, refs.MethodSig.RetType);
                
                if (refs.MethodSig.HasThis)
                {
                    refs.MethodSig.HasThis = false;
                    refs.MethodSig.Params.Insert(0, refs.DeclaringType.ToTypeSig());
                }
                item.Operand = CreateFunction(module, name, refs.MethodSig.RetType, refs.MethodSig.Params.ToArray());

            }
            if (refs2 != null && refs2.DeclaringType != type && refs2.DeclaringType != dontUseThisClass)
            {
                var name = ConvertMethod(refs2.Method.DeclaringType, refs2.Name, refs2.Method.MethodSig.HasThis, refs2.Method.MethodSig.Params, refs2.Method.MethodSig.RetType);

                //item.OpCode = OpCodes.Call;
                if (refs2.Method.MethodSig.HasThis)
                {
                    refs2.Method.MethodSig.HasThis = false;
                    refs2.Method.MethodSig.Params.Insert(0, refs2.DeclaringType.ToTypeSig());
                }

                item.Operand = CreateFunction(module, name, refs2.Method.MethodSig.RetType, refs2.Method.MethodSig.Params.ToArray());


            }
            if (refs3 != null && refs3.DeclaringType != type)
            {
                var name = ConvertMethod(refs3.DeclaringType, refs3.Name, refs3.MethodSig.HasThis, refs3.MethodSig.Params, refs3.MethodSig.RetType);
                if (refs3.MethodSig.HasThis)
                {
                    refs3.MethodSig.HasThis = false;
                    refs3.MethodSig.Params.Insert(0, refs3.DeclaringType.ToTypeSig());
                }
                //item.OpCode = OpCodes.Call;
                item.Operand = CreateFunction(module, name, refs3.MethodSig.RetType, refs3.MethodSig.Params.ToArray());

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
            if (current.OpCode == OpCodes.Ldelem_Ref || current.OpCode == OpCodes.Ldelem)
            {
                current.Operand = CreateFunction(module, "Arr_Get_Object", module.CorLibTypes.Int32, module.CorLibTypes.Int32, module.CorLibTypes.Int32);
            }
            else if (current.OpCode == OpCodes.Ldelem_I4)
            {
                current.Operand = CreateFunction(module, "Arr_Get_Int", module.CorLibTypes.Int32, module.CorLibTypes.Int32, module.CorLibTypes.Int32);
            }
            else if (current.OpCode == OpCodes.Ldelem_I8)
            {
                current.Operand = CreateFunction(module, "Arr_Get_Long", module.CorLibTypes.Int64, module.CorLibTypes.Int32, module.CorLibTypes.Int32);
            }
            else if (current.OpCode == OpCodes.Ldelem_R4)
            {
                current.Operand = CreateFunction(module, "Arr_Get_Float", module.CorLibTypes.Single, module.CorLibTypes.Int32, module.CorLibTypes.Int32);
            }
            else if (current.OpCode == OpCodes.Ldelem_R8)
            {
                current.Operand = CreateFunction(module, "Arr_Get_Double", module.CorLibTypes.Double, module.CorLibTypes.Int32, module.CorLibTypes.Int32);
            }
            current.OpCode = OpCodes.Call;
        }
        private static void HandleSetArray(ModuleDef module, Instruction current)
        {
            if (current.OpCode == OpCodes.Stelem || current.OpCode == OpCodes.Stelem_Ref)
            {
                current.Operand = CreateFunction(module, "Arr_Set_Object", module.CorLibTypes.Void, module.CorLibTypes.Int32, module.CorLibTypes.Int32, module.CorLibTypes.Object);
            }
            else if (current.OpCode == OpCodes.Stelem_I4)
            {
                current.Operand = CreateFunction(module, "Arr_Set_Int", module.CorLibTypes.Void, module.CorLibTypes.Int32, module.CorLibTypes.Int32, module.CorLibTypes.Int32);
            }
            else if (current.OpCode == OpCodes.Stelem_I8)
            {
                current.Operand = CreateFunction(module, "Arr_Set_Long", module.CorLibTypes.Void, module.CorLibTypes.Int32, module.CorLibTypes.Int32, module.CorLibTypes.Int64);
            }
            else if (current.OpCode == OpCodes.Stelem_R4)
            {
                current.Operand = CreateFunction(module, "Arr_Set_Float", module.CorLibTypes.Void, module.CorLibTypes.Int32, module.CorLibTypes.Int32, module.CorLibTypes.Single);
            }
            else if (current.OpCode == OpCodes.Stelem_R8)
            {
                current.Operand = CreateFunction(module, "Arr_Set_Double", module.CorLibTypes.Void, module.CorLibTypes.Int32, module.CorLibTypes.Int32, module.CorLibTypes.Double);
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