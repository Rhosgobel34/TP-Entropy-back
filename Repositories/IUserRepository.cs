using TP_Entropy_back.Model;

namespace TP_Entropy_back.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByUsernameAsync(string username);
        Task<User> CreateAsync(User user);
        Task<bool> DeleteAsync(int id);
    }
}
