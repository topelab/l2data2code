# L2 Data2Code 

Have an idea of data model and needs to generate some code for that idea?

Here you will find some utilities to mix a data source with some templates and get generated files.

I started with a UI tool that read a scheme from a database and merge with some files having an special format (mustache templates) getting code generated.

Now, you have some tools:

- UI tool called [L2Data2CodeWPF](src/L2Data2Code/L2Data2CodeWPF/README.md) that can read a data source (database, special JSON file or an .NET assembly) and merge with templates to get result. 
- CLI tool [HandleBarsCLI](src/L2Data2Code/HundleBarsCLI/readme.md) (based on [HandleBars](https://HandleBars.github.io/)), tha is a command line utility to execute a transformation using a json file for every path and file on a directory tree.
- Schema2Json, a CL tool to generate a special JSON file from an database scheme. L2Data2CodeWPF understand this special JSON file as a data source.


