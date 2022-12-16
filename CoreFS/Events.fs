namespace CoreFS.Events

module PauseEvents =
    let Pause = Event<unit>()
    let Unpause = Event<unit>()

module DialogEvents =
    let DialogInteractionStart = Event<unit>()

    let DialogInteractionComplete =
        Event<unit>()

    let PlayerInteractionAvailabilityChange =
        Event<bool>()
