using BusService;
using BusService.Contracts;
using EventService.Model;

namespace EventService.Service.Interface
{
    public interface IEventSyncService: ISyncService<Event, EventContract>
    {
    }
}
