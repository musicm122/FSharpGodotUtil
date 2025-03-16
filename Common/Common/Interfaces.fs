namespace Common.Interfaces

open System
open System.Collections.Generic
open Common.Types
open Godot

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

[<Interface>]
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

[<Interface>]
type IPauseable =
    abstract Pause: unit -> unit
    abstract Unpause: unit -> unit
    abstract TogglePause: unit -> unit


[<Interface>]
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

type IServices =
    { Logger: ILogger
      PauseManager: IPauseable
      AudioManager: IPlayAudio
      DialogManager: IDialogManager
      InventoryManager: IInventoryManager
      Database: IDatabase }
