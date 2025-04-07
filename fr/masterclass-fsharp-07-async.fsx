#r "nuget: FSToolkit.ErrorHandling"
open FsToolkit.ErrorHandling

type User = { Name: string; Password: string }

type AuthError = AuthError
type AuthToken = AuthToken
type TokenError = TokenError

type LoginError = 
    | InvalidUser
    | InvalidPassword
    | Unauthorized of AuthError
    | TokenErr of TokenError

let tryGetUser username = async {
    return
        if System.String.IsNullOrWhiteSpace username
        then None
        else Some { Name = username; Password = "123" }
}

let isPasswordValid password user =
    password = user.Password

let authorize user : Async<Result<unit, AuthError>> = async {
    return
        if user.Name = "Guest"
        then Error AuthError
        else Ok ()
}

let createAuthToken user : Result<AuthToken, TokenError> =
    Ok AuthToken

let login username password =
    asyncResult {
        let! user = username |> tryGetUser |> AsyncResult.requireSome InvalidUser

        // Result.requireTrue: (error: 'err) -> (value: bool) -> Result<unit, 'err>
        // ☝ Conversion de type implicite : Result → Async<Result> - cf. `member _.Source(result : Result<_,_>) : Async<Result<_,_>>`
        //   https://github.com/demystifyfp/FsToolkit.ErrorHandling/blob/e2379b52aab5c1d7e6ea23a3832fbea0586b38f8/src/FsToolkit.ErrorHandling/AsyncResultCE.fs#L109
        do! user |> isPasswordValid password |> Result.requireTrue InvalidPassword

        // AsyncResult.mapError: (f: 'e1 -> 'e2) -> Async<Result<'t, 'e1>> -> Async<Result<'t, 'e2>>
        do! user |> authorize |> AsyncResult.mapError Unauthorized

        return! user |> createAuthToken |> Result.mapError TokenErr
    } // Async<Result<AuthToken, LoginError>>

let u1 = login "     " "---" |> Async.RunSynchronously // Error InvalidUser
let u2 = login "Guest" "---" |> Async.RunSynchronously // Error InvalidPassword
let u3 = login "Guest" "123" |> Async.RunSynchronously // Error (Unauthorized AuthError)
let u4 = login "Johny" "123" |> Async.RunSynchronously // Ok AuthToken
