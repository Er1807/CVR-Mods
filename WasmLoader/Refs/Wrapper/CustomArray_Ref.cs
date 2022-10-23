using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasmtime;

namespace WasmLoader.Refs.Wrapper
{
    internal class CustomArray_Ref : IRef
    {
        public void Setup(Dictionary<string, Action<Linker, Store, Objectstore, WasmType>> functions)
        {
            #region newArrays
            functions["Newarr_Int"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "Newarr_Int", (Caller caller, int size) =>
            {
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg(size);
                WasmLoaderMod.Instance.LoggerInstance.Msg("Newarr_Int");
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                return objects.StoreObject(new int[size]);
            });

            functions["Newarr_Long"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "Newarr_Long", (Caller caller, int size) =>
            {
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg(size);
                WasmLoaderMod.Instance.LoggerInstance.Msg("Newarr_Long");
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                return objects.StoreObject(new long[size]);
            });

            functions["Newarr_Single"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "Newarr_Single", (Caller caller, int size) =>
            {
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg(size);
                WasmLoaderMod.Instance.LoggerInstance.Msg("Newarr_Single");
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                return objects.StoreObject(new float[size]);
            });

            functions["Newarr_Double"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "Newarr_Double", (Caller caller, int size) =>
            {
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg(size);
                WasmLoaderMod.Instance.LoggerInstance.Msg("Newarr_Double");
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                return objects.StoreObject(new double[size]);
            });

            functions["Newarr_Obj"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "Newarr_Obj", (Caller caller, int size) =>
            {
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg(size);
                WasmLoaderMod.Instance.LoggerInstance.Msg("Newarr_Obj");
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                return objects.StoreObject(new object[size]);
            });
            #endregion
            
            #region SetArray
            functions["Arr_Set_Int"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "Arr_Set_Int", (Caller caller, int parameter_this, int index, int value) =>
            {
                var resolved_this = objects.RetriveObject<int[]>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(value);
                WasmLoaderMod.Instance.LoggerInstance.Msg("Arr_Set_Int");
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                resolved_this[index] = value;
            });

            functions["Arr_Set_Long"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "Arr_Set_Long", (Caller caller, int parameter_this, int index, long value) =>
            {
                var resolved_this = objects.RetriveObject<long[]>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(value);
                WasmLoaderMod.Instance.LoggerInstance.Msg("Arr_Set_Long");
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                resolved_this[index] = value;
            });

            functions["Arr_Set_Float"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "Arr_Set_Float", (Caller caller, int parameter_this, int index, float value) =>
            {
                var resolved_this = objects.RetriveObject<float[]>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(value);
                WasmLoaderMod.Instance.LoggerInstance.Msg("Arr_Set_Float");
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                resolved_this[index] = value;
            });

            functions["Arr_Set_Double"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "Arr_Set_Double", (Caller caller, int parameter_this, int index, double value) =>
            {
                var resolved_this = objects.RetriveObject<double[]>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(value);
                WasmLoaderMod.Instance.LoggerInstance.Msg("Arr_Set_Object");
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                resolved_this[index] = value;
            });

            functions["Arr_Set_Object"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "Arr_Set_Object", (Caller caller, int parameter_this, int index, int value) =>
            {
                var resolved_this = objects.RetriveObject<object[]>(parameter_this, caller);
                var resolved_value = objects.RetriveObject<object>(value, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_value);
                WasmLoaderMod.Instance.LoggerInstance.Msg("Arr_Set_Object");
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                resolved_this[index] = resolved_value;
            });
            #endregion
            
            #region GetArray
            functions["Arr_Get_Int"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "Arr_Get_Int", (Caller caller, int parameter_this, int index) =>
            {
                var resolved_this = objects.RetriveObject<int[]>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg("Arr_Get_Int");
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                return resolved_this[index];
            });

            functions["Arr_Get_Long"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "Arr_Get_Long", (Caller caller, int parameter_this, int index) =>
            {
                var resolved_this = objects.RetriveObject<long[]>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg("Arr_Get_Long");
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                return resolved_this[index];
            });

            functions["Arr_Get_Float"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "Arr_Get_Float", (Caller caller, int parameter_this, int index) =>
            {
                var resolved_this = objects.RetriveObject<float[]>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg("Arr_Get_Float");
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                return resolved_this[index];
            });

            functions["Arr_Get_Double"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "Arr_Get_Double", (Caller caller, int parameter_this, int index) =>
            {
                var resolved_this = objects.RetriveObject<double[]>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg("Arr_Get_Double");
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                return resolved_this[index];
            });
            
            functions["Arr_Get_Object"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "Arr_Get_Object", (Caller caller, int parameter_this, int index) =>
            {
                var resolved_this = objects.RetriveObject<object[]>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg("Arr_Get_Object");
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                return objects.StoreObject(resolved_this[index]);
            });
            #endregion

            #region LengthArray
            functions["Arr_Count_Int"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "Arr_Count_Int", (Caller caller, int parameter_this) =>
            {
                var resolved_this = objects.RetriveObject<int[]>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg("Arr_Count_Int");
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                return resolved_this.Length;
            });

            functions["Arr_Count_Long"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "Arr_Count_Long", (Caller caller, int parameter_this) =>
            {
                var resolved_this = objects.RetriveObject<long[]>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg("Arr_Count_Long");
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                return resolved_this.Length;
            });

            functions["Arr_Count_Float"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "Arr_Count_Float", (Caller caller, int parameter_this) =>
            {
                var resolved_this = objects.RetriveObject<float[]>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg("Arr_Count_Float");
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                return resolved_this.Length;
            });

            functions["Arr_Count_Double"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "Arr_Count_Double", (Caller caller, int parameter_this) =>
            {
                var resolved_this = objects.RetriveObject<double[]>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg("Arr_Count_Double");
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                return resolved_this.Length;
            });

            functions["Arr_Count_Object"] = (Linker linker, Store store, Objectstore objects, WasmType wasmType) =>
            linker.DefineFunction("env", "Arr_Count_Object", (Caller caller, int parameter_this) =>
            {
                var resolved_this = objects.RetriveObject<object[]>(parameter_this, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg("Arr_Count_Object");
                WasmLoaderMod.Instance.LoggerInstance.Msg("----------------------");
#endif
                return resolved_this.Length;
            });
            #endregion
        }
    }
}
