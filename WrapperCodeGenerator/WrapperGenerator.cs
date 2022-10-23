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
            StringBuilder sb = new StringBuilder();
            
            var testDomain = AppDomain.CurrentDomain;

            testDomain.AssemblyResolve += new ResolveEventHandler(LoadFromMelonLoader);
            testDomain.AssemblyResolve += new ResolveEventHandler(LoadFromManagedLoader);


            testDomain.Load("Assembly-CSharp");
            testDomain.Load("UnityEngine");
            testDomain.Load("UnityEngine.CoreModule");

            var allowedFunctions = new List<string>();
            var types = new List<Type>();

            foreach (var item in testDomain.GetAssemblies())
            {
                try
                {
                    types.AddRange(item.DefinedTypes);
                }
                catch (Exception)
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
                    else
                    {
                        sb.AppendLine("// Generating " + item +" with "+ allowedFunctions.Count);
                        
                        Generate(context, types.Where(x => x.FullName == item).FirstOrDefault(), allowedFunctions);
                        allowedFunctions.Clear();
                    }
                }
                catch (Exception)
                {
                    sb.AppendLine("// Errpr " + item + " with " + allowedFunctions.Count);
                }
                
            }
            context.AddSource("Results.g.cs", sb.ToString());
        }

        public void Generate(GeneratorExecutionContext context, Type type, List<string> allowedFunctions)
        {
            if (type == null) return;
            
            string source = GenerateClass(type, allowedFunctions).ToString();
            context.AddSource($"{(type.Namespace + type.Name).Replace(".", "_") + "_Ref"}.g.cs", source);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }



        public static StringBuilder GenerateClass(Type type, List<string> allowedFunctions)
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

            sb.AppendLine($"return objects.StoreObject(typeof({type.FullName}));");
            sb.AppendLine("});");
            sb.AppendLine();

            foreach (var member in methods)
            {
                if (Check(member, allowedFunctions))
                    continue;

                sb.AppendLine($@"functions[""{ConvertMethod(member)}""] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>");

                sb.AppendLine(Header(member));
                if (HasThis(member) && !IsSimple(member.DeclaringType))
                    sb.AppendLine(RetieveThis(member));
                foreach (var item in member.GetParameters())
                {
                    sb.AppendLine(Retieve(item));
                }
                sb.AppendLine("#if Debug");
                sb.AppendLine($@"WasmLoaderMod.Instance.LoggerInstance.Msg("""");");
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
                sb.AppendLine($@"WasmLoaderMod.Instance.LoggerInstance.Msg(""{ConvertMethod(member)}"");");
                sb.AppendLine($@"WasmLoaderMod.Instance.LoggerInstance.Msg("""");");

                sb.AppendLine("#endif");
                sb.AppendLine(Call(member));
                sb.AppendLine(ReturnValue(member));
                sb.AppendLine("});");
                sb.AppendLine();
            }
            sb.AppendLine("}");
            sb.AppendLine("}");
            sb.AppendLine("}");


            return sb;
        }

        public static bool Check(MethodInfo method, List<string> allowedFunctions)
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

            if (allowedFunctions.Count != 0 && !allowedFunctions.Contains(ConvertMethod(method)))
            {
                return true;
            }

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
            return $"{method.DeclaringType.FullName.Replace(".", "_")}__{method.Name.Replace(".", "")}{GetParamStr(HasThis(method), method.GetParameters().Select(x => x.ParameterType).ToList(), method.ReturnType)}";
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
                if (name == "op_LessThan")
                    return $"var result = {parameters[0]} < {parameters[1]};";
                if (name == "op_GreaterThan")
                    return $"var result = {parameters[0]} > {parameters[1]};";
                if (name == "op_LessThanOrEqual")
                    return $"var result = {parameters[0]} <= {parameters[1]};";
                if (name == "op_GreaterThanOrEqual")
                    return $"var result = {parameters[0]} >= {parameters[1]};";
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

        public static bool HasSpecialName(MethodInfo method)
        {
            return (method.Attributes & MethodAttributes.SpecialName) == MethodAttributes.SpecialName;
        }

    }
}
