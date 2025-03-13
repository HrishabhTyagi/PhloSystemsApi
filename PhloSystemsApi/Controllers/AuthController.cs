using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PhloSystemsApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace PhloSystemsApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Authenticates the user and generates a JWT token if the credentials are valid.
        /// </summary>
        /// <param name="login">The login model containing the username and password.</param>
        /// <returns>Returns an Ok result with the JWT token if successful, otherwise returns Unauthorized.</returns>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            // Hardcoded credentials
            var users = new Dictionary<string, string>
            {
                { "imran", "password1" },
                { "rishabh", "password2" }
            };

            if (!users.TryGetValue(login.Username, out var password) || password != login.Password)
            {
                return Unauthorized("Invalid credentials");
            }

            // Generate JWT token
            var token = GenerateJwtToken(login.Username);
            return Ok(new { Token = token });
        }

        private string GenerateJwtToken(string username)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Issuer"],
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

