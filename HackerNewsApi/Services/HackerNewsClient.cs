using System.Net.Http.Json;
using HackerNewsApi.Models;
using Microsoft.Extensions.Caching.Memory;

namespace HackerNewsApi.Services;

public sealed class HackerNewsClient : IHackerNewsClient
{
    private const string BestStoriesCacheKey = "hackernews.beststories";
    private static readonly Uri BaseUri = new("https://hacker-news.firebaseio.com/v0/");
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;

    public HackerNewsClient(HttpClient httpClient, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _cache = cache;
        _httpClient.BaseAddress = BaseUri;
    }

    public async Task<IReadOnlyList<HackerNewsItem>> GetBestStoriesAsync(int count, CancellationToken cancellationToken)
    {
        var storyIds = await GetBestStoryIdsAsync(cancellationToken);
        var takeCount = Math.Min(count, storyIds.Count);
        var idsToFetch = storyIds.Take(takeCount).ToArray();

        //To do not overload the hacker-news api, implemented throttling system.
        var throttler = new SemaphoreSlim(10, 10);
        var tasks = idsToFetch.Select(async id =>
        {
            await throttler.WaitAsync(cancellationToken);
            try
            {
                return await GetStoryAsync(id, cancellationToken);
            }
            finally
            {
                throttler.Release();
            }
        });

        var results = await Task.WhenAll(tasks);
        return results.Where(item => item is not null).Cast<HackerNewsItem>().ToArray();
    }

    private async Task<IReadOnlyList<long>> GetBestStoryIdsAsync(CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue(BestStoriesCacheKey, out IReadOnlyList<long>? cachedIds))
        {
            return cachedIds;
        }

        var ids = await _httpClient.GetFromJsonAsync<List<long>>("beststories.json", cancellationToken)
            ?? new List<long>();

        //Chace Item ids for a minute
        _cache.Set(
            BestStoriesCacheKey,
            ids,
            new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(1)));

        return ids;
    }

    private async Task<HackerNewsItem?> GetStoryAsync(long id, CancellationToken cancellationToken)
    {
        var cacheKey = $"hackernews.item.{id}";
        if (_cache.TryGetValue(cacheKey, out HackerNewsItem? cachedItem))
        {
            return cachedItem;
        }

        var item = await _httpClient.GetFromJsonAsync<HackerNewsItem>($"item/{id}.json", cancellationToken);
        if (item is null)
        {
            return null;
        }

        //Chace payloads for 5 minute
        _cache.Set(
            cacheKey,
            item,
            new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));

        return item;
    }
}
