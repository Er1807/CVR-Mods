using ABI_RC.Core.InteractionSystem;
using HarmonyLib;
using HighlightPlus;
using MelonLoader;
using NoHighlight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

[assembly: MelonInfo(typeof(NoHighlightMod), "NoHighlight", "1.0.1", "Eric van Fandenfart")]
[assembly: MelonGame]

namespace NoHighlight
{

    public class NoHighlightMod : MelonMod { 
    
        public static NoHighlightMod Instance;
        private MelonPreferences_Entry<bool> hideHighlights;
        private MelonPreferences_Entry<bool> fadeHighlights;

        public override void OnApplicationStart()
        {
            Instance = this;

            
            MelonPreferences_Category category = MelonPreferences.CreateCategory("NoHighlight");

            hideHighlights = category.CreateEntry("Disable Highlights", true, description: "Hide Highlights");
            fadeHighlights = category.CreateEntry("Fade Highlights", false, description: "Fade Highlights instead of disabeling");

            LoggerInstance.Msg("Patching HighlightEffect");
            
            HarmonyInstance.Patch(typeof(HighlightEffect).GetMethod(nameof(HighlightEffect.SetHighlighted), BindingFlags.Instance | BindingFlags.Public), prefix: new HarmonyMethod(typeof(NoHighlightMod).GetMethod(nameof(DoNothing), BindingFlags.Static | BindingFlags.Public)));
            
            LoggerInstance.Msg("Done patching HighlightEffect");
        }

        private static Dictionary<HighlightEffect, DateTime> fadeTimers = new Dictionary<HighlightEffect, DateTime>();
        private static Dictionary<HighlightEffect, DateTime> lastCall = new Dictionary<HighlightEffect, DateTime>();

        public static void DoNothing(HighlightEffect __instance, ref bool state)
        {
            if (Instance.fadeHighlights.Value)
            {
                if (state)
                {
                    if (lastCall.TryGetValue(__instance, out var lastTime))
                        if (lastTime.AddSeconds(2) < DateTime.Now)
                            fadeTimers.Remove(__instance);


                    if (!fadeTimers.ContainsKey(__instance))
                        fadeTimers.Add(__instance, DateTime.Now);

                    if (fadeTimers[__instance].AddSeconds(2) < DateTime.Now)
                    {
                        state = false;
                    }
                    lastCall[__instance] = DateTime.Now;
                }
            }

            if(Instance.hideHighlights.Value)
                state = false;
        }


    }
}
