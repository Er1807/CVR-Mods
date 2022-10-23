using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WasmLoader.TypeWrappers;

namespace WasmLoader.Examples
{
    public class Arrays : WasmBehavior
    {
        private int[] arr;
        private float[] arrf;
        private double[] arrd;
        private long[] arrl;
        private string[] arro;
        private int i = 0;
        public override void Start()
        {
            arr = new int[20];
            arrf = new float[20];
            arrd = new double[20];
            arrl = new long[20];
            arro = new string[8];
            arro[0] = "Hello";
            arro[1] = "this";
            arro[2] = "is";
            arro[3] = "a";
            arro[4] = "Test";
            arro[5] = "I hope";
            arro[6] = "it";
            arro[7] = "works";
        }

        public override void InteractDown()
        {
            i = (i + 1) % arro.Length;
            Logger.Msg("current " + arro[i]);
        }
    }
}
