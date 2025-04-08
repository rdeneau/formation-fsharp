---
marp: true
html: true
theme: 'd-edge'
title: 'F♯ Training • Les types'
footer: 'F♯ Training • Les types'
paginate: true
---

<!-- Notes :
F# ne se limite pas aux fonctions ; le puissant système de types est un autre ingrédient clé. Et comme pour les fonctions, il est essentiel de comprendre le système de types pour être à l'aise dans le langage.

En plus des types courants de .NET, F# possède d'autres types qui sont très courants dans les langages fonctionnels mais qui ne sont pas disponibles dans les langages impératifs comme C# ou Java.
-->

<!-- _class: title invert -->

# F♯ Training

## *Types composites*

### 2025 April

---

<!-- _class: toc agenda invert lead -->

![bg right:30% h:300](../themes/d-edge/pictos/SOAT_pictos_formation.png)

## Table of contents

1. Généralités
2. Tuples
3. Records
4. Unions
5. Enums
6. Records anonymes
7. Types valeur

---

<!-- _class: chapter invert -->

![bg-right h:300](../themes/d-edge/pictos/SOAT_pictos_idee.png)

# 1.

## Généralités sur les types

---

# Vue d'ensemble

Classifications des types .NET :

1. Types valeur *vs* types référence -- abrégés *TVal* et *TRef*
2. Types primitifs *vs* types composites
3. Types génériques
4. Types créés à partir de valeurs littérales
5. Types algébriques : somme *vs* produit

---

<!-- _footer: '' -->

# Types composites

Créés par combinaison d'autres types

| Catégorie      | *Version* | Nom                    | *TRef*   | *TVal*    |
|----------------|-----------|------------------------|----------|-----------|
| Types .NET     |           | `class`                | ✅        | ❌         |
|                |           | `struct`, `enum`       | ❌        | ✅         |
| Spécifiques C♯ | C♯ 3.0    | Type anonyme           | ✅        | ❌         |
|                | C♯ 7.0    | *Value tuple*          | ❌        | ✅         |
|                | C♯ 9.0    | `record (class)`       | ✅        | ❌         |
|                | C♯ 10.0   | `record struct`        | ❌        | ✅         |
| Spécifiques F♯ |           | *Tuple, Record, Union* | *Opt-in* | *Opt-out* |
|                | F♯ 4.6    | *Record* anonyme       | *Opt-in* | *Opt-out* |

---

# Types composites (2)

Peuvent être génériques (sauf `enum`)

Localisation :

- *Top-level* : `namespace`, *top-level* module F♯
- *Nested* : `class` (C♯), `module` (F♯)
- Non définissables dans méthode (C♯) ou valeur simple / fonction (F♯) !

En F♯ toutes les définitions de type se font avec mot clé `type`
→ y compris les classes, les enums et les interfaces !
→ mais les tuples n'ont pas besoin d'une définition de type

---

# Particularité des types F♯ / types .NET

*Tuple, Record, Union* sont :

- Immuables
- Non nullables
- Égalité et comparison structurelles *(sauf avec champ fonction)*
- `sealed` : ne peuvent pas être hérités
- Déconstruction, avec même syntaxe que construction 📍

Reflète approches différentes selon paradigme :

- FP : focus sur les données organisées en types
- OOP : focus sur les comportements, possiblement polymorphiques

---

# Types à valeurs littérales

Valeurs littérales = instances dont le type est inféré

- Types primitifs :          `true` (`bool`) • `"abc"` (`string`) • `1.0m` (`decimal`)
- Tuples C♯ / F♯ :           `(1, true)`
- Types anonymes C♯ : `new { Name = "Joe", Age = 18 }`
- Records F♯ :                `{ Name = "Joe"; Age = 18 }`

☝ **Note :**

- Les types doivent avoir été définis au préalable ❗
- Sauf tuples et types anonymes C♯

---

# Types algébriques

> Types composites, combinant d'autres types par produit ou par somme.

Soit les types `A` et `B`, alors on peut créer :

- Le type produit `A × B` :
  - Contient 1 composante de type `A` ET 1 de type `B`
  - Composantes anonymes ou nommées
- Le type somme `A + B` :
  - Contient 1 composante de type `A` OU 1 de type `B`

Idem par extension les types produit / somme de N types

---

## Pourquoi les termes "Somme" et "Produit" ?

Soit `N(T)` le nombre de valeurs dans le type `T`, par exemples :

- `bool` → 2 valeurs : `true` et `false`
- `unit` → 1 valeur `()`

Alors :

- Le nombre de valeurs dans le type somme `A + B` est `N(A) + N(B)`.
- Le nombre de valeurs dans le type produit `A × B` est `N(A) * N(B)`.

