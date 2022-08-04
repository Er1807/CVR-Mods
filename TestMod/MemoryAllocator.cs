using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Refs;
using Wasmtime;

namespace Test
{
    internal class MemoryAllocator : IRef
    {
        private class Allocations
        {
            public int Ptr { get; set; }
            public int Size { get; set; }
            public Allocations Pref { get; set; }
            public Allocations Next { get; set; }
        }


        private int start = 0;
        private List<Allocations> allocations = new List<Allocations>();

        public void Init(Memory memory, IStore store)
        {
            start = memory.ReadInt32(store, 0);
            allocations.Add(new Allocations() { Ptr = start, Size=10 });
        }
        
        public void Setup(Linker linker, Store store)
        {
            linker.DefineFunction("env", "malloc", (Caller caller, int size) =>
            {
                foreach (var alloc in allocations)
                {
                    if (alloc.Next != null)
                    {
                        if (alloc.Next.Ptr - alloc.Ptr - alloc.Size >= size)
                        {
                            var newAlloc = new Allocations()
                            {
                                Ptr = alloc.Ptr+alloc.Size,
                                Size = size,
                                Pref = alloc,
                                Next = alloc.Next
                            };
                            alloc.Next.Pref = newAlloc;
                            alloc.Next = newAlloc;
                            allocations.Insert(allocations.IndexOf(alloc)+1, newAlloc);
                            return newAlloc.Ptr;
                        }
                    }
                    if (alloc.Next == null)
                    {
                        alloc.Next = new Allocations()
                        {
                            Ptr = alloc.Ptr + alloc.Size,
                            Pref = alloc
                        };
                        allocations.Insert(allocations.IndexOf(alloc)+1, alloc.Next);
                        return alloc.Next.Ptr;
                    }
                }
                return 0;
            }
            );

            linker.DefineFunction("env", "free", (Caller caller, int ptr) =>
            {
                var memory = caller.GetMemory("memory");
                foreach (var alloc in allocations)
                {
                    if (alloc.Ptr != ptr)
                        continue;

                    if (alloc.Pref != null)
                        alloc.Pref.Next = alloc.Next;
                    if (alloc.Next != null)
                        alloc.Next.Pref = alloc.Pref;

                    allocations.Remove(alloc);
                    return;
                }
                
            }
            );
        }
    }
}
