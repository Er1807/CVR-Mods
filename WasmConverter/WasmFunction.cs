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

        private List<(int, int)> branches = new List<(int, int)>();

        public void FixControlFlow()
        {
            var counter = 1;
            var branchInstructions = Instructions.Where(x => x.Instruction == WasmInstructions.br
                || x.Instruction == WasmInstructions.br_if).ToList();

            

            foreach (var target in branchInstructions.Where(x => (uint)x.Operand < x.Offset)
                .Distinct())
            {
                var index = Instructions.FindIndex(x => x.Offset == target.Offset);
                var index2 = Instructions.FindIndex(x => x.Offset == (uint)target.Operand);
                Instructions.Insert(index + 1, new WasmInstruction(WasmInstructions.end, 9999));
                Instructions.Insert(index2, new WasmInstruction(WasmInstructions.loop, 9999, $"lp{target.Operand}"));

                if (Instructions[index2 - 1].Instruction == WasmInstructions.br)//loop
                {
                    Locals.Add($"for{counter}", WasmDataType.i32);
                    Instructions.Insert(0, new WasmInstruction(WasmInstructions.i32_const, 9999, 1));
                    Instructions.Insert(1, new WasmInstruction(WasmInstructions.set_local, 9999, $"for{counter}"));

                    Instructions.Insert(index2 + 3, new WasmInstruction(WasmInstructions.get_local, 9999, $"for{counter}"));
                    Instructions.Insert(index2 + 4, new WasmInstruction(WasmInstructions.i32_const, 9999, 0));
                    Instructions.Insert(index2 + 5, new WasmInstruction(WasmInstructions.set_local, 9999, $"for{counter}"));
                    Instructions[index2 + 1].Instruction = WasmInstructions.br_if;
                    Instructions.Insert(index2 + 6, Instructions[index2 + 1]);
                    Instructions.RemoveAt(index2 + 1);
                    counter++;
                }

                index2 = Instructions.FindIndex(x => x.Offset == (uint)target.Operand);
            
            }

            foreach (var target in branchInstructions.Where(x => (uint)x.Operand > x.Offset).Distinct().OrderBy(x => (uint)x.Operand))
            {
                var index = Instructions.FindIndex(x => x.Offset == (uint)target.Operand);
                var index2 = Instructions.FindIndex(x => x.Offset == target.Offset);
                while (Instructions[index2 - 1].Instruction == WasmInstructions.get_local)
                {
                    index2--;
                }
                if(Instructions[index2 - 4].Instruction == WasmInstructions.loop)
                {
                    index2 -= 4;
                    index2++;
                }
                var res = branches.FindLast(x => x.Item1 < index2 && x.Item2 > index2);
                if (res.Item1 != 0)
                    index2 = res.Item1;
                branches.Add((index2, index + 1));
                Instructions.Insert(index2, new WasmInstruction(WasmInstructions.block, 9999, $"bl{target.Operand}"));
                Instructions.Insert(index + 1, new WasmInstruction(WasmInstructions.end, 9999));
            }



            foreach (var instr in branchInstructions)
            {
                if((uint)instr.Operand > instr.Offset)
                    instr.Operand = $"bl{(uint)instr.Operand}";
                else
                    instr.Operand = $"lp{(uint)instr.Operand}";
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
