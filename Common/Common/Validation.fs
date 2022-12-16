namespace Common.Validation

open System
open Common.Constants
open Common.Types

type SignalValidator() =
    member this.validSignalName signalName : SignalProblem =
        match String.IsNullOrWhiteSpace(signalName) with
        | true -> SignalProblem.MissingField MissingSignalField.SignalName
        | false ->
            match SignalUtil.AllSignals |> Array.contains signalName with
            | true -> OkSignal
            | _ -> InvalidSignal signalName

    member this.validSource source =
        match source with
        | null -> SignalProblem.MissingField MissingSignalField.Source
        | _ -> OkSignal

    member this.validTarget target =
        match target with
        | null -> SignalProblem.MissingField MissingSignalField.Target
        | _ -> OkSignal

    member this.validMethodName methodName =
        match String.IsNullOrWhiteSpace(methodName) with
        | true -> SignalProblem.MissingField MissingSignalField.MethodName
        | _ -> OkSignal

    member this.validate(signalConnection: SignalConnection) =
        [ this.validTarget signalConnection.target
          this.validSignalName signalConnection.signal
          this.validMethodName signalConnection.methodName ]
        |> List.filter (fun validations ->
            match validations with
            | OkSignal -> false
            | _ -> true)
