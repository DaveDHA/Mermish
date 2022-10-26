module Mermish.GanttChart

open System
open Mermish.Utility


type GanttDate = string
type GanttId = string


////////////////////////////////////////////////////////
// Exclusion
type Exclusion = 
| Monday
| Tuesday
| Wednesday
| Thursday
| Friday
| Saturday
| Sunday
| Weekends
| Date of GanttDate


module Exclusion =
    let Render = function
        | Date d -> d
        | other -> $"{other}".ToLower()


    let RenderAll items = 
        seq {
            if not (Seq.isEmpty items) then
                yield
                    items 
                    |> Seq.map Render
                    |> String.concat ", "
                    |> sprintf "    excludes %s"
        }



////////////////////////////////////////////////////////
// StartOption
type StartOption = 
| After of GanttId
| At of GanttDate
| Default


module StartOption =
    let Render = function
        | After id -> Some $"after {id}"
        | At d -> Some d
        | Default -> None



////////////////////////////////////////////////////////
// Duration
type Duration = 
| EndAt of GanttDate
| Weeks of int
| Days of int
| Hours of int
| Minutes of int
| Default


module Duration =
    let Render = function
        | EndAt d -> Some d
        | Weeks w -> Some (sprintf "%dw" w)
        | Days d -> Some (sprintf "%dd" d)
        | Hours h -> Some (sprintf "%dh" h)
        | Minutes m -> Some (sprintf "%dmin" m)
        | Default -> None


////////////////////////////////////////////////////////
// TaskState
type TaskState = Done | Active | Default

module TaskState = 
    let Render = function
        | Default -> None
        | other -> Some ($"{other}".ToLower())



////////////////////////////////////////////////////////
// Item
type ItemType = Task | Milestone

type Item = {
    Id : GanttId
    Name : string
    ItemType : ItemType
    IsCritical : bool
    State : TaskState
    StartOption : StartOption
    Duration : Duration
}

module Item =        
    let Default() =
        {
            Id = Guid.NewGuid().ToString()
            Item.Name = ""
            ItemType = Task
            IsCritical = false
            State = TaskState.Default
            StartOption = StartOption.Default
            Duration = Days 1
        }

    let Render item =
        let fragments = 
            [
                if item.ItemType = Milestone then Some "milestone"
                if item.IsCritical then Some "crit" else None
                TaskState.Render item.State
                if (String.IsNullOrWhiteSpace item.Id) then None else Some item.Id
                StartOption.Render item.StartOption
                Duration.Render item.Duration
            ]
            |> Seq.choose id
            |> String.concat ", "
        
        $"    {item.Name} : {fragments}"


    let RenderAll = Seq.map Render
   


////////////////////////////////////////////////////////
// Section
type Section = {
    Name : string
    Items : UMap<GanttId, Item>
}


module Section =

    let Render section = 
        seq {
            yield $"    Section {section.Name}"
            yield! section.Items |> UMap.values |> Item.RenderAll
        }


    let RenderAll = Seq.collect Render


////////////////////////////////////////////////////////
// GanttChart
[<StructuredFormatDisplay("{MermaidSyntax}")>]
type GanttChart = {
    Title : string
    DateFormat : string
    AxisFormat : string
    Exclusions : Exclusion Set
    Items : UMap<GanttId, Item>
    Sections : UMap<string, Section>
}
    with        
        member this.MermaidSyntax =
            let optionalLines items = seq {
                for (label, str) in items do
                    if not (String.IsNullOrWhiteSpace str) then yield $"    {label} {str}"
            }

            seq {
                yield "gantt"
                yield! optionalLines [ 
                        "title", this.Title
                        "dateFormat", this.DateFormat
                        "axisFormat", this.AxisFormat
                    ]
                yield! this.Exclusions |> Exclusion.RenderAll
                yield! this.Items |> UMap.values |> Item.RenderAll
                yield! this.Sections |> UMap.values |> Section.RenderAll
            }
            |> String.concat "\n"


        override this.ToString() = this.MermaidSyntax

        interface IMermaidChart with member this.MermaidSyntax = this.MermaidSyntax


////////////////////////////////////////////////////////
// GanttChart Module Members

let Default = {
    Title = ""
    DateFormat = ""
    AxisFormat = ""
    Exclusions = Set.empty
    Items = UMap.empty
    Sections = UMap.empty
}









