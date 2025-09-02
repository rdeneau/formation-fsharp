module FSharpTraining.ValidationTests

open Swensen.Unquote
open Xunit

module Custom =
    type Validation<'t, 'e> = Result<'t, 'e list>

    let (|Err|) (vx: Validation<'x, 'e>) =
        match vx with
        | Ok _ -> []
        | Error errors -> errors

    module Validation =
        let ok (x: 'x) : Validation<'x, 'e> = Ok x
        let error (e: 'e) : Validation<'x, 'e> = Error [ e ]
        let errors (e: 'e list) : Validation<'x, 'e> = Error e

        let bind2 (vx: Validation<'x, 'e>) (vy: Validation<'y, 'e>) (f: 'x -> 'y -> Validation<'u, 'e>) : Validation<'u, 'e> =
            match vx, vy with
            | Ok x, Ok y -> f x y
            | Err ex, Err ey -> Error(ex @ ey)

        let bind3 (vx: Validation<'x, 'e>) (vy: Validation<'y, 'e>) (vz: Validation<'z, 'e>) (f: 'x -> 'y -> 'z -> Validation<'u, 'e>) : Validation<'u, 'e> =
            match vx, vy, vz with
            | Ok x, Ok y, Ok z -> f x y z
            | Err ex, Err ey, Err ez -> Error(ex @ ey @ ez)

    // The builder is empty: its methods are defined by type augmentations provided in the ValidationBuilderMethod module below
    type ValidationBuilder() =
        do ()

    let validation = ValidationBuilder()

    module ValidationBuilderMethod =
        module Bind =
            type ValidationBuilder with
                member _.Bind(x: Validation<'t, 'e>, f: 't -> Validation<'u, 'e>) : Validation<'u, 'e> = // ↩
                    Result.bind f x

        module BindReturn =
            type ValidationBuilder with
                member _.BindReturn(x: Validation<'t, 'e>, f: 't -> 'u) : Validation<'u, 'e> = // ↩
                    Result.map f x

        module Bind2 =
            type ValidationBuilder with
                member _.Bind2(vx: Validation<'x, 'e>, vy: Validation<'y, 'e>, f: 'x * 'y -> Validation<'u, 'e>) : Validation<'u, 'e> =
                    Validation.bind2 vx vy (fun x y -> f (x, y))

        module Bind2Return =
            type ValidationBuilder with
                member _.Bind2Return(vx: Validation<'x, 'e>, vy: Validation<'y, 'e>, f: 'x * 'y -> 'u) : Validation<'u, 'e> =
                    Validation.bind2 vx vy (fun x y -> Validation.ok (f (x, y)))

        module Bind3Return =
            type ValidationBuilder with
                member _.Bind3Return(vx: Validation<'x, 'e>, vy: Validation<'y, 'e>, vz: Validation<'z, 'e>, f: 'x * 'y * 'z -> 'u) : Validation<'u, 'e> =
                    Validation.bind3 vx vy vz (fun x y z -> Validation.ok (f (x, y, z)))

        module MergeSources =
            type ValidationBuilder with
                member _.MergeSources(vx: Validation<'x, 'e>, vy: Validation<'y, 'e>) : Validation<'x * 'y, 'e> =
                    Validation.bind2 vx vy (fun x y -> Validation.ok (x, y))

        module MergeSources3 =
            type ValidationBuilder with
                member _.MergeSources3(vx: Validation<'x, 'e>, vy: Validation<'y, 'e>, vz: Validation<'z, 'e>) : Validation<'x * 'y * 'z, 'e> =
                    Validation.bind3 vx vy vz (fun x y z -> Validation.ok (x, y, z))

        module Return =
            type ValidationBuilder with
                member _.Return(x: 't) = Validation.ok x

    // -- Tests ---

    type TestCase =
        | OutputDesugaring
        | CheckDesugaring

        member this.Invoke(expr, desugaring) =
            match this with
            | OutputDesugaring -> test <@ (%expr) = Validation.ok 0 @> // Force the test to fail to see the desugaring in the test results thanks to Unquote
            | CheckDesugaring -> test <@ (%expr) = desugaring @>

    type TestCaseData() as this =
        inherit TheoryData<TestCase>()
        do this.Add(TestCase.OutputDesugaring)
        do this.Add(TestCase.CheckDesugaring)

    module ``Desugar a 2-element expression`` =
        module ``1_ with BindReturn and MergeSources`` =
            open ValidationBuilderMethod.BindReturn
            open ValidationBuilderMethod.MergeSources

            [<Theory; ClassData(typeof<TestCaseData>)>]
            let ``expect BindReturn(MergeSources)`` (testCase: TestCase) =
                testCase.Invoke(
                    <@
                        validation {
                            let! x = Validation.ok 1
                            and! y = Validation.ok 2
                            return x + y
                        }
                    @>,
                    desugaring =
                        validation.BindReturn(
                            validation.MergeSources( // ↩
                                Validation.ok 1,
                                Validation.ok 2
                            ),
                            (fun (x, y) -> x + y)
                        )
                )

        module ``2_ with Bind and Return`` =
            open ValidationBuilderMethod.Bind
            open ValidationBuilderMethod.Return
            open ValidationBuilderMethod.MergeSources

            [<Theory; ClassData(typeof<TestCaseData>)>]
            let ``expect Bind(MergeSources, Return)`` (testCase: TestCase) =
                testCase.Invoke(
                    <@
                        validation {
                            let! x = Validation.ok 1
                            and! y = Validation.ok 2
                            return x + y
                        }
                    @>,
                    desugaring =
                        validation.Bind(
                            validation.MergeSources( // ↩
                                Validation.ok 1,
                                Validation.ok 2
                            ),
                            (fun (x, y) -> validation.Return(x + y))
                        )
                )

        module ``3_ with Bind2 and Return`` =
            open ValidationBuilderMethod.Bind2
            open ValidationBuilderMethod.Return

            [<Theory; ClassData(typeof<TestCaseData>)>]
            let ``expect Bind2(Return)`` (testCase: TestCase) =
                testCase.Invoke(
                    <@
                        validation {
                            let! x = Validation.ok 1
                            and! y = Validation.ok 2
                            return x + y
                        }
                    @>,
                    desugaring =
                        validation.Bind2( // ↩
                            Validation.ok 1,
                            Validation.ok 2,
                            (fun (x, y) -> validation.Return(x + y))
                        )
                )

        module ``4_ with Bind2Return`` =
            open ValidationBuilderMethod.Bind2Return

            [<Theory; ClassData(typeof<TestCaseData>)>]
            let ``expect Bind(MergeSources, Return)`` (testCase: TestCase) =
                testCase.Invoke(
                    <@
                        validation {
                            let! x = Validation.ok 1
                            and! y = Validation.ok 2
                            return x + y
                        }
                    @>,
                    desugaring =
                        validation.Bind2Return( // ↩
                            Validation.ok 1,
                            Validation.ok 2,
                            (fun (x, y) -> x + y)
                        )
                )

    module ``Desugar a 3-element expression`` =
        module ``1_ with BindReturn and MergeSources`` =
            open ValidationBuilderMethod.BindReturn
            open ValidationBuilderMethod.MergeSources

            [<Theory; ClassData(typeof<TestCaseData>)>]
            let ``expect BindReturn(MergeSources(MergeSources))`` (testCase: TestCase) =
                testCase.Invoke(
                    <@
                        validation {
                            let! x = Validation.ok 1
                            and! y = Validation.ok 2
                            and! z = Validation.ok 3
                            return (z - x) * y
                        }
                    @>,
                    desugaring =
                        validation.BindReturn(
                            validation.MergeSources(
                                Validation.ok 1,
                                validation.MergeSources( // ↩
                                    Validation.ok 2,
                                    Validation.ok 3
                                )
                            ),
                            (fun (x, (y, z)) -> (z - x) * y)
                        )
                )

        module ``2_ with MergeSources3`` =
            open ValidationBuilderMethod.BindReturn
            open ValidationBuilderMethod.MergeSources // false warning "Open directive is not required by the code and can be safely removed" - MergeSources is needed in the validation {} expression below
            open ValidationBuilderMethod.MergeSources3

            [<Theory; ClassData(typeof<TestCaseData>)>]
            let ``expect BindReturn(MergeSources3)`` (testCase: TestCase) =
                testCase.Invoke(
                    <@
                        validation {
                            let! x = Validation.ok 1
                            and! y = Validation.ok 2
                            and! z = Validation.ok 3
                            return (z - x) * y
                        }
                    @>,
                    desugaring =
                        validation.BindReturn(
                            validation.MergeSources3( // ↩
                                Validation.ok 1,
                                Validation.ok 2,
                                Validation.ok 3
                            ),
                            (fun (x, y, z) -> (z - x) * y)
                        )
                )

        module ``3_ with Bind3Return`` =
            open ValidationBuilderMethod.Bind3Return

            [<Theory; ClassData(typeof<TestCaseData>)>]
            let ``expect Bind3Return`` (testCase: TestCase) =
                testCase.Invoke(
                    <@
                        validation {
                            let! x = Validation.ok 1
                            and! y = Validation.ok 2
                            and! z = Validation.ok 3
                            return (z - x) * y
                        }
                    @>,
                    desugaring =
                        validation.Bind3Return(
                            Validation.ok 1,
                            Validation.ok 2,
                            Validation.ok 3,
                            (fun (x, y, z) -> (z - x) * y)
                        )
                )

module FsToolkit =
    open FsToolkit.ErrorHandling

    [<Theory>]
    [<InlineData("✅", 11)>]
    [<InlineData("❌", 0)>]
    let ``Desugar let! ... and!`` (_, expected) =
        test
            <@
                validation {
                    let! x = Validation.ok 1
                    and! y = Validation.ok 10
                    return x + y
                } = Validation.ok expected
            @>

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