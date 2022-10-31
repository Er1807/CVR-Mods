using System;
using System.Collections.Generic;
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
        void Setup(Dictionary<string, Action<Linker, Store, Objectstore, WasmType>> functions);
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
    }
}
