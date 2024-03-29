﻿using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace WasmLoader.Components
{
    public class WasmLoaderBehavior : WasmBehavior
    {
        public string WasmCode;
        public string Variables;
#if UNITY_EDITOR
        public string EditorVariables;
        public MonoScript behavior;
#endif
        public GameObject[] AttributesGameObject;
        public Transform[] AttributesTransform;
        public string[] AttributesString;
        public Text[] AttributesText;
        public int[] AttributesInt;
        public bool[] AttributesBool;
#if UNITY_EDITOR
        public DictionaryStringGameObject AttributesGameObjectDict = new DictionaryStringGameObject();
        public DictionaryStringTransform AttributesTransformDict = new DictionaryStringTransform();
        public DictionaryStringString AttributesStringDict = new DictionaryStringString();
        public DictionaryStringText AttributesTextDict = new DictionaryStringText();
        public DictionaryStringInt AttributesIntDict = new DictionaryStringInt();
        public DictionaryStringBool AttributesBoolDict = new DictionaryStringBool();
#endif
    }
#if UNITY_EDITOR
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
#endif
}