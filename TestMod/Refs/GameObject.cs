using ABI_RC.Systems.MovementSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Wasmtime;

namespace Test.Refs
{
    public class GameObject_Ref : IRef
    {
        public void Setup(Linker linker, Store store)
        {
            linker.DefineFunction("GameObject", "SetActive", (Caller caller, object obj, int value) =>
            {
                (obj as GameObject)?.SetActive(value > 0);
            });

            linker.DefineFunction("GameObject", "IsActive", (Caller caller, object obj) =>
            {
                var t = (obj as GameObject)?.activeInHierarchy;
                if(t.HasValue)
                    return t.Value ? 1 : 0;
                return 0;
            });

            linker.DefineFunction("GameObject", "GetGameobjectByPath", (Caller caller, int ptr) =>
            {
                var str = caller.GetMemory("memory").ReadNullTerminatedString(store, ptr);
                return (object) GameObject.Find(str);
            });

            linker.DefineFunction("GameObject", "transform", (Caller caller, object obj) =>
            {
                return (object) (obj as GameObject)?.transform;
            });


            linker.DefineFunction("MovementSystem", "setPos", (Caller caller, int x, int y, int z) =>
            {
                MovementSystem.Instance.TeleportTo(new Vector3(x, y, z));
            });
        }
    }
}
