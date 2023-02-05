namespace Common.Services

open Common.Interfaces
open Godot

module Log =
    let Logger: ILogger =
        { new ILogger with
            member this.Debug args = GD.Print("Debug:", args)

            member this.Info args = GD.Print("Info:", args)

            member this.Error args = GD.PrintErr(args) }

    let defaultLogger: ILogger =
        { new ILogger with
            member this.Debug args = printfn "Debug:%A" args
            member this.Info args = printfn "Info:%A" args
            member this.Error args = printfn "Error:%A" args }
