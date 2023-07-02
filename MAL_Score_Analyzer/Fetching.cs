using Microsoft.EntityFrameworkCore;
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
        static internal async Task FetchAndSaveAll(DbContextOptions dbOptions, HttpClient client)
        {
            int offset = 0;
            int limit = 500;
            int exceptionCount = 0;
            int exceptionMaxTries = 3;

            void errorHandler(Exception e)
            {
                if (exceptionCount >= exceptionMaxTries) throw e;
                exceptionCount++;
                Console.WriteLine(e.ToString());
                Console.WriteLine("Possibly rate-limited, waiting...");
                Thread.Sleep(1000 * exceptionCount);
            }

            while (true)
            {
                ResponseModel response;

                try
                {
                    response = await FetchMalOne(client, offset, limit);
                }
                catch (Exception e) when 
                ((e is HttpRequestException exception && exception.StatusCode == HttpStatusCode.Forbidden) || 
                e is JsonException)
                {
                    errorHandler(e);
                    continue;
                }

                if (response.data.Length == 0)
                {
                    offset = 0;
                    break;
                }

                await DataBase.SaveResponse(dbOptions, response);

                if (response.data.Last().node.mean is null)
                {
                    offset = 0;
                    break;
                }

                offset += limit;
                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// Fetches MAL API "/ranking" endpoint with provided offset and limit.
        /// </summary>
        /// <returns>Response object containing list of requested anime.</returns>
        static async Task<ResponseModel> FetchMalOne(HttpClient client, int offset, int limit)
        {
            string query = $"ranking?offset={offset}&ranking_type=all&limit={limit}&fields=id,title,mean,genres";
            Console.WriteLine($"Fetching: {client.BaseAddress}{query}");

            var json = await client.GetByteArrayAsync(query);

            ResponseModel response = JsonSerializer.Deserialize<ResponseModel>(json)!;

            return response;
        }
    }
}
