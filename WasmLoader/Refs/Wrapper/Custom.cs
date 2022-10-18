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
            functions["System_Environment__get_NewLine__SystemString"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "System_Environment__get_NewLine__SystemString", (Caller caller) =>
            {
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg("System_Environment__get_NewLine__SystemString");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                return objects.StoreObject(Environment.NewLine);
            });

            functions["System_String__Type"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "System_String__Type", (Caller caller) =>
            {
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg("System_String__Type");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                return objects.StoreObject(typeof(System.String));
            });

            functions["UnityEngine_UI_Text__Type"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "UnityEngine_UI_Text__Type", (Caller caller) =>
            {
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_UI_Text__Type");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                return objects.StoreObject(typeof(UnityEngine.UI.Text));
            });

            functions["UnityEngine_UI_InputField__Type"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "UnityEngine_UI_InputField__Type", (Caller caller) =>
            {
                #if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_UI_InputField__Type");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                return objects.StoreObject(typeof(UnityEngine.UI.InputField));
            });

            functions["ABI_CCK_Components_CVRVideoPlayer__Type"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "ABI_CCK_Components_CVRVideoPlayer__Type", (Caller caller) =>
            {
                #if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg("ABI_CCK_Components_CVRVideoPlayer__Type");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                return objects.StoreObject(typeof(ABI.CCK.Components.CVRVideoPlayer));
            });


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

            functions["ABI_CCK_Components_CVRVideoPlayer__SetUrl_this_SystemString__SystemVoid"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "ABI_CCK_Components_CVRVideoPlayer__SetUrl_this_SystemString__SystemVoid", (Caller caller, int instance, int str) =>
            {
                objects.RetriveObject<ABI.CCK.Components.CVRVideoPlayer>(instance, caller)?.SetUrl(objects.RetriveObject<string>(str, caller));
            });

            functions["ABI_RC_Systems_MovementSystem_MovementSystem__Instance__ABI_RCSystemsMovementSystemMovementSystem"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "ABI_RC_Systems_MovementSystem_MovementSystem__Instance__ABI_RCSystemsMovementSystemMovementSystem", (Caller caller) =>
            {
                return objects.StoreObject(MovementSystem.Instance);
            });

            functions["UnityEngine_Vector3__ctor_SystemSingle_SystemSingle_SystemSingle__SystemVoid"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "UnityEngine_Vector3__ctor_SystemSingle_SystemSingle_SystemSingle__SystemVoid", (Caller caller, float x, float y, float z) =>
            {
                return objects.StoreObject(new Vector3(x, y, z));
            });

            functions["ABI_RC_Systems_MovementSystem_MovementSystem__TeleportTo_this_UnityEngineVector3_UnityEngineVector3_SystemBoolean__SystemVoid"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "ABI_RC_Systems_MovementSystem_MovementSystem__TeleportTo_this_UnityEngineVector3_UnityEngineVector3_SystemBoolean__SystemVoid", (Caller caller, int instance, int vector, int rot, int rotAllAxis) =>
            {
                objects.RetriveObject<MovementSystem>(instance, caller)?.TeleportTo(objects.RetriveObject<Vector3>(vector, caller), objects.RetriveObject<Vector3?>(rot, caller), rotAllAxis > 0);
            });

            functions["UnityEngine_UI_Text__set_text_this_SystemString__SystemVoid"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "UnityEngine_UI_Text__set_text_this_SystemString__SystemVoid", (Caller caller, int text, int strP) =>
            {
                var test = objects.RetriveObject<Text>(text, caller);
                var str = objects.RetriveObject<string>(strP, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(test);
                WasmLoaderMod.Instance.LoggerInstance.Msg(str);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_UI_Text__get_text_this__SystemString");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                test.text = str;
            });

            functions["UnityEngine_UI_Text__get_text_this__SystemString"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "UnityEngine_UI_Text__get_text_this__SystemString", (Caller caller, int text) =>
            {
                var test = objects.RetriveObject<Text>(text, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(test);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_UI_Text__get_text_this__SystemString");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                return objects.StoreObject(test.text);
            });

            functions["UnityEngine_UI_InputField__get_text_this__SystemString"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "UnityEngine_UI_InputField__get_text_this__SystemString", (Caller caller, int text) =>
            {
                var test = objects.RetriveObject<InputField>(text, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(test);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_UI_InputField__get_text_this__SystemString");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                return objects.StoreObject(test.text);
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
