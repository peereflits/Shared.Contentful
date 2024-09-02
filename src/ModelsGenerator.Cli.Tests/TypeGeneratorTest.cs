using System.Collections.Generic;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Xunit;

namespace Peereflits.Shared.Contentful.ModelsGenerator.Cli.Tests;

public class TypeGeneratorTest
{
    private readonly List<ContentType> contentTypes = [];


    [Fact]
    public void WhenGenerateClass_WithBasicTypes_ItShouldGenerateCorrectProperties()
    {
        var contentType = new ContentType
        {
            SystemProperties = new SystemProperties { Id = "testContentType" },
            Fields =
            [
                new Field { Id = "field1", Type = "Text", Required = true },
                new Field { Id = "field2", Type = "Symbol", Required = false },
                new Field { Id = "field3", Type = "Integer", Required = false },
                new Field { Id = "field4", Type = "Number", Required = false },
                new Field { Id = "field5", Type = "Boolean", Required = false },
                new Field { Id = "field6", Type = "Date", Required = false },
                new Field { Id = "field7", Type = "RichText", Required = false },
                new Field { Id = "field8", Type = "Location", Required = false },
                new Field { Id = "field9", Type = "Any other type", Required = false },
            ]
        };

        contentTypes.Add(contentType);
        var subject = new TypeGenerator("Contentful.Models", true, contentTypes);

        var result = subject.Generate(contentType);

        Assert.Contains("internal partial record TestContentType", result);
        Assert.Contains("public const string ContentTypeId = \"testContentType\";", result);
        Assert.Contains("public required string Field1 { get; set; }", result);
        Assert.Contains("public string? Field2 { get; set; }", result);
        Assert.Contains("public int? Field3 { get; set; }", result);
        Assert.Contains("public float? Field4 { get; set; }", result);
        Assert.Contains("public bool? Field5 { get; set; }", result);
        Assert.Contains("public DateTime? Field6 { get; set; }", result);
        Assert.Contains("public Document? Field7 { get; set; }", result);
        Assert.Contains("public Location? Field8 { get; set; }", result);
        Assert.Contains("public object? Field9 { get; set; }", result);
    }

    [Fact]
    public void WhenGenerateClass_WithLinkTypes_ItShouldGenerateCorrectProperties()
    {
        var contentType1 = new ContentType
        {
            SystemProperties = new SystemProperties { Id = "MyContentType" },
            Fields =
            [
                new Field { Id = "field1", Type = "Symbol", Required = false },
            ]
        };

        var contentType2 = new ContentType
        {
            SystemProperties = new SystemProperties { Id = "testContentType" },
            Fields =
            [
                new Field { Id = "field1", Type = "Link", Required = true, LinkType = "Asset" },
                new Field { Id = "field2", Type = "Link", Required = true, LinkType = "Anything..." },
                new Field
                {
                    Id = "field3", Type = "Link", Required = true, LinkType = "Entry",
                    Validations = [new LinkContentTypeValidator { ContentTypeIds = ["MyContentType"] }]
                },
                new Field
                {
                    Id = "field4", Type = "Link", Required = true, LinkType = "Entry",
                    Validations = [new LinkContentTypeValidator
                    {
                        ContentTypeIds = ["MyContentType", "Any other type"]
                    }]
                }
            ]
        };

        contentTypes.Add(contentType1);
        contentTypes.Add(contentType2);
        var subject = new TypeGenerator("Contentful.Models", true, contentTypes);

        var result = subject.Generate(contentType2);

        Assert.Contains("internal partial record TestContentType", result);
        Assert.Contains("public const string ContentTypeId = \"testContentType\";", result);
        Assert.Contains("public required Asset Field1 { get; set; }", result);
        Assert.Contains("public required object Field2 { get; set; }", result);
        Assert.Contains("public required MyContentType Field3 { get; set; }", result);
        Assert.Contains("public required object Field4 { get; set; }", result);
    }

    [Fact]
    public void WhenGenerateClass_WithArrayTypes_ItShouldGenerateCorrectProperties()
    {
        var contentType1 = new ContentType
        {
            SystemProperties = new SystemProperties { Id = "MyContentType" },
            Fields =
            [
                new Field { Id = "field1", Type = "Symbol", Required = false },
            ]
        };

        var contentType2 = new ContentType
        {
            SystemProperties = new SystemProperties { Id = "testContentType" },
            Fields =
            [
                new Field
                {
                    Id = "field1", Type = "Array", Required = true,
                    Items = new Schema { Type = "Symbol" }
                },
                new Field
                {
                    Id = "field2", Type = "Array", Required = false,
                    Items = new Schema { Type = "Any other non-entry type. This should never occur." }
                },
                new Field
                {
                    Id = "field3", Type = "Array", Required = true, 
                    Items = new Schema
                    {
                        LinkType = "Entry",
                        Validations = [new LinkContentTypeValidator { ContentTypeIds = ["MyContentType"] }]
                    },
                },
                new Field
                {
                    Id = "field4", Type = "Array", Required = false, 
                    Items = new Schema
                    {
                        LinkType = "Entry",
                        Validations = [new LinkContentTypeValidator { ContentTypeIds = ["MyContentType", "AnotherType"] }]
                    },
                },
                new Field
                {
                    Id = "field5", Type = "Array", Required = false, 
                    Items = new Schema
                    {
                        LinkType = "Entry",
                        Validations = null
                    },
                }
            ]
        };

        contentTypes.Add(contentType1);
        contentTypes.Add(contentType2);
        var subject = new TypeGenerator("Contentful.Models", true, contentTypes);

        var result = subject.Generate(contentType2);

        Assert.Contains("internal partial record TestContentType", result);
        Assert.Contains("public const string ContentTypeId = \"testContentType\";", result);
        Assert.Contains("public required List<string> Field1 { get; set; }", result);
        Assert.Contains("public List<object>? Field2 { get; set; }", result);
        Assert.Contains("public required List<MyContentType> Field3 { get; set; }", result);
        Assert.Contains("public List<object>? Field4 { get; set; }", result);
        Assert.Contains("public List<object>? Field5 { get; set; }", result);
    }
}