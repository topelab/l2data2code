# L2Data2Code tools

Have an idea of data model and needs to generate some code for that idea?

Here you will find some utilities to mix a data source with some templates and get generated files.

I started with a UI tool that read a scheme from a database and merge with some files having an special format (mustache templates) getting code generated.

Now, you have some tools:

- UI tool called [L2Data2CodeWPF](src/L2Data2Code/L2Data2CodeWPF/README.md) that can read a data source (database, special JSON file or an .NET assembly) and merge with templates to get result. 
- Command line tool [HandleBarsCLI](src/L2Data2Code/HandleBarsCLI/readme.md) (based on [HandleBars](https://HandleBars.github.io/)), to execute a transformation using a JSON file for every path and file on a directory tree.
- [Scheme2Json](src/L2Data2Code/Scheme2Json/readme.md), a command line tool to generate a special JSON file from an database scheme. L2Data2CodeWPF understand this special JSON file as a data source.
- **Renewed UI Tool** [L2Data2Code](src/L2Data2Code/L2Data2Code/README.md) made using AvaloniaUI that will be the next version of L2Data2CodeWPF, and possibly, a replacement for it.


## .Net MAUI resources I used

- [Custom Fonts & Material Design Icons in .NET MAUI](https://cedricgabrang.medium.com/custom-fonts-material-design-icons-in-net-maui-acf59c9f98fe)
- [Material Design Icons web font](https://github.com/Templarian/MaterialDesign-Webfont/blob/master/fonts/materialdesignicons-webfont.ttf)
- [Material Design Icons home page](https://pictogrammers.com/library/mdi/)
