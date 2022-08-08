using ABI_RC.Systems.MovementSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WasmLoader;

namespace Test
{
    public class Class1
    {
        private GameObject obj;
        public void Setup()
        {
            obj = GameObject.Find("MirrorButtons/Mirrors/Mirrorrightp/");
        }
        
        public void Teleport()
        {
            MovementSystem.Instance.TeleportTo(new Vector3(93, 87, -40));
        }

        public void ToggleMirror()
        {
            var state = obj.activeSelf;
            state = !state;
            obj.SetActive(state);
        }

        public void ToggleMirror2()
        {
            var obj = GameObject.Find("MirrorButtons/Mirrors/Mirrorrightp/");
            obj.SetActive(!obj.activeSelf);
            if (obj.activeSelf)
                Logtest.Msg("Mirror is now Visible");
            else
                Logtest.Msg("Mirror is now Invisible");
            Logtest.Msg("Method Done");

        }

        public void Test()
        {
            for (int i = 0; i < 10; i++)
            {
                Logtest.Msg("Current number is " + i);
            }
        }
        public void Test2()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    Logtest.Msg("Current number i is " + i + " j is " + j);
                }
            }
        }

        

        //broken
        public void FizzBuzz()
        {
            if (GameObject.Find("MirrorButtons/Mirrors/Mirrorrightp/").activeSelf)
            {
                Logtest.Msg("Mirror Right is Visible");
            }
            else if (GameObject.Find("MirrorButtons/Mirrors/Mirrorleftp/").activeSelf)
            {
                Logtest.Msg("Mirror Left is Visible");

            }
            else
            {
                Logtest.Msg("No Mirror is active");
            }
        }

        public void Test3()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    TestFunction(i, j);
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void TestFunction(int i, int j)
        {
            Logtest.Msg("Inside sep function");
            Logtest.Msg("Current number i is " + i + " j is " + j);
        }
    }
}
