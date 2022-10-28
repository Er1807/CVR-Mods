using ABI.CCK.Components;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using WasmLoader;
using WasmLoader.TypeWrappers;

namespace WasmLoader.Examples
{

    public class Search : WasmBehavior
    {
        private GameObject[] list;
        private InputField searchfield;
        public override void Start()
        {
            list = new GameObject[4];
            list[0] = GameObject.Find("Movies/Lil Jon - Hey (Official Music Video) ft. 3OH!3");
            list[1] = GameObject.Find("Movies/SoMo - Bad Chick (Audio)");
            list[2] = GameObject.Find("Movies/Alice Deejay - Better Off Alone (Official Video)");
            list[3] = GameObject.Find("Movies/Macklemore & Ryan Lewis - Can't Hold Us (Lyrics) ft. Ray Dalton");
            searchfield = GameObject.Find("Canvas (1)/InputField").GetComponent(typeof(InputField)) as InputField;
        }

        public override void InteractDown()
        {
            var str = searchfield.text;
            for (int i = 0; i < list.Length; i++)
            {
                var obj = list[i];
                if (obj.name.Contains(str))
                {
                    obj.SetActive(true);
                }
                else
                {
                    obj.SetActive(false);
                }
            }
        }
    }
}
