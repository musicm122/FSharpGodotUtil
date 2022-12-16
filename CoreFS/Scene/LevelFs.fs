namespace CoreFS

open Common.Extensions
open CoreFS.Types
open Godot

type LevelFS() =
    inherit Spatial()

    [<Export>]
    member val fastClose = false with get, set

    override this._Ready() =

        Input.MouseMode <- Input.MouseModeEnum.Captured

        if OS.IsDebugBuild() = false then
            this.fastClose <- false

        if this.fastClose = true then
            GD.Print("** Fast Close enabled in the 'Level' script **")
            GD.Print("** 'Esc' to close 'Shift + F1' to release mouse **")

        this.SetProcessInput this.fastClose

    override this._Input(event: InputEvent) =
        let changeMouseInput (input: SupportedInput) =
            match event.IsActionPressed(input.AsString) with
            | true when input = SupportedInput.UICancel -> this.GetTree().Quit()
            | true when input = SupportedInput.ChangeMouseInput ->
                match Input.MouseMode with
                | Input.MouseModeEnum.Captured -> Input.MouseMode <- Input.MouseModeEnum.Visible
                | Input.MouseModeEnum.Visible -> Input.MouseMode <- Input.MouseModeEnum.Captured
                | _ -> ()
            | _ -> ()

        SupportedInput.Cases
        |> Array.iter changeMouseInput

    override this._UnhandledInput(event: InputEvent) =
        match event with
        | :? InputEventMouseButton as buttonEvent ->
            if buttonEvent.isLeftButtonPressed () then
                Input.MouseMode <- Input.MouseModeEnum.Captured
        | _ -> ()
