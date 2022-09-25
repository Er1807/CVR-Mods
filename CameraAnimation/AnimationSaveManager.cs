using ABI.CCK.Components;
using ABI_RC.Core.InteractionSystem;
using ABI_RC.Core.IO;
using ABI_RC.Core.Savior;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CameraAnimation
{
    public class AnimationSaveManager
    {
        public static AnimationSaveManager Instance;
        private CVRPathCamController GetInstance => CVRPathCamController.Instance;

        public AnimationSaveManager()
        {
            Instance = this;
        }

        public void OpenSaveDialog()
        {
            OpenKeyboard("", (str) => Save(str));
        }

        private void OpenKeyboard(string currentValue, Action<string> callback)
        {
            ViewManagerPatches.OnKeyboardSubmitted = callback;
            ViewManager.Instance.openMenuKeyboard(currentValue);
        }

        public List<string> AvailableSaveNames()
        {   
            var path = Path.Combine("UserData", "CameraAnimation", MetaPort.Instance.CurrentWorldId);
            if (Directory.Exists(path))
                return Directory.GetDirectories(path).Select(x => Path.GetFileName(x)).ToList();
            else
                return new List<string>();
        }

        public void Delete(string saveName)
        {
            Directory.Delete(Path.Combine("UserData", "CameraAnimation", MetaPort.Instance.CurrentWorldId, $"{saveName}.save"), true);
        }

        public void Load(string saveName)
        {
            var path = Path.Combine("UserData", "CameraAnimation", MetaPort.Instance.CurrentWorldId, $"{saveName}.save");

            Deserialize(path);
        }
        public void Save(string saveName)
        {
            var folder = Path.Combine("UserData", "CameraAnimation", MetaPort.Instance.CurrentWorldId);
            Directory.CreateDirectory(folder);

            var path = Path.Combine("UserData", "CameraAnimation", MetaPort.Instance.CurrentWorldId, $"{saveName}.save");

            Serialize(path);
        }

        private void Serialize(string path)
        {
            using (var stream = new FileStream(path, FileMode.Create))
            using (var writer = new BinaryWriter(stream, Encoding.UTF8))
            {
                writer.Write(GetInstance.points.Count);

                foreach (var point in GetInstance.points)
                {
                    writer.Write(point.position.x);
                    writer.Write(point.position.y);
                    writer.Write(point.position.z);
                    writer.Write(point.rotation.x);
                    writer.Write(point.rotation.y);
                    writer.Write(point.rotation.z);
                    writer.Write(point.rotation.w);
                }
            }
        }

        private void Deserialize(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open))
            using (var reader = new BinaryReader(stream, Encoding.UTF8))
            {
                int count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    var pos = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    var rot = new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    var point = new CVRPathCamPoint(pos, rot, GetInstance.points.Count);
                    GetInstance.points.Add(point);
                    point.displayObject.GetComponent<CVRPickupObject>().enabled = CameraAnimationMod.Instance.EnablePickup;

                }
            }
            
            CameraAnimationCalculator.GenerateCurves();
        }
    }

    [HarmonyPatch(typeof(ViewManager))]
    class ViewManagerPatches
    {
        public static Action<string> OnKeyboardSubmitted;

        [HarmonyPatch("SendToWorldUi")]
        [HarmonyPostfix]
        static void SendToWorldUi(string value)
        {
            OnKeyboardSubmitted?.Invoke(value);

            OnKeyboardSubmitted = null;
        }
    }

}
