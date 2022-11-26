#if UNITY_EDITOR
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
        i32_mul, i64_mul, f32_mul, f64_mul,
        i32_div_s, i64_div_s, i32_div_u, i64_div_u, f32_div, f64_div,
        i32_rem_s, i64_rem_s, i32_rem_u, i64_rem_u,
        i64_extend_i32_s, i64_extend_i32_u, i32_wrap_i64,
        f64_promote_f32, f32_demote_f64,
        f32_convert_i32_s, f32_convert_i32_u, f32_convert_i64_s, f32_convert_i64_u,
        f64_convert_i32_s, f64_convert_i32_u, f64_convert_i64_s, f64_convert_i64_u,
        i32_trunc_f32_s, i32_trunc_f32_u, i32_trunc_f64_s, i32_trunc_f64_u,
        i64_trunc_f32_s, i64_trunc_f32_u, i64_trunc_f64_s, i64_trunc_f64_u,
        i32_eq, i64_eq, f32_eq, f64_eq,
        i32_eqz, i64_eqz,
        i32_gt_s, i64_gt_s, f32_gt, f64_gt,
        i32_gt_u, i64_gt_u,
        i32_lt_s, i64_lt_s, f32_lt, f64_lt,
        i32_lt_u, i64_lt_u,
        i32_ge_s, i64_ge_s, f32_ge, f64_ge,
        i32_ge_u, i64_ge_u,
        get_local, set_local, tee_local,
        get_global, set_global,
        br, br_if, block, loop, end
    }
}
#endif