using Contentful.Core;
using Contentful.Core.Configuration;
using Contentful.Core.Models;

namespace Peereflits.Shared.Contentful.ModelsGenerator.Cli
{
    internal class ModelReader
    {
        public async Task<IEnumerable<ContentType>> Execute(string apiKey, string spaceId, string environment)
        {
            using var http = new HttpClient();
            var options = new ContentfulOptions
            {
                DeliveryApiKey = apiKey,
                SpaceId = spaceId,
                Environment = environment
            };

            var client = new ContentfulClient(http, options);
            return await client.GetContentTypes();
        }
    }
}