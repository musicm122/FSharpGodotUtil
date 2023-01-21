namespace Common.Reader

[<Struct>]
type Effect<'Env, 'Out> = Effect of ('Env -> 'Out)

module Effect =
    /// Create value with no dependency requirements.
    let inline value (x: 'Out) : Effect<'Env, 'Out> = Effect(fun _ -> x)
    /// Create value which uses depenendency.
    let inline apply (fn: 'Env -> 'Out) : Effect<'Env, 'Out> = Effect fn
    let run (env: 'Env) (Effect fn) : 'Out = fn env

    let inline bind (fn: 'A -> Effect<'Env, 'B>) effect =
        Effect (fun env ->
            let x = run env effect // compute result of the first effect
            run env (fn x) // run second effect, based on result of first one
        )

[<Struct>]
type EffectBuilder =
    member inline __.Return value = Effect.value value
    member inline __.Zero() = Effect.value (Unchecked.defaultof<_>)
    member inline __.ReturnFrom(effect: Effect<'Env, 'Out>) = effect
    member inline __.Bind(effect, fn) = Effect.bind fn effect


[<Interface>]
type ILogger =
    abstract Debug: obj [] -> unit
    abstract Error: obj [] -> unit



[<Interface>]
type ILog =
    abstract Logger: ILogger

type IPlayAudio =
    abstract member PlaySound: string -> unit
