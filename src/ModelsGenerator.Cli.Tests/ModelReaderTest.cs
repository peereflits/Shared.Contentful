using System.Threading.Tasks;
using Contentful.Core.Errors;
using Peereflits.Shared.Contentful.ModelsGenerator.Cli.Tests.Helpers;
using Xunit;

namespace Peereflits.Shared.Contentful.ModelsGenerator.Cli.Tests;

[Trait("Category", "Integration")]
public class ModelReaderTest : IClassFixture<IntegrationTestsFixture>
{
    private readonly TestSettings settings;

    public ModelReaderTest(IntegrationTestsFixture fixture)
    {
        settings = fixture.Settings;
    }

    [Fact]
    public async Task WhenExecute_WithValidParams_ItShouldReturnAListOfContentTypes()
    {
        var subject = new ModelReader();

        var result = await subject.Execute(settings.ApiKey, settings.SpaceId, settings.Environment);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task WhenExecute_WithInvalidParams_ItShouldThrow()
    {
        var subject = new ModelReader();

        var result = await Assert.ThrowsAsync<ContentfulException>(()=>  subject.Execute("ApiKey", "SpaceId", "Environment"));
        Assert.NotNull(result);
    }
}