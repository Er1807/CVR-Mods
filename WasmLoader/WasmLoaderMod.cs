using MelonLoader;
using System;
using WasmLoader;
using WasmLoader.Refs;
using UnityEngine;
using Wasmtime;
using ABI.CCK.Components;
using System.Collections.Generic;
using System.Reflection;
using ABI_RC.Core;
using System.Linq;
using WasmLoader.Components;

[assembly: MelonInfo(typeof(WasmLoaderMod), "WasmLoader", "1.0.1", "Eric van Fandenfart")]
[assembly: MelonGame]

namespace WasmLoader
{

    public class WasmLoaderMod : MelonMod
    {
        public static WasmLoaderMod Instance;
        public Dictionary<string, Action<Linker, Store, Objectstore, WasmType>> Functions = new Dictionary<string, Action<Linker, Store, Objectstore, WasmType>>();


        public Dictionary<CVRInteractable, WasmInstance> WasmInstances = new Dictionary<CVRInteractable, WasmInstance>();
        public WasmLoaderMod()
        {
            Instance = this;
        }

        public void UnloadWasmInstance(WasmInstance instance)
        {
            instance.store.Dispose();
            instance.linker.Dispose();
            instance.module.Dispose();
            instance.engine.Dispose();
        }


        public WasmInstance GetWasmInstance(string wasmCode, WasmType wasmType)
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
                    LoggerInstance.Warning($"No function found for {import.Name}");
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


            LoggerInstance?.Msg("Loaded WASM");
            wasminstance.engine = engine;
            wasminstance.linker = linker;
            wasminstance.store = store;
            wasminstance.objects = objects;
            wasminstance.module = module;
            wasminstance.instance = instance;
            return wasminstance;
        }

        public void ClearInstances()
        {
            foreach (var item in WasmInstances.ToArray())
            {
                if ((item.Key == null && !ReferenceEquals(item.Key, null))
                    || (item.Key.gameObject == null && !ReferenceEquals(item.Key.gameObject, null)))
                {
                    WasmInstances.Remove(item.Key);
                    UnloadWasmInstance(item.Value);
                }
            }
        }

        public void SetupGameobject(GameObject obj, WasmInstance wasm)
        {

            ClearInstances();

            var interactable = obj.AddComponent<CVRInteractable>();
            interactable.actions.Add(new CVRInteractableAction() { actionType = CVRInteractableAction.ActionRegister.OnGrab });
            interactable.actions.Add(new CVRInteractableAction() { actionType = CVRInteractableAction.ActionRegister.OnDrop });
            interactable.actions.Add(new CVRInteractableAction() { actionType = CVRInteractableAction.ActionRegister.OnInteractUp });
            interactable.actions.Add(new CVRInteractableAction() { actionType = CVRInteractableAction.ActionRegister.OnInteractDown });
            var behavior = obj.AddComponent<WasmBehavior_Internal>();
            behavior.Instance = wasm;
            wasm.behavior = behavior;
            WasmInstances.Add(interactable, wasm);
        }

        public override void OnApplicationStart()
        {
            Patches.SetupHarmony();
            var arr = (HashSet<Type>)typeof(CVRTools).GetField("componentWhiteList", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
            arr.Add(typeof(WasmLoaderBehavior));
            var arr2 = (HashSet<Type>)typeof(CVRTools).GetField("rootComponents", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
            arr2.Add(typeof(WasmLoaderBehavior));

            var refs = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).Where(x => typeof(IRef).IsAssignableFrom(x) && typeof(IRef) != x).ToList();
            for (int i = 0; i < refs.Count; i++)
            {
                var temp = (Activator.CreateInstance(refs[i]) as IRef);
                temp.Setup(Functions);
                LoggerInstance.Msg($" Loaded {temp} {i+1} out of {refs.Count}");
            }

        }
    }
}