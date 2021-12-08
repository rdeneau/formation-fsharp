# Formation F\#

Support de ma formation de 3 jours sur F\# (actuellement la version 5.0 avec quelques éléments de la 6.0).

## Commandes

Utilisation de `marp` pour convertir les slides markdown en HTML ou PDF.

### Markdown vers HTML

Une session en mode *watch :*

```bash
marp masterclass-fsharp-01-bases.md --watch --theme themes/soat.css
```

Toutes les sessions :

```bash
marp masterclass-fsharp-01-bases.md --theme themes/soat.css
marp masterclass-fsharp-02-fonctions.md --theme themes/soat.css
marp masterclass-fsharp-03-types.md --theme themes/soat.css
marp masterclass-fsharp-04-types-comp.md --theme themes/soat.css
marp masterclass-fsharp-05-pattern-matching.md --theme themes/soat.css
marp masterclass-fsharp-06-collections.md --theme themes/soat.css
marp masterclass-fsharp-07-async.md --theme themes/soat.css
marp masterclass-fsharp-08-monad.md --theme themes/soat.css
marp masterclass-fsharp-09-module.md --theme themes/soat.css
marp masterclass-fsharp-10-oo.md --theme themes/soat.css
```

### Markdown vers PDF

Toutes les sessions :

```bash
marp masterclass-fsharp-01-bases.md --pdf --allow-local-files --theme themes/soat.css
marp masterclass-fsharp-02-fonctions.md --pdf --allow-local-files --theme themes/soat.css
marp masterclass-fsharp-03-types.md --pdf --allow-local-files --theme themes/soat.css
marp masterclass-fsharp-04-types-comp.md --pdf --allow-local-files --theme themes/soat.css
marp masterclass-fsharp-05-pattern-matching.md --pdf --allow-local-files --theme themes/soat.css
marp masterclass-fsharp-06-collections.md --pdf --allow-local-files --theme themes/soat.css
marp masterclass-fsharp-07-async.md --pdf --allow-local-files --theme themes/soat.css
marp masterclass-fsharp-08-monad.md --pdf --allow-local-files --theme themes/soat.css
marp masterclass-fsharp-09-module.md --pdf --allow-local-files --theme themes/soat.css
marp masterclass-fsharp-10-oo.md --pdf --allow-local-files --theme themes/soat.css
```
