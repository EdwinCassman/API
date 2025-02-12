using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Models.FilmStudio; 
using Models.Film;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using API.DTOs;
using API.Mappers;

namespace API.Controllers
{
    [Route("api/films")]
    [ApiController]
    public class FilmController : ControllerBase
    {
        private readonly FilmStudioDbContext _context;

        public FilmController(FilmStudioDbContext context)
        {
            _context = context;
        }

        // GET: /api/films
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FilmDto>>> GetAllFilms()
        {
            var isAuthenticated = User.Identity.IsAuthenticated;

            var films = await _context.Films
                .Include(f => f.FilmCopies) // 🔸 Viktigt! Annars laddas inte FilmCopies
                .ToListAsync();

            var filmDtos = films.Select(f => new FilmDto
            {
                Id = f.Id,
                Title = f.Title,
                Genre = f.Genre,
                ReleaseYear = f.ReleaseYear,
                AvailableCopies = f.AvailableCopies,
                FilmCopies = isAuthenticated 
                    ? f.FilmCopies.Select(fc => new FilmCopyDto
                    {
                        Id = fc.Id,
                        IsAvailable = fc.IsAvailable
                    }).ToList() 
                    : new List<FilmCopyDto>() // 🔹 Skicka tom lista istället för null
            }).ToList();

            return Ok(filmDtos);
        }



        // POST: /api/films (Endast Admin kan lägga till filmer)
        [HttpPost]
        [Authorize(Roles = "Admin")] // Endast Admin kan lägga till filmer
        public async Task<ActionResult<FilmDto>> AddFilm([FromBody] FilmDto newFilmDto)
        {
            if (newFilmDto == null || string.IsNullOrEmpty(newFilmDto.Title))
            {
                return BadRequest("Film information is incomplete.");
            }

            var newFilm = new Film
            {
                Title = newFilmDto.Title,
                Genre = newFilmDto.Genre,
                ReleaseYear = newFilmDto.ReleaseYear,
                AvailableCopies = newFilmDto.AvailableCopies,
                FilmCopies = new List<FilmCopy>
                {
                    new FilmCopy { IsAvailable = true}
                }
            };

            _context.Films.Add(newFilm);
            await _context.SaveChangesAsync();

            var createdFilmDto = new FilmDto
            {
                Id = newFilm.Id,
                Title = newFilm.Title,
                Genre = newFilm.Genre,
                ReleaseYear = newFilm.ReleaseYear,
                AvailableCopies = newFilm.AvailableCopies,
                FilmCopies = newFilm.FilmCopies.Select(fc => new FilmCopyDto
                {
                    Id = fc.Id,
                    IsAvailable = fc.IsAvailable
                }).ToList()
            };

            return Ok(createdFilmDto);
        }

        // GET: /api/films/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<FilmDto>> GetFilm(int id)
        {
            var film = await _context.Films
                .Include(f => f.FilmCopies)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (film == null)
            {
                return NotFound($"Film with id {id} not found.");
            }

            var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if(userRole == "Admin" || userRole == "FilmStudio")
            {
                return Ok(FilmMapper.ToDto(film));
            }
            else 
            {
                var limitedFilm = new
                {
                    film.Id,
                    film.Title,
                    film.Genre,
                    film.ReleaseYear
                };

                return Ok(limitedFilm);
            }

        }



        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Film>> UpdateFilm(int id, [FromBody] Film updatedFilm)
        {
            var film = await _context.Films.FindAsync(id);

            if (film == null)
            {
                return NotFound($"Film with ID {id} not found.");
            }

            if(updatedFilm == null || string.IsNullOrEmpty(updatedFilm.Title))
            {
                return BadRequest("Invalid film data.");
            }

            film.Title = updatedFilm.Title;
            film.Genre = updatedFilm.Genre;
            film.ReleaseYear = updatedFilm.ReleaseYear;
            film.AvailableCopies = updatedFilm.AvailableCopies;

            await _context.SaveChangesAsync();

            return Ok(film);
        }

        [Authorize(Roles = "Admin")] // Endast administratörer får använda denna metod
        [HttpPatch("{id}")]
        public async Task<ActionResult<FilmDto>> UpdateFilmCopies(int id, [FromBody] UpdateFilmCopiesDto updateDto)
        {
            if (updateDto == null)
            {
                return BadRequest("Felaktig data skickad.");
            }

            var film = await _context.Films.Include(f => f.FilmCopies).FirstOrDefaultAsync(f => f.Id == id);

            if (film == null)
            {
                return NotFound("Filmen kunde inte hittas.");
            }

            // Uppdatera antalet tillgängliga kopior
            int availableCopiesCount = updateDto.AvailableCopies;

            // Om det finns fler kopior än tidigare, skapa nya
            if (film.FilmCopies.Count < availableCopiesCount)
            {
                int copiesToAdd = availableCopiesCount - film.FilmCopies.Count;

                for (int i = 0; i < copiesToAdd; i++)
                {
                    film.FilmCopies.Add(new FilmCopy { IsAvailable = true });
                }
            }
            // Om det finns fler kopior än antalet tillgängliga kopior, sätt IsAvailable = false för överskottet
            else if (film.FilmCopies.Count > availableCopiesCount)
            {
                var copiesToRemove = film.FilmCopies.Count - availableCopiesCount;
                for (int i = 0; i < copiesToRemove; i++)
                {
                    var copy = film.FilmCopies.Last();
                    film.FilmCopies.Remove(copy);
                }
            }

            // Uppdatera `IsAvailable` för varje kopia
            foreach (var copy in film.FilmCopies)
            {
                copy.IsAvailable = true;
            }

            // Uppdatera det tillgängliga antalet kopior i Film-entiteten
            film.AvailableCopies = availableCopiesCount;

            // Spara ändringarna i databasen
            await _context.SaveChangesAsync();

            // Returnera den uppdaterade filmen som DTO
            return Ok(FilmMapper.ToDto(film));
        }



    }
}