namespace Core2D

open Godot

[<Signal>]
type TargetReached = delegate of unit -> unit

[<Signal>]
type PathChanged = delegate of Vector2 [] -> unit

type NavClient2D() =
    inherit NavigationAgent2D()
    member this.PathChanged: option<Vector2 [] -> unit> = None
    member this.TargetReached: option<unit -> unit> = None
    member this.VelocityComputed: option<Vector2 -> unit> = None
    member this.MoveToTarget: option<Vector2 -> unit> = None

    member val Velocity = Vector2(0.0f, 0.0f) with get, set

    member val OwnerBody: KinematicBody2D = null with get, set

    [<Export>]
    member val Speed: float32 = 10f with get, set

    [<Export>]
    member val TargetPath: NodePath = null with get, set

    [<Export>]
    member val Target: KinematicBody2D = null with get, set

    member this.HasArrivedAtTarget = base.IsNavigationFinished()

    member this.OnPathChanged() = this.EmitSignal(nameof PathChanged)

    member this.OnVelocityComputed safeVelocity =
        match this.VelocityComputed with
        | Some vc -> vc safeVelocity
        | None -> ()

        if this.HasArrivedAtTarget then
            this.EmitSignal(nameof TargetReached, Array.empty<Vector2>)
            this.EmitSignal(nameof TargetReached)
        else
            match this.MoveToTarget with
            | Some movToTarget -> movToTarget safeVelocity
            | _ -> failwith "Missing required move to target callback"

    // member this.processMovement()=
    //     let moveDirection = this.Owner.Pos Position.DirectionTo(NavAgent.GetNextLocation());
    //         Velocity = moveDirection * Speed;
    //         NavAgent.SetVelocity(Velocity);
    //         SetTargetLocation(Target.GlobalPosition);
    member this.GetDirectionToTarget() =
        this.OwnerBody.Position.DirectionTo(this.GetNextLocation())

    member this.SetNavServerEdgeConnectionMargin margin =
        let rids = Navigation2DServer.GetMaps()

        for ridObj in rids do
            Navigation2DServer.MapSetEdgeConnectionMargin((ridObj :?> RID), margin)

        ()

    override this._Ready() =
        this.OwnerBody <- this.GetOwner<KinematicBody2D>()
        let pathChangedSignal = "path_changed"
        let velocityComputedSignal = "velocity_computed"
        this.SetNavServerEdgeConnectionMargin 400f

        match this.Connect(pathChangedSignal, this, nameof this.OnPathChanged) with
        | Error.Ok -> ()
        | err ->
            failwith (
                "Signal "
                + pathChangedSignal
                + "Failed to connect in NavClient2D with err code "
                + err.ToString()
            )

        match this.Connect(velocityComputedSignal, this, nameof this.OnVelocityComputed) with
        | Error.Ok -> ()
        | err ->
            failwith (
                "Signal "
                + velocityComputedSignal
                + "Failed to connect in NavClient2D with err code "
                + err.ToString()
            )

        match this.TargetPath with
        | null -> this.SetTargetLocation this.OwnerBody.Position
        | targetPath ->
            this.Target <- this.GetNode<KinematicBody2D>(targetPath)
            this.SetTargetLocation this.Target.GlobalPosition



    override this._PhysicsProcess _ =
        this.Velocity <- this.GetDirectionToTarget () * this.Speed
        this.SetVelocity(this.Velocity)
        this.SetTargetLocation(this.Target.GlobalPosition)
