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
        public void Excute<T>(WasmInstance instance, string method, T parameter)
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
            Excute(Instance, nameof(OnCollisionEnter), collision);

        }
        public void OnCollisionExit(Collision other)
        {
            Excute(Instance, nameof(OnCollisionExit), other);
        }
        public void OnCollisionStay(Collision collisionInfo)
        {
            Excute(Instance, nameof(OnCollisionStay), collisionInfo);
        }
        public void OnTriggerEnter(Collision collision)
        {
            Excute(Instance, nameof(OnTriggerEnter), collision);
        }
        public void OnTriggerExit(Collision other)
        {
            Excute(Instance, nameof(OnTriggerExit), other);
        }
        public void OnTriggerStay(Collision collisionInfo)
        {
            Excute(Instance, nameof(OnTriggerStay), collisionInfo);
        }
        

        //Not yet
        public void OnPlayerJoined()
        {
            Execute(Instance, nameof(OnPlayerJoined));
        }
        public void OnPlayerLeft()
        {
            Execute(Instance, nameof(OnPlayerLeft));
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
    }
}
