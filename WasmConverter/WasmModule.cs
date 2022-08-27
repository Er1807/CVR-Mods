using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter
{
    public class WasmModule
    {
        public List<WasmFunction> Functions = new List<WasmFunction>();
        public Dictionary<string, int> Strings = new Dictionary<string, int>();
        public Dictionary<string, TypeSig> Fields = new Dictionary<string, TypeSig>();
        public int MemoryPtr = 4;
        public TypeSig declaringType;

        public string CreateWat()
        {
            var builder = new StringBuilder();
            builder.AppendLine("(module");
            foreach (var str in Strings)
            {
                builder.AppendLine($"  (data (i32.const {str.Value}) \"{str.Key}\\00\")");
            }
            //int at 0
            //data
            foreach (var function in Functions.SelectMany(x => x.Instructions).Where(x => x.Instruction == WasmInstructions.call && x.Operand is WasmExternFunctionOperand).Select(x => x.Operand as WasmExternFunctionOperand).GroupBy(x => x.FunctionName).Select(x => x.First()))
            {
                builder.AppendLine($@"  (import ""env"" ""{function.FunctionName}"" (func ${function.FunctionName} {WasmFunction.BuildParamString(function.Params.Select(x => Converter.GetWasmType(x).Value).ToList(), true)} {WasmFunction.BuildResultString(Converter.GetWasmType(function.ReturnValue.FullName))}))");

                /*if (func is MethodDef method)
                {
                    if (method.Name == ".ctor")
                    {
                        var namectr = WasmInstruction.ConvertMethod(method.DeclaringType.FullName, method.Name, false, method.GetParams(), method.ReturnType ?? method?.DeclaringType.ToTypeSig());
                        builder.AppendLine($"  (import \"env\" \"{namectr}\"(func ${namectr} {WasmFunction.BuildParamString(method.GetParams().Select(x => Converter.GetWasmType(x).Value).ToList(), true)} {WasmFunction.BuildResultString(Converter.GetWasmType(method.DeclaringType.FullName))}))");
                        continue;
                    }
                    var name = WasmInstruction.ConvertMethod(method.DeclaringType.FullName, method.Name, method.HasThis, method.GetParams(), method.ReturnType);
                    builder.AppendLine($"  (import \"env\" \"{name}\"(func ${name} {WasmFunction.BuildParamString(method.GetParams().Select(x => Converter.GetWasmType(x).Value).ToList(), true)} {WasmFunction.BuildResultString(Converter.GetWasmType(method.ReturnType))}))");
                }
                if (func is MemberRef member)
                {
                    var name = "";
                    if(member.Name == ".ctor")
                    {
                        name = WasmInstruction.ConvertMethod(member.Class.FullName, member.Name, false, member.GetParams(), member.ReturnType ?? member?.FieldSig.Type);
                        builder.AppendLine($"  (import \"env\" \"{name}\"(func ${name} {WasmFunction.BuildParamString(member.GetParams().Select(x => Converter.GetWasmType(x).Value).ToList(), true)} {WasmFunction.BuildResultString(Converter.GetWasmType(member.DeclaringType.FullName))}))");
                    }
                    else if (member.IsFieldRef)
                    {
                        name = WasmInstruction.ConvertMethod(member.Class.FullName, member.Name, member.HasThis, member.GetParams(), member.ReturnType ?? member?.FieldSig.Type);
                        builder.AppendLine($"  (import \"env\" \"{name}\"(func ${name} {WasmFunction.BuildParamString(new List<WasmDataType>() { WasmDataType.i32 }, true, member.HasThis)} {WasmFunction.BuildResultString(Converter.GetWasmType(member.ReturnType ?? member.FieldSig?.Type))}))");
                    }
                    else
                    {
                        name = WasmInstruction.ConvertMethod(member.Class.FullName, member.Name, member.HasThis, member.GetParams(), member.ReturnType ?? member?.FieldSig.Type);
                        builder.AppendLine($"  (import \"env\" \"{name}\"(func ${name} {WasmFunction.BuildParamString(member.GetParams().Select(x => Converter.GetWasmType(x).Value).ToList(), true, member.HasThis)} {WasmFunction.BuildResultString(Converter.GetWasmType(member.ReturnType ?? member.FieldSig?.Type))}))");
                    }
                    
                    
                }

                if (func is MethodDef method)
                {
                    if (method.Name == ".ctor")
                    {
                        var namectr = WasmInstruction.ConvertMethod(method.DeclaringType.FullName, method.Name, false, method.GetParams(), method.ReturnType ?? method?.DeclaringType.ToTypeSig());
                        builder.AppendLine($"  (import \"env\" \"{namectr}\"(func ${namectr} {WasmFunction.BuildParamString(method.GetParams().Select(x => Converter.GetWasmType(x).Value).ToList(), true)} {WasmFunction.BuildResultString(Converter.GetWasmType(method.DeclaringType.FullName))}))");
                        continue;
                    }
                    var name = WasmInstruction.ConvertMethod(method.DeclaringType.FullName, method.Name, method.HasThis, method.GetParams(), method.ReturnType);
                    builder.AppendLine($"  (import \"env\" \"{name}\"(func ${name} {WasmFunction.BuildParamString(method.GetParams().Select(x => Converter.GetWasmType(x).Value).ToList(), true)} {WasmFunction.BuildResultString(Converter.GetWasmType(method.ReturnType))}))");
                }
                if (func is string str)
                {
                    
                    builder.AppendLine($"  (import \"env\" \"{str}\"(func ${str} (result i32)))");
                }*/
            }

            foreach (var field in Fields)
            {
                builder.AppendLine($"  (global ${field.Key} (mut {Converter.GetWasmType(field.Value)}) ({Converter.GetWasmType(field.Value)}.const 0))");
            }

            foreach (var function in Functions)
            {
                builder.Append($"  (export \"{function.Name}\" (func ${function.Name}))\n");
            }
            foreach (var field in Fields)
            {

                builder.AppendLine($"  (export \"{field.Key}\"(global ${field.Key}))");
            }
            builder.AppendLine("  (memory (export \"memory\") 1 2)");

            foreach (var func in Functions)
            {
                builder.AppendLine(func.CreateWat());
            }
            builder.AppendLine($")");

            return builder.ToString();
        }

        public int Allocate(string str)
        {
            if (Strings.ContainsKey(str))
            {
                return Strings[str];
            }
            else
            {
                Strings.Add(str, MemoryPtr);
                MemoryPtr += str.Length + 1;
                return Strings[str];
            }
        }

    }
}
