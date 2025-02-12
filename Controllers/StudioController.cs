using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Models.FilmStudio; 
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/filmstudio")]
    [ApiController]
    public class FilmStudioController : ControllerBase
    {
        private readonly FilmStudioDbContext _context;

        public FilmStudioController(FilmStudioDbContext context)
        {
            _context = context;
        }

        // POST: api/filmstudio/register
        [HttpPost("register")]
        public async Task<ActionResult<FilmStudio>> RegisterFilmStudio([FromBody] RegisterFilmStudio studio)
        {
            if (studio == null || string.IsNullOrEmpty(studio.Name))
            {
                return BadRequest("Film studio information is incomplete.");
            }

            try
            {
                var newFilmStudio = new FilmStudio
                {
                    Name = studio.Name,
                    Email = studio.Email
                };

                _context.FilmStudios.Add(newFilmStudio);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetFilmStudio), new { id = newFilmStudio.Id }, newFilmStudio);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error creating a new Film Studio: {ex.Message}");
            }
        }

        // GET: api/filmstudio/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetFilmStudio(int id)
        {
            var filmStudio = await _context.FilmStudios  // Ladda RentedFilmCopies om nödvändigt
                .FirstOrDefaultAsync(fs => fs.Id == id);

            if (filmStudio == null)
            {
                return NotFound($"Film Studio with ID {id} not found.");
            }

            // Hämta användarens roll från JWT-tokenet
            var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var userFilmStudioId = User.Claims.FirstOrDefault(c => c.Type == "FilmStudioId")?.Value;

            // Kontrollera om användaren är en admin
            if (userRole == "Admin")
            {
                // Admin får se all information
                return Ok(filmStudio);
            }

            // Om den autentiserade användaren är en filmstudio och det är den filmstudio som eftersöks
            if (userRole == "FilmStudio" && userFilmStudioId == id.ToString())
            {
                return Ok(filmStudio);  // Filmstudio får också se all information om sig själv
            }

            // För alla andra (oautentiserad användare eller annan filmstudio), ta bort City och RentedFilmCopies
            var limitedFilmStudio = new
            {
                filmStudio.Id,
                filmStudio.Name,
                filmStudio.Email  // Här kan du lägga till fler egenskaper som du vill visa för icke-autentiserade användare
            };

            return Ok(limitedFilmStudio);
        }


        // GET: api/filmstudio
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAllFilmStudios()
        {
            var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            var filmStudios = await _context.FilmStudios
                .ToListAsync();

            if (userRole == "Admin")
            {
                // 🔸 Admin får se all information
                return Ok(filmStudios);
            }
            else
            {
                // 🔸 Oautentiserad eller filmstudio → Begränsad information (utan RentedFilmCopies & City)
                var limitedFilmStudios = filmStudios.Select(fs => new
                {
                    fs.Id,
                    fs.Name
                });

                return Ok(limitedFilmStudios);
            }
        }
    }
}
