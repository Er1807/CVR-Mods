using dnlib.DotNet;
using System.Collections.Generic;
using System.Text;

namespace Converter
{
    public class WasmInstruction
    {
        public WasmInstruction(WasmInstructions instruction, uint offset, object operand = null)
        {
            Instruction = instruction;
            Offset = offset;
            Operand = operand;
        }

        public WasmInstructions Instruction;
        public uint Offset; //il offset
        public object Operand;

        public override string ToString()
        {
            return $"{Offset}: {Instruction} {Operand}";
        }
        public string ToInstructionString()
        {
            if (Operand is MemberRef member) {
                if (member.Name == ".ctor")
                    return $"    {OpCodeToText()} ${ConvertMethod(member.Class.FullName, member.Name, false, member.GetParams(), member.ReturnType ?? member.FieldSig?.Type)}";
                return $"    {OpCodeToText()} ${ConvertMethod(member.Class.FullName, member.Name, member.HasThis, member.GetParams(), member.ReturnType ?? member.FieldSig?.Type)}";
            }
            if (Operand is MethodDef method)
                return $"    {OpCodeToText()} ${ConvertMethod(method.DeclaringType.FullName, method.Name, method.HasThis, method.GetParams(), method.ReturnType)}";

            if (Operand is string str)
                return $"    {OpCodeToText()} ${str}";
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
                    return Instruction.ToString().Replace("_",".");
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
                default:
                    return Instruction.ToString();
            }
        }

        public static string ConvertMethod(string className, string methodName, bool hasThis, IList<TypeSig> parameters, TypeSig returnType)
        {
            return $"{className.Replace(".", "_")}__{methodName.Replace(".", "")}{GetParamStr(hasThis, parameters, returnType)}";
        }

        public static string GetParamStr(bool hasThis, IList<TypeSig> parameters, TypeSig returnType)
        {
            StringBuilder builder = new StringBuilder();

            if (hasThis)
                builder.Append("_this");

            foreach (var item in parameters)
            {
                if (item.FullName.StartsWith("System.Nullable`1<"))
                    builder.Append("_" + item.FullName.Replace("System.Nullable`1<", "").Replace(">", "").Replace(".", ""));
                else
                    builder.Append($"_{item.FullName.Replace(".", "")}");

            }

            if (returnType != null)
            {
                if (returnType.FullName.StartsWith("System.Nullable`1<"))
                    builder.Append("__" + returnType.FullName.Replace("System.Nullable`1<", "").Replace(">", "").Replace(".", ""));
                else
                    builder.Append($"__{returnType.FullName.Replace(".", "")}");
            }

            return builder.ToString();
        }
    }
}
