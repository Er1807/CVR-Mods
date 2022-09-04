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
        public List<Block> Blocks = new List<Block>();
        public MethodDef Method;


        public Stack<WasmDataType> stack = new Stack<WasmDataType>();

        private List<(int, int)> branches = new List<(int, int)>();
        
        public void FixControlFlow()
        {
            var branchInstructions = Instructions.Where(x => x.Instruction == WasmInstructions.br
                || x.Instruction == WasmInstructions.br_if).ToList();

            foreach (var inst in branchInstructions)
            {
                if ((inst.Operand as WasmLongOperand).AsUInt < inst.Offset)
                {
                    var index = Instructions.FindLastIndex(x => x.Offset == inst.Offset);
                    Instructions.Insert(index + 1, new WasmInstruction(WasmInstructions.end, 9999, 9999));
                }
                else
                {
                    var index = Instructions.FindLastIndex(x => x.Offset == (inst.Operand as WasmLongOperand).AsUInt);
                    Instructions.Insert(index, new WasmInstruction(WasmInstructions.end, 9999, 9999));
                }
            }

            DetectForLoops();
            DetectIfs();

        }

        public void DetectIfs()
        {
            WasmInstruction br = null;
            WasmInstruction end = null;

            for (int i = 0; i < Instructions.Count; i++)
            {
                WasmInstruction inst = Instructions[i];
                
                if ((inst.Instruction == WasmInstructions.br || inst.Instruction == WasmInstructions.br_if) && inst.Operand is WasmLongOperand)
                {
                    br = inst;
                    end = Instructions[Instructions.FindIndex(x => x.Offset == (inst.Operand as WasmLongOperand).AsUInt) - 1];

                    var brIndex = Instructions.IndexOf(br);
                    var endIndex = Instructions.IndexOf(end);
                    while (Instructions[brIndex].StackSizeBefore != 0)
                    {
                        brIndex--;
                    }
                    var res = branches.FindLast(x => x.Item1 < brIndex && x.Item2 > brIndex);
                    if (res.Item1 != 0)
                        brIndex = res.Item1;
                    branches.Add((brIndex, endIndex + 1));
                    Instructions.Insert(brIndex, new WasmInstruction(WasmInstructions.block, 9999, 9999, WasmOperand.FromStaticStringmField($"bl{inst.Operand}")));
                    br.Operand = WasmOperand.FromStaticStringmField($"bl{inst.Operand}");
                }
            }
            
        }

        public void DetectForLoops()
        {
            var counter = 1;
            WasmInstruction br = null;
            WasmInstruction end = null;
            WasmInstruction brif = null;
            WasmInstruction end2 = null;

            for (int i = 0; i < Instructions.Count; i++)
            {
                WasmInstruction inst = Instructions[i];

                if (inst.Instruction == WasmInstructions.br
                    && Instructions[i + 1].Instruction == WasmInstructions.nop
                    && Instructions.Any(x => x.Instruction == WasmInstructions.br_if && x.Operand is WasmLongOperand && (x.Operand as WasmLongOperand).AsUInt == Instructions[i + 1].Offset))
                {
                    br = inst;
                    end = Instructions[Instructions.FindIndex(x => x.Offset == (inst.Operand as WasmLongOperand).AsUInt) - 1];
                    brif = Instructions.Single(x => x.Instruction == WasmInstructions.br_if && x.Operand is WasmLongOperand && (x.Operand as WasmLongOperand).AsUInt == Instructions[i + 1].Offset);
                    end2 = Instructions[Instructions.IndexOf(brif) + 1];

                    Locals.Add($"for{counter}", WasmDataType.i32);
                    Instructions.Insert(0, new WasmInstruction(WasmInstructions.i32_const, 9999,0, WasmOperand.FromInt(1)));
                    Instructions.Insert(1, new WasmInstruction(WasmInstructions.set_local, 9999,1, WasmOperand.FromStaticStringmField($"for{counter}")));

                    //loop $lp5
                    //block $bl34
                    //get_local $for1
                    //i32.const 0
                    //set_local $for1
                    //br_if $bl34
                    var index = Instructions.IndexOf(br);
                    Instructions.Insert(index, new WasmInstruction(WasmInstructions.set_local, 9999,9999, WasmOperand.FromStaticStringmField($"for{counter}")));
                    Instructions.Insert(index, new WasmInstruction(WasmInstructions.i32_const, 9999, 9999, WasmOperand.FromInt(0)));
                    Instructions.Insert(index, new WasmInstruction(WasmInstructions.get_local, 9999, 9999, WasmOperand.FromStaticStringmField($"for{counter}")));
                    Instructions.Insert(index, new WasmInstruction(WasmInstructions.block, 9999, 9999, WasmOperand.FromStaticStringmField($"bl{br.Operand}")));
                    Instructions.Insert(index, new WasmInstruction(WasmInstructions.loop, 9999, 9999, WasmOperand.FromStaticStringmField($"lp{brif.Operand}")));
                    br.Instruction = WasmInstructions.br_if;
                    br.Operand = WasmOperand.FromStaticStringmField($"bl{br.Operand}");
                    brif.Operand = WasmOperand.FromStaticStringmField($"lp{brif.Operand}");

                    counter++;
                }
            }

        }


        public string CreateWat()
        {
            var builder = new StringBuilder();
            builder.Append($"  (func ${Name}");
            for (int i = 0; i < Parameters.Count; i++)
            {
                builder.Append($" (param $param{i} {Parameters[i]})");
            }
            builder.AppendLine($" {BuildResultString(ReturnType)}");

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
