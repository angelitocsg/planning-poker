using Microsoft.AspNetCore.ResponseCompression;
using PlanningPoker.Bff;

const string CFG_CORS_HOSTS = "CorsHosts";
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();
builder.Services.AddMemoryCache();
builder.Services.AddSignalR();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" });
});

var app = builder.Build();

app.UseCors(config =>
{
    config.AllowAnyHeader();
    config.AllowAnyMethod();
    config.AllowCredentials();
    config.WithOrigins(builder.Configuration.GetSection(CFG_CORS_HOSTS).Get<string[]>());
});

app.UseResponseCompression();
app.MapHub<GameHub>("/game");

app.Run();
