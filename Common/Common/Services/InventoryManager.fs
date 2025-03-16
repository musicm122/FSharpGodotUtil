namespace Common.Manager

open System
open System.Collections.Generic
open Common.Interfaces
open Common.Types


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
                //todo: detail out io errors vs logic errors
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
            this.Items |> Seq.filter (fun item -> item.Definition.Name = name) |> Seq.length = amount


        [<CLIEvent>]
        member this.OnRemoveItemEventHandler =
            this.InventoryManagerImpl.OnRemoveItemEventHandler

        member this.RemoveItems item count =
            let index = this.Items.FindIndex(fun i -> i.Definition = item.Definition)

            match this.Items[index].Count with
            | currentCount when currentCount >= count ->
                let newItem = this.Items[index]
                this.Items[ index ] <- { newItem with Count = (currentCount - count) }
                RemoveItemResult.Success this.Items[index]
            | _ -> RemoveItemResult.Fail "Not enough of item"
