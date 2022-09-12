using DarkRift;
using System;
using UnityEngine;

namespace FreezeFrame
{



    public class AnimationContainer
    {
        public AnimationCurve Curve = new AnimationCurve();

        public AnimationContainer() { }


        public void Record(float currentTime, float value)
        {
            if (Curve.length != 0 && Curve.keys[Curve.length - 1].value == value)
                return;
            
            Curve.AddKey(currentTime, value);
        }

        internal void Serialize(DarkRiftWriter writer)
        {
            //writer.Write(Path);
            //writer.Write(Property);
            writer.Write(Curve.keys.Length);
            foreach (var key in Curve.keys)
            {
                writer.Write(key.time);
                writer.Write(key.value);
                writer.Write(key.inTangent);
                writer.Write(key.outTangent);
            }
        }

        internal void Deserialize(DarkRiftReader reader)
        {
            //Path = reader.ReadString();
            //Property = reader.ReadString();
            var length = reader.ReadInt32();
            for (int i = 0; i < length; i++)
            {
                var time = reader.ReadSingle();
                var value = reader.ReadSingle();
                var inTangent = reader.ReadSingle();
                var outTangent = reader.ReadSingle();
                Curve.AddKey(time, value);
                Curve.keys[i].inTangent = inTangent;
                Curve.keys[i].outTangent = outTangent;
            }
        }
    }
}
