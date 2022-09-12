using BusService;
using BusService.Contracts;
using EventService.Model;
using EventService.Repository.Interface;
using EventService.Service.Interface;
using Microsoft.Extensions.Logging;

namespace EventService.Service
{
    public class EventSyncService : ConsumerBase<Event, ProfileContract>, IEventSyncService
    {
        private readonly IMessageBusService _messageBusService;
        private readonly IEventRepository _eventRepository;

        public EventSyncService(IMessageBusService messageBusService, IEventRepository eventRepository,
            ILogger<EventSyncService> logger) : base(logger)
        {
            _messageBusService = messageBusService;
            _eventRepository = eventRepository;
        }

        public override Task PublishAsync(Event entity, string action)
        {
            throw new NotImplementedException();
        }

        public override Task SynchronizeAsync(ProfileContract entity, string action)
        {
            // promena contracta -> treba nam event
            if (action == Events.Created)
            {
                Event entityEvent = new Event
                {
                    Id = Guid.NewGuid(),
                    Timestamp = DateTime.Now,
                    Source = "AuthService",
                    RequestType = "POST",
                    Message = "poruka",
                    StatusCode = 201,
                    StatusCodeText = "Created"
                };
                return _eventRepository.Save(entityEvent);
            }
            return Task.CompletedTask;
        }
    }
}
