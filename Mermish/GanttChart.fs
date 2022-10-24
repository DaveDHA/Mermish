namespace Mermish

open System


[<StructuredFormatDisplay("{MermaidSyntax}")>]
type GanttChart = {
    Title : string
    DateFormat : string
}
    with        
        member this.MermaidSyntax =
            seq {
                yield "gantt"
                if not (String.IsNullOrEmpty this.Title) then yield $"    title {this.Title}"
                if not (String.IsNullOrEmpty this.DateFormat) then yield $"    dateFormat {this.DateFormat}"
            }
            |> String.concat "\n"

        override this.ToString() = this.MermaidSyntax

        interface IMermaidChart with member this.MermaidSyntax = this.MermaidSyntax