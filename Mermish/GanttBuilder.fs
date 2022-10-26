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

        member _.Zero() = defaultItem
        member _.Yield(()) = defaultItem

        [<CustomOperation("id")>]
        member _.Name((state : Item), id) = { state with Id = id }

        [<CustomOperation("critical")>]
        member _.Critical(state) = { state with IsCritical = true }

        [<CustomOperation("activeState")>]
        member _.Active(state) = { state with State = Active }

        [<CustomOperation("doneState")>]
        member _.Done(state) = { state with State = Done }

        [<CustomOperation("startsAfter")>]
        member _.StartsAfter(state, id) = { state with StartOption = After id }

        [<CustomOperation("startsAfter")>]
        member _.StartsAfter(state, target) = { state with StartOption = After target.Id }

        [<CustomOperation("startsOn")>]
        member _.StartsOn(state, date) = { state with StartOption = On date }

        [<CustomOperation("endsAfter")>]
        member _.EndsAfter(state, date) = { state with Duration = Until date }
        
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
    // GanttPropertiesBuilder

    type GanttProperties = {
        Title : string
        DateFormat : string
        AxisFormat : string
    }
        with static member Default = { Title = "" ; DateFormat = "YYYY-MM-DD" ; AxisFormat = "" }

    type GanttPropertiesBuilder() =

        member _.Zero() = GanttProperties.Default
        member _.Yield(()) = GanttProperties.Default

        [<CustomOperation("title")>]
        member _.Title((state : GanttProperties), title) = { state with Title = title }

        [<CustomOperation("dateFormat")>]
        member _.DateFormat((state : GanttProperties), df) = { state with DateFormat = df }

        [<CustomOperation("axisFormat")>]
        member _.AxisFormat((state : GanttProperties), af) = { state with AxisFormat = af }



    let props = GanttPropertiesBuilder()


    ////////////////////////////////////////////////////////
    // ExclusionBuilder        
    type ExclusionBuilder() =
        member _.Zero() = []

        member _.Yield(x : Exclusion) = [x]

        member _.YieldFrom (x : Exclusion list) = x

        member _.Combine((x : Exclusion list), y) = x @ y

        member _.Delay f = f()

        member _.For (items : 't seq, f : 't -> Exclusion seq) =
            items
            |> Seq.collect f
            |> Seq.toList


    let excludes = ExclusionBuilder()
        

    ////////////////////////////////////////////////////////
    // GanttChartBuilder        
    type BuilderNode = 
    | Item of Item 
    | Section of Section 
    | Properties of GanttProperties
    | Exclusion of Exclusion 


    type GanttChartBuilder() =
        member _.Zero() = []

        member _.Yield (x : Item) = [ Item x ]

        member _.YieldFrom (x : Item list) = x |> List.map Item 

        member _.Yield (x : Section) = [ Section x ]

        member _.YieldFrom (x : Section list) = x |> List.map Section

        member _.Yield (x : GanttProperties) = [ Properties x ]

        member _.YieldFrom (x : GanttProperties list) = x |> List.map Properties

        member _.Yield (x : Exclusion) = [ Exclusion x ]

        member _.YieldFrom (x : Exclusion list) = x |> List.map Exclusion

        member _.Combine((x : BuilderNode list), y) = x @ y

        member _.Delay f = f()


        member _.For (items : 't seq, f : 't -> BuilderNode seq) =
            items
            |> Seq.collect f
            |> Seq.toList


        member _.Run(nodes : BuilderNode list) =
            let props = 
                nodes 
                |> Seq.choose (function | Properties p -> Some p | _ -> None)
                |> Seq.tryLast
                |> function
                    | None -> GanttProperties.Default
                    | Some p -> p

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
                GanttChart.Title = props.Title
                DateFormat = props.DateFormat
                AxisFormat = props.AxisFormat
                Exclusions = excludes
                Items = items
                Sections = sections
            }
                
    
    let chart = GanttChartBuilder()
