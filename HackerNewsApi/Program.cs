using HackerNewsApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<IHackerNewsClient, HackerNewsClient>();

var app = builder.Build();

app.MapControllers();

app.Run();
