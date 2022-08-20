using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Test;
using UnityEngine;

namespace LinkerCreator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            GenerateClass(typeof(ListGameobject));
        }

        private static void GenerateClass(Type type)
        {
            var className = (type.Namespace + type.Name).Replace(".", "_") + "_Ref";
            var methods = type.GetMethods();
            Console.WriteLine("using System;");
            Console.WriteLine("using Wasmtime;");
            Console.WriteLine("namespace WasmLoader.Refs {");
            Console.WriteLine($"public class {className} : IRef {{");
            Console.WriteLine("public void Setup(Linker linker, Store store, Objectstore objects) {");

            Console.WriteLine($@"linker.DefineFunction(""env"", ""{type.FullName.Replace(".", "_")}__Type"", (Caller caller) => {{");

            Console.WriteLine($"return objects.StoreObject(typeof({type.FullName}));");
            Console.WriteLine("});");
            Console.WriteLine();

            foreach (var member in methods)
            {
                if (Check(member))
                    continue;
                Console.WriteLine(Header(member));
                if (HasThis(member) && !IsSimple(member.DeclaringType))
                    Console.WriteLine(RetieveThis(member));
                foreach (var item in member.GetParameters())
                {
                    Console.WriteLine(Retieve(item));
                }
                Console.WriteLine("#if Debug");
                Console.WriteLine($@"WasmLoaderMod.Instance.LoggerInstance.Msg("""");");
                if (HasThis(member))
                {
                    Console.WriteLine($"WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);");
                }
                foreach (var item in member.GetParameters())
                {
                    var name = item.Name;
                    if (!IsSimple(item.ParameterType))
                    {
                        name = "resolved_" + item.Name;
                    }
                    Console.WriteLine($"WasmLoaderMod.Instance.LoggerInstance.Msg({name});");
                }
                Console.WriteLine($@"WasmLoaderMod.Instance.LoggerInstance.Msg(""{ConvertMethod(member)}"");");
                Console.WriteLine($@"WasmLoaderMod.Instance.LoggerInstance.Msg("""");");

                Console.WriteLine("#endif");
                Console.WriteLine(Call(member));
                Console.WriteLine(ReturnValue(member));
                Console.WriteLine("});");
                Console.WriteLine();
            }
            Console.WriteLine("}");
            Console.WriteLine("}");
            Console.WriteLine("}");

            Console.ReadLine();
        }

        public static bool Check(MethodInfo method)
        {
            if (method.IsGenericMethod)
                return true;
            if (method.GetParameters().Any(x => x.ParameterType.FullName == null))
                return true; 
            if (method.GetParameters().Any(x => x.ParameterType.FullName.Contains("`")))
                return true;
            if (method.GetParameters().Any(x => x.IsOut))
                return true;
            if (method.Name == "GetType")
                return true;
            return false;
        }

        public static string Header(MethodInfo method)
        {
            var param = method.GetParameters().Select(x => $"{GetWasmType(x.ParameterType)} {x.Name}").ToList();
            if (HasThis(method))
                param.Insert(0, $"{GetWasmType(method.DeclaringType)} parameter_this");
            string paramsting = "";
            if (param.Count > 0)
                paramsting = ", ";
            paramsting += string.Join(", ", param);
            
            return $"linker.DefineFunction(\"env\", \"{ConvertMethod(method)}\", (Caller caller {paramsting}) => {{";
        }
        public static string ConvertMethod(MethodInfo method)
        {
            return $"{method.DeclaringType.FullName.Replace(".", "_")}__{method.Name.Replace(".", "")}{GetParamStr(HasThis(method), method.GetParameters().Select(x =>x.ParameterType).ToList(), method.ReturnType)}";
        }
        public static string GetParamStr(bool hasThis, List<Type> parameters, Type returnType)
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
        public static string Retieve(ParameterInfo param)
        {
            if (IsSimple(param.ParameterType))
                return "";
            return $"var resolved_{param.Name} = objects.RetriveObject<{param.ParameterType.FullName}>({param.Name}, caller);";
        }

        private static string RetieveThis(MethodInfo member)
        {
            return $"var resolved_this = objects.RetriveObject<{member.DeclaringType.FullName}>(parameter_this, caller);";
        }
        public static string Call(MethodInfo member)
        {
            var parameters = member.GetParameters().Select(x => GetParameterForCall(x)).ToList();
            var result = member.ReturnType == (typeof(void)) ? "" : "var result =";
            if (!HasSpecialName(member))
            {

                if (member.IsStatic)
                    return $"{result} {member.DeclaringType.FullName}.{member.Name}({string.Join(" ,", parameters)});";
                return $"{result} {(IsSimple(member.DeclaringType) ? "parameter_this" : "resolved_this")}?.{member.Name}({string.Join(" ,", parameters)});";

            }

            var name = member.Name;
            if (name.StartsWith("get_"))
            {
                name = name.Replace("get_", "");
                if (parameters.Count == 0)
                {
                    if (member.IsStatic)
                        return $"var result = {member.DeclaringType.FullName}.{name};";
                    return $"var result = {(IsSimple(member.DeclaringType) ? "parameter_this" : "resolved_this")}?.{name};";
                }
                else
                {
                    return $"var result = {(IsSimple(member.DeclaringType) ? "parameter_this" : "resolved_this")}[{parameters[0]}];";
                }

            }

            if (name.StartsWith("set_"))
            {
                name = name.Replace("set_", "");
                if (member.IsStatic)
                    return $"{member.DeclaringType.FullName}.{name} = {parameters};";
                return $"{(IsSimple(member.DeclaringType) ? "parameter_this" : "resolved_this")}.{name} = {parameters[0]};";

            }
            if (name.StartsWith("op_"))
            {
                if (name == "op_Equality")
                    return $"var result = {parameters[0]} == {parameters[1]};";
                if (name == "op_Inequality")
                    return $"var result = {parameters[0]} != {parameters[1]};";
            }
            return "Error";
        }

        private static string GetParameterForCall(ParameterInfo x)
        {
            if (x.ParameterType == typeof(bool))
                return $"{x.Name} > 0";
            return IsSimple(x.ParameterType) ? x.Name : $"resolved_{x.Name}";
        }

        public static string ReturnValue(MethodInfo method)
        {
            var type = method.ReturnType;
            if (type == null || type == typeof(void))
                return "";
            if (type == typeof(bool) && !method.IsStatic)
                return "return result ?? false ? 1 : 0;";
            if (type == typeof(bool) && method.IsStatic)
                return "return result ? 1 : 0;";
            if (IsSimple(type) && !HasThis(method))
                return $"return result;";
            if (IsSimple(type) && HasThis(method) && type != typeof(bool))
                return $"return result ?? 0;";
            if (IsSimple(type) && HasThis(method) && type == typeof(bool))
                return $"return result ?? false;";
            return $"return objects.StoreObject(result);";
        }

        public static bool IsSimple(Type type)
        {
            if (type == typeof(int)
                || type == typeof(long)
                || type == typeof(float)
                || type == typeof(double)
                || type == typeof(bool))
                return true;
            return false;
        }
        public static Type GetWasmType(Type type)
        {
            if(type == typeof(int)
                || type == typeof(long)
                || type == typeof(float)
                || type == typeof(double))
                return type;
            if (type == typeof(short)
                || type == typeof(byte)
                || type == typeof(bool))
                return typeof(int);
            return typeof(int);
        }

        public static bool HasThis(MethodInfo method)
        {
            return (method.CallingConvention & CallingConventions.HasThis) == CallingConventions.HasThis;
        }

        public static bool HasSpecialName(MethodInfo method)
        {
            return (method.Attributes & MethodAttributes.SpecialName) == MethodAttributes.SpecialName;
        }
    }
}
