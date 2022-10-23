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
            }

            foreach (var field in Fields)
            {
                builder.AppendLine($"  (global ${field.Key} (mut {Converter.GetWasmTypeWoArray(field.Value)}) ({Converter.GetWasmTypeWoArray(field.Value)}.const 0))");
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
