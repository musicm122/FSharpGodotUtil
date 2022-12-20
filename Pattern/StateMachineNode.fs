namespace Pattern.StateMachine

//https://github.com/GDQuest/godot-design-patterns/blob/main/godot-csharp/StateMachine/Player/StateMachine.cs
//https://github.com/GDQuest/godot-design-patterns/blob/d7ff00899a55965bcf74c0954355efc39432852a/godot-csharp/StateMachine/State.cs#L4
//https://www.gdquest.com/tutorial/godot/design-patterns/finite-state-machine/#:~:text=Split%20an%20object%27s%20finite%20number,often%20represented%20as%20a%20graph.
open System.Collections.Generic
open System.Threading.Tasks
open Godot
open Common.Extensions

[<Signal>]
type Transitioned = delegate of string -> unit

type StateMessage<'a> =
    | Args of Dictionary<string, 'a>
    | NoOp
// delegate void Transitioned(string stateName);
// NodePath InitialState;
// State State;
// override void _Ready()
// override void _UnhandledInput(InputEvent @event)
// override void _Process(float delta)
// override void _PhysicsProcess(float delta)
// void TransitionTo(string targetStateName, Dictionary<string, bool> message = null)

type StateNode() =
    inherit Node()
    member val Update: (float32 -> unit) option = None with get, set
    member val PhysicsUpdate: ((float32 -> unit) option) = None with get, set
    member val Enter: ((StateMessage<'a> -> unit) option) = None with get, set
    member val Exit: ((unit -> unit) option) = None with get, set
    member val HandleInputs: ((InputEvent -> unit) option) = None with get, set


//virtual void HandleInputs(InputEvent inputEvent)
//virtual void Update(float delta)
//virtual void PhysicsUpdate(float delta)
//virtual void Enter(Dictionary<string, bool> message = null)
//virtual void Exit()


type StateMachineNode() =
    inherit Node()

    let exitState (state: StateNode) : unit =
        match state.Exit with
        | Some exit -> exit ()
        | _ -> ()

    let enterState (state: StateNode) (msg: StateMessage<'a>) : unit =
        match state.Enter with
        | Some enter -> enter msg
        | _ -> ()

    let exitCurrentStateOption (currentStateOption: StateNode option) =
        match currentStateOption with
        | (Some current) -> current |> exitState
        | _ -> ()

    let enterNewStateOption (newState: StateNode option) msg =
        match newState with
        | (Some newState) -> (newState, msg) ||> enterState
        | _ -> ()

    let updateState (currentState: StateNode option) (newState: StateNode option) msg : StateNode option =
        currentState |> exitCurrentStateOption
        (newState, msg) ||> enterNewStateOption
        newState

    let mutable (currentState: StateNode option) = None

    member this.CurrentState
        with get () = currentState
        and private set (value) = currentState <- value

    [<Export>]
    member val InitialState: option<StateNode> = None with get, set

    member val States = Dictionary<string, StateNode>() with get, set

    override this._UnhandledInput(inputEvent: InputEvent) =
        match this.CurrentState with
        | (Some state) ->
            match state.HandleInputs with
            | (Some handleInputs) -> handleInputs inputEvent
            | _ -> ()
        | _ -> ()

    override this._Ready() =
        this.CurrentState <- this.InitialState
        task { this.ToSignal(this.Owner, "ready") |> ignore } |> ignore
    //this.GetChildren().Iter(fun child -> child.<-this)
    override this._Process delta =
        match this.CurrentState with
        | Some current ->
            match current.Update with
            | Some update -> update delta
            | _ -> ()
        | _ -> ()
    
    override this._PhysicsProcess delta =
        match this.CurrentState with
        | Some current ->
            match current.PhysicsUpdate with
            | Some update -> update delta
            | _ -> ()
        | _ -> ()
    
    member this.TransitionTo (stateName: string) (msg: StateMessage<'a>) =
        match this.States.ContainsKey stateName with
        | true ->
            let newState = Some(this.States.Item stateName)
            this.CurrentState <- updateState this.CurrentState newState msg
            this.EmitSignal(nameof Transitioned)
        | _ -> ()


// delegate void Transitioned(string stateName);
// NodePath InitialState;
// State State;
// override void _Ready()
// override void _UnhandledInput(InputEvent @event)
// override void _Process(float delta)
// override void _PhysicsProcess(float delta)
// void TransitionTo(string targetStateName, Dictionary<string, bool> message = null)
