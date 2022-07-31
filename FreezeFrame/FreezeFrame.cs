using FreezeFrame;
using HarmonyLib;
using MelonLoader;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using UnityEngine;
using ABI_RC.Core.Player;

[assembly: MelonInfo(typeof(FreezeFrameMod), "FreezeFrame", "1.4.2", "Eric van Fandenfart")]
[assembly: MelonGame]

namespace FreezeFrame
{

    public class FreezeFrameMod : MelonMod
    {

        static FreezeFrameMod()
        {
            try
            {
                //Adapted from knah's JoinNotifier mod found here: https://github.com/knah/VRCMods/blob/master/JoinNotifier/JoinNotifierMod.cs 
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("FreezeFrame.icons-freeze"))
                using (var tempStream = new MemoryStream((int)stream.Length))
                {
                    stream.CopyTo(tempStream);
                    iconsAssetBundle = AssetBundle.LoadFromMemory(tempStream.ToArray(), 0);
                    iconsAssetBundle.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                }
            }
            catch (Exception e)
            {
                MelonLogger.Warning("Consider checking for newer version as mod possibly no longer working, Exception occured OnAppStart(): " + e.Message);
            }
        }

        public Texture2D LoadImage(string name)
        {
            return iconsAssetBundle.LoadAsset($"Assets/icons-freeze/{name}.png", typeof(Texture2D)) as Texture2D;
        }

        public GameObject ClonesParent = null;
        private DateTime? DelayedSelf = null;
        private DateTime? DelayedAll = null;

        private static bool active = false;
        public MelonPreferences_Entry<FreezeType> freezeType;
        public MelonPreferences_Entry<bool> recordBlendshapes;
        public MelonPreferences_Entry<int> skipFrames;
        public MelonPreferences_Entry<float> smoothLoopingDuration;
        public MelonPreferences_Entry<bool> saveAnimations;


        private bool deleteMode;

        public static FreezeFrameMod Instance;

        public override void OnApplicationStart()
        {
            Instance = this;
            MelonLogger.Msg("Patching AssetBundle unloading");
            HarmonyInstance.Patch(typeof(AssetBundle).GetMethod("Unload"), prefix: new HarmonyMethod(typeof(FreezeFrameMod).GetMethod("PrefixUnload", BindingFlags.Static | BindingFlags.Public)));


            var freeze = LoadImage("freeze");
            freeze.hideFlags |= HideFlags.DontUnloadUnusedAsset;

            var category = MelonPreferences.CreateCategory("FreezeFrame");
            freezeType = category.CreateEntry("FreezeType", FreezeType.PerformanceFreeze, display_name: "Freeze Type", description: "Full Freeze is more accurate and copys everything but is less performant");
            recordBlendshapes = category.CreateEntry("recordBlendshapes", true, display_name: "Record Blendshapes", description: "Blendshape Recording can quite limit the performance you can disable it here");
            skipFrames = category.CreateEntry("skipFrames", 0, display_name: "Skip Frames", description: "Amount of frames to skip between recordings");
            saveAnimations = category.CreateEntry("saveAnimations", true, display_name: "Save Animations on freeze animations", description: "If anaimations are saved they can be later exported (uses more ram)");
            smoothLoopingDuration = category.CreateEntry("smoothLoopingDuration", 0.2f, "Smoothing Duration", "Duration of loop smoothing");


            MelonCoroutines.Start(WaitForUIInit());
        }
        /*
        private void CreateActionMenu()
        {
            MelonLogger.Msg("Freeze Frame Menu Opened");
            CustomSubMenu.AddButton("Delete Last", DeleteLast, LoadImage("delete last"));
            CustomSubMenu.AddButton("Delete First", () => Delete(0), LoadImage("delete first"));
            CustomSubMenu.AddButton("Freeze Self", CreateSelf, LoadImage("freeze"));
            CustomSubMenu.AddToggle("Record", animationModule.Recording, (state) =>
            {
                if (state) animationModule.StartRecording(Player.prop_Player_0);
                else animationModule.StopRecording();
            }, LoadImage("record"));

            CustomSubMenu.AddButton("Resync Animations", Resync, LoadImage("resync"));
            CustomSubMenu.AddSubMenu("Advanced", delegate
            {
                CustomSubMenu.AddButton("Freeze Self (5s)", () => DelayedSelf = DateTime.Now.AddSeconds(5), LoadImage("freeze 5sec"));
                CustomSubMenu.AddButton("Delete All", Delete, LoadImage("delete all"));
                CustomSubMenu.AddButton("Freeze All", Create, LoadImage("freeze all"));
                CustomSubMenu.AddButton("Freeze All (5s)", () => DelayedAll = DateTime.Now.AddSeconds(5), LoadImage("freeze all 5sec"));
                CustomSubMenu.AddToggle("Record Time Limit", animationModule.Recording, (state) =>
                {
                    if (state) animationModule.StartRecording(Player.prop_Player_0);
                    else animationModule.StopRecording(isMain: true);
                }, LoadImage("record"));
                CustomSubMenu.AddToggle("Delete Mode", deleteMode, SwitchDeleteMode, LoadImage("delete mode")); ;
            }, LoadImage("advanced"));
        }
        */
        private void SwitchDeleteMode(bool state)
        {
            deleteMode = state;
            EnsureHolderCreated();
            if (deleteMode)
            {
                for (int i = 0; i < ClonesParent.transform.childCount; i++)
                {
                    var collidor = ClonesParent.transform.GetChild(i).gameObject.AddComponent<BoxCollider>();
                    collidor.center = new Vector3(0, 0.7f, 0);
                    collidor.size = new Vector3(0.2f, 1, 0.2f);
                }
                foreach (var item in ClonesParent.GetComponentsInChildren<Animation>())
                {
                    item.Stop();
                }
            }
            else
            {
                for (int i = 0; i < ClonesParent.transform.childCount; i++)
                {
                    var collidor = ClonesParent.transform.GetChild(i).gameObject.GetComponent<BoxCollider>();
                    GameObject.Destroy(collidor);
                }
            }
        }

