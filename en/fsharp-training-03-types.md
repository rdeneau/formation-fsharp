---
marp: true
html: true
theme: 'd-edge'
title: 'F♯ Training • Types'
footer: 'F♯ Training • Types'
paginate: true
---

<!-- Notes :
F# isn't just about functions; the powerful type system is another key ingredient. And as with functions, it's essential to understand the type system to be comfortable with the language.

In addition to the common .NET types, F# has other types that are very common in functional languages but not available in imperative languages like C# or Java.
-->

<!-- _class: title invert -->

# F♯ Training

## *Types composites*

### 2025 April

---

<!-- _class: toc agenda invert lead -->

![bg right:30% h:300](../themes/d-edge/pictos/SOAT_pictos_formation.png)

## Table of contents

- Overview
- Tuples
- Records
- Unions
- Enums
- Anonymous records
- Value types

---

<!-- _class: chapter invert -->

![bg-right h:300](../themes/d-edge/pictos/SOAT_pictos_idee.png)

# 1.

## Types Overview

---

# Overview

.NET type classifications:

1. Value types *vs* reference types -- abbreviated *TVal* and *TRef*
2. Primitive types *vs* composite types
3. Generic types
4. Types created from literal values
5. Algebraic types: sum *vs* product

---

<!-- _footer: '' -->

# Composite types

Created by combining other types

| Types          | *Version* | Name                   | *TRef*   | *TVal*    |
|----------------|-----------|------------------------|----------|-----------|
| Types .NET     |           | `class`                | ✅        | ❌         |
|                |           | `struct`, `enum`       | ❌        | ✅         |
| Specific to C♯ | C♯ 3.0    | Anonymous type         | ✅        | ❌         |
|                | C♯ 7.0    | *Value tuple*          | ❌        | ✅         |
|                | C♯ 9.0    | `record (class)`       | ✅        | ❌         |
|                | C♯ 10.0   | `record struct`        | ❌        | ✅         |
| Specific to F♯ |           | *Tuple, Record, Union* | *Opt-in* | *Opt-out* |
|                | F♯ 4.6    | Anonymous *Record*     | *Opt-in* | *Opt-out* |

---

# Composite types (2)

Can be generic (except `enum`)

Location:

- *Top-level* : `namespace`, top-level `module` F♯
- *Nested* : `class` (C♯), `module` (F♯)
- Not definable in method (C♯) or simple value / function (F♯)!

In F♯ all type definitions are made with keyword `type`
→ including classes, enums and interfaces!
→ but tuples don't need a type definition

---

# Particularity of F♯ types / .NET types

*Tuple, Record, Union* are :

- Immutable
- Non-nullable
- Equality and structural comparison *(except with fields of `function` type)*
- `sealed`: cannot be inherited
- Deconstruction, with same syntax as construction 📍

Reflects different approaches depending on paradigm:

- FP: focus on data organized into types
- OOP: focus on behaviors, possibly polymorphic

---

# Types with literal values

Literal values = instances whose type is inferred

- Primitive types: `true` (`bool`) - `"abc"` (`string`) - `1.0m` (`decimal`)
- Tuples C♯ / F♯ : `(1, true)`
- Anonymous types C♯ : `new { Name = "Joe", Age = 18 }`
- Records F♯ : `{ Name = "Joe"; Age = 18 }`

☝ **Note :**

- Types must be defined beforehand ❗
- Exception: tuples and C♯ anonymous types: implicit definition

---

# Algebraic data types (ADT)

> Composite types, combining other types by *product* or *sum.*

Let's take the types `A` and `B`, then we can create:

- The product type `A × B` :
  - Contains 1 component of type `A` AND 1 of type `B`.
  - Anonymous or named components
- Sum type `A + B` :
  - Contains 1 component of type `A` OR 1 of type `B`.

By extension, same for the product/sum types of N types.

---

## Why *Sum* and *Product* terms?

It's related to the *number of values*:

- `bool` → 2 values: `true` and `false`
- `unit` → 1 value `()`
- `int` → infinite number of values

