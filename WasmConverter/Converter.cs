﻿using dnlib.DotNet;
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

            foreach (var parameter in method.Parameters.Where(x => !x.IsHiddenThisParameter))
            {
                func.Parameters.Add(GetWasmType(parameter.Type).Value);
            }

            for (int i = 0; i < func.Parameters.Count; i++)
            {
                func.Locals.Add($"param{i}", func.Parameters[i]);
            }
            var instructions = method.Body.Instructions;
            for (int i = 0; i < instructions.Count; i++)
            {
                /*if (instructions[i].OpCode == OpCodes.Stloc_0 && instructions[i + 1].OpCode == OpCodes.Ldloc_0)
                {
                    instructions.RemoveAt(i);
                    instructions.RemoveAt(i);
                }
                else*/
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
            func.FixControlFlow();
            return func;
        }

        public void TranslateInstruction(Instruction instruction, WasmFunction func)
        {
            switch (instruction.OpCode.Code)
            {
                case Code.Ldc_R4:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.f32_const, instruction.Offset, instruction.Operand));
                    func.stack.Push(WasmDataType.f32);
                    break;
                case Code.Ldc_R8:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.f64_const, instruction.Offset, instruction.Operand));
                    func.stack.Push(WasmDataType.f64);
                    break;
                case Code.Ldc_I8:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.i64_const, instruction.Offset, instruction.Operand));
                    func.stack.Push(WasmDataType.i64);
                    break;
                case Code.Ldc_I4:
                case Code.Ldc_I4_S:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_const, instruction.Offset, instruction.Operand));
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Ldstr:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_const, instruction.Offset, Allocate((string)instruction.Operand, func)));
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Ldc_I4_M1:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_const, instruction.Offset, -1));
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Ldc_I4_0:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_const, instruction.Offset, 0));
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Ldc_I4_1:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_const, instruction.Offset, 1));
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Ldc_I4_2:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_const, instruction.Offset, 2));
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Ldc_I4_3:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_const, instruction.Offset, 3));
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Ldc_I4_4:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_const, instruction.Offset, 4));
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Ldc_I4_5:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_const, instruction.Offset, 5));
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Ldc_I4_6:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_const, instruction.Offset, 6));
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Ldc_I4_7:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_const, instruction.Offset, 7));
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Ldc_I4_8:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_const, instruction.Offset, 8));
                    func.stack.Push(WasmDataType.i32);
                    break;

                case Code.Ldarg_0:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.get_local, instruction.Offset, "param0"));
                    func.stack.Push(func.Parameters[0]);
                    break;
                case Code.Ldarg_1:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.get_local, instruction.Offset, "param1"));
                    func.stack.Push(func.Parameters[1]);
                    break;
                case Code.Ldarg_2:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.get_local, instruction.Offset, "param2"));
                    func.stack.Push(func.Parameters[2]);
                    break;
                case Code.Ldarg_3:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.get_local, instruction.Offset, "param3"));
                    func.stack.Push(func.Parameters[3]);
                    break;
                case Code.Ldarg:
                case Code.Ldarg_S:
                    var Ldarg = (int)instruction.Operand;
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.get_local, instruction.Offset, $"param{Ldarg}"));
                    func.stack.Push(func.Parameters[Ldarg]);
                    break;

                case Code.Stloc_0:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.set_local, instruction.Offset, "local0"));
                    if (!func.Locals.ContainsKey("local0"))
                        func.Locals.Add($"local0", func.stack.Peek());
                    func.stack.Pop();
                    break;
                case Code.Stloc_1:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.set_local, instruction.Offset, "local1"));
                    if (!func.Locals.ContainsKey("local1"))
                        func.Locals.Add($"local1", func.stack.Peek());
                    func.stack.Pop();
                    break;
                case Code.Stloc_2:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.set_local, instruction.Offset, "local2"));
                    if (!func.Locals.ContainsKey("local2"))
                        func.Locals.Add($"local2", func.stack.Peek());
                    func.stack.Pop();
                    break;
                case Code.Stloc_3:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.set_local, instruction.Offset, "local3"));
                    if (!func.Locals.ContainsKey("local3"))
                        func.Locals.Add($"local3", func.stack.Peek());
                    func.stack.Pop();
                    break;
                case Code.Stloc:
                case Code.Stloc_S:
                    var Stloc = (int)instruction.Operand;
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.set_local, instruction.Offset, $"local{Stloc}"));
                    if (!func.Locals.ContainsKey($"local{Stloc}"))
                        func.Locals.Add($"local{Stloc}", func.stack.Peek());
                    func.stack.Pop();
                    break;
                case Code.Ldloc_0:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.get_local, instruction.Offset, "local0"));
                    func.stack.Push(func.Locals["local0"]);
                    break;
                case Code.Ldloc_1:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.get_local, instruction.Offset, "local1"));
                    func.stack.Push(func.Locals["local1"]);
                    break;
                case Code.Ldloc_2:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.get_local, instruction.Offset, "local2"));
                    func.stack.Push(func.Locals["local2"]);
                    break;
                case Code.Ldloc_3:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.get_local, instruction.Offset, "local3"));
                    func.stack.Push(func.Locals["local3"]);
                    break;
                case Code.Ldloc:
                case Code.Ldloc_S:
                    var Ldloc = (int)instruction.Operand;
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.get_local, instruction.Offset, $"local{Ldloc}"));
                    func.stack.Push(func.Locals[$"local{Ldloc}"]);
                    break;
                case Code.Ldloca_S:
                    var Ldlocas = (Local)instruction.Operand;
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.get_local, instruction.Offset, $"local{Ldlocas.Index}"));
                    func.stack.Push(func.Locals[$"local{Ldlocas.Index}"]);
                    break;
                case Code.Dup:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.tee_local, instruction.Offset, $"temp{func.stack.Peek()}"));
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.get_local, instruction.Offset, $"temp{func.stack.Peek()}"));
                    if (!func.Locals.ContainsKey($"temp{func.stack.Peek()}"))
                        func.Locals.Add($"temp{func.stack.Peek()}", func.stack.Peek());
                    func.stack.Push(func.stack.Peek());
                    
                    break;

                case Code.Call:
                case Code.Callvirt:
                case Code.Ldsfld:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.call, instruction.Offset, instruction.Operand));
                    var method = instruction.Operand as MethodDef;
                    var member = instruction.Operand as MemberRef;
                    if ((member?.HasThis ?? false) || (method?.HasThis ?? false))
                    {
                        func.stack.Pop();
                    }

                    if (member != null)
                        for (int i = 0; i < (member?.GetParamCount() ?? method.GetParamCount()); i++)
                        {
                            func.stack.Pop();
                        }

                    var type = GetWasmType(member?.ReturnType ?? method?.ReturnType);
                    if (type.HasValue)
                    {
                        func.stack.Push(type.Value);
                    }
                    type = GetWasmType(member?.FieldSig?.Type);
                    if (type.HasValue)
                    {
                        func.stack.Push(type.Value);
                    }
                    break;
                case Code.Newobj:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.call, instruction.Offset, instruction.Operand));

                    foreach (var item in ((MemberRef)instruction.Operand).GetParams())
                    {
                        func.stack.Pop();
                    }
                    func.stack.Push(WasmDataType.i32);

                    break;
                case Code.Initobj:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_const, instruction.Offset, 0));
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Ret:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions._return, instruction.Offset));
                    break;
                case Code.Nop:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.nop, instruction.Offset));
                    break;
                case Code.Add:
                    switch (func.stack.Peek())
                    {
                        case WasmDataType.i32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_add, instruction.Offset));
                            func.stack.Pop();
                            func.stack.Pop();
                            func.stack.Push(WasmDataType.i32);
                            break;
                        case WasmDataType.i64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i64_add, instruction.Offset));
                            func.stack.Pop();
                            func.stack.Pop();
                            func.stack.Push(WasmDataType.i64);
                            break;
                        case WasmDataType.f32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f32_add, instruction.Offset));
                            func.stack.Pop();
                            func.stack.Pop();
                            func.stack.Push(WasmDataType.f32);
                            break;
                        case WasmDataType.f64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f64_add, instruction.Offset));
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
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_sub, instruction.Offset));
                            func.stack.Pop();
                            func.stack.Pop();
                            func.stack.Push(WasmDataType.i32);
                            break;
                        case WasmDataType.i64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i64_sub, instruction.Offset));
                            func.stack.Pop();
                            func.stack.Pop();
                            func.stack.Push(WasmDataType.i64);
                            break;
                        case WasmDataType.f32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f32_sub, instruction.Offset));
                            func.stack.Pop();
                            func.stack.Pop();
                            func.stack.Push(WasmDataType.f32);
                            break;
                        case WasmDataType.f64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f64_sub, instruction.Offset));
                            func.stack.Pop();
                            func.stack.Pop();
                            func.stack.Push(WasmDataType.f64);
                            break;
                    }
                    break;
                case Code.Ceq:
                    switch (func.stack.Peek())
                    {
                        case WasmDataType.i32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_eq, instruction.Offset));
                            break;
                        case WasmDataType.i64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i64_eq, instruction.Offset));
                            break;
                        case WasmDataType.f32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f32_eq, instruction.Offset));
                            break;
                        case WasmDataType.f64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f64_eq, instruction.Offset));
                            break;
                    }
                    func.stack.Pop();
                    func.stack.Pop();
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Cgt:
                    switch (func.stack.Peek())
                    {
                        case WasmDataType.i32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_gt_s, instruction.Offset));
                            break;
                        case WasmDataType.i64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i64_gt_s, instruction.Offset));
                            break;
                        case WasmDataType.f32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f32_gt, instruction.Offset));
                            break;
                        case WasmDataType.f64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f64_gt, instruction.Offset));
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
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_gt_u, instruction.Offset));
                            break;
                        case WasmDataType.i64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i64_gt_u, instruction.Offset));
                            break;
                    }
                    func.stack.Pop();
                    func.stack.Pop();
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Clt:
                    switch (func.stack.Peek())
                    {
                        case WasmDataType.i32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_lt_s, instruction.Offset));
                            break;
                        case WasmDataType.i64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i64_lt_s, instruction.Offset));
                            break;
                        case WasmDataType.f32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f32_lt, instruction.Offset));
                            break;
                        case WasmDataType.f64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f64_lt, instruction.Offset));
                            break;
                    }
                    func.stack.Pop();
                    func.stack.Pop();
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Clt_Un:
                    switch (func.stack.Peek())
                    {
                        case WasmDataType.i32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_lt_u, instruction.Offset));
                            break;
                        case WasmDataType.i64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i64_lt_u, instruction.Offset));
                            break;
                    }
                    func.stack.Pop();
                    func.stack.Pop();
                    func.stack.Push(WasmDataType.i32);
                    break;
                case Code.Brfalse_S:
                    switch (func.stack.Peek())
                    {
                        case WasmDataType.i32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_const, instruction.Offset, 0));
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i32_eq, instruction.Offset));
                            break;
                        case WasmDataType.i64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i64_const, instruction.Offset, 0));
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.i64_eq, instruction.Offset));
                            break;
                        case WasmDataType.f32:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f32_const, instruction.Offset, 0));
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f32_eq, instruction.Offset));
                            break;
                        case WasmDataType.f64:
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f64_const, instruction.Offset, 0));
                            func.Instructions.Add(new WasmInstruction(WasmInstructions.f64_eq, instruction.Offset));
                            break;
                    }
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.br_if, instruction.Offset, ((Instruction)instruction.Operand).Offset));
                    func.stack.Pop();
                    break;
                case Code.Brtrue_S:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.br_if, instruction.Offset, ((Instruction)instruction.Operand).Offset));
                    func.stack.Pop();
                    break;
                case Code.Br_S:
                    func.Instructions.Add(new WasmInstruction(WasmInstructions.br, instruction.Offset, ((Instruction)instruction.Operand).Offset));
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
            return WasmDataType.i32;
        }

        public int Allocate(string str, WasmFunction func)
        {
            if (func.Module.Strings.ContainsKey(str))
            {
                return func.Module.Strings[str];
            }
            else
            {
                func.Module.Strings.Add(str, func.Module.MemoryPtr);
                func.Module.MemoryPtr += str.Length + 1;
                return func.Module.Strings[str];
            }
        }
    }
}