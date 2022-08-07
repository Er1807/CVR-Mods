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
        public int MemoryPtr = 4;
        
        public string CreateWat()
        {
            var builder = new StringBuilder();
            builder.AppendLine("(module");
            foreach (var str in Strings)
            {
                builder.Append($"  (data (i32.const {str.Value}) \"{str.Key}\\00\")\n");
            }
            //int at 0
            //data
            foreach (var func in Functions.SelectMany(x=>x.Instructions).Select(x=>x.Operand).Distinct().Where(x=>x is MemberRef || x is MethodDef))
            {
                if(func is MemberRef member)
                {
                    var name = "";
                    if(member.Name == ".ctor")
                    {
                        name = WasmInstruction.ConvertMethod(member.Class.FullName, member.Name, false, member.GetParams(), member.ReturnType ?? member?.FieldSig.Type);
                        builder.AppendLine($"  (import \"env\" \"{name}\"(func ${name} {WasmFunction.BuildParamString(member.GetParams().Select(x => Converter.GetWasmType(x).Value).ToList(), true)} {WasmFunction.BuildResultString(Converter.GetWasmType(member.DeclaringType.FullName))}))");
                    }
                    else
                    {
                        name = WasmInstruction.ConvertMethod(member.Class.FullName, member.Name, member.HasThis, member.GetParams(), member.ReturnType ?? member?.FieldSig.Type);
                        builder.AppendLine($"  (import \"env\" \"{name}\"(func ${name} {WasmFunction.BuildParamString(member.GetParams().Select(x => Converter.GetWasmType(x).Value).ToList(), true, member.HasThis)} {WasmFunction.BuildResultString(Converter.GetWasmType(member.ReturnType ?? member.FieldSig?.Type))}))");
                    }
                    
                    
                }
                if (func is MethodDef method)
                {
                    var name = WasmInstruction.ConvertMethod(method.DeclaringType.FullName, method.Name, method.HasThis, method.GetParams(), method.ReturnType);
                    builder.AppendLine($"  (import \"env\" \"{name}\"(func ${name} {WasmFunction.BuildParamString(method.GetParams().Select(x => Converter.GetWasmType(x).Value).ToList(), true)} {WasmFunction.BuildResultString(Converter.GetWasmType(method.ReturnType))}))");
                }
            }

            foreach (var function in Functions)
            {
                builder.Append($"  (export \"{function.Name}\" (func ${function.Name}))\n");
            }
            builder.AppendLine("  (memory (export \"memory\") 1 2)");

            foreach (var func in Functions)
            {
                builder.AppendLine(func.CreateWat());
            }
            builder.AppendLine($")");

            return builder.ToString();
        }

    }
}
