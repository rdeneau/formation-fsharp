module FSharpTraining.OptionTests

open Swensen.Unquote
open Xunit

type OptionBuilder() =
    member _.Bind(x, f) = x |> Option.bind f
    member _.Return(x) = Some x

let option = OptionBuilder()

let (>>=) x f = x |> Option.bind f

let tmp = Option.map2

[<Theory>]
[<InlineData(0)>]
[<InlineData(1)>]
[<InlineData(10)>]
let ``Monad associativity law`` value =
    let x = Some value
    let f y = Some (y * 10)
    let g z = Some (z + 1)
    let left = x >>= f >>= g
    let right = (x >>= f) >>= g
    left =! right
