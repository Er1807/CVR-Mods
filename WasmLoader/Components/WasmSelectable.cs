using System;
using System.Linq;
using UnityEngine;
using WasmLoader.Refs;

namespace WasmLoader.Components
{
    public class WasmSelectable : MonoBehaviour
    {
        public bool AllowWorldScripts;
        public bool AllowAvatarScripts;
        public bool AllowPropScripts;
        public bool AllowUserScripts;

        internal bool IsAllowed(WasmType type)
        {
            switch (type)
            {
                case WasmType.World:
                    return AllowWorldScripts;
                case WasmType.Avatar:
                    return AllowAvatarScripts;
                case WasmType.Prop:
                    return AllowPropScripts;
                case WasmType.User:
                    return AllowUserScripts;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}