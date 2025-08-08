module FSharpTraining.ListSeqTests

open Swensen.Unquote
open Xunit

// Build a list using internally sequences, like the list comprehension does.
type ListSeqBuilder() =
    member inline _.Zero() = Seq.empty
    member inline _.Yield(x) = Seq.singleton x
    member inline _.YieldFrom(xs) = Seq.ofList xs
    member inline _.Delay([<InlineIfLambda>] thunk) = Seq.delay thunk
    member inline _.Combine(xs, ys) = Seq.append xs ys
    member inline _.For(xs, [<InlineIfLambda>] f) = xs |> Seq.collect f
    member inline _.Run(xs) = xs |> Seq.toList

let listSeq = ListSeqBuilder()

let (|ListOfInt|) (s: string) =
    s.Split([|';'|], System.StringSplitOptions.RemoveEmptyEntries)
    |> Seq.map int
    |> List.ofSeq

[<Theory>]
[<InlineData("✅", "-999; 16; 9; 4; 1; 2; 4; 6; 8; +999")>]
[<InlineData("❌", "")>]
let ``Desugar complex list {} expression`` _ (ListOfInt expected) =
    let desugarList =
        listSeq.Run(
            listSeq.Delay (fun () ->
                listSeq.Combine(
                    listSeq.Yield(-999),
                    listSeq.Delay (fun () ->
                        listSeq.Combine(
                            listSeq.For({-4..4}, (fun i ->
                                if i < 0 then listSeq.Yield(i * i)
                                elif i > 0 then listSeq.Yield(2 * i)
                                else listSeq.Zero()
                            )),
                            listSeq.Delay (fun () -> listSeq.Yield(999))
                        )
                    )
                )
            )
        )

    test
        <@
            listSeq {
                yield -999
                for i in -4..4 do
                    if i < 0 then yield i * i
                    elif i > 0 then yield 2 * i
                yield +999
            }
             = expected
        @>

    desugarList =! expected
