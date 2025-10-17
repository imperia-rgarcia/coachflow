using Microsoft.EntityFrameworkCore;

using MyApp.Models;

namespace MyApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Routine> Routines { get; set; } = null!;
    }
}
