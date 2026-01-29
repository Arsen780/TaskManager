using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;


namespace TaskManager.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> option) : base(option)
        {
        }

        public DbSet<Task> Tasks => Set<Task>();
    }
}
