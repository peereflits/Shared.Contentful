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
    private readonly IConsole console;
    private readonly string outputDirectory;
    private readonly string @namespace;
    private readonly bool isInternal;

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
        @namespace = "TestNamespace";
        isInternal = true;

        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }
    }

    [Fact]
    public async Task WhenWriteModels_ItShouldGenerateFiles()
    {
        var contentTypes = new List<ContentType>
        {
            new ContentType
            {
                SystemProperties = new SystemProperties { Id = "myOwnContentType" },
                Fields =
                [
                    new Field { Id = "field1", Type = "Text", Required = true },
                    new Field { Id = "field2", Type = "Integer", Required = false }
                ]
            }
        };

        var subject = new ModelWriter(console, outputDirectory, true, @namespace, isInternal);

        await subject.WriteModels(contentTypes);

        var generatedFile = Path.Combine(outputDirectory, "MyOwnContentType.g.cs");
        Assert.True(File.Exists(generatedFile));
        var generatedContent = await File.ReadAllTextAsync(generatedFile);
        Directory.Delete(outputDirectory, true);

        Assert.Contains($"namespace {@namespace};", generatedContent);
        Assert.Contains("internal partial record MyOwnContentType", generatedContent);
        Assert.Contains("public const string ContentTypeId = \"myOwnContentType\";", generatedContent);
        Assert.Contains("public required string Field1 { get; set; }", generatedContent);
        Assert.Contains("public int? Field2 { get; set; }", generatedContent);
    }

    [Fact]
    public async Task WhenWriteModels_WhileShouldNotOverwrite_ItShouldSkipExistingFiles()
    {
        var shouldOverWrite = false;

        var existingFile = Path.Combine(outputDirectory, "TestContentType.g.cs");
        await File.WriteAllTextAsync(existingFile, "Existing content");

        var contentTypes = new List<ContentType>
        {
            new ContentType
            {
                SystemProperties = new SystemProperties { Id = "testContentType" },
                Fields = [new Field { Id = "field1", Type = "Text", Required = true }]
            }
        };

        var subject = new ModelWriter(console, outputDirectory, shouldOverWrite, @namespace, isInternal);

        await subject.WriteModels(contentTypes);

        var content = await File.ReadAllTextAsync(existingFile);

        Directory.Delete(outputDirectory, true);
        
        Assert.Equal("Existing content", content);
    }

    [Fact]
    public async Task WhenWriteModels_WhitShouldOverwrite_ItShouldOverwriteExistingFiles()
    {
        var shouldOverWrite = true;

        var existingFile = Path.Combine(outputDirectory, "TestContentType.g.cs");
        await File.WriteAllTextAsync(existingFile, "Existing content");

        var contentTypes = new List<ContentType>
        {
            new ContentType
            {
                SystemProperties = new SystemProperties { Id = "testContentType" },
                Fields = [new Field { Id = "field1", Type = "Text", Required = true }]
            }
        };

        var subject = new ModelWriter(console, outputDirectory, shouldOverWrite, @namespace, isInternal);

        await subject.WriteModels(contentTypes);

        var content = await File.ReadAllTextAsync(existingFile);

        Directory.Delete(outputDirectory, true);
        
        Assert.DoesNotContain("Existing content", content);
        Assert.Contains("public const string ContentTypeId = \"testContentType\";", content);
        Assert.Contains("public required string Field1 { get; set; }", content);
    }
}