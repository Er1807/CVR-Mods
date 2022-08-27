using dnlib.DotNet;
using System.Collections.Generic;
using System.Text;

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

        public WasmInstructions Instruction;
        public uint Offset; //il offset
        public int StackSizeBefore;
        public WasmOperand Operand;

        public override string ToString()
        {
            return $"{Offset}:({StackSizeBefore}) {Instruction} {Operand}";
        }
        public string ToInstructionString()
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
            switch (Instruction)
            {
                case WasmInstructions.i32_const:
                case WasmInstructions.i64_const:
                case WasmInstructions.f32_const:
                case WasmInstructions.f64_const:
                case WasmInstructions.i32_add:
                case WasmInstructions.i64_add:
                case WasmInstructions.f32_add:
                case WasmInstructions.f64_add:
                case WasmInstructions.i32_sub:
                case WasmInstructions.i64_sub:
                case WasmInstructions.f32_sub:
                case WasmInstructions.f64_sub:
                case WasmInstructions.i32_eq:
                case WasmInstructions.i64_eq:
                case WasmInstructions.f32_eq:
                case WasmInstructions.f64_eq:
                case WasmInstructions.i32_eqz:
                case WasmInstructions.i64_eqz:
                case WasmInstructions.f32_gt:
                case WasmInstructions.f64_gt:
                    return Instruction.ToString().Replace("_", ".");
                case WasmInstructions._return:
                    return "return";

                case WasmInstructions.i32_gt_s:
                    return "i32.gt_s";
                case WasmInstructions.i64_gt_s:
                    return "i64.gt_s";
                case WasmInstructions.i32_gt_u:
                    return "i32.gt_u";
                case WasmInstructions.i64_gt_u:
                    return "i64.gt_u";

                case WasmInstructions.i32_lt_s:
                    return "i32.lt_s";
                case WasmInstructions.i64_lt_s:
                    return "i64.lt_s";
                case WasmInstructions.i32_lt_u:
                    return "i32.lt_u";
                case WasmInstructions.i64_lt_u:
                    return "i64.lt_u";

                case WasmInstructions.i32_rem_s:
                    return "i32.rem_s";
                case WasmInstructions.i64_rem_s:
                    return "i64.rem_s";
                case WasmInstructions.i32_rem_u:
                    return "i32.rem_u";
                case WasmInstructions.i64_rem_u:
                    return "i64.rem_u";
                default:
                    return Instruction.ToString();
            }
        }


    }
}
