using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TP_Entropy_back.Model;

namespace TP_Entropy_back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger _logger;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("usernames")]
        public async Task<ActionResult<IEnumerable<string>>> GetUsernames()
        {
            var usernames = await _context.Users
                .Select(u => u.Username)
                .ToListAsync();

            return Ok(usernames);
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<User>> GetUserByUsername(string username)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody] User newUser)
        {
            if (newUser == null || string.IsNullOrEmpty(newUser.Username) || string.IsNullOrEmpty(newUser.Password) || string.IsNullOrEmpty(newUser.Email))
            {
                return BadRequest("Invalid user data.");
            }
            newUser.CreatedAt = DateTime.UtcNow;
            newUser.LastUpdated = DateTime.UtcNow;
            newUser.Username = newUser.Username.ToLower();
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUserByUsername), new { username = newUser.Username }, newUser);
        }

        [HttpDelete("{username}")]
        public async Task<IActionResult> DeleteUser(string username)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username.ToLower());
            if (user == null)
            {
                return NotFound();
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}