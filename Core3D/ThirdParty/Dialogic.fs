namespace Core3D.ThirdParty.Dialogic

namespace CoreFS.Component

open System.Threading.Tasks
open Common.Global
open Common.Interfaces
open Common.Services
open Common.Types
open Common.Uti
open Godot
open Common.Extensions

type ExamineFS() =
    inherit Spatial()

    member val Log = Global.Log()

    member val DialogManager: DialogManager = Global.DialogManager()

    member this.PlayerInteractionAvailabilityChangeEvent =
        DialogEvents.PlayerInteractionAvailabilityChange.Publish

    member this.OnInteractionComplete() =
        this.Log.Debug [| "Examinable.OnInteractionComplete called\r\n" |]

    member val CanInteract = false with get, set

    [<Export>]
    member val ShouldRemove = false with get, set

    interface IExaminable with
        member this.OnExaminableBodyEntered(body: Node) =
            if body.IsPlayer() then
                this.Log.Debug [| "Examinable.OnExaminableBodyEntered called \r\n" |]
                DialogEvents.PlayerInteractionAvailabilityChange.Trigger(true)
                this.CanInteract <- true

        member this.OnExaminableBodyExited(body: Node) =
            if body.IsPlayer() then
                this.Log.Debug [| "Examinable.OnExaminableAreaExited called \r\n" |]
                DialogEvents.PlayerInteractionAvailabilityChange.Trigger(false)
                this.CanInteract <- false

    member this.OnExaminableBodyEntered(body: Node) =
        (this :> IExaminable)
            .OnExaminableBodyEntered(body)

    member this.OnExaminableBodyExited(body: Node) =
        (this :> IExaminable).OnExaminableBodyExited(body)


    [<Export>]
    member val areaPath = new NodePath("../") with get, set

    member val InteractableArea: Area = null with get, set

    [<Export>]
    member val Timeline = "" with get, set

    member this.OnLeftClick (camera: Node) (event: InputEventMouseButton) clickPos clickNormal shapeId =
        this.Log.Debug [| "Left clicked " + this.Name |]

    member this.OnRightClick (camera: Node) (event: InputEventMouseButton) clickPos clickNormal shapeId =
        this.Log.Debug [| "Right clicked " + this.Name |]

    member this.OnAreaInput
        (camera: Node)
        (event: InputEvent)
        (clickPos: Vector3)
        (clickNormal: Vector3)
        (shapeId: int)
        =
        match event with
        | :? InputEventMouseButton as ie ->
            match ie with
            | ie when ie.IsLeftButtonPressed () -> this.OnLeftClick camera ie clickPos clickNormal shapeId
            | ie when ie.IsRightButtonPressed () -> this.OnRightClick camera ie clickPos clickNormal shapeId
            | _ -> ()
        | _ -> ()

    member this.DialogListener(listenerArg: System.Object) =
        GD.Print("In Examinable.DialogListener with args " + listenerArg.ToString() + "\r\n")
  
    member this.StartDialog() =
        let args =
            { DialogArg.timeline = this.Timeline
              shouldRemove = this.ShouldRemove
              methodName = nameof this.DialogListener
              onComplete = Some this.OnInteractionComplete }

        let dialogManager =
            this.DialogManager :> IDialogManager

        dialogManager.StartDialog this args

    override this._Ready() =
        this.InteractableArea <- this.GetNode<Area>(this.areaPath)

        this.Log.Debug [| "InteractableArea connection to Area BodyEntered "
                          this.InteractableArea.Connect(
                              Signals.Area.BodyEntered,
                              this,
                              nameof this.OnExaminableBodyEntered
                          ) |]

        this.Log.Debug [| "InteractableArea connection to Area BodyExited "
                          this.InteractableArea.Connect(
                              Signals.Area.BodyExited,
                              this,
                              nameof this.OnExaminableBodyExited
                          ) |]

    override this._Process(_delta) =
        if this.CanInteract
           && InputUtil.IsInteractionJustPressed() then
            this.CanInteract <- false
            this.StartDialog ()

type IndicatorFS() =
    inherit Area()

    member val Log = Global.Log()

    member val DialogManager: DialogManager = Global.DialogManager()
    member val CanInteract = false with get, set

    [<Export>]
    member val ShouldRemove = false with get, set

    member this.OnExaminableBodyEntered(body: Node) =
        if body.IsPlayer() then
            this.Log.Debug [| "OnExaminableBodyEntered" |]
            DialogEvents.PlayerInteractionAvailabilityChange.Trigger(true)
            this.CanInteract <- true

    member this.OnExaminableBodyExited(body: Node) =
        if body.IsPlayer() then
            this.Log.Debug [| "OnExaminableAreaExited" |]
            DialogEvents.PlayerInteractionAvailabilityChange.Trigger(false)
            this.CanInteract <- false

    override this._Ready() =
        this.Log.Debug [| "IndicatorFS OnReady" |]
