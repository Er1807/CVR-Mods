using System.Collections.Generic;
using UnityEngine;

namespace WasmLoader.TypeWrappers
{
    public class ListGameobject
    {
        private List<GameObject> list = new List<GameObject>();
        public void Add(GameObject obj) => list.Add(obj);
        public int Count => list.Count;
        public GameObject Get(int i) => list[i];
    }
}
