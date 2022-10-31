using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasmtime;
using WasmLoader.Refs;

namespace WasmLoader.Refs.Wrapper
{
    internal class Logger : IRef
    {
        static MelonLogger.Instance logger = new MelonLogger.Instance("WebAssembly", ConsoleColor.DarkGreen);
        public void Setup(Dictionary<string, Action<Linker, Store, Objectstore, WasmType>> functions)
        {
            functions["UnityEngine_Debug__Log_SystemObject__SystemVoid"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "UnityEngine_Debug__Log_SystemObject__SystemVoid", (Caller caller, int ptr) =>
            {
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_Debug__Log_SystemObject__SystemVoid");
                WasmLoaderMod.Instance.LoggerInstance.Msg(objects.RetriveObject<string>(ptr, caller));
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                logger.Msg(objects.RetriveObject<string>(ptr, caller));
            });
        }
    }
}
