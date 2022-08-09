using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WasmLoader
{
    public class WasmBehavior
    {
        public virtual void Start() { }
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
        public virtual void LateUpdate() { }
        public virtual void OnDisable() { }
        public virtual void OnEnable() { }
        public virtual void OnCollisionEnter(Collision collision) { }
        public virtual void OnCollisionExit(Collision other) { }
        public virtual void OnCollisionStay(Collision collisionInfo) { }
        public virtual void OnTriggerEnter(Collision collision) { }
        public virtual void OnTriggerExit(Collision other) { }
        public virtual void OnTriggerStay(Collision collisionInfo) { }
        public virtual void InteractDown() { }
        public virtual void InteractUp() { }
        public virtual void OnPlayerJoined() { }
        public virtual void OnPlayerLeft() { }
        public virtual void Grab() { }
        public virtual void Drop() { }
    }
}
