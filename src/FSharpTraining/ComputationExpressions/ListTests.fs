module FSharpTraining.ListTests

open Swensen.Unquote
open Xunit

type ListBuilder() =
    member _.Zero() = [] // List.empty
    member _.Yield(x) = [x] // List.singleton
    member _.YieldFrom(xs) = xs
    member _.Delay(thunk: unit -> 't list) = thunk () // eager evaluation
    member _.Combine(xs, ys) = xs @ ys // List.append
    member _.For(xs, f) = xs |> Seq.collect f |> Seq.toList

let list = ListBuilder()

let (|ListOfString|) (s: string) =
    s.Split([|';'|], System.StringSplitOptions.RemoveEmptyEntries)
    |> Seq.map _.Trim()
    |> List.ofSeq

[<Theory>]
[<InlineData("✅", "begin; 16; 9; 4; 1; 2; 4; 6; 8; end")>]
[<InlineData("❌", "")>]
let ``Desugar complex list {} expression`` _ (ListOfString expected) =
    let desugarList =
        list.Delay (fun () ->
            list.Combine(
                list.Yield "begin",
                list.Delay (fun () ->
                    list.Combine(
                        list.For({-4..4}, (fun i ->
                            if i < 0 then list.Yield $"{i * i}"
                            elif i > 0 then list.Yield $"{2 * i}"
                            else list.Zero()
                        )),
                        list.Delay (fun () ->
                            list.Yield "end"
                        ))
                ))
        )

    test
        <@
            list {
                yield "begin"
                for i in -4..4 do
                    if i < 0 then yield $"{i * i}"
                    elif i > 0 then yield $"{2 * i}"
                yield "end"
            }
             = expected
        @>

    desugarList =! expected

let tmp =
    Seq.delay (fun () ->
        Seq.append
            (Seq.singleton "begin")
            (Seq.delay (fun () ->
                Seq.append
                    (
                     {-4..4}
                     |> Seq.collect (fun i ->
                         if i < 0 then Seq.singleton $"{i * i}"
                         elif i > 0 then Seq.singleton $"{2 * i}"
                         else Seq.empty
                    ) )
                    (Seq.delay (fun () ->
                        Seq.singleton "end"
                    )
                )
            )
        )
    )
    |> Seq.toList

[<Theory>]
[<InlineData("✅", "begin; 16; 9; 4; 1; 2; 4; 6; 8; end")>]
[<InlineData("❌", "")>]
let ``Desugar complex list [] comprehension`` _ (ListOfString expected) =
    let desugarList =
        list.Delay (fun () ->
            list.Combine(
                list.Yield("begin"),
                list.Delay (fun () ->
                    list.Combine(
                        list.For({-4..4}, (fun i ->
                            if i < 0 then list.Yield($"{i * i}")
                            elif i > 0 then list.Yield($"{2 * i}")
                            else list.Zero()
                        )),
                        list.Delay (fun () ->
                            list.Yield("end")
                        ))
                ))
        )

    test
        <@
            [
                yield "begin"
                for i in -4..4 do
                    if i < 0 then yield $"{i * i}"
                    elif i > 0 then yield $"{2 * i}"
                yield "end"
            ]
             = expected
        @>

    desugarList =! expected
