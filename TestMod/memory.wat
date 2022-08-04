(module
  (import "Log" "Msg" (func $log (param i32)))
  (import "GameObject" "SetActive" (func $SetActive (param externref i32)))
  (import "GameObject" "IsActive" (func $IsActive (param externref) (result i32)))
  (import "GameObject" "GetGameobjectByPath" (func $GetGameobjectByPath (param i32) (result externref)))
  (import "MovementSystem" "setPos" (func $setPos (param i32 i32 i32)))
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
    (local $obj externref)
    i32.const 94
    call $log
    i32.const 128
    call $GetGameobjectByPath
    local.set $obj
    local.get $obj
    local.get $obj
    call $IsActive
    i32.const 1
	i32.xor
    call $SetActive
  )
  (func $event
    i32.const 64
    call $log
    i32.const 93
    i32.const 87
    i32.const -40
    call $setPos
  )
    
  (export "OnApplicationStart" (func $OnApplicationStart))
  (export "OnApplicationLateStart" (func $OnApplicationLateStart))
  (export "event" (func $event))
  (export "event2" (func $event2))
)