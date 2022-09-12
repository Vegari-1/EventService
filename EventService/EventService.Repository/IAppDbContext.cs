using EventService.Model;
using Microsoft.EntityFrameworkCore;

namespace EventService.Repository
{
    public interface IAppDbContext
    {
        DbSet<Event> Events { get; set; }
    }
}
