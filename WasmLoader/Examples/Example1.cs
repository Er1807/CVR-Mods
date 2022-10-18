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
    public class Example1 : WasmBehavior
    {
        public void ifF()
        {
            var obj = GameObject.Find("MirrorButtons/Mirrors/Mirrorrightp/");
            obj.SetActive(!obj.activeSelf);
            if (obj.activeSelf)
                WasmLoader.Logger.Msg("Mirror is now Visible");
            WasmLoader.Logger.Msg("Method Done");

        }
        
        public void ifelse()
        {
            var obj = GameObject.Find("MirrorButtons/Mirrors/Mirrorrightp/");
            obj.SetActive(!obj.activeSelf);
            if (obj.activeSelf)
                WasmLoader.Logger.Msg("Mirror is now Visible");
            else
                WasmLoader.Logger.Msg("Mirror is now Invisible");
            WasmLoader.Logger.Msg("Method Done");

        }
        
        public int ifelsereturn(bool v)
        {

            if (v)
                return 0;
            else
                return 45;

        }

        public void For()
        {
            for (int i = 0; i < 10; i++)
            {
                WasmLoader.Logger.Msg("Current number is " + i);
            }
        }
        public void While()
        {
            int i = 10;
            while(i %3 !=0)
            {
                WasmLoader.Logger.Msg("Current number is " + i);
                i++;
            }
        }
        public void DoubleFor()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    WasmLoader.Logger.Msg("Current number i is " + i + " j is " + j);
                }
            }
        }

        //broken
        public void ifelsif()
        {
            if (GameObject.Find("MirrorButtons/Mirrors/Mirrorrightp/").activeSelf)
            {
                WasmLoader.Logger.Msg("Mirror Right is Visible");
            }
            else if (GameObject.Find("MirrorButtons/Mirrors/Mirrorleftp/").activeSelf)
            {
                WasmLoader.Logger.Msg("Mirror Left is Visible");
            }
        }

        public void ifelseelse()
        {
            if (GameObject.Find("MirrorButtons/Mirrors/Mirrorrightp/").activeSelf)
            {
                WasmLoader.Logger.Msg("Mirror Right is Visible");
            }
            else if (GameObject.Find("MirrorButtons/Mirrors/Mirrorleftp/").activeSelf)
            {
                WasmLoader.Logger.Msg("Mirror Left is Visible");

            }
            else
            {
                WasmLoader.Logger.Msg("No Mirror is active");
            }
        }

        public void NoInlining()
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
            WasmLoader.Logger.Msg("Inside sep function");
            WasmLoader.Logger.Msg("Current number i is " + i + " j is " + j);
        }
    }
}
