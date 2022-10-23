using ABI_RC.Systems.MovementSystem;
using UnityEngine;

namespace WasmLoader.TypeWrappers
{
    public abstract class CVRPlayerApi
    {
        public abstract bool isLocal { get; }
        public abstract string displayName { get; }
        public abstract string userId { get; }
        //public bool isMaster;
        public abstract int playerId { get; }


        public static CVRPlayerApi GetPlayerById(int playerId)
        {
            if (playerId == 0)
            {
                return CVRPlayerApiLocal.Instance;
            }
            else if (CVRPlayerApiRemote.RemotePlayers.Count < playerId - 1)
            {
                return CVRPlayerApiRemote.RemotePlayers[playerId - 1];
            }
            return null;
        }
        public static int GetPlayerCount()
        {
            return 0;
        }
        public static ListCVRPlayerApi GetPlayers()
        {
            return null;
        }
        public abstract bool IsPlayerGrounded();
        public abstract Vector3 GetBonePosition(HumanBodyBones bone);
        public abstract Quaternion GetBoneRotation(HumanBodyBones bone);
        public abstract void TeleportTo(Vector3 teleportPos, Quaternion teleportRot);
        public abstract void EnablePickup(bool enable);
        public abstract void SetPlayerTag(string tagName, string tagValue);
        public abstract string GetPlayerTag(string tagName);
        public abstract void ClearPlayerTags();
        public abstract bool IsUserInVR();
        public abstract void Immobilize(bool immobile);
        public abstract void SetVelocity(Vector3 velocity);
        public abstract Vector3 GetVelocity();
        public abstract Vector3 GetPosition();
        public abstract Quaternion GetRotation();

    }


}
