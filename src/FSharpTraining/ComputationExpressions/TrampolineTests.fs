// Instructions to run these tests:
// 1. Open the file in your IDE.
// 2. Set breakpoints at the marked lines (1️⃣).
// 3. Run the tests in debug mode.
// 4. Observe the stack allocation issues in the naive implementations.
// 5. Run the tests for the trampoline implementations to see they do not have stack allocation issues.
// ---
// 🔗 The trampoline computation expression is inspired by:
//    - https://johnazariah.github.io/2020/12/07/bouncing-around-with-recursion.html#trampolines
//    - https://stackoverflow.com/a/49681264/8634147
//    - https://gist.github.com/Porges/dda3b1a5de3f8c8988962f891597cce5
module FSharpTraining.TrampolineTests

open Swensen.Unquote
open Xunit

module ``Naive implementations have stack allocation issues`` =
    let rec factorial n =
        if n = 0 then
            1 // 1️⃣ Place a breakpoint here to see the stack allocation
        else
            let res = factorial (n - 1)
            n * res

    [<Fact>]
    let ``Factorial of 5`` () =
        let result = factorial 5
        test <@ result = 5 * 4 * 3 * 2 @>

    let rec fibonacci n =
        match n with
        | 0 -> 1 // 1️⃣ Place a breakpoint here to see the stack allocation
        | 1 -> 1
        | _ -> fibonacci (n - 1) + fibonacci (n - 2)

    [<Theory>]
    [<InlineData(5)>]
    let ``Fibonacci of`` n =
        let result = fibonacci n
        test <@ result = fibonacci (n - 1) + fibonacci (n - 2) @>

module Trampoline =
    /// Trampoline type is a `Free (unit -> _)` monad with an additional `Bind` case to turn it stackless.
    type Trampoline<'a> =
        private
        /// Terminal case that returns a value
        | Return of 'a

        /// Non-terminal case that delays computation and allows continuation to be resumed later
        | Delay of cont: (unit -> Trampoline<'a>)

        /// Non-terminal case that binds a computation to a function, allowing for chaining
        /// This is not a real monadic bind because f should return a Trampoline<'b>
        | Bind of m: Trampoline<'a> * f: ('a -> Trampoline<'a>)

    let inline private bind f m = Bind(m, f)

    type TrampolineBuilder() =
        member _.Return(value) = Return value
        member _.ReturnFrom(m: Trampoline<'a>) = m
        member _.Bind(m: Trampoline<'a>, f) = bind f m
        member _.Delay(f) = Delay(fun () -> f ())

    let trampoline = TrampolineBuilder()

    let rec private run (t: Trampoline<'a>) : 'a =
        let next mx f =
            match mx with
            | Return x -> f x
            | Delay cont -> cont () |> bind f
            | Bind (my, g) -> my |> bind (g >> bind f)

        match t with
        | Return x -> x
        | Delay cont -> cont () |> run
        | Bind (mx, f) -> next mx f |> run

    type Trampoline<'a> with
        static member Eval(m: Trampoline<'a>) = run m

module ``Implementations based on trampoline CE does not have stack allocation issues`` =
    open Trampoline

    let factorial n =
        let rec loop n =
            trampoline {
                if n = 0 then
                    return 1 // 1️⃣ Place a breakpoint here to see the stack allocation
                else
                    let! res = loop (n - 1)
                    return n * res
            }

        if (n < 0) then
            invalidArg (nameof n) "should be > 0"
        else
            Trampoline.Eval(loop n)

    [<Fact>]
    let ``Factorial of 5`` () =
        let result = factorial 5
        test <@ result = 5 * 4 * 3 * 2 @>

    let fibonacci n =
        let rec loop n =
            trampoline {
                match n with
                | 0 -> return 1 // 1️⃣ Place a breakpoint here to see the stack allocation
                | 1 -> return 1
                | _ ->
                    let! ``fibonacci (n - 1)`` = loop (n - 1)
                    let! ``fibonacci (n - 2)`` = loop (n - 2)
                    return ``fibonacci (n - 1)`` + ``fibonacci (n - 2)``
            }

        if (n < 0) then
            invalidArg (nameof n) "should be > 0"
        else
            Trampoline.Eval(loop n)

    [<Theory>]
    [<InlineData(5)>]
    let ``Fibonacci of `` n =
        let result = fibonacci n
        test <@ result = fibonacci (n - 1) + fibonacci (n - 2) @>