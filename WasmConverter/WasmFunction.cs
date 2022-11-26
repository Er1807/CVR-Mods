#if UNITY_EDITOR
using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
        public bool IsPublic;
        public WasmDataType? ReturnType;
        public List<WasmDataType> Parameters = new List<WasmDataType>();
        public Dictionary<string, WasmDataType> Locals = new Dictionary<string, WasmDataType>();
        public List<WasmInstruction> Instructions = new List<WasmInstruction>();
        public List<Block> Blocks = new List<Block>();
        public MethodDef Method;


        public Stack<WasmDataType> stack = new Stack<WasmDataType>();
        
        public string CreateWat()
        {
            var builder = new StringBuilder();
            builder.Append($"  (func ${Name}");

            for (int i = 0; i < Parameters.Count; i++)
            {
                if (Parameters[i] == WasmDataType.arri32 ||
                    Parameters[i] == WasmDataType.arri64 ||
                    Parameters[i] == WasmDataType.arrf32 ||
                    Parameters[i] == WasmDataType.arrf64 ||
                    Parameters[i] == WasmDataType.arrObj)
                    Parameters[i] = WasmDataType.i32;
            }

            for (int i = 0; i < Parameters.Count; i++)
            {
                builder.Append($" (param $param{i} {Parameters[i]})");
            }
            builder.AppendLine($" {BuildResultString(ReturnType)}");

            foreach (var local in Locals)
            {
                if (local.Key.StartsWith("param"))
                    continue;
                builder.AppendLine($"    (local ${local.Key} {(local.Value.ToString().Contains("arr") ? WasmDataType.i32: local.Value)})");
            }
            
            foreach (var block in Blocks)
            {
                builder.Append(block.ToInstructionString());

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
#endif