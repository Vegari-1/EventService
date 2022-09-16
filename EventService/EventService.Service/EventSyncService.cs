using BusService;
using BusService.Contracts;
using EventService.Model;
using EventService.Repository.Interface;
using EventService.Service.Interface;
using Microsoft.Extensions.Logging;

namespace EventService.Service
{
    public class EventSyncService : ConsumerBase<Event, EventContract>, IEventSyncService
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

        public override Task SynchronizeAsync(EventContract entity, string action)
        {
            if (action == Events.Created)
            {
                Event entityEvent = new Event
                {
                    Timestamp = entity.Timestamp,
                    Source = entity.Source,
                    RequestType = entity.RequestType,
                    Message = entity.Message,
                    StatusCode = entity.StatusCode,
                    StatusCodeText = entity.StatusCodeText
                };
                _eventRepository.Save(entityEvent);
            }
            return Task.CompletedTask;
        }
    }
}
