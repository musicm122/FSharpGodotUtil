namespace Common.Services.ThirdParty.Dialogic

open Godot
open System

type GCArray = Godot.Collections.Array
type GCDictionary = Godot.Collections.Dictionary


type DialogicSignals =
    | DialogicSignal
    | EventStart
    | EventEnd
    | TimelineStart
    | TimelineEnd
    | AutoAdvanceToggled

    member this.AsString() =
        match this with
        | DialogicSignal -> "dialogic_signal"
        | EventStart -> "event_start"
        | EventEnd -> "event_end"
        | TimelineStart -> "timeline_start"
        | TimelineEnd -> "timeline_end"
        | AutoAdvanceToggled -> "auto_advance_toggled"

    static member All() =
        [| "dialogic_signal"
           "event_start"
           "event_end"
           "timeline_start"
           "timeline_end"
           "auto_advance_toggled" |]

type DialogicSharp() =

    static member val private _dialogic =
        GD.Load<Script>("res://addons/dialogic/Other/DialogicClass.gd") with get, set

    static member val private DEFAULT_DIALOG_RESOURCE = "res://addons/dialogic/Nodes/DialogNode.tscn" with get, set

    static member Start(timeline: string) : Node =
        let callResult =
            DialogicSharp._dialogic.Call("start", timeline, String.Empty, DialogicSharp.DEFAULT_DIALOG_RESOURCE, true)

        let result = callResult :?> Node
        result

    static member StartWithAllArgs<'T>
        (timeline: string)
        (default_timeline: string)
        (dialogScenePath: string)
        (useCanvasInstead: bool)
        =
        DialogicSharp._dialogic.Call("start", timeline, default_timeline, dialogScenePath, useCanvasInstead) :?> 'T

    static member Load(?slotName: String) =
        let slot_name = (defaultArg slotName) ""
        DialogicSharp._dialogic.Call("load", slot_name)

    static member Save(?slotName: String) =
        let slot_name = (defaultArg slotName) ""
        DialogicSharp._dialogic.Call("save", slot_name)

    static member GetSlotNames() =
        DialogicSharp._dialogic.Call("get_slot_names") :?> GCArray

    static member EraseSlot(slot_name: String) =
        DialogicSharp._dialogic.Call("erase_slot", slot_name)

    static member HasCurrentDialogNode() =
        DialogicSharp._dialogic.Call("has_current_dialog_node") :?> Boolean

    static member ResetSaves() =
        DialogicSharp._dialogic.Call("reset_saves")

    static member GetCurrentSlot() =
        DialogicSharp._dialogic.Call("get_current_slot") :?> String

    static member Export() =
        DialogicSharp._dialogic.Call("export") :?> GCDictionary

    static member Import(data: GCDictionary) =
        DialogicSharp._dialogic.Call("import", data)

    static member GetVariable(name: String) =
        DialogicSharp._dialogic.Call("get_variable", name) :?> String

    static member SetVariable(name: String, value: String) =
        DialogicSharp._dialogic.Call("set_variable", name, value)

    static member CurrentTimeline
        with get () = DialogicSharp._dialogic.Call("get_current_timeline") :?> String
        and set (value: String) = DialogicSharp._dialogic.Call("set_current_timeline", value) |> ignore

    static member Definitions =
        DialogicSharp._dialogic.Call("get_definitions") :?> GCDictionary

    static member DefaultDefinitions =
        DialogicSharp._dialogic.Call("get_default_definitions") :?> GCDictionary

    static member Autosave
        with get () = DialogicSharp._dialogic.Call("get_autosave") :?> Boolean
        and set (value: Boolean) = DialogicSharp._dialogic.Call("set_autosave", value) |> ignore
