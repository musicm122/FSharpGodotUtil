module Tests

open System
open Xunit

module Extensions =
    module GodotObject =
        module TryConnectSignal =
            [<Fact>]
            let ``My test`` () =
                Assert.True(true)    