        public void Resync()
        {
            if (ClonesParent != null && ClonesParent.scene.IsValid())
                foreach (var item in ClonesParent.GetComponentsInChildren<Animation>())
                {
                    item.Stop();
                }
        }

        private IEnumerator WaitForUIInit()
        {
            //while (VRCUiManager.prop_VRCUiManager_0 == null)
            //    yield return null;
            while (GameObject.Find("UserInterface").transform.Find("Canvas_QuickMenu(Clone)/Container/Window/QMParent") == null)
                yield return null;

            //LoadUI();
        }

        public static List<AssetBundle> StillLoaded = new List<AssetBundle>();

        public static void PrefixUnload(AssetBundle __instance, ref bool unloadAllLoadedObjects)
        {
            if (!active)
                return;

            unloadAllLoadedObjects = false;
            StillLoaded.Add(__instance);
        }

        public override void OnUpdate()
        {
            if (DelayedSelf.HasValue && DelayedSelf < DateTime.Now)
            {
                DelayedSelf = null;
                CreateSelf();
            }
            if (DelayedAll.HasValue && DelayedAll < DateTime.Now)
            {
                DelayedAll = null;
                Create();
            }

            AnimationModule.Update();



            if (ClonesParent != null && ClonesParent.scene.IsValid())
                foreach (var anim in ClonesParent.GetComponentsInChildren<Animation>())
                {
                    if (!anim.IsPlaying("FreezeAnimation") && !deleteMode)
                    {
                        anim.Play("FreezeAnimation");
                        if (anim.gameObject.GetComponent<FreezeData>().IsMain)
                        {
                            foreach (var anim2 in ClonesParent.GetComponentsInChildren<Animation>())
                            {
                                anim2.Stop();
                                anim2.Play("FreezeAnimation");
                            }

                        }
                    }
                }
        }

        private void CreateSelf()
        {
            MelonLogger.Msg($"Creating Freeze Frame for yourself");
            EnsureHolderCreated();
            
            InstantiateAvatar(PlayerSetup.Instance.GetComponent<PlayerDescriptor>());
        }

        private static AssetBundle iconsAssetBundle;

