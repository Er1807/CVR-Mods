using DarkRift;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WasmLoader.Serialisation
{
    public class SerialisationManager
    {
        public byte[] SerialiseToBytes(WasmInstance instance)
        {
            using (var writer = Serialise(instance))
                return writer.ToArray();
        }
        public DarkRiftWriter Serialise(WasmInstance instance)
        {
            var writer = DarkRiftWriter.Create();
            writer.Encoding = Encoding.UTF8;
            
            foreach (var obj in instance.synchronizedVariables)
            {
                switch (obj.Value)
                {
                    case null:
                        writer.Write(0);
                        break;
                    case short value:
                        writer.Write(value);
                        break;
                    case ushort value:
                        writer.Write(value);
                        break;
                    case int value:
                        writer.Write(value);
                        break;
                    case uint value:
                        writer.Write(value);
                        break;
                    case long value:
                        writer.Write(value);
                        break;
                    case ulong value:
                        writer.Write(value);
                        break;
                    case float value:
                        writer.Write(value);
                        break;
                    case double value:
                        writer.Write(value);
                        break;
                    case string value:
                        writer.Write(value);
                        break;
                    case byte value:
                        writer.Write(value);
                        break;
                    case sbyte value:
                        writer.Write(value);
                        break;
                    case char value:
                        writer.Write(value);
                        break;
                    case bool value:
                        writer.Write(value);
                        break;
                    case short[] value:
                        writer.Write(value);
                        break;
                    case ushort[] value:
                        writer.Write(value);
                        break;
                    case int[] value:
                        writer.Write(value);
                        break;
                    case uint[] value:
                        writer.Write(value);
                        break;
                    case long[] value:
                        writer.Write(value);
                        break;
                    case ulong[] value:
                        writer.Write(value);
                        break;
                    case float[] value:
                        writer.Write(value);
                        break;
                    case double[] value:
                        writer.Write(value);
                        break;
                    case string[] value:
                        writer.Write(value);
                        break;
                    case byte[] value:
                        writer.Write(value);
                        break;
                    case sbyte[] value:
                        writer.Write(value);
                        break;
                    case char[] value:
                        writer.Write(value);
                        break;
                    case bool[] value:
                        writer.Write(value);
                        break;
                }
            }
            return writer;
        }

        public void DeserialiseFromBytes(byte[] data, WasmInstance instance)
        {
            using (var reader = DarkRiftReader.CreateFromArray(data, 0, data.Length))
                Deserialise(reader, instance);
        }

        public void Deserialise(DarkRiftReader reader, WasmInstance instance)
        {
            reader.Encoding = Encoding.UTF8;
            
            foreach (var obj in instance.synchronizedVariables)
            {
                if (obj.ValueType == typeof(short))
                    obj.Value = reader.ReadInt16();
                else if (obj.ValueType == typeof(ushort))
                    obj.Value = reader.ReadUInt16();
                else if (obj.ValueType == typeof(int))
                    obj.Value = reader.ReadInt32();
                else if (obj.ValueType == typeof(uint))
                    obj.Value = reader.ReadUInt32();
                else if (obj.ValueType == typeof(long))
                    obj.Value = reader.ReadInt64();
                else if (obj.ValueType == typeof(ulong))
                    obj.Value = reader.ReadUInt64();
                else if (obj.ValueType == typeof(float))
                    obj.Value = reader.ReadSingle();
                else if (obj.ValueType == typeof(double))
                    obj.Value = reader.ReadDouble();
                else if (obj.ValueType == typeof(string))
                    obj.Value = reader.ReadString();
                else if (obj.ValueType == typeof(byte))
                    obj.Value = reader.ReadByte();
                else if (obj.ValueType == typeof(sbyte))
                    obj.Value = reader.ReadSByte();
                else if (obj.ValueType == typeof(char))
                    obj.Value = reader.ReadChar();
                else if (obj.ValueType == typeof(bool))
                    obj.Value = reader.ReadBoolean();

                //arrays
                else if (obj.ValueType == typeof(short[]))
                    obj.Value = reader.ReadInt16s();
                else if (obj.ValueType == typeof(ushort[]))
                    obj.Value = reader.ReadUInt16s();
                else if (obj.ValueType == typeof(int[]))
                    obj.Value = reader.ReadInt32s();
                else if (obj.ValueType == typeof(uint[]))
                    obj.Value = reader.ReadUInt32s();
                else if (obj.ValueType == typeof(long[]))
                    obj.Value = reader.ReadInt64s();
                else if (obj.ValueType == typeof(ulong[]))
                    obj.Value = reader.ReadUInt64s();
                else if (obj.ValueType == typeof(float[]))
                    obj.Value = reader.ReadSingles();
                else if (obj.ValueType == typeof(double[]))
                    obj.Value = reader.ReadDoubles();
                else if (obj.ValueType == typeof(string[]))
                    obj.Value = reader.ReadStrings();
                else if (obj.ValueType == typeof(byte[]))
                    obj.Value = reader.ReadBytes();
                else if (obj.ValueType == typeof(sbyte[]))
                    obj.Value = reader.ReadSBytes();
                else if (obj.ValueType == typeof(char[]))
                    obj.Value = reader.ReadChars();
                else if (obj.ValueType == typeof(bool[]))
                    obj.Value = reader.ReadBooleans();
                


            }
        }

    }
}
