namespace Common

open Common.Types
open Common.Uti
open Godot
open System.ComponentModel
open System
open System.Reflection
open Microsoft.FSharp.Reflection


[<AutoOpen>]
module Extensions =

    type Godot.Object with

        member this.TryConnectSignal(signalConnection: SignalConnection) =

            let result =
                signalConnection.Source.Connect(
                    signalConnection.Signal,
                    signalConnection.Target,
                    signalConnection.MethodName
                )

            match result with
            | Error.Ok -> result
            | err ->
                let msg = signalConnection.GetSigFailMessage err

                let ex = GodotSignalConnectionFailureException(signalConnection, err)

                GD.PrintErr(msg, ex)
                GD.PrintStack()
                raise ex

        member this.TryDisconnectSignal(signalConnection: SignalConnection) =
            try
                if this.HasSignal(signalConnection.Signal) then
                    this.Disconnect(signalConnection.Signal, signalConnection.Target, signalConnection.MethodName)
                else
                    raise (
                        GodotSignalDisconnectionFailureException(
                            signalConnection,
                            SignalDisconnectionProblem.DoesNotHaveSignal
                        )
                    )
            with ex ->
                raise (
                    GodotSignalDisconnectionFailureException(
                        signalConnection,
                        SignalDisconnectionProblem.OtherException ex
                    )
                )

    type Area with

        member this.ConnectAreaEntered target methodName =
            { MethodName = methodName
              Source = this
              Target = target
              SignalConnection.Signal = Godot.Signals.Area.AreaEntered
              Args = None }
            |> this.TryConnectSignal

        member this.ConnectAreaExited target methodName =
            { MethodName = methodName
              Source = this
              Target = target
              SignalConnection.Signal = Godot.Signals.Area.AreaExited
              Args = None }
            |> this.TryConnectSignal

        member this.ConnectAreaShapeEntered target methodName =
            { MethodName = methodName
              Source = this
              Target = target
              SignalConnection.Signal = Godot.Signals.Area.AreaShapeEntered
              Args = None }
            |> this.TryConnectSignal

        member this.ConnectAreaShapeExited target methodName =
            { MethodName = methodName
              Source = this
              Target = target
              SignalConnection.Signal = Godot.Signals.Area.AreaShapeExited
              Args = None }
            |> this.TryConnectSignal

        member this.ConnectBodyEntered target methodName =
            //this.Connect("body_entered", this, nameof this.OnExaminableBodyEntered)
            { MethodName = methodName
              Source = this
              Target = target
              SignalConnection.Signal = Godot.Signals.Area.BodyEntered
              Args = None }
            |> this.TryConnectSignal

        member this.ConnectBodyExited target methodName =
            { MethodName = methodName
              Source = this
              Target = target
              SignalConnection.Signal = Godot.Signals.Area.BodyExited
              Args = None }
            |> this.TryConnectSignal

        member this.ConnectBodyShapeEntered target methodName =
            { MethodName = methodName
              Source = this
              Target = target
              SignalConnection.Signal = Godot.Signals.Area.BodyShapeEntered
              Args = None }
            |> this.TryConnectSignal

        member this.ConnectBodyShapeExited target methodName =
            { MethodName = methodName
              Source = this
              Target = target
              SignalConnection.Signal = Godot.Signals.Area.BodyShapeExited
              Args = None }
            |> this.TryConnectSignal

        member this.DisconnectAreaEntered target methodName =
            { MethodName = methodName
              Source = this
              Target = target
              SignalConnection.Signal = Godot.Signals.Area.AreaEntered
              Args = None }
            |> this.TryDisconnectSignal

        member this.DisconnectAreaExited target methodName =
            { MethodName = methodName
              Source = this
              Target = target
              SignalConnection.Signal = Godot.Signals.Area.AreaExited
              Args = None }
            |> this.TryDisconnectSignal

        member this.DisconnectAreaShapeEntered target methodName =
            { MethodName = methodName
              Source = this
              Target = target
              SignalConnection.Signal = Godot.Signals.Area.AreaShapeEntered
              Args = None }
            |> this.TryDisconnectSignal

        member this.DisconnectAreaShapeExited target methodName =
            { MethodName = methodName
              Source = this
              Target = target
              SignalConnection.Signal = Godot.Signals.Area.AreaShapeExited
              Args = None }
            |> this.TryDisconnectSignal

        member this.DisconnectBodyEntered target methodName =
            { MethodName = methodName
              Source = this
              Target = target
              SignalConnection.Signal = Godot.Signals.Area.BodyEntered
              Args = None }
            |> this.TryDisconnectSignal

        member this.DisconnectBodyExited target methodName =
            { MethodName = methodName
              Source = this
              Target = target
              SignalConnection.Signal = Godot.Signals.Area.BodyExited
              Args = None }
            |> this.TryDisconnectSignal

        member this.DisconnectBodyShapeEntered target methodName =
            { MethodName = methodName
              Source = this
              Target = target
              SignalConnection.Signal = Godot.Signals.Area.BodyShapeEntered
              Args = None }
            |> this.TryDisconnectSignal

        member this.DisconnectBodyShapeExited target methodName =
            { MethodName = methodName
              Source = this
              Target = target
              SignalConnection.Signal = Godot.Signals.Area.BodyShapeExited
              Args = None }
            |> this.TryDisconnectSignal

    type Area2D with
    
        member this.ConnectAreaEntered target methodName = 
            { MethodName = methodName
              Source = this
              Target = target
              SignalConnection.Signal = Godot.Signals.Area2D.AreaEntered
              Args = None }
            |> this.TryConnectSignal
            
        member this.ConnectAreaExited target methodName = 
            { MethodName = methodName
              Source = this
              Target = target
              SignalConnection.Signal = Godot.Signals.Area2D.AreaExited
              Args = None }
            |> this.TryConnectSignal
            
        member this.ConnectAreaShapeEntered target methodName = 
            { MethodName = methodName
              Source = this
              Target = target
              SignalConnection.Signal = Godot.Signals.Area2D.AreaShapeEntered
              Args = None }
            |> this.TryConnectSignal
            
        member this.ConnectAreaShapeExited target methodName = 
            { MethodName = methodName
              Source = this
              Target = target
              SignalConnection.Signal = Godot.Signals.Area2D.AreaShapeExited
              Args = None }
            |> this.TryConnectSignal
            
        member this.ConnectBodyEntered target methodName = 
            { MethodName = methodName
              Source = this
              Target = target
              SignalConnection.Signal = Godot.Signals.Area2D.BodyEntered
              Args = None }
            |> this.TryConnectSignal
            
        member this.ConnectBodyExited target methodName = 
            { MethodName = methodName
              Source = this
              Target = target
              SignalConnection.Signal = Godot.Signals.Area2D.BodyExited
              Args = None }
            |> this.TryConnectSignal
            
        member this.ConnectBodyShapeEntered target methodName = 
            { MethodName = methodName
              Source = this
              Target = target
              SignalConnection.Signal = Godot.Signals.Area2D.BodyShapeEntered
              Args = None }
            |> this.TryConnectSignal
            
        member this.ConnectBodyShapeExited target methodName = 
            { MethodName = methodName
              Source = this
              Target = target
              SignalConnection.Signal = Godot.Signals.Area2D.BodyShapeExited
              Args = None }
            |> this.TryConnectSignal
            
    type Button with

        member this.ConnectButtonPressed source target methodName =
            { MethodName = methodName
              Source = source
              Target = target
              SignalConnection.Signal = Godot.Signals.Button.Pressed
              Args = None }
            |> this.TryConnectSignal
        
        member this.ConnectButtonDown source target methodName =
            { MethodName = methodName
              Source = source
              Target = target
              SignalConnection.Signal = Godot.Signals.Button.ButtonDown
              Args = None }
            |> this.TryConnectSignal
            
        member this.ConnectButtonUp source target methodName =
            { MethodName = methodName
              Source = source
              Target = target
              SignalConnection.Signal = Godot.Signals.Button.ButtonUp
              Args = None }
            |> this.TryConnectSignal                        
            
        member this.ConnectButtonToggle source target methodName =
            { MethodName = methodName
              Source = source
              Target = target
              SignalConnection.Signal = Godot.Signals.Button.Toggled
              Args = None }
            |> this.TryConnectSignal                
    
    
    type InputEventMouseButton with

        member this.IsMouseButtonPressed(btnListItem: ButtonList) =
            this.ButtonIndex = int btnListItem && this.Pressed

        member this.IsMouseButtonDoubleClicked(btnListItem: ButtonList) =
            this.ButtonIndex = int btnListItem && this.Doubleclick

        member this.IsMouseButtonReleased(btnListItem: ButtonList) =
            this.ButtonIndex = int btnListItem && not this.Pressed

        member this.IsLeftButtonPressed() =
            this.IsMouseButtonPressed ButtonList.Left

        member this.IsLeftButtonDoubleClicked() =
            this.IsMouseButtonDoubleClicked ButtonList.Left

        member this.IsLeftButtonReleased() =
            this.IsMouseButtonReleased ButtonList.Left

        member this.IsRightButtonPressed() =
            this.IsMouseButtonPressed ButtonList.Right

        member this.IsRightButtonReleased() =
            this.IsMouseButtonReleased ButtonList.Right

    type Godot.Collections.Array with

        member this.ToList<'A>() =
            [ for item in this do
                  item :?> 'A ]

        member this.ToSeq<'A>() =
            seq {
                for i in 0 .. this.Count do
                    yield (this.Item(i) :?> 'A)
            }

        member this.ToArray<'A>() =
            let arr = Array.zeroCreate<'A> this.Count

            for i = 0 to this.Count do
                arr[i] <- (this.Item(i) :?> 'A)

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

        member this.GetGlobalPositionOfNode2d nodeName =
            (this.CurrentScene.FindNode(nodeName) :?> Node2D).GlobalPosition

        member this.GetNodesByType<'A>() =
            this
                .CurrentScene
                .GetChildren()
                .Filter(fun node ->
                    match node with
                    | :? 'A -> true
                    | _ -> false)

    type Node with

        member this.IsPaused() = this.GetTree().Paused

        member this.Pause() = this.GetTree().Paused
        member this.Unpause() = this.GetTree().Paused |> not

        member this.TogglePause() =
            this.GetTree().Paused = this.GetTree().Paused <> this.GetTree().Paused

        member this.ReloadScene() = this.GetTree().ReloadCurrentScene()

        member this.IsPlayer() = this.Name.ToLowerInvariant() = "player"

        member this.WaitForSeconds seconds =
            task { return this.ToSignal(this.GetTree().CreateTimer(seconds), Godot.Signals.Timer.Timeout) }

        member this.GetOwnerAsSpatial() = this.Owner :?> Spatial
        member inline this.GetOwnerAs<'A when 'A :> Node>() = this.Owner :?> 'A

        member this.GetNode<'A when 'A :> Node and 'A: not struct>(path: string) =
            lazy (this.GetNode<'A>(new NodePath(path)))

        member this.GetChildrenOfSeq() = this.GetChildren() |> Seq.cast<Node>

        member this.GetNodeOption<'A when 'A :> Node and 'A: not struct and 'A: equality and 'A: null> path =
            match this.GetNode<'A>(new NodePath(path)) with
            | node when isNull node -> None
            | node -> Some node

    type Area2D with

        member this.HasLineOfSight(point: Vector2) =
            let spaceState = this.GetWorld2d().DirectSpaceState

            let result =
                spaceState.IntersectRay(this.GlobalTransform.origin, point, null, this.CollisionMask)

            match result with
            | intersection when not(isNull intersection) && intersection.Count > 0 -> true
            | _ -> false

    type Node2D with

        member this.FireInDirection dir speed delta = this.Translate(dir * speed * delta)

        member this.FireAtAngle (speed: float32) (delta: float32) =
            let angle = this.Rotation - Mathf.Pi / 2f

            let dir = Vector2(cos angle, - sin(angle))

            this.FireInDirection dir speed delta

        member this.PrintLocalPosition() =
            GD.Print("Postion x, y :", this.Position.x.ToString(), this.Position.y.ToString())

        member this.PrintGlobalPosition() =
            GD.Print("Postion x, y :", this.GlobalPosition.x.ToString(), this.GlobalPosition.y.ToString())

        member this.PrintLocalPosition(msg) =
            GD.Print(msg + " Postion x, y :", this.Position.x.ToString(), this.Position.y.ToString())

        member this.PrintGlobalPosition(msg) =
            GD.Print(msg + " Postion x, y :", this.GlobalPosition.x.ToString(), this.GlobalPosition.y.ToString())

        member this.GetNodeLazy<'A when 'A :> Node and 'A: not struct>(path: string) =
            lazy (this.GetNode<'A>(new NodePath(path)))

        member this.DrawCircleArc center radius angleFrom angleTo color =
            let pts = 32
            let range = [| 0 .. (pts + 1) |]

            let pointsArc =
                range
                |> Array.map (fun (i: int) ->
                    let anglePoint =
                        (float32) angleFrom
                        + (float32) i * ((float32) angleTo - (float32) angleFrom) / (float32) pts
                        - 90f
                        |> Mathf.Deg2Rad

                    center + Vector2(Mathf.Cos(anglePoint), Mathf.Sin(anglePoint)) * radius)

            range
            |> Array.iter (fun i -> this.DrawLine(pointsArc[i], pointsArc[i + 1], color))

        member this.GetChildrenOfType<'A when 'A :> Node and 'A: not struct>() =
            this.GetChildrenOfSeq()
            |> Seq.filter (fun testType ->
                let t1 = (testType.GetType().AssemblyQualifiedName)
                let t2 = (typeof<'A>).AssemblyQualifiedName
                t1 = t2)

    type Vector2 with

        member this.WithX(newX) = Vector2(newX, this.y)

        member this.AddToX(newX) = Vector2(this.x + newX, this.y)
        member this.SubFromX(newX) = Vector2(this.x - newX, this.y)

        member this.WithY(newY) = Vector2(this.x, newY)

        member this.AddToY(newY) = Vector2(this.x, this.y + newY)
        member this.SubFromY(newY) = Vector2(this.x, this.y - newY)

        member this.Add(v2: Vector2) =
            let x = this.x + v2.x
            let y = this.y + v2.y
            Vector2(x, y)

        member this.Subtract(v2: Vector2) =
            let x = this.x - v2.x
            let y = this.y - v2.y
            Vector2(x, y)


        member this.RotateCounterClockwiseByAngle radianAngle =
            //x' = x cos θ − y sin θ
            //y' = x sin θ + y cos θ
            //newX = oldX * cos(angle) - oldY * sin(angle)
            //newY = oldX * sin(angle) + oldY * cos(angle)
            let newX = (this.x * Mathf.Cos(radianAngle)) - (this.y * Mathf.Sin(radianAngle))
            let newY = (this.x * Mathf.Sin(radianAngle)) + (this.y * Mathf.Cos(radianAngle))
            Vector2(newX, newY)

    type Vector3 with

        member this.WithX(newX) = Vector3(newX, this.y, this.z)

        member this.AddToX(newX) = Vector3(this.x + newX, this.y, this.z)
        member this.SubFromX(newX) = Vector3(this.x - newX, this.y, this.z)

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
            if not (isNull this) then
                let body = this.Collider :?> PhysicsBody
                body.IsInGroup groupName
            else
                false

    type KinematicBody with

        member this.GetAllColliders() : Option<seq<KinematicCollision>> =
            if isNull this then
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
