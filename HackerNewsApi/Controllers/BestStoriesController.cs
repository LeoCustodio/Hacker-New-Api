using HackerNewsApi.Models;
using HackerNewsApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace HackerNewsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class BestStoriesController : ControllerBase
{
    private const int MaxStories = 500;
    private readonly IHackerNewsClient _hackerNewsClient;

    public BestStoriesController(IHackerNewsClient hackerNewsClient)
    {
        _hackerNewsClient = hackerNewsClient;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BestStoryResponse>>> GetBestStories([FromQuery] int n = 10, CancellationToken cancellationToken = default)
    {
        //Must be greater than zero to return data.
        if (n <= 0)
        {
            return BadRequest("Query parameter 'n' must be greater than zero.");
        }
        //Max is set to 500.
        if (n > MaxStories)
        {
            return BadRequest($"Query parameter 'n' must be less than or equal to {MaxStories}.");
        }

        var stories = await _hackerNewsClient.GetBestStoriesAsync(n, cancellationToken);
        var response = stories
            .OrderByDescending(story => story.Score)
            .Select(story => new BestStoryResponse
            {
                Title = story.Title,
                Uri = story.Url,
                PostedBy = story.By,
                Time = DateTimeOffset.FromUnixTimeSeconds(story.Time).ToString("O"),
                Score = story.Score,
                CommentCount = story.Descendants
            })
            .ToArray();

        return Ok(response);
    }
}
