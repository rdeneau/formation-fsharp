namespace FSharpTraining

open Swensen.Unquote
open Xunit
open Xunit.Abstractions

type LoggerBuilder(logMessage: string -> unit) =
    let log value =
        logMessage $"Value: {value}"
        value

    member _.Bind(x, f) = x |> log |> f
    member _.Return(x) = x

type LoggerTests(output: ITestOutputHelper) =
    let logger = LoggerBuilder(output.WriteLine)

    [<Theory>]
    [<InlineData("✅", 85)>]
    [<InlineData("❌", 850)>]
    member _.``3 Binds`` (_, expected) =
        let _ =
            logger.Bind(
                42,
                (fun _arg1 ->
                    let x = _arg1

                    logger.Bind(
                        43,
                        (fun _arg2 ->
                            let y = _arg2

                            logger.Bind(
                                x + y,
                                (fun _arg3 ->
                                    let z = _arg3
                                    logger.Return(z))
                            ))
                    ))
            )

        test
            <@ logger {
                let! x = 42
                let! y = 43
                let! z = x + y
                return z
            } = expected @>
