using UnityEngine;
using UnityEngine.UI;

namespace WasmLoader
{
    public class WasmLoaderTest : MonoBehaviour
        {
            public string WasmCode;

            public Text obj;
        
        public void Start()
        {
            //var wasm = GameObject.Find("Cube (1)").GetComponent<WasmLoader.WasmLoaderTest>();

            var instance = WasmLoader.WasmLoaderMod.Instance.GetWasmInstance(WasmCode);
            instance.instance.GetGlobal(instance.store, "obj").SetValue(instance.store, instance.objects.StoreObject(obj));
            WasmLoader.WasmLoaderMod.Instance.SetupGameobject(gameObject, instance);
        }
    }

    
}
