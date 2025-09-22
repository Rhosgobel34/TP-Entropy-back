using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TP_Entropy_back.Model;
using TP_Entropy_back.DTOs;
using TP_Entropy_back.Repositories;
using TP_Entropy_back.Services;



namespace TP_Entropy_back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger _logger;

        public UserController(IUserRepository userRepository, ILogger<UserController> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        [Authorize]
        [HttpGet("usernames")]
        public async Task<ActionResult<IEnumerable<string>>> GetUsernames()
        {
            IEnumerable<User> usernames = await _userRepository.GetAllAsync();
            return Ok(usernames.Select(u => u.Username));
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<User>> GetUserByUsername(string username)
        {
            User? user = await _userRepository.GetByUsernameAsync(username);
            
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody] RegisterRequest newUser)
        {
            if (newUser == null || string.IsNullOrEmpty(newUser.Username) || string.IsNullOrEmpty(newUser.Password) || string.IsNullOrEmpty(newUser.Email))
            {
                return BadRequest("Invalid user data.");
            }

            var strength = Services.PasswordEstimator.Evaluate(newUser.Password);
            if (strength.Score < 3) {
                return BadRequest("Password is too weak.");
            }

            newUser.Password = BCrypt.Net.BCrypt.HashPassword(newUser.Password);

            User createdUser = await _userRepository.CreateAsync(newUser);
            return CreatedAtAction(nameof(GetUserByUsername), new { username = createdUser.Username }, createdUser);
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var deleted = await _userRepository.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}