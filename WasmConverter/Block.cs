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
            stringBuilder.AppendLine("Block: ZeroStack");
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

        public ForBlock(int counter, List<Block> instructions, List<Block> increment, List<Block> check)
        {
            Counter = counter;
            Instructions = instructions;
            Increment = increment;
            Check = check;
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
            stringBuilder.AppendLine("Block: ForBlock");
            stringBuilder.AppendLine("SubBlock: Increment");
            foreach (var item in Increment)
            {
                stringBuilder.AppendLine(item.ToInstructionString());
            }
            stringBuilder.AppendLine("SubBlock: Check");
            foreach (var item in Check)
            {
                stringBuilder.AppendLine(item.ToInstructionString());
            }
            stringBuilder.AppendLine("SubBlock: Instructions");
            foreach (var item in Instructions)
            {
                stringBuilder.AppendLine(item.ToInstructionString());
            }
            return stringBuilder.ToString();
        }

    }
    public class IfBlock : Block
    {
        public List<(List<Block>, List<Block>)> Cases { get; set; } = new List<(List<Block>, List<Block>)>();
    }
}
