module FSharpTraining.SeqTests

open Swensen.Unquote
open Xunit

let private intList (s: string) =
    s.Split(';') |> Array.map int |> Array.toList

[<Theory>]
[<InlineData("✅", "0; 0; 1; 10; 2; 20; 3; 30")>]
[<InlineData("❌", "0; 0; 1; 10; 2; 20; 3; 30; 31")>]
let ``Seq with multiple of 10`` (_, expected: string) =
    test
        <@ seq {
            for i in 0..3 do
                yield i
                yield i * 10
           }
           |> Seq.toList = intList expected @>

[<Theory>]
[<InlineData("✅", "42; 43; 44; 45")>]
[<InlineData("❌", "42; 43; 44; 45; 46")>]
let ``Seq 42..45`` (_, expected: string) =
    test
        <@ seq {
            yield 42
            yield 43
            yield! [ 44; 45 ]
           }
           |> Seq.toList = intList expected @>