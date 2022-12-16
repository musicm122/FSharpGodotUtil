namespace CoreFS.Services.ThirdParty.Dialogic

open Godot
open System

type GC_Array = Godot.Collections.Array
type GC_Dictionary = Godot.Collections.Dictionary


type DialogicSignals =
    | Dialogic_Signal
    | Event_Start
    | Event_End
    | Timeline_Start
    | Timeline_End
    | Auto_Advance_Toggled
    member this.AsString() =
        match this with
        | Dialogic_Signal -> "dialogic_signal"
        | Event_Start -> "event_start"
        | Event_End -> "event_end"
        | Timeline_Start -> "timeline_start"
        | Timeline_End -> "timeline_end"
        | Auto_Advance_Toggled -> "auto_advance_toggled"

    static member All() =
        [| "dialogic_signal"
           "event_start"
           "event_end"
           "timeline_start"
           "timeline_end"
           "auto_advance_toggled" |]

type DialogicSharp() =

    static member val private _dialogic =
        ((GD.Load<Script>("res://addons/dialogic/Other/DialogicClass.gd"))) with get, set

    static member val private DEFAULT_DIALOG_RESOURCE = ("res://addons/dialogic/Nodes/DialogNode.tscn") with get, set

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
        (DialogicSharp._dialogic.Call("start", timeline, default_timeline, dialogScenePath, useCanvasInstead)) :?> 'T

    // static member _Start<'T> (?timeline: String) (?default_timeline: String) (?dialogScenePath: String) (?useCanvasInstead: System.Boolean) =
    //     let timeline = (defaultArg timeline) ""
    //
    //     let default_timeline =
    //         (defaultArg default_timeline) ""
    //
    //     let dialogScenePath =
    //         (defaultArg dialogScenePath) ""
    //
    //     let useCanvasInstead =
    //         (defaultArg useCanvasInstead) true
    //     DialogicSharp.callStartWithAllArgs<'T> timeline default_timeline DialogicSharp.DEFAULT_DIALOG_RESOURCE useCanvasInstead
    //(DialogicSharp._dialogic.Call("start", timeline, default_timeline, dialogScenePath, useCanvasInstead)) :?> 'T

    //static member Start(?timeline: String) (?default_timeline: String) (?useCanvasInstead: System.Boolean) =

    // static member Start (?timeline:string) (?default_timeline:string) (?useCanvasInstead:bool) =
    //     let timeline = (defaultArg timeline) ""
    //     let default_timeline = (defaultArg default_timeline) ""
    //     let useCanvasInstead = (defaultArg useCanvasInstead) true
    //     DialogicSharp.callStartWithAllArgs<Node> timeline default_timeline DialogicSharp.DEFAULT_DIALOG_RESOURCE useCanvasInstead
    //
    //result
    //let result = Start timeline default_timeline DialogicSharp.DEFAULT_DIALOG_RESOURCE useCanvasInstead
    //result
    //Start.Start<Node>(timeline, default_timeline, DialogicSharp.DEFAULT_DIALOG_RESOURCE, useCanvasInstead)

    static member Load(?slot_name: String) =
        let slot_name = (defaultArg slot_name) ""
        DialogicSharp._dialogic.Call("load", slot_name)

    static member Save(?slot_name: String) =
        let slot_name = (defaultArg slot_name) ""
        DialogicSharp._dialogic.Call("save", slot_name)

    static member GetSlotNames() =
        (DialogicSharp._dialogic.Call("get_slot_names")) :?> GC_Array

    static member EraseSlot(slot_name: String) =
        DialogicSharp._dialogic.Call("erase_slot", slot_name)

    static member HasCurrentDialogNode() =
        (DialogicSharp._dialogic.Call("has_current_dialog_node")) :?> System.Boolean

    static member ResetSaves() =
        DialogicSharp._dialogic.Call("reset_saves")

    static member GetCurrentSlot() =
        (DialogicSharp._dialogic.Call("get_current_slot")) :?> String

    static member Export() =
        (DialogicSharp._dialogic.Call("export")) :?> GC_Dictionary

    static member Import(data: GC_Dictionary) =
        DialogicSharp._dialogic.Call("import", data)

    static member GetVariable(name: String) =
        (DialogicSharp._dialogic.Call("get_variable", name)) :?> String

    static member SetVariable(name: String, value: String) =
        DialogicSharp._dialogic.Call("set_variable", name, value)

    static member CurrentTimeline
        with get () = (DialogicSharp._dialogic.Call("get_current_timeline")) :?> String
        and set (value: String) =
            DialogicSharp._dialogic.Call("set_current_timeline", value)
            |> ignore

    static member Definitions =
        (DialogicSharp._dialogic.Call("get_definitions")) :?> GC_Dictionary

    static member DefaultDefinitions =
        (DialogicSharp._dialogic.Call("get_default_definitions")) :?> GC_Dictionary

    static member Autosave
        with get () = (DialogicSharp._dialogic.Call("get_autosave")) :?> System.Boolean
        and set (value: Boolean) =
            DialogicSharp._dialogic.Call("set_autosave", value)
            |> ignore
