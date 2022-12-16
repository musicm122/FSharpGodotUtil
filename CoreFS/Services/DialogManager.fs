namespace CoreFS.Services

open System.Threading.Tasks
open Common.Interfaces
open CoreFS.Events
open Godot
open Common
open CoreFS.Services.ThirdParty.Dialogic

type DialogManager() =
    inherit Node()

    member val dialogCompleteCallback: (unit -> unit) option = None with get, set

    member this.PlayerInteractingCompleted =
        DialogEvents.DialogInteractionComplete.Publish

    member this.PauseEvent =
        PauseEvents.Pause.Publish

    member this.UnpauseEvent =
        PauseEvents.Unpause.Publish

    member this.OnDialogListener(arg: System.Object) =
        GD.Print(
            "DialogManager.OnDialogListener called with "
            + arg.ToString()
        )

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

    interface IDialogManager with
        member this.PauseForCompletion seconds =
            GD.Print(
                "PauseForCompletion starting with "
                + seconds.ToString()
                + "seconds remaining\r\n"
            )

            // let runner =
            //     (task {
            //         GD.Print(
            //             "PauseForCompletion in task... waiting for  "
            //             + seconds.ToString()
            //             + "\r\n"
            //         )
            //
            //         return this.WaitForSeconds seconds
            //     })
            //
            // runner.ConfigureAwait(false) |> ignore
            // runner

        member this.DialogListener(listenerArg: System.Object) =
            let me = this :> IDialogManager
            (this :> IPauseable).Pause()
            this.OnDialogListener(listenerArg)
            me.DialogComplete()
            
        // Task.Run(fun () -> me.DialogComplete() |> Async.AwaitTask)
        // |> ignore

        member this.StartDialog (owner: Node) (dialogArg: DialogArg) =
            try
                this.dialogCompleteCallback <- dialogArg.onComplete

                GD.Print(
                    "DialogManager.StartDialog called with args "
                    + dialogArg.ToString()
                )
                DialogEvents.DialogInteractionStart.Trigger()

                let dialog =
                    DialogicSharp.Start(dialogArg.timeline)

                let result =
                    dialog.Connect(Timeline_End.AsString(), this, nameof this.DialogListener)

                match result with
                | Error.Ok -> owner.AddChild(dialog)
                | _ ->
                    GD.PrintErr(
                        "DialogManager.StartDialog failed with args"
                        + dialogArg.ToString()
                    )
            with
            | ex ->
                GD.PrintErr(
                    "DialogManager.StartDialog failed with args"
                    + dialogArg.ToString()
                )

                failwith "StartDialog failed. Check error log for details."

        member this.DialogComplete() =            
            GD.Print "\tDialogManager.DialogComplete() call\r\n"
            this.DialogCompleted() 

// member this.DialogComplete() : Task =
//     let me = this :> IDialogManager
//     let waitTime = 0.2f
//
//     GD.Print("DialogManager.DialogComplete called")
//     DialogEvents.DialogInteractionComplete.Trigger()
//     task {
//         let! awaiter = me.PauseForCompletion waitTime
//         return me.DialogComplete()
//     }
