---
marp: true
html: true
theme: 'd-edge'
title: 'F♯ Training • Computation Expressions'
footer: 'F♯ Training • Computation Expressions'
paginate: true
---

<!-- _class: title invert -->

# F♯ Training [🖇️](fsharp-training-00-toc.html#2 "Root table of contents")

## Computation Expressions (CE)

### 2025 July

---

<!-- _class: agenda invert lead -->

![bg right:30% h:300](../themes/d-edge/pictos/SOAT_pictos_formation.png)

## Table of contents

- Intro

---

<!-- _class: chapter invert -->

![bg-right h:300](../themes/d-edge/pictos/SOAT_pictos_lieu.png)

# 1.

## Intro

---

# Presentation

1. Computation expressions in F# provide a convenient **syntax** for writing computations that can be sequenced and combined using control flow constructs and bindings.
2. Depending on the kind of computation expression, they can be thought of as a way to express monads, monoids, monad transformers, and applicatives \
   → **Functional patterns** seen previously, *except monad transformers* 📍

🔗 [Learn F# - Computation Expressions](https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/computation-expressions), by Microsoft

Built-in CEs: `async` and `task`, `seq`, `query` \
→ Easy to use, once we know the syntax and its keywords

We can write our own CE too \
→ More challenging!

---

# Syntax

CE = block like `myCE { body }` where `body` looks like **imperative** F# code with:

- regular keywords: `let`, `do`, `if`/`then`/`else`, `match`, `for`...
- dedicated keywords: `yield`, `return`
- "banged" keywords: `let!`, `do!`, `match!`, `yield!`, `return!`

These keywords hide a ❝ **machinery** ❞ to perform background **specific** effects:

- Asynchronous computations like with `async` and `task`
- State management: e.g. a sequence with `seq`
- Absence of value with `option` CE
- ...

---

<!-- _class: chapter invert -->

![bg-right h:300](../themes/d-edge/pictos/SOAT_pictos_atelier.png)

# 2.

## Builder

---

# Builder

A *computation expression* relies on an object called *Builder*.

⚠️ This is not exactly the *Builder* OO design pattern.

For each supported **keyword** (`let!`, `return`...), the *Builder* implements one or more related **methods**.

The 2 fundamental methods to know when writing our own CE:

- `builder.Return(expr)` used to handle the `return` keyword
- `builder.Bind(expr, f)` used for `let!` keyword

💡 Looks familiar, no? Hello, monads!

---

<!-- _footer: '' -->

# Builder example: `logger`

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

**Issues** ⚠️
① *Verbose*: the `log x` interfere with reading
② *Error prone*: easy to forget to log a value,
or to log the wrong variable after a bad copy-paste-update...

---

# Builder example: `logger` (2)

💡 V2: make logs implicit in a CE by implementing a custom `let!` / `Bind()` :

```fsharp
type LoggingBuilder() =
    let log value = printfn $"{value}"; value
    member _.Bind(x, f) = x |> log |> f
    member _.Return(x) = x

let logger = LoggingBuilder()

//---

let loggedCalc = logger {
    let! x = 42     // 👈 Implicitly perform `log x`
    let! y = 43     // 👈                    `log y`
    let! z = x + y  // 👈                    `log z`
    return z
}
```

---

# Builder example: `logger` - Desugaring

The 3 consecutive `let!` is translated into 3 **nested** `Bind`:

```fsharp
// let! x = 42
logger.Bind(42, (fun _arg1 ->
    let x = _arg1

    // let! y = 43
    logger.Bind(43, (fun _arg2 ->
        let y = _arg2

        // let! z = x + y
        logger.Bind(x + y, (fun _arg3 ->
            let z = _arg3
            logger.Return(z))
        ))
    ))
)
```

---

# CE desugaring: tips 💡

I found a simple way to desugar a computation expression: \
→ Write a failing unit test and use [Unquote](https://github.com/SwensenSoftware/unquote) - 🔗 [Example](https://github.com/rdeneau/formation-fsharp/blob/main/src/FSharpTraining/04-Monad/LoggerTests.fs#L42)

![](../themes/d-edge/img/desugar-ce-with-unquote.png)

---

# Builder example: `option`

Need: successively find in maps by identifiers

1. `roomRateId` → `policyCode`
2. `policyCode` → `policyType`
3. `policyCode` and `policyType` → `result`

```fsharp
// 1: with match expressions → nesting!
match policyCodesByRoomRate.TryFind(roomRateId) with
| None -> None
| Some policyCode ->
    match policyTypesByCode.TryFind(policyCode) with
    | None -> None
    | Some policyType -> Some(buildResult policyCode policyType)
```

---

# Builder example: `option` (2)

```fsharp
// 2: with Option module helpers → terser but harder to read
policyCodesByRoomRate.TryFind(roomRateId)
|> Option.bind (fun policyCode -> policyCode, policyTypesByCode.TryFind(policyCode))
|> Option.map (fun (policyCode, policyType) -> buildResult policyCode policyType)
```

---

# Builder example: `option` (3)

```fsharp
// 3: with an option CE → both terse and readable 🎉

type OptionBuilder() =
    member _.Bind(x, f) = x |> Option.bind f
    member _.Return(x) = Some x

let option = OptionBuilder()

// ---

option {
    let! policyCode = policyCodesByRoomRate.TryFind(roomRateId)
    let! policyType = policyTypesByCode.TryFind(policyCode)
    return buildResult policyCode policyType
}
```

---

<!-- _class: chapter invert -->

![bg-right h:300](../themes/d-edge/pictos/SOAT_pictos_architecture.png)

# 3.

## CE monoidal

---

# CE monoidal

A monoidal CE *(such as `seq`)* is revealed through the usage of keywords like `yield` (even implicit), `yield!`, and sometimes `for`.

Behind the scene, builders of these CE should implement:

- `Yield` to build the collection element by element
- `YieldFrom` to support `yield!`
- `Combine` ≡ `(+)` (`Seq.append`)
- `Zero` ≡ neutral element (`Seq.empty`)
- `For` to support `for x in xs do ...`

---

<!-- _class: chapter invert -->

![bg-right h:300](../themes/d-edge/pictos/SOAT_pictos_delivery2.png)

# 4.

## CE monadic

---

# CE monadic

The builder of a monadic CE has `Return` and `Bind` methods.

The `Option` and `Result` types are monadic.

- We can use `option` and `result` CEs from [FsToolkit](https://github.com/demystifyfp/FsToolkit.ErrorHandling/) 
- or create their own minimalistic CE:

```fsharp
type ResultBuilder() =
    member _.Bind(x, f) = x |> Result.bind f
    member _.Return(x) = Some x

let result = ResultBuilder()
```

---

# CE monadic (2)

```fsharp
type OptionBuilder() =
    member _.Bind(x, f) = x |> Option.bind f
    member _.Return(x) = Some x

let option = OptionBuilder()

// ----

let addOptionalInt x' y' = option {
    let! x = x'
    let! y = y'
    return x + y
}

let v1 = addOptionalInt (Some 1) (Some 2) // = Some 3
let v2 = addOptionalInt (Some 1) None     // = None
```

---

# CE monadic: FSharpPlus `monad` CE

[FSharpPlus](http://fsprojects.github.io/FSharpPlus//computation-expressions.html) provides a `monad` CE

- Works for all monadic types: `Option`, `Result`, ... and even `Lazy` 🎉
- Supports monad stacks with monad transformers

⚠️ **Limits:**

- Confusing: the `monad` CE has 4 flavours to cover all cases: delayed or strict, embedded side-effects or not
- Based on SRTP: can be very long to compile!
- Very Haskell-oriented: learning curve + not idiomatic

---

# Monad stack, monad transformers

A monad stack is a composition of different monads. \
→ Example: `Async`+`Option`.

How to handle it? \
→ Academic style *vs* idiomatic F♯

#### 1. Academic style (with FSharpPlus)

Monad transformer (here `MaybeT`) \
→ Extends `Async` to handle both effects \
→ Resulting type: `MaybeT<Async<'t>>`

✅ reusable with other inner monad
❌ less easy to evaluate the resulting value
❌ not idiomatic

---

# Monad stack, monad transformers (2)

#### 2. Idiomatic style

Custom CE `asyncOption`, based on the `async` CE, handling `Async<Option<'t>>` type

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

⚠️ Limits: not reusable, just copiable for `asyncResult` for instance

---

<!-- _class: chapter invert -->

![bg-right h:300](../themes/d-edge/pictos/SOAT_pictos_delivery.png)

# 4.

## CE Applicative

---

# CE Applicative

Since F♯ 5.0, the applicative behaviour of a CE is revealed through the usage of the `and!` keyword.

The and! Keyword ( and later)

An applicative CE builder is special compared to monoidal and monadic CE builders:

- Not following the definition of the applicative: there is no method associated with the `apply` operation.
- Based on `mapN` operations like `map2`

Contrairement aux  CE monoidal et monadique, 

The builder of an applicative CE has `Return` and `Bind` methods.


---

# CE Applicative

[FsToolkit.ErrorHandling](https://github.com/demystifyfp/FsToolkit.ErrorHandling/) offers:

- Type `Validation<'Ok, 'Err>` ≡ `Result<'Ok, 'Err list>`
- CE `validation {}` supporting `let!...and!...` syntax.

Allows errors to be accumulated in use cases like:

- Parsing external inputs
- *Smart constructor* *(example on the next slide...)*

---

<!-- _footer: '' -->

# CE Applicative: example

```fsharp
#r "nuget: FSToolkit.ErrorHandling"
open FsToolkit.ErrorHandling

type [<Measure>] cm
type Customer = { Name: string; Height: int<cm> }

let validateHeight height =
    if height <= 0<cm>
    then Error "Height must me positive"
    else Ok height

let validateName name =
    if System.String.IsNullOrWhiteSpace name
    then Error "Name can't be empty"
    else Ok name

module Customer =
    let tryCreate name height : Result<Customer, string list> =
        validation {
            let! validName = validateName name
            and! validHeight = validateHeight height
            return { Name = validName; Height = validHeight }
        }

let c1 = Customer.tryCreate "Bob" 180<cm>  // Ok { Name = "Bob"; Height = 180 }
let c2 = Customer.tryCreate "Bob" 0<cm> // Error ["Height must me positive"]
let c3 = Customer.tryCreate "" 0<cm>    // Error ["Name can't be empty"; "Height must me positive"]
```

---

# Other CE

We've seen 2 libraries that extend F♯ and offer their CEs:

- FSharpPlus → `monad`
- FsToolkit.ErrorHandling → `option`, `result`, `validation`

Many libraries have their own DSL *(Domain Specific Language)*.
Some are based on computation expression(s):

- [Expecto](https://github.com/haf/expecto): Testing library (`test "..." {...}`)
- [Farmer](https://github.com/compositionalit/farmer): Infra as code for Azure (`storageAccount {...}`)
- [Saturn](https://saturnframework.org/): Web framework on top of ASP.NET Core (`application {...}`)

---

# Writing our own CE

- Choose the main **behaviour**: monoidal? monadic? applicative?
  - Prefer a single behaviour unless it's a generic/multi-purpose CE
- Create a **builder** class
- Implement the main **methods** to get the selected behaviour
- Implement [additional methods](https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/computation-expressions#creating-a-new-type-of-computation-expression) like `Delay` and `Run`
  - If needed, to fine tune the behaviour or the performances,
  - or by the compiler!
- There is a flexibility in the signature of the methods.
  - It can be tricky to get them right!
  - 🔗 [Computation Expressions Workshop: 2 - Choice Builder | GitHub](https://github.com/panesofglass/computation-expressions-workshop/blob/master/exercises/02_ChoiceBuilder.pdf)

---

# Writing our own CE - Tips 💡

- Overload methods to support more use cases like different input types
  - `Async<Return<_,_>>` + `Async<_>` + `Result<_,_>`
  - `Option<_>` and `Nullable<_>`
- Get inspired by the existing codebases that provide CEs \
  → e.g. Tips found in [FsToolkit/OptionCE.fs](https://github.com/demystifyfp/FsToolkit.ErrorHandling/blob/master/src/FsToolkit.ErrorHandling/OptionCE.fs):
  - Undocumented `Source` methods
  - Force the method overload order with extension methods \
    → to get a better code completion assistance.

🔗 [Computation Expressions Workshop: 6 - Extensions | GitHub](https://github.com/panesofglass/computation-expressions-workshop/blob/master/exercises/06_Extensions.pdf)

---

# Writing our own CE - Custom operations 🚀

What: builder methods annotated with `[<CustomOperation("myOperation")>]`

Use cases: add new keywords, build a custom DSL
→ Example: the `query` core CE supports `where` and `select` keywords like LINQ

⚠️ **Warning:** you may need additional things that are not well documented:

- Additional properties for the `CustomOperation` attribute:
  - `AllowIntoPattern`, `MaintainsVariableSpace`
  - `IsLikeJoin`, `IsLikeGroupJoin`, `JoinConditionWord`
  - `IsLikeZip`...
- Additional attributes on the method parameters, like `[<ProjectionParameter>]`

🔗 [Computation Expressions Workshop: 7 - Query Expressions | GitHub](https://github.com/panesofglass/computation-expressions-workshop/blob/master/exercises/07_Queries.pdf)

---

# CE benefits ✅

- **Increased Readability**: imperative-like code
- **Reduced Boilerplate**: hides a "machinery"
- **Extensibility**: we can write our own "builder" for specific logic

---

# CE limits ⚠️

- **Compiler error messages** within a CE body can be cryptic
- **Nesting different CEs** can make the code more cumbersome
  - E.g. `async` + `result`
  - Alternative: custom combining CE - see `asyncResult` in [FsToolkit](https://demystifyfp.gitbook.io/fstoolkit-errorhandling/#a-motivating-example)
- Writing our own CE can be **challenging**
  - Implementing the right methods, each the right way
  - Understanding the underlying concepts

---

# CE additional resources 🔗

- [Computation expressions series | F# for fun and profit](https://fsharpforfunandprofit.com/series/computation-expressions)
- [F# computation expressions | Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/computation-expressions)
- [Computation Expressions Workshop | GitHub](https://github.com/panesofglass/computation-expressions-workshop)

---

<!-- _class: chapter invert -->

![bg-right h:300](../themes/d-edge/pictos/SOAT_pictos_diplome.png)

# 6.

## Wrap up

---

# *Computation expression (CE)*

- Syntactic sugar: inner syntax: standard or "banged" (`let!`) \
  → Imperative-like • Easy to use
- CE is based on a *builder*
  - instance of a class with standard methods like `Bind` and `Return`
- *Separation of Concerns*
  - Business logic in the CE body
  - Machinery behind the scene in the CE builder
- Little issues for nesting or combining CEs
- Underlying functional patterns: monoid, monad, applicative
- Libraries: FSharpPlus, FsToolkit, Expecto, Farmer, Saturn...

---

# 🔗 Additional resources

- [*The "Computation Expressions" series*](https://kutt.it/drchkQ), F# for Fun and Profit
- Extending F# through Computation Expressions: 📹 [Video](https://youtu.be/bYor0oBgvws) • 📜 [Article](https://panesofglass.github.io/computation-expressions/#/)
- [Computation Expressions Workshop](https://github.com/panesofglass/computation-expressions-workshop)

---

<!-- _class: end invert lead-->

# Thanks 🙏
