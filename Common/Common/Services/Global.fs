namespace Common.Services



open Common.Services

module Global =
    let private _dialogManager = Lazy.Create(fun () -> new DialogManager())

    let private _logger = Lazy.Create(fun () -> Log())

    let DialogManager () : DialogManager = _dialogManager.Value

    let Log () = _logger.Value :> Common.Reader.ILogger
