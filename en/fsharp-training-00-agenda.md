# F♯ Training - Agenda

## Pitch: Why F♯?

[Answer in a single tweet](https://nitter.net/MokoSharma/status/1458151277343379457)

In detail :

- Multi-paradigm language with a strong functional orientation
  - Functional programming _(FP)_ principles: immutability and composition
  - Building blocks: functions and algebraic data types _(ADT)_
- "Fun" ! Very pleasant to write and read
  - Expressive and concise (not very verbose syntax)
  - Sensitive to indentation → easy to read
  - Strong static typing but it's almost seamless thanks to type inference
- Language entreprise-friendly
  - Runs on the .NET platform → high performance, easy interop with C# projects, .NET ecosystem and libraries
  - Very good tooling: Visual Studio, VsCode, Rider
  - Robust code: predictable and reproducible results (immutability, structural equality, no nulls, exhaustive pattern matching)
  - Interactive programming: check code by evaluating it in the FSI console
  - Narrowed community but strong and friendly
- F# compared to other back-end functional languages
  - Its syntax is not hybrid, unlike **Scala** and **Kotlin** (mixing OOP and FP)
  - Easier to learn than **Haskell** or **OCaml** _(but with fewer FP features, partially compensated for by OOP features)_
  - Static typing, unlike **Closure** _(and far fewer brackets 😜)_

## Plan

1. Bases - ⏱ 3h
2. Fonctions - ⏱ 2h
3. Types - ⏱ 2h
4. Types: addendum - ⏱ 3h
5. Pattern matching - ⏱ 2-3h
6. Collections - ⏱ 3h
7. Programmation asynchrone - ⏱ 1,5h
8. Types monadiques - ⏱ 3h
9. Module & namespace - ⏱ 1,5h
10. Orientée-objet - ⏱ 1,5h

## Ressources

- [F# 6.0](https://devblogs.microsoft.com/dotnet/whats-new-in-fsharp-6/) : sorti en [novembre 2021](https://devblogs.microsoft.com/dotnet/fsharp-6-is-officially-here/)
- Domain modelling
  - 📘 [Domain modelling made functional](https://www.goodreads.com/book/show/34921689-domain-modeling-made-functional) *(Nov 2017)* de Scott Wlaschin
  - 📜 [Alternate Ways of Creating Single-Case Unions in F#](https://trustbit.tech/blog/2021/05/01/alternate-ways-of-creating-single-case-discriminated-unions-in-f-sharp) *(May 2021)*, par Ian Russel
  - 📜 Single-Case Unions: [Part 1](https://paul.blasuc.ci/posts/really-scu.html) *(May 2021)*, [Part 2](https://paul.blasuc.ci/posts/even-more-scu.html) *(Jul 2021)* de Paul Blasucci
- [Choisir entre C# et F#](https://www.partech.nl/nl/publicaties/2021/06/key-differences-between-c-sharp-and-f-sharp)
- [Paket](https://fsprojects.github.io/Paket/index.html) plutôt que Nuget
- [FAKE](https://fake.build/) : outil de build
- [Fantomas](https://github.com/fsprojects/fantomas) : formateur de code F#
- Unit Tests
  - Libraries : xUnit + [FsUnit](http://fsprojects.github.io/FsUnit/), [Unquote](https://github.com/SwensenSoftware/unquote), [Expecto](https://github.com/haf/expecto)
    - [Review: F# unit testing frameworks and libraries](https://devonburriss.me/review-fsharp-test-libs/)
  - BDD : [TickSpec](https://github.com/mchaloupka/tickspec)
  - Property-based testing : [FsCheck](https://fscheck.github.io/FsCheck/)
- [Query expressions](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/query-expressions) : support de LINQ en F#
- Concurrent programming
  - Keyword `lock` → [Exemple](https://www.compositional-it.com/news-blog/testing-for-breaking-changes/)
  - [MailboxProcessor](https://fsharpforfunandprofit.com/posts/concurrency-actor-model/) *(actor-based concurrent programing model)*
- [Quotation](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/code-quotations)
- Data access
  - [Guide : data access with F#](https://fsharp.org/guides/data-access/) sur fsharp.org
  - [Type providers](https://docs.microsoft.com/en-us/dotnet/fsharp/tutorials/type-providers/)
  - ORM : EFCore, Dapper, [RepoDb](https://repodb.net/)
- [Dév Web](https://docs.microsoft.com/fr-fr/dotnet/fsharp/scenarios/web-development)
  - [Giraffe](https://github.com/giraffe-fsharp/Giraffe#giraffe) : surcouche à ASP.NET Core
  - [Saturne](https://saturnframework.org/) : framework alternatif à ASP.NET Core
  - [Fable](https://fable.io/) : compilateur F# → JavaScript
  - [SAFE stack](https://safe-stack.github.io/) : stack complète comprenant (entre autres) **S**aturn, **A**zure, **F**able, **E**lmish.
- Cloud
  - [Guide on fsharp.org](https://fsharp.org/guides/cloud/)
  - Infra-as-Code : Azure + [Farmer](https://compositionalit.github.io/farmer/) ([intro](https://docs.microsoft.com/fr-fr/dotnet/fsharp/using-fsharp-on-azure/deploying-and-managing))
- Data Science et Notebook
  - [Guide on fsharp.org](https://fsharp.org/guides/data-science/)
- Misc
  - 🔗 [Awesome F#](https://github.com/fsprojects/awesome-fsharp)
  - 🔗 [Microsoft F# Style guide](https://docs.microsoft.com/en-us/dotnet/fsharp/style-guide/conventions#use-classes-to-contain-values-that-have-side-effects) : organisation du code, usage adéquate des classes, gestion des erreurs (par types ou exceptions), application partielle et style point-free, encapsulation, inférence de types et génériques, performance
