﻿namespace Common.Services

open Common.Events
open Common.Types
open Godot
open Common.Interfaces
open Common.Services.ThirdParty.Dialogic


module DialogEvents =
    let DialogInteractionStart = Event<unit>()

    let DialogInteractionComplete = Event<unit>()

    let PlayerInteractionAvailabilityChange = Event<bool>()

type DialogManager() =
    inherit Node()

    member val dialogCompleteCallback: (unit -> unit) option = None with get, set

    member this.PlayerInteractingCompleted = DialogEvents.DialogInteractionComplete.Publish

    member this.PauseEvent = PauseEvents.Pause.Publish

    member this.UnpauseEvent = PauseEvents.Unpause.Publish

    member this.OnDialogListener(arg: System.Object) =
        GD.Print("DialogManager.OnDialogListener called with " + arg.ToString())

    member this.DialogListener(listenerArg: System.Object) =
        let me = this :> IDialogManager
        me.DialogListener(listenerArg)

    member this.DialogCompleted() =
        match this.dialogCompleteCallback with
        | Some onDialogComplete -> onDialogComplete ()
        | None -> ()

        this.dialogCompleteCallback <- None
        DialogEvents.DialogInteractionComplete.Trigger()


    interface IPauseable with
        member this.Pause() = PauseEvents.Pause.Trigger()
        member this.Unpause() = PauseEvents.Pause.Trigger()
        member this.TogglePause() = failwith "todo"

    interface IDialogManager with
        member this.PauseForCompletion seconds =
            GD.Print(
                "PauseForCompletion starting with "
                + seconds.ToString()
                + "seconds remaining\r\n"
            )

        member this.DialogListener(listenerArg: System.Object) =
            let me = this :> IDialogManager
            (this :> IPauseable).Pause()
            this.OnDialogListener(listenerArg)
            me.DialogComplete()

        member this.StartDialog (owner: Node) (dialogArg: DialogArg) =
            try
                this.dialogCompleteCallback <- dialogArg.OnComplete

                GD.Print("DialogManager.StartDialog called with args " + dialogArg.ToString())
                DialogEvents.DialogInteractionStart.Trigger()

                let dialog = DialogicSharp.Start(dialogArg.Timeline)

                let result =
                    dialog.Connect(TimelineEnd.AsString(), this, nameof this.DialogListener)

                match result with
                | Error.Ok -> owner.AddChild(dialog)
                | _ -> GD.PrintErr("DialogManager.StartDialog failed with args" + dialogArg.ToString())
            with
            | ex ->
                GD.PrintErr("DialogManager.StartDialog failed with args" + dialogArg.ToString())

                failwith "StartDialog failed. Check error log for details."

        member this.DialogComplete() =
            GD.Print "\tDialogManager.DialogComplete() call\r\n"
            this.DialogCompleted()
