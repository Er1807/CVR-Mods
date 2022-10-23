using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter
{
    public enum WasmDataType
    {
        i32, i64, f32, f64,
        arri32, arri64, arrf32, arrf64, arrObj
    }

    public enum WasmInstructions
    {
        i32_const, i64_const, f32_const, f64_const,
        call, nop, _return, drop,
        i32_add, i64_add, f32_add, f64_add,
        i32_sub, i64_sub, f32_sub, f64_sub,
        i32_rem_s, i64_rem_s, i32_rem_u, i64_rem_u,
        i32_eq, i64_eq, f32_eq, f64_eq,
        i32_eqz, i64_eqz,
        i32_gt_s, i64_gt_s, f32_gt, f64_gt,
        i32_gt_u, i64_gt_u,
        i32_lt_s, i64_lt_s, f32_lt, f64_lt,
        i32_lt_u, i64_lt_u,
        get_local, set_local, tee_local,
        get_global, set_global,
        br, br_if, block, loop, end
    }
}
