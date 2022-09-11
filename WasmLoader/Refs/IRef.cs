using UnityEngine;
using WasmLoader.Components;
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

    public static class HelperFunctions
    {
        public static bool IsAllowed(this GameObject obj, WasmType type)
        {
            var res = obj.GetComponent<WasmSelectable>()?.IsAllowed(type) ?? false;
            if (!res)
                WasmLoaderMod.Instance.LoggerInstance.Msg("Unallowed gameobject " + obj.name);
            return res;
        }

        public static bool IsAllowed(this Component obj, WasmType type)
        {var res = obj.GetComponent<WasmSelectable>()?.IsAllowed(obj, type) ?? false;
            if (!res)
                WasmLoaderMod.Instance.LoggerInstance.Msg("Unallowed type "+ obj.GetType().ToString()+ obj.GetComponent<WasmSelectable>().AllowedTypes[0]);
            return res;
        }
    }
}
