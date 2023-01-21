namespace Common.Services

open Godot
open Common.Reader

module Log =
    type Logger() =
        interface ILogger with
            member this.Debug fmt =
                let ap s =
                    Effect.apply (fun (x: #ILog) -> x.Logger.Debug s)

                GD.Print(fmt, ap)

            member this.Error fmt =
                let ap s =
                    Effect.apply (fun (x: #ILog) -> x.Logger.Error s)

                GD.PrintErr(fmt, ap)

    let (live: ILogger) = Logger()

type Log() =
    interface ILogger with
        member this.Debug(args) = GD.Print args
        member this.Error(args) = GD.PrintErr args
