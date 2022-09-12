using BusService;
using BusService.Routing;
using EventService.Service.Interface;
using Polly;

namespace EventService.Messaging
{
    public class EventMessageBusService : MessageBusHostedService
    {
        public EventMessageBusService(IMessageBusService serviceBus, IServiceScopeFactory serviceScopeFactory) : base(serviceBus, serviceScopeFactory)
        {
        }

        protected override void ConfigureSubscribers()
        {
            var policy = BuildPolicy();
            // Novi topic -> Topics.Event
            Subscribers.Add(new MessageBusSubscriber(policy, SubjectBuilder.Build(Topics.Profile), typeof(IEventSyncService)));
        }

        private Policy BuildPolicy()
        {
            return Policy
                    .Handle<Exception>()
                    .WaitAndRetry(5, _ => TimeSpan.FromSeconds(5), (exception, _, _, _) =>
                    { });
        }
    }
}
