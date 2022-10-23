﻿using ABI_RC.Core.Player;
using ABI_RC.Systems.MovementSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using WasmLoader;

namespace WasmLoader.Examples
{
    public class Example2 : WasmBehavior
    {
        private Text obj;
        private int Counter;
        public override void Start()
        {
            Counter = 0;
            obj = GameObject.Find("Canvas/Text").GetComponent(typeof(Text)) as Text;
            WasmLoader.Logger.Msg("Initialied Counter W# Instance");
        }

        public override void InteractDown()
        {
            Counter = Counter + 1;
            obj.text = "Button was pressed " + Counter + " Times";
            WasmLoader.Logger.Msg("Incremented Counter");
        }
        public override void OnPlayerJoined(CVRPlayerEntity player)
        {
            var t = obj.text;
            t = t + "Joined: " + player.Username+Environment.NewLine;
            obj.text = t;
            WasmLoader.Logger.Msg("Joined: " + player.Username);
        }
        public override void OnPlayerLeft(CVRPlayerEntity player)
        {
            var t = obj.text;
            t = t + "Left: " + player.Username + Environment.NewLine;
            obj.text = t;
            WasmLoader.Logger.Msg("Left: " + player.Username);
        }

    }
}
