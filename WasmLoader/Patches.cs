using ABI.CCK.Components;
using ABI_RC.Core.InteractionSystem;
using ABI_RC.Core.IO;
using ABI_RC.Core.Player;
using HarmonyLib;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using WasmLoader.Components;
using WasmLoader.Refs;

namespace WasmLoader
{
    public class Patches
    {

        public static void SetupHarmony()
        {
            WasmLoaderMod.Instance.HarmonyInstance.Patch(typeof(CVRInteractable).GetMethod(nameof(CVRInteractable.Grab), BindingFlags.Instance | BindingFlags.Public), prefix: new HarmonyMethod(typeof(Patches).GetMethod(nameof(Grab), BindingFlags.Static | BindingFlags.Public)));
            WasmLoaderMod.Instance.HarmonyInstance.Patch(typeof(CVRInteractable).GetMethod(nameof(CVRInteractable.Drop), BindingFlags.Instance | BindingFlags.Public), prefix: new HarmonyMethod(typeof(Patches).GetMethod(nameof(Drop), BindingFlags.Static | BindingFlags.Public)));
            WasmLoaderMod.Instance.HarmonyInstance.Patch(typeof(CVRInteractable).GetMethod(nameof(CVRInteractable.InteractDown), BindingFlags.Instance | BindingFlags.Public), prefix: new HarmonyMethod(typeof(Patches).GetMethod(nameof(InteractDown), BindingFlags.Static | BindingFlags.Public)));
            WasmLoaderMod.Instance.HarmonyInstance.Patch(typeof(CVRInteractable).GetMethod(nameof(CVRInteractable.InteractUp), BindingFlags.Instance | BindingFlags.Public), prefix: new HarmonyMethod(typeof(Patches).GetMethod(nameof(InteractUp), BindingFlags.Static | BindingFlags.Public)));

        }



        public static void Grab(CVRInteractable __instance)
        {
            if (!WasmLoaderMod.Instance.WasmInstances.TryGetValue(__instance, out var instance))
                return;
            instance.behavior.Grab();
        }

        public static void Drop(CVRInteractable __instance)
        {
            if (!WasmLoaderMod.Instance.WasmInstances.TryGetValue(__instance, out var instance))
                return;
            instance.behavior.Drop();
        }
        public static void InteractDown(CVRInteractable __instance)
        {
            if (!WasmLoaderMod.Instance.WasmInstances.TryGetValue(__instance, out var instance))
                return;
            instance.behavior.InteractDown();
        }
        public static void InteractUp(CVRInteractable __instance)
        {
            if (!WasmLoaderMod.Instance.WasmInstances.TryGetValue(__instance, out var instance))
                return;
            instance.behavior.InteractUp();
        }

        public static void GetRequestedUser(CVRInteractable __instance)
        {
            if (!WasmLoaderMod.Instance.WasmInstances.TryGetValue(__instance, out var instance))
                return;
            instance.behavior.InteractUp();
        }

        private static void UserJoinPatch(CVRPlayerEntity player)
        {
            foreach (var instance in WasmLoaderMod.Instance.WasmInstances.Values)
            {
                instance.behavior.OnPlayerJoined(player);
            }


        }

        public static void UserLeavePatch(CVRPlayerEntity player)
        {
            foreach (var instance in WasmLoaderMod.Instance.WasmInstances.Values)
            {
                instance.behavior.OnPlayerLeft(player);
            }
        }
    }