        /*private void LoadUI()
        {
            menuStateController = GameObject.Find("UserInterface").transform.Find("Canvas_QuickMenu(Clone)").GetComponent<MenuStateController>();
            //based on VRCUKs code
            var camera = menuStateController.transform.Find("Container/Window/QMParent/Menu_Camera/Scrollrect/Viewport/VerticalLayoutGroup/Buttons/Button_Screenshot");
            var useractions = menuStateController.transform.Find("Container/Window/QMParent/Menu_SelectedUser_Local/ScrollRect/Viewport/VerticalLayoutGroup/Buttons_UserActions");
            var createFreezeButton = GameObject.Instantiate(camera, useractions);
            createFreezeButton.GetComponent<Button>().onClick.RemoveAllListeners();
            createFreezeButton.GetComponent<Button>().onClick.AddListener(new Action(() =>
            {
                MelonLogger.Msg($"Creating Freeze Frame for selected avatar");
                EnsureHolderCreated();
                string userid = menuStateController.GetComponentInChildren<SelectedUserMenuQM>().field_Private_IUser_0.prop_String_0;
                InstantiateByID(userid);
            }));
            createFreezeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Create Freeze";



            MelonLogger.Msg("Buttons sucessfully created");
        }*/

        public void Delete()
        {
            MelonLogger.Msg("Deleting all Freeze Frames");
            if (ClonesParent != null && ClonesParent.scene.IsValid())
            {
                GameObject.Destroy(ClonesParent);
                ClonesParent = null;
                active = false;
                MelonLogger.Msg("Cleanup after all Freezes are gone");
                foreach (var item in StillLoaded)
                {
                    item.Unload(true);
                }
                StillLoaded.Clear();
            }
        }

        public void DeleteLast()
        {
            EnsureHolderCreated();
            Delete(ClonesParent.transform.childCount - 1);
        }

        public void Delete(int i)
        {
            EnsureHolderCreated();
            if (ClonesParent.transform.childCount > i && i >= 0)
            {
                MelonLogger.Msg($"Deleting Frame {i}");
                GameObject.DestroyImmediate(ClonesParent.gameObject.transform.GetChild(i).gameObject);
                if (ClonesParent.transform.childCount == 0)
                {
                    active = false;
                    MelonLogger.Msg("Cleanup after all Freezes are gone");
                    foreach (var item in StillLoaded)
                        item.Unload(true);
                }
            }
        }

        public void EnsureHolderCreated()
        {
            active = true;
            if (ClonesParent == null || !ClonesParent.scene.IsValid())
            {
                ClonesParent = new GameObject("Avatar Clone Holder");
            }
        }

        public void Create()
        {
            MelonLogger.Msg("Creating Freeze Frame for all Avatars");
            EnsureHolderCreated();


            InstantiateAll();
        }


        public void InstantiateAll()
        {
            foreach (var player in GameObject.FindObjectsOfType<PlayerDescriptor>())
                InstantiateAvatar(player);
        }

        public void InstantiateAvatar(PlayerDescriptor player)
        {
            if (player == null)
                return;

            if (freezeType.Value == FreezeType.FullFreeze)
                FullCopy(player);
            else
                PerformantCopy(player);


        }

        

        public void FullCopyWithAnimations(PlayerDescriptor player, AnimationClip animationClip, bool isMain, Dictionary<(string, string), AnimationContainer> animationsCache)
        {
            EnsureHolderCreated();
            var copy = FullCopy(player);
            if (isMain == true)
                ClonesParent.GetComponentsInChildren<FreezeData>().Do(x => x.IsMain = false);
            var data = copy.GetComponent<FreezeData>();
            data.IsMain = isMain;
            data.Animation = animationsCache;

            var animator = copy.AddComponent<Animation>();
            animator.AddClip(animationClip, animationClip.name);
            animator.Play(animationClip.name);
        }

