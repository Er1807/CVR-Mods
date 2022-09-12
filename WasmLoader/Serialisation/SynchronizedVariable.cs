using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasmtime;

namespace WasmLoader.Serialisation
{
    public class SynchronizedVariable
    {
        public WasmInstance Instance;
        public Global ValueGlobal;
        public string Name;
        public Type ValueType;

        public SynchronizedVariable(WasmInstance instance, string name, Global valueGlobal, Type valueType)
        {
            Instance = instance;
            ValueGlobal = valueGlobal;
            Name = name;
            ValueType = valueType;
        }

        public object Value
        {
            get
            {
                return ValueGlobal.GetValue(Instance.store);
            }
            set
            {
                var oldValue = ValueGlobal.GetValue(Instance.store);
                if (oldValue == value)
                    return;
                ValueGlobal.SetValue(Instance.store, value);
                Instance.behavior.Execute<string>("OnValueChanged", Name);
            }

        }
    }
}
