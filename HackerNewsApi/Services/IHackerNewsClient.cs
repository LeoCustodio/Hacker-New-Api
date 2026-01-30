using HackerNewsApi.Models;

namespace HackerNewsApi.Services;

public interface IHackerNewsClient
{
    Task<IReadOnlyList<HackerNewsItem>> GetBestStoriesAsync(int count, CancellationToken cancellationToken);
}
