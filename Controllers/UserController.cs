using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using API.Models.Users;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.Models.FilmStudio;

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

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegister userRegister)
        {
            if (userRegister == null)
            {
                return BadRequest("User data is invalid.");
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(userRegister.Password);

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

        [HttpPost("authenticate")]
        public async Task<ActionResult<object>> AuthenticateUser([FromBody] UserAuthenticate userAuth)
        {
            if (userAuth == null || string.IsNullOrEmpty(userAuth.Username) || string.IsNullOrEmpty(userAuth.Password))
            {
                return BadRequest("Login information is incomplete.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == userAuth.Username);

            if (user != null && BCrypt.Net.BCrypt.Verify(userAuth.Password, user.PasswordHash))
            {
                var role = user.Role.ToLower() == "admin" ? "Admin" : "User";
                var token = GenerateJwtToken(user.Username, role, user.UserId);

                return Ok(new
                {
                    user.UserId,
                    user.Username,
                    Role = role,
                    Token = token
                });
            }

            return Unauthorized("Invalid username or password.");
        }

        private string GenerateJwtToken(string username, string role, int userId)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
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