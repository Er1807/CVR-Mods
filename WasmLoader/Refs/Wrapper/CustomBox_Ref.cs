﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasmtime;

namespace WasmLoader.Refs.Wrapper
{
    internal class CustoBox_Ref : IRef
    {
        public void Setup(Dictionary<string, Action<Linker, Store, Objectstore, WasmType>> functions)
        {
            functions["Box_Int"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "Box_Int", (Caller caller, int obj) =>
            {
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg("Box_Int");
                WasmLoaderMod.Instance.LoggerInstance.Msg(obj);
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                return objects.StoreObject(obj);
            });

            functions["Box_Long"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "Box_Long", (Caller caller, long obj) =>
            {
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg("Box_Longt");
                WasmLoaderMod.Instance.LoggerInstance.Msg(obj);
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                return objects.StoreObject(obj);
            });
            functions["Box_Float"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "Box_Float", (Caller caller, float obj) =>
            {
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg("Box_Float");
                WasmLoaderMod.Instance.LoggerInstance.Msg(obj);
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                return objects.StoreObject(obj);
            });
            functions["Box_Double"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "Box_Double", (Caller caller, double obj) =>
            {
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg("Box_Double");
                WasmLoaderMod.Instance.LoggerInstance.Msg(obj);
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                return objects.StoreObject(obj);
            });

            functions["Unbox_Int"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "Unbox_Int", (Caller caller, int obj) =>
            {
                var resolved_obj = objects.RetriveObject<object>(obj, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg("Unbox_Int");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_obj);
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                return (int)resolved_obj;
            });
            functions["Unbox_Long"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "Unbox_Long", (Caller caller, int obj) =>
            {
                var resolved_obj = objects.RetriveObject<object>(obj, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg("Unbox_Long");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_obj);
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                return (long)resolved_obj;
            });
            functions["Unbox_Float"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "Unbox_Float", (Caller caller, int obj) =>
            {
                var resolved_obj = objects.RetriveObject<object>(obj, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg("Unbox_Float");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_obj);
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                return (float)resolved_obj;
            });
            functions["Unbox_Double"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "Unbox_Double", (Caller caller, int obj) =>
            {
                var resolved_obj = objects.RetriveObject<object>(obj, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg("Unbox_Double");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_obj);
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                return (double)resolved_obj;
            });
            
        }
    }
}
