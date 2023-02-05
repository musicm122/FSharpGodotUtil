namespace Common.Interfaces

open System
open System.Collections.Generic
open Common.Types
open Common.Uti
open Godot
open Microsoft.CodeAnalysis.Operations

[<Interface>]
type ILogger =
    abstract Debug: obj[] -> unit
    abstract Info: obj[] -> unit
    abstract Error: obj[] -> unit

[<Interface>]
type IDialogManager =
    abstract member DialogListener: System.Object -> unit
    abstract member DialogComplete: unit -> unit
    abstract member StartDialog: Node -> DialogArg -> unit
    abstract member PauseForCompletion: float32 -> unit

type IPlayAudio =
    abstract member PlaySound: string -> unit


type AddItemResult=
    | Success of IEnumerable<ItemInstance>
    | Fail of String

type RemoveItemResult=
    | Success of ItemInstance
    | Fail of String
    

type GetItemOfCategoryResult =
    | Success of IEnumerable<ItemInstance>
    | Fail of String

[<Interface>]
type IInventoryManager =
    [<CLIEvent>]
    abstract OnRemoveItemEventHandler: IEvent<InventoryEventArgs>

    abstract member AddItems: ItemInstance -> int -> AddItemResult
    abstract member RemoveItems: ItemInstance -> int -> RemoveItemResult
    abstract member HasItemInInventory: string -> bool
    abstract member HasMinimumAmountOfItem: string -> int -> bool
    abstract member GetItemsOfCategory: ItemCategory -> GetItemOfCategoryResult 
    abstract member GetAllItems: unit -> IEnumerable<ItemInstance>

[<Interface>]
type IDatabase =
    //abstract member saveItem: item: ItemDefinition -> SaveSta
    abstract member FindById: id: Guid -> Option<ItemDefinition>
    abstract member GetByCategory: category: ItemCategory -> IEnumerable<ItemDefinition>
    abstract member GetAllDefinitions: unit -> IEnumerable<ItemDefinition>

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
type IExaminable =
    abstract member OnExaminableBodyEntered: Node -> unit
    abstract member OnExaminableBodyExited: Node -> unit

type ScreenShakeInstance =
    { Duration: float32
      mutable RemainingDuration: float32
      mutable X: float32
      mutable Y: float32
      Frequency: float32
      Amplitude: float32 }

    member this.Intensity() =
        this.Amplitude
        * (1.0f - ((this.Duration - this.RemainingDuration) / this.Duration))

    member this.PeriodInMs() = 1.0f / this.Frequency

    member private this.NewPoint(previous, delta) =
        let newX = MathUtils.getRandomPosNegOne ()
        float32 (this.Intensity()) * (previous + (delta * (newX - previous)))

    member this.NewPointXY(oldX, oldY, delta) =
        Vector2(this.NewPoint(oldX, delta), this.NewPoint(oldY, delta))

    member this.UpdatePos(newX, newY) =
        this.X <- newX
        this.Y <- newY
        Vector2(newX, newY)

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
          X = 0f
          Y = 0f
          Frequency = 0.0f
          Amplitude = 0.0f }


type IServices =
    { Logger: ILogger
      Pauser: IPauseable
      AudioManager: IPlayAudio
      DialogManager: IDialogManager
      InventoryManager: IInventoryManager
      Database: IDatabase }
