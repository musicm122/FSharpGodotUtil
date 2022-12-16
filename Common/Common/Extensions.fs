namespace Common

open Common.Types
open Godot

[<AutoOpen>]
module Extensions =

    type Godot.Object with
        member this.TryConnectSignal(signalConnection: SignalConnection) =
            //this.Connect("body_entered", this, nameof this.OnExaminableBodyEntered)

            let result =
                this.Connect(signalConnection.signal, this, signalConnection.methodName)

            match result with
            | Error.Ok -> result
            | err ->
                let msg =
                    signalConnection.getSigFailMessage err

                let ex =
                    GodotSignalConnectionFailure(signalConnection, err)

                GD.PrintErr(msg, ex)
                GD.PrintStack()
                raise ex

        member this.TryDisconnectSignal(signalConnection: SignalConnection) =
            try
                if this.HasSignal(signalConnection.signal) then
                    this.Disconnect(signalConnection.signal, signalConnection.target, signalConnection.methodName)
                else
                    raise (
                        GodotSignalDisconnectionFailure(signalConnection, SignalDisconnectionProblem.DoesNotHaveSignal)
                    )
            with
            | ex ->
                raise (GodotSignalDisconnectionFailure(signalConnection, SignalDisconnectionProblem.OtherException ex))

    type Area with
        member this.ConnectAreaEntered target methodName =
            { methodName = methodName
              target = target
              SignalConnection.signal = Godot.Signals.Area.AreaEntered
              args = None }
            |> this.TryConnectSignal

        member this.ConnectAreaExited target methodName =
            { methodName = methodName
              target = target
              SignalConnection.signal = Godot.Signals.Area.AreaExited
              args = None }
            |> this.TryConnectSignal

        member this.ConnectAreaShapeEntered target methodName =
            { methodName = methodName
              target = target
              SignalConnection.signal = Godot.Signals.Area.AreaShapeEntered
              args = None }
            |> this.TryConnectSignal

        member this.ConnectAreaShapeExited target methodName =
            { methodName = methodName
              target = target
              SignalConnection.signal = Godot.Signals.Area.AreaShapeExited
              args = None }
            |> this.TryConnectSignal

        member this.ConnectBodyEntered target methodName =
            //this.Connect("body_entered", this, nameof this.OnExaminableBodyEntered)

            { methodName = methodName
              target = target
              SignalConnection.signal = Godot.Signals.Area.BodyEntered
              args = None }
            |> this.TryConnectSignal

        member this.ConnectBodyExited target methodName =
            { methodName = methodName
              target = target
              SignalConnection.signal = Godot.Signals.Area.BodyExited
              args = None }
            |> this.TryConnectSignal

        member this.ConnectBodyShapeEntered target methodName =
            { methodName = methodName
              target = target
              SignalConnection.signal = Godot.Signals.Area.BodyShapeEntered
              args = None }
            |> this.TryConnectSignal

        member this.ConnectBodyShapeExited target methodName =
            { methodName = methodName
              target = target
              SignalConnection.signal = Godot.Signals.Area.BodyShapeExited
              args = None }
            |> this.TryConnectSignal

        member this.DisconnectAreaEntered target methodName =
            { methodName = methodName
              target = target
              SignalConnection.signal = Godot.Signals.Area.AreaEntered
              args = None }
            |> this.TryDisconnectSignal

        member this.DisconnectAreaExited target methodName =
            { methodName = methodName
              target = target
              SignalConnection.signal = Godot.Signals.Area.AreaExited
              args = None }
            |> this.TryDisconnectSignal

        member this.DisconnectAreaShapeEntered target methodName =
            { methodName = methodName
              target = target
              SignalConnection.signal = Godot.Signals.Area.AreaShapeEntered
              args = None }
            |> this.TryDisconnectSignal

        member this.DisconnectAreaShapeExited target methodName =
            { methodName = methodName
              target = target
              SignalConnection.signal = Godot.Signals.Area.AreaShapeExited
              args = None }
            |> this.TryDisconnectSignal

        member this.DisconnectBodyEntered target methodName =
            { methodName = methodName
              target = target
              SignalConnection.signal = Godot.Signals.Area.BodyEntered
              args = None }
            |> this.TryDisconnectSignal

        member this.DisconnectBodyExited target methodName =
            { methodName = methodName
              target = target
              SignalConnection.signal = Godot.Signals.Area.BodyExited
              args = None }
            |> this.TryDisconnectSignal

        member this.DisconnectBodyShapeEntered target methodName =
            { methodName = methodName
              target = target
              SignalConnection.signal = Godot.Signals.Area.BodyShapeEntered
              args = None }
            |> this.TryDisconnectSignal

        member this.DisconnectBodyShapeExited target methodName =
            { methodName = methodName
              target = target
              SignalConnection.signal = Godot.Signals.Area.BodyShapeExited
              args = None }
            |> this.TryDisconnectSignal

    type InputEventMouseButton with
        member this.isMouseButtonPressed(btnListItem: ButtonList) =
            this.ButtonIndex = int btnListItem && this.Pressed

        member this.isMouseButtonDoubleClicked(btnListItem: ButtonList) =
            this.ButtonIndex = int btnListItem
            && this.Doubleclick

        member this.isMouseButtonReleased(btnListItem: ButtonList) =
            this.ButtonIndex = int btnListItem
            && this.Pressed = false

        member this.isLeftButtonPressed() =
            this.isMouseButtonPressed ButtonList.Left

        member this.isLeftButtonDoubleClicked() =
            this.isMouseButtonDoubleClicked ButtonList.Left

        member this.isLeftButtonReleased() =
            this.isMouseButtonReleased ButtonList.Left

        member this.isRightButtonPressed() =
            this.isMouseButtonPressed ButtonList.Right

        member this.isRightButtonReleased() =
            this.isMouseButtonReleased ButtonList.Right


    type Godot.Collections.Array with
        member this.toList<'a>() =
            [ for item in this do
                  item :?> 'a ]

        member this.toSeq<'a>() =
            seq {
                for i in 0 .. this.Count do
                    yield (this.Item(i) :?> 'a)
            }

        member this.toArray<'a>() =
            let arr = Array.zeroCreate<'a> this.Count

            for i = 0 to this.Count do
                arr[i] <- (this.Item(i) :?> 'a)

            arr

        member this.Map(f) =
            let result = new Godot.Collections.Array()

            for item in this do
                result.Add(f item) |> ignore

            result

        member this.Iter(f) =
            for item in this do
                f item

            ()

        member this.Filter(predicate) =
            let result = new Godot.Collections.Array()

            for item in this do
                match predicate item with
                | true -> result.Add item |> ignore
                | _ -> ()
                |> ignore

            result

    type SceneTree with
        member this.getNodesByType<'a>() =
            this
                .CurrentScene
                .GetChildren()
                .Filter(fun node ->
                    match node with
                    | :? 'a -> true
                    | _ -> false)

    type Node with

        member this.ReloadScene() = this.GetTree().ReloadCurrentScene()

        member this.IsPlayer() = this.Name.ToLowerInvariant() = "player"

        member this.WaitForSeconds seconds =
            task { return this.ToSignal(this.GetTree().CreateTimer(seconds), Godot.Signals.Timer.Timeout) }

        member this.GetOwnerAsSpatial() = this.Owner :?> Spatial
        member inline this.GetOwnerAs<'a when 'a :> Node>() = this.Owner :?> 'a

        member this.GetNode<'a when 'a :> Node and 'a: not struct>(path: string) =
            lazy this.GetNode<'a>(new NodePath(path))

        member this.getChildren() = this.GetChildren() |> Seq.cast<Node>

    type Node2D with

        (*let drawCircleArcPoly (center:Vector2) (radius:float32) (angleFrom:float32) (angleTo:float32) (color:Color) =
                let nbPoints = 32
                let pointsArc =  Array.zeroCreate 32
                pointsArc[0] = center
                let colors = [| color |]
                [ 0 .. nbPoints ].
                this.DrawLine
            *)

        member this.FireInDirection dir speed delta = this.Translate(dir * speed * delta)

        member this.FireAtAngle (dir: Vector2) (speed: float32) (delta: float32) =
            let angle = this.Rotation - Mathf.Pi / 2f

            let dir =
                new Vector2(cos angle, - sin(angle))

            this.FireInDirection dir speed delta

        member this.PrintLocalPosition() =
            GD.Print("Postion x, y :", this.Position.x.ToString(), this.Position.y.ToString())

        member this.PrintGlobalPosition() =
            GD.Print("Postion x, y :", this.GlobalPosition.x.ToString(), this.GlobalPosition.y.ToString())

        member this.PrintLocalPosition(msg) =
            GD.Print(msg + " Postion x, y :", this.Position.x.ToString(), this.Position.y.ToString())

        member this.PrintGlobalPosition(msg) =
            GD.Print(msg + " Postion x, y :", this.GlobalPosition.x.ToString(), this.GlobalPosition.y.ToString())

        member this.GetNodeLazy<'a when 'a :> Node and 'a: not struct>(path: string) =
            lazy this.GetNode<'a>(new NodePath(path))

    type Vector3 with
        member this.WithX(newX) = Vector3(newX, this.y, this.z)

        member this.AddToX(newX) = Vector3(this.x + newX, this.y, this.z)
        member this.SubToX(newX) = Vector3(this.x - newX, this.y, this.z)

        member this.WithY(newY) = Vector3(this.x, newY, this.z)

        member this.AddToY(newY) = Vector3(this.x, this.y + newY, this.z)
        member this.SubFromY(newY) = Vector3(this.x, this.y - newY, this.z)


        member this.WithZ(newZ) = Vector3(this.x, this.y, newZ)
        member this.AddToZ(newZ) = Vector3(this.x, this.y, this.z + newZ)
        member this.SubFromZ(newZ) = Vector3(this.x, this.y, this.z - newZ)

        member this.Add(v2: Vector3) =
            let x = this.x + v2.x
            let y = this.y + v2.y
            let z = this.z + v2.z
            Vector3(x, y, z)

        member this.Subtract(v2: Vector3) =
            let x = this.x - v2.x
            let y = this.y - v2.y
            let z = this.z - v2.z
            Vector3(x, y, z)

    type KinematicCollision with
        member this.ColliderIsInGroup(groupName: string) : bool =
            if this <> null then
                let body = this.Collider :?> PhysicsBody
                body.IsInGroup groupName
            else
                false

    type KinematicBody with
        member this.GetAllColliders() : Option<seq<KinematicCollision>> =
            if this = null then
                Option.None
            else
                let slideCount = this.GetSlideCount()

                match slideCount with
                | 0 -> Option.None
                | _ ->
                    let result =
                        seq {
                            for x in 0 .. this.GetSlideCount() - 1 do
                                yield this.GetSlideCollision(x)
                        }

                    Some(result)

        member this.GetAllCollidersInGroup(groupName: string) =
            let inGroup (collider: KinematicCollision) : bool = collider.ColliderIsInGroup(groupName)

            match this.GetAllColliders() with
            | Some colliders -> colliders |> Seq.filter inGroup
            | Option.None -> Seq.empty
