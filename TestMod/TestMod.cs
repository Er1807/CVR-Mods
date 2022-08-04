using MelonLoader;
using System;
using Test;
using Test.Refs;
using UnityEngine;
using Wasmtime;

[assembly: MelonInfo(typeof(TestMod), "TestMod", "1.0.1", "Eric van Fandenfart")]
[assembly: MelonGame]

namespace Test
{

    public class TestMod : MelonMod
    {
        private Engine engine;
        private Linker linker;
        private Store store;
        private Module module;
        private Action onApplicationStart;
        private Action onApplicationLateStart;
        private Action onUpdate;
        private Action onLateUpdate;
        private Action eventr;
        private Action eventr2;

        public TestMod()
        {
            engine = new Engine();
            module = Module.FromTextFile(engine, "memory.wat");
            linker = new Linker(engine);
            store = new Store(engine);

            new GameObject_Ref().Setup(linker, store);
            new Log().Setup(linker, store);
            var memall = new MemoryAllocator();
            memall.Setup(linker, store);
            
            
            var instance = linker.Instantiate(store, module);
            memall.Init(instance.GetMemory(store, "memory"), store);
            
            onApplicationStart = instance.GetAction(store, "OnApplicationStart");
            onApplicationLateStart = instance.GetAction(store, "OnApplicationLateStart");
            onUpdate = instance.GetAction(store, "OnUpdate");
            onLateUpdate = instance.GetAction(store, "OnLateUpdate");
            eventr = instance.GetAction(store, "event");
            eventr2 = instance.GetAction(store, "event2");



        }

        public override void OnApplicationStart()
        {
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
                eventr();
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                eventr2();
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