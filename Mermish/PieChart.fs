
namespace Mermish

open System


[<StructuredFormatDisplay("{MermaidSyntax}")>]
type PieChart = {
    Title : string
    ShowData : bool
    Data : Map<string, decimal>
}
    with        
        member this.MermaidSyntax =
            seq {
                yield sprintf "pie%s" (if this.ShowData then " showData" else "")
                if not (String.IsNullOrWhiteSpace this.Title) then yield $"    title {this.Title}"
                for kvp in this.Data do
                    yield $"    \"{kvp.Key}\" : {kvp.Value}"
            }
            |> String.concat "\n"

        interface IMermaidChart with member this.MermaidSyntax = this.MermaidSyntax
        



type PieNode<'t> =
    | Title of string
    | ShowData
    | HideData
    | PieSlice of (string * 't)
    | PieSlices of (string * 't) seq


module PieChart =
    
    let Default = { Title = "" ; ShowData = false ; Data = Map.empty }


    let private decimalize<'t> (x : 't) = Decimal.Round((Convert.ToDecimal x), 2)
        

    let private decimalizePairs<'t>  = Seq.map (Tuple.mapSnd decimalize)
        

    let private fromNode chart node =
        match node with
        | Title str -> { chart with Title = str }
        | ShowData -> { chart with ShowData = true }
        | HideData -> { chart with ShowData = false }
        | PieSlice (label, number) -> { chart with Data = chart.Data |> Map.add label (decimalize number) }
        | PieSlices items -> { chart with Data = chart.Data |> Map.addAll (decimalizePairs items)}


    let Add node chart = fromNode chart node

    let AddAll nodes chart = nodes |> Seq.fold fromNode chart

    let FromNodes nodes = AddAll nodes Default

    let AddSlices data chart = Add (PieSlices data) chart

    let RemoveSlice key chart = { chart with Data = chart.Data |> Map.remove key }
    

    //let WithTitle title pc = { pc with Title = title }

    
    //let ShowData pc = { pc with ShowData = true }


    //let HideData pc = { pc with ShowData = false }


    //let SetData data pc = { pc with Data = data |> decimalizePairs |> Map.ofSeq }


    //let AppendData data pc = { pc with Data = pc.Data |> Map.addAll (decimalizePairs data) }





    