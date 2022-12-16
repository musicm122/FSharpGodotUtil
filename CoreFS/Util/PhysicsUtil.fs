namespace CoreFS

open Godot
open Common.Extensions

module PhysicsUtil =
    let calculatePlayerAcceleration
        (direction: Vector3)
        (speed: float32)
        acceleration
        deceleration
        airControl
        (currentVelocity: Vector3)
        isOnFloor
        (delta: float32)
        =
        //# Using only the horizontal velocity, interpolate towards the input.
        let mutable tempV =
            currentVelocity.WithY(0f)

        let mutable acc = 0f
        let target = direction * speed

        acc <-
            if direction.Dot tempV > 0.0f then
                acceleration
            else
                deceleration

        if isOnFloor = false then
            acc <- acc * airControl

        tempV <- tempV.LinearInterpolate(target, acc * delta)

        currentVelocity.WithX(tempV.x).WithZ(tempV.z)
