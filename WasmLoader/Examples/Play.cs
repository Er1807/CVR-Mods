using ABI.CCK.Components;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using WasmLoader;

namespace WasmLoader.Examples
{

    public class Play : WasmBehavior
    {
        private CVRVideoPlayer player;
        public override void Start() {
            player = GameObject.Find("VideoPlayerUiPrefab/Script-Component").GetComponent(typeof(CVRVideoPlayer)) as CVRVideoPlayer;
        }

        public override void InteractDown()
        {
            player.SetUrl((CurrentGameObject().GetComponent(typeof(Text)) as Text).text);
        }
    }
}
