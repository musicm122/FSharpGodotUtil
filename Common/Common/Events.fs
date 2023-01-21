namespace Common.Events

open Common.Interfaces


module CustomEvents =
    let PlayerInteracting = Event<IExaminable>()

    let PlayerInteractingComplete = Event<unit>()

    let PlayerInteractingEvent = Event<unit>()

    let PlayerInteractingUnavailable = Event<unit>()

    let PlayerInteractingAvailable = Event<unit>()

module PauseEvents =
    let Pause = Event<unit>()
    let Unpause = Event<unit>()
