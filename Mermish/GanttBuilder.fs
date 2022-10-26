namespace Mermish


// UNTESTED

open System
open Mermish.Utility
open Mermish.GanttChart

module gantt =
    
    ////////////////////////////////////////////////////////
    // ItemBuilder   
    type ItemBuilder(itemType, name) =
        let defaultItem = {
            Item.Id = Guid.NewGuid().ToString()
            Item.Name = name
            ItemType = itemType
            IsCritical = false
            State = TaskState.Default
            StartOption = StartOption.Default
            Duration = Days 1
        }
        

        let duration ctor (state, (x : int)) = { state with Duration = (ctor x) }


        let adjustState taskState x state =
            let finalState =
                match x, state.State with
                | false, s when s = taskState -> TaskState.Default
                | true, _ -> taskState
                | _, original -> original
            
            { state with State = finalState }


        member _.Zero() = defaultItem
        member _.Yield(()) = defaultItem

        [<CustomOperation("id")>]
        member _.Id((state : Item), id) = { state with Id = id }

        [<CustomOperation("isCritical")>]
        member _.IsCritical(state) = { state with IsCritical = true }

        [<CustomOperation("isCritical")>]
        member _.IsCritical(state, x) = { state with IsCritical = x }

        [<CustomOperation("isActive")>]
        member _.IsActive(state) = { state with State = Active }

        [<CustomOperation("isActive")>]
        member _.IsActive(state, x) = adjustState Active x state

        [<CustomOperation("isDone")>]
        member _.IsDone(state) = { state with State = Done }

        [<CustomOperation("isDone")>]
        member _.IsDone((state, x)) = adjustState Done x state

        [<CustomOperation("startAfter")>]
        member _.StartAfter(state, id) = { state with StartOption = After id }

        [<CustomOperation("startAfter")>]
        member _.StartAfter(state, target) = { state with StartOption = After target.Id }

        [<CustomOperation("startAt")>]
        member _.StartAt(state, date) = { state with StartOption = At date }

        [<CustomOperation("endAt")>]
        member _.EndAt(state, date) = { state with Duration = EndAt date }
        
        [<CustomOperation("durationWeeks")>]
        member _.DurationWeeks(state, x) = duration Weeks (state, x)

        [<CustomOperation("durationDays")>]
        member _.DurationDays(state, x) = duration Days (state, x)

        [<CustomOperation("durationHours")>]
        member _.DurationHours(state, x) = duration Hours (state, x)

        [<CustomOperation("durationMinutes")>]
        member _.DurationMinutes(state, x) = duration Minutes (state, x)



    let task name = ItemBuilder(Task, name)

    let milestone name = ItemBuilder(Milestone, name)

    ////////////////////////////////////////////////////////
    // SectionBuilder    
    type SectionBuilder(name) =
        member _.Zero() = []

        member _.Yield(x : Item) = [x]

        member _.YieldFrom (x : Item list) = x

        member _.Combine((x : Item list), y) = x @ y

        member _.Delay f = f()

        member _.For (items : 't seq, f : 't -> Item seq) =
            items
            |> Seq.collect f
            |> Seq.toList

        member _.Run(items : Item list) =
            {
                Section.Name = name
                Items = items |> Seq.map (fun i -> i.Id, i) |> UMap.ofSeq
            }
        


    let section name = SectionBuilder(name)

        

    ////////////////////////////////////////////////////////
    // GanttChartBuilder        
    type BuilderNode = 
    | Item of Item 
    | Section of Section 
    | Title of string
    | DateFormat of string
    | AxisFormat of string
    | Exclusion of Exclusion

    let title = Title

    let dateFormat = DateFormat

    let axisFormat = AxisFormat

    let excludeMondays = Exclusion Monday

    let excludeTuesdays = Exclusion Tuesday

    let excludeWednesdays = Exclusion Wednesday

    let excludeThursdays = Exclusion Thursday

    let excludeFridays = Exclusion Friday

    let excludeSaturdays = Exclusion Saturday

    let excludeSundays = Exclusion Sunday

    let excludeWeekends = Exclusion Weekends

    let excludeDate = Date >> Exclusion


    type GanttChartBuilder() =
        member _.Zero() = []

        member _.Yield (x : Item) = [ Item x ]

        member _.YieldFrom (x : Item seq) = x |> Seq.map Item |> Seq.toList

        member _.Yield (x : Section) = [ Section x ]

        member _.YieldFrom (x : Section seq) = x |> Seq.map Section |> Seq.toList

        member _.Yield (x : BuilderNode) = [ x ]

        member _.YieldFrom (x : BuilderNode seq) = x |> List.ofSeq

        member _.Combine((x : BuilderNode list), y) = x @ y

        member _.Delay f = f()


        member _.For (items : 't seq, f : 't -> BuilderNode seq) =
            items
            |> Seq.collect f
            |> Seq.toList


        member _.Run(nodes : BuilderNode list) =            
            let getLast chooser = nodes |> Seq.pickBackWithDefault "" chooser

            let excludes = 
                nodes
                |> Seq.choose (function | Exclusion ex -> Some ex | _ -> None)
                |> Set.ofSeq

            let items = 
                nodes
                |> Seq.choose (function | Item i -> Some i | _ -> None)
                |> Seq.map (fun i -> i.Id, i)
                |> UMap.ofSeq

            let sections = 
                nodes
                |> Seq.choose (function | Section s -> Some s | _ -> None)
                |> Seq.map (fun s -> s.Name, s)
                |> UMap.ofSeq

            {
                GanttChart.Title = getLast (function Title x -> Some x | _ -> None)
                DateFormat = getLast (function DateFormat x -> Some x | _ -> None)
                AxisFormat = getLast (function AxisFormat x -> Some x | _ -> None)
                Exclusions = excludes
                Items = items
                Sections = sections
            }
                
    
    let chart = GanttChartBuilder()
