module FSharpTraining.ValidationTests

open FsToolkit.ErrorHandling
open Swensen.Unquote
open Xunit

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