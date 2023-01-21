module Extensions

    open Xunit

    module GodotObject =
        module TryConnectSignal =
            [<Fact>]
            let ``My test`` () =
                Assert.True(true)    

