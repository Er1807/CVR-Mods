using ABI_RC.Core.Player;
using ABI_RC.Systems.MovementSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Wasmtime;

namespace WasmLoader.Refs.Wrapper
{
    public class Custom_Ref : IRef
    {
        public void Setup(Dictionary<string, Action<Linker, Store, Objectstore, WasmType>> functions)
        {

            functions["System_Object__GetType_this__SystemType"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "System_Object__GetType_this__SystemType", (Caller caller, System.Int32 parameter_this) =>
            {
                var resolved_this = objects.RetriveObject<System.Object>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg("System_Object__GetType_this__SystemType");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.GetType();
                return objects.StoreObject(result);
            });
            
            functions["ABI_RC_Systems_MovementSystem_MovementSystem__Instance__ABI_RCSystemsMovementSystemMovementSystem"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "ABI_RC_Systems_MovementSystem_MovementSystem__Instance__ABI_RCSystemsMovementSystemMovementSystem", (Caller caller) =>
            {
                return objects.StoreObject(MovementSystem.Instance);
            });

            functions["ABI_RC_Systems_MovementSystem_MovementSystem__TeleportTo_this_UnityEngineVector3_UnityEngineVector3_SystemBoolean__SystemVoid"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "ABI_RC_Systems_MovementSystem_MovementSystem__TeleportTo_this_UnityEngineVector3_UnityEngineVector3_SystemBoolean__SystemVoid", (Caller caller, int instance, int vector, int rot, int rotAllAxis) =>
            {
                objects.RetriveObject<MovementSystem>(instance, caller)?.TeleportTo(objects.RetriveObject<Vector3>(vector, caller), objects.RetriveObject<Vector3?>(rot, caller), rotAllAxis > 0);
            });
            
            
            functions["ABI_RC_Core_Player_CVRPlayerEntity__Username__SystemString"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "ABI_RC_Core_Player_CVRPlayerEntity__Username__SystemString", (Caller caller, int player) =>
            {

                var test = objects.RetriveObject<CVRPlayerEntity>(player, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg(test);
                WasmLoaderMod.Instance.LoggerInstance.Msg("ABI_RC_Core_Player_CVRPlayerEntity__Username__SystemString");
#endif

                return objects.StoreObject(test?.Username);
            });
        }
    }
}
