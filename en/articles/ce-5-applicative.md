---
title: F# applicative computation expressions
description: Guide to write F# computation expressions having an applicative behavior
series: F# Computation Expressions
tags: #fsharp #dotnet #fp
cover_image: https://dev-to-uploads.s3.amazonaws.com/uploads/articles/3lw7hnzaptmywhl7xta9.png
published: false
published_at: 2025-08-22 16:30 +0200
---

This fifth article in the series dedicated to F# computation expressions is a guide to writing F# computation expressions having an [applicative](https://dev.to/rdeneau/functional-patterns-for-f-computation-expressions-46c7#applicative) behavior.

{% collapsible Table of contents %}

- [Introduction](#introduction)
- [Builder method signatures](#builder-method-signatures)
- [CE Applicative example - `validation {}`](#ce-applicative-example---validation-)
- [Trap](#trap)
- [FsToolkit `validation {}`](#fstoolkit-validation-)
  - [`Source` methods](#source-methods)
- [Conclusion](#conclusion)

{% endcollapsible %}

## Introduction

An applicative CE is revealed through the usage of the `and!` keyword _(F# 5)._

## Builder method signatures

An applicative CE builder should define these methods:

```fsharp
// Method        | Signature                        | Equivalence
    MergeSources : mx: M<X> * my: M<Y> -> M<X * Y>  ; map2 (fun x y -> x, y) mx my
    BindReturn   : m: M<T> * f: (T -> U) -> M<U>    ; map f m
```

## CE Applicative example - `validation {}`

```fsharp
type Validation<'t, 'e> = Result<'t, 'e list>

type ValidationBuilder() =
    member _.BindReturn(x: Validation<'t, 'e>, f: 't -> 'u) =
        Result.map f x

    member _.MergeSources(x: Validation<'t, 'e>, y: Validation<'u, 'e>) =
        match (x, y) with
        | Ok v1,    Ok v2    -> Ok(v1, v2)     // Merge both values in a pair
        | Error e1, Error e2 -> Error(e1 @ e2) // Merge errors in a single list
        | Error e, _ | _, Error e -> Error e   // Short-circuit single error source

let validation = ValidationBuilder()
```

**Usage:** validate a customer

* Name not null or empty
* Height strictly positive

```fsharp
type [<Measure>] cm
type Customer = { Name: string; Height: int<cm> }

let validateHeight height =
    if height <= 0<cm>
    then Error ["Height must be positive"]
    else Ok height

let validateName name =
    if System.String.IsNullOrWhiteSpace name
    then Error ["Name can't be empty"]
    else Ok name

module Customer =
    let tryCreate name height : Result<Customer, string list> =
        validation {
            let! validName = validateName name
            and! validHeight = validateHeight height
            return { Name = validName; Height = validHeight }
        }

let c1 = Customer.tryCreate "Bob" 180<cm>  // Ok { Name = "Bob"; Height = 180 }
let c2 = Customer.tryCreate "Bob" 0<cm> // Error ["Height must be positive"]
let c3 = Customer.tryCreate "" 0<cm>    // Error ["Name can't be empty"; "Height must be positive"]
```

Desugaring:

```fsharp
validation {                                ; validation.BindReturn(
                                            ;     validation.MergeSources(
    let! name = validateName "Bob"          ;         validateName "Bob",
    and! height = validateHeight 0<cm>      ;         validateHeight 0<cm>
                                            ;     ),
    return { Name = name; Height = height } ;     (fun (name, height) -> { Name = name; Height = height })
}                                           ; )
```

## Trap

⚠️ The compiler accepts that we define `ValidationBuilder` without `BindReturn` but with `Bind` and `Return`. But in this case, we can lose the applicative behavior and it enables monadic CE bodies!

## FsToolkit `validation {}`

[FsToolkit.ErrorHandling](https://github.com/demystifyfp/FsToolkit.ErrorHandling/) offers a similar `validation {}`.

The desugaring reveals the definition of more methods: `Delay`, `Run`, `Source`📍

```fsharp
validation {                                ;  validation.Run(
    let! name = validateName "Bob"          ;      validation.Delay(fun () ->
    and! height = validateHeight 0<cm>      ;          validation.BindReturn(
    return { Name = name; Height = height } ;              validation.MergeSources(
}                                           ;                  validation.Source(validateName "Bob"),
                                            ;                  validation.Source(validateHeight 0<cm>)
                                            ;              ),
                                            ;              (fun (name, height) -> { Name = name; Height = height })
                                            ;          )
                                            ;      )
                                            ;  )
```

### `Source` methods

In FsToolkit `validation {}`, there are a couple of `Source` methods defined:

* The main definition is the `id` function.
* Another overload is interesting: it converts a `Result<'a, 'e>` into a `Validation<'a, 'e>`. As it's defined as an extension method, it has a lower priority for the compiler, leading to a better type inference. Otherwise, we would need to add type annotations.

☝️ **Note:** `Source` documentation is scarce. The most valuable information comes from a [question on Stack Overflow](https://stackoverflow.com/a/35301315/8634147) mentioned in FsToolkit source code!

## Conclusion

Applicative computation expressions in F# enable parallel computation and error accumulation through the `and!` syntax introduced in F# 5. By implementing `MergeSources` and `BindReturn` methods, you can create powerful validation workflows that collect all errors rather than stopping at the first failure, as well as performant computations that leverage parallelism. This approach is particularly valuable for form validation, configuration parsing, and any scenario where you want to provide comprehensive feedback to users about multiple validation failures simultaneously. While applicative CEs are less versatile than monadic ones, they excel in specific use cases where their distinct capabilities make a significant difference.
