namespace Mermish


open System
open Mermish.Utility
open Mermish.PieChart

module pie =

    type BuilderNode =
    | Title of string
    | ShowData of bool
    | PieSlice of (string * decimal)

    let private decimalize<'t> (x : 't) = Decimal.Round((Convert.ToDecimal x), 2)
    
    let private decimalizePairs<'t> (pairs : (string * 't) seq)  = pairs |> Seq.map (Tuple.mapSnd decimalize)
    
    let slice (name, value) = PieSlice (name, decimalize value)
    
    let slices data = data |> Seq.map slice

    let title = Title

    let showData = ShowData true

    let hideData = ShowData false


    type PieChartBuilder() =        
        member _.Zero = []

        member _.Yield (x : BuilderNode) = [ x ]

        member _.YieldFrom (x : BuilderNode seq) = x |> Seq.toList

        member _.Yield pair = [ slice pair ]

        member _.YieldFrom pairs = pairs |> Seq.map slice |> Seq.toList

        member _.Yield title = [ Title title ]

        member _.YieldFrom titles = titles |> Seq.map Title |> Seq.toList

        member _.Combine ((x : BuilderNode list), y) = x @ y

        member _.Delay f = f()

        member _.For (items, f) =
            items
            |> Seq.collect f
            |> Seq.toList

        member _.Run(nodes : BuilderNode list) =
            let getLast defVal chooser = nodes |> Seq.pickBackWithDefault defVal chooser

            let slices = 
                nodes
                |> Seq.choose (function | PieSlice x -> Some x | _ -> None)
                |> UMap.ofSeq

            {
                PieChart.Title = getLast "" (function Title x -> Some x | _ -> None)
                ShowData = getLast false (function ShowData x -> Some x | _ -> None)
                Data = slices
            }

    let chart = PieChartBuilder()