---

# Types algébriques *vs* Types composites

| Type *custom*                    | Somme | Produit | Composantes nommées |
|----------------------------------|-------|---------|---------------------|
| `enum`                           | ✅     | ❌       | ➖                   |
| *Union* F♯                       | ✅     | ❌       | ➖                   |
| `class` ⭐, `interface`, `struct` | ❌     | ✅       | ✅                   |
| *Record* F♯                      | ❌     | ✅       | ✅                   |
| *Tuple* F♯                       | ❌     | ✅       | ❌                   |

⭐ Classes + variations C♯ : type anonyme, *Value tuple* et `record`

👉 En C♯, pas de type somme sauf enum, très limité / type union 📍

---

# Abréviation de type

**Alias** d'un autre type : `type [name] = [existingType]`

Différents usages :

```fs
// Documenter le code voire éviter répétitions
type ComplexNumber = float * float
type Addition<'num> = 'num -> 'num -> 'num  // 👈 Marche aussi avec les génériques

// Découpler (en partie) usages / implémentation pour faciliter son changement
type ProductCode = string
type CustomerId = int
```

⚠️ Effacée à la compilation → ~~*type safety*~~
→ Compilateur autorise de passer `int` à la place de `CustomerId` !

---

<!-- _class: chapter invert -->

![bg-right h:300](../themes/d-edge/pictos/SOAT_pictos_produit.png)

# 2.

## Les Tuples

---

# Tuples : points clés

- Types à valeurs littérales
- Types "anonymes" mais on peut leur définir des alias
- Types produit par excellence
  - Signe `*` dans signature `A * B`
  - **Produit cartésien** des ensembles de valeurs de A et de B
- Nombre d'éléments:
  - 👌 2 ou 3 (`A * B * C`)
  - ⚠️ \> 3 : possible mais préférer *Record*
- Ordre des éléments est important
  - Si `A` ≠ `B`, alors `A * B` ≠ `B * A`

---

# Tuples : construction

- Syntaxe des littéraux : `a,b` ou `a, b` ou `(a, b)`
  - Virgule `,` caractéristique des tuples
  - Espaces optionnels
  - Parenthèses `()` peuvent être nécessaires
- ⚠️ Piège : séparateur différent entre un littéral et sa signature
  - `,` pour littéral
  - `*` pour signature
  - Ex : `true, 1.2` → `bool * float`

---

# Tuples : déconstruction

- Même syntaxe que construction
  - mais « de l'autre côté du `=` »
- Tous les éléments doivent apparaître dans la déconstruction
  - Utiliser la discard `_` pour ignorer l'un des éléments

```fs
let point = 1.0,2.5
let x, y = point

let x, y = 1, 2, 3 // 💥 Erreur FS0001: Incompatibilité de type...
                   // ... Les tuples ont des longueurs différentes de 2 et 3

let result = System.Int32.TryParse("123") // (bool * int)
let _, value = result // Ignore le "bool"
```

---

# Tuples en pratique

Utiliser un tuple pour une structure de données :

- Petite : 2 à 3 éléments
- Légère : pas besoin de nom pour les éléments
- Locale : échange local de données qui n'intéresse pas toute la *codebase*
  - Renvoyer plusieurs valeurs - cf. `Int32.TryParse`

Tuple immuable : les modifications se font en créant un nouveau tuple

```fs
let addOneToTuple (x,y,z) = (x+1,y+1,z+1)
```

---

# Tuples en pratique (2)

**Égalité structurelle**, mais uniquement entre 2 tuples de même signature !

```fs
(1,2) = (1,2)       // true
(1,2) = (0,0)       // false
(1,2) = (1,2,3)     // 💥 Erreur FS0001: Incompatibilité de type...
                    // ... Les tuples ont des longueurs différentes de 2 et 3
(1,2) = (1,(2,3))   // 💥 Erreur FS0001: Cette expression était censée avoir le type `int`
                    // ... mais elle a ici le type `'a * 'b`
```

**Imbrication** de tuples grâce aux `()`

```fs
let doublet = (true,1), (false,"a")     // (bool * int) * (bool * string) → pair de pairs
let quadruplet = true, 1, false, "a"    // bool * int * bool * string     → quadruplet
doublet = quadruplet                    // 💥 Erreur FS0001: Incompatibilité de type...
```

---

# Tuples : pattern matching

Patterns reconnus avec les tuples :

```fs
let print move =
    match move with
    | 0, 0 -> "No move"                     // Constante 0
    | 0, y -> $"Vertical {y}"               // Variable y (!= 0)
    | x, 0 -> $"Horizontal {x}"
    | x, y when x = y -> $"Diagonal {x}"    // Condition x et y égaux
  // `x, x` n'est pas un pattern reconnu ❗
    | x, y -> $"Other ({x}, {y})"
```

