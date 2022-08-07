using MelonLoader;
using System;
using WasmLoader;
using WasmLoader.Refs;
using UnityEngine;
using Wasmtime;

[assembly: MelonInfo(typeof(WasmLoaderMod), "WasmLoader", "1.0.1", "Eric van Fandenfart")]
[assembly: MelonGame]

namespace WasmLoader
{

    public class WasmLoaderMod : MelonMod
    {
        private Engine engine;
        private Linker linker;
        private Store store;
        private Objectstore objects;
        private Module module;
        private Action onApplicationStart;
        private Action onApplicationLateStart;
        private Action onUpdate;
        private Action onLateUpdate;
        private Action teleport;
        private Action mirrottoggle1;
        private Action mirrortoggle2;
        private Action test;

        public static WasmLoaderMod Instance;

        public WasmLoaderMod()
        {
            Instance = this;
        }

        public void Setup()
        {
            if (store != null)
            {
                store.Dispose();
                store = null;
            }
            if (linker != null)
            {
                linker.Dispose();
                linker = null;
            }
            if (module != null)
            {
                module.Dispose();
                module = null;
            }
            if (engine != null)
            {
                engine.Dispose();
                engine = null;
            }
            engine = new Engine();
            module = Module.FromTextFile(engine, "memory.wat");
            linker = new Linker(engine);
            store = new Store(engine);
            objects = new Objectstore(store);
            new GameObject_Ref().Setup(linker, store, objects);
            new Log().Setup(linker, store, objects);


            var instance = linker.Instantiate(store, module);

            onUpdate = instance.GetAction(store, "OnUpdate");
            onLateUpdate = instance.GetAction(store, "OnLateUpdate");
            teleport = instance.GetAction(store, "Teleport");
            mirrottoggle1 = instance.GetAction(store, "ToggleMirror");
            mirrortoggle2 = instance.GetAction(store, "ToggleMirror2");
            test = instance.GetAction(store, "Test");

            LoggerInstance?.Msg("Loaded WASM");

        }

        public override void OnApplicationStart()
        {
            Setup();
            if (onApplicationStart is null)
            {
                //Console.WriteLine("error: onApplicationStart export is missing");
                return;
            }

            onApplicationStart();
        }


        public override void OnApplicationLateStart()
        {
            if (onApplicationLateStart is null)
            {
                //Console.WriteLine("error: onApplicationLateStart export is missing");
                return;
            }

            onApplicationLateStart();
        }

        public override void OnUpdate()
        {
            
            
            if (Input.GetKeyDown(KeyCode.P))
            {
                teleport();
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                mirrottoggle1();
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                mirrortoggle2();
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                test();
            }

        }

        public override void OnLateUpdate()
        {
            if (onLateUpdate is null)
            {
                //Console.WriteLine("error: OnLateUpdate export is missing");
                return;
            }

            onLateUpdate();
        }





    }
}