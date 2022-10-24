namespace Mermish.Testing.Fs.Utility

open Xunit
open FsUnit.Xunit
open Mermish.Utility

module UMap =
    [<Fact>]
    let ``Can create from and convert to Seq``() =
        let original = seq { (1, 1) ; (2, 2) ; (3, 3) }
        let result = original |> UMap.ofSeq |> UMap.toSeq

        result |> should haveLength (Seq.length original)
        Seq.zip original result
        |> Seq.iter (fun (original', result') -> result' |> should equal original')


    [<Fact>]
    let ``Can create from and convert to List``() =
        let original = [ (1, 1) ; (2, 2) ; (3, 3) ]
        let result = original |> UMap.ofList |> UMap.toList

        result |> should equal original


    [<Fact>]
    let ``Is Enumerable``() =
        let mutable count = 0

        let map = 
            [ (1, 1) ; (2, 2) ; (3, 3) ]
            |> UMap.ofSeq

        for (k,v) in map do
            count <- count + 1
            k |> should equal v

        count |> should equal 3


    [<Fact>]
    let ``Is Indexable by Key``() =
        let result = UMap.ofSeq [ (1, 1) ; (2, 2) ; (3, 3) ]
        result[1] |> should equal 1
        result[2] |> should equal 2
        result[3] |> should equal 3
        (fun () -> result[4] |> ignore) |> should throw typeof<System.ArgumentException>



    [<Fact>]
    let ``Duplicate keys overwrite when converting from Seq``() =
        let original = seq { (1, "1") ; (2, "2") ; (3, "3") ; (2, "4") }
        let result = original |> UMap.ofSeq

        result |> should haveLength 3
        result[1] |> should equal "1"
        result[2] |> should equal "4"
        result[3] |> should equal "3"


    [<Fact>]
    let ``Duplicate keys overwrite when converting from List``() =
        let original = [ ("2", 2) ; ("1", 1) ; ("3", 3) ; ("2", 4) ]
        let result = original |> UMap.ofList

        result |> should haveLength 3
        result["1"] |> should equal 1
        result["2"] |> should equal 4
        result["3"] |> should equal 3
        
        