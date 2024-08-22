![Logo](./img/peereflits-logo.png) 

# Contentful.ModelsGenerator.Cli
A dotnet CLI to automatically generate strongly typed C# models/DTO's from Contentful contenttypes.

**Note:** This project is a clone and refactor of the [Contentful.ModelsCreator.Cli](https://github.com/contentful/dotnet-models-creator-cli) that at the time of creating this project ran on .NET Core 2.1.

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
   - `-e` or `-environment`: specifies the environment that contains the content-types that need to be generated. Optional, defaults to `develop`.

* Code generation parameters:
   - `-ns` or `-namespace`: sets the namespace for the classes being created, Optional, defaults to `Contentful.Models`.
   - `-f` or `-force`: a flag that forces an overwrite of any existing files. Optional, default is `true`.
   - `-p` or `-path`: sets the output folder where the class files should be generated. Optional, defaults to a `Models`-folder under the current directory.
   - `-i` or `-internal` a flag to generate the classes with an internal access modifier. Optional, is default `true`. If omitted classes will be generated with a public access modifier.

### Examples
Running `contentful.modelsgenerator.cli -k YOUR_API_KEY -s YOUR_SPACE_ID -e YOUR_ENVIRONMENT` will create a number of classes in a `Models`-folder under your current working directory.

If you want to specify the namespace of the created classes use the `-ns` switch: `contentful.modelsgenerator.cli -k YOUR_API_KEY -s YOUR_SPACE_ID -e YOUR_ENVIRONMENT -ns MyProject.ASubNamespace.Models` 

If you want to specify the path to create the DTO's in use the `-p` switch: `contentful.modelsgenerator.cli -k YOUR_API_KEY -s YOUR_SPACE_ID -e YOUR_ENVIRONMENT -p c:\projects\MyProject\ASubNamespace\Models`

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