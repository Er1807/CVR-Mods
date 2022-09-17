using ABI_RC.Core.Player;
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
            using (FileStream stream = new FileStream("UserData/test.txt", FileMode.Create))
                SerializeToStream(stream);
        }

        public static void Test2()
        {
            var data = new FreezeData();
            
            using (var stream = new FileStream("UserData/test.txt", FileMode.Open))
                data.Deserialize(stream);

            var anim = AnimationModule.GetAnimationModuleForPlayer(PlayerSetup.Instance.GetComponent<PlayerDescriptor>());
            anim.LoadFromSave(data);
            anim.StopRecording();
        }

        public void SerializeToStream(Stream stream)
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

            
            
            using (var writer = new BinaryWriter(stream, Encoding.UTF8))
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

                    writer.Write(lookup[anim.Key.path]);
                    writer.Write(lookup[anim.Key.property]);
                    var count = anim.Value.Serialize(writer);
                    FreezeFrameMod.Instance.LoggerInstance.Msg(anim.Key.path + " " + anim.Key.property + " " + count);
                }
            }
        }

        public void Deserialize(Stream data)
        {
            Dictionary<int, string> lookup = new Dictionary<int, string>();
            Animation = new Dictionary<(string path, string property), AnimationContainer>();
            
            
            using (var reader = new BinaryReader(data, Encoding.UTF8))
            {
                var lookupLength = reader.ReadInt32();
                for (int i = 0; i < lookupLength; i++)
                {
                    lookup[i] = reader.ReadString();
                }

                guid = new Guid(reader.ReadBytes(16));
                AvatarId = reader.ReadString();
                Type = (FreezeType)reader.ReadByte();
                var length = reader.ReadInt32();
                for (int i = 0; i < length; i++)
                {
                    var path = lookup[reader.ReadInt32()];
                    var property = lookup[reader.ReadInt32()];
                    
                    var anim = new AnimationContainer();
                    var count = anim.Deserialize(reader);
                    Animation.Add((path, property), anim);
                    FreezeFrameMod.Instance.LoggerInstance.Msg(path + " " + property + " " + count);
                }
            }
        }
    }
}
