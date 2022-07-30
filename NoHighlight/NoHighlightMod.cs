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

[assembly: MelonInfo(typeof(NoHighlightMod), "NoHightlight", "1.0.0", "Eric van Fandenfart")]
[assembly: MelonGame]

namespace NoHighlight
{

    public class NoHighlightMod : MelonMod { 
    
        public static NoHighlightMod Instance;
        public override void OnApplicationStart()
        {
            Instance = this;
            LoggerInstance.Msg("Patching HighlightEffect");
            
            HarmonyInstance.Patch(typeof(HighlightEffect).GetMethod(nameof(HighlightEffect.SetHighlighted), BindingFlags.Instance | BindingFlags.Public), prefix: new HarmonyMethod(typeof(NoHighlightMod).GetMethod(nameof(DoNothing), BindingFlags.Static | BindingFlags.Public)));
            
            LoggerInstance.Msg("Done patching HighlightEffect");
        }

        public static void DoNothing(ref bool state)
        {
            state = false;
        }


    }
}
