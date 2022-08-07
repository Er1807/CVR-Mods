(module
  (import "Log" "Msg" (func $log (param i32)))
  (import "GameObject" "SetActive" (func $GameObject_SetActive (param i32 i32)))
  (import "GameObject" "IsActive" (func $GameObject_IsActive (param i32) (result i32)))
  (import "GameObject" "Find" (func $GameObject_Find (param i32) (result i32)))
  (import "MovementSystem" "Instance" (func $MovementSystem_Instance (result i32)))
  (import "MovementSystem" "TeleportTo" (func $MovementSystem_TeleportTo (param i32 i32)))
  (import "Vector3" "ctor" (func $Vector3_ctor (param i32 i32 i32) (result i32)))
  (memory (export "memory") 1 2)
 (data (i32.const 0) "\00\00\03\EA")
 (data (i32.const 16) "start\00")
 (data (i32.const 32) "latestart\00")
 (data (i32.const 64) "teleporting\00")
 (data (i32.const 94) "Toggle Mirror\00")
 (data (i32.const 128) "MirrorButtons/Mirrors/Mirrorrightp/\00")
  (func $OnApplicationStart
    i32.const 16
    call $log
  )
    (func $OnApplicationLateStart
    i32.const 32
    call $log
  )
  (func $event2
    (local $obj i32)
    i32.const 94
    call $log
    i32.const 128
    call $GameObject_Find
    set_local $obj
    get_local $obj
    get_local $obj
    call $GameObject_IsActive
    i32.const 1
	i32.xor
    call $GameObject_SetActive
    call $MovementSystem_TeleportTo
  )
  (func $event
    i32.const 64
    call $log
	call $MovementSystem_Instance
    i32.const 93
    i32.const 87
    i32.const -40
    call $Vector3_ctor
    call $Vector3_ctor
  )
    
  (export "OnApplicationStart" (func $OnApplicationStart))
  (export "OnApplicationLateStart" (func $OnApplicationLateStart))
  (export "event" (func $event))
  (export "event2" (func $event2))
)