namespace Mermish.Testing.Fs.PieChart

open System
open Xunit
open FsUnit
open FsUnit.Xunit
open Mermish

module ``Basic rendering`` =
    [<Fact>]
    let ``Syntax starts with 'pie'``() =
        ({
            PieChart.Title = "Wow!"
            ShowData = true
            Data = Map.empty
        } :> IMermaidChart).MermaidSyntax
        |> should startWith "pie"


    [<Fact>]
    let ``Renders Title`` () =
        ({
            PieChart.Title = "Wow!"
            ShowData = true
            Data = Map.empty
        } :> IMermaidChart).MermaidSyntax
        |> should haveSubstring "title Wow!"

    
    [<Fact>]
    let ``Renders ShowData``() =
        ({
            PieChart.Title = "Wow!"
            ShowData = true
            Data = Map.empty
        } :> IMermaidChart).MermaidSyntax
        |> should haveSubstring "showData"


    [<Fact>]
    let ``Renders without ShowData``() =
        ({
            PieChart.Title = "Wow!"
            ShowData = false
            Data = Map.empty
        } :> IMermaidChart).MermaidSyntax
        |> should not' (haveSubstring "showData")


    [<Fact>]
    let ``Renders all slices``() =
        let output = 
            ({
                PieChart.Title = "Wow!"
                ShowData = false
                Data = [ "one", 1M ; "second item", 2.5M ] |> Map.ofSeq
            } :> IMermaidChart).MermaidSyntax
        
        output |> should haveSubstring "\"one\" : 1"
        output |> should haveSubstring "\"second item\" : 2.5"


    [<Fact>]
    let ``Supports string formatting``() =
        let chart = {
            PieChart.Title = "Wow!"
            ShowData = false
            Data = [ "one", 1M ; "second item", 2.5M ] |> Map.ofSeq
        }

        sprintf "%A" chart |> should equal (chart :> IMermaidChart).MermaidSyntax
