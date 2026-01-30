using System.Text.Json.Serialization;

namespace HackerNewsApi.Models;

public sealed class BestStoryResponse
{
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [JsonPropertyName("uri")]
    public string? Uri { get; init; }

    [JsonPropertyName("postedBy")]
    public string? PostedBy { get; init; }

    [JsonPropertyName("time")]
    public string? Time { get; init; }

    [JsonPropertyName("score")]
    public int Score { get; init; }

    [JsonPropertyName("commentCount")]
    public int CommentCount { get; init; }
}
