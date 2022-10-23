using ABI_RC.Core.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WasmLoader.TypeWrappers
{
    internal class CVRPlayerApiRemote : CVRPlayerApi
    {

        public static List<CVRPlayerApiRemote> RemotePlayers = new List<CVRPlayerApiRemote>();
        
        public CVRPlayerApiRemote(PuppetMaster puppet)
        {
            puppetMaster = puppet;
            playerDescriptor = puppet.GetComponent<PlayerDescriptor>();
        }

        private static FieldInfo animatorGet;

        static CVRPlayerApiRemote()
        {

            animatorGet = typeof(PuppetMaster).GetField("_animator", BindingFlags.NonPublic | BindingFlags.Instance);
        }


        private PuppetMaster puppetMaster;
        private PlayerDescriptor playerDescriptor;
        private Animator animator => animatorGet.GetValue(puppetMaster) as Animator;

        private Dictionary<string, string> tags = new Dictionary<string, string>();


        public override bool isLocal => false;

        public override string displayName => playerDescriptor.userName;

        public override string userId => throw new NotImplementedException();
        
        public override int playerId => RemotePlayers.IndexOf(this) + 1;

        public override void ClearPlayerTags()
        {
            tags.Clear();
        }

        public override void EnablePickup(bool enable)
        {
            //Remote
        }

        public override Vector3 GetBonePosition(HumanBodyBones bone)
        {
            return animator.GetBoneTransform(bone).position;
        }

        public override Quaternion GetBoneRotation(HumanBodyBones bone)
        {
            return animator.GetBoneTransform(bone).rotation;
        }

        public override string GetPlayerTag(string tagName)
        {
            if (tags.ContainsKey(tagName))
                return tags[tagName];
            return null;
        }

        public override Vector3 GetPosition()
        {
            return puppetMaster.transform.position;
        }

        public override Quaternion GetRotation()
        {
            return puppetMaster.transform.rotation;
        }

        public override Vector3 GetVelocity()
        {
            throw new NotImplementedException();
        }

        public override void Immobilize(bool immobile)
        {
            ///Remote
        }

        public override bool IsPlayerGrounded()
        {
            return puppetMaster.PlayerAvatarMovementDataInput.AnimatorGrounded;
        }

        public override bool IsUserInVR()
        {
            return puppetMaster.PlayerAvatarMovementDataInput.DeviceType == PlayerAvatarMovementData.UsingDeviceType.PCVR;
        }

        public override void SetPlayerTag(string tagName, string tagValue)
        {
            tags[tagName] = tagValue;
        }

        public override void SetVelocity(Vector3 velocity)
        {
            //Remote
        }

        public override void TeleportTo(Vector3 teleportPos, Quaternion teleportRot)
        {
            //Remote
        }
    }
}
