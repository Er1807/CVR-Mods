using System;
using Wasmtime;
using System.Collections.Generic;
using WasmLoader.Refs;
namespace WasmLoader.Refs.Wrapper
{
    public class WasmLoader_TypeWrappersCVRPlayerApi_Ref : IRef
    {
        public void Setup(Dictionary<string, Action<Linker, Store, Objectstore, WasmType>> functions)
        {
            functions["WasmLoader_TypeWrappers_CVRPlayerApi__Type"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "WasmLoader_TypeWrappers_CVRPlayerApi__Type", (Caller caller) => {
                return objects.StoreObject(typeof(WasmLoader.TypeWrappers.CVRPlayerApi));
            });

            functions["WasmLoader_TypeWrappers_CVRPlayerApi__get_isLocal_this__SystemBoolean"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "WasmLoader_TypeWrappers_CVRPlayerApi__get_isLocal_this__SystemBoolean", (Caller caller, System.Int32 parameter_this) => {
                var resolved_this = objects.RetriveObject<WasmLoader.TypeWrappers.CVRPlayerApi>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_TypeWrappers_CVRPlayerApi__get_isLocal_this__SystemBoolean");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.isLocal;
                return result ?? false ? 1 : 0;
            });

            functions["WasmLoader_TypeWrappers_CVRPlayerApi__get_displayName_this_WasmLoaderTypeWrappersCVRPlayerApi__SystemString"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "WasmLoader_TypeWrappers_CVRPlayerApi__get_displayName_this_WasmLoaderTypeWrappersCVRPlayerApi__SystemString", (Caller caller, System.Int32 parameter_this) => {
                var resolved_this = objects.RetriveObject<WasmLoader.TypeWrappers.CVRPlayerApi>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_TypeWrappers_CVRPlayerApi__get_displayName_this_WasmLoaderTypeWrappersCVRPlayerApi__SystemString");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.displayName;
                return objects.StoreObject(result);
            });

            functions["WasmLoader_TypeWrappers_CVRPlayerApi__get_userId_this__SystemString"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "WasmLoader_TypeWrappers_CVRPlayerApi__get_userId_this__SystemString", (Caller caller, System.Int32 parameter_this) => {
                var resolved_this = objects.RetriveObject<WasmLoader.TypeWrappers.CVRPlayerApi>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_TypeWrappers_CVRPlayerApi__get_userId_this__SystemString");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.userId;
                return objects.StoreObject(result);
            });

            functions["WasmLoader_TypeWrappers_CVRPlayerApi__get_playerId_this__SystemInt32"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "WasmLoader_TypeWrappers_CVRPlayerApi__get_playerId_this__SystemInt32", (Caller caller, System.Int32 parameter_this) => {
                var resolved_this = objects.RetriveObject<WasmLoader.TypeWrappers.CVRPlayerApi>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_TypeWrappers_CVRPlayerApi__get_playerId_this__SystemInt32");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.playerId;
                return result;
            });

            functions["WasmLoader_TypeWrappers_CVRPlayerApi__GetPlayerById_SystemInt32__WasmLoaderTypeWrappersCVRPlayerApi"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "WasmLoader_TypeWrappers_CVRPlayerApi__GetPlayerById_SystemInt32__WasmLoaderTypeWrappersCVRPlayerApi", (Caller caller, System.Int32 playerId) => {

#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(playerId);
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_TypeWrappers_CVRPlayerApi__GetPlayerById_SystemInt32__WasmLoaderTypeWrappersCVRPlayerApi");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = WasmLoader.TypeWrappers.CVRPlayerApi.GetPlayerById(playerId);
                return objects.StoreObject(result);
            });

            functions["WasmLoader_TypeWrappers_CVRPlayerApi__GetPlayerCount__SystemInt32"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "WasmLoader_TypeWrappers_CVRPlayerApi__GetPlayerCount__SystemInt32", (Caller caller) => {
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_TypeWrappers_CVRPlayerApi__GetPlayerCount__SystemInt32");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = WasmLoader.TypeWrappers.CVRPlayerApi.GetPlayerCount();
                return result;
            });

            functions["WasmLoader_TypeWrappers_CVRPlayerApi__GetPlayers__WasmLoaderTypeWrappersListCVRPlayerApi"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "WasmLoader_TypeWrappers_CVRPlayerApi__GetPlayers__WasmLoaderTypeWrappersListCVRPlayerApi", (Caller caller) => {
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_TypeWrappers_CVRPlayerApi__GetPlayers__WasmLoaderTypeWrappersListCVRPlayerApi");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = WasmLoader.TypeWrappers.CVRPlayerApi.GetPlayers();
                return objects.StoreObject(result);
            });

            functions["WasmLoader_TypeWrappers_CVRPlayerApi__IsPlayerGrounded_this__SystemBoolean"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "WasmLoader_TypeWrappers_CVRPlayerApi__IsPlayerGrounded_this__SystemBoolean", (Caller caller, System.Int32 parameter_this) => {
                var resolved_this = objects.RetriveObject<WasmLoader.TypeWrappers.CVRPlayerApi>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_TypeWrappers_CVRPlayerApi__IsPlayerGrounded_this__SystemBoolean");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.IsPlayerGrounded();
                return result ?? false ? 1 : 0;
            });

            functions["WasmLoader_TypeWrappers_CVRPlayerApi__GetBonePosition_this_UnityEngineHumanBodyBones__UnityEngineVector3"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "WasmLoader_TypeWrappers_CVRPlayerApi__GetBonePosition_this_UnityEngineHumanBodyBones__UnityEngineVector3", (Caller caller, System.Int32 parameter_this, System.Int32 bone) => {
                var resolved_this = objects.RetriveObject<WasmLoader.TypeWrappers.CVRPlayerApi>(parameter_this, caller);
                var resolved_bone = objects.RetriveObject<UnityEngine.HumanBodyBones>(bone, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_bone);
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_TypeWrappers_CVRPlayerApi__GetBonePosition_this_UnityEngineHumanBodyBones__UnityEngineVector3");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.GetBonePosition(resolved_bone);
                return objects.StoreObject(result);
            });

            functions["WasmLoader_TypeWrappers_CVRPlayerApi__GetBoneRotation_this_UnityEngineHumanBodyBones__UnityEngineQuaternion"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "WasmLoader_TypeWrappers_CVRPlayerApi__GetBoneRotation_this_UnityEngineHumanBodyBones__UnityEngineQuaternion", (Caller caller, System.Int32 parameter_this, System.Int32 bone) => {
                var resolved_this = objects.RetriveObject<WasmLoader.TypeWrappers.CVRPlayerApi>(parameter_this, caller);
                var resolved_bone = objects.RetriveObject<UnityEngine.HumanBodyBones>(bone, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_bone);
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_TypeWrappers_CVRPlayerApi__GetBoneRotation_this_UnityEngineHumanBodyBones__UnityEngineQuaternion");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.GetBoneRotation(resolved_bone);
                return objects.StoreObject(result);
            });

            functions["WasmLoader_TypeWrappers_CVRPlayerApi__TeleportTo_this_UnityEngineVector3_UnityEngineQuaternion__SystemVoid"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "WasmLoader_TypeWrappers_CVRPlayerApi__TeleportTo_this_UnityEngineVector3_UnityEngineQuaternion__SystemVoid", (Caller caller, System.Int32 parameter_this, System.Int32 teleportPos, System.Int32 teleportRot) => {
                var resolved_this = objects.RetriveObject<WasmLoader.TypeWrappers.CVRPlayerApi>(parameter_this, caller);
                var resolved_teleportPos = objects.RetriveObject<UnityEngine.Vector3>(teleportPos, caller);
                var resolved_teleportRot = objects.RetriveObject<UnityEngine.Quaternion>(teleportRot, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_teleportPos);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_teleportRot);
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_TypeWrappers_CVRPlayerApi__TeleportTo_this_UnityEngineVector3_UnityEngineQuaternion__SystemVoid");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                resolved_this?.TeleportTo(resolved_teleportPos, resolved_teleportRot);

            });

