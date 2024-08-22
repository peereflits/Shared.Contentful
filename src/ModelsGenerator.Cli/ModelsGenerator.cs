﻿using System.ComponentModel.DataAnnotations;
using Contentful.Core.Models;
using McMaster.Extensions.CommandLineUtils;

namespace Peereflits.Shared.Contentful.ModelsGenerator.Cli;

[Command(Name = "Contentful.ModelsGenerator.Cli",
    FullName = "Contentful Models Generator CLI",
    Description = "Generates C# POCO classes from Contentful content types.")]
[HelpOption]
public class ModelsGenerator
{
    [Option(CommandOptionType.SingleValue, Description = "The Contentful API key for the Content Delivery API", ShortName = "k")]
    [Required(ErrorMessage = "You must specify the Contentful API key for the Content Delivery API")]
    public string ApiKey { get; set; } = null!;

    [Option(CommandOptionType.SingleValue, Description = "The Contentful space id to fetch content types from", ShortName = "s")]
    [Required(ErrorMessage = "You must specify the space id to fetch content types from.")]
    public string SpaceId { get; set; } = null!;

    [Option(CommandOptionType.SingleValue, Description = "The environment to fetch the content model types from", ShortName = "e")]
    public string Environment { get; set; } = "develop";

    [Option(CommandOptionType.SingleValue, Description = "The namespace the classes should be generated in", ShortName = "ns")]
    public string Namespace { get; set; } = "Contentful.Models";

    [Option(CommandOptionType.NoValue, Description = "Automatically overwrite files that already exist", ShortName = "f")]
    public bool Force { get; set; } = true;

    [Option(CommandOptionType.SingleValue, Description = "Path to the file or directory to create files in", ShortName = "p")]
    public string Path { get; set; } = "";

    [Option(CommandOptionType.NoValue, Description = "Generate the contacts as internal classes", ShortName = "i")]
    public bool Internal { get; set; } = true;

    [VersionOption("1.0.1")]
    public bool Version { get; }


    public async Task<int> OnExecute(CommandLineApplication app, IConsole console)
    {
        var reader = new ModelReader();
        var contentTypes = new List<ContentType>(await reader.Execute(ApiKey, SpaceId, Environment));

        var path = string.IsNullOrEmpty(Path)
            ? System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Models")
            : Path;
        console.WriteLine(string.IsNullOrWhiteSpace(Path)
            ? $"No path specified, creating files in current working directory {path}"
            : $"Path is specified. Files will be created at {path}");

        var dir = new DirectoryInfo(path);
        if (!dir.Exists)
        {
            console.WriteLine($"Path {path} does not exist.");
            dir.Create();
            console.WriteLine($"Path {path} is created.");
        }

        var writer = new ModelWriter(console, path, Force, Namespace, Internal);
        await writer.WriteModels(contentTypes);

        console.ForegroundColor = ConsoleColor.Green;
        console.WriteLine("Files successfully created!");
        console.ResetColor();

        return Program.Ok;
    }
}