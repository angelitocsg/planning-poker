using Microsoft.AspNetCore.ResponseCompression;
using PlanningPoker.Bff;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddSignalR();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" });
});

var app = builder.Build();

app.UseResponseCompression();

app.MapHub<GameHub>("/game");

app.Run();
