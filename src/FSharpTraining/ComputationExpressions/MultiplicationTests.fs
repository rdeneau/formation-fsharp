module FSharpTraining.MultiplicationTests

open Swensen.Unquote
open Xunit

module Eager =
    type MultiplicationBuilder() =
        member _.Zero() = 1
        member _.Yield(x) = x
        member _.Delay(thunk: unit -> int) = thunk () // eager evaluation
        member _.Combine(x, y) = x * y

        member m.For(xs, f) =
            (m.Zero(), xs) ||> Seq.fold (fun res x -> m.Combine(res, f x))

    let multiplication = MultiplicationBuilder()

    [<Theory>]
    [<InlineData("✅", 10)>]
    [<InlineData("❌", 0)>]
    let ``Desugar 'multiplication { 5; 2 }'`` _ expected =
        let desugarMultiplication =
            multiplication.Delay(fun () ->
                multiplication.Combine( // ↩
                    multiplication.Yield(5),
                    multiplication.Delay(fun () -> multiplication.Yield(2))
                )
            )

        test <@ multiplication { 5; 2 } = expected @>
        test <@ desugarMultiplication = expected @>

    [<Theory>]
    [<InlineData(0, 1)>]
    [<InlineData(1, 1)>]
    [<InlineData(2, 2)>]
    [<InlineData(3, 6)>]
    [<InlineData(4, 24)>]
    let ``Factorial recursive`` n expected =
        let factorial n =
            let rec loop i =
                multiplication {
                    yield i
                    if i < n then yield loop (i + 1)
                }

            loop 1

        factorial n =! expected

    [<Theory>]
    [<InlineData(0, 1)>]
    [<InlineData(1, 1)>]
    [<InlineData(2, 2)>]
    [<InlineData(3, 6)>]
    [<InlineData(4, 24)>]
    let ``Factorial using for`` n expected =
        let factorial n = multiplication { for i in 1..n -> i }

        factorial n =! expected

    [<Theory>]
    [<InlineData("✅", 24)>]
    [<InlineData("❌", 0)>]
    let ``Desugar 'factorialOf 4'`` _ expected =
        let desugarMultiplication =
            multiplication.Delay (fun () ->
                multiplication.For({1..4}, multiplication.Yield)
            )

        test <@ multiplication { for i in 1..4 -> i } = expected @>
        test <@ desugarMultiplication = expected @>

module Lazy =
    type MultiplicationBuilder() =
        member _.Zero() = 1
        member _.Yield(x) = x
        member _.Delay(thunk: unit -> int) = thunk // lazy evaluation
        member _.Run(delayedX: unit -> int) = delayedX()

        member _.Combine(x: int, delayedY: unit -> int) : int =
            match x with
            | 0 -> 0 // short-circuit for multiplication by zero
            | _ -> x * delayedY()

        member m.For(xs, f) =
            (m.Zero(), xs) ||> Seq.fold (fun res x -> m.Combine(res, m.Delay(fun () -> f x)))

    let multiplication = MultiplicationBuilder()

    [<Theory>]
    [<InlineData("✅", 10)>]
    [<InlineData("❌", 0)>]
    let ``Desugar 'multiplication { 5; 2 }'`` _ expected =
        let desugarMultiplication =
            multiplication.Run(
                multiplication.Delay(fun () ->
                    multiplication.Combine(
                        multiplication.Yield(5),
                        multiplication.Delay(fun () ->
                            multiplication.Yield(2)
                        )
                    )
                )
            )

        test <@ multiplication { 5; 2 } = expected @>
        test <@ desugarMultiplication = expected @>

    [<Theory>]
    [<InlineData(0, 1)>]
    [<InlineData(1, 1)>]
    [<InlineData(2, 2)>]
    [<InlineData(3, 6)>]
    [<InlineData(4, 24)>]
    let ``Factorial using for`` n expected =
        let factorial n = multiplication { for i in 1..n -> i }

        factorial n =! expected
