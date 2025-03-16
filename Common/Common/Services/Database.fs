namespace Common.Database

open System
open System.IO
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

