using EveStationJanitor;
using EveStationJanitor.Authentication;
using EveStationJanitor.Authentication.Persistence;
using EveStationJanitor.Core;
using EveStationJanitor.Core.DataAccess;
using EveStationJanitor.Core.StaticData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Globalization;

var applicationDataDirectoryPath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    "Eve Station Janitor");

var databasePath = Path.Combine(applicationDataDirectoryPath, "esj.db");
var tokenPath = Path.Combine(applicationDataDirectoryPath, ".tokens");
var logPath = Path.Combine(applicationDataDirectoryPath, "esj.txt");

Directory.CreateDirectory(applicationDataDirectoryPath);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File(logPath, rollingInterval: RollingInterval.Month, formatProvider: CultureInfo.InvariantCulture)
    .CreateLogger();

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSerilog();
builder.Services.AddAuth(tokenPath);
builder.Services.AddCore(databasePath);

builder.Services.AddScoped<IAuthenticationDataProvider, DatabaseAuthDataPersistence>();
builder.Services.AddScoped<Janitor>();
builder.Services.AddScoped<App>();

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var app = scope.ServiceProvider.GetRequiredService<App>();
    await app.Run();
}

internal class App(AppDbContext context, Janitor janitor, FuzzworksStaticDataDownloader staticDataLoader)
{
    public async Task Run()
    {
        Console.WriteLine("Initialising database...");
        context.Database.Migrate();

        Console.WriteLine("Loading static data...");
        await staticDataLoader.Run();

        await janitor.Run();
    }
}
