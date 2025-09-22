using TP_Entropy_back.Model;
using TP_Entropy_back.DTOs;

namespace TP_Entropy_back.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByUsernameAsync(string username);
        Task<User> CreateAsync(RegisterRequest user);
        Task<bool> DeleteAsync(int id);
    }
}
