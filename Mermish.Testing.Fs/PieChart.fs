namespace Mermish.Testing.Fs.PieChart

open System
open Xunit
open FsUnit
open FsUnit.Xunit
open Mermish

module ``Writing Mermaid syntax`` =
    [<Fact>]
    let ``Syntax starts with 'pie'``() =
        ({
            PieChart.Title = "Wow!"
            ShowData = true
            Data = Map.empty
        } :> IMermaidChart).MermaidSyntax
        |> should startWith "pie"


    [<Fact>]
    let ``Renders Title`` () =
        ({
            PieChart.Title = "Wow!"
            ShowData = true
            Data = Map.empty
        } :> IMermaidChart).MermaidSyntax
        |> should haveSubstring "title Wow!"

    
    [<Fact>]
    let ``Empty Title`` () =
        ({
            PieChart.Title = ""
            ShowData = true
            Data = Map.empty
        } :> IMermaidChart).MermaidSyntax
        |> should not' (haveSubstring "title")


    [<Fact>]
    let ``Renders ShowData``() =
        ({
            PieChart.Title = "Wow!"
            ShowData = true
            Data = Map.empty
        } :> IMermaidChart).MermaidSyntax
        |> should haveSubstring "showData"


    [<Fact>]
    let ``Renders without ShowData``() =
        ({
            PieChart.Title = "Wow!"
            ShowData = false
            Data = Map.empty
        } :> IMermaidChart).MermaidSyntax
        |> should not' (haveSubstring "showData")


    [<Fact>]
    let ``Renders all slices``() =
        let output = 
            ({
                PieChart.Title = "Wow!"
                ShowData = false
                Data = [ "one", 1M ; "second item", 2.5M ] |> Map.ofSeq
            } :> IMermaidChart).MermaidSyntax
        
        output |> should haveSubstring "\"one\" : 1"
        output |> should haveSubstring "\"second item\" : 2.5"


    [<Fact>]
    let ``Supports string formatting``() =
        let chart = {
            PieChart.Title = "Wow!"
            ShowData = false
            Data = [ "one", 1M ; "second item", 2.5M ] |> Map.ofSeq
        }

        sprintf "%A" chart |> should equal (chart :> IMermaidChart).MermaidSyntax


module Construction =
    
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
        |> Map.toSeq
        |> should contain ("jello", 1M)


    [<Fact>]
    let ``Can add multiple PieSlice items``() =
        let slices = 
            (Mermaid.PieChart [ PieSlice ("jello", 1) ; PieSlice ("banana", 2)]).Data
            |> Map.toSeq

        slices |> Seq.length |> should equal 2
        slices |> should contain ("jello", 1M)
        slices |> should contain ("banana", 2M)


    [<Fact>]
    let ``Can add multiple slices with one PieSlices node``() =
        let slices = 
            (Mermaid.PieChart [ PieSlices [ ("jello", 1) ; ("banana", 2) ]]).Data
            |> Map.toSeq

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
            chart.Data |> Map.toSeq |> should contain ("jello", 2M)            
        )
        

    [<Fact>]
    let ``Construction via Add is the same as basic construction``() =
        [
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
        |> Seq.iter (fun nodes ->            
            for initialTake in 1..((Seq.length nodes) - 1) do
                let c1 = Mermaid.PieChart nodes
                let c2 = nodes |> Seq.take initialTake |> Mermaid.PieChart
                let c3 = 
                    nodes 
                    |> Seq.skip initialTake
                    |> Seq.fold (fun c n -> PieChart.Add n c) c2
                c3 |> should equal c1
        )