The number of values in the composed type will be:

- The sum of numbers for a *sum type*: `N(A) + N(B)`
- The product of numbers for a *product type*: `N(A) * N(B)`

---

# Algebraic types *vs* Composite types

| Type                             | Sum   | Product |
|----------------------------------|-------|---------|
| `enum`                           | ✅     | ❌       |
| *Union* F♯                       | ✅     | ❌       |
| `class` ⭐, `interface`, `struct` | ❌     | ✅       |
| *Record* F♯                      | ❌     | ✅       |
| *Tuple* F♯                       | ❌     | ✅       |

⭐ C♯ classes, including variations : type anonyme, *Value tuple* and `record`

👉 In C♯, no sum type except `enum`, very limited / union type 📍

---

# Type abbreviation

**Alias** of another type: `type [name] = [existingType]`

Different uses:

```fs
// Document code to avoid repetition
type ComplexNumber = float * float
type Addition<'num> = 'num -> 'num -> 'num // 👈 Also works with generics

// Decouple (partially) usage / implementation
// → Easier to change the implementation (for a stronger type)
type ProductCode = string
type CustomerId = int
```

⚠️ Deleted at compile time → ~~*type safety*~~
→ Compiler allows `int` to be passed instead of `CustomerId` !

---

<!-- _class: chapter invert -->

![bg-right h:300](../themes/d-edge/pictos/SOAT_pictos_produit.png)

# 2.

## Tuple Type

---

# Tuples: key points

- Types with literal values
- Anonymous types, but aliases can be defined to give them a name
- Product types by definition
  - Hence the `*` sign in the type signature: `A * B`
- Number of elements in the tuples:
  - 👌 2 or 3 (`A * B * C`)
  - ⚠️ \> 3 : possible but prefer *Record*
- Order of elements is important
  - If `A` ≠ `B`, then `A * B` ≠ `B * A`

---

# Tuples: construction

Syntax of literals: `a,b` or `a, b` or `(a, b)`

- Comma `,`: symbol dedicated to tuples in F#
- Spaces ` ` are optional
- Parentheses `()` may be necessary

**⚠️ Pitfall:** the symbol used is different for literals *vs* types

- `,` for literal
- `*` for signature
- E.g. `true, 1.2` → `bool * float`

---

# Tuples: deconstruction

- Same syntax as construction 👍
- All elements must appear in the deconstruction ⚠️
  - Use discard `_` to ignore one of the elements

```fs
let point = 1.0, 2.5
let x, y = point

let x, y = 1, 2, 3 // 💥 Error FS0001: Type incompatibility...
                   // ... Tuples have lengths other than 2 and 3

let result = System.Int32.TryParse("123") // (bool * int)
let _, value = result // Ignore the "bool".
```

---

# Tuples in practice

![bg-right h:300](../themes/d-edge/pictos/SOAT_pictos_idee.png)

Use a tuple for a data structure:

- Small: 2 to 3 elements
- Light: no need for element names
- Local: local data exchange not relevant to the whole *codebase*
  - Return multiple values - see `Int32.TryParse: bool * int`

Immutable tuple:
→ modifications are made by creating a new tuple

```fs
let addOneToTuple (x, y, z) = x + 1, y + 1, z + 1
```

---

# Tuples in practice (2)

**Structural equality**, but only between 2 tuples of the same signature!

```fs
(1,2) = (1,2)       // true
(1,2) = (0,0)       // false
(1,2) = (1,2,3)     // 💥 Error FS0001: Type incompatibility...
                    // ... Tuples have lengths other than 2 and 3
(1,2) = (1,(2,3))   // 💥 Error FS0001: This expression was supposed to have type `int`...
                    // ... but here it has type `'a * 'b`
```

**Nesting** of tuples using `()`

```fs
let doublet = (true,1), (false, "a")  // (bool * int) * (bool * string) → pair of pairs
let quadruplet = true, 1, false, "a"  // bool * int * bool * string → quadruplet
doublet = quadruplet                  // 💥 Error FS0001: Type incompatibility...
```

