using EventService.Model;
using EventService.Repository.Interface;
using EventService.Service.Interface;

namespace EventService.Service
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;

        public EventService(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<IEnumerable<Event>> GetAllSortByTimestamp()
        {
            return await _eventRepository.GetAllSortByTimestamp();
        }
    }
}