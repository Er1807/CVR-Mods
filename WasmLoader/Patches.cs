using ABI.CCK.Components;
using ABI_RC.Core.InteractionSystem;
using ABI_RC.Core.Player;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

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