---

# Tuples : pattern matching

Patterns recognized with tuples :

```fs
let print move =
    match move with
    | 0, 0 -> "No move"                     // Constant 0
    | 0, y -> $"Vertical {y}"               // Variable y (!= 0)
    | x, 0 -> $"Horizontal {x}"
    | x, y when x = y -> $"Diagonal {x}"    // Condition x and y equal
                                            // `x, x` is not a recognized pattern ❗
    | x, y -> $"Other ({x}, {y})"
```

☝ **Notes :**

- Patterns are ordered from specific to generic
- The last pattern `x, y` is the default one to deconstruct a tuple

---

# Pairs

- 2-element tuples
- So common that 2 helpers are associated with them:
  - `fst` as *first* to extract the 1st element of the pair
  - `snd` as *second* to extract the 2nd element of the pair
  - ⚠️ Only works for pairs

```fs
let pair = 'a', "b"
fst pair  // 'a' (char)
snd pair  // "b" (string)
```

---

![bg-right h:300](../themes/d-edge/pictos/SOAT_pictos_question.png)

# Pair Quiz 🕹️

#### **1.** How do you implement `fst` and `snd` yourself?

```fs
let fst ... ?
let snd ... ?
```

#### **2.** What is the signature of this function?

```fs
let toList (x, y) = [x; y]
```

---

![bg-right h:300](../themes/d-edge/pictos/SOAT_pictos_solution.png)

# Pair Quiz 🎲

#### **1.** How do you implement `fst` and `snd` yourself?

```fs
let inline fst (x, _) = x  // Signature : 'a * 'b -> 'a
let inline snd (_, y) = y  // Signature : 'a * 'b -> 'b
```

- Tuple deconstruction: `(x, y)`
- We *discard* one element using `_` wildcard
- Functions can be `inline`

---

![bg-right h:300](../themes/d-edge/pictos/SOAT_pictos_solution.png)

# Pair Quiz 🎲🎲

#### **2.** Signature of `toList` ?

```fs
let inline toList (x, y) = [x; y]
```

- Returns a list with the 2 elements of the pair
- The elements are therefore of the same type
- There is no constraint on this type → generic `'a`

**Answer :** `x: 'a * y: 'a -> 'a list`

---

<!-- _class: chapter invert -->

![bg-right h:300](../themes/d-edge/pictos/SOAT_pictos_note.png)

# 3.

## Record Type

---

# Record: key points

> Product type with named elements called *fields.*

Alternative to tuples when they are imprecise, for instance `float * float`:

- Point? Coordinates? Vector?
- Real and imaginary parts of a complex number?

Eleviate the doubt by naming both the type and its elements:

```fs
type Point = { X: float; Y: float }
type Coordinate = { Latitude: float; Longitude: float }
type ComplexNumber = { Real: float; Imaginary: float }
```

---

# Record: declaration

Base syntax:

```fs
type RecordName =
    { Label1: type1
      Label2: type2
      ... }
```

