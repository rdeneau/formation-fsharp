module FSharpTraining.ResultTests

open System

type ResultBuilder() =
    member _.Bind(rx, f) = rx |> Result.bind f
    member _.Return(x) = Ok x
    member _.ReturnFrom(rx) = rx

    // Delayed execution inside the CE
    member _.Delay(thunk: unit -> Result<_, _>) = thunk
    member _.Run(delayedRX: unit -> Result<_, _>) = delayedRX ()

    // Sequencing
    member m.Zero() = m.Return(())
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

let result = ResultBuilder()

// ---

let rollDice =
    let random = Random(Guid.NewGuid().GetHashCode())
    fun () -> random.Next(1, 7)

let tryGetDice dice =
    result {
        if rollDice () <> dice then
            return! Error $"Not the expected dice {dice}."
    }

let atRoll n res =
    match res with
    | Ok x -> Ok x
    | Error msg -> Error $"{msg} at roll #{n}"

let tryGet421 () =
    result {
        do! (tryGetDice 4 |> atRoll 1)
        do! (tryGetDice 2 |> atRoll 2)
        do! (tryGetDice 1 |> atRoll 3)
    }