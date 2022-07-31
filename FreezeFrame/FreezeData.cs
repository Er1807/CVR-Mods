using MelonLoader;
using System;
using System.Collections.Generic;
using UnityEngine;
using static FreezeFrame.AnimationModule;

namespace FreezeFrame
{
    
    

    public class FreezeData : MonoBehaviour 
    {
        public bool IsMain;
        public Dictionary<(string, string), AnimationContainer> Animation;
        public string AvatarId;
        public FreezeType Type;
    }
}
