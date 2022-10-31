using ABI_RC.Core.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WasmLoader.TypeWrappers;

namespace WasmLoader.Components
{
    public class WasmBehavior_Internal : MonoBehaviour
    {
        public WasmInstance Instance;

        public void Execute(string method)
        {
            try
            {
                Instance.instance.GetAction(Instance.store, method)?.Invoke();
                Instance.CleanUpLocals();
            }
            catch (Exception ex)
            {
                WasmLoaderMod.Instance.LoggerInstance.Warning(ex.Message);
                WasmLoaderMod.Instance.LoggerInstance.Warning(ex.StackTrace);
            }
            
        }
        public void Execute<T>(string method, T parameter)
        {
            var paramAsId = Instance.objects.StoreObject(parameter);
            Instance.instance.GetAction<int>(Instance.store, method)?.Invoke(paramAsId);
            Instance.CleanUpLocals();
        }
        public void Execute<T, U>(string method, T parameter, U parameter2)
        {
            var paramAsId = Instance.objects.StoreObject(parameter);
            var paramAsId2 = Instance.objects.StoreObject(parameter2);
            Instance.instance.GetAction<int, int>(Instance.store, method)?.Invoke(paramAsId, paramAsId2);
            Instance.CleanUpLocals();
        }

        public void Start()
        {
            Execute(nameof(Start));
        }
        public void Update()
        {
            Execute(nameof(Update));
        }
        public void FixedUpdate()
        {
            Execute(nameof(FixedUpdate));
        }
        public void LateUpdate()
        {
            Execute(nameof(LateUpdate));
        }
        public void OnDisable()
        {
            Execute(nameof(OnDisable));
        }
        public void OnEnable()
        {
            Execute(nameof(OnEnable));
        }
        public void OnCollisionEnter(Collision collision)
        {
            Execute(nameof(OnCollisionEnter), collision);

        }
        public void OnCollisionExit(Collision other)
        {
            Execute(nameof(OnCollisionExit), other);
        }
        public void OnCollisionStay(Collision collisionInfo)
        {
            Execute(nameof(OnCollisionStay), collisionInfo);
        }
        public void OnTriggerEnter(Collision collision)
        {
            Execute(nameof(OnTriggerEnter), collision);
        }
        public void OnTriggerExit(Collision other)
        {
            Execute(nameof(OnTriggerExit), other);
        }
        public void OnTriggerStay(Collision collisionInfo)
        {
            Execute(nameof(OnTriggerStay), collisionInfo);
        }
        //Via Patches

        public void Grab()
        {
            Execute(nameof(Grab));
        }
        public void Drop()
        {
            Execute(nameof(Drop));
        }
        public void InteractUp()
        {
            Execute(nameof(InteractUp));
        }

        public void InteractDown()
        {
            Execute(nameof(InteractDown));
        }

        public void OnPlayerJoined(CVRPlayerApi player)
        {
            Execute(nameof(OnPlayerJoined), player);
        }  
        
        public void OnPlayerLeft(CVRPlayerApi player)
        {
            Execute(nameof(OnPlayerLeft), player);
        }
    }
}
