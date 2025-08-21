module FSharpTraining.ValidationTests

open Swensen.Unquote
open Xunit

module FsToolkit =
    open FsToolkit.ErrorHandling

    [<Theory>]
    [<InlineData("✅", 11)>]
    [<InlineData("❌", 0)>]
    let ``Desugar let! ... and!`` (_, expected) =
        test
            <@ validation {
                let! x = Validation.ok 1
                and! y = Validation.ok 10
                return x + y
            } = Validation.ok expected @>

        let desugarValidation =
            validation.Run(
                validation.Delay(fun () ->
                    validation.BindReturn(
                        validation.MergeSources( // ↩
                            validation.Source(Validation.ok 1),
                            validation.Source(Validation.ok 10)
                        ),
                        (fun (x, y) -> x + y)
                    )
                )
            )

        desugarValidation =! Validation.ok expected

module Custom =
    type Validation<'t, 'e> = Result<'t, 'e list>

    module Validation =
        let ok x = Ok x
        let error e = Error [ e ]

        type ValidationBuilder() =
            member _.BindReturn(x: Validation<'t, 'e>, f: 't -> 'u) = Result.map f x
            member _.Bind(x: Validation<'t, 'e>, f: 't -> Validation<'u, 'e>) : Validation<'u, 'e> = Result.bind f x
            member _.Return(x: 't) = Ok x
            member _.ReturnFrom(x: Validation<'t, 'e>) = x

            member _.MergeSources(x: Validation<'t, 'e>, y: Validation<'u, 'e>) =
                match (x, y) with
                | Ok v1, Ok v2 -> Ok(v1, v2)
                | Error e1, Error e2 -> Error(e1 @ e2)
                | Error e, _
                | _, Error e -> Error e

    let validation = Validation.ValidationBuilder()

    [<Theory>]
    [<InlineData("✅", 11)>]
    [<InlineData("❌", 0)>]
    let ``Desugar let! ... and!`` (_, expected) =
        test
            <@ validation {
                let! x = Validation.ok 1
                and! y = Validation.ok 10
                return x + y
            } = Validation.ok expected @>

        let ``desugaring given Bind and Return without BindReturn`` =
            validation.Bind(
                validation.MergeSources( // ↩
                    Validation.ok 1,
                    Validation.ok 10
                ),
                (fun (x, y) -> validation.Return(x + y))
            )

        ``desugaring given Bind and Return without BindReturn`` =! Validation.ok expected

        let ``desugaring given BindReturn`` =
            validation.BindReturn(
                validation.MergeSources( // ↩
                    Validation.ok 1,
                    Validation.ok 10
                ),
                (fun (x, y) -> x + y)
            )

        ``desugaring given BindReturn`` =! Validation.ok expected