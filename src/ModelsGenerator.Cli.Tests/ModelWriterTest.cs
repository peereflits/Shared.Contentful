using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Contentful.Core.Models;
using McMaster.Extensions.CommandLineUtils;
using NSubstitute;
using Xunit;
using File = System.IO.File;

namespace Peereflits.Shared.Contentful.ModelsGenerator.Cli.Tests;

public class ModelWriterTest
{
    private const string GeneratedContent = "=== This is generated content. ===";
    private const string ExistingContent = "This is existing content.";

    private readonly IConsole console;
    private readonly string outputDirectory;
    private readonly IGenerateType typeGenerator;

    public ModelWriterTest()
    {
        var reader = Substitute.For<TextReader>();
        reader
            .ReadLineAsync()
            .Returns("n");

        console = Substitute.For<IConsole>();
        console
            .Out
            .Returns(Substitute.For<TextWriter>());
        console
            .In
            .Returns(reader);


        outputDirectory = Path.Combine(Path.GetTempPath(), "ModelWriterTests");
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        typeGenerator = Substitute.For<IGenerateType>();
        typeGenerator
            .Generate(Arg.Any<ContentType>())
            .Returns(GeneratedContent);
    }

    [Fact]
    public async Task WhenWriteModels_ItShouldGenerateFiles()
    {
        var contentTypes = new List<ContentType>
        {
            new ContentType
            {
                SystemProperties = new SystemProperties { Id = "myOwnContentType" },
                Fields = []
            }
        };

        var subject = new ModelWriter(console, outputDirectory, true, typeGenerator);

        await subject.WriteModels(contentTypes);

        var generatedFile = Path.Combine(outputDirectory, "MyOwnContentType.g.cs");
        Assert.True(File.Exists(generatedFile));

        var content = await File.ReadAllTextAsync(generatedFile);
        Directory.Delete(outputDirectory, true);

        Assert.Contains(GeneratedContent, content);
    }

    [Fact]
    public async Task WhenWriteModels_WhileShouldNotOverwrite_ItShouldSkipExistingFiles()
    {
        var shouldOverWrite = false;

        var existingFile = Path.Combine(outputDirectory, "TestContentType.g.cs");
        await File.WriteAllTextAsync(existingFile, ExistingContent);

        var contentTypes = new List<ContentType>
        {
            new ContentType
            {
                SystemProperties = new SystemProperties { Id = "testContentType" },
                Fields = []
            }
        };

        var subject = new ModelWriter(console, outputDirectory, shouldOverWrite, typeGenerator);

        await subject.WriteModels(contentTypes);

        var content = await File.ReadAllTextAsync(existingFile);

        Directory.Delete(outputDirectory, true);

        Assert.Equal(ExistingContent, content);
    }

    [Fact]
    public async Task WhenWriteModels_WithShouldOverwrite_ItShouldOverwriteExistingFiles()
    {
        var shouldOverWrite = true;

        var existingFile = Path.Combine(outputDirectory, "TestContentType.g.cs");
        await File.WriteAllTextAsync(existingFile, ExistingContent);

        var contentTypes = new List<ContentType>
        {
            new ContentType
            {
                SystemProperties = new SystemProperties { Id = "testContentType" },
                Fields = []
            }
        };

        var subject = new ModelWriter(console, outputDirectory, shouldOverWrite, typeGenerator);

        await subject.WriteModels(contentTypes);

        var content = await File.ReadAllTextAsync(existingFile);

        Directory.Delete(outputDirectory, true);

        Assert.DoesNotContain(ExistingContent, content);
    }
}