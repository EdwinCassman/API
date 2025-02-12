using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using API.Models.Users;
using API.Models.FilmStudio;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly FilmStudioDbContext _context;
        private readonly IConfiguration _configuration;

        public UserController(FilmStudioDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")] // Definiera en POST-metod på /api/users/register
        public async Task<IActionResult> RegisterUser([FromBody] UserRegister userRegister)
        {
            if (userRegister == null)
            {
                return BadRequest("User data is invalid.");
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(userRegister.Password);
            // Här kan du lägga till din logik för att registrera användare (t.ex. hashning av lösenord etc.)
            var newUser = new User
            {
                Username = userRegister.Username,
                PasswordHash = passwordHash,
                Role = userRegister.IsAdmin ? "Admin" : "User"
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                UserId = newUser.UserId,
                Username = newUser.Username,
                Role = newUser.Role
            });
        }

        
        // POST: api/users/authenticate
        [HttpPost("authenticate")]
        public async Task<ActionResult<object>> AuthenticateUser([FromBody] UserAuthenticate userAuth)
        {
            if (userAuth == null || string.IsNullOrEmpty(userAuth.Username) || string.IsNullOrEmpty(userAuth.Password))
            {
                return BadRequest("Login information is incomplete.");
            }

            // Kontrollera om användaren är en admin
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == userAuth.Username);

            if (user != null && BCrypt.Net.BCrypt.Verify(userAuth.Password, user.PasswordHash))
            {
                // Kolla om användaren är en admin
                var role = user.Role.ToLower() == "admin" ? "Admin" : "User"; // Om användaren har rollen "admin", sätt rollen som "Admin"

                var token = GenerateJwtToken(user.Username, role);

                return Ok(new
                {
                    user.UserId,
                    user.Username,
                    Role = role,  // Returnera rätt roll
                    Token = token // Returnera JWT-token
                });
            }

            // Kontrollera om användaren är en filmstudio
            var filmStudio = await _context.FilmStudios.FirstOrDefaultAsync(fs => fs.Name == userAuth.Username);
            if (filmStudio != null)
            {
                var token = GenerateJwtToken(filmStudio.Name, "FilmStudio"); // Skapa JWT-token för filmstudio

                return Ok(new
                {
                    FilmStudioId = filmStudio.Id,
                    Username = filmStudio.Name,
                    Role = "FilmStudio",  // Returnera rätt roll för filmstudio
                    FilmStudio = new
                    {
                        filmStudio.Id,
                        filmStudio.Name,
                        filmStudio.Email // Lägg till andra egenskaper om det behövs
                    },
                    Token = token // Returnera JWT-token
                });
            }

            return Unauthorized("Invalid username or password.");
        }

        // Skapar JWT-token för autentisering
        private string GenerateJwtToken(string username, string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddHours(2), // Token gäller i 2 timmar
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
