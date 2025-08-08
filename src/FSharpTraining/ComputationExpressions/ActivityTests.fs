module FSharpTraining.ActivityTests

open System
open System.ComponentModel
open System.Diagnostics
open System.Runtime.CompilerServices

/// <remarks>
/// We're using a delegate here for two reasons:
/// <list type="bullet">
/// <item>
///   If we use an F# function type, then a function such as <c>setTag</c> is compiled as a 3-argument function returning unit.
///   So when we call <c>setTag myTag myValue</c> inside a computation expression, it's a curried call, which doesn't get inlined correctly.
/// </item>
/// <item>
///   Since this function is never meant to be called by user code, but only by the computation expression methods,
///   it's better developer experience to use a nominal type.
/// </item>
/// </list>
/// </remarks>
type ActivityAction = delegate of Activity -> unit

let inline private action ([<InlineIfLambda>] f: Activity -> _) =
    ActivityAction(fun ac -> f ac |> ignore)

let inline addLink link = action _.AddLink(link)
let inline setTag name value = action _.SetTag(name, value)
let inline setStartTime time = action _.SetStartTime(time)

type ActivityExtensions =
    [<Extension; EditorBrowsable(EditorBrowsableState.Never)>]
    static member inline Zero(_: Activity | null) = ActivityAction(fun _ -> ())

    [<Extension; EditorBrowsable(EditorBrowsableState.Never)>]
    static member inline Yield(_: Activity | null, [<InlineIfLambda>] a: ActivityAction) = a

    // [<Extension; EditorBrowsable(EditorBrowsableState.Never)>]
    // static member inline Yield(_: Activity | null, ()) = ActivityAction(fun _ -> ())

    [<Extension; EditorBrowsable(EditorBrowsableState.Never)>]
    static member inline Combine(_: Activity | null, [<InlineIfLambda>] a1: ActivityAction, [<InlineIfLambda>] a2: ActivityAction) =
        ActivityAction(fun ac ->
            a1.Invoke(ac)
            a2.Invoke(ac)
        )

    [<Extension; EditorBrowsable(EditorBrowsableState.Never)>]
    static member inline For(_: Activity | null, s: seq<'T>, [<InlineIfLambda>] f: 'T -> ActivityAction) =
        ActivityAction(fun ac ->
            for x in s do
                f(x).Invoke(ac)
        )

    [<Extension; EditorBrowsable(EditorBrowsableState.Never)>]
    static member inline While(_: Activity | null, [<InlineIfLambda>] condition: unit -> bool, [<InlineIfLambda>] a: ActivityAction) =
        ActivityAction(fun ac ->
            while condition () do
                a.Invoke(ac)
        )

    [<Extension; EditorBrowsable(EditorBrowsableState.Never)>]
    static member inline Delay(_: Activity | null, [<InlineIfLambda>] f: unit -> ActivityAction) = f() // ActivityAction is already delayed by nature

    [<Extension; EditorBrowsable(EditorBrowsableState.Never)>]
    static member inline Run(ac: Activity | null, [<InlineIfLambda>] f: ActivityAction) =
        match ac with
        | null -> ()
        | ac -> f.Invoke(ac)

let activity = new Activity("Tests")

activity {
    setStartTime DateTime.UtcNow
    setTag "count" 2
}