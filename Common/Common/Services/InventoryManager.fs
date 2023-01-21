namespace Common.Manager

open System
open System.IO
open System.Collections.Generic
open FSharp.Json

type DatabaseSaveStatus =
    | Success
    | Failure of string

type ItemCategory =
    | Consumable
    | Ammo
    | Key
    | Equipment
    | Other

type CurrencyDefinition =
    { id: Guid
      name: string
      description: string
      maxCarryAmount: int
      pickupSoundPath: option<string>
      imagePath: option<string> }

type ItemDefinition =
    { category: ItemCategory
      id: Guid
      name: string
      description: string
      maxCarryAmount: int
      weight: float32
      pickupSoundPath: option<string>
      usageSoundPath: option<string>
      imagePath: option<string> }

    static member Default =
        { id = Guid.NewGuid()
          category = ItemCategory.Other
          name = ""
          description = ""
          maxCarryAmount = 99
          weight = 1.0f
          pickupSoundPath = None
          usageSoundPath = None
          imagePath = None }

type ItemInstance =
    { definition: ItemDefinition
      count: int }

    static member Default(definition) = { definition = definition; count = 1 }

type InventoryEventArgs(item: ItemInstance) =
    inherit System.EventArgs()
    member this.Item = item

type IInventoryManager =
    abstract member addItemEvent: Event<InventoryEventArgs>
    abstract member removeItemEvent: Event<InventoryEventArgs>

    [<CLIEvent>]
    abstract onAddItemEventHandler: IEvent<InventoryEventArgs>

    [<CLIEvent>]
    abstract onRemoveItemEventHandler: IEvent<InventoryEventArgs>

    abstract member addItems: ItemInstance -> int -> unit
    abstract member removeItems: ItemInstance -> int -> unit
    abstract member hasItemInInventory: string -> bool
    abstract member hasMinimumAmountOfItem: string -> int -> bool
    abstract member getItemsOfCategory: ItemCategory -> IEnumerable<ItemInstance>
    abstract member getAllItems: unit -> IEnumerable<ItemInstance>

type IDatabase =
    //abstract member saveItem: item: ItemDefinition -> SaveSta
    abstract member findById: id: Guid -> Option<ItemDefinition>
    abstract member getByCategory: category: ItemCategory -> IEnumerable<ItemDefinition>
    abstract member getAllDefinitions: unit -> IEnumerable<ItemDefinition>

type JsonItemDatabase(jsonFilePath: string) =
    let db = readOnlyDict []

    member this.findById id = (this :> IDatabase).findById id

    member this.getByCategory category =
        (this :> IDatabase).getByCategory category

    member this.all = (this :> IDatabase).getAllDefinitions ()

    member this.rawJson =
        let vals = db.Values |> Seq.map Json.serialize
        let out = vals |> String.concat ("," + Environment.NewLine)
        "[" + out + "]"

    member this.updateFile =
        try
            // let options = new FileStreamOptions()
            // options.Mode <- FileMode.OpenOrCreate
            // options.Share <- FileShare.Write
            // options.Access <- FileAccess.Write
            //use sw = new StreamWriter(jsonFilePath,options)
            use sw = new StreamWriter(jsonFilePath)
            sw.Write this.rawJson
            DatabaseSaveStatus.Success
        with
        | ex -> DatabaseSaveStatus.Failure ex.Message

    interface IDatabase with
        member this.getAllDefinitions() = db.Values

        member this.findById id =
            match db.ContainsKey id with
            | true -> Some db[id]
            | _ -> None

        member this.getByCategory category =
            db.Values |> Seq.filter (fun def -> def.category = category)
