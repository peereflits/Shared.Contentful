using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Contentful.Core;
using Contentful.Core.Errors;
using Contentful.Core.Search;
using Contentful.Implementation.Models;

namespace Contentful.Example.Implementation;

internal interface IQueryContentPages
{
    Task<IEnumerable<ContentPage>> GetContentPagesAsync(string locale = "en-US", CancellationToken token = default);
    Task<ContentPage?> GetContentPageAsync(string slug, CancellationToken token = default);
}

internal class ContentPagesQuery(IContentfulClientBuilder clientBuilder) : IQueryContentPages
{
    private const int UpToFourLevelsDeep = 4;

    private readonly IContentfulClient client = clientBuilder.BuildClient();

    public async Task<IEnumerable<ContentPage>> GetContentPagesAsync(string locale = "en-US", CancellationToken token = default)
    {
        if (token.IsCancellationRequested)
        {
            throw new OperationCanceledException();
        }

        try
        {
            var locales = (await client.GetLocales(token)).ToList();
            var language = locales.FirstOrDefault(x => x.Code == locale) ?? locales.First();

            var builder = new QueryBuilder<ContentPage>()
                .ContentTypeIs(ContentPage.ContentTypeId)
                .LocaleIs(language .Code)
                .Limit(25)
                .Include(UpToFourLevelsDeep);

            var pages = await client.GetEntries(builder, token);

            return pages == null ? [] : pages.Items;
        }
        catch (ContentfulException exception)
        {
            Debug.WriteLine(exception);
            throw;
        }
    }

    public async Task<ContentPage?> GetContentPageAsync(string slug, CancellationToken token = default)
    {
        if (token.IsCancellationRequested)
        {
            throw new OperationCanceledException();
        }

        try
        {
            var builder = new QueryBuilder<ContentPage>()
                .ContentTypeIs(ContentPage.ContentTypeId)
                .FieldEquals(x => x.Slug, slug)
                .Limit(1)
                .Include(UpToFourLevelsDeep);

            var pages = await client.GetEntries(builder, token);

            return pages.SingleOrDefault();
        }
        catch (ContentfulException exception)
        {
            Debug.WriteLine(exception);
            throw;
        }
    }
}