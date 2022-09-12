using EventService.Model;
using EventService.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using OpenTracing;
using Prometheus;

namespace EventService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {

        private readonly IEventService _eventService;
        private readonly ITracer _tracer;

        Counter counter = Metrics.CreateCounter("event_service_counter", "event counter");

        public EventController(IEventService eventService, ITracer tracer)
        {
            _eventService = eventService;
            _tracer = tracer;
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var actionName = ControllerContext.ActionDescriptor.DisplayName;
            using var scope = _tracer.BuildSpan(actionName).StartActive(true);
            scope.Span.Log("get events");
            counter.Inc();

            IEnumerable<Event> events = await _eventService.GetAllSortByTimestamp();

            return Ok(events);
        }

    }
}
