namespace Core3D.Components

open Common.Uti
open Common.Extensions
open Core3D
open Godot


[<Tool>]
type PlayerConfigFS() =
    inherit Resource()

    [<Export>]
    member val gravityMultiplier = 3.0f with get, set

    [<Export>]
    member val speed = 10.0f with get, set

    [<Export>]
    member val acceleration = 8.0f with get, set

    [<Export>]
    member val deceleration = 10.0f with get, set

    [<Export(PropertyHint.Range, "0.01,100,0.01")>]
    member val airControl = 0.3f with get, set

    [<Export>]
    member val jumpHeight = 10.0f with get, set

    static member Default() : PlayerConfigFS = new PlayerConfigFS()


[<AllowNullLiteral>]
type MovementControllerFS() =
    inherit KinematicBody()

    member val CanMove = true with get, set

    [<Export>]
    member val playerConfig = PlayerConfigFS.Default() with get, set

    member val inputAxis = Vector2.Zero with get, set
    member val direction = Vector3.Zero with get, set
    member val velocity = Vector3.Zero with get, set
    member val snap = Vector3.Zero with get, set
    member val upDirection = Vector3.Up with get, set
    member val stopOnSlope = true with get, set
    member val floorMaxAngle = Mathf.Deg2Rad(45.0f) with get, set
    member val gravity = 0.0f with get, set

    override this._Ready() =

        DialogEvents.DialogInteractionStart.Publish.Add(fun _ -> this.CanMove <- false)

        DialogEvents.DialogInteractionComplete.Publish.Add(fun _ -> this.CanMove <- true)

        this.gravity <- ProjectSetting.getGravity * this.playerConfig.gravityMultiplier
        GD.Print("Current Gravity = " + this.gravity.ToString())
        ()

    member this.GetCurrentSnap(delta: float32) =
        - this.GetFloorNormal() - this.GetFloorVelocity() * delta

    override this._PhysicsProcess delta =

        let inline accelerate velocity delta =
            PhysicsUtil.calculatePlayerAcceleration
                this.direction
                this.playerConfig.speed
                this.playerConfig.acceleration
                this.playerConfig.deceleration
                this.playerConfig.airControl
                velocity
                (this.IsOnFloor())
                delta

        let inline move velocity =
            this.MoveAndSlideWithSnap(velocity, this.snap, this.upDirection, this.stopOnSlope, 4, this.floorMaxAngle)

        let inline slopeVelocityCheck (vel: Vector3) =
            if vel.y < 0.0f then vel.WithY(0.0f) else vel

        this.inputAxis <- InputUtil.getInputAxis ()
        this.direction <- InputUtil.getDirectionInput this.GlobalTransform.basis this.inputAxis

        match this.IsOnFloor() with
        | true ->
            this.snap <- this.GetCurrentSnap delta
            // Workaround for sliding down after jump on slope
            this.velocity <- slopeVelocityCheck this.velocity

            if this.velocity.y < 0f then
                this.velocity <- this.velocity.WithY(0f)

            if this.CanMove && InputUtil.IsJumpJustPressed() then
                this.snap <- Vector3.Zero
                this.velocity <- this.velocity.WithY(this.playerConfig.jumpHeight)
        | false ->
            // Workaround for 'vertical bump' when going off platform
            if this.snap <> Vector3.Zero && this.velocity.y <> 0.0f then
                this.velocity <- this.velocity.WithY(0f)

            this.snap <- Vector3.Zero
            this.velocity <- this.velocity.SubFromY(this.gravity * delta)

        if this.CanMove then
            this.velocity <- accelerate this.velocity delta |> move

