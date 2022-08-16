using System;
using Wasmtime;
namespace WasmLoader.Refs
{
    public class SystemInt32_Ref : IRef
    {
        public void Setup(Linker linker, Store store, Objectstore objects)
        {
            linker.DefineFunction("env", "System_Int32__Type", (Caller caller) => {
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg("System_Int32__Type");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                return objects.StoreObject(typeof(System.Int32));
            });

            linker.DefineFunction("env", "System_Int32__CompareTo_this_SystemObject__SystemInt32", (Caller caller, System.Int32 parameter_this, System.Int32 value) => {
                var resolved_value = objects.RetriveObject<System.Object>(value, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_value);
                WasmLoaderMod.Instance.LoggerInstance.Msg("System_Int32__CompareTo_this_SystemObject__SystemInt32");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = parameter_this.CompareTo(resolved_value);
                return result;
            });

            linker.DefineFunction("env", "System_Int32__CompareTo_this_SystemInt32__SystemInt32", (Caller caller, System.Int32 parameter_this, System.Int32 value) => {

#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(value);
                WasmLoaderMod.Instance.LoggerInstance.Msg("System_Int32__CompareTo_this_SystemInt32__SystemInt32");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = parameter_this.CompareTo(value);
                return result;
            });

            linker.DefineFunction("env", "System_Int32__Equals_this_SystemObject__SystemBoolean", (Caller caller, System.Int32 parameter_this, System.Int32 obj) => {
                var resolved_obj = objects.RetriveObject<System.Object>(obj, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_obj);
                WasmLoaderMod.Instance.LoggerInstance.Msg("System_Int32__Equals_this_SystemObject__SystemBoolean");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = parameter_this.Equals(resolved_obj);
                return result ? 1 : 0;
            });

            linker.DefineFunction("env", "System_Int32__Equals_this_SystemInt32__SystemBoolean", (Caller caller, System.Int32 parameter_this, System.Int32 obj) => {

#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(obj);
                WasmLoaderMod.Instance.LoggerInstance.Msg("System_Int32__Equals_this_SystemInt32__SystemBoolean");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = parameter_this.Equals(obj);
                return result ? 1 : 0;
            });

            linker.DefineFunction("env", "System_Int32__GetHashCode_this__SystemInt32", (Caller caller, System.Int32 parameter_this) => {
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg("System_Int32__GetHashCode_this__SystemInt32");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = parameter_this.GetHashCode();
                return result;
            });

            linker.DefineFunction("env", "System_Int32__ToString_this__SystemString", (Caller caller, System.Int32 parameter_this) => {
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg("System_Int32__ToString_this__SystemString");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = parameter_this.ToString();
                return objects.StoreObject(result);
            });

            linker.DefineFunction("env", "System_Int32__ToString_this_SystemString__SystemString", (Caller caller, System.Int32 parameter_this, System.Int32 format) => {
                var resolved_format = objects.RetriveObject<System.String>(format, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_format);
                WasmLoaderMod.Instance.LoggerInstance.Msg("System_Int32__ToString_this_SystemString__SystemString");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = parameter_this.ToString(resolved_format);
                return objects.StoreObject(result);
            });

            linker.DefineFunction("env", "System_Int32__ToString_this_SystemIFormatProvider__SystemString", (Caller caller, System.Int32 parameter_this, System.Int32 provider) => {
                var resolved_provider = objects.RetriveObject<System.IFormatProvider>(provider, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_provider);
                WasmLoaderMod.Instance.LoggerInstance.Msg("System_Int32__ToString_this_SystemIFormatProvider__SystemString");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = parameter_this.ToString(resolved_provider);
                return objects.StoreObject(result);
            });

            linker.DefineFunction("env", "System_Int32__ToString_this_SystemString_SystemIFormatProvider__SystemString", (Caller caller, System.Int32 parameter_this, System.Int32 format, System.Int32 provider) => {
                var resolved_format = objects.RetriveObject<System.String>(format, caller);
                var resolved_provider = objects.RetriveObject<System.IFormatProvider>(provider, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_format);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_provider);
                WasmLoaderMod.Instance.LoggerInstance.Msg("System_Int32__ToString_this_SystemString_SystemIFormatProvider__SystemString");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = parameter_this.ToString(resolved_format, resolved_provider);
                return objects.StoreObject(result);
            });

            linker.DefineFunction("env", "System_Int32__Parse_SystemString__SystemInt32", (Caller caller, System.Int32 s) => {
                var resolved_s = objects.RetriveObject<System.String>(s, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_s);
                WasmLoaderMod.Instance.LoggerInstance.Msg("System_Int32__Parse_SystemString__SystemInt32");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = System.Int32.Parse(resolved_s);
                return result;
            });

            linker.DefineFunction("env", "System_Int32__Parse_SystemString_SystemGlobalizationNumberStyles__SystemInt32", (Caller caller, System.Int32 s, System.Int32 style) => {
                var resolved_s = objects.RetriveObject<System.String>(s, caller);
                var resolved_style = objects.RetriveObject<System.Globalization.NumberStyles>(style, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_s);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_style);
                WasmLoaderMod.Instance.LoggerInstance.Msg("System_Int32__Parse_SystemString_SystemGlobalizationNumberStyles__SystemInt32");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = System.Int32.Parse(resolved_s, resolved_style);
                return result;
            });

            linker.DefineFunction("env", "System_Int32__Parse_SystemString_SystemIFormatProvider__SystemInt32", (Caller caller, System.Int32 s, System.Int32 provider) => {
                var resolved_s = objects.RetriveObject<System.String>(s, caller);
                var resolved_provider = objects.RetriveObject<System.IFormatProvider>(provider, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_s);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_provider);
                WasmLoaderMod.Instance.LoggerInstance.Msg("System_Int32__Parse_SystemString_SystemIFormatProvider__SystemInt32");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = System.Int32.Parse(resolved_s, resolved_provider);
                return result;
            });

            linker.DefineFunction("env", "System_Int32__Parse_SystemString_SystemGlobalizationNumberStyles_SystemIFormatProvider__SystemInt32", (Caller caller, System.Int32 s, System.Int32 style, System.Int32 provider) => {
                var resolved_s = objects.RetriveObject<System.String>(s, caller);
                var resolved_style = objects.RetriveObject<System.Globalization.NumberStyles>(style, caller);
                var resolved_provider = objects.RetriveObject<System.IFormatProvider>(provider, caller);
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_s);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_style);
                WasmLoaderMod.Instance.LoggerInstance.Msg(resolved_provider);
                WasmLoaderMod.Instance.LoggerInstance.Msg("System_Int32__Parse_SystemString_SystemGlobalizationNumberStyles_SystemIFormatProvider__SystemInt32");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = System.Int32.Parse(resolved_s, resolved_style, resolved_provider);
                return result;
            });

            linker.DefineFunction("env", "System_Int32__GetTypeCode_this__SystemTypeCode", (Caller caller, System.Int32 parameter_this) => {
#if Debug
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
                WasmLoaderMod.Instance.LoggerInstance.Msg(parameter_this);
                WasmLoaderMod.Instance.LoggerInstance.Msg("System_Int32__GetTypeCode_this__SystemTypeCode");
                WasmLoaderMod.Instance.LoggerInstance.Msg("");
#endif
                var result = parameter_this.GetTypeCode();
                return objects.StoreObject(result);
            });

        }
    }
}