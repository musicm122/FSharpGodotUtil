namespace Core2D.Effects

open Godot

type BloodSpatter() =
    inherit CPUParticles2D()

    [<Export>]
    member val TargetNodePath = null with get, set

    [<Export>]
    member val TimerNodePath = new NodePath("Timer") with get, set

    member val Timer = null with get, set

    member this.getGlobalPositionOfNode2d nodeName =
        (this.GetTree().CurrentScene.FindNode(nodeName) :?> Node2D)
            .GlobalPosition

    member this.OnTimeout() = this.QueueFree()

    member this.updateDirection() =
        let targetPos = this.getGlobalPositionOfNode2d this.TargetNodePath

        if targetPos.x > this.GlobalPosition.x then
            this.Direction <- Vector2(-1f, this.Direction.y)

        if targetPos.x < this.GlobalPosition.x then
            this.Direction <- Vector2(1f, this.Direction.y)

    override this._Ready() =
        this.Timer <- this.GetNode<Timer>(this.TimerNodePath)

        this.Timer.Connect(Signals.Timer.Timeout, this, nameof (this.OnTimeout))
        |> ignore

        this.updateDirection ()
        this.Emitting <- true
