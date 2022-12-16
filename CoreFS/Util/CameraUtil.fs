namespace CoreFS

open Godot

module CameraUtil =
    let calculateCameraFieldOfView currentFov lerpTo lerpWeight delta =
        Mathf.Lerp(currentFov, lerpTo, delta * lerpWeight)
