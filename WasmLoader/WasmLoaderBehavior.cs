using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WasmLoader
{
    public class WasmLoaderBehavior : MonoBehaviour
    {
        public string WasmCode;
        public string Debug;

        public Dictionary<string, object> Attributes = new Dictionary<string, object>();

        public void Start()
        {
            try
            {
                var instance = WasmLoaderMod.Instance.GetWasmInstance(WasmCode);

                foreach (var item in Attributes)
                {
                    instance.instance.GetGlobal(instance.store, item.Key)?.SetValue(instance.store, instance.objects.StoreObject(item.Value));
                }

                WasmLoaderMod.Instance.SetupGameobject(gameObject, instance);
            }
            catch (Exception ex)
            {
                WasmLoaderMod.Instance.LoggerInstance.Error(ex);
            }
        }
    }


}
