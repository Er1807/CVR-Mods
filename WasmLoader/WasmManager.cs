using ABI.CCK.Components;
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
                var instance = GetWasmInstance(Encoding.UTF8.GetString(Convert.FromBase64String(wasmLoader.WasmCode)), type);

                instance.gameObject = wasmLoader.gameObject;
                instance.InitMemoryManagment();
                AssignVariables(wasmLoader, instance);
                SetupGameobject(wasmLoader.gameObject, instance);
            }
            catch (Exception ex)
            {
                WasmLoaderMod.Instance.LoggerInstance.Error(ex);
            }
        }

        private WasmInstance GetWasmInstance(string wasmCode, WasmType wasmType)
        {
            var engine = new Engine();
            var module = Wasmtime.Module.FromText(engine, Guid.NewGuid().ToString(), wasmCode);
            var linker = new Linker(engine);
            var store = new Store(engine);

            var objects = new Objectstore(store);
            foreach (var import in module.Imports)
            {
                if (Functions.TryGetValue(import.Name, out var action))
                    action(linker, store, objects, wasmType);
                else if (import.Name != "WasmLoader_WasmBehavior__CurrentGameObject_this__UnityEngineGameObject")
                {
                    WasmLoaderMod.Instance.LoggerInstance.Warning($"No function found for {import.Name}");
                }
            }

            var wasminstance = new WasmInstance();

            linker.DefineFunction("env", "WasmLoader_WasmBehavior__CurrentGameObject_this__UnityEngineGameObject", (Caller caller) =>
            {
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg("WasmLoader_WasmBehavior__CurrentGameObject_this__UnityEngineGameObject");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                return objects.StoreObject(wasminstance.gameObject);
            });

            var instance = linker.Instantiate(store, module);

            objects.Counter = (int)((instance.GetMemory(store, "memory").Maximum + 1/*Just to be sure*/) * 64 * 1024); //64*1024 is 1 page

            WasmLoaderMod.Instance.LoggerInstance?.Msg("Loaded WASM");
            wasminstance.engine = engine;
            wasminstance.linker = linker;
            wasminstance.store = store;
            wasminstance.objects = objects;
            wasminstance.module = module;
            wasminstance.instance = instance;
            return wasminstance;
        }

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
            behavior.CvrInteractable = interactable;
            wasm.behavior = behavior;
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
