using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasmtime;
namespace WasmLoader.Refs
{
    public enum WasmType
    {
        World, Avatar, User, Prop
    }

    public interface IRef
    {
        void Setup(Linker linker, Store store, Objectstore objects, WasmType wasmType);
    }
}
