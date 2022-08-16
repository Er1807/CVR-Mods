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
            linker.DefineFunction("env", "WasmLoader_Logtest__Msg_SystemString__SystemVoid", (Caller caller, int ptr) =>
            {
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(objects.RetriveObject<string>(ptr, caller));
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_Logtest__Msg_SystemString__SystemVoid");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                logger.Msg(objects.RetriveObject<string>(ptr, caller));
            });
        }
    }
}
