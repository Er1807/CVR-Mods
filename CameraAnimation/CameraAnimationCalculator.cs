using ABI_RC.Core.IO;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CameraAnimation
{
    public class CameraAnimationCalculator : MonoBehaviour
    {
        public static AnimationCurve PosX;
        public static AnimationCurve PosY;
        public static AnimationCurve PosZ;


        public static AnimationCurve RotX;
        public static AnimationCurve RotY;
        public static AnimationCurve RotZ;

        private static CVRPathCamController GetInstance => CVRPathCamController.Instance;
        private static HarmonyLib.Harmony HarmonyInstance => CameraAnimationMod.Instance.HarmonyInstance;

        int currentWaypointIndex = 0;
        float currentTimeInWaypoint = 0;
        public bool Active = false;
        public float Speed { get; set; } = 1f;

        public static CameraAnimationCalculator Instance;

        public CameraAnimationCalculator()
        {
            Instance = this;
        }

        //public void OnPreCull()
        public void Update()
        {
            if (!Active || GetInstance.points.Count == 0)
                return;

            currentTimeInWaypoint += Time.deltaTime / GetInstance.points[currentWaypointIndex % GetInstance.points.Count].time * Speed;

            if (currentTimeInWaypoint >= 1.0)
            {
                currentTimeInWaypoint = 0;
                currentWaypointIndex++;
            }

            if (currentWaypointIndex == GetInstance.points.Count - 1 && !GetInstance.looping)
            {
                currentWaypointIndex = 0;
                currentTimeInWaypoint = 0;
                Active = false;
                GetInstance.StopPath();
                return;
            }

            if (currentWaypointIndex == PosX.length - 1 && GetInstance.looping)
            {
                currentWaypointIndex = currentWaypointIndex % GetInstance.points.Count;
                currentTimeInWaypoint = 0;
            }

            Vector3 vector3 = new Vector3();
            GetBezierPosition(ref vector3, currentWaypointIndex, currentTimeInWaypoint);
            GetInstance.selectedCamera.transform.position = vector3;

            Quaternion quaternion = new Quaternion();
            GetLerpRotation(ref quaternion, currentWaypointIndex, currentTimeInWaypoint);
            GetInstance.selectedCamera.transform.rotation = quaternion;
        }

        public static void ApplyPatches()
        {
            HarmonyInstance.Patch(
                typeof(CVRPathCamController).GetMethod("GetBezierPosition", BindingFlags.Instance | BindingFlags.NonPublic),
                prefix: new HarmonyMethod(typeof(CameraAnimationCalculator).GetMethod("GetBezierPosition", BindingFlags.Static | BindingFlags.Public)));

            HarmonyInstance.Patch(
                typeof(CVRPathCamController).GetMethod("GetLerpRotation", BindingFlags.Instance | BindingFlags.NonPublic),
                prefix: new HarmonyMethod(typeof(CameraAnimationCalculator).GetMethod("GetLerpRotation", BindingFlags.Static | BindingFlags.Public)));

            HarmonyInstance.Patch(
                typeof(CVRPathCamController).GetMethod("PlayPath", BindingFlags.Instance | BindingFlags.Public),
                prefix: new HarmonyMethod(typeof(CameraAnimationCalculator).GetMethod("PlayPath", BindingFlags.Static | BindingFlags.Public)));

            HarmonyInstance.Patch(
                typeof(CVRPathCamController).GetMethod("RefreschIndexes", BindingFlags.Instance | BindingFlags.Public),
                prefix: new HarmonyMethod(typeof(CameraAnimationCalculator).GetMethod("GenerateCurves", BindingFlags.Static | BindingFlags.Public)));
        }

        public static void GenerateCurves()
        {
            CameraAnimationMod.Instance.LoggerInstance.Msg("Generating Curves");

            PosX = new AnimationCurve();
            PosY = new AnimationCurve();
            PosZ = new AnimationCurve();

            RotX = new AnimationCurve();
            RotY = new AnimationCurve();
            RotZ = new AnimationCurve();

            for (int i = 0; i < GetInstance.points.Count; i++)
            {
                CVRPathCamPoint point = GetInstance.points[i];

                PosX.AddKey(i, point.position.x);
                PosY.AddKey(i, point.position.y);
                PosZ.AddKey(i, point.position.z);

                RotX.AddKey(i, point.rotation.eulerAngles.x);
                RotY.AddKey(i, point.rotation.eulerAngles.y);
                RotZ.AddKey(i, point.rotation.eulerAngles.z);
            }

            if (GetInstance.looping)
            {
                
                //atleast 2 points required
                if (GetInstance.points.Count >= 2)
                {
                    var time = GetInstance.points.Count;
                    var firstPoint = GetInstance.points[0];
                    var secondPoint = GetInstance.points[1];

                    
                    PosX.AddKey(time, firstPoint.position.x);
                    PosY.AddKey(time, firstPoint.position.y);
                    PosZ.AddKey(time, firstPoint.position.z);

                    RotX.AddKey(time, firstPoint.rotation.eulerAngles.x);
                    RotY.AddKey(time, firstPoint.rotation.eulerAngles.y);
                    RotZ.AddKey(time, firstPoint.rotation.eulerAngles.z);

                    PosX.AddKey(time + 1, secondPoint.position.x);
                    PosY.AddKey(time + 1, secondPoint.position.y);
                    PosZ.AddKey(time + 1, secondPoint.position.z);

                    RotX.AddKey(time + 1, secondPoint.rotation.eulerAngles.x);
                    RotY.AddKey(time + 1, secondPoint.rotation.eulerAngles.y);
                    RotZ.AddKey(time + 1, secondPoint.rotation.eulerAngles.z);

                }
            }

            FixRotationCurve(RotX);
            FixRotationCurve(RotY);
            FixRotationCurve(RotZ);
        }

        private static void FixRotationCurve(AnimationCurve curve)
        {
            float lastValue = 0;
            for (int i = 0; i < curve.length; i++)
            {
                var currentValue = curve.keys[i].value;

                var diff = (currentValue - lastValue) % 360;
                if (diff < -180)
                    diff += 360;
                if (diff > 180)
                    diff -= 360;

                curve.RemoveKey(i);
                //only works because time diff is exacltly 1
                curve.AddKey(i, lastValue + diff);
                //CameraAnimationMod.Instance.LoggerInstance.Msg($"{i}: lastValue: {lastValue} currentValue: {currentValue} diff: {diff} newMan: {lastValue + diff} newValue: {curve.keys[i].value}");

                lastValue = curve.keys[i].value;
            }
        }

        public static bool PlayPath()
        {
            //FreezeFrame integration
            var clonesParent = GameObject.Find("Avatar Clone Holder");
            if (clonesParent != null && clonesParent.scene.IsValid())
                foreach (var item in clonesParent.GetComponentsInChildren<Animation>())
                {
                    item.Stop();
                }

            GetInstance.selectedCamera.stereoTargetEye = StereoTargetEyeMask.None;
            GetInstance.selectedCamera.enabled = true;
            GenerateCurves();
            Instance.Active = true;
            return false;
        }

        public static bool GetBezierPosition(ref Vector3 __result, int pointIndex, float time)
        {
            var t = pointIndex + time;
            __result = new Vector3(PosX.Evaluate(t), PosY.Evaluate(t), PosZ.Evaluate(t));

            return false;
        }

        public static bool GetLerpRotation(ref Quaternion __result, int pointIndex, float time)
        {
            var t = pointIndex + time;
            __result = Quaternion.Euler(RotX.Evaluate(t), RotY.Evaluate(t), RotZ.Evaluate(t));

            return false;
        }

    }
}
