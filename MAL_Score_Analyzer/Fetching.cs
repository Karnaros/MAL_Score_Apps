using Microsoft.EntityFrameworkCore.Infrastructure;
using Models;
using System.Net;
using System.Text.Json;

namespace MAL_Score_Analyzer
{
    internal class Fetching
    {
        /// <summary>
        /// Iteratively fetches MAL API to get data about all anime that has user score.
        /// </summary>
        static internal async Task FetchAndSaveAll(PooledDbContextFactory<MalContext> dbFactory, HttpClient client)
        {
            var offset = 0;
            var limit = 500;
            var exceptionCount = 0;
            var exceptionMaxTries = 3;
            ResponseModel response;

            while (true)
            {
                try
                {
                    response = await FetchMalOne(client, offset, limit);
                }
                catch (Exception e) when
                ((e is HttpRequestException exception && exception.StatusCode == HttpStatusCode.Forbidden) ||
                e is JsonException)
                {
                    if (exceptionCount >= exceptionMaxTries)
                        throw;
                    exceptionCount++;
                    Console.WriteLine(e.ToString());
                    Console.WriteLine("Possibly rate-limited, waiting...");
                    await Task.Delay(1000 * exceptionCount);
                    continue;
                }

                if (response.data.Length == 0)
                    break;

                await DataBase.SaveResponse(dbFactory, response);

                if (response.data.Last().node.mean is null)
                    break;

                offset += limit;
                await Task.Delay(500);
            }
        }

        /// <summary>
        /// Fetches MAL API "/ranking" endpoint with provided offset and limit.
        /// </summary>
        /// <returns>Response object containing list of requested anime.</returns>
        static async Task<ResponseModel> FetchMalOne(HttpClient client, int offset, int limit)
        {
            var query = $"ranking?offset={offset}&ranking_type=all&limit={limit}&fields=id,title,mean,genres";
            Console.WriteLine($"Fetching: {client.BaseAddress}{query}");

            var json = await client.GetByteArrayAsync(query);

            var response = JsonSerializer.Deserialize<ResponseModel>(json)!;

            return response;
        }
    }
}
