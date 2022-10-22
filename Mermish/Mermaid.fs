namespace Mermish

open System


module private Html =
    [<Literal>]
    let head = """
<html>
    <body>
        <script src="https://cdn.jsdelivr.net/npm/mermaid/dist/mermaid.min.js"></script>
        <script>
            mermaid.initialize({ startOnLoad: true });
        </script>
"""

    [<Literal>]
    let tail = """
    </body>
</html>
"""


    let indentify spaces (text : string) =
        text.Split("\n")
        |> Seq.map (fun s -> sprintf "%s%s" (String.replicate spaces " ") s)
        |> String.concat "\n"



module Mermaid = 
    let private notImplemented = { new IMermaidChart with member this.MermaidSyntax = "not implemented" }

    let FlowChart = notImplemented

    let SequenceDiagram = notImplemented

    let ClassDiagram = notImplemented

    let StateDiagram = notImplemented

    let EntityRelationshipDiagram = notImplemented

    let UserJourney = notImplemented

    let GanttChart = notImplemented

    let PieChart = PieChart.Default

    let RequirementDiagram = notImplemented

    let GitGraph = notImplemented

    let ContextDiagram = notImplemented

    let ToSyntax item = (item :> IMermaidChart).MermaidSyntax


    let WriteAllToFile path items =
        let output = 
            seq {
                yield Html.head
                for item in items do
                    yield "        <div class='mermaid'>"
                    yield item |> ToSyntax |> Html.indentify 12
                    yield "        </div>"
                yield Html.tail
            }

        System.IO.File.WriteAllLines(path, output)


    let WriteToFile path item = WriteAllToFile path [ item ]

        
    


