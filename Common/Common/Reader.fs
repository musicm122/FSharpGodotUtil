namespace Common.Reader


type Reader<'Env,'A> = Reader of action:('Env -> 'A)

module Reader =
    /// Run a Reader with a given environment
    let run env (Reader action)  = 
        action env  // simply call the inner function

    /// Create a Reader which returns the environment itself
    let ask = Reader id 

    /// Map a function over a Reader 
    let map f reader = 
        Reader (fun env -> f (run env reader))

    /// flatMap a function over a Reader 
    let bind f reader =
        let newAction env =
            let x = run env reader 
            run env (f x)
        Reader newAction

    /// Transform a Reader's environment from subtype to supertype.
    let withEnv (f:'SuperEnv->'SubEnv) reader = 
        Reader (fun superEnv -> (run (f superEnv) reader))  
        // The new Reader environment is now "superEnv"


type ReaderBuilder() =
    member this.Return(x) = Reader (fun _ -> x)
    member this.Bind(x,f) = Reader.bind f x
    member this.Zero() = Reader (fun _ -> ())

// the builder instance
//let reader = ReaderBuilder()
