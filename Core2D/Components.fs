namespace Core2D.Components

open Common.Constants
open Common.Types
open Godot
open Common.Extensions

[<Tool>]
type DebugDrawNode() =
    inherit Node2D()

    [<Export>]
    member val Radius = 10f with get, set

    [<Export>]
    member val Color = Color(255f, 0f, 0f) with get, set

    override this._Draw() =
        this.DrawCircle(Vector2(0f, 0f), this.Radius, this.Color)

type PauserFs() =
    inherit Godot.Node()

    member this.Label = this.GetNode<Label>(new NodePath("PauseText"))

    member private this.TogglePause() =
        this.GetTree().Paused <- not (this.GetTree().Paused)
        this.GetTree().Paused

    member this.PauseCheck() =

        match Input.IsActionJustPressed(InputActions.Pause) with
        | true ->
            match this.TogglePause() with
            | true -> this.Label.Show()
            | false -> this.Label.Hide()
        | _ -> ignore ()

    override this._Ready() =
        this.Label.Hide()
        this.GetTree().Paused <- false

    override this._Process(delta) = this.PauseCheck()


