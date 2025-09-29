# F♯ Training

Support for a 3-day training course on F♯

## Slides

Use `marp` ([link](https://marp.app)) to convert markdown slides to HTML or PDF.

### Markdown to HTML

All files:

```bash
marp fsharp-training-00-toc.md --output ./html/fsharp-training-00-toc.html --theme themes/d-edge.css
marp fsharp-training-01-bases.md --output ./html/fsharp-training-01-bases.html --theme themes/d-edge.css
marp fsharp-training-02-functions.md --output ./html/fsharp-training-02-functions.html --theme themes/d-edge.css
marp fsharp-training-03-types.md --output ./html/fsharp-training-03-types.html --theme themes/d-edge.css
marp fsharp-training-04-option-result.md --output ./html/fsharp-training-04-option-result.html --theme themes/d-edge.css
marp fsharp-training-05-pattern-matching.md --output ./html/fsharp-training-05-pattern-matching.html --theme themes/d-edge.css
marp fsharp-training-06-collections.md --output ./html/fsharp-training-06-collections.html --theme themes/d-edge.css
marp fsharp-training-07-async.md --output ./html/fsharp-training-07-async.html --theme themes/d-edge.css
marp fsharp-training-08-module.md --output ./html/fsharp-training-08-module.html --theme themes/d-edge.css
marp fsharp-training-09-oo.md --output ./html/fsharp-training-09-oo.html --theme themes/d-edge.css
marp fsharp-training-10-types-addendum.md --output ./html/fsharp-training-10-types-addendum.html --theme themes/d-edge.css
marp fsharp-training-11-functional-patterns.md --output ./html/fsharp-training-11-functional-patterns.html --theme themes/d-edge.css
marp fsharp-training-12-computation-expr.md --output ./html/fsharp-training-12-computation-expr.html --theme themes/d-edge.css
marp fsharp-training-13-domain-workflows.md --output ./html/fsharp-training-13-domain-workflows.html --theme themes/d-edge.css
```

*watch* mode: add `--watch`

```bash
marp fsharp-training-01-bases.md --output ./html/fsharp-training-01-bases.html --theme themes/d-edge.css --watch
```

### Markdown to PDF

All files:

```bash
marp fsharp-training-01-bases.md --output ./pdf/fsharp-training-01-bases.pdf --theme themes/d-edge.css --allow-local-files
marp fsharp-training-02-functions.md --output ./pdf/fsharp-training-02-functions.pdf --theme themes/d-edge.css --allow-local-files
marp fsharp-training-03-types.md --output ./pdf/fsharp-training-03-types.pdf --theme themes/d-edge.css --allow-local-files
marp fsharp-training-04-option-result.md --output ./pdf/fsharp-training-04-option-result.pdf --theme themes/d-edge.css --allow-local-files
marp fsharp-training-05-pattern-matching.md --output ./pdf/fsharp-training-05-pattern-matching.pdf --theme themes/d-edge.css --allow-local-files
marp fsharp-training-06-collections.md --output ./pdf/fsharp-training-06-collections.pdf --theme themes/d-edge.css --allow-local-files
marp fsharp-training-07-async.md --output ./pdf/fsharp-training-07-async.pdf --theme themes/d-edge.css --allow-local-files
marp fsharp-training-08-module.md --output ./pdf/fsharp-training-08-module.pdf --theme themes/d-edge.css --allow-local-files
marp fsharp-training-09-oo.md --output ./pdf/fsharp-training-09-oo.pdf --theme themes/d-edge.css --allow-local-files
marp fsharp-training-10-types-addendum.md --output ./pdf/fsharp-training-10-types-addendum.pdf --theme themes/d-edge.css --allow-local-files
marp fsharp-training-11-functional-patterns.md --output ./pdf/fsharp-training-11-functional-patterns.pdf --theme themes/d-edge.css --allow-local-files
marp fsharp-training-12-computation-expr.md --output ./pdf/fsharp-training-12-computation-expr.pdf --theme themes/d-edge.css --allow-local-files
marp fsharp-training-13-domain-workflows.md --output ./pdf/fsharp-training-13-domain-workflows.pdf --theme themes/d-edge.css --allow-local-files
```

In case of `Some of the local files are missing and will be ignored. Make sure the file paths are correct.` warning, we can:

- open the HTML in a browser and Print > Save as PDF...
- edit both markdown file and `.\en\themes\d-edge.css`: replace the relative path `../themes` to the absolute path, e.g. `C:/Dev/_github/rdeneau/formation-fsharp/en/themes`.

## Useful Characters

- ① ② ③
- Ⓐ Ⓑ Ⓒ
- → • · À É ♯
- ≃ ≠ (different) ≡ (equivalent to)
- ∀ ∃ ∈ ⊂ ×
- ∅ ∑ ∞ ▷ (pipe operator in markdown table)
- ellipsis … ⋯ ︙
- dash: – (en dash: between hyphen and minus sign) — (em dash ≃ double en dash) ― (horizontal bar) ─ (box drawings light horizontal) \
  〜 (wave dash) 〰 (wavy dash)
- bar: ┆｜
- space: ' ' (thin) ' ' (non breakable)
- quotes: ❝ ❞

## Speaker tips

VSCode:

| Feature         | Command                                                |
|-----------------|--------------------------------------------------------|
| Full screen     | <kbd>F11</kbd>                                         |
| Zoom            | <kbd>Ctrl</kbd>+<kbd>KeyPad+/-</kbd>                   |
| Show key stroke | <kbd>F1</kbd> / Screencast mode                        |
| Show terminal   | <kbd>Ctrl</kbd>+<kbd>ù</kbd>                           |
| Clear terminal  | <kbd>Ctrl</kbd>+<kbd>L</kbd>                           |
| Move terminal   | `Right click` on the *Terminal tab* / Move panel right |
