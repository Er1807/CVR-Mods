using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasmtime;

namespace WasmLoader
{
    public class Objectstore
    {
        private readonly Store store;
        public Dictionary<int, object> objects = new Dictionary<int, object>();
        public Dictionary<object, int> reverseObjects = new Dictionary<object, int>();
        public int Counter = 10000;
        public int NullCounter = 0;
        public Objectstore(Store store)
        {
            objects[NullCounter] = null;
            this.store = store;
        }
        
        public T RetriveObject<T>(int id, Caller caller)
        {
            if (typeof(T).FullName == "System.String" && id < 10000)
                return (T)(object)caller.GetMemory("memory").ReadNullTerminatedString(store, id);
            if (!objects.TryGetValue(id, out object obj))
                return default(T);
            if (obj == null)
                return default(T);
            try
            {
                return (T)obj;
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public int StoreObject(object obj)

        {
            if (obj == null)
                return NullCounter;

            if (reverseObjects.ContainsKey(obj))
            {
                return reverseObjects[obj];
            }

            Counter++;

            objects[Counter] = obj;
            reverseObjects[obj] = Counter;
            return Counter;
        }
    }
}
