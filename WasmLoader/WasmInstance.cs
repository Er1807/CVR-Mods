using UnityEngine;
using Wasmtime;
using System.Collections.Generic;
using System.Linq;
using WasmLoader.Serialisation;
using WasmLoader.Components;
using System.Runtime.Remoting.Messaging;

namespace WasmLoader
{
    public class WasmInstance
    {
        public Engine engine;
        public Linker linker;
        public Store store;
        public Objectstore objects;
        public Module module;
        public Instance instance;
        public WasmBehavior_Internal behavior;
        public GameObject gameObject;
        public Dictionary<string, Global> exports = new Dictionary<string, Global>();
        public List<SynchronizedVariable> synchronizedVariables = new List<SynchronizedVariable>();
        public List<VariableDefinition> variables = new List<VariableDefinition>();

        public void InitMemoryManagment()
        {
            foreach (var export in module.Exports)
            {
                var global = instance.GetGlobal(store, export.Name);
                if (global != null && global.Kind == ValueKind.Int32)
                    exports.Add(export.Name, global);
            }
            //WasmLoaderMod.Instance.LoggerInstance.Msg("Got globals " + exports);
        }

        public void CleanUpLocals()
        {
            foreach (var key in objects.objects.Keys.ToList())
            {
                if (key == objects.NullCounter)
                    continue;
                if (!exports.Values.Any(x => key.Equals(x.GetValue(store))))
                {
                    objects.objects.Remove(key);
                    //WasmLoaderMod.Instance.LoggerInstance.Msg("Deleting " + key);
                }
            }
        }
    }
}