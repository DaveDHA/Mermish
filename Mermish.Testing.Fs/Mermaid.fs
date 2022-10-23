module Mermish.Testing.Fs.Mermaid

open Xunit
open FsUnit.Xunit
open System.IO
open Mermish


let charts = [
    Mermaid.PieChart [
        Title "Testing Odds"
        PieSlice ("Success", 25)
        PieSlice ("Failure", 75)
    ] :> IMermaidChart
    
    Mermaid.ManualChart """
    graph TD
        run[Run this script] --> choice{Success?}
        choice -->|no| fail[Fix it!]    
        choice -->|yes| examine[Examine output]
        examine --> result{Output correct?}
        result -->|no| fail
        result -->|yes| success[Celebrate!]
    """
]


[<Fact>]
let ``Can write a chart to a file'``() =
    charts[0] |> Mermaid.WriteToFile "CanWriteAChartToAFile.html"
    
    let content = File.ReadAllText("CanWriteAChartToAFile.html")

    content |> should haveSubstring "pie"
    content |> should haveSubstring "title Testing Odds"
    content |> should haveSubstring "\"Success\" : 25"
    content |> should haveSubstring "\"Failure\" : 75"


[<Fact>]
let ``Can write all charts to a file'``() =
    charts |> Mermaid.WriteAllToFile "CanWriteAllChartsToAFile.html"
    
    let content = File.ReadAllText("CanWriteAllChartsToAFile.html")

    content |> should haveSubstring "pie"
    content |> should haveSubstring "title Testing Odds"
    content |> should haveSubstring "\"Failure\" : 75"
    content |> should haveSubstring "graph TD"
    content |> should haveSubstring "choice -->|yes| examine[Examine output]"
    content |> should haveSubstring "result -->|yes| success[Celebrate!]"