            functions["WasmLoader_TypeWrappers_CVRPlayerApi__EnablePickup_this_SystemBoolean__SystemVoid"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "WasmLoader_TypeWrappers_CVRPlayerApi__EnablePickup_this_SystemBoolean__SystemVoid", (Caller caller, System.Int32 parameter_this, System.Int32 enable) => {
                var resolved_this = objects.RetriveObject<WasmLoader.TypeWrappers.CVRPlayerApi>(parameter_this, caller);

#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(enable);
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_TypeWrappers_CVRPlayerApi__EnablePickup_this_SystemBoolean__SystemVoid");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                resolved_this?.EnablePickup(enable > 0);

            });

            functions["WasmLoader_TypeWrappers_CVRPlayerApi__SetPlayerTag_this_SystemString_SystemString__SystemVoid"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "WasmLoader_TypeWrappers_CVRPlayerApi__SetPlayerTag_this_SystemString_SystemString__SystemVoid", (Caller caller, System.Int32 parameter_this, System.Int32 tagName, System.Int32 tagValue) => {
                var resolved_this = objects.RetriveObject<WasmLoader.TypeWrappers.CVRPlayerApi>(parameter_this, caller);
                var resolved_tagName = objects.RetriveObject<System.String>(tagName, caller);
                var resolved_tagValue = objects.RetriveObject<System.String>(tagValue, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_tagName);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_tagValue);
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_TypeWrappers_CVRPlayerApi__SetPlayerTag_this_SystemString_SystemString__SystemVoid");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                resolved_this?.SetPlayerTag(resolved_tagName, resolved_tagValue);

            });

            functions["WasmLoader_TypeWrappers_CVRPlayerApi__GetPlayerTag_this_SystemString__SystemString"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "WasmLoader_TypeWrappers_CVRPlayerApi__GetPlayerTag_this_SystemString__SystemString", (Caller caller, System.Int32 parameter_this, System.Int32 tagName) => {
                var resolved_this = objects.RetriveObject<WasmLoader.TypeWrappers.CVRPlayerApi>(parameter_this, caller);
                var resolved_tagName = objects.RetriveObject<System.String>(tagName, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_tagName);
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_TypeWrappers_CVRPlayerApi__GetPlayerTag_this_SystemString__SystemString");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.GetPlayerTag(resolved_tagName);
                return objects.StoreObject(result);
            });

