#if UNITY_EDITOR
using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter
{
    public class WasmField
    {
        public string Name;
        public TypeSig Type;
        public bool IsPublic;
        public FieldDef Field;

        public WasmField(FieldDef field)
        {
            Field = field;
            Name = field.Name;
            Type = field.FieldType;
            IsPublic = field.IsPublic;
        }
    }
}
#endif