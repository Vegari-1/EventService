using EventService.Model;
using EventService.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace EventService.Repository
{
    public class EventRepository : Repository<Event>, IEventRepository
    {
        public EventRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Event>> GetAllSortByTimestamp()
        {
            return await _context.Events.OrderByDescending(x => x.Timestamp).ToListAsync();
        }

    }
}
