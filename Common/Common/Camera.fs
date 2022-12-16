namespace Common.Camera

open Common.Interfaces
open Common.Uti
open Godot

type PlayerCameraFs() =
    inherit Camera2D()

    member val CurrentShake: ScreenShakeInstance = ScreenShakeInstance.Default() with get, set

    //member val duration = 0.0 with get, set
    //member val period_in_ms = 0.0 with get, set
    //member val amplitude = 0.0 with get, set
    //member val timer = 0.0 with get, set
    member val last_shook_timer = 0f with get, set
    member val previous_x = 0.0f with get, set
    member val previous_y = 0.0f with get, set
    member val last_offset = Vector2.Zero with get, set

    member this.scheduleShake(shake: ScreenShakeInstance) : unit =
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
                this.CurrentShake.newPointXY (this.CurrentShake.x, this.CurrentShake.y, delta)

            this.Offset <- this.Offset - this.last_offset + newOffset
            this.last_offset <- newOffset

        this.CurrentShake.RemainingDuration <- this.CurrentShake.RemainingDuration - delta

        if this.CurrentShake.RemainingDuration <= 0f then
            this.CurrentShake.RemainingDuration <- 0f
            this.Offset <- this.Offset - this.last_offset
