using EventService.Model;

namespace EventService.Service.Interface
{
    public interface IEventService
    {
        Task<IEnumerable<Event>> GetAllSortByTimestamp();
    }
}