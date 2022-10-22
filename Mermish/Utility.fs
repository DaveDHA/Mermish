namespace Mermish

[<AutoOpen>]
module internal Utility =
    
    module Tuple =
        let mapFst mapper (a,b) = (mapper a, b)

        let mapSnd mapper (a, b) = (a, mapper b)


    module Map =
        let addAll items map =
            items
            |> Seq.fold (fun m (a,b) -> Map.add a b m) map
    


