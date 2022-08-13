using ABI_RC.Core.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WasmLoader
{
    public class WasmBehavior_Internal : MonoBehaviour
    {
        public WasmInstance Instance;

        public void Execute(WasmInstance instance, string method)
        {
            instance.instance.GetAction(instance.store, method)?.Invoke();
        }
        public void Execute<T>(WasmInstance instance, string method, T parameter)
        {
            var paramAsId = instance.objects.StoreObject(parameter);
            instance.instance.GetAction<int>(instance.store, method)?.Invoke(paramAsId);
        }
        
        public void Start()
        {
            Execute(Instance, nameof(Start));
        }
        public void Update()
        {
            Execute(Instance, nameof(Update));
        }
        public void FixedUpdate()
        {
            Execute(Instance, nameof(FixedUpdate));
        }
        public void LateUpdate()
        {
            Execute(Instance, nameof(LateUpdate));
        }
        public void OnDisable()
        {
            Execute(Instance, nameof(OnDisable));
        }
        public void OnEnable()
        {
            Execute(Instance, nameof(OnEnable));
        }
        public void OnCollisionEnter(Collision collision)
        {
            Execute(Instance, nameof(OnCollisionEnter), collision);

        }
        public void OnCollisionExit(Collision other)
        {
            Execute(Instance, nameof(OnCollisionExit), other);
        }
        public void OnCollisionStay(Collision collisionInfo)
        {
            Execute(Instance, nameof(OnCollisionStay), collisionInfo);
        }
        public void OnTriggerEnter(Collision collision)
        {
            Execute(Instance, nameof(OnTriggerEnter), collision);
        }
        public void OnTriggerExit(Collision other)
        {
            Execute(Instance, nameof(OnTriggerExit), other);
        }
        public void OnTriggerStay(Collision collisionInfo)
        {
            Execute(Instance, nameof(OnTriggerStay), collisionInfo);
        }
        //Via Patches

        public void Grab()
        {
            Execute(Instance, nameof(Grab));
        }
        public void Drop()
        {
            Execute(Instance, nameof(Drop));
        }
        public void InteractUp()
        {
            Execute(Instance, nameof(InteractUp));
        }

        public void InteractDown()
        {
            Execute(Instance, nameof(InteractDown));
        }

        public void OnPlayerJoined(CVRPlayerEntity player)
        {
            Execute(Instance, nameof(OnPlayerJoined), player);
        }  
        
        public void OnPlayerLeft(CVRPlayerEntity player)
        {
            Execute(Instance, nameof(OnPlayerLeft), player);
        }
    }
}
