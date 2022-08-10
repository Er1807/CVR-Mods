using ABI.CCK.Components;
using HarmonyLib;
using System.Reflection;

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
    }
}
