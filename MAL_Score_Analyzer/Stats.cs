using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Skia;
using Models;

namespace MAL_Score_Analyzer
{
    internal class Stats
    {
        /// <summary>
        /// Calculates and adds mean and median score for all genres in database.
        /// Also generates and saves to filesystem score heatmap images for all genres that
        /// have anime count more than certain threshold.
        /// </summary>
        /// <returns></returns>
        static internal async Task CalcGenreStats(PooledDbContextFactory<MalContext> dbFactory, Uri apiUri)
        {
            using var context = await dbFactory.CreateDbContextAsync();

            var basePath = "./src/static/";
            Directory.CreateDirectory(basePath);

            var genres = context.Genres.ToList();
            var total = genres.FirstOrDefault(genre => genre.name == "Total");

            if (total == null)
            {
                total = new Genre
                {
                    id = 2000,
                    name = "Total",
                    anime = new List<Anime>()
                };

                genres.Add(total);
                context.Genres.Add(total);
            }

            foreach (var genre in genres)
            {
                var queryBase = context.AnimeList
                        .Where(anime => anime.genres.Contains(genre));

                if (genre.name == "Total")
                {
                    queryBase = context.AnimeList;
                }

                var scores = queryBase
                        .Select(anime => anime.mean ?? 0)
                        .Where(score => score > 0)
                        .ToList();

                if (scores.Count > 500)
                    genre.heatMapUri = new Uri(
                            apiUri,
                            DrawHeatMap(scores, genre.name, basePath))
                        .ToString();

                scores.Sort();

                genre.mean = scores.Average();
                genre.median = CalcMedian(scores);
            }

            await context.SaveChangesAsync();
        }

        static float? CalcMedian(List<float> scores)
        {
            if (scores.Count == 0)
                return null;
            if (scores.Count % 2 == 0)
            {
                var left = scores[(scores.Count / 2)];
                var right = scores[(scores.Count / 2) + 1];
                return (left + right) / 2.0f;
            }
            else
            {
                return scores[(scores.Count / 2)];
            }
        }

        /// <summary>
        /// Generates score heatmap for <paramref name="genreName"/>
        /// based on <paramref name="scores"/>
        /// and saves it to filesystem at <paramref name="basePath"/>
        /// </summary>
        /// <returns>Heatmap file name <see cref="string"/>.</returns>
        static string DrawHeatMap(List<float> scores, string genreName, string basePath)
        {
            using SkiaBitmapExportContext bmp = new(400, 40, 1.0f);
            var canvas = bmp.Canvas;
            var maxHue = 240;

            canvas.FillColor = Color.FromHsv(maxHue, 100, 75);
            canvas.FillRectangle(0, 0, bmp.Width, bmp.Height);
            canvas.StrokeSize = 4;

            var valuesGraph = scores
                .GroupBy(
                    value => Math.Round((decimal)value, 1),
                    (value, group) => new
                    {
                        Key = value,
                        Count = group.Count()
                    });

            var maxCount = valuesGraph
                .Select(value => value.Count)
                .Max();
            var coeff = maxHue / (double)maxCount;


            foreach (var value in valuesGraph)
            {
                var hue = 240 - (int)(value.Count * coeff);
                var x = (int)(value.Key * 40);
                canvas.StrokeColor = Color.FromHsv(hue, 100, 75);

                canvas.DrawLine(x, 0, x, bmp.Height);
            }

            var heatMapFileName = $"{genreName}.png";
            bmp.WriteToFile(Path.Combine(basePath, heatMapFileName));

            return heatMapFileName;
        }
    }
}
