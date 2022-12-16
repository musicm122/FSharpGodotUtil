namespace Pattern.StateMachine

//https://github.com/GDQuest/godot-design-patterns/blob/main/godot-csharp/StateMachine/Player/StateMachine.cs
//https://github.com/GDQuest/godot-design-patterns/blob/d7ff00899a55965bcf74c0954355efc39432852a/godot-csharp/StateMachine/State.cs#L4
//https://www.gdquest.com/tutorial/godot/design-patterns/finite-state-machine/#:~:text=Split%20an%20object%27s%20finite%20number,often%20represented%20as%20a%20graph.
open Godot

[<Signal>]
type Transitioned = delegate of string -> unit

type StateMachine() = 
    inherit Node()
    
    [<Export>]
    member val initialState = new NodePath() with get,set
    // delegate void Transitioned(string stateName);
    // NodePath InitialState;
    // State State;
    // override void _Ready()    
    // override void _UnhandledInput(InputEvent @event)
    // override void _Process(float delta)    
    // override void _PhysicsProcess(float delta)    
    // void TransitionTo(string targetStateName, Dictionary<string, bool> message = null)    
               
type State() =
    inherit Node()
    //virtual void HandleInputs(InputEvent inputEvent)    
    //virtual void Update(float delta)    
    //virtual void PhysicsUpdate(float delta)    
    //virtual void Enter(Dictionary<string, bool> message = null)
    //virtual void Exit()
    
    