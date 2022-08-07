using ABI_RC.Systems.MovementSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Wasmtime;

namespace WasmLoader.Refs
{
    public class GameObject_Ref : IRef
    {
        public void Setup(Linker linker, Store store, Objectstore objects)
        {
            linker.DefineFunction("env", "UnityEngine_GameObject__SetActive_this_SystemBoolean__SystemVoid", (Caller caller, int obj, int value) =>
            {
                objects.RetriveObject<GameObject>(obj)?.SetActive(value > 0);
            });

            linker.DefineFunction("env", "UnityEngine_GameObject__get_activeSelf_this__SystemBoolean", (Caller caller, int obj) =>
            {
                return objects.RetriveObject<GameObject>(obj)?.activeSelf ?? false ? 1 : 0;
            });

            linker.DefineFunction("env", "UnityEngine_GameObject__Find_SystemString__UnityEngineGameObject", (Caller caller, int ptr) =>
            {
                var str = caller.GetMemory("memory").ReadNullTerminatedString(store, ptr);
                return objects.StoreObject(GameObject.Find(str));
            });

            linker.DefineFunction("env", "transform", (Caller caller, int obj) =>
            {
                return objects.StoreObject(objects.RetriveObject<GameObject>(obj)?.transform);
            });

            linker.DefineFunction("env", "ABI_RC_Systems_MovementSystem_MovementSystem__Instance__ABI_RCSystemsMovementSystemMovementSystem", (Caller caller) =>
            {
                return objects.StoreObject(MovementSystem.Instance);
            });

            linker.DefineFunction("env", "UnityEngine_Vector3__ctor_SystemSingle_SystemSingle_SystemSingle__SystemVoid", (Caller caller, float x, float y, float z) =>
            {
                return objects.StoreObject(new Vector3(x, y, z));
            });

            linker.DefineFunction("env", "ABI_RC_Systems_MovementSystem_MovementSystem__TeleportTo_this_UnityEngineVector3_UnityEngineVector3_SystemBoolean__SystemVoid", (Caller caller, int instance, int vector, int rot, int rotAllAxis) =>
            {
                objects.RetriveObject<MovementSystem>(instance)?.TeleportTo(objects.RetriveObject<Vector3>(vector), objects.RetriveObject<Vector3?>(rot), rotAllAxis > 0);
            });
        }
    }
}
