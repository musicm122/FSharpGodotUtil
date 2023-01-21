module Utils

open Xunit
open System.ComponentModel
open Common.Uti
open FsUnit.Xunit
open System

module EnumUtilTests =

    type FooEnum =
        | [<Description("A")>] One = 1
        | [<Description("B")>] Two = 2
        | Three = 3
        
    module getDescription =
        [<Fact>]
        let ``should return a string if description is assigned``() =
            FooEnum.One |> EnumUtil.getDescription |> should equal "A"
        
        [<Fact>]
        let ``should return an empty string if description is not assigned``() =
            FooEnum.Three |> EnumUtil.getDescription |> should equal String.Empty
           
                   