☝️ Field labels in PascalCase, not ~~camelCase~~ → see [MS style guide](https://docs.microsoft.com/en-us/dotnet/fsharp/style-guide/formatting#use-pascalcase-for-type-declarations-members-and-labels)

Complete syntax:

```fs
[ attributes ]                                // [<Struct>]
type [accessibility-modifier] RecordName =    // private, internal
    { [ mutable ] Label1: type1
      [ mutable ] Label2: type2
      ... }
    [ member-list ]                           // Properties, methods...
```

---

# Record declaration: formatting styles

- Single-line: properties separated by `;`
- Multi-line: properties separated by line breaks
  - 3 variations: *Cramped, Aligned, Stroustrup*

```fs
// Single line
type PostalAddress = { Address: string; City: string; Zip: string }

// Cramped: historical ┆  // Aligned: C#-like      ┆  // Stroustrup: C++-like
type PostalAddress =   ┆  type PostalAddress =     ┆  type PostalAddress = {
    { Address: string  ┆      {                    ┆      Address: string
      City: string     ┆          Address: string  ┆      City: string
      Zip: string }    ┆          City: string     ┆      Zip: string
                       ┆          Zip: string      ┆  }
                       ┆      }
```

---

# Record: styles comparison

| Criterion | Best styles 🏆 |
|-----------|-------------|
| Compactness |  Single-line, Cramped |
| Refacto Easiness <br/> *(re)indentation, fields (re)ordering* | Aligned, Stroustrup |

☝️ **Recommendation:** *Strive for Consistency*
→ Apply consistently the same multi-line style across a repository
→ In addition, use the single-line style when relevant: line with < 80 chars

---

# Record: styles configuration

Fantomas configuration in the `.editorconfig` file:

```ini
max_line_length = 180
fsharp_multiline_bracket_style = cramped | aligned | stroustrup

fsharp_record_multiline_formatter = number_of_items
fsharp_max_record_number_of_items = 3
# or
fsharp_record_multiline_formatter = character_width
fsharp_max_record_width = 120
```

🔗 https://fsprojects.github.io/fantomas/docs/end-users/Configuration.html#fsharp_record_multiline_formatter

---

# Record members declaration styles

👉 Members are declared after the fields

#### Single-line

```fs
// `with` keyword required
type PostalAddress = { Address: string; City: string; Zip: string } with
    member x.ZipAndCity = $"{x.Zip} {x.City}"

// Or use line breaks (recommended when >= 2 members)
type PostalAddress =
    { Address: string; City: string; Zip: string }

    member x.ZipAndCity = $"{x.Zip} {x.City}"
    member x.CityAndZip = $"%s{x.City}, %s{x.Zip}"
```

---

# Record member declaration styles (2)

#### Multi-line: *Cramped* and *Aligned*

☝️ 2 line breaks

```fs
type PostalAddress =
    { Address: string
      City: string
      Zip: string }

    member x.ZipAndCity = $"{x.Zip} {x.City}"
    member x.CityAndZip = $"%s{x.City}, %s{x.Zip}"
```

---

# Record member declaration styles (3)

#### Multi-line: *Stroustrup*

☝️ `with` keyword + 1 line break + indentation

```fs
type PostalAddress = {
    Address: string
    City: string
    Zip: string
} with
    member x.ZipAndCity = $"{x.Zip} {x.City}"
    member x.CityAndZip = $"%s{x.City}, %s{x.Zip}"
```

---

# Record expression for instanciation

- Same syntax as an anonymous C♯ object without the `new` keyword
- All fields must be populated, but in any order (but can be confusing)
- Same possible styles: single/multi-lines

```fs
type Point = { X: float; Y: float }
let point1 = { X = 1.0; Y = 2.0 }
let pointKo = { Y = 2.0 }           // 💥 Error FS0764
//            ~~~~~~~~~~~ FS0764: No assignment given for field 'X' of type 'Point'
```

⚠️ **Trap:** differences declaration / instanciation
     → `:` for field type in record declaration
     → `=` for field value in record expression

---

# Record deconstruction

- Fields are accessible by "dotting" into the object
- Alternative: deconstruction
  - Same syntax for deconstructing a *Record* as for instantiating it 👍
  - Unused fields can be ignored 💡

```fs
let { X = x1 } = point1
let { X = x2; Y = y2 } = point1
```

---

# Record deconstruction (2)

⚠️ Additional members *(properties)* cannot be deconstructed!

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
//    ~~~~~~~~ The record label 'CityLine' is not defined
let cityLine = address.CityLine         // 👌 OK
```

---

# Record: inference

- A record type can be inferred from the fields used 👍 but not with members ❗
- As soon as the type is inferred, IntelliSense will work

```fs
type PostalAddress =
    { Address: string
      City: string
      Zip: string }

let department address =
    address.Zip.Substring(0, 2) |> int
    //     ^^^^ 💡 Infer that address is of type `PostalAddress`.

let departmentKo zip =
    zip.Substring(0, 2) |> int
//  ~~~~~~~~~~~~~ Error FS0072: Lookup on object of indeterminate type
```

---

# Record: pattern matching

Let's use an example: `inhabitantOf` is a function giving the inhabitants name *(in French)* at a given address *(in France)*

```fs
type Address = { Street: string; City: string; Zip: string }

let department { Zip = zip } = int zip[0..1] // Address -> int

let private IleDeFrance = Set [ 75; 77; 78; 91; 92; 93; 94; 95 ]
let inIleDeFrance departmentNum = IleDeFrance.Contains(departmentNum) // int -> bool

let inhabitantOf address = // Address -> string
    match address with
    | { Street = "Pôle"; City = "Nord" } -> "Père Noël"
    | { City = "Paris" } -> "Parisien"
    | _ when department address = 78 -> "Yvelinois"
    | _ when department address |> inIleDeFrance -> "Francilien"
    | _ -> "Français"
```

---

# Record: name conflict

In F♯, typing is nominal, not structural as in TypeScript
→ Use qualification to resolve ambiguity
→ Even better: write ≠ types or put them in ≠ modules

```fs
type Person1 = { First: string; Last: string }
type Person2 = { First: string; Last: string }
let alice = { First = "Alice"; Last = "Jones"}  // val alice: Person2... (by proximity)

// ⚠️ Deconstruction
let { First = firstName } = alice   // Warning FS0667 (in F# 6)
//  ~~~~~~~~~~~~~~~~~~~~~  The labels of this record do not uniquely
//                         determine a corresponding record type

let { Person2.Last = lastName } = alice     // 👌 OK with qualification
let { Person1.Last = lastName } = alice     // 💥 Error FS0001
//                                ~~~~~ Type 'Person1' expected, 'Person2' given
```

---

# Record: modification

Record is immutable, but easy to get a modified copy
→ **copy and update** expression of  of a *Record*

```fs
// Single-line
let address2 = { address with Street = "Rue Vivienne" }

// Multi-line
let address3 =
    { address with
        City = "Lyon"
        Zip  = "69001" }
```

---

# Record *copy-update*: C♯ / F♯ / JS

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

# Record *copy-update* : limits (< F# 8)

Reduced readability with several nested levels

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

# Record *copy-update* : F# 8 improvements

```fs
type Street = { Num: string; Label: string }
type Address = { Street: Street }
type Person = { Address: Address }

let person = { Address = { Street = { Num = "15"; Label = "rue Neuf" } } }

let person' =
    { person with
        Person.Address.Street.Num = person.Address.Street.Num + " bis" }
```

---

<!-- _class: chapter invert -->

![bg-right h:300](../themes/d-edge/pictos/SOAT_pictos_baseDonnees.png)

# 4.

## Union Type

---

# Unions: key points

- Exact term: *Discriminated Union (DU)*
- Sum Type: represents an **OR**, a **choice** between several *Cases*
  - Same principle as for an `enum`, but on steroids 💪
- Each *case* must have a *Tag* *(a.k.a Label, Discriminator)* -- in `PascalCase` ❗
- Each *case* **may** contain data
  - As Tuple: its elements can be named -- in camelCase 🙏

```fs
type Ticket =
    | Adult                  // no data -> ≃ singleton stateless
    | Senior of int          // holds an 'int' (w/o more precision)
    | Child of age: int      // holds an 'int' named 'age'
    | Family of Ticket list  // holds a list of tickets
                             // recursive type by default (no 'rec' keyword)
```

---

# Unions: declaration

On several lines: 1 line / *case*
→ ☝ Line indented and starting with a `|`

On a single (short) line
→ 💡 No need for the 1st `|`

```fs
open System

type IntOrBool =
    | Int32 of Int32                        // 💡 Tag with the same name as the data
    | Boolean of Boolean

type OrderId = OrderId of int               // 👈 Single-case union
                                            // 💡 Tag with the same name as the parent union

type Found<'T> = Found of 'T | NotFound     // 💡 Generic union type (no auto generalization)
```

---

# Unions: instanciation

*Tag* ≃ **constructor**
→ Function called with any *case* data

```fs
type Shape =
    | Circle of radius: int
    | Rectangle of width: int * height: int

let circle = Circle 12         // Type: 'Shape', Valeur: 'Circle 12'
let rect = Rectangle(4, 3)     // Type: 'Shape', Valeur: 'Rectangle (4, 3)'

let circles = [1..4] |> List.map Circle     // 👈 Tag used as function
```

---

# Unions: name conflict

When 2 unions have tags with the same name
→ Qualify the tag with the union name

```fs
type Shape =
    | Circle of radius: int
    | Rectangle of width: int * height: int

type Draw = Line | Circle    // 'Circle' will conflict with the 'Shape' tag

let draw = Circle            // Type='Draw' (closest type) -- ⚠️ to be avoided as ambiguous

// Tags qualified by their union type
let shape = Shape.Circle 12
let draw' = Draw.Circle
```

---

# Unions: get the data

Only via *pattern matching*.
Matching a Union type is **exhaustive**.

```fs
type Shape =
    | Circle of radius: float
    | Rectangle of width: float * height: float

let area shape =
    match shape with
    | Circle r -> Math.PI * r * r   // 💡 Same syntax as instantiation
    | Rectangle (w, h) -> w * h

let isFlat = function
    | Circle 0.
    | Rectangle (0., _)
    | Rectangle (_, 0.) -> true
    | Circle _
    | Rectangle _ -> false
```

---

# Unions: *single-case*

Union with a single case encapsulating a type (usually primitive)

```fs
type CustomerId = CustomerId of int
type OrderId = OrderId of int

let fetchOrder (OrderId orderId) =    // 💡 Direct deconstruction without 'match' expression
    ...
```

- **Benefits** 👍
  - Ensures *type safety* unlike simple type alias <br/>      → Impossible to pass a `CustomerId` to a function waiting for an `OrderId`
  - Prevents *Primitive Obsession* at a minimal cost
- **Trap** ⚠️
  - `OrderId orderId` looks like C# parameter definition

---

# Unions: "enum" style

All *cases* are empty = devoid of data
→ ≠ `enum` .NET 📍

Instantiation and pattern matching are done just with the *tag*.
→ The *tag* is no longer a ~~function~~ but a *singleton* value.

```fs
type Answer = Yes | No | Maybe
let answer = Yes

let print answer =
    match answer with
    | Yes   -> printfn "Oui"
    | No    -> printfn "Non"
    | Maybe -> printfn "Peut-être"
```

🔗 [“Enum” style unions | F# for fun and profit](https://fsharpforfunandprofit.com/posts/fsharp-decompiled/#enum-style-unions)

---

<!-- _class: chapter invert -->

![bg-right h:300](../themes/d-edge/pictos/SOAT_pictos_innovation.png)

# 5.

## Enum Type

Real .NET `enum`

---

# Enum: declaration

- Set of integer constants (`byte`, `int`...)
- Syntax ≠ union

```fs
type Color  = Red | Green | Blue        // Union
type ColorN = Red=1 | Green=2 | Blue=3  // Enum

type AnswerChar = Yes='Y' | No='N'  // 💡 enum based on 'char' but not on string:
type AnswerChar = Yes="Y" | No="N"  // 💥 Error FS0037
//   ~~~~~~~~~~ Duplicate definition of type, exception or module 'AnswerChar'

type File = a='a' | b='b' | c='c'  // 💡 enum members can be in camelCase
```

---

# Enum: usage

⚠️ Unlike unions, the use of an `enum` literal is necessarily qualified.

```fs
let answerKo = Yes            // 💥 Error FS0039
//             ~~~ The value or constructor 'Yes' is not defined.
let answer = AnswerChar.Yes   // 👌 OK
```

Conversion `int <-> enum`:

```fs
let redValue = int ColorN.Red         // enum -> int
let redAgain = enum<ColorN> redValue  // int -> enum
let red: ColorN = enum redValue       // int -> enum

// ⚠️ Does not work with char enum
let ko = char AnswerChar.No   // 💥 Error FS0001
let no: AnswerChar = enum 'N' // 💥 Error FS0001
```

---

# Enum: matching

⚠️ Unlike unions, *pattern matching* on enums is not exhaustive

```fs
type ColorN = Red=1 | Green=2 | Blue=3  // Enum

let toHex color =
    match color with
    | ColorN.Red   -> "FF0000"
    | ColorN.Green -> "00FF00"
    | ColorN.Blue  -> "0000FF"
    // ⚠️ Warning FS0104: Enums may take values outside known cases.
    // For example, the value 'enum<ColorN> (0)' may indicate a case not covered by the pattern(s).F# Compiler104

    // 💡 To remove the warning, add a last discard pattern
    | _ -> invalidArg (nameof color) $"Color {color} not supported"
```

---

# Enum: flags

Same principle as in C♯:

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

💡 Note the `|||` operator: binary OR *(`|` in C♯)*.

---

# Enum *vs* Union

| Type  | Data inside | Qualification | Exhaustivity |
|-------|-------------|---------------|--------------|
| Enum  | integers    | Required      | ❌ No        |
| Union | any         | Optional      | ✅ Yes       |

☝ **Recommendation :**

- Prefer Union over Enum in most cases
- Choose an Enum for :
  - .NET Interop
  - int data

---

<!-- _class: chapter invert -->

![bg-right h:300](../themes/d-edge/pictos/SOAT_pictos_rechercheProfil.png)

# 6.

## Anonymous Record

---

# Anonymous Record

- Since F♯ 4.6 *(March 2019)*
- Syntax: same as *Record* with "fat" braces `{| fields |}`
  - `{| Age: int |}` → signature
  - `{| Age = 15 |}` → instance
- Inline typing: no need to pre-define a named `type`
  - Alternative to *Tuples*
- Allowed in function input/output
  - ≠ Anonymous type C♯

---

# Anonymous Record : benefits ✅

• Reduce *boilerplate*
• Improve interop with external systems (JavaScript, SQL...)

Examples *(more on this later)* :

- LINQ projection
- Customization of an existing record
- JSON serialization
- Inline signature
- Alias by module

---

## ✅ LINQ Projection

💡 Select a subset of properties

```fs
let names =
    query {
        for p in persons do
        select {| Name = p.FirstName |}
    }
```

In C♯, we would use an anonymous type:

```cs
var names =
    from p in persons
    select new { Name = p.FirstName };
```

🔗 [F# vs C#: Anonymous Records](https://queil.net/2019/10/fsharp-vs-csharp-anonymous-records/) by Krzysztof Kraszewski

---

## ✅ Customize an existing record

💡 An anonymous record can be instantiated from a record instance

```fs
type Person = { Age: int; Name: string }
let william = { Age = 12; Name = "William" }

// Add a field (Gender)
let william' = {| william with Gender = "Male" |}
            // {| Age = 12; Name = "William"; Gender = "Male" |}

// Modify fields (Name, Age: int => float)
let jack = {| william' with Name = "Jack"; Age = 16.5 |}
        // {| Age = 16.5; Name = "Jack"; Gender = "Male" |}
```

---

## ✅ JSON serialization

😕 Unions can be serialized in an impractical format

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

## ✅ JSON serialization (2)

💡 Define an anonymous record to serialize a *customer*

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

💡 Use an anonymous *inline* record to reduce cognitive load

```fs
type Title = Mr | Mrs
type Customer =
    { Age  : int
      Name : {| First: string; Middle: string option; Last: string |} // 👈
      Title: Title option }
```

---

## ✅ Alias per module

```fs
module Api =
    type Customer = // ☝ Customer is an alias
        {| Id   : System.Guid
           Name : string
           Age  : int |}

module Dto =
    type Customer =
        {| Id   : System.Guid
           Name : string
           Age  : int |}

let (customerApi: Api.Customer) = {| Id = Guid.Empty; Name = "Name"; Age = 34 |}
let (customerDto: Dto.Customer) = customerApi // 🎉 No need to convert
```

💡 Now: same type in 2 modules
💡 Later: easy customization of types in each module

---

# Anonymous Record: Limits 🛑

```fs
// Limited inference
let nameKo x = x.Name  // 💥 Error FS0072: Lookup on object of indeterminate type...
let nameOk (x: {| Name:string |}) = x.Name

// No deconstruction
let x = {| Age = 42 |}
let {  Age = age  } = x  // 💥 Error FS0039: The record label 'Age' is not defined
let {| Age = age |} = x  // 💥 Error FS0010: Unexpected symbol '{|' in let binding

// No full objects merge
let banana = {| Fruit = "Banana" |}
let yellow = {| Color = "Yellow" |}
let banYelKo = {| banana with yellow |} // 💥 Error FS0609...
let banYelOk = {| banana with Color = "Yellow" |}

// No omissions
let ko = {| banYelOk without Color |}  // 💥 No 'without' keyword
```

---

# Anonymous Record: Limits (2) 🛑

```fs
// Not structural typing => all fields are required
let capitaliseFruit (x: {| Fruit: string |}) = x.Fruit.ToUpper()
capitaliseFruit {| Fruit = "Banana" |}                      // 👌 "BANANA"
capitaliseFruit {| Fruit = "Banana"; Origin = "Réunion" |}  // 💥 Too much fields... [Origin]
```

Does not cross project (`.fsproj`) boundaries ❗

---

<!-- _class: chapter invert -->

![bg-right h:300](../themes/d-edge/pictos/SOAT_pictos_entretien.png)

# 7.

## Value Types

---

# Struct Tuple/Record/Union

- Regular tuple/record/union are reference-types
- Possible to get them as value-types
  - Instances stored on the *Stack* rather than in the *Heap*
  - Tuple: `struct` keyword
  - Record/Union: `[<Struct>]` attribute

```fs
// Tuple
let a = struct (1, 'b', "trois") // struct (int * char * string)

// Record
[<Struct>]
type Point = { X: float; Y: float }

let p = { X = 1.0; Y = 2.3 } // Point
```

---

# Struct: recommendations

⚖️ **Pros/Cons:**

- ✅ Efficient because no *garbage collection*
- ⚠️ Passed by value → memory pressure

🔗 [F# coding conventions / Performance](https://learn.microsoft.com/en-us/dotnet/fsharp/style-guide/conventions#performance)

> Consider structs for small types with high allocation rates

---

<!-- _class: chapter invert -->

![bg-right h:300](../themes/d-edge/pictos/SOAT_pictos_diplome.png)

# 8.

## Wrap up 🌯

---

<!-- _footer: '' -->
![bg-right h:300](../themes/d-edge/pictos/SOAT_pictos_question.png)

# 🕹️ Quiz wrap up

```fs
// Match types with concepts (1 to many)
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

# 🎲 Quiz wrap up

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

# Types Composition

Creating new types?
→ ❌ Algebraic data types do not support inheritance
→ ✅ By composition, in *sum/product types*
→ 💡 Extension of a *Record* into an anonymous *Record* with more fields

Combine 2 unions?
→ ❌ Not "flattenable" as in TypeScript ①
→ ✅ New parent union type ②

```fs
// French-suited cards
type Noir = Pique | Trefle // pikes or spades ♠ | clovers or clubs ♣
type Rouge = Coeur | Carreau // hearts ♥ | tiles or diamonds ♦
type CouleurKo = Noir | Rouge  // (1) ⚠️ not the expected union of Pique | Trefle | Coeur | Carreau
type Couleur = Noir of Noir | Rouge of Rouge // (2) ✅
let c1 = Noir Pique // Couleur
```

---

![bg-right h:300](../themes/d-edge/pictos/SOAT_pictos_cocktail.png)

# Conclusion

Lots of ways to model!

💡 Favor:

- Team discussions
- Business domain encoding in types

---

<!-- _class: end invert lead-->

# Thanks 🙏
