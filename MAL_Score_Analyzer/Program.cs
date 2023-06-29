using MAL_Score_Analyzer;
using Microsoft.EntityFrameworkCore;

string connection = "Host=localhost;Port=5432;Database=mal-score-db;Username=postgres;Password=admin;Include Error Detail=true";
DbContextOptionsBuilder optionsBuilder = new();
var dbOptions = optionsBuilder
    .UseNpgsql(connection)
    .Options;

using HttpClient client = new();
client.DefaultRequestHeaders.Add("X-MAL-CLIENT-ID", "33539df35957bd126a2ccba68f68bc2a");
client.BaseAddress = new Uri("https://api.myanimelist.net/v2/anime/");

await Fetching.FetchAndSaveAll(dbOptions, client);

await Stats.CalcGenreStats(dbOptions);