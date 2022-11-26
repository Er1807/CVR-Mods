#if UNITY_EDITOR
using dnlib.DotNet;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Converter
{
    public class WasmInstruction
    {
        public WasmInstruction(WasmInstructions instruction, uint offset, int stackSizeBefore, WasmOperand operand = null)
        {
            Instruction = instruction;
            Offset = offset;
            StackSizeBefore = stackSizeBefore;
            Operand = operand;
        }
        // Only used for blocks
        public WasmInstruction()
        {
        }

        public WasmInstructions Instruction;
        public uint Offset; //il offset
        public int StackSizeBefore;
        public WasmOperand Operand;

        public override string ToString()
        {
            return $"{Offset}:({StackSizeBefore}) {Instruction} {Operand}";
        }
        public virtual string ToInstructionString()
        {
            if (Operand is WasmExternFunctionOperand
               || Operand is WasmLocalFieldOperand
               || Operand is WasmParamFieldOperand
               || Operand is WasmGlobalFieldOperand
               || Operand is WasmLocalFieldOperand
               || Operand is WasmStaticStringOperand
               || Operand is WasmLocalFunctionOperand)
            {
                return $"    {OpCodeToText()} ${Operand}";
            }

            return $"    {OpCodeToText()} {Operand}";
        }

        public string OpCodeToText()
        {
            var str = Instruction.ToString();
            if (str.StartsWith("i32") || str.StartsWith("i64") || str.StartsWith("f32") || str.StartsWith("f64"))
            {
                var regex = new Regex(Regex.Escape("_"));
                var newText = regex.Replace(str, ".", 1);
                return newText;
            }
            else if (Instruction == WasmInstructions._return)
                return "return";
            return str;
        }


    }
}
#endif