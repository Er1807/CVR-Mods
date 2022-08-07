using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WasmLoader
{
    public class Objectstore
    {
        public Dictionary<int, object> objects = new Dictionary<int, object>();

        public Objectstore()
        {
            objects[0] = null;
        }

        public T RetriveObject<T>(int id)
        {
            if (!objects.TryGetValue(id, out object obj))
                return default(T);
            if (obj == null)
                    return default(T);
            try
            {
                return (T) obj;
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public int StoreObject(object obj)

        {
            if (obj == null)
                return 0;
            
            objects[obj.GetHashCode()] = obj;
            return obj.GetHashCode();
        }
    }
}
