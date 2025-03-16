namespace Common.Camera

open Common.Interfaces
open Common.Types
open Common.Uti
open Godot


type RollingCameraFs() =
    inherit KinematicBody2D()

    member val CurrentMoveDirection = MoveDirection.Up with get, set

    [<Export>]
    member val IsEnabled = true with get, set

    [<Export>]
    member val CurrentVelocity = Vector2.Zero with get, set

    member this.GetVelocityInMoveDirection() =
        this.CurrentMoveDirection.GetVelocityInMoveDirection this.CurrentVelocity this.Speed

    [<Export>]
    member val Speed = 20f with get, set

    override this._PhysicsProcess _ =
        if this.IsEnabled then
            this.GetVelocityInMoveDirection() 
            |> this.MoveAndSlide 
            |> ignore



type PlayerCameraFs() =
    inherit Camera2D()

    member val CurrentShake: ScreenShakeInstance = ScreenShakeInstance.Default() with get, set
    member val last_shook_timer = 0f with get, set
    member val previous_x = 0.0f with get, set
    member val previous_y = 0.0f with get, set
    member val last_offset = Vector2.Zero with get, set

    member this.ScheduleShake(shake: ScreenShakeInstance) : unit =
        this.CurrentShake <- shake
        this.previous_x <- MathUtils.getRandomInRange -1.0f 1.0f
        this.Offset <- this.Offset - this.last_offset
        this.last_offset <- Vector2.Zero

    override this._Ready() = this.SetProcess(true)

    //wip adapting from https://github.com/MrEliptik/godot_experiments
    override this._Process(delta) =
        if this.CurrentShake.RemainingDuration = 0.0f then ignore ()
        this.last_shook_timer <- this.last_shook_timer + delta
        let period = float32 (this.CurrentShake.PeriodInMs())

        while this.last_shook_timer >= period do
            this.last_shook_timer <- this.last_shook_timer - period

            let newOffset =
                this.CurrentShake.NewPointXY (this.CurrentShake.X, this.CurrentShake.Y, delta)

            this.Offset <- this.Offset - this.last_offset + newOffset
            this.last_offset <- newOffset

        this.CurrentShake.RemainingDuration <- this.CurrentShake.RemainingDuration - delta

        if this.CurrentShake.RemainingDuration <= 0f then
            this.CurrentShake.RemainingDuration <- 0f
            this.Offset <- this.Offset - this.last_offset
