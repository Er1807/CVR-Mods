using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter
{
    public abstract class WasmOperand
    {
        public static WasmStringOperand FromString(string value) { return new WasmStringOperand() { StrValue = value }; }
        public static WasmIntOperand FromInt(int value) { return new WasmIntOperand() { Value = value }; }
        public static WasmLongOperand FromLong(long value) { return new WasmLongOperand() { Value = value }; }
        public static WasmFloatOperand FromFloat(float value) { return new WasmFloatOperand() { Value = value }; }
        public static WasmDoubleOperand FromDouble(double value) { return new WasmDoubleOperand() { Value = value }; }
        public static WasmExternFunctionOperand FromExtern(MethodDef value) {

            if(value.Name == ".ctor")
            {
                return new WasmExternFunctionOperand()
                {
                    HasThis = value.HasThis,
                    FunctionName = ConvertMethod(value.DeclaringType.FullName, value.Name, value.HasThis, value.Parameters.Skip(1).Select(x => x.Type).ToList(), value.DeclaringType.ToTypeSig()),
                    Params = value.GetParams().Skip(1).ToList() ?? new List<TypeSig>(),
                    ReturnValue = value.DeclaringType.ToTypeSig(),
                    DeclaringType = value.DeclaringType.ToTypeSig()
                };
            }

            return new WasmExternFunctionOperand() {
                HasThis = value.HasThis,
                FunctionName = ConvertMethod(value.DeclaringType.FullName, value.Name, value.HasThis, value.Parameters.Select(x => x.Type).ToList(), value.MethodSig.GetRetType()),
                Params = value.GetParams() ?? new List<TypeSig>(),
                ReturnValue = value.MethodSig.GetRetType(),
                DeclaringType = value.DeclaringType.ToTypeSig()
            }; 
        }
        public static WasmExternFunctionOperand FromExtern(MemberRef value)
        {
            if (value.Name == ".ctor")
            {
                return new WasmExternFunctionOperand()
                {
                    HasThis = value.HasThis,
                    FunctionName = ConvertMethod(value.DeclaringType.FullName, value.Name, value.HasThis, value.GetParams().ToList(), value.DeclaringType.ToTypeSig()),
                    Params = value.GetParams().Skip(1).ToList() ?? new List<TypeSig>(),
                    ReturnValue = value.DeclaringType.ToTypeSig(),
                    DeclaringType = value.DeclaringType.ToTypeSig()
                };
            }
                return new WasmExternFunctionOperand()
            {
                HasThis = value.HasThis,
                FunctionName = ConvertMethod(value.DeclaringType.FullName, value.Name, value.HasThis, value.GetParams().ToList(), value.MethodSig.GetRetType()),
                Params = value.GetParams() ?? new List<TypeSig>(),
                ReturnValue = value.MethodSig.GetRetType(),
                DeclaringType = value.DeclaringType.ToTypeSig()
            };
        }

        public static WasmLocalFunctionOperand FromLocalFunction(string value)
        {
            return new WasmLocalFunctionOperand() { Value = value };
        }
        public static WasmLocalFieldOperand FromLocalField(string value)
        {
            return new WasmLocalFieldOperand() { Value = value };
        }
        public static WasmGlobalFieldOperand FromGlobalField(string value)
        {
            return new WasmGlobalFieldOperand() { Value = value };
        }

        public static WasmParamFieldOperand FromParamField(string value)
        {
            return new WasmParamFieldOperand() { Value = value };
        }
        public static WasmStaticStringOperand FromStaticStringmField(string value)
        {
            return new WasmStaticStringOperand() { Value = value };
        }

        private static string ConvertMethod(string className, string methodName, bool hasThis, IList<TypeSig> parameters, TypeSig returnType)
        {
            return $"{className.Replace(".", "_")}__{methodName.Replace(".", "")}{GetParamStr(hasThis, parameters, returnType)}";
        }

        private static string GetParamStr(bool hasThis, IList<TypeSig> parameters, TypeSig returnType)
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

    public class WasmStringOperand : WasmOperand
    {
        public string StrValue;
        public int Value;
        public override string ToString() => Value.ToString();
    }

    public class WasmIntOperand : WasmOperand
    {
        public int Value;
        public override string ToString() => Value.ToString();
    }
    public class WasmLongOperand : WasmOperand
    {
        public long Value;
        public uint AsUInt => (uint)Value;
        public override string ToString() => Value.ToString();
    }
    public class WasmFloatOperand : WasmOperand
    {
        public float Value;
        public override string ToString() => Value.ToString();
    }
    public class WasmDoubleOperand : WasmOperand
    {
        public double Value;
        public override string ToString() => Value.ToString();
    }
    public class WasmExternFunctionOperand : WasmOperand
    {
        public TypeSig DeclaringType;
        public bool HasThis;
        public IList<TypeSig> Params;
        public TypeSig ReturnValue;
        public string FunctionName;
        public override string ToString() => FunctionName;
    }
    public class WasmLocalFunctionOperand : WasmOperand
    {
        public string Value;
        public override string ToString() => Value;
    }
    public class WasmLocalFieldOperand : WasmOperand
    {
        public string Value;
        public override string ToString() => Value;
    }
    public class WasmGlobalFieldOperand : WasmOperand
    {
        public string Value;
        public override string ToString() => Value;
    }
    public class WasmParamFieldOperand : WasmOperand
    {
        public string Value;
        public override string ToString() => Value;
    }
    public class WasmStaticStringOperand : WasmOperand
    {
        public string Value;

        public override string ToString() => Value;
    }

}
