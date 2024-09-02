![Logo](./img/peereflits-logo.svg) 

# Contentful.ModelsGenerator.Cli
**A dotnet CLI to automatically generate strongly typed C# models/DTO's from [Contentful](https://www.contentful.com/) contenttypes.**

The generated classes are based on the content-types in your Contentful space/environment and are generated as .NET/C# partial records with properties that match the (required or optional) fields in the content-types. 
The generated files do have a dependency on `Contentful.Core.Models` in the `contentful.csharp`-[package](https://www.nuget.org/packages/contentful.csharp/).

This project is a clone and refactor of the [Contentful.ModelsCreator.Cli](https://github.com/contentful/dotnet-models-creator-cli) that at the time of creating this project ran on .NET Core 2.1. 
This repository is now a public archive. The `Contentful.ModelsGenerator.Cli` is a .NET 8 project and is intended to be used in .NET 8 projects. It fixes some issues in the original project and adds some new features like:
1. using records
1. making required/optional explicit
1. generate the types as partial
1. generate the types with an internal or public access modifier

## Prerequisites
The CLI tool uses the "global tools" feature of .NET 8 and requires the .NET 8 SDK to be installed: https://dotnet.microsoft.com/en-us/download/dotnet/8.0 .

Before installation the project must be published and packed (default to Nuget). Otherwise, run this command as a console app (Debug | Start without debugging).

## Installation
Run `dotnet tool install -g contentful.modelsgenerator.cli` from your command line.

## Usage
Once installed you should now be able to run `contentful.modelsgenerator.cli --help` to list all the available commands.

* Miscelenious parameters:
   - `-h` to list this help
   - `-v` to show the version of the tool installed

* Parameters for communicating with Contentful:
   - `-k` or `-apikey`: sets the Contentful (delivery) Api **K**ey. Required.
   - `-s` or `-spaceid`: sets the Contentful **S**pace id of your Contentful instance Required.
   - `-e` or `-environment`: specifies the environment that contains the content-types that need to be generated. Optional, defaults to `master`.

* Code generation parameters:
   - `-ns` or `-namespace`: sets the namespace for the classes being created, Optional, defaults to `Contentful.Models`.
   - `-f` or `-force`: a flag that forces an overwrite of any existing files. Optional, default is `true`.
   - `-p` or `-path`: sets the output folder where the class files should be generated. Optional, defaults to a `Models`-folder under the current directory.
   - `-i` or `-internal`: a flag to generate the classes with an internal access modifier. Optional, is default `false`. If omitted classes will be generated with a `public` access modifier. If `-i` is provided, an `internal` access modifier is used.

### Examples
Running `contentful.modelsgenerator.cli -k YOUR_API_KEY -s YOUR_SPACE_ID -e YOUR_ENVIRONMENT` will create a number of public classes (records) in a `Models`-folder under your current working directory.

If you want to specify the namespace of the created classes use the `-ns` switch: `contentful.modelsgenerator.cli -k YOUR_API_KEY -s YOUR_SPACE_ID -e YOUR_ENVIRONMENT -ns MyProject.ASubNamespace.Models` 

If you want to specify the path to create the DTO's in use the `-p` switch: `contentful.modelsgenerator.cli -k YOUR_API_KEY -s YOUR_SPACE_ID -e YOUR_ENVIRONMENT -p c:\\\\projects\\MyProject\\ASubNamespace\\Models`

If you want to run the tool direclty from within Visual Studio, add a `Properties\launchSettings.json`-file to the project with the following content:
```json
{
  "profiles": {
    "ModelsGenerator.Cli": {
      "commandName": "Project",
      "commandLineArgs": "-k MY_APIKEY -s MY_SPACE_ID -e MY_ENVIRONMENT"
    }
  }
}
```

## Using the generated code

The generated code contains, besides one file per content type, also a file called `GeneratedContentTypeResolver.g.cs`. 
For the Contentful client to be able to deserialize the content types, this file is needed. 
It also is advised to use the `ResolveEntriesSelectively`-option in the `ContentfulOptions`-object when creating the client.

See the code below:

``` csharp
public IContentfulClient BuildClient()
{
    var options = new ContentfulOptions
    {
        DeliveryApiKey = "MY API KEY",
        SpaceId = "MY SPACE ID",
        Environment = "My ENVIRONMENT",
        UsePreviewApi = false,
        ResolveEntriesSelectively = true,
    };

    return new ContentfulClient(factory.CreateClient(nameof(ContentfulClientBuilder)), options)
    {
        ContentTypeResolver = new GeneratedContentTypeResolver()
    };
}

```

> **Note:** A big thank you to @kheurterincentro for explaining some of the hidden SDK internals and providing the initial implementation of the `ContentTypeResolverWriter`.

An implementation example can be found [here](./src/Contentful.Implementation/ReadMe.md).
