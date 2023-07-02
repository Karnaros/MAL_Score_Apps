using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace MAL_Score_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Genres : ControllerBase
    {
        private readonly MalContext db;

        public Genres(MalContext context)
        {
            db = context;
        }

        // GET api/genres/Comedy
        [HttpGet("{name}")]
        public async Task<ActionResult<Genre>> Get(string name)
        {
            var genre = await db.Genres
                .FirstOrDefaultAsync(x => x.name == name);
            if (genre == null)
            {
                return NotFound();
            }

            return genre;
        }
    }
}
