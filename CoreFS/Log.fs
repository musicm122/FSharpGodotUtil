namespace CoreFS

open Common
open Common.Log
open Godot


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
