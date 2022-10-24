namespace Mermish

open System
open Mermish.Utility


type GanttDate = string
type GanttId = string


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
    let GetLine items = 
        seq {
            if not (Seq.isEmpty items) then
                yield
                    items 
                    |> Seq.map (function
                        | Date d -> d
                        | other -> $"{other}".ToLower()
                    )
                    |> String.concat ", "
                    |> sprintf "    excludes %s"
        }



type StartOption = 
| After of GanttId
| Date of GanttDate
| Default


module StartOption =
    let GetToken = function
        | After id -> Some $"after {id}"
        | Date d -> Some d
        | Default -> None



type Duration = 
| EndDate of GanttDate
| Weeks of int
| Days of int
| Hours of int
| Minutes of int
| Default


module Duration =
    let GetFragment = function
        | EndDate d -> Some d
        | Weeks w -> Some (sprintf "%dw" w)
        | Days d -> Some (sprintf "%dd" d)
        | Hours h -> Some (sprintf "%dh" h)
        | Minutes m -> Some (sprintf "%dmin" m)
        | Default -> None



type GanttState = Done | Active | Default

module GanttState = 
    let GetFragment = function
        | Default -> None
        | other -> Some ($"{other}".ToLower())



type GanttItem = {
    Name : string
    IsCritical : bool
    State : GanttState
    Id : GanttId
    Starts : StartOption
    Duration : Duration
}

module GanttItem =
    let Default = {
        Name = ""
        IsCritical = false
        State = Default
        Id = ""
        Starts = StartOption.Default
        Duration = Duration.Default
    }


    let GetLine item =
        let fragments = 
            [
                if item.IsCritical then Some "crit" else None
                GanttState.GetFragment item.State
                if (String.IsNullOrWhiteSpace item.Id) then None else Some item.Id
                StartOption.GetToken item.Starts
                Duration.GetFragment item.Duration
            ]
            |> Seq.choose id
            |> String.concat ", "
        
        $"    {item.Name} : {fragments}"
        


type Milestone = {
    Name : string
    Id : GanttId
    Starts : StartOption
    Duration : Duration
}


module Milestone = 
    let Default = {
        Name = ""
        Id = ""
        Starts = StartOption.Default
        Duration = Duration.Default
    }


    let GetLine item =
        let fragments = 
            [
                Some "milestone"
                if (String.IsNullOrWhiteSpace item.Id) then None else Some item.Id
                StartOption.GetToken item.Starts
                Duration.GetFragment item.Duration
            ]
            |> Seq.choose id
            |> String.concat ", "
    
        $"    {item.Name} : {fragments}"
    



type GanttRecord = 
| Section of string
| Item of GanttItem
| Milestone of Milestone


module GanttRecord =
    let GetLine = function
        | Section name -> $"    section {name}"
        | Item x -> GanttItem.GetLine x
        | Milestone x -> Milestone.GetLine x



[<StructuredFormatDisplay("{MermaidSyntax}")>]
type GanttChart = {
    Title : string
    DateFormat : string
    AxisFormat : string
    Exclusions : Exclusion list
    Records : GanttRecord list
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
                yield! Exclusion.GetLine this.Exclusions
                yield! this.Records |> Seq.map GanttRecord.GetLine
            }
            |> String.concat "\n"


        override this.ToString() = this.MermaidSyntax

        interface IMermaidChart with member this.MermaidSyntax = this.MermaidSyntax


type MilestoneNode = 
| Id of GanttId
| Start of StartOption
| Duration of Duration


type ItemNode = 
| Critical
| State of GanttState
| Id of GanttId
| Start of StartOption
| Duration of Duration


type GanttNode =
| Title of string
| DateFormat of string
| AxisFormat of string
| Exclude of Exclusion list
| Section of string
| Milestone of string * (MilestoneNode list)
| Item of string * (ItemNode list)


module GanttChart =
    let Default = {
        Title = ""
        DateFormat = ""
        AxisFormat = ""
        Exclusions = List.empty
        Records = List.empty
    }


    let private foldMilestoneNode (milestone : Milestone) node =
        match node with
        | MilestoneNode.Id x -> { milestone with Id = x }
        | MilestoneNode.Start s -> { milestone with Starts = s}
        | MilestoneNode.Duration d -> { milestone with Duration = d }


    let private buildMilestone name nodes =
        nodes
        |> Seq.fold foldMilestoneNode { Milestone.Default with Name = name }
        |> GanttRecord.Milestone


    let private foldItemNode item node =
        match node with
        | Critical -> { item with IsCritical = true }
        | State s -> { item with State = s }
        | Id id -> { item with Id = id }
        | Start s -> { item with Starts = s }
        | Duration d -> { item with Duration = d }
        

    let private buildItem name nodes =
        nodes
        |> Seq.fold foldItemNode { GanttItem.Default with Name = name }
        |> GanttRecord.Item


    let private foldGanttNode (chart : GanttChart) node =
        match node with
        | Title t -> { chart with Title = t }
        | DateFormat df -> { chart with DateFormat = df }
        | AxisFormat af -> { chart with AxisFormat = af }
        | Exclude exs -> { chart with Exclusions = chart.Exclusions @ exs }
        | Section name -> { chart with Records = chart.Records @ [ GanttRecord.Section name ] }
        | Milestone (name, nodes) -> { chart with Records = chart.Records @ [ buildMilestone name nodes ]}
        | Item (name, nodes) -> { chart with Records = chart.Records @ [ buildItem name nodes ] }


    let Add node chart = foldGanttNode chart node

    let AddAll nodes chart = nodes |> Seq.fold foldGanttNode chart

