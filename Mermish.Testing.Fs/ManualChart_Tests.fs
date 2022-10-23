module Mermish.Testing.Fs.ManualChart

open Xunit
open FsUnit.Xunit
open Mermish

[<Fact>]
let ``Can write Mermaid syntax``() =
    (Mermaid.ManualChart "test" :> IMermaidChart).MermaidSyntax
    |> should equal "test"


[<Fact>]
let ``Supports string formatting``() =
    sprintf "%A" (Mermaid.ManualChart "test2") |> should equal "test2"


[<Fact>]
let ``Supports ToString``() =
    (Mermaid.ManualChart "test3").ToString() |> should equal "test3"