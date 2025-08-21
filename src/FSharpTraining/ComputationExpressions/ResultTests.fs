module FSharpTraining.ResultTests

open System
open Swensen.Unquote
open Xunit

module Result =
    type ResultBuilder() =
        member _.Bind(rx, f) = rx |> Result.bind f
        member _.Return(x) = Ok x
        member _.ReturnFrom(rx) = rx

        // Delayed execution inside the CE
        // member _.Delay(thunk: unit -> Result<_, _>) = thunk
        // member _.Run(delayedRX: unit -> Result<_, _>) = delayedRX ()

        // Sequencing
        member m.Zero() = m.Return(())

    (*
        member m.Combine(cmd: Result<unit, _>, delayedRY) = m.Bind(cmd, (fun () -> delayedRY ()))

        // Control flow
        member m.TryWith(delayedRX, handler) =
            try
                m.Run(delayedRX)
            with
            | e -> handler e

        member m.TryFinally(delayedRX, finalizer) =
            try
                m.Run(delayedRX)
            finally
                finalizer ()

        member m.Using(rx, f) =
            m.Bind(rx, (fun disposableRes ->
                m.TryFinally( // ↩
                    m.Delay(fun () -> f disposableRes),
                    finalizer = (fun () -> (disposableRes :> IDisposable).Dispose()))
            ))
    *)

    let result = ResultBuilder()

module Dice =
    open Result

    let rollDice =
        let random = Random(Guid.NewGuid().GetHashCode())
        fun () -> random.Next(1, 7)

    let tryGetDice dice =
        result {
            if rollDice () <> dice then
                return! Error $"Not the expected dice {dice}."
        }

module DiceTests =
    open Dice
    open Result

    [<Theory>]
    [<InlineData("✅", false)>]
    [<InlineData("❌", true)>]
    let ``Pair of 6`` (_, isEmptyError: bool) =
        test
            <@ (result {
                let n = 6
                do! tryGetDice n
                do! tryGetDice n
                return true
            } = Error "") = isEmptyError @>

        // Desugaring
        let n = 6
        result.Bind(tryGetDice n, (fun () ->
            result.Bind(tryGetDice n, (fun () ->
                result.Return(true)
            ))
        ))