        public GameObject FullCopy(PlayerDescriptor player)
        {

            var source = player.GetAvatarGameObject();

            var copy = GameObject.Instantiate(source, ClonesParent.transform, true);
            copy.name = "Avatar Clone";
            
            foreach (var copycomp in copy.GetComponents<Component>())
            {
                if (copycomp != copy.transform)
                {
                    GameObject.DestroyImmediate(copycomp);
                }
            }

            UpdateShadersRecurive(copy, source); //update/restore AFTER Animator was destroyed

            copy.GetComponentsInChildren<SkinnedMeshRenderer>().Do(x => x.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On);
            copy.GetComponentsInChildren<SkinnedMeshRenderer>().DoIf( x=> x.name.EndsWith("_ShadowClone"), x => GameObject.Destroy(x));
            var data = copy.AddComponent<FreezeData>();
            data.AvatarId = player.GetAvatarId();
            data.Type = FreezeType.FullFreeze;
            return copy;
        }

        public void UpdateShadersRecurive(GameObject copy, GameObject original)
        {
            Renderer copyRenderer = copy.GetComponent<Renderer>();
            Renderer orginalRenderer = original.GetComponent<Renderer>();
            if (copyRenderer != null && orginalRenderer != null)
            {
                MaterialPropertyBlock p = new MaterialPropertyBlock();

                orginalRenderer.GetPropertyBlock(p);
                copyRenderer.SetPropertyBlock(p);
            }


            for (int i = 0; i < copy.transform.childCount; i++)
            {
                UpdateShadersRecurive(copy.transform.GetChild(i).gameObject, original.transform.GetChild(i).gameObject);
            }

        }

        public void PerformantCopy(PlayerDescriptor player)
        {
            var source = player.GetAvatarGameObject();
            var avatar = new GameObject("Avatar Clone");
            avatar.layer = LayerMask.NameToLayer("Player");
            avatar.transform.SetParent(ClonesParent.transform);

            // Get all the SkinnedMeshRenderers that belong to this avatar
            foreach (var renderer in source.GetComponentsInChildren<Renderer>())
            {
                if (renderer.name.EndsWith("_ShadowClone"))
                    continue;
                // Bake all the SkinnedMeshRenderers that belong to this avatar
                if (renderer is SkinnedMeshRenderer skinnedMeshRenderer)
                {
                    MelonLogger.Msg(renderer.gameObject.name);
                    // Create a new GameObject to house our mesh
                    var holder = new GameObject("Mesh Holder for " + skinnedMeshRenderer.name);
                    holder.layer = LayerMask.NameToLayer("Player");
                    holder.transform.SetParent(avatar.transform, false);

                    // Fix mesh location
                    holder.transform.position = skinnedMeshRenderer.transform.position;
                    holder.transform.rotation = skinnedMeshRenderer.transform.rotation;

                    // Bake the current pose
                    var mesh = new Mesh();
                    skinnedMeshRenderer.BakeMesh(mesh);

                    // setup the rendering components;
                    holder.AddComponent<MeshFilter>().mesh = mesh;
                    var target = holder.AddComponent<MeshRenderer>();
                    target.materials = skinnedMeshRenderer.materials;
                    var propertyBlock = new MaterialPropertyBlock();
                    skinnedMeshRenderer.GetPropertyBlock(propertyBlock);
                    target.SetPropertyBlock(propertyBlock);
                }
                else
                {
                    var holder = GameObject.Instantiate(renderer.gameObject, avatar.transform, true);
                    holder.layer = LayerMask.NameToLayer("Player");
                }
            }
            foreach (var lightsource in source.GetComponentsInChildren<Light>())
            {
                if (lightsource.isActiveAndEnabled)
                {
                    var holder = new GameObject("Wholesome Light Holder");
                    holder.layer = LayerMask.NameToLayer("Player");
                    holder.transform.SetParent(avatar.transform, false);
                    Light copy = holder.AddComponent<Light>();
                    copy.intensity = lightsource.intensity;
                    copy.range = lightsource.range;
                    copy.shape = lightsource.shape;
                    copy.color = lightsource.color;
                    copy.colorTemperature = lightsource.colorTemperature;
                    holder.transform.position = lightsource.transform.position;
                    holder.transform.rotation = lightsource.transform.rotation;
                }
            }
            var data = avatar.AddComponent<FreezeData>();
            data.AvatarId = player.GetAvatarId();
            data.Type = FreezeType.PerformanceFreeze;
        }
    }
}
