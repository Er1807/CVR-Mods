using ABI_RC.Core.InteractionSystem;
using ABI_RC.Core.Player;
using ABI_RC.Core.Savior;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FreezeFrame
{
    public class FreezeSaveManager
    {
        public static FreezeSaveManager Intstance;
        private GameObject ClonesParent => FreezeFrameMod.Instance.ClonesParent;

        public FreezeSaveManager()
        {
            Intstance = this;
        }

        public void OpenSaveDialog()
        {
            OpenKeyboard("", (str) => SaveAll(str));
        }

        private void OpenKeyboard(string currentValue, Action<string> callback)
        {
            ViewManagerPatches.OnKeyboardSubmitted = callback;
            ViewManager.Instance.openMenuKeyboard(currentValue);
        }

        public List<string> AvailableSaveNames()
        {
            var path = Path.Combine("UserData", "FreezeFrame");
            return Directory.GetDirectories(path).Select(x => Path.GetFileName(x)).ToList();
        }

        public bool ExistsGuid(Guid guid)
        {
            foreach (var item in ClonesParent.GetComponentsInChildren<FreezeData>())
                if (item.guid == guid)
                    return true;
            return false;
        }

        public void Delete(string sceneName)
        {
            Directory.Delete(Path.Combine("UserData", "FreezeFrame", sceneName), true);
        }

        public void LoadAll(string sceneName)
        {
            var path = Path.Combine("UserData", "FreezeFrame", sceneName);

            //Directory.CreateDirectory(path);

            foreach (var file in Directory.GetFiles(path))
            {
                var guid = Guid.Parse(Path.GetFileNameWithoutExtension(file));

                if (ExistsGuid(guid))
                    continue;


                var data = new FreezeData();
                using (var stream = new FileStream(file, FileMode.Open))
                    data.Deserialize(stream);

                if (MetaPort.Instance.currentAvatarGuid != data.AvatarId)
                    continue;
                //Optimize a little

                var anim = AnimationModule.GetAnimationModuleForPlayer(PlayerSetup.Instance.GetComponent<PlayerDescriptor>());
                anim.LoadFromSave(data);
                anim.StopRecording(guid: data.guid);
            }


        }

        public void SaveAll(string sceneName)
        {
            foreach (var item in ClonesParent.GetComponentsInChildren<FreezeData>())
            {
                item.Save(sceneName);
            }
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
