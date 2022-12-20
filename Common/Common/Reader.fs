namespace Common.Reader

[<Struct>] type Effect<'env, 'out> = Effect of ('env -> 'out)

module Effect =
    /// Create value with no dependency requirements.
    let inline value (x: 'out): Effect<'env,'out> = Effect (fun _ -> x)
    /// Create value which uses depenendency.
    let inline apply (fn: 'env -> 'out): Effect<'env,'out> = Effect fn
    let run (env: 'env) (Effect fn): 'out = fn env

    let inline bind (fn: 'a -> Effect<'env,'b>) effect =
        Effect (fun env ->
            let x = run env effect // compute result of the first effect
            run env (fn x) // run second effect, based on result of first one
        )
        
[<Struct>]
type EffectBuilder =
    member inline __.Return value = Effect.value value
    member inline __.Zero () = Effect.value (Unchecked.defaultof<_>)
    member inline __.ReturnFrom (effect: Effect<'env, 'out>) = effect
    member inline __.Bind(effect, fn) = Effect.bind fn effect
    

[<Interface>]
type ILogger =
    abstract debug: obj [] -> unit
    abstract error: obj [] -> unit



[<Interface>] type ILog = abstract Logger: ILogger

type IPlayAudio =
    abstract member PlaySound: string -> unit
        