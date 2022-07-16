# Mustache

Mustache (based on [mustache](https://mustache.github.io/)), is a command line utility to execute a transformation using a json file for every path and file on a directory tree.

We have this in `data.json`

```json
{
    "Dishes":[
        {
            "Dish": "Apple",
            "Type": "Postre"
        },
        {
            "Dish": "Melon",
            "Type": "Postre"
        },
        {
            "Dish": "Black rice",
            "Type": "First course"
        }
    ]
}
```

And these directory tree:

- templatedir
    - file1.md
    - file2.md
    - {{Type}}
        - {{Dish}}.md


The idea is that when you run:

```cmd
Mustache data.json templatedir outputdir Dishes
```

It will generate:

- outputdir
    - file1.md
    - file2.md
    - Dish
        - Apple.md
        - Melon.md
    - First course
        - Black rice.md

Inside files, can use almost all the [mustache](https://mustache.github.io/mustache.5.html) template language
