namespace CoreFS.Services

open Common.Interfaces
open Godot

type Log() =
    interface ILogger with
        member this.debug(args) = GD.Print args
        member this.error(args) = GD.PrintErr args