            functions["WasmLoader_TypeWrappers_CVRPlayerApi__ClearPlayerTags_this__SystemVoid"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "WasmLoader_TypeWrappers_CVRPlayerApi__ClearPlayerTags_this__SystemVoid", (Caller caller, System.Int32 parameter_this) => {
                var resolved_this = objects.RetriveObject<WasmLoader.TypeWrappers.CVRPlayerApi>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_TypeWrappers_CVRPlayerApi__ClearPlayerTags_this__SystemVoid");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                resolved_this?.ClearPlayerTags();

            });

            functions["WasmLoader_TypeWrappers_CVRPlayerApi__IsUserInVR_this__SystemBoolean"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "WasmLoader_TypeWrappers_CVRPlayerApi__IsUserInVR_this__SystemBoolean", (Caller caller, System.Int32 parameter_this) => {
                var resolved_this = objects.RetriveObject<WasmLoader.TypeWrappers.CVRPlayerApi>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_TypeWrappers_CVRPlayerApi__IsUserInVR_this__SystemBoolean");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.IsUserInVR();
                return result ?? false ? 1 : 0;
            });

            functions["WasmLoader_TypeWrappers_CVRPlayerApi__Immobilize_this_SystemBoolean__SystemVoid"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "WasmLoader_TypeWrappers_CVRPlayerApi__Immobilize_this_SystemBoolean__SystemVoid", (Caller caller, System.Int32 parameter_this, System.Int32 immobile) => {
                var resolved_this = objects.RetriveObject<WasmLoader.TypeWrappers.CVRPlayerApi>(parameter_this, caller);

#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(immobile);
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_TypeWrappers_CVRPlayerApi__Immobilize_this_SystemBoolean__SystemVoid");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                resolved_this?.Immobilize(immobile > 0);

            });

            functions["WasmLoader_TypeWrappers_CVRPlayerApi__SetVelocity_this_UnityEngineVector3__SystemVoid"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "WasmLoader_TypeWrappers_CVRPlayerApi__SetVelocity_this_UnityEngineVector3__SystemVoid", (Caller caller, System.Int32 parameter_this, System.Int32 velocity) => {
                var resolved_this = objects.RetriveObject<WasmLoader.TypeWrappers.CVRPlayerApi>(parameter_this, caller);
                var resolved_velocity = objects.RetriveObject<UnityEngine.Vector3>(velocity, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_velocity);
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_TypeWrappers_CVRPlayerApi__SetVelocity_this_UnityEngineVector3__SystemVoid");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                resolved_this?.SetVelocity(resolved_velocity);

            });

            functions["WasmLoader_TypeWrappers_CVRPlayerApi__GetVelocity_this__UnityEngineVector3"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "WasmLoader_TypeWrappers_CVRPlayerApi__GetVelocity_this__UnityEngineVector3", (Caller caller, System.Int32 parameter_this) => {
                var resolved_this = objects.RetriveObject<WasmLoader.TypeWrappers.CVRPlayerApi>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_TypeWrappers_CVRPlayerApi__GetVelocity_this__UnityEngineVector3");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.GetVelocity();
                return objects.StoreObject(result);
            });

            functions["WasmLoader_TypeWrappers_CVRPlayerApi__GetPosition_this__UnityEngineVector3"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "WasmLoader_TypeWrappers_CVRPlayerApi__GetPosition_this__UnityEngineVector3", (Caller caller, System.Int32 parameter_this) => {
                var resolved_this = objects.RetriveObject<WasmLoader.TypeWrappers.CVRPlayerApi>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_TypeWrappers_CVRPlayerApi__GetPosition_this__UnityEngineVector3");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.GetPosition();
                return objects.StoreObject(result);
            });

            functions["WasmLoader_TypeWrappers_CVRPlayerApi__GetRotation_this__UnityEngineQuaternion"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "WasmLoader_TypeWrappers_CVRPlayerApi__GetRotation_this__UnityEngineQuaternion", (Caller caller, System.Int32 parameter_this) => {
                var resolved_this = objects.RetriveObject<WasmLoader.TypeWrappers.CVRPlayerApi>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_TypeWrappers_CVRPlayerApi__GetRotation_this__UnityEngineQuaternion");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.GetRotation();
                return objects.StoreObject(result);
            });

            functions["System_Object__Equals_this_SystemObject__SystemBoolean"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
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

            functions["System_Object__GetHashCode_this__SystemInt32"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "System_Object__GetHashCode_this__SystemInt32", (Caller caller, System.Int32 parameter_this) => {
                var resolved_this = objects.RetriveObject<System.Object>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg("System_Object__GetHashCode_this__SystemInt32");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.GetHashCode();
                return result;
            });

            functions["System_Object__ToString_this__SystemString"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
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