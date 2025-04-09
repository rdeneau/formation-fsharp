# F♯ Training

Support for a 3-day training course on F♯

## Commandes

Use `marp` ([link](https://marp.app)) to convert markdown slides to HTML or PDF.

### Markdown to HTML

*watch* mode:

```bash
marp fsharp-training-01-bases.md --output ./html/fsharp-training-01-bases.html --theme themes/d-edge.css --watch
```

All files:

```bash
marp fsharp-training-01-bases.md --output ./html/fsharp-training-01-bases.html --theme themes/d-edge.css
marp fsharp-training-02-functions.md --output ./html/fsharp-training-02-functions.html --theme themes/d-edge.css
marp fsharp-training-03-types.md --output ./html/fsharp-training-03-types.html --theme themes/d-edge.css
marp fsharp-training-04-types-addendum.md --output ./html/fsharp-training-04-types-addendum.html --theme themes/d-edge.css
marp fsharp-training-05-pattern-matching.md --output ./html/fsharp-training-05-pattern-matching.html --theme themes/d-edge.css
marp fsharp-training-06-collections.md --output ./html/fsharp-training-06-collections.html --theme themes/d-edge.css
marp fsharp-training-07-async.md --output ./html/fsharp-training-07-async.html --theme themes/d-edge.css
marp fsharp-training-08-monad.md --output ./html/fsharp-training-08-monad.html --theme themes/d-edge.css
marp fsharp-training-09-module.md --output ./html/fsharp-training-09-module.html --theme themes/d-edge.css
marp fsharp-training-10-oo.md --output ./html/fsharp-training-10-oo.html --theme themes/d-edge.css
```

### Markdown to PDF

All files:

```bash
marp fsharp-training-01-bases.md --pdf --allow-local-files --theme themes/d-edge.css
marp fsharp-training-02-functions.md --pdf --allow-local-files --theme themes/d-edge.css
marp fsharp-training-03-types.md --pdf --allow-local-files --theme themes/d-edge.css
marp fsharp-training-04-types-addendum.md --pdf --allow-local-files --theme themes/d-edge.css
marp fsharp-training-05-pattern-matching.md --pdf --allow-local-files --theme themes/d-edge.css
marp fsharp-training-06-collections.md --pdf --allow-local-files --theme themes/d-edge.css
marp fsharp-training-07-async.md --pdf --allow-local-files --theme themes/d-edge.css
marp fsharp-training-08-monad.md --pdf --allow-local-files --theme themes/d-edge.css
marp fsharp-training-09-module.md --pdf --allow-local-files --theme themes/d-edge.css
marp fsharp-training-10-oo.md --pdf --allow-local-files --theme themes/d-edge.css
```