☝ **Notes :**

- Les patterns sont à ordonner du \+ spécifique au \+ générique
- Le dernier pattern `(x, y)` correspond au pattern par défaut (obligatoire)

---

# Paires

- Tuples à 2 éléments
- Tellement courant que 2 helpers leur sont associés :
  - `fst` comme *first* pour extraire le 1° élément de la paire
  - `snd` comme *second* pour extraire le 2° élément de la paire
  - ⚠️ Ne marche que pour les paires

```fs
let pair = ('a', "b")
fst pair  // 'a' (char)
snd pair  // "b" (string)
```

---

![bg-right h:300](../themes/d-edge/pictos/SOAT_pictos_question.png)

# Pair Quiz 🕹️

#### **1.** Comment implémenter soi-même `fst` et `snd` ?

```fs
let fst ... ?
let snd ... ?
```

#### **2.** Quelle est la signature de cette fonction ?

```fs
let toList (x, y) = [x; y]
```

---

![bg-right h:300](../themes/d-edge/pictos/SOAT_pictos_solution.png)

# Pair Quiz 🎲

#### **1.** Implémenter soi-même `fst` et `snd` ?

```fs
let inline fst (x, _) = x  // Signature : 'a * 'b -> 'a
let inline snd (_, y) = y  // Signature : 'a * 'b -> 'b
```

- Déconstruction avec *discard*, le tout entre `()`
- Fonctions peuvent être `inline`

---

![bg-right h:300](../themes/d-edge/pictos/SOAT_pictos_solution.png)

# Pair Quiz 🎲🎲

#### **2.** Signature de `toList` ?

```fs
let inline toList (x, y) = [x; y]
```

- Renvoie une liste avec les 2 éléments de la paire
- Les éléments sont donc du même type
- Ce type est quelconque → générique `'a`

**Réponse :** `x: 'a * y: 'a -> 'a list` 
→ Soit le type `'a * 'a -> 'a list`

---

# Tuple `struct`

- Littéral : `struct(1, 'b', "trois")`
- Signature : `struct (int * char * string)`
- Usage : optimiser performance

