#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter
{
    public abstract class Block
    {

        public WasmInstructions FirstInstruction;
        public uint FirstOffset; //il offset
        public WasmOperand FirstOperand;

        public WasmInstructions LastInstruction;
        public uint LastOffset; //il offset
        public WasmOperand LastOperand;

        public virtual string ToInstructionString()
        {
            return "NI";
        }
    }

    public class ZeroStackBlock : Block
    {
        public List<WasmInstruction> Instructions { get; set; } = new List<WasmInstruction>();
        public ZeroStackBlock(List<WasmInstruction> instructions)
        {
            Instructions = instructions;
            if (instructions.Count > 0)
            {
                FirstInstruction = Instructions.First().Instruction;
                FirstOperand = Instructions.First().Operand;
                FirstOffset = Instructions.First().Offset;

                LastInstruction = Instructions.Last().Instruction;
                LastOperand = Instructions.Last().Operand;
                LastOffset = Instructions.Last().Offset;
            }
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Block: ZeroStack");
            foreach (var item in Instructions)
            {
                stringBuilder.AppendLine(item.ToString());
            }
            return stringBuilder.ToString();
        }
        public override string ToInstructionString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("    ;;Block: ZeroStack");
            foreach (var item in Instructions)
            {
                stringBuilder.AppendLine(item.ToInstructionString());
            }
            return stringBuilder.ToString();
        }
    }

    public class ForBlock : Block //is also a while block
    {
        public readonly int Counter;

        public ForBlock(int counter, List<Block> instructions, List<Block> increment, List<Block> check, Block checkJump)
        {
            Counter = counter;
            Instructions = instructions;
            Increment = increment;
            Check = check;
            CheckJump = checkJump;


            if (Instructions.Count > 0)
            {
                FirstInstruction = Instructions.First().FirstInstruction;
                FirstOperand = Instructions.First().FirstOperand;
                FirstOffset = Instructions.First().FirstOffset;

                LastInstruction = Instructions.Last().LastInstruction;
                LastOperand = Instructions.Last().LastOperand;
                LastOffset = Instructions.Last().LastOffset;
            }
        }

        public List<Block> Instructions { get; set; } = new List<Block>();
        public List<Block> Increment { get; set; } = new List<Block>();
        public List<Block> Check { get; set; } = new List<Block>();
        public Block CheckJump { get; }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Block: ForBlock");
            stringBuilder.AppendLine("SubBlock: Increment");
            foreach (var item in Increment)
            {
                stringBuilder.AppendLine(item.ToString());
            }
            stringBuilder.AppendLine("SubBlock: Check");
            foreach (var item in Check)
            {
                stringBuilder.AppendLine(item.ToString());
            }
            stringBuilder.AppendLine("SubBlock: Instructions");
            foreach (var item in Instructions)
            {
                stringBuilder.AppendLine(item.ToString());
            }
            return stringBuilder.ToString();
        }
        public override string ToInstructionString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("    ;;Block: ForBlock");

            stringBuilder.AppendLine("    ;;SubBlock: CheckJump");
            stringBuilder.Append(CheckJump.ToInstructionString());
            stringBuilder.AppendLine("    ;;SubBlock: Instructions");
            foreach (var item in Instructions)
            {
                stringBuilder.Append(item.ToInstructionString());
            }
            stringBuilder.AppendLine("    ;;SubBlock: Increment");
            foreach (var item in Increment)
            {
                stringBuilder.Append(item.ToInstructionString());
            }
            stringBuilder.AppendLine("    ;;SubBlock: EnBLock");
            stringBuilder.AppendLine(new WasmInstruction(WasmInstructions.end, 9999, 0).ToInstructionString());
            stringBuilder.AppendLine("    ;;SubBlock: Check");
            foreach (var item in Check)
            {
                stringBuilder.Append(item.ToInstructionString());
            }
            stringBuilder.AppendLine(new WasmInstruction(WasmInstructions.end, 9999, 0).ToInstructionString());

            return stringBuilder.ToString();
        }

    }
    public class IfBlock : Block
    {
        public List<(List<Block>, List<Block>)> Cases { get; set; } = new List<(List<Block>, List<Block>)>();

        public IfBlock(List<(List<Block>, List<Block>)> cases)
        {
            Cases = cases;
            foreach (var cond in Cases)
            {
                if (cond.Item1 != null && cond.Item1.Last() is ZeroStackBlock block && block.LastInstruction == WasmInstructions.br_if)
                {
                    block.Instructions.Remove(block.Instructions.Last());
                    //invert result
                    block.Instructions.Insert(block.Instructions.Count, new WasmInstruction(WasmInstructions.i32_const, 9999, 0, WasmOperand.FromInt(0)));
                    block.Instructions.Insert(block.Instructions.Count, new WasmInstruction(WasmInstructions.i32_eq, 9999, 0));

                    block.LastOffset = block.Instructions.Last().Offset;
                    block.LastInstruction = block.Instructions.Last().Instruction;
                    block.LastOperand = block.Instructions.Last().Operand;
                }
            }
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Block: IfBlock");
            foreach (var ifCase in Cases)
            {
                stringBuilder.AppendLine("SubBlock: Condition");
                if (ifCase.Item1 == null)
                    stringBuilder.AppendLine("else");
                else
                    foreach (var inst in ifCase.Item1)
                    {
                        stringBuilder.AppendLine(inst.ToString());
                    }
                stringBuilder.AppendLine("SubBlock: Instructions");
                foreach (var inst in ifCase.Item2)
                {
                    stringBuilder.AppendLine(inst.ToString());
                }
            }
            return stringBuilder.ToString();
        }
        public override string ToInstructionString()
        {
            return EvaluateRecursive(Cases);
        }

        public static string EvaluateRecursive(List<(List<Block>, List<Block>)> cases)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (cases.Count == 0)
            {
                stringBuilder.AppendLine();
            }
            else if (cases[0].Item1 == null)
            {
                stringBuilder.AppendLine("    ;;SubBlock: Instructions");
                foreach (var inst in cases[0].Item2)
                {
                    stringBuilder.Append(inst.ToInstructionString());
                }
            }
            else
            {
                stringBuilder.AppendLine("    ;;SubBlock: Condition");
                foreach (var inst in cases[0].Item1)
                {
                    stringBuilder.Append(inst.ToInstructionString());
                }
                stringBuilder.AppendLine("    if");
                stringBuilder.AppendLine("    ;;SubBlock: Instructions");
                foreach (var inst in cases[0].Item2)
                {
                    stringBuilder.AppendLine(inst.ToInstructionString());
                }
                stringBuilder.AppendLine("    else");
                stringBuilder.Append(EvaluateRecursive(cases.Skip(1).ToList()));
                stringBuilder.AppendLine("    end");
            }

            return stringBuilder.ToString();
        }

    }
}
#endif