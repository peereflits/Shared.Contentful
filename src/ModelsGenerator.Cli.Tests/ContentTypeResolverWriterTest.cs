using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Contentful.Core.Models;
using McMaster.Extensions.CommandLineUtils;
using NSubstitute;
using Xunit;

using File = System.IO.File;

namespace Peereflits.Shared.Contentful.ModelsGenerator.Cli.Tests;

public class ContentTypeResolverWriterTest
{
    private readonly IConsole console;
    private readonly string outputDirectory;
    private readonly string @namespace;
    private readonly bool isInternal;

    public ContentTypeResolverWriterTest()
    {
        console = Substitute.For<IConsole>();
        console
            .Out
            .Returns(Substitute.For<TextWriter>());

        outputDirectory = Path.Combine(Path.GetTempPath(), "ContentTypeResolverWriterTests");
        @namespace = "TestNamespace";
        isInternal = true;

        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }
    }

    [Fact]
    public async Task WhenWriteContentTypeResolver_ItShouldDoSo()
    {
        var contentTypes = new List<ContentType>
        {
            new ContentType
            {
                SystemProperties = new SystemProperties { Id = "myFirstContentType" },
                Fields = []
            },
            new ContentType
            {
                SystemProperties = new SystemProperties { Id = "mySecondContentType" },
                Fields =[]
            }
        };

        var subject = new ContentTypeResolverWriter(console, outputDirectory, @namespace, isInternal);

        await subject.WriteContentTypeResolver(contentTypes);

        var generatedFile = Path.Combine(outputDirectory, "GeneratedContentTypeResolver.g.cs");
        Assert.True(File.Exists(generatedFile));
        var generatedContent = await File.ReadAllTextAsync(generatedFile);
        Directory.Delete(outputDirectory, true);

        Assert.Contains("namespace TestNamespace;", generatedContent);
        Assert.Contains("internal partial class GeneratedContentTypeResolver", generatedContent);
        Assert.Contains(" [MyFirstContentType.ContentTypeId] = typeof(MyFirstContentType),", generatedContent);
        Assert.Contains(" [MySecondContentType.ContentTypeId] = typeof(MySecondContentType),", generatedContent);
    }

    [Fact]
    public async Task WhenWriteContentTypeResolver_ItShouldOverwrite()
    {
        var generatedFile = Path.Combine(outputDirectory, "GeneratedContentTypeResolver.g.cs");
        await File.WriteAllTextAsync(generatedFile , "Existing content");

        var contentTypes = new List<ContentType>
        {
            new ContentType
            {
                SystemProperties = new SystemProperties { Id = "myFirstContentType" },
                Fields = []
            }
        };

        var subject = new ContentTypeResolverWriter(console, outputDirectory, @namespace, isInternal);

        await subject.WriteContentTypeResolver(contentTypes);

        Assert.True(File.Exists(generatedFile));
        var generatedContent = await File.ReadAllTextAsync(generatedFile);
        Directory.Delete(outputDirectory, true);

        Assert.Contains(" [MyFirstContentType.ContentTypeId] = typeof(MyFirstContentType),", generatedContent);
    }
}