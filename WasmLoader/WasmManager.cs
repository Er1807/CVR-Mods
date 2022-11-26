using ABI.CCK.Components;
using ABI_RC.Core.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WasmLoader.Components;
using WasmLoader.Refs;
using Wasmtime;

namespace WasmLoader
{
    public class WasmManager
    {
        public static WasmManager Instance = new WasmManager();


        public Dictionary<CVRInteractable, WasmInstance> WasmInstances = new Dictionary<CVRInteractable, WasmInstance>();
        public Dictionary<string, Action<Linker, Store, Objectstore, WasmType>> Functions = new Dictionary<string, Action<Linker, Store, Objectstore, WasmType>>();

        public void CollectFuntions()
        {
            var refs = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).Where(x => typeof(IRef).IsAssignableFrom(x) && typeof(IRef) != x).ToList();
            for (int i = 0; i < refs.Count; i++)
            {
                var temp = (Activator.CreateInstance(refs[i]) as IRef);
                temp.Setup(Functions);
                WasmLoaderMod.Instance.LoggerInstance.Msg($" Loaded {temp} {i + 1} out of {refs.Count}");
            }
        }

        public void InitializeWasm(WasmLoaderBehavior wasmLoader, WasmType type)
        {

            WasmLoaderMod.Instance.LoggerInstance.Msg("LoadingPatches WasmBehavior on " + wasmLoader.gameObject.name);
            try
            {
                var instance = GetWasmInstance(wasmLoader, type);
                
                AssignVariables(wasmLoader, instance);
                SetupGameobject(wasmLoader.gameObject, instance);
            }
            catch (Exception ex)
            {
                WasmLoaderMod.Instance.LoggerInstance.Error(ex);
            }
        }