[<AllowNullLiteral>]
type HeadFS() =
    inherit Spatial()

    [<Export>]
    member val mouseSensitivity = 2.0f with get, set

    [<Export>]
    member val yLimit = 90.0f with get, set

    member val mouseAxis = Vector2() with get, set
    member val rot = Vector3() with get, set

    [<Export>]
    member val cameraPath = new NodePath("Camera") with get, set

    member val camera: Camera = null with get, set

    override this._Ready() =
        this.camera <- this.GetNode(this.cameraPath) :?> Camera
        this.mouseSensitivity <- this.mouseSensitivity / 1000f
        this.yLimit <- Mathf.Deg2Rad(this.yLimit)

    member this.CameraUpdate() =
        //let newY = this.mouseAxis.x * this.mouseSensitivity
        //let newX = Mathf.Clamp(this.rot.x - this.mouseAxis.y  * this.mouseSensitivity, (-this.yLimit), this.yLimit)
        let newY = this.mouseAxis.x * this.mouseSensitivity

        let newX =
            Mathf.Clamp(this.rot.x - this.mouseAxis.y * this.mouseSensitivity, (-this.yLimit), this.yLimit)

        this.rot <- this.rot.SubFromY(newY).WithX(newX)

        let ownerRotation = this.GetOwnerAsSpatial().Rotation

        let newOwnerRotation = ownerRotation.WithY(this.rot.y)

        this.GetOwnerAsSpatial().Rotation <- newOwnerRotation
        this.Rotation <- this.Rotation.WithX(this.rot.x)


    override this._Input(event) =

        match event with
        | :? InputEventMouseMotion as mouseMotionEvent when Input.MouseMode = Input.MouseModeEnum.Captured ->
            this.mouseAxis <- mouseMotionEvent.Relative
            this.CameraUpdate()
        //this.rot <- this.updateCameraRotation this.rot this.mouseAxis this.mouseSensitivity this.yLimit
        // todo: issue with mouse movement update mode
        // todo: mouse cursor capture functionality * in the main level needs to be added
        //let owner =this.GetOwnerAsSpatial()
        //owner.Rotation<-this.GetOwnerAsSpatial().Rotation.WithX(this.rot.x)
        //this.GetOwnerAsSpatial().Rotation <- this.GetOwnerAsSpatial().Rotation.WithX(this.rot.x)

        | _ -> ()

[<AllowNullLiteral>]
type SprintFS() =
    inherit Node()

    member val controller: MovementControllerFS = null with get, set
    member val head: HeadFS = null with get, set
    member val camera: Camera = null with get, set

    [<Export>]
    member val controllerPath = new NodePath("../") with get, set

    [<Export>]
    member val headPath = new NodePath("../") with get, set

    [<Export>]
    member val sprintSpeed = 16.0f with get, set

    [<Export>]
    member val fovMultiplier = 1.05f with get, set

    [<Export>]
    member val normalSpeed = 0f with get, set

    [<Export>]
    member val normalFov = 0f with get, set

    override this._Ready() =
        this.controller <- this.GetNode<MovementControllerFS>(this.controllerPath)
        this.head <- this.GetNode<HeadFS>(this.headPath)
        this.camera <- this.head.camera

        this.head.camera <- this.camera
        this.normalFov <- this.camera.Fov
        this.normalSpeed <- this.controller.playerConfig.speed

    member this.CanSprint(controller: MovementControllerFS) =
        controller.IsOnFloor()
        && InputUtil.IsSprintPressed()
        && controller.inputAxis.x >= 0.0f

    override this._PhysicsProcess(delta: float32) =

        let inline calcFOV (lerpTo: float32) =
            let lerpWeight = 8.0f
            CameraUtil.calculateCameraFieldOfView this.camera.Fov lerpTo lerpWeight delta

        let multipliedFov = (this.normalFov * this.fovMultiplier)

        match this.CanSprint this.controller with
        | true ->
            this.controller.playerConfig.speed <- this.sprintSpeed
            this.camera.Fov <- calcFOV multipliedFov
        | false ->
            this.controller.playerConfig.speed <- this.normalSpeed
            this.camera.Fov <- calcFOV this.normalFov
