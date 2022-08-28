using UnityEngine;
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
            return obj.GetComponent<WasmSelectable>()?.IsAllowed(type) ?? false;
        }

        public static bool IsAllowed(this Component obj, WasmType type)
        {
            return obj.GetComponent<WasmSelectable>()?.IsAllowed(obj, type) ?? false;
        }
    }
}
