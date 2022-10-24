module Mermish.Utility

open System.Collections
open System.Collections.Generic


module Tuple =
    let mapFst mapper (a,b) = (mapper a, b)

    let mapSnd mapper (a, b) = (a, mapper b)


module Map =
    let addAll items map =
        items
        |> Seq.fold (fun m (a,b) -> Map.add a b m) map
    

// UMap is a Map that preserves the order of items added
type UMap<'k,'v> = private UMap of ('k * 'v) list
    with
        interface IEnumerable<'k * 'v> with
            member this.GetEnumerator() = let (UMap lst) = this in (lst :> IEnumerable<'k * 'v>).GetEnumerator()

        interface IEnumerable with
            member this.GetEnumerator() = (this :> IEnumerable<'k * 'v>).GetEnumerator()


module UMap =
    let empty<'k, 'v> = List.empty<'k * 'v> |> UMap

    let ofSeq items = items |> List.ofSeq |> UMap
    let ofList items = UMap items
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
