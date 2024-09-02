using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Contentful.Core.Models;
using McMaster.Extensions.CommandLineUtils;

namespace Peereflits.Shared.Contentful.ModelsGenerator.Cli;

internal class ModelWriter : ClassGeneratorBase
{
    private readonly IConsole console;
    private readonly string outputDirectory;
    private readonly bool shouldOverWrite;
    private readonly IGenerateType typeGenerator;

    private readonly List<ContentType> contentTypes = [];

    public ModelWriter
    (
        IConsole console,
        string outputDirectory,
        bool shouldOverWrite,
        IGenerateType typeGenerator
    )
    {
        this.console = console;
        this.outputDirectory = outputDirectory;
        this.shouldOverWrite = shouldOverWrite;
        this.typeGenerator = typeGenerator;
    }

    public async Task WriteModels(IEnumerable<ContentType> contentfulTypes)
    {
        this.contentTypes.AddRange(contentfulTypes);

        foreach (var contentType in contentTypes)
        {
            var fileName = GetSafeFilename(FormatClassName(contentType.SystemProperties.Id));
            var path = Path.Combine(outputDirectory, $"{fileName}.g.cs");

            if (System.IO.File.Exists(path) && !shouldOverWrite)
            {
                console.ForegroundColor = ConsoleColor.Yellow;
                var msg = $"The file '{fileName}.g.cs' already exists. Do you want to overwrite it? [y/n]";
                console.WriteLine(msg);
                bool isYes = (await console.In.ReadLineAsync())?.ToLowerInvariant() == "y";
                console.ResetColor();

                if (!isYes)
                {
                    console.ForegroundColor = ConsoleColor.Red;
                    console.WriteLine($"Skipping {fileName}.g.cs");
                    console.ResetColor();
                    continue;
                }
            }

            console.WriteLine($"Generating file {fileName}.g.cs");
            await System.IO.File.WriteAllTextAsync(path, typeGenerator.Generate(contentType));
        }
    }

    private string GetSafeFilename(string filename)
        => string
            .Join("-", filename.Split(Path.GetInvalidFileNameChars()))
            .Replace(" ", string.Empty);
}