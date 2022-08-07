using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasmtime;

namespace WasmLoader.Refs
{
    internal class Log : IRef
    {
        static MelonLogger.Instance logger = new MelonLogger.Instance("WebAssembly", ConsoleColor.DarkGreen);
        public void Setup(Linker linker, Store store, Objectstore objects)
        {
            linker.DefineFunction("env", "Test_Logtest__Msg_SystemString__SystemVoid", (Caller caller, int ptr) =>
            {
                logger.Msg(caller.GetMemory("memory").ReadNullTerminatedString(store, ptr));
            });
        }
    }
}
