namespace Core2D.Effects

open Godot

type BloodSpatterFS() =
    inherit CPUParticles2D()
    
    [<Export>]
    member val TargetGlobalPosition: Vector2 = Vector2.Zero with get, set

    [<Export>]
    member val TimerNodePath = new NodePath("Timer") with get, set

    member val Timer = null with get, set

    member this.GetGlobalPositionOfNode2d nodeName =
        (this.GetTree().CurrentScene.FindNode(nodeName) :?> Node2D)
            .GlobalPosition

    member this.OnTimeout() = this.QueueFree()

    member this.UpdateDirection() =
        let targetPos = this.TargetGlobalPosition

        if targetPos.x > this.GlobalPosition.x then
            this.Direction <- Vector2(-1f, this.Direction.y)

        if targetPos.x < this.GlobalPosition.x then
            this.Direction <- Vector2(1f, this.Direction.y)

    override this._Ready() =
        this.Timer <- this.GetNode<Timer>(this.TimerNodePath)

        this.Timer.Connect(Signals.Timer.Timeout, this, nameof this.OnTimeout)
        |> ignore

        this.UpdateDirection ()
        this.Emitting <- true
