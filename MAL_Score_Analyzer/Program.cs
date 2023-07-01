using MAL_Score_Analyzer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

ConfigurationBuilder configBuilder = new();

if (Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") == "Development")
{
    configBuilder.AddJsonFile("settings.json", false, true);
    configBuilder.AddJsonFile("sharedsettings.json", false, true);
}
else
{
    configBuilder.AddJsonFile("/run/secrets/analyzer_settings", false);
    configBuilder.AddJsonFile("/run/secrets/shared_settings", false);
}

var config = configBuilder.Build();

Uri apiUri = new(config.GetValue<string>("ApiUri")!);

var dbOptions = new DbContextOptionsBuilder()
    .UseNpgsql(config.GetConnectionString("PostgreSQL"))
    .Options;

using HttpClient client = new();
client.DefaultRequestHeaders.Add(
    config.GetSection("MalHeaders")["name"]!,
    config.GetSection("MalHeaders")["value"]!);
client.BaseAddress = new Uri("https://api.myanimelist.net/v2/anime/");

await Fetching.FetchAndSaveAll(dbOptions, client);

await Stats.CalcGenreStats(dbOptions, apiUri);