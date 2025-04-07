// https://kata-log.rocks/mars-rover-kata

type Direction = North | West | South | East

type Position = { X: int; Y: int }

let pos (x, y) = { X = x; Y = y }

type Rover = { Position: Position ; Direction: Direction }

type Command =
    | Forward = 'f'
    | Backward = 'b'
    | Left = 'l'
    | Right = 'r'

type Commands = Command seq

type Move = Commands -> Rover -> Rover

let executeCommand rover command =
    let { X = x; Y = y } = rover.Position
    match command, rover.Direction with
    | Command.Forward, North -> { rover with Position = { X = x; Y = y + 1 } }
    | Command.Backward, North -> { rover with Position = { X = x; Y = y - 1 } }
    // | Command.Backward -> 
    // | Command.Left -> a
    // | Command.Right -> 
    | _ -> rover

let move: Move = fun commands rover ->
    commands
    |> Seq.fold executeCommand rover

// -- Tests ----

let inline (=!) expected actual =
    if expected <> actual then failwith "❌"
    printfn "✅"

let initialRover = { Position = pos (0, 0); Direction = North }

let commands (text: string) =
    text |> Seq.map (fun c ->
        match c with
        | 'f' -> Command.Forward
        | 'b' -> Command.Backward
        | 'l' -> Command.Left
        | 'r' -> Command.Right
        | _ -> invalidArg (nameof c) $"{c} is not a command")

let checkRover cmds expected =
    let actual = initialRover |> move (commands cmds)
    expected =! actual

let ``Rover should move forward`` =
    checkRover "f" { initialRover with Position = pos (0, 1) }

let ``Rover should move backward`` =
    checkRover "b" { initialRover with Position = pos (0, -1) }

let ``Rover should move backward twice`` =
    checkRover "bb" { initialRover with Position = pos (0, -2) }
