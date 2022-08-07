using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter
{
    public class WasmFunction
    {
        public WasmModule Module;

        public WasmFunction(WasmModule module)
        {
            Module = module;
        }

        public string Name;
        public WasmDataType? ReturnType;
        public List<WasmDataType> Parameters = new List<WasmDataType>();
        public Dictionary<string, WasmDataType> Locals = new Dictionary<string, WasmDataType>();
        public List<WasmInstruction> Instructions = new List<WasmInstruction>();



        public Stack<WasmDataType> stack = new Stack<WasmDataType>();


        public void FixControlFlow()
        {
            var branchInstructions = Instructions.Where(x => x.Instruction == WasmInstructions.br
                || x.Instruction == WasmInstructions.br_if).ToList();
            var branchTargets = branchInstructions.Select(x => (uint)x.Operand)
                .Distinct().OrderBy(x=>x).ToList();

            Dictionary<uint, string> branchTargetLookup = new Dictionary<uint, string>();
            
            foreach (var target in branchTargets)
            {
                Instructions.Insert(0, new WasmInstruction(WasmInstructions.block, 9999, $"bl{target}"));
                var index = Instructions.FindIndex(x => x.Offset == target);
                Instructions.Insert(index, new WasmInstruction(WasmInstructions.end, 9999));
            }

            foreach (var instr in branchInstructions)
            {
                instr.Operand = $"bl{(uint)instr.Operand}";
            }
        }
        

        public string CreateWat()
        {
            var builder = new StringBuilder();
            builder.AppendLine($"  (func ${Name} {BuildParamString(Parameters)} {BuildResultString(ReturnType)}");

            foreach (var local in Locals)
            {
                if (local.Key.StartsWith("param"))
                    continue;
                builder.AppendLine($"    (local ${local.Key} {local.Value})");
            }

            foreach (var inst in Instructions)
            {
                builder.AppendLine(inst.ToInstructionString());

            }

            builder.AppendLine($"  )");
            return builder.ToString();
        }

        public static string BuildParamString(List<WasmDataType> parameters, bool dontIncludeName = false, bool includeThis = false)
        {
            if (includeThis)
                parameters.Insert(0, WasmDataType.i32);
            if (parameters.Count == 0)
            {
                return "";
            }
            var builder = new StringBuilder();
            builder.Append("(param");
            for (int i = 0; i < parameters.Count; i++)
            {
                if (dontIncludeName)
                    builder.Append($" {parameters[i]}");
                else
                    builder.Append($" $param{i} {parameters[i]}");
            }
            builder.Append(")");
            return builder.ToString();
        }

        public static string BuildResultString(WasmDataType? returnType)
        {
            if (!returnType.HasValue)
            {
                return "";
            }
            return $"(result {returnType.Value})";
        }
    }
}
