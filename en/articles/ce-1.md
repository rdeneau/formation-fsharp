---
title: F# Computation Expressions 🧑‍🏫
description: F# Computation Expressions Guide, presenting in details the different kind of computation expressions
series: F# Computation Expressions
tags: #fsharp #dotnet #fp
cover_image: https://dev-to-uploads.s3.amazonaws.com/uploads/articles/3a9z5x7bw9l882x4h261.png
published: false
# published_at: 2025-04-02 06:43 +0000
---

Computation expressions—abbreviated as CE—are among the most powerful features of F#: they are not just syntactic sugar, but also an entry point for extending the F# language locally. They have been carefully designed to provide a great deal of flexibility, making them a unique and distinctive feature compared to what other programming languages offer.

This power and flexibility come at a price: the available literature on the topic is scarce. Scott Wlaschin has made a great effort to popularize the topic on F# for Fun and Profit, especially with the ["Computation Expressions" series](https://fsharpforfunandprofit.com/series/computation-expressions/). Everything is accurate and still relevant; it just lacks coverage of the latest F# features such as the `and!` keyword. While the articles are designed to be educational, this material is not optimal as a reference when writing your own computation expressions.

As for the documentation on [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/computation-expressions), it is concise and complementary to Scott Wlaschin's articles. This conciseness results in a lack of precision that makes it unsuitable for getting started with writing CEs.

This series of articles offers an alternative approach: it aims to present the concepts underlying CEs—which we call functional patterns—to F# programmers, in order to categorize the types of CEs that you might need to design. The base material comes from my F# training, available in this [GitBook](https://rdeneau.gitbook.io/fsharp-training/).

{% collapsible Table of contents %}

