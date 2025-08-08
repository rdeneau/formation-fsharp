module FSharpTraining.EventuallyTests

type Eventually<'t> =
    | Done of 't
    | NotYetDone of (unit -> Eventually<'t>)

type EventuallyBuilder() =
    member _.Return x = Done x
    member _.ReturnFrom expr = expr
    member _.Zero() = Done()
    member _.Delay f = NotYetDone f

    member m.Bind(expr, f) =
        match expr with
        | Done x -> f x
        | NotYetDone work -> NotYetDone(fun () -> m.Bind(work (), f))

    member m.Combine(command, expr) = m.Bind(command, (fun () -> expr))

let eventually = EventuallyBuilder()

let step expr =
    match expr with
    | Done _ -> expr
    | NotYetDone func -> func ()

let delayPrintMessage i =
    NotYetDone(fun () ->
        printfn "Message %d" i
        Done ())

let test =
    eventually {
        do! delayPrintMessage 1
        do! delayPrintMessage 2
        return 3 + 4
    }

let step1 = test |> step // NotYetDone <fun:Bind@14-1>
let step2 = step1 |> step // = NotYetDone <fun:Bind@14-1>
let step3 = step2 |> step // = Done 7
