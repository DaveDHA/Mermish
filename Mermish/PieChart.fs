
namespace Mermish

open System


[<StructuredFormatDisplay("{MermaidMarkdown}")>]
type PieChart = {
    Title : string
    ShowData : bool
    Data : Map<string, decimal>
}
    with        
        member this.MermaidMarkdown =
            seq {
                yield sprintf "pie%s" (if this.ShowData then " showData" else "")
                if not (String.IsNullOrWhiteSpace this.Title) then yield $"    title {this.Title}"
                for kvp in this.Data do
                    yield $"    \"{kvp.Key}\" : {kvp.Value}"
            }
            |> String.concat "\n"

        interface IMermaidChart with member this.MermaidMarkdown = this.MermaidMarkdown
        



module PieChart =
    let Default = { Title = "" ; ShowData = false ; Data = Map.empty }


    let private decimalize<'t> (data : (string * 't) seq) =
        data
        |> Seq.map (Tuple.mapSnd (fun x -> Decimal.Round(Convert.ToDecimal(x), 2)))


    let WithTitle title pc = { pc with Title = title }


    let ShowData pc = { pc with ShowData = true }


    let HideData pc = { pc with ShowData = false }


    let SetData data pc = { pc with Data = data |> decimalize |> Map.ofSeq }


    let AppendData data pc = { pc with Data = pc.Data |> Map.addAll (decimalize data) }

