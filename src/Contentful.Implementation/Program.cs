using System;
using System.Threading.Tasks;
using Contentful.Core.Models;
using Microsoft.Extensions.Configuration;

namespace Contentful.Example.Implementation;

internal class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=======================================");
        Console.WriteLine("Working with contentful ModelsGenerator");
        Console.WriteLine("=======================================");
        Console.WriteLine();
        Console.WriteLine("This is an implementation example");
        Console.WriteLine();

        IContentfulClientBuilder clientBuilder = new ContentfulClientBuilder(new QuickAndDirtyHttpClientFactory(), GetSettings());

        var query = new ContentPagesQuery(clientBuilder);
        var pages = await query.GetContentPagesAsync();

        Console.WriteLine("Available pages:");

        foreach (var page in pages)
        {
            Console.WriteLine($"{page.Slug}: {page.Title} by {page.Author}");
        }

        var choice = "---";
        while (choice != "q")
        {
            Console.WriteLine();
            Console.WriteLine("Enter a slug to view a page, or 'q' to quit:");
            choice = Console.ReadLine();

            if (choice == "q")
            {
                break;
            }

            var page = await query.GetContentPageAsync(choice ?? string.Empty);

            if (page == null)
            {
                Console.WriteLine("Page not found.");
            }
            else
            {
                Console.WriteLine($"{page.Slug}: {page.Title} by {page.Author}");
                Console.WriteLine($"Summary: {page.Summary}");
                Console.WriteLine("Content:");
                Console.WriteLine();
                Console.WriteLine(await new HtmlRenderer().ToHtml(page.Content));
                Console.WriteLine();
            }
        }
    }

    private static ContentfulSettings GetSettings()
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appSettings.json")
            .Build();

        var settings = new ContentfulSettings();
        builder.GetSection("AppSettings:Contentful").Bind(settings);

        return settings;
    }
}