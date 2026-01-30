# HackerNewsApi

## Overview
This project implements a small ASP.NET Core REST API for retrieving the first `n` "best stories" from the Hacker News API, sorted by score in descending order.

## Running the application
1. Install the .NET 10 SDK.
2. From the repository root:

```bash
dotnet --version
dotnet restore HackerNewsApi
dotnet run --project HackerNewsApi
```

The API will start and listen on the default ASP.NET Core port (typically https://localhost:5001 or http://localhost:5000).

### Example request
```bash
curl "http://localhost:5000/api/beststories?n=5"
```

## Assumptions
- The Hacker News API is available and returns well-formed JSON.
- Requests for more than 500 stories are rejected to prevent excessive load on the upstream API.
- Response timestamps use ISO-8601 with UTC offset ("O" format).

## Caching and resiliency
- Best story IDs are cached for 1 minute.
- Individual story payloads are cached for 5 minutes.
- Requests are throttled to 10 concurrent outbound story fetches per API call to avoid overwhelming the upstream API.

## Potential enhancements
- Add background refresh of best stories to serve from cache without waiting on upstream responses.
- Add distributed caching (e.g., Redis) for multi-instance deployments.
- Add resilience policies such as retries with exponential backoff and circuit breaking.
- Add OpenAPI/Swagger documentation.



## Challenge
- Santander - Developer Coding Test
- Using ASP.NET Core, implement a RESTful API to retrieve the details of the first n "best stories" from the Hacker News API, where n is specified by the caller to the API.
- The Hacker News API is documented here: https://github.com/HackerNews/API .
- The IDs for the "best stories" can be retrieved from this URI: https://hacker-news.firebaseio.com/v0/beststories.json .
- The details for an individual story ID can be retrieved from this URI: https://hacker-news.firebaseio.com/v0/item/21233041.json (in this case for the story with ID
21233041 )
- The API should return an array of the first n "best stories" as returned by the Hacker News API, sorted by their score in a descending order, in the form:
<!-- [
    {
        "title": "A uBlock Origin update was rejected from the Chrome Web Store",
        "uri": "https://github.com/uBlockOrigin/uBlock-issues/issues/745",
        "postedBy": "ismaildonmez",
        "time": "2019-10-12T13:43:01+00:00",
        "score": 1716,
        "commentCount": 572
    },
    { ... },
    { ... },
    { ... },
    ...
] -->
- In addition to the above, your API should be able to efficiently service large numbers of requests without risking overloading of the Hacker News API.