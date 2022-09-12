using EventService.Model;
using Microsoft.EntityFrameworkCore;

namespace EventService.Repository
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt) { }

        public DbSet<Event> Events { get; set; }
    }
}
