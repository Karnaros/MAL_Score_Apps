using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Models;

namespace MAL_Score_Analyzer
{
    internal class DataBase
    {
        /// <summary>
        /// Extracts anime data from <paramref name="response"/>
        /// object and saves it to database.
        /// </summary>
        static internal async Task SaveResponse(PooledDbContextFactory<MalContext> dbFactory, ResponseModel response)
        {
            using var context = await dbFactory.CreateDbContextAsync();

            var dataDict = response.data
                .Select(x => x.node)
                .ToDictionary(item => item.id);

            Upsert(context, dataDict);

            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Adds or updates anime data from <paramref name="dataDict"/>
        /// to the <paramref name="context"/>.
        /// </summary>
        static void Upsert(MalContext context, Dictionary<int, Anime> dataDict)
        {
            var genreComparer = new GenreComparer();
            var genres = context.Genres.ToList();

            genres.Sort(genreComparer);

            var dataToUpdate = context.AnimeList
                .Where(x => dataDict.Keys.Contains(x.id))
                .ToList();

            dataToUpdate.ForEach(item =>
            {
                if (item.mean != dataDict[item.id].mean)
                    item.mean = dataDict[item.id].mean;
            });

            var dataToAdd = dataDict.Values
                .Where(item => !dataToUpdate
                    .Select(item => item.id)
                    .Contains(item.id))
                .Select(BindGenres(genres, genreComparer));

            context.AddRange(dataToAdd);
        }

        /// <summary>
        /// A delegate function for LINQ Select.
        /// Binds <paramref name="genres"/> from list to existing Anime object
        /// so that all Anime objects refer to the same <paramref name="genres"/>.
        /// </summary>
        static Func<Anime, Anime> BindGenres(List<Genre> genres, GenreComparer comparer)
        {
            return item =>
            {
                item.genres = item.genres.Select(x =>
                {
                    var i = genres.BinarySearch(x, comparer);
                    if (i < 0)
                    {
                        i = ~i;
                        genres.Insert(i, x);
                    }

                    return genres[i];
                }).ToList();

                return item;
            };
        }
    }
}
