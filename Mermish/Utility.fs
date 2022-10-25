module Mermish.Utility

open System.Collections
open System.Collections.Generic


// UNTESTED
module Tuple =
    let mapFst mapper (a,b) = (mapper a, b)

    let mapSnd mapper (a, b) = (a, mapper b)

    

// UNTESTED
module Set = 
    let addAll items set = 
        items |> Seq.fold (fun set item -> Set.add item set) set



// UMap is a Map that preserves the order of items added
type UMap<'k,'v when 'k : equality> = private UMap of ('k * 'v) list    
    with
        member private this.InnerList = let (UMap lst) = this in lst


        member this.Item with get key = 
            this.InnerList
            |> List.tryFind (fun (k,_) -> k = key)
            |> function
                | Some (_,v) -> v
                | None -> invalidArg (nameof key) "The index was outside the range of elements in the UMap."


        member this.Length = this.InnerList.Length


        interface IEnumerable<'k * 'v> with
            member this.GetEnumerator() = let (UMap lst) = this in (lst :> IEnumerable<'k * 'v>).GetEnumerator()


        interface IEnumerable with
            member this.GetEnumerator() = (this :> IEnumerable<'k * 'v>).GetEnumerator()


module UMap =
    let empty<'k, 'v when 'k : equality> = List.empty<'k * 'v> |> UMap
    
    // The double reverse is because we want the last item for a given key.
    // Not the greatest perf, but fine for our purpose in this library.
    let ofSeq items = items |> Seq.rev |> Seq.distinctBy fst |> Seq.rev |> List.ofSeq |> UMap
    
    let ofList items = items |> List.toSeq |> ofSeq
    
    let toSeq (UMap items) = items |> List.toSeq
    
    let toList (UMap items) = items


    let add key value (UMap items) =
        match items |> List.tryFindIndex (fun (k,_) -> k = key) with
        | None -> items @ [ (key, value) ] |> UMap
        | Some idx -> ((items |> List.removeAt idx) @ [ (key, value) ]) |> UMap


    let addAll items umap =
        items |> Seq.fold (fun umap (key, value) -> umap |> add key value ) umap


    let remove key (UMap items) = 
        items
        |> List.tryFindIndex(fun (k,_) -> k = key)
        |> function
            | Some idx -> items |> List.removeAt idx |> UMap
            | None -> failwith $"Key {key} not found in the given UMap."


    // UNTESTED
    let keys (UMap items) = items |> Seq.map fst
    
    // UNTESTED
    let values (UMap items) = items |> Seq.map snd
