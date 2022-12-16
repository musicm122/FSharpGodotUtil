namespace Common.Interfaces

open System.Threading.Tasks
open Common.Uti
open Godot

type DialogArg =
    { timeline: string
      methodName: string
      shouldRemove: bool
      onComplete: (unit -> unit) option }

type IPauseable =
    abstract Pause: unit -> unit
    abstract Unpause: unit -> unit

type ICanApplyPause =
    abstract OnPause: unit -> unit
    abstract OnUnpause: unit -> unit

[<Interface>]
type IExaminer =
    abstract member PlayerInteracting: IEvent<unit>
    abstract member PlayerInteractingComplete: IEvent<unit>
    abstract member PlayerInteractingAvailable: IEvent<unit>
    abstract member PlayerInteractingUnavailable: IEvent<unit>

[<Interface>]
type ILogger =
    abstract debug: obj [] -> unit
    abstract error: obj [] -> unit

[<Interface>]
type IDialogManager =
    abstract member DialogListener: System.Object -> unit
    abstract member DialogComplete: unit -> unit
    abstract member StartDialog: Node -> DialogArg -> unit
    abstract member PauseForCompletion: float32 -> unit

[<Interface>]
type IExaminable =
    abstract member OnExaminableBodyEntered: Node -> unit
    abstract member OnExaminableBodyExited: Node -> unit

type ScreenShakeInstance =
    { Duration: float32
      mutable RemainingDuration: float32
      mutable x :float32
      mutable y :float32
      Frequency: float32
      Amplitude: float32 }

    member this.Intensity() =
        this.Amplitude * (1.0f - ((this.Duration - this.RemainingDuration) / this.Duration))

    member this.PeriodInMs() =
        1.0f / this.Frequency

    member private this.newPoint(previous, delta) =
        let newX = MathUtils.getRandomPosNegOne()
        float32(this.Intensity()) * (previous + (delta * (newX - previous)))

    member this.newPointXY(oldX, oldY, delta) =
        new Vector2(this.newPoint(oldX,delta), this.newPoint(oldY,delta))

    member this.UpdatePos(newX,newY) =
        this.x<-newX
        this.y<-newY
        new Vector2(newX, newY)

    //member this.getShakeOffset(delta)=
        //if this.RemainingDuration = 0.0 then (false, Vector2.Zero)
        //this.last_shook_timer<-this.last_shook_timer + delta

        //while this.last_shook_timer >= this.PeriodInMs do
        //    this.last_shook_timer<- this.last_shook_timer - this.PeriodInMs
        //    let intensity = this.CurrentShake.Intensity(this.timer)
        //    let newOffset =
        //        this.newPointXY(intensity, this.x, this.y, delta)
        //        |> this.UpdatePos
        //    (true, newOffset)

    static member Default() =
        { Duration = 0.0f
          RemainingDuration = 0.0f
          x = 0f
          y = 0f
          Frequency = 0.0f
          Amplitude = 0.0f }
