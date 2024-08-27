using System.Net.Http;
using Contentful.Core;
using Contentful.Core.Configuration;
using Contentful.Implementation.Models;

namespace Contentful.Example.Implementation;

internal interface IContentfulClientBuilder
{
    IContentfulClient BuildClient();
}

internal class ContentfulClientBuilder
(
    IHttpClientFactory factory,
    ContentfulSettings settings
) : IContentfulClientBuilder
{
    public IContentfulClient BuildClient()
    {
        var options = new ContentfulOptions
        {
            DeliveryApiKey = settings.ApiKey,
            SpaceId = settings.SpaceId,
            Environment = settings.Environment,
            UsePreviewApi = false,
            ResolveEntriesSelectively = true,
        };

        return new ContentfulClient(factory.CreateClient(nameof(ContentfulClientBuilder)), options)
        {
            ContentTypeResolver = new GeneratedContentTypeResolver()
        };
    }
}