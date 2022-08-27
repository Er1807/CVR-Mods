using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Wasmtime;

namespace WasmLoader
{
    public class WasmLoaderBehavior : MonoBehaviour
    {
        public string WasmCode;
        public string Debug;
        
        public DictionaryStringGameObject AttributesGameObject = new DictionaryStringGameObject();
        public DictionaryStringTransform AttributesTransform = new DictionaryStringTransform();
        public DictionaryStringString AttributesString = new DictionaryStringString();
        public DictionaryStringText AttributesText = new DictionaryStringText();
        public DictionaryStringInt AttributesInt = new DictionaryStringInt();
        public DictionaryStringBool AttributesBool = new DictionaryStringBool();
    }

    [Serializable]
    public class DictionaryStringGameObject : CustomDict<string, GameObject> { }
    [Serializable]
    public class DictionaryStringTransform : CustomDict<string, Transform> { }
    [Serializable]
    public class DictionaryStringString : CustomDict<string, string> { }
    [Serializable]
    public class DictionaryStringText : CustomDict<string, Text> { }
    [Serializable]
    public class DictionaryStringInt : CustomDict<string, int> { }
    [Serializable]
    public class DictionaryStringBool : CustomDict<string, bool> { }

    [Serializable]
    public class CustomDict<K, V>
    {
        [SerializeField]
        public List<K> keys = new List<K>();
        [SerializeField]
        public List<V> values = new List<V>();

        public bool ContainsKey(K key)
        {
            return keys.Contains(key);
        }
        public V Get(K key)
        {
            return values[keys.IndexOf(key)];
        }
        public void Set(K key, V value)
        {
            Remove(key);
            keys.Add(key);
            values.Add(value);
        }

        public List<(K Key, V Value)> GetAsList()
        {
            List<(K, V)> result = new List<(K, V)>();
            for (int i = 0; i < keys.Count; i++)
            {
                result.Add((keys[i], values[i]));
            }
            return result;
        }
        public void Remove(K key)
        {
            if (!ContainsKey(key))
                return;
            var index = keys.IndexOf(key);
            keys.RemoveAt(index);
            values.RemoveAt(index);
        }
    }
}
