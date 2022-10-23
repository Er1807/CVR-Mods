using ABI_RC.Core.Player;
using ABI_RC.Core.Savior;
using ABI_RC.Systems.MovementSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WasmLoader.TypeWrappers
{
    internal class CVRPlayerApiLocal : CVRPlayerApi
    {
        public static CVRPlayerApiLocal Instance = new CVRPlayerApiLocal();


        private Dictionary<string, string> tags = new Dictionary<string, string>();
        private readonly PlayerSetup playerSetup = PlayerSetup.Instance;
        private MovementSystem movementSystem => playerSetup._movementSystem;
        private Animator animator => playerSetup._animator;

        private static FieldInfo grounded;
        private static FieldInfo deltaVelocityX;
        private static FieldInfo deltaVelocityY;
        private static FieldInfo deltaVelocityZ;

        static CVRPlayerApiLocal()
        {
            grounded = typeof(MovementSystem).GetField("_grounded", BindingFlags.NonPublic | BindingFlags.Instance);

            deltaVelocityX = typeof(MovementSystem).GetField("_deltaVelocityX", BindingFlags.NonPublic | BindingFlags.Instance);
            deltaVelocityY = typeof(MovementSystem).GetField("_deltaVelocityY", BindingFlags.NonPublic | BindingFlags.Instance);
            deltaVelocityZ = typeof(MovementSystem).GetField("_deltaVelocityZ", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public override bool isLocal => true;

        public override string displayName => MetaPort.Instance.username;

        public override string userId => throw new NotImplementedException();

        public override int playerId => 0;

        public override void EnablePickup(bool enable)
        {
            throw new NotImplementedException();
        }

        public override Vector3 GetBonePosition(HumanBodyBones bone)
        {
            return animator.GetBoneTransform(bone).position;
        }

        public override Quaternion GetBoneRotation(HumanBodyBones bone)
        {
            return animator.GetBoneTransform(bone).rotation;
        }

        public override void ClearPlayerTags()
        {
            tags.Clear();
        }
        public override void SetPlayerTag(string tagName, string tagValue)
        {
            tags[tagName] = tagValue;
        }

        public override string GetPlayerTag(string tagName)
        {
            if (tags.ContainsKey(tagName))
            {
                return tags[tagName];
            }
            return null;
        }

        public override Vector3 GetPosition()
        {
            return playerSetup.transform.position;
        }

        public override Quaternion GetRotation()
        {
            return playerSetup.transform.rotation;
        }

        
        public override bool IsPlayerGrounded()
        {
            return (bool)grounded.GetValue(movementSystem);
        }

        public override void SetVelocity(Vector3 velocity)
        {
            deltaVelocityX.SetValue(movementSystem, velocity.x);
            deltaVelocityY.SetValue(movementSystem, velocity.y);
            deltaVelocityZ.SetValue(movementSystem, velocity.z);
        }
        public override Vector3 GetVelocity()
        {
            return new Vector3(
                (float)deltaVelocityX.GetValue(movementSystem),
                (float)deltaVelocityY.GetValue(movementSystem),
                (float)deltaVelocityZ.GetValue(movementSystem));
        }

        public override void Immobilize(bool immobile)
        {
            movementSystem.SetImmobilized(immobile);
        }

        public override bool IsUserInVR()
        {
            return playerSetup._inVr;
        }

        public override void TeleportTo(Vector3 teleportPos, Quaternion teleportRot)
        {
            movementSystem.TeleportToPosRotStatic(teleportPos, teleportRot);
        }
    }
}
