#if UNITY_EDITOR
using dnlib.DotNet.Emit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter
{
    public class BlockRebuilder
    {
        private readonly WasmFunction wasmFunction;

        public BlockRebuilder(WasmFunction wasmFunction)
        {
            this.wasmFunction = wasmFunction;
        }

        private int ForCounter = 1;

        public void Rebuild()
        {
            FixJumpsWithStack();
            BuildZeroStackBlocks();
            RebuildSubBlock(wasmFunction.Blocks);
        }

        private void FixJumpsWithStack()
        {
            int counter = 0;
            for (int i = 0; i < wasmFunction.Instructions.Count; i++)
            {
                var inst = wasmFunction.Instructions[i];

                if (inst.Instruction == WasmInstructions.br && inst.StackSizeBefore != 0)
                {
                    Console.WriteLine("Funtion needs jump fixing " + wasmFunction.Name);
                    var altTarget = wasmFunction.Instructions[i + 1];
                    var thisTarget = wasmFunction.Instructions[i + 2];

                    wasmFunction.Locals.Add($"jumpFix{counter}", WasmDataType.i32);
                    wasmFunction.Instructions.Insert(i, new WasmInstruction(WasmInstructions.set_local, inst.Offset, inst.StackSizeBefore, WasmOperand.FromLocalField($"jumpFix{counter}")));
                    wasmFunction.Instructions.Insert(wasmFunction.Instructions.IndexOf(thisTarget), new WasmInstruction(WasmInstructions.set_local, 9999, inst.StackSizeBefore, WasmOperand.FromLocalField($"jumpFix{counter}")));
                    wasmFunction.Instructions.Insert(wasmFunction.Instructions.IndexOf(thisTarget), new WasmInstruction(WasmInstructions.get_local, 0, 0, WasmOperand.FromLocalField($"jumpFix{counter}")));
                    counter++;

                    inst.StackSizeBefore = 0;
                    altTarget.StackSizeBefore = 0;
                    //fix stack count for all other instructions
                    for (int j = wasmFunction.Instructions.IndexOf(thisTarget); j < wasmFunction.Instructions.Count; j++)
                    {
                        wasmFunction.Instructions[j].StackSizeBefore--;

                    }


                }
            }
        }

        public void RebuildSubBlock(List<Block> blocks)
        {
            BuildForBlocks(blocks);
            BuildIfBlocks(blocks);
        }

        private void BuildIfBlocks(List<Block> blocks)
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                ZeroStackBlock block = blocks[i] as ZeroStackBlock;
                if (block == null)
                    continue;

                List<(List<Block>, List<Block>)> cases = new List<(List<Block>, List<Block>)>();

                if (block.LastInstruction == WasmInstructions.br_if)
                {
                    var EndOfIf = blocks.IndexOf(blocks.Single(x => x.FirstOffset == (block.LastOperand as WasmLongOperand).AsUInt)) - 1;
                    if (blocks[EndOfIf].FirstInstruction != WasmInstructions.br)//simple if
                    {
                        cases.Add((new List<Block>() { block }, blocks.Skip(i + 1).Take(EndOfIf - i).ToList()));

                        var ifBlock = new IfBlock(cases);

                        blocks.Insert(i, ifBlock);
                        blocks.RemoveRange(i + 1, EndOfIf - i + 1);
                        foreach (var ifcase in ifBlock.Cases)
                        {
                            RebuildSubBlock(ifcase.Item2);
                        }
                    }
                    else if (!blocks.Any(x => x.FirstOffset == (blocks[EndOfIf].LastOperand as WasmLongOperand).AsUInt)) // ||
                    {
                        var shortFin = blocks.Single(x => x.FirstOffset == (block.LastOperand as WasmLongOperand).AsUInt);

                        cases.Add((new List<Block>() { block }, new List<Block>() { shortFin }));
                        cases.Add((null, blocks.Skip(i + 1).Take(blocks.IndexOf(shortFin) - 1 - i - 1).ToList() )); // remove last br

                        var ifBlock = new IfBlock(cases);

                        blocks.Insert(i, ifBlock);
                        blocks.RemoveRange(i + 1, EndOfIf - i + 2);
                        foreach (var ifcase in ifBlock.Cases)
                        {
                            RebuildSubBlock(ifcase.Item2);
                        }
                    }
                    else
                    {
                        var actualEnd = blocks.IndexOf(blocks.Single(x => x.FirstOffset == (blocks[EndOfIf].LastOperand as WasmLongOperand).AsUInt)) - 1;

                        cases.Add((new List<Block>() { block }, blocks.Skip(i + 1).Take(EndOfIf - i - 1).ToList()));

                        var remaining = blocks.Skip(EndOfIf + 1).Take(actualEnd - EndOfIf).ToList();
                        for (int j = 0; j < remaining.Count; j++)
                        {
                            var remainingBlock = remaining[j];
                            if (remainingBlock.LastInstruction == WasmInstructions.br_if)
                            {
                                if (remaining.Last().FirstOffset < (remainingBlock.LastOperand as WasmLongOperand).AsUInt)
                                {//else
                                    cases.Add((remaining.Take(j + 1).ToList(), remaining.Skip(j + 1).ToList()));
                                    remaining.Clear();
                                    j = 0;
                                }
                                else
                                {
                                    var elseifEnd = remaining.IndexOf(remaining.Single(x => x.FirstOffset == (remainingBlock.LastOperand as WasmLongOperand).AsUInt)) - 1;
                                    cases.Add((remaining.Take(j + 1).ToList(), remaining.Skip(j + 1).Take(elseifEnd - j - 1).ToList()));
                                    remaining.RemoveRange(0, elseifEnd + 1);
                                    j = 0;
                                }
                            }
                        }
                        if (remaining.Count != 0)
                        {
                            cases.Add((null, remaining));
                        }

                        var ifBlock = new IfBlock(cases);

                        blocks.Insert(i, ifBlock);
                        blocks.RemoveRange(i + 1, actualEnd - i);
                        foreach (var ifcase in ifBlock.Cases)
                        {
                            RebuildSubBlock(ifcase.Item2);
                        }
                    }
                }
            }
        }

        private void BuildForBlocks(List<Block> blocks)
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                ZeroStackBlock block = blocks[i] as ZeroStackBlock;
                if (block == null)
                    continue;

                if (block.Instructions.Count == 1 && block.FirstInstruction == WasmInstructions.br
                    && blocks[i + 1].FirstInstruction == WasmInstructions.nop
                    && blocks.Any(x => x.LastInstruction == WasmInstructions.br_if && x.LastOperand is WasmLongOperand && (x.LastOperand as WasmLongOperand).AsUInt == blocks[i + 1].FirstOffset))

                {
                    var brif = blocks.Single(x => x.LastInstruction == WasmInstructions.br_if && x.LastOperand is WasmLongOperand && (x.LastOperand as WasmLongOperand).AsUInt == blocks[i + 1].FirstOffset);

                    int start = i;
                    int end = blocks.IndexOf(brif);

                    if (end < start)
                        continue;//elseif whatever ignore

                    int startInstructionBlock = i + 2;//nop ignored

                    int endInstructionBlock = end - 3;

                    int incBlock = end - 2;
                    int checkBlock = end - 1;

                    (blocks[checkBlock + 1] as ZeroStackBlock).Instructions.Last().Operand = WasmOperand.FromStaticStringmField($"lpfor{ForCounter}");
                    var forBlock = new ForBlock(ForCounter,
                        blocks.Skip(startInstructionBlock).Take(endInstructionBlock - startInstructionBlock + 1).ToList(),
                        new List<Block>() { blocks[incBlock] },
                        new List<Block>() { blocks[checkBlock], blocks[checkBlock + 1] },//garbage needs to be improved
                        CreateForCheckBlock(ForCounter)
                    );
                    wasmFunction.Locals.Add($"for{ForCounter}", WasmDataType.i32);

                    blocks.Insert(i, forBlock);
                    blocks.RemoveRange(start + 1, end - start + 1);
                    blocks.Insert(0, CreateForInitializerBlock(ForCounter));
                    ForCounter++;
                    RebuildSubBlock(forBlock.Instructions);
                }
            }
        }

        private void BuildZeroStackBlocks()
        {
            List<Block> blocks = new List<Block>();
            int lastIndex = 0;
            for (int i = 0; i < wasmFunction.Instructions.Count; i++)
            {
                WasmInstruction instruction = wasmFunction.Instructions[i];
                if (instruction.StackSizeBefore == 0)
                {
                    blocks.Add(new ZeroStackBlock(wasmFunction.Instructions.Skip(lastIndex).Take(i - lastIndex).ToList()));
                    lastIndex = i;
                }
            }
            if (lastIndex != wasmFunction.Instructions.Count - 1)
            {
                blocks.Add(new ZeroStackBlock(wasmFunction.Instructions.Skip(lastIndex).Take(wasmFunction.Instructions.Count - 1 - lastIndex).ToList()));
            }

            if (wasmFunction.Instructions.Last().Instruction == WasmInstructions._return)//wtf. why is the return not added
            {
                blocks.Add(new ZeroStackBlock(new List<WasmInstruction>() { wasmFunction.Instructions.Last() }));
            }
            blocks.RemoveAt(0);
            wasmFunction.Blocks = blocks;
        }



        public ZeroStackBlock CreateForInitializerBlock(int counter)
        {
            var block = new ZeroStackBlock(new List<WasmInstruction>()
            {
                new WasmInstruction(WasmInstructions.i32_const, 9999, 9999, WasmOperand.FromInt(1)),
                new WasmInstruction(WasmInstructions.set_local, 9999, 9999, WasmOperand.FromStaticStringmField($"for{counter}"))
            });
            return block;
        }
        public ZeroStackBlock CreateForCheckBlock(int counter)
        {
            var block = new ZeroStackBlock(new List<WasmInstruction>()
            {
                new WasmInstruction(WasmInstructions.loop, 9999, 9999, WasmOperand.FromStaticStringmField($"lpfor{counter}")),
                new WasmInstruction(WasmInstructions.block, 9999, 9999, WasmOperand.FromStaticStringmField($"blfor{counter}")),
                new WasmInstruction(WasmInstructions.get_local, 9999, 9999, WasmOperand.FromStaticStringmField($"for{counter}")),
                new WasmInstruction(WasmInstructions.i32_const, 9999, 9999, WasmOperand.FromInt(0)),
                new WasmInstruction(WasmInstructions.set_local, 9999, 9999, WasmOperand.FromStaticStringmField($"for{counter}")),
                new WasmInstruction(WasmInstructions.br_if, 9999, 9999, WasmOperand.FromStaticStringmField($"blfor{counter}"))
            });
            return block;
        }
    }
}
#endif