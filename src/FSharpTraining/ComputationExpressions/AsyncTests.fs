/// Testing async computation expression extended to support parallel execution of tasks unwrapped with `let! ... and! ...`
module FSharpTraining.AsyncTests

open System
open Swensen.Unquote
open Xunit

module Async =
    let bind2 (tx: Async<'x>) (ty: Async<'y>) (f: 'x -> 'y -> Async<'u>) : Async<'u> =
        async {
            let! cx = Async.StartChild tx
            let! cy = Async.StartChild ty
            let! rx = cx
            let! ry = cy
            return! f rx ry
        }

    let bind3 (tx: Async<'x>) (ty: Async<'y>) (tz: Async<'z>) (f: 'x -> 'y -> 'z -> Async<'u>) : Async<'u> =
        async {
            let! cx = Async.StartChild tx
            let! cy = Async.StartChild ty
            let! cz = Async.StartChild tz
            let! rx = cx
            let! ry = cy
            let! rz = cz
            return! f rx ry rz
        }

type AsyncBuilder with
    member _.Bind2Return(tx: Async<'x>, ty: Async<'y>, f: 'x * 'y -> 'u) : Async<'u> =
        Async.bind2 tx ty (fun x y -> async { return f (x, y) })

    member _.Bind3Return(tx: Async<'x>, ty: Async<'y>, tz: Async<'z>, f: 'x * 'y * 'z -> 'u) : Async<'u> =
        Async.bind3 tx ty tz (fun x y z -> async { return f (x, y, z) })

    member _.MergeSources(tx: Async<'x>, ty: Async<'y>) : Async<'x * 'y> =
        Async.bind2 tx ty (fun x y -> async { return x, y })

    member _.MergeSources3(tx: Async<'x>, ty: Async<'y>, tz: Async<'z>) : Async<'x * 'y * 'z> =
        Async.bind3 tx ty tz (fun x y z -> async { return x, y, z })

let returnAfter (t: TimeSpan) x =
    async {
        do! Async.Sleep t
        return x
    }

type Take =
    | LessThan of TimeSpan
    | MoreThan of TimeSpan

let should takes task =
    let sw = Diagnostics.Stopwatch.StartNew()
    let _ = Async.RunSynchronously task
    sw.Stop()
    let actualTime = sw.Elapsed

    test
        <@
            takes
            |> List.forall (
                function
                | Take.LessThan t -> actualTime < t
                | Take.MoreThan t -> actualTime > t
            )
        @>

[<Fact>]
let ``Should run the tasks in series`` () =
    let taskInSeries =
        async {
            let! x = 1 |> returnAfter (TimeSpan.FromMilliseconds 10)
            let! y = 2 |> returnAfter (TimeSpan.FromMilliseconds 15)
            let! z = 3 |> returnAfter (TimeSpan.FromMilliseconds 5)
            return x + y + z
        }

    taskInSeries |> should [ Take.MoreThan(TimeSpan.FromMilliseconds 40) ]

[<Fact>]
let ``Should run the 3 tasks in parallel`` () =
    let tasksInParallel =
        async {
            let! x = 1 |> returnAfter (TimeSpan.FromMilliseconds 10)
            and! y = 2 |> returnAfter (TimeSpan.FromMilliseconds 15)
            and! z = 3 |> returnAfter (TimeSpan.FromMilliseconds 5)
            return x + y + z
        }

    tasksInParallel |> should [ // ↩
        Take.MoreThan(TimeSpan.FromMilliseconds 15)
        Take.LessThan(TimeSpan.FromMilliseconds 40)
    ]