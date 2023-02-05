namespace Common.Manager

open System
open System.IO
open System.Collections.Generic
open Common.Interfaces
open Common.Types
open FSharp.Json

type JsonItemDatabase(jsonFilePath: string) =
    let db = readOnlyDict []

    member this.FindById id = (this :> IDatabase).FindById id

    member this.GetByCategory category =
        (this :> IDatabase).GetByCategory category

    member this.All = (this :> IDatabase).GetAllDefinitions()

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
        with ex ->
            DatabaseSaveStatus.Failure ex.Message

    interface IDatabase with
        member this.GetAllDefinitions() = db.Values

        member this.FindById id =
            match db.ContainsKey id with
            | true -> Some db[id]
            | _ -> None

        member this.GetByCategory category =
            db.Values |> Seq.filter (fun def -> def.Category = category)


type InventoryManager(db: IDatabase) =
    //let addItemEvent = new Event<InventoryEventArgs>()
    // let removeItemEvent = new Event<InventoryEventArgs>()
    
    member this.InventoryManagerImpl = (this :> IInventoryManager)
    member this.ItemDb = db
    member this.Items = List<ItemInstance>()

    interface IInventoryManager with
        member this.AddItems item count =
            try
                seq { 0..count }
                |> Seq.map (fun i -> item)
                |> Seq.append this.Items
                |> AddItemResult.Success
            with :? Exception as ex ->
                AddItemResult.Fail ex.Message

        member this.GetAllItems() = this.Items

        member this.GetItemsOfCategory category =
            try 
                this.Items
                |> Seq.filter (fun item -> item.Definition.Category = category)
                |> GetItemOfCategoryResult.Success
            with :? Exception as ex ->
                 GetItemOfCategoryResult.Fail ex.Message
                 
        member this.HasItemInInventory itemName =
            this.Items |> Seq.exists (fun item -> item.Definition.Name = itemName)

        member this.HasMinimumAmountOfItem name amount =
            this.Items
            |> Seq.filter (fun item -> item.Definition.Name = name)
            |> Seq.length = amount


        [<CLIEvent>]
        member this.OnRemoveItemEventHandler = this.InventoryManagerImpl.OnRemoveItemEventHandler

        member this.RemoveItems item count =
            let index = this.Items.FindIndex(fun i -> i.Definition = item.Definition)            
            match this.Items[index].Count with
            | currentCount when currentCount >= count ->
                let newItem = this.Items[index]
                this.Items[index] <- { newItem with Count = (currentCount - count) }
                RemoveItemResult.Success this.Items[index]
            | _ ->
                RemoveItemResult.Fail "Not enough of item"