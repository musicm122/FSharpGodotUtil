namespace CoreFS.Types

open Common.Constants
open Common.Interfaces
open Common.Uti
open Godot

type PlayerState =
    | Idle
    | Move
    | Sprint
    | Jump
    | Die
    

module CustomEvents =
    let PlayerInteracting = Event<IExaminable>()

    let PlayerInteractingComplete =
        Event<unit>()

    let PlayerInteractingEvent = Event<unit>()

    let PlayerInteractingUnavailable =
        Event<unit>()

    let PlayerInteractingAvailable =
        Event<unit>()

//type Weapon =
    

[<Struct>]
type SupportedInput =
    | UICancel
    | ChangeMouseInput

    member this.AsString =
        match this with
        | UICancel -> InputActions.UICancel
        | ChangeMouseInput -> InputActions.ChangeMouseInput

    static member Cases =
        DIUtil.UnionCasesOf<SupportedInput>()

    static member InputStrings() =
        SupportedInput.Cases
        |> Array.map (fun case -> case.AsString)
