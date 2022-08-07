(module
  (data (i32.const 4) "MirrorButtons/Mirrors/Mirrorrightp/\00")
  (data (i32.const 40) "Mirror is now Visible\00")
  (data (i32.const 62) "Mirror is now Invisible\00")
  (data (i32.const 86) "Method Done\00")
  (data (i32.const 98) "Current number is \00")
  (import "env" "ABI_RC_Systems_MovementSystem_MovementSystem__Instance__ABI_RCSystemsMovementSystemMovementSystem"(func $ABI_RC_Systems_MovementSystem_MovementSystem__Instance__ABI_RCSystemsMovementSystemMovementSystem  (result i32)))
  (import "env" "UnityEngine_Vector3__ctor_SystemSingle_SystemSingle_SystemSingle__SystemVoid"(func $UnityEngine_Vector3__ctor_SystemSingle_SystemSingle_SystemSingle__SystemVoid (param f32 f32 f32) (result i32)))
  (import "env" "ABI_RC_Systems_MovementSystem_MovementSystem__TeleportTo_this_UnityEngineVector3_UnityEngineVector3_SystemBoolean__SystemVoid"(func $ABI_RC_Systems_MovementSystem_MovementSystem__TeleportTo_this_UnityEngineVector3_UnityEngineVector3_SystemBoolean__SystemVoid (param i32 i32 i32 i32) ))
  (import "env" "UnityEngine_GameObject__Find_SystemString__UnityEngineGameObject"(func $UnityEngine_GameObject__Find_SystemString__UnityEngineGameObject (param i32) (result i32)))
  (import "env" "UnityEngine_GameObject__get_activeSelf_this__SystemBoolean"(func $UnityEngine_GameObject__get_activeSelf_this__SystemBoolean (param i32) (result i32)))
  (import "env" "UnityEngine_GameObject__SetActive_this_SystemBoolean__SystemVoid"(func $UnityEngine_GameObject__SetActive_this_SystemBoolean__SystemVoid (param i32 i32) ))
  (import "env" "WasmLoader_Logtest__Msg_SystemString__SystemVoid"(func $WasmLoader_Logtest__Msg_SystemString__SystemVoid (param i32) ))
  (import "env" "System_Int32__ToString_this__SystemString"(func $System_Int32__ToString_this__SystemString (param i32) (result i32)))
  (import "env" "System_String__Concat_SystemString_SystemString__SystemString"(func $System_String__Concat_SystemString_SystemString__SystemString (param i32 i32) (result i32)))
  (export "Teleport" (func $Teleport))
  (export "ToggleMirror" (func $ToggleMirror))
  (export "ToggleMirror2" (func $ToggleMirror2))
  (export "Test" (func $Test))
  (memory (export "memory") 1 2)
  (func $Teleport
    nop
    call $ABI_RC_Systems_MovementSystem_MovementSystem__Instance__ABI_RCSystemsMovementSystemMovementSystem
    f32.const 93
    f32.const 87
    f32.const -40
    call $UnityEngine_Vector3__ctor_SystemSingle_SystemSingle_SystemSingle__SystemVoid
    i32.const 0
    i32.const 0
    call $ABI_RC_Systems_MovementSystem_MovementSystem__TeleportTo_this_UnityEngineVector3_UnityEngineVector3_SystemBoolean__SystemVoid
    nop
    return
  )

  (func $ToggleMirror
    (local $local0 i32)
    (local $local1 i32)
    nop
    i32.const 4
    call $UnityEngine_GameObject__Find_SystemString__UnityEngineGameObject
    set_local $local0
    get_local $local0
    call $UnityEngine_GameObject__get_activeSelf_this__SystemBoolean
    set_local $local1
    get_local $local1
    i32.const 0
    i32.eq
    set_local $local1
    get_local $local0
    get_local $local1
    call $UnityEngine_GameObject__SetActive_this_SystemBoolean__SystemVoid
    nop
    return
  )

  (func $ToggleMirror2
    (local $local0 i32)
    (local $local1 i32)
    nop
    i32.const 4
    call $UnityEngine_GameObject__Find_SystemString__UnityEngineGameObject
    set_local $local0
    get_local $local0
    get_local $local0
    call $UnityEngine_GameObject__get_activeSelf_this__SystemBoolean
    i32.const 0
    i32.eq
    call $UnityEngine_GameObject__SetActive_this_SystemBoolean__SystemVoid
    nop
    get_local $local0
    call $UnityEngine_GameObject__get_activeSelf_this__SystemBoolean
    set_local $local1
    block $bl51
    get_local $local1
    i32.const 0
    i32.eq
    br_if $bl51
    i32.const 40
    call $WasmLoader_Logtest__Msg_SystemString__SystemVoid
    nop
    block $bl62
    br $bl62
    end
    i32.const 62
    call $WasmLoader_Logtest__Msg_SystemString__SystemVoid
    nop
    end
    i32.const 86
    call $WasmLoader_Logtest__Msg_SystemString__SystemVoid
    nop
    return
  )

  (func $Test
    (local $local0 i32)
    (local $local1 i32)
    (local $for1 i32)
    i32.const 1
    set_local $for1
    nop
    i32.const 0
    set_local $local0
    loop $lp5
    block $bl34
    get_local $for1
    i32.const 0
    set_local $for1
    br_if $bl34
    nop
    i32.const 98
    get_local $local0
    call $System_Int32__ToString_this__SystemString
    call $System_String__Concat_SystemString_SystemString__SystemString
    call $WasmLoader_Logtest__Msg_SystemString__SystemVoid
    nop
    nop
    get_local $local0
    i32.const 1
    i32.add
    set_local $local0
    end
    get_local $local0
    i32.const 10
    i32.lt_s
    set_local $local1
    get_local $local1
    br_if $lp5
    end
    return
  )

)