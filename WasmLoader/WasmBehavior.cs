using UnityEngine;
using WasmLoader.Components;
using WasmLoader.TypeWrappers;

namespace WasmLoader
{
    public class WasmBehavior : MonoBehaviour
    {
#if !UNITY_EDITOR && !UNITY_STANDALONE
        public WasmBehavior_Internal _internal;
#endif
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
        public virtual void OnPlayerJoined(CVRPlayerApi player) { }
        public virtual void OnPlayerLeft(CVRPlayerApi player) { }
        public virtual void Grab() { }
        public virtual void Drop() { }
        public virtual void OnValueChanged(string name) { }
        public GameObject CurrentGameObject() { return null; }
        public void SendCustomEvent(string eventName)
        {
#if !UNITY_EDITOR && !UNITY_STANDALONE
            _internal.Execute(eventName);
#endif
        }

        public void SendCustomEvent(string eventName, int parameter)
        {
#if !UNITY_EDITOR && !UNITY_STANDALONE
            _internal.Execute(eventName, parameter);
#endif
        }

        public void SendCustomEvent(string eventName, long parameter)
        {
#if !UNITY_EDITOR && !UNITY_STANDALONE
            _internal.Execute(eventName, parameter);
#endif
        }

        public void SendCustomEvent(string eventName, float parameter)
        {
#if !UNITY_EDITOR && !UNITY_STANDALONE
            _internal.Execute(eventName, parameter);
#endif
        }

        public void SendCustomEvent(string eventName, double parameter)
        {
#if !UNITY_EDITOR && !UNITY_STANDALONE
            _internal.Execute(eventName, parameter);
#endif
        }
        public void SendCustomEvent(string eventName, object parameter)
        {
#if !UNITY_EDITOR && !UNITY_STANDALONE
            _internal.Execute(eventName, parameter);
#endif
        }

        public void SetProgramVariable(string variable, int value)
        {
#if !UNITY_EDITOR && !UNITY_STANDALONE
            _internal.SetProgramVariable(variable, value);
#endif
        }
        public void SetProgramVariable(string variable, long value)
        {
#if !UNITY_EDITOR && !UNITY_STANDALONE
            _internal.SetProgramVariable(variable, value);
#endif
        }
        public void SetProgramVariable(string variable, float value)
        {
#if !UNITY_EDITOR && !UNITY_STANDALONE
            _internal.SetProgramVariable(variable, value);
#endif
        }
        public void SetProgramVariable(string variable, double value)
        {
#if !UNITY_EDITOR && !UNITY_STANDALONE
            _internal.SetProgramVariable(variable, value);
#endif
        }
        public void SetProgramVariable(string variable, object value)
        {
#if !UNITY_EDITOR && !UNITY_STANDALONE
            _internal.SetProgramVariable(variable, value);
#endif
        }
        public int GetProgramVariableInt(string variable)
        {
#if !UNITY_EDITOR && !UNITY_STANDALONE
            return _internal.GetProgramVariableInt(variable);
#else
            return 0;
#endif
        }
        public long GetProgramVariableLong(string variable)
        {
#if !UNITY_EDITOR && !UNITY_STANDALONE
            return _internal.GetProgramVariableLong(variable);
#else
            return 0;
#endif
        }
        public float GetProgramVariableFloat(string variable)
        {
#if !UNITY_EDITOR && !UNITY_STANDALONE
            return _internal.GetProgramVariableFloat(variable);
#else
            return 0;
#endif
        }
        public double GetProgramVariableDouble(string variable)
        {
#if !UNITY_EDITOR && !UNITY_STANDALONE
            return _internal.GetProgramVariableDouble(variable);
#else
            return 0;
#endif
        }
        public object GetProgramVariableObject(string variable)
        {
#if !UNITY_EDITOR && !UNITY_STANDALONE
            return _internal.GetProgramVariableObject(variable);
#else
            return 0;
#endif
        }
    }
}
