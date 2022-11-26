#if UNITY_EDITOR
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter
{
    public class Converter
    {
        public WasmFunction Convert(WasmModule module, MethodDef method)
        {
            WasmFunction func = new WasmFunction(module);
            module.Functions.Add(func);
            func.Name = method.Name;
            func.ReturnType = GetWasmType(method.ReturnType);
            func.Method = method;



            foreach (var parameter in method.Parameters.Where(x => !x.IsHiddenThisParameter))
            {
                func.Parameters.Add(GetWasmType(parameter.Type).Value);
            }

            for (int i = 0; i < func.Parameters.Count; i++)
            {
                func.Locals.Add($"param{i}", func.Parameters[i]);
            }

            for (int i = 0; i < method.Body.Variables.Count; i++)
            {
                func.Locals.Add($"local{method.Body.Variables[i].Index}", GetWasmType(method.Body.Variables[i].Type).Value);
            }
            var instructions = method.Body.Instructions;
            for (int i = 0; i < instructions.Count; i++)
            {
                if (instructions[i].OpCode == OpCodes.Ldloca_S
                     && instructions[i + 1].OpCode == OpCodes.Initobj && instructions[i + 1].Operand.ToString().StartsWith("System.Nullable")
                     && instructions[i + 2].OpCode == OpCodes.Ldloc_0)
                {
                    instructions.RemoveAt(i + 2);
                    instructions.RemoveAt(i);
                }
            }

            foreach (var instruction in instructions)
            {
                TranslateInstruction(instruction, func);
            }

#if DEBUG
            Console.WriteLine(func.Name);
            foreach (var item in func.Parameters)
            {
                Console.WriteLine("Param " + item);
            }
            Console.WriteLine("Retr " + func.ReturnType);
            foreach (var item in func.Instructions)
            {
                Console.WriteLine("Inst:" + item);
            }
            Console.WriteLine();
#endif
            new BlockRebuilder(func).Rebuild();
#if DEBUG
            Console.WriteLine("After BlockRebuild fix");
            Console.WriteLine(func.Name);
            foreach (var item in func.Parameters)
            {
                Console.WriteLine("Param " + item);
            }
            Console.WriteLine("Retr " + func.ReturnType);
            foreach (var item in func.Blocks)
            {
                Console.WriteLine("Block:" + item);
            }
            Console.WriteLine();
#endif
            return func;
        }

        public void TranslateInstruction(Instruction instruction, WasmFunction func)
        {
            switch (instruction.OpCode.Code)
            {
                case Code.Ldnull:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_const, instruction.Offset, func.stack.Count, WasmOperand.FromInt(0)));
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Ldc_R4:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.f32_const, instruction.Offset, func.stack.Count, WasmOperand.FromFloat((float)instruction.Operand)));
                    func.stack.Push(WasmDataType.f32);
                    break;
                case Code.Ldc_R8:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.f64_const, instruction.Offset, func.stack.Count, WasmOperand.FromDouble((double)instruction.Operand)));
                    func.stack.Push(WasmDataType.f64);
                    break;
                case Code.Ldc_I8:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.i64_const, instruction.Offset, func.stack.Count, WasmOperand.FromLong((long)instruction.Operand)));
                    func.stack.Push(WasmDataType.i64);
                    break;
                case Code.Ldc_I4_S:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_const, instruction.Offset, func.stack.Count, WasmOperand.FromInt((sbyte)instruction.Operand)));
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Ldc_I4:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_const, instruction.Offset, func.stack.Count, WasmOperand.FromInt((int)instruction.Operand)));
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Ldstr:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_const, instruction.Offset, func.stack.Count, WasmOperand.FromString((string)instruction.Operand)));
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Ldc_I4_M1:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_const, instruction.Offset, func.stack.Count, WasmOperand.FromInt(-1)));
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Ldc_I4_0:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_const, instruction.Offset, func.stack.Count, WasmOperand.FromInt(0)));
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Ldc_I4_1:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_const, instruction.Offset, func.stack.Count, WasmOperand.FromInt(1)));
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Ldc_I4_2:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_const, instruction.Offset, func.stack.Count, WasmOperand.FromInt(2)));
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Ldc_I4_3:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_const, instruction.Offset, func.stack.Count, WasmOperand.FromInt(3)));
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Ldc_I4_4:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_const, instruction.Offset, func.stack.Count, WasmOperand.FromInt(4)));
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Ldc_I4_5:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_const, instruction.Offset, func.stack.Count, WasmOperand.FromInt(5)));
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Ldc_I4_6:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_const, instruction.Offset, func.stack.Count, WasmOperand.FromInt(6)));
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Ldc_I4_7:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_const, instruction.Offset, func.stack.Count, WasmOperand.FromInt(7)));
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Ldc_I4_8:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_const, instruction.Offset, func.stack.Count, WasmOperand.FromInt(8)));
                    func.stack.Push(WasmDataType.i32);
                    break;

                case Code.Ldarg_0:
                    //needed in case its an if jump target. but we dont load a variable
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.nop, instruction.Offset, func.stack.Count));
                    break;
                case Code.Ldarg_1:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.get_local, instruction.Offset, func.stack.Count, WasmOperand.FromParamField("param0")));
                    func.stack.Push(func.Parameters[0]);
                    break;
                case Code.Ldarg_2:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.get_local, instruction.Offset, func.stack.Count, WasmOperand.FromParamField("param1")));
                    func.stack.Push(func.Parameters[1]);
                    break;
                case Code.Ldarg_3:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.get_local, instruction.Offset, func.stack.Count, WasmOperand.FromParamField("param2")));
                    func.stack.Push(func.Parameters[2]);
                    break;
                case Code.Ldarg:
                case Code.Ldarg_S:
                    var Ldarg = instruction.Operand is Parameter param ? param.Index : (int)instruction.Operand;
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.get_local, instruction.Offset, func.stack.Count, WasmOperand.FromParamField($"param{Ldarg - 1}")));
                    func.stack.Push(func.Parameters[Ldarg - 1]);
                    break;
                case Code.Ldarga_S:
                    var Ldarga_S = -1;
                    if (instruction.Operand is Parameter)
                        Ldarga_S = ((Parameter)instruction.Operand).Index;
                    else
                        Ldarga_S = (int)instruction.Operand;
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.get_local, instruction.Offset, func.stack.Count, WasmOperand.FromParamField($"param{Ldarga_S - 1}")));
                    func.stack.Push(func.Parameters[Ldarga_S - 1]);
                    break;
                case Code.Stloc_0:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.set_local, instruction.Offset, func.stack.Count, WasmOperand.FromLocalField("local0")));
                    func.stack.Pop();
                    break;
                case Code.Stloc_1:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.set_local, instruction.Offset, func.stack.Count, WasmOperand.FromLocalField("local1")));
                    func.stack.Pop();
                    break;
                case Code.Stloc_2:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.set_local, instruction.Offset, func.stack.Count, WasmOperand.FromLocalField("local2")));
                    func.stack.Pop();
                    break;
                case Code.Stloc_3:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.set_local, instruction.Offset, func.stack.Count, WasmOperand.FromLocalField("local3")));
                    func.stack.Pop();
                    break;
                case Code.Stloc:
                case Code.Stloc_S:
                    var Stloc = -1;
                    if (instruction.Operand is Local)
                        Stloc = ((Local)instruction.Operand).Index;
                    else
                        Stloc = (int)instruction.Operand;
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.set_local, instruction.Offset, func.stack.Count, WasmOperand.FromLocalField($"local{Stloc}")));
                    func.stack.Pop();
                    break;
                case Code.Starg:
                case Code.Starg_S:
                    var Starg = ((Parameter)instruction.Operand).Index;
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.set_local, instruction.Offset, func.stack.Count, WasmOperand.FromLocalField($"param{Starg}")));
                    func.stack.Pop();
                    break;
                case Code.Ldloc_0:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.get_local, instruction.Offset, func.stack.Count, WasmOperand.FromLocalField("local0")));
                    func.stack.Push(func.Locals["local0"]);
                    break;
                case Code.Ldloc_1:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.get_local, instruction.Offset, func.stack.Count, WasmOperand.FromLocalField("local1")));
                    func.stack.Push(func.Locals["local1"]);
                    break;
                case Code.Ldloc_2:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.get_local, instruction.Offset, func.stack.Count, WasmOperand.FromLocalField("local2")));
                    func.stack.Push(func.Locals["local2"]);
                    break;
                case Code.Ldloc_3:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.get_local, instruction.Offset, func.stack.Count, WasmOperand.FromLocalField("local3")));
                    func.stack.Push(func.Locals["local3"]);
                    break;
                case Code.Ldloc:
                case Code.Ldloc_S:
                    var Ldloc = -1;
                    if (instruction.Operand is Local)
                        Ldloc = ((Local)instruction.Operand).Index;
                    else
                        Ldloc = (int)instruction.Operand;
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.get_local, instruction.Offset, func.stack.Count, WasmOperand.FromLocalField($"local{Ldloc}")));
                    func.stack.Push(func.Locals[$"local{Ldloc}"]);
                    break;
                case Code.Ldloca_S:
                    var Ldlocas = (Local)instruction.Operand;
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.get_local, instruction.Offset, func.stack.Count, WasmOperand.FromLocalField($"local{Ldlocas.Index}")));
                    func.stack.Push(func.Locals[$"local{Ldlocas.Index}"]);
                    break;
                case Code.Dup:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.tee_local, instruction.Offset, func.stack.Count, WasmOperand.FromLocalField($"temp{func.stack.Peek()}")));
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.get_local, instruction.Offset, func.stack.Count, WasmOperand.FromLocalField($"temp{func.stack.Peek()}")));
                    if (!func.Locals.ContainsKey($"temp{func.stack.Peek()}"))
                        func.Locals.Add($"temp{func.stack.Peek()}", func.stack.Peek());
                    func.stack.Push(func.stack.Peek());

                    break;

                case Code.Call:
                case Code.Callvirt:
                case Code.Ldsfld:
                case Code.Ldfld:
                    var method = instruction.Operand as MethodDef;
                    var member = instruction.Operand as MemberRef;
                    var field = instruction.Operand as FieldDef;

                    if (member != null && member.Name == "GetTypeFromHandle")
                        return;


                    if (field != null && field.DeclaringType == func.Method.DeclaringType)
                    {
                        func.Instructions.Add(new WasmInstruction(WasmInstructions.get_global, instruction.Offset, func.stack.Count, WasmOperand.FromGlobalField(field.Name.ToString())));
                        func.stack.Push(GetWasmType(func.Module.Fields[field.Name]).Value);
                        break;
                    }

                    if (field != null && field.DeclaringType != func.Method.DeclaringType)
                    {
                        func.Instructions.Add(new WasmInstruction(WasmInstructions.call, instruction.Offset, func.stack.Count, WasmOperand.FromExtern(field.DeclaringType.FullName, $"get_{field.Name}", true, new List<TypeSig>(), field.FieldType)));

                        func.stack.Pop();
                        func.stack.Push(GetWasmType(field.FieldType).Value);
                        break;
                    }

                    if (method != null && method.DeclaringType.FullName == func.Module.declaringType.FullName)
                    {
                        func.Instructions.Add(new WasmInstruction(WasmInstructions.call, instruction.Offset, func.stack.Count, WasmOperand.FromLocalFunction(method.Name)));
                        for (int i = 0; i < method.Parameters.Count - 1; i++)
                        {
                            func.stack.Pop();
                        }
                        if (method.ReturnType != null && method.ReturnType.FullName != "System.Void")
                            func.stack.Push(GetWasmType(method.ReturnType).Value);

                        return;
                    }

                    WasmExternFunctionOperand externFunction = null;
                    if (member != null)
                        externFunction = WasmOperand.FromExtern(member);
                    else if (method != null)
                        externFunction = WasmOperand.FromExtern(method);
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.call, instruction.Offset, func.stack.Count, externFunction));

                    if ((instruction.Operand as IFullName).Name == "CurrentGameObject")
                    {
                        externFunction.Params.Clear();
                        func.stack.Push(WasmDataType.i32);
                        break;
                    }

                    foreach (var item in externFunction.Params)
                    {
                        func.stack.Pop();
                    }
                    if (externFunction.ReturnValue != null && externFunction.ReturnValue.FullName != "System.Void")
                        func.stack.Push(GetWasmType(externFunction.ReturnValue).Value);


                    break;
                case Code.Ldflda:
                    if (instruction.Operand is FieldDef fieldDefa && fieldDefa.DeclaringType == func.Method.DeclaringType)
                    {
                        func.Instructions.Add(new WasmInstruction(WasmInstructions.get_global, instruction.Offset, func.stack.Count, WasmOperand.FromGlobalField(fieldDefa.Name.ToString())));
                        func.stack.Push(GetWasmType(func.Module.Fields[fieldDefa.Name]).Value);
                        break;
                    }
                    Console.Error.WriteLine("Missing ldflda");
                    break;

                case Code.Ldtoken:
                    var ldtoken = instruction.Operand as IFullName;
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.call, instruction.Offset, func.stack.Count, new WasmExternFunctionOperand() { FunctionName = ldtoken.FullName.Replace(".", "_") + "__Type", ReturnValue = Program.TypeType.ToTypeSig(), Params = new List<TypeSig>() }));
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Ldftn:
                    var Ldftn = instruction.Operand as MethodDef;
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_const, instruction.Offset, func.stack.Count, WasmOperand.FromString(Ldftn.Name)));
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Stfld:
                    if (instruction.Operand is FieldDef fieldDef2 && fieldDef2.DeclaringType == func.Method.DeclaringType)
                    {
                        func.Instructions.Add(new WasmInstruction(WasmInstructions.set_global, instruction.Offset, func.stack.Count, WasmOperand.FromGlobalField(fieldDef2.Name.ToString())));
                        func.stack.Pop();
                        break;
                    }

                    if (instruction.Operand is FieldDef fieldDef3 && fieldDef3.DeclaringType != func.Method.DeclaringType)
                    {
                        func.Instructions.Add(new WasmInstruction(WasmInstructions.call, instruction.Offset, func.stack.Count, WasmOperand.FromExtern(fieldDef3.DeclaringType.FullName, $"set_{fieldDef3.Name}", true, new List<TypeSig>(), fieldDef3.FieldType)));

                        func.stack.Pop();
                        func.stack.Pop();
                        break;
                    }

                    if (instruction.Operand is MemberRef memberef && memberef.DeclaringType != func.Method.DeclaringType)
                    {
                        func.Instructions.Add(new WasmInstruction(WasmInstructions.call, instruction.Offset, func.stack.Count, WasmOperand.FromExtern(memberef.DeclaringType.FullName, $"set_{memberef.Name}", true, new List<TypeSig>(), memberef.FieldSig.Type)));

                        func.stack.Pop();
                        func.stack.Pop();
                        break;
                    }

                    Console.Error.WriteLine("Unkown opcode op " + instruction.OpCode.Code);
                    break;
                case Code.Newobj:
                    var method2 = instruction.Operand as MethodDef;
                    var member2 = instruction.Operand as MemberRef;
                    WasmExternFunctionOperand externFunction2 = null;
                    if (member2 != null)
                        externFunction2 = WasmOperand.FromExtern(member2);
                    else if (method2 != null)
                        externFunction2 = WasmOperand.FromExtern(method2);
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.call, instruction.Offset, func.stack.Count, externFunction2));

                    foreach (var item in externFunction2.Params)
                    {
                        func.stack.Pop();
                    }
                    if (externFunction2.ReturnValue != null && externFunction2.ReturnValue.FullName != "System.Void")
                        func.stack.Push(GetWasmType(externFunction2.ReturnValue).Value);

                    break;
                case Code.Initobj:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_const, instruction.Offset, func.stack.Count, WasmOperand.FromInt(0)));
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Ret:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions._return, instruction.Offset, func.stack.Count));
                    break;
                case Code.Isinst:
                case Code.Nop:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.nop, instruction.Offset, func.stack.Count));
                    break;
                case Code.Add:
                    switch (func.stack.Peek())
                    {
                        case WasmDataType.i32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_add, instruction.Offset, func.stack.Count));
                            func.stack.Pop();
                            func.stack.Pop();
                            func.stack.Push(WasmDataType.i32);
                            break;
                        case WasmDataType.i64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i64_add, instruction.Offset, func.stack.Count));
                            func.stack.Pop();
                            func.stack.Pop();
                            func.stack.Push(WasmDataType.i64);
                            break;
                        case WasmDataType.f32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f32_add, instruction.Offset, func.stack.Count));
                            func.stack.Pop();
                            func.stack.Pop();
                            func.stack.Push(WasmDataType.f32);
                            break;
                        case WasmDataType.f64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f64_add, instruction.Offset, func.stack.Count));
                            func.stack.Pop();
                            func.stack.Pop();
                            func.stack.Push(WasmDataType.f64);
                            break;
                    }
                    break;
                case Code.Div:
                    switch (func.stack.Peek())
                    {
                        case WasmDataType.i32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_div_s, instruction.Offset, func.stack.Count));
                            func.stack.Pop();
                            func.stack.Pop();
                            func.stack.Push(WasmDataType.i32);
                            break;
                        case WasmDataType.i64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i64_div_s, instruction.Offset, func.stack.Count));
                            func.stack.Pop();
                            func.stack.Pop();
                            func.stack.Push(WasmDataType.i64);
                            break;
                        case WasmDataType.f32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f32_div, instruction.Offset, func.stack.Count));
                            func.stack.Pop();
                            func.stack.Pop();
                            func.stack.Push(WasmDataType.f32);
                            break;
                        case WasmDataType.f64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f64_div, instruction.Offset, func.stack.Count));
                            func.stack.Pop();
                            func.stack.Pop();
                            func.stack.Push(WasmDataType.f64);
                            break;
                    }
                    break;
                case Code.Mul:
                    switch (func.stack.Peek())
                    {
                        case WasmDataType.i32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_mul, instruction.Offset, func.stack.Count));
                            func.stack.Pop();
                            func.stack.Pop();
                            func.stack.Push(WasmDataType.i32);
                            break;
                        case WasmDataType.i64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i64_mul, instruction.Offset, func.stack.Count));
                            func.stack.Pop();
                            func.stack.Pop();
                            func.stack.Push(WasmDataType.i64);
                            break;
                        case WasmDataType.f32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f32_mul, instruction.Offset, func.stack.Count));
                            func.stack.Pop();
                            func.stack.Pop();
                            func.stack.Push(WasmDataType.f32);
                            break;
                        case WasmDataType.f64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f64_mul, instruction.Offset, func.stack.Count));
                            func.stack.Pop();
                            func.stack.Pop();
                            func.stack.Push(WasmDataType.f64);
                            break;
                    }
                    break;
                case Code.Sub:
                    switch (func.stack.Peek())
                    {
                        case WasmDataType.i32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_sub, instruction.Offset, func.stack.Count));
                            func.stack.Pop();
                            func.stack.Pop();
                            func.stack.Push(WasmDataType.i32);
                            break;
                        case WasmDataType.i64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i64_sub, instruction.Offset, func.stack.Count));
                            func.stack.Pop();
                            func.stack.Pop();
                            func.stack.Push(WasmDataType.i64);
                            break;
                        case WasmDataType.f32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f32_sub, instruction.Offset, func.stack.Count));
                            func.stack.Pop();
                            func.stack.Pop();
                            func.stack.Push(WasmDataType.f32);
                            break;
                        case WasmDataType.f64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f64_sub, instruction.Offset, func.stack.Count));
                            func.stack.Pop();
                            func.stack.Pop();
                            func.stack.Push(WasmDataType.f64);
                            break;
                    }
                    break;
                case Code.Rem:
                    switch (func.stack.Peek())
                    {
                        case WasmDataType.i32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_rem_s, instruction.Offset, func.stack.Count));
                            func.stack.Pop();
                            func.stack.Pop();
                            func.stack.Push(WasmDataType.i32);
                            break;
                        case WasmDataType.i64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i64_rem_s, instruction.Offset, func.stack.Count));
                            func.stack.Pop();
                            func.stack.Pop();
                            func.stack.Push(WasmDataType.i64);
                            break;
                    }
                    break;
                case Code.Rem_Un:
                    switch (func.stack.Peek())
                    {
                        case WasmDataType.i32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_rem_u, instruction.Offset, func.stack.Count));
                            func.stack.Pop();
                            func.stack.Pop();
                            func.stack.Push(WasmDataType.i32);
                            break;
                        case WasmDataType.i64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i64_rem_u, instruction.Offset, func.stack.Count));
                            func.stack.Pop();
                            func.stack.Pop();
                            func.stack.Push(WasmDataType.i64);
                            break;
                    }
                    break;
                case Code.Ceq:
                    switch (func.stack.Peek())
                    {
                        case WasmDataType.i32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_eq, instruction.Offset, func.stack.Count));
                            break;
                        case WasmDataType.i64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i64_eq, instruction.Offset, func.stack.Count));
                            break;
                        case WasmDataType.f32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f32_eq, instruction.Offset, func.stack.Count));
                            break;
                        case WasmDataType.f64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f64_eq, instruction.Offset, func.stack.Count));
                            break;
                    }
                    func.stack.Pop();
                    func.stack.Pop();
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Bgt:
                case Code.Bgt_S:
                    switch (func.stack.Peek())
                    {
                        case WasmDataType.i32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_gt_s, instruction.Offset, func.stack.Count));
                            break;
                        case WasmDataType.i64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i64_gt_s, instruction.Offset, func.stack.Count));
                            break;
                        case WasmDataType.f32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f32_gt, instruction.Offset, func.stack.Count));
                            break;
                        case WasmDataType.f64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f64_gt, instruction.Offset, func.stack.Count));
                            break;
                    }
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.br_if, instruction.Offset, func.stack.Count - 1, WasmOperand.FromLong(((Instruction)instruction.Operand).Offset)));
                    func.stack.Pop();
                    func.stack.Pop();
                    break;
                case Code.Cgt:
                    switch (func.stack.Peek())
                    {
                        case WasmDataType.i32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_gt_s, instruction.Offset, func.stack.Count));
                            break;
                        case WasmDataType.i64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i64_gt_s, instruction.Offset, func.stack.Count));
                            break;
                        case WasmDataType.f32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f32_gt, instruction.Offset, func.stack.Count));
                            break;
                        case WasmDataType.f64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f64_gt, instruction.Offset, func.stack.Count));
                            break;
                    }
                    func.stack.Pop();
                    func.stack.Pop();
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Cgt_Un:
                    switch (func.stack.Peek())
                    {
                        case WasmDataType.i32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_gt_u, instruction.Offset, func.stack.Count));
                            break;
                        case WasmDataType.i64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i64_gt_u, instruction.Offset, func.stack.Count));
                            break;
                    }
                    func.stack.Pop();
                    func.stack.Pop();
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Ble:
                case Code.Ble_S:
                case Code.Clt:
                    switch (func.stack.Peek())
                    {
                        case WasmDataType.i32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_lt_s, instruction.Offset, func.stack.Count));
                            break;
                        case WasmDataType.i64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i64_lt_s, instruction.Offset, func.stack.Count));
                            break;
                        case WasmDataType.f32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f32_lt, instruction.Offset, func.stack.Count));
                            break;
                        case WasmDataType.f64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f64_lt, instruction.Offset, func.stack.Count));
                            break;
                    }
                    func.stack.Pop();
                    func.stack.Pop();
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Ble_Un:
                case Code.Ble_Un_S:
                case Code.Clt_Un:
                    switch (func.stack.Peek())
                    {
                        case WasmDataType.i32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_lt_u, instruction.Offset, func.stack.Count));
                            break;
                        case WasmDataType.i64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i64_lt_u, instruction.Offset, func.stack.Count));
                            break;
                    }
                    func.stack.Pop();
                    func.stack.Pop();
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Bge:
                case Code.Bge_S:
                    switch (func.stack.Peek())
                    {
                        case WasmDataType.i32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_ge_s, instruction.Offset, func.stack.Count));
                            break;
                        case WasmDataType.i64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i64_ge_s, instruction.Offset, func.stack.Count));
                            break;
                        case WasmDataType.f32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f32_ge, instruction.Offset, func.stack.Count));
                            break;
                        case WasmDataType.f64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f64_ge, instruction.Offset, func.stack.Count));
                            break;
                    }
                    func.stack.Pop();
                    func.stack.Pop();
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Bge_Un:
                case Code.Bge_Un_S:
                    switch (func.stack.Peek())
                    {
                        case WasmDataType.i32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_ge_u, instruction.Offset, func.stack.Count));
                            break;
                        case WasmDataType.i64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i64_ge_u, instruction.Offset, func.stack.Count));
                            break;
                    }
                    func.stack.Pop();
                    func.stack.Pop();
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Beq:
                case Code.Beq_S:
                    switch (func.stack.Peek())
                    {
                        case WasmDataType.i32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_eq, instruction.Offset, func.stack.Count));
                            break;
                        case WasmDataType.i64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i64_eq, instruction.Offset, func.stack.Count));
                            break;
                        case WasmDataType.f32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f32_eq, instruction.Offset, func.stack.Count));
                            break;
                        case WasmDataType.f64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f64_eq, instruction.Offset, func.stack.Count));
                            break;
                    }
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.br_if, instruction.Offset, func.stack.Count - 1, WasmOperand.FromLong(((Instruction)instruction.Operand).Offset)));
                    func.stack.Pop();
                    func.stack.Pop();
                    break;
                case Code.Bgt_Un:
                case Code.Bgt_Un_S:
                    switch (func.stack.Peek())
                    {
                case WasmDataType.i32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_gt_u, instruction.Offset, func.stack.Count));
                            break;
                        case WasmDataType.i64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i64_gt_u, instruction.Offset, func.stack.Count));
                            break;
                        case WasmDataType.f32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f32_gt, instruction.Offset, func.stack.Count));
                            break;
                        case WasmDataType.f64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f64_gt, instruction.Offset, func.stack.Count));
                            break;
                    }
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.br_if, instruction.Offset, func.stack.Count - 1, WasmOperand.FromLong(((Instruction)instruction.Operand).Offset)));
                    func.stack.Pop();
                    func.stack.Pop();
                    break;
                case Code.Brfalse:
                case Code.Brfalse_S:
                    switch (func.stack.Peek())
                    {
                        case WasmDataType.i32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_const, instruction.Offset, func.stack.Count, WasmOperand.FromInt(0)));
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_eq, instruction.Offset, func.stack.Count + 1));
                            break;
                        case WasmDataType.i64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i64_const, instruction.Offset, func.stack.Count, WasmOperand.FromLong(0)));
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i64_eq, instruction.Offset, func.stack.Count + 1));
                            break;
                        case WasmDataType.f32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f32_const, instruction.Offset, func.stack.Count, WasmOperand.FromFloat(0)));
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f32_eq, instruction.Offset, func.stack.Count + 1));
                            break;
                        case WasmDataType.f64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f64_const, instruction.Offset, func.stack.Count, WasmOperand.FromDouble(0)));
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f64_eq, instruction.Offset, func.stack.Count + 1));
                            break;
                    }
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.br_if, instruction.Offset, func.stack.Count, WasmOperand.FromLong(((Instruction)instruction.Operand).Offset)));
                    func.stack.Pop();
                    break;
                case Code.Brtrue:
                case Code.Brtrue_S:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.br_if, instruction.Offset, func.stack.Count, WasmOperand.FromLong(((Instruction)instruction.Operand).Offset)));
                    func.stack.Pop();
                    break;
                case Code.Br:
                case Code.Br_S:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.br, instruction.Offset, func.stack.Count, WasmOperand.FromLong(((Instruction)instruction.Operand).Offset)));
                    break;
                case Code.Blt:
                case Code.Blt_S:
                    switch (func.stack.Peek())
                    {
                        case WasmDataType.i32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_lt_s, instruction.Offset, func.stack.Count));
                            break;
                        case WasmDataType.i64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i64_lt_s, instruction.Offset, func.stack.Count));
                            break;
                        case WasmDataType.f32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f32_lt, instruction.Offset, func.stack.Count));
                            break;
                        case WasmDataType.f64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f64_lt, instruction.Offset, func.stack.Count));
                            break;
                    }
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.br_if, instruction.Offset, func.stack.Count, WasmOperand.FromLong(((Instruction)instruction.Operand).Offset)));
                    func.stack.Pop();
                    break;
                case Code.Pop:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.drop, instruction.Offset, func.stack.Count));
                    func.stack.Pop();
                    break;
                case Code.Newarr:
                    func.stack.Pop();
                    string functionname;
                    TypeRef returnValue;
                    switch (instruction.Operand is TypeRef ? (instruction.Operand as TypeRef).FullName : (instruction.Operand as TypeDef).FullName)
                    {
                        case "System.Int32":
                            functionname = "Newarr_Int";
                            returnValue = Program.TypeInt;
                            func.stack.Push(WasmDataType.arri32);
                            break;
                        case "System.Int64":

                            functionname = "Newarr_Long";
                            returnValue = Program.TypeLong;
                            func.stack.Push(WasmDataType.arri64);
                            break;
                        case "System.Single":

                            functionname = "Newarr_Single";
                            returnValue = Program.TypeFloat;
                            func.stack.Push(WasmDataType.arrf32);
                            break;
                        case "System.Double":

                            functionname = "Newarr_Double";
                            returnValue = Program.TypeDouble;
                            func.stack.Push(WasmDataType.arrf64);
                            break;
                        default:

                            functionname = "Newarr_Obj";
                            returnValue = Program.TypeObject;
                            func.stack.Push(WasmDataType.arrObj);
                            break;
                    }
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.call, instruction.Offset, func.stack.Count + 1, new WasmExternFunctionOperand()
                    {
                        FunctionName = functionname,
                        ReturnValue = Program.TypeInt.ToTypeSig(),
                        Params = new List<TypeSig>() { Program.TypeInt.ToTypeSig() }
                    }));
                    break;
                case Code.Stelem_I1:
                case Code.Stelem_I2:
                case Code.Stelem_I4:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.call, instruction.Offset, func.stack.Count, new WasmExternFunctionOperand()
                    {
                        FunctionName = "Arr_Set_Int",
                        ReturnValue = Program.TypeVoid.ToTypeSig(),
                        Params = new List<TypeSig>() { Program.TypeInt.ToTypeSig(), Program.TypeInt.ToTypeSig(), Program.TypeInt.ToTypeSig() }
                    }));
                    func.stack.Pop();
                    func.stack.Pop();
                    func.stack.Pop();
                    break;
                case Code.Stelem_I8:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.call, instruction.Offset, func.stack.Count, new WasmExternFunctionOperand()
                    {
                        FunctionName = "Arr_Set_Long",
                        ReturnValue = Program.TypeVoid.ToTypeSig(),
                        Params = new List<TypeSig>() { Program.TypeInt.ToTypeSig(), Program.TypeInt.ToTypeSig(), Program.TypeLong.ToTypeSig() }
                    }));
                    func.stack.Pop();
                    func.stack.Pop();
                    func.stack.Pop();
                    break;
                case Code.Stelem_R4:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.call, instruction.Offset, func.stack.Count, new WasmExternFunctionOperand()
                    {
                        FunctionName = "Arr_Set_Float",
                        ReturnValue = Program.TypeVoid.ToTypeSig(),
                        Params = new List<TypeSig>() { Program.TypeInt.ToTypeSig(), Program.TypeInt.ToTypeSig(), Program.TypeFloat.ToTypeSig() }
                    }));
                    func.stack.Pop();
                    func.stack.Pop();
                    func.stack.Pop();
                    break;
                case Code.Stelem_R8:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.call, instruction.Offset, func.stack.Count, new WasmExternFunctionOperand()
                    {
                        FunctionName = "Arr_Set_Double",
                        ReturnValue = Program.TypeVoid.ToTypeSig(),
                        Params = new List<TypeSig>() { Program.TypeInt.ToTypeSig(), Program.TypeInt.ToTypeSig(), Program.TypeDouble.ToTypeSig() }
                    }));
                    func.stack.Pop();
                    func.stack.Pop();
                    func.stack.Pop();
                    break;
                case Code.Stelem:
                case Code.Stelem_Ref:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.call, instruction.Offset, func.stack.Count, new WasmExternFunctionOperand()
                    {
                        FunctionName = "Arr_Set_Object",
                        ReturnValue = Program.TypeVoid.ToTypeSig(),
                        Params = new List<TypeSig>() { Program.TypeInt.ToTypeSig(), Program.TypeInt.ToTypeSig(), Program.TypeObject.ToTypeSig() }
                    }));
                    func.stack.Pop();
                    func.stack.Pop();
                    func.stack.Pop();
                    break;
                case Code.Ldelem_I4:
                case Code.Ldelem_U1:
                case Code.Ldelem_U2:
                case Code.Ldelem_U4:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.call, instruction.Offset, func.stack.Count, new WasmExternFunctionOperand()
                    {
                        FunctionName = "Arr_Get_Int",
                        ReturnValue = Program.TypeInt.ToTypeSig(),
                        Params = new List<TypeSig>() { Program.TypeInt.ToTypeSig(), Program.TypeInt.ToTypeSig() }
                    }));
                    func.stack.Pop();
                    func.stack.Pop();
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Ldelem_I8:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.call, instruction.Offset, func.stack.Count, new WasmExternFunctionOperand()
                    {
                        FunctionName = "Arr_Get_Long",
                        ReturnValue = Program.TypeLong.ToTypeSig(),
                        Params = new List<TypeSig>() { Program.TypeInt.ToTypeSig(), Program.TypeInt.ToTypeSig() }
                    }));
                    func.stack.Pop();
                    func.stack.Pop();
                    func.stack.Push(WasmDataType.i64);
                    break;
                case Code.Ldelem_R4:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.call, instruction.Offset, func.stack.Count, new WasmExternFunctionOperand()
                    {
                        FunctionName = "Arr_Get_Float",
                        ReturnValue = Program.TypeFloat.ToTypeSig(),
                        Params = new List<TypeSig>() { Program.TypeInt.ToTypeSig(), Program.TypeInt.ToTypeSig() }
                    }));
                    func.stack.Pop();
                    func.stack.Pop();
                    func.stack.Push(WasmDataType.f32);
                    break;
                case Code.Ldelem_R8:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.call, instruction.Offset, func.stack.Count, new WasmExternFunctionOperand()
                    {
                        FunctionName = "Arr_Get_Double",
                        ReturnValue = Program.TypeDouble.ToTypeSig(),
                        Params = new List<TypeSig>() { Program.TypeInt.ToTypeSig(), Program.TypeInt.ToTypeSig() }
                    }));
                    func.stack.Pop();
                    func.stack.Pop();
                    func.stack.Push(WasmDataType.f64);
                    break;
                case Code.Ldelem:
                case Code.Ldelem_Ref:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.call, instruction.Offset, func.stack.Count, new WasmExternFunctionOperand()
                    {
                        FunctionName = "Arr_Get_Object",
                        ReturnValue = Program.TypeObject.ToTypeSig(),
                        Params = new List<TypeSig>() { Program.TypeInt.ToTypeSig(), Program.TypeInt.ToTypeSig() }
                    }));
                    func.stack.Pop();
                    func.stack.Pop();
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Ldlen:
                    string lengthFunc = "invalid_Ldlen";
                    switch (func.stack.Peek())
                    {
                        case WasmDataType.arri32:
                            lengthFunc = "Arr_Count_Int";
                            break;
                        case WasmDataType.arri64:
                            lengthFunc = "Arr_Count_Long";
                            break;
                        case WasmDataType.arrf32:
                            lengthFunc = "Arr_Count_Float";
                            break;
                        case WasmDataType.arrf64:
                            lengthFunc = "Arr_Count_Double";
                            break;
                        case WasmDataType.arrObj:
                            lengthFunc = "Arr_Count_Object";
                            break;
                    }
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.call, instruction.Offset, func.stack.Count, new WasmExternFunctionOperand()
                    {
                        FunctionName = lengthFunc,
                        ReturnValue = Program.TypeInt.ToTypeSig(),
                        Params = new List<TypeSig>() { Program.TypeInt.ToTypeSig() }
                    }));
                    func.stack.Pop();
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Box:
                    switch (func.stack.Peek())
                    {
                        case WasmDataType.i32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.call, instruction.Offset, func.stack.Count, new WasmExternFunctionOperand()
                            {
                                FunctionName = "Box_Int",
                                ReturnValue = Program.TypeInt.ToTypeSig(),
                                Params = new List<TypeSig>() { Program.TypeInt.ToTypeSig() }
                            }));
                            break;
                        case WasmDataType.i64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.call, instruction.Offset, func.stack.Count, new WasmExternFunctionOperand()
                            {
                                FunctionName = "Box_Long",
                                ReturnValue = Program.TypeInt.ToTypeSig(),
                                Params = new List<TypeSig>() { Program.TypeLong.ToTypeSig() }
                            }));
                            break;
                        case WasmDataType.f32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.call, instruction.Offset, func.stack.Count, new WasmExternFunctionOperand()
                            {
                                FunctionName = "Box_Float",
                                ReturnValue = Program.TypeInt.ToTypeSig(),
                                Params = new List<TypeSig>() { Program.TypeFloat.ToTypeSig() }
                            }));
                            break;
                        case WasmDataType.f64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.call, instruction.Offset, func.stack.Count, new WasmExternFunctionOperand()
                            {
                                FunctionName = "Box_Double",
                                ReturnValue = Program.TypeInt.ToTypeSig(),
                                Params = new List<TypeSig>() { Program.TypeDouble.ToTypeSig() }
                            }));
                            break;
                        default:
                            break;
                    }

                    func.stack.Pop();
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Conv_I4:
                    switch (func.stack.Peek())
                    {
                        case WasmDataType.i32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.nop, instruction.Offset, func.stack.Count));
                            break;
                        case WasmDataType.i64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_wrap_i64, instruction.Offset, func.stack.Count));
                            break;
                        case WasmDataType.f32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_trunc_f32_s, instruction.Offset, func.stack.Count));
                            break;
                        case WasmDataType.f64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_trunc_f64_s, instruction.Offset, func.stack.Count));
                            break;
                        default:
                            break;
                    }
                    func.stack.Pop();
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Conv_I8:
                    switch (func.stack.Peek())
                    {
                        case WasmDataType.i32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i64_extend_i32_s, instruction.Offset, func.stack.Count));
                            break;
                        case WasmDataType.i64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.nop, instruction.Offset, func.stack.Count));
                            break;
                        case WasmDataType.f32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i64_trunc_f32_s, instruction.Offset, func.stack.Count));
                            break;
                        case WasmDataType.f64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i64_trunc_f64_s, instruction.Offset, func.stack.Count));
                            break;
                        default:
                            break;
                    }
                    func.stack.Pop();
                    func.stack.Push(WasmDataType.i64);
                    break;
                case Code.Conv_R4:
                    switch (func.stack.Peek())
                    {
                        case WasmDataType.i32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f32_convert_i32_s, instruction.Offset, func.stack.Count));
                            break;
                        case WasmDataType.i64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f32_convert_i64_s, instruction.Offset, func.stack.Count));
                            break;
                        case WasmDataType.f32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.nop, instruction.Offset, func.stack.Count));
                            break;
                        case WasmDataType.f64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f32_demote_f64, instruction.Offset, func.stack.Count));
                            break;
                        default:
                            break;
                    }
                    func.stack.Pop();
                    func.stack.Push(WasmDataType.f32);
                    break;
                case Code.Conv_R8:
                    switch (func.stack.Peek())
                    {
                        case WasmDataType.i32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f64_convert_i32_s, instruction.Offset, func.stack.Count));
                            break;
                        case WasmDataType.i64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f64_convert_i64_s, instruction.Offset, func.stack.Count));
                            break;
                        case WasmDataType.f32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f64_promote_f32, instruction.Offset, func.stack.Count));
                            break;
                        case WasmDataType.f64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.nop, instruction.Offset, func.stack.Count));
                            break;
                        default:
                            break;
                    }
                    func.stack.Pop();
                    func.stack.Push(WasmDataType.f64);
                    break;
                case Code.Unbox_Any:
                    func.stack.Pop();
                    switch (instruction.Operand is TypeRef ? (instruction.Operand as TypeRef).FullName : (instruction.Operand as TypeDef).FullName)
                    {
                        case "System.Boolean":
                        case "System.Integer":
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.call, instruction.Offset, func.stack.Count, new WasmExternFunctionOperand()
                            {
                                FunctionName = "Unbox_Int",
                                ReturnValue = Program.TypeInt.ToTypeSig(),
                                Params = new List<TypeSig>() { Program.TypeInt.ToTypeSig() }
                            }));
                            func.stack.Push(WasmDataType.i32);
                            break;
                        case "System.Long":
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.call, instruction.Offset, func.stack.Count, new WasmExternFunctionOperand()
                            {
                                FunctionName = "Unbox_Long",
                                ReturnValue = Program.TypeLong.ToTypeSig(),
                                Params = new List<TypeSig>() { Program.TypeInt.ToTypeSig() }
                            }));
                            func.stack.Push(WasmDataType.i64);
                            break;
                        case "System.Float":
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.call, instruction.Offset, func.stack.Count, new WasmExternFunctionOperand()
                            {
                                FunctionName = "Unbox_Float",
                                ReturnValue = Program.TypeFloat.ToTypeSig(),
                                Params = new List<TypeSig>() { Program.TypeInt.ToTypeSig() }
                            }));
                            func.stack.Push(WasmDataType.f32);
                            break;
                        case "System.Double":
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.call, instruction.Offset, func.stack.Count, new WasmExternFunctionOperand()
                            {
                                FunctionName = "Unbox_Double",
                                ReturnValue = Program.TypeDouble.ToTypeSig(),
                                Params = new List<TypeSig>() { Program.TypeInt.ToTypeSig() }
                            }));
                            func.stack.Push(WasmDataType.f64);
                            break;
                        default:
                            break;
                    }
                    break;
                case Code.Neg:
                    switch (func.stack.Peek())
                    {
                        case WasmDataType.i32:
                            if (!func.Locals.ContainsKey($"temp{func.stack.Peek()}"))
                                func.Locals.Add($"temp{func.stack.Peek()}", func.stack.Peek());
                            
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.set_local, instruction.Offset, func.stack.Count, WasmOperand.FromLocalField($"temp{func.stack.Peek()}")));
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_const, instruction.Offset, func.stack.Count - 1, WasmOperand.FromInt(0)));
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.get_local, instruction.Offset, func.stack.Count, WasmOperand.FromLocalField($"temp{func.stack.Peek()}")));
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_sub, instruction.Offset, func.stack.Count + 1));
                            
                            break;
                        case WasmDataType.i64:
                            if (!func.Locals.ContainsKey($"temp{func.stack.Peek()}"))
                                func.Locals.Add($"temp{func.stack.Peek()}", func.stack.Peek());

                            func.Instructions.Add(new WasmInstruction(WasmInstructions.set_local, instruction.Offset, func.stack.Count, WasmOperand.FromLocalField($"temp{func.stack.Peek()}")));
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i64_const, instruction.Offset, func.stack.Count - 1, WasmOperand.FromLong(0)));
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.get_local, instruction.Offset, func.stack.Count, WasmOperand.FromLocalField($"temp{func.stack.Peek()}")));
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i64_sub, instruction.Offset, func.stack.Count + 1));

                            break;
                        case WasmDataType.f32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f32_neg, instruction.Offset, func.stack.Count));
                            break;
                        case WasmDataType.f64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f64_neg, instruction.Offset, func.stack.Count));
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    Console.Error.WriteLine("Unkown opcode " + instruction.OpCode.Code);
                    break;
            }
        }


        public static WasmDataType? GetWasmType(TypeSig type)
        {
            return GetWasmType(type?.FullName);
        }
        public static WasmDataType? GetWasmTypeWoArray(TypeSig type)
        {
            return GetWasmTypeWoArray(type?.FullName);
        }
        public static WasmDataType? GetWasmTypeWoArray(string type)
        {
            if (type == null)
                return null;
            if (type == "System.Void")
                return null;
            if (type == "System.Single")
                return WasmDataType.f32;
            if (type == "System.Double")
                return WasmDataType.f64;
            if (type == "System.Int64")
                return WasmDataType.i64;

            return WasmDataType.i32;
        }
        public static WasmDataType? GetWasmType(string type)
        {
            if (type == null)
                return null;
            if (type == "System.Void")
                return null;
            if (type == "System.Single")
                return WasmDataType.f32;
            if (type == "System.Double")
                return WasmDataType.f64;
            if (type == "System.Int64")
                return WasmDataType.i64;

            if (type == "System.Int32[]")
                return WasmDataType.arri32;
            if (type == "System.Int64[]")
                return WasmDataType.arri64;
            if (type == "System.Single[]")
                return WasmDataType.arrf32;
            if (type == "System.Double[]")
                return WasmDataType.arrf64;
            if (type.Contains("[]"))
                return WasmDataType.arrObj;

            return WasmDataType.i32;
        }


    }
}
#endif