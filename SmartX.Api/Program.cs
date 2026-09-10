using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text.Json.Serialization;
using SmartX.Api.Domain.Collections;
using SmartX.Api.Endpoints;
using SmartX.Api.Hubs;
using SmartX.Api.Infrastructure.Data;
using SmartX.Api.Infrastructure.FileStorage;
using SmartX.Api.Infrastructure.Seeding;
using SmartX.Api.Middleware;
using SmartX.Api.Services;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddSignalR().AddJsonProtocol(options =>
    options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Default")!);

var clientOrigin = builder.Configuration["ClientOrigin"] ?? "http://localhost:8081";
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.WithOrigins(clientOrigin).AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

builder.Services.AddSingleton<AnomalyDetectionService>();
builder.Services.AddSingleton<TelemetryBatchStore>();
builder.Services.AddSingleton<IFileStorageService, EncryptedFileStorageService>();
builder.Services.AddScoped<SensorService>();
builder.Services.AddScoped<TelemetryIngestionService>();
builder.Services.AddScoped<DeploymentTreeService>();
builder.Services.AddHostedService<MockTelemetrySeeder>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