        private WasmInstance GetWasmInstance(WasmLoaderBehavior wasmLoader, WasmType wasmType)
        {
            var engine = new Engine();
            var module = Wasmtime.Module.FromText(engine, Guid.NewGuid().ToString(), Encoding.UTF8.GetString(Convert.FromBase64String(wasmLoader.WasmCode)));
            var linker = new Linker(engine);
            var store = new Store(engine);

            var objects = new Objectstore(store);
            foreach (var import in module.Imports)
            {
                if (Functions.TryGetValue(import.Name, out var action))
                    action(linker, store, objects, wasmType);
                else if (!import.Name.StartsWith("WasmLoader_WasmBehavior"))
                {
                    WasmLoaderMod.Instance.LoggerInstance.Warning($"No function found for {import.Name}");
                }
            }

            var wasminstance = new WasmInstance();
            SetupCustomFunctions(linker, wasminstance);

            var instance = linker.Instantiate(store, module);

            objects.Counter = (int)((instance.GetMemory(store, "memory").Maximum + 1/*Just to be sure*/) * 64 * 1024); //64*1024 is 1 page

            WasmLoaderMod.Instance.LoggerInstance?.Msg("Loaded WASM");
            wasminstance.behaviorExternal = wasmLoader;
            wasminstance.gameObject = wasmLoader.gameObject;
            wasminstance.engine = engine;
            wasminstance.linker = linker;
            wasminstance.store = store;
            wasminstance.objects = objects;
            wasminstance.module = module;
            wasminstance.instance = instance;

            wasminstance.InitMemoryManagment();

            return wasminstance;
        }
        #region CustomEvents
        private static void SetupCustomFunctions(Linker linker, WasmInstance wasminstance)
        {
            linker.DefineFunction("env", "WasmLoader_WasmBehavior__CurrentGameObject_this__UnityEngineGameObject", (Caller caller) =>
            {
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_WasmBehavior__CurrentGameObject_this__UnityEngineGameObject");
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                return wasminstance.objects.StoreObject(wasminstance.gameObject);
            });

            linker.DefineFunction("env", "WasmLoader_WasmBehavior__SendCustomEvent_this_SystemString__SystemVoid", (Caller caller, System.Int32 parameter_this, System.Int32 eventName) =>
            {
                var resolved_this = wasminstance.objects.RetriveObject<WasmBehavior>(parameter_this, caller);
                var resolved_eventName = wasminstance.objects.RetriveObject<string>(eventName, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_WasmBehavior__SendCustomEvent_this_SystemString__SystemVoid");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_eventName);
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                resolved_this.SendCustomEvent(resolved_eventName);
            });

            linker.DefineFunction("env", "WasmLoader_WasmBehavior__SendCustomEvent_this_SystemString_SystemObject__SystemVoid", (Caller caller, System.Int32 parameter_this, System.Int32 eventName, System.Int32 parameter) =>
            {
                var resolved_this = wasminstance.objects.RetriveObject<WasmBehavior>(parameter_this, caller);
                var resolved_eventName = wasminstance.objects.RetriveObject<string>(eventName, caller);
                var resolved_parameter = wasminstance.objects.RetriveObject<object>(parameter, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_WasmBehavior__SendCustomEvent_this_SystemString_SystemObject__SystemVoid");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_eventName);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_parameter);
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                resolved_this.SendCustomEvent(resolved_eventName, resolved_parameter);
            });

            linker.DefineFunction("env", "WasmLoader_WasmBehavior__SendCustomEvent_this_SystemString_SystemInt32__SystemVoid", (Caller caller, System.Int32 parameter_this, System.Int32 eventName, System.Int32 parameter) =>
            {
                var resolved_this = wasminstance.objects.RetriveObject<WasmBehavior>(parameter_this, caller);
                var resolved_eventName = wasminstance.objects.RetriveObject<string>(eventName, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_WasmBehavior__SendCustomEvent_this_SystemString_SystemInt32__SystemVoid");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_eventName);
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                resolved_this.SendCustomEvent(resolved_eventName, parameter);
            });

            linker.DefineFunction("env", "WasmLoader_WasmBehavior__SendCustomEvent_this_SystemString_SystemInt64__SystemVoid", (Caller caller, System.Int32 parameter_this, System.Int32 eventName, System.Int64 parameter) =>
            {
                var resolved_this = wasminstance.objects.RetriveObject<WasmBehavior>(parameter_this, caller);
                var resolved_eventName = wasminstance.objects.RetriveObject<string>(eventName, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_WasmBehavior__SendCustomEvent_this_SystemString_SystemInt64__SystemVoid");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_eventName);
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                resolved_this.SendCustomEvent(resolved_eventName, parameter);
            });

            linker.DefineFunction("env", "WasmLoader_WasmBehavior__SendCustomEvent_this_SystemString_SystemSingle__SystemVoid", (Caller caller, System.Int32 parameter_this, System.Int32 eventName, System.Single parameter) =>
            {
                var resolved_this = wasminstance.objects.RetriveObject<WasmBehavior>(parameter_this, caller);
                var resolved_eventName = wasminstance.objects.RetriveObject<string>(eventName, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_WasmBehavior__SendCustomEvent_this_SystemString_SystemSingle__SystemVoid");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_eventName);
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                resolved_this.SendCustomEvent(resolved_eventName, parameter);
            });

            linker.DefineFunction("env", "WasmLoader_WasmBehavior__SendCustomEvent_this_SystemDouble_SystemInt32__SystemVoid", (Caller caller, System.Int32 parameter_this, System.Int32 eventName, System.Double parameter) =>
            {
                var resolved_this = wasminstance.objects.RetriveObject<WasmBehavior>(parameter_this, caller);
                var resolved_eventName = wasminstance.objects.RetriveObject<string>(eventName, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_WasmBehavior__SendCustomEvent_this_SystemDouble_SystemInt32__SystemVoid");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_eventName);
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                resolved_this.SendCustomEvent(resolved_eventName, parameter);
            });

            linker.DefineFunction("env", "WasmLoader_WasmBehavior__SetProgramVariable_this_SystemString_SystemInt32__SystemVoid", (Caller caller, System.Int32 parameter_this, System.Int32 variable, System.Int32 value) =>
            {
                var resolved_this = wasminstance.objects.RetriveObject<WasmBehavior>(parameter_this, caller);
                var resolved_variable = wasminstance.objects.RetriveObject<string>(variable, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_WasmBehavior__SetProgramVariable_this_SystemString_SystemInt32__SystemVoid");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_variable);
                WasmLoaderMod.Instance.LoggerInstance.Msg(value);
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                resolved_this.SetProgramVariable(resolved_variable, value);
            });

            linker.DefineFunction("env", "WasmLoader_WasmBehavior__SetProgramVariable_this_SystemString_SystemInt64__SystemVoid", (Caller caller, System.Int32 parameter_this, System.Int32 variable, System.Int64 value) =>
            {
                var resolved_this = wasminstance.objects.RetriveObject<WasmBehavior>(parameter_this, caller);
                var resolved_variable = wasminstance.objects.RetriveObject<string>(variable, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_WasmBehavior__SetProgramVariable_this_SystemString_SystemInt64__SystemVoid");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_variable);
                WasmLoaderMod.Instance.LoggerInstance.Msg(value);
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                resolved_this.SetProgramVariable(resolved_variable, value);
            });

            linker.DefineFunction("env", "WasmLoader_WasmBehavior__SetProgramVariable_this_SystemString_SystemSingle__SystemVoid", (Caller caller, System.Int32 parameter_this, System.Int32 variable, System.Single value) =>
            {
                var resolved_this = wasminstance.objects.RetriveObject<WasmBehavior>(parameter_this, caller);
                var resolved_variable = wasminstance.objects.RetriveObject<string>(variable, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_WasmBehavior__SetProgramVariable_this_SystemString_SystemSingle__SystemVoid");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_variable);
                WasmLoaderMod.Instance.LoggerInstance.Msg(value);
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                resolved_this.SetProgramVariable(resolved_variable, value);
            });

            linker.DefineFunction("env", "WasmLoader_WasmBehavior__SetProgramVariable_this_SystemString_SystemDouble__SystemVoid", (Caller caller, System.Int32 parameter_this, System.Int32 variable, System.Double value) =>
            {
                var resolved_this = wasminstance.objects.RetriveObject<WasmBehavior>(parameter_this, caller);
                var resolved_variable = wasminstance.objects.RetriveObject<string>(variable, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_WasmBehavior__SetProgramVariable_this_SystemString_SystemDouble__SystemVoid");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_variable);
                WasmLoaderMod.Instance.LoggerInstance.Msg(value);
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                resolved_this.SetProgramVariable(resolved_variable, value);
            });

            linker.DefineFunction("env", "WasmLoader_WasmBehavior__SetProgramVariable_this_SystemString_SystemObject__SystemVoid", (Caller caller, System.Int32 parameter_this, System.Int32 variable, System.Int32 value) =>
            {
                var resolved_this = wasminstance.objects.RetriveObject<WasmBehavior>(parameter_this, caller);
                var resolved_variable = wasminstance.objects.RetriveObject<string>(variable, caller);
                var resolved_value = wasminstance.objects.RetriveObject<object>(value, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_WasmBehavior__SetProgramVariable_this_SystemString_SystemDouble__SystemVoid");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_variable);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_value);
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                resolved_this.SetProgramVariable(resolved_variable, value);
            });

            linker.DefineFunction("env", "WasmLoader_WasmBehavior__GetProgramVariableInt_this_SystemString__SystemInt32", (Caller caller, System.Int32 parameter_this, System.Int32 variable) =>
            {
                var resolved_this = wasminstance.objects.RetriveObject<WasmBehavior>(parameter_this, caller);
                var resolved_variable = wasminstance.objects.RetriveObject<string>(variable, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_WasmBehavior__GetProgramVariableInt_this_SystemString__SystemInt32");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_variable);
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                return resolved_this.GetProgramVariableInt(resolved_variable);
            });

            linker.DefineFunction("env", "WasmLoader_WasmBehavior__GetProgramVariableLong_this_SystemString__SystemInt64", (Caller caller, System.Int32 parameter_this, System.Int32 variable) =>
            {
                var resolved_this = wasminstance.objects.RetriveObject<WasmBehavior>(parameter_this, caller);
                var resolved_variable = wasminstance.objects.RetriveObject<string>(variable, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_WasmBehavior__GetProgramVariableLong_this_SystemString__SystemInt64");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_variable);
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                return resolved_this.GetProgramVariableLong(resolved_variable);
            });

            linker.DefineFunction("env", "WasmLoader_WasmBehavior__GetProgramVariableFloat_this_SystemString__SystemSingle", (Caller caller, System.Int32 parameter_this, System.Int32 variable) =>
            {
                var resolved_this = wasminstance.objects.RetriveObject<WasmBehavior>(parameter_this, caller);
                var resolved_variable = wasminstance.objects.RetriveObject<string>(variable, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_WasmBehavior__GetProgramVariableFloat_this_SystemString__SystemSingle");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_variable);
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                return resolved_this.GetProgramVariableFloat(resolved_variable);
            });

            linker.DefineFunction("env", "WasmLoader_WasmBehavior__GetProgramVariableDouble_this_SystemString__SystemDouble", (Caller caller, System.Int32 parameter_this, System.Int32 variable) =>
            {
                var resolved_this = wasminstance.objects.RetriveObject<WasmBehavior>(parameter_this, caller);
                var resolved_variable = wasminstance.objects.RetriveObject<string>(variable, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_WasmBehavior__GetProgramVariableDouble_this_SystemString__SystemDouble");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_variable);
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                return resolved_this.GetProgramVariableDouble(resolved_variable);
            });

            linker.DefineFunction("env", "WasmLoader_WasmBehavior__GetProgramVariableObject_this_SystemString__SystemDouble", (Caller caller, System.Int32 parameter_this, System.Int32 variable) =>
            {
                var resolved_this = wasminstance.objects.RetriveObject<WasmBehavior>(parameter_this, caller);
                var resolved_variable = wasminstance.objects.RetriveObject<string>(variable, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_WasmBehavior__GetProgramVariableDouble_this_SystemString__SystemDouble");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_variable);
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                return (int) resolved_this.GetProgramVariableObject(resolved_variable);
            });

            

            linker.DefineFunction("env", "WasmLoader_WasmBehavior__Type", (Caller caller) =>
            {
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_WasmBehavior__Type");
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                return wasminstance.objects.StoreObject(typeof(WasmBehavior));
            });
        }
        #endregion

        private void SetupGameobject(GameObject obj, WasmInstance wasm)
        {
            var interactable = obj.AddComponent<CVRInteractable>();
            if (wasm.module.Exports.Any(x => x.Name == "Grab"))
                interactable.actions.Add(new CVRInteractableAction() { actionType = CVRInteractableAction.ActionRegister.OnGrab });
            if (wasm.module.Exports.Any(x => x.Name == "Drop"))
                interactable.actions.Add(new CVRInteractableAction() { actionType = CVRInteractableAction.ActionRegister.OnDrop });
            if (wasm.module.Exports.Any(x => x.Name == "InteractUp"))
                interactable.actions.Add(new CVRInteractableAction() { actionType = CVRInteractableAction.ActionRegister.OnInteractUp });
            if (wasm.module.Exports.Any(x => x.Name == "InteractDown"))
                interactable.actions.Add(new CVRInteractableAction() { actionType = CVRInteractableAction.ActionRegister.OnInteractDown });

            var behavior = obj.AddComponent<WasmBehavior_Internal>();
            behavior.Instance = wasm;
            wasm.behaviorExternal._internal = behavior;
            behavior.CvrInteractable = interactable;
            wasm.behaviorInternal = behavior;
            WasmManager.Instance.WasmInstances.Add(interactable, wasm);
        }



        private static void AssignVariables(WasmLoaderBehavior wasmLoader, WasmInstance instance)
        {
            try
            {
                var variableDefinitions = JsonConvert.DeserializeObject<List<VariableDefinition>>(wasmLoader.Variables);
                instance.variables = variableDefinitions;
            }
            catch (Exception)
            {
                instance.variables = new List<VariableDefinition>();
            }


            foreach (var variableDefinition in instance.variables)
            {
                try
                {
                    switch (variableDefinition.VariableType)
                    {
                        case "UnityEngine.GameObject":
                            SetVariable(instance, variableDefinition, wasmLoader.AttributesGameObject);
                            break;
                        case "UnityEngine.UI.Text":
                            SetVariable(instance, variableDefinition, wasmLoader.AttributesText);
                            break;
                        case "System.String":
                            SetVariable(instance, variableDefinition, wasmLoader.AttributesString);
                            break;
                        case "UnityEngine.Transform":
                            SetVariable(instance, variableDefinition, wasmLoader.AttributesTransform);
                            break;
                        case "System.Int32":
                            SetVariable(instance, variableDefinition, wasmLoader.AttributesInt);
                            break;
                        case "System.Boolean":
                            SetVariable(instance, variableDefinition, wasmLoader.AttributesBool);
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception)
                {//ignore
                }
            }
        }

        private static void SetVariable(WasmInstance instance, VariableDefinition variableDefinition, Array source)
        {
            var type = Type.GetType(variableDefinition.VariableType) ?? typeof(object);
            if (variableDefinition.IsArray)
            {
                Array array;
                if (type.IsValueType)
                {
                    array = Array.CreateInstance(type, variableDefinition.ArrayCount);
                }
                else
                {
                    array = Array.CreateInstance(typeof(object), variableDefinition.ArrayCount);
                }
                
                for (var i = 0; i < variableDefinition.ArrayCount; i++)
                    array.SetValue(source.GetValue(variableDefinition.StartIndex + i), i);

                instance.instance.GetGlobal(instance.store, variableDefinition.VariableName)?.
                    SetValue(instance.store, instance.objects.StoreObject(array));
            }
            else
            {
                object objectToStore = source.GetValue(variableDefinition.StartIndex);
                if (!type.IsValueType)
                    objectToStore = instance.objects.StoreObject(objectToStore);
                
                instance.instance.GetGlobal(instance.store, variableDefinition.VariableName)?.
                    SetValue(instance.store, objectToStore);
            }
        }

    }
}
