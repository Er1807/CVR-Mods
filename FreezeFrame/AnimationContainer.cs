using DarkRift;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FreezeFrame
{



    public class AnimationContainer
    {
        public AnimationCurve Curve = new AnimationCurve();

        public AnimationContainer() { }


        public void Record(float currentTime, float value)
        {
            //if (Curve.length != 0 && Curve.keys[Curve.length - 1].value == value)
            //    return;
            
            Curve.AddKey(currentTime, value);
        }

        public void Optimize()
        {
            for (int i = 1; i < Curve.length-1; i++)
            {
                if (Math.Abs(Curve[i].value - Curve[i - 1].value) < 0.00001f && Math.Abs(Curve[i].value - Curve[i + 1].value) < 0.00001f)
                {
                    Curve.RemoveKey(i);
                    i--;
                }
            }

            if (Curve.length == 3) // hack
            {
                if (Math.Abs(Curve[1].value - Curve[0].value) < 0.00001f && Math.Abs(Curve[1].value - Curve[2].value) < 0.00001f)
                {
                    Curve.RemoveKey(1);
                    Curve.RemoveKey(2);
                }
            }
            
            if (Curve.length == 2)
            {
                if (Math.Abs(Curve[1].value - Curve[0].value) < 0.00001f)
                {
                    Curve.RemoveKey(1);
                }
            }

        }

        internal int Serialize(BinaryWriter writer)
        {
            writer.Write(Curve.keys.Length);
            foreach (var key in Curve.keys)
            {
                writer.Write(key.time);
                writer.Write(key.value);
                writer.Write(key.inTangent);
                writer.Write(key.outTangent);
            }
            return Curve.keys.Length;
        }

        internal int Deserialize(BinaryReader reader)
        {
            var length = reader.ReadInt32();
            List<Keyframe> keys = new List<Keyframe>();
            for (int i = 0; i < length; i++)
            {
                var time = reader.ReadSingle();
                var value = reader.ReadSingle();
                var inTangent = reader.ReadSingle();
                var outTangent = reader.ReadSingle();
                keys.Add(new Keyframe(time, value, inTangent, outTangent));
            }
            Curve.keys = keys.ToArray();
            return length;
        }
    }
}
