namespace Core2D.CollisionBoxes

open Common.Types
open Godot
open Common.Extensions

type IHurtBox =
    abstract member InvincibleStartedEvent: Event<unit> with get, set
    abstract member InvincibleEndedEvent: Event<unit> with get, set

    [<CLIEvent>]
    abstract OnInvincibleStartedEventHandler: IEvent<unit>

    [<CLIEvent>]
    abstract OnInvincibleEndedEventHandler: IEvent<unit>

    abstract member OnTimerTimeout: unit -> unit
    abstract member StartInvincibility: unit -> unit
    abstract member OnHurtboxInvincibilityStarted: unit -> unit
    abstract member OnHurtboxInvincibilityEnded: unit -> unit

type HurtBoxFs() =
    inherit Area2D()
    let mutable invincibleStartedEvent = Event<unit>()
    let mutable invincibilityEndedEvent = Event<unit>()

    let mutable isInvincible = false

    [<Export>]
    member val InvincibleTime = 0.6f with get, set

    [<Export>]
    member val EffectForce = 50.0f with get, set

    member val Timer: Timer option = None with get, set
    member val CollisionShape: CollisionShape2D option = None with get, set

    member this.IsInvincible
        with get () = isInvincible
        and set value =
            isInvincible <- value
            //todo: see if this stack overflow can still can be reproduced
            //todo: figure out why this is causing a stack overflow when shooting enemy

            match value with
            | true -> (this :> IHurtBox).InvincibleStartedEvent.Trigger()
            | false -> (this :> IHurtBox).InvincibleEndedEvent.Trigger()

    member this.OnTimerTimeout() = (this :> IHurtBox).OnTimerTimeout()

    member this.OnHurtboxInvincibilityStarted() =
        (this :> IHurtBox).OnHurtboxInvincibilityStarted()

    member this.OnHurtboxInvincibilityEnded() =
        (this :> IHurtBox).OnHurtboxInvincibilityEnded()

    member this.InvincibleStartedEvent
        with get () = (this :> IHurtBox).InvincibleStartedEvent
        and set value = (this :> IHurtBox).InvincibleStartedEvent <- value

    member this.InvincibleEndedEvent = (this :> IHurtBox).InvincibleEndedEvent


    member this.AppendTimerSignals(currentTimer: Timer option) =
        match currentTimer with
        | Some timer ->
            let conn =
                SignalConnection.Default Signals.Timer.Timeout this timer (nameof this.OnTimerTimeout)

            match timer.TryConnectSignal conn with
            | err when err <> Error.Ok ->
                GD.PrintErr(
                    "Could not connect signal: "
                    + Signals.Timer.Timeout
                    + " on Timer in Hurtbox attached to owner "
                    + this.Owner.Name
                )
            | _ -> ()
        | None -> GD.PrintErr("Timer not set on Hurtbox attached to owner " + this.Owner.Name)

        currentTimer

    member this.InstantiateTimer path =
        this.GetNodeOption<Timer> path |> this.AppendTimerSignals

    member this.InstantiateCollisionShape path =
        this.GetNodeOption<CollisionShape2D> path

    member this.StartInvincibility() = (this :> IHurtBox).StartInvincibility

    override this._Ready() =
        this.Timer <- this.InstantiateTimer "Timer"
        this.CollisionShape <- this.GetNodeOption<CollisionShape2D> "CollisionShape"

    //todo: properly dispose of event handlers on cleanup
    member this.OnInvincibleStartedEventHandler =
        (this :> IHurtBox).OnInvincibleStartedEventHandler

    member this.OnInvincibleEndedEventHandler =
        (this :> IHurtBox).OnInvincibleEndedEventHandler

    interface IHurtBox with

        member this.OnHurtboxInvincibilityEnded() =
            match this.CollisionShape with
            | Some cs -> cs.Disabled <- false
            | None -> GD.PrintErr("OnHurtboxInvincibilityEnded failed due to no CollisionShape being set")

        member this.OnHurtboxInvincibilityStarted() =
            match this.CollisionShape with
            | Some cs -> cs.SetDeferred("disabled", true)
            | None -> GD.PrintErr("OnHurtboxInvincibilityStarted failed due to no CollisionShape being set")

        [<CLIEvent>]
        override this.OnInvincibleEndedEventHandler =
            (this :> IHurtBox).InvincibleEndedEvent.Publish

        [<CLIEvent>]
        override this.OnInvincibleStartedEventHandler =
            (this :> IHurtBox).InvincibleStartedEvent.Publish

        member this.OnTimerTimeout() =
            match this.Timer with
            | Some timer ->
                timer.Stop()
                this.IsInvincible <- false
                invincibilityEndedEvent.Trigger()
            | None -> failwith ("Timer not set on Hurtbox attached to owner " + this.Owner.Name)

        member this.StartInvincibility() =
            this.IsInvincible <- true

            match this.Timer with
            | Some timer ->
                timer.Start()
                invincibleStartedEvent.Trigger()
            | None -> GD.PrintErr("Timer not set on Hurtbox owned by " + this.Owner.Name)
        member this.InvincibleEndedEvent
            with get () = invincibilityEndedEvent
            and set value = invincibilityEndedEvent <- value

        member this.InvincibleStartedEvent
            with get () = invincibleStartedEvent
            and set value = invincibleStartedEvent <- value

type HitBoxFS() =
    inherit Area2D()

    [<Export>]
    member val Damage = 1 with get, set

    [<Export>]
    member val EffectForce = 50.0f with get, set
