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
        public Dictionary<(string path, string property), AnimationContainer> AnimationsCache = new Dictionary<(string, string), AnimationContainer>();
        private HumanPoseHandler humanPoseHandler;
        private HumanPose pose = new HumanPose();
        public float CurrentTime;
        private bool _recording = false;

        public void StartRecording(bool calledByRemote = false)
        {
            AnimationsCache = new Dictionary<(string, string), AnimationContainer>();
            humanPoseHandler = new HumanPoseHandler(Player.GetAvatarGameObject().GetComponent<Animator>().avatar, Player.GetAvatarGameObject().GetComponent<Animator>().transform);

            _recording = true;

            FreezeFrameMod.Instance.Resync();
            CurrentTime = 0;
        }

        public void LoadFromSave(FreezeData data)
        {
            AnimationsCache = data.Animation;
        }

        public void StopRecording(bool isMain = false)
        {
            _recording = false;
            FreezeFrameMod.Instance.LoggerInstance.Msg("Optimizing Animations");
            foreach (var item in AnimationsCache)
            {
                if (item.Key.property.Contains("localPosition"))
                    continue;
                if (item.Key.property.Contains("localRotation"))
                    continue;
                item.Value.Optimize();
            }
            FreezeFrameMod.Instance.FullCopyWithAnimations(Player, CreateClip(), isMain, AnimationsCache);
        }

        public bool Recording => _recording;

        public AnimationClip CreateClip()
        {
            AnimationClip clip = new AnimationClip();
            clip.legacy = true;
            clip.name = "FreezeAnimation";

            var transformType = typeof(Transform);
            var animatorType = typeof(Animator);
            var skinnedMeshrendererType = typeof(SkinnedMeshRenderer);
            var gameObjectType = typeof(GameObject);

            float loopingDelay = FreezeFrameMod.Instance.smoothLoopingDuration.Value;
            foreach (var item in AnimationsCache)
            {
                if (item.Key.property.StartsWith("blendShape"))
                {
                    if (loopingDelay > 0)
                        FixLooping(item.Value.Curve);
                    clip.SetCurve(item.Key.path, skinnedMeshrendererType, item.Key.property, item.Value.Curve);
                }
                else if (item.Key.property == "m_IsActive")
                {
                    clip.SetCurve(item.Key.path, gameObjectType, item.Key.property, item.Value.Curve);
                }
                else
                {
                    if (loopingDelay > 0)
                        FixLooping(item.Value.Curve);
                    clip.SetCurve(item.Key.path, animatorType, item.Key.property, item.Value.Curve);
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

        public string GetPathRelative(Transform transform, Transform root)
        {
            if (transform == root)
                return "";

            return GetPathRelative(transform.parent, root) + "/" + transform.name;
        }
    
        public void Record()
        {
            if (!Recording)
                return;
            if (Player == null)
            {
                _recording = false;
                AnimationsCache.Clear();
                return;
            }

            humanPoseHandler.GetHumanPose(ref pose);

            for (int i = 0; i < HumanTrait.MuscleCount; i++)
            {
                Save("", HumanTrait.MuscleName[i], pose.muscles[i]);
            }
            Save("", "RootT.x", pose.bodyPosition.x);
            Save("", "RootT.y", pose.bodyPosition.y);
            Save("", "RootT.z", pose.bodyPosition.z);
            
            Save("", "RootQ.x", pose.bodyRotation.x);
            Save("", "RootQ.y", pose.bodyRotation.y);
            Save("", "RootQ.z", pose.bodyRotation.z);
            Save("", "RootQ.w", pose.bodyRotation.w);

            //Save bones
            var avatar = Player.GetAvatarGameObject();
            var animator = avatar.GetComponent<Animator>();
            
            //Save Blendshapes
            foreach (var renderer in avatar.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                if (renderer.name.EndsWith("_ShadowClone"))
                    continue;
                var path = GetPathRelative(renderer.transform, animator.transform).TrimStart('/');
                SaveBlendshapes(path, renderer);
            }

            /*
            //Save Root
            Save("", "localPosition.x", avatar.transform.position.x);
            Save("", "localPosition.y", avatar.transform.position.y);
            Save("", "localPosition.z", avatar.transform.position.z);
            Save("", "localRotation.x", avatar.transform.rotation.x);
            Save("", "localRotation.y", avatar.transform.rotation.y);
            Save("", "localRotation.z", avatar.transform.rotation.z);
            Save("", "localRotation.w", avatar.transform.rotation.w);
            Save("", "localScale.x", avatar.transform.lossyScale.x);
            Save("", "localScale.y", avatar.transform.lossyScale.y);
            Save("", "localScale.z", avatar.transform.lossyScale.z);
            Save("", "m_IsActive", avatar.activeInHierarchy ? 1 : 0);
            */
        }

        private void SaveBlendshapes(string path, SkinnedMeshRenderer renderer)
        {
            if (renderer != null && FreezeFrameMod.Instance.recordBlendshapes.Value)
            {
                string[] lookup = GetBlendShapeLookup(renderer);

                for (int i = 0; i < renderer.sharedMesh.blendShapeCount; i++)
                {
                    Save(path, $"blendShape.{lookup[i]}", renderer.GetBlendShapeWeight(i));
                }

            }
        }

        private string[] GetBlendShapeLookup(SkinnedMeshRenderer renderer)
        {
            if (!blendShapeLookup.TryGetValue(renderer, out var lookup))
            {
                lookup = new string[renderer.sharedMesh.blendShapeCount];
                for (int i = 0; i < renderer.sharedMesh.blendShapeCount; i++)
                {
                    lookup[i] = renderer.sharedMesh.GetBlendShapeName(i);
                }
                blendShapeLookup[renderer] = lookup;
            }

            return lookup;
        }

        private Dictionary<SkinnedMeshRenderer, string[]> blendShapeLookup = new Dictionary<SkinnedMeshRenderer, string[]>();

        private void Save(string path, string propertyName, float value)
        {
            AnimationContainer container;
            if (!AnimationsCache.TryGetValue((path, propertyName), out container))
            {
                container = new AnimationContainer();
                AnimationsCache[(path, propertyName)] = container;
            }

            container.Record(CurrentTime, value);
        }
    }
}
