using System;
using System.Collections.Generic;
using System.Linq;
using Wasmtime;
namespace WasmLoader.Refs.Wrapper
{
    public class UnityEngineGameObject_Ref : IRef
    {
        public void Setup(Dictionary<string, Action<Linker, Store, Objectstore, WasmType>> functions)
        {
            functions["UnityEngine_GameObject__FindGameObjectWithTag_SystemString__UnityEngineGameObject"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "UnityEngine_GameObject__FindGameObjectWithTag_SystemString__UnityEngineGameObject", (Caller caller, System.Int32 tag) =>
            {
                var resolved_tag = objects.RetriveObject<System.String>(tag, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_tag);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__FindGameObjectWithTag_SystemString__UnityEngineGameObject");
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                var result = UnityEngine.GameObject.FindGameObjectWithTag(resolved_tag);
                if (result?.IsAllowed(wasmType) ?? false)
                    return objects.StoreObject(result);

                return objects.StoreObject(null);
            });

            functions["UnityEngine_GameObject__FindGameObjectsWithTag_SystemString__UnityEngineGameObject"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "UnityEngine_GameObject__FindGameObjectsWithTag_SystemString__UnityEngineGameObject[]", (Caller caller, System.Int32 tag) =>
            {
                var resolved_tag = objects.RetriveObject<System.String>(tag, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_tag);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__FindGameObjectsWithTag_SystemString__UnityEngineGameObject[]");
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                var result = UnityEngine.GameObject.FindGameObjectsWithTag(resolved_tag);
                result = result.Where(x => x.IsAllowed(wasmType)).ToArray();
                return objects.StoreObject(result);
            });

            functions["UnityEngine_GameObject__Find_SystemString__UnityEngineGameObject"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "UnityEngine_GameObject__Find_SystemString__UnityEngineGameObject", (Caller caller, System.Int32 name) =>
            {
                var resolved_name = objects.RetriveObject<System.String>(name, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_name);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__Find_SystemString__UnityEngineGameObject");
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                var result = UnityEngine.GameObject.Find(resolved_name);
                if (result?.IsAllowed(wasmType) ?? false)
                    return objects.StoreObject(result);

                return objects.StoreObject(null);
            });
            
            functions["UnityEngine_GameObject__FindWithTag_SystemString__UnityEngineGameObject"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "UnityEngine_GameObject__FindWithTag_SystemString__UnityEngineGameObject", (Caller caller, System.Int32 tag) =>
            {
                var resolved_tag = objects.RetriveObject<System.String>(tag, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__FindWithTag_SystemString__UnityEngineGameObject");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_tag);
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                var result = UnityEngine.GameObject.FindWithTag(resolved_tag);
                if (result?.IsAllowed(wasmType) ?? false)
                    return objects.StoreObject(result);

                return objects.StoreObject(null);
            });
        }
    }
}