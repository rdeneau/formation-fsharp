# Formation F# 5.0

<!--
Commandes :
- HTM: marp masterclass-fsharp-6-module.md --watch --theme themes/soat.css
- PDF: marp masterclass-fsharp-5-types.md --pdf --allow-local-files --theme themes/soat.css
-->

## Pitch : pourquoi F# ?

[En un tweet](https://twitter.com/MokoSharma/status/1458151277343379457)

En détail :

- Langage multi-paradigme avec une forte orientation fonctionnelle
  - Principes fonctionnels : immutabilité et composition
  - Briques : fonctions et types algébriques
- "Fun" ! Très agréable à écrire et à lire
  - Expressif et concis (syntaxe peu verbeuse)
  - Sensible à l'indentation → facilite la lecture
  - Typage statique fort mais quasi-transparent grâce à l'inférence de type
- Langage entreprise friendly
  - Tourne sur la plateforme .NET → performant, interop aisée avec projets C#
  - Très bon tooling : VS, VScode, Rider
  - Code robuste : résultats prédictibles et reproductibles (immuabilité, égalité structurelle, absence de null, vérification exhaustive des cas dans *pattern matching*)
  - Communauté solide, nombreuses librairies *F# friendly*
  - Programmation interactive : vérifier un code en l'évaluant dans la console FSI
- F# par rapport aux autres langages fonctionnels "Back"
  - Sa syntaxe n'est pas hybride contrairement à Scala et Kotlin (mixe OOP/FP)
  - Plus facile à apprendre que Haskell *(mais moins pur)*
  - Typage statique contrairement à Closure *(et beaucoup beaucoup moins de parenthèses)*

## 1. Bases - ⏱ 3h

- Le F♯, c'est quoi ?
- Syntaxe : fondamentaux, indentation
- Premiers concepts
  - Curryfication, application partielle
  - Tout est expression
  - Inférence de type

## 2. Fonctions - ⏱ 3h

- Signature des fonctions
- Fonctions
- Opérateurs
- Interop avec BCL .NET

## 3. Typage - Types composites - ⏱ 3h

- Généralités, type algébrique
- Tuples
- Records
- Unions
- Enums
- Records anonymes
- Types valeur

## 4. Typage - Compléments - ⏱ 3h

- Type `unit`
- Génériques
- Contraintes sur paramètres de type
- Type flexible
- Unités de mesure
- *Casting* et conversion

## 5. *Pattern matching* - ⏱ 2-3h

- Patterns
- Match expression
- Active patterns

## 6. Collections - ⏱ 3h

`Array`, `List`, `Seq`, `Set`, `Map`, `String`

- Vue d'ensemble
- Types
- Fonctions

## 7. Programmation asynchrone - ⏱ 1,5h

- *Workflow* asynchrone - Bloc `async {}`
- Interop avec la TPL .NET - bloc `task {}`

## 8. Types monadiques - ⏱ 3h

- Types `Option` et `Result` *(Railway-oriented programming)*
- *Smart constructors*
- *Computation expression*
  - Bases
  - Librairies
  - Concepts théoriques : Monoïde, Monade, Applicative

## 9. Module & namespace - ⏱ 1,5h

- Différentier module et namespace
- Organiser son code

## 10. Orientée-objet - ⏱ 1,5h

- Membres : méthodes, propriétés, opérateurs
- Classe, structure
- Interface
- Expression objet
- Extensions de type

## Pour aller plus loin

- [F# 6.0](https://devblogs.microsoft.com/dotnet/whats-new-in-fsharp-6/) : sorti en [novembre 2021](https://devblogs.microsoft.com/dotnet/fsharp-6-is-officially-here/)
- Domain modelling
  - 📘 [Domain modelling made functional](https://www.goodreads.com/book/show/34921689-domain-modeling-made-functional) *(Nov 2017)* de Scott Wlaschin
  - 📜 [Alternate Ways of Creating Single-Case Unions in F#](https://trustbit.tech/blog/2021/05/01/alternate-ways-of-creating-single-case-discriminated-unions-in-f-sharp) *(May 2021)*, par Ian Russel
  - 📜 Single-Case Unions: [Part 1](https://paul.blasuc.ci/posts/really-scu.html) *(May 2021)*, [Part 2](https://paul.blasuc.ci/posts/even-more-scu.html) *(Jul 2021)* de Paul Blasucci
- [Choisir entre C# et F#](https://www.partech.nl/nl/publicaties/2021/06/key-differences-between-c-sharp-and-f-sharp)
- [Paket](https://fsprojects.github.io/Paket/index.html) plutôt que Nuget
- [FAKE](https://fake.build/) : outil de build
- [Fantomas](https://github.com/fsprojects/fantomas) : formateur de code F#
- Tests unitaires
  - Librairies : xUnit + [FsUnit](http://fsprojects.github.io/FsUnit/), [Unquote](https://github.com/SwensenSoftware/unquote), [Expecto](https://github.com/haf/expecto)
    - [Review: F# unit testing frameworks and libraries](https://devonburriss.me/review-fsharp-test-libs/)
  - BDD : [TickSpec](https://github.com/mchaloupka/tickspec)
  - Property-based testing : [FsCheck](https://fscheck.github.io/FsCheck/)
- [Query expressions](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/query-expressions) : support de LINQ en F#
- Programmation concurrente
  - Mot-clé `lock` → [Exemple](https://www.compositional-it.com/news-blog/testing-for-breaking-changes/)
  - [MailboxProcessor](https://fsharpforfunandprofit.com/posts/concurrency-actor-model/) *(actor-based concurrent programing model)*
- [Quotation](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/code-quotations)
- Accès aux données
  - [Guide : data access with F#](https://fsharp.org/guides/data-access/) sur fsharp.org
  - [Type providers](https://docs.microsoft.com/en-us/dotnet/fsharp/tutorials/type-providers/)
  - ORM : EFCore, Dapper, [RepoDb](https://repodb.net/)
- [Dév Web](https://docs.microsoft.com/fr-fr/dotnet/fsharp/scenarios/web-development)
  - [Giraffe](https://github.com/giraffe-fsharp/Giraffe#giraffe) : surcouche à ASP.NET Core
  - [Saturne](https://saturnframework.org/) : framework alternatif à ASP.NET Core
  - [Fable](https://fable.io/) : compilateur F# → JavaScript
  - [SAFE stack](https://safe-stack.github.io/) : stack complète comprenant (entre autres) **S**aturn, **A**zure, **F**able, **E**lmish.
- Cloud
  - [Guide sur fsharp.org](https://fsharp.org/guides/cloud/)
  - Infra-as-Code : Azure + [Farmer](https://compositionalit.github.io/farmer/) ([intro](https://docs.microsoft.com/fr-fr/dotnet/fsharp/using-fsharp-on-azure/deploying-and-managing))
- Data Science et Notebook :
  - [Guide sur fsharp.org](https://fsharp.org/guides/data-science/)

## Autres ressources

- 🔗 [Awesome F#](https://github.com/fsprojects/awesome-fsharp)
- 🔗 [Microsoft F# Style guide](https://docs.microsoft.com/en-us/dotnet/fsharp/style-guide/conventions#use-classes-to-contain-values-that-have-side-effects) : organisation du code, usage adéquate des classes, gestion des erreurs (par types ou exceptions), application partielle et style point-free, encapsulation, inférence de types et génériques, performance
