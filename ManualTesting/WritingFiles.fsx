#i "nuget: C:/Dev/Mermish/Mermish/bin/Debug"
#r "nuget: Mermish, 0.0.0-dev"

open Mermish
open System.IO


let pie = Mermaid.PieChart [
    Title "Testing Odds"
    PieSlice ("Success", 25)
    PieSlice ("Failure", 75)
]

printfn "%A" pie

let manual = Mermaid.ManualChart """
graph TD
    run[Run this script] --> choice{Success?}
    choice -->|no| fail[Fix it!]    
    choice -->|yes| examine[Examine output]
    examine --> result{Output correct?}
    result -->|no| fail
    result -->|yes| success[Celebrate!]
"""

printfn "%A" manual

if not (Directory.Exists "./output") then Directory.CreateDirectory "./output" |> ignore

Mermaid.WriteToFile "./output/JustPie.html" pie
Mermaid.WriteAllToFile "./output/All.html" [ manual ; pie ]
