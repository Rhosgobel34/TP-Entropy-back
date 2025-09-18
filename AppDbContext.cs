using Microsoft.EntityFrameworkCore;
using TP_Entropy_back.Model;

namespace TP_Entropy_back
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        
        public DbSet<User> Users { get; set; }
    }
}