    [HarmonyPatch(typeof(CVRObjectLoader))]
    internal class LoadingPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(CVRObjectLoader.InstantiateAvatar))]
        public static void InstantiateAvatarPostfix(IEnumerator __result, string instTarget)
        {
            WasmLoaderMod.Instance.LoggerInstance.Msg("Avatar started loading. Waiting for finish");
            MelonCoroutines.Start(WaitForAvatarLoaded(__result, instTarget));
        }

        public static IEnumerator WaitForAvatarLoaded(IEnumerator __result, string instTarget)
        {
            while (AvatarQueueSystem.Instance.activeCoroutines.Any(x => x.function == __result))
            {
                yield return new WaitForSeconds(0.2f); ;
            }
            WasmLoaderMod.Instance.LoggerInstance.Msg("Avatar finished loading. Applying Wasm");
            //Initialize Avatar
            var player = GameObject.Find($"{instTarget}/[PlayerAvatar]/_CVRAvatar(Clone)");
            if (player != null)
            {

                foreach (var wasmLoader in player.GetComponentsInChildren<WasmLoaderBehavior>())
                {
                    InitializeWasm(wasmLoader, WasmType.Avatar);
                }
            }

        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(CVRObjectLoader.InstantiateProp))]
        public static void InstantiatePropPostfix(IEnumerator __result, string instTarget)
        {
            WasmLoaderMod.Instance.LoggerInstance.Msg("Prop started loading. Waiting for finish");
            MelonCoroutines.Start(WaitForPropLoaded(__result, instTarget));
        }
        public static IEnumerator WaitForPropLoaded(IEnumerator __result, string instTarget)
        {
            yield return new WaitForSeconds(0.2f);
            while (PropQueueSystem.Instance.activeCoroutines.Any(x => x.function == __result))
            {
                yield return new WaitForSeconds(0.2f);
            }
            //Initialize Prop
            WasmLoaderMod.Instance.LoggerInstance.Msg("Prop finished loading. Applying Wasm");
            var prop = GameObject.Find($"{instTarget}/_CVRSpawnable(Clone)");
            if (prop != null)
            {
                foreach (var wasmLoader in prop.GetComponentsInChildren<WasmLoaderBehavior>())
                {
                    InitializeWasm(wasmLoader, WasmType.Prop);
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(CVRObjectLoader.LoadIntoWorld))]
        public static void LoadIntoWorldPostfix()
        {
            WasmLoaderMod.Instance.LoggerInstance.Msg("World started loading. Waiting for finish");

            MelonCoroutines.Start(WaitForWorldLoaded());
        }

        

        
        public static IEnumerator WaitForWorldLoaded()
        {
            while (CVRObjectLoader.Instance.IsLoadingWorldToJoin)
            {
                yield return new WaitForSeconds(0.2f); ;
            }

            WasmLoaderMod.Instance.LoggerInstance.Msg("World finished loading. Applying Wasm");
            //Initialize World
            Scene scene = SceneManager.GetActiveScene();
            var rootObjects = scene.GetRootGameObjects();

            // iterate root objects and do something
            foreach (var gameObject in rootObjects)
            {
                if (gameObject.GetComponent<PlayerDescriptor>() != null || gameObject.GetComponentInChildren<CVRAssetInfo>() != null)
                    continue;
                
                foreach (var wasmLoader in gameObject.GetComponentsInChildren<WasmLoaderBehavior>())
                {
                    InitializeWasm(wasmLoader, WasmType.World);
                }
            }
        }

        private static void InitializeWasm(WasmLoaderBehavior wasmLoader, WasmType type)
        {

            WasmLoaderMod.Instance.LoggerInstance.Msg("LoadingPatches WasmBehavior on " + wasmLoader.gameObject.name);
            try
            {
                var instance = WasmLoaderMod.Instance.GetWasmInstance(Encoding.UTF8.GetString(Convert.FromBase64String(wasmLoader.WasmCode)), type);
                instance.gameObject = wasmLoader.gameObject;
                instance.InitMemoryManagment();
                //Not working yet
                foreach (var item in wasmLoader.AttributesGameObject.GetAsList())
                {
                    instance.instance.GetGlobal(instance.store, item.Key)?.SetValue(instance.store, instance.objects.StoreObject(item.Value));
                }
                foreach (var item in wasmLoader.AttributesTransform.GetAsList())
                {
                    instance.instance.GetGlobal(instance.store, item.Key)?.SetValue(instance.store, instance.objects.StoreObject(item.Value));
                }
                foreach (var item in wasmLoader.AttributesString.GetAsList())
                {
                    instance.instance.GetGlobal(instance.store, item.Key)?.SetValue(instance.store, instance.objects.StoreObject(item.Value));
                }
                foreach (var item in wasmLoader.AttributesText.GetAsList())
                {
                    instance.instance.GetGlobal(instance.store, item.Key)?.SetValue(instance.store, instance.objects.StoreObject(item.Value));
                }
                foreach (var item in wasmLoader.AttributesInt.GetAsList())
                {
                    instance.instance.GetGlobal(instance.store, item.Key)?.SetValue(instance.store, instance.objects.StoreObject(item.Value));
                }
                foreach (var item in wasmLoader.AttributesBool.GetAsList())
                {
                    instance.instance.GetGlobal(instance.store, item.Key)?.SetValue(instance.store, instance.objects.StoreObject(item.Value));
                }
                WasmLoaderMod.Instance.SetupGameobject(wasmLoader.gameObject, instance);
            }
            catch (Exception ex)
            {
                WasmLoaderMod.Instance.LoggerInstance.Error(ex);
            }
        }
    }

    [HarmonyPatch]
    class CVRPlayerManagerJoin
    {
        private static readonly MethodInfo _targetMethod = typeof(List<CVRPlayerEntity>).GetMethod("Add", BindingFlags.Public | BindingFlags.Instance);
        private static readonly MethodInfo _userJoinMethod = typeof(Patches).GetMethod("UserJoinPatch", BindingFlags.Static | BindingFlags.NonPublic);
        private static readonly FieldInfo _playerEntity = typeof(CVRPlayerManager).GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Instance).Single(t => t.GetField("p") != null).GetField("p");

        static MethodInfo TargetMethod()
        {
            return typeof(CVRPlayerManager).GetMethod(nameof(CVRPlayerManager.TryCreatePlayer), BindingFlags.Instance | BindingFlags.Public);
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var code = new CodeMatcher(instructions)
                .MatchForward(true, new CodeMatch(OpCodes.Callvirt, _targetMethod))
                .Insert(
                    new CodeInstruction(OpCodes.Ldloc_0),
                    new CodeInstruction(OpCodes.Ldfld, _playerEntity),
                    new CodeInstruction(OpCodes.Call, _userJoinMethod)
                )
                .InstructionEnumeration();

            return code;
        }
    }

    [HarmonyPatch(typeof(CVRPlayerEntity))]
    class CVRPlayerEntityLeave
    {
        [HarmonyPatch(nameof(CVRPlayerEntity.Recycle))]
        [HarmonyPrefix]
        private static void RecyclePatch(CVRPlayerEntity __instance)
        {
            Patches.UserLeavePatch(__instance);
        }
    }
}
