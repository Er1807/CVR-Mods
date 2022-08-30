using ABI.CCK.Components;
using System.Linq;
using UnityEngine;
using WasmLoader;
using WasmLoader.TypeWrappers;

namespace Test
{

    public class Search : WasmBehavior
    {
        private ListGameobject list;
        public override void Start()
        {
            list = new ListGameobject();
            list.Add(GameObject.Find("Movies/Lil Jon - Hey (Official Music Video) ft. 3OH!3"));
            list.Add(GameObject.Find("Movies/SoMo - Bad Chick (Audio)"));
            list.Add(GameObject.Find("Movies/Alice Deejay - Better Off Alone (Official Video)"));
            list.Add(GameObject.Find("Movies/Macklemore & Ryan Lewis - Can't Hold Us (Lyrics) ft. Ray Dalton"));
            list.Add(GameObject.Find("Movies/Macklemore & Ryan Lewis - Can't Hold Us (Lyrics) ft. Ray Dalton"));
        }

        public void SearchMovie(string str)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var obj = list.Get(i);
                CheckTrue(str, obj);
                CheckFalse(str, obj);
            }
        }

        public void CheckTrue(string str, GameObject obj)
        {
            if (obj.name.Contains(str))
            {
                obj.SetActive(true);
            }
        }

        public void CheckFalse(string str, GameObject obj)
        {
            if (!obj.name.Contains(str))
            {
                obj.SetActive(false);
            }
        }
    }
}
