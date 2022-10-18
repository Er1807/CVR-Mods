using ABI.CCK.Components;
using ABI_RC.Core.IO;
using ABI_RC.Core.Player;
using ABI_RC.Systems.Camera;
using ActionMenu;
using CameraAnimation;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[assembly: MelonInfo(typeof(CameraAnimationMod), "CameraAnimation", "3.1.1", "Eric van Fandenfart")]
[assembly: MelonAdditionalDependencies("ActionMenu")]
[assembly: MelonGame]

namespace CameraAnimation
{

    public class CameraAnimationMod : MelonMod
    {
        public static CameraAnimationMod Instance;



        public override void OnApplicationStart()
        {
            Instance = this;
            new AnimationSaveManager();
            new CameraAnimationMenu();

            CameraAnimationCalculator.ApplyPatches();

            MelonCoroutines.Start(WaitForPathingCam());
        }

        public IEnumerator WaitForPathingCam()
        {
            while (GetInstance == null)
            {
                yield return null;
            }
            while (GetInstance.selectedCamera == null)
            {
                yield return null;
            }
            GetInstance.selectedCamera.gameObject.AddComponent<CameraAnimationCalculator>();
        }

        public class CameraAnimationMenu : ActionMenuMod.Lib
        {
            protected override string modName => "CameraAnimation";

            protected override List<MenuItem> modMenuItems()
            {
                return new List<MenuItem>() {
                    MenuButtonWrapper("Save Pos", Instance.SavePos, "save pos"),
                    MenuButtonWrapper("Delete last Pos", Instance.RemoveLastPosition, "delete last pos"),
                    MenuButtonWrapper("Play Anim", Instance.PlayAnimation, "play"),
                    MenuButtonWrapper("Stop Anim", Instance.StopAnimation, "stop"),
                    MenuButtonWrapper("Clear Anim", Instance.ClearAnimation, "clear anim"),
                    DynamicMenuWrapper("Settings", GenerateSettingsMenu, "settings"),
                    DynamicMenuWrapper("Saved", GenerateSavedMenu, "saved"),
                };
            }
            
            private List<MenuItem> GenerateSavedMenu()
            {
                IList<MenuItem> saves = new List<MenuItem>();

                saves.Add(MenuButtonWrapper("Save", AnimationSaveManager.Instance.OpenSaveDialog, "save"));
                
                foreach (string availableSave in AnimationSaveManager.Instance.AvailableSaveNames())
                {
                    saves.Add(DynamicMenuWrapper(availableSave, () => GenerateSavedSubMenu(availableSave), "save current"));
                }
                return saves as List<MenuItem>;
            } 

            private List<MenuItem> GenerateSavedSubMenu(string availableSave)
            {
                return new List<MenuItem>() {
                    MenuButtonWrapper("Load", () => AnimationSaveManager.Instance.Load(availableSave), "load"),
                    MenuButtonWrapper("Delete", () => AnimationSaveManager.Instance.Delete(availableSave), "delete"),
                };
            }

            private List<MenuItem> GenerateSettingsMenu()
            {
                return new List<MenuItem>() {
                    MenuRadialWrapper("Speed", (f) => CameraAnimationCalculator.Instance.Speed = f, "speed", CameraAnimationCalculator.Instance.Speed, 0.1f, 3),
                    MenuToggleWrapper("Loop mode", (f) => Instance.LoopMode = f, "loop mode", Instance.LoopMode),
                    MenuToggleWrapper("Enable Pickup", (f) => Instance.EnablePickup = f, "play", Instance.EnablePickup),
                };
            }

            public MenuItem MenuButtonWrapper(string name, Action action, string icon = null)
            {
                return new MenuItem() { name = name, action = BuildButtonItem(name.Replace(" ", ""), action), icon = icon };
            }
            public MenuItem MenuToggleWrapper(string name, Action<bool> action, string icon = null, bool defaultValue = false)
            {
                return new MenuItem() { name = name, action = BuildToggleItem(name.Replace(" ", ""), action), icon = icon, enabled = defaultValue };
            }
            public MenuItem MenuRadialWrapper(string name, Action<float> action, string icon = null, float defaultValue = 0, float minValue = 0, float maxValue = 1)
            {
                return new MenuItem() { name = name, action = BuildRadialItem(name.Replace(" ", ""), action, minValue, maxValue, defaultValue), icon = icon };
            }
            public MenuItem DynamicMenuWrapper(string name, Func<List<MenuItem>> action, string icon = null)
            {
                return new MenuItem() { name = name, action = BuildCallbackMenu(name.Replace(" ", ""), action), icon = icon };
            }

        }

        private bool _enablePickup = true;
        public bool EnablePickup
        {
            get => _enablePickup; set
            {
                _enablePickup = value;
                foreach (var point in GetInstance.points)
                    point.displayObject.GetComponent<CVRPickupObject>().enabled = EnablePickup;
            }
        }
        
        private bool LoopMode { get => GetInstance.looping; set => GetInstance.looping = value; }

        private void ClearAnimation()
        {
            GetInstance.DeleteAllPoints();
        }

        private void StopAnimation()
        {
            GetInstance.StopPath();
        }

        private void PlayAnimation()
        {
            GetInstance.CopyRefCam();
            GetInstance.PlayPath(1);//doesnt matter. not used
        }

        private void RemoveLastPosition()
        {
            var last = GetInstance.points.Last();
            last.SelectThisPoint();
            GetInstance.DeleteSelectedPoint();
        }

        private void SavePos()
        {
            Transform rotationPivot = PortableCamera.Instance.cameraComponent.transform;
            var point = new CVRPathCamPoint(rotationPivot.position, rotationPivot.rotation, GetInstance.points.Count);
            GetInstance.points.Add(point);
            point.displayObject.GetComponent<CVRPickupObject>().enabled = EnablePickup;

            CameraAnimationCalculator.GenerateCurves();

        }

        private CVRPathCamController GetInstance => CVRPathCamController.Instance;

    }


}
