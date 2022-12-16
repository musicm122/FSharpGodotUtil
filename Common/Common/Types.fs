namespace Common.Types

open System
open Godot

type MoveDirection =
    | Left
    | Right
    | Up
    | Down
    | UpRight
    | DownRight
    | UpLeft
    | DownLeft

    member this.GetVelocityInMoveDirection (velocity: Vector2) (speed: float32) =
        let halfSpeed = speed * 0.5f

        match this with
        | Left -> Vector2(velocity.x - speed, 0f)
        | Right -> Vector2(velocity.x + speed, 0f)
        | Up -> Vector2(0f, velocity.y - speed)
        | Down -> Vector2(0f, velocity.y + speed)
        | UpLeft -> Vector2(velocity.x - halfSpeed, velocity.y - halfSpeed)
        | UpRight -> Vector2(velocity.x + halfSpeed, velocity.y - halfSpeed)
        | DownRight -> Vector2(velocity.x + halfSpeed, velocity.y + halfSpeed)
        | DownLeft -> Vector2(velocity.x - halfSpeed, velocity.y + halfSpeed)


type MissingSignalField =
    | Source
    | Target
    | MethodName
    | SignalName
    | OkField

type SignalProblem =
    | MissingField of MissingSignalField
    | InvalidSignal of string
    | ConnectionError of Error
    | OkSignal

type SignalDisconnectionProblem =
    | DoesNotHaveSignal
    | OtherException of Exception
    | OkDisconnection

type SignalConnection =
    { methodName: string
      target: Godot.Object
      signal: string
      args: Godot.Collections.Array option }

    static member Default(signal, target, methodName) =
        { methodName = methodName
          target = target
          signal = signal
          args = None }

    member this.getSigFailMessage(error: Error) =
        $@"-------------------------------------
        ConnectBodyEntered with args failed with {error.ToString()}
        TryConnectSignal args
        signal:{this.signal}
        target: {this.target.ToString()}
        methodName :{this.methodName}
        -------------------------------------"

exception GodotSignalConnectionFailure of SignalConnection * Error
exception GodotSignalDisconnectionFailure of SignalConnection * SignalDisconnectionProblem
exception GodotAudioSignalException of Godot.Error * string
