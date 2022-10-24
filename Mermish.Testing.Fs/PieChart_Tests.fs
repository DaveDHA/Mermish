module Mermish.Testing.Fs.PieChart

open Xunit
open FsUnit.Xunit
open Mermish
open Mermish.Utility

[<Fact>]
let ``Syntax starts with 'pie'``() =
    ({
        PieChart.Title = "Wow!"
        ShowData = true
        Data = UMap.empty
    } :> IMermaidChart).MermaidSyntax
    |> should startWith "pie"


[<Fact>]
let ``Renders Title`` () =
    ({
        PieChart.Title = "Wow!"
        ShowData = true
        Data = UMap.empty
    } :> IMermaidChart).MermaidSyntax
    |> should haveSubstring "title Wow!"

    
[<Fact>]
let ``Empty Title`` () =
    ({
        PieChart.Title = ""
        ShowData = true
        Data = UMap.empty
    } :> IMermaidChart).MermaidSyntax
    |> should not' (haveSubstring "title")


[<Fact>]
let ``Renders ShowData``() =
    ({
        PieChart.Title = "Wow!"
        ShowData = true
        Data = UMap.empty
    } :> IMermaidChart).MermaidSyntax
    |> should haveSubstring "showData"


[<Fact>]
let ``Renders without ShowData``() =
    ({
        PieChart.Title = "Wow!"
        ShowData = false
        Data = UMap.empty
    } :> IMermaidChart).MermaidSyntax
    |> should not' (haveSubstring "showData")


[<Fact>]
let ``Renders all slices``() =
    let output = 
        ({
            PieChart.Title = "Wow!"
            ShowData = false
            Data = [ "one", 1M ; "second item", 2.5M ] |> UMap.ofSeq
        } :> IMermaidChart).MermaidSyntax
        
    output |> should haveSubstring "\"one\" : 1"
    output |> should haveSubstring "\"second item\" : 2.5"


[<Fact>]
let ``Supports string formatting``() =
    let chart = {
        PieChart.Title = "Wow!"
        ShowData = false
        Data = [ "one", 1M ; "second item", 2.5M ] |> UMap.ofSeq
    }

    sprintf "%A" chart |> should equal (chart :> IMermaidChart).MermaidSyntax


[<Fact>]
let ``Supports ToString``() =
    let chart = {
        PieChart.Title = "Wow!"
        ShowData = false
        Data = [ "one", 1M ; "second item", 2.5M ] |> UMap.ofSeq
    }

    chart.ToString() |> should equal (chart :> IMermaidChart).MermaidSyntax


[<Fact>]
let ``Can set Title``() =
    (Mermaid.PieChart [ Title "hello world" ]).Title
    |> should equal "hello world"
             

[<Fact>]
let ``Multiples Titles overwrite``() =
    (Mermaid.PieChart [ Title "hello world" ; Title "goodbye!" ]).Title
    |> should equal "goodbye!"


[<Fact>]
let ``Can ShowData``() =
    (Mermaid.PieChart [ ShowData ]).ShowData
    |> should equal true


[<Fact>]
let ``Can HideData``() =
    (Mermaid.PieChart [ HideData ]).ShowData
    |> should equal false


[<Fact>]
let ``Show/HideData overwrite``() =
    (Mermaid.PieChart [ ShowData ; HideData ]).ShowData
    |> should equal false

    (Mermaid.PieChart [ HideData ; ShowData ]).ShowData
    |> should equal true

    (Mermaid.PieChart [ ShowData ; HideData ; ShowData ]).ShowData
    |> should equal true


[<Fact>]
let ``Can add PieSlice``() =
    (Mermaid.PieChart [ PieSlice ("jello", 1)]).Data
    |> UMap.toSeq
    |> should contain ("jello", 1M)


[<Fact>]
let ``Can add multiple PieSlice items``() =
    let slices = 
        (Mermaid.PieChart [ PieSlice ("jello", 1) ; PieSlice ("banana", 2)]).Data
        |> UMap.toSeq

    slices |> Seq.length |> should equal 2
    slices |> should contain ("jello", 1M)
    slices |> should contain ("banana", 2M)


[<Fact>]
let ``Can add multiple slices with one PieSlices node``() =
    let slices = 
        (Mermaid.PieChart [ PieSlices [ ("jello", 1) ; ("banana", 2) ]]).Data
        |> UMap.toSeq

    slices |> Seq.length |> should equal 2
    slices |> should contain ("jello", 1M)
    slices |> should contain ("banana", 2M)


[<Fact>]
let ``Identically named slices overwrite``() =
    [
        Mermaid.PieChart [ PieSlice ("jello", 1) ; PieSlice ("jello", 2) ]
        Mermaid.PieChart [ PieSlices [ ("jello", 1) ; ("jello", 2) ]]
    ]
    |> Seq.iter (fun chart ->
        chart.Data |> UMap.toSeq |> should contain ("jello", 2M)            
    )
        

