module Mermish.GanttChart

// UNTESTED

open System
open Mermish.Utility


type GanttDate = string
type GanttId = string


////////////////////////////////////////////////////////
// Exclusion
type Exclusion = 
| Monday
| Tuesday
| Wendesday
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
| On of GanttDate
| Default


module StartOption =
    let Render = function
        | After id -> Some $"after {id}"
        | On d -> Some d
        | Default -> None



////////////////////////////////////////////////////////
// Duration
type Duration = 
| Until of GanttDate
| Weeks of int
| Days of int
| Hours of int
| Minutes of int
| Default


module Duration =
    let Render = function
        | Until d -> Some d
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


    let WithId id item = { item with Id = id }

    let Critical item = { item with IsCritical = true }

    let WithState state item = { item with State = state }

    let Starts start item = { item with StartOption = start }

    let Ends duration item = { item with Duration = duration }

    

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


let WithTitle title (chart : GanttChart) = { chart with Title = title }

let WithDateFormat df chart = { chart with DateFormat = df }

let WithAxisFormat af chart = { chart with AxisFormat = af }

let AddExclusions exs chart = { chart with Exclusions = chart.Exclusions |> Set.addAll exs }


let AddItems items (chart : GanttChart) =
    { chart with Items = chart.Items |> UMap.addAll (items |> Seq.map (fun i -> i.Id, i)) }


let AddSection name items chart = 
    let items' = items |> Seq.map (fun i -> i.Id, i) |> UMap.ofSeq
    let section = { Section.Name = name ; Items = items' }
    { chart with Sections = chart.Sections |> UMap.add section.Name section }


let CreateItem itemType duration name = 
    {
        Item.Id = ""
        Name = name
        ItemType = itemType
        IsCritical = false
        State = TaskState.Default
        StartOption = StartOption.Default
        Duration = duration
    }


let CreateTask = CreateItem Task 

let CreateMilestone = CreateItem Milestone (Days 1)    







