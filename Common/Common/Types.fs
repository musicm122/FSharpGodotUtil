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

    member this.GetVelocityInMoveDirection (velocity: Vector2) (speed: float32) : Vector2 =
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
    { MethodName: string
      Source: Godot.Object
      Target: Object
      Signal: string
      Args: Godot.Collections.Array option }

    static member Default signal target source methodName =
        { MethodName = methodName
          Source = source
          Target = target
          Signal = signal
          Args = None }

    member this.GetSigFailMessage(error: Error) =
        $@"-------------------------------------
        ConnectBodyEntered with args failed with {error.ToString()}
        TryConnectSignal args
        signal:{this.Signal}
        source:{this.Source}
        target: {this.Target.ToString()}
        methodName :{this.MethodName}
        -------------------------------------"

exception GodotSignalConnectionFailureException of (SignalConnection * Error)
exception GodotSignalConnectionFailureDetailedException of (SignalConnection * Error * string)

exception GodotSignalDisconnectionFailureException of SignalConnection * SignalDisconnectionProblem
exception GodotAudioSignalException of Godot.Error * string
exception GodotSignalException of Godot.Error * string