let multipleNodeSets = [
    [ 
        Title "have a nice day"
        ShowData
        PieSlice ("rainbows", 100)
        PieSlice ("unicorns", 100) 
    ]
    [ 
        Title "have a terrible day"
        PieSlices [ ("spiders", 100) ; ("snakes", 100) ]
        PieSlice ("stubbed toes", 3) 
    ]
    [ Title "monkey" ; Title "Lion" ]   // overwrites title
    [ ShowData ; HideData ; ShowData ]  // overwrites ShowData
    [                                   // overwriting slice
        PieSlice ("unique", 1)
        PieSlice ("unique", 2)
        PieSlice ("unique", 3) 
    ] 
    [                                   // overwriting slices
        PieSlices [ ("u1", 1) ; ("u2", 1) ]
        PieSlices [ ("u1", 2) ; ("u2", 2) ]
        PieSlices [ ("u1", 3) ; ("u1", 3) ]
        PieSlices [ ("u2", 4) ; ("u2", 4) ]
    ]
]


[<Fact>]
let ``Construction via Add is the same as basic construction``() =
    multipleNodeSets
    |> Seq.iter (fun nodes ->            
        for initialTake in 1..((Seq.length nodes) - 1) do
            let expected = Mermaid.PieChart nodes
                
            nodes 
            |> Seq.skip initialTake
            |> Seq.fold (fun c n -> PieChart.Add n c) (nodes |> Seq.take initialTake |> Mermaid.PieChart)
            |> should equal expected
    )


[<Fact>]
let ``Construction via AddAll is the same as basic construction``() =
    multipleNodeSets
    |> Seq.iter (fun nodes ->            
        let expected = Mermaid.PieChart nodes

        for initialTake in 1..((Seq.length nodes) - 1) do                
            nodes
            |> Seq.take initialTake
            |> Mermaid.PieChart
            |> PieChart.AddAll (nodes |> Seq.skip initialTake)
            |> should equal expected
    )

    
[<Fact>]
let ``Construction via AddSlices is the same as basic construction``() =
    [
        [ ("item1", 1) ; ("item2", 2) ]
        [ ("item1", 1) ; ("item2", 2) ; ("item3", 1) ; ("item4", 2) ]
        [ ("unique", 1) ; ("unique", 2) ; ("unique", 3) ]
        [ ("unique", 1) ; ("different", 2) ; ("unique", 3) ]
        [ ("different", 1) ; ("unique", 2) ; ("unique", 3) ]
    ]
    |> Seq.iter (fun slices ->
        let expected = slices |> Seq.map PieSlice |> Mermaid.PieChart
        for initialTake in 1..((Seq.length slices) - 1) do
            slices
            |> Seq.take initialTake
            |> Seq.map (fun x -> PieSlice x)
            |> Mermaid.PieChart
            |> PieChart.AddSlices (slices |> Seq.skip initialTake)
            |> should equal expected                
    )


[<Fact>]
let ``Can RemoveSlice``() =
    let expected = Mermaid.PieChart [ PieSlice ("one", 1) ]
    expected
    |> PieChart.Add (PieSlice ("two", 2))
    |> PieChart.RemoveSlice "two"
    |> should equal expected


[<Fact>]
let ``Can construct with ints``() =
    Mermaid.PieChart [ PieSlice ("one", 1) ; PieSlices [ ("two", 2) ; ("three", 3) ]]
    |> should equal { 
        Title = ""
        ShowData = false
        Data = [ ("one", 1M) ; ("two", 2M) ; ("three", 3M) ] |> UMap.ofSeq 
    }


[<Fact>]
let ``Can construct with decimals``() =
    Mermaid.PieChart [ PieSlice ("one", 1.404M) ; PieSlices [ ("two", 2M) ; ("three", 3.8768M) ]]
    |> should equal { 
        Title = ""
        ShowData = false
        Data = [ ("one", 1.40M) ; ("two", 2M) ; ("three", 3.88M) ] |> UMap.ofSeq 
    }


[<Fact>]
let ``Can construct with floats``() =
    Mermaid.PieChart [ PieSlice ("one", 1.404) ; PieSlices [ ("two", 2.0) ; ("three", 3.8768) ]]
    |> should equal { 
        Title = ""
        ShowData = false
        Data = [ ("one", 1.40M) ; ("two", 2M) ; ("three", 3.88M) ] |> UMap.ofSeq 
    }


[<Fact>]
let ``Can construct with strings``() =
    Mermaid.PieChart [ PieSlice ("one", "1.404") ; PieSlices [ ("two", "2") ; ("three", "3.8768") ]]
    |> should equal { 
        Title = ""
        ShowData = false
        Data = [ ("one", 1.40M) ; ("two", 2M) ; ("three", 3.88M) ] |> UMap.ofSeq 
    }
