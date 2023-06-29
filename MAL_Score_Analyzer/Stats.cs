using Microsoft.Maui.Graphics.Skia;
using Microsoft.Maui.Graphics;
using Models;
using Microsoft.EntityFrameworkCore;

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
        static internal async Task CalcGenreStats(DbContextOptions dbOptions)
        {
            using var context = new MalContext(dbOptions);

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
                IQueryable<Anime> queryBase;

                if (genre.name == "Total")
                {
                    queryBase = context.AnimeList;
                }
                else
                {
                    queryBase = context.AnimeList
                        .Where(anime => anime.genres.Contains(genre));
                }

                var scores = queryBase
                        .Select(anime => anime.mean ?? 0)
                        .Where(score => score > 0)
                        .ToList();

                if (scores.Count > 500)
                    genre.heatMapFileName = DrawHeatMap(scores, genre.name);

                scores.Sort();

                genre.mean = scores.Average();
                genre.median = scores.Count > 0 ? scores[(scores.Count / 2)] : null;
            }

            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Generates and saves to filesystem score heatmap for <paramref name="genreName"/>
        /// based on <paramref name="scores"/>.
        /// </summary>
        /// <returns>Heatmap file name <see cref="string"/>.</returns>
        static string DrawHeatMap(List<float> scores, string genreName)
        {
            using SkiaBitmapExportContext bmp = new(400, 40, 1.0f);
            ICanvas canvas = bmp.Canvas;
            int maxHue = 240;

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

            string heatMapFileName = $"{genreName}.png";
            bmp.WriteToFile(heatMapFileName);

            return heatMapFileName;
        }
    }
}
