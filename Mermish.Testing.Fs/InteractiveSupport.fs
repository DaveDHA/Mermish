module Mermish.Testing.Fs.MerInteractiveSupport

open Xunit
open FsUnit.Xunit
open System.IO
open Mermish


let chart = Mermaid.ManualChart """
graph TD
    run[Run this script] --> choice{Success?}
    choice -->|no| fail[Fix it!]    
    choice -->|yes| examine[Examine output]
    examine --> result{Output correct?}
    result -->|no| fail
    result -->|yes| success[Celebrate!]
"""


[<Fact>]
let ``Formatting for DotNet Interactive includes chart markdown``() =
    use writer = new StringWriter()
    InteractiveSupport.FormatChartForDotNetInteractive chart writer
    let text = writer.ToString()

    text |> should haveSubstring "graph TD"
    text |> should haveSubstring "choice -->|yes| examine[Examine output]"
    text |> should haveSubstring "result -->|yes| success[Celebrate!]"
    

[<Fact>]
let ``Formatting for DotNet Interactive includes Mermaid JS``() =
    use writer = new StringWriter()
    InteractiveSupport.FormatChartForDotNetInteractive chart writer
    let text = writer.ToString()

    text |> should haveSubstring "https://cdn.jsdelivr.net/npm/mermaid@9.1.3/dist/mermaid.min"