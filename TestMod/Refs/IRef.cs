﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasmtime;
namespace Test.Refs
{
    internal interface IRef
    {
        void Setup(Linker linker, Store store);
    }
}
