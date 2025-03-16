namespace Common.Uti

open System
open System.Collections.Generic
open Common.Constants
open Godot
open Microsoft.FSharp.Reflection
open System.ComponentModel
open System.Reflection

module EnumUtil =
    let asEnum<'T when 'T: enum<int> and 'T: struct and 'T :> ValueType and 'T: (new: unit -> 'T)> text =
        match Enum.TryParse<'T>(text) with
        | true, value -> Some value
        | _ -> None

    
    let getDescription (enumVal:Enum) =
        let field = enumVal.GetType().GetField(enumVal.ToString())
        let attrib =
            field.GetCustomAttribute(typeof<DescriptionAttribute>) :?> DescriptionAttribute
        if isNull attrib then
            String.Empty
        else
            attrib.Description

    let toList<'A>() =
        let enumType = typeof<'A>
        if enumType.BaseType <> typeof<Enum> then
            invalidArg "toList" "'A must be of type System.Enum"        
                
        let enumVals = Enum.GetValues(enumType)
        let enumValList = List<'A>(enumVals.Length);
         // foreach (int val in enumValArray) enumValList.Add((T)System.Enum.Parse(enumType, val.ToString()));

        for enum in enumVals do
            enumValList.Add(Enum.Parse(enumType, enum.ToString()) :?> 'A)
           
        enumValList
    
    let enumCount<'A>() =
        let enumType = typeof<'A>
        if enumType.BaseType <> typeof<Enum> then
            invalidArg "enumCount" "'A must be of type System.Enum"        
        Enum.GetNames(typeof<'A>).Length;        
        
module InputUtil =

    let getDirectionInput (globalBasis: Basis) (inputAxis: Vector2) =
        globalBasis.z * -inputAxis.x + globalBasis.x * inputAxis.y

    let IsInteractionJustPressed () =
        Input.IsActionJustPressed(InputActions.Sprint)

    let IsJumpJustPressed () =
        Input.IsActionJustPressed(InputActions.Jump)

    let IsSprintPressed () =
        Input.IsActionPressed(InputActions.Sprint)

    let getInputAxis () =
        Input.GetVector(InputActions.MoveBack, InputActions.MoveForward, InputActions.MoveLeft, InputActions.MoveRight)

    let IsAnyKeyJustPressed () =
        InputActions.AllInputs |> Array.exists Input.IsActionJustPressed

    let IsAnyKeyPressed () =
        InputActions.AllInputs |> Array.exists Input.IsActionPressed

    let GetTopDownWithDiagMovementInputStrengthVector () =
        let x =
            Input.GetActionStrength(InputActions.Right)
            - Input.GetActionStrength(InputActions.Left)

        let y =
            Input.GetActionStrength(InputActions.Down)
            - Input.GetActionStrength(InputActions.Up)

        Vector2(x, y).Normalized()

module MouseUtil =
    let isMouseButtonPressed (btnEvent: InputEventMouseButton) (btnListItem: ButtonList) =
        btnEvent.ButtonIndex = int btnListItem && btnEvent.Pressed

    let isLeftMouseButtonPressed (btnEvent: InputEventMouseButton) =
        isMouseButtonPressed btnEvent ButtonList.Left

    let isRightMouseButtonPressed (btnEvent: InputEventMouseButton) =
        isMouseButtonPressed btnEvent ButtonList.Right

module ActivePatterns =
    let (|EmptySeq|_|) a =
        if Seq.isEmpty a then Some() else Option.None

module DIUtil =
    let UnionCasesOf<'A> () =
        FSharpType.GetUnionCases typeof<'A>
        |> Array.map (fun case -> FSharpValue.MakeUnion(case, [||]) :?> 'A)

module ProjectSetting =
    let getGravity =
        ProjectSettings.GetSetting("physics/3d/default_gravity") :?> float32
        
module NodeUtil =

    let inline getNodeResultFromPath<'A when 'A: not struct> (node: Node) (path: NodePath) =
        try
            match node.GetNode<'A>(path) with
            | result -> Result.Ok result
        with e ->
            Result.Error e.Message

    let inline getNodeFromPath<'A when 'A: not struct> (node: Node) (path: NodePath) = node.GetNode<'A>(path)

module MathUtils =

    let getRandomInRange min max =
        let rand = new RandomNumberGenerator()
        rand.RandfRange(min, max)

    let getRandomPosNegOne () = getRandomInRange -1.0f -1.0f

    let clamp (min: float) (max: float) (value: float) =
        let currentValue = float32 value
        let min = float32 min
        let max = float32 max
        let result = Mathf.Clamp(currentValue, min, max)
        float result

    let clampMinZero max value = clamp 0.0 max value

    //Using a couple of unit's Mathf
    //https://github.com/Unity-Technologies/UnityCsReference/blob/master/Runtime/Export/Math/Mathf.cs
    // Loops the value t, so that it is never larger than length and never smaller than 0.
    let repeat tVal max =
        let inputVal = (tVal - Mathf.Floor(tVal / max) * max)
        Mathf.Clamp(inputVal, 0f, max)

    // PingPongs the value t, so that it is never larger than length and never smaller than 0.
    let pingPongMax tVal max =
        let t' = repeat tVal (max * 2f)
        max - Mathf.Abs t' - max

    // PingPongs the value t, so that it is never larger than length and never smaller than 0.

    let pingPongMinMax tVal min max =
        let t' = repeat tVal (max * 2f)
        max - Mathf.Abs(t' - min)

    let clamp01 t =
        match t with
        | t when t < 0f -> 0f
        | t when t > 1f -> 1f
        | _ -> t

    // Compares two floating point values if they are similar.
    // If a or b is zero, compare that the other is less or equal to epsilon.
    // If neither a or b are 0, then find an epsilon that is good for
    // comparing numbers at the maximum magnitude of a and b.
    // Floating points have about 7 significant digits, so
    // 1.000001f can be represented while 1.0000001f is rounded to zero,
    // thus we could use an epsilon of 0.000001f for comparing values close to 1.
    // We multiply this epsilon by the biggest magnitude of a and b.

    let approximately (a: float32) (b: float32) =
        let diff = b - a
        let abs = Mathf.Abs(diff)

        let mag =
            Mathf.Max(0.000001f * Mathf.Max(Mathf.Abs(a), Mathf.Abs(b)), Mathf.Epsilon * 8f)

        abs < mag
//Mathf.Abs(b-a) < Mathf.Max(0.000001f * Mathf.Max())
//return Mathf.Abs(b - a) < Mathf.Max(0.000001f * Mathf.Max(Mathf.Abs(a), Mathf.Abs(b)), Mathf.Epsilon * 8);
