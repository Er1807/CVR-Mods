using ABI_RC.Systems.MovementSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Test
{
    public class Class1
    {
        public void Teleport()
        {
            MovementSystem.Instance.TeleportTo(new Vector3(93,87,-40));
        }

        public void ToggleMirror()
        {
            var obj = GameObject.Find("MirrorButtons/Mirrors/Mirrorrightp/");
            var state = obj.activeSelf;
            state = !state;
            obj.SetActive(state);
        }

        public void ToggleMirror2()
        {
            var obj = GameObject.Find("MirrorButtons/Mirrors/Mirrorrightp/");
            obj.SetActive(!obj.activeSelf);
            if (obj.activeSelf)
                Logtest.Msg("Mirror is now Visible");
            else
                Logtest.Msg("Mirror is now Invisible");
            Logtest.Msg("Method DOne");
        }
    }
}
