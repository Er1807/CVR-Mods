using System;
using System.Collections.Generic;
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
            
            functions["UnityEngine_GameObject__CreatePrimitive_UnityEnginePrimitiveType__UnityEngineGameObject"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "UnityEngine_GameObject__CreatePrimitive_UnityEnginePrimitiveType__UnityEngineGameObject", (Caller caller, System.Int32 type) =>
            {
                var resolved_type = objects.RetriveObject<UnityEngine.PrimitiveType>(type, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_type);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__CreatePrimitive_UnityEnginePrimitiveType__UnityEngineGameObject");
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                var result = UnityEngine.GameObject.CreatePrimitive(resolved_type);
                return objects.StoreObject(result);
            });

            functions["UnityEngine_GameObject__GetComponent_this_SystemType__UnityEngineComponent"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "UnityEngine_GameObject__GetComponent_this_SystemType__UnityEngineComponent", (Caller caller, System.Int32 parameter_this, System.Int32 type) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.GameObject>(parameter_this, caller);
                var resolved_type = objects.RetriveObject<System.Type>(type, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_type);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__GetComponent_this_SystemType__UnityEngineComponent");
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                var result = resolved_this?.GetComponent(resolved_type);
                if (result?.IsAllowed(wasmType) ?? false)
                    return objects.StoreObject(result);

                return objects.StoreObject(null);
            });

            functions["UnityEngine_GameObject__GetComponent_this_SystemString__UnityEngineComponent"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "UnityEngine_GameObject__GetComponent_this_SystemString__UnityEngineComponent", (Caller caller, System.Int32 parameter_this, System.Int32 type) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.GameObject>(parameter_this, caller);
                var resolved_type = objects.RetriveObject<System.String>(type, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_type);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__GetComponent_this_SystemString__UnityEngineComponent");
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                var result = resolved_this?.GetComponent(resolved_type);
                if (result?.IsAllowed(wasmType) ?? false)
                    return objects.StoreObject(result);

                return objects.StoreObject(null);
            });

            functions["UnityEngine_GameObject__GetComponentInChildren_this_SystemType_SystemBoolean__UnityEngineComponent"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "UnityEngine_GameObject__GetComponentInChildren_this_SystemType_SystemBoolean__UnityEngineComponent", (Caller caller, System.Int32 parameter_this, System.Int32 type, System.Int32 includeInactive) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.GameObject>(parameter_this, caller);
                var resolved_type = objects.RetriveObject<System.Type>(type, caller);

#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_type);
                WasmLoaderMod.Instance.LoggerInstance.Msg(includeInactive);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__GetComponentInChildren_this_SystemType_SystemBoolean__UnityEngineComponent");
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                var result = resolved_this?.GetComponentInChildren(resolved_type, includeInactive > 0);
                if (result?.IsAllowed(wasmType) ?? false)
                    return objects.StoreObject(result);

                return objects.StoreObject(null);
            });

            functions["UnityEngine_GameObject__GetComponentInChildren_this_SystemType__UnityEngineComponent"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "UnityEngine_GameObject__GetComponentInChildren_this_SystemType__UnityEngineComponent", (Caller caller, System.Int32 parameter_this, System.Int32 type) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.GameObject>(parameter_this, caller);
                var resolved_type = objects.RetriveObject<System.Type>(type, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_type);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__GetComponentInChildren_this_SystemType__UnityEngineComponent");
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                var result = resolved_this?.GetComponentInChildren(resolved_type);
                if (result?.IsAllowed(wasmType) ?? false)
                    return objects.StoreObject(result);

                return objects.StoreObject(null);
            });


            functions["UnityEngine_GameObject__GetComponents_this_SystemType__UnityEngineComponent"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "UnityEngine_GameObject__GetComponents_this_SystemType__UnityEngineComponent[]", (Caller caller, System.Int32 parameter_this, System.Int32 type) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.GameObject>(parameter_this, caller);
                var resolved_type = objects.RetriveObject<System.Type>(type, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_type);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__GetComponents_this_SystemType__UnityEngineComponent[]");
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                var result = resolved_this?.GetComponents(resolved_type);
                return objects.StoreObject(result);
            });

            functions["UnityEngine_GameObject__GetComponentsInChildren_this_SystemType__UnityEngineComponent"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "UnityEngine_GameObject__GetComponentsInChildren_this_SystemType__UnityEngineComponent[]", (Caller caller, System.Int32 parameter_this, System.Int32 type) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.GameObject>(parameter_this, caller);
                var resolved_type = objects.RetriveObject<System.Type>(type, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_type);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__GetComponentsInChildren_this_SystemType__UnityEngineComponent[]");
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                var result = resolved_this?.GetComponentsInChildren(resolved_type);
                return objects.StoreObject(result);
            });

            functions["UnityEngine_GameObject__GetComponentsInChildren_this_SystemType_SystemBoolean__UnityEngineComponent"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "UnityEngine_GameObject__GetComponentsInChildren_this_SystemType_SystemBoolean__UnityEngineComponent[]", (Caller caller, System.Int32 parameter_this, System.Int32 type, System.Int32 includeInactive) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.GameObject>(parameter_this, caller);
                var resolved_type = objects.RetriveObject<System.Type>(type, caller);

#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_type);
                WasmLoaderMod.Instance.LoggerInstance.Msg(includeInactive);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__GetComponentsInChildren_this_SystemType_SystemBoolean__UnityEngineComponent[]");
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                var result = resolved_this?.GetComponentsInChildren(resolved_type, includeInactive > 0);
                return objects.StoreObject(result);
            });


            functions["UnityEngine_GameObject__GetComponentsInParent_this_SystemType_SystemBoolean__UnityEngineComponent"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "UnityEngine_GameObject__GetComponentsInParent_this_SystemType_SystemBoolean__UnityEngineComponent[]", (Caller caller, System.Int32 parameter_this, System.Int32 type, System.Int32 includeInactive) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.GameObject>(parameter_this, caller);
                var resolved_type = objects.RetriveObject<System.Type>(type, caller);

#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_type);
                WasmLoaderMod.Instance.LoggerInstance.Msg(includeInactive);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__GetComponentsInParent_this_SystemType_SystemBoolean__UnityEngineComponent[]");
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                var result = resolved_this?.GetComponentsInParent(resolved_type, includeInactive > 0);
                return objects.StoreObject(result);
            });

            functions["UnityEngine_GameObject__FindWithTag_SystemString__UnityEngineGameObject"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "UnityEngine_GameObject__FindWithTag_SystemString__UnityEngineGameObject", (Caller caller, System.Int32 tag) =>
            {
                var resolved_tag = objects.RetriveObject<System.String>(tag, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_tag);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__FindWithTag_SystemString__UnityEngineGameObject");
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                var result = UnityEngine.GameObject.FindWithTag(resolved_tag);
                if (result?.IsAllowed(wasmType) ?? false)
                    return objects.StoreObject(result);

                return objects.StoreObject(null);
            });


            functions["UnityEngine_GameObject__AddComponent_this_SystemType__UnityEngineComponent"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "UnityEngine_GameObject__AddComponent_this_SystemType__UnityEngineComponent", (Caller caller, System.Int32 parameter_this, System.Int32 componentType) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.GameObject>(parameter_this, caller);
                var resolved_componentType = objects.RetriveObject<System.Type>(componentType, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_componentType);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__AddComponent_this_SystemType__UnityEngineComponent");
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                var result = resolved_this?.AddComponent(resolved_componentType);
                return objects.StoreObject(result);
            });
        }
    }
}