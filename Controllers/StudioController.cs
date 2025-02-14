using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using API.Models.FilmStudio;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/filmstudio")]
    [ApiController]
    public class FilmStudioController : ControllerBase
    {
        private readonly FilmStudioDbContext _context;
        private readonly IConfiguration _configuration;

        public FilmStudioController(FilmStudioDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterFilmStudio([FromBody] RegisterFilmStudio filmStudioRegister)
        {
            if (filmStudioRegister == null)
            {
                return BadRequest("Film studio data is invalid.");
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(filmStudioRegister.Password);

            var newFilmStudio = new FilmStudio
            {
                Name = filmStudioRegister.Name,
                PasswordHash = passwordHash,
                Email = filmStudioRegister.Email,
                City = filmStudioRegister.City
            };

            _context.FilmStudios.Add(newFilmStudio);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                FilmStudioId = newFilmStudio.Id,
                Name = newFilmStudio.Name,
                Email = newFilmStudio.Email,
                City = newFilmStudio.City
            });
        }

        [HttpPost("authenticate")]
        public async Task<ActionResult<object>> AuthenticateFilmStudio([FromBody] FilmStudioAuthenticate filmStudioAuth)
        {
            if (filmStudioAuth == null || string.IsNullOrEmpty(filmStudioAuth.Name) || string.IsNullOrEmpty(filmStudioAuth.Password))
            {
                return BadRequest("Login information is incomplete.");
            }

            var filmStudio = await _context.FilmStudios.FirstOrDefaultAsync(fs => fs.Name == filmStudioAuth.Name);

            if (filmStudio != null && BCrypt.Net.BCrypt.Verify(filmStudioAuth.Password, filmStudio.PasswordHash))
            {
                var token = GenerateJwtToken(filmStudio.Name, "FilmStudio", filmStudio.Id);

                return Ok(new
                {
                    FilmStudioId = filmStudio.Id,
                    Username = filmStudio.Name,
                    Role = "FilmStudio",
                    FilmStudio = new
                    {
                        filmStudio.Id,
                        filmStudio.Name,
                        filmStudio.Email
                    },
                    Token = token
                });
            }

            return Unauthorized("Invalid username or password.");
        }

        private string GenerateJwtToken(string name, string role, int userId)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}