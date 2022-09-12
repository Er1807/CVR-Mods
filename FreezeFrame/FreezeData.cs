using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace FreezeFrame
{



    public class FreezeData : MonoBehaviour
    {
        public Guid guid = Guid.NewGuid();
        public bool IsMain;
        public Dictionary<(string path, string property), AnimationContainer> Animation;
        public string AvatarId;
        public FreezeType Type;

        public void Test()
        {
            File.WriteAllBytes("UserData/test.txt", Serialize());
        }

        public byte[] Serialize()
        {
            FreezeFrameMod.Instance.LoggerInstance.Msg("Creating Lookup");
            int counter = 0;
            Dictionary<string, int> lookup = new Dictionary<string, int>();
            Dictionary<int, string> reverseLookup = new Dictionary<int, string>();

            foreach (var item in Animation.Keys)
            {
                if (!lookup.ContainsKey(item.path))
                {
                    lookup.Add(item.path, counter);
                    reverseLookup.Add(counter, item.path);
                    counter++;
                }
                if (!lookup.ContainsKey(item.property))
                {
                    lookup.Add(item.property, counter);
                    reverseLookup.Add(counter, item.property);
                    counter++;
                }
            }


            FreezeFrameMod.Instance.LoggerInstance.Msg("Done");


            using (var writer = DarkRift.DarkRiftWriter.Create(int.MaxValue, Encoding.Unicode))
            {

                FreezeFrameMod.Instance.LoggerInstance.Msg("Writing Lookup");

                writer.Write(lookup.Count);
                for (int i = 0; i < lookup.Count; i++)
                {
                    writer.Write(reverseLookup[i]);
                }

                FreezeFrameMod.Instance.LoggerInstance.Msg("Done Writing Lookup");


                writer.Write(guid.ToByteArray());
                writer.Write(AvatarId);
                writer.Write((byte)Type);
                writer.Write(Animation.Count);
                foreach (var anim in Animation)
                {

                    FreezeFrameMod.Instance.LoggerInstance.Msg(anim.Key.path + " " + anim.Key.property);
                    writer.Write(lookup[anim.Key.path]);
                    writer.Write(lookup[anim.Key.property]);
                    anim.Value.Serialize(writer);
                }
                return writer.ToArray();
            }
        }

        public void Deserialize(byte[] data)
        {
            Dictionary<int, string> lookup = new Dictionary<int, string>();
            
            using (var reader = DarkRift.DarkRiftReader.CreateFromArray(data, 0, data.Length))
            {

                var lookupLength = reader.ReadInt32();
                for (int i = 0; i < lookupLength; i++)
                {
                    lookup[i] = reader.ReadString();
                }

                guid = new Guid(reader.ReadBytes());
                AvatarId = reader.ReadString();
                Type = (FreezeType)reader.ReadByte();
                var length = reader.ReadInt32();
                for (int i = 0; i < length; i++)
                {
                    var path = reader.ReadString();
                    var property = reader.ReadString();
                    var anim = new AnimationContainer();
                    anim.Deserialize(reader);
                    Animation.Add((path, property), anim);
                }
            }
        }
    }
}
