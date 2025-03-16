namespace CoreUI

open Godot
open Common

type TitleFs() =
    inherit Node()

    [<Export>]
    member val newGameScenePath: NodePath = new NodePath("res://Scenes/Title.tscn") with get, set

    member val titleSongAudioPath: NodePath = new NodePath("Music") with get, set
    member val buttonAudioPath: NodePath = new NodePath("ButtonSound") with get, set

    member val titleSongAudio: AudioStreamPlayer = null with get, set
    member val buttonAudio: AudioStreamPlayer = null with get, set

    override this._Ready() =
        this.titleSongAudio <- this.GetNode<AudioStreamPlayer>(new NodePath("Music"))

        this.buttonAudio <- this.GetNode<AudioStreamPlayer>(new NodePath("ButtonSound"))

        let startButton = base.FindNode("StartButton") :?> Button

        if not(isNull startButton) then
            startButton.GrabFocus()
        else
            GD.PushWarning("startButton is null ")

    member this.OnStartButtonPressed() =
        base.GetTree().ChangeScene(this.newGameScenePath)

    member this.OnQuitButtonPressed() = base.GetTree().Quit()

    member this.OnVBoxFocusEntered() = this.buttonAudio.Play()

type EndFs() =
    inherit Node()

    [<Export>]
    member val TitleScreen: NodePath = new NodePath("res://Scenes/Level1.tscn") with get, set

    [<Export>]
    member val Cooldown = 2f with get, set

    member val RemainingCooldownTime = 0f with get, set

    override this._Ready() =
        this.RemainingCooldownTime <- this.Cooldown

    override this._Process delta =
        if this.RemainingCooldownTime <= 0f then
            match this.GetTree().ChangeScene(this.TitleScreen) with
            | errNum when errNum <> Error.Ok ->
                GD.PrintErr [ "An Error occured when attempting to transition to a new scene",
                              this.TitleScreen.ToString(),
                              errNum,
                              errNum.ToString() ]
            | _ -> GD.Print [ "Successfully transitioned to scene at path " + this.TitleScreen.ToString() ]

        this.RemainingCooldownTime <- this.RemainingCooldownTime - delta

type LevelSelectorFs() =
    inherit Node()
    [<Export(PropertyHint.File, "*.tscn")>]
    member val PlayGameScene = new NodePath("res://Scenes/Level/TestLevel.tscn") with get,set
    
    [<Export>]
    member val LevelButtonPath: NodePath = new NodePath("RootControl/VBoxContainer/PlayGame") with get, set
    
    [<Export>]
    member val QuitButtonPath: NodePath = new NodePath("RootControl/VBoxContainer/Quit") with get, set

    member this.OnInteractionButtonPressed() =
        this.GetTree().ChangeScene(this.PlayGameScene);
    
    member this.OnQuitButtonPressed() =
        this.GetTree().Quit();
    
    override this._Ready() =
        let interaction = this.GetNode<Button>(this.LevelButtonPath)
        let quit = this.GetNode<Button>(this.QuitButtonPath)
        
        match interaction.ConnectButtonPressed interaction this (nameof(this.OnInteractionButtonPressed)) with
        | err when err <> Error.Ok ->
            printfn "Connection to signal failed %s" (nameof(this.OnInteractionButtonPressed))
        | _ -> ()
                
        match quit.ConnectButtonPressed quit this (nameof(this.OnQuitButtonPressed)) with
        | err when err <> Error.Ok ->
            printfn "Connection to signal failed %s" (nameof(this.OnQuitButtonPressed)) 
        | _ -> ()
        interaction.GrabFocus();
