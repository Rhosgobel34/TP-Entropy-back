using Microsoft.EntityFrameworkCore;
using TP_Entropy_back.Model;
using TP_Entropy_back.DTOs;

namespace TP_Entropy_back.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username.ToLower());
        }

        public async Task<User> CreateAsync(RegisterRequest user)
        {
            user.Username = user.Username.ToLower();

            User newUser = new User
            {
                Username = user.Username,
                Password = user.Password,
                Email = user.Email
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return newUser;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            User? user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
