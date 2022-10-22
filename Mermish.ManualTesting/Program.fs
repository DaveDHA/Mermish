open Mermish
open System.Diagnostics

let charts = 
    [
        Mermaid.PieChart
        |> PieChart.WithTitle "Testing"
        |> PieChart.AppendData [ "first", 200 ; "second", 300 ]
        |> PieChart.ShowData
        |> PieChart.AppendData [ "second", 400 ; "third", 500 ]
    ]

charts |> Seq.iter (printfn "%A")

Mermaid.WriteAllToFile "MermishTest.html" charts

ProcessStartInfo("MermishTest.html", UseShellExecute = true)
|> Process.Start
|> ignore