🔗 [https://docs.microsoft.com/en-us/dotnet/fsharp/style-guide/conventions
#performance](https://docs.microsoft.com/en-us/dotnet/fsharp/style-guide/conventions#performance)

---

<!-- _class: chapter invert -->

![bg-right h:300](../themes/d-edge/pictos/SOAT_pictos_note.png)

# 3.

## Les   *Records*

---

# Record : points clés

Type produit, alternative au Tuple quand type imprécis sous forme de tuple

- Exemple : `float * float`
  - Point dans un plan ?
  - Coordonnées géographiques ?
  - Parties réelle et imaginaire d'un nombre complexe ?
- *Record* permet de lever le doute en nommant le type et ses éléments

```fs
type Point = { X: float; Y: float }
type Coordinate = { Latitude: float; Longitude: float }
type ComplexNumber = { Real: float; Imaginary: float }
```

---

# Record : déclaration

- Membres nommés en PascalCase, pas en ~~camelCase~~
- Membres séparés `;` ou retours à la ligne
- Saut de ligne après `{` qu'en cas de membre additionnels

```fs
type PostalAddress =        ┆     type PostalAddress =
    { Address: string       ┆         {
      City: string          ┆             Address: string
      Zip: string }         ┆             City: string
                            ┆             Zip: string
                            ┆         }
                            ┆         member x.ZipAndCity = $"{x.Zip} {x.City}"
```

🔗 https://docs.microsoft.com/en-us/dotnet/fsharp/style-guide/formatting
     - [#use-pascalcase-for-type-declarations-members-and-labels](https://docs.microsoft.com/en-us/dotnet/fsharp/style-guide/formatting#use-pascalcase-for-type-declarations-members-and-labels)
     - [#formatting-record-declarations](https://docs.microsoft.com/en-us/dotnet/fsharp/style-guide/formatting#formatting-record-declarations)

---

<!-- _footer: '' -->

# Record : instanciation

- Même syntaxe qu'un objet anonyme C♯ sans `new`
  - Mais le record doit avoir été déclaré au-dessus !
- Membres peuvent être renseignés dans n'importe quel ordre
  - Mais doivent tous être renseignés → pas de membres optionnels !

```fs
type Point = { X: float; Y: float }
let point1 = { X = 1.0; Y = 2.0 }
let point2 = { Y = 2.0; X = 1.0 }   // 👈 Possible mais confusant ici
let pointKo = { Y = 2.0 }           // 💥 Error FS0764
//            ~~~~~~~~~~~ Aucune assignation spécifiée pour le champ 'X' de type 'Point'
```

⚠️ **Piège :** Syntaxe similaire mais pas identique à celle de la déclaration
     → `:` pour définir le type du membre
     → `=` pour définir la valeur du membre

---

# Record : instanciation (2)

- Les instances "longues" devraient être écrites sur plusieurs lignes
  - On peut aligner les `=` pour aider la lecture
- Les `{}` peuvent apparaître sur leur propre ligne
  - \+ facile à ré-indenter et à ré-ordonner (`Lackeys` avant `Boss`)

```fs
let rainbow =
    { Boss = "Jeffrey"
      Lackeys = ["Zippy"; "George"; "Bungle"] }

let rainbow =
    {
        Boss    = "Jeffrey"
        Lackeys = ["Zippy"; "George"; "Bungle"]
    }
```

---

# Record : déconstruction

Même syntaxe pour déconstruire un *Record* que pour l'instancier

💡 On peut ignorer certains champs
→ Rend explicite les champs utilisés 👍

```fs
let { X = x1 } = point1
let { X = x2; Y = y2 } = point1

// On peut aussi accéder aux champs via le point '.'
let x3 = point1.X
let y3 = point1.Y
```

---

# Record : déconstruction (2)

⚠️ On ne peut pas déconstruire les membres additionnels *(propriétés)* !

```fs
type PostalAddress =
    {
        Address: string
        City: string
        Zip: string
    }
    member x.CityLine = $"{x.Zip} {x.City}"

let address = { Address = ""; City = "Paris"; Zip = "75001" }

let { CityLine = cityLine } = address   // 💥 Error FS0039
//    ~~~~~~~~ L'étiquette d'enregistrement 'CityLine' n'est pas définie
let cityLine = address.CityLine         // 👌 OK
```

---

# Record : inférence

- L'inférence de type ne marche pas quand on *"dot"* une `string`
- ... mais elle marche avec un *Record* ?!

```fs
type PostalAddress =
    { Address: string
      City   : string
      Zip    : string }

let department address =
    address.Zip.Substring(0, 2) |> int
    //     ^^^^ 💡 Permet d'inférer que address est de type `PostalAddress`

let departmentKo zip =
    zip.Substring(0, 2) |> int
//  ~~~~~~~~~~~~~ Error FS0072
```

---

# Record : pattern matching

Fonction `inhabitantOf` donnant le nom des habitants à une adresse :

```fs
type Address = { Street: string; City: string; Zip: string }

let department { Zip = zip } = zip.Substring(0, 2) |> int

let inIleDeFrance departmentNumber =
    [ 75; 77; 78 ] @ [ 91..95 ] |> List.contains departmentNumber

let inhabitantOf address =
    match address with
    | { Street = "Pôle"; City = "Nord" } -> "Père Noël"
    | { City = "Paris" } -> "Parisien"
    | _ when department address = 78 -> "Yvelinois"
    | _ when department address |> inIleDeFrance -> "Francilien"
    | _ -> "Français"   // Le discard '_' sert de pattern par défaut (obligatoire)
```

---

<!-- _footer: '' -->

# Record : conflit de noms

En F♯, typage est nominal, pas structurel comme en TypeScript
→ Les mêmes étiquettes `First` et `Last` ci-dessous donnent 2 Records ≠
→ Mieux vaut écrire des types distincts ou les séparer dans ≠ modules

```fs
type Person1 = { First: string; Last: string }
type Person2 = { First: string; Last: string }
let alice = { First = "Alice"; Last = "Jones"}  // val alice: Person2...
// (car Person2 est le type le + proche qui correspond aux étiquettes First et Last)

// ⚠️ Déconstruction
let { First = firstName } = alice   // Warning FS0667
//  ~~~~~~~~~~~~~~~~~~~~~  Les étiquettes et le type attendu du champ de ce Record
//                         ne déterminent pas de manière unique un type Record correspondant

let { Person2.Last = lastName } = alice     // 👌 OK avec type en préfixe
let { Person1.Last = lastName } = alice     // 💥 Error FS0001
//                                ~~~~~ Type 'Person1' attendu, 'Person2' reçu
```

---

# Record : modification

Record immuable mais facile de créer nouvelle instance ou copie modifiée
→ Expression de _**copy and update**_ d'un *Record*
→ Syntaxe spéciale pour ne modifier que certains champs
→ Multi-lignes si expression longue

```fs
let address2 = { address with Street = "Rue Vivienne" }

let { City = city; Zip = zip } = address
let address2' = { Street = "Rue Vivienne"; City = city; Zip = zip }
// address2 = address2'

let address3 =                  ┆      let address3' =
    { address with              ┆          { Street = address.Street
        City = "Lyon"           ┆            City   = "Lyon"
        Zip  = "69001" }        ┆            Zip    = "69001" }
// address3 = address3'
```

---

# Record *copy-update* : C♯ / F♯ / JS

```cs
// Record C♯ 9.0
address with { Street = "Rue Vivienne" }
```

```fs
// F♯ copy and update
{ address with Street = "Rue Vivienne" }
```

```js
// Object destructuring with spread operator
{ ...address, street: "Rue Vivienne" }
```

---

# Record *copy-update* : limites 🛑

Lisibilité réduite quand plusieurs niveaux imbriqués

```fs
type Street = { Num: string; Label: string }
type Address = { Street: Street }
type Person = { Address: Address }

let person = { Address = { Street = { Num = "15"; Label = "rue Neuf" } } }

let person' =
    { person with
        Address =
          { person.Address with
              Street =
                { person.Address.Street with
                    Num = person.Address.Street.Num + " bis" } } }
```

---

# Record `struct`

Attribut `[<Struct>]` permet de passer d'un type référence à un type valeur :

```fs
[<Struct>]
type Point = { X: float; Y: float; Z: float }
```

⚖️ **Pros/Cons d'une `struct` :**

- ✅ Performant car ne nécessite pas de *garbage collection*
- ⚠️ Passée par valeur → pression sur la mémoire

👉 Adapté à un type "petit" en mémoire (~2-3 champs)

---

<!-- _class: chapter invert -->

![bg-right h:300](../themes/d-edge/pictos/SOAT_pictos_baseDonnees.png)

# 4.

## Les   Unions

---

# Unions : points clés

- Terme exacte : « Union discriminée », *Discriminated Union (DU)*
- Types Somme : représente un **OU**, un **choix** entre plusieurs *Cases*
  - Même principe que pour une `enum` mais généralisé
- Chaque *case* doit avoir un *Tag* *(a.k.a Label)* -- en PascalCase ❗
  - C'est le **discriminant** de l'union pour identifier le *case*
- Chaque *case* **peut** contenir des données
  - Si Tuple, ses éléments peuvent être nommés -- en camelCase

```fs
type Billet =
    | Adulte                 // aucune donnée -> ≃ singleton stateless
    | Senior of int          // contient un 'int' (mais on ne sait pas ce que c'est)
    | Enfant of age: int     // contient un 'int' de nom 'age'
    | Famille of Billet list // contient une liste de billet
                             // type récursif -- pas besoin de 'rec'
```

---

# Unions : déclaration

Sur plusieurs lignes : 1 ligne / *case*
→ ☝ Ligne indentée et commençant par `|`

Sur une seule ligne -- si déclaration reste **courte** ❗
→ 💡 Pas besoin du 1er `|`

```fs
open System

type IntOrBool =
    | Int32 of Int32                        // 💡 Tag de même nom que ses données
    | Boolean of Boolean

type OrderId = OrderId of int               // 👈 Single-case union
                                            // 💡 Tag de même nom que l'union parent
type Found<'T> = Found of 'T | NotFound     // 💡 Type générique
```

---

# Unions : instanciation

*Tag* ≃ **constructeur**
→ Fonction appelée avec les éventuelles données du *case*

```fs
type Shape =
    | Circle of radius: int
    | Rectangle of width: int * height: int

let circle = Circle 12          // Type: 'Shape', Valeur: 'Circle 12'
let rect = Rectangle (4, 3)     // Type: 'Shape', Valeur: 'Rectangle (4, 3)'

let circles = [1..4] |> List.map Circle     // 👈 Tag employé comme fonction
```

---

# Unions : conflit de noms

Quand 2 unions ont des tags de même nom
→ Qualifier le tag avec le nom de l'union

```fs
type Shape =
    | Circle of radius: int
    | Rectangle of width: int * height: int

type Draw = Line | Circle       // 'Circle' sera en conflit avec le tag de 'Shape'

let draw = Circle              // Type='Draw' (type le + proche) -- ⚠️ à éviter car ambigu

// Tags qualifiés par leur type union
let shape = Shape.Circle 12
let draw' = Draw.Circle
```

---

# Unions : accès aux données

Uniquement via *pattern matching*
Matching d'un type Union est **exhaustif**

```fs
type Shape =
    | Circle of radius: float
    | Rectangle of width: float * height: float

let area shape =
    match shape with
    | Circle r -> Math.PI * r * r   // 💡 Même syntaxe que instanciation
    | Rectangle (w, h) -> w * h

let isFlat = function 
    | Circle 0.                     // 💡 Constant pattern
    | Rectangle (0., _)
    | Rectangle (_, 0.) -> true     // 💡 OR pattern
    | Circle _
    | Rectangle _ -> false
```

---

# Unions : *single-case*

Union avec un seul cas encapsulant un type (généralement primitif)

```fs
type CustomerId = CustomerId of int
type OrderId = OrderId of int

let fetchOrder (OrderId orderId) =    // 💡 Déconstruction directe sans 'match'
    ...
```

Assure *type safety* contrairement au simple type alias
→ Impossible de passer un `CustomerId` à une fonction attendant un `OrderId` 👍

Permet d'éviter *Primitive Obsession* à coût minime

---

# Unions : style "enum"

Tous les *cases* sont vides = dépourvus de données
→ ≠ `enum` .NET 📍

L'instanciation et le pattern matching se font juste avec le *tag*
→ Le *tag* n'est plus une ~~fonction~~ mais une valeur *([singleton](https://fsharpforfunandprofit.com/posts/fsharp-decompiled/#enum-style-unions))*

```fs
type Answer = Yes | No | Maybe
let answer = Yes

let print answer =
    match answer with
    | Yes   -> printfn "Oui"
    | No    -> printfn "Non"
    | Maybe -> printfn "Peut-être"
```

---

<!-- _class: chapter invert -->

![bg-right h:300](../themes/d-edge/pictos/SOAT_pictos_innovation.png)

# 5.

## Les    Enums

Vraies `enum` .NET

---

# Enum : déclaration

- Ensemble de constantes de type entier (`byte`, `int`...)
- Syntaxe ≠ union

```fs
type Color  = Red | Green | Blue        // Union
type ColorN = Red=1 | Green=2 | Blue=3  // Enum

type AnswerChar = Yes='Y' | No='N'  // 💡 enum basée sur 'char'
type AnswerChar = Yes="Y" | No="N"  // 💥 Error FS0951
//   ~~~~~~~~~~ Littéraux énumérés doivent être de type 'int'...

type File = a='a' | b='b' | c='c'  // 💡 Membres d'enum peuvent être en camelCase
```

---

# Enum : usage

⚠️ Contrairement aux unions, l'emploi d'un littéral d'enum est forcément qualifié

```fs
let answerKo = Yes            // 💥 Error FS0039
//             ~~~ La valeur ou le constructeur 'Yes' n'est pas défini.
let answer = AnswerChar.Yes   // 👌 OK
```

Cast via helpers `int` et `enum` (mais pas `char`) :

```fs
let redValue = int ColorN.Red         // enum -> int
let redAgain = enum<ColorN> redValue  // int -> enum via type générique
let red: ColorN = enum redValue       // int -> enum via annotation

// ⚠️ Ne marche pas avec char enum
let ko = char AnswerChar.No   // 💥 Error FS0001
let no: AnswerChar = enum 'N' // 💥 Error FS0001
```

---

# Enum : matching

⚠️ Contrairement aux unions, le *pattern matching* n'est pas exhaustif

```fs
type ColorN = Red=1 | Green=2 | Blue=3  // Enum

let toHex color =
    match color with
    | ColorN.Red   -> "FF0000"
    | ColorN.Green -> "00FF00"
    | ColorN.Blue  -> "0000FF"
    // ⚠️ Warning FS0104: Les enums peuvent accepter des valeurs en dehors des cas connus.
    // Par exemple, la valeur 'enum<ColorN> (0)' peut indiquer un cas non traité.

    // 💡 Pour enlever le warning, il faut ajouter un pattern générique
    | _ -> invalidArg (nameof color) $"Color {color} not supported"
```

---

# Enum : flags

Même principe qu'en C♯ :

```fs
open System

[<FlagsAttribute>]
type PermissionFlags =
    | Read    = 1
    | Write   = 2
    | Execute = 4

let permission = PermissionFlags.Read ||| PermissionFlags.Write

let canRead = permission.HasFlag PermissionFlags.Read
```

💡 Noter l'opérateur `|||` : OU binaire *(`|` en C♯)*

---

# Enum *vs* Union

| Type  | Données     | Qualification        | Exhaustivité |
|-------|-------------|----------------------|--------------|
| Enum  | Entières    | Obligatoire          | ❌ Non        |
| Union | Quelconques | Qu'en cas de conflit | ✅ Oui        |

☝ **Recommandation :**

- Préférer une Union dans la majorité des cas
- Choisir une Enum pour :
  - Interop .NET
  - Besoin de lier données entières

---

<!-- _class: chapter invert -->

![bg-right h:300](../themes/d-edge/pictos/SOAT_pictos_rechercheProfil.png)

# 6.

## Record anonyme

---

# Record anonyme

- Depuis F♯ 4.6 *(mars 2019)*
- Syntaxe : idem *Record* avec accolades "larges" `{| fields |}`
  - `{| Age: int |}` → signature
  - `{| Age = 15 |}` → instance
- Typage *inline* : pas besoin de pré-définir un `type` nommé
  - Alternative aux *Tuples*
- Autorisé en entrée/sortie de fonction
  - ≠ Type anonyme C♯

---

# Record anonyme : bénéfices ✅

• Réduire *boilerplate*
• Améliorer interop avec systèmes externes (JavaScript, SQL...)

Exemples *(détaillés ensuite)* :

- Projection LINQ
- Personnalisation d'un record existant
- Sérialisation JSON
- Signature *inline*
- Alias par module

---

## ✅ Projection LINQ

💡 Sélectionner un sous-ensemble de propriétés

```fs
let names =
    query {
        for p in persons do
        select {| Name = p.FirstName |}
    }
```

En C♯, on utiliserait un type anonyme :

```cs
var names =
    from p in persons
    select new { Name = p.FirstName };
```

*🔗 https://queil.net/2019/10/fsharp-vs-csharp-anonymous-records/*

---

## ✅ Personnalisation d'un record existant

💡 Un record anonyme peut être instancié à partir d'une instance de record

```fs
type Person = { Age: int; Name: string }
let william = { Age = 12; Name = "William" }

// Ajout d'un champ (Gender)
let william' = {| william with Gender = "Male" |}
            // {| Age = 12; Name = "William"; Gender = "Male" |}

// Modification de champs (Name, Age: int => float)
let jack = {| william' with Name = "Jack"; Age = 16.5 |}
        // {| Age = 16.5; Name = "Jack"; Gender = "Male" |}
```

---

## ✅ Sérialisation JSON

😕 Unions sérialisées dans un format pas pratique

```fs
#r "nuget: Newtonsoft.Json"
let serialize obj = Newtonsoft.Json.JsonConvert.SerializeObject obj

type CustomerId = CustomerId of int
type Customer = { Id: CustomerId; Age: int; Name: string; Title: string option }

serialize { Id = CustomerId 1; Age = 23; Name = "Abc"; Title = Some "Mr" }
```

```json
{
  "Id": { "Case": "CustomerId", "Fields": [ 1 ] }, // 👀
  "Age": 23,
  "Name": "Abc",
  "Title": { "Case": "Some", "Fields": [ "Mr" ] }  // 👀
}
```

---

## ✅ Sérialisation JSON (2)

💡 Définir un record anonyme pour sérialiser un *customer*

```fs
let serialisable customer =
    let (CustomerId customerId) = customer.Id
    {| customer with
         Id = customerId
         Title = customer.Title |> Option.toObj |}

serialize (serialisable { Id = CustomerId 1; Age = 23; Name = "Abc"; Title = Some "Mr" })
```

```json
{
  "Id": 1, // ✅
  "Age": 23,
  "Name": "Abc",
  "Title": "Mr"  // ✅
}
```

---

## ✅ Signature *inline*

💡 Utiliser un record anonyme *inline* pour réduire charge cognitive

```fs
type Title = Mr | Mrs
type Customer =
    { Age  : int
      Name : {| First: string; Middle: string option; Last: string |} // 👈
      Title: Title option }
```

---

## ✅ Alias par module

```fs
module Api =
    type Customer = // ☝ Customer est un alias
        {| Id   : System.Guid
           Name : string
           Age  : int |}

module Dto =
    type Customer =
        {| Id   : System.Guid
           Name : string
           Age  : int |}

let (customerApi: Api.Customer) = {| Id = Guid.Empty; Name = "Name"; Age = 34 |}
let (customerDto: Dto.Customer) = customerApi // 🎉 Pas besoin de convertir
```

💡 Instant t : même type dans 2 modules
💡 Plus tard : facilite personnalisation des types par module

---

# Record anonyme : limites 🛑

```fs
// Inférence limitée
let nameKo x = x.Name  // 💥 Error FS0072: Lookup on object of indeterminate type...
let nameOk (x: {| Name:string |}) = x.Name

// Pas de déconstruction
let x = {| Age = 42 |}
let {  Age = age  } = x  // 💥 Error FS0039: The record label 'Age' is not defined
let {| Age = age |} = x  // 💥 Error FS0010: Unexpected symbol '{|' in let binding

// Pas de fusion
let banana = {| Fruit = "Banana" |}
let yellow = {| Color = "Yellow" |}
let banYelKo = {| banana with yellow |} // 💥 Error FS0609...
let banYelOk = {| banana with Color = "Yellow" |}

// Pas d'omission
let ko = {| banYelOk without Color |}  // 💥 Mot clé 'without' n'existe pas
```

---

# Record anonyme : limites 🛑

```fs
// Pas du typage structurel => tous les champs sont requis
let capitaliseFruit (x: {| Fruit: string |}) = x.Fruit.ToUpper()
capitaliseFruit {| Fruit = "Banana" |}                      // 👌 "BANANA"
capitaliseFruit {| Fruit = "Banana"; Origin = "Réunion" |}  // 💥 Too much fields... [Origin]
```

---

<!-- _class: purple chapter invert -->

![bg-right h:300](../themes/d-edge/pictos/SOAT_pictos_entretien.png)

# 7.

## Types valeur

---

# Type composite *struct*

Type composite : peut être déclaré en tant que type valeur
→ Instances stockées dans la **pile** *(stack)* plutôt que dans le tas *(heap)*
→ Permet parfois de gagner en performance
→ Plutôt adapté aux types compacts : peu de champs, peu de comportements

- Attribut `[<Struct>]`
- Mot clé `struct`
- Structure

---

# Attribut `[<Struct>]`

Pour *Record* et *Union*

À placer avant ou après le mot cle `type`

```fs
type [<Struct>] Point = { X: float; Y: float }

[<Struct>]
type SingleCase = Case of string
```

---

# Mot clé `struct`

Pour littéral de Tuple et *Record* anonyme

```fs
let t = struct (1, "a")
// struct (int * string)

let a = struct {| Id = 1; Value = "a" |}
// struct {| Id: int; Value: string |}
```

---

# Structures

Alternatives aux classes 📍 mais \+ limités / héritage et récursivité

👉 Cf. session sur l'orienté-objet et les classes...

---

<!-- _class: chapter invert -->

![bg-right h:300](../themes/d-edge/pictos/SOAT_pictos_diplome.png)

# 8.

## Le    Récap’

---

<!-- _footer: '' -->
![bg-right h:300](../themes/d-edge/pictos/SOAT_pictos_question.png)

# 🕹️ Récap' Quiz

```fs
// Relier types et concepts
type Color1 = int * int * int
type Color2 = Red | Green | Blue
type Color3 = Red=1 | Green=2 | Blue=3
type Color4 = { Red: int; Green: int; Blue: int }
type Color5 = {| Red: int; Green: int; Blue: int |}
type Color6 = Color of Red: int * Green: int * Blue: int
type Color7 =
    | RGB of { Red: int; Green: int; Blue: int }
    | HSL of { Hue: int; Saturation: int; Lightness: int }

// A. Alias
// B. Enum
// C. Record
// D. Record anonyme
// E. Single-case union
// F. Union
// G. Union enum-like
// H. Tuple
```

---

# 🎲 Récap' Quiz

| Types                                       | Concepts                                |
|---------------------------------------------|-----------------------------------------|
| `type Color1 = int * int * int`             | **H.** Tuple + **A.** Alias             |
| `type Color2 = Red ∣ Green ∣ Blue`           | **G.** Union enum-like                  |
| `type Color3 = Red=1 ∣ Green=2 ∣ Blue=3`     | **B.** Enum                             |
| `type Color4 = { Red: int; Green: int… }`   | **C.** Record                           |
| `type Color5 = {∣ Red: int; Green: int… ∣}`  | **D.** Record anonyme + **A.** Alias    |
| `type Color6 = Color of Red: int * …`       | **E.** Single-case union + **H.** Tuple |
| `type Color7 = RGB of { Red: int… } ∣ HSL…`  | **F.** Union + **C.** Record            |

---

# Composition de types

Création de nouveaux types ?
→ ❌ Les types algébriques ne supportent pas l'héritage.
→ ✅ Par composition, dans *sum/product type*
→ 💡 Extension d'un *Record* en un *Record* anonyme avec champs en \+

Combiner 2 unions ?
→ ❌ Pas "aplatissable" comme en TypeScript ①
→ ✅ Nouveau type union ②

```fs
type Noir = Pique | Trefle
type Rouge = Coeur | Carreau
type CouleurKo = Noir | Rouge  // (1) ❌ ≠ Pique | Trefle | Coeur | Carreau 
type Couleur = Noir of Noir | Rouge of Rouge // (2) ✅
let c1 = Noir Pique
```

---

![bg-right h:300](../themes/d-edge/pictos/SOAT_pictos_cocktail.png)

# Conclusion

Beaucoup de façons de modéliser !

De quoi s'adapter :

- À tous les goûts ?
- En fait surtout au domaine métier !

---

<!-- _class: end invert lead-->

# Thanks 🙏
