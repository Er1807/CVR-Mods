using ABI_RC.Core.IO;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CameraAnimation
{
    public class CameraAnimationCalculator
    {
        public static AnimationCurve PosX;
        public static AnimationCurve PosY;
        public static AnimationCurve PosZ;


        public static AnimationCurve RotX;
        public static AnimationCurve RotY;
        public static AnimationCurve RotZ;
        public static AnimationCurve RotW;
        
        private static CVRPathCamController GetInstance => CVRPathCamController.Instance;
        private static HarmonyLib.Harmony HarmonyInstance => CameraAnimationMod.Instance.HarmonyInstance;

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
                prefix: new HarmonyMethod(typeof(CameraAnimationCalculator).GetMethod("GenerateCurves", BindingFlags.Static | BindingFlags.Public)));


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
            RotW = new AnimationCurve();

            for (int i = 0; i < GetInstance.points.Count; i++)
            {
                CVRPathCamPoint point = GetInstance.points[i];

                PosX.AddKey(i, point.position.x);
                PosY.AddKey(i, point.position.y);
                PosZ.AddKey(i, point.position.z);

                RotX.AddKey(i, point.rotation.x);
                RotY.AddKey(i, point.rotation.y);
                RotZ.AddKey(i, point.rotation.z);
                RotW.AddKey(i, point.rotation.w);
            }
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
            __result = new Quaternion(RotX.Evaluate(t), RotY.Evaluate(t), RotZ.Evaluate(t), RotW.Evaluate(t));
            
            return false;
        }

    }
}
