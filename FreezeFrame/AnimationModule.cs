using System.Collections.Generic;
using ABI_RC.Core.Player;
using UnityEngine;

namespace FreezeFrame
{


    public partial class AnimationModule
    {

        public static readonly Dictionary<PlayerDescriptor, AnimationModule> animationModules = new Dictionary<PlayerDescriptor, AnimationModule>();
        public static AnimationModule GetAnimationModuleForPlayer(PlayerDescriptor player)
        {
            if (!animationModules.ContainsKey(player))
                animationModules.Add(player, new AnimationModule(player));

            return animationModules[player];
        }

        public static void Update()
        {


            if (Time.frameCount % (FreezeFrameMod.Instance.skipFrames.Value + 1) == 0)
                foreach (var module in animationModules.Values)
                {
                    module.Record();
                }
            
            foreach (var module in animationModules.Values)
            {
                module.CurrentTime += Time.deltaTime;
            }
        }

        private AnimationModule(PlayerDescriptor player)
        {
            Player = player;
        }

        private readonly PlayerDescriptor Player;
        public Dictionary<(string, string), AnimationContainer> AnimationsCache = new Dictionary<(string, string), AnimationContainer>();

        public float CurrentTime;
        private bool _recording = false;

        public void StartRecording(PlayerDescriptor player, bool calledByRemote = false)
        {
            AnimationsCache.Clear();
            _recording = true;
            
            FreezeFrameMod.Instance.Resync();
            CurrentTime = 0;
        }
        
        public void StopRecording(bool isMain = false)
        {
            _recording = false;
            FreezeFrameMod.Instance.FullCopyWithAnimations(Player, CreateClip(), isMain, AnimationsCache);

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
            
            float loopingDelay = FreezeFrameMod.Instance.smoothLoopingDuration.Value;
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
            if (Player == null)
            {
                _recording = false;
                AnimationsCache.Clear();
                return;
            }
            
            if (transform == null) transform = Player.GetAvatarGameObject().transform;
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

            if (renderer != null && FreezeFrameMod.Instance.recordBlendshapes.Value)
            {
                string[] lookup;
                if (!blendShapeLookup.TryGetValue(renderer, out lookup))
                {
                    FreezeFrameMod.Instance.LoggerInstance.Msg("Building lookup for " + path);
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

            container.Record(CurrentTime, value);
        }
    }
}
