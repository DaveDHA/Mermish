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
    let private notImplemented = ManualChart """
journey
  title This chart type is not yet implemented
    Waiting for Implementation: 1
    Using a Manual Chart to fill the gaps : 3
    This finally gets implemented: 5
    """

    let ManualChart str = ManualChart str
    
    let FlowChart = notImplemented

    let SequenceDiagram = notImplemented

    let ClassDiagram = notImplemented

    let StateDiagram = notImplemented

    let EntityRelationshipDiagram = notImplemented

    let UserJourney = notImplemented

    let GanttChart = GanttChart.Default

    let PieChart nodes = PieChart.AddAll nodes PieChart.Default

    let RequirementDiagram = notImplemented

    let GitGraph = notImplemented

    let ContextDiagram = notImplemented

    let WriteAllToFile path items =
        let output = 
            seq {
                yield Html.head
                for item in (items |> Seq.cast<IMermaidChart>) do
                    yield "        <div class='mermaid'>"
                    yield item.MermaidSyntax |> Html.indentify 12
                    yield "        </div>"
                yield Html.tail
            }

        System.IO.File.WriteAllLines(path, output)


    let WriteToFile path item = WriteAllToFile path [ item ]





        
    


