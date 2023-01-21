namespace Pattern.StateMachine

open System
open Common.Uti
open Godot.Collections

//type State<'A when 'A FSharpType.> =


[<Serializable>]
type State =
    { Name: string
      OnEnter: Action<unit> option
      OnExit: Action<unit> option
      OnFrame: Action<float> option }

    static member Default =
        { State.Name = String.Empty
          OnEnter = None
          OnExit = None
          OnFrame = None }

type StateMachine() =
    member val States = Dictionary<string, State>()
    member val CurrentState: State option = None with get, set
    member val InitialState: State option = None with get, set

    member this.Add(state: State) =
        if this.States.Count = 0 then
            this.InitialState <- Some state

        this.States.Add(state.Name, state)
        state

    member this.TransitionTo(stateName: string) =
        let newState = this.States[stateName]

        match this.CurrentState with
        | Some curr ->
            match curr.OnExit with
            | Some onExit ->
                printfn $"Transitioning from '%s{curr.Name}' to '%s{stateName}'"
                onExit.Invoke()
            | _ -> printfn $"Transitioning from '%s{curr.Name}' to '%s{stateName}'"
        | _ -> ()

        this.CurrentState <- Some newState

    member this.TransitionToEnum<'T when 'T: enum<int> and 'T: struct and 'T :> ValueType and 'T: (new: unit -> 'T)>
        enumVal
        =
        let description = enumVal |> EnumUtil.getDescription

        if String.IsNullOrWhiteSpace(description) then
            failwith "State enum missing description!"

        match this.States.ContainsKey(description) with
        | false -> failwith ("State machine doesn't contain a state named " + description)
        | true -> this.TransitionTo description

        ()

    member this.Update delta =
        match this.InitialState with
        | Some init -> this.States.Add(init.Name, init)
        | _ -> failwith "State machine has no initial states!"

        if this.States.Count = 0 then
            failwith "State machine has no states!"

        match this.CurrentState with
        | None ->
            match this.InitialState with
            | None -> failwith "State machine has no initial state!"
            | Some init -> this.TransitionTo init.Name
        | Some curr ->
            match curr.OnFrame with
            | Some onFrame -> onFrame.Invoke(delta)
            | _ -> ()

        ()
