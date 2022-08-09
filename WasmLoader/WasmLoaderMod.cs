using MelonLoader;
using System;
using WasmLoader;
using WasmLoader.Refs;
using UnityEngine;
using Wasmtime;
using ABI.CCK.Components;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

[assembly: MelonInfo(typeof(WasmLoaderMod), "WasmLoader", "1.0.1", "Eric van Fandenfart")]
[assembly: MelonGame]

namespace WasmLoader
{

    public class WasmInstance
    {
        public Engine engine;
        public Linker linker;
        public Store store;
        public Objectstore objects;
        public Wasmtime.Module module;
        public Instance instance;
        internal WasmBehavior_Internal behavior;
    }

    public class WasmLoaderMod : MelonMod
    {
        public static WasmLoaderMod Instance;

        public Dictionary<CVRInteractable, WasmInstance> WasmInstances = new Dictionary<CVRInteractable, WasmInstance>();
        public WasmLoaderMod()
        {
            Instance = this;
            //WasmLoader.WasmLoaderMod.Instance.SetupGameobject(GameObject.Find("cvrhubfin/cvrlogo"), WasmLoader.WasmLoaderMod.Instance.GetWasmInstance(System.IO.File.ReadAllText("memory.wat")));
            
        }

        public void UnloadWasmInstance(WasmInstance instance)
        {
            instance.store.Dispose();
            instance.linker.Dispose();
            instance.module.Dispose();
            instance.engine.Dispose();
        }


        public WasmInstance GetWasmInstance(string wasmCode)
        {
            var engine = new Engine();
            var module = Wasmtime.Module.FromText(engine, Guid.NewGuid().ToString(), wasmCode);
            var linker = new Linker(engine);
            var store = new Store(engine);
            var objects = new Objectstore(store);
            new GameObject_Ref().Setup(linker, store, objects);
            new Log().Setup(linker, store, objects);

            

            var instance = linker.Instantiate(store, module);
            

            LoggerInstance?.Msg("Loaded WASM");
            return new WasmInstance()
            {
                engine = engine,
                linker = linker,
                store = store,
                objects = objects,
                module = module,
                instance = instance
            };
        }

        public void SetupGameobject(GameObject obj, WasmInstance wasm)
        {

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
        }
        
        public override void OnUpdate()
        {
            
        }
    }
}