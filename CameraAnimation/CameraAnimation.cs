﻿using ABI.CCK.Components;
using ABI_RC.Core.IO;
using ABI_RC.Core.Player;
using ABI_RC.Systems.Camera;
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
                return new List<MenuItem>() { 
                
                };
                
            }

            private List<MenuItem> GenerateSettingsMenu()
            {
                return new List<MenuItem>() {
                    MenuRadialWrapper("Speed", (f) => Instance.Speed = f, "speed", Instance.Speed, 0.5, 5),
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
                return new MenuItem() { name = name, action = BuildToggleItem(name.Replace(" ", ""), action), icon = icon };
            }
            public MenuItem MenuRadialWrapper(string name, Action<double> action, string icon = null, double defaultValue = 0, double minValue= 0, double maxValue = 1)
            {
                return new MenuItem() { name = name, action = BuildRadialItem(name.Replace(" ", ""), action, (float)minValue, (float)maxValue, (float)defaultValue), icon = icon };
            }
            public MenuItem DynamicMenuWrapper(string name, Func<List<MenuItem>> action, string icon = null)
            {
                return new MenuItem() { name = name, action = BuildCallbackMenu(name.Replace(" ", ""), action), icon = icon };
            }

        }

        private bool _enablePickup = true;
        private bool EnablePickup { get => _enablePickup; set { 
                _enablePickup = value;
                foreach (var point in GetInstance.points)
                    point.displayObject.GetComponent<CVRPickupObject>().enabled = EnablePickup;
            } 
        }

        private double Speed { get; set; } = 2.5;
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
            GetInstance.PlayPath((float)Speed * GetInstance.points.Count);
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
            
        }

        private CVRPathCamController GetInstance => CVRPathCamController.Instance;

    }


}