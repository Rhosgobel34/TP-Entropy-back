using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TP_Entropy_back.DTOs;
using TP_Entropy_back.Model;
using TP_Entropy_back.Repositories;

namespace TP_Entropy_back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger _logger;
        private readonly IConfiguration _config;
        
        public AuthController(IUserRepository userRepository, ILogger<AuthController> logger, IConfiguration config)
        {
            _userRepository = userRepository;
            _logger = logger;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            User? user = await _userRepository.GetByUsernameAsync(loginRequest.Username);
            if (user == null)
                return Unauthorized("Invalid username.");

            if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.Password))
                return Unauthorized("Invalid password.");
            
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.UTF8.GetBytes(_config["JWT:Key"]);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim("id", user.Id.ToString()),
                    new Claim("username", user.Username)
                ]),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            string jwtToken = tokenHandler.WriteToken(token);

            return Ok(new { token = jwtToken });
        }

        [HttpPost("verify_token")]
        public IActionResult VerifyToken([FromBody] TokenRequest request)
        {
            if (string.IsNullOrEmpty(request.Token))
                return BadRequest(new { verified = false, error = "Token is required." });

            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                byte[] key = Encoding.UTF8.GetBytes(_config["JWT:Key"]);

                tokenHandler.ValidateToken(request.Token, new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out SecurityToken validatedToken);

                return Ok(new { verified = true });
            }
            catch
            {
                return Unauthorized(new { verified = false });
            }
        }
    }
}
