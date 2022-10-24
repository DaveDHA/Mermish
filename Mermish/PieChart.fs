
namespace Mermish

open System
open Mermish.Utility


[<StructuredFormatDisplay("{MermaidSyntax}")>]
type PieChart = {
    Title : string
    ShowData : bool
    Data : UMap<string, decimal>
}
    with        
        member this.MermaidSyntax =
            seq {
                yield sprintf "pie%s" (if this.ShowData then " showData" else "")
                if not (String.IsNullOrWhiteSpace this.Title) then yield $"    title {this.Title}"
                for (key, value) in this.Data do
                    yield $"    \"{key}\" : {value}"
            }
            |> String.concat "\n"

        override this.ToString() = this.MermaidSyntax

        interface IMermaidChart with member this.MermaidSyntax = this.MermaidSyntax
        



type PieNode<'t> =
    | Title of string
    | ShowData
    | HideData
    | PieSlice of (string * 't)
    | PieSlices of (string * 't) seq


module PieChart =
    
    let Default = { Title = "" ; ShowData = false ; Data = UMap.empty }


    let private decimalize<'t> (x : 't) = Decimal.Round((Convert.ToDecimal x), 2)
        

    let private decimalizePairs<'t> (pairs : (string * 't) seq)  = pairs |> Seq.map (Tuple.mapSnd decimalize)
        

    let private fromNode chart node =
        match node with
        | Title str -> { chart with Title = str }
        | ShowData -> { chart with ShowData = true }
        | HideData -> { chart with ShowData = false }
        | PieSlice (label, number) -> { chart with Data = chart.Data |> UMap.add label (decimalize number) }
        | PieSlices items -> { chart with Data = chart.Data |> UMap.addAll (decimalizePairs items)}


    let Add node chart = fromNode chart node

    let AddAll nodes chart = nodes |> Seq.fold fromNode chart

    let AddSlices data chart = Add (PieSlices data) chart

    let RemoveSlice key chart = { chart with Data = chart.Data |> UMap.remove key }
 





    