using System;
using Wasmtime;
namespace WasmLoader.Refs
{
    public class TestListGameobject_Ref : IRef
    {
        public void Setup(Linker linker, Store store, Objectstore objects, WasmType wasmType)
        {
            linker.DefineFunction("env", "Test_ListGameobject__Type", (Caller caller) => {
                return objects.StoreObject(typeof(TypeWrappers.ListGameobject));
            });

            linker.DefineFunction("env", "WasmLoader_TypeWrappers_ListGameobject__ctor_this__WasmLoaderTypeWrappersListGameobject", (Caller caller) => {
               
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_TypeWrappers_ListGameobject__ctor_this__WasmLoaderTypeWrappersListGameobject");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                return objects.StoreObject(new TypeWrappers.ListGameobject());

            });

            linker.DefineFunction("env", "WasmLoader_TypeWrappers_ListGameobject__Add_this_WasmLoaderTypeWrappersListGameobject_UnityEngineGameObject__SystemVoid", (Caller caller, System.Int32 parameter_this, System.Int32 obj) => {
                var resolved_this = objects.RetriveObject<TypeWrappers.ListGameobject>(parameter_this, caller);
                var resolved_obj = objects.RetriveObject<UnityEngine.GameObject>(obj, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_obj);
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_TypeWrappers_ListGameobject__Add_this_WasmLoaderTypeWrappersListGameobject_UnityEngineGameObject__SystemVoid");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                resolved_this?.Add(resolved_obj);

            });

            linker.DefineFunction("env", "WasmLoader_TypeWrappers_ListGameobject__get_Count_this_WasmLoaderTypeWrappersListGameobject__SystemInt32", (Caller caller, System.Int32 parameter_this) => {
                var resolved_this = objects.RetriveObject<TypeWrappers.ListGameobject>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_TypeWrappers_ListGameobject__get_Count_this_WasmLoaderTypeWrappersListGameobject__SystemInt32");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.Count;
                return result ?? 0;
            });
            
            linker.DefineFunction("env", "WasmLoader_TypeWrappers_ListGameobject__Get_this_WasmLoaderTypeWrappersListGameobject_SystemInt32__UnityEngineGameObject", (Caller caller, System.Int32 parameter_this, System.Int32 i) => {
                var resolved_this = objects.RetriveObject<TypeWrappers.ListGameobject>(parameter_this, caller);

#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(i);
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_TypeWrappers_ListGameobject__Get_this_WasmLoaderTypeWrappersListGameobject_SystemInt32__UnityEngineGameObject");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.Get(i);
                return objects.StoreObject(result);
            });

            linker.DefineFunction("env", "System_Object__Equals_this_SystemObject__SystemBoolean", (Caller caller, System.Int32 parameter_this, System.Int32 obj) => {
                var resolved_this = objects.RetriveObject<System.Object>(parameter_this, caller);
                var resolved_obj = objects.RetriveObject<System.Object>(obj, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_obj);
                WasmLoaderMod.Instance.LoggerInstance.Msg("System_Object__Equals_this_SystemObject__SystemBoolean");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.Equals(resolved_obj);
                return result ?? false ? 1 : 0;
            });

            linker.DefineFunction("env", "System_Object__GetHashCode_this__SystemInt32", (Caller caller, System.Int32 parameter_this) => {
                var resolved_this = objects.RetriveObject<System.Object>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg("System_Object__GetHashCode_this__SystemInt32");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.GetHashCode();
                return result ?? 0;
            });

            linker.DefineFunction("env", "System_Object__ToString_this__SystemString", (Caller caller, System.Int32 parameter_this) => {
                var resolved_this = objects.RetriveObject<System.Object>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg("System_Object__ToString_this__SystemString");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.ToString();
                return objects.StoreObject(result);
            });

        }
    }
}