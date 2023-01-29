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
    { Id: Guid
      Name: string
      Description: string
      MaxCarryAmount: int
      PickupSoundPath: option<string>
      ImagePath: option<string> }

type ItemDefinition =
    { Category: ItemCategory
      Id: Guid
      Name: string
      Description: string
      MaxCarryAmount: int
      Weight: float32
      PickupSoundPath: option<string>
      UsageSoundPath: option<string>
      ImagePath: option<string> }

    static member Default =
        { Id = Guid.NewGuid()
          Category = ItemCategory.Other
          Name = ""
          Description = ""
          MaxCarryAmount = 99
          Weight = 1.0f
          PickupSoundPath = None
          UsageSoundPath = None
          ImagePath = None }

type ItemInstance =
    { Definition: ItemDefinition
      Count: int }

    static member Default(definition) = { Definition = definition; Count = 1 }

type InventoryEventArgs(item: ItemInstance) =
    inherit EventArgs()
    member this.Item = item

type IInventoryManager =
 
    [<CLIEvent>]
    abstract OnRemoveItemEventHandler: IEvent<InventoryEventArgs>

    abstract member AddItems: ItemInstance -> int -> unit
    abstract member RemoveItems: ItemInstance -> int -> unit
    abstract member HasItemInInventory: string -> bool
    abstract member HasMinimumAmountOfItem: string -> int -> bool
    abstract member GetItemsOfCategory: ItemCategory -> IEnumerable<ItemInstance>
    abstract member GetAllItems: unit -> IEnumerable<ItemInstance>

type IDatabase =
    //abstract member saveItem: item: ItemDefinition -> SaveSta
    abstract member FindById: id: Guid -> Option<ItemDefinition>
    abstract member GetByCategory: category: ItemCategory -> IEnumerable<ItemDefinition>
    abstract member GetAllDefinitions: unit -> IEnumerable<ItemDefinition>

type JsonItemDatabase(jsonFilePath: string) =
    let db = readOnlyDict []

    member this.FindById id = (this :> IDatabase).FindById id

    member this.GetByCategory category =
        (this :> IDatabase).GetByCategory category

    member this.All = (this :> IDatabase).GetAllDefinitions ()

    member this.RawJson =
        let vals = db.Values |> Seq.map Json.serialize
        let out = vals |> String.concat ("," + Environment.NewLine)
        "[" + out + "]"

    member this.UpdateFile =
        try
            // let options = new FileStreamOptions()
            // options.Mode <- FileMode.OpenOrCreate
            // options.Share <- FileShare.Write
            // options.Access <- FileAccess.Write
            //use sw = new StreamWriter(jsonFilePath,options)
            use sw = new StreamWriter(jsonFilePath)
            sw.Write this.RawJson
            DatabaseSaveStatus.Success
        with
        | ex -> DatabaseSaveStatus.Failure ex.Message

    interface IDatabase with
        member this.GetAllDefinitions() = db.Values

        member this.FindById id =
            match db.ContainsKey id with
            | true -> Some db[id]
            | _ -> None

        member this.GetByCategory category =
            db.Values |> Seq.filter (fun def -> def.Category = category)
