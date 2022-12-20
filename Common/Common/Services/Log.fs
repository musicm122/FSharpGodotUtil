namespace Common.Services

open Common.Interfaces
open Godot
open Common.Reader

module Log =
    type Logger()=
        interface ILogger with  
            member this.debug fmt =
                let ap s = Effect.apply (fun (x: #ILog) -> x.Logger.debug s)
                GD.Print(fmt, ap) 
                
            member this.error fmt =
                let ap s = Effect.apply (fun (x: #ILog) -> x.Logger.error s)
                GD.PrintErr(fmt, ap) 

    let (live:ILogger) = Logger()

type Log() =
    interface ILogger with
        member this.debug(args) = GD.Print args
        member this.error(args) = GD.PrintErr args