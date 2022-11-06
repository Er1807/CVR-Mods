using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace WrapperCodeGenerator
{
    [Generator]
    public class WrapperGenerator : ISourceGenerator
    {
        static Assembly LoadFromMelonLoader(object sender, ResolveEventArgs args)
        {
            string folderPath = @"C:\Program Files (x86)\Steam\steamapps\common\ChilloutVR\MelonLoader";
            string assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
            if (!File.Exists(assemblyPath)) return null;
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            return assembly;
        }
        static Assembly LoadFromManagedLoader(object sender, ResolveEventArgs args)
        {
            string folderPath = @"C:\Program Files (x86)\Steam\steamapps\common\ChilloutVR\ChilloutVR_Data\Managed";
            string assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
            if (!File.Exists(assemblyPath)) return null;
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            return assembly;
        }

        public void Execute(GeneratorExecutionContext context)
        {
            //return;
            StringBuilder sb = new StringBuilder();

            var testDomain = AppDomain.CurrentDomain;

            testDomain.AssemblyResolve += new ResolveEventHandler(LoadFromMelonLoader);
            testDomain.AssemblyResolve += new ResolveEventHandler(LoadFromManagedLoader);

            string folderPath = @"C:\Program Files (x86)\Steam\steamapps\common\ChilloutVR\ChilloutVR_Data\Managed";

            foreach (var item in Directory.GetFiles(folderPath))
            {
                testDomain.Load(Path.GetFileNameWithoutExtension(item));
            }

            var allowedFunctions = new List<string>();
            var disallowedFunctions = new List<string>();
            var types = new List<Type>();

            foreach (var item in testDomain.GetAssemblies())
            {
                try
                {
                    types.AddRange(item.DefinedTypes);
                }
                catch (Exception ex)
                {
                }
            }

            foreach (var item in File.ReadAllLines("C:\\Users\\Eric\\Documents\\GitHub\\CVR-Mods\\WasmLoader\\AllowedClasses.txt"))
            {
                try
                {
                    if (item.StartsWith("* "))
                    {
                        allowedFunctions.Add(item.Replace("* ", ""));
                    }
                    else if (item.StartsWith("- "))
                    {
                        disallowedFunctions.Add(item.Replace("- ", ""));
                    }
                    else
                    {
                        sb.AppendLine("// Generating " + item + " with " + allowedFunctions.Count);

                        Generate(context, types.Where(x => x.FullName == item).First(), allowedFunctions, disallowedFunctions);
                        allowedFunctions.Clear();
                    }
                }
                catch (Exception ex)
                {
                    sb.AppendLine("// Error " + item + " with " + allowedFunctions.Count + " " + ex.Message);
                }

            }
            context.AddSource("Results.g.cs", sb.ToString());
        }

        public void Generate(GeneratorExecutionContext context, Type type, List<string> allowedFunctions, List<string> disallowedFunctions)
        {
            if (type == null) return;

            string source = GenerateClass(type, allowedFunctions, disallowedFunctions).ToString();
            context.AddSource($"{(type.Namespace + type.Name).Replace(".", "_") + "_Ref"}.g.cs", source);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }



        public static StringBuilder GenerateClass(Type type, List<string> allowedFunctions, List<string> disallowedFunctions)
        {
            var sb = new StringBuilder();
            var className = (type.Namespace + type.Name).Replace(".", "_") + "_Ref";
            var methods = type.GetMethods();
            sb.AppendLine("using System;");
            sb.AppendLine("using Wasmtime;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using WasmLoader.Refs;");

            sb.AppendLine("namespace WasmLoader.Refs.Wrapper.Generated {");
            sb.AppendLine($"public class {className} : IRef {{");
            sb.AppendLine("public void Setup(Dictionary<string, Action<Linker, Store, Objectstore, WasmType>> functions) {");

            sb.AppendLine($@"functions[""{type.FullName.Replace(".", "_")}__Type""] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>");
            sb.AppendLine($@"linker.DefineFunction(""env"", ""{type.FullName.Replace(".", "_")}__Type"", (Caller caller) => {{");

            sb.AppendLine($"return objects.StoreObject(typeof({type.FullName.Replace("+", ".")}));");
            sb.AppendLine("});");
            sb.AppendLine();

            foreach (var member in methods)
            {
                if (member.DeclaringType != type)
                    continue;
                if (Check(member, allowedFunctions, disallowedFunctions))
                    continue;

                sb.AppendLine($@"functions[""{ConvertMethod(member)}""] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>");

                sb.AppendLine(Header(member));
                if (HasThis(member) && !IsSimple(member.DeclaringType))
                    sb.AppendLine(Retieve(member.DeclaringType));
                foreach (var item in member.GetParameters())
                {
                    sb.AppendLine(Retieve(item));
                }
                sb.AppendLine("#if Debug");
                sb.AppendLine($@"WasmLoaderMod.Instance.LoggerInstance.Msg(""----------------------"");");
                sb.AppendLine($@"WasmLoaderMod.Instance.LoggerInstance.Msg(""{ConvertMethod(member)}"");");
                if (HasThis(member))
                {
                    sb.AppendLine($"WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);");
                }
                foreach (var item in member.GetParameters())
                {
                    var name = item.Name;
                    if (!IsSimple(item.ParameterType))
                    {
                        name = "resolved_" + item.Name;
                    }
                    sb.AppendLine($"WasmLoaderMod.Instance.LoggerInstance.Msg({name});");
                }
                sb.AppendLine($@"WasmLoaderMod.Instance.LoggerInstance.Msg(""----------------------"");");

                sb.AppendLine("#endif");
                sb.AppendLine(Call(member));
                sb.AppendLine(ReturnValue(member));
                sb.AppendLine("});");
                sb.AppendLine();
            }

            foreach (var item in type.GetFields())
            {
                sb.AppendLine(GenerateField(item, allowedFunctions, disallowedFunctions));
            }

            sb.AppendLine("}");
            sb.AppendLine("}");
            sb.AppendLine("}");


            return sb;
        }


        public static bool Check(MethodInfo method, List<string> allowedFunctions, List<string> disallowedFunctions)
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
            if (method.Name.StartsWith("set_") && method.GetParameters().Length != 1)
                return true;
            if (method.Name.StartsWith("add_") || method.Name.StartsWith("remove_"))
                return true;
            if (method.GetParameters().Any(x => x.ParameterType.IsByRef))
                return true;

            var convertedName = ConvertMethod(method);


            if (disallowedFunctions.Contains(convertedName))
                return true;
            if (convertedName.Contains("&") || convertedName.Contains("`"))
                return true;

            if (allowedFunctions.Count != 0 && !allowedFunctions.Contains(convertedName))
            {
                return true;
            }

            return false;
        }

        public static bool Check(FieldInfo field, List<string> allowedFunctions, List<string> disallowedFunctions, bool read)
        {
            if (field.FieldType.IsGenericType)
                return true;
            if (field.IsInitOnly || field.IsLiteral)
                return true;

            var getName = ConvertMethod(field.DeclaringType, $"get_{field.Name}", HasThis(field), new List<Type>(), field.FieldType);
            var setName = ConvertMethod(field.DeclaringType, $"set_{field.Name}", HasThis(field), new List<Type>() { field.FieldType }, field.FieldType);



            if (disallowedFunctions.Contains(getName) && read)
                return true;
            if (disallowedFunctions.Contains(setName) && !read)
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
        public static string GenerateField(FieldInfo field, List<string> allowedFunctions, List<string> disallowedFunctions)
        {

            if (field.FieldType.IsGenericType)
                return "";

            var param = new List<string>();
            if (HasThis(field))
                param.Insert(0, $"{GetWasmType(field.DeclaringType)} parameter_this");

            string paramsting = "";
            if (param.Count > 0)
                paramsting = ", ";
            paramsting += string.Join(", ", param);

            StringBuilder builder = new StringBuilder();

            var getName = ConvertMethod(field.DeclaringType, $"get_{field.Name}", HasThis(field), new List<Type>(), field.FieldType);
            var setName = ConvertMethod(field.DeclaringType, $"set_{field.Name}", HasThis(field), new List<Type>() { field.FieldType }, field.FieldType);
            if (!Check(field, allowedFunctions, disallowedFunctions, true))
            {
                builder.AppendLine($@"functions[""{getName}""] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>");
                builder.AppendLine($"linker.DefineFunction(\"env\", \"{getName}\", (Caller caller {paramsting}) => {{");

                builder.AppendLine("#if Debug");
                builder.AppendLine($@"WasmLoaderMod.Instance.LoggerInstance.Msg(""----------------------"");");
                builder.AppendLine($@"WasmLoaderMod.Instance.LoggerInstance.Msg(""{getName}"");");
                builder.AppendLine($@"#endif");
                if (HasThis(field))
                {
                    builder.AppendLine(Retieve(field.DeclaringType));

                    builder.AppendLine("#if Debug");
                    builder.AppendLine($@"WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_this);");
                    builder.AppendLine($@"WasmLoaderMod.Instance.LoggerInstance.Msg(""----------------------"");");
                    builder.AppendLine($@"#endif");
                    builder.AppendLine($"return resolved_this.{field.Name};");
                }
                else
                {
                    builder.AppendLine("#if Debug");
                    builder.AppendLine($@"WasmLoaderMod.Instance.LoggerInstance.Msg(""----------------------"");");
                    builder.AppendLine($@"#endif");
                    builder.AppendLine($"return {field.DeclaringType.FullName}.{field.Name};");
                }
                builder.AppendLine("});");

            }
            builder.AppendLine("");

            param.Add($"{GetWasmType(field.FieldType)} value");

            paramsting = "";
            if (param.Count > 0)
                paramsting = ", ";
            paramsting += string.Join(", ", param);

            if (field.IsInitOnly || field.IsLiteral)
                return builder.ToString();
            if (!Check(field, allowedFunctions, disallowedFunctions, false))
            {
                builder.AppendLine($@"functions[""{setName}""] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>");
                builder.AppendLine($"linker.DefineFunction(\"env\", \"{setName}\", (Caller caller {paramsting}) => {{");

                builder.AppendLine("#if Debug");
                builder.AppendLine($@"WasmLoaderMod.Instance.LoggerInstance.Msg(""----------------------"");");
                builder.AppendLine($@"WasmLoaderMod.Instance.LoggerInstance.Msg(""{setName}"");");
                builder.AppendLine($@"#endif");
                if (HasThis(field))
                {
                    builder.AppendLine(Retieve(field.DeclaringType));
                    builder.AppendLine(Retieve(field.FieldType, "value"));

                    builder.AppendLine("#if Debug");
                    builder.AppendLine($@"WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_this);");
                    builder.AppendLine($@"WasmLoaderMod.Instance.LoggerInstance.Msg(""----------------------"");");
                    builder.AppendLine($@"#endif");
                    builder.AppendLine($"resolved_this.{field.Name} = {GetParameterForCall("value", field.FieldType)};");
                }
                else
                {
                    builder.AppendLine(Retieve(field.FieldType, "value"));
                    builder.AppendLine("#if Debug");
                    builder.AppendLine($@"WasmLoaderMod.Instance.LoggerInstance.Msg(""----------------------"");");
                    builder.AppendLine($@"#endif");
                    builder.AppendLine($"{field.DeclaringType.FullName}.{field.Name} = {GetParameterForCall("value", field.FieldType)};");
                }
                builder.AppendLine("});");
            }
            return builder.ToString();
        }

        public static string ConvertMethod(MethodInfo method)
        {
            return ConvertMethod(method.DeclaringType, method.Name, HasThis(method), method.GetParameters().Select(x => x.ParameterType).ToList(), method.ReturnType);
        }
        public static string ConvertMethod(Type type, string name, bool hasThis, List<Type> parameters, Type returnType)
        {
            return $"{type.FullName.Replace(".", "_")}__{name.Replace(".", "")}{GetParamStr(hasThis, parameters, returnType)}";
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
            return Retieve(param.ParameterType, param.Name);
        }

        public static string Retieve(Type type, string name = "this")
        {
            var parameterName = name;
            if (name == "this")
                parameterName = "parameter_this";
            if (IsSimple(type))
                return "";
            return $"var resolved_{name} = objects.RetriveObject<{type.FullName.Replace("+", ".")}>({parameterName}, caller);";
        }

        public static string Call(MethodInfo member)
        {
            var parameters = member.GetParameters().Select(x => GetParameterForCall(x)).ToList();
            var result = member.ReturnType == (typeof(void)) ? "" : "var result =";
            if (!HasSpecialName(member))
            {

                if (member.IsStatic)
                    return $"{result} {member.DeclaringType.FullName}.{member.Name}({string.Join(" ,", parameters)});";
                if (member.DeclaringType.IsValueType)
                    return $"{result} {(IsSimple(member.DeclaringType) ? "parameter_this" : "resolved_this")}.{member.Name}({string.Join(" ,", parameters)});";
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
                    if (member.DeclaringType.IsValueType)
                        return $"var result = {(IsSimple(member.DeclaringType) ? "parameter_this" : "resolved_this")}.{name};";
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
                    return $"{member.DeclaringType.FullName}.{name} = {parameters[0]};";
                return $"{(IsSimple(member.DeclaringType) ? "parameter_this" : "resolved_this")}.{name} = {parameters[0]};";

            }
            if (name.StartsWith("op_"))
            {
                if (name == "op_Equality")
                    return $"var result = {parameters[0]} == {parameters[1]};";
                if (name == "op_Inequality")
                    return $"var result = {parameters[0]} != {parameters[1]};";
                if (name == "op_LessThan")
                    return $"var result = {parameters[0]} < {parameters[1]};";
                if (name == "op_GreaterThan")
                    return $"var result = {parameters[0]} > {parameters[1]};";
                if (name == "op_LessThanOrEqual")
                    return $"var result = {parameters[0]} <= {parameters[1]};";
                if (name == "op_GreaterThanOrEqual")
                    return $"var result = {parameters[0]} >= {parameters[1]};";
                if (name.StartsWith("op_Addition"))
                    return $"var result = {parameters[0]} + {parameters[1]};";
                if (name.StartsWith("op_Subtraction"))
                    return $"var result = {parameters[0]} - {parameters[1]};";
                if (name.StartsWith("op_Multiply"))
                    return $"var result = {parameters[0]} * {parameters[1]};";
                if (name.StartsWith("op_Division"))
                    return $"var result = {parameters[0]} / {parameters[1]};";
                if (name.StartsWith("op_UnaryNegation"))
                    return $"var result = - {parameters[0]};";
                if (name.StartsWith("op_Explicit"))
                    return $"var result = ({member.ReturnType.FullName}) {parameters[0]};";
                if (name.StartsWith("op_Implicit"))
                    return $"var result = {parameters[0]};";
            }
            return "Error";
        }

        private static string GetParameterForCall(ParameterInfo x)
        {
            return GetParameterForCall(x.Name, x.ParameterType);
        }

        private static string GetParameterForCall(string name, Type type)
        {
            if (type == typeof(bool))
                return $"{name} > 0";
            return IsSimple(type) ? name : $"resolved_{name}";
        }

        public static string ReturnValue(MethodInfo method)
        {
            var type = method.ReturnType;
            if (type == null || type == typeof(void))
                return "";
            if (type == typeof(bool) && !method.IsStatic && !method.DeclaringType.IsValueType)
                return "return result ?? false ? 1 : 0;";
            if (type == typeof(bool))
                return "return result ? 1 : 0;";
            //if (type == typeof(bool))
            //    return "return result ? 1 : 0;";
            if (IsSimple(type) && (!HasThis(method) || type.IsValueType))
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
            if (type == typeof(int)
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
        public static bool HasThis(FieldInfo field)
        {
            return !field.IsStatic;
        }
        public static bool HasSpecialName(MethodInfo method)
        {
            return (method.Attributes & MethodAttributes.SpecialName) == MethodAttributes.SpecialName;
        }

    }
}
