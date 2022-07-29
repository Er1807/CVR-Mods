using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MelonLoader;
using UnityEngine;

namespace FreezeFrame
{


    public class AnimationModule
    {

        public class AnimationContainer
        {
            public string Path;
            public string Property;
            public AnimationCurve Curve = new AnimationCurve();

            public AnimationContainer(string path, string property)
            {
                Path = path;
                Property = property;
            }


            public void Record(float value)
            {
                Curve.AddKey(CurrentTime, value);
            }
        }

        public AnimationModule(FreezeFrameMod fr) => freezeFrame = fr;

        public FreezeFrameMod freezeFrame;

        public Dictionary<(string, string), AnimationContainer> AnimationsCache = new Dictionary<(string, string), AnimationContainer>();

        public static float CurrentTime;

        private Transform recordingAvatar;
        private GameObject recordingPlayer;
        private bool _recording = false;

        public void StartRecording(GameObject player, bool calledByRemote = false)
        {
            if (calledByRemote && !freezeFrame.allowRemoteRecording.Value)
                return;
            _recording = true;
            recordingPlayer = player;
            
            AnimationsCache.Clear();
            freezeFrame.Resync();
            CurrentTime = 0;

            if (freezeFrame.VRCWSLibaryPresent && !calledByRemote)
            {
                //VRCWSLibaryIntegration.CreateFreezeOf(FreezeAction.StartAnim, player.field_Private_APIUser_0.id);
            }

        }
        public void StopRecording(bool calledByRemote = false, bool isMain = false)
        {
            if (calledByRemote && !freezeFrame.allowRemoteRecording.Value)
                return;
            _recording = false;
            freezeFrame.FullCopyWithAnimations(recordingPlayer, CreateClip(), isMain);

            if (freezeFrame.VRCWSLibaryPresent && !calledByRemote)
            {
                //VRCWSLibaryIntegration.CreateFreezeOf(FreezeAction.StopAnim, Player.prop_Player_0.field_Private_APIUser_0.id);
            }
        }

        public bool Recording => _recording;

        public AnimationClip CreateClip()
        {
            AnimationClip clip = new AnimationClip();
            clip.legacy = true;
            clip.name = "FreezeAnimation";

            var transformType = typeof(Transform);
            var skinnedMeshrendererType = typeof(SkinnedMeshRenderer);
            var gameObjectType = typeof(GameObject);
            
            float loopingDelay = freezeFrame.smoothLoopingDuration.Value;
            foreach (var item in AnimationsCache)
            {
                if (item.Value.Property.StartsWith("blendShape"))
                {
                    if(loopingDelay > 0)
                        FixLooping(item.Value.Curve);
                    clip.SetCurve(item.Value.Path, skinnedMeshrendererType, item.Value.Property, item.Value.Curve);
                }
                else if (item.Value.Property == "m_IsActive")
                {
                    clip.SetCurve(item.Value.Path, gameObjectType, item.Value.Property, item.Value.Curve);
                }
                else
                {
                    if(loopingDelay > 0)
                        FixLooping(item.Value.Curve);
                    clip.SetCurve(item.Value.Path, transformType, item.Value.Property, item.Value.Curve);
                }
            }
            return clip;

            void FixLooping(AnimationCurve curve)
            {
                curve.preWrapMode = WrapMode.Loop;
                curve.postWrapMode = WrapMode.Loop;
                
                Keyframe frame = curve.keys[0];
                frame.time = CurrentTime + loopingDelay;
                frame.weightedMode = WeightedMode.None;
                curve.AddKey(frame);
            }
        }

        public void Record(Transform transform = null, string path = "")
        {
            if (transform == null) transform = recordingAvatar;
            if (path == "")
            {
                Save(path, "localPosition.x", transform.position.x);
                Save(path, "localPosition.y", transform.position.y);
                Save(path, "localPosition.z", transform.position.z);
            }
            else
            {
                Save(path, "localPosition.x", transform.localPosition.x);
                Save(path, "localPosition.y", transform.localPosition.y);
                Save(path, "localPosition.z", transform.localPosition.z);
            }

            if (path == "")
            {
                Save(path, "localRotation.x", transform.rotation.x);
                Save(path, "localRotation.y", transform.rotation.y);
                Save(path, "localRotation.z", transform.rotation.z);
                Save(path, "localRotation.w", transform.rotation.w);
            }
            else
            {
                Save(path, "localRotation.x", transform.localRotation.x);
                Save(path, "localRotation.y", transform.localRotation.y);
                Save(path, "localRotation.z", transform.localRotation.z);
                Save(path, "localRotation.w", transform.localRotation.w);
            }
            if (Time.frameCount % 10 == 0)
            {
                Save(path, "localScale.x", transform.localScale.x);
                Save(path, "localScale.y", transform.localScale.y);
                Save(path, "localScale.z", transform.localScale.z);
            }
            var renderer = transform.gameObject.GetComponent<SkinnedMeshRenderer>();

            if (renderer != null && freezeFrame.recordBlendshapes.Value)
            {
                string[] lookup;
                if (!blendShapeLookup.TryGetValue(renderer, out lookup))
                {
                    freezeFrame.LoggerInstance.Msg("Building lookup for " + path);
                    lookup = new string[renderer.sharedMesh.blendShapeCount];
                    for (int i = 0; i < renderer.sharedMesh.blendShapeCount; i++)
                    {
                        lookup[i] = renderer.sharedMesh.GetBlendShapeName(i);
                    }
                    blendShapeLookup[renderer] = lookup;
                }
                {
                    for (int i = 0; i < renderer.sharedMesh.blendShapeCount; i++)
                    {
                        if (Time.frameCount % 5 == i % 5)
                            Save(path, $"blendShape.{lookup[i]}", renderer.GetBlendShapeWeight(i));
                    }
                }
            }

            Save(path, "m_IsActive", transform.gameObject.activeInHierarchy ? 1 : 0);

            for (int i = 0; i < transform.childCount; i++)
            {
                if (path == "")
                    Record(transform.GetChild(i), path + transform.GetChild(i).name);
                else
                    Record(transform.GetChild(i), $"{path}/{transform.GetChild(i).name}");
            }
        }

        private Dictionary<SkinnedMeshRenderer, string[]> blendShapeLookup = new Dictionary<SkinnedMeshRenderer, string[]>();

        private void Save(string path, string propertyName, float value)
        {
            AnimationContainer container;
            if (!AnimationsCache.TryGetValue((path, propertyName), out container))
            {
                container = new AnimationContainer(path, propertyName);
                AnimationsCache[(path, propertyName)] = container;
            }

            container.Record(value);
        }
    }
}
