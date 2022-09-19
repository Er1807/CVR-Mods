using ABI_RC.Core.IO;
using ABI_RC.Core.Player;
using ActionMenu;
using CameraAnimation;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[assembly: MelonInfo(typeof(CameraAnimationMod), "Camera Animations", "2.3.1", "Eric van Fandenfart")]
//[assembly: MelonAdditionalDependencies("ActionMenuApi", "UIExpansionKit")]
//[assembly: MelonOptionalDependencies("TouchCamera")]
[assembly: MelonGame]

namespace CameraAnimation
{

    public class CameraAnimationMod : MelonMod
    {
        public static CameraAnimationMod Instance;

        

        public override void OnApplicationStart()
        {
            Instance = this;
            new CameraAnimationMenu();
        }

        public void RetrieveMethods()
        {
            var type = typeof(CVRPathCamController);
        }


        /*
         
         CustomSubMenu.AddButton("Save\nCurrent", () => savedAnimations.Save(), LoadImage("save current"));
                    CustomSubMenu.AddButton("Load from Clipboard", () => savedAnimations.CopyFromClipBoard(), LoadImage("load from clipboard"));
                    CustomSubMenu.AddButton("Copy to Clipboard", () => savedAnimations.CopyToClipBoard(), LoadImage("copy to clipboard"));
                    foreach (string availableSave in savedAnimations.AvailableSaves)
                    {
                        CustomSubMenu.AddSubMenu(availableSave,
                            delegate
                            {
                                CustomSubMenu.AddButton("Load", () => savedAnimations.Load(availableSave), LoadImage("load"));
                                CustomSubMenu.AddButton("Delete", () => savedAnimations.Delete(availableSave), LoadImage("delete"));
                                CustomSubMenu.AddButton("Copy to Clipboard", () => savedAnimations.CopyToClipBoard(availableSave), LoadImage("copy to clipboard"));

                            }, LoadImage("save current"));
                    }
         */

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
                throw new NotImplementedException();
            }

            private List<MenuItem> GenerateSettingsMenu()
            {
                return new List<MenuItem>() {
                    MenuRadialWrapper("Speed", Instance.SetSpeed, "speed"),
                    MenuToggleWrapper("Loop mode", Instance.SetLoopMode, "loop mode"),
                    MenuToggleWrapper("Enable Pickup", Instance.SetEnablePickup, "play"),
                };
            }

            public MenuItem MenuButtonWrapper(string name, Action action, string icon = null)
            {
                return new MenuItem() { name = name, action = BuildButtonItem(name.Replace(" ", ""), action), icon = icon };
            }
            public MenuItem MenuToggleWrapper(string name, Action<bool> action, string icon = null)
            {
                return new MenuItem() { name = name, action = BuildToggleItem(name.Replace(" ", ""), action), icon = icon };
            }
            public MenuItem MenuRadialWrapper(string name, Action<double> action, string icon = null)
            {
                return new MenuItem() { name = name, action = BuildRadialItem(name.Replace(" ", ""), action), icon = icon };
            }
            public MenuItem DynamicMenuWrapper(string name, Func<List<MenuItem>> action, string icon = null)
            {
                return new MenuItem() { name = name, action = BuildCallbackMenu(name.Replace(" ", ""), action), icon = icon };
            }

        }

        private void SetEnablePickup(bool obj)
        {
            throw new NotImplementedException();
        }

        private void SetLoopMode(bool obj)
        {
            throw new NotImplementedException();
        }

        private void SetSpeed(double obj)
        {
            throw new NotImplementedException();
        }

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
            GetInstance.PlayPath(5);
        }

        private void RemoveLastPosition()
        {
            var last = GetInstance.points.Last();
            last.SelectThisPoint();
            GetInstance.DeleteSelectedPoint();
        }
        
        private void SavePos()
        {
            Transform rotationPivot = PlayerSetup.Instance._movementSystem.rotationPivot;
            GetInstance.points.Add(new CVRPathCamPoint(rotationPivot.position, rotationPivot.rotation, GetInstance.points.Count));
        }

        private CVRPathCamController GetInstance => CVRPathCamController.Instance;

    }


}
