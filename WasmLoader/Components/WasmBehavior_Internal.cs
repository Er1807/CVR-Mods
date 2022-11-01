using ABI.CCK.Components;
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
        public CVRInteractable CvrInteractable;

        public Dictionary<string, Action> funtionLookup = new Dictionary<string, Action>();
        public Dictionary<string, Action<int>> funtionLookupInt = new Dictionary<string, Action<int>>();

        public void OnDestroy()
        {
            WasmLoaderMod.Instance.LoggerInstance.Msg("Unloading Wasm Instance " + gameObject.name);
            try
            {
                WasmManager.Instance.WasmInstances.Remove(CvrInteractable);
            }
            catch (Exception) { }
            Instance.store.Dispose();
            Instance.linker.Dispose();
            Instance.module.Dispose();
            Instance.engine.Dispose();
        }

        public void Execute(string method)
        {
            if (!funtionLookup.TryGetValue(method, out Action action))
            {
                action = Instance.instance.GetAction(Instance.store, method);
                funtionLookup[method] = action;
            }
            if (action == null)
                return;
            try
            {
                action.Invoke();
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
            try
            {
                if (typeof(T) == typeof(int) || typeof(T) == typeof(long) || typeof(T) == typeof(float) || typeof(T) == typeof(double))
                {
                    Action<T> action = Instance.instance.GetAction<T>(Instance.store, method);

                    if (action == null)
                        return;
                    action.Invoke(parameter);
                    Instance.CleanUpLocals();
                }
                else
                {
                    if (!funtionLookupInt.TryGetValue(method, out Action<int> action))
                    {
                        action = Instance.instance.GetAction<int>(Instance.store, method);
                        funtionLookupInt[method] = action;
                    }
                    if (action == null)
                        return;


                    var paramAsId = Instance.objects.StoreObject(parameter);
                    action.Invoke(paramAsId);
                    Instance.CleanUpLocals();


                }
            }
            catch (Exception ex)
            {
                WasmLoaderMod.Instance.LoggerInstance.Warning(ex.Message);
                WasmLoaderMod.Instance.LoggerInstance.Warning(ex.StackTrace);
            }

        }

        public void Execute<T, U>(string method, T parameter, U parameter2)
        {
            try
            {
                var paramAsId = Instance.objects.StoreObject(parameter);
                var paramAsId2 = Instance.objects.StoreObject(parameter2);
                Instance.instance.GetAction<int, int>(Instance.store, method)?.Invoke(paramAsId, paramAsId2);
                Instance.CleanUpLocals();
            }
            catch (Exception ex)
            {
                WasmLoaderMod.Instance.LoggerInstance.Warning(ex.Message);
                WasmLoaderMod.Instance.LoggerInstance.Warning(ex.StackTrace);
            }
        }

        //Forwarding events
        #region Events
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

        public int GetProgramVariableInt(string variable)
        {
            if (Instance.exports.TryGetValue(variable, out var value) && value.Kind == Wasmtime.ValueKind.Int32)
            {
                return (int)value.GetValue(Instance.store);
            }
            throw new Exception("Variable not found or Type wrong");
        }

        public long GetProgramVariableLong(string variable)
        {
            if (Instance.exports.TryGetValue(variable, out var value) && value.Kind == Wasmtime.ValueKind.Int64)
            {
                return (long)value.GetValue(Instance.store);
            }
            throw new Exception("Variable not found or Type wrong");
        }

        public float GetProgramVariableFloat(string variable)
        {
            if (Instance.exports.TryGetValue(variable, out var value) && value.Kind == Wasmtime.ValueKind.Float32)
            {
                return (float)value.GetValue(Instance.store);
            }
            throw new Exception("Variable not found or Type wrong");
        }

        public double GetProgramVariableDouble(string variable)
        {
            if (Instance.exports.TryGetValue(variable, out var value) && value.Kind == Wasmtime.ValueKind.Float64)
            {
                return (double)value.GetValue(Instance.store);
            }
            throw new Exception("Variable not found or Type wrong");
        }

        internal object GetProgramVariableObject(string variable)
        {
            if (Instance.exports.TryGetValue(variable, out var value) && value.Kind == Wasmtime.ValueKind.Int32)
            {
                return value.GetValue(Instance.store);
            }
            throw new Exception("Variable not found or Type wrong");
        }

        internal void SetProgramVariable(string variable, object value)
        {
            if (Instance.exports.TryGetValue(variable, out var value2) && value2.Kind == Wasmtime.ValueKind.Int32)
            {
                value2.SetValue(Instance.store, value);
            }
        }
        #endregion
    }
}
