using Microsoft.Extensions.Configuration;

namespace Peereflits.Shared.Contentful.ModelsGenerator.Cli.Tests.Helpers;

public class IntegrationTestsFixture
{
    private static TestSettings GetTestSettings()
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("testsettings.json")
            .AddJsonFile("testsettings.Development.json", true)
            .Build();

        var settings = new TestSettings();
        builder.GetSection(nameof(TestSettings)).Bind(settings);

        return settings;
    }

    public TestSettings Settings { get; } = GetTestSettings();
}