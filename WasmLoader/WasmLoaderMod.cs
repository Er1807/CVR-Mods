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
        

        public WasmLoaderMod()
        {
            Instance = this;
        }
        
        public override void OnInitializeMelon()
        {
            Patches.SetupHarmony();
            var arr = (HashSet<Type>)typeof(CVRTools).GetField("componentWhiteList", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
            arr.Add(typeof(WasmLoaderBehavior));
            var arr2 = (HashSet<Type>)typeof(CVRTools).GetField("rootComponents", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
            arr2.Add(typeof(WasmLoaderBehavior));

            WasmManager.Instance.CollectFuntions();

        }
    }
}