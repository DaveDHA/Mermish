namespace Mermish

open System
open Mermish.Utility
open Mermish.GanttChart

module gantt =
    type GanttChartNode = 
    | Id of string

    type ItemBuilder(itemType, name) =
        let defaultItem = {
            Item.Id = Guid.NewGuid().ToString()
            Item.Name = name
            ItemType = itemType
            IsCritical = false
            State = TaskState.Default
            StartOption = StartOption.Default
            Duration = Duration.Default
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


        

