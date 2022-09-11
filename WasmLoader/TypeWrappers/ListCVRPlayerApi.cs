using System.Collections.Generic;
using UnityEngine;

namespace WasmLoader.TypeWrappers
{
    public class ListCVRPlayerApi
    {
        private List<CVRPlayerApi> list = new List<CVRPlayerApi>();
        public void Add(CVRPlayerApi obj) => list.Add(obj);
        public int Count => list.Count;
        public CVRPlayerApi Get(int i) => list[i];
    }
}
