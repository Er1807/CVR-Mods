using System;
using Wasmtime;
namespace WasmLoader.Refs
{
    public class UnityEngineGameObject_Ref : IRef
    {
        public void Setup(Linker linker, Store store, Objectstore objects, WasmType wasmType)
        {
            linker.DefineFunction("env", "UnityEngine_GameObject__Type", (Caller caller) =>
            {
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__Type");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                return objects.StoreObject(typeof(UnityEngine.GameObject));
            });

            linker.DefineFunction("env", "UnityEngine_GameObject__get_tag_this__SystemString", (Caller caller, System.Int32 parameter_this) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.GameObject>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__get_tag_this__SystemString");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.tag;
                return objects.StoreObject(result);
            });

            linker.DefineFunction("env", "UnityEngine_GameObject__set_tag_this_SystemString__SystemVoid", (Caller caller, System.Int32 parameter_this, System.Int32 value) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.GameObject>(parameter_this, caller);
                var resolved_value = objects.RetriveObject<System.String>(value, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_value);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__set_tag_this_SystemString__SystemVoid");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                resolved_this.tag = resolved_value;

            });

            linker.DefineFunction("env", "UnityEngine_GameObject__CompareTag_this_SystemString__SystemBoolean", (Caller caller, System.Int32 parameter_this, System.Int32 tag) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.GameObject>(parameter_this, caller);
                var resolved_tag = objects.RetriveObject<System.String>(tag, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_tag);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__CompareTag_this_SystemString__SystemBoolean");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.CompareTag(resolved_tag);
                return result ?? false ? 1 : 0;
            });

            linker.DefineFunction("env", "UnityEngine_GameObject__FindGameObjectWithTag_SystemString__UnityEngineGameObject", (Caller caller, System.Int32 tag) =>
            {
                var resolved_tag = objects.RetriveObject<System.String>(tag, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_tag);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__FindGameObjectWithTag_SystemString__UnityEngineGameObject");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = UnityEngine.GameObject.FindGameObjectWithTag(resolved_tag);
                if (result?.IsAllowed(wasmType) ?? false)
                    return objects.StoreObject(result);

                return objects.StoreObject(null);
            });

            linker.DefineFunction("env", "UnityEngine_GameObject__FindGameObjectsWithTag_SystemString__UnityEngineGameObject[]", (Caller caller, System.Int32 tag) =>
            {
                var resolved_tag = objects.RetriveObject<System.String>(tag, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_tag);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__FindGameObjectsWithTag_SystemString__UnityEngineGameObject[]");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = UnityEngine.GameObject.FindGameObjectsWithTag(resolved_tag);
                return objects.StoreObject(result);
            });

            linker.DefineFunction("env", "UnityEngine_GameObject__Find_SystemString__UnityEngineGameObject", (Caller caller, System.Int32 name) =>
            {
                var resolved_name = objects.RetriveObject<System.String>(name, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_name);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__Find_SystemString__UnityEngineGameObject");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = UnityEngine.GameObject.Find(resolved_name);
                if (result?.IsAllowed(wasmType) ?? false)
                    return objects.StoreObject(result);

                return objects.StoreObject(null);
            });

            linker.DefineFunction("env", "UnityEngine_GameObject__get_scene_this__UnityEngineSceneManagementScene", (Caller caller, System.Int32 parameter_this) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.GameObject>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__get_scene_this__UnityEngineSceneManagementScene");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.scene;
                return objects.StoreObject(result);
            });

            linker.DefineFunction("env", "UnityEngine_GameObject__get_sceneCullingMask_this__SystemUInt64", (Caller caller, System.Int32 parameter_this) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.GameObject>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__get_sceneCullingMask_this__SystemUInt64");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.sceneCullingMask;
                return objects.StoreObject(result);
            });

            linker.DefineFunction("env", "UnityEngine_GameObject__get_gameObject_this__UnityEngineGameObject", (Caller caller, System.Int32 parameter_this) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.GameObject>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__get_gameObject_this__UnityEngineGameObject");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.gameObject;
                return objects.StoreObject(result);
            });

            linker.DefineFunction("env", "UnityEngine_GameObject__CreatePrimitive_UnityEnginePrimitiveType__UnityEngineGameObject", (Caller caller, System.Int32 type) =>
            {
                var resolved_type = objects.RetriveObject<UnityEngine.PrimitiveType>(type, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_type);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__CreatePrimitive_UnityEnginePrimitiveType__UnityEngineGameObject");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = UnityEngine.GameObject.CreatePrimitive(resolved_type);
                return objects.StoreObject(result);
            });

            linker.DefineFunction("env", "UnityEngine_GameObject__GetComponent_this_SystemType__UnityEngineComponent", (Caller caller, System.Int32 parameter_this, System.Int32 type) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.GameObject>(parameter_this, caller);
                var resolved_type = objects.RetriveObject<System.Type>(type, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_type);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__GetComponent_this_SystemType__UnityEngineComponent");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.GetComponent(resolved_type);
                return objects.StoreObject(result);
            });

            linker.DefineFunction("env", "UnityEngine_GameObject__GetComponent_this_SystemString__UnityEngineComponent", (Caller caller, System.Int32 parameter_this, System.Int32 type) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.GameObject>(parameter_this, caller);
                var resolved_type = objects.RetriveObject<System.String>(type, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_type);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__GetComponent_this_SystemString__UnityEngineComponent");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.GetComponent(resolved_type);
                return objects.StoreObject(result);
            });

            linker.DefineFunction("env", "UnityEngine_GameObject__GetComponentInChildren_this_SystemType_SystemBoolean__UnityEngineComponent", (Caller caller, System.Int32 parameter_this, System.Int32 type, System.Int32 includeInactive) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.GameObject>(parameter_this, caller);
                var resolved_type = objects.RetriveObject<System.Type>(type, caller);

#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_type);
                WasmLoaderMod.Instance.LoggerInstance.Msg(includeInactive);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__GetComponentInChildren_this_SystemType_SystemBoolean__UnityEngineComponent");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.GetComponentInChildren(resolved_type, includeInactive > 0);
                return objects.StoreObject(result);
            });

            linker.DefineFunction("env", "UnityEngine_GameObject__GetComponentInChildren_this_SystemType__UnityEngineComponent", (Caller caller, System.Int32 parameter_this, System.Int32 type) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.GameObject>(parameter_this, caller);
                var resolved_type = objects.RetriveObject<System.Type>(type, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_type);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__GetComponentInChildren_this_SystemType__UnityEngineComponent");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.GetComponentInChildren(resolved_type);
                return objects.StoreObject(result);
            });

            linker.DefineFunction("env", "UnityEngine_GameObject__GetComponentInParent_this_SystemType__UnityEngineComponent", (Caller caller, System.Int32 parameter_this, System.Int32 type) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.GameObject>(parameter_this, caller);
                var resolved_type = objects.RetriveObject<System.Type>(type, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_type);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__GetComponentInParent_this_SystemType__UnityEngineComponent");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.GetComponentInParent(resolved_type);
                return objects.StoreObject(result);
            });

            linker.DefineFunction("env", "UnityEngine_GameObject__GetComponents_this_SystemType__UnityEngineComponent[]", (Caller caller, System.Int32 parameter_this, System.Int32 type) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.GameObject>(parameter_this, caller);
                var resolved_type = objects.RetriveObject<System.Type>(type, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_type);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__GetComponents_this_SystemType__UnityEngineComponent[]");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.GetComponents(resolved_type);
                return objects.StoreObject(result);
            });

            linker.DefineFunction("env", "UnityEngine_GameObject__GetComponentsInChildren_this_SystemType__UnityEngineComponent[]", (Caller caller, System.Int32 parameter_this, System.Int32 type) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.GameObject>(parameter_this, caller);
                var resolved_type = objects.RetriveObject<System.Type>(type, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_type);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__GetComponentsInChildren_this_SystemType__UnityEngineComponent[]");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.GetComponentsInChildren(resolved_type);
                return objects.StoreObject(result);
            });

            linker.DefineFunction("env", "UnityEngine_GameObject__GetComponentsInChildren_this_SystemType_SystemBoolean__UnityEngineComponent[]", (Caller caller, System.Int32 parameter_this, System.Int32 type, System.Int32 includeInactive) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.GameObject>(parameter_this, caller);
                var resolved_type = objects.RetriveObject<System.Type>(type, caller);

#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_type);
                WasmLoaderMod.Instance.LoggerInstance.Msg(includeInactive);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__GetComponentsInChildren_this_SystemType_SystemBoolean__UnityEngineComponent[]");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.GetComponentsInChildren(resolved_type, includeInactive > 0);
                return objects.StoreObject(result);
            });

            linker.DefineFunction("env", "UnityEngine_GameObject__GetComponentsInParent_this_SystemType__UnityEngineComponent[]", (Caller caller, System.Int32 parameter_this, System.Int32 type) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.GameObject>(parameter_this, caller);
                var resolved_type = objects.RetriveObject<System.Type>(type, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_type);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__GetComponentsInParent_this_SystemType__UnityEngineComponent[]");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.GetComponentsInParent(resolved_type);
                return objects.StoreObject(result);
            });

            linker.DefineFunction("env", "UnityEngine_GameObject__GetComponentsInParent_this_SystemType_SystemBoolean__UnityEngineComponent[]", (Caller caller, System.Int32 parameter_this, System.Int32 type, System.Int32 includeInactive) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.GameObject>(parameter_this, caller);
                var resolved_type = objects.RetriveObject<System.Type>(type, caller);

#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_type);
                WasmLoaderMod.Instance.LoggerInstance.Msg(includeInactive);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__GetComponentsInParent_this_SystemType_SystemBoolean__UnityEngineComponent[]");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.GetComponentsInParent(resolved_type, includeInactive > 0);
                return objects.StoreObject(result);
            });

            linker.DefineFunction("env", "UnityEngine_GameObject__FindWithTag_SystemString__UnityEngineGameObject", (Caller caller, System.Int32 tag) =>
            {
                var resolved_tag = objects.RetriveObject<System.String>(tag, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_tag);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__FindWithTag_SystemString__UnityEngineGameObject");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = UnityEngine.GameObject.FindWithTag(resolved_tag);
                return objects.StoreObject(result);
            });

            linker.DefineFunction("env", "UnityEngine_GameObject__SendMessageUpwards_this_SystemString_UnityEngineSendMessageOptions__SystemVoid", (Caller caller, System.Int32 parameter_this, System.Int32 methodName, System.Int32 options) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.GameObject>(parameter_this, caller);
                var resolved_methodName = objects.RetriveObject<System.String>(methodName, caller);
                var resolved_options = objects.RetriveObject<UnityEngine.SendMessageOptions>(options, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_methodName);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_options);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__SendMessageUpwards_this_SystemString_UnityEngineSendMessageOptions__SystemVoid");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                resolved_this?.SendMessageUpwards(resolved_methodName, resolved_options);

            });

            linker.DefineFunction("env", "UnityEngine_GameObject__SendMessage_this_SystemString_UnityEngineSendMessageOptions__SystemVoid", (Caller caller, System.Int32 parameter_this, System.Int32 methodName, System.Int32 options) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.GameObject>(parameter_this, caller);
                var resolved_methodName = objects.RetriveObject<System.String>(methodName, caller);
                var resolved_options = objects.RetriveObject<UnityEngine.SendMessageOptions>(options, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_methodName);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_options);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__SendMessage_this_SystemString_UnityEngineSendMessageOptions__SystemVoid");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                resolved_this?.SendMessage(resolved_methodName, resolved_options);

            });

            linker.DefineFunction("env", "UnityEngine_GameObject__BroadcastMessage_this_SystemString_UnityEngineSendMessageOptions__SystemVoid", (Caller caller, System.Int32 parameter_this, System.Int32 methodName, System.Int32 options) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.GameObject>(parameter_this, caller);
                var resolved_methodName = objects.RetriveObject<System.String>(methodName, caller);
                var resolved_options = objects.RetriveObject<UnityEngine.SendMessageOptions>(options, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_methodName);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_options);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__BroadcastMessage_this_SystemString_UnityEngineSendMessageOptions__SystemVoid");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                resolved_this?.BroadcastMessage(resolved_methodName, resolved_options);

            });

            linker.DefineFunction("env", "UnityEngine_GameObject__AddComponent_this_SystemType__UnityEngineComponent", (Caller caller, System.Int32 parameter_this, System.Int32 componentType) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.GameObject>(parameter_this, caller);
                var resolved_componentType = objects.RetriveObject<System.Type>(componentType, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_componentType);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__AddComponent_this_SystemType__UnityEngineComponent");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.AddComponent(resolved_componentType);
                return objects.StoreObject(result);
            });

            linker.DefineFunction("env", "UnityEngine_GameObject__get_transform_this__UnityEngineTransform", (Caller caller, System.Int32 parameter_this) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.GameObject>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__get_transform_this__UnityEngineTransform");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.transform;
                return objects.StoreObject(result);
            });

            linker.DefineFunction("env", "UnityEngine_GameObject__get_layer_this__SystemInt32", (Caller caller, System.Int32 parameter_this) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.GameObject>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__get_layer_this__SystemInt32");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.layer;
                return result ?? 0;
            });

            linker.DefineFunction("env", "UnityEngine_GameObject__set_layer_this_SystemInt32__SystemVoid", (Caller caller, System.Int32 parameter_this, System.Int32 value) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.GameObject>(parameter_this, caller);

#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(value);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__set_layer_this_SystemInt32__SystemVoid");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                resolved_this.layer = value;

            });

            linker.DefineFunction("env", "UnityEngine_GameObject__get_active_this__SystemBoolean", (Caller caller, System.Int32 parameter_this) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.GameObject>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__get_active_this__SystemBoolean");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.active;
                return result ?? false ? 1 : 0;
            });

            linker.DefineFunction("env", "UnityEngine_GameObject__set_active_this_SystemBoolean__SystemVoid", (Caller caller, System.Int32 parameter_this, System.Int32 value) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.GameObject>(parameter_this, caller);

#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(value);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__set_active_this_SystemBoolean__SystemVoid");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                resolved_this.active = value > 0;

            });

            linker.DefineFunction("env", "UnityEngine_GameObject__SetActive_this_SystemBoolean__SystemVoid", (Caller caller, System.Int32 parameter_this, System.Int32 value) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.GameObject>(parameter_this, caller);

#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(value);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__SetActive_this_SystemBoolean__SystemVoid");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                resolved_this?.SetActive(value > 0);

            });

            linker.DefineFunction("env", "UnityEngine_GameObject__get_activeSelf_this__SystemBoolean", (Caller caller, System.Int32 parameter_this) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.GameObject>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__get_activeSelf_this__SystemBoolean");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.activeSelf;
                return result ?? false ? 1 : 0;
            });

            linker.DefineFunction("env", "UnityEngine_GameObject__get_activeInHierarchy_this__SystemBoolean", (Caller caller, System.Int32 parameter_this) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.GameObject>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__get_activeInHierarchy_this__SystemBoolean");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.activeInHierarchy;
                return result ?? false ? 1 : 0;
            });

            linker.DefineFunction("env", "UnityEngine_GameObject__SetActiveRecursively_this_SystemBoolean__SystemVoid", (Caller caller, System.Int32 parameter_this, System.Int32 state) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.GameObject>(parameter_this, caller);

#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(state);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__SetActiveRecursively_this_SystemBoolean__SystemVoid");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                resolved_this?.SetActiveRecursively(state > 0);

            });

            linker.DefineFunction("env", "UnityEngine_GameObject__get_isStatic_this__SystemBoolean", (Caller caller, System.Int32 parameter_this) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.GameObject>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__get_isStatic_this__SystemBoolean");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.isStatic;
                return result ?? false ? 1 : 0;
            });

            linker.DefineFunction("env", "UnityEngine_GameObject__set_isStatic_this_SystemBoolean__SystemVoid", (Caller caller, System.Int32 parameter_this, System.Int32 value) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.GameObject>(parameter_this, caller);

#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(value);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_GameObject__set_isStatic_this_SystemBoolean__SystemVoid");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                resolved_this.isStatic = value > 0;

            });

            linker.DefineFunction("env", "UnityEngine_Object__GetInstanceID_this__SystemInt32", (Caller caller, System.Int32 parameter_this) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.Object>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_Object__GetInstanceID_this__SystemInt32");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.GetInstanceID();
                return result ?? 0;
            });

            linker.DefineFunction("env", "UnityEngine_Object__GetHashCode_this__SystemInt32", (Caller caller, System.Int32 parameter_this) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.Object>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_Object__GetHashCode_this__SystemInt32");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.GetHashCode();
                return result ?? 0;
            });

            linker.DefineFunction("env", "UnityEngine_Object__Equals_this_SystemObject__SystemBoolean", (Caller caller, System.Int32 parameter_this, System.Int32 other) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.Object>(parameter_this, caller);
                var resolved_other = objects.RetriveObject<System.Object>(other, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_other);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_Object__Equals_this_SystemObject__SystemBoolean");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.Equals(resolved_other);
                return result ?? false ? 1 : 0;
            });

            linker.DefineFunction("env", "UnityEngine_Object__get_name_this__SystemString", (Caller caller, System.Int32 parameter_this) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.Object>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_Object__get_name_this__SystemString");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.name;
                return objects.StoreObject(result);
            });

            linker.DefineFunction("env", "UnityEngine_Object__set_name_this_SystemString__SystemVoid", (Caller caller, System.Int32 parameter_this, System.Int32 value) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.Object>(parameter_this, caller);
                var resolved_value = objects.RetriveObject<System.String>(value, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_value);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_Object__set_name_this_SystemString__SystemVoid");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                resolved_this.name = resolved_value;

            });

            linker.DefineFunction("env", "UnityEngine_Object__get_hideFlags_this__UnityEngineHideFlags", (Caller caller, System.Int32 parameter_this) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.Object>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_Object__get_hideFlags_this__UnityEngineHideFlags");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.hideFlags;
                return objects.StoreObject(result);
            });

            linker.DefineFunction("env", "UnityEngine_Object__set_hideFlags_this_UnityEngineHideFlags__SystemVoid", (Caller caller, System.Int32 parameter_this, System.Int32 value) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.Object>(parameter_this, caller);
                var resolved_value = objects.RetriveObject<UnityEngine.HideFlags>(value, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_value);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_Object__set_hideFlags_this_UnityEngineHideFlags__SystemVoid");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                resolved_this.hideFlags = resolved_value;

            });

            linker.DefineFunction("env", "UnityEngine_Object__ToString_this__SystemString", (Caller caller, System.Int32 parameter_this) =>
            {
                var resolved_this = objects.RetriveObject<UnityEngine.Object>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg("UnityEngine_Object__ToString_this__SystemString");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = resolved_this?.ToString();
                return objects.StoreObject(result);
            });

        }
    }
}