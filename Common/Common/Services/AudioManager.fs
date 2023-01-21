namespace Common.Manager

open System.Collections.Generic
open Common.Types
open Godot

type AudioManager() =
    inherit Node()

    [<Literal>]
    static let TotalChannels = 16

    [<Literal>]
    static let Bus = "master"

    static member val freeChannels: Queue<AudioStreamPlayer> = Queue<AudioStreamPlayer>() with get, set
    static member val queue: Queue<string> = Queue<string>() with get, set

    member this.onStreamFinished(channel: AudioStreamPlayer) : unit =
        AudioManager.freeChannels.Enqueue channel

    member this.bindChannelToParent(channel: AudioStreamPlayer) : unit =
        this.AddChild(channel)
        AudioManager.freeChannels.Enqueue channel

    static member PlaySound streamName : unit =
        try 
            if not (AudioManager.queue.Contains(streamName)) then
                AudioManager.queue.Enqueue streamName
        with
        |  ex ->
            GD.PrintErr([ex,"AudioManager Exception thrown while playing sound:"+ streamName])
            failwith ex.Message

    member this.initializeChannel busName =
        let channel = new AudioStreamPlayer()
        channel.Autoplay <- true
        channel.Bus <- busName

        let args = Some(new Godot.Collections.Array(channel))

        //let connection = { SignalConnection.Default(Signals.AudioStreamPlayer.Finished, this, nameof this.onStreamFinished, channel) with args = args }
        // channel.Connect("finished", this, nameof(this.onStreamFinished), new Godot.Collections.Array(channel)) with

        //match channel.TryConnectSignal(connection) with
        match channel.Connect("finished", this, nameof this.onStreamFinished, new Godot.Collections.Array(channel)) with
        | err when err = Error.Ok -> channel
        | ex ->
            let gEx =
                GodotAudioSignalException(
                    ex,
                    $"connecting {channel.GetType().ToString()} signal 'finished' to {nameof this.onStreamFinished} failed"
                )

            raise gEx

    member this.initializeChannels =
        let initChannelOnMaster _ = this.initializeChannel Bus

        [ 0..TotalChannels ]
        |> List.map initChannelOnMaster
        |> List.iter this.bindChannelToParent

        ignore

    member this.hasWork =
        AudioManager.queue.Count > 0 && AudioManager.freeChannels.Count > 0

    override this._Ready() = this.initializeChannels ()

    override this._Process delta =
        try
            if this.hasWork then
                let player = AudioManager.freeChannels.Dequeue()

                player.Stream <- GD.Load<AudioStream>(AudioManager.queue.Dequeue())
                player.Play()
        with
        |  ex ->
            GD.PrintErr([ex,"AudioManager Exception thrown"])
            failwith ex.Message

type IPlayAudio =
    abstract member PlaySound: string -> unit

type AudioService() =
    inherit Node()

    interface IPlayAudio with
        member this.PlaySound path = AudioManager.PlaySound path
