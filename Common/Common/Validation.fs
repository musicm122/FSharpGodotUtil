namespace Common.Validation

open System
open Common.Constants
open Common.Types

type SignalValidator() =
    member this.ValidSignalName signalName : SignalProblem =
        match String.IsNullOrWhiteSpace(signalName) with
        | true -> SignalProblem.MissingField MissingSignalField.SignalName
        | false ->
            match SignalUtil.AllSignals |> Array.contains signalName with
            | true -> OkSignal
            | _ -> InvalidSignal signalName

    member this.ValidSource source =
        match source with
        | null -> SignalProblem.MissingField MissingSignalField.Source
        | _ -> OkSignal

    member this.ValidTarget target =
        match target with
        | null -> SignalProblem.MissingField MissingSignalField.Target
        | _ -> OkSignal

    member this.ValidMethodName methodName =
        match String.IsNullOrWhiteSpace(methodName) with
        | true -> SignalProblem.MissingField MissingSignalField.MethodName
        | _ -> OkSignal

    member this.Validate(signalConnection: SignalConnection) =
        [ this.ValidTarget signalConnection.Target
          this.ValidSignalName signalConnection.Signal
          this.ValidMethodName signalConnection.MethodName ]
        |> List.filter (fun validations ->
            match validations with
            | OkSignal -> false
            | _ -> true)
