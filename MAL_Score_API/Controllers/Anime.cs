using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MAL_Score_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Anime : ControllerBase
    {
        private readonly MalContext db;

        public Anime(MalContext context)
        {
            db = context;
        }

        // GET api/anime/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Models.Anime>> Get(int id)
        {
            var anime = await db.AnimeList
                .Include(a => a.genres)
                .FirstOrDefaultAsync(x => x.id == id);
            if (anime == null)
            {
                return NotFound();
            }

            return anime;
        }
    }
}
