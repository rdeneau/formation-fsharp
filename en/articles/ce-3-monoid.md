---
title: F# monoidal computation expressions
description: Guide to write F# computation expressions having a monoidal behaviour
series: F# Computation Expressions
tags: #fsharp #dotnet #fp
cover_image: https://dev-to-uploads.s3.amazonaws.com/uploads/articles/thogfvejlomlswwesm3n.png
published: true
published_at: 2025-08-22 15:50 +0200
---

This third article in the series dedicated to F# computation expressions is a guide to writing F# computation expressions having a [monoidal](https://dev.to/rdeneau/functional-patterns-for-f-computation-expressions-46c7#monoid) behavior.

{% collapsible Table of contents %}

- [Introduction](#introduction)
- [Builder method signatures](#builder-method-signatures)
- [CE monoidal _vs_ comprehension](#ce-monoidal-vs-comprehension)
  - [Comprehension definition](#comprehension-definition)
  - [Comparison](#comparison)
  - [Minimal set of methods expected for each](#minimal-set-of-methods-expected-for-each)
- [CE monoidal example: `multiplication {}`](#ce-monoidal-example-multiplication-)
  - [Exercise](#exercise)
  - [Desugaring](#desugaring)
- [`Delayed<T>` type](#delayedt-type)
- [CE monoidal kinds](#ce-monoidal-kinds)
  - [CE monoidal to generate a collection](#ce-monoidal-to-generate-a-collection)
- [Conclusion](#conclusion)

{% endcollapsible %}

## Introduction

A monoidal CE can be identified by the usage of `yield` and `yield!` keywords.

**Relationship with the monoid:**
→ Hidden in the builder methods:

* `+` operation → `Combine` method
* `e` neutral element → `Zero` method

## Builder method signatures

Like we did for functional patterns, we use the generic type notation:

* `M<T>`: type returned by the CE
* `Delayed<T>`: presented later 📍

```fsharp
// Method     | Signature                           | CE syntax supported
    Yield     : T -> M<T>                           ; yield x
    YieldFrom : M<T> -> M<T>                        ; yield! xs
    Zero      : unit -> M<T>                        ; if // without `else`  // Monoid neutral element
    Combine   : M<T> * Delayed<T> -> M<T>                                   // Monoid + operation
    Delay     : (unit -> M<T>) -> Delayed<T>        ; // always required with Combine

// Other additional methods
    Run       : Delayed<T> -> M<T>
    For       : seq<T> * (T -> M<U>) -> M<U>        ; for i in seq do yield ... ; for i = 0 to n do yield ...
                              (* or *)  seq<M<U>>
    While     : (unit -> bool) * Delayed<T> -> M<T> ; while cond do yield...
    TryWith   : M<T> -> (exn -> M<T>) -> M<T>       ; try/with
    TryFinally: Delayed<T> * (unit -> unit) -> M<T> ; try/finally
```

## CE monoidal _vs_ comprehension

### Comprehension definition

> It is the concise and declarative syntax to build collections with control flow keywords `if`, `for`, `while`... and ranges `start..end`.

### Comparison

* Similar syntax from caller perspective
* Distinct overlapping concepts

### Minimal set of methods expected for each

* Monoidal CE: `Yield`, `Combine`, `Zero`
* Comprehension: `For`, `Yield`

## CE monoidal example: `multiplication {}`

Let's build a CE that multiplies the integers yielded in the computation body:
→ CE type: `M<T> = int` • Monoid operation = `*` • Neutral element = `1`

```fsharp
type MultiplicationBuilder() =
    member _.Zero() = 1
    member _.Yield(x) = x
    member _.Combine(x, y) = x * y
    member _.Delay(f) = f () // eager evaluation

    member m.For(xs, f) =
        (m.Zero(), xs)
        ||> Seq.fold (fun res x -> m.Combine(res, f x))

let multiplication = MultiplicationBuilder()

let shouldBe10 = multiplication { yield 5; yield 2 }
let factorialOf5 = multiplication { for i in 2..5 -> i } // 2 * 3 * 4 * 5
```

### Exercise

* Copy this snippet in vs code
* Comment out builder methods
  * To start with an empty builder, add this line `let _ = ()` in the body.
  * After adding the first method, this line can be removed.
* Let the compiler errors in `shouldBe10` and `factorialOf5` guide you to add the relevant methods.

### Desugaring

Desugared `multiplication { yield 5; yield 2 }`:

```fsharp
// Original
let shouldBe10 =
    multiplication.Delay(fun () ->
        multiplication.Combine(
            multiplication.Yield(5),
            multiplication.Delay(fun () ->
                multiplication.Yield(2)
            )
        )
    )

// Simplified (without Delay)
let shouldBe10 =
    multiplication.Combine(
        multiplication.Yield(5),
        multiplication.Yield(2)
    )
```

Desugared `multiplication { for i in 2..5 -> i }`:

```fsharp
// Original
let factorialOf5 =
    multiplication.Delay (fun () ->
        multiplication.For({2..5}, (fun _arg2 ->
            let i = _arg2 in multiplication.Yield(i))
        )
    )

// Simplified
let factorialOf5 =
    multiplication.For({2..5}, (fun i -> multiplication.Yield(i)))
```

## `Delayed<T>` type

`Delayed<T>` represents a delayed computation and is used in these methods:

* `Delay` returns this type, hence defines it for the CE
* `Combine`, `Run`, `While` and `TryFinally` used it as input parameter

```fsharp
 Delay      : thunk: (unit -> M<T>) -> Delayed<T>
 Combine    : M<T> * Delayed<T> -> M<T>
 Run        : Delayed<T> -> M<T>
 While      : predicate: (unit -> bool) * Delayed<T> -> M<T>
 TryFinally : Delayed<T> * finalizer: (unit -> unit) -> M<T>
```

* `Delay` is required by the presence of `Combine`.
* `Delay` is called each time converting from `M<T>` to `Delayed<T>` is needed.
* `Delayed<T>` is internal to the CE.
  * `Run` is required at the end to get back the `M<T>`...
  * ... **only** when `Delayed<T>` ≠ `M<T>`, otherwise it can be omitted.

👉 Enables to implement **laziness and short-circuiting** at the CE level.

**Example:** lazy `multiplication {}` with `Combine` optimized when `x = 0`

```fsharp
type MultiplicationBuilder() =
    member _.Zero() = 1
    member _.Yield(x) = x
    member _.Delay(thunk: unit -> int) = thunk // Lazy evaluation
    member _.Run(delayedX: unit -> int) = delayedX () // Required to get a final `int`

    member _.Combine(x: int, delayedY: unit -> int) : int =
        match x with
        | 0 -> 0 // 👈 Short-circuit for multiplication by zero
        | _ -> x * delayedY ()

    member m.For(xs, f) =
        (m.Zero(), xs) ||> Seq.fold (fun res x -> m.Combine(res, m.Delay(fun () -> f x)))
```

| Difference              | Eager   | Lazy                           |
| ----------------------- | ------- | ------------------------------ |
| `Delay` return type     | `int`   | `unit -> int`                  |
| `Run`                   | Omitted | Required to get back an `int`  |
| `Combine` 2nd parameter | `int`   | `unit -> int`                  |
| `For` calling `Delay`   | Omitted | Explicit but not required here |

![Differences between the eager and the lazy implementation of the multiplication CE](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/39dx3tm50mxhft9r9y8q.png)

## CE monoidal kinds

With `multiplication {}`, we've seen a first kind of monoidal CE:
→ To reduce multiple yielded values into 1.

There is a second kind of monoidal CE:
→ To aggregate multiple yielded values into a collection.
→ Example: `seq {}` returns a `'t seq`.

### CE monoidal to generate a collection

Let's build a `list {}` monoidal CE!

```fsharp
type ListBuilder() =
    member _.Zero() = [] // List.empty
    member _.Yield(x) = [x] // List.singleton
    member _.YieldFrom(xs) = xs
    member _.Delay(thunk: unit -> 't list) = thunk () // eager evaluation
    member _.Combine(xs, ys) = xs @ ys // List.append
    member _.For(xs, f: _ seq) = xs |> Seq.collect f |> Seq.toList

let list = ListBuilder()
```

> 💡 Notes:
>
> * `M<T>` is `'t list` → type returned by `Yield` and `Zero`
> * `For` uses an intermediary sequence to collect the values returned by `f`.

Let's test the CE to generate the list `[begin; 16; 9; 4; 1; 2; 4; 6; 8; end]`
_(Desugared code simplified)_

![list {} CE desugared](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/t963gf9k2qzv6cno6zey.png)

Comparison with the same expression in a list comprehension:

![List comprehension desugared](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/o9gksnrzmac3nciwhg6y.png)

`list { expr }` _vs_ `[ expr ]`:

* `[ expr ]` uses a hidden `seq` all through the computation and ends with a `toList`
* All methods are inlined:

| Method    | `list { expr }`              | `[ expr ]`      |
| --------- | ---------------------------- | --------------- |
| `Combine` | `xs @ ys => List.append`     | `Seq.append`    |
| `Yield`   | `[x] => List.singleton`      | `Seq.singleton` |
| `Zero`    | `[] => List.empty`           | `Seq.empty`     |
| `For`     | `Seq.collect` & `Seq.toList` | `Seq.collect`   |

## Conclusion

Monoidal computation expressions provide an elegant and powerful syntax for combining and aggregating values in F#. By implementing a builder with just a few key methods—`Combine` and `Zero` which correspond to the monoid's `+` operation and `e` neutral element, alongside `Yield`, `YieldFrom`, and `For` methods to support comprehension syntax—you can either reduce values to a single result (similar to the _Composite_ design pattern) or build collections with natural, imperative-like code. Additionally, leveraging the `Delayed<T>` type enables optimization opportunities for both behavior and performance within your computation expressions.
