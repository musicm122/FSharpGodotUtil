namespace CoreFS.Services

open Common.Interfaces
open Godot

type Log() =
    interface ILogger with
        member this.Debug(args) = GD.Print args
        member this.Error(args) = GD.PrintErr args
        member this.Info(args) = GD.PrintS  args
