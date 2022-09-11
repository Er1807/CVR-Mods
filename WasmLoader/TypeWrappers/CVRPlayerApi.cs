using UnityEngine;

namespace WasmLoader.TypeWrappers
{
    public class CVRPlayerApi
    {
        public bool isLocal { get; private set; }
        public bool displayName { get; private set; }
        public bool userId { get; private set; }
        //public bool isMaster;
        public bool playerId { get; private set; }

        private GameObject player; 

        public bool IsPlayerGrounded()
        {
            return false;
        }
        public CVRPlayerApi GetPlayerById(int playerId)
        {
            return null;
        }
        public int GetPlayerCount()
        {
            return 0;
        }
        public ListCVRPlayerApi GetPlayers()
        {
            return new ListCVRPlayerApi();
        }
        public Vector3 GetBonePosition(HumanBodyBones bone)
        {
            return default(Vector3);
        }
        public Quaternion GetBoneRotation(HumanBodyBones bone)
        {
            return default(Quaternion);
        }
        public void TeleportTo(Vector3 teleportPos, Quaternion teleportRot)
        {

        }
        public void EnablePickup(bool enable)
        {

        }
        public void SetPlayerTag(string tagName, string tagValue)
        {

        }
        public string GetPlayerTag(string tagName)
        {
            return "";
        }
        public void ClearPlayerTags()
        {

        }
        public bool IsUserInVR()
        {
            return false;
        }
        public void Immobilize(bool immobile)
        {
        }
        public void SetVelocity(Vector3 velocity)
        {
        }
        public Vector3 GetVelocity()
        {
            return default(Vector3);
        }
        public Vector3 GetPosition()
        {
            return player.transform.position;
        }
        public Quaternion GetRotation()
        {
            return player.transform.rotation;
        }

    }


}
