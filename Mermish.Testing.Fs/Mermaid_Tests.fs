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
let ``Can write a chart to a file``() =
    let fname = "CanWriteAChartToAFile.html"
    if (File.Exists fname) then File.Delete fname

    charts[0] |> Mermaid.WriteToFile fname
    
    let content = File.ReadAllText fname

    content |> should haveSubstring "pie"
    content |> should haveSubstring "title Testing Odds"
    content |> should haveSubstring "\"Success\" : 25"
    content |> should haveSubstring "\"Failure\" : 75"


[<Fact>]
let ``Can write all charts to a file``() =
    let fname = "CanWriteAllChartsToAFile.html"
    if (File.Exists fname) then File.Delete fname

    charts |> Mermaid.WriteAllToFile fname
    
    let content = File.ReadAllText fname 

    content |> should haveSubstring "pie"
    content |> should haveSubstring "title Testing Odds"
    content |> should haveSubstring "\"Failure\" : 75"
    content |> should haveSubstring "graph TD"
    content |> should haveSubstring "choice -->|yes| examine[Examine output]"
    content |> should haveSubstring "result -->|yes| success[Celebrate!]"
