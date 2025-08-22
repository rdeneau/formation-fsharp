---
title: F# monadic computation expressions
description: Guide to write F# computation expressions having a monadic behaviour
series: F# Computation Expressions
tags: #fsharp #dotnet #fp
cover_image: https://dev-to-uploads.s3.amazonaws.com/uploads/articles/fl7xsqe6uf2t62949bcb.png
published: false
published_at: 2025-08-22 15:50 +0200
---

This fourth article in the series dedicated to F# computation expressions is a guide to writing F# computation expressions having a [monadic](https://dev.to/rdeneau/functional-patterns-for-f-computation-expressions-46c7#monad) behavior.

{% collapsible Table of contents %}

- [Introduction](#introduction)
- [Builder method signatures](#builder-method-signatures)
  - [Monadic _vs_ Monoidal](#monadic-vs-monoidal)
  - [CE monadic and delayed](#ce-monadic-and-delayed)
- [CE monadic examples](#ce-monadic-examples)
  - [CE monadic example - `result {}`](#ce-monadic-example---result-)
  - [CE monadic: FSharpPlus `monad {}`](#ce-monadic-fsharpplus-monad-)
- [Monad stack, monad transformers](#monad-stack-monad-transformers)
  - [1. Academic style (with FSharpPlus)](#1-academic-style-with-fsharpplus)
  - [2. Idiomatic style](#2-idiomatic-style)
- [Conclusion](#conclusion)

{% endcollapsible %}

## Introduction

A monadic CE can be identified by the usage of `let!` and `return` keywords, revealing the monadic `bind` and `return` operations.

## Builder method signatures

Behind the scenes, builders of these CEs should/can implement these methods:

```fsharp
// Method     | Signature                                     | CE syntax supported
    Bind      : M<T> * (T -> M<U>) -> M<U>                    ; let! x = xs in ...
                (* when T = unit *)                           ; do! command
    Return    : T -> M<T>                                     ; return x
    ReturnFrom: M<T> -> M<T>                                  ; return!

// Additional methods
    Zero      : unit -> M<T>                                  ; if // without `else` // Typically `unit -> M<unit>`
    Combine   : M<unit> * M<T> -> M<T>                        ; e1; e2  // e.g. one loop followed by another one
    TryWith   : M<T> -> (exn -> M<T>) -> M<T>                 ; try/with
    TryFinally: M<T> * (unit -> M<unit>) -> M<T>              ; try/finally
    While     : (unit -> bool) * (unit -> M<unit>) -> M<unit> ; while cond do command ()
    For       : seq<T> * (T -> M<unit>) -> M<unit>            ; for i in xs do command i ; for i = 0 to n do command i
    Using     : T * (T -> M<U>) -> M<U> when T :> IDisposable ; use! x = xs in ...
```

### Monadic _vs_ Monoidal

#### `Return` (monadic) _vs_ `Yield` (monoidal)

* Same signature: `T -> M<T>`
* A series of `return` is not expected\
  → Monadic `Combine` takes only a monadic command `M<unit>` as 1st param
* CE enforces appropriate syntax by implementing one of these methods:
  * `seq {}` allows `yield` but not `return`
  * `async {}`: the reverse

#### `For` and `While`

<table><thead><tr><th width="99">Method</th><th width="147">CE</th><th>Signature</th></tr></thead><tbody><tr><td><code>For</code></td><td>Monoidal</td><td><code>seq&#x3C;T> * (T -> M&#x3C;U>) -> M&#x3C;U> or seq&#x3C;M&#x3C;U>></code></td></tr><tr><td></td><td>Monadic</td><td><code>seq&#x3C;T> * (T -> M&#x3C;unit>) -> M&#x3C;unit></code></td></tr><tr><td><code>While</code></td><td>Monoidal</td><td><code>(unit -> bool) * Delayed&#x3C;T> -> M&#x3C;T></code></td></tr><tr><td></td><td>Monadic</td><td><code>(unit -> bool) * (unit -> M&#x3C;unit>) -> M&#x3C;unit></code></td></tr></tbody></table>

👉 Different use cases:

* Monoidal: Comprehension syntax
* Monadic: Series of effectful commands

### CE monadic and delayed

Like monoidal CE, monadic CE can use a `Delayed<'t>` type.\
→ Impacts on the method signatures:

```fsharp
 Delay      : thunk: (unit -> M<T>) -> Delayed<T>
 Run        : Delayed<T> -> M<T>
 Combine    : M<unit> * Delayed<T> -> M<T>
 While      : predicate: (unit -> bool) * Delayed<unit> -> M<unit>
 TryFinally : Delayed<T> * finalizer: (unit -> unit) -> M<T>
 TryWith    : Delayed<T> * handler: (exn -> unit) -> M<T>
```

## CE monadic examples

☝️ The initial CEs studied—`logger {}` and `option {}`—were monadic.

### CE monadic example - `result {}`

Let's build a `result {}` CE to play with dice!

```fsharp
type ResultBuilder() =
    member _.Bind(rx, f) = rx |> Result.bind f
    member _.Return(x) = Ok x
    member _.ReturnFrom(rx) = rx

let result = ResultBuilder()

// ---

let rollDice =
    let random = Random(Guid.NewGuid().GetHashCode())
    fun () -> random.Next(1, 7)

let tryGetDice dice =
    result {
        if rollDice() <> dice then
            return! Error $"Not the expected dice {dice}."
    }

let tryGetAPairOf6 =
    result {
        let n = 6
        do! tryGetDice n
        do! tryGetDice n
        return true
    }
```

Desugaring:

```fsharp
let tryGetAPairOf6 =
    result {                ;
        let n = 6           ;   let n = 6
        do! tryGetDice n    ;   result.Bind(tryGetDice n, (fun () ->
        do! tryGetDice n    ;        result.Bind(tryGetDice n, (fun () ->
        return true         ;            result.Return(true)
    }                       ;        ))
                            ;   ))
```

### CE monadic: FSharpPlus `monad {}`

[FSharpPlus](http://fsprojects.github.io/FSharpPlus/computation-expressions.html) provides a `monad` CE

* Works for all monadic types: `Option`, `Result`, ... and even `Lazy` 🎉
* Supports monad stacks with monad transformers 📍

⚠️ **Limits:**

* Confusing: the `monad` CE has 4 flavours to cover all cases: delayed or strict, embedded side-effects or not
* Based on SRTP: can be very long to compile!
* Documentation not exhaustive, relying on Haskell knowledges
* Very Haskell-oriented: not idiomatic F#

## Monad stack, monad transformers

A monad stack is a composition of different monads.\
→ Example: `Async`+`Option`.

We can handle it with 2 styles: academic or F# idiomatic.

### 1. Academic style (with FSharpPlus)

Monad transformer (here `MaybeT`)\
→ Extends `Async` to handle both effects\
→ Resulting type: `MaybeT<Async<'t>>`

✅ Reusable with other inner monads \
❌ Less easy to evaluate the resulting value \
❌ Not idiomatic

### 2. Idiomatic style

Custom CE `asyncOption`, based on the `async` CE, handling the `Async<Option<'t>>` type

```fsharp
type AsyncOption<'T> = Async<Option<'T>> // Convenient alias, not required

type AsyncOptionBuilder() =
    member _.Bind(aoX: AsyncOption<'a>, f: 'a -> AsyncOption<'b>) : AsyncOption<'b> =
        async {
            match! aoX with
            | Some x -> return! f x
            | None -> return None
        }

    member _.Return(x: 'a) : AsyncOption<'a> =
        async { return Some x }
```

⚠️ Limits: Not reusable, just copiable for `asyncResult`, for instance

## Conclusion

Monadic computation expressions in F# provide a familiar syntax—based on `let!` and `return` keywords—for sequencing effectful computations. While you can build custom monadic CEs for specific domain needs or leverage libraries like FSharpPlus for academic-style programming, the most practical approach is often creating idiomatic F# builders tailored to your specific monad combinations, such as `Async<Option<'t>>` or `Async<Result<'t, 'e>>`. This strikes the right balance between expressiveness and maintainability in typical F# codebases.
