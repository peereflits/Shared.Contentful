using System.Collections.Generic;
using System.Net.Http;

namespace Contentful.Example.Implementation;

internal class QuickAndDirtyHttpClientFactory : IHttpClientFactory
{
    private readonly Dictionary<string, HttpClient> cache = new Dictionary<string, HttpClient>();

    public HttpClient CreateClient(string name)
    {
        if(!cache.ContainsKey(name))
        {
            cache[name] = new HttpClient();
        }

        return cache[name];
    }
}