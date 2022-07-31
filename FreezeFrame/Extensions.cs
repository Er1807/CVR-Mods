using ABI.CCK.Components;
using ABI_RC.Core.Player;
using ABI_RC.Core.Savior;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FreezeFrame
{
    internal static class Extensions
    {

        public static GameObject GetAvatarGameObject(this PlayerDescriptor desriptor)
        {
            return desriptor.transform.Find("[PlayerAvatar]/_CVRAvatar(Clone)").gameObject;
        }

        public static string GetAvatarId(this PlayerDescriptor desriptor)
        {
            if (desriptor.name == "_PLAYERLOCAL")
                return MetaPort.Instance.currentAvatarGuid;
            return desriptor.GetAvatarGameObject().GetComponent<CVRAssetInfo>().guid;
        }
    }
}
