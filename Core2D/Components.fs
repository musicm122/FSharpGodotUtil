namespace Core2D.Components

open Common.Constants
open Common.Types
open Godot

type PauserFs() =
    inherit Godot.Node()

    member this.Label = this.GetNode<Label>(new NodePath("PauseText")) 

    member private this.TogglePause() = 
        this.GetTree().Paused <- not(this.GetTree().Paused)
        this.GetTree().Paused 

    member this.PauseCheck() = 
        
        match Input.IsActionJustPressed(InputActions.Pause) with 
        | true -> 
            match this.TogglePause() with
            | true -> this.Label.Show()
            | false -> this.Label.Hide()
        | _ ->  ignore()

    override this._Ready()=
        this.Label.Hide()
        this.GetTree().Paused <- false

    override this._Process(delta) = 
        this.PauseCheck()

type RollingCameraFs() =
    inherit KinematicBody2D()

    member val CurrentMoveDirection =  MoveDirection.Up with get, set

    [<Export>]
    member val IsEnabled = true with get, set

    [<Export>]
    member val CurrentVelocity = Vector2.Zero with get, set

    member this.GetVelocityInMoveDirection() =
        this.CurrentMoveDirection.GetVelocityInMoveDirection this.CurrentVelocity this.Speed

    [<Export>]
    member val Speed = 20f with get, set

    override this._PhysicsProcess(delta) =
        if this.IsEnabled then 
            this.GetVelocityInMoveDirection()
            |> this.MoveAndSlide
            |> ignore
