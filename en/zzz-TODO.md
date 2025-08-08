# TODO

- [x] Créer un support séparé pour functional pattern + CE
- [x] Expliquer ce qu'on entend par "effects" - cf. Gemini "What is the relationship between monads and effects?"
- [x] Toucher 2 mots à propos des monad stack & monad transformers (au niveau des limites des monades) - cf. Gemini "monad transformers?"
- [x] Corriger FSharpPlus monad: on peut utiliser plusieurs monades, mais il faut utiliser des monad transformers - cf. Gemini
- [x] En intro des CE, citer la doc Ms - https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/computation-expressions
  - Computation expressions in F# provide a convenient syntax for writing computations that can be sequenced and combined using control flow constructs and bindings. Depending on the kind of computation expression, they can be thought of as a way to express monads, monoids, monad transformers, and applicative functors.
  - L'aspect "monad transformers" est un peu usurpé puisqu'il s'agit en fait d'écrire un custom builder that directly handles the combined logic/effects.
- [x] CE applicative: expliquer quelles méthodes implémenter - cf. Gemini "Relationship between applicative and F# computation expressions?"
- [x] The `Delay` method is not a part of the monoidal structure itself. It is an **orthogonal feature** related to the evaluation strategy of the computation expression, enabling **laziness and short-circuiting**. While not required for the underlying type to be a monoid, it is often a practical necessity for building efficient and powerful computation expressions that leverage a monoidal structure.
- [x] F# compiler injects a call to Zero in certain situations, such as an if expression without an else branch.
- [x] Inversion d'ordre entre Bind et let! -> continuation
- [x] Vérifier si le `do!` nécessite de définir la méthode `Zero() = M<unit>`? Donner un exemple avec `result {}`
- [x] Implémenter `TryXxx` et `Using` pour `result {}`
- [n] Diff keywords banged or not: The difference is easy to remember when you realize that the operations without a “!” always have unwrapped types on the right hand side, while the ones with a “!” always have wrapped types.
- [x] Diff yield vs return: At this point you might be wondering: if return and yield are basically the same thing, why are there two different keywords? The answer is mainly so that you can enforce appropriate syntax by implementing one but not the other. For example, the seq expression does allow yield but doesn’t allow return, while the async does allow return, but does not allow yield
- [x] Combine needs Delay, otherwise: `This control construct may only be used if the computation expression builder defines a 'Delay' method`. `Run` method exists for exactly this reason. It is called as the final step in the process of evaluating a computation expression, and can be used to undo the delay.
- [x] Monoid: example by implementing a `list` CE and compare desugaring of it vs regular list comprehension
- [n] monadic containers vs monadic computations
- [x] lien vers la solution FSharpTraining
- [x] verify Ce applicative: map2 is MergeSource or MergeSource2 ?
- [x] `for = do` vs `for in do`
- [n] CE activity de Loïc: on peut étendre tout type pour ajouter le support syntaxe CE...
- [ ] Mettre à jour le GitBook
- [ ] Extraire des articles du GitBook

## Tables

| Function    | Regular    | Monadic       |
|-------------|------------|---------------|
| Signature   | `'a -> 'b` | `'a -> M<'b>` |
| Pipeline    | `▷`        | `>>=`         |
| Composition | `>>`       | `>=>` (*)     |

| Difference              | Eager   | Lazy                          |
|-------------------------|---------|-------------------------------|
| `Delay` return type     | `int`   | `unit -> int`                 |
| `Run`                   | Omitted | Required to get back an `int` |
| `Combine` 2nd parameter | `int`   | `unit -> int`                 |
| `For` call to `Delay`   | Omitted | Explicit but not required     |

| Method  | CE       | Signature                                       |
|---------|----------|-------------------------------------------------|
| `For`   | Monoidal | `seq<T> * (T -> M<U>)    -> M<U> or seq<M<U>>`  |
|         | Monadic  | `seq<T> * (T -> M<unit>) -> M<unit>`            |
| `While` | Monoidal | `(unit -> bool) * Delayed<T>        -> M<T>`    |
|         | Monadic  | `(unit -> bool) * (unit -> M<unit>) -> M<unit>` |

| Method    | CE                           | `seq`           |
|-----------|------------------------------|-----------------|
| `Combine` | `xs @ ys => List.append`     | `Seq.append`    |
| `Yield`   | `[x]     => List.singleton`  | `Seq.singleton` |
| `Zero`    | `[]      => List.empty`      | `Seq.empty`     |
| `For`     | `Seq.collect` & `Seq.toList` | `Seq.collect`   |
