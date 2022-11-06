using UnityEngine;
using Wasmtime;
using System.Collections.Generic;
using System.Linq;
using WasmLoader.Serialisation;
using WasmLoader.Components;
using System;

namespace WasmLoader
{
    public class WasmInstance
    {
        public WasmLoaderBehavior behaviorExternal;
        public Engine engine;
        public Linker linker;
        public Store store;
        public Objectstore objects;
        public Module module;
        public Instance instance;
        public WasmBehavior_Internal behaviorInternal;
        public GameObject gameObject;
        public Dictionary<string, Global> exports = new Dictionary<string, Global>();
        public List<SynchronizedVariable> synchronizedVariables = new List<SynchronizedVariable>();
        public List<VariableDefinition> variables = new List<VariableDefinition>();

        public static readonly string[] allowedSynchronized = new string[] { "System.String", "System.Int32", "System.Int64", "System.Single", "System.Double" };
        public void InitMemoryManagment()
        {
            foreach (var export in module.Exports)
            {
                var global = instance.GetGlobal(store, export.Name);
                if (global != null && global.Kind == ValueKind.Int32)
                    exports.Add(export.Name, global);
            }
            foreach (var variable in variables.Where(x=>x.IsSynchronized && allowedSynchronized.Contains(x.VariableType)))
            {
                var global = instance.GetGlobal(store, variable.VariableName);
                if (global != null)
                    synchronizedVariables.Add(new SynchronizedVariable(this, variable.VariableName, global, Type.GetType(variable.VariableType)));
            }
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