- [Introduction](#introduction)
  - [Syntax](#syntax)
  - [Functional patterns](#functional-patterns)
- [Builder](#builder)
  - [Builder example: `logger {}`](#builder-example-logger-)
  - [`Bind` _vs_ `let!`](#bind-vs-let)
  - [CE desugaring: tips 💡](#ce-desugaring-tips-)
  - [Constructor parameters](#constructor-parameters)
  - [Builder example: `option {}`](#builder-example-option-)
- [Conclusion](#conclusion)

{% endcollapsible %}

## Introduction

The page on [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/computation-expressions) is the ideal entry point into the subject:

> 1. Computation expressions in F# provide a convenient **syntax** for writing computations that can be sequenced and combined using control flow constructs and bindings.
> 2. Depending on the kind of computation expression, they can be thought of as a way to express monads, monoids, monad transformers, and applicatives.

### Syntax

With regard to the first point, as F# provides `async`, `task`, and `seq`  computation expressions directly in `FSharp.Core`, you can quickly be led to use them and find that, indeed, CEs are relatively easy to use—I say relatively because they are still harder to master than C#'s `async`/`await` pattern.

The syntax of a CE can be summarized in a block of code such as `myCE { body }` where `body` looks like **imperative** F# code with:

* Regular keywords: `let`, `do`, `if`/`then`/`else`, `match`, `for`...
* Dedicated keywords: `yield`, `return`
* "Banged" keywords: `let!`, `do!`, `match!`, `yield!`, `return!`

These keywords hide a ❝ **machinery** ❞ to perform background **specific** effects:

* Asynchronous computations like with `async` and `task`
* State management: e.g. a sequence with `seq`
* Absence of a value with `option`
* Error handling with `result`
* ...

### Functional patterns

"Monads, monoids, monad transformers, and applicatives" are the functional patterns mentioned just above. These patterns are only mentioned in passing: they are not explained and are not used to organize the documentation. On the F# for Fun and Profit website, Scott deliberately avoids explicitly mentioning these functional patterns: with the exception of the monoid with the ["Understanding monoids" series](https://fsharpforfunandprofit.com/series/understanding-monoids/), the other patterns are not explained from the front, but from the point of view of their operations: `map` for functors, `bind` for monads, ... - see ["Map and Bind and Apply, Oh my!" series](https://fsharpforfunandprofit.com/series/map-and-bind-and-apply-oh-my/).

The available literature about these patterns is extensive, but can be arduous to work through, especially for .NET developers with no experience of other strongly typed functional languages such as Haskell and Scala. We'll look at each of these patterns in detail, either because they're already built into the F# language, sometimes without us even realizing it, or to help us write a kind of CE related to one of these patterns.

## Builder

A _computation expression_ relies on an object called _Builder_.

⚠️ **Warning:** This is not exactly the _Builder_ object-oriented design pattern.

For each supported **keyword** (`let!`, `return`...), the _Builder_ implements one or more related **methods**. The compiler provides **flexibility** in the builder **method signatures**, as long as the methods can be **chained together** properly when the compiler evaluates the CE's body on the **caller side.** This versatility is powerful, but can lead to difficulties in designing and testing a CE.

### Builder example: `logger {}`

Need: log the intermediate values of a calculation

```fsharp
// First version
let log value = printfn $"{value}"

let loggedCalc =
    let x = 42
    log x  // ❶
    let y = 43
    log y  // ❶
    let z = x + y
    log z  // ❶
    z
```

⚠️ **Issues**

1. Verbose: the `log x` interfere with reading
2. _Error prone_: forget a `log`, log wrong value...

💡 **Solutions**

Make logs implicit in a CE by implementing a custom `let!`/`Bind()`:

```fsharp
type LoggerBuilder() =
    let log value = printfn $"{value}"; value
    member _.Bind(x, f) = x |> log |> f
    member _.Return(x) = x

let logger = LoggerBuilder()

//---

let loggedCalc = logger {
    let! x = 42     // 👈 Implicitly perform `log x`
    let! y = 43     // 👈                    `log y`
    let! z = x + y  // 👈                    `log z`
    return z
}
```

The three consecutive `let!` statements are desugared into three **nested** calls to `Bind` with:

* 1st argument: the right side of the `let!` (e.g. `42` with `let! x = 42`)
* 2nd argument: a lambda taking the variable defined at the left side of the `let!` (e.g. `x`) and returning the whole expression below the `let!` until the `}`

```fsharp
// let! x = 42
logger.Bind(42, (fun x ->
    // let! y = 43
    logger.Bind(43, (fun y ->
        // let! z = x + y
        logger.Bind(x + y, (fun z ->
            // return z
            logger.Return z)
        ))
    ))
)
```

### `Bind` _vs_ `let!`

`logger { let! var = expr in cexpr }` is desugared as:\
`logger.Bind(expr, fun var -> cexpr)`

👉 **Key points:**

* `var` and `expr` appear in reverse order
* `var` is used in the rest of the computation `cexpr`\
  → highlighted using the `in` keyword of the verbose syntax
* the lambda `fun var -> cexpr` is a **continuation** function

### CE desugaring: tips 💡

I found a simple way to desugar computation expressions:\
→ Write a failing unit test and use [Unquote](https://github.com/SwensenSoftware/unquote) - 🔗 [Example](https://github.com/rdeneau/formation-fsharp/blob/main/src/FSharpTraining/04-Monad/LoggerTests.fs#L42)

![CE desugaring with Unquote](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/r9xwcjmj9nahpr5rlkqk.png)

### Constructor parameters

The builder can be constructed with additional parameters.\
→ The CE syntax allows us to pass these arguments when using the CE:

```fsharp
type LoggerBuilder(prefix: string) =
    let log value = printfn $"{prefix}{value}"; value
    member _.Bind(x, f) = x |> log |> f
    member _.Return(x) = x

let logger prefix = LoggerBuilder(prefix)

//---

let loggedCalc = logger "[Debug] " {
    let! x = 42     // 👈 Output "[Debug] 42"
    let! y = 43     // 👈 Output "[Debug] 43"
    let! z = x + y  // 👈 Output "[Debug] 85"
    return z
}
```

### Builder example: `option {}`

Need: successively try to find values in maps using identifiers\
→ Steps:

1. Find `policyCode` by `roomRateId` in `policyCodesByRoomRate` map
2. Find `policyType` by `policyCode` in `policyTypesByCode` map
3. Build the "result" from both `policyCode` and `policyType`

#### Implementation #1: based on match expressions

```fsharp
match policyCodesByRoomRate.TryFind(roomRateId) with
| None -> None
| Some policyCode ->
    match policyTypesByCode.TryFind(policyCode) with  // ⚠️ Nesting
    | None -> None                                    // ⚠️ Duplicates line 2
    | Some policyType -> Some(buildResult policyCode policyType)
```

#### Implementation #2: based on `Option` module helpers

```fsharp
policyCodesByRoomRate.TryFind(roomRateId)
|> Option.bind (fun policyCode ->
    policyTypesByCode.TryFind(policyCode)
    |> Option.map (fun policyType -> buildResult policyCode policyType)
)
```

👉 Issues ⚠️:

* Nesting too
* Even more difficult to read because of parentheses

#### Implementation #3: based on the `option {}` CE

```fsharp
type OptionBuilder() =
    member _.Bind(x, f) = x |> Option.bind f
    member _.Return(x) = Some x

let option = OptionBuilder()

option {
    let! policyCode = policyCodesByRoomRate.TryFind(roomRateId)
    let! policyType = policyTypesByCode.TryFind(policyCode)
    return buildResult policyCode policyType
}
```

👉 Both terse and readable ✅🎉

## Conclusion

After this introduction to computation expressions and their builders, illustrated with the `logger {}` and `option {}` CEs, let's study functional patterns. We'll then look at how to write monoidal CEs, monadic CEs, and applicative CEs.
