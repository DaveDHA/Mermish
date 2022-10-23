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
    [<Literal>]
    let private notImplemented = """
journey
  title This chart type is not yet implemented
    Waiting for Implementation: 1
    Using a Manual Chart to fill the gaps : 3
    This finally gets implemented: 5
    """

    let ManualChart str = { new IMermaidChart with member _.MermaidSyntax = str }
    
    let FlowChart = ManualChart notImplemented

    let SequenceDiagram = ManualChart notImplemented

    let ClassDiagram = ManualChart notImplemented

    let StateDiagram = ManualChart notImplemented

    let EntityRelationshipDiagram = ManualChart notImplemented

    let UserJourney = ManualChart notImplemented

    let GanttChart = ManualChart notImplemented

    let PieChart nodes = PieChart.AddAll nodes PieChart.Default

    let RequirementDiagram = ManualChart notImplemented

    let GitGraph = ManualChart notImplemented

    let ContextDiagram = ManualChart notImplemented

    let SyntaxFor item = (item :> IMermaidChart).MermaidSyntax

    let WriteAllToFile path items =
        let output = 
            seq {
                yield Html.head
                for item in items do
                    yield "        <div class='mermaid'>"
                    yield item |> SyntaxFor |> Html.indentify 12
                    yield "        </div>"
                yield Html.tail
            }

        System.IO.File.WriteAllLines(path, output)


    let WriteToFile path item = WriteAllToFile path [ item ]





        
    


