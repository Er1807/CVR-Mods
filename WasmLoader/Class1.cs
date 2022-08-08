using ABI_RC.Systems.MovementSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WasmLoader;

namespace Test
{
    public class Class1
    {
        public void Teleport()
        {
            MovementSystem.Instance.TeleportTo(new Vector3(93,87,-40));
        }

        public void ToggleMirror()
        {
            var obj = GameObject.Find("MirrorButtons/Mirrors/Mirrorrightp/");
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
                    Logtest.Msg("Current number i is " + i+ " j is " + j);
                }
            }
        }

        public void FizzBuzz()
        {
            for (int i = 1; i <= 100; i++)
            {
                if (i % 3 == 0 && i % 5 == 0)
                {
                    Logtest.Msg("FizzBuzz");
                }
                else if (i % 3 == 0)
                {
                    Logtest.Msg("Fizz");
                }
                else if (i % 5 == 0)
                {
                    Logtest.Msg("Buzz");
                }
                else
                {
                    Logtest.Msg(""+i);
                }
            }
        }
    }
}
