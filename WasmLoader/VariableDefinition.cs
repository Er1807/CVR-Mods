using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WasmLoader
{
    public class VariableDefinition
    {
        public string VariableType;
        public string VariableName;
        public bool IsArray;
        public bool IsSynchronized;
        //Added later
        public int StartIndex;
        public int ArrayCount;
    }
}
