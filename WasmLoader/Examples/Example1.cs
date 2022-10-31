using ABI_RC.Systems.MovementSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WasmLoader;

namespace WasmLoader.Examples
{
    public class Example1 : WasmBehavior
    {
        public void ifF()
        {
            var obj = GameObject.Find("MirrorButtons/Mirrors/Mirrorrightp/");
            obj.SetActive(!obj.activeSelf);
            if (obj.activeSelf)
                Debug.Log("Mirror is now Visible");
            Debug.Log("Method Done");

        }
        
        public void ifelse()
        {
            var obj = GameObject.Find("MirrorButtons/Mirrors/Mirrorrightp/");
            obj.SetActive(!obj.activeSelf);
            if (obj.activeSelf)
                Debug.Log("Mirror is now Visible");
            else
                Debug.Log("Mirror is now Invisible");
            Debug.Log("Method Done");

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
                Debug.Log("Current number is " + i);
            }
        }
        public void While()
        {
            int i = 10;
            while(i %3 !=0)
            {
                Debug.Log("Current number is " + i);
                i++;
            }
        }
        public void DoubleFor()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    Debug.Log("Current number i is " + i + " j is " + j);
                }
            }
        }

        //broken
        public void ifelsif()
        {
            if (GameObject.Find("MirrorButtons/Mirrors/Mirrorrightp/").activeSelf)
            {
                Debug.Log("Mirror Right is Visible");
            }
            else if (GameObject.Find("MirrorButtons/Mirrors/Mirrorleftp/").activeSelf)
            {
                Debug.Log("Mirror Left is Visible");
            }
        }

        public void ifelseelse()
        {
            if (GameObject.Find("MirrorButtons/Mirrors/Mirrorrightp/").activeSelf)
            {
                Debug.Log("Mirror Right is Visible");
            }
            else if (GameObject.Find("MirrorButtons/Mirrors/Mirrorleftp/").activeSelf)
            {
                Debug.Log("Mirror Left is Visible");

            }
            else
            {
                Debug.Log("No Mirror is active");
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
            Debug.Log("Inside sep function");
            Debug.Log("Current number i is " + i + " j is " + j);
        }
    }